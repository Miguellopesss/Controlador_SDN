using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Net.Mail;

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

        private async void setMacAddress()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Ignorar a validação do certificado SSL
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                var byteArray = Encoding.ASCII.GetBytes("admin:proxmox123");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                System.Diagnostics.Debug.WriteLine("testeeeeeeeee");
                var endpoint = "http://192.168.1.145/rest/interface";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonString = JsonSerializer.Deserialize<List<LoginInterface>>(responseBody);

                    System.Diagnostics.Debug.WriteLine(responseBody);
                    System.Diagnostics.Debug.WriteLine(jsonString);

                    if (jsonString != null)
                    {
                        foreach (LoginInterface loginInterface in jsonString)
                        {
                            // Procurar a primeira interface ativa com MAC address válido
                            if (loginInterface.Running == "true" && !string.IsNullOrWhiteSpace(loginInterface.MacAddress))
                            {
                                macAddress.Text = loginInterface.MacAddress;
                                runningLabel.Text = loginInterface.DefaultName + " (ativa)";
                                break; // Já encontrámos a interface desejada
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erro: " + ex.Message);
                }
            }
        }


        private void Login_Load(object sender, EventArgs e)
        {
            setMacAddress();

            MNDPDiscovery.DiscoverDevices(device =>
            {
                Invoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[MNDP] {device}");
                    System.Diagnostics.Debug.WriteLine(device.Identity);
                    System.Diagnostics.Debug.WriteLine(device.MacAddress);
                   
                }));
            });
        }


        private void OnDeviceDiscovered(DeviceInfo deviceInfo)
        {
            Invoke(new Action(() =>
            {
                System.Diagnostics.Debug.WriteLine($"Dispositivo encontrado: {deviceInfo.Identity}, MAC: {deviceInfo.MacAddress}");
            }));
        }

    }
}
