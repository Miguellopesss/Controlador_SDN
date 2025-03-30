using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LTI_Mikrotik
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        private async void Login_Click(object sender, EventArgs e)
        {
            var urlLogin = "https://10.0.0.254/rest/interface";
            string username = User.Text;
            string password = Password.Text;

            using (HttpClient httpClient = new HttpClient())
            {
                // Ignorar a validação do certificado SSL
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = await httpClient.GetAsync(urlLogin);
                System.Diagnostics.Debug.WriteLine(response.StatusCode);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Environment.SetEnvironmentVariable("username", username);
                    Environment.SetEnvironmentVariable("password", password);

                    Form1 form1 = new Form1();
                    form1.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Login Failed");
                }
            }
        }

        }
}
