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
using Extensions;

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
            StoryPageView storyPage = new StoryPageView();

            // Assert
            Assert.IsNotNull(storyPage.UserStoryList.UserStories);
            

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
            Story result = (Story)controller.ReadStory(1).Data;
                        

            // Assert
            Assert.AreEqual(result.Paragraphs[0], "123");
            Assert.AreEqual(result.Paragraphs[1], "456");
            Assert.AreEqual(result.Paragraphs[2], "789");


        }

        


        [Test]
        public void AddWordCreatesANewStory()
        {
            // Setup
            Mock<IStoryRepository> mock = new Mock<IStoryRepository>();
            
            mock.Setup(m => m.AddWord(
                            It.Is<string>(s => s == ""), 
                            It.Is<string>(w => w == "Once"),
                            It.Is<string>(u => u == "users/1"), false))
                            .Returns(new AddWordResult 
                            {
                                 ErrorCode = StoryErrorCode.Success,
                                  Story = FakeEntityFactory.GetGenericStory("")
                            });

            
            var controller = new StoryController(mock.Object, new CurrentUser("users/1"));
            
            // Act
            controller.CreateNewStory(new AddWord { Word = "Once" });

            // Assert
            mock.VerifyAll();

        }

        #endregion


    }
}
