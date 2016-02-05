'use strict';
define([appLocation.userPostLogin], function (app) {

    app.controller('UserAfterLoginSurvey', function ($scope, $http, $route, $rootScope, $routeParams, CookieUtil) {
        $scope.logoImage = { url: logoImage };
        //showToastMessage("Error", "Title of the Template cann't be empty");
        $('title').html("index"); //TODO: change the title so cann't be tracked in log
        $scope.userSurveyResult = {
            surveySingleAnswerQuestion: [],
            surveyMultipleAnswerQuestion: [],
            surveyListBoxAnswerQuestion: [],
            surveyTextBoxAnswerQuestion: []
        };

        $scope.attemptedSurveyQuestions = [];

        function insertAttemptedSurveyQuestionsList(key) {
            $scope.attemptedSurveyQuestions.push({ key: key, attempted: false });
        }
        function removeAttemptedSurveyQuestionsList(key) {
            var i;
            for (i = 0; i < $scope.attemptedSurveyQuestions.length; i++) {
                if ($scope.attemptedSurveyQuestions.key == key) {
                    break;
                }
            }
            $scope.attemptedSurveyQuestions.splice(i, 1);
        }
        function checkEveryQuestionsAttempted() {
            var i, flag = true;
            for (i = 0; i < $scope.attemptedSurveyQuestions.length; i++) {
                if ($scope.attemptedSurveyQuestions[i].attempted == false) {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
        function markAttemptedSurveyQuestionsList(key) {
            var i;
            for (i = 0; i < $scope.attemptedSurveyQuestions.length; i++) {
                if ($scope.attemptedSurveyQuestions[i].key == key) {
                    break;
                }
            }
            $scope.attemptedSurveyQuestions[i].attempted = true;
            console.log($scope.attemptedSurveyQuestions);
        }
        function unmarkAttemptedSurveyQuestionsList(key) {
            var i;
            console.log(key);
            for (i = 0; i < $scope.attemptedSurveyQuestions.length; i++) {
                if ($scope.attemptedSurveyQuestions[i].key == key) {
                    break;
                }
            }

            //console.log(key);
            $scope.attemptedSurveyQuestions[i].attempted = false;
            console.log($scope.attemptedSurveyQuestions);
        }
        //        $scope.surveyInfoTitle = "This is the title of the survey";

        //        $scope.surveyInfoInstruction = {
        //            type: "",
        //            subType: "",
        //            data: [
        //                { instruction: "The Question relates to your life , so they are simple and can be easily answered." },
        //                { instruction: "This survey is only to know the basics of human mentality, so you need not to worry , just choose the best option according to you." }
        //            ]
        //        };

        //        $scope.surveyInfoSingleAnswerQuestion = {
        //            type: "",
        //            subType: "",
        //            data: [
        //                   { id: "SAQ-1", question: "Where do you live?", options: splitOptionsToList("Mumbai;Delhi;Kolkata;Chennai"), answer: "-" },
        //                   { id: "SAQ-2", question: "What is your favorite passtime?", options: splitOptionsToList("Studying;Playing;Dancing;Coding"), answer: "-" },
        //                   { id: "SAQ-3", question: "Which among these is animal?", options: splitOptionsToList("<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/wom619S.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/wom619Ss.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/FhD2x5H.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/FhD2x5Hs.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/TvI9dOg.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/TvI9dOgs.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/6oXVy0a.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/6oXVy0as.jpg\" class=\"MaxUploadedSmallSized\"></a>"), answer: "-" },
        //                   { id: "SAQ-4", question: "is the following image obscene?</b></p><a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/z8oQdAh.png\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/z8oQdAhs.png\" class=\"MaxUploadedSmallSized\"></a>", options: splitOptionsToList("Yes;No"), answer: "-" },
        //                   { id: "SAQ-5", question: "Can you Name this famous Personality?</b></p> <a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://upload.wikimedia.org/wikipedia/commons/d/d7/Bundesarchiv_Bild_183-61849-0001%2C_Indien%2C_Otto_Grotewohl_bei_Ministerpr%C3%A4sident_Nehru_cropped.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://upload.wikimedia.org/wikipedia/commons/d/d7/Bundesarchiv_Bild_183-61849-0001%2C_Indien%2C_Otto_Grotewohl_bei_Ministerpr%C3%A4sident_Nehru_cropped.jpg\" class=\"MaxUploadedSmallSized\"></a>", options: splitOptionsToList("Indra Gandi;Jawahar Lal Nehru;Amitabh Bacchan;Abdul Kalam"), answer: "-" }

        //            ]
        //        };

        //        $scope.surveyInfoMultipleAnswerQuestion = {
        //            type: "",
        //            subType: "",
        //            data: [
        //            { id: "MAQ-1", question: "Which among these is perfect buy Phone in the Market?", options: splitOptionsToList("Iphone 5s;Samsung Galaxy S5;Moto X;Moto E"), answer: "-" },
        //            { id: "MAQ-2", question: "Which among follower cricket deserves Bharat Ratna Award?", options: splitOptionsToList("Sachin Tendulkar;Rahul Dravid;Kapil Dev;Narayana Kartiken;Dhayn Chnadra;Rahul Gandhi;Narendra Modi;Shahrukh Khan"), answer: "-" },
        //            { id: "MAQ-3", question: "Which among these is animal?", options: splitOptionsToList("<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/wom619S.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/wom619Ss.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/FhD2x5H.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/FhD2x5Hs.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/TvI9dOg.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/TvI9dOgs.jpg\" class=\"MaxUploadedSmallSized\"></a>;<a title=\"Personalized Title\" data-fancybox-group=\"gallery\" href=\"http://i.imgur.com/6oXVy0a.jpg\" class=\"fancybox\"><img alt=\"\" src=\"http://i.imgur.com/6oXVy0as.jpg\" class=\"MaxUploadedSmallSized\"></a>"), answer: "-" },
        //            { id: "MAQ-4", question: "News Channel which are Transparent in india?", options: splitOptionsToList("Aaj Tak;NDTV;ZEE News;DD News"), answer: "-" },

        //            ]
        //        };

        //        $scope.surveyInfoListBoxAnswerQuestion = {
        //            type: "",
        //            subType: "",
        //            data: [
        //                { id: "LAQ-1", question: "Which among these is perfect buy Phone in the Market?", options: splitOptionsToList("Iphone 5s;Samsung Galaxy S5;Moto X;Moto E"), answer: "-" },
        //                { id: "LAQ-2", question: "Which among follower cricket deserves Bharat Ratna Award?", options: splitOptionsToList("Sachin Tendulkar;Rahul Dravid;Kapil Dev;Narayana Kartiken;Dhayn Chnadra;Rahul Gandhi;Narendra Modi;Shahrukh Khan"), answer: "-" },
        //                { id: "LAQ-3", question: "News Channel which are Transparent in india?", options: splitOptionsToList("Aaj Tak;NDTV;ZEE News;DD News"), answer: "-" },

        //            ]
        //        };

        //        $scope.surveyInfoTextBoxAnswerQuestion = {
        //            type: "",
        //            subType: "",
        //            data: [
        //                { id: "TAQ-1", question: "Enter your City Pin Code?", options: splitOptionsToList("Iphone 5s;Samsung Galaxy S5;Moto X;Moto E"), answer: "-" },
        //                { id: "TAQ-2", question: "What is your Favorite passtime?", options: splitOptionsToList("Iphone 5s;Samsung Galaxy S5;Moto X;Moto E"), answer: "-" }

        //            ]
        //        };

        $scope.surveyInfoTitle = "";
        $scope.surveyInfoInstruction = {};
        $scope.surveyInfoSingleAnswerQuestion = {};
        $scope.surveyInfoMultipleAnswerQuestion = {};
        $scope.surveyInfoListBoxAnswerQuestion = {};
        $scope.surveyInfoTextBoxAnswerQuestion = {};
        var isDemo = false;
        if ($routeParams.isDemo == "demo") {
            isDemo = true;
        }
        var url = ServerContextPath.empty + '/User/GetTemplateSurveyQuestionsByRefKey?refKey=' + $routeParams.refKey;
        var headers = {
            'Content-Type': 'application/json',
            'UTMZT': CookieUtil.getUTMZT(),
            'UTMZK': CookieUtil.getUTMZK(),
            'UTMZV': CookieUtil.getUTMZV()
        };
        startBlockUI('wait..', 3);
        $http({
            url: url,
            method: "POST",
            data: "",
            headers: headers
        }).success(function (data, status, headers, config) {
            //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
            stopBlockUI();
            if (data.Status == "200") {
                $scope.surveyInfoTitle = data.Payload.surveyTitle;
                $scope.surveyInfoInstruction = data.Payload.Instructions;
                $scope.surveyInfoSingleAnswerQuestion = data.Payload.SingleAnswerQuestion;
                $scope.surveyInfoMultipleAnswerQuestion = data.Payload.MultipleAnswerQuestion;
                $scope.surveyInfoListBoxAnswerQuestion = data.Payload.ListBoxAnswerQuestion;
                $scope.surveyInfoTextBoxAnswerQuestion = data.Payload.TextBoxAnswerQuestion;
                $scope.type = data.Payload.type;
                $scope.subType = data.Payload.subType;
                renderPageAfterAjaxCall();
                console.log($scope.attemptedSurveyQuestions);
            }

        }).error(function (data, status, headers, config) {

        });

        function renderPageAfterAjaxCall() {
            var renderSurveyQuestion = "";

            if (mobileDevice) {
                // instruction list
                if ($scope.surveyInfoInstruction.data.length != 0) {
                    renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                    renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                    renderSurveyQuestion += "<div class='box-title'>";
                    renderSurveyQuestion += "<h3>";
                    renderSurveyQuestion += "<i class='icon-file'></i>";
                    renderSurveyQuestion += "Instructions";
                    renderSurveyQuestion += "</h3>";
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "<div class='box-content'>";
                    $.each($scope.surveyInfoInstruction.data, function () {
                        renderSurveyQuestion += "<li>" + this.instruction + "</li>";
                    });
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                // single answer question..
                if ($scope.surveyInfoSingleAnswerQuestion.data.length != 0) {
                    $.each($scope.surveyInfoSingleAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                        renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                        renderSurveyQuestion += "<div class='box-title'>";
                        renderSurveyQuestion += "<h3>";
                        renderSurveyQuestion += "<i class='icon-file'></i>";
                        renderSurveyQuestion += "Single Answer Question";
                        renderSurveyQuestion += "</h3>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "<div class='box-content'>";
                        renderSurveyQuestion += "<p><b>";
                        renderSurveyQuestion += this.question;
                        renderSurveyQuestion += "</b></p>";
                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var singleQuestionsOptionList = this.options.split(';');
                        for (var j = 0; j < singleQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<input type='radio' class='userSurveyRadioButton' name='" + id + "' value='" + id + "_" + j + "'/> " + singleQuestionsOptionList[j] + "<br/>";
                        }
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                    });
                }

                //multiple answer question
                if ($scope.surveyInfoMultipleAnswerQuestion.data.length != 0) {
                    $.each($scope.surveyInfoMultipleAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                        renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                        renderSurveyQuestion += "<div class='box-title'>";
                        renderSurveyQuestion += "<h3>";
                        renderSurveyQuestion += "<i class='icon-file'></i>";
                        renderSurveyQuestion += "Multiple Answer Question";
                        renderSurveyQuestion += "</h3>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "<div class='box-content'>";
                        renderSurveyQuestion += "<p><b>";
                        renderSurveyQuestion += this.question;
                        renderSurveyQuestion += "</b></p>";
                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var multipleQuestionsOptionList = this.options.split(';');
                        for (var j = 0; j < multipleQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<input type='checkbox' class='userSurveyCheckBoxButton' name='" + id + "' value='" + id + "_" + j + "'/> " + multipleQuestionsOptionList[j] + "<br/>";

                        };
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                    });
                }

                //listbox answer question
                if ($scope.surveyInfoListBoxAnswerQuestion.data.length != 0) {
                    $.each($scope.surveyInfoListBoxAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                        renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                        renderSurveyQuestion += "<div class='box-title'>";
                        renderSurveyQuestion += "<h3>";
                        renderSurveyQuestion += "<i class='icon-file'></i>";
                        renderSurveyQuestion += "ListBox Answer Question";
                        renderSurveyQuestion += "</h3>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "<div class='box-content'>";
                        //renderSurveyQuestion += "<p><b>";
                        //renderSurveyQuestion += this.question;
                        //renderSurveyQuestion += "</b></p>";


                        renderSurveyQuestion += "<fieldset>";

                        renderSurveyQuestion += "<label>";
                        renderSurveyQuestion += "<b>" + this.question + "</b>";
                        renderSurveyQuestion += "</label>";

                        //var listBoxQuestionsOptionList = this.Options.split(';');
                        renderSurveyQuestion += "<select name='" + this.id + "' class='form-control userSurveyListBoxButton'>";
                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var listBoxQuestionsOptionList = this.options.split(';');
                        renderSurveyQuestion += "<option value='" + id + "_-1'>---SELECT---</option>";
                        for (var j = 0; j < listBoxQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<option value='" + id + "_" + j + "'>" + listBoxQuestionsOptionList[j] + "</option>";
                        };
                        renderSurveyQuestion += "</select>";
                        renderSurveyQuestion += "</fieldset>";

                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                    });
                }
                //textBox answer question
                var textBoxRenderSurveyQuestion = "";
                if ($scope.surveyInfoTextBoxAnswerQuestion.data.length != 0) {
                    $.each($scope.surveyInfoTextBoxAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                        renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                        renderSurveyQuestion += "<div class='box-title'>";
                        renderSurveyQuestion += "<h3>";
                        renderSurveyQuestion += "<i class='icon-file'></i>";
                        renderSurveyQuestion += "TextBox Answer Question";
                        renderSurveyQuestion += "</h3>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "<div class='box-content'>";
                        renderSurveyQuestion += "<p><b>";
                        renderSurveyQuestion += this.question;
                        renderSurveyQuestion += "</b></p>";
                        insertAttemptedSurveyQuestionsList(this.id);
                        renderSurveyQuestion += "<input type='textarea' name='" + this.id + "' class='userSurveyTextBoxButton' placeholder='Enter Your Answer'/><br/>";

                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                        renderSurveyQuestion += "</div>";
                    });
                }

                // adding button at last
                renderSurveyQuestion += "<div class='swiper-slide gray-slide box-body maxHeight800px'>";
                renderSurveyQuestion += "<div class='box box-color box-bordered blue'>";
                renderSurveyQuestion += "<div class='box-title'>";
                renderSurveyQuestion += "<h3>";
                renderSurveyQuestion += "<i class='icon-file'></i>";
                renderSurveyQuestion += "Submit Your Survey";
                renderSurveyQuestion += "</h3>";
                renderSurveyQuestion += "</div>";
                renderSurveyQuestion += "<div class='box-content'>";
                renderSurveyQuestion += "<button  class=\"btn btn-success btn-sm\" id='userSurveySubmitButtonId'>submit</button>";
                renderSurveyQuestion += "</div>";
                renderSurveyQuestion += "</div>";
                renderSurveyQuestion += "</div>";

                $('#swiperWrapperId').html(renderSurveyQuestion);
                initializeSwiperFunction();
            }
            else { // if it is web.

                renderSurveyQuestion += "<div style='' class='control-group'>";
                renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                renderSurveyQuestion += "<b>Title</b></label>";
                renderSurveyQuestion += "<div class='controls'><p><b>" + $scope.surveyInfoTitle + "</b></p></div></div>";

                // instruction list
                if ($scope.surveyInfoInstruction.data.length != 0) {
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>Instructions</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";
                    renderSurveyQuestion += "<ul>";
                    $.each($scope.surveyInfoInstruction.data, function () {
                        renderSurveyQuestion += "<li>" + this.instruction + "</li>";
                    });
                    renderSurveyQuestion += "</ul>";
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                // single answer question..
                if ($scope.surveyInfoSingleAnswerQuestion.data.length != 0) {
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>SAQ</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";

                    $.each($scope.surveyInfoSingleAnswerQuestion.data, function () {

                        renderSurveyQuestion += "<fieldset>";

                        renderSurveyQuestion += "<label>";
                        renderSurveyQuestion += "<b>" + this.question + "</b>";
                        renderSurveyQuestion += "</label>";

                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var singleQuestionsOptionList = this.options.split(';');
                        for (var j = 0; j < singleQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<div class='radio' style='padding: 0px 0px 0px 20px;'>";
                            //renderSurveyQuestion += "<label>";
                            renderSurveyQuestion += "<input type='radio' class='userSurveyRadioButton' name='" + id + "' value='" + id + "_" + j + "'/> <span>" + singleQuestionsOptionList[j] + "</span><br/>";
                            //renderSurveyQuestion += "</label>";
                            renderSurveyQuestion += "</div>";
                        };


                        renderSurveyQuestion += "</fieldset>";

                    });
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                //multiple answer question
                if ($scope.surveyInfoMultipleAnswerQuestion.data.length != 0) {
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>MAQ</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";

                    $.each($scope.surveyInfoMultipleAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<fieldset>";
                        renderSurveyQuestion += "<label>";
                        renderSurveyQuestion += "<b>" + this.question + "</b>";
                        renderSurveyQuestion += "</label>";
                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var multipleQuestionsOptionList = this.options.split(';');
                        for (var j = 0; j < multipleQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<div class='checkbox' style='padding: 0px 0px 0px 20px;'>";

                            renderSurveyQuestion += "<input type='checkbox' class='userSurveyCheckBoxButton' name='" + id + "' value='" + id + "_" + j + "'/> " + multipleQuestionsOptionList[j] + "<br/>";

                            renderSurveyQuestion += "</div>";
                        };

                        renderSurveyQuestion += "</fieldset>";

                    });
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                //listbox answer question
                if ($scope.surveyInfoListBoxAnswerQuestion.data.length != 0) {
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>LAQ</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";

                    $.each($scope.surveyInfoListBoxAnswerQuestion.data, function () {

                        renderSurveyQuestion += "<fieldset>";

                        renderSurveyQuestion += "<label>";
                        renderSurveyQuestion += "<b>" + this.question + "</b>";
                        renderSurveyQuestion += "</label>";

                        //var listBoxQuestionsOptionList = this.Options.split(';');
                        renderSurveyQuestion += "<select name='" + this.id + "' class='form-control userSurveyListBoxButton'>";
                        var id = this.id;
                        insertAttemptedSurveyQuestionsList(id);
                        var listBoxQuestionsOptionList = this.options.split(';');
                        renderSurveyQuestion += "<option value='" + id + "_-1'>---SELECT---</option>";
                        for (var j = 0; j < listBoxQuestionsOptionList.length; j++) {
                            renderSurveyQuestion += "<option value='" + id + "_" + j + "'>" + listBoxQuestionsOptionList[j] + "</option>";
                        };
                        renderSurveyQuestion += "</select>";
                        renderSurveyQuestion += "</fieldset>";

                    });
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                //textBox answer question
                var textBoxRenderSurveyQuestion = "";
                if ($scope.surveyInfoTextBoxAnswerQuestion.data.length != 0) {
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>TAQ</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";
                    $.each($scope.surveyInfoTextBoxAnswerQuestion.data, function () {
                        renderSurveyQuestion += "<fieldset>";
                        renderSurveyQuestion += "<label>";
                        renderSurveyQuestion += "<b>" + this.question; +"</b>";
                        renderSurveyQuestion += "</label><br/>";
                        insertAttemptedSurveyQuestionsList(this.id);
                        if ($scope.type == TemplateInfoModel.type_contentWritting) {
                            renderSurveyQuestion += "<textarea style='height: 150px; width: 100%' type='textarea' class='userSurveyTextBoxButton textarea' name='" + this.id + "' placeholder='Enter Your Answer'/><br/>";
                        }
                        else {
                            renderSurveyQuestion += "<input type='text' class='userSurveyTextBoxButton' name='" + this.id + "' placeholder='Enter Your Answer'/><br/>";
                        }
                        

                        renderSurveyQuestion += "</fieldset>";

                    });
                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }

                if (!isDemo) {
                    // adding button at last
                    renderSurveyQuestion += "<div style='' class='control-group'>";
                    renderSurveyQuestion += "<label for=\"textfield\" class=\"control-label\">";
                    renderSurveyQuestion += "<b>Submit</b></label>";
                    renderSurveyQuestion += "<div class='controls'>";

                    renderSurveyQuestion += "<fieldset>";
                    renderSurveyQuestion += "<label>";

                    renderSurveyQuestion += "</label><br/>";

                    renderSurveyQuestion += "<button  class=\"btn btn-success btn-sm\" id='userSurveySubmitButtonId'>submit</button>";

                    renderSurveyQuestion += "</fieldset>";

                    renderSurveyQuestion += "</div>";
                    renderSurveyQuestion += "</div>";
                }
                

                $('#userSurveyWebViewId').html(renderSurveyQuestion);

                $('.textarea').wysihtml5({
                    "font-styles": true, //Font styling, e.g. h1, h2, etc. Default true
                    "emphasis": true, //Italics, bold, etc. Default true
                    "lists": true, //(Un)ordered lists, e.g. Bullets, Numbers. Default true
                    "html": false, //Button which allows you to edit the generated HTML. Default false
                    "link": true, //Button to insert a link. Default true
                    "image": false, //Button to insert an image. Default true,
                    "color": false //Button to change color of font  
                });

                initializeSwiperFunction();
            }
        }


        $scope.newValue = function (value) {
            console.log(value);
        }

        $scope.showCurrentData = function () {
            console.log(surveyInfoSingleAnswerQuestion.data);
        }

        function initializeSwiperFunction() {
            if (mobileDevice) {
                var mySwiper = new Swiper('.swiper-container', {
                    pagination: '.pagination',
                    paginationClickable: true
                });
                reinitSwiper(mySwiper);
            }

            //            $("input[type='checkbox'], input[type='radio']").iCheck({
            //                checkboxClass: 'icheckbox_minimal',
            //                radioClass: 'iradio_minimal'
            //            });

            $('.fancybox').fancybox();


            //radiobutton
            $('.userSurveyRadioButton').on('change', function () {
                //var a = "change " + $(this).val();
                //console.log(a + "---" + this.value);
                var data = this.value.split('_');
                $scope.userSurveyResult.surveySingleAnswerQuestion.push(commonUserSurveyRadioButtonFunction(data));
                markAttemptedSurveyQuestionsList(data[0]);
            });

            $('.userSurveyRadioButton').on('ifChecked', function (event) {
                //var a = "ifChecked "+ $(this).val();
                //console.log(a + "---" + this.value);
                var data = this.value.split('_');
                $scope.userSurveyResult.surveySingleAnswerQuestion.push(commonUserSurveyRadioButtonFunction(data));
                markAttemptedSurveyQuestionsList(data[0]);
            });

            function commonUserSurveyRadioButtonFunction(data) {
                var radioButtonAnswer = { key: data[0], value: data[1] };

                var i, flag = false;
                for (i = 0; i < $scope.userSurveyResult.surveySingleAnswerQuestion.length; i++) {
                    if ($scope.userSurveyResult.surveySingleAnswerQuestion[i].key == data[0]) {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    $scope.userSurveyResult.surveySingleAnswerQuestion.splice(i, 1);

                return radioButtonAnswer;
            }

            //checkbox
            $('.userSurveyCheckBoxButton').on('change', function () {
                var a;
                var data = this.value.split('_');
                var checked = true;
                var i;
                if ($(this).is(':checked')) {
                    checked = true;
                }
                else {
                    checked = false;
                }

                commonUserSurveyCheckBoxButtonFunction(data, checked);
                //console.log($scope.userSurveyResult.surveyMultipleAnswerQuestion);

            });

            $('.userSurveyCheckBoxButton').on('ifChecked', function (event) {
                var data = this.value.split('_');
                commonUserSurveyCheckBoxButtonFunction(data, true);
                //console.log($scope.userSurveyResult.surveyMultipleAnswerQuestion);
            });
            $('.userSurveyCheckBoxButton').on('ifUnchecked', function (event) {
                var data = this.value.split('_');
                commonUserSurveyCheckBoxButtonFunction(data, false);
                //console.log($scope.userSurveyResult.surveyMultipleAnswerQuestion);
            });

            function commonUserSurveyCheckBoxButtonFunction(data, checked) {
                var i, flag = false;
                for (i = 0; i < $scope.userSurveyResult.surveyMultipleAnswerQuestion.length; i++) {
                    if ($scope.userSurveyResult.surveyMultipleAnswerQuestion[i].key == data[0]) {
                        flag = true;
                        break;
                    }
                }

                if (flag) {
                    if (checked) {
                        $scope.userSurveyResult.surveyMultipleAnswerQuestion[i].value += data[1] + ';';
                        markAttemptedSurveyQuestionsList(data[0]);
                    } else {
                        $scope.userSurveyResult.surveyMultipleAnswerQuestion[i].value = $scope.userSurveyResult.surveyMultipleAnswerQuestion[i].value.replace(data[1] + ';', "");
                        if ($scope.userSurveyResult.surveyMultipleAnswerQuestion[i].value == "" || $scope.userSurveyResult.surveyMultipleAnswerQuestion[i].value == null) {
                            unmarkAttemptedSurveyQuestionsList(data[0]);
                        }
                    }

                }
                else {
                    var checkBoxButtonAnswer = { key: data[0], value: data[1] + ';' };
                    $scope.userSurveyResult.surveyMultipleAnswerQuestion.push(checkBoxButtonAnswer);
                    markAttemptedSurveyQuestionsList(data[0]);
                }
            }

            //checkbox
            $('.userSurveyListBoxButton').on('change', function () {
                var a;
                var data = this.value.split('_');

                $scope.userSurveyResult.surveyListBoxAnswerQuestion.push(commonUserSurveyListBoxButtonFunction(data));
                //commonUserSurveyCheckBoxButtonFunction(data, checked);
                //console.log($scope.userSurveyResult.surveyListBoxAnswerQuestion);

            });

            function commonUserSurveyListBoxButtonFunction(data) {
                var radioButtonAnswer = { key: data[0], value: data[1] };

                var i, flag = false;
                for (i = 0; i < $scope.userSurveyResult.surveyListBoxAnswerQuestion.length; i++) {
                    if ($scope.userSurveyResult.surveyListBoxAnswerQuestion[i].key == data[0]) {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    $scope.userSurveyResult.surveyListBoxAnswerQuestion.splice(i, 1);

                if (data[1] != '-1')
                    markAttemptedSurveyQuestionsList(data[0]);
                else
                    unmarkAttemptedSurveyQuestionsList(data[0]);
                return radioButtonAnswer;
            }

            $('.userSurveyTextBoxButton').bind('input propertychange', function () {
                var data = this.name.split('_');
                $scope.userSurveyResult.surveyTextBoxAnswerQuestion.push(commonUserSurveyTextBoxFunction(this.name, $(this).val()));
                //console.log($scope.userSurveyResult.surveyTextBoxAnswerQuestion);
                //console.log(this.name + " --- " + $(this).val());
            });

            function commonUserSurveyTextBoxFunction(key, value) {
                var textBoxAnswer = { key: key, value: value };

                var i, flag = false;
                for (i = 0; i < $scope.userSurveyResult.surveyTextBoxAnswerQuestion.length; i++) {
                    if ($scope.userSurveyResult.surveyTextBoxAnswerQuestion[i].key == key) {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    $scope.userSurveyResult.surveyTextBoxAnswerQuestion.splice(i, 1);
                if (value != "")
                    markAttemptedSurveyQuestionsList(key);
                else
                    unmarkAttemptedSurveyQuestionsList(key);
                return textBoxAnswer;
            }

            //submit button
            $('#userSurveySubmitButtonId').on('click', function () {
                var isValidInput = checkEveryQuestionsAttempted();
                if ($scope.type == TemplateInfoModel.type_contentWritting) {
                    if (confirm("You won't be able to edit after submitted. Are you sure you want to submit?") == true) {
                        $scope.userSurveyResult.surveyTextBoxAnswerQuestion.push(commonUserSurveyTextBoxFunction($scope.surveyInfoTextBoxAnswerQuestion.data[0].id, $('.userSurveyTextBoxButton').val()));
                        isValidInput = true;
                    }
                    else {
                        isValidInput = false;
                    }
                }
                if (isValidInput) {
                    var url = ServerContextPath.empty + '/User/SubmitTemplateSurveyResultByRefKey?refKey=' + $routeParams.refKey;
                    var userSurveyResultData = $scope.userSurveyResult;
                    var headers = {
                        'Content-Type': 'application/json',
                        'UTMZT': CookieUtil.getUTMZT(),
                        'UTMZK': CookieUtil.getUTMZK(),
                        'UTMZV': CookieUtil.getUTMZV()
                    };
                    startBlockUI('wait..', 3);
                    $http({
                        url: url,
                        method: "POST",
                        data: userSurveyResultData,
                        headers: headers
                    }).success(function (data, status, headers, config) {
                        //$scope.persons = data; // assign  $scope.persons here as promise is resolved here
                        stopBlockUI();
                        if (data.Status == "200") {
                            showToastMessage("Success", "Survey Successfully submitted");
                            location.href = "#/";
                        }

                    }).error(function (data, status, headers, config) {

                    });
                } else {
                    if ($scope.type != TemplateInfoModel.type_contentWritting) {
                        showToastMessage("Warning", "Attempt All Questions Before Submitting.");
                    }

                }

            });
        }
    });

});

function splitOptionsToList(data) {
    return data;// currently login is changed.
    //return data.split(';');
}

function reinitSwiper(swiper) {
    setTimeout(function () {
        swiper.reInit();
    }, 500);
}

