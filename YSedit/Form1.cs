using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace YSedit
{
    public partial class Form1 : Form
    {
        const string appTitle = "YSedit";

        ROM rom = null;
        MainView mainView = null;

        public Form1()
        {
            InitializeComponent();

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
                mainView.Dispose();
                mainView = null;
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
                rom.changedChanged += romChangedChanged;

                rom.mainView = mainView = new MainView(mainViewPanel, rom.romIF);

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
                    if (mainView != null)
                        mainView.Dispose();
                    mainView = null;
                }
            }
            return proceeded;
        }

        void romChangedChanged(bool changed)
        {
            saveMapToolStripMenuItem.Enabled = changed;
            Text = appTitle + (changed ? " *" : "");
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rom != null)
            {
                if (closeRomFile())
                    e.Cancel = true;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainView != null)
                mainView.keyDown(sender, e);
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rom != null)
                rom.saveToROM();
        }
    }
}
