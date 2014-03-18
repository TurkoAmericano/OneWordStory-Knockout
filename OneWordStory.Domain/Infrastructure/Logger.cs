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

        internal void LogError(Exception exception, string errorCode = "")
        {

            LogError repoError = new LogError()
            {
                DateOfOccurence = DateTime.Now,
                ErrorCode = errorCode,
                Exception = exception

            };


            using (var session = _store.OpenSession())
            {
                session.Store(repoError);
                session.SaveChanges();
            }
        }


    }
}
