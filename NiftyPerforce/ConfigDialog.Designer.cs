namespace Aurora
{

	namespace NiftyPerforce
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
				this.checkBox1 = new System.Windows.Forms.CheckBox();
				this.checkBox2 = new System.Windows.Forms.CheckBox();
				this.checkBox3 = new System.Windows.Forms.CheckBox();
				this.groupBox1 = new System.Windows.Forms.GroupBox();
				this.groupBox1.SuspendLayout();
				this.SuspendLayout();
				// 
				// button1
				// 
				this.button1.Location = new System.Drawing.Point(12, 166);
				this.button1.Name = "button1";
				this.button1.Size = new System.Drawing.Size(75, 23);
				this.button1.TabIndex = 3;
				this.button1.Text = "Ok";
				this.button1.UseVisualStyleBackColor = true;
				this.button1.Click += new System.EventHandler(this.button1_Click);
				// 
				// button2
				// 
				this.button2.Location = new System.Drawing.Point(119, 166);
				this.button2.Name = "button2";
				this.button2.Size = new System.Drawing.Size(75, 23);
				this.button2.TabIndex = 4;
				this.button2.Text = "Cancel";
				this.button2.UseVisualStyleBackColor = true;
				this.button2.Click += new System.EventHandler(this.button2_Click);
				// 
				// checkBox1
				// 
				this.checkBox1.AutoSize = true;
				this.checkBox1.Location = new System.Drawing.Point(19, 19);
				this.checkBox1.Name = "checkBox1";
				this.checkBox1.Size = new System.Drawing.Size(159, 17);
				this.checkBox1.TabIndex = 5;
				this.checkBox1.Text = "Automagic checkout on edit";
				this.checkBox1.UseVisualStyleBackColor = true;
				// 
				// checkBox2
				// 
				this.checkBox2.AutoSize = true;
				this.checkBox2.Location = new System.Drawing.Point(19, 42);
				this.checkBox2.Name = "checkBox2";
				this.checkBox2.Size = new System.Drawing.Size(97, 17);
				this.checkBox2.TabIndex = 6;
				this.checkBox2.Text = "Automagic add";
				this.checkBox2.UseVisualStyleBackColor = true;
				// 
				// checkBox3
				// 
				this.checkBox3.AutoSize = true;
				this.checkBox3.Location = new System.Drawing.Point(19, 65);
				this.checkBox3.Name = "checkBox3";
				this.checkBox3.Size = new System.Drawing.Size(114, 17);
				this.checkBox3.TabIndex = 7;
				this.checkBox3.Text = "Automagic remove";
				this.checkBox3.UseVisualStyleBackColor = true;
				// 
				// groupBox1
				// 
				this.groupBox1.Controls.Add(this.checkBox2);
				this.groupBox1.Controls.Add(this.checkBox3);
				this.groupBox1.Controls.Add(this.checkBox1);
				this.groupBox1.Location = new System.Drawing.Point(12, 12);
				this.groupBox1.Name = "groupBox1";
				this.groupBox1.Size = new System.Drawing.Size(182, 101);
				this.groupBox1.TabIndex = 8;
				this.groupBox1.TabStop = false;
				this.groupBox1.Text = "Settings";
				// 
				// ConfigDialog
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(206, 201);
				this.Controls.Add(this.groupBox1);
				this.Controls.Add(this.button2);
				this.Controls.Add(this.button1);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.Name = "ConfigDialog";
				this.Text = "NiftyPerforce configuration";
				this.groupBox1.ResumeLayout(false);
				this.groupBox1.PerformLayout();
				this.ResumeLayout(false);

			}

			#endregion

			public bool wasCancelled = true;
			private System.Windows.Forms.Button button1;
			private System.Windows.Forms.Button button2;
			public System.Windows.Forms.CheckBox checkBox1;
			public System.Windows.Forms.CheckBox checkBox2;
			public System.Windows.Forms.CheckBox checkBox3;
			private System.Windows.Forms.GroupBox groupBox1;
		}
	}

}
