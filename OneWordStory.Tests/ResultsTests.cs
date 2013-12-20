using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NUnit.Framework;
using OneWordStory.Domain.Infrastructure;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class ResultsTests
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
        public void ResultsInitialize()
        {
            // Setup
            AddWordResult addWordResult = new AddWordResult();
            GetUserResult getUserResult = new GetUserResult();
            GetStoriesResult getStoriesResult = new GetStoriesResult();


            // Assert
            Assert.IsNotNull(addWordResult.Story);
            Assert.IsNotNull(addWordResult.ErrorCode);
            Assert.IsNotNull(getUserResult.User);
            Assert.IsNotNull(getUserResult.UserCode);
            Assert.IsNotNull(getStoriesResult.Stories);
            
            

        }

        #endregion
    }
}
