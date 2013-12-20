﻿using System;
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
            IDocumentStore store = Global.GetInMemoryStore();

            User user = FakeEntityFactory.GetGenericUser();

            Story story1;
            Story story2;
            Story story3;
            Story story4;
            Story story5;

            using (var session = store.OpenSession())
            {
                session.Store(user);
                story1 = FakeEntityFactory.GetGenericStory(user.Id);
                story2 = FakeEntityFactory.GetGenericStory(user.Id);
                story3 = FakeEntityFactory.GetGenericStory(user.Id);
                story4 = FakeEntityFactory.GetGenericStory(user.Id);
                story5 = FakeEntityFactory.GetGenericStory(user.Id);
                session.Store(story1);
                session.Store(story2);
                session.Store(story3);
                session.Store(story4);
                session.Store(story5);
                session.SaveChanges();

                UpdateIndex(session);


                var st = session.Query<Story>().ToList<Story>();

            }

            // Act
            IStoryRepository rep = new StoryRepository(store);
            List<Story> result = rep.GetStoriesByUser(user.Id).Stories;

            // Assert
            Assert.AreEqual(result.Count(), 5);
            Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story1.Paragraphs[0])).Count() > 0);
            Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story2.Paragraphs[0])).Count() > 0);
            Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story3.Paragraphs[0])).Count() > 0);
            Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story4.Paragraphs[0])).Count() > 0);
            Assert.IsTrue(result.Where(s => s.Paragraphs.Contains(story5.Paragraphs[0])).Count() > 0);


        }



        private static void UpdateIndex(IDocumentSession session)
        {
            RavenQueryStatistics stats;
            var results = session.Query<Story>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResults())
                .ToArray();
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
                UpdateIndex(session);
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
            Assert.IsNotNull(story.CurrentEditor);
            Assert.IsNotNull(story.EditHistory);

        }


        [Test]
        public void GetStoryByIdThrowsException()
        {

            // Setup
            StoryRepository repository = new StoryRepository();
            Assert.Throws<ArgumentNullException>(() => repository.GetStoryById(""));
            Assert.Throws<ArgumentNullException>(() => repository.GetStoryById(null));

        }


        
        [Test]
        public void GetStoriesByUserOrderTest()
        {
            // Setup


            // Act


            // Assert
            Assert.Inconclusive();

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
                UpdateIndex(session);

                
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


            // Act


            // Assert
            Assert.Inconclusive();

        }

        
        [Test]
        public void PageNoWithNoPageSizeReturnsError()
        {
            // Setup


            // Act


            // Assert
            Assert.Inconclusive();

        }
        [Test]
        public void GetStoryByIdReturnsStory()
        {
            // Setup
            Story story = new Story()
            {
                CurrentEditor = new User() { Email = "email@email.com", Password = "asdjhaksjdh" },
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
            Global.AreEqualByJson(story.CurrentEditor, savedStory.CurrentEditor);
            Global.AreEqualByJson(story.EditHistory, savedStory.EditHistory);
            Assert.AreEqual(story.Id, savedStory.Id);
            Global.AreEqualByJson(story.Paragraphs, savedStory.Paragraphs);

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

                Assert.IsNullOrEmpty(story.CurrentEditor.Id);
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
            story.CurrentEditorId = "users/1";

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
            story.CurrentEditorId = "users/1";

            IDocumentStore store = Global.GetInMemoryStore();

            using (var session = store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
            }

            StoryRepository repository = new StoryRepository(store);

            // Act
            StoryErrorCode result = repository.LockStory(story.Id, "users/1");


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
                Assert.AreEqual(savedStory.CurrentEditorId, "users/1");
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

