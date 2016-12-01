//(function () {
//   'use strict';

angular.module('emailapp', ['ui.bootstrap', 'angular-loading-bar', 'growlNotifications'])
    .controller('MainController', ['$scope', '$http', main])
    .directive('multipleEmails', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    if (!viewValue || viewValue.trim().length == 0) {
                        ctrl.$setValidity('multipleEmails', true);
                        return viewValue;
                    }
                    var emails = viewValue.split(',');
                    // define single email validator here
                    var re = /\S+@\S+\.\S+/;

                    // angular.foreach(emails, function() {
                    var validityArr = emails.map(function (str) {
                        return re.test(str.trim());
                    }); // sample return is [true, true, true, false, false, false]
                    //console.log(emails, validityArr); 
                    var atLeastOneInvalid = false;
                    angular.forEach(validityArr, function (value) {
                        if (value === false)
                            atLeastOneInvalid = true;
                    });
                    if (!atLeastOneInvalid) {
                        // ^ all I need is to call the angular email checker here, I think.
                        ctrl.$setValidity('multipleEmails', true);
                        return viewValue;
                    } else {
                        ctrl.$setValidity('multipleEmails', false);
                        return undefined;
                    }
                    // })
                });
            }
        };
    });

function main($scope, $http) {
    var self = this;
    self.isSending = false;

    self.sendEmail = function () {
        $scope.addNotification("Contacting mail server...");
        self.isSending = true;
        $http.post("api/email", self).then(function (response) {
            console.log(response);
            self.response = response.data;
            $scope.addNotification(response.data);
            self.isSending = false;
        });
    };

    self.noRecipients = function () {
        return ((!self.tos || self.tos.length == 0) && (!self.ccs || self.ccs.length == 0) && (!self.bccs || self.bccs.length == 0));
    };

    self.hasErrors = function () {
        var errorsExists = angular.element(document.querySelector('form .has-error')).length > 0;
        console.log("hasErrors=" + errorsExists);
        return errorsExists;
    };

    $scope.notifications = [];

    $scope.addNotification = function (notification) {
        $scope.invalidNotification = false;
        $scope.notifications.push(notification);        
    };
}
//})();