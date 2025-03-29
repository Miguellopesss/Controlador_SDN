namespace LTI_Mikrotik
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage6 = new TabPage();
            tabPage1 = new TabPage();
            groupBox1 = new GroupBox();
            Mode = new ComboBox();
            button6 = new Button();
            SSID = new TextBox();
            label8 = new Label();
            Band = new ComboBox();
            label7 = new Label();
            SecurityProf = new ComboBox();
            label6 = new Label();
            Channel = new ComboBox();
            label5 = new Label();
            Frequency = new ComboBox();
            label4 = new Label();
            label3 = new Label();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            label1 = new Label();
            listBox1 = new ListBox();
            tabPage2 = new TabPage();
            label2 = new Label();
            button4 = new Button();
            button5 = new Button();
            listBox2 = new ListBox();
            tabPage3 = new TabPage();
            tabPage4 = new TabPage();
            tabPage5 = new TabPage();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(31, 39);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1264, 701);
            tabControl1.TabIndex = 0;
            // 
            // tabPage6
            // 
            tabPage6.Location = new Point(4, 29);
            tabPage6.Name = "tabPage6";
            tabPage6.Size = new Size(1256, 668);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "Interfaces";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Controls.Add(button3);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(listBox1);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1256, 668);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Wireless";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Gainsboro;
            groupBox1.Controls.Add(Mode);
            groupBox1.Controls.Add(button6);
            groupBox1.Controls.Add(SSID);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(Band);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(SecurityProf);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(Channel);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(Frequency);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(820, 79);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(404, 464);
            groupBox1.TabIndex = 18;
            groupBox1.TabStop = false;
            groupBox1.Text = "Configurar Rede";
            // 
            // Mode
            // 
            Mode.FormattingEnabled = true;
            Mode.Items.AddRange(new object[] { "alignment only", "ap bridge", "bridge", "nstreme dual slave", "station", "station bridge", "station pseudobridge", "station pseudobridge clone", "station wds", "wds slave" });
            Mode.Location = new Point(190, 37);
            Mode.Name = "Mode";
            Mode.Size = new Size(151, 28);
            Mode.TabIndex = 6;
            // 
            // button6
            // 
            button6.Location = new Point(207, 409);
            button6.Name = "button6";
            button6.Size = new Size(115, 33);
            button6.TabIndex = 17;
            button6.Text = "Atualizar";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // SSID
            // 
            SSID.Location = new Point(190, 359);
            SSID.Name = "SSID";
            SSID.Size = new Size(151, 27);
            SSID.TabIndex = 5;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(63, 366);
            label8.Name = "label8";
            label8.Size = new Size(40, 20);
            label8.TabIndex = 16;
            label8.Text = "SSID";
            // 
            // Band
            // 
            Band.FormattingEnabled = true;
            Band.Location = new Point(190, 96);
            Band.Name = "Band";
            Band.Size = new Size(151, 28);
            Band.TabIndex = 7;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(63, 292);
            label7.Name = "label7";
            label7.Size = new Size(108, 20);
            label7.TabIndex = 15;
            label7.Text = "Security Profile";
            // 
            // SecurityProf
            // 
            SecurityProf.FormattingEnabled = true;
            SecurityProf.Location = new Point(190, 289);
            SecurityProf.Name = "SecurityProf";
            SecurityProf.Size = new Size(151, 28);
            SecurityProf.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(63, 224);
            label6.Name = "label6";
            label6.Size = new Size(76, 20);
            label6.TabIndex = 14;
            label6.Text = "Frequency";
            // 
            // Channel
            // 
            Channel.FormattingEnabled = true;
            Channel.Items.AddRange(new object[] { "20mhz", "10mhz", "5mhz", "20/40mhz-eC", "20/40mhz-Ce", "20/40mhz-XX" });
            Channel.Location = new Point(190, 157);
            Channel.Name = "Channel";
            Channel.Size = new Size(151, 28);
            Channel.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(63, 160);
            label5.Name = "label5";
            label5.Size = new Size(106, 20);
            label5.TabIndex = 13;
            label5.Text = "Channel Width";
            // 
            // Frequency
            // 
            Frequency.FormattingEnabled = true;
            Frequency.Items.AddRange(new object[] { "auto", "2412", "2417", "2422", "2427", "2432", "2437", "2442", "2447", "2452" });
            Frequency.Location = new Point(190, 221);
            Frequency.Name = "Frequency";
            Frequency.Size = new Size(151, 28);
            Frequency.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(63, 99);
            label4.Name = "label4";
            label4.Size = new Size(43, 20);
            label4.TabIndex = 12;
            label4.Text = "Band";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(63, 40);
            label3.Name = "label3";
            label3.Size = new Size(48, 20);
            label3.TabIndex = 11;
            label3.Text = "Mode";
            // 
            // button3
            // 
            button3.Location = new Point(462, 561);
            button3.Name = "button3";
            button3.Size = new Size(115, 46);
            button3.TabIndex = 4;
            button3.Text = "Atualizar";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.Red;
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.ForeColor = SystemColors.Control;
            button2.Location = new Point(609, 148);
            button2.Name = "button2";
            button2.Size = new Size(115, 46);
            button2.TabIndex = 3;
            button2.Text = "Desativar";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 192, 0);
            button1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.Control;
            button1.Location = new Point(609, 79);
            button1.Name = "button1";
            button1.Size = new Size(115, 46);
            button1.TabIndex = 2;
            button1.Text = "Ativar";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(196, 38);
            label1.Name = "label1";
            label1.Size = new Size(188, 20);
            label1.TabIndex = 1;
            label1.Text = "Redes Wireless Disponiveis";
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.Gainsboro;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(27, 79);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(550, 464);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(button5);
            tabPage2.Controls.Add(listBox2);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1256, 668);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Bridge";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(173, 36);
            label2.Name = "label2";
            label2.Size = new Size(177, 20);
            label2.TabIndex = 6;
            label2.Text = "Redes Bridge Disponiveis";
            // 
            // button4
            // 
            button4.Location = new Point(597, 144);
            button4.Name = "button4";
            button4.Size = new Size(115, 46);
            button4.TabIndex = 5;
            button4.Text = "Remover";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(597, 84);
            button5.Name = "button5";
            button5.Size = new Size(115, 46);
            button5.TabIndex = 4;
            button5.Text = "Editar";
            button5.UseVisualStyleBackColor = true;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.Location = new Point(28, 84);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(519, 484);
            listBox2.TabIndex = 1;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1256, 668);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "DHCP";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Location = new Point(4, 29);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1256, 668);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "DNS";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            tabPage5.Location = new Point(4, 29);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(1256, 668);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Static Route";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1327, 776);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private Label label1;
        private ListBox listBox1;
        private Button button3;
        private Button button2;
        private Button button1;
        private Label label2;
        private Button button4;
        private Button button5;
        private ListBox listBox2;
        private ComboBox Frequency;
        private ComboBox Channel;
        private ComboBox SecurityProf;
        private ComboBox Band;
        private ComboBox Mode;
        private TextBox SSID;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Button button6;
        private TabPage tabPage6;
        private GroupBox groupBox1;
    }
}
