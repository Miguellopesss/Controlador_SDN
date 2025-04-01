using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LTI_Mikrotik
{
    public partial class LoginForm : Form
    {
        private List<Device> devices = new List<Device>();

        public LoginForm()
        {
            InitializeComponent();
            password.PasswordChar = '●';
            LoadDevicesFromDatabase();

            username.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            password.KeyDown += new KeyEventHandler(OnKeyDownHandler);
            listBox1.KeyDown += new KeyEventHandler(OnKeyDownHandler);
        }

        private void OnKeyDownHandler(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login_Click_1(sender ?? this, e);
            }
        }



        private async void LoadDevicesFromDatabase()
        {
            using (var context = new LTI_Mikrotik.Data.AppDbContext())
            {
                devices = await context.Devices.ToListAsync();
            }

            listBox1.Items.Clear();
            foreach (var device in devices)
            {
                listBox1.Items.Add($"{device.name} ({device.ipAddress}) - {device.username}");
            }
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < devices.Count)
            {
                var selecionado = devices[listBox1.SelectedIndex];
                name.Text = selecionado.name;
                ipAddress.Text = selecionado.ipAddress;
                username.Text = selecionado.username;
                password.Text = selecionado.password;
            }
        }

        private async void Login_Click_1(object sender, EventArgs e)
        {
            string nameValue = name.Text.Trim();
            string ipAddressValue = ipAddress.Text.Trim();
            string usernameValue = username.Text.Trim();
            string passwordValue = password.Text;

            if (string.IsNullOrWhiteSpace(nameValue) ||
                string.IsNullOrWhiteSpace(ipAddressValue) ||
                string.IsNullOrWhiteSpace(usernameValue) ||
                string.IsNullOrWhiteSpace(passwordValue))
            {
                MessageBox.Show("Preencha todos os campos antes de iniciar sessão.");
                return;
            }

            var device = new Device
            {
                name = nameValue,
                ipAddress = ipAddressValue,
                username = usernameValue,
                password = passwordValue
            };

            bool sucesso = await ConnectToDevice(device);
            if (sucesso)
            {
                MessageBox.Show("Login efetuado com sucesso!");
                using (var context = new LTI_Mikrotik.Data.AppDbContext())
                {
                    var existingDevice = await context.Devices
                        .FirstOrDefaultAsync(d => d.name == device.name && d.ipAddress == device.ipAddress);

                    if (existingDevice != null)
                    {
                        return;
                    }

                    context.Devices.Add(device);
                    await context.SaveChangesAsync();
                }
                listBox1.Items.Clear();
                devices.Add(device);
                foreach (var d in devices)
                {
                    listBox1.Items.Add($"{d.name} ({d.ipAddress}) - {d.username}");
                    System.Diagnostics.Debug.WriteLine($"Nome: {d.name}");
                    System.Diagnostics.Debug.WriteLine($"IP: {d.ipAddress}");
                    System.Diagnostics.Debug.WriteLine($"Username: {d.username}");
                    System.Diagnostics.Debug.WriteLine("------------------------------");
                }
            }
        }

        private async Task<bool> ConnectToDevice(Device device)
        {
            var urlLogin = $"http://{device.ipAddress}/rest/interface";

            using (HttpClient httpClient = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender2, cert, chain, sslPolicyErrors) => true;

                var byteArray = Encoding.ASCII.GetBytes($"{device.username}:{device.password}");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(urlLogin);
                    System.Diagnostics.Debug.WriteLine(response.StatusCode);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Form1 form1 = new Form1(device);
                        form1.FormClosed += (s, args) =>
                        {
                            username.Text = "";
                            password.Text = "";
                            this.Show();
                        };

                        form1.Show();
                        this.Hide();

                        return true;
                    }
                    else
                    {
                        MessageBox.Show($"Login falhou para {device.name}: credenciais inválidas.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro na ligação com {device.name}: " + ex.Message);
                    return false;
                }
            }
        }
    }


}
