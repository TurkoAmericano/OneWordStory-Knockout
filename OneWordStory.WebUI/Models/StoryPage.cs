using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OneWordStory.Domain.Entities;
using Raven.Imports.Newtonsoft.Json;

namespace OneWordStory.WebUI.Models
{
    public class StoryPage
    {

        public List<Story> UserStories { get; set; }

        [Required(ErrorMessage="Please enter a word")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage="Please enter only letters and/or numbers")]
        public string WordForNewStory { get; set; }

        [JsonIgnore]
        public string ReadStory { get; set; }

        public bool IsStale { get; set; }


        public StoryPage()
        {
            UserStories = new List<Story>();
        }

    }


    

}
