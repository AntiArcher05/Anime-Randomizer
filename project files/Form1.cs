using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace Anime_Randomizer
{
    public partial class Form1 : Form
    {
        List<string> Animes = new List<string>();
        Random zufall = new Random();
        string dateiPfad;
        HttpClient client = new HttpClient();
        Dictionary<string, JObject> animeCache = new Dictionary<string, JObject>();

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(1280, 720);
            this.MinimumSize = new Size(800, 450);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            dateiPfad = Path.Combine(Application.StartupPath, "animes.txt");
            if (!File.Exists(dateiPfad)) File.WriteAllText(dateiPfad, "");
            Animes = File.ReadAllLines(dateiPfad).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

            DarkMode();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await CheckForUpdatesAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using HttpClient c = new HttpClient();
                c.DefaultRequestHeaders.UserAgent.ParseAdd("AnimeRandomizerUpdater");
                string apiUrl = "https://api.github.com/repos/AntiArcher05/Anime-Randomizer/releases/latest";
                var json = JObject.Parse(await c.GetStringAsync(apiUrl));
                string latestTag = json["tag_name"]?.ToString() ?? "";
                string latestVersionString = latestTag.Split('-').Last();
                Version current = Assembly.GetExecutingAssembly().GetName().Version;
                Version latest = new Version(latestVersionString);

                string downloadUrl = json["assets"]?.First?["browser_download_url"]?.ToString()
                                     ?? $"https://github.com/AntiArcher05/Anime-Randomizer/archive/refs/tags/{latestTag}.zip";
                btnUpdate.Tag = downloadUrl;

                if (latest > current)
                {
                    lblUpdateHinweis.Text = $"Update {latestVersionString} verfügbar!";
                    lblUpdateHinweis.ForeColor = Color.Orange;
                    btnUpdate.Visible = true;
                }
                else
                {
                    lblUpdateHinweis.Text = "Kein Update verfügbar";
                    lblUpdateHinweis.ForeColor = Color.LightGreen;
                    btnUpdate.Visible = false;
                }
            }
            catch
            {
                lblUpdateHinweis.Text = "Updateprüfung fehlgeschlagen ❌";
                lblUpdateHinweis.ForeColor = Color.Red;
                btnUpdate.Visible = false;
            }
            lblUpdateHinweis.Visible = true;
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string downloadUrl = btnUpdate.Tag?.ToString() ?? "";
            if (string.IsNullOrEmpty(downloadUrl)) return;
            string tempPath = Path.Combine(Path.GetTempPath(), "AnimeRandomizerUpdate.zip");

            try
            {
                using var stream = await client.GetStreamAsync(downloadUrl);
                using var fs = File.Create(tempPath);
                await stream.CopyToAsync(fs);
                MessageBox.Show($"Update heruntergeladen:\n{tempPath}\nBitte entpacken und starten.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Herunterladen: {ex.Message}");
            }
        }

        private void DarkMode()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            foreach (Control c in this.Controls)
            {
                c.ForeColor = Color.White;
                if (c is Button b) b.BackColor = Color.FromArgb(50, 50, 50);
                if (c is TextBox t) t.BackColor = Color.FromArgb(40, 40, 40);
            }

            listBoxErgebnisse.BackColor = Color.FromArgb(30, 30, 30);
            listBoxErgebnisse.ForeColor = Color.White;
            listBoxErgebnisse.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxErgebnisse.DrawItem += listBoxErgebnisse_DrawItem;

            progressBarBewertung.Maximum = 100;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var gültigeAnimes = Animes.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            if (!gültigeAnimes.Any())
            {
                MessageBox.Show("Keine Anime vorhanden!");
                return;
            }
            string anime = gültigeAnimes[zufall.Next(gültigeAnimes.Count)];
            lblAnime.Text = anime;
            await LadeAnimeDetails(anime);
        }

        private async void btnSuche_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSuche.Text)) return;
            await SucheAnime(txtSuche.Text);
        }

        private async Task SucheAnime(string name)
        {
            string query = @"query ($search: String) { Page(perPage: 5) { media(search: $search, type: ANIME) { title { romaji } } } }";
            var payload = new { query, variables = new { search = name } };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://graphql.anilist.co", content);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            listBoxErgebnisse.Items.Clear();
            if (json["data"]?["Page"]?["media"] is JArray mediaArray)
            {
                foreach (var item in mediaArray)
                {
                    var titel = item["title"]?["romaji"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(titel)) listBoxErgebnisse.Items.Add(titel);
                }
            }
        }

        private async void listBoxErgebnisse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxErgebnisse.SelectedItem == null) return;
            string anime = listBoxErgebnisse.SelectedItem.ToString();
            lblAnime.Text = anime;
            await LadeAnimeDetails(anime);
        }

        private void listBoxErgebnisse_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color foreColor = selected ? Color.Black : Color.White;
            Color backColor = selected ? Color.White : Color.FromArgb(30, 30, 30);
            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            using (SolidBrush textBrush = new SolidBrush(foreColor))
                e.Graphics.DrawString(listBoxErgebnisse.Items[e.Index].ToString(), e.Font, textBrush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private async Task LadeAnimeDetails(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            if (!animeCache.TryGetValue(name, out JObject media))
            {
                string query = @"query ($search: String) { Media(search: $search, type: ANIME) {
                                    description(asHtml: false) averageScore episodes season seasonYear coverImage { large } relations { edges { relationType } } } }";
                var payload = new { query, variables = new { search = name } };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://graphql.anilist.co", content);
                var token = JObject.Parse(await response.Content.ReadAsStringAsync()).SelectToken("data.Media");
                if (token == null) return;
                media = (JObject)token;
                animeCache[name] = media;
            }

            txtBeschreibung.Text = Regex.Replace(media["description"]?.ToString() ?? "", "<.*?>", "");
            int score = media["averageScore"]?.ToObject<int?>() ?? 0;
            progressBarBewertung.Value = score;
            lblBewertung.Text = $"Bewertung: {score}/100";

            int episodes = media["episodes"]?.ToObject<int?>() ?? 0;
            string season = media["season"]?.ToString() ?? "Unbekannt";
            string year = media["seasonYear"]?.ToString() ?? "";
            lblStaffelInfo.Text = $"Staffel: {season} {year} | Episoden: {episodes}";

            bool hatFortsetzung = media["relations"]?["edges"]?.Any(e => e["relationType"]?.ToString() == "SEQUEL") == true;
            lblFortsetzung.Text = hatFortsetzung ? "Weitere Staffel vorhanden ✅" : "Keine weitere Staffel ❌";

            string img = media["coverImage"]?["large"]?.ToString();
            if (!string.IsNullOrEmpty(img))
            {
                try
                {
                    using var stream = await client.GetStreamAsync(img);
                    pictureBoxCover.Image = Image.FromStream(stream);
                }
                catch { }
            }
        }

        private void btnHinzufuegen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblAnime.Text) && !Animes.Contains(lblAnime.Text))
            {
                Animes.Add(lblAnime.Text);
                File.WriteAllLines(dateiPfad, Animes);
                MessageBox.Show("Hinzugefügt!");
            }
        }

        private void btnEntfernen_Click(object sender, EventArgs e)
        {
            if (Animes.Contains(lblAnime.Text))
            {
                Animes.Remove(lblAnime.Text);
                File.WriteAllLines(dateiPfad, Animes);
                MessageBox.Show("Entfernt!");
            }
        }

        private void btnNeustarten_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath);
            Application.Exit();
        }
    }
}