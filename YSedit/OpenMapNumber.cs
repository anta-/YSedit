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
    public partial class OpenMapNumber : Form
    {
        class Item
        {
            uint num;
            public uint Value
            {
                get { return num; }
            }
            public string Str
            {
                get {
                    if (Value == UnknownItemValue)
                        return "";
                    return num.ToString("x2") + " " + MapName.getMapName(num);
                }
            }

            public Item(uint n) { num = n; }

            public static readonly uint UnknownItemValue = 0xffffffff;
            public static readonly Item UnknownItem =
                new Item(UnknownItemValue);
        }

        public OpenMapNumber(uint defaultMapNumber)
        {
            InitializeComponent();

            mapName.ValueMember = "Value";
            mapName.DisplayMember = "Str";
            mapName.DataSource =
                Enumerable.Repeat(Item.UnknownItem, 1).Concat(
                Enumerable.Range(0, 0x100)
                .Select(x => (uint)x)
                .Where(x => MapName.getMapName(x) != "")
                .Select(x => new Item(x))
                .OrderBy(x => MapName.getMapName(x.Value)))
                .ToArray();

            mapNumber.Text = defaultMapNumber.ToString("x2");
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void mapName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            changedSelf = true;
            var x = (uint)mapName.SelectedValue;
            mapNumber.Text = x == Item.UnknownItemValue ? "" : x.ToString("x2");
            changedSelf = false;
        }

        bool changedSelf = false;
        private void mapNumber_TextChanged(object sender, EventArgs e)
        {
            if (changedSelf) return;
            try
            {
                var x = mapNumber.Text.parseHex(0xff);
                var i = ((Item[])mapName.DataSource).First(t => t.Value == x);
                mapName.SelectedValue = i.Value;
            }
            catch (Exception)
            {
                mapName.SelectedValue = Item.UnknownItem;
            }
        }
    }

    static class MapName
    {
        static string[] mapNames = new string[0x100];

        public static void init()
        {
            foreach (var line in Util.getLinesFromFile("MapNames.txt"))
            {
                string[] t = line.Split('\t');
                mapNames[t[0].parseHex(0xff)] = t[1];
            }
        }

        public static string getMapName(uint x)
        {
            if (x > 0x100 || mapNames[x] == null)
                return "";
            return mapNames[x];
        }
    }
}
