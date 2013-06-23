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
        /// pathのファイルを開く
        /// </summary>
        /// <exception cref="System.IO.IOException">ファイルが開けなかった時</exception>
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

            romFile.Close();
            romFile = null;
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
        /// ROMファイルに変更を書き込む
        /// </summary>
        /// <exception cref="DataException">ROMファイル中におかしなデータがあった場合</exception>
        public void saveToROM()
        {
            if (map_ == null)
                return;

            saveObjPlaces();

            romFile.Flush();
            
            if (changed)
            {
                changed = false;
                if (changedChanged != null)
                    changedChanged(false);
            }
        }

        void saveObjPlaces()
        {
            var objPlaces = map.getObjPlaces();
            var numberOfObjects = objPlaces.bytes.Length / romIF.objPlaceC_size;

            var ustruct5 = readData(romIF.ustruct5Table + map.currentMapNumber * romIF.ustruct5_size, romIF.ustruct5_size);
            var iUStruct3 = ustruct5.getDataID(romIF.uttruct5_iUStruct3);
            var ustruct3 = readData(iUStruct3, romIF.ustruct3_size);
            var iiPlaceVector = ustruct3.getDataID(romIF.ustruct3_iiPlaceVector);
            var iPlaceVector = readData(iiPlaceVector, 4).getDataID(0);
            var placeVector = readData(iPlaceVector, romIF.placeVector_size);
            var iObjPlaces = placeVector.getDataID(romIF.placeVector_iObjPlaces);
            var oldNumberOfObjects = placeVector.getHalf(romIF.placeVector_numberOfObjPlaces);

            if (numberOfObjects != oldNumberOfObjects)
            {
                throw new NotImplementedException("オブジェクト数が増加した時どこにバッファをとろう…");
            }

            writeData(iObjPlaces, objPlaces);
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
            romFile.Write(data.bytes, 0, data.bytes.Length);
        }

        /// <summary>
        /// データをROMに単に書き込む。DataIDで指定する
        /// </summary>
        void writeData(DataID dataID, Data data)
        {
            writeData(dataIDToRomAddress(dataID), data);
        }

        /// <summary>
        /// dataIDをROMアドレスに変換する
        /// </summary>
        public RomAddress dataIDToRomAddress(DataID dataID)
        {
            return romIF.dataIDbases[dataID.seg] + dataID.offset;
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
