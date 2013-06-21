﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YSedit
{
    public partial class Form1 : Form
    {

        ROM rom = null;
        MainView mainView = null;
        Bitmap mainViewBmp = new Bitmap(1, 1);

        public Form1()
        {
            InitializeComponent();
            
            
            Form1_Resize(this, new EventArgs());
            setInfoStatusText("");
            setROMOpend(false);
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openROMFileDialog.ShowDialog() == DialogResult.OK)
               setROMOpend(openRomFile(openROMFileDialog.FileName));
        }

        private void closeROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setROMOpend(closeRomFile());
        }

        void setROMOpend(bool b)
        {
            ToolStripItem[] items = {
                closeROMToolStripMenuItem,
                openMapNumberToolStripMenuItem,
                saveMapToolStripMenuItem,
                mapEditToolStripMenuItem,
                mapInformationToolStripMenuItem,
            };
            foreach (var i in items)
                i.Enabled = b;

            saveMapToolStripMenuItem.Enabled = false;
        }

        private void editObjectListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rom.map.editObjectList();
        }

        private void openMapNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenMapNumber();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var num = dialog.mapNumber.Text.parseHex(0xff);
                    openMapNumber((byte)num);
                }
                catch (FormatException)
                {
                    MessageBox.Show("invalid map number", "Error");
                }
            }
        }

        public void setInfoStatusText(string str)
        {
            infoStatusText.ToolTipText = str;
            statusStrip1_Resize(statusStrip1, new EventArgs());
        }

        private void statusStrip1_Resize(object sender, EventArgs e)
        {
            var graphics = statusStrip1.CreateGraphics();
            var width = statusStrip1.ClientSize.Width;
            var text = infoStatusText.ToolTipText;

            if (graphics.MeasureString(text, statusStrip1.Font).Width - 1e-9 <= width)
            {
                infoStatusText.Text = text;
            }
            else
            {
                int l = 0, u = text.Length + 1;
                while (u - l > 1)
                {
                    int m = (l + u) / 2;
                    string str = text.Substring(0, m) + "...";
                    if (graphics.MeasureString(str, statusStrip1.Font).Width - 1e-9 <= width)
                        l = m;
                    else
                        u = m;
                }
                infoStatusText.Text = text.Substring(0, l) + "...";
            }
        }
        /// <summary>
        /// 今開いているROMを閉じる
        /// </summary>
        /// <returns>実際に閉じたならfalse, 閉じなかったならtrue</returns>
        public bool closeRomFile()
        {
            try
            {
                rom.Dispose();
                rom = null;
                mainView = null;
                renderingMainView();
                setInfoStatusText("ROM closed");
                return false;
            }
            catch (ROM.UserCancel)
            {
                return true;
            }
        }

        /// <summary>
        /// pathのROMファイルを開く。既に開いていたら閉じる
        /// </summary>
        /// <returns>成功したかどうか</returns>
        public bool openRomFile(string path)
        {
            Action<string> err = s =>
            {
                MessageBox.Show(s, "Error");
            };

            if (rom != null)
                closeRomFile();

            bool proceeded = false;
            try
            {
                rom = new ROM(path);
                rom.changedChanged = romChangedChanged;

                rom.mainView = mainView = new MainView(pictureBox1, rom.romIF);
                mainView.changeSize = mainView_setSize;

                rom.openPracticeMap();
                setInfoStatusText("ROM \"" + path + "\", practice map opened");
                proceeded = true;
            }
            catch (System.IO.IOException)
            {
                err("Can't open ROM file");
            }
            catch (ROM.UnknownROMException)
            {
                err("Unknown ROM file");
            }
            catch (DataException e)
            {
                err("ROM is broken (" + e.Message + ")");
            }
            finally
            {
                if (!proceeded)
                {
                    if (rom != null)
                        rom.Dispose();
                    rom = null;
                    mainView = null;
                }
                renderingMainView();
            }
            return proceeded;
        }

        void romChangedChanged(bool changed)
        {
            saveMapToolStripMenuItem.Enabled = changed;
        }

        /// <summary>
        /// mapNumberを開く
        /// </summary>
        /// <returns>成功したかどうか</returns>
        public bool openMapNumber(byte mapNumber)
        {
            try
            {
                rom.openMapNumber(mapNumber);
                setInfoStatusText("Map " + mapNumber.ToString("x2") + " opened");
                return true;
            }
            catch (ROM.UserCancel)
            {
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Can't open map number");
                return false;
            }
        }

        private void mapInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(rom.map.getMapInformationStr(), "Map information");
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(pictureBox1.BackColor);
            g.DrawImageUnscaled(mainViewBmp, 0, 0);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            setScrollBarSize();
            renderingMainView();
        }

        void setScrollBarSize()
        {
            Size mainViewSize = mainView != null ? mainView.size : new Size(1, 1);

            var pictureBox1Width = Math.Max(1, ClientSize.Width - vScrollBar1.Width);
            var pictureBox1Height = Math.Max(1, ClientSize.Height - statusStrip1.Height - hScrollBar1.Height - menuStrip1.Height);

            vScrollBar1.Maximum = Math.Max(0, mainViewSize.Height - 1 - pictureBox1Height);
            vScrollBar1.Visible = vScrollBar1.Maximum > 0;
            vScrollBar1.Left = ClientSize.Width - vScrollBar1.Width;
            var vScrollBar1Height = statusStrip1.Top - menuStrip1.Height - hScrollBar1.Height;
            vScrollBar1.LargeChange = pictureBox1Height / 2;

            hScrollBar1.Maximum = Math.Max(0, mainViewSize.Width - 1 - pictureBox1Width);
            hScrollBar1.Visible = hScrollBar1.Maximum > 0;
            hScrollBar1.Top = statusStrip1.Top - hScrollBar1.Height;
            var hScrollBar1Width = ClientSize.Width - vScrollBar1.Width;
            hScrollBar1.LargeChange = pictureBox1Width / 2;

            if (!vScrollBar1.Visible)
            {
                pictureBox1Width += vScrollBar1.Width;
                hScrollBar1Width += vScrollBar1.Width;
            }

            if (!hScrollBar1.Visible)
            {
                pictureBox1Height += hScrollBar1.Height;
                vScrollBar1Height += hScrollBar1.Height;
            }

            vScrollBar1.Enabled = hScrollBar1.Enabled = mainView != null;

            pictureBox1.Size = new Size(pictureBox1Width, pictureBox1Height);
            hScrollBar1.Width = hScrollBar1Width;
            vScrollBar1.Height = vScrollBar1Height;
        }

        void mainView_setSize(MainView sender)
        {
            setScrollBarSize();
        }

        void renderingMainView()
        {
            if (mainView == null ||
                pictureBox1.Width <= 0 ||
                pictureBox1.Height <= 0)
            {
                mainViewBmp = new Bitmap(1, 1);
            }
            else
            {

                mainViewBmp = mainView.rendering(new Rectangle(
                    new Point(hScrollBar1.Value, vScrollBar1.Value),
                    pictureBox1.Size));
            }
            pictureBox1.Invalidate();
        }

        private void scrollBar1_ValueChanged(object sender, EventArgs e)
        {
            renderingMainView();
        }

        private void scrollBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (mainView != null)
            {
                var scrollBar = (ScrollBar)sender;
                if (scrollBar.Capture)
                    mainView.scrollStart();
                else
                    mainView.scrollEnd(hScrollBar1.Value, vScrollBar1.Value);
            }
        }
    }
}
