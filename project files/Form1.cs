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
using System.Drawing.Drawing2D;

namespace Anime_Randomizer
{
    public partial class Form1 : Form
    {
        List<string> Animes = new List<string>();
        Random zufall = new Random();
        string dateiPfad;
        HttpClient client = new HttpClient();
        Dictionary<string, JObject> animeCache = new Dictionary<string, JObject>();
        private Color coverBorderColor = Color.Gray;
        private string aktuelleCoverUrl = "";

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(800, 450);
            AutoScaleMode = AutoScaleMode.Dpi;

            dateiPfad = Path.Combine(Application.StartupPath, "animes.txt");
            if (!File.Exists(dateiPfad)) File.WriteAllText(dateiPfad, "");
            Animes = File.ReadAllLines(dateiPfad).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

            SetupAnchors();
            DarkMode();
            pictureBoxCover.Paint += pictureBoxCover_Paint;
            ResizeEnd += (s, e) => pictureBoxCover.Invalidate();
            txtSuche.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = SucheAnime(txtSuche.Text); } };
        }

        private void SetupAnchors()
        {
            var topLeft = AnchorStyles.Top | AnchorStyles.Left;
            var topRight = AnchorStyles.Top | AnchorStyles.Right;
            pictureBoxCover.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            listBoxErgebnisse.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtBeschreibung.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblAnime.Anchor = topLeft;
            button1.Anchor = topLeft;
            btnSuche.Anchor = topLeft;
            btnHinzufuegen.Anchor = topLeft;
            btnEntfernen.Anchor = topLeft;
            btnNeustarten.Anchor = topLeft;
            lblUpdateHinweis.Anchor = topRight;
            btnUpdate.Anchor = topRight;
        }

        private void DarkMode()
        {
            BackColor = Color.FromArgb(25, 25, 30);
            foreach (Control c in Controls)
            {
                c.ForeColor = c is Label ? Color.WhiteSmoke : Color.White;
                if (c is Button b) { b.BackColor = Color.FromArgb(45, 45, 50); b.FlatStyle = FlatStyle.Flat; b.FlatAppearance.BorderSize = 0; b.MouseEnter += (s, e) => b.BackColor = Color.FromArgb(70, 70, 80); b.MouseLeave += (s, e) => b.BackColor = Color.FromArgb(45, 45, 50); }
                if (c is TextBox t) { t.BackColor = Color.FromArgb(35, 35, 40); t.BorderStyle = BorderStyle.FixedSingle; t.ForeColor = Color.White; }
            }
            listBoxErgebnisse.BackColor = Color.FromArgb(30, 30, 40);
            listBoxErgebnisse.ForeColor = Color.White;
            listBoxErgebnisse.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxErgebnisse.DrawItem += listBoxErgebnisse_DrawItem;
            progressBarBewertung.Maximum = 100;
            progressBarBewertung.ForeColor = Color.Orange;
            progressBarBewertung.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void pictureBoxCover_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int borderWidth = 6, radius = 10;
            Rectangle rect = new Rectangle(borderWidth / 2, borderWidth / 2, pictureBoxCover.Width - borderWidth, pictureBoxCover.Height - borderWidth);
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                e.Graphics.FillPath(new SolidBrush(Color.FromArgb(35, 35, 40)), path);

                if (pictureBoxCover.Image != null)
                {
                    float imgRatio = (float)pictureBoxCover.Image.Width / pictureBoxCover.Image.Height;
                    float boxRatio = (float)pictureBoxCover.Width / pictureBoxCover.Height;
                    int drawWidth, drawHeight;
                    if (imgRatio > boxRatio) { drawHeight = pictureBoxCover.Height; drawWidth = (int)(drawHeight * imgRatio); }
                    else { drawWidth = pictureBoxCover.Width; drawHeight = (int)(drawWidth / imgRatio); }
                    int x = (pictureBoxCover.Width - drawWidth) / 2;
                    int y = (pictureBoxCover.Height - drawHeight) / 2;
                    e.Graphics.SetClip(path);
                    e.Graphics.DrawImage(pictureBoxCover.Image, new Rectangle(x, y, drawWidth, drawHeight));
                    e.Graphics.ResetClip();
                }
                using (Pen pen = new Pen(coverBorderColor == Color.Transparent ? Color.Gray : coverBorderColor, borderWidth))
                    e.Graphics.DrawPath(pen, path);
            }
        }

        private async Task LadeCoverBild(string url) { if (string.IsNullOrEmpty(url)) return; try { using var stream = await client.GetStreamAsync(url); pictureBoxCover.Image = Image.FromStream(stream); } catch { } pictureBoxCover.Invalidate(); }

        private async void Form1_Load(object sender, EventArgs e) { await CheckForUpdatesAsync(); }

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
                string downloadUrl = json["assets"]?.First?["browser_download_url"]?.ToString() ?? $"https://github.com/AntiArcher05/Anime-Randomizer/archive/refs/tags/{latestTag}.zip";
                btnUpdate.Tag = downloadUrl;

                if (latest > current) { lblUpdateHinweis.Text = $"Update {latestVersionString} verfügbar!"; lblUpdateHinweis.ForeColor = Color.Orange; btnUpdate.Visible = true; }
                else { lblUpdateHinweis.Text = "Kein Update verfügbar"; lblUpdateHinweis.ForeColor = Color.LightGreen; btnUpdate.Visible = false; }
            }
            catch { lblUpdateHinweis.Text = "Updateprüfung fehlgeschlagen ❌"; lblUpdateHinweis.ForeColor = Color.Red; btnUpdate.Visible = false; }
            lblUpdateHinweis.Visible = true;
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            string downloadUrl = btnUpdate.Tag?.ToString() ?? ""; if (string.IsNullOrEmpty(downloadUrl)) return;
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "AnimeRandomizerUpdate.zip");
            try { using var stream = await client.GetStreamAsync(downloadUrl); using var fs = File.Create(downloadPath); await stream.CopyToAsync(fs); MessageBox.Show($"Update heruntergeladen:\n{downloadPath}\nBitte entpacken und starten."); }
            catch (Exception ex) { MessageBox.Show($"Fehler beim Herunterladen: {ex.Message}"); }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var gültigeAnimes = Animes.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
            if (!gültigeAnimes.Any()) { MessageBox.Show("Keine Anime vorhanden!"); return; }
            lblAnime.Text = gültigeAnimes[zufall.Next(gültigeAnimes.Count)];
            await LadeAnimeDetails(lblAnime.Text);
        }

        private async Task LadeAnimeDetails(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (!animeCache.TryGetValue(name, out JObject media))
            {
                string query = @"query ($search: String) { Media(search: $search, type: ANIME) { description(asHtml: false) averageScore episodes season seasonYear coverImage { large extraLarge } relations { edges { relationType } } } }";
                var payload = new { query, variables = new { search = name } };
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://graphql.anilist.co", content);
                var token = JObject.Parse(await response.Content.ReadAsStringAsync()).SelectToken("data.Media");
                if (token == null) return;
                media = (JObject)token; animeCache[name] = media;
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
            string img = media["coverImage"]?["extraLarge"]?.ToString() ?? media["coverImage"]?["large"]?.ToString();
            aktuelleCoverUrl = img; await LadeCoverBild(img);
            coverBorderColor = Animes.Contains(name) ? Color.Red : Color.Green;
            pictureBoxCover.Invalidate();
        }

        private void btnHinzufuegen_Click(object sender, EventArgs e) { if (!string.IsNullOrWhiteSpace(lblAnime.Text) && !Animes.Contains(lblAnime.Text)) { Animes.Add(lblAnime.Text); File.WriteAllLines(dateiPfad, Animes); MessageBox.Show("Hinzugefügt!"); coverBorderColor = Color.Red; pictureBoxCover.Invalidate(); } }
        private void btnEntfernen_Click(object sender, EventArgs e) { if (Animes.Contains(lblAnime.Text)) { Animes.Remove(lblAnime.Text); File.WriteAllLines(dateiPfad, Animes); MessageBox.Show("Entfernt!"); coverBorderColor = Color.Green; pictureBoxCover.Invalidate(); } }
        private void btnNeustarten_Click(object sender, EventArgs e) { Process.Start(Application.ExecutablePath); Application.Exit(); }
        private async void btnSuche_Click(object sender, EventArgs e) { if (!string.IsNullOrWhiteSpace(txtSuche.Text)) await SucheAnime(txtSuche.Text); }

        private async Task SucheAnime(string name)
        {
            string query = @"query ($search: String) { Page(perPage: 50) { media(search: $search, type: ANIME) { title { romaji } } } }";
            var payload = new { query, variables = new { search = name } };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://graphql.anilist.co", content);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            listBoxErgebnisse.Items.Clear();
            if (json["data"]?["Page"]?["media"] is JArray mediaArray)
                foreach (var item in mediaArray) { var titel = item["title"]?["romaji"]?.ToString(); if (!string.IsNullOrWhiteSpace(titel)) listBoxErgebnisse.Items.Add(titel); }
        }

        private void listBoxErgebnisse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxErgebnisse.SelectedItem == null) return;
            lblAnime.Text = listBoxErgebnisse.SelectedItem.ToString();
            _ = LadeAnimeDetails(lblAnime.Text);
        }

        private void listBoxErgebnisse_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color foreColor = selected ? Color.White : Color.LightGray;
            Color backColor = selected ? Color.FromArgb(70, 70, 80) : Color.FromArgb(30, 30, 40);
            using (SolidBrush b = new SolidBrush(backColor)) e.Graphics.FillRectangle(b, e.Bounds);
            using (SolidBrush t = new SolidBrush(foreColor)) e.Graphics.DrawString(listBoxErgebnisse.Items[e.Index].ToString(), e.Font, t, e.Bounds.X + 5, e.Bounds.Y + 2);
            e.DrawFocusRectangle();
        }
    }
}