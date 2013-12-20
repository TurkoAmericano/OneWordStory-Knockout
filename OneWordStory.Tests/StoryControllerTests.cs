using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using OneWordStory.Concrete;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.WebUI.Controllers;
using OneWordStory.WebUI.Controllers.Adapters;
using OneWordStory.WebUI.Models;
using Raven.Client;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class StoryControllerTests
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
        public void StoryPageObjectPropertiesAreInitialized()
        {

            // Setup
            StoryPage storyPage = new StoryPage();

            // Assert
            Assert.IsNotNull(storyPage.UserStories);
            

        }

        
        [Test]
        public void ReadStoryReturnsAStory()
        {
            

            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            User user = FakeEntityFactory.GetGenericUser();
            Story story;
            string userId = "users/1";

            using (var session = store.OpenSession())
            {
                session.Store(user);
                story = FakeEntityFactory.GetSpecificStoryWithText(userId, new List<string> { "123", "456", "789" });
                session.Store(story);
                session.SaveChanges();
            }

            IStoryRepository repo = new StoryRepository(store);
            StoryController controller = new StoryController(repo, new CurrentUser("users/1"));


            // Act
            StoryPage result = (StoryPage)((ViewResult)controller.ReadStory(1)).Model;
            

            // Assert
            Assert.IsTrue(result.ReadStory == "<p>123</p><p>456</p><p>789</p>");

        }
        
        [Test]
        public void AddWordCreatesANewStory()
        {
            // Setup
            Mock<IStoryRepository> mock = new Mock<IStoryRepository>();
            mock.Setup(m => m.AddWord(
                            It.Is<string>(s => s == ""), 
                            It.Is<string>(w => w == "Once"),
                            It.Is<string>(u => u == "users/1")))
                            .Returns(new AddWordResult 
                            {
                                 ErrorCode = StoryErrorCode.Success,
                                  Story = FakeEntityFactory.GetGenericStory("")
                            });

            mock.Setup(m => m.GetStoriesByUser(It.IsAny<string>(), 0, 0)).Returns(new GetStoriesResult());

            var controller = new StoryController(mock.Object, new CurrentUser("users/1"));
            

            // Act
            controller.CreateNewStory(new StoryPage { WordForNewStory = "Once" });

            // Assert
            mock.VerifyAll();

        }

        #endregion


    }
}
