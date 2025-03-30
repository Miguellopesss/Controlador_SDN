using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace LTI_Mikrotik
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            Password.PasswordChar = '●'; // Protege a password
        }

        private async void Login_Click_1(object sender, EventArgs e)
        {
            var urlLogin = "http://192.168.1.145/rest/interface";
            string username = User.Text;
            string password = Password.Text;

            using (HttpClient httpClient = new HttpClient())
            {
                // Ignorar certificado SSL (caso uses HTTPS no futuro)
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(urlLogin);
                    System.Diagnostics.Debug.WriteLine(response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Form1 form1 = new Form1(username, password);
                        form1.FormClosed += (s, args) => this.Show(); // Voltar ao Login ao fechar
                        form1.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Login falhou: credenciais inválidas.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro na ligação: " + ex.Message);
                }
            }
        }
    }
}
