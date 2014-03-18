using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Infrastructure;
using Raven.Client;

namespace OneWordStory.Domain.Concrete
{
    public class InfrastructureRepository : IInfrastructureRepository
    {

        
        private IDocumentStore _store;

        public InfrastructureRepository()
        {
            _store = DocumentStoreSingleton.DocumentStore;
        }

        public InfrastructureRepository(IDocumentStore store)
        {
            _store = store;
        }
        
        public void LogError(Exception exception, string errorCode = "")
        {
            new Logger(_store).LogError(exception, errorCode);
        }
    }
}
