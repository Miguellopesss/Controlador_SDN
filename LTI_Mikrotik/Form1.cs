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
        private RotaIP? rotaSelecionada;
        private List<IpAddressEntry> enderecosIp = new();
        private string urlLink = "http://192.168.1.145/rest/";
        private IpAddressEntry? enderecoSelecionado;
        private List<InterfaceGenerica> todasInterfaces = new List<InterfaceGenerica>();
        private List<DnsStaticEntry> dnsStaticEntries = new List<DnsStaticEntry>();
        private DnsStaticEntry? dnsStaticSelecionado;


        public Form1(string username, string password)
        {
            InitializeComponent();

            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            var loginForm = Application.OpenForms["LoginForm"] as LoginForm;

            if (loginForm != null)
            {
                var userControl = loginForm.Controls["User"] as TextBox;
                var passwordControl = loginForm.Controls["Password"] as TextBox;

                if (userControl != null && passwordControl != null)
                {
                    userControl.Text = "";
                    passwordControl.Text = "";
                }

                loginForm.Show();
            }
        }





        private async void Form1_Load(object sender, EventArgs e)
        {


            await CarregarPerfisDeSegurancaAsync();
            await CarregarInterfacesWireless();
            await CarregarTodasInterfaces();
            await CarregarRotas();
            await CarregarEnderecosIP();
            await CarregarDnsAsync();
            await CarregarDnsStaticAsync();



        }



        private async Task CarregarInterfacesWireless()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "interface/wireless");

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

            HttpResponseMessage response = await client.PostAsync($"{urlLink}interface/wireless/{comando}", content);

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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WirelessInterface iface)
            {
                Mode.Text = iface.Mode;

                Channel.Text = iface.ChannelWidth;
                Frequency.Text = iface.Frequency;
                SSID.Text = iface.SSID;
                SecurityProf.Text = iface.SecurityProfile;

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
                HttpResponseMessage response = await client.GetAsync(urlLink + "interface/wireless/security-profiles");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    var perfisSegurancaTemp = JsonSerializer.Deserialize<List<SecurityProfile>>(json);
                    perfisSeguranca = perfisSegurancaTemp ?? new List<SecurityProfile>();

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
                System.Diagnostics.Debug.WriteLine(debugJson);


                var content = new StringContent(debugJson, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}interface/wireless/{iface.Id}")
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


        private async Task CarregarTodasInterfaces()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "interface");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var interfacesTemp = JsonSerializer.Deserialize<List<InterfaceGenerica>>(json);
                    todasInterfaces = interfacesTemp ?? new List<InterfaceGenerica>();

                    listBox3.Items.Clear();
                    comboBox1.Items.Clear();
                    comboBox2.Items.Clear();

                    if (todasInterfaces != null)
                    {
                        foreach (var iface in todasInterfaces)
                        {
                            listBox3.Items.Add(iface);
                            comboBox1.Items.Add(iface.Name);
                            comboBox2.Items.Add(iface.Name);
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter interfaces:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }



        private async Task CarregarRotas()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/route");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var rotas = JsonSerializer.Deserialize<List<RotaIP>>(json);

                    listBox4.Items.Clear();

                    if (rotas != null)
                    {
                        foreach (var rota in rotas)
                        {
                            if (!string.IsNullOrWhiteSpace(rota.Gateway))
                                listBox4.Items.Add(rota);
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao carregar rotas:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }


        private async void button8_Click_1(object sender, EventArgs e)
        {
            if (rotaSelecionada == null)
            {
                MessageBox.Show("Seleciona uma rota primeiro.");
                return;
            }

            var rotaAtualizada = new Dictionary<string, string>
    {
        { "dst-address", textBox1.Text },
        { "gateway", textBox2.Text }
    };

            var content = new StringContent(JsonSerializer.Serialize(rotaAtualizada), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/route/{rotaSelecionada.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Rota atualizada com sucesso.");
                await CarregarRotas(); // Atualiza a listBox4
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar rota:\n{response.StatusCode}\n{erro}");
            }
        }

        private void listBox4_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem is RotaIP rota)
            {
                rotaSelecionada = rota;
                textBox1.Text = rota.DstAddress;
                textBox2.Text = rota.Gateway;
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            if (rotaSelecionada == null)
            {
                MessageBox.Show("Seleciona uma rota primeiro.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Tens a certeza que queres apagar a rota {rotaSelecionada.DstAddress} via {rotaSelecionada.Gateway}?",
                "Confirmar remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{urlLink}ip/route/{rotaSelecionada.Id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rota apagada com sucesso.");
                    await CarregarRotas();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar rota:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message);
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            string dstAddress = textBox3.Text.Trim();
            string gateway = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(dstAddress) || string.IsNullOrWhiteSpace(gateway))
            {
                MessageBox.Show("Preenche o Dst. Address e o Gateway.");
                return;
            }

            var novaRota = new Dictionary<string, string>
            {
                { "dst-address", dstAddress },
                { "gateway", gateway }
            };

            var content = new StringContent(JsonSerializer.Serialize(novaRota), Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, urlLink + "ip/route")
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rota criada com sucesso.");
                    await CarregarRotas();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar a rota:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message);
            }
        }

        private async Task CarregarEnderecosIP()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/address");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var enderecosIpTemp = JsonSerializer.Deserialize<List<IpAddressEntry>>(json);
                    enderecosIp = enderecosIpTemp ?? new List<IpAddressEntry>();

                    listBox5.Items.Clear();

                    if (enderecosIp != null)
                    {
                        foreach (var ip in enderecosIp)
                            listBox5.Items.Add(ip);
                    }
                }

                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao buscar IPs:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar IPs: " + ex.Message);
            }
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox5.SelectedItem is IpAddressEntry ip)
            {
                enderecoSelecionado = ip;
                textBox7.Text = ip.Address;
                textBox8.Text = ip.Network;
                comboBox1.SelectedItem = ip.Interface;
            }
        }


        private async void button11_Click(object sender, EventArgs e)
        {
            if (enderecoSelecionado == null)
            {
                MessageBox.Show("Seleciona um endereço IP da lista.");
                return;
            }

            var novo = new Dictionary<string, string>
            {
                ["address"] = textBox7.Text,
                ["network"] = textBox8.Text,
                ["interface"] = comboBox1.SelectedItem?.ToString() ?? ""
            };

            var content = new StringContent(JsonSerializer.Serialize(novo), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(new HttpMethod("PATCH"),
                $"{urlLink}ip/address/{enderecoSelecionado.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Endereço atualizado com sucesso.");
                await CarregarEnderecosIP();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar endereço:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button12_Click_1(object sender, EventArgs e)
        {
            if (enderecoSelecionado == null)
            {
                MessageBox.Show("Seleciona um endereço IP da lista.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Tens a certeza que queres apagar o endereço:\n{enderecoSelecionado.Address}?",
                "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{urlLink}ip/address/{enderecoSelecionado.Id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Endereço apagado com sucesso.");
                    await CarregarEnderecosIP(); // atualiza a lista
                    enderecoSelecionado = null;
                    textBox7.Clear();
                    textBox8.Clear();
                    comboBox1.SelectedIndex = -1;
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar o endereço:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            string address = textBox5.Text.Trim();
            string network = textBox6.Text.Trim();
            string iface = comboBox2.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(iface))
            {
                MessageBox.Show("Preenche todos os campos.");
                return;
            }

            // Preenche o network automaticamente se estiver vazio
            if (string.IsNullOrWhiteSpace(network))
            {
                int barraIndex = address.IndexOf('/');
                if (barraIndex != -1)
                    network = address[..barraIndex];
                else
                    network = address;
            }

            var novoEndereco = new Dictionary<string, string>
            {
                ["address"] = address,
                ["network"] = network,
                ["interface"] = iface
            };

            var content = new StringContent(JsonSerializer.Serialize(novoEndereco), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{urlLink}ip/address")
            {
                Content = content
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Endereço criado com sucesso.");
                    await CarregarEnderecosIP();
                    textBox5.Clear();
                    textBox6.Clear();
                    comboBox2.SelectedIndex = -1;
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar endereço:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async Task CarregarDnsAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/dns");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var dnsInfo = JsonSerializer.Deserialize<DnsInfo>(json);

                    listBox6.Items.Clear();
                    listBox6.Items.Add($"Servers: {dnsInfo?.Servers}");
                    listBox6.Items.Add($"Dynamic Servers: {dnsInfo?.DynamicServers}");
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter DNS:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar DNS: " + ex.Message);
            }
        }

        private async Task CarregarDnsStaticAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/dns/static");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var dnsStaticEntriesTemp = JsonSerializer.Deserialize<List<DnsStaticEntry>>(json);
                    dnsStaticEntries = dnsStaticEntriesTemp ?? new List<DnsStaticEntry>();

                    listBox7.Items.Clear();

                    if (dnsStaticEntries != null)
                    {
                        foreach (var entrada in dnsStaticEntries)
                        {
                            if (entrada != null)
                            {
                                listBox7.Items.Add(entrada.ToString() ?? string.Empty);
                            }
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter DNS Static:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar DNS Static: " + ex.Message);
            }
        }





        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox7.SelectedIndex;
            if (index >= 0 && index < dnsStaticEntries.Count)
            {
                dnsStaticSelecionado = dnsStaticEntries[index];

                // Preencher os campos
                textBox11.Text = dnsStaticSelecionado.Name;
                textBox13.Text = dnsStaticSelecionado.Address;
                comboBox3.SelectedItem = "A"; // Fixo, se só tiveres esse
            }
        }




        private async void button17_Click_1(object sender, EventArgs e)
        {
            if (dnsStaticSelecionado == null)
            {
                MessageBox.Show("Seleciona uma entrada DNS static.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["disabled"] = "false"
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/dns/static/{dnsStaticSelecionado.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("DNS static ativado com sucesso.");
                await CarregarDnsStaticAsync();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao ativar:\n{response.StatusCode}\n{erro}");
            }
        }



        private async void button16_Click(object sender, EventArgs e)
        {
            if (dnsStaticSelecionado == null)
            {
                MessageBox.Show("Seleciona uma entrada DNS static.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["disabled"] = "true"
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/dns/static/{dnsStaticSelecionado.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("DNS static desativado com sucesso.");
                await CarregarDnsStaticAsync();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao desativar:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            string novosServers = textBox12.Text.Trim();

            if (string.IsNullOrWhiteSpace(novosServers))
            {
                MessageBox.Show("Insere pelo menos um servidor DNS.");
                return;
            }

            var body = new Dictionary<string, string>
    {
        { "servers", novosServers }
    };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync($"{urlLink}ip/dns/set", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("DNS atualizado com sucesso.");
                    await CarregarDnsAsync();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao atualizar DNS:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async void button15_Click(object sender, EventArgs e)
        {
            if (dnsStaticSelecionado == null)
            {
                MessageBox.Show("Seleciona uma entrada DNS static.");
                return;
            }

            string name = textBox11.Text.Trim();
            string type = comboBox3.SelectedItem?.ToString() ?? "";
            string address = textBox13.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Preenche todos os campos.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["name"] = name,
                ["type"] = type,
                ["address"] = address
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/dns/static/{dnsStaticSelecionado.Id}")
            {
                Content = content
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Entrada DNS static atualizada com sucesso.");
                    await CarregarDnsStaticAsync();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao atualizar:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

    }

    public class WirelessInterface
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("ssid")] public string SSID { get; set; } = string.Empty;
        [JsonPropertyName("frequency")] public string Frequency { get; set; } = string.Empty;
        [JsonPropertyName("mode")] public string Mode { get; set; } = string.Empty;
        [JsonPropertyName("band")] public string Band { get; set; } = string.Empty;
        [JsonPropertyName("channel-width")] public string ChannelWidth { get; set; } = string.Empty;
        [JsonPropertyName("security-profile")] public string SecurityProfile { get; set; } = string.Empty;
        [JsonPropertyName("disabled")] public string DisabledRaw { get; set; } = string.Empty;
        public bool Disabled => DisabledRaw == "true";

        public override string ToString()
        {
            string estado = Disabled ? "Disable" : "Enable";
            return $"{Name} - {Mode} - {Band} - {ChannelWidth} - {SSID} - {Frequency} - {estado}";
        }
    }

    public class InterfaceGenerica
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Name: {Name} - Type: {Type}";
        }
    }

    public class RotaIP
    {
        [JsonPropertyName(".id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("dst-address")]
        public string DstAddress { get; set; } = string.Empty;

        [JsonPropertyName("gateway")]
        public string Gateway { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Dst Address: {DstAddress} - Gateway: {Gateway}";
        }
    }

 
    public class IpAddressEntry
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("address")] public string Address { get; set; } = string.Empty;
        [JsonPropertyName("network")] public string Network { get; set; } = string.Empty;
        [JsonPropertyName("interface")] public string Interface { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Address: {Address} - Network: {Network} - Interface: {Interface}";
        }
    }

    public class DnsInfo
    {
        [JsonPropertyName("servers")] public string Servers { get; set; } = string.Empty;
        [JsonPropertyName("dynamic-servers")] public string DynamicServers { get; set; } = string.Empty;
    }



    public class DnsStaticEntry
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
        [JsonPropertyName("address")] public string Address { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Name: {Name} -> Address: {Address}";
        }
    }




    public class SecurityProfile : Dictionary<string, string>
    {
        public string Id => this.ContainsKey(".id") ? this[".id"] : "";
        public string Name => this.ContainsKey("name") ? this["name"] : "(sem nome)";
    }
}
