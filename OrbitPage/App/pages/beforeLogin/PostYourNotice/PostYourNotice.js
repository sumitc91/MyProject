'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginPostYourNotice', function ($scope,$location, $http, $rootScope, CookieUtil) {
        $('title').html("index-1"); //TODO: change the title so cann't be tracked in log


        $scope.postYourNoticeFormData = {
            constants: {
                employmentStatusSelectList: [
                    { id: 'REGULAR', name: 'Full-time' },
                    { id: 'PART_TIME', name: 'Part-time' },
                    { id: 'CONTRACT', name: 'Contract' },
                    { id: 'INTERN', name: 'Intern' },
                    { id: 'FREELANCE', name: 'Freelance' }
                ],
                currencySelectList: [
                    { id: 'INR', name: 'Indian Rupees' },
                    { id: 'USD',name: 'US Dollar' }                    
                ],
            },
            
            companyReview: {
                employerStatus: 'current',
                lastYearAtEmployer: '',
                employmentStatusSelect: 'REGULAR',
                reviewTitle: '',
                reviewDescription:''
            },
            companySalary: {
                amount: '',
                currency: 'INR'
            }

        };

        $scope.selectCompany = function (selected) {
            console.log(selected);
            $scope.postYourNoticeFormData.companyReview.employerName = selected.originalObject.companyname;
            $scope.postYourNoticeFormData.companyReview.employerVertexId = selected.originalObject.guid;
            //location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
        };

        $scope.selectedProject = function (selected) {
            console.log(selected);
            //location.href = "/#companydetails/" + selected.originalObject.companyname.replace(/ /g, "_").replace(/\//g, "_OR_") + "/" + selected.originalObject.guid;
        };

    });

});

