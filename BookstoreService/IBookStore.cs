using BookstoreService;
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
        Task<bool> RollBack(bool ind);
        [OperationContract]
        Task<bool> Commit();

        [OperationContract]
        void ListAvaibleItems();
        [OperationContract]
        void EnlistPurchase(string bookID);
        [OperationContract]
        Task<int> GetItemPrice(string bookID);
        [OperationContract]
        Task<Dictionary<string, BooksData>> GetBookData();
    }
}
