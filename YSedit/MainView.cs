using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace YSedit
{
    /// <summary>
    /// メインビューの操作と描画をする。
    /// ObjPlacesなどのDataをやり取りする。
    /// </summary>
    class MainView
    {
        PictureBox pictureBox;
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
        List<PictureBox> objPictureBoxes = new List<PictureBox>();

        readonly Size objectDrawBox = new Size(17, 17);
        Point currentScroll = new Point();

        public MainView(PictureBox pictureBox, ROMInterface romIF)
        {
            this.pictureBox = pictureBox;
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
            var rectBrush = new SolidBrush(Color.FromArgb(0xff, Color.White));
            var rectPen = new Pen(Color.FromArgb(0xff, Color.SkyBlue));
            var strBrush = new SolidBrush(Color.FromArgb(0xff, Color.Black));
            foreach(var o in objPlaceList)
            {
                var oRect = new Rectangle(new Point((int)o.x - rect.X, (int)o.y - rect.Y),
                    new Size(objectDrawBox.Width - 1, objectDrawBox.Height - 1));
                g.FillRectangle(rectBrush, oRect);
                g.DrawRectangle(rectPen, oRect);
                string s = ((o.kind & 0xf000) != 0x4000) ? "xxx" : (o.kind & 0xfff).ToString("x3");
                g.DrawString(s, font, strBrush, new PointF(oRect.X + -1, oRect.Y + 3));
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
            objPictureBoxes = new List<PictureBox>(number);

            pictureBox.Controls.Clear();
            pictureBox.SuspendLayout();
            for (var i = 0; i < number; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                var p = new ObjPlace(
                    objPlaces.getHalf(o + romIF.objPlace_kind),
                    objPlaces.getHalf(o + romIF.objPlace_info),
                    objPlaces.getFloat(o + romIF.objPlace_xpos),
                    objPlaces.getFloat(o + romIF.objPlace_ypos));
                objPlaceList.Add(p);
                var b = new PictureBox();
                b.Location = new Point((int)p.x - currentScroll.X, (int)p.y - currentScroll.Y);
                b.Size = objectDrawBox;
                b.BackColor = Color.Transparent;
                b.MouseHover += tooltipPopup;
                objPictureBoxes.Add(b);
                pictureBox.Controls.Add(b);
            }
            scrollEnd(currentScroll.X, currentScroll.Y);
        }

        void tooltipPopup(object sender, EventArgs e)
        {
            var t = new ToolTip();
            var p = (PictureBox)sender;
            var i = objPictureBoxes.IndexOf(p);
            var o = objPlaceList[i];
            var namee = ObjectName.getObjectName(o.kind, ObjectName.Language.English);
            var namej = ObjectName.getObjectName(o.kind, ObjectName.Language.Japanese);
            t.SetToolTip(p,
                "#" + i.ToString("x2") + "\n" +
                "Kind: " + o.kind.ToString("x4") + "\n" +
                (namee == null ? "" :
                "Name: " + namee + "\n" +
                "(ja): " + namej + "\n") +
                "Info: " + o.info.ToString("x4") + "\n" +
                "XPos: 0x" + o.x.floatToHexString() + " / " + o.x.ToString() + "\n" +
                "YPos: 0x" + o.y.floatToHexString() + " / " + o.y.ToString());
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

        Rectangle getObjectBox(ObjPlace o)
        {
            return new Rectangle(new Point((int)o.x, (int)o.y), objectDrawBox);
        }

        /// <summary>
        /// スクロールし始めた時に呼ぶ
        /// </summary>
        public void scrollStart()
        {
            for (var i = 0; i < objPlaceList.Count; i++)
            {
                objPictureBoxes[i].Visible = false;
            }
        }

        /// <summary>
        /// スクロールし終わった時に呼ぶ
        /// </summary>
        public void scrollEnd(int x, int y)
        {
            currentScroll = new Point(x, y);

            pictureBox.SuspendLayout();
            for (var i = 0; i < objPlaceList.Count; i++)
            {
                var o = objPlaceList[i];
                objPictureBoxes[i].Location =
                    new Point((int)o.x - x, (int)o.y - y);
                objPictureBoxes[i].Visible = true;
            }
            pictureBox.ResumeLayout(false);
        }
    }
}
