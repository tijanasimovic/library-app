using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using PresentationLayer.Properties;


namespace PresentationLayer
{
    public partial class KnjigeForm
    {
        private System.ComponentModel.IContainer components = null;

       
        private Panel panelFormFields;   // gornji deo: labele/texbox/dugmad
        private Panel panelGrid;         // donji deo: grid

        private Label lblID, lblNaslov, lblAutor, lblGodina, lblISBN, lblTrazi;
        private TextBox txtID, txtNaslov, txtAutor, txtGodina, txtISBN, txtTrazi;
        private Button btnDodaj, btnIzmeni, btnObrisi, btnOsvezi, btnTrazi, btnIznajmi, btnIzdavanja;
        private DataGridView dgvKnjige;

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
            Font = new Font("Bookman Old Style", 11F);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Knjige";
            BackColor = Color.White;
            ClientSize = new Size(1150, 720);



            try
            {
                string bg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "knjige.jpg");
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
                Height = 210,
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

           
            lblID = MakeLabel("ID", 20, 20);
            txtID = MakeText(120, 16, 260);

            lblNaslov = MakeLabel("Naslov", 20, 58);
            txtNaslov = MakeText(120, 54, 260);

            lblAutor = MakeLabel("Autor", 20, 96);
            txtAutor = MakeText(120, 92, 260);

            lblGodina = MakeLabel("Godina", 20, 134);
            txtGodina = MakeText(120, 130, 260);

            lblISBN = MakeLabel("ISBN", 20, 172);
            txtISBN = MakeText(120, 168, 260);

            btnDodaj = MakeBtn("Dodaj", 410, 16, 110);
            btnIzmeni = MakeBtn("Izmeni", 410, 56, 110);
            btnObrisi = MakeBtn("Obriši", 410, 96, 110);
            btnOsvezi = MakeBtn("Osveži", 410, 136, 110);
            btnIznajmi = MakeBtn("Iznajmi", 410, 172, 110);

            lblTrazi = MakeLabel("Pretraga", 540, 20);
            txtTrazi = MakeText(630, 16, 190);
            btnTrazi = MakeBtn("Traži", 830, 16, 80);

            btnIzdavanja = MakeBtn("Iznajmljivanja", 540, 56, 170);

            
            panelFormFields.Controls.AddRange(new Control[]
            {
                lblID, txtID, lblNaslov, txtNaslov, lblAutor, txtAutor,
                lblGodina, txtGodina, lblISBN, txtISBN,
                btnDodaj, btnIzmeni, btnObrisi, btnOsvezi, btnIznajmi,
                lblTrazi, txtTrazi, btnTrazi, btnIzdavanja
            });

            
            dgvKnjige = new DataGridView
            {
                Location = new Point(0, 0),
                Size = new Size(panelGrid.Width, panelGrid.Height),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvKnjige.ColumnHeadersDefaultCellStyle.Font = new Font("Bookman Old Style", 12F, FontStyle.Bold);
            dgvKnjige.DefaultCellStyle.Font = new Font("Bookman Old Style", 11F);

           
            dgvKnjige.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            panelGrid.Controls.Add(dgvKnjige);

         
            Controls.Add(panelFormFields);
            Controls.Add(panelGrid);

            
            Resize += (s, e) => CenterPanels();
            Load += (s, e) => CenterPanels();
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
                Size = new Size(w, 29)
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
            if (availableH < 200) availableH = 200;  
            panelGrid.Height = availableH;

           
            int x = (ClientSize.Width - panelFormFields.Width) / 2;
            if (x < 8) x = 8;

            panelFormFields.Location = new Point(x, topMargin);
            panelGrid.Location = new Point(x, panelFormFields.Bottom + middleGap);

         
        }
    }
}
