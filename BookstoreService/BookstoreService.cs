using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using BookService;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BookstoreService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BookstoreService : StatefulService
    {

        BookStoreS bookStoreS;
        public BookstoreService(StatefulServiceContext context)
            : base(context)
        { bookStoreS = new BookStoreS(this.StateManager); }


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
                return new WcfCommunicationListener<IBookStore>(context, bookStoreS, WcfUtility.CreateTcpListenerBinding(maxMessageSize: 1024*1024*1024),this.CreateAddress(context,"BookStoreService"));
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
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            var temp = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("bookstemp");
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
        public async void AddToDictionary()
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                
                await books.TryAddAsync(tx, "enciklopedija", new BooksData(1, "enciklopedija", 500, 4));
                await books.TryAddAsync(tx, "zbirka pesama", new BooksData(2, "zbirka pesama", 300, 3));
                await books.TryAddAsync(tx, "bajka", new BooksData(3, "bajka", 800, 10));
                await books.TryAddAsync(tx, "roman", new BooksData(4, "roman", 600, 4));

                await tx.CommitAsync();
            }
        }
    }
}
