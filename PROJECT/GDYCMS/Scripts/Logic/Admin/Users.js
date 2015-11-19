angular.module('GDYApp', []);
angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http) {
    $(document).ready(function () {
        alert('Ready');
    });
    $scope.SelectedUser = null;
    $scope.SelectUser = function (SelectedOptionElement) {
        $scope.SelectedUser = SelectedOptionElement.currentTarget.value
    }
    $scope.ErrorMessage = '';

    $scope.ShowDeleteUserWindow = function () {
        if ($scope.SelectedUser == null) {            
            $scope.ErrorMessage = 'User is not selected';
            $('#ErrorModal').modal('show');
        }
        else {
            $('#UserName').val($scope.SelectedUser);
            $('#cmd').val('Delete');
            $('#DeleteUser').modal('show');
        }
    }

    $scope.ShowChangePasswordWindow = function () {
        if ($scope.SelectedUser == null) {
            $scope.ErrorMessage = 'User is not selected';
            $('#ErrorModal').modal('show');
        }
        else {
            $('#UserName').val($scope.SelectedUser);
            $('#cmd').val('ChangePassword');
            $('#ChangePassword').modal('show');
        }
    }

    $scope.ShowNewUserCreationWindow = function () {
        $('#cmd').val('Add');
        $('#NewUser').modal('show');
    }

});



