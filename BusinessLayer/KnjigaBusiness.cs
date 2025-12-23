using System;
using System.Collections.Generic;
using DataLayer;
using DataLayer.Models;

namespace BussinesLayer
{
    public class KnjigaBussines
    {
        private readonly KnjigaRepository _repo = new KnjigaRepository();

      
        public List<Knjiga> GetAll() => _repo.GetAll();
        public Knjiga? GetById(int id) => _repo.GetById(id);

        
        public int Create(Knjiga k)
        {
            
            if (k == null) throw new ArgumentException("Knjiga nije prosleđena.");
            if (string.IsNullOrWhiteSpace(k.Naslov)) throw new ArgumentException("Naslov je obavezan.");
            if (string.IsNullOrWhiteSpace(k.Autor)) throw new ArgumentException("Autor je obavezan.");
            if (k.Godina is < 0) throw new ArgumentException("Godina ne može biti negativna.");
            

            return _repo.Insert(k);
        }

        
        public bool Update(Knjiga k)
        {
            if (k == null) throw new ArgumentException("Knjiga nije prosleđena.");
            if (k.KnjigaID <= 0) throw new ArgumentException("Nepoznat ID knjige.");
            if (string.IsNullOrWhiteSpace(k.Naslov)) throw new ArgumentException("Naslov je obavezan.");
            if (string.IsNullOrWhiteSpace(k.Autor)) throw new ArgumentException("Autor je obavezan.");
            if (k.Godina is < 0) throw new ArgumentException("Godina ne može biti negativna.");

            return _repo.Update(k);
        }

        
        public bool Delete(int id)
        {
            if (id <= 0) throw new ArgumentException("Nepoznat ID knjige.");
            return _repo.Delete(id);
        }

       
        public List<Knjiga> SearchInMemory(string query)
        {
            query = (query ?? "").Trim().ToLower();
            if (string.IsNullOrEmpty(query)) return GetAll();

            var rez = new List<Knjiga>();
            foreach (var k in GetAll())
            {
                if (k.KnjigaID.ToString() == query ||
                    k.Naslov.ToLower().Contains(query) ||
                    k.Autor.ToLower().Contains(query))
                {
                    rez.Add(k);
                }
            }
            return rez;
        }
    }
}
