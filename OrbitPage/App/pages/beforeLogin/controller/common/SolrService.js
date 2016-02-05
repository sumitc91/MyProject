'use strict';
define([appLocation.preLogin], function (app) {
    /*app.factory('SolrServiceUtiltest', function ($resource,$rootScope, $routeParams) {

        return {
            getCompetitors: function () {

                $resource('http://localhost:28308/search/GetCompanyCompetitorsDetail?size=10001&rating=0&speciality=Management%20Consulting,Systems%20Integration%20and%20Technology,Business%20Process%20Outsourcing,Application%20and%20Infrastructure%20Outsourcing', { user: "user" });
            }            
        };
        {size:'10001',rating:'0',speciality:'Management%20Consulting,Systems%20Integration%20and%20Technology,Business%20Process%20Outsourcing,Application%20and%20Infrastructure%20Outsourcing'}
    });*/

    app.factory('SolrServiceUtil', function ($resource) {
        return $resource('http://www.orbitpage.com/searchapi/Search/GetCompanyCompetitorsDetail?size=:size&rating=:rating&speciality=:speciality', { id: '@id' }, {
            query: {
                isArray: false,
                method: 'GET'
            }
        });
    });
});