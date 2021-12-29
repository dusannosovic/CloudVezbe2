using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TransactionCordinator
{
    [ServiceContract]
    public interface ITransaction
    {
        [OperationContract]
        Task<bool> PrepareAsync(string ime,string prezime ,string bookid,int id);
        [OperationContract]
        void Commit();
        [OperationContract]
        void RollBack();
    }
}
