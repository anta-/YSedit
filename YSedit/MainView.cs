using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        class Obj {
            public ushort kind, info;
            public float x, y;

            public readonly Size objectDrawBox = new Size(17, 17);

            public Rectangle rect
            {
                get {
                    return new Rectangle(new Point((int)x, (int)y), objectDrawBox);
                }
            }

            public Obj(ushort kind, ushort info, float x, float y)
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

        List<Obj> objList = new List<Obj>();
        List<ObjectBox> objBoxes = new List<ObjectBox>();

        Size currentScroll = new Size();

        readonly Size objectMoveEdge = new Size(9, 9);
        /// <summary>
        /// マウスでオブジェクトをドラッグしながら画面端に行くとスクロールするようにするが、
        /// その範囲(正なら外側、負なら内側)
        /// </summary>
        readonly Size mouseDragScrollEdge = new Size(-1, -1);

        HashSet<int> selectObjs = new HashSet<int>();
        bool objDrag = false;
        ObjectBox dragStartControl = null;
        bool dragFore = false;
        Point? dragStartPoint;
        List<Point?> dragStartObjPoses;

        bool rectSelecting = false;
        Point? selectionStartPoint = null;
        Rectangle? selectionRect = null;
        int[] selectObjsAtSelectionStart;

        public MainView(Panel panel, ROMInterface romIF)
        {
            this.romIF = romIF;
            this.panel = panel;

            pictureBox = new PictureBox();
            //子供の場所の描画をするために
            pictureBox.SetWindowLong(-16, pictureBox.GetWindowLong(-16) & ~0x02000000);
            pictureBox.Paint += pictureBox_Paint;
            pictureBox.MouseDown += mouseDown;
            pictureBox.MouseMove += mouseMove;
            pictureBox.MouseUp += mouseUp;
            pictureBox.MouseCaptureChanged += captureChanged;
            
            vScrollBar = new VScrollBar();
            hScrollBar = new HScrollBar();
            vScrollBar.Top = 0;
            hScrollBar.Left = 0;
            vScrollBar.SmallChange = 16;
            hScrollBar.SmallChange = 16;
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

            for (var i = 0; i < objList.Count; i++ )
            {
                var o = objList[i];
                var selected = selectObjs.Contains(i);
                var oRect = o.rect.SubMove((Size)rect.Location).SubResize(new Size(1, 1));
                g.FillRectangle(selected ? selectedRectBrush : rectBrush, oRect);
                g.DrawRectangle(selected ? selectedRectPen : rectPen, oRect);
                string s = ((o.kind & 0xf000) != 0x4000) ? "xxx" : (o.kind & 0xfff).ToString("x3");
                g.DrawString(s, font, selected ? selectedStrBrush : strBrush, new PointF(oRect.X + -1, oRect.Y + 3));
            }

            var selectingRectPen = new Pen(Color.FromArgb(0xff, SystemColors.Highlight));
            var selectingRectBrush = new SolidBrush(Color.FromArgb(0x40, SystemColors.Highlight));
            if (selectionRect != null)
            {
                var select = selectionRect.Value.SubMove((Size)rect.Location);
                g.FillRectangle(selectingRectBrush, select);
                g.DrawRectangle(selectingRectPen, select);
            }
        }

        int? addObj(Obj p)
        {
            int i = objList.Count;
            if (i >= romIF.numberOfObjPlacesMax)
                return null;

            objList.Add(p);
            var b = new ObjectBox(this);
            var rect = p.rect.SubMove(currentScroll);
            b.Location = rect.Location;
            b.Size = rect.Size;

            b.MouseHover += tooltipPopup;
            b.MouseDown += objMouseDown;
            b.MouseUp += objMouseUp;
            b.MouseMove += objMouseMove;
            b.MouseCaptureChanged += objMouseCaptureChanged;
            objBoxes.Add(b);
            pictureBox.Controls.Add(b);
            setObjectChildIndex(i);

            dragStartObjPoses.Add(null);

            return i;
        }

        void deleteObj(int i)
        {
            objList.RemoveAt(i);
            objBoxes.RemoveAt(i);
            pictureBox.Controls.RemoveAt(i);
            dragStartObjPoses.RemoveAt(i);
            if (selectObjs.Contains(i))
                selectObjs.Remove(i);
            changed();
        }

        /// <summary>
        /// objPlacesを設定する
        /// </summary>
        /// <param name="objPlaces">ObjPlace_C[]のData</param>
        public void setObjPlaces(Data objPlaces)
        {
            int number = (int)(objPlaces.bytes.Length / romIF.objPlaceC_size);
            objList = new List<Obj>(number);
            objBoxes = new List<ObjectBox>(number);
            dragStartObjPoses = new List<Point?>(number);

            pictureBox.SuspendDrawing();
            pictureBox.Controls.Clear();
            for (var i = 0; i < number; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                var p = new Obj(
                    objPlaces.getHalf(o + romIF.objPlace_kind),
                    objPlaces.getHalf(o + romIF.objPlace_info),
                    objPlaces.getFloat(o + romIF.objPlace_xpos),
                    objPlaces.getFloat(o + romIF.objPlace_ypos));
                addObj(p);
            }

            selectObjs.Clear();

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
            if (i == -1)
                return;
            var o = objList[i];
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
            Data objPlaces = new Data(new byte[objList.Count * romIF.objPlaceC_size]);

            for (var i = 0; i < objList.Count; i++)
            {
                uint o = (uint)(i * romIF.objPlaceC_size);
                var objPlace = objList[i];
                objPlaces.setHalf(o + romIF.objPlace_kind, objPlace.kind);
                objPlaces.setHalf(o + romIF.objPlace_info, objPlace.info);
                objPlaces.setFloat(o + romIF.objPlace_xpos, objPlace.x);
                objPlaces.setFloat(o + romIF.objPlace_ypos, objPlace.y);
            }

            return objPlaces;
        }

        /// <summary>
        /// スクロール・リサイズをし終わった時
        /// </summary>
        void movingEnd()
        {
            currentScroll = new Size(hScrollBar.Value, vScrollBar.Value);
            pictureBox.SuspendDrawing();
            for (var i = 0; i < objList.Count; i++)
            {
                var o = objList[i];
                objBoxes[i].Location =
                    new Point((int)o.x, (int)o.y) - currentScroll;
            }
            pictureBox.ResumeDrawing();
        }

        void objMouseDown(object sender, MouseEventArgs e)
        {
            var p = (ObjectBox)sender;
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                mouseRightDown(p.Location.Add(e.Location) + currentScroll);
                return;
            }

            dragStart(p, pictureBox.PointToClient(p.PointToScreen(e.Location)) + currentScroll);
        }

        /// <param name="pos">スクロール済み座標</param>
        void dragStart(ObjectBox p, Point pos)
        {
            var i = objBoxes.IndexOf(p);
            if (selectObjs.Contains(i))
            {
            }
            else if (Control.ModifierKeys.HasFlag(Keys.Control))
            {
                selectObjs.Add(i);
            }
            else
            {
                selectObjs.Clear();
                selectObjs.Add(i);
            }
            infoObjSelected();

            p.Capture = true;
            dragStartControl = p;
            objDrag = true;
            dragFore = false;
            dragStartPoint = pos;

            foreach (var j in selectObjs)
            {
                dragStartObjPoses[j] = new Point((int)objList[j].x, (int)objList[j].y);
                objBoxes[j].Visible = false;
            }
        }

        void changed()
        {
            if (onChanged != null) onChanged();
        }

        void moveObject(int i, float x, float y)
        {
            if (objList[i].x != x ||
                objList[i].y != y) changed();
            objList[i].x = x;
            objList[i].y = y;
        }

        int fitGrid(int x)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Shift))
            {   //1ずつ動かすモード
                return x;
            }
            else
            {   //8ずつ動かすモード
                return (Math.Abs(x) + 4) / 8 * 8 * Math.Sign(x);
            }
        }

        bool moveSelectObjs(ObjectBox p, Point pos)
        {
            pos += currentScroll;
            int dx = fitGrid(pos.X - dragStartPoint.Value.X),
                dy = fitGrid(pos.Y - dragStartPoint.Value.Y);

            foreach (var i in selectObjs)
            {
                Point t =
                    dragStartObjPoses[i].Value + new Size(dx, dy);
                t = t.Fit(
                    ((Point)objectMoveEdge - objList[i].objectDrawBox).TwoPoints(
                        (Point)size - objectMoveEdge + new Size(1, 1)));
                moveObject(i, t.X, t.Y);
            }

            if (dragFore || dx != 0 || dy != 0)
                Program.showInfo("Move objects " + dx.formatSigned() + " x " + dy.formatSigned());

            return dx != 0 || dy != 0;
        }

        Point getMouseClientPos(ObjectBox sender)
        {
            return pictureBox.PointToClient(Control.MousePosition);
        }

        void objMouseUp(object sender, MouseEventArgs e)
        {
            if (!objDrag) return;
            var p = (ObjectBox)sender;

            endDrag();
        }

        void objMouseMove(object sender, MouseEventArgs e)
        {
            if (!objDrag) return;
            var p = (ObjectBox)sender;

            var pos = getMouseClientPos(p);

            dragScroll(pos);
            bool moving = moveSelectObjs(p, pos);

            if (!dragFore && moving)
            {
                var perm = (
                    from i in Enumerable.Range(0, objList.Count)
                    orderby Tuple.Create(selectObjs.Contains(i), i)
                    select i).ToArray();
                changeObjectID(perm);
                dragFore = true;
            }

            redraw();
        }

        void endDrag()
        {
            var p = dragStartControl;
            moveSelectObjs(p, getMouseClientPos(p));

            dragFore = false;
            objDrag = false;
            dragStartPoint = null;
            p.Capture = false;
            dragStartControl = null;
            
            movingEnd();
            redraw();

            foreach (var j in selectObjs)
            {
                dragStartObjPoses[j] = null;
                objBoxes[j].Visible = true;
            }

            infoObjSelected();
        }

        /// <summary>
        /// 画面端での自動スクロール処理
        /// </summary>
        /// <param name="pos">pictureBoxでの位置(スクロールは足さない)</param>
        void dragScroll(Point pos)
        {
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
        }

        void objMouseCaptureChanged(object sender, EventArgs e)
        {
            var p = (ObjectBox)sender;
            if (!p.Capture && objDrag)
            {
                endDrag();
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
            var num = objList.Count;
            var tmpObjPlaceList = new Obj[num];
            objList.CopyTo(tmpObjPlaceList);
            var tmpObjPictureBoxes = new ObjectBox[num];
            objBoxes.CopyTo(tmpObjPictureBoxes);
            var tmpSelectObjs = new int[selectObjs.Count];
            selectObjs.CopyTo(tmpSelectObjs);
            var tmpDragStartObjPoses = new Point?[num];
            dragStartObjPoses.CopyTo(tmpDragStartObjPoses);

            selectObjs.Clear();
            for (var i = 0; i < num; i++)
            {
                objList[i] = tmpObjPlaceList[perm[i]];
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

        void mouseDown(object sender, MouseEventArgs e)
        {
            Debug.Assert(!objDrag);

            if (e.Button.HasFlag(MouseButtons.Right))
            {
                mouseRightDown(e.Location + currentScroll);
                return;
            }

            if (!Control.ModifierKeys.HasFlag(Keys.Control))
            {
                selectObjs.Clear();
            }
            selectObjsAtSelectionStart = new int[selectObjs.Count];
            selectObjs.CopyTo(selectObjsAtSelectionStart);
            selectionStartPoint = e.Location + currentScroll;
            pictureBox.Capture = true;
            rectSelecting = true;
            redraw();
        }

        void mouseMove(object sender, MouseEventArgs e)
        {
            if (!rectSelecting) return;

            dragScroll(e.Location);
            selectionRect = selectionStartPoint.Value.TwoPoints(
                e.Location + currentScroll);
            Program.showInfo("Selection " +
                selectionRect.Value.Width.ToString("x") + " x " +
                selectionRect.Value.Height.ToString("x"));

            selectObjs = new HashSet<int>(selectObjsAtSelectionStart);
            for (var i = 0; i < objList.Count; i++)
            {
                var p = objList[i];
                if (selectionRect.Value.Contains(objList[i].rect))
                {
                    if (selectObjs.Contains(i))
                        selectObjs.Remove(i);
                    else
                        selectObjs.Add(i);
                }
            }
            redraw();
        }

        void mouseUp(object sender, MouseEventArgs e)
        {
            endSelecting();
        }

        void captureChanged(object sender, EventArgs e)
        {
            if (!pictureBox.Capture && rectSelecting)
            {
                endSelecting();
            }
        }

        void endSelecting()
        {
            rectSelecting = false;
            pictureBox.Capture = false;
            selectionStartPoint = null;
            selectObjsAtSelectionStart = null;
            selectionRect = null;
            movingEnd();
            redraw();
            infoObjSelected();
        }

        void infoObjSelected()
        {
            if (selectObjs.Count != 0)
                Program.showInfo(selectObjs.Count.ToString("x") + " objects selected");
            else
                Program.showInfo("");
        }

        /// <summary>
        /// 選択中のオブジェクトをコピーする
        /// </summary>
        /// <param name="pos">スクロール済み</param>
        void mouseRightDown(Point pos)
        {
            pos = new Point(fitGrid(pos.X), fitGrid(pos.Y));
            if (selectObjs.Count == 0)
                return;

            Point originPos = new Point(
                Enumerable.Min((IEnumerable<int>)selectObjs, (int i) => (int)objList[i].x),
                Enumerable.Min((IEnumerable<int>)selectObjs, (int i) => (int)objList[i].y)
                );
            List<int> addedObjs = new List<int>();
            foreach (var i in selectObjs)
            {
                var p = objList[i];
                int? j = addObj(new Obj(p.kind, p.info,
                    p.x - originPos.X + pos.X,
                    p.y - originPos.Y + pos.Y));
                if (j == null)
                {
                    Program.showError("Objects is too many!");
                    break;
                }
                addedObjs.Add(j.Value);
                changed();
            }
            selectObjs.Clear();
            foreach (var j in addedObjs)
                selectObjs.Add(j);

            if (selectObjs.Count() != 0)
            {
                int k = selectObjs.First();
                dragStart(objBoxes[k], pos);
            }

            redraw();
        }

        public void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                deleteSelectObject();
        }

        void deleteSelectObject()
        {
            if (objDrag)
                endDrag();
            if (rectSelecting)
                endSelecting();
            int[] tmpSelectObjs = new int[selectObjs.Count];
            selectObjs.CopyTo(tmpSelectObjs);
            tmpSelectObjs = tmpSelectObjs.OrderByDescending(x => x).ToArray();
            foreach (var i in tmpSelectObjs)
                deleteObj(i);
            
            redraw();
        }
    }
}
