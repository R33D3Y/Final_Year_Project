namespace Final_Year_Project
{
    partial class Friend_Request
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Delete_Button = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Add_Button = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Delete_Button
            // 
            this.Delete_Button.BackColor = System.Drawing.Color.White;
            this.Delete_Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Delete_Button.Font = new System.Drawing.Font("Candara", 12F);
            this.Delete_Button.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Delete_Button.Location = new System.Drawing.Point(3, 41);
            this.Delete_Button.Name = "Delete_Button";
            this.Delete_Button.Size = new System.Drawing.Size(90, 30);
            this.Delete_Button.TabIndex = 35;
            this.Delete_Button.Text = "Delete";
            this.Delete_Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Delete_Button.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            this.Delete_Button.MouseHover += new System.EventHandler(this.Label_MouseHover);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Candara", 12F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(214, 38);
            this.label5.TabIndex = 36;
            this.label5.Text = "You have a new friend request\r\nfrom @Username!";
            // 
            // Add_Button
            // 
            this.Add_Button.BackColor = System.Drawing.Color.White;
            this.Add_Button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Add_Button.Font = new System.Drawing.Font("Candara", 12F);
            this.Add_Button.ForeColor = System.Drawing.Color.RoyalBlue;
            this.Add_Button.Location = new System.Drawing.Point(127, 41);
            this.Add_Button.Name = "Add_Button";
            this.Add_Button.Size = new System.Drawing.Size(90, 30);
            this.Add_Button.TabIndex = 37;
            this.Add_Button.Text = "Add";
            this.Add_Button.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Add_Button.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            this.Add_Button.MouseHover += new System.EventHandler(this.Label_MouseHover);
            // 
            // Friend_Request
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.Add_Button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Delete_Button);
            this.Name = "Friend_Request";
            this.Size = new System.Drawing.Size(221, 75);
            this.Load += new System.EventHandler(this.Friend_Request_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label Delete_Button;
        public System.Windows.Forms.Label Add_Button;
    }
}
