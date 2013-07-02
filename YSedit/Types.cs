using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace YSedit
{
    /// <summary>
    /// ROM上のデータがおかしいと投げられる
    /// </summary>
    class DataException : ApplicationException
    {
        public DataException() {}
        public DataException(string message): base(message) {}
    }

    class Address
    {

        public uint x;
        public Address(uint a)
        {
            x = a;
        }
        public static Address operator +(Address a, uint offset)
        {
            return new Address(a.x + offset);
        }
    
        public override string ToString()
        {
            return x.ToString("x8");
        }
    }

    class RamAddress : Address
    {
        public RomAddress toRomAddress()
        {
            return new RomAddress(x - 0x80000400 + 0x1000);
        }
        public RamAddress(uint a) : base(a)
        {
            if (!(0x80000000 <= a && a <= 0x807fffff))
            {
                throw new DataException("this is not a RDRAM address");
            }
        }
        public static implicit operator RamAddress(uint a)
        {
            return new RamAddress(a);
        }
        public static RamAddress operator +(RamAddress a, uint offset)
        {
            return new RamAddress(a.x + offset);
        }
    }

    class RomAddress : Address
    {
        public RamAddress toRamAddress()
        {
            return new RamAddress(x - 0x1000 + 0x80000400);
        }
        public RomAddress(uint a) : base(a)
        {
        }
        public static implicit operator RomAddress(uint a)
        {
            return new RomAddress(a);
        }
        public static RomAddress operator +(RomAddress a, uint offset)
        {
            return new RomAddress(a.x + offset);
        }
    }

    class DataID : Address
    {
        public DataID(uint a) : base(a) {
            if (a >= 0x10000000)
            {
                throw new DataException("this is not a data ID");
            }
        }
        public static implicit operator DataID(uint a)
        {
            return new DataID(a);
        }
        public uint seg
        {
            get
            {
                return x >> 0x18;
            }
            set
            {
                Debug.Assert(value < 0x10);
                x = (x & 0x00ffffff) | (value << 0x18);
            }
        }
        public uint offset
        {
            get
            {
                return x & 0x00ffffff;
            }
            set
            {
                Debug.Assert(value < 0x1000000);
                x = x & 0xff000000 | value;
            }
        }
    }

    class Data
    {
        public byte[] bytes;
        public int Length
        {
            get { return bytes.Length; }
        }

        public Data(byte[] bytes_)
        {
            bytes = bytes_;
        }

        public byte getByte(uint offset)
        {
            return bytes[offset];
        }
        public ushort getHalf(uint offset)
        {
            return (ushort)(getByte(offset) << 8 | getByte(offset + 1));
        }
        public uint getWord(uint offset)
        {
            return (uint)(getHalf(offset) << 16 | getHalf(offset + 2));
        }
        public ulong getDWord(uint offset)
        {
            return (ulong)((ulong)getWord(offset) << 32 | getWord(offset + 4));
        }
        public void setByte(uint offset, byte val)
        {
            bytes[offset] = val;
        }
        public void setHalf(uint offset, ushort val)
        {
            setByte(offset, (byte)(val >> 8));
            setByte(offset + 1, (byte)(val & 0xff));
        }
        public void setWord(uint offset, uint val)
        {
            setHalf(offset, (ushort)(val >> 16));
            setHalf(offset + 2, (ushort)(val & 0xffff));
        }
        public void setDWord(uint offset, ulong val)
        {
            setWord(offset, (uint)(val >> 32));
            setWord(offset + 4, (uint)(val & 0xffffffff));
        }

        public float getFloat(uint offset) { return Util.wordToFloat(getWord(offset)); }
        public void setFloat(uint offset, float val) { setWord(offset, Util.floatToWord(val)); }

        public Address getAddress(uint offset) { return new Address(getWord(offset)); }
        public void setAddress(uint offset, Address val) { setWord(offset, val.x); }
        public RamAddress getRamAddress(uint offset) { return new RamAddress(getWord(offset)); }
        public void setRamAddress(uint offset, RamAddress val) { setWord(offset, val.x); }
        public RomAddress getRomAddress(uint offset) { return new RomAddress(getWord(offset)); }
        public void setRomAddress(uint offset, RomAddress val) { setWord(offset, val.x); }
        public DataID getDataID(uint offset) { return new DataID(getWord(offset)); }
        public void setDataID(uint offset, DataID val) { setWord(offset, val.x); }

        public Data getData(uint offset, uint size)
        {
            Debug.Assert(offset + size <= Length);
            return new Data(new List<byte>(bytes).GetRange((int)offset, (int)size).ToArray());
        }
        public void setData(uint offset, Data data)
        {
            Debug.Assert(offset + data.Length <= Length);
            data.bytes.CopyTo(bytes, offset);
        }
    }

    
}
