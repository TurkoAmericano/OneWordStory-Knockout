using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Exceptions;
using Raven.Bundles.UniqueConstraints;
using Raven.Client;

namespace OneWordStory.Domain.Infrastructure
{

    
    public interface IRavenUniqueInserter
    {
        void StoreUnique<T, TUnique>(
            IDocumentSession session, T entity,
            Expression<Func<T, TUnique>> keyProperty);
    }

    public class RavenUniqueInserter : IRavenUniqueInserter
    {

        private static string PREFIX = "UniqueConstraints/{0}/{1}";


        public void StoreUnique<T, TUnique>(IDocumentSession session, T entity,
                                        Expression<Func<T, TUnique>> keyProperty)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (keyProperty == null) throw new ArgumentNullException("keyProperty");
            if (entity == null) throw new ArgumentNullException("entity");

            var key = keyProperty.Compile().Invoke(entity).ToString();

            var constraint = new UniqueConstraint
            {
                PropName = typeof(T).Name, 

            };
                      

            DoStore(session, entity, constraint, key);

        }


        private string GetConstraintId(string name, string key)
        {
                return String.Format(PREFIX, name, key);
        }

        public void DeleteConstraint(IDocumentSession session, string name, string key)
        { 
            session.Advanced.Defer(new DeleteCommandData { Key = String.Format(PREFIX, name, key) });
        }

        static void DoStore<T>(IDocumentSession session, T entity,
            UniqueConstraint constraint, string key)
        {
            var previousSetting = session.Advanced.UseOptimisticConcurrency;

            try
            {
                session.Advanced.UseOptimisticConcurrency = true;
                session.Store(constraint,
                                String.Format(PREFIX,
                                            constraint.PropName, key));
                session.Store(entity);
                session.SaveChanges();
            }
            catch (ConcurrencyException)
            {
                // rollback changes so we can keep using the session
                session.Advanced.Evict(entity);
                session.Advanced.Evict(constraint);
                throw;
            }
            finally
            {
                session.Advanced.UseOptimisticConcurrency = previousSetting;
            }
        }
    }
}
