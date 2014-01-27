/// <reference path="../../knockout-3.0.0.js" />
/// <reference path="../../jquery-2.0.3.js" />
/// <reference path="../ajaxService.js" />

/// <reference path="Index.DataServices.js" />





$(function () {




    nm.Story = function () {
        this.DocumentId = ko.observable();
        this.Paragraphs = ko.observableArray([]);
        this.CurrentEditorId = ko.observable();
        this.LastEditorId = ko.observable();


    };

    nm.StoryPreview = function () {

        this.Preview = "";
        this.documentId = 0;


    };

    nm.vm = function () {

        var UserStoryList = ko.observableArray([]),
            CurrentStory = ko.observable(),
            ShowAddWordForm = ko.observable(false),
            ShowAddWordButton = ko.observable(false),
            SelectStoryError = ko.observable(''),
            WordToAdd = ko.observable(''),
            AddParagraph = ko.observable(false),
            HideToggles = function () {
                ShowAddWordButton(false);
                ShowAddWordForm(false);
                SelectStoryError('');


            },
            GetCurrentStory = function (data) {
                nm.getStory(data.documentId, GetCurrentStoryCallback, GetCurrentStoryErrorCallback)
            },
            GetCurrentStoryCallback = function (data) {
                var story = new nm.Story().CurrentEditorId(data.CurrentEditorId)
                     .LastEditorId(data.LastEditorId).DocumentId(data.DocumentId);
                story.Paragraphs(data.Paragraphs);

                CurrentStory(story);

            },
            GetCurrentStoryErrorCallback = function (data) {

            },
            AddWord = function (data) {

                nm.addWord(data.DocumentId(), WordToAdd(), AddParagraph(), AddWordCallback, AddWordErrorCallback)

            },
            AddWordCallback = function (data) {
                HideToggles();
                if (data.ErrorCodeDescription) {
                    SelectStoryError(data.ErrorCodeDescription);
                }
                else {
                    nm.getStory(CurrentStory().DocumentId(), GetCurrentStoryCallback, GetCurrentStoryErrorCallback)
                }
            },
            AddWordErrorCallback = function (data) {
                HideToggles();
                SelectStoryError('An error has occurred. Please re-select the story to try again.');
            },
            LockStory = function (data) {

                nm.lockStory(data.DocumentId(), LockStoryCallback, LockStoryErrorCallback)
            },
            LockStoryCallback = function (data) {
                HideToggles();
                ShowAddWordForm(true);
            },
            LockStoryErrorCallback = function (data) {
                HideToggles();
                SelectStoryError('An error has occurred. Please re-select the story to try again.');
            },
            LoadUserStoriesList = function () {

                nm.getStories(LoadUserStoriesListCallback, LoadUserStoriesListErrorCallback);

            },
            LoadUserStoriesListCallback = function (data) {

                $.each(data, function (index, value) {
                    var storyPreview = new nm.StoryPreview();
                    storyPreview.Preview = value.Preview;
                    storyPreview.documentId = value.DocumentId;
                    UserStoryList.push(storyPreview);
                });


            },
            LoadUserStoriesListErrorCallback = function (data) {



            };

        return {

            UserStoryList: UserStoryList,
            CurrentStory: CurrentStory,
            LoadUserStoriesList: LoadUserStoriesList,
            LoadUserStoriesListCallback: LoadUserStoriesListCallback,
            GetCurrentStory: GetCurrentStory,
            ShowAddWordForm: ShowAddWordForm,
            ShowAddWordButton: ShowAddWordButton,
            SelectStoryError: SelectStoryError,
            HideToggles: HideToggles,
            LockStory: LockStory,
            AddParagraph: AddParagraph,
            AddWord: AddWord,
            WordToAdd: WordToAdd

        }

    }();


    nm.vm.CurrentStory.subscribe(function (newValue) {

        nm.vm.HideToggles();

        if (newValue.LastEditorId() === currentUser) {
            nm.vm.SelectStoryError('You were the last one to add a word to this story. You must wait for another person to add a word.');

        }
        else {
            nm.vm.ShowAddWordButton(true);
        }


    });

    nm.vm.LoadUserStoriesList();
    ko.applyBindings(nm.vm);

});


