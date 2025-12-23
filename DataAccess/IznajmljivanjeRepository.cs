using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DataLayer.Models;
using Shared;

namespace DataLayer
{
    public class IznajmljivanjeRepository
    {
        public List<Iznajmljivanje> GetAll()
        {
            var list = new List<Iznajmljivanje>();
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT IznajmljivanjeID, KnjigaID, DatumOd, DatumDo, Vracena FROM Iznajmljivanje", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Iznajmljivanje
                {
                    IznajmljivanjeID = r.GetInt32(0),
                    KnjigaID = r.GetInt32(1),
                    DatumOd = r.GetDateTime(2),
                    DatumDo = r.IsDBNull(3) ? (DateTime?)null : r.GetDateTime(3),
                    Vracena = r.GetBoolean(4)
                });
            }
            return list;
        }

        public List<Iznajmljivanje> GetOpen()
        {
            var list = new List<Iznajmljivanje>();
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT IznajmljivanjeID, KnjigaID, DatumOd, DatumDo, Vracena " +
                "FROM Iznajmljivanje WHERE Vracena = 0", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Iznajmljivanje
                {
                    IznajmljivanjeID = r.GetInt32(0),
                    KnjigaID = r.GetInt32(1),
                    DatumOd = r.GetDateTime(2),
                    DatumDo = r.IsDBNull(3) ? (DateTime?)null : r.GetDateTime(3),
                    Vracena = r.GetBoolean(4)
                });
            }
            return list;
        }

        public bool ExistsOpenForBook(int knjigaId)
        {
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM Iznajmljivanje WHERE KnjigaID = @id AND Vracena = 0", conn);
            cmd.Parameters.AddWithValue("@id", knjigaId);
            var count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public int Insert(Iznajmljivanje x)
        {
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO Iznajmljivanje (KnjigaID, DatumOd, DatumDo, Vracena) " +
                "OUTPUT INSERTED.IznajmljivanjeID VALUES (@k,@od,@do,@vr)", conn);
            cmd.Parameters.AddWithValue("@k", x.KnjigaID);
            cmd.Parameters.AddWithValue("@od", x.DatumOd);
            cmd.Parameters.AddWithValue("@do", (object?)x.DatumDo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@vr", x.Vracena);
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Iznajmljivanje x)
        {
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Iznajmljivanje SET KnjigaID=@k, DatumOd=@od, DatumDo=@do, Vracena=@vr " +
                "WHERE IznajmljivanjeID=@id", conn);
            cmd.Parameters.AddWithValue("@id", x.IznajmljivanjeID);
            cmd.Parameters.AddWithValue("@k", x.KnjigaID);
            cmd.Parameters.AddWithValue("@od", x.DatumOd);
            cmd.Parameters.AddWithValue("@do", (object?)x.DatumDo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@vr", x.Vracena);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Iznajmljivanje WHERE IznajmljivanjeID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
        public List<Iznajmljivanje> GetReturnedUntil(DateTime toInclusive)
        {
            var list = new List<Iznajmljivanje>();
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT IznajmljivanjeID, KnjigaID, DatumOd, DatumDo, Vracena
                  FROM Iznajmljivanje
                  WHERE Vracena = 1 AND DatumDo IS NOT NULL AND CAST(DatumDo AS date) <= @to", conn);
            cmd.Parameters.AddWithValue("@to", toInclusive.Date);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Iznajmljivanje
                {
                    IznajmljivanjeID = r.GetInt32(0),
                    KnjigaID = r.GetInt32(1),
                    DatumOd = r.GetDateTime(2),
                    DatumDo = r.IsDBNull(3) ? (DateTime?)null : r.GetDateTime(3),
                    Vracena = r.GetBoolean(4)
                });
            }
            return list;
        }

        public int DeleteReturnedUntil(DateTime toInclusive)
        {
            using var conn = new SqlConnection(Konekcija.connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"DELETE FROM Iznajmljivanje
                  WHERE Vracena = 1 AND DatumDo IS NOT NULL AND CAST(DatumDo AS date) <= @to", conn);
            cmd.Parameters.AddWithValue("@to", toInclusive.Date);
            return cmd.ExecuteNonQuery();
        }
    }
}

