angular.module('GDYApp', ['ui.tinymce']);
angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http) {
    $scope.tinymceOptions = {
        //encoding:"xml",
        resize: false,
        //width: 100%,  // I *think* its a number and not '400' string
        height: 599,
        plugins: 'advlist anchor autolink code colorpicker image imagetools link table textcolor',
        toolbar: "undo redo styleselect bold italic print forecolor backcolor"
    };

    $scope.to_trusted = function (html_code) {
        return $sce.trustAsHtml(html_code);
    }
});
$(document).ready(function () {
    $('#MaterialGroupList_Keys').find("option").attr("selected", false);
    $('#MaterialList_Keys').find("option").attr("selected", false);

    $('#MaterialGroupList_Keys').click(function () {
        $('#SelectedMaterialGroup').val(SelectedMaterialGroup);
        $('#SubmitID').trigger('click');
    });

    $('#MaterialList_Keys').click(function () {
        var SelectedMaterialGroup = $('#MaterialGroupList_Keys').find(":selected").text();
        var SelectedMaterial = $('#MaterialList_Keys').find(":selected").text();
        $('#SelectedMaterial').val(SelectedMaterial);
        $('#SelectedMaterialGroup').val(SelectedMaterialGroup);
        $('#SubmitID').trigger('click');
    });
});