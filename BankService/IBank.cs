using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreService
{
    [ServiceContract]
    public interface IBank
    {
        
        [OperationContract]
        void ListClients();
        [OperationContract]
        void EnlistMoneyTransfer();
        [OperationContract]
        Task<bool> CheckMoney(string ime);
    }
}
