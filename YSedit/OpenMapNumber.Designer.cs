namespace YSedit
{
    partial class OpenMapNumber
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
            this.mapName = new System.Windows.Forms.ComboBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.mapNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mapName
            // 
            this.mapName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mapName.FormattingEnabled = true;
            this.mapName.Location = new System.Drawing.Point(37, 12);
            this.mapName.Name = "mapName";
            this.mapName.Size = new System.Drawing.Size(225, 20);
            this.mapName.TabIndex = 1;
            this.mapName.SelectionChangeCommitted += new System.EventHandler(this.mapName_SelectionChangeCommitted);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(185, 38);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(77, 27);
            this.Cancel.TabIndex = 3;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(102, 38);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(77, 27);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // mapNumber
            // 
            this.mapNumber.Location = new System.Drawing.Point(12, 13);
            this.mapNumber.Name = "mapNumber";
            this.mapNumber.Size = new System.Drawing.Size(19, 19);
            this.mapNumber.TabIndex = 0;
            this.mapNumber.TextChanged += new System.EventHandler(this.mapNumber_TextChanged);
            // 
            // OpenMapNumber
            // 
            this.AcceptButton = this.OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(274, 74);
            this.Controls.Add(this.mapNumber);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.mapName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OpenMapNumber";
            this.Text = "OpenMapNumber";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mapName;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        public System.Windows.Forms.TextBox mapNumber;
    }
}