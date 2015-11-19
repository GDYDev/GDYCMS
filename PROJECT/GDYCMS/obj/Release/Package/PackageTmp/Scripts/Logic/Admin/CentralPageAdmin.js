angular.module('LinkBuffer', ['ngCookies']).service('LinkBufferModel', function ($cookies) {
    var _this = this;

    var LinkType = function (LinkName, Address) {        
        this.LinkName = LinkName;
        this.Address = Address;
        return this;
    }

    var SaveBufferToCookie = function () {
        var tmp=JSON.stringify(_this.ObjectList);
        $cookies.put('GDYSimpleCMS_ADMIN_LINKS_BUFFER', tmp)
    }
    this.ObjectList = [];
    
    this.SelectedLink = null;
    this.DeleteSelected = function () {
        _this.ObjectList.forEach(function (item,idx,arr) {
            if (item.Address == _this.SelectedLink) {
                _this.ObjectList.splice(idx, 1);
            }
        });
        SaveBufferToCookie();
    }
    this.AddLink = function (LinkName, Address) {
        _this.ObjectList.push(new LinkType(LinkName, Address));
        SaveBufferToCookie();
    }
    this.ClearAllLinks = function () {
        _this.ObjectList = [];
        SaveBufferToCookie();
    }

    function directCopy(str) {
        document.oncopy = function (event) {
            event.clipboardData.setData("Text", str);
            event.preventDefault();
        };
        document.execCommand("Copy");
        document.oncopy = undefined;
    }

    this.CopyToClipBoard = function () {
        directCopy(_this.SelectedLink[0]);
    }
    
    var tmp = $cookies.get('GDYSimpleCMS_ADMIN_LINKS_BUFFER');
    if (tmp != null) {
        tmp=JSON.parse(tmp);
        _this.ObjectList = tmp;
    }

});

angular.module('GDYApp', ['ui.tinymce','LinkBuffer']);

angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http, LinkBufferModel) {
    
    $scope.LinkBufferModel = LinkBufferModel;
    $scope.CurrentMaterialHTML =document.getElementById('HTML').value;
    
    $scope.tinymceOptions = {
        //encoding:"xml",
        resize: false,
        //width: 100%,  // I *think* its a number and not '400' string
        height: 300,
        plugins: 'advlist anchor autolink code colorpicker image imagetools link table textcolor',
        toolbar: "undo redo styleselect bold italic print forecolor backcolor"

    };

    $scope.to_trusted = function (html_code) {
        return $sce.trustAsHtml(html_code);
    }
    $scope.OpenLinkBuffer = function () {
        $('#LinkBufferModal').modal('show');
    }
    $scope.Test = "Hello. It's test";
});

