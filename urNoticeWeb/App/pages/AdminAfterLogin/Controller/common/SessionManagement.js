'use strict';
define([appLocation.adminPostLogin], function (app) {
    app.factory('SessionManagementUtil', function ($rootScope, $location, $http, $routeParams) {

        return {
            isValidSession: function (headerSessionData) {
                $http({
                    url: ServerContextPath.empty + '/Auth/IsValidSession',
                    method: "POST",
                    data: headerSessionData,
                    headers: { 'Content-Type': 'application/json' }
                }).success(function (data, status, headers, config) {
                    //$scope.persons = data; // assign  $scope.persons here as promise is resolved here                
                    if (data.Status == "200") {
                        if (data.Payload == "true" || data.Payload == "True") {
                            return true;
                        }
                        else
                            return false;
                    }

                }).error(function (data, status, headers, config) {
                    return false;
                });
            }
        };

    });

});

