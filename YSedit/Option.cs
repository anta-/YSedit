using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace YSedit
{
    class Option
    {
        Dictionary<string, object> options = new Dictionary<string,object>();

        public object this[string i] {
            get
            {
                Debug.Assert(options.ContainsKey(i));
                return options[i];
            }
            set
            {
                Debug.Assert(options.ContainsKey(i));
                options[i] = value;
            }
        }

        public void registerBooleanMenuItem(ToolStripMenuItem t, string name)
        {
            Debug.Assert(!options.ContainsKey(name));
            options[name] = t.Checked;
            t.CheckedChanged +=
                (object sender, EventArgs e) =>
                    this[name] = ((ToolStripMenuItem)sender).Checked;
        }
    }
}
