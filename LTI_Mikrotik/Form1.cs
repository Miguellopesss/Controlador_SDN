using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace LTI_Mikrotik
{
    public partial class Form1 : Form
    {
        private readonly HttpClient client = new HttpClient();
        private List<SecurityProfile> perfisSeguranca = new List<SecurityProfile>();

        public Form1()
        {
            InitializeComponent();

            var byteArray = Encoding.ASCII.GetBytes("admin:proxmox123");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CarregarPerfisDeSegurancaAsync();
            await CarregarInterfacesWireless();
        }

        private async Task CarregarInterfacesWireless()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://192.168.1.145/rest/interface/wireless");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    List<WirelessInterface>? interfaces = JsonSerializer.Deserialize<List<WirelessInterface>>(json);

                    listBox1.Items.Clear();

                    if (interfaces != null)
                    {
                        foreach (var iface in interfaces)
                        {
                            listBox1.Items.Add(iface);
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao aceder à API:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async Task EnviarComando(string comando, string id)
        {
            var payload = new Dictionary<string, string> { { ".id", id } };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"http://192.168.1.145/rest/interface/wireless/{comando}", content);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Interface {comando} com sucesso.");
                await CarregarInterfacesWireless();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao tentar {comando}:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WirelessInterface iface)
                await EnviarComando("enable", iface.Id);
            else
                MessageBox.Show("Seleciona uma interface primeiro.");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WirelessInterface iface)
                await EnviarComando("disable", iface.Id);
            else
                MessageBox.Show("Seleciona uma interface primeiro.");
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await CarregarInterfacesWireless();
        }

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WirelessInterface iface)
            {
                Mode.Text = iface.Mode;

                Channel.Text = iface.ChannelWidth;
                Frequency.Text = iface.Frequency;
                SSID.Text = iface.SSID;
                SecurityProf.Text = iface.SecurityProfile;

                // Atualiza os itens da comboBox Band com base no nome da interface
                Band.Items.Clear();
                if (iface.Name == "wlan2")
                {
                    Band.Items.AddRange(new object[] {
                    "5ghz-a", "5ghz-onlyn", "5ghz-a/n", "5ghz-a/n/ac",
                    "5ghz-onlyac", "5ghz-n/ac"
                });
                }
                else
                {
                    Band.Items.AddRange(new object[] {
                    "2ghz-b", "2ghz-onlyg", "2ghz-b/g", "2ghz-onlyn",
                    "2ghz-b/g/n", "2ghz-g/n"
                });
                }

                Band.Text = iface.Band;

            }


        }

        private async Task CarregarPerfisDeSegurancaAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://192.168.1.145/rest/interface/wireless/security-profiles");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    perfisSeguranca = JsonSerializer.Deserialize<List<SecurityProfile>>(json);

                    SecurityProf.Items.Clear();

                    if (perfisSeguranca != null)
                    {
                        foreach (var perfil in perfisSeguranca)
                        {
                            SecurityProf.Items.Add(perfil.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar perfis de segurança: " + ex.Message);
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is not WirelessInterface iface)
            {
                MessageBox.Show("Seleciona uma interface primeiro.");
                return;
            }

            try
            {
                string nomePerfil = SecurityProf.SelectedItem?.ToString() ?? "";
                string idPerfil = perfisSeguranca.Find(p => p.Name == nomePerfil)?.Id ?? "";
                string perfilName = SecurityProf.SelectedItem?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(idPerfil))
                {
                    MessageBox.Show("Perfil de segurança inválido ou não selecionado.");
                    return;
                }

                var novoObjeto = new Dictionary<string, object>
                {
                    ["ssid"] = SSID.Text,
                    ["disabled"] = iface.Disabled,
                    ["frequency"] = Frequency.Text,
                    ["mode"] = Mode.Text.Trim().ToString().Replace(" ", "-"),
                    ["band"] = Band.Text.Trim().ToString(),
                    ["channel-width"] = Channel.Text.Trim().ToString(),
                    ["security-profile"] = perfilName
                };

                string debugJson = JsonSerializer.Serialize(novoObjeto, new JsonSerializerOptions { WriteIndented = true });
                System.Diagnostics.Debug.WriteLine(debugJson); // Mostra na janela de saída do Visual Studio


                var content = new StringContent(debugJson, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"http://192.168.1.145/rest/interface/wireless/{iface.Id}")
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Configuração atualizada com sucesso.");
                    await CarregarInterfacesWireless();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao atualizar a configuração:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message);
            }
        }

    }

    public class WirelessInterface
    {
        [JsonPropertyName(".id")] public string Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("ssid")] public string SSID { get; set; }
        [JsonPropertyName("frequency")] public string Frequency { get; set; }
        [JsonPropertyName("mode")] public string Mode { get; set; }
        [JsonPropertyName("band")] public string Band { get; set; }
        [JsonPropertyName("channel-width")] public string ChannelWidth { get; set; }
        [JsonPropertyName("security-profile")] public string SecurityProfile { get; set; }

        [JsonPropertyName("disabled")]
        public string DisabledRaw { get; set; }

        public bool Disabled => DisabledRaw == "true";

        public override string ToString()
        {
            string estado = Disabled ? "Disable" : "Enable";
            return $"{Name} - {Mode} - {Band} - {ChannelWidth} - {SSID} - {Frequency} - {estado}";
        }
    }

    public class SecurityProfile : Dictionary<string, string>
    {
        public string Id => this.ContainsKey(".id") ? this[".id"] : "";
        public string Name => this.ContainsKey("name") ? this["name"] : "(sem nome)";
    }
}
