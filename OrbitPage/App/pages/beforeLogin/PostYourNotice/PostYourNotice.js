'use strict';
define([appLocation.preLogin], function (app) {

    app.controller('beforeLoginPostYourNotice', function ($scope,$location, $http, $rootScope, CookieUtil) {
        $('title').html("index-1"); //TODO: change the title so cann't be tracked in log

        //Range slider config
        $scope.minRangeSlider = {
            minValue: 0,
            maxValue: 0,
            options: {
                floor: 0,
                ceil: 100,
                step: 1
            }
        };

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
                companyGoodPointList: [
                    { isSelected: false, id: 'APPRECIATED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Appreciated.png' },
                    { isSelected: false, id: 'CHALLENGED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Challenged.png' },
                    { isSelected: false, id: 'EMPOWERED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Empowered.png' },
                    { isSelected: false, id: 'INVOLVED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Involved.png' },
                    { isSelected: false, id: 'MENTORED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Mentored.png' },
                    { isSelected: false, id: 'ONAMISSION', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_OnAMission.png' },
                    { isSelected: false, id: 'PROMOTED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Promoted.png' },
                    { isSelected: false, id: 'TRUSTED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Trusted.png' },
                    { isSelected: false, id: 'VALUED', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_Valued.png' },
                    { isSelected: false, id: 'PAIDWELL', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_stay_paid_well.png' }
                ],
                companyBadPointList: [
                    { isSelected: false, id: 'BETTERCOMPANY', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Better_Company.jpg' },
                    { isSelected: false, id: 'BURNOUT', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Burnout.jpg' },
                    { isSelected: false, id: 'DESIGNATIONJUMP', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Designation_Jump.jpg' },
                    { isSelected: false, id: 'ENVIRONMENT', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Environment.jpg' },
                    { isSelected: false, id: 'FAMILY', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Family.jpg' },
                    { isSelected: false, id: 'GROWTH', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Growth.jpg' },
                    { isSelected: false, id: 'MARRIAGE', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Marriage.jpg' },
                    { isSelected: false, id: 'WRONGPERCEPTION', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_Wrong_Perception.jpg' },
                    { isSelected: false, id: 'COMPENSATION', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave__Compensation.jpg' },
                    { isSelected: false, id: 'IMMEDIATEBOSS', img: 'https://s3-ap-southeast-1.amazonaws.com/urnotice/OrbitPage/images/reason_to_leave_immediate_boss.jpg' }
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
        $scope.postYourNoticeFormData.companyReview.toggleCompanyGoodPoint = function(index) {
            console.log($scope.postYourNoticeFormData.constants.companyGoodPointList[index].isSelected);
            $scope.postYourNoticeFormData.constants.companyGoodPointList[index].isSelected = $scope.postYourNoticeFormData.constants.companyGoodPointList[index].isSelected?false:true;
            console.log($scope.postYourNoticeFormData.constants.companyGoodPointList[index].isSelected);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
            }
        };

        $scope.postYourNoticeFormData.companyReview.toggleCompanyBadPoint = function (index) {
            console.log($scope.postYourNoticeFormData.constants.companyBadPointList[index].isSelected);
            $scope.postYourNoticeFormData.constants.companyBadPointList[index].isSelected = $scope.postYourNoticeFormData.constants.companyBadPointList[index].isSelected ? false : true;
            console.log($scope.postYourNoticeFormData.constants.companyBadPointList[index].isSelected);
            if ($scope.$root.$$phase != '$apply' && $scope.$root.$$phase != '$digest') {
                $scope.$apply();
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

