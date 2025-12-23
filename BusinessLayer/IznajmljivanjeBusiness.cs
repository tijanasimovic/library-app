using System;
using System.Collections.Generic;
using DataLayer;
using DataLayer.Models;

namespace BussinesLayer
{
    public class IznajmljivanjeBusiness
    {
        private readonly IznajmljivanjeRepository _repo = new IznajmljivanjeRepository();

        public List<Iznajmljivanje> GetAll() => _repo.GetAll();
        public List<Iznajmljivanje> GetOpen() => _repo.GetOpen();

        public bool ExistsOpenForBook(int knjigaId) => _repo.ExistsOpenForBook(knjigaId);

        public int Create(Iznajmljivanje x)
        {
            if (x.DatumOd == default) x.DatumOd = DateTime.Today;
            return _repo.Insert(x);
        }

        public bool Update(Iznajmljivanje x) => _repo.Update(x);
        public bool Delete(int id) => _repo.Delete(id);
        public List<Iznajmljivanje> GetReturnedUntil(DateTime toInclusive) =>
          _repo.GetReturnedUntil(toInclusive);

        public int DeleteReturnedUntil(DateTime toInclusive) =>
            _repo.DeleteReturnedUntil(toInclusive);
    }
}

