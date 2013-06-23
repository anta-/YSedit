using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

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
        public static string floatToHexString(this float x)
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

        public static float floatFromHexString(this string s)
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

        public static float? tryFloatFromHexString(this string s)
        {
            if (s == null) return null;
            try
            {
                return s.floatFromHexString();
            }
            catch (FormatException)
            {
                return null;
            }
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

    //Vector(System.Windowsネームスペース)にしてやるのめんどくさい
    static class PointVectorExts
    {
        public static Point Add(this Point p, Point q)
        {
            return new Point(p.X + q.X, p.Y + q.Y);
        }
        public static Rectangle AddMove(this Rectangle r, Size x)
        {
            return new Rectangle(r.Location + x, r.Size);
        }
        public static Rectangle AddResize(this Rectangle r, Size x)
        {
            return new Rectangle(r.Location, r.Size + x);
        }
        public static Point Sub(this Point p, Point q)
        {
            return new Point(p.X - q.X, p.Y - q.Y);
        }
        public static Rectangle SubResize(this Rectangle r, Size x)
        {
            return new Rectangle(r.Location, r.Size - x);
        }
        public static Rectangle SubMove(this Rectangle r, Size x)
        {
            return new Rectangle(r.Location - x, r.Size);
        }
        public static Size Mult(this Size x, int y)
        {
            return new Size(x.Width * y, x.Height * y);
        }

        public static Rectangle Expand(this Rectangle x, Size y)
        {
            return new Rectangle(x.Location - y, x.Size + y.Mult(2));
        }

        public static Point Fit(this Point p, Rectangle r)
        {
            if (p.X < r.X) p.X = r.X;
            if (p.Y < r.Y) p.Y = r.Y;
            if (r.Right <= p.X) p.X = r.Right;
            if (r.Bottom <= p.Y) p.Y = r.Bottom;
            return p;
        }

        public static Rectangle TwoPoints(this Point p, Point q)
        {
            return new Rectangle(p, (Size)q.Sub(p)).Normalize();
        }

        public static Point Min2(this Point p, Point q)
        {
            return new Point(Math.Min(p.X, q.X), Math.Min(p.Y, q.Y));
        }

        public static Point Max2(this Point p, Point q)
        {
            return new Point(Math.Max(p.X, q.X), Math.Max(p.Y, q.Y));
        }

        public static Rectangle Normalize(this Rectangle x)
        {
            if (x.Width < 0) {
                x.X = x.Right;
                x.Width *= -1;
            }
            if (x.Height < 0)
            {
                x.Y = x.Bottom;
                x.Height *= -1;
            }
            return x;
        }
    }

    static class WindowsUtil
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        public static IntPtr SendMessage(this Control control, UInt32 Msg, IntPtr wParam, IntPtr lParam)
        {
            return SendMessage(control.Handle, Msg, wParam, lParam);
        }
        public static IntPtr SendMessage(this Control control, UInt32 Msg, uint wParam, uint lParam)
        {
            return SendMessage(control.Handle, Msg, new IntPtr(wParam), new IntPtr(lParam));
        }

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(this Control c)
        {
            c.SendMessage(WM_SETREDRAW, 0, 0);
        }

        public static void ResumeDrawing(this Control c, bool refresh = true)
        {
            c.SendMessage(WM_SETREDRAW, 1, 0);
            if (refresh)
                c.Refresh();
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static int SetWindowLong(this Control control, int nIndex, int dwNewLong)
        {
            return SetWindowLong(control.Handle, nIndex, dwNewLong);
        }
        public static int GetWindowLong(this Control control, int nIndex)
        {
            return GetWindowLong(control.Handle, nIndex);
        }
    }

}
