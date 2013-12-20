using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace OneWordStory.Domain.Indexes
{
    public class StoriesByUser : AbstractIndexCreationTask<Story>
    {
        public override string IndexName
        {
            get
            {
                return "Stories/ByUser";
            }
        }


        public StoriesByUser()
        {
            Map = stories => from story in stories
                             from history in story.EditHistory
                             select new 
                             { 
                                 EditHistory = history,
                                 EditHistory_UserId = history.UserId
                             };

            Sort(s => s.DocumentId, SortOptions.Int);
                                
            
        }

    }
}
