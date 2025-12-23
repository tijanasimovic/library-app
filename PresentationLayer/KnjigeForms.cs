using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BussinesLayer;
using DataLayer.Models;

namespace PresentationLayer
{
    public partial class KnjigeForm : Form
    {
        private readonly KnjigaBussines _knjigeBL = new KnjigaBussines();
        private readonly IznajmljivanjeBusiness _iznajBL = new IznajmljivanjeBusiness();

        public KnjigeForm()
        {
            InitializeComponent();

           
            txtID.ReadOnly = true;
            dgvKnjige.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKnjige.MultiSelect = false;
            dgvKnjige.AutoGenerateColumns = true;

           
            dgvKnjige.CellClick += dgvKnjige_CellClick;
            btnOsvezi.Click += (s, e) => LoadGrid();
            btnTrazi.Click += btnTrazi_Click;
            btnDodaj.Click += btnDodaj_Click;
            btnIzmeni.Click += btnIzmeni_Click;
            btnObrisi.Click += btnObrisi_Click;

            btnIznajmi.Click += btnIznajmi_Click;      
            btnIzdavanja.Click += btnIzdavanja_Click;    

            this.Load += KnjigeForm_Load;
        }

        private static bool InDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private void KnjigeForm_Load(object sender, EventArgs e)
        {
            if (InDesignMode) return;
            LoadGrid();
        }

        private void LoadGrid()
        {
            dgvKnjige.DataSource = null;
            dgvKnjige.DataSource = _knjigeBL.GetAll();
        }

        private void dgvKnjige_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvKnjige.Rows[e.RowIndex].DataBoundItem is not Knjiga k) return;

            txtID.Text = k.KnjigaID.ToString();
            txtNaslov.Text = k.Naslov;
            txtAutor.Text = k.Autor;
            txtGodina.Text = k.Godina?.ToString() ?? "";
            txtISBN.Text = k.ISBN ?? "";
        }

        private void btnTrazi_Click(object sender, EventArgs e)
        {
            var q = txtTrazi.Text.Trim();
            dgvKnjige.DataSource = string.IsNullOrWhiteSpace(q)
                ? _knjigeBL.GetAll()
                : _knjigeBL.SearchInMemory(q);
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            var k = new Knjiga
            {
                Naslov = txtNaslov.Text.Trim(),
                Autor = txtAutor.Text.Trim(),
                Godina = string.IsNullOrWhiteSpace(txtGodina.Text) ? null : int.Parse(txtGodina.Text),
                ISBN = string.IsNullOrWhiteSpace(txtISBN.Text) ? null : txtISBN.Text.Trim()
            };
            _knjigeBL.Create(k);
            LoadGrid();
            ClearInputs();
        }

        private void btnIzmeni_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out var id)) { MessageBox.Show("Izaberite knjigu."); return; }
            var k = new Knjiga
            {
                KnjigaID = id,
                Naslov = txtNaslov.Text.Trim(),
                Autor = txtAutor.Text.Trim(),
                Godina = string.IsNullOrWhiteSpace(txtGodina.Text) ? null : int.Parse(txtGodina.Text),
                ISBN = string.IsNullOrWhiteSpace(txtISBN.Text) ? null : txtISBN.Text.Trim()
            };
            _knjigeBL.Update(k);
            LoadGrid();
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out var id)) { MessageBox.Show("Izaberite knjigu."); return; }
            if (MessageBox.Show("Obrisati zapis?", "Potvrda", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            _knjigeBL.Delete(id);
            LoadGrid();
            ClearInputs();
        }

       
        private void btnIznajmi_Click(object sender, EventArgs e)
        {
            if (dgvKnjige.CurrentRow?.DataBoundItem is not Knjiga k)
            {
                MessageBox.Show("Prvo izaberite knjigu.");
                return;
            }

           
            if (_iznajBL.ExistsOpenForBook(k.KnjigaID))
            {
                MessageBox.Show("Knjiga trenutno nije dostupna (već je iznajmljena).");
                return;
            }

            
            var x = new Iznajmljivanje
            {
                KnjigaID = k.KnjigaID,
                DatumOd = DateTime.Today,
                DatumDo = null,
                Vracena = false
            };
            _iznajBL.Create(x);

            MessageBox.Show("Knjiga je uspešno iznajmljena za današnji datum.");
        }


        private void btnIzdavanja_Click(object sender, EventArgs e)
        {
            this.Hide(); 

            using (var f = new IzdavanjeForm())
            {
                f.ShowDialog();
            }

            this.Close(); 
        }


        private void ClearInputs()
        {
            txtID.Clear(); txtNaslov.Clear(); txtAutor.Clear(); txtGodina.Clear(); txtISBN.Clear();
            txtNaslov.Focus();
        }
    }
}
