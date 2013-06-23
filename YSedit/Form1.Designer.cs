namespace YSedit
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMapNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editObjectModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editUStruct3ManualyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editObjectListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.change8xxxObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editBlocksInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.objectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autmaticSetKindListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.infoStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainViewPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mapEditToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.utilToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(384, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.closeROMToolStripMenuItem,
            this.openMapNumberToolStripMenuItem,
            this.saveMapToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(36, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openROMToolStripMenuItem.Text = "&Open ROM...";
            this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // closeROMToolStripMenuItem
            // 
            this.closeROMToolStripMenuItem.Name = "closeROMToolStripMenuItem";
            this.closeROMToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.closeROMToolStripMenuItem.Text = "&Close ROM";
            this.closeROMToolStripMenuItem.Click += new System.EventHandler(this.closeROMToolStripMenuItem_Click);
            // 
            // openMapNumberToolStripMenuItem
            // 
            this.openMapNumberToolStripMenuItem.Name = "openMapNumberToolStripMenuItem";
            this.openMapNumberToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openMapNumberToolStripMenuItem.Text = "Open &Map Number...";
            this.openMapNumberToolStripMenuItem.Click += new System.EventHandler(this.openMapNumberToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Enabled = false;
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.saveMapToolStripMenuItem.Text = "&Save Map to ROM";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // mapEditToolStripMenuItem
            // 
            this.mapEditToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editObjectModeToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripSeparator1,
            this.editUStruct3ManualyToolStripMenuItem,
            this.toolStripSeparator2,
            this.addObjectToolStripMenuItem,
            this.editObjectListToolStripMenuItem,
            this.toolStripSeparator3,
            this.change8xxxObjectToolStripMenuItem,
            this.editBlocksInfoToolStripMenuItem});
            this.mapEditToolStripMenuItem.Name = "mapEditToolStripMenuItem";
            this.mapEditToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.mapEditToolStripMenuItem.Text = "Map &Edit";
            // 
            // editObjectModeToolStripMenuItem
            // 
            this.editObjectModeToolStripMenuItem.Enabled = false;
            this.editObjectModeToolStripMenuItem.Name = "editObjectModeToolStripMenuItem";
            this.editObjectModeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editObjectModeToolStripMenuItem.Text = "Edit &Objects Mode";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Enabled = false;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editToolStripMenuItem.Text = "Edit &Blocks Mode";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(204, 6);
            // 
            // editUStruct3ManualyToolStripMenuItem
            // 
            this.editUStruct3ManualyToolStripMenuItem.Enabled = false;
            this.editUStruct3ManualyToolStripMenuItem.Name = "editUStruct3ManualyToolStripMenuItem";
            this.editUStruct3ManualyToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editUStruct3ManualyToolStripMenuItem.Text = "Edit &Map Info Manually...";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(204, 6);
            // 
            // addObjectToolStripMenuItem
            // 
            this.addObjectToolStripMenuItem.Enabled = false;
            this.addObjectToolStripMenuItem.Name = "addObjectToolStripMenuItem";
            this.addObjectToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.addObjectToolStripMenuItem.Text = "&Add Object...";
            // 
            // editObjectListToolStripMenuItem
            // 
            this.editObjectListToolStripMenuItem.Name = "editObjectListToolStripMenuItem";
            this.editObjectListToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editObjectListToolStripMenuItem.Text = "Edit Object &List...";
            this.editObjectListToolStripMenuItem.Click += new System.EventHandler(this.editObjectListToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(204, 6);
            // 
            // change8xxxObjectToolStripMenuItem
            // 
            this.change8xxxObjectToolStripMenuItem.Enabled = false;
            this.change8xxxObjectToolStripMenuItem.Name = "change8xxxObjectToolStripMenuItem";
            this.change8xxxObjectToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.change8xxxObjectToolStripMenuItem.Text = "&Change 8xxx Objects...";
            // 
            // editBlocksInfoToolStripMenuItem
            // 
            this.editBlocksInfoToolStripMenuItem.Enabled = false;
            this.editBlocksInfoToolStripMenuItem.Name = "editBlocksInfoToolStripMenuItem";
            this.editBlocksInfoToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.editBlocksInfoToolStripMenuItem.Text = "Edit &Blocks Info Manually...";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapInformationToolStripMenuItem,
            this.toolStripSeparator4,
            this.objectsToolStripMenuItem,
            this.visualToolStripMenuItem,
            this.collisionToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // mapInformationToolStripMenuItem
            // 
            this.mapInformationToolStripMenuItem.Name = "mapInformationToolStripMenuItem";
            this.mapInformationToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.mapInformationToolStripMenuItem.Text = "Map &Information...";
            this.mapInformationToolStripMenuItem.Click += new System.EventHandler(this.mapInformationToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(155, 6);
            // 
            // objectsToolStripMenuItem
            // 
            this.objectsToolStripMenuItem.Checked = true;
            this.objectsToolStripMenuItem.CheckOnClick = true;
            this.objectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.objectsToolStripMenuItem.Enabled = false;
            this.objectsToolStripMenuItem.Name = "objectsToolStripMenuItem";
            this.objectsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.objectsToolStripMenuItem.Text = "&Objects";
            // 
            // visualToolStripMenuItem
            // 
            this.visualToolStripMenuItem.Checked = true;
            this.visualToolStripMenuItem.CheckOnClick = true;
            this.visualToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.visualToolStripMenuItem.Enabled = false;
            this.visualToolStripMenuItem.Name = "visualToolStripMenuItem";
            this.visualToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.visualToolStripMenuItem.Text = "Block &Visual";
            // 
            // collisionToolStripMenuItem
            // 
            this.collisionToolStripMenuItem.CheckOnClick = true;
            this.collisionToolStripMenuItem.Enabled = false;
            this.collisionToolStripMenuItem.Name = "collisionToolStripMenuItem";
            this.collisionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.collisionToolStripMenuItem.Text = "Block &Collision";
            // 
            // utilToolStripMenuItem
            // 
            this.utilToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autmaticSetKindListToolStripMenuItem});
            this.utilToolStripMenuItem.Name = "utilToolStripMenuItem";
            this.utilToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.utilToolStripMenuItem.Text = "&Option";
            // 
            // autmaticSetKindListToolStripMenuItem
            // 
            this.autmaticSetKindListToolStripMenuItem.Checked = true;
            this.autmaticSetKindListToolStripMenuItem.CheckOnClick = true;
            this.autmaticSetKindListToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autmaticSetKindListToolStripMenuItem.Enabled = false;
            this.autmaticSetKindListToolStripMenuItem.Name = "autmaticSetKindListToolStripMenuItem";
            this.autmaticSetKindListToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.autmaticSetKindListToolStripMenuItem.Text = "&Autmatic Setting Kind List";
            // 
            // openROMFileDialog
            // 
            this.openROMFileDialog.Filter = "ROM files|*.z64|All files|*.*";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoStatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 310);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(384, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Resize += new System.EventHandler(this.statusStrip1_Resize);
            // 
            // infoStatusText
            // 
            this.infoStatusText.Name = "infoStatusText";
            this.infoStatusText.Size = new System.Drawing.Size(369, 17);
            this.infoStatusText.Spring = true;
            this.infoStatusText.Text = "The Status Strip Text";
            this.infoStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.infoStatusText.ToolTipText = "Ready";
            // 
            // mainViewPanel
            // 
            this.mainViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainViewPanel.Location = new System.Drawing.Point(0, 24);
            this.mainViewPanel.Name = "mainViewPanel";
            this.mainViewPanel.Size = new System.Drawing.Size(384, 286);
            this.mainViewPanel.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 332);
            this.Controls.Add(this.mainViewPanel);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "YSedit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMapNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapEditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editObjectModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUStruct3ManualyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem change8xxxObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editBlocksInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openROMFileDialog;
        private System.Windows.Forms.ToolStripMenuItem closeROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editObjectListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autmaticSetKindListToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel infoStatusText;
        private System.Windows.Forms.ToolStripMenuItem mapInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Panel mainViewPanel;
    }
}

