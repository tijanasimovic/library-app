using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PresentationLayer
{
    partial class IzdavanjeForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelFormFields;   // gornji blok: polja/dugmad/filter
        private Panel panelGrid;         // donji blok: tabela

        private Label lblID, lblKnjiga, lblOd, lblDo, lblVracena, lblDoDatuma, lblRokInfo;
        private TextBox txtID;
        private ComboBox cbKnjiga;
        private DateTimePicker dtpOd, dtpDo, dtpFilterDo;
        private CheckBox chkVracena;
        private Button btnDodaj, btnIzmeni, btnObrisi, btnOsvezi, btnNevracene, btnFiltriraj, btnObrisiDo, btnKnjige, btnFiltrirajNaslov;
        private GroupBox grpFilter;
        private DataGridView dgvIzdavanja;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

           
            AutoScaleMode = AutoScaleMode.Dpi;                 
            AutoScaleDimensions = new SizeF(96F, 96F);
            Font = new Font("Bookman Old Style", 10.5f);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Izdavanje (iznajmljivanja)";
            BackColor = Color.White;
            ClientSize = new Size(1150, 720);

            try
            {
                var bg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "knjige.jpg");
                if (File.Exists(bg))
                {
                    BackgroundImage = Image.FromFile(bg);
                    BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            catch {  }

            
            panelFormFields = new Panel
            {
                Width = 920,
                Height = 240,             
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.None
            };

            panelGrid = new Panel
            {
                Width = 920,
                Height = 440,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.None
            };

          
            lblID = MakeLabel("ID", 20, 24);
            txtID = MakeText(140, 20, 260);

            lblKnjiga = MakeLabel("Knjiga", 20, 62);
            cbKnjiga = new ComboBox
            {
                Location = new Point(140, 58),
                Size = new Size(260, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblOd = MakeLabel("Datum od", 20, 100);
            dtpOd = MakeDate(140, 96);

            lblDo = MakeLabel("Datum do", 20, 138);
            dtpDo = MakeDate(140, 134);

            lblVracena = MakeLabel("Vraćena", 20, 176);
            chkVracena = new CheckBox { Location = new Point(140, 174), Size = new Size(18, 18), BackColor = Color.Transparent };

            btnDodaj = MakeBtn("Dodaj", 430, 20, 120);
            btnIzmeni = MakeBtn("Izmeni", 430, 60, 120);
            btnObrisi = MakeBtn("Obriši", 430, 100, 120);
            btnOsvezi = MakeBtn("Osveži", 430, 140, 120);
            btnNevracene = MakeBtn("Samo nevraćene", 570, 20, 170);
            btnFiltrirajNaslov = MakeBtn("Filtriraj po naslovu", 570, 60, 170);
            panelFormFields.Controls.Add(btnFiltrirajNaslov);


            lblRokInfo = new Label
            {
                AutoSize = true,
                Text = "Prošao je rok za vraćanje",
                ForeColor = Color.DarkRed,
                Font = new Font("Bookman Old Style", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(200, 255, 255, 255),
                Location = new Point(20, 205),
                Visible = false
            };

            // Filter grupa
            grpFilter = new GroupBox
            {
                Text = "Brisanje po datumu vraćanja",
                Location = new Point(570, 100),
                Size = new Size(330, 112),
                BackColor = Color.FromArgb(180, 255, 255, 255)
            };
            lblDoDatuma = new Label { AutoSize = true, Text = "Do datuma", Location = new Point(6, 32), BackColor = Color.Transparent };
            dtpFilterDo = MakeDate(95, 28);
            btnFiltriraj = MakeBtn("Filtriraj", 242, 26, 80);
            btnObrisiDo = MakeBtn("Obriši", 75, 66, 180);

            grpFilter.Controls.AddRange(new Control[] { lblDoDatuma, dtpFilterDo, btnFiltriraj, btnObrisiDo });

           
            panelFormFields.Controls.AddRange(new Control[]
            {
                lblID, txtID, lblKnjiga, cbKnjiga, lblOd, dtpOd, lblDo, dtpDo, lblVracena, chkVracena,
                btnDodaj, btnIzmeni, btnObrisi, btnOsvezi, btnNevracene, lblRokInfo, grpFilter
            });

           
            dgvIzdavanja = new DataGridView
            {
                Location = new Point(0, 0),
                Size = new Size(panelGrid.Width, panelGrid.Height),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            dgvIzdavanja.DefaultCellStyle.Font = new Font("Bookman Old Style", 10.5f);
            dgvIzdavanja.ColumnHeadersDefaultCellStyle.Font = new Font("Bookman Old Style", 11.5f, FontStyle.Bold);
            

            panelGrid.Controls.Add(dgvIzdavanja);

           
            btnKnjige = MakeBtn("← Knjige", panelFormFields.Width - 120, 10, 110);
            btnKnjige.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelFormFields.Controls.Add(btnKnjige);

            Controls.Add(panelFormFields);
            Controls.Add(panelGrid);

            
            Load += (s, e) => CenterPanels();
            Resize += (s, e) => CenterPanels();
        }

       
        private Label MakeLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(x, y),
                BackColor = Color.FromArgb(200, 255, 255, 255) 
            };
        }

        private TextBox MakeText(int x, int y, int w)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 26)
            };
        }

        private DateTimePicker MakeDate(int x, int y)
        {
            return new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(140, 26),
                Format = DateTimePickerFormat.Short
            };
        }

        private Button MakeBtn(string text, int x, int y, int w)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 32),
                FlatStyle = FlatStyle.Standard
            };
        }

        private void CenterPanels()
        {
            int topMargin = 12;
            int middleGap = 16;
            int bottomMargin = 12;

           
            int availableH = ClientSize.Height - topMargin - panelFormFields.Height - middleGap - bottomMargin;
            if (availableH < 220) availableH = 220;
            panelGrid.Height = availableH;

            int x = (ClientSize.Width - panelFormFields.Width) / 2;
            if (x < 8) x = 8;

            panelFormFields.Location = new Point(x, topMargin);
            panelGrid.Location = new Point(x, panelFormFields.Bottom + middleGap);
        }
    }
}
