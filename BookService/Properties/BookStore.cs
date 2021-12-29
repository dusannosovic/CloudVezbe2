using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookService.Properties
{
    class BookStore : IBookStore
    {
        public void EnlistPurchase(string bookID)
        {
            throw new NotImplementedException();
        }

        public double GetItemPrice(string bookID)
        {
            throw new NotImplementedException();
        }

        public void ListAvaibleItems()
        {
            throw new NotImplementedException();
        }
    }
}
