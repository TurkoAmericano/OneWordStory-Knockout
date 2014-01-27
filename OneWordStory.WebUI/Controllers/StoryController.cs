using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Infrastructure;
using OneWordStory.WebUI.Controllers.Adapters;
using OneWordStory.WebUI.Models;
using Extensions;

namespace OneWordStory.WebUI.Controllers
{
    [Authorize]
    public class StoryController : Controller
    {

        private IStoryRepository _repository;
        CurrentUser _currentUser;

        public StoryController(IStoryRepository repository, CurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        //
        // GET: /Story/
        public ActionResult Index()
        {
            return View(new StoryPageView());
        }




        [HttpPost]
        public JsonResult GetStories()
        {
            var storyResult = _repository.GetStoriesByUser(_currentUser.UserID);

            UserStoryList returnValue = new UserStoryList { UserStories = storyResult.Stories, IsStale = storyResult.IsStale };
            
            return Json(returnValue.UserStories, "application/json");

        }

        [HttpPost]
        public JsonResult AddWord(AddWord addWord)
        {

            if (string.IsNullOrEmpty(addWord.StoryId)) throw new Exception("Story is required");
            if (string.IsNullOrEmpty(addWord.Word)) throw new Exception("Word is required");

            var result = _repository.AddWord("stories/" + addWord.StoryId, addWord.Word, _currentUser.UserID, addWord.NewParagraph);

            return Json(result, "application/json");

        }


        public ActionResult CreateNewStory(AddWord addWord)
        {

            if (!ModelState.IsValid) return View("Index");

            _repository.AddWord(string.Empty, addWord.Word, _currentUser.UserID);

            return View("Index");
        
        }

        public JsonResult ReadStory(int id)
        {

            if (id < 1) throw new ArgumentException("Story Id");

            var story = _repository.GetStoryById("stories/" + id);

            return Json(story, "application/json");
        }

        
        public JsonResult LockStory(int id)
        {

            StoryErrorCode result = _repository.LockStory("stories/" + id.ToString(), _currentUser.UserID);

            if (result == StoryErrorCode.Success)
            {
                return Json("true", "application/json");
            }
            else
            {
                return Json(result.GetEnumDescription(), "application/json");
            }
        
        }



    }
}
