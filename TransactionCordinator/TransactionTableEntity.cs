using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;

namespace TransactionCordinator
{
    public class TransactionTableEntity : TableEntity
    {
        public TransactionTableEntity(int id, string ime,string prezime,int iznos)
        {
            Id = id;
            RowKey = id.ToString();
            PartitionKey = "Bank";
            Ime = ime;
            Prezime = prezime;
            Iznos = iznos;
        }
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int Iznos { get; set; }
    }
}
