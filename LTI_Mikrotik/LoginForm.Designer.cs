namespace LTI_Mikrotik
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            label1 = new Label();
            User = new TextBox();
            Password = new TextBox();
            Login = new Button();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            macAddress = new Label();
            running = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(88, 17);
            label1.TabIndex = 2;
            // 
            // User
            // 
            User.Location = new Point(338, 278);
            User.Margin = new Padding(3, 2, 3, 2);
            User.Name = "User";
            User.Size = new Size(110, 23);
            User.TabIndex = 1;
            // 
            // Password
            // 
            Password.Location = new Point(338, 326);
            Password.Margin = new Padding(3, 2, 3, 2);
            Password.Name = "Password";
            Password.Size = new Size(110, 23);
            Password.TabIndex = 3;
            // 
            // Login
            // 
            Login.BackColor = Color.LightSkyBlue;
            Login.FlatAppearance.BorderColor = Color.Black;
            Login.FlatStyle = FlatStyle.Popup;
            Login.ForeColor = SystemColors.ActiveCaptionText;
            Login.Location = new Point(508, 334);
            Login.Margin = new Padding(3, 2, 3, 2);
            Login.Name = "Login";
            Login.Size = new Size(91, 26);
            Login.TabIndex = 4;
            Login.Text = "Login";
            Login.UseVisualStyleBackColor = false;
            Login.Click += Login_Click_1;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.Control;
            pictureBox1.ErrorImage = null;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = null;
            pictureBox1.Location = new Point(255, 98);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(192, 154);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(265, 281);
            label2.Name = "label2";
            label2.Size = new Size(39, 15);
            label2.TabIndex = 6;
            label2.Text = "Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(260, 328);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 7;
            label3.Text = "Password";
            // 
            // macAddress
            // 
            macAddress.AutoSize = true;
            macAddress.Location = new Point(266, 35);
            macAddress.Name = "macAddress";
            macAddress.Size = new Size(0, 15);
            macAddress.TabIndex = 8;
            // 
            // running
            // 
            running.AutoSize = true;
            running.Location = new Point(464, 35);
            running.Name = "running";
            running.Size = new Size(38, 15);
            running.TabIndex = 9;
            running.Text = "label5";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 400);
            Controls.Add(running);
            Controls.Add(macAddress);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(Login);
            Controls.Add(Password);
            Controls.Add(User);
            Controls.Add(label1);
            Margin = new Padding(3, 2, 3, 2);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += Login_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox User;
        private TextBox Password;
        private Button Login;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private Label macAddress;
        private Label running;
    }
}