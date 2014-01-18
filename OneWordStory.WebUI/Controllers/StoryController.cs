using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OneWordStory.Domain.Abstract;
using OneWordStory.WebUI.Controllers.Adapters;
using OneWordStory.WebUI.Models;

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
            return View(GetStoryPage());
        }


        private StoryPage GetStoryPage(int id = 0)
        {


            var result = _repository.GetStoriesByUser(_currentUser.UserID);

            var storyPage = new StoryPage
            {
                UserStories = result.Stories,

            };

            if (id > 0)
            {
                var storyResult = _repository.GetStoryById("stories/" + id);



                storyPage.ReadStory = storyResult;
            }

            return storyPage;

        }


        [HttpPost]
        public JsonResult GetStories(StoryPage storyPage)
        {
            var storyResult = _repository.GetStoriesByUser(_currentUser.UserID);

            var returnValue = new StoryPage 
            {
                 UserStories = storyResult.Stories,
                  IsStale = storyResult.IsStale,

            };

            return Json(returnValue, "application/json");

        }

        [HttpPost]
        public JsonResult AddWord(StoryPage storyPage)
        {
            throw new NotImplementedException();

        }


        public ActionResult CreateNewStory(StoryPage storyPage)
        {

            if (!ModelState.IsValid) return View("Index");

            _repository.AddWord(string.Empty, storyPage.WordForNewStory, _currentUser.UserID);

            return View("Index", GetStoryPage());
        
        }

        public JsonResult ReadStory(int id)
        {

            if (id < 1) throw new ArgumentException("Story Id");

            var story = _repository.GetStoryById("stories/" + id);

            return Json(story, "application/json");
        }



    }
}
