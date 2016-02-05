'use strict';
define([appLocation.preLogin], function (app) {    
    app.factory('AuthCreateAccountService', function ($resource) {
        return $resource('http://orbitpage.com/authapi/Auth/CreateAccount'); // Note the full endpoint address
    });
});