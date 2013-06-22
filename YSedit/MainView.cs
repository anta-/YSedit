﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Input = System.Windows.Input;

namespace YSedit
{
    /// <summary>
    /// メインビューの操作と描画をする。
    /// ObjPlacesなどのDataをやり取りする。
    /// </summary>
    class MainView : IDisposable
    {
        public delegate void OnChangedProc();
        /// <summary>
        /// 変化しえたこと(実際のデータは変化しているとは限らない)を通知する
        /// </summary>
        public event OnChangedProc onChanged;

        ROMInterface romIF;

        Panel panel;
        PictureBox pictureBox;
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        Bitmap mainBmp = new Bitmap(1, 1);

        Size size_;
        public Size size {
            get { return size_; }
            set {
                size_ = value;
                setScrollBarSize();
                redraw();
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
        class ObjectBox : PictureBox
        {
            MainView mainView;
            public ObjectBox(MainView mainView)
                : base()
            {
                this.mainView = mainView;
            }

            [DllImport("user32.dll")]
            static extern IntPtr BeginPaint(IntPtr hwnd, IntPtr lpPaint);
            [DllImport("user32.dll")]
            static extern bool EndPaint(IntPtr hWnd, IntPtr lpPaint);

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x14)  //WM_ERASEBKGND
                {
                    m.Result = new IntPtr(1);   //TRUEを返すと背景が塗りつぶされない
                    return;
                }
                if (m.Msg == 0xf)
                {
                    BeginPaint(m.HWnd, m.LParam);
                    EndPaint(m.HWnd, m.LParam);
                    m.Result = new IntPtr(0);
                    return;
                }
                base.WndProc(ref m);
            }
        }

        List<ObjPlace> objPlaceList = new List<ObjPlace>();
        List<ObjectBox> objBoxes = new List<ObjectBox>();

        Size currentScroll = new Size();

        readonly Size objectDrawBox = new Size(17, 17);
        readonly Size objectMoveEdge = new Size(17, 17);
        /// <summary>
        /// マウスでオブジェクトをドラッグしながら画面端に行くとスクロールするようにするが、
        /// その範囲(正なら外側、負なら内側)
        /// </summary>
        readonly Size mouseDragScrollEdge = new Size(-1, -1);

        HashSet<int> selectObjs = new HashSet<int>();
        bool objDrag = false;
        bool dragFore = false;
        Point? dragStartPoint;
        List<Point?> dragStartObjPoses;

        public MainView(Panel panel, ROMInterface romIF)
        {
            this.romIF = romIF;
            this.panel = panel;

            pictureBox = new PictureBox();
            //子供の場所の描画をするために
            pictureBox.SetWindowLong(-16, pictureBox.GetWindowLong(-16) & ~0x02000000);
            pictureBox.Paint += pictureBox_Paint;
            
            vScrollBar = new VScrollBar();
            hScrollBar = new HScrollBar();
            vScrollBar.Top = 0;
            hScrollBar.Left = 0;
            vScrollBar.MouseCaptureChanged += scrollBar_MouseCaptureChanged;
            hScrollBar.MouseCaptureChanged += scrollBar_MouseCaptureChanged;
            vScrollBar.ValueChanged += scrollBar_ValueChanged;
            hScrollBar.ValueChanged += scrollBar_ValueChanged;

            hScrollBar.ResumeDrawing();
            vScrollBar.ResumeDrawing();

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(vScrollBar);
            panel.Controls.Add(hScrollBar);
            panel.Resize += panel_Resize;

            panel_Resize(panel, new EventArgs());
        }

        public void Dispose()
        {
            panel.Controls.Clear();
            panel.Resize -= panel_Resize;
        }

        /// <summary>
        /// rectで指定された範囲を描画して返す
        /// </summary>
        /// <param name="bmp">描画するビットマップ</param>
        /// <param name="rect">描画する範囲</param>
        void render(Bitmap bmp, Rectangle rect)
        {
            var g = Graphics.FromImage(bmp);
            var font = new Font("MS Gothic", 8);
            var rectBrush = new SolidBrush(Color.FromArgb(0xff, Color.White));
            var rectPen = new Pen(Color.FromArgb(0xff, Color.SkyBlue));
            var strBrush = new SolidBrush(Color.FromArgb(0xff, Color.Black));
            var selectedRectBrush = new SolidBrush(Color.FromArgb(0xff, Color.Black));
            var selectedRectPen = new Pen(Color.FromArgb(0xff, Color.SkyBlue));
            var selectedStrBrush = new SolidBrush(Color.FromArgb(0xff, Color.White));

            for (var i = 0; i < objPlaceList.Count; i++ )
            {
                var o = objPlaceList[i];
                var selected = selectObjs.Contains(i);
                var oRect = new Rectangle(new Point((int)o.x, (int)o.y).Sub(rect.Location),
                    new Size(objectDrawBox.Width - 1, objectDrawBox.Height - 1));
                g.FillRectangle(selected ? selectedRectBrush : rectBrush, oRect);
                g.DrawRectangle(selected ? selectedRectPen : rectPen, oRect);
                string s = ((o.kind & 0xf000) != 0x4000) ? "xxx" : (o.kind & 0xfff).ToString("x3");
                g.DrawString(s, font, selected ? selectedStrBrush : strBrush, new PointF(oRect.X + -1, oRect.Y + 3));
            }
        }



        /// <summary>
        /// objPlacesを設定する
        /// </summary>
        /// <param name="objPlaces">ObjPlace_C[]のData</param>
        public void setObjPlaces(Data objPlaces)
        {
            int number = (int)(objPlaces.bytes.Length / romIF.objPlaceC_size);
            objPlaceList = new List<ObjPlace>(number);
            objBoxes = new List<ObjectBox>(number);

            pictureBox.SuspendDrawing();
            pictureBox.Controls.Clear();
            for (var i = 0; i < number; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                var p = new ObjPlace(
                    objPlaces.getHalf(o + romIF.objPlace_kind),
                    objPlaces.getHalf(o + romIF.objPlace_info),
                    objPlaces.getFloat(o + romIF.objPlace_xpos),
                    objPlaces.getFloat(o + romIF.objPlace_ypos));
                objPlaceList.Add(p);
                var b = new ObjectBox(this);
                b.Location = new Point((int)p.x, (int)p.y) - currentScroll;
                b.Size = objectDrawBox;
                
                b.MouseHover += tooltipPopup;
                b.MouseDown += objMouseDown;
                b.MouseUp += objMouseUp;
                b.MouseMove += objMouseMove;
                b.MouseCaptureChanged += objMouseCaptureChanged;
                objBoxes.Add(b);
                pictureBox.Controls.Add(b);
                setObjectChildIndex(i);
            }
            pictureBox.MouseClick += mouseClick;

            selectObjs.Clear();
            dragStartObjPoses = Enumerable.Repeat<Point?>(null, number).ToList();

            movingEnd();
            redraw();
        }

        void setObjectChildIndex(int i)
        {
            //値が小さい方が前面に出る
            pictureBox.Controls.SetChildIndex(
                objBoxes[i], objBoxes.Count - i);
        }

        void tooltipPopup(object sender, EventArgs e)
        {
            var t = new ToolTip();
            var p = (ObjectBox)sender;
            var i = objBoxes.IndexOf(p);
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
        /// スクロール・リサイズをし終わった時
        /// </summary>
        void movingEnd()
        {
            currentScroll = new Size(hScrollBar.Value, vScrollBar.Value);
            pictureBox.SuspendDrawing();
            for (var i = 0; i < objPlaceList.Count; i++)
            {
                var o = objPlaceList[i];
                objBoxes[i].Location =
                    new Point((int)o.x, (int)o.y) - currentScroll;
            }
            pictureBox.ResumeDrawing();
        }

        void objMouseDown(object sender, MouseEventArgs e)
        {
            var p = (ObjectBox)sender;
            var i = objBoxes.IndexOf(p);
            if (selectObjs.Contains(i))
            {
            }
            else if (getKey(Input.Key.LeftCtrl) || getKey(Input.Key.RightCtrl))
            {
                selectObjs.Add(i);
            }
            else
            {
                selectObjs.Clear();
                selectObjs.Add(i);
            }
            p.Capture = true;
            objDrag = true;
            dragFore = false;
            dragStartPoint = pictureBox.PointToClient(p.PointToScreen(e.Location)) + currentScroll;

            foreach (var j in selectObjs)
            {
                dragStartObjPoses[j] = new Point((int)objPlaceList[j].x, (int)objPlaceList[j].y);
                objBoxes[j].Visible = false;
            }
        }

        void moveObject(int i, float x, float y)
        {
            if ((objPlaceList[i].x != x ||
                objPlaceList[i].y != y) &&
                onChanged != null) onChanged();
            objPlaceList[i].x = x;
            objPlaceList[i].y = y;
        }

        void mouseClick(object sender, MouseEventArgs e)
        {
            selectObjs.Clear();
            redraw();
        }

        bool getKey(Input.Key key)
        {
            return Input.Keyboard.GetKeyStates(key).HasFlag(Input.KeyStates.Down);
        }

        int fitMoving(int x)
        {
            if (getKey(Input.Key.LeftShift) || getKey(Input.Key.RightShift))
            {   //1ずつ動かすモード
                return x;
            }
            else
            {   //8ずつ動かすモード
                return (x + 4) / 8 * 8;
            }
        }

        bool moveSelectObjs(ObjectBox p, Point pos)
        {
            Program.form.setInfoStatusText(currentScroll.ToString());
            pos += currentScroll;
            int dx = fitMoving(pos.X - dragStartPoint.Value.X),
                dy = fitMoving(pos.Y - dragStartPoint.Value.Y);

            foreach (var i in selectObjs)
            {
                Point t =
                    dragStartObjPoses[i].Value + new Size(dx, dy);
                t = t.Fit(
                    ((Point)objectMoveEdge - objectDrawBox).TwoPoints(
                        (Point)size - objectMoveEdge));
                moveObject(i, t.X, t.Y);
            }

            return dx != 0 || dy != 0;
        }

        Point getMouseClientPos(ObjectBox sender, MouseEventArgs e)
        {
            return pictureBox.PointToClient(
                sender.PointToScreen(e.Location));
        }

        void objMouseUp(object sender, MouseEventArgs e)
        {
            if (!objDrag) return;
            var p = (ObjectBox)sender;
            moveSelectObjs(p, getMouseClientPos(p, e));

            dragFore = false;
            objDrag = false;
            dragStartPoint = null;
            p.Capture = false;
            
            movingEnd();
            redraw();

            foreach (var j in selectObjs)
            {
                dragStartObjPoses[j] = null;
                objBoxes[j].Visible = true;
            }
        }

        void objMouseMove(object sender, MouseEventArgs e)
        {
            if (!objDrag) return;
            var p = (ObjectBox)sender;

            var pos = getMouseClientPos(p, e);

            var notScrollRect = pictureBox.ClientRectangle.Expand(mouseDragScrollEdge);
            var scroll =
                pos.Sub(notScrollRect.Location).Min2(Point.Empty).Add(
                pos.Sub(notScrollRect.Location + notScrollRect.Size - new Size(1, 1)).Max2(Point.Empty));
            if (scroll.X != 0 || scroll.Y != 0)
            {
                scrollBarValueChangedRedraw = false;
                hScrollBar.Value = Math.Max(0, Math.Min(hScrollBar.Value + Math.Sign(scroll.X) * 16, hScrollBar.Maximum - hScrollBar.LargeChange + 1));
                vScrollBar.Value = Math.Max(0, Math.Min(vScrollBar.Value + Math.Sign(scroll.Y) * 16, vScrollBar.Maximum - vScrollBar.LargeChange + 1));
                scrollBarValueChangedRedraw = true;
            }

            bool moving = moveSelectObjs(p, pos);

            if (!dragFore && moving)
            {
                var perm = (
                    from i in Enumerable.Range(0, objPlaceList.Count)
                    orderby Tuple.Create(selectObjs.Contains(i), i)
                    select i).ToArray();
                changeObjectID(perm);
                dragFore = true;
            }

            redraw();
        }

        void objMouseCaptureChanged(object sender, EventArgs e)
        {
            var p = (ObjectBox)sender;
            if (!p.Capture && objDrag)
            {
                objDrag = false;
                dragStartPoint = null;
                redraw();

                foreach (var j in selectObjs)
                {
                    dragStartObjPoses[j] = null;
                    moveObject(j, objPlaceList[j].x, objPlaceList[j].y);
                    objBoxes[j].Visible = true;
                }
            }
        }

        void swapWithIndex<T>(List<T> o, int i, int j)
        {
            var tmp = o[i];
            o[i] = o[j];
            o[j] = tmp;
        }

        /// <summary>
        /// オブジェクトをIDだけ変える。perm[i]をiへ移動させる
        /// </summary>
        /// <param name="perm">移動の順列の逆。順列であることが仮定される</param>
        void changeObjectID(int[] perm)
        {
            var num = objPlaceList.Count;
            var tmpObjPlaceList = new ObjPlace[num];
            objPlaceList.CopyTo(tmpObjPlaceList);
            var tmpObjPictureBoxes = new ObjectBox[num];
            objBoxes.CopyTo(tmpObjPictureBoxes);
            var tmpSelectObjs = new int[selectObjs.Count];
            selectObjs.CopyTo(tmpSelectObjs);
            var tmpDragStartObjPoses = new Point?[num];
            dragStartObjPoses.CopyTo(tmpDragStartObjPoses);

            selectObjs.Clear();
            for (var i = 0; i < num; i++)
            {
                objPlaceList[i] = tmpObjPlaceList[perm[i]];
                objBoxes[i] = tmpObjPictureBoxes[perm[i]];
                dragStartObjPoses[i] = tmpDragStartObjPoses[perm[i]];
                if (tmpSelectObjs.Contains(perm[i]))
                    selectObjs.Add(i);
                if (perm[i] != i)
                    setObjectChildIndex(i);
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(pictureBox.BackColor);
            g.DrawImageUnscaled(mainBmp, 0, 0);
        }

        void setScrollBarSize()
        {
            var pictureBoxWidth = Math.Min(Math.Max(1, panel.ClientSize.Width - vScrollBar.Width), size.Width);
            var pictureBoxHeight = Math.Min(Math.Max(1, panel.ClientSize.Height - hScrollBar.Height), size.Height);

            var vScrollBarSliderSize = pictureBoxHeight / 2;
            vScrollBar.LargeChange = vScrollBarSliderSize;
            vScrollBar.Maximum = Math.Max(0, size.Height - vScrollBarSliderSize - 1);
            vScrollBar.Visible = vScrollBar.Maximum - vScrollBarSliderSize > 0;
            vScrollBar.Left = pictureBoxWidth;
            var vScrollBarHeight = panel.Height - hScrollBar.Height;

            var hScrollBarSliderSize = pictureBoxWidth / 2;
            hScrollBar.LargeChange = hScrollBarSliderSize;
            hScrollBar.Maximum = Math.Max(0, size.Width - hScrollBarSliderSize - 1);
            hScrollBar.Visible = hScrollBar.Maximum - hScrollBarSliderSize > 0;
            hScrollBar.Top = pictureBoxHeight;
            var hScrollBarWidth = panel.Width - vScrollBar.Width;

            if (!vScrollBar.Visible)
            {
                pictureBoxWidth += vScrollBar.Width;
                hScrollBarWidth += vScrollBar.Width;
                hScrollBarSliderSize = pictureBoxWidth / 2;
                hScrollBar.LargeChange = hScrollBarSliderSize;
                hScrollBar.Maximum = Math.Max(0, size.Width - hScrollBarSliderSize - 1);
            }

            if (!hScrollBar.Visible)
            {
                pictureBoxHeight += hScrollBar.Height;
                vScrollBarHeight += hScrollBar.Height;
                vScrollBarSliderSize = pictureBoxHeight / 2;
                vScrollBar.LargeChange = vScrollBarSliderSize;
                vScrollBar.Maximum = Math.Max(0, size.Height - vScrollBarSliderSize - 1);
            }

            pictureBox.Size = new Size(pictureBoxWidth, pictureBoxHeight);
            hScrollBar.Width = hScrollBarWidth;
            vScrollBar.Height = vScrollBarHeight;

            if (vScrollBar.Maximum - vScrollBarSliderSize + 1 < vScrollBar.Value ||
                hScrollBar.Maximum - hScrollBarSliderSize + 1 < hScrollBar.Value)
            {
                scrollBarValueChangedRedraw = false;
                vScrollBar.Value = Math.Max(0, Math.Min(vScrollBar.Value, vScrollBar.Maximum - vScrollBarSliderSize + 1));
                hScrollBar.Value = Math.Max(0, Math.Min(hScrollBar.Value, hScrollBar.Maximum - hScrollBarSliderSize + 1));
                scrollBarValueChangedRedraw = true;
            }
        }

        int redrawCount = 0;
        void redraw()
        {
            Program.form.setInfoStatusText(redrawCount.ToString());
            redrawCount++;

            if (pictureBox.Width <= 0 ||
                pictureBox.Height <= 0)
            {
                mainBmp = new Bitmap(1, 1);
            }
            else
            {
                if (mainBmp.Size != pictureBox.Size)
                    mainBmp = new Bitmap(pictureBox.Width, pictureBox.Height);
                else
                    Graphics.FromImage(mainBmp).Clear(Color.Transparent);

                render(mainBmp, new Rectangle(
                    new Point(hScrollBar.Value, vScrollBar.Value),
                    pictureBox.Size));
            }
            pictureBox.Invalidate();
        }



        private void panel_Resize(object sender, EventArgs e)
        {
            setScrollBarSize();
            redraw();
            movingEnd();
        }

        private void scrollBar_MouseCaptureChanged(object sender, EventArgs e)
        {
            var scrollBar = (ScrollBar)sender;
            if (!scrollBar.Capture)
                movingEnd();
        }

        bool scrollBarValueChangedRedraw = true;
        private void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            currentScroll = new Size(hScrollBar.Value, vScrollBar.Value);
            if (scrollBarValueChangedRedraw)
                redraw();
        }
    }
}
