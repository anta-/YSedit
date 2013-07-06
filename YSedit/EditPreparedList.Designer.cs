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
            this.label2 = new System.Windows.Forms.Label();
            this.newResolvedIDs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.information = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(296, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Additional IDs: (comma sepalated)";
            // 
            // additionalIDs
            // 
            this.additionalIDs.Location = new System.Drawing.Point(298, 24);
            this.additionalIDs.Multiline = true;
            this.additionalIDs.Name = "additionalIDs";
            this.additionalIDs.Size = new System.Drawing.Size(137, 240);
            this.additionalIDs.TabIndex = 7;
            // 
            // resolvedIDs
            // 
            this.resolvedIDs.Location = new System.Drawing.Point(12, 24);
            this.resolvedIDs.Multiline = true;
            this.resolvedIDs.Name = "resolvedIDs";
            this.resolvedIDs.ReadOnly = true;
            this.resolvedIDs.Size = new System.Drawing.Size(137, 240);
            this.resolvedIDs.TabIndex = 9;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(457, 231);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(71, 32);
            this.Cancel.TabIndex = 11;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(457, 193);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "New resolved IDs:";
            // 
            // newResolvedIDs
            // 
            this.newResolvedIDs.Location = new System.Drawing.Point(155, 24);
            this.newResolvedIDs.Multiline = true;
            this.newResolvedIDs.Name = "newResolvedIDs";
            this.newResolvedIDs.ReadOnly = true;
            this.newResolvedIDs.Size = new System.Drawing.Size(137, 240);
            this.newResolvedIDs.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(532, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "Resolve information:";
            // 
            // information
            // 
            this.information.Location = new System.Drawing.Point(534, 24);
            this.information.Multiline = true;
            this.information.Name = "information";
            this.information.ReadOnly = true;
            this.information.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.information.Size = new System.Drawing.Size(107, 240);
            this.information.TabIndex = 7;
            // 
            // EditPreparedList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 275);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.newResolvedIDs);
            this.Controls.Add(this.resolvedIDs);
            this.Controls.Add(this.information);
            this.Controls.Add(this.additionalIDs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditPreparedList";
            this.Text = "EditPreparedList";
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox newResolvedIDs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox information;

    }
}