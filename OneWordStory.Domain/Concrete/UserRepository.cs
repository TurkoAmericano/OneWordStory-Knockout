using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Extensions;
using OneWordStory.Domain.Indexes;

namespace OneWordStory.Domain.Concrete
{
    public class UserRepository : IUserRepository
    {


        private IDocumentStore _store;
        

        public UserRepository()
        {
            _store = DocumentStoreSingleton.DocumentStore;
        }

        public UserRepository(IDocumentStore store)
        {
            _store = store;
        }



        public UserErrorCode SaveUser(User user)
        {
            using (var session = _store.OpenSession())
            {


                RunBusinessOperations(user);

                session.Advanced.UseOptimisticConcurrency = true;

                new RavenUniqueInserter()
                    .StoreUnique(session, user, p => p.Email);

                try
                {
                    session.SaveChanges();
                    return UserErrorCode.Success;
                }
                catch (ConcurrencyException)
                {
                    return UserErrorCode.EmailAlreadyExists;
                }
            }
        }

        private void RunBusinessOperations(User user)
        {
            user.Password = user.Password.Hash();
        }



        public UserErrorCode DeleteUser(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            using (var session = _store.OpenSession())
            {

                User user = session.Load<User>(userId);

                new RavenUniqueInserter()
                    .DeleteConstraint(session, "User", user.Email);

                session.Delete<User>(user);
                session.SaveChanges();
                return UserErrorCode.Success;

            }

        }


        
        public GetUserResult GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId");

            using (var session = _store.OpenSession())
            {
                var user = session.Load<User>(userId);

                return ReturnGetUserResult(user, UserErrorCode.UserNotFoundInRepository);

            }
        }

        private static GetUserResult ReturnGetUserResult(User user, UserErrorCode failureCode)
        {
            var result = new GetUserResult() { User = user };

            if (user == null)
                result.UserCode = failureCode;
            else
                result.UserCode = UserErrorCode.Success;

            return result;
        }





        public GetUserResult GetUserByEmail(string email)
        {
            using (var session = _store.OpenSession())
            {
                User user = session.Query<User, UserByEmail>().Where(u => u.Email == email).FirstOrDefault();

                return ReturnGetUserResult(user, UserErrorCode.EmailNotFoundInRepository);
            }

            
        }
    }
}
