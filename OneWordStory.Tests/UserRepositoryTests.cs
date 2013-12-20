using System;
using Moq;
using NUnit.Framework;
using OneWordStory.Domain.Concrete;
using OneWordStory.Domain.Entities;
using Raven.Client;
using Raven.Client.Embedded;
using System.Linq;
using OneWordStory;
using Raven.Abstractions.Exceptions;
using System.Text;
using System.Security.Cryptography;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.Domain.Indexes;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {



        
        [Test]
        public void SaveUserReturnsUserId()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            UserRepository repository = new UserRepository(store);

            User user = new User() { Email = "email", Password = "password" };

            // Act
            repository.SaveUser(user);
            

            // Assert
            Assert.IsNotNullOrEmpty(user.Id);

        }
        
        [Test]
        public void GetUserByEmailReturnsErrorCodeIfEmailDoesntExist()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            UserRepository repository = new UserRepository(store);

            // Act
            var result = repository.GetUserByEmail("");

            // Assert
            Assert.AreEqual(result.UserCode, UserErrorCode.EmailNotFoundInRepository);
            Assert.IsNull(result.User);

        }

        [Test]
        public void GetUserByEmailReturnsUser()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                var user = new User() { Email = "email", Password = "pass" };
                session.Store(user);
                session.SaveChanges();

                RavenQueryStatistics stats;
                var results = session.Query<User>()
                    .Statistics(out stats)
                    .Customize(x => x.WaitForNonStaleResults())
                    .Where(u => u.Email == "email")
                    .ToArray();

                

            }
            
            UserRepository repository = new UserRepository(store);

            // Act
            var result = repository.GetUserByEmail("email");

            // Assert
            Assert.AreEqual(result.UserCode, UserErrorCode.Success);
            Assert.IsNotNull(result.User);

        }

       

        [Test]
        public void GetUserByIdReturnsErrorCodeIfEmailDoesntExist()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            UserRepository repository = new UserRepository(store);

            // Act
            var result = repository.GetUserById("not here");

            // Assert
            Assert.AreEqual(result.UserCode, UserErrorCode.UserNotFoundInRepository);
            Assert.IsNull(result.User);

        }

        private User GetGenericUser()
        {
            return new User()
            {
                Email = "email@email.com",
                Password = "password"

            };
        }

        [Test]
        public void CanSaveUser()
        {

            var store = Global.GetInMemoryStore();
            // Setup
            User user = GetGenericUser();

            // Act
            new UserRepository(store)
                .SaveUser(user);

            // Assert
            using (var session = store.OpenSession())
            {

                
                var result = session.Query<User>()
                                        .Customize(x => x.WaitForNonStaleResults())
                                        .Where(u => u.Email == user.Email);

                User savedUser = result.First<User>();
                
                Assert.IsNotNull(savedUser);
                Assert.AreEqual(user.Email, savedUser.Email);
            }

            
        }

        

        
        [Test]
        [ExpectedException(typeof(ConcurrencyException))]
        public void EmailsMustBeUnique()
        {

            var store = Global.GetInMemoryStore();

            // Setup
            User user1 = GetGenericUser();
            User user2 = GetGenericUser();
                       

            UserRepository userRepository = new UserRepository(store);
            
            // Act
            userRepository.SaveUser(user1);
            userRepository.SaveUser(user2);


            // Assert
            

        }

        
        [Test]
        public void PasswordGetsHashed()
        {

            var store = Global.GetInMemoryStore();

            // Setup
            User user = GetGenericUser();

            string hashedPassword = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Password)));

            UserRepository userRepository = new UserRepository(store);

            // Act
            userRepository.SaveUser(user);


            // Assert
            using (var session = store.OpenSession())
            {


                User savedUser = session.Load<User>(user.Id);

                Assert.IsNotNull(savedUser);
                Assert.AreEqual(hashedPassword, savedUser.Password);
            }

            

        }


        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EmptyUserIdThrowsExceptionOnDelete()
        {
            var store = Global.GetInMemoryStore();

            // Setup
            UserRepository userRepository = new UserRepository(store);

            // Act
            userRepository.DeleteUser("");



        }
        
        [Test]
        public void CanDeleteUserUsingId()
        {

            var store = Global.GetInMemoryStore();

            // Setup
            var user = GetGenericUser();
            UserRepository userRepository = new UserRepository(store);

            // Act
            userRepository.SaveUser(user);

            // Assert
            using (var session = store.OpenSession())
            {

                User savedUser = session.Load<User>(user.Id);

                Assert.IsNotNull(savedUser);

                userRepository.DeleteUser(user.Id);
            }

            using (var session = store.OpenSession())
            {
                User deletedUser = session.Load<User>(user.Id);

                Assert.IsNull(deletedUser);
            }   
            

        }

        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByIdThrowExceptionOnNull()
        {

            var store = Global.GetInMemoryStore();

            // Setup
            var repository = new UserRepository(store);

            // Act
            repository.GetUserById("");

            

        }

        [Test]
        public void StoryObjectPropertiesAreInitialized()
        {

            // Setup
            Story story = new Story();

            // Assert
            Assert.IsNotNull(story.Paragraphs);
            Assert.IsNotNull(story.CurrentEditor);
            Assert.IsNotNull(story.EditHistory);

        }

        [Test]
        public void GetUserByIdReturnsUser()
        {

            var store = Global.GetInMemoryStore();

            // Setup
            var repository = new UserRepository(store);

            User user = GetGenericUser();

            repository.SaveUser(user);

            User queryUser = new User()
            {
                Email = "query@email.com",
                Password = "querypassword"
            };

            repository.SaveUser(queryUser);

            string userId = queryUser.Id;

            user = GetGenericUser();
            user.Email = "email@email3.com";

            repository.SaveUser(user);

            // Act
            var result = repository.GetUserById(userId);

            //Assert
            Assert.IsNotNull(result.User);
            Assert.AreEqual(result.User.Email, queryUser.Email);
            Assert.AreEqual(result.User.Password, queryUser.Password);




        }

    }
}

