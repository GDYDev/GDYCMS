angular.module('GDYApp', []);
angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http) {
    $scope.isResulted = false;
    $(document).ready(function () {
        $scope.isResulted = $("#isResulted").val();
        if ($scope.isResulted == "True") {
            $('#RedirectModal').modal('show');
        }
    });

    $scope.RedirectToAdmin = function () {
        window.location = '/Admin/Index';
    }
    
});