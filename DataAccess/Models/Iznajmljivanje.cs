using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Iznajmljivanje
    {
        public int IznajmljivanjeID { get; set; }
        public int KnjigaID { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public bool Vracena { get; set; }
    }
}
