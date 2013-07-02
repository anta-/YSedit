namespace YSedit
{
    partial class EditPreparedList
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
            this.label1 = new System.Windows.Forms.Label();
            this.additionalIDs = new System.Windows.Forms.TextBox();
            this.resolvedIDs = new System.Windows.Forms.TextBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(155, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Additional IDs: (comma sepalated)";
            // 
            // additionalIDs
            // 
            this.additionalIDs.Location = new System.Drawing.Point(157, 24);
            this.additionalIDs.Multiline = true;
            this.additionalIDs.Name = "additionalIDs";
            this.additionalIDs.Size = new System.Drawing.Size(137, 169);
            this.additionalIDs.TabIndex = 7;
            // 
            // resolvedIDs
            // 
            this.resolvedIDs.Location = new System.Drawing.Point(12, 24);
            this.resolvedIDs.Multiline = true;
            this.resolvedIDs.Name = "resolvedIDs";
            this.resolvedIDs.ReadOnly = true;
            this.resolvedIDs.Size = new System.Drawing.Size(137, 169);
            this.resolvedIDs.TabIndex = 9;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(306, 161);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(71, 32);
            this.Cancel.TabIndex = 11;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(306, 123);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(71, 32);
            this.OK.TabIndex = 10;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Resolved IDs:";
            // 
            // EditPreparedList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 205);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.resolvedIDs);
            this.Controls.Add(this.additionalIDs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditPreparedList";
            this.Text = "EditPreparedList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditPreparedList_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox additionalIDs;
        private System.Windows.Forms.TextBox resolvedIDs;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Label label3;

    }
}