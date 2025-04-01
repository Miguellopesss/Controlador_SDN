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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LTI_Mikrotik
{
    public partial class Form1 : Form
    {
        private readonly HttpClient client = new HttpClient();
        private List<SecurityProfile> perfisSeguranca = new List<SecurityProfile>();
        private RotaIP? rotaSelecionada;
        private List<IpAddressEntry> enderecosIp = new();
        private string urlLink = "";
        private IpAddressEntry? enderecoSelecionado;
        private List<InterfaceGenerica> todasInterfaces = new List<InterfaceGenerica>();
        private List<DnsStaticEntry> dnsStaticEntries = new List<DnsStaticEntry>();
        private DnsStaticEntry? dnsStaticSelecionado;
        private List<DhcpServer> dhcpServers = new();
        private DhcpServer? dhcpSelecionado;
        private AddressPool? poolSelecionada;
        private List<Bridge> bridges = new List<Bridge>();
        private Bridge? bridgeSelecionada;
        private List<BridgePort> bridgePorts = new List<BridgePort>();
        private BridgePort? portSelecionado;
        private Device device;
        private SecurityProfile? perfilSelecionado;

        private string IP;


        public Form1(Device device)
        {
            InitializeComponent();
            this.device = device;
            this.IP = device.ipAddress;
            urlLink = $"http://{IP}/rest/";
            var byteArray = Encoding.ASCII.GetBytes($"{device.username}:{device.password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            PreencherComboboxSecurityProfile();
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object? sender, FormClosedEventArgs e)
        {
            var loginForm = Application.OpenForms["LoginForm"] as LoginForm;

            if (loginForm != null)
            {
                var userControl = loginForm.Controls["User"] as System.Windows.Forms.TextBox;
                var passwordControl = loginForm.Controls["Password"] as System.Windows.Forms.TextBox;

                if (userControl != null && passwordControl != null)
                {
                    userControl.Text = "";
                    passwordControl.Text = "";
                }

                loginForm.Show();
            }
        }


        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl? tabControl = sender as TabControl;
            if (tabControl == null) return;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabRect = tabControl.GetTabRect(e.Index);

            // Verificar se a aba é a aba ativa
            if (e.Index == tabControl.SelectedIndex)
            {
                // Desenhar o fundo da aba ativa com uma cor diferente
                e.Graphics.FillRectangle(Brushes.LightBlue, tabRect);
            }
            else
            {
                // Desenhar o fundo das outras abas
                e.Graphics.FillRectangle(SystemBrushes.Control, tabRect);
            }

            // Desenhar o texto da aba
            TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font, tabRect, tabPage.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
            await CarregarDhcpServersAsync();
            await CarregarAddressPoolsAsync();
            await CarregarBridgesAsync();
            await CarregarPortsAsync();
            await CarregarSecurityProfilesAsync();
            comboBox16.SelectedIndexChanged += comboBox16_SelectedIndexChanged;
            comboBox14.SelectedIndexChanged += comboBox14_SelectedIndexChanged;

            textBox25.Enabled = false;
            textBox27.Enabled = false;
            textBox28.Enabled = false;
            textBox23.Enabled = false;


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
                    comboBox6.Items.Clear();
                    comboBox5.Items.Clear();
                    comboBox9.Items.Clear();
                    comboBox11.Items.Clear();

                    if (todasInterfaces != null)
                    {
                        foreach (var iface in todasInterfaces)
                        {
                            listBox3.Items.Add(iface);
                            comboBox1.Items.Add(iface.Name);
                            comboBox2.Items.Add(iface.Name);
                            comboBox6.Items.Add(iface.Name);
                            comboBox5.Items.Add(iface.Name);
                            comboBox9.Items.Add(iface.Name);
                            comboBox11.Items.Add(iface.Name);
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

        private async Task CarregarDhcpServersAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/dhcp-server");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var dhcpServersTemp = JsonSerializer.Deserialize<List<DhcpServer>>(json);
                    dhcpServers = dhcpServersTemp ?? new List<DhcpServer>();

                    listBox8.Items.Clear();

                    if (dhcpServers != null)
                    {
                        foreach (var server in dhcpServers)
                            listBox8.Items.Add(server);
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter servidores DHCP:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }


        private void listBox8_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox8.SelectedIndex >= 0 && listBox8.SelectedIndex < dhcpServers.Count)
            {
                dhcpSelecionado = dhcpServers[listBox8.SelectedIndex];

                textBox16.Text = dhcpSelecionado.Name;
                textBox17.Text = dhcpSelecionado.LeaseTime;
                comboBox6.SelectedItem = dhcpSelecionado.Interface;
                comboBox8.SelectedItem = dhcpSelecionado.AddressPool;
            }
        }


        private async void button20_Click_1(object sender, EventArgs e)
        {
            if (dhcpSelecionado == null)
            {
                MessageBox.Show("Seleciona um servidor DHCP.");
                return;
            }

            HttpResponseMessage response = await client.DeleteAsync($"{urlLink}ip/dhcp-server/{dhcpSelecionado.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Servidor DHCP removido com sucesso.");
                await CarregarDhcpServersAsync();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao remover DHCP:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button19_Click(object sender, EventArgs e)
        {
            if (dhcpSelecionado == null)
            {
                MessageBox.Show("Seleciona um servidor DHCP.");
                return;
            }

            var update = new Dictionary<string, string>
            {
                ["name"] = textBox16.Text.Trim(),
                ["lease-time"] = textBox17.Text.Trim(),
                ["interface"] = comboBox6.SelectedItem?.ToString() ?? "",
                ["address-pool"] = comboBox8.SelectedItem?.ToString() ?? ""
            };

            var content = new StringContent(JsonSerializer.Serialize(update), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/dhcp-server/{dhcpSelecionado.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("DHCP atualizado com sucesso.");
                await CarregarDhcpServersAsync();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar DHCP:\n{response.StatusCode}\n{erro}");
            }
        }


        private async Task CarregarAddressPoolsAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "ip/pool");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var pools = JsonSerializer.Deserialize<List<AddressPool>>(json);

                    comboBox8.Items.Clear();
                    comboBox7.Items.Clear();

                    listBox9.Items.Clear();

                    if (pools != null)
                    {
                        foreach (var pool in pools)
                        {
                            comboBox8.Items.Add(pool.Name);
                            comboBox7.Items.Add(pool.Name);

                            listBox9.Items.Add(pool);
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter address pools:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar address pools: " + ex.Message);
            }
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            string name = textBox14.Text.Trim();
            string leaseTime = textBox15.Text.Trim();
            string iface = comboBox5.SelectedItem?.ToString() ?? "";
            string pool = comboBox7.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(leaseTime) || string.IsNullOrWhiteSpace(iface) || string.IsNullOrWhiteSpace(pool))
            {
                MessageBox.Show("Preenche todos os campos para criar o DHCP Server.");
                return;
            }

            var novoDhcp = new Dictionary<string, string>
            {
                ["name"] = name,
                ["lease-time"] = leaseTime,
                ["interface"] = iface,
                ["address-pool"] = pool
            };

            var content = new StringContent(JsonSerializer.Serialize(novoDhcp), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{urlLink}ip/dhcp-server")
            {
                Content = content
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("DHCP Server criado com sucesso.");
                    await CarregarDhcpServersAsync(); // se tiveres esta função para atualizar a lista
                    textBox14.Clear();
                    textBox15.Clear();
                    comboBox5.SelectedIndex = -1;
                    comboBox7.SelectedIndex = -1;
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar DHCP Server:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void listBox9_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox9.SelectedItem is AddressPool selecionada)
            {
                poolSelecionada = selecionada;
                textBox20.Text = selecionada.Name;
                textBox21.Text = selecionada.Ranges;

            }
        }


        private async void button22_Click(object sender, EventArgs e)
        {
            if (poolSelecionada == null)
            {
                MessageBox.Show("Seleciona uma pool primeiro.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["name"] = textBox20.Text.Trim(),
                ["ranges"] = textBox21.Text.Trim(),

            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}ip/pool/{poolSelecionada.Id}")
            {
                Content = content
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Address pool atualizada com sucesso.");
                    await CarregarAddressPoolsAsync();
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

        private async void button23_Click(object sender, EventArgs e)
        {
            if (poolSelecionada == null)
            {
                MessageBox.Show("Seleciona uma pool primeiro.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Tens a certeza que queres apagar a pool '{poolSelecionada.Name}'?",
                "Confirmar eliminação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{urlLink}ip/pool/{poolSelecionada.Id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Address pool apagada com sucesso.");
                    await CarregarAddressPoolsAsync();
                    poolSelecionada = null;
                    textBox20.Clear();
                    textBox21.Clear();

                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async void button21_Click(object sender, EventArgs e)
        {
            string nome = textBox18.Text.Trim();
            string enderecos = textBox19.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(enderecos))
            {
                MessageBox.Show("Preenche todos os campos.");
                return;
            }

            var novaPool = new Dictionary<string, string>
            {
                ["name"] = nome,
                ["ranges"] = enderecos
            };

            var content = new StringContent(JsonSerializer.Serialize(novaPool), Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, $"{urlLink}ip/pool")
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Address pool criada com sucesso.");
                    textBox18.Clear();
                    textBox19.Clear();
                    await CarregarAddressPoolsAsync();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar address pool:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage9;
        }

        private void button25_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage9;
        }

        private void button26_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
            this.Close(); // Fecha o Form1 e ativa a lógica do FormClosed
=======
            this.Close(); // Fecha o Form1 e ativa a l�gica do FormClosed
            MessageBox.Show("Sess�o fechada!");
>>>>>>> 888ca3fba4e3cd4af0ada9c605332481dc262cd2
        }

        private async Task CarregarBridgesAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "interface/bridge");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    bridges = JsonSerializer.Deserialize<List<Bridge>>(json) ?? new List<Bridge>();

                    listBox2.Items.Clear();
                    comboBox10.Items.Clear();
                    comboBox12.Items.Clear();

                    if (bridges != null)
                    {
                        foreach (var bridge in bridges)
                        {
                            listBox2.Items.Add(bridge);
                            comboBox10.Items.Add(bridge.Name);
                            comboBox12.Items.Add(bridge.Name);
                        }
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao carregar bridges:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar bridges: " + ex.Message);
            }
        }


        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0 && listBox2.SelectedIndex < bridges.Count)
            {
                bridgeSelecionada = bridges[listBox2.SelectedIndex];
                textBox24.Text = bridgeSelecionada.Name;
            }
        }

        private async void button4_Click_1(object sender, EventArgs e)
        {
            if (bridgeSelecionada == null)
            {
                MessageBox.Show("Seleciona uma bridge primeiro.");
                return;
            }

            HttpResponseMessage response = await client.DeleteAsync($"{urlLink}interface/bridge/{bridgeSelecionada.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Bridge apagada com sucesso.");
                await CarregarBridgesAsync();
                await CarregarPortsAsync();
                LimparCamposPortBridge();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao apagar:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button28_Click_1(object sender, EventArgs e)
        {
            if (bridgeSelecionada == null)
            {
                MessageBox.Show("Seleciona uma bridge primeiro.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["name"] = textBox24.Text.Trim()
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}interface/bridge/{bridgeSelecionada.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Bridge atualizada com sucesso.");
                LimparCamposPortBridge();
                textBox24.Clear();
                await CarregarBridgesAsync();

            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button27_Click(object sender, EventArgs e)
        {
            string nomeBridge = textBox22.Text.Trim();

            if (string.IsNullOrWhiteSpace(nomeBridge))
            {
                MessageBox.Show("Insere um nome para a bridge.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["name"] = nomeBridge
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{urlLink}interface/bridge")
            {
                Content = content
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Bridge criada com sucesso.");
                    await CarregarBridgesAsync();
                    LimparCamposPortBridge();
                    textBox22.Clear();
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar bridge:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private async Task CarregarPortsAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(urlLink + "interface/bridge/port");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var bridgePortsTemp = JsonSerializer.Deserialize<List<BridgePort>>(json);
                    bridgePorts = bridgePortsTemp ?? new List<BridgePort>();

                    listBox10.Items.Clear();

                    if (bridgePorts != null)
                    {
                        foreach (var port in bridgePorts)
                            listBox10.Items.Add(port);
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter ports:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar ports: " + ex.Message);
            }
        }


        private void listBox10_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox10.SelectedIndex >= 0 && listBox10.SelectedIndex < bridgePorts.Count)
            {
                portSelecionado = bridgePorts[listBox10.SelectedIndex];

                // Preencher as ComboBoxes se os valores existirem
                if (!string.IsNullOrWhiteSpace(portSelecionado.Interface))
                    comboBox9.SelectedItem = portSelecionado.Interface;

                if (!string.IsNullOrWhiteSpace(portSelecionado.Bridge))
                    comboBox10.SelectedItem = portSelecionado.Bridge;
            }
        }



        private async void button5_Click_1(object sender, EventArgs e)
        {
            if (portSelecionado == null)
            {
                MessageBox.Show("Seleciona um port da lista.");
                return;
            }

            HttpResponseMessage response = await client.DeleteAsync($"{urlLink}interface/bridge/port/{portSelecionado.Id}");

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Port apagado com sucesso.");
                await CarregarBridgesAsync();
                await CarregarPortsAsync();
                LimparCamposPortBridge();

            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao apagar port:\n{response.StatusCode}\n{erro}");
            }
        }

        private async void button30_Click(object sender, EventArgs e)
        {
            if (portSelecionado == null)
            {
                MessageBox.Show("Seleciona um port primeiro.");
                return;
            }

            string novaInterface = comboBox9.SelectedItem?.ToString() ?? "";
            string novaBridge = comboBox10.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(novaInterface) || string.IsNullOrWhiteSpace(novaBridge))
            {
                MessageBox.Show("Seleciona a interface e a bridge.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["interface"] = novaInterface,
                ["bridge"] = novaBridge
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{urlLink}interface/bridge/port/{portSelecionado.Id}")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Port atualizado com sucesso.");
                await CarregarPortsAsync();
                LimparCamposPortBridge();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao atualizar o port:\n{response.StatusCode}\n{erro}");
            }
        }


        private async void button29_Click(object sender, EventArgs e)
        {
            string iface = comboBox11.SelectedItem?.ToString() ?? "";
            string bridge = comboBox12.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(iface) || string.IsNullOrWhiteSpace(bridge))
            {
                MessageBox.Show("Seleciona a interface e a bridge.");
                return;
            }

            var novoPort = new Dictionary<string, string>
            {
                ["interface"] = iface,
                ["bridge"] = bridge
            };

            var content = new StringContent(JsonSerializer.Serialize(novoPort), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{urlLink}interface/bridge/port")
            {
                Content = content
            };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Port criado com sucesso.");
                await CarregarPortsAsync();
                LimparCamposPortBridge();
            }
            else
            {
                string erro = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Erro ao criar o port:\n{response.StatusCode}\n{erro}");
            }
        }

        private void LimparCamposPortBridge()
        {
            textBox22.Clear();   // para criar nova bridge
            textBox24.Clear();   // para atualizar bridge
            comboBox9.SelectedIndex = -1;  // Interface (update port)
            comboBox10.SelectedIndex = -1; // Bridge (update port)
            comboBox11.SelectedIndex = -1; // Interface (create port)
            comboBox12.SelectedIndex = -1; // Bridge (create port)
        }

        private async Task CarregarSecurityProfilesAsync()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"{urlLink}interface/wireless/security-profiles");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var perfis = JsonSerializer.Deserialize<List<SecurityProfile>>(json);

                    listBox11.Items.Clear();

                    if (perfis != null)
                    {
                        foreach (var perfil in perfis)
                            listBox11.Items.Add(perfil);
                    }
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao obter perfis de segurança:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar perfis de segurança: " + ex.Message);
            }
        }



        private void listBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox11.SelectedItem is SecurityProfile selectedProfile)
            {
                perfilSelecionado = selectedProfile;

                textBox26.Text = selectedProfile.Name;

                // Atualizar Mode
                int modeIndex = comboBox17.Items.IndexOf(selectedProfile.Mode);
                comboBox17.SelectedIndex = modeIndex >= 0 ? modeIndex : -1;

                // Atualizar Authentication Type
                // Verifica se o valor exato existe nos items
                int authIndex = comboBox16.Items.IndexOf(selectedProfile.AuthenticationTypes);
                if (authIndex >= 0)
                {
                    comboBox16.SelectedIndex = authIndex;
                }
                else
                {
                    comboBox16.SelectedIndex = -1;
                    comboBox16.Text = selectedProfile.AuthenticationTypes; // mostra como texto mesmo que não esteja na lista
                }

                // Força atualização dos textboxes WPA/WPA2
                comboBox16_SelectedIndexChanged(comboBox16, EventArgs.Empty);
            }
        }


        private void LimparCamposSecurityProfile()
        {
            textBox26.Clear();
            comboBox17.SelectedIndex = -1;
            comboBox16.SelectedIndex = -1;
            textBox25.Clear(); // WPA
            textBox27.Clear(); // WPA2
            textBox25.Enabled = false;
            textBox27.Enabled = false;

            perfilSelecionado = null;
        }


        private async void button35_Click(object sender, EventArgs e)
        {
            if (perfilSelecionado == null)
            {
                MessageBox.Show("Seleciona um perfil de segurança.");
                return;
            }

            var confirm = MessageBox.Show($"Deseja apagar o perfil '{perfilSelecionado.Name}'?",
                                          "Confirmar eliminação",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"{urlLink}interface/wireless/security-profiles/{perfilSelecionado.Id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Perfil apagado com sucesso.");
                    await CarregarSecurityProfilesAsync(); // Atualiza lista
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao apagar:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao apagar: " + ex.Message);
            }

            LimparCamposSecurityProfile();
        }

        private void PreencherComboboxSecurityProfile()
        {
            // Para edição
            comboBox17.Items.Clear(); // Mode
            comboBox17.Items.AddRange(new object[] { "none", "static-keys-required", "static-keys-optional", "dynamic-keys" });

            comboBox16.Items.Clear(); // Authentication Types
            comboBox16.Items.AddRange(new object[] { "wpa-psk", "wpa2-psk", "wpa-eap", "wpa2-eap", "wpa2-psk,wpa-psk" });

            // Para criação
            comboBox13.Items.Clear();
            comboBox13.Items.AddRange(new object[] { "none", "static-keys-required", "static-keys-optional", "dynamic-keys" });

            comboBox14.Items.Clear();
            comboBox14.Items.AddRange(new object[] { "wpa-psk", "wpa2-psk", "wpa-eap", "wpa2-eap", "wpa2-psk,wpa-psk" });
        }



        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {
            string authType = comboBox16.SelectedItem?.ToString()?.ToLower() ?? "";

            // WPA2-PSK → Ativa WPA2 textbox, desativa WPA
            if (authType.Contains("wpa2-psk") && !authType.Contains("wpa-psk"))
            {
                textBox27.Enabled = true;   // WPA2 Pre-shared key
                textBox25.Enabled = false;  // WPA Pre-shared key
                textBox25.Clear();
            }
            // WPA-PSK → Ativa WPA textbox, desativa WPA2
            else if (authType.Contains("wpa-psk") && !authType.Contains("wpa2-psk"))
            {
                textBox25.Enabled = true;
                textBox27.Enabled = false;
                textBox27.Clear();
            }
            // Ambos → Ativa os dois
            else if (authType.Contains("wpa2-psk") && authType.Contains("wpa-psk"))
            {
                textBox25.Enabled = true;
                textBox27.Enabled = true;
            }
            else
            {
                // Nenhum → Desativa ambos
                textBox25.Enabled = false;
                textBox27.Enabled = false;
                textBox25.Clear();
                textBox27.Clear();
            }
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            string authType = comboBox14.SelectedItem?.ToString()?.ToLower() ?? "";

            if (authType.Contains("wpa2-psk") && !authType.Contains("wpa-psk"))
            {
                textBox23.Enabled = true;   // WPA2 Pre-shared key
                textBox28.Enabled = false;  // WPA Pre-shared key
                textBox28.Clear();
            }
            else if (authType.Contains("wpa-psk") && !authType.Contains("wpa2-psk"))
            {
                textBox28.Enabled = true;
                textBox23.Enabled = false;
                textBox23.Clear();
            }
            else if (authType.Contains("wpa2-psk") && authType.Contains("wpa-psk"))
            {
                textBox28.Enabled = true;
                textBox23.Enabled = true;
            }
            else
            {
                textBox28.Enabled = false;
                textBox23.Enabled = false;
                textBox28.Clear();
                textBox23.Clear();
            }
        }



        public class SecurityProfile
        {
            [JsonPropertyName(".id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("mode")]
            public string Mode { get; set; } = string.Empty;

            [JsonPropertyName("authentication-types")]
            public string AuthenticationTypes { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"Name: {Name} - Mode: {Mode} - Authentication Type: {AuthenticationTypes}";
            }
        }

        private async void button34_Click(object sender, EventArgs e)
        {
            if (perfilSelecionado == null)
            {
                MessageBox.Show("Seleciona um perfil de segurança.");
                return;
            }

            string name = textBox26.Text.Trim();
            string mode = comboBox17.SelectedItem?.ToString() ?? "";
            string authType = comboBox16.SelectedItem?.ToString()?.ToLower() ?? "";

            string wpaKey = textBox25.Text.Trim();
            string wpa2Key = textBox27.Text.Trim();

            var body = new Dictionary<string, string>
            {
                ["name"] = name,
                ["mode"] = mode,
                ["authentication-types"] = authType
            };

            if (authType.Contains("wpa-psk"))
            {
                if (wpaKey.Length < 8 || wpaKey.Length > 64)
                {
                    MessageBox.Show("WPA Pre-Shared Key deve ter entre 8 e 64 caracteres.");
                    return;
                }

                body["wpa-pre-shared-key"] = wpaKey;
            }

            if (authType.Contains("wpa2-psk"))
            {
                if (wpa2Key.Length < 8 || wpa2Key.Length > 64)
                {
                    MessageBox.Show("WPA2 Pre-Shared Key deve ter entre 8 e 64 caracteres.");
                    return;
                }

                body["wpa2-pre-shared-key"] = wpa2Key;
            }

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Patch, $"{urlLink}interface/wireless/security-profiles/{perfilSelecionado.Id}")
                {
                    Content = content
                };

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Perfil atualizado com sucesso.");
                    await CarregarSecurityProfilesAsync();
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

            LimparCamposSecurityProfile();

        }

        private async void button32_Click(object sender, EventArgs e)
        {
            string name = textBox29.Text.Trim();
            string mode = comboBox13.SelectedItem?.ToString() ?? "";
            string authType = comboBox14.SelectedItem?.ToString()?.ToLower() ?? "";
            string wpaKey = textBox28.Text.Trim();
            string wpa2Key = textBox23.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mode) || string.IsNullOrWhiteSpace(authType))
            {
                MessageBox.Show("Preenche todos os campos obrigatórios.");
                return;
            }

            var body = new Dictionary<string, string>
            {
                ["name"] = name,
                ["mode"] = mode,
                ["authentication-types"] = authType
            };

            if (authType.Contains("wpa-psk"))
            {
                if (wpaKey.Length < 8 || wpaKey.Length > 64)
                {
                    MessageBox.Show("WPA Pre-Shared Key deve ter entre 8 e 64 caracteres.");
                    return;
                }
                body["wpa-pre-shared-key"] = wpaKey;
            }

            if (authType.Contains("wpa2-psk"))
            {
                if (wpa2Key.Length < 8 || wpa2Key.Length > 64)
                {
                    MessageBox.Show("WPA2 Pre-Shared Key deve ter entre 8 e 64 caracteres.");
                    return;
                }
                body["wpa2-pre-shared-key"] = wpa2Key;
            }

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PutAsync($"{urlLink}interface/wireless/security-profiles", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Perfil criado com sucesso.");
                    await CarregarSecurityProfilesAsync();
                    // Limpar campos
                    textBox29.Clear();
                    textBox28.Clear();
                    textBox23.Clear();
                    comboBox13.SelectedIndex = -1;
                    comboBox14.SelectedIndex = -1;
                }
                else
                {
                    string erro = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao criar:\n{response.StatusCode}\n{erro}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            LimparCamposSecurityProfile();

        }

    }
    public class AddressPool
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("ranges")] public string Ranges { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Name: {Name} - Addresses: {Ranges}";
        }
    }

    public class Bridge
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }

    public class BridgePort
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("interface")] public string Interface { get; set; } = string.Empty;
        [JsonPropertyName("bridge")] public string Bridge { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Port: {Interface} -> Bridge: {Bridge}";
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


    public class DhcpServer
    {
        [JsonPropertyName(".id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("interface")] public string Interface { get; set; } = string.Empty;
        [JsonPropertyName("lease-time")] public string LeaseTime { get; set; } = string.Empty;
        [JsonPropertyName("address-pool")] public string AddressPool { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} | Interface: {Interface} | Lease Time: {LeaseTime} | Pool: {AddressPool}";
        }
    }





    public class SecurityProfile : Dictionary<string, string>
    {
        public string Id => this.ContainsKey(".id") ? this[".id"] : "";
        public string Name => this.ContainsKey("name") ? this["name"] : "(sem nome)";
    }
}
