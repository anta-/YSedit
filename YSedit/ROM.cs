using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace YSedit
{
    /// <summary>
    /// ROMファイルを持って、構造体やデータを読む
    /// </summary>
    class ROM : IDisposable
    {
        FileStream romFile;
        public readonly ROMInterface romIF;
        Map map_;
        public MainView mainView;
        YSeditInfo yseditInfo;

        public Map map { get {
            Debug.Assert(map_ != null);
            return map_;
        } }
        /// <summary>
        /// 今のマップで保存されていない変更があるかどうか
        /// </summary>
        bool changed;
        public delegate void ChangedChanged(bool changed);
        /// <summary>
        /// 保存されていない変更の状態の変化を通知する
        /// </summary>
        public event ChangedChanged changedChanged = null;

        /// <summary>
        /// このツールで用いる情報を保存する
        /// </summary>
        class YSeditInfo
        {
            public ulong magic;
            public uint reserved;
            /// <summary>
            /// 追加データを保存するアドレス
            /// </summary>
            public uint exSpaceAddr;
            /// <summary>
            /// 追加データ領域用の空き領域サイズ(このYSeditInfoのサイズは含めない)
            /// </summary>
            public uint exSpaceSize;
            public uint exDataHead;
            //後はreserved

            public static readonly uint size = 0x20;
            public static readonly ulong Magic = 0x5953656469743030;
        }

        //追加データはヘッダ付の可変長データで、
        //アドレス順にソートされたDouble-linked list。
        //追加する時はO(データ数)でexSpace中のどこか適当な場所に追加する
        //ヘッダ:
        // 0   RomAddr prev。最初の場合は0
        // 4   RomAddr next。最後の場合はexSpaceAddr + exSpaceSize
        // 8   uint    このデータのサイズ(このヘッダー部分は含めない)

        /// <summary>
        /// pathのファイルを開く
        /// </summary>
        /// <exception cref="System.IO.IOException">ファイルが開けなかった時</exception>
        /// <exception cref="UnauthorizedAccessException">ファイルが開けなかった時</exception>
        /// <exception cref="ROM.UnknownROMException">ROMのバージョンが不明・ROMファイルでない場合</exception>
        /// <exception cref="DataException">ROMファイル中におかしなデータがあった場合</exception>
        public ROM(string path)
        {
            bool proceeded = false;
            try
            {
                romFile = File.Open(
                    path,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.Read);

                romIF = new ROMInterface(checkValidROM());

                proceeded = true;
            }
            finally
            {
                if (!proceeded)
                    Dispose();
            }
        }

        ~ROM()
        {
            if (romFile != null)
                Dispose();
        }

        public void Dispose()
        {
            if(map_ != null)
                closeMap();

            if (romFile != null)
            {
                romFile.Close();
                romFile = null;
            }
        }

        public class UnknownROMException : ApplicationException
        {
        }

        /// <summary>
        /// ROMをチェックしてバージョンを求める
        /// </summary>
        ROMInterface.Ver checkValidROM()
        {
            checkBytesROM(0, new byte[]{
                0x80, 0x37, 0x12, 0x40, 0x00, 0x00, 0x00, 0x0F, 0x80, 0x20, 0x04, 0x00, 0x00, 0x00, 0x14, 0x48
            });
            //YOSHI STORY
            checkBytesROM(0x20, new byte[]{
                0x59, 0x4f, 0x53, 0x48, 0x49, 0x20, 0x53, 0x54, 0x4f, 0x52, 0x59
            });
            byte countryCode = readData(new RomAddress(0x3e), 1).bytes[0];
            if (countryCode == 0x4a)
                return ROMInterface.Ver.Japanese;
            else
                throw new UnknownROMException();
        }

        void checkBytesROM(uint offset, byte[] bytes)
        {
            if (romFile.Length < (long)offset + bytes.Length)
                throw new UnknownROMException();

            byte[] buffer = new byte[bytes.Length];
            romFile.Seek(offset, SeekOrigin.Begin);
            romFile.Read(buffer, 0, (int)bytes.Length);
            if (!buffer.SequenceEqual(bytes))
                throw new UnknownROMException();
        }

        public class UserCancel : ApplicationException
        {
            public UserCancel() : base() { }
        }
        void closeMap()
        {
            if (changed)
            {
                DialogResult res = MessageBox.Show("Save map to ROM?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if(res == DialogResult.Cancel)
                    throw new UserCancel();
                else if (res == DialogResult.Yes)
                    saveToROM();
                else if (res == DialogResult.No)
                {
                    if (changed)
                    {
                        changed = false;
                        if (changedChanged != null)
                            changedChanged(false);
                    }
                }
            }
            map.Dispose();
        }

        /// <summary>
        /// ROM容量が足りない時
        /// </summary>
        class NeedExpansionException : ApplicationException
        {
            public NeedExpansionException()
                : base() { }
        }

        /// <summary>
        /// 新しい領域をROM上に確保し、dataを書き込む
        /// </summary>
        /// <param name="data">確保してデータを書き込んだ場所</param>
        /// <exception cref="NeedExpansionException">ROM容量が足りない時</exception>
        RomAddress newExData(Data data)
        {
            var size = 0xc + data.Length;
            var last = yseditInfo.exSpaceAddr + yseditInfo.exSpaceSize;

            RomAddress a = yseditInfo.exSpaceAddr;
            RomAddress next = yseditInfo.exDataHead;
            RomAddress prev = new RomAddress(0);

            while (a.x != last)
            {
                var spaceSize = next.x - a.x;
                if (spaceSize >= size)
                    break;
                Data header = readData(next, 0xc);
                prev = next;
                a = next + 0xc + header.getWord(8);
                next = header.getWord(4);
            }

            if (a.x == last)
                throw new NeedExpansionException();

            //ここでaが確保する領域、nextがその後のデータヘッダーを指す

            Data dataHeader = new Data(new byte[0xc]);
            //このprevと前のnext
            dataHeader.setWord(0, prev.x);
            if (prev.x == 0)
                yseditInfo.exDataHead = a.x;
            else
                writeWord(prev + 4, a.x);
            //このnextと次のprev
            dataHeader.setWord(4, next.x);
            if (next.x != last)
                writeWord(next + 0, a.x);
            dataHeader.setWord(8, (uint)data.Length);
            writeData(a, dataHeader);
            writeData(a + 0xc, data);
            return a + 0xc;
        }

        bool isExData(RomAddress addr)
        {
            return yseditInfo.exSpaceAddr <= addr.x &&
                addr.x < yseditInfo.exSpaceAddr + yseditInfo.exSpaceSize;
        }

        /// <summary>
        /// 新しくExDataを確保し、pointerAddrの場所に書き込む。
        /// 既にpointerAddrの場所にExDataの場所が書き込まれている場合はそれを削除する。
        /// つまりこれ1つから参照されているもの限定
        /// </summary>
        void newDataPointerWrite(RomAddress pointerAddr, Data data)
        {
            var oldAddr = dataIDToRomAddress(readData(pointerAddr, 4).getDataID(0));
            if (isExData(oldAddr))
                deleteExData(oldAddr);
            var addr = newExData(data);
            writeWord(pointerAddr, romAddressToDataID(addr).x);
        }

        void deleteExData(RomAddress addr)
        {
            var headerAddr = new RomAddress(addr.x - 0xc);
            var last = yseditInfo.exSpaceAddr + yseditInfo.exSpaceSize;

            Data header = readData(headerAddr, 0xc);
            var prev = header.getWord(0);
            var next = header.getWord(4);
            var size = header.getWord(8);
            if (prev == 0)
                yseditInfo.exDataHead = next;
            else
                writeWord(new RomAddress(prev) + 4, next);
            if (next != last)
                writeWord(new RomAddress(next) + 0, prev);

            //塗りつぶしておく
            writeData(headerAddr, new Data(
                Enumerable.Repeat<byte>(0xff, 0xc + (int)size).ToArray()));
        }



        /// <summary>
        /// ROMファイルに変更を書き込む
        /// </summary>
        /// <exception cref="DataException">ROMファイル中におかしなデータがあった場合</exception>
        /// <exception cref="NeedExpansionException">ROMの空き領域が足りない場合</exception>
        public void saveToROM()
        {
            if (map_ == null)
                return;

            yseditInfo = readYSeditInfo();
            bool modInstall = false;
            if (yseditInfo.magic != YSeditInfo.Magic) {
                installMod();
                yseditInfo = readYSeditInfo();
                modInstall = true;
            }

            try
            {
                saveMapInfos();
            }
            catch (Map.CannotSaveException e)
            {
                MessageBox.Show("Not correctly saved: " + e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveYSeditInfo(yseditInfo);
            yseditInfo = null;

            romFile.Flush();
            Program.showInfo("Map " + map.currentMapNumber.ToString("x2") + " saved to ROM" +
                (modInstall ? ", Mod installed" : ""));
            
            if (changed)
            {
                changed = false;
                if (changedChanged != null)
                    changedChanged(false);
            }
        }

        void installMod()
        {
            //チェックサムを回避する
            writeData(romIF.headerModAddr1, new Data(new byte[] { romIF.headerModByte1 }));
            writeData(romIF.headerModAddr2, new Data(new byte[] { romIF.headerModByte2 }));

            YSeditInfo info = new YSeditInfo();
            info.magic = YSeditInfo.Magic;
            info.reserved = 0;
            //exSpaceはYSeditInfoの直ぐ後で
            info.exSpaceAddr = (romIF.freeSpaceAddr + YSeditInfo.size).x;
            info.exSpaceSize = romIF.freeSpaceSize - YSeditInfo.size;
            info.exDataHead = info.exSpaceAddr + info.exSpaceSize;

            saveYSeditInfo(info);
        }

        void saveYSeditInfo(YSeditInfo info)
        {
            Data data = new Data(new byte[YSeditInfo.size]);
            data.setDWord(0x0, info.magic);
            data.setWord(0x8, info.reserved);
            data.setWord(0xc, info.exSpaceAddr);
            data.setWord(0x10, info.exSpaceSize);
            data.setWord(0x14, info.exDataHead);
            writeData(romIF.freeSpaceAddr, data);
        }

        YSeditInfo readYSeditInfo()
        {
            YSeditInfo info = new YSeditInfo();
            Data data = readData(romIF.freeSpaceAddr, YSeditInfo.size);
            info.magic = data.getDWord(0x0);
            info.reserved = data.getWord(0x8);
            info.exSpaceAddr = data.getWord(0xc);
            info.exSpaceSize = data.getWord(0x10);
            info.exDataHead = data.getWord(0x14);
            return info;
        }

        void saveMapInfos()
        {
            var objPlaces = map.getObjPlaces();
            var numberOfObjects = objPlaces.Length / romIF.objPlaceC_size;
            var ustruct3 = map.getUStruct3();

            var ustruct5 = readData(romIF.ustruct5Table + map.currentMapNumber * romIF.ustruct5_size, romIF.ustruct5_size);
            var iUStruct3 = ustruct5.getDataID(romIF.uttruct5_iUStruct3);
            writeData(iUStruct3, ustruct3);

            var iiPlaceVector = ustruct3.getDataID(romIF.ustruct3_iiPlaceVector);
            var iPlaceVector = readData(iiPlaceVector, 4).getDataID(0);
            var placeVector = readData(iPlaceVector, romIF.placeVector_size);
            Data tmp = new Data(new byte[2]);
            tmp.setHalf(0, (ushort)numberOfObjects);
            var pPlaceVector = dataIDToRomAddress(iPlaceVector);
            writeData(pPlaceVector + romIF.placeVector_numberOfObjPlaces, tmp);
            newDataPointerWrite(pPlaceVector + romIF.placeVector_iObjPlaces, objPlaces);
        }

        /// <summary>
        /// マップを番号で開く
        /// </summary>
        /// <exception cref="ArgumentException">mapNumberが大きすぎた場合</exception>
        /// <exception cref="DataException">mapNumberが無効なマップを指している、またはROMが壊れている場合</exception>
        public void openMapNumber(byte mapNumber)
        {
            if (map_ != null)
                closeMap();
            map_ = new Map(this, mapNumber, mainView);
            changed = false;
        }

        /// <summary>
        /// "れんしゅう"マップを開く
        /// </summary>
        /// <exception cref="DataException">ROMが壊れている場合</exception>
        public void openPracticeMap()
        {
            openMapNumber(romIF.practiceMapNumber);
        }

        /// <summary>
        /// データをROMから単に読みこむ
        /// </summary>
        public Data readData(RomAddress addr, uint size)
        {
            if (romFile.Length < (long)addr.x + size)
                throw new DataException("readStruct: addr + size > romFile.length");

            byte[] buffer = new byte[size];
            romFile.Seek(addr.x, SeekOrigin.Begin);
            romFile.Read(buffer, 0, (int)size);
            return new Data(buffer);
        }

        /// <summary>
        /// データをROMから単に読み込む。DataIDで指定する
        /// </summary>
        public Data readData(DataID dataID, uint size)
        {
            return readData(dataIDToRomAddress(dataID), size);
        }

        /// <summary>
        /// データをROMに単に書き込む
        /// </summary>
        void writeData(RomAddress addr, Data data)
        {
            romFile.Seek(addr.x, SeekOrigin.Begin);
            romFile.Write(data.bytes, 0, data.Length);
        }

        /// <summary>
        /// データをROMに単に書き込む。DataIDで指定する
        /// </summary>
        void writeData(DataID dataID, Data data)
        {
            writeData(dataIDToRomAddress(dataID), data);
        }

        /// <summary>
        /// 4byteのデータをROMに単に書き込む
        /// </summary>
        void writeWord(RomAddress addr, uint val)
        {
            Data data = new Data(new byte[4]);
            data.setWord(0, val);
            writeData(addr, data);
        }

        /// <summary>
        /// 4byteのデータをROMに単に書き込む。DataIDで指定する
        /// </summary>
        void writeWord(DataID dataID, uint val)
        {
            writeWord(dataIDToRomAddress(dataID), val);
        }

        /// <summary>
        /// dataIDをROMアドレスに変換する
        /// </summary>
        public RomAddress dataIDToRomAddress(DataID dataID)
        {
            return romIF.dataIDbases[dataID.seg] + dataID.offset;
        }

        /// <summary>
        /// ROMアドレスをdataIDに変換する
        /// </summary>
        public DataID romAddressToDataID(RomAddress addr)
        {
            if (addr.x >= 0x1000000)
                throw new NotImplementedException("dataIDbasesを拡張する");
            Debug.Assert(romIF.dataIDbases[0x8].x == 0);
            return 0x08000000 | addr.x;
        }

        /// <summary>
        /// 変更したかどうかを設定する
        /// </summary>
        /// <param name="change">変更したかどうか</param>
        public void newChange(bool change)
        {
            if (!changed && change)
            {
                changed = true;
                if (changedChanged != null)
                    changedChanged(true);
            }
        }
    }
}
