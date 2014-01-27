using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Imports.Newtonsoft.Json;
using Extensions;

namespace OneWordStory.Domain.Entities
{
    public class Story : BaseEntity
    {

        
        public List<string> Paragraphs { get; set; }
        
        public StoryLock Lock { get; set; }

        
        [JsonIgnore]
        public string LastEditorId
        {
            get
            {
                
                if (EditHistory.Count > 0)
                    return EditHistory.Last().UserId;
                else
                    return string.Empty;
                    
            }
            
        }
        public List<EditHistory> EditHistory { get; set; }

        public Story()
        {
            Paragraphs = new List<string>();
            EditHistory = new List<EditHistory>();
            Lock = new StoryLock();
            
        }

        [JsonIgnore]
        public string Preview 
        {
            get
            {
                if (Paragraphs.Count > 0)
                    return Paragraphs[0].SafeSubstring(0, 15, true);
                else
                    return string.Empty;

            }
            
        }

        [JsonIgnore]
        public bool HasEditor
        {
            get
            {
                return !string.IsNullOrEmpty(Lock.UserId);
            }
        }

        [JsonIgnore]
        public bool HasEditHistory
        {
            get
            {
                return EditHistory.Count > 0;
            }
        }



        
 
    }

    public class EditHistory
    {
        public string UserId { get; set; }
        public int ParagraphNumber { get; set; }
        public int ParagraphIndex { get; set; }
        public DateTime DateAdded { get; set; }

    }

    public class StoryLock
    {
        public string UserId { get; set; }
        public DateTime LockedDate { get; set; }
    }
}
