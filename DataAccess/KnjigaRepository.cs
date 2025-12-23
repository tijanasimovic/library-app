using System.Data;
using DataLayer.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataLayer
{
    public class KnjigaRepository
    {
        public List<Knjiga> GetAll()
        {
            List<Knjiga> lista = new List<Knjiga>();

            using (SqlConnection sqlConnection = new SqlConnection(Konekcija.connString))
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand("SELECT KnjigaID, Naslov, Autor, Godina, ISBN FROM Knjiga", sqlConnection);
                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    lista.Add(new Knjiga
                    {
                        KnjigaID = (int)r["KnjigaID"],
                        Naslov = r["Naslov"].ToString()!,
                        Autor = r["Autor"].ToString()!,
                        Godina = r["Godina"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["Godina"]),
                        ISBN = r["ISBN"] as string
                    });
                }
            }

            return lista;
        }

        public int Insert(Knjiga k)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Konekcija.connString))
            {
                sqlConnection.Open();

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Knjiga (Naslov, Autor, Godina, ISBN)
                    OUTPUT INSERTED.KnjigaID
                    VALUES (@n,@a,@g,@i)", sqlConnection);

                cmd.Parameters.AddWithValue("@n", k.Naslov);
                cmd.Parameters.AddWithValue("@a", k.Autor);
                cmd.Parameters.AddWithValue("@g", (object?)k.Godina ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@i", (object?)k.ISBN ?? DBNull.Value);

                return (int)cmd.ExecuteScalar();
            }
        }
        public Knjiga? GetById(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(Konekcija.connString))
            {
                sqlConnection.Open();

                string q = "SELECT KnjigaID, Naslov, Autor, Godina, ISBN FROM Knjiga WHERE KnjigaID=@id";
                SqlCommand cmd = new SqlCommand(q, sqlConnection);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataReader r = cmd.ExecuteReader();
                if (!r.Read()) return null;

                return new Knjiga
                {
                    KnjigaID = (int)r["KnjigaID"],
                    Naslov = r["Naslov"].ToString()!,
                    Autor = r["Autor"].ToString()!,
                    Godina = r["Godina"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["Godina"]),
                    ISBN = r["ISBN"] as string
                };
            }
        }
        public bool Update(Knjiga k)
        {
            using (SqlConnection con = new SqlConnection(Konekcija.connString))
            {
                con.Open();
                string q = @"
                    UPDATE Knjiga
                    SET Naslov=@n, Autor=@a, Godina=@g, ISBN=@i
                    WHERE KnjigaID=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", k.KnjigaID);
                cmd.Parameters.AddWithValue("@n", k.Naslov);
                cmd.Parameters.AddWithValue("@a", k.Autor);
                cmd.Parameters.AddWithValue("@g", (object?)k.Godina ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@i", (object?)k.ISBN ?? DBNull.Value);

                return cmd.ExecuteNonQuery() == 1;
            }
        }

       
        public bool Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(Konekcija.connString))
            {
                con.Open();
                string q = "DELETE FROM Knjiga WHERE KnjigaID=@id";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@id", id);

                try { return cmd.ExecuteNonQuery() == 1; }
                catch (SqlException) { return false; } // FK constraint
            }
        }

      
        public List<Knjiga> Search(string q)
        {
            var lista = new List<Knjiga>();
            using (SqlConnection con = new SqlConnection(Konekcija.connString))
            {
                con.Open();
                SqlCommand cmd;
                if (int.TryParse(q, out var id))
                {
                    cmd = new SqlCommand("SELECT KnjigaID,Naslov,Autor,Godina,ISBN FROM Knjiga WHERE KnjigaID=@id", con);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        SELECT KnjigaID,Naslov,Autor,Godina,ISBN
                        FROM Knjiga
                        WHERE Naslov LIKE @q OR Autor LIKE @q", con);
                    cmd.Parameters.AddWithValue("@q", "%" + q + "%");
                }

                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    lista.Add(new Knjiga
                    {
                        KnjigaID = (int)r["KnjigaID"],
                        Naslov = r["Naslov"].ToString()!,
                        Autor = r["Autor"].ToString()!,
                        Godina = r["Godina"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["Godina"]),
                        ISBN = r["ISBN"] as string
                    });
                }
            }
            return lista;
        }
    }
}