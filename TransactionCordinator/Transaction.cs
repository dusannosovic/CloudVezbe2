using BookService;
using BookstoreService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using Microsoft.Azure;
using ServiceBank;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionCordinator
{
    public class Transaction : ITransaction
    {
        public void Commit()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PrepareAsync(string ime, string prezime, string bookid,int id)
        {
            var binding1 = WcfUtility.CreateTcpClientBinding();
            var binding2 = WcfUtility.CreateTcpClientBinding();
            int index = 0;
            //for (int i = 0; i < partitionsNumber; i++)
            //{
           ServicePartitionClient<WcfCommunicationClient<IBookStore>> servicePartitionClient1 = new ServicePartitionClient<WcfCommunicationClient<IBookStore>>(
                new WcfCommunicationClientFactory<IBookStore>(clientBinding: binding1),
                new Uri("fabric:/CloudVezbe2/BookstoreService"),
                new ServicePartitionKey(0));

            ServicePartitionClient<WcfCommunicationClient<IBank>> servicePartitionClient2 = new ServicePartitionClient<WcfCommunicationClient<IBank>>(
                new WcfCommunicationClientFactory<IBank>(clientBinding: binding2),
                new Uri("fabric:/CloudVezbe2/ServiceBank"),
                new ServicePartitionKey(0));
            int a = await servicePartitionClient1.InvokeWithRetryAsync(client => client.Channel.GetItemPrice(bookid));
            bool book;

            bool c = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.CheckMoney(ime,id,a));

            if (c)
            {
                c = false;
                book = false;
                c = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.Commit());
                book = await servicePartitionClient1.InvokeWithRetryAsync(client => client.Channel.Commit());
                if (c && book)
                {
                    c = false;
                    book = false;
                    c = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.RollBack(true));
                    book = await servicePartitionClient1.InvokeWithRetryAsync(client => client.Channel.RollBack(true));
                    AddDataToTable(servicePartitionClient1, servicePartitionClient2);
                }
                else
                {
                    c = false;
                    book = false;
                    c = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.RollBack(false));
                    book = await servicePartitionClient1.InvokeWithRetryAsync(client => client.Channel.RollBack(false));
                }
                
            }
            return true;
        }
        public async Task AddDataToTable(ServicePartitionClient<WcfCommunicationClient<IBookStore>> servicePartitionClient1, ServicePartitionClient<WcfCommunicationClient<IBank>> servicePartitionClient2)
        {
            Dictionary<string, BooksData> books = await servicePartitionClient1.InvokeWithRetryAsync(client => client.Channel.GetBookData());
            Dictionary<int, BankData> bankData = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.GetBankData());
            CloudStorageAccount _storageAccount;
            CloudTable _table;
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("StoreBankTable");
            foreach(BooksData bk in books.Values)
            {
                TableOperation insertOperation = TableOperation.InsertOrReplace(new BookTableEntity(bk.NazivKnjige, bk.Cena, bk.BrojKnjiga));
                _table.Execute(insertOperation);
            }
            foreach(BankData bk in bankData.Values)
            {
                TableOperation insertOperation = TableOperation.InsertOrReplace(new TransactionTableEntity(bk.Id,bk.Ime,bk.Prezime,bk.Iznos));
                _table.Execute(insertOperation);
            }
            

        }
        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
