/// <reference path="../ajaxService.js" />


nm.getStories = function (callback, errorCallback) {
    
    nm.ajaxService.ajaxPostJson("/Story/GetStories/", "", callback, errorCallback);
    

};

nm.getStory = function (id, callback, errorCallback) {

    nm.ajaxService.ajaxPostJson("/Story/ReadStory/" + id, "", callback, errorCallback);


};

nm.lockStory = function (id, callback, errorCallback) {

    nm.ajaxService.ajaxPostJson("/Story/LockStory/" + id, "", callback, errorCallback);


};

nm.addWord = function (id, word, newParagraph, callback, errorCallback) {


    var json = {
        StoryId: id,
        Word: word,
        NewParagraph: newParagraph
    };

    nm.ajaxService.ajaxPostJson("/Story/AddWord/", json, callback, errorCallback);


};
