﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.Domain;
using Raven.Abstractions.Exceptions;
using Raven.Client;
using OneWordStory.Domain.Indexes;

namespace OneWordStory.Concrete
{

    

    public class StoryRepository : IStoryRepository
    {



        private IDocumentStore _store;



        public StoryRepository()
        {
            _store = DocumentStoreSingleton.DocumentStore;
        }

        public StoryRepository(IDocumentStore store)
        {
            _store = store;
        }

        public GetStoriesResult GetStoriesByUser(string userId, int pageNo = 0, int pageSize = 0)
        {

            if (pageNo > 0 && pageSize < 1) throw new ArgumentException("Must include Page Size with Page No");
            if (pageSize > 0 && pageNo < 1) throw new ArgumentException("Must include Page No with Page Size");

            using (var session = _store.OpenSession())
            {
                var stats = new RavenQueryStatistics();
                var result = session.Query<Story, StoriesByUser>()
                    .Statistics(out stats)
                    .Where(s => s.EditHistory.Any(eh => eh.UserId == userId));

                var Stories = result.ToList<Story>();

                 if (pageNo > 0)
                {
                    result = result.Skip<Story>(pageSize * (pageNo - 1));
                    result = result.Take<Story>(pageSize);
                }

                return new GetStoriesResult 
                { 
                    Stories = result.ToList<Story>(), 
                    IsStale = stats.IsStale 
                };

            }
            
        }


        public Story GetStoryById(string storyId)
        {
            if (string.IsNullOrEmpty(storyId)) throw new ArgumentNullException("storyId");

            using (var session = _store.OpenSession())
            {
                return session.Load<Story>(storyId);
            }

        }

        public AddWordResult AddWord(string storyId, string word, string userId, bool addParagraph = false)
        {

            if (string.IsNullOrEmpty(word)) throw new ArgumentNullException("word");
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");


            var story = new Story();

            if (string.IsNullOrEmpty(storyId))
            {
                CreateNewStory(word, userId, story);
                return new AddWordResult() { ErrorCode = StoryErrorCode.Success, Story = story };
                
            }
            else
            {
                var result = AddToExistingStory(word, userId, storyId, addParagraph);
                return result;
            }

            

        }

        private AddWordResult AddToExistingStory(string word, string userId, string storyId, bool addParagraph)
        {
            using (var session = _store.OpenSession())
            {
                var story = session.Load<Story>(storyId);
                if (story == null) return new AddWordResult() { ErrorCode = StoryErrorCode.StoryNotFoundInRepository, Story = new Story() }; 

                if (story.HasEditor 
                    && story.Lock.UserId != userId
                    && story.Lock.LockedDate > DateTime.Now.AddMinutes(-10)) throw new StoryHasEditorException();

                if (story.HasEditHistory && story.EditHistory.Last<EditHistory>().UserId == userId)
                    return new AddWordResult() { ErrorCode = StoryErrorCode.UserAddedTheLastWordInThisStory, Story = story };

                if (story.Lock.UserId == userId && story.Lock.LockedDate < DateTime.Now.AddMinutes(-10)) return new AddWordResult() { ErrorCode = StoryErrorCode.TenMinuteLockWindowHasClosed, Story = new Story() };


                if (addParagraph)
                {
                    story.Paragraphs.Add("");
                }
                else
                {
                    word = " " + word;
                }
                
                int paragraphIndex = story.Paragraphs.Count - 1;

                int preAddParagraphLength = story.Paragraphs[paragraphIndex].Length;
                string lastParagraph = story.Paragraphs[paragraphIndex];
                
                lastParagraph += word;
                story.Paragraphs[paragraphIndex] = lastParagraph;
                
                story.EditHistory.Add(new EditHistory()
                {
                    DateAdded = DateTime.Now,
                    ParagraphIndex = preAddParagraphLength + 1,
                    ParagraphNumber = paragraphIndex + 1,
                    UserId = userId

                });

                story.Lock = new StoryLock();

                session.SaveChanges();
                return new AddWordResult() { ErrorCode = StoryErrorCode.Success, Story = story }; ;
            }
        }

        private void CreateNewStory(string word, string userId, Story story)
        {
            story.EditHistory.Add(new EditHistory()
            {
                DateAdded = DateTime.Now,
                ParagraphIndex = 0,
                ParagraphNumber = 1,
                UserId = userId

            });
            story.Paragraphs.Add(word);

            using (var session = _store.OpenSession())
            {
                session.Store(story);
                session.SaveChanges();
                
            }
        }

        
        

        /// <summary>
        /// LockStory so that only that user can add a word
        /// </summary>
        /// <param name="storyId">Story Id of the story to lock</param>
        /// <param name="userId">User Id of the locking User</param>
        /// <returns></returns>
        public StoryErrorCode LockStory(string storyId, string userId)
        {
            using (var session = _store.OpenSession())
            {
                session.Advanced.UseOptimisticConcurrency = true;
                var story = session.Load<Story>(storyId);
                if (story == null) return StoryErrorCode.Unknown;

                if (!string.IsNullOrEmpty(story.Lock.UserId) && story.Lock.UserId != userId) return StoryErrorCode.StoryLockedForEditing;

                story.Lock.UserId = userId;
                story.Lock.LockedDate = DateTime.Now;

                try
                {
                    session.Store(story);
                    session.SaveChanges();
                    return StoryErrorCode.Success;
                }
                catch (ConcurrencyException)
                {
                    return StoryErrorCode.StoryIsBeingUpdated;
                }

                
            }
        }





        public List<Story> GetRandomStories(int amount)
        {
             using (var session = _store.OpenSession())
            {
                var result = session.Query<Story>().Customize(q => q.RandomOrdering()).Take(amount);
                return result.ToList<Story>();
            }
        }
    }
}

