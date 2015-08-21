namespace Recorder
{
    partial class Recorder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Recorder));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.RecordBtn = new System.Windows.Forms.ToolStripButton();
            this.SaveRecordingBtn = new System.Windows.Forms.ToolStripButton();
            this.SaveAsBtn = new System.Windows.Forms.ToolStripButton();
            this.OpenRecordingBtn = new System.Windows.Forms.ToolStripButton();
            this.PlayBackBtn = new System.Windows.Forms.ToolStripButton();
            this.EditRecordingBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RecordBtn,
            this.SaveRecordingBtn,
            this.SaveAsBtn,
            this.OpenRecordingBtn,
            this.PlayBackBtn,
            this.EditRecordingBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(308, 34);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // RecordBtn
            // 
            this.RecordBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RecordBtn.Image = global::Recorder.Properties.Resources.offIcon;
            this.RecordBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RecordBtn.Name = "RecordBtn";
            this.RecordBtn.Size = new System.Drawing.Size(23, 31);
            this.RecordBtn.Text = "Start New Recording";
            this.RecordBtn.Click += new System.EventHandler(this.RecordBtn_Click);
            // 
            // SaveRecordingBtn
            // 
            this.SaveRecordingBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveRecordingBtn.Image = global::Recorder.Properties.Resources.Programming_Save_icon;
            this.SaveRecordingBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveRecordingBtn.Name = "SaveRecordingBtn";
            this.SaveRecordingBtn.Size = new System.Drawing.Size(23, 31);
            this.SaveRecordingBtn.Text = "Save Recording";
            this.SaveRecordingBtn.Click += new System.EventHandler(this.SaveRecordingBtn_Click);
            // 
            // SaveAsBtn
            // 
            this.SaveAsBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsBtn.Image = global::Recorder.Properties.Resources.Programming_Save_As_icon;
            this.SaveAsBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsBtn.Name = "SaveAsBtn";
            this.SaveAsBtn.Size = new System.Drawing.Size(23, 31);
            this.SaveAsBtn.Text = "Save As...";
            this.SaveAsBtn.Click += new System.EventHandler(this.SaveAsBtn_Click);
            // 
            // OpenRecordingBtn
            // 
            this.OpenRecordingBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenRecordingBtn.Image = ((System.Drawing.Image)(resources.GetObject("OpenRecordingBtn.Image")));
            this.OpenRecordingBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenRecordingBtn.Name = "OpenRecordingBtn";
            this.OpenRecordingBtn.Size = new System.Drawing.Size(23, 31);
            this.OpenRecordingBtn.Text = "Open Recording";
            this.OpenRecordingBtn.Click += new System.EventHandler(this.OpenFileBtn_Click);
            // 
            // PlayBackBtn
            // 
            this.PlayBackBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PlayBackBtn.Image = global::Recorder.Properties.Resources.play_icon;
            this.PlayBackBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PlayBackBtn.Name = "PlayBackBtn";
            this.PlayBackBtn.Size = new System.Drawing.Size(23, 31);
            this.PlayBackBtn.Text = "Play Back";
            this.PlayBackBtn.Click += new System.EventHandler(this.PlayBackBtn_Click);
            // 
            // EditRecordingBtn
            // 
            this.EditRecordingBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.EditRecordingBtn.Image = ((System.Drawing.Image)(resources.GetObject("EditRecordingBtn.Image")));
            this.EditRecordingBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EditRecordingBtn.Name = "EditRecordingBtn";
            this.EditRecordingBtn.Size = new System.Drawing.Size(23, 31);
            this.EditRecordingBtn.Text = "Edit Recording";
            this.EditRecordingBtn.Click += new System.EventHandler(this.EditRecordingBtn_Click);
            // 
            // Recorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 34);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Recorder";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Recorder_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton RecordBtn;
        private System.Windows.Forms.ToolStripButton OpenRecordingBtn;
        private System.Windows.Forms.ToolStripButton PlayBackBtn;
        private System.Windows.Forms.ToolStripButton SaveRecordingBtn;
        private System.Windows.Forms.ToolStripButton SaveAsBtn;
        private System.Windows.Forms.ToolStripButton EditRecordingBtn;
    }
}

