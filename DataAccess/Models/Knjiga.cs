using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Knjiga
    {
        public int KnjigaID { get; set; }
        public string Naslov { get; set; } = "";
        public string Autor { get; set; } = "";
        public int? Godina { get; set; }
        public string? ISBN { get; set; }
    }
}
