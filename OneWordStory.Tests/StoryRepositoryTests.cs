using System;
using System.Collections.Generic;
using NUnit.Framework;
using OneWordStory.Concrete;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using Raven.Client;
using System.Linq;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Indexes;

namespace OneWordStory.Tests
{
    [TestFixture]
    public class StoryRepositoryTests
    {






        [Test]
        public void GetStoriesByUserReturnsStories()
        {
            
            // Setup
            IDocumentStore store;
            User user;
            var stories = CreateFakeStories(out store, out user, 5);

            // Act
            IStoryRepository rep = new StoryRepository(store);
            List<Story> result = rep.GetStoriesByUser(user.Id).Stories;

            // Assert
            Assert.AreEqual(result.Count(), 5);

            foreach (var story in stories)
            {
                Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story.Paragraphs[0])).Count() > 0);    
            }

            
            

        }

        private static List<Story> CreateFakeStories(out IDocumentStore store, out User user, int numberOfStories)
        {
            store = Global.GetInMemoryStore();

            user = FakeEntityFactory.GetGenericUser();

            

            using (var session = store.OpenSession())
            {
                session.Store(user);

                for (int i = 1; i <= numberOfStories; i++)
                {
                    var story = FakeEntityFactory.GetGenericStory(user.Id);
                    session.Store(story);
                }

                session.SaveChanges();

                Global.UpdateIndex<Story>(session);

                return session.Query<Story>().ToList<Story>();

            }
        }




        
        [Test]
        public void GetRandomStoriesReturnsStories()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();


            using (var session = store.OpenSession())
            {
                for(int i = 0; i < 20; i++)
                {
                    session.Store(FakeEntityFactory.GetGenericStory(string.Empty));
                }

                session.SaveChanges();
                Global.UpdateIndex<Story>(session);
            }

            IStoryRepository rep = new StoryRepository(store);

            // Act
            var result = rep.GetRandomStories(10);


            // Assert
            Assert.AreEqual(result.Count(), 10);

        }

        [Test]
        public void StoryObjectPropertiesAreInitialized()
        {

            // Setup
            Story story = new Story();

            // Assert
            Assert.IsNotNull(story.Paragraphs);
            Assert.IsNotNull(story.LastEditorId);
            Assert.IsNotNull(story.EditHistory);

        }


        [Test]
        public void GetStoryByIdThrowsException()
        {

            // Setup

            IDocumentStore store = Global.GetInMemoryStore();

            StoryRepository repository = new StoryRepository(store);
            Assert.Throws<ArgumentNullException>(() => repository.GetStoryById(""));
            Assert.Throws<ArgumentNullException>(() => repository.GetStoryById(null));

        }


        
        [Test]
        public void GetStoriesByUserOrderTest()
        {
            
            // Setup
            IDocumentStore store;
            User user;
            var stories = CreateFakeStories(out store, out user, 10);

            // Act
            StoryRepository repository = new StoryRepository(store);
            var result = repository.GetStoriesByUser(user.Id);

            var sortedResult = result.Stories.OrderBy(s => s.DocumentId).ToList();

            // Assert
            for (int i = 0; i < result.Stories.Count; i++)
            {
                Assert.AreEqual(result.Stories[i].DocumentId, sortedResult[i].DocumentId);
            }

        }

        
        [Test]
        public void StartNewParagraphTest()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();

            var repository = new StoryRepository(store);

            var wordToAdd = "Hey";

            var story = FakeEntityFactory.GetGenericStory("users/1");

            var numberofParagraphs = story.Paragraphs.Count;

            using (var session = store.OpenSession())
            {
                
                session.Store(story);
                session.SaveChanges();
            }

            

            // Act
            repository.AddWord(story.Id, wordToAdd, "users/1", true);

            // Assert
            using (var session = store.OpenSession())
            {
                Global.UpdateIndex<Story>(session);
                var resultStory = session.Load<Story>(story.Id);
                Assert.AreEqual(resultStory.Paragraphs.Count, numberofParagraphs + 1);
                Assert.AreEqual(resultStory.Paragraphs[numberofParagraphs], wordToAdd);
            }
            
            

        }

        [Test]
        public void GetSecondPageTest()
        {
            
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();

            string userId = "users/123";

            using (var session = store.OpenSession())
            {

                int i = 1;
                while (i <= 50)
                {
                    var story = FakeEntityFactory.GetGenericStory(userId);
                    story.Paragraphs[0] = story.Paragraphs[0].Insert(0, "Story" + i + " ");
                    session.Store(story);
                    i++;
                }

                session.SaveChanges();
                Global.UpdateIndex<Story>(session);

                
            }
            
            StoryRepository repo = new StoryRepository(store);

            // Act
            var result = repo.GetStoriesByUser(userId, 3, 10);

            // Assert
            Assert.IsTrue(result.Stories[0].Paragraphs[0].Contains("Story21"));
            Assert.IsTrue(result.Stories[1].Paragraphs[0].Contains("Story22"));
            Assert.IsTrue(result.Stories[2].Paragraphs[0].Contains("Story23"));
            Assert.IsTrue(result.Stories[3].Paragraphs[0].Contains("Story24"));
            Assert.IsTrue(result.Stories[4].Paragraphs[0].Contains("Story25"));
            Assert.IsTrue(result.Stories[5].Paragraphs[0].Contains("Story26"));
            Assert.IsTrue(result.Stories[6].Paragraphs[0].Contains("Story27"));
            Assert.IsTrue(result.Stories[7].Paragraphs[0].Contains("Story28"));
            Assert.IsTrue(result.Stories[8].Paragraphs[0].Contains("Story29"));
            Assert.IsTrue(result.Stories[9].Paragraphs[0].Contains("Story30"));


        }

        
        [Test]
        public void PageSizeWithNoPageNoReturnsError()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);
            
            // Act
            Assert.Throws<ArgumentException>(() => repository.GetStoriesByUser("string", pageSize: 5), "Must include Page No with Page Size");

        }

        
        [Test]
        public void PageNoWithNoPageSizeReturnsError()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);


            // Act
            Assert.Throws<ArgumentException>(() => repository.GetStoriesByUser("string", 2), "Must include Page Size with Page No");

            

        }
        [Test]
        public void GetStoryByIdReturnsStory()
        {
            // Setup
            Story story = new Story()
            {
                
                EditHistory = new List<EditHistory>() { 
                    new EditHistory() {
                        DateAdded = DateTime.Now.AddHours(-1),  
                        ParagraphIndex = 45,
                        ParagraphNumber = 2,
                         UserId = "users/2"
                    },
                    new EditHistory() {
                        DateAdded = DateTime.Now.AddHours(-2),  
                        ParagraphIndex = 25,
                        ParagraphNumber = 6,
                         UserId = "users/5"
                    },
                    new EditHistory() {
                        DateAdded = DateTime.Now.AddHours(-8),  
                        ParagraphIndex = 25,
                        ParagraphNumber = 4,
                         UserId = "users/8"
                    }
                },
                Paragraphs = new List<string>() { "para1", "para2", "para3" }

            };

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            StoryRepository repository = new StoryRepository(store);

            // Act
            var savedStory = repository.GetStoryById(story.Id);

            // Assert
            Global.AreEqualByJson(story.EditHistory, savedStory.EditHistory);
            Assert.AreEqual(story.Id, savedStory.Id);
            Global.AreEqualByJson(story.Paragraphs, savedStory.Paragraphs);

        }



        
        [Test]
        public void AllowsALockIfYouveAlreadyLockedIt()
        {
            // Setup
            var store = Global.GetInMemoryStore();

            var userId = "users/1";

            var story = FakeEntityFactory.GetGenericStory(userId);
            story.Lock.UserId = userId;
            

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            // Act
            StoryRepository repo = new StoryRepository(store);
            var result = repo.LockStory("stories/1", userId);

            // Assert
            Assert.AreEqual(result, StoryErrorCode.Success);

        }

        [Test]
        public void AddWordThrowsExceptionsIfWordOrUserIdIsNullOrEmpty()
        {
            // Setup
            var store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);

            // Assert

            Assert.Throws<ArgumentNullException>(() => repository.AddWord("story/1", "", "userId"), "word");
            Assert.Throws<ArgumentNullException>(() => repository.AddWord("story/2", null, "userId"), "word");
            Assert.Throws<ArgumentNullException>(() => repository.AddWord("story/1", "Once", ""), "userId");
            Assert.Throws<ArgumentNullException>(() => repository.AddWord("story/2", "Once", null), "userId");



        }


        
        [Test]
        public void AddWordAllowsUserToAddWordIfUserIsCurrentEditor()
        {

            // Setup
            var store = Global.GetInMemoryStore();
            var userId = "users/1";
            var word = "New";
            var story = FakeEntityFactory.GetGenericStory(userId);
            story.Lock.UserId = userId;
            story.Lock.LockedDate = DateTime.Now.AddMinutes(-8);

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
                Global.UpdateIndex<Story>(session);
            }

            // Act
            StoryRepository repository = new StoryRepository(store);
            var result = repository.AddWord(story.Id, word, userId);

            // Assert
            Assert.AreEqual(result.ErrorCode, StoryErrorCode.Success);
            Assert.IsTrue(result.Story.Paragraphs.Last().EndsWith(word));
            

        }



        [Test]
        public void AddWordWorksIfTenMinuteWindowHasPassed()
        {

            // Setup
            var store = Global.GetInMemoryStore();
            var userId = "users/1";
            var userId2 = "users/2";
            var word = "New";
            var story = FakeEntityFactory.GetGenericStory(userId);
            story.Lock.UserId = userId;
            story.Lock.LockedDate = DateTime.Now.AddMinutes(-15);

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
                Global.UpdateIndex<Story>(session);
            }

            StoryRepository repository = new StoryRepository(store);
            var result = repository.AddWord(story.Id, word, userId2);
            // Act


            // Assert
            Assert.AreEqual(result.ErrorCode, StoryErrorCode.Success);
            Assert.IsTrue(result.Story.Paragraphs.Last().EndsWith(word));

        }
        
        
        [Test]
        public void AddWordTenMinutesAfterLockReturnsError()
        {
            
            // Setup
            var store = Global.GetInMemoryStore();
            var userId = "users/1";
            var story = FakeEntityFactory.GetGenericStory(userId);
            story.Lock.UserId = userId;
            story.Lock.LockedDate = DateTime.Now.AddMinutes(-15);

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
                Global.UpdateIndex<Story>(session);
            }

            StoryRepository repository = new StoryRepository(store);
            var result = repository.AddWord(story.Id, "New", userId);
            // Act


            // Assert
            Assert.AreEqual(result.ErrorCode, StoryErrorCode.TenMinuteLockWindowHasClosed);

        }

        [Test]
        public void AddWordToNewStory()
        {
            // Setup
            var store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);

            string userId = "users/2";
            string word = "Once";

            // Act
            var result = repository.AddWord("", word, userId);

            // Assert
            using (var session = store.OpenSession())
            {
                var story = session.Load<Story>(result.Story.Id);

                Assert.AreEqual(story.LastEditorId, userId);
                Assert.IsNotNull(story.EditHistory);
                Assert.Greater(story.EditHistory[0].DateAdded, DateTime.Now.AddMinutes(-1));

                Assert.AreEqual(story.EditHistory[0].ParagraphIndex, 0);
                Assert.AreEqual(story.EditHistory[0].ParagraphNumber, 1);
                Assert.AreEqual(story.EditHistory[0].UserId, userId);

                Assert.AreEqual(story.EditHistory.Count, 1);


                Assert.AreEqual(story.Id, result.Story.Id);
                Assert.AreEqual(story.Paragraphs.Count, 1);
                Assert.AreEqual(story.Paragraphs[0], word);

            }

        }


        [Test]
        public void AddWordToOneParagraphExistingStory()
        {
            // Setup
            Story story = new Story();
            story.Paragraphs.Add("Here is the first paragraph of the story. Content doesn't really matter at the moment");

            var store = Global.GetInMemoryStore();

            SaveStory(story, store);

            var rep = new StoryRepository(store);

            // Act
            rep.AddWord(story.Id, "added.", "users/1");


            // Assert
            using (var session = store.OpenSession())
            {
                var savedStory = session.Load<Story>(story.Id);
                Assert.AreEqual(savedStory.Paragraphs[0], "Here is the first paragraph of the story. Content doesn't really matter at the moment added.");
            }

        }

        [Test]
        public void AddWordToMultiParagraphExistingStory()
        {
            // Setup
            Story story = new Story();
            story.Paragraphs.Add("Here is the first paragraph of the story. Content doesn't really matter at the moment");
            story.Paragraphs.Add("Here is the second paragraph of the story. Content doesn't really matter at the moment");



            var store = Global.GetInMemoryStore();

            SaveStory(story, store);

            StoryRepository repository = new StoryRepository(store);

            // Act
            repository.AddWord(story.Id, "Added", "users/1");


            // Assert
            using (var session = store.OpenSession())
            {
                var savedStory = session.Load<Story>(story.Id);
                Assert.IsTrue(savedStory.Paragraphs[1] == "Here is the second paragraph of the story. Content doesn't really matter at the moment Added");

            }

        }


        [Test]
        public void AddWordToExistingUpdatesEditHistory()
        {
            // Setup
            Story story = new Story();
            story.Paragraphs.Add("Here is the first paragraph of the story. Content doesn't really matter at the moment");

            var store = Global.GetInMemoryStore();

            SaveStory(story, store);

            StoryRepository repository = new StoryRepository(store);

            // Act
            repository.AddWord(story.Id, "Added", "users/1");

            // Assert
            using (var session = store.OpenSession())
            {
                var savedStory = session.Load<Story>(story.Id);

                Assert.Greater(savedStory.EditHistory[0].DateAdded, DateTime.Now.AddMinutes(-1));
                Assert.AreEqual(savedStory.EditHistory[0].ParagraphIndex, 86);
                Assert.AreEqual(savedStory.EditHistory[0].ParagraphNumber, 1);
                Assert.AreEqual(savedStory.EditHistory[0].UserId, "users/1");

            }
        }

        [Test]
        [ExpectedException(typeof(StoryHasEditorException))]
        public void AddWordToStoryWithCurrentEditorThrowsException()
        {
            // Setup
            Story story = new Story();

            story.Paragraphs.Add("Here is the first paragraph of the story. Content doesn't really matter at the moment");
            story.Lock.UserId = "users/2";
            story.Lock.LockedDate = DateTime.Now;

            IDocumentStore store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);

            SaveStory(story, store);

            repository.AddWord(story.Id, "word", "users/1");



        }


        [Test]
        public void LockStoryReturnsFalseIfStoryIsAlreadyLocked()
        {

            // Setup
            Story story = new Story();
            story.Lock.UserId = "users/1";

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            StoryRepository repository = new StoryRepository(store);

            // Act
            StoryErrorCode result = repository.LockStory(story.Id, "users/2");


            // Assert
            Assert.AreEqual(result, StoryErrorCode.StoryLockedForEditing);

        }



        [Test]
        public void LockStoryReturnsTrueIfStoryIsNotLocked()
        {

            Story story = new Story();

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            StoryRepository repository = new StoryRepository(store);

            // Act
            StoryErrorCode result = repository.LockStory(story.Id, "users/1");

            using (var session = store.OpenSession())
            {
                var savedStory = session.Load<Story>(story.Id);
                Assert.AreEqual(savedStory.Lock.UserId, "users/1");
                Assert.GreaterOrEqual(savedStory.Lock.LockedDate, DateTime.Now.AddMinutes(-1));
            }

            // Assert
            Assert.AreEqual(result, StoryErrorCode.Success);


        }


        [Test]
        public void AddWordReturnsProperErrorCodeIfStoryIsNotFound()
        {
            // Setup
            IDocumentStore store = Global.GetInMemoryStore();
            StoryRepository repository = new StoryRepository(store);

            // Act
            var result = repository.AddWord("story/id", "Add", "users/1");

            // Assert
            Assert.AreEqual(result.ErrorCode, StoryErrorCode.StoryNotFoundInRepository);

        }

        [Test]
        public void AddWordWontAllowTheSameUserToAddTwoConsecutiveWords()
        {
            // Setup
            Story story = new Story();
            string userId = "users/1";
            story.EditHistory.Add(new EditHistory()
            {
                DateAdded = DateTime.Now.AddHours(-4),
                UserId = userId

            });

            IDocumentStore store = Global.GetInMemoryStore();
            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            StoryRepository repository = new StoryRepository(store);

            // Act
            var result = repository.AddWord(story.Id, "Add", userId);

            // Assert
            Assert.AreEqual(result.ErrorCode, StoryErrorCode.UserAddedTheLastWordInThisStory);

        }


        private static void SaveStory(Story story, IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }
        }







    }
}

