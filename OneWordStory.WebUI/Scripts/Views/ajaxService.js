/// <reference path="../jquery-2.0.3.js" />
/// <reference path="../knockout-3.0.0.js" />

var nm = {};

nm.ajaxService = (function () {

    var ajaxGetJson = function (url, callback, errorCallback) {

        $.ajax({

            url: url,
            type: "GET",
            contentType: "application/json",
            success: function (json) {
                callback(json);
            },
            error: function () {
                errorCallback();
            }

        });
    },


    ajaxPostJson = function (url, jsonIn, successCallback, errorCallback) {

        $.ajax({

            url: url,
            type: "POST",
            data: ko.toJSON(jsonIn || "{}"),
            datatype: "json",
            contentType: "application/json",
            success: function (json) {
                successCallback(json);
            },
            error: function () { errorCallback() }


        });

    };

    return {

        ajaxGetJson: ajaxGetJson,
        ajaxPostJson: ajaxPostJson
    }

})();