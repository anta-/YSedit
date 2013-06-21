namespace YSedit
{
    partial class EditObjectList
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Kind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ObjName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Info = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PosY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Insert = new System.Windows.Forms.Button();
            this.Remove = new System.Windows.Forms.Button();
            this.Clone = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Kind,
            this.ObjName,
            this.Info,
            this.PosX,
            this.PosY});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(498, 297);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.dataGridView1_CurrentCellChanged);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.MaxInputLength = 2;
            this.ID.Name = "ID";
            this.ID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ID.Width = 20;
            // 
            // Kind
            // 
            this.Kind.HeaderText = "Kind";
            this.Kind.MaxInputLength = 4;
            this.Kind.Name = "Kind";
            this.Kind.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Kind.Width = 30;
            // 
            // ObjName
            // 
            this.ObjName.HeaderText = "Name";
            this.ObjName.Name = "ObjName";
            this.ObjName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ObjName.Width = 200;
            // 
            // Info
            // 
            this.Info.HeaderText = "Info";
            this.Info.MaxInputLength = 4;
            this.Info.Name = "Info";
            this.Info.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Info.Width = 30;
            // 
            // PosX
            // 
            this.PosX.HeaderText = "PosX";
            this.PosX.Name = "PosX";
            this.PosX.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // PosY
            // 
            this.PosY.HeaderText = "PosY";
            this.PosY.Name = "PosY";
            this.PosY.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(504, 164);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(71, 32);
            this.OK.TabIndex = 5;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(504, 202);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(71, 32);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Insert
            // 
            this.Insert.Location = new System.Drawing.Point(504, 12);
            this.Insert.Name = "Insert";
            this.Insert.Size = new System.Drawing.Size(71, 32);
            this.Insert.TabIndex = 1;
            this.Insert.Text = "Insert";
            this.Insert.UseVisualStyleBackColor = true;
            this.Insert.Click += new System.EventHandler(this.Insert_Click);
            // 
            // Remove
            // 
            this.Remove.Location = new System.Drawing.Point(504, 50);
            this.Remove.Name = "Remove";
            this.Remove.Size = new System.Drawing.Size(71, 32);
            this.Remove.TabIndex = 2;
            this.Remove.Text = "Remove";
            this.Remove.UseVisualStyleBackColor = true;
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // Clone
            // 
            this.Clone.Location = new System.Drawing.Point(504, 88);
            this.Clone.Name = "Clone";
            this.Clone.Size = new System.Drawing.Size(71, 32);
            this.Clone.TabIndex = 3;
            this.Clone.Text = "Clone";
            this.Clone.UseVisualStyleBackColor = true;
            this.Clone.Click += new System.EventHandler(this.Clone_Click);
            // 
            // EditObjectList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 297);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Clone);
            this.Controls.Add(this.Remove);
            this.Controls.Add(this.Insert);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.dataGridView1);
            this.MaximumSize = new System.Drawing.Size(592, 10000);
            this.MinimumSize = new System.Drawing.Size(592, 34);
            this.Name = "EditObjectList";
            this.Text = "EditObjectList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditObjectList_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Insert;
        private System.Windows.Forms.Button Remove;
        private System.Windows.Forms.Button Clone;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Kind;
        private System.Windows.Forms.DataGridViewComboBoxColumn ObjName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info;
        private System.Windows.Forms.DataGridViewTextBoxColumn PosX;
        private System.Windows.Forms.DataGridViewTextBoxColumn PosY;


    }
}