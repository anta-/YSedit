using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace YSedit
{
    /// <summary>
    /// ROMのバージョンごとにstaticアドレスや構造体の配置が違うので、それの定数を提供する
    /// </summary>
    class ROMInterface
    {
        /// <summary>
        /// ROMのバージョンを表す
        /// </summary>
        public enum Ver {
            Japanese
        }
        readonly Ver ver;

        public readonly RomAddress[] dataIDbases;

        public readonly byte mapNumbers;
        public readonly byte practiceMapNumber;

        public readonly uint ustruct3_size;
        public readonly uint ustruct3_iiPlaceVector;
        public readonly uint placeVector_size;
        public readonly uint placeVector_numberOfObjPlaces;
        public readonly uint placeVector_iObjPlaces;
        public readonly uint objPlaceC_size;
        public readonly uint objPlace_kind;
        public readonly uint objPlace_info;
        public readonly uint objPlace_xpos;
        public readonly uint objPlace_ypos;
        public readonly uint numberOfObjPlacesMax;

        public readonly RomAddress ustruct5Table;

        public readonly uint ustruct5_size;
        public readonly uint uttruct5_iUStruct3;

        public readonly RomAddress headerModAddr1;
        public readonly RomAddress headerModAddr2;
        public readonly byte headerModByte1;
        public readonly byte headerModByte2;

        public readonly RomAddress freeSpaceAddr;
        public readonly uint freeSpaceSize;

        public ROMInterface(Ver ver_)
        {
            ver = ver_;

            dataIDbases = new RomAddress[] { 0x1060, 0, 0, 0x528430, 0xb16170, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            mapNumbers = 0xb0;
            practiceMapNumber = 0x19;

            ustruct3_size = 0x118;
            ustruct3_iiPlaceVector = 0x5c;
            placeVector_size = 8;
            placeVector_numberOfObjPlaces = 0;
            placeVector_iObjPlaces = 4;
            objPlaceC_size = 0xc;
            objPlace_kind = 0;
            objPlace_info = 2;
            objPlace_xpos = 4;
            objPlace_ypos = 8;
            numberOfObjPlacesMax = 150;

            ustruct5Table = new RamAddress(0x800abf60).toRomAddress();

            ustruct5_size = 8;
            uttruct5_iUStruct3 = 4;

            headerModAddr1 = 0x67f;
            headerModAddr2 = 0x68b;
            headerModByte1 = 0x63;
            headerModByte2 = 0xba;
            freeSpaceAddr = 0xe67fd0;
            freeSpaceSize = 0x1000000 - freeSpaceAddr.x;

            switch (ver)
            {
                case Ver.Japanese:
                    break;
            }
        }

    }
}
