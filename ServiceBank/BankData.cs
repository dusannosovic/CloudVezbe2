using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBank
{
    public class BankData
    {
        public BankData(int id, string ime, string prezime, int iznos)
        {
            Id = id;
            Ime = ime;
            Prezime = prezime;
            Iznos = iznos;
        }
        public BankData()
        {

        }
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public int Iznos { get; set; }

    }
}
