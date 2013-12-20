using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;

namespace OneWordStory.Tests
{
    public static class FakeEntityFactory
    {

        private static Random _random = new Random();

        public static User GetGenericUser()
        {
            return new User {

                Email = Guid.NewGuid().ToString().Substring(0, 8) +  "@email.com",
                Password = Guid.NewGuid().ToString().Substring(0, 10)
                               
            };
        }

        public static Story GetSpecificStoryWithText(string userId, List<string> paragraphs)
        {
            Story story = new Story
            {
                EditHistory = GetGenericEditHistory(userId),
                Paragraphs = paragraphs
            };

            return story;
        }


        public static Story GetGenericStory(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = "users/" + _random.Next(9999).ToString();
            }

            Story story = new Story
            {
                EditHistory = GetGenericEditHistory(userId),
                Paragraphs = new List<string>
                  {
                      Guid.NewGuid().ToString(),
                      Guid.NewGuid().ToString(),
                      Guid.NewGuid().ToString(),
                      Guid.NewGuid().ToString(),
                      Guid.NewGuid().ToString()
                  }
            };

            return story;

        }

        private static List<EditHistory> GetGenericEditHistory(string userId)
        {

            if (string.IsNullOrEmpty(userId))
                userId = "users/90348";

            var editHistory = new List<EditHistory>()
            {
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = "users/827634" },    
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = "users/827634" } ,   
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = "users/827634" } ,  
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = userId },    
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = "users/827634" },    
              new EditHistory{ DateAdded = DateTime.Now, ParagraphIndex = 0, ParagraphNumber = 3, UserId = "users/827634" }    

            };

            return editHistory;
        }

    }
}
