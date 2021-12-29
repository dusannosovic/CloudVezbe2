using BookstoreService;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankService
{
    class BankServiceS : IBank
    {
        IReliableDictionary<int, BankData> counts;
        IReliableStateManager StateManager;
        public BankServiceS(IReliableStateManager stateManager)
        {
            //ToDictionary(stateManager);
            StateManager = stateManager;
        }

        private async void ToDictionary(IReliableStateManager stateManager)
        {
            using(var tx = stateManager.CreateTransaction())
            {
                counts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("counts");
                await counts.TryAddAsync(tx, 1, new BankData(1, "Petar", "Petrovic", 1500));
                await counts.TryAddAsync(tx, 2, new BankData(2, "Nikola", "Nikolic", 1000));
                await counts.TryAddAsync(tx, 3, new BankData(3, "Nikola", "Peric", 1500));
                await counts.TryAddAsync(tx, 4, new BankData(4, "Aleksandar", "Petrovic", 6000));

                await tx.CommitAsync();
            }
        }
        

        float status = 500;
        public void Abort()
        {
            throw new NotImplementedException();
        }

        async Task<bool> IBank.CheckMoney(string ime)
        {         
            return true;
        }

        public Task CommitAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EnlistMoneyTransfer()
        {
            throw new NotImplementedException();
        }

        public Task<long> GetVisibilitySequenceNumberAsync()
        {
            throw new NotImplementedException();
        }

        public void ListClients()
        {
            throw new NotImplementedException();
        }
    }
}
