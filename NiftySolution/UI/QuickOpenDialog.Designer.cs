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
            this.mStatuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mStatuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusLabel,
            this.mSelectionInfo});
            this.mStatuStrip.Location = new System.Drawing.Point(0, 892);
            this.mStatuStrip.Name = "mStatuStrip";
            this.mStatuStrip.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.mStatuStrip.Size = new System.Drawing.Size(1174, 37);
            this.mStatuStrip.TabIndex = 0;
            this.mStatuStrip.Text = "statusStrip1";
            // 
            // mStatusLabel
            // 
            this.mStatusLabel.Name = "mStatusLabel";
            this.mStatusLabel.Size = new System.Drawing.Size(71, 32);
            this.mStatusLabel.Text = "Hello";
            // 
            // mSelectionInfo
            // 
            this.mSelectionInfo.Name = "mSelectionInfo";
            this.mSelectionInfo.Size = new System.Drawing.Size(57, 32);
            this.mSelectionInfo.Text = "Info";
            // 
            // mInputText
            // 
            this.mInputText.Dock = System.Windows.Forms.DockStyle.Top;
            this.mInputText.Location = new System.Drawing.Point(0, 0);
            this.mInputText.Margin = new System.Windows.Forms.Padding(6);
            this.mInputText.Name = "mInputText";
            this.mInputText.Size = new System.Drawing.Size(1174, 31);
            this.mInputText.TabIndex = 0;
            this.mInputText.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.mInputText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // mSearchResults
            // 
            this.mSearchResults.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.mSearchResults.CausesValidation = false;
            this.mSearchResults.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mSearchResults.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.mSearchResults.Location = new System.Drawing.Point(0, 110);
            this.mSearchResults.Margin = new System.Windows.Forms.Padding(6);
            this.mSearchResults.Name = "mSearchResults";
            this.mSearchResults.Size = new System.Drawing.Size(1174, 782);
            this.mSearchResults.TabIndex = 1;
            this.mSearchResults.Resize += new System.EventHandler(this.searchResultControl1_Resize);
            // 
            // QuickOpenDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 929);
            this.ControlBox = false;
            this.Controls.Add(this.mSearchResults);
            this.Controls.Add(this.mInputText);
            this.Controls.Add(this.mStatuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
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
