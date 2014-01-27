/// <reference path="../ajaxService.js" />
var getStory = function (id) {

	$.ajax({
		url: '/Story/ReadStory/' + id,
		dataType: 'json',
		type: 'POST'
	})
	.success(function (result) {

		var $storyBody = $('#storyBody').html('');
		var $postStoryBody = $('#postStoryBody');
		

		$storyBody.append('<input id="storyId" type="hidden" />');
		$('#storyId').val(result.DocumentId);

		$.each(result.Paragraphs, function (index, value) {
			$storyBody.append('<p>' + value + '</p>');
		});


		if (result.LastEditorId === '@User.Identity.Name')
			$postStoryBody.append('<p class="text-primary">You are the last person to add a word to this story</p>');
		else
			$postStoryBody.append('<button type="button" id="addWord" class="btn btn-primary btn-xs">Add a word</button>');

		$('#addWord').click(lockStoryForEdit);





	})
	.error(function (xhRequest, ErrorText, thrownError) {

	});


};



nm.lockStoryForEdit = function () {

	$('#postStoryBody').html("");
	var $addWordForm = $('#addWordForm');
	var $addWordError = $('#addWordError');

	$.ajax({
		url: '/Story/LockStory/' + $('#storyId').val(),
		dataType: 'json',
		type: 'POST'
	})
	.success(function (result) {

		if (result === "true") {
			$addWordForm.show();
			
		}
		else {
			$addWordError.append('<p class="text-danger">' + result + '<p>');
		}

	})
	.error(function (xhRequest, ErrorText, thrownError) {
		$addWordError.append('<p class="text-danger">' + ErrorText + '<p>');
	});


};

var addWordValidate = function () {

	$addWordError = $('#inputWord').html();

	var word = new RegExp(/^[\S]*$/);

	if (!$('#inputWord').val().match(word)) {
		$('addWordError').val('Word cannot contain spaces.');
		return false;
	}

	if ($('#inputWord').val() === "") {
		$('#addWordError').text('Please enter a word.');
		return false;
	}

	return true;
};

var addWord = function () {


	data = ko.toJSON(model);
	alert(data);
	
	return;

	$.ajax({
		url: '/Story/AddWord',
		dataType: 'json',
		type: 'POST',
		data: data
	})
	.success(function (result) {




	})
	.error(function (xhRequest, ErrorText, thrownError) {

	});

};

var displayStories = function () {



	$.ajax({
		url: '/Story/GetStories',
		dataType: 'json',
		type: 'POST'
	})
	.success(function (result) {

		result = result.UserStoryList;

		if (result.UserStories.length < 1) {

			$("#StoryList").html("<p>You have no stories.</p>");
			return;
		}



		var list = [];
		$.each(result.UserStories, function (index, value) {
			var template = $("#ReadStoryTemplate").clone().html();
			var html = template.replace("{PREVIEW}", value.Preview);
			html = html.replace("{STORY_ID}", value.Id.replace("stories\/", ""));
			list.push(html);
		});


		$("#StoryList").html(list.join(""))

		if (result.IsStale) {

			counter++;
			$("#StoryList").append("<span class='typeout'>Updating...</span>");
			setTimeout(displayStories, 1000);
		}


	})
	.error(function (xhRequest, ErrorText, thrownError) {

	});
};