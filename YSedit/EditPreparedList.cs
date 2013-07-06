using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace YSedit
{
    partial class EditPreparedList : Form
    {
        ROMInterface romIF;
        List<ushort> oldIDs, resolved, newResolved, additional;
        public Data resultData;
        string infoText = "";

        public EditPreparedList(ROMInterface romIF, bool resolve, Data objPlaces, Data data)
        {
            this.romIF = romIF;

            loadData(resolve, data, objPlaces);

            InitializeComponent();

            setText(additionalIDs, additional);
            setText(resolvedIDs, resolved);
            setText(newResolvedIDs, newResolved);
            information.Text = infoText.Replace("\n", "\r\n");
            resolvedIDs.Enabled = resolve;
        }

        void loadData(bool r, Data data, Data objPlaces)
        {
            HashSet<ushort> ids = new HashSet<ushort>();
            for (uint i = 0; i < romIF.preparedList_count; i++)
            {
                var h = data.getHalf(i * 2);
                if (h == 0xffff) break;
                ids.Add(h);
            }
            oldIDs = ids.OrderBy(x => x).ToList();

            HashSet<ushort> t = r ? resolve(ids, objPlaces) : new HashSet<ushort>();
            newResolved = t.Except(ids).ToList();
            resolved = t.Except(newResolved).ToList();
            additional = ids.Except(t).ToList();
        }

        HashSet<ushort> resolve(HashSet<ushort> ids, Data objPlaces)
        {
            HashSet<ushort> s = new HashSet<ushort>();

            Action<ushort,string> addObject = (ushort k, string info) =>
            {
                if (s.Contains(k))
                    return;
                infoText += info + " -> " + k.ToString("x4") + "\n";
                s.Add(k);
            };

            foreach (var k in romIF.basicObjects)
                addObject(k, "basic");

            int numberOfObjs = (int)(objPlaces.Length / romIF.objPlaceC_size);
            for (uint i = 0; i < numberOfObjs; i++)
            {
                var kind = objPlaces.getHalf(i * romIF.objPlaceC_size + romIF.objPlace_kind);
                foreach (var dep in ObjectInfo.getObjectDependents(kind))
                {
                    switch (dep.c)
                    {
                        case ObjectInfo.Dependent.Case.ObjCfg:
                            addObject((ushort)dep.a, "#" + kind.ToString("x4"));
                            break;
                        case ObjectInfo.Dependent.Case.ObjFunc:
                            var ks = ObjectInfo.getFuncObjects(dep.a);
                            if (ks.Count == 0)
                            {
                                //どうしようもない
                                infoText += "!" + kind.ToString("x4") + "->" + dep.a.ToString("x8") + "\n";
                                break;
                            }
                            if (!ks.Any(k => s.Contains(k)))
                            {
                                if (ks.Any(k => ids.Contains(k)))
                                    addObject(ks.Find(k => ids.Contains(k)), "%" + kind.ToString("x4"));
                                else 
                                    addObject(ks[0], "$" + kind.ToString("x4"));
                            }
                            break;
                    }
                }
            }

            for (uint i = 0; i < numberOfObjs; i++)
            {
                var kind = objPlaces.getHalf(i * romIF.objPlaceC_size + romIF.objPlace_kind);
                if (s.Remove(kind))
                    infoText += kind.ToString("x4") + " remove" + "\n";
            }

            return s;
        }

        void setText(TextBox t, List<ushort> l)
        {
            t.Text = String.Join(", ", l.OrderBy(x => x).Select(x => x.ToString("x4")));
        }

        ushort[] getText(TextBox t)
        {
            if (t.Text.Trim() == "") return new ushort[]{};
            return t.Text.Split(',').Select(x => (ushort)x.Trim().parseHex(0xffff)).ToArray();
        }

        public class IDsTooMany : ApplicationException
        {
            public int num;
            public IDsTooMany(int num) : base() {
                this.num = num;
            }
        }
        public class ChangeNothing : ApplicationException
        {
            public ChangeNothing() : base() { }
        }

        public Data saveData()
        {
            ushort[] IDs =
                Enumerable.Empty<ushort>()
                .Union(getText(resolvedIDs))
                .Union(getText(newResolvedIDs))
                .Union(getText(additionalIDs))
                .OrderBy(x => x)
                .ToArray();
            if (IDs.Length > romIF.preparedList_count - 1)
                throw new IDsTooMany(IDs.Length);
            if (IDs.SequenceEqual(oldIDs))
                throw new ChangeNothing();
            Data data = new Data(new byte[romIF.preparedList_count * 2]);
            for (uint i = 0; i < IDs.Length; i++)
                data.setHalf(i * 2, IDs[i]);
            data.setHalf((uint)IDs.Length * 2, 0xffff);
            return data;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Action<string> error = s => MessageBox.Show(s, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            try
            {
                resultData = saveData();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ChangeNothing)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            catch (IDsTooMany ex)
            {
                error("IDs is too many (" + ex.num + " / " + (romIF.preparedList_count-1) + ")");
            }
            catch (Exception)
            {
                error("Invalid input");
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
