using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneWordStory.Domain.Infrastructure
{

    

        public enum StoryErrorCode
        { 
            [Description("You added the last word in the story, you must wait until someone else adds a word.")]
            UserAddedTheLastWordInThisStory,
            [Description("Someone else is currently adding a word to this story.")]
            StoryLockedForEditing,
            [Description("This story is currently being updated. Please try again in a minute.")]
            StoryIsBeingUpdated,
            [Description("More than ten minutes have passed since you locked this story. Click 'Add word' to re-lock the story.")]
            TenMinuteLockWindowHasClosed,
            [Description("An unknown error has occurred.")]
            StoryNotFoundInRepository,
            [Description("An unknown error has occurred.")]
            Unknown,
            Success
        }

        public enum UserErrorCode
        {
            [Description("There alreaedy is an account in the system with that email address.")]
            EmailAlreadyExists,
            [Description("Login failed.")]
            LoginFailed,
            [Description("An unknown error has occurred.")]
            UserNotFoundInRepository,
            [Description("An unknown error has occurred.")]
            EmailNotFoundInRepository,
            Success
        }

    
}
