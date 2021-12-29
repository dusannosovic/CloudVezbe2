using BookService;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;

namespace BookstoreService
{
    public class BookStoreS : IBookStore
    {
        IReliableDictionary<string, BooksData> books;
        IReliableDictionary<string, BooksData> bookstemp;
        IReliableStateManager StateManager;
        public BookStoreS(IReliableStateManager stateManager)
        {
            //ToDictionary(stateManager);
            StateManager = stateManager;
        }
        public void EnlistPurchase(string bookID)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Commit()
        {
            books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            bookstemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("bookstemp");
            BooksData book;
            BooksData tempBook;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await bookstemp.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken())) {
                    string key = enumerator.Current.Key;
                    tempBook = (await bookstemp.TryGetValueAsync(tx, key)).Value;
                    book = (await books.TryGetValueAsync(tx, key)).Value;
                    await books.TryRemoveAsync(tx, key);
                    await books.AddAsync(tx, key, tempBook);
                    await bookstemp.TryRemoveAsync(tx, key);
                    await bookstemp.TryAddAsync(tx, key, book);
                    //await accounts.AddOrUpdateAsync;
                    await tx.CommitAsync();
                }
            }
            return true;
        }
        public async Task<bool> RollBack(bool ind)
        {
            books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            bookstemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("bookstemp");
            BooksData book;
            BooksData tempBook;
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await bookstemp.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                {
                    
                    tempBook = (await bookstemp.TryGetValueAsync(tx, enumerator.Current.Key)).Value;
                    book = (await books.TryGetValueAsync(tx, enumerator.Current.Key)).Value;
                    if (!ind) {
                        await books.TryRemoveAsync(tx, enumerator.Current.Key);
                        await books.AddAsync(tx, enumerator.Current.Key, tempBook);
                    }
                    await bookstemp.TryRemoveAsync(tx, enumerator.Current.Key);
                    //await bookstemp.TryAddAsync(tx, bookId, book);
                    //await accounts.AddOrUpdateAsync;
                }
                    await tx.CommitAsync();
                
            }
            return true;
        }

        public void ListAvaibleItems()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetItemPrice(string bookID)
        {
            books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            bookstemp = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("bookstemp");
            BooksData book;
            using (var tx = this.StateManager.CreateTransaction())
            {
                book = (await books.TryGetValueAsync(tx, bookID)).Value;
                book.BrojKnjiga = book.BrojKnjiga - 1;
                await bookstemp.AddAsync(tx, book.NazivKnjige, book);
                await tx.CommitAsync();
                //iznos = Convert.ToDouble(temp);
            }
            return book.Cena;
        }
        public async Task<Dictionary<string,BooksData>> GetBookData(){
            books = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, BooksData>>("books");
            Dictionary<string, BooksData> booksDict = new Dictionary<string, BooksData>();
            using(var tx = this.StateManager.CreateTransaction())
            {
                var enumerator = (await bookstemp.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                {
                    booksDict.Add(enumerator.Current.Key, enumerator.Current.Value);
                }
            }
            return booksDict;
        }
    }
}
