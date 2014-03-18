using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using NUnit.Framework;
using OneWordStory.Concrete;
using OneWordStory.Domain;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using Raven.Client;

namespace OneWordStory.Tests
{

    public class LockStoryRunner
    {
        string _storyId = "";
        string _userId = "";
        StoryRepository _repository = null;

        public LockStoryRunner(string storyId, string userId, StoryRepository repository)
        {
            _storyId = storyId;
            _userId = userId;
            _repository = repository;
        }

        public StoryErrorCode LockStory()
        {
            return _repository.LockStory(_storyId, _userId);
        }

    }

    [TestFixture]
    public class LatencyTests
    {
        #region SetUp / TearDown

        [SetUp]
        public void Init()
        { }

        [TearDown]
        public void Dispose()
        { }

        #endregion




        [Test]
        public void UserAttemptsToLockRecentlyLockedStory()
        {
            // Setup

            for (int i = 0; i < 10; i++)
            {
                RunLockTest();
            }
            

        }

        private static void RunLockTest()
        {
            var user1 = "users/1";
            var user2 = "users/2";

            var story = FakeEntityFactory.GetGenericStory("users/23");

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
                Global.UpdateIndex<Story>(session);
            }

            var repository = new StoryRepository(store);

            var tasks = new List<Task>();

            LockStoryRunner runner1 = new LockStoryRunner(story.Id, user1, repository);
            LockStoryRunner runner2 = new LockStoryRunner(story.Id, user2, repository);

            tasks.Add(Task.Factory.StartNew<StoryErrorCode>(runner1.LockStory));
            tasks.Add(Task.Factory.StartNew<StoryErrorCode>(runner2.LockStory));

            Task.WaitAll(tasks.ToArray());




            var result1 = (StoryErrorCode)tasks[0].Status;
            var result2 = (StoryErrorCode)tasks[1].Status;

            if (result1 == StoryErrorCode.Success)
                Assert.AreEqual(result2, StoryErrorCode.StoryLockedForEditing);

            if (result2 == StoryErrorCode.Success)
                Assert.AreEqual(result1, StoryErrorCode.StoryLockedForEditing);
        }


        
        [Test]
        public void UserCanLockAfterWordHasBeenAdded()
        {
            // Setup
            var user1 = "users/1";
            var user2 = "users/2";
            

            var story = FakeEntityFactory.GetGenericStory("users/23");

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();

           }


            var repository = new StoryRepository(store);

            using (var session = store.OpenSession())
            {

                
                var firstLockResult = repository.LockStory(story.Id, user1);

                var saveStory = session.Load<Story>(story.Id);
                Assert.IsTrue(saveStory.HasEditor);
                Assert.AreEqual(saveStory.Lock.UserId, user1);

                var secondLockResult = repository.LockStory(story.Id, user2);
                saveStory = session.Load<Story>(story.Id);
                Assert.AreEqual(secondLockResult, StoryErrorCode.StoryLockedForEditing);
                Assert.AreEqual(saveStory.Lock.UserId, user1);

            }

            using (var session = store.OpenSession())
            {

                var firstAddWordResult = repository.AddWord(story.Id, "word", user1);
                var saveStory = session.Load<Story>(story.Id);
                Assert.AreEqual(firstAddWordResult.ErrorCode, StoryErrorCode.Success);
                Assert.IsFalse(saveStory.HasEditor);
            }

            using (var session = store.OpenSession())
            {
                var thirdLockResult = repository.LockStory(story.Id, user2);
                var saveStory = session.Load<Story>(story.Id);
                Assert.AreEqual(thirdLockResult, StoryErrorCode.Success);
                Assert.IsTrue(saveStory.HasEditor);
                Assert.AreEqual(saveStory.Lock.UserId, user2);
            }


            // Act


            // Assert
            

        }



    }


    


}

