using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YSedit
{
    public partial class ObjectPlaceList : Form
    {
        public ObjectPlaceList()
        {
            InitializeComponent();
        }

        private void ObjectPlaceList_Resize(object sender, EventArgs e)
        {
            listBox1.Height = ClientSize.Height - listBox1.Top - 12;
            listBox1.Width = ClientSize.Width - listBox1.Left - 12;
        }
    }
}
