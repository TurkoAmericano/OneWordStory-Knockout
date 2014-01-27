using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OneWordStory.Domain.Entities;
using Raven.Imports.Newtonsoft.Json;

namespace OneWordStory.WebUI.Models
{
    public class StoryPageView
    {


        public UserStoryList UserStoryList { get; set; }
        public AddWord AddWord { get; set; }

        public StoryPageView()
        {
            UserStoryList = new UserStoryList();
            AddWord = new AddWord();
        }
        

    }

    public class AddWord
    {

        [Required(ErrorMessage = "Please enter a word")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Please enter only letters and/or numbers")]
        public string Word { get; set; }

        public string StoryId { get; set; }

        public bool NewParagraph { get; set; }

        public AddWord()
        {
            Word = "";
            StoryId = "";
        }

    }

    public class UserStoryList
    {

        public List<Story> UserStories { get; set; }
        public bool IsStale { get; set; }
        public UserStoryList()
        {
            UserStories = new List<Story>();
        }

    }
    

}
