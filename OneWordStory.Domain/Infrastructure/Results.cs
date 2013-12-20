using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Domain.Entities;


namespace OneWordStory.Domain.Infrastructure
{
    public class AddWordResult
    {

        public AddWordResult()
        {
            Story = new Story();
        }
        
        public StoryErrorCode ErrorCode { get; set; }
        public Story Story { get; set; }
    }

    public class GetUserResult
    {

        public GetUserResult()
        {
            User = new User();
        }

        public UserErrorCode UserCode { get; set; }
        public User User { get; set; }

    }

    public class GetStoriesResult
    {

        public GetStoriesResult()
        {
            Stories = new List<Story>();
        }

        public bool IsStale { get; set; }
        public List<Story> Stories { get; set; }
    }

}
