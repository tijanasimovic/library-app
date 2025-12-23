using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using BussinesLayer;
using DataLayer.Models;

namespace PresentationLayer
{
    
    public class IznajmljivanjeView
    {
        public int IznajmljivanjeID { get; set; }
        public int KnjigaID { get; set; }
        public string Naslov { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public bool Vracena { get; set; }
        public bool Kasni { get; set; }
        public int DanaKasni { get; set; }
    }

    public partial class IzdavanjeForm : Form
    {
        private readonly IznajmljivanjeBusiness _iznajBL = new IznajmljivanjeBusiness();
        private readonly KnjigaBussines _knjigeBL = new KnjigaBussines();

        private const int RokDana = 30;

        public IzdavanjeForm()
        {
            InitializeComponent();

            txtID.ReadOnly = true;
            dgvIzdavanja.AutoGenerateColumns = true;
            dgvIzdavanja.MultiSelect = false;
            dgvIzdavanja.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            chkVracena.CheckedChanged += (s, e) => dtpDo.Enabled = chkVracena.Checked;
            dgvIzdavanja.CellClick += dgvIzdavanja_CellClick;

            btnOsvezi.Click += (s, e) => LoadGrid();
            btnDodaj.Click += btnDodaj_Click;
            btnIzmeni.Click += btnIzmeni_Click;
            btnObrisi.Click += btnObrisi_Click;
            btnNevracene.Click += (s, e) => dgvIzdavanja.DataSource = BuildView(_iznajBL.GetOpen());

            btnFiltriraj.Click += btnFiltriraj_Click;
            btnObrisiDo.Click += btnObrisiDo_Click;

         
            btnKnjige.Click += (s, e) =>
            {
                this.Hide();
                this.Close();
                var f = new KnjigeForm();
                f.ShowDialog();
            };



            this.Load += IzdavanjeForm_Load;
            btnFiltrirajNaslov.Click += (s, e) =>
            {
                if (cbKnjiga.SelectedValue is int knjigaId)
                {
                    var svi = _iznajBL.GetAll();
                    var filtrirani = svi.Where(x => x.KnjigaID == knjigaId);
                    dgvIzdavanja.DataSource = BuildView(filtrirani);

                    if (dgvIzdavanja.Columns["KnjigaID"] != null)
                        dgvIzdavanja.Columns["KnjigaID"].Visible = false;
                }
                else
                {
                    MessageBox.Show("Izaberite knjigu iz liste.");
                }
            };

        }


        private static bool InDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private void IzdavanjeForm_Load(object sender, EventArgs e)
        {
            if (InDesignMode) return;

            LoadKnjige();
            dtpDo.Enabled = chkVracena.Checked;
            dtpFilterDo.Value = DateTime.Today;
            LoadGrid();
        }

        private void LoadKnjige()
        {
            var knjige = _knjigeBL.GetAll();
            cbKnjiga.DisplayMember = "Naslov";
            cbKnjiga.ValueMember = "KnjigaID";
            cbKnjiga.DataSource = knjige;
        }

        private void LoadGrid()
        {
            var list = _iznajBL.GetAll();
            dgvIzdavanja.DataSource = BuildView(list);

           
            if (dgvIzdavanja.Columns["KnjigaID"] != null)
                dgvIzdavanja.Columns["KnjigaID"].Visible = false;
        }

        
        private List<IznajmljivanjeView> BuildView(IEnumerable<Iznajmljivanje> source)
        {
            var dictNaslovi = _knjigeBL.GetAll().ToDictionary(k => k.KnjigaID, k => k.Naslov ?? "(bez naslova)");
            var today = DateTime.Today;

            return source.Select(x =>
            {
                bool kasni = !x.Vracena && today > x.DatumOd.AddDays(RokDana);
                int danaKasni = kasni ? (today - x.DatumOd.AddDays(RokDana)).Days : 0;

                return new IznajmljivanjeView
                {
                    IznajmljivanjeID = x.IznajmljivanjeID,
                    KnjigaID = x.KnjigaID,
                    Naslov = dictNaslovi.TryGetValue(x.KnjigaID, out var n) ? n : $"ID {x.KnjigaID}",
                    DatumOd = x.DatumOd,
                    DatumDo = x.DatumDo,
                    Vracena = x.Vracena,
                    Kasni = kasni,
                    DanaKasni = danaKasni
                };
            }).ToList();
        }

        private void dgvIzdavanja_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvIzdavanja.Rows[e.RowIndex].DataBoundItem is not IznajmljivanjeView v) return;

            txtID.Text = v.IznajmljivanjeID.ToString();
            cbKnjiga.SelectedValue = v.KnjigaID;
            dtpOd.Value = v.DatumOd;

            if (v.DatumDo.HasValue)
            {
                chkVracena.Checked = true;
                dtpDo.Enabled = true;
                dtpDo.Value = v.DatumDo.Value;
            }
            else
            {
                chkVracena.Checked = false;
                dtpDo.Enabled = false;
                dtpDo.Value = DateTime.Today;
            }

           
            if (v.Kasni)
            {
                lblRokInfo.Text = $"Prošao je rok za vraćanje (kasni {v.DanaKasni} dana).";
                lblRokInfo.Visible = true;
            }
            else
            {
                lblRokInfo.Visible = false;
            }
        }

        private void btnDodaj_Click(object sender, EventArgs e)
        {
            int knjigaId = (int)cbKnjiga.SelectedValue;

           
            var aktivna = _iznajBL.GetOpen().Any(x => x.KnjigaID == knjigaId);
            if (aktivna)
            {
                MessageBox.Show("Ova knjiga je već iznajmljena i nije vraćena. Nije moguće dodati novo iznajmljivanje dok se ne vrati.");
                return;
            }

            
            var model = new Iznajmljivanje
            {
                KnjigaID = knjigaId,
                DatumOd = dtpOd.Value.Date,
                DatumDo = chkVracena.Checked ? dtpDo.Value.Date : (DateTime?)null,
                Vracena = chkVracena.Checked
            };

            _iznajBL.Create(model);
            LoadGrid();
            ClearInputs();
        }


        
        private IznajmljivanjeView GetSelectedView()
        {
            if (dgvIzdavanja.CurrentRow?.DataBoundItem is IznajmljivanjeView v)
                return v;
            return null;
        }

        
        private void btnIzmeni_Click(object sender, EventArgs e)
        {
           
            var v = GetSelectedView();

            
            if (v == null)
            {
                if (!int.TryParse(txtID.Text, out var idFromTxt))
                {
                    MessageBox.Show("Izaberite izdavanje u tabeli ili unesite ID.");
                    return;
                }

           
                var modelFallback = new Iznajmljivanje
                {
                    IznajmljivanjeID = idFromTxt,
                    KnjigaID = (int)cbKnjiga.SelectedValue,
                    DatumOd = dtpOd.Value.Date,
                    DatumDo = DateTime.Today,   
                    Vracena = true              
                };

                _iznajBL.Update(modelFallback);
                LoadGrid();
                return;
            }

            
            if (v.Vracena)
            {
                MessageBox.Show("Ovo iznajmljivanje je već označeno kao vraćeno.");
                return;
            }

           
            var model = new Iznajmljivanje
            {
                IznajmljivanjeID = v.IznajmljivanjeID,
                KnjigaID = v.KnjigaID,
                DatumOd = v.DatumOd,
                DatumDo = DateTime.Today,   
                Vracena = true
            };

            _iznajBL.Update(model);
            LoadGrid();

            
            lblRokInfo.Visible = false;
            dtpDo.Enabled = true;
            dtpDo.Value = DateTime.Today;
            chkVracena.Checked = true;
        }


        private void btnObrisi_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtID.Text, out var id))
            {
                MessageBox.Show("Izaberite zapis.");
                return;
            }
            if (MessageBox.Show("Obrisati ovaj zapis?", "Potvrda", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            _iznajBL.Delete(id);
            LoadGrid();
            ClearInputs();
        }

      
        private void btnFiltriraj_Click(object sender, EventArgs e)
        {
            var to = dtpFilterDo.Value.Date;
            dgvIzdavanja.DataSource = BuildView(_iznajBL.GetReturnedUntil(to));
            if (dgvIzdavanja.Columns["KnjigaID"] != null)
                dgvIzdavanja.Columns["KnjigaID"].Visible = false;
        }

        
        private void btnObrisiDo_Click(object sender, EventArgs e)
        {
            var to = dtpFilterDo.Value.Date;
            var count = _iznajBL.GetReturnedUntil(to).Count;

            if (count == 0)
            {
                MessageBox.Show("Nema vraćenih iznajmljivanja do izabranog datuma.");
                return;
            }

            if (MessageBox.Show($"Obrisati SVA iznajmljivanja koja su vraćena do {to:d}? (Ukupno: {count})",
                "Potvrda brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var deleted = _iznajBL.DeleteReturnedUntil(to);
            MessageBox.Show($"Obrisano zapisa: {deleted}");
            LoadGrid();
        }

        private void ClearInputs()
        {
            txtID.Clear();
            if (cbKnjiga.Items.Count > 0) cbKnjiga.SelectedIndex = 0;
            dtpOd.Value = DateTime.Today;
            chkVracena.Checked = false;
            dtpDo.Enabled = false;
            dtpDo.Value = DateTime.Today;
            lblRokInfo.Visible = false;
        }

    }
}
