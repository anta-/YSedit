namespace YSedit
{
    partial class ObjectSelector
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
            this.objectImageView = new System.Windows.Forms.PictureBox();
            this.objectSelect = new System.Windows.Forms.ListBox();
            this.objectGroupSelect = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.objectDescription = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.objectImageView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectImageView
            // 
            this.objectImageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectImageView.Location = new System.Drawing.Point(0, 0);
            this.objectImageView.Margin = new System.Windows.Forms.Padding(0);
            this.objectImageView.Name = "objectImageView";
            this.objectImageView.Size = new System.Drawing.Size(256, 234);
            this.objectImageView.TabIndex = 0;
            this.objectImageView.TabStop = false;
            // 
            // objectSelect
            // 
            this.objectSelect.FormattingEnabled = true;
            this.objectSelect.ItemHeight = 12;
            this.objectSelect.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d"});
            this.objectSelect.Location = new System.Drawing.Point(0, 20);
            this.objectSelect.Margin = new System.Windows.Forms.Padding(0);
            this.objectSelect.Name = "objectSelect";
            this.objectSelect.ScrollAlwaysVisible = true;
            this.objectSelect.Size = new System.Drawing.Size(256, 124);
            this.objectSelect.TabIndex = 1;
            // 
            // objectGroupSelect
            // 
            this.objectGroupSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectGroupSelect.FormattingEnabled = true;
            this.objectGroupSelect.Location = new System.Drawing.Point(0, 0);
            this.objectGroupSelect.Margin = new System.Windows.Forms.Padding(0);
            this.objectGroupSelect.Name = "objectGroupSelect";
            this.objectGroupSelect.Size = new System.Drawing.Size(256, 20);
            this.objectGroupSelect.TabIndex = 2;
            this.objectGroupSelect.SelectedIndexChanged += new System.EventHandler(this.objectGroupSelect_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectImageView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(256, 468);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.objectDescription);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.objectGroupSelect);
            this.splitContainer2.Panel2.Controls.Add(this.objectSelect);
            this.splitContainer2.Panel2.Resize += new System.EventHandler(this.splitContainer2_Panel2_Resize);
            this.splitContainer2.Size = new System.Drawing.Size(256, 232);
            this.splitContainer2.SplitterDistance = 79;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 3;
            // 
            // objectDescription
            // 
            this.objectDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectDescription.Location = new System.Drawing.Point(0, 0);
            this.objectDescription.Margin = new System.Windows.Forms.Padding(0);
            this.objectDescription.Multiline = true;
            this.objectDescription.Name = "objectDescription";
            this.objectDescription.ReadOnly = true;
            this.objectDescription.Size = new System.Drawing.Size(256, 79);
            this.objectDescription.TabIndex = 0;
            // 
            // ObjectSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 468);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ObjectSelector";
            this.Text = "ObjectSelector";
            ((System.ComponentModel.ISupportInitialize)(this.objectImageView)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox objectImageView;
        private System.Windows.Forms.ListBox objectSelect;
        private System.Windows.Forms.ComboBox objectGroupSelect;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox objectDescription;
    }
}