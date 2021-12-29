using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBank
{
    [ServiceContract]
    public interface IBank
    {
        [OperationContract]
        Task<bool> RollBack(bool ind);
        [OperationContract]
        Task<bool> Commit();
        [OperationContract]
        Task<bool> CheckMoney(string ime,int id, int money);
        [OperationContract]
        Task<Dictionary<int, BankData>> GetBankData();
    }
}
