namespace Aurora
{
	namespace NiftySolution
	{
		partial class ConfigDialog
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
				this.button1 = new System.Windows.Forms.Button();
				this.button2 = new System.Windows.Forms.Button();
				this.groupBox2 = new System.Windows.Forms.GroupBox();
				this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
				this.groupBox2.SuspendLayout();
				this.SuspendLayout();
				// 
				// button1
				// 
				this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.button1.Location = new System.Drawing.Point(70, 442);
				this.button1.Name = "button1";
				this.button1.Size = new System.Drawing.Size(75, 23);
				this.button1.TabIndex = 3;
				this.button1.Text = "Ok";
				this.button1.UseVisualStyleBackColor = true;
				this.button1.Click += new System.EventHandler(this.button1_Click);
				// 
				// button2
				// 
				this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.button2.Location = new System.Drawing.Point(177, 442);
				this.button2.Name = "button2";
				this.button2.Size = new System.Drawing.Size(75, 23);
				this.button2.TabIndex = 4;
				this.button2.Text = "Cancel";
				this.button2.UseVisualStyleBackColor = true;
				this.button2.Click += new System.EventHandler(this.button2_Click);
				// 
				// groupBox2
				// 
				this.groupBox2.Controls.Add(this.propertyGrid1);
				this.groupBox2.Location = new System.Drawing.Point(12, 12);
				this.groupBox2.Name = "groupBox2";
				this.groupBox2.Size = new System.Drawing.Size(299, 400);
				this.groupBox2.TabIndex = 9;
				this.groupBox2.TabStop = false;
				this.groupBox2.Text = "Settings";
				// 
				// propertyGrid1
				// 
				this.propertyGrid1.Location = new System.Drawing.Point(6, 19);
				this.propertyGrid1.Name = "propertyGrid1";
				this.propertyGrid1.Size = new System.Drawing.Size(287, 375);
				this.propertyGrid1.TabIndex = 0;
				this.propertyGrid1.Click += new System.EventHandler(this.propertyGrid1_Click);
				// 
				// ConfigDialog
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(323, 477);
				this.Controls.Add(this.groupBox2);
				this.Controls.Add(this.button2);
				this.Controls.Add(this.button1);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.Name = "ConfigDialog";
				this.Text = "NiftySolution configuration";
				this.groupBox2.ResumeLayout(false);
				this.ResumeLayout(false);

			}

			#endregion

			private System.Windows.Forms.Button button1;
			private System.Windows.Forms.Button button2;
			private System.Windows.Forms.GroupBox groupBox2;
			public System.Windows.Forms.PropertyGrid propertyGrid1;
		}
	}

}
