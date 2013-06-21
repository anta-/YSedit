using System;
using System.Collections.Specialized;
using System.Linq;

namespace YSedit
{
    static class Util
    {
        /// <summary>
        /// 16進数をパースする
        /// </summary>
        /// <param name="s">パースする文字列</param>
        /// <param name="maxNumber">パース結果の最大値。デファルトは上限無しです</param>
        /// <exception cref="FormatException">文字列が16進数を表していない、またはパース結果が大きすぎた場合です</exception>
        public static uint parseHex(this string s, uint maxNumber = 0xffffffff)
        {
            uint x = uint.Parse(s, System.Globalization.NumberStyles.HexNumber);
            if (x > maxNumber)
                throw new FormatException();
            return x;
        }

        public static uint floatToWord(float f)
        {
            return (uint)BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
        }
        public static float wordToFloat(uint x)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(x), 0);
        }

        /// <summary>
        /// floatを16進数でフォーマットする
        /// </summary>
        /// <param name="x">フォーマットする値</param>
        public static string floatToHexString(float x)
        {
            const int expBits = 8, fracBits = 23, bias = 127;
            
            var w = floatToWord(x);

            bool sig = (w >> 31) != 0;
            int exp = (int)(w << 1 >> (32 - expBits));
            uint frac = w << (1 + expBits) >> (32 - fracBits);
            uint frac1;

            string sigs = sig ? "-" : "";

            if (exp == 0)
            {
                if (frac == 0)
                    return sigs + "0";
                else
                {
                    //非正規化数
                    var log2 = hightestBit(frac);
                    frac1 = frac << (fracBits - log2);
                    exp = 1 - (fracBits - log2);
                }
            }
            else if (exp == 255)
            {
                if (frac == 0)
                    return sigs + "Inf";
                else
                    return sigs + "NaN" + frac.ToString("x6");  //適当に
            }else frac1 = frac | (1 << fracBits);

            exp -= bias;

            //2^exp * frac1 / 2^fracBits
            // = frac1 / 2^(fracBits-exp)
            string s = sigs;
            s += (frac1 >> Math.Min(Math.Max(fracBits - exp, 0), 31) << -Math.Min(fracBits - exp, 0)%4).ToString("x");
            s += new String('0', -Math.Min(fracBits - exp, 0) / 4); //4は2^4=16進法だから
            if (fracBits - exp < 32)
                frac1 &= (1U << Math.Max(0, fracBits - exp)) - 1;
            //小数点以下
            if (frac1 != 0)
            {
                var log2 = hightestBit(frac1);
                frac1 <<= fracBits - log2;
                exp -= fracBits - log2;

                s += ".";
                s += new String('0', Math.Max(0, -exp - 1) / 4);
                for (var i = fracBits - 3 + Math.Max(0, -exp - 1) % 4; i + 4 >= 0 && (frac1 & (1U << i + 4) - 1) != 0; i -= 4)
                {
                    s += "0123456789abcdef"[(int)((i >= 0 ? frac1 >> i : frac1 << -i) & 0xf)];
                }
            }

            return s;
        }

        static int readHexDigit(char c)
        {
            int t = "0123456789abcdefABCDEF".IndexOf(c);
            if (t == -1) throw new FormatException();
            if (t >= 0x10) t -= 6;
            return t;
        }

        public static float floatFromHexString(string s)
        {
            const int expBits = 8, fracBits = 23;

            s = s.Trim();

            if (s.Length == 0)
                throw new FormatException();

            bool sign = s[0] == '-';
            if (sign) s = s.Substring(1);
            uint signw = sign ? (1U << expBits + fracBits) : 0;

            if (s == "Inf") return wordToFloat(signw | (255 << fracBits));
            if (s.Length == 9 && s.Substring(0, 3) == "NaN")
                return wordToFloat(signw | (255 << fracBits) |
                    s.Substring(3, 6).parseHex());

            //とりあえず適当実装…
            float f = 0;
            {
                int i;
                for (i = 0; i < s.Length && s[i] != '.'; i++)
                {
                    f = f * 16 + readHexDigit(s[i]);
                }
                float x = 1 / 16f;
                for (i++; i < s.Length; i++)
                {
                    f += x * readHexDigit(s[i]);
                    x /= 16f;
                }
            }

            return f;
        }

        /// <summary>
        /// 一番MSBに近いビットの位置(LSBから)を返す
        /// </summary>
        public static int hightestBit(uint x)
        {   //とりあえず普通実装
            int res = -1;
            while (x != 0)
            {
                res++;
                x >>= 1;
            }
            return res;
        }
    }
}