namespace Aurora
{

namespace NiftySolution
{
	partial class QuickOpenDialog
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
            this.mStatuStrip = new System.Windows.Forms.StatusStrip();
            this.mStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mSelectionInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.mInputText = new System.Windows.Forms.TextBox();
            this.mSearchResults = new Aurora.NiftySolution.SearchResultControl();
            this.mStatuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mStatuStrip
            // 
            this.mStatuStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mStatuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mStatuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mStatuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusLabel,
            this.mSelectionInfo});
            this.mStatuStrip.Location = new System.Drawing.Point(0, 464);
            this.mStatuStrip.Name = "mStatuStrip";
            this.mStatuStrip.Size = new System.Drawing.Size(95, 22);
            this.mStatuStrip.TabIndex = 0;
            this.mStatuStrip.Text = "statusStrip1";
            // 
            // mStatusLabel
            // 
            this.mStatusLabel.Name = "mStatusLabel";
            this.mStatusLabel.Size = new System.Drawing.Size(35, 17);
            this.mStatusLabel.Text = "Hello";
            // 
            // mSelectionInfo
            // 
            this.mSelectionInfo.Name = "mSelectionInfo";
            this.mSelectionInfo.Size = new System.Drawing.Size(28, 17);
            this.mSelectionInfo.Text = "Info";
            // 
            // mInputText
            // 
            this.mInputText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mInputText.Location = new System.Drawing.Point(0, 0);
            this.mInputText.Name = "mInputText";
            this.mInputText.Size = new System.Drawing.Size(589, 20);
            this.mInputText.TabIndex = 0;
            this.mInputText.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.mInputText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // mSearchResults
            // 
            this.mSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.mSearchResults.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.mSearchResults.CausesValidation = false;
            this.mSearchResults.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.mSearchResults.Location = new System.Drawing.Point(0, 26);
            this.mSearchResults.Name = "mSearchResults";
            this.mSearchResults.Size = new System.Drawing.Size(587, 438);
            this.mSearchResults.TabIndex = 1;
            // 
            // QuickOpenDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 483);
            this.ControlBox = false;
            this.Controls.Add(this.mSearchResults);
            this.Controls.Add(this.mInputText);
            this.Controls.Add(this.mStatuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "QuickOpenDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "QuickOpen";
            this.Shown += new System.EventHandler(this.QuickOpenDialog_Shown);
            this.mStatuStrip.ResumeLayout(false);
            this.mStatuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip mStatuStrip;
		private System.Windows.Forms.TextBox mInputText;
		private SearchResultControl mSearchResults;
		private System.Windows.Forms.ToolStripStatusLabel mStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel mSelectionInfo;
	}
}

}
