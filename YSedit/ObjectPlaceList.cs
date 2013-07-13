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
    partial class ObjectPlaceList : Form
    {
        public MainView mainView;

        public class ObjPlace
        {
            public ushort kind, info;
            public float x, y;
            public ObjPlace(ushort k, ushort i, float x, float y)
            {
                kind = k; info = i; this.x = x; this.y = y;
            }
        }

        class Item
        {
            public int Id;
            ObjPlace objPlace;

            public Item(int i, ObjPlace o)
            {
                Id = i; objPlace = o;
            }

            public string Str
            {
                get {
                    return Id.ToString("x2") + " " +
                        objPlace.kind.ToString("x4") + " " +
                        ObjectInfo.getObjectName(objPlace.kind) +
                        " (" + objPlace.x.floatToHexString() + ", " + objPlace.y.floatToHexString() + ")";
                }
            }

            public ObjPlace ObjPlace
            {
                get { return objPlace; }
            }
        }

        public ObjectPlaceList()
        {
            InitializeComponent();

            listBox1.ValueMember = "ObjPlace";
            listBox1.DisplayMember = "Str";
            listBox1.DataSource = new Item[0] { };
        }

        private void ObjectPlaceList_Resize(object sender, EventArgs e)
        {
            listBox1.Height = ClientSize.Height - listBox1.Top - 12;
            listBox1.Width = ClientSize.Width - listBox1.Left - 12;
        }

        public void SetObjPlaceList(List<ObjPlace> list)
        {
            List<Item> items = new List<Item>();
            for (var i = 0; i < list.Count; i++)
            {
                items.Add(new Item(i, list[i]));
            }
            changedBySelf = true;
            listBox1.DataSource = items.ToArray();
            changedBySelf = false;
            selectedObjectChanged();
        }

        bool changedBySelf = false;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedObjectChanged();
            if (changedBySelf) return;
            changedBySelf = true;
            List<int> list = new List<int>();
            foreach(var i in listBox1.SelectedIndices)
                list.Add((int)i);
            changedBySelf = false;
            mainView.SetSelectedIds(list);
        }

        class Unified<T> where T : struct
        {
            public enum State
            {
                Init,
                One,
                Null
            }
            T? val = null;
            State state = State.Init;

            public T Value
            {
                get { return val.Value; }
            }

            public T? Nullable
            {
                get { return val; }
            }

            public void Unify(T y)
            {
                switch (state) {
                    case State.Init:
                        val = y;
                        state = State.One;
                        break;
                    case State.One:
                        if (!val.Value.Equals(y))
                        {
                            state = State.Null;
                            val = null;
                        }
                        break;
                    case State.Null:
                        break;
                }
            }
        }

        void setValues(int? id, ushort? kind, ushort? info, float? x, float? y)
        {
            textBoxID.Text = id == null ? "" : id.Value.ToString("x2");
            textBoxKind.Text = kind == null ? "" : kind.Value.ToString("x4");
            textBoxInfo.Text = info == null ? "" : info.Value.ToString("x4");
            textBoxPosX.Text = x == null ? "" : x.Value.floatToHexString();
            textBoxPosY.Text = y == null ? "" : y.Value.floatToHexString();
        }
        void selectedObjectChanged()
        {
            Unified<int> id = new Unified<int>();
            Unified<ushort> kind = new Unified<ushort>(), info = new Unified<ushort>();
            Unified<float> x = new Unified<float>(), y = new Unified<float>();

            foreach (var i_ in listBox1.SelectedIndices)
            {
                int i = (int)i_;
                id.Unify(i);
                var o = ((Item[])listBox1.DataSource)[i].ObjPlace;
                kind.Unify(o.kind);
                info.Unify(o.info);
                x.Unify(o.x);
                y.Unify(o.y);
            }
            setValues(id.Nullable, kind.Nullable, info.Nullable, x.Nullable, y.Nullable);
        }

        void assign<T>(ref T x, T? y) where T : struct
        {
            if (y != null)
                x = y.Value;
        }

        bool equal<T>(T x, T? y) where T : struct
        {
            return y == null || x.Equals(y.Value);
        }

        private void textBoxValueChanged()
        {
            int? id = (int?)textBoxID.Text.tryParseHex(0xff);
            ushort?
                kind = (ushort?)textBoxKind.Text.tryParseHex(0xffff),
                info = (ushort?)textBoxInfo.Text.tryParseHex(0xffff);
            float?
                x = textBoxPosX.Text.tryFloatFromHexString(),
                y = textBoxPosY.Text.tryFloatFromHexString();

            var items = ((Item[])listBox1.DataSource).ToArray();
            List<Item> selectedItems = new List<Item>();
            foreach (var i_ in listBox1.SelectedIndices)
                selectedItems.Add(items[(int)i_]);
            selectedItems.Sort(new Comparison<Item>((Item t, Item u) => {
                return t.Id.CompareTo(u.Id); }));

            if (selectedItems.All(t =>
            {
                var o = t.ObjPlace;
                return equal(t.Id, id) &&
                    equal(o.kind, kind) &&
                    equal(o.info, info) &&
                    equal(o.x, x) &&
                    equal(o.y, y);
            }))
            {
                selectedObjectChanged();
                return;
            }

            var selected = selectedIndices();
            if (id != null && selectedItems.Any(t => t.Id != id.Value))
            {
                var notSelectedItems =
                    items.Except(selectedItems);
                int firstPartSize = Math.Min((int)id.Value, notSelectedItems.Count());
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    selected[i] = selectedItems[i].Id = (int)(firstPartSize + i);
                }
                items =
                    notSelectedItems.Take(firstPartSize)
                    .Concat(selectedItems)
                    .Concat(notSelectedItems.Skip(firstPartSize)).ToArray();
            }
            foreach (var t in selectedItems)
            {
                var o = t.ObjPlace;
                assign(ref o.kind, kind);
                assign(ref o.info, info);
                assign(ref o.x, x);
                assign(ref o.y, y);
            }

            listBox1.SuspendDrawing();
            mainView.setObjPlaces(items
                .Select(i => {
                    var o = i.ObjPlace;
                    return new MainView.Obj(o.kind, o.info, o.x, o.y);
                }).ToList());
            mainView.changed();
            changedBySelf = true;
            foreach (var i in selected)
                listBox1.SelectedIndices.Add(i);
            changedBySelf = false;
            selectedObjectChanged();
            listBox1.ResumeDrawing();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            textBoxValueChanged();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBoxValueChanged();
            }
        }

        List<int> selectedIndices()
        {
            List<int> list = new List<int>();
            foreach (var i in listBox1.SelectedIndices)
                list.Add((int)i);
            list.Sort();
            return list;
        }

        public void SetSelectedIds(List<int> ids)
        {
            if (changedBySelf)
                return;

            if (ids.OrderBy(x => x).SequenceEqual(selectedIndices()))
                return;

            changedBySelf = true;
            listBox1.SuspendDrawing();
            listBox1.SelectedIndices.Clear();
            foreach (var id in ids)
                listBox1.SelectedIndices.Add(id);
            listBox1.ResumeDrawing();
            changedBySelf = false;
        }
    }
}
