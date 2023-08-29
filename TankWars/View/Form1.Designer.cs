//Author: Yanzheng Wu and Qingwen Bao
//University of Utah
//Date: 2021/04/09
namespace TankWars
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.entered_server_box = new System.Windows.Forms.TextBox();
            this.entered_name_box = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "serve:";
            // 
            // entered_server_box
            // 
            this.entered_server_box.Location = new System.Drawing.Point(64, 12);
            this.entered_server_box.Name = "entered_server_box";
            this.entered_server_box.Size = new System.Drawing.Size(100, 21);
            this.entered_server_box.TabIndex = 1;
            // 
            // entered_name_box
            // 
            this.entered_name_box.Location = new System.Drawing.Point(264, 12);
            this.entered_name_box.Name = "entered_name_box";
            this.entered_name_box.Size = new System.Drawing.Size(100, 21);
            this.entered_name_box.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(209, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "name:";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(417, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.Text = "connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.entered_name_box);
            this.Controls.Add(this.entered_server_box);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Tank Wars";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox entered_server_box;
        private System.Windows.Forms.TextBox entered_name_box;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ConnectButton;
        
    }
}

