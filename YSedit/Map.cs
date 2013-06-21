using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSedit
{
    /// <summary>
    /// あるマップを管理する。
    /// MapはデータをDataの形で持つ。
    /// 高級な構造(ビューア・エディタ全般が管理)はData(Mapが管理)(から読み出される/に保存される)、
    /// Data(Mapが管理)はファイル(ROMが管理)(から読み出される/に保存される)。
    /// </summary>
    class Map : IDisposable
    {
        ROM rom;
        readonly ROMInterface romIF;
        readonly MainView mainView;

        public readonly byte currentMapNumber;
        Data ustruct5;
        Data ustruct3;
        Data placeVector;
        public Data getObjPlaces()
        {
            return mainView.getObjPlaces();
        }

        /// <summary>
        /// Mapに関してデータを読み、初期化をする
        /// </summary>
        /// <exception cref="ArgumentException">mapNumberが大きすぎた場合</exception>
        /// <exception cref="DataException">mapNumberが無効なマップを指している、またはROMが壊れている場合</exception>
        public Map(ROM rom_, byte mapNumber, MainView mainView)
        {
            rom = rom_;
            romIF = rom.romIF;
            this.mainView = mainView;

            if (romIF.mapNumbers <= mapNumber)
                throw new ArgumentException("map number is too big");

            currentMapNumber = mapNumber;
            ustruct5 = rom.readData(romIF.ustruct5Table + mapNumber * romIF.ustruct5_size, romIF.ustruct5_size);
            var iUStruct3 = ustruct5.getDataID(romIF.uttruct5_iUStruct3);
            ustruct3 = rom.readData(iUStruct3, romIF.ustruct3_size);
            var iiPlaceVector = ustruct3.getDataID(romIF.ustruct3_iiPlaceVector);
            var iPlaceVector = rom.readData(iiPlaceVector, 4).getDataID(0);
            placeVector = rom.readData(iPlaceVector, romIF.placeVector_size);
            var iObjPlaces = placeVector.getDataID(romIF.placeVector_iObjPlaces);
            var objPlaces = rom.readData(iObjPlaces,
                placeVector.getHalf(romIF.placeVector_numberOfObjPlaces) * romIF.objPlaceC_size);

            mainView.size = new System.Drawing.Size(0x2000, 0x200); //TODO: データから読み込む
            mainView.setObjPlaces(objPlaces);   //objPlacesはmainViewが持つ
        }

        public void Dispose()
        {

        }

        public void editObjectList()
        {
            var oldObjPlaces = mainView.getObjPlaces();
            var form = new EditObjectList(romIF, oldObjPlaces);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rom.newChange(!oldObjPlaces.bytes.SequenceEqual(form.objPlaces.bytes));
                mainView.setObjPlaces(form.objPlaces);
            }
        }

        public string getMapInformationStr()
        {
            string s = "";
            s += "Map number: " + currentMapNumber.ToString("x2") + "\n";
            s += "objPlaces: " + rom.dataIDToRomAddress(placeVector.getDataID(romIF.placeVector_iObjPlaces)).ToString() + "\n";
            return s;
        }
    }
}
