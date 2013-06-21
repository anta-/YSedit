using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YSedit
{
    /// <summary>
    /// メインビューの操作と描画をする。
    /// ObjPlacesなどのDataをやり取りする。
    /// </summary>
    class MainView
    {
        ROMInterface romIF;

        public delegate void ChangeSizeProc(MainView sender);
        public ChangeSizeProc changeSize;
        Size size_;
        public Size size {
            get { return size_; }
            set {
                size_ = value;
                if(changeSize != null) changeSize(this);
            }
        }

        class ObjPlace {
            public ushort kind, info;
            public float x, y;

            public ObjPlace(ushort kind, ushort info, float x, float y)
            {
                this.kind = kind;
                this.info = info;
                this.x = x;
                this.y = y;
            }
        };
        List<ObjPlace> objPlaceList = new List<ObjPlace>();

        public MainView(ROMInterface romIF)
        {
            this.romIF = romIF;
        }

        /// <summary>
        /// rectで指定された範囲を描画して返す
        /// </summary>
        /// <param name="rect">描画する範囲</param>
        /// <returns>描画されたBitmap</returns>
        public Bitmap rendering(Rectangle rect)
        {
            var bmp = new Bitmap(rect.Width, rect.Height);
            
            var g = Graphics.FromImage(bmp);
            var font = new Font("MS Gothic", 8);
            var brush = new SolidBrush(Color.FromArgb(0x80, Color.Red));
            var strBrush = new SolidBrush(Color.FromArgb(0xff, Color.Black));
            var strBackBrush = new SolidBrush(Color.FromArgb(0xff, Color.White));
            foreach(var o in objPlaceList)
            {
                var p = new Point((int)o.x - rect.X, (int)o.y - rect.Y);
                var text = o.kind.ToString("x4");
                var textP = new PointF(p.X, p.Y);
                g.FillRectangle(strBackBrush, new RectangleF(textP, g.MeasureString(text, font)));
                g.DrawString(text, font, strBrush, textP);
                g.FillRectangle(brush, new Rectangle(new Point(p.X - 1, p.Y - 1), new Size(3, 3)));
            }

            return bmp;
        }

        /// <summary>
        /// objPlacesを強制的に設定する
        /// </summary>
        /// <param name="objPlaces">ObjPlace_C[]のData</param>
        public void setObjPlaces(Data objPlaces)
        {
            int number = (int)(objPlaces.bytes.Length / romIF.objPlaceC_size);
            objPlaceList = new List<ObjPlace>(number);

            for (var i = 0; i < number; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                objPlaceList.Add(new ObjPlace(
                    objPlaces.getHalf(o + romIF.objPlace_kind),
                    objPlaces.getHalf(o + romIF.objPlace_info),
                    objPlaces.getFloat(o + romIF.objPlace_xpos),
                    objPlaces.getFloat(o + romIF.objPlace_ypos)));
            }
        }

        /// <summary>
        /// 現在の状態からobjPlacesを取得する
        /// </summary>
        /// <returns>ObjPlace_C[]のData</returns>
        public Data getObjPlaces()
        {
            Data objPlaces = new Data(new byte[objPlaceList.Count * romIF.objPlaceC_size]);

            for (var i = 0; i < objPlaceList.Count; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                var objPlace = objPlaceList[i];
                objPlaces.setHalf(o + romIF.objPlace_kind, objPlace.kind);
                objPlaces.setHalf(o + romIF.objPlace_info, objPlace.info);
                objPlaces.setFloat(o + romIF.objPlace_xpos, objPlace.x);
                objPlaces.setFloat(o + romIF.objPlace_ypos, objPlace.y);
            }

            return objPlaces;
        }
    }
}
