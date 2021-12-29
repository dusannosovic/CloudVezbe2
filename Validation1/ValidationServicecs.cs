
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionCordinator;

namespace Validation1
{
    class ValidationServicecs:IValidation
    {
       public async Task<bool> ValidateAsync(string ime,string prezime, string book,int id)
        {
            if(string.IsNullOrEmpty(ime) || string.IsNullOrWhiteSpace(ime))
            {
                return false;
            }
            if(string.IsNullOrEmpty(ime) || string.IsNullOrWhiteSpace(ime))
            {
                return false;
            }
            if(string.IsNullOrEmpty(ime) || string.IsNullOrWhiteSpace(ime))
            {
                return false;
            }
            FabricClient fabricClient = new FabricClient();
            int partitionsNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri("fabric:/CloudVezbe2/TransactionCordinator"))).Count;
            var binding = WcfUtility.CreateTcpClientBinding();
            int index = 0;
            //for (int i = 0; i < partitionsNumber; i++)
            //{
                ServicePartitionClient<WcfCommunicationClient<ITransaction>> servicePartitionClient = new ServicePartitionClient<WcfCommunicationClient<ITransaction>>(
                    new WcfCommunicationClientFactory<ITransaction>(clientBinding: binding),
                    new Uri("fabric:/CloudVezbe2/TransactionCordinator"),
                    new ServicePartitionKey(0));

                await servicePartitionClient.InvokeWithRetryAsync(client => client.Channel.PrepareAsync(ime,prezime,book,id));
            //}
            return true;
        }

    }
}
