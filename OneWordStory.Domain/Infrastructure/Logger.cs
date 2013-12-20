using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;
using Raven.Client;


[assembly: InternalsVisibleTo("OneWordStory.Tests")]

namespace OneWordStory.Domain.Infrastructure
{
    
    internal class Logger
    {

        private IDocumentStore _store;

        public Logger(IDocumentStore store)
        {
            _store = store;
        }

        internal void LogError(RepositoryError error)
        {
            using (var session = _store.OpenSession())
            {
                session.Store(error);
                session.SaveChanges();
            }
        }


    }
}
