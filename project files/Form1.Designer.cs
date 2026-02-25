namespace Anime_Randomizer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblAnime, lblUpdateHinweis, lblBewertung, lblStaffelInfo, lblFortsetzung;
        private Button btnUpdate, button1, btnSuche, btnHinzufuegen, btnEntfernen, btnNeustarten;
        private TextBox txtSuche, txtBeschreibung;
        private ListBox listBoxErgebnisse;
        private ProgressBar progressBarBewertung;
        private PictureBox pictureBoxCover;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));

            lblAnime = new Label { Font = new Font("Segoe UI", 20F, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(500, 40), Text = "Anime Name" };
            lblUpdateHinweis = new Label { Location = new Point(1080, 680), Size = new Size(180, 20), ForeColor = Color.Orange };
            btnUpdate = new Button { Location = new Point(1080, 650), Size = new Size(100, 25), Text = "Update", Visible = false };
            btnUpdate.Click += btnUpdate_Click;

            lblBewertung = new Label { Location = new Point(20, 80), Size = new Size(300, 25) };
            lblStaffelInfo = new Label { Location = new Point(20, 110), Size = new Size(400, 25) };
            lblFortsetzung = new Label { Location = new Point(20, 140), Size = new Size(300, 25) };

            txtSuche = new TextBox { Location = new Point(20, 180), Size = new Size(500, 23) };
            button1 = new Button { Location = new Point(540, 20), Size = new Size(100, 40), Text = "Zufall" };
            button1.Click += button1_Click;
            btnSuche = new Button { Location = new Point(540, 70), Size = new Size(100, 30), Text = "Suchen" };
            btnSuche.Click += btnSuche_Click;
            btnHinzufuegen = new Button { Location = new Point(540, 110), Size = new Size(100, 30), Text = "Hinzufügen" };
            btnHinzufuegen.Click += btnHinzufuegen_Click;
            btnEntfernen = new Button { Location = new Point(540, 150), Size = new Size(100, 30), Text = "Entfernen" };
            btnEntfernen.Click += btnEntfernen_Click;
            btnNeustarten = new Button { Location = new Point(540, 190), Size = new Size(100, 30), Text = "Neustarten" };
            btnNeustarten.Click += btnNeustarten_Click;

            listBoxErgebnisse = new ListBox { Location = new Point(20, 220), Size = new Size(620, 199) };
            listBoxErgebnisse.SelectedIndexChanged += listBoxErgebnisse_SelectedIndexChanged;

            txtBeschreibung = new TextBox { Location = new Point(20, 440), Size = new Size(620, 200), Multiline = true, ScrollBars = ScrollBars.Vertical };

            progressBarBewertung = new ProgressBar { Location = new Point(20, 400), Size = new Size(300, 25) };
            pictureBoxCover = new PictureBox { Location = new Point(660, 20), Size = new Size(600, 620), SizeMode = PictureBoxSizeMode.Zoom };

            Controls.AddRange(new Control[] { lblAnime, lblUpdateHinweis, btnUpdate, lblBewertung, lblStaffelInfo, lblFortsetzung,
                button1, btnSuche, btnHinzufuegen, btnEntfernen, btnNeustarten, txtSuche, listBoxErgebnisse, txtBeschreibung,
                progressBarBewertung, pictureBoxCover });

            ClientSize = new Size(1280, 720);
            MinimumSize = new Size(800, 450);
            Text = "Anime Randomizer";
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Load += Form1_Load;
        }
    }
}