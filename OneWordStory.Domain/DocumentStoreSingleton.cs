using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;


namespace OneWordStory.Domain
{
    public static class DocumentStoreSingleton
    {

        private static readonly Lazy<IDocumentStore> _DocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Url = "http://study:8080"
            };
            store.Initialize();

            IndexCreation.CreateIndexes(typeof(UserByEmail).Assembly, store);
            IndexCreation.CreateIndexes(typeof(StoriesByUser).Assembly, store);

            return store;

        });

        public static IDocumentStore DocumentStore
        {
            get { return _DocStore.Value; }
        }

        


        

    }
}
