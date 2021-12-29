using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionCordinator
{
    class BookTableEntity : TableEntity
    {
        public BookTableEntity(string nazivKnjige, int cena,int brojKnjiga)
        {
            PartitionKey = "Book";
            RowKey = nazivKnjige;
            NazivKnjige = nazivKnjige;
            Cena = cena;
            BrojKnjiga = brojKnjiga;
        }
        public int Id { get; set; }
        public string NazivKnjige { get; set; }
        public int Cena { get; set; }
        public int BrojKnjiga { get; set; }
    }
}
