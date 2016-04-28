/**
 * Simple bootstrapper to load all the pre-requisite AngularJS dependencies needed by Login SPA [Single Page Application]
 * @class PreLoginInit
 * @module PreLogin
 */
define(['angular'], function() {

    var dependances = ['restangular', 'ngtimeago','infinite-scroll', 'angularFileUpload', 'ngResource', 'ngRoute', 'ngSanitize', 'ngAutocomplete', 'angucomplete-alt', 'angular-input-stars', 'ui.bootstrap'];
    var app = angular.module("beforeLoginApp", dependances);
    return app;
});
