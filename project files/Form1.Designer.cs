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

        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            lblAnime = new Label(); lblUpdateHinweis = new Label(); lblBewertung = new Label(); lblStaffelInfo = new Label(); lblFortsetzung = new Label();
            btnUpdate = new Button(); button1 = new Button(); btnSuche = new Button(); btnHinzufuegen = new Button(); btnEntfernen = new Button(); btnNeustarten = new Button();
            txtSuche = new TextBox(); txtBeschreibung = new TextBox(); listBoxErgebnisse = new ListBox(); progressBarBewertung = new ProgressBar(); pictureBoxCover = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).BeginInit();
            SuspendLayout();

            lblAnime.Font = new Font("Segoe UI", 20F, FontStyle.Bold); lblAnime.Location = new Point(20, 20); lblAnime.Size = new Size(600, 40);
            lblUpdateHinweis.Anchor = AnchorStyles.Top | AnchorStyles.Right; lblUpdateHinweis.Location = new Point(980, 20); lblUpdateHinweis.Size = new Size(220, 23);
            lblBewertung.Location = new Point(20, 80); lblBewertung.Size = new Size(300, 23);
            lblStaffelInfo.Location = new Point(20, 140); lblStaffelInfo.Size = new Size(500, 23);
            lblFortsetzung.Location = new Point(20, 165); lblFortsetzung.Size = new Size(500, 23);
            btnUpdate.Anchor = AnchorStyles.Top | AnchorStyles.Right; btnUpdate.Location = new Point(1205, 18); btnUpdate.Size = new Size(75, 27); btnUpdate.Text = "Update"; btnUpdate.Click += btnUpdate_Click;
            button1.Location = new Point(550, 210); button1.Size = new Size(120, 35); button1.Text = "Zufall"; button1.Click += button1_Click;
            btnSuche.Location = new Point(430, 208); btnSuche.Size = new Size(90, 27); btnSuche.Text = "Suchen"; btnSuche.Click += btnSuche_Click;
            btnHinzufuegen.Location = new Point(550, 255); btnHinzufuegen.Size = new Size(120, 30); btnHinzufuegen.Text = "Hinzufügen"; btnHinzufuegen.Click += btnHinzufuegen_Click;
            btnEntfernen.Location = new Point(550, 295); btnEntfernen.Size = new Size(120, 30); btnEntfernen.Text = "Entfernen"; btnEntfernen.Click += btnEntfernen_Click;
            btnNeustarten.Location = new Point(550, 335); btnNeustarten.Size = new Size(120, 30); btnNeustarten.Text = "Neustarten"; btnNeustarten.Click += btnNeustarten_Click;
            txtSuche.Location = new Point(20, 210); txtSuche.Size = new Size(400, 23);
            txtBeschreibung.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; txtBeschreibung.Location = new Point(20, 460); txtBeschreibung.Multiline = true; txtBeschreibung.ScrollBars = ScrollBars.Vertical; txtBeschreibung.Size = new Size(760, 200);
            listBoxErgebnisse.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left; listBoxErgebnisse.Location = new Point(20, 245); listBoxErgebnisse.Size = new Size(500, 199); listBoxErgebnisse.SelectedIndexChanged += listBoxErgebnisse_SelectedIndexChanged;
            progressBarBewertung.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; progressBarBewertung.Location = new Point(20, 105); progressBarBewertung.Size = new Size(500, 23);
            pictureBoxCover.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right; pictureBoxCover.Location = new Point(800, 70); pictureBoxCover.Size = new Size(460, 590); pictureBoxCover.SizeMode = PictureBoxSizeMode.Zoom;

            ClientSize = new Size(1280, 720);
            Controls.AddRange(new Control[] { lblAnime, lblUpdateHinweis, btnUpdate, lblBewertung, progressBarBewertung, lblStaffelInfo, lblFortsetzung, txtSuche, btnSuche, listBoxErgebnisse, txtBeschreibung, button1, btnHinzufuegen, btnEntfernen, btnNeustarten, pictureBoxCover });
            Icon = (Icon)resources.GetObject("$this.Icon"); MinimumSize = new Size(1000, 600); Name = "Form1"; Text = "Anime Randomizer"; Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxCover).EndInit(); ResumeLayout(false); PerformLayout();
        }
    }
}