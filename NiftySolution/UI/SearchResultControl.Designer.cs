namespace Aurora.NiftySolution
{
	partial class SearchResultControl
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
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// SearchResultControl
			// 
			this.Name = "SearchResultControl";
			this.Size = new System.Drawing.Size(315, 281);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SearchResultControl_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SearchResultControl_MouseClick);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
