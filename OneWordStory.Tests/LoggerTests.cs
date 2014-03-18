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
                        
            // Act
            logger.LogError(exception, "StoryNotFoundInRepository");

            // Assert
            using (var session = store.OpenSession())
            {
                LogError savedError = (from e in session.Query<LogError>() select e).First();
                Assert.GreaterOrEqual(savedError.DateOfOccurence, date.AddMinutes(-1));
                Global.PropertyValuesAreEquals(exception, savedError.Exception);
                Assert.AreEqual(savedError.ErrorCode, "StoryNotFoundInRepository");
            }
            
            
            
            

        }

        #endregion
    }
}
