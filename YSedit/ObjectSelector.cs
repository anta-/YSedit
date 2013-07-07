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
    public partial class ObjectSelector : Form
    {
        class ObjectItem
        {
            ushort kind;
            public ushort Kind
            {
                get { return kind; }
            }
            public string String
            {
                get {
                    return kind.ToString("x4") + " " +
                        ObjectInfo.getObjectName(kind, ObjectInfo.Language.Japanese);
                }
            }

            public ObjectItem(ushort kind)
            {
                this.kind = kind;
            }
        };

        List<ObjectItem[]> objectGroups;

        public ObjectSelector()
        {
            InitializeComponent();

            objectSelect.ValueMember = "Kind";
            objectSelect.DisplayMember = "String";
            objectSelect.SelectedValueChanged += new EventHandler(objectSelect_SelectedValueChanged);

            loadObjectGroups();
        }

        void objectSelect_SelectedValueChanged(object sender, EventArgs e)
        {
            var kind = (ushort)objectSelect.SelectedValue;
            objectDescription.Text =
                ObjectInfo.getObjectDescription(kind,
                    ObjectInfo.Language.Japanese);
        }

        void loadObjectGroups()
        {

            objectGroups = new List<ObjectItem[]>();
            var groupNames = new List<string>();
            foreach (var g in ObjectInfo.objectGroups)
            {
                groupNames.Add(g.groupName);
                objectGroups.Add(g.objects.Select(k => new ObjectItem(k)).ToArray());
            }
            objectGroupSelect.DataSource = groupNames.ToArray();
        }

        private void objectGroupSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objects = objectGroups[objectGroupSelect.SelectedIndex];
            objectSelect.DataSource = objects;
        }

        private void splitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            var p = (Panel)sender;
            objectGroupSelect.Width =
                objectSelect.Width = p.ClientSize.Width;
            objectSelect.Height = p.ClientSize.Height - objectSelect.Top;
        }
    }
}
