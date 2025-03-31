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
            runningLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(101, 23);
            label1.TabIndex = 2;
            // 
            // User
            // 
            User.Location = new Point(386, 371);
            User.Name = "User";
            User.Size = new Size(125, 27);
            User.TabIndex = 1;
            // 
            // Password
            // 
            Password.Location = new Point(386, 435);
            Password.Name = "Password";
            Password.Size = new Size(125, 27);
            Password.TabIndex = 3;
            // 
            // Login
            // 
            Login.BackColor = Color.LightSkyBlue;
            Login.FlatAppearance.BorderColor = Color.Black;
            Login.FlatStyle = FlatStyle.Popup;
            Login.ForeColor = SystemColors.ActiveCaptionText;
            Login.Location = new Point(581, 445);
            Login.Name = "Login";
            Login.Size = new Size(104, 35);
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
            pictureBox1.Location = new Point(291, 131);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(219, 205);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(303, 375);
            label2.Name = "label2";
            label2.Size = new Size(49, 20);
            label2.TabIndex = 6;
            label2.Text = "Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(297, 437);
            label3.Name = "label3";
            label3.Size = new Size(70, 20);
            label3.TabIndex = 7;
            label3.Text = "Password";
            // 
            // macAddress
            // 
            macAddress.AutoSize = true;
            macAddress.Location = new Point(304, 47);
            macAddress.Name = "macAddress";
            macAddress.Size = new Size(0, 20);
            macAddress.TabIndex = 8;
            // 
            // runningLabel
            // 
            runningLabel.AutoSize = true;
            runningLabel.Location = new Point(527, 47);
            runningLabel.Name = "runningLabel";
            runningLabel.Size = new Size(50, 20);
            runningLabel.TabIndex = 9;
            runningLabel.Text = "label5";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 533);
            Controls.Add(runningLabel);
            Controls.Add(macAddress);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(pictureBox1);
            Controls.Add(Login);
            Controls.Add(Password);
            Controls.Add(User);
            Controls.Add(label1);
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
        private Label runningLabel;
    }
}