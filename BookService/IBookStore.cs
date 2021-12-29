using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BookService
{
    [ServiceContract]
    public interface IBookStore
    {
        [OperationContract]
        void ListAvaibleItems();
        [OperationContract]
        void EnlistPurchase(string bookID);
        [OperationContract]
        double GetItemPrice(string bookID);
    }
}
