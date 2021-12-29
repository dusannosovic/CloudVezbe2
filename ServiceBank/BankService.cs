using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBank
{
    public class BankService : IBank
    {
        IReliableDictionary<int, BankData> accounts;
        IReliableDictionary<int, BankData> accountsTemp;
        IReliableStateManager StateManager;
        public BankService(IReliableStateManager stateManager)
        {
            //ToDictionary(stateManager);
            StateManager = stateManager;
        }
        public async Task<bool> Commit()
        {
            accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            accountsTemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accountstemp");
            int a;
            BankData bank;
            BankData tempBank;
            int key;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await accountsTemp.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken())) {
                    key = enumerator.Current.Key;
                    tempBank = enumerator.Current.Value;
                    //tempBank = (await accountsTemp.TryGetValueAsync(tx, key)).Value
                    bank = (await accounts.TryGetValueAsync(tx, key)).Value;
                    await accounts.TryRemoveAsync(tx, key);
                    await accounts.AddAsync(tx, tempBank.Id, tempBank);
                    await accountsTemp.TryRemoveAsync(tx, key);
                    await accountsTemp.TryAddAsync(tx, tempBank.Id, bank);
                }
                await tx.CommitAsync();
            }
            return true;
            
        }
        public async Task<bool> RollBack(bool ind)
        {
            accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            accountsTemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accountstemp");
            int a;
            BankData bank;
            BankData tempBank;
            using (var tx = this.StateManager.CreateTransaction())
            {

                var enumerator = (await accountsTemp.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                {
                    tempBank = (await accountsTemp.TryGetValueAsync(tx, enumerator.Current.Key)).Value;
                    bank = (await accounts.TryGetValueAsync(tx, enumerator.Current.Key)).Value;
                    if (!ind)
                    {
                        await accounts.TryRemoveAsync(tx, enumerator.Current.Key);
                        await accounts.AddAsync(tx, tempBank.Id, tempBank);
                    }
                    await accountsTemp.TryRemoveAsync(tx, enumerator.Current.Key);
                }
                await tx.CommitAsync();
            }
            return true;
        }
        public async Task<bool> CheckMoney(string ime,int id, int money)
        {
            accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            accountsTemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accountstemp");
            BankData accountState;
            using (var tx = this.StateManager.CreateTransaction())
            {
                accountState = (await accounts.TryGetValueAsync(tx, id)).Value;
                if (accountState.Iznos < money)
                {
                    return false;
                }
                accountState.Iznos = accountState.Iznos - money;
                await accountsTemp.AddAsync(tx,id, accountState);
                await tx.CommitAsync();
                //iznos = Convert.ToDouble(temp);
            }
            return true;
        }
        public async Task<Dictionary<int,BankData>> GetBankData()
        {
            accounts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            Dictionary<int, BankData> dict = new Dictionary<int, BankData>();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await accounts.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                {
                    dict.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }
            return dict;
        }
    }
}
