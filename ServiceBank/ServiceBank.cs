using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ServiceBank
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ServiceBank : StatefulService
    {
        BankService bankService;
        public ServiceBank(StatefulServiceContext context)
            : base(context)
        { bankService = new BankService(this.StateManager); }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener> {
            new ServiceReplicaListener(context =>
            {
                return new WcfCommunicationListener<IBank>(context, bankService, WcfUtility.CreateTcpListenerBinding(maxMessageSize: 1024*1024*1024),this.CreateAddress(context,"ServiceB"));
            }
            )
            };
        }
        private EndpointAddress CreateAddress(StatefulServiceContext context, string endpointName)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint(endpointName);
            int port = endpointConfig.Port;
            string scheme = endpointConfig.Protocol.ToString();

            ServiceEventSource.Current.Message("Napravljen Bookstore listener!");

            return new EndpointAddress(string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/{3}", scheme, host, port, endpointName));

        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            var counts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            var tempaccount = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accountstemp");
            AddToDictionary();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
        private async void AddToDictionary()
        {
            var counts = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, BankData>>("accounts");
            using (var tx = this.StateManager.CreateTransaction())
            {

                await counts.TryAddAsync(tx, 1, new BankData(1, "Petar", "Petrovic", 1500));
                await counts.TryAddAsync(tx, 2, new BankData(2, "Nikola", "Nikolic", 1000));
                await counts.TryAddAsync(tx, 3, new BankData(3, "Nikola", "Peric", 1500));
                await counts.TryAddAsync(tx, 4, new BankData(4, "Aleksandar", "Petrovic", 6000));

                await tx.CommitAsync();
            }
        }
    }
}
