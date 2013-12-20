using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using OneWordStory.Domain;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using Raven.Client;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        #region SetUp / TearDown

        [SetUp]
        public void Init()
        { }

        [TearDown]
        public void Dispose()
        { }

        #endregion

        #region Tests

        
        [Test]
        public void LoggerLogsError()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            Logger logger = new Logger(store);

            DateTime date = DateTime.Now;
            Exception exception = new Exception("Here is the message");

            RepositoryError error = new RepositoryError();
            error.Exception = exception;
            error.ErrorCode = StoryErrorCode.StoryNotFoundInRepository.ToString();
            error.DateOfOccurence = date;

            // Act
            logger.LogError(error);

            // Assert
            using (var session = store.OpenSession())
            {
                RepositoryError savedError = (from e in session.Query<RepositoryError>() select e).First();
                Assert.AreEqual(savedError.DateOfOccurence, date);
                Global.PropertyValuesAreEquals(exception, savedError.Exception);
                Assert.AreEqual(savedError.ErrorCode, "StoryNotFoundInRepository");
            }
            
            
            
            

        }

        #endregion
    }
}
