using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreService
{
    public class BooksData
    {
        public BooksData(int id, string nazivKnjige, int cena, int brojKnjiga)
        {
            Id = id;
            NazivKnjige = nazivKnjige;
            Cena = cena;
            BrojKnjiga = brojKnjiga;
        }
        public BooksData()
        {

        }
        public int Id { get; set; }
        public string NazivKnjige { get; set; }
        public int Cena { get; set; }
        public int BrojKnjiga { get; set; }
    }
}
