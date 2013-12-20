using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneWordStory.Concrete;
using OneWordStory.Domain.Entities;
using OneWordStory.Domain.Infrastructure;

namespace OneWordStory.Domain.Abstract
{
    public interface IStoryRepository
    {

        Story GetStoryById(string storyId);
        AddWordResult AddWord(string storyID, string word, string userId);
        StoryErrorCode LockStory(string storyId, string userId);
        GetStoriesResult GetStoriesByUser(string userId, int pageNo = 0, int pageSize = 0);
        List<Story> GetRandomStories(int amount);
    }
}
