using BookService;
using BookstoreService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
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
                 c = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.Commit());
                 book = await servicePartitionClient2.InvokeWithRetryAsync(client => client.Channel.Commit());
                if (c && book)
                {
                    ;
                }
                
            }
            return true;
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
}
