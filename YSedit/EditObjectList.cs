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
    partial class EditObjectList : Form
    {
        ROMInterface romIF;
        /// <summary>
        /// 返り値
        /// </summary>
        public Data objPlaces = null;
        readonly Data firstObjPlaces;

        uint unknownKindValue = 0xffffffff;
        readonly Dictionary<uint, string> objNameMap;

        public EditObjectList(ROMInterface romIF_, Data objPlaces_)
        {
            romIF = romIF_;
            firstObjPlaces = objPlaces_;

            InitializeComponent();


            DataGridViewComboBoxColumn c = (DataGridViewComboBoxColumn)dataGridView1.Columns[2];
            objNameMap = ObjectName.getEnMap;
            var dataSource = objNameMap.ToList();
            dataSource.Insert(0, new KeyValuePair<uint,string>(unknownKindValue, ""));
            c.DataSource = dataSource.OrderBy(x => x.Value).ToArray();
            c.ValueMember = "Key";
            c.DisplayMember = "Value";

            uint objs = (uint)firstObjPlaces.bytes.Length / romIF.objPlaceC_size;

            for (var i = 0; i < objs; i++)
            {
                var p = (uint)i * romIF.objPlaceC_size;
                var kind = (uint)firstObjPlaces.getHalf(p + romIF.objPlace_kind);
                dataGridView1.Rows.Add(
                    formatCellValue((uint)i, 0),
                    formatCellValue(kind, 1),
                    objNameMap.ContainsKey(kind) ? kind : unknownKindValue,
                    formatCellValue((uint)firstObjPlaces.getHalf(p + romIF.objPlace_info), 3),
                    formatCellValue(firstObjPlaces.getFloat(p + romIF.objPlace_xpos), 4),
                    formatCellValue(firstObjPlaces.getFloat(p + romIF.objPlace_ypos), 5));
            }
        }

        string getObjectNameDisplayString(uint i)
        {
            return ObjectName.getObjectName(i, ObjectName.Language.English);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        class IndvalidResultDataException : ApplicationException
        {
            public IndvalidResultDataException(string msg) : base(msg) { }
        }

        Data getObjPlaces()
        {
            var numberOfObjs = dataGridView1.Rows.Count;

            if (numberOfObjs > romIF.numberOfObjPlacesMax)
                throw new IndvalidResultDataException("Number of objects is too big");

            var p = new Data(new byte[numberOfObjs * romIF.objPlaceC_size]);

            for (var i = 0; i < numberOfObjs; i++)
            {
                var row = dataGridView1.Rows[i].Cells;
                var id = (uint)parseCellValue((string)row[0].Value, 0);
                var o = (uint)(id * romIF.objPlaceC_size);
                p.setHalf(o + romIF.objPlace_kind,
                    (ushort)(uint)parseCellValue((string)row[1].Value, 1));
                p.setHalf(o + romIF.objPlace_info,
                    (ushort)(uint)parseCellValue((string)row[3].Value, 3));
                p.setFloat(o + romIF.objPlace_xpos,
                    (float)parseCellValue((string)row[4].Value, 4));
                p.setFloat(o + romIF.objPlace_ypos,
                    (float)parseCellValue((string)row[5].Value, 5));
            }
            return p;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (errorCells != 0)
                return;

            try
            {
                objPlaces = getObjPlaces();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (IndvalidResultDataException ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void Insert_Click(object sender, EventArgs e)
        {
            var g = dataGridView1;
            var index =
                g.SelectedCells.Count == 0
                ? g.Rows.Count
                : g.CurrentCell.RowIndex;

            insertRow(index, new object[] { null, "0000", null, "0000", "0", "0" });
        }

        void reassignIDs(int editRowIndex)
        {
            var rows = (
                from DataGridViewRow r in dataGridView1.Rows
                orderby new Tuple<string, int>(
                    (string)r.Cells[0].Value, r.Index)
                select r).ToArray();

            for (var i = 0; i < rows.Length; i++)
                if (rows[i].Index == editRowIndex) {
                    var x = parseCellValue((string)rows[i].Cells[0].Value, 0);
                    if (x != null && (uint)x < rows.Length)
                    {
                        var tmp = rows[i];
                        rows[i] = rows[(uint)x];
                        rows[(uint)x] = tmp;
                    }
                }

            for (var i = 0; i < rows.Length; i++)
                rows[i].Cells[0].Value = i.ToString("x2");

        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
                dataGridView1.BeginEdit(true);
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
                return;
            var index = dataGridView1.CurrentCell.RowIndex;

            for (var i = 0; i < 6; i++)
            {
                if (isErrorCell(dataGridView1.Rows[index].Cells[i]))
                    errorCells--;
            }
            changeErrorCells();

            dataGridView1.Rows.RemoveAt(index);

            reassignIDs(index);
        }

        private void Clone_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null)
                return;
            var index = dataGridView1.CurrentCell.RowIndex;
            var row = dataGridView1.Rows[index];

            insertRow(index, (from DataGridViewCell c in dataGridView1.Rows[index].Cells select c.Value).ToArray());

            for (var i = 0; i < 6; i++)
            {
                if (isErrorCell(row.Cells[i]))
                {
                    dataGridView1.Rows[index].Cells[i].Style.BackColor = errorBackColor;
                    errorCells++;
                }
            }
            changeErrorCells();
        }

        void insertRow(int index, object[] p)
        {
            var g = dataGridView1;
            if (p[0] == null)
            {
                bool idSorted = g.SortedColumn == null || g.SortedColumn.HeaderText == "ID";
                var id = idSorted ? index : g.Rows.Count;
                p[0] = id.ToString("x2");
            }
            p[2] = null;    //Name
            g.Rows.Insert(index, p);
            reassignIDs(index);
        }

        object parseCellValue(string s, int column)
        {
            if (s == null)
                return null;
            try
            {
                switch (column)
                {
                    case 0:
                        return s.parseHex(0xff);
                    case 1:
                    case 3:
                        return s.parseHex(0xffff);
                    case 4:
                    case 5:
                        return Util.floatFromHexString(s);
                    default:
                        throw new Exception();
                }
            }
            catch (FormatException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }
        }

        readonly Color errorBackColor = Color.FromArgb(0xff, 0xcc, 0xcc);
        int errorCells = 0;

        bool isErrorCell(DataGridViewCell cell)
        {
            return cell.Style.BackColor == errorBackColor;
        }

        string formatCellValue(object o, int index)
        {
            switch (index)
            {
                case 0:
                    return ((uint)o).ToString("x2");
                case 1:
                case 3:
                    return ((uint)o).ToString("x4");
                case 4:
                case 5:
                    return Util.floatToHexString((float)o);
                default:
                    throw new Exception();
            }
        }

        void changeErrorCells()
        {
            OK.Enabled = errorCells == 0;
        }

        void updateCellValue(DataGridViewCell cell)
        {
            if (cell.ColumnIndex == 2)
            {
                var kindCell = dataGridView1.Rows[cell.RowIndex].Cells[1];

                if (cell.Value == null)
                    cell.Value = unknownKindValue;

                if ((uint)cell.Value != unknownKindValue)
                {
                    kindCell.Value = ((uint)cell.Value).ToString("x4");
                    updateCellValue(kindCell);

                }
                else
                {
                    var kind = parseCellValue((string)kindCell.Value, kindCell.ColumnIndex);
                    if (kind != null && objNameMap.ContainsKey((uint)kind))
                    {
                        cell.Value = (uint)kind;
                    }
                }
                return;
            }

            var wasError = isErrorCell(cell);
            var value = parseCellValue((string)cell.Value, cell.ColumnIndex);
            var isError = value == null && cell.ColumnIndex != 0;
            if (wasError != isError)
            {
                if (isError)
                {
                    cell.Style.BackColor = errorBackColor;
                    errorCells++;
                }
                else
                {
                    cell.Style.BackColor = Color.White;
                    errorCells--;
                }
            }

            changeErrorCells();

            if (value != null)
                cell.Value = formatCellValue(value, cell.ColumnIndex);

            if (cell.ColumnIndex == 0)
                reassignIDs(cell.RowIndex);

            if (cell.ColumnIndex == 1)
                dataGridView1.Rows[cell.RowIndex].Cells[2].Value =
                    value != null && objNameMap.ContainsKey((uint)value) ? value : unknownKindValue;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            updateCellValue(cell);
        }



        private void EditObjectList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
            {
                Data p = null;
                try
                {
                    p = getObjPlaces();
                }
                catch (Exception) { }
                if (p == null || !p.bytes.SequenceEqual(firstObjPlaces.bytes))
                {
                    if (MessageBox.Show(this, "Close?", "Close?", MessageBoxButtons.YesNo) == DialogResult.No)
                        e.Cancel = true;
                }
            }
            
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 4 || e.Column.Index == 5)
            {
                var x = ((string)e.CellValue1).tryFloatFromHexString();
                var y = ((string)e.CellValue2).tryFloatFromHexString();
                if (x == null && y == null)
                    e.SortResult = 0;
                else if (x == null)
                    e.SortResult = -1;
                else if (y == null)
                    e.SortResult = 1;
                else
                    e.SortResult = x.Value.CompareTo(y.Value);
                e.Handled = true;
            }
        }

    }
}
