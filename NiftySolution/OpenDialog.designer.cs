namespace Aurora
{

	namespace NiftySolution
	{
		partial class OpenDialog
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
				this.m_listView = new System.Windows.Forms.ListView();
				this.statusStrip1 = new System.Windows.Forms.StatusStrip();
				this.m_toolStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
				this.statusStrip1.SuspendLayout();
				this.SuspendLayout();
				// 
				// m_listView
				// 
				this.m_listView.AllowColumnReorder = true;
				this.m_listView.Dock = System.Windows.Forms.DockStyle.Fill;
				this.m_listView.Location = new System.Drawing.Point(0, 0);
				this.m_listView.MultiSelect = false;
				this.m_listView.Name = "m_listView";
				this.m_listView.Size = new System.Drawing.Size(482, 279);
				this.m_listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
				this.m_listView.TabIndex = 0;
				this.m_listView.UseCompatibleStateImageBehavior = false;
				this.m_listView.View = System.Windows.Forms.View.List;
				this.m_listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_listView_MouseDoubleClick);
				this.m_listView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.m_listView_KeyPress);
				this.m_listView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_listView_KeyUp);
				// 
				// statusStrip1
				// 
				this.statusStrip1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_toolStripLabel});
				this.statusStrip1.Location = new System.Drawing.Point(0, 257);
				this.statusStrip1.Name = "statusStrip1";
				this.statusStrip1.Size = new System.Drawing.Size(482, 22);
				this.statusStrip1.TabIndex = 1;
				this.statusStrip1.Text = "statusStrip1";
				// 
				// m_toolStripLabel
				// 
				this.m_toolStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				this.m_toolStripLabel.Name = "m_toolStripLabel";
				this.m_toolStripLabel.Size = new System.Drawing.Size(15, 17);
				this.m_toolStripLabel.Text = "\"\"";
				// 
				// OpenDialog
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(482, 279);
				this.ControlBox = false;
				this.Controls.Add(this.statusStrip1);
				this.Controls.Add(this.m_listView);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "OpenDialog";
				this.ShowIcon = false;
				this.ShowInTaskbar = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
				this.Text = "Search for file (F5 refresh files)";
				this.Activated += new System.EventHandler(this.OpenDialog_Activated);
				this.statusStrip1.ResumeLayout(false);
				this.statusStrip1.PerformLayout();
				this.ResumeLayout(false);
				this.PerformLayout();

			}

			#endregion

			private System.Windows.Forms.ListView m_listView;
			private System.Windows.Forms.StatusStrip statusStrip1;
			private System.Windows.Forms.ToolStripStatusLabel m_toolStripLabel;
		}
	}
}
