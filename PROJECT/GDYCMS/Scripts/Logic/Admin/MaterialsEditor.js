angular.module('LinkBuffer', ['ngCookies']).service('LinkBufferModel', function ($cookies) {
    var _this = this;

    var LinkType = function (LinkName, Address) {
        this.LinkName = LinkName;
        this.Address = Address;
        return this;
    }

    var SaveBufferToCookie = function () {
        var tmp = JSON.stringify(_this.ObjectList);
        $cookies.put('GDYSimpleCMS_ADMIN_LINKS_BUFFER', tmp)
    }
    this.ObjectList = [];

    this.SelectedLink = null;
    this.DeleteSelected = function () {
        _this.ObjectList.forEach(function (item, idx, arr) {
            if (item.Address == _this.SelectedLink) {
                _this.ObjectList.splice(idx, 1);
            }
        });
        SaveBufferToCookie();
        _this.Load();
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
    this.SaveBuffer = function () {
        SaveBufferToCookie();
    }

    this.Load = function () {
        var tmp = $cookies.get('GDYSimpleCMS_ADMIN_LINKS_BUFFER');
        if (tmp != null) {
            tmp = JSON.parse(tmp);
            _this.ObjectList = tmp;
        }
    }
    this.Load();    
});




function ProcessAJAXRequest(URL, RequestData) {
    var ret = 'no data';
    $.ajax({
        url: URL,
        async: false,
        type: "POST",
        data: JSON.stringify(RequestData),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR + "-" + textStatus + "-" + errorThrown);
        },
        success: function (data, textStatus, jqXHR) {
            ret = data;
        }
    });
    return ret;
}

function EditorModel() {

    this.ServerPath = null;
    this.NewMaterialName = '';
    this.NewMaterialGroupName = '';
    this.UserName = '';
    this.LastChange = '';
	this.AnswerStatus='Error';

    this.SelectedMaterialGroup = '';
    this.SelectedMaterial = '';
    this.SelectedMaterialID = null;
    this.MaterialGroupList = [];
    this.MaterialList = [];
    this.MaterialGroupName = '';
    this.MaterialName = '';
    this.HTML = '';
    this.FirstCreator = '';
    this.LastModifier = '';
    this.cmd = '';
    this.isPublished = false;
    return this;
}

angular.module('GDYApp', ['ui.tinymce','LinkBuffer']);
angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http,LinkBufferModel) {
	$scope.LinkBuffer = LinkBufferModel;
	
    $scope.tinymceOptions = {
        //encoding:"xml",
        resize: false,
        //width: 100%,  // I *think* its a number and not '400' string
        height: 730,
        plugins: 'advlist anchor autolink code colorpicker image imagetools link table textcolor',
        toolbar: "undo redo styleselect bold italic print forecolor backcolor"
    };

    $scope.to_trusted = function (html_code) {
        return $sce.trustAsHtml(html_code);
    }
    var _this = this;
    $scope.CurrentEditor = new EditorModel();;
    $scope.ErrorMessage = '';
    $scope.LoadMaterialGroups = function () {
        $scope.CurrentEditor.cmd = 'GetMaterialGroups'
        var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
        $scope.CurrentEditor.MaterialGroupList = Returned.MaterialGroupList;
        $scope.CurrentEditor.SelectedMaterial = '';
    }

    $scope.LoadMaterialsForSelectedGroup = function (SelectedGroup) {
        if (SelectedGroup != null) {
            $scope.CurrentEditor.SelectedMaterialGroup = SelectedGroup;
        }
        $scope.CurrentEditor.cmd = "LoadMaterialListForSelectedGroup";
        var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
        $scope.CurrentEditor.MaterialList = Returned.MaterialList;
        $scope.CurrentEditor.SelectedMaterial = '';
    }

    $scope.LoadMaterial = function (MaterialName) {
        $scope.CurrentEditor.SelectedMaterial = MaterialName;
        $scope.CurrentEditor.cmd = "LoadSelectedMaterial";
        var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
        $scope.CurrentEditor.HTML = Returned.HTML;
        $scope.CurrentEditor.SelectedMaterialID = Returned.SelectedMaterialID;
        $scope.CurrentEditor.isPublished = Returned.isPublished;
        $scope.CurrentEditor.LastChange = Returned.LastChange;
        $scope.CurrentEditor.LastModifier = Returned.LastModifier;
        $scope.CurrentEditor.FirstCreator = Returned.FirstCreator
    }

    $scope.TryToAddMaterialGroup = function () {
        var isSameMaterialGroupFound = false;
        $scope.CurrentEditor.MaterialGroupList.forEach(function (item, idx, arr) {
            if (item.toLowerCase() == $scope.CurrentEditor.NewMaterialGroupName.toLowerCase()) {
                isSameMaterialGroupFound = true;
            }
        });
        if (isSameMaterialGroupFound == true) {
            $scope.ErrorMessage = 'Sorry, but same material group already exists';
            $('#ErrorMessage').modal('show');
        }
        else {
            $scope.CurrentEditor.cmd = 'AddNewMaterialGroup';
            var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
            $scope.LoadMaterialGroups();
        }
    }

    $scope.TryToRenameMaterialGroup = function () {
        var isSameMaterialGroupFound = false
        $scope.CurrentEditor.MaterialGroupList.forEach(function (item, idx, arr) {
            if (item.toLowerCase() == $scope.CurrentEditor.NewMaterialGroupName.toLowerCase()) {
                isSameMaterialGroupFound = true;
            }
        });
        if (isSameMaterialGroupFound == true) {
            $scope.ErrorMessage = 'Sorry, but same material group already exists';
            $('#ErrorMessage').modal('show');
        }
        else {
            if ($scope.CurrentEditor.NewMaterialGroupName == '') {
                $scope.ErrorMessage = 'Sorry, but new name is empty';
                $('#ErrorMessage').modal('show');
            }
            else {
                $scope.CurrentEditor.cmd = 'RenameMaterialGroup';
                var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
                $scope.LoadMaterialGroups();
            };
        }
    }

    $scope.TryToDeleteMaterialGroup = function () {
        $scope.CurrentEditor.cmd = 'DeleteMaterialGroup';
        var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
        $scope.LoadMaterialGroups();
    }

    $scope.TryToAddMaterial = function () {
        var isSameMaterialFound = false;
        if ($scope.CurrentEditor.NewMaterialName != '') {
            $scope.CurrentEditor.MaterialList.forEach(function (item, idx, arr) {
                if (item.toLowerCase() == $scope.CurrentEditor.NewMaterialName.toLowerCase()) {
                    isSameMaterialFound = true;
                }
            });
            if (isSameMaterialFound == true) {
                $scope.ErrorMessage = 'Sorry, but material with same name found';
                $('#ErrorMessage').modal('show');
            }
            else {
                $scope.CurrentEditor.cmd = 'AddNewMaterial';
                var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
                $scope.LoadMaterialsForSelectedGroup();
            }
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material name is empty';
            $('#ErrorMessage').modal('show');
        }
    };


    $scope.TryToRenameMaterial = function () {
        var isSameMaterialFound = false;
        if ($scope.CurrentEditor.NewMaterialName != '') {
            $scope.CurrentEditor.MaterialList.forEach(function (item, idx, arr) {
                if (item.toLowerCase() == $scope.CurrentEditor.NewMaterialName.toLowerCase()) {
                    isSameMaterialFound = true;
                }
            });
            if (isSameMaterialFound == true) {
                $scope.ErrorMessage = 'Sorry, but material with same name already exists';
                $('#ErrorMessage').modal('show');
            }
            else {
                $scope.CurrentEditor.cmd = 'RenameMaterial';
                var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
                $scope.LoadMaterialsForSelectedGroup();
            }
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material name is empty';
            $('#ErrorMessage').modal('show');
        }
    }

    $scope.TryToDeleteMaterial = function () {
        $scope.CurrentEditor.cmd = 'DeleteMaterial';
        var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);
        $scope.LoadMaterialsForSelectedGroup();
    }


    $scope.DeleteMaterial = function () {
        if ($scope.CurrentEditor.SelectedMaterial != '') {
            $('#DeleteMaterial').modal('show');
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material is not selected';
            $('#ErrorMessage').modal('show');
        }
    }

    $scope.RenameMaterial = function () {
        if ($scope.CurrentEditor.SelectedMaterial != '') {
            $('#RenameMaterial').modal('show');
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material is not selected';
            $('#ErrorMessage').modal('show');
        }
    }

    $scope.AddNewMaterial = function () {
        if ($scope.CurrentEditor.SelectedMaterialGroup != '') {
            $('#AddNewMaterial').modal('show');
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material group is not selected';
            $('#ErrorMessage').modal('show');
        }
    }



    $scope.DeleteMaterialGroup = function () {
        if ($scope.CurrentEditor.SelectedMaterialGroup != '') {
            $('#DeleteMaterialGroup').modal('show');
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material group is not selected';
            $('#ErrorMessage').modal('show');
        }
    }


    $scope.AddNewMaterialGroup = function () {
        $('#AddNewMaterialGroup').modal('show');
    }

    $scope.RenameMaterialGroup = function () {
        if ($scope.CurrentEditor.SelectedMaterialGroup != '') {
            $('#RenameMaterialGroup').modal('show');
        }
        else {
            $scope.ErrorMessage = 'Sorry, but material group is not selected';
            $('#ErrorMessage').modal('show');
        }
    }
	
    $scope.UpdateMaterial = function () { // Save material changes
        alert('Update process');
		if ($scope.CurrentEditor.SelectedMaterial!=''){ 
			$scope.CurrentEditor.cmd = 'UpdateMaterial';
            var Returned = ProcessAJAXRequest("/Admin/MaterialsEditorJSON", $scope.CurrentEditor);	
			if (Returned.ActionResult==true){
				$('#MaterialChangesSaved').modal('show');
			}
		}
		else {
			$scope.ErrorMessage = 'Sorry, but material is not selected';
            $('#ErrorMessage').modal('show');			
		}	
		
	}
	
	$scope.LinkName='LINK NAME'; // Material name that would be copied to link buffer
	
	$scope.OpenLinkBuffer=function(){ // Open link buffer
		$('#LinkBufferModal').modal('show');
	}
	
	$scope.TryToAddThisMaterialLinkToBuffer=function(){ // Add current material to buffer
		if ($scope.LinkName!='') {
			var link='/Home/Index/'+$scope.CurrentEditor.SelectedMaterialID;
			$scope.LinkBuffer.AddLink($scope.LinkName,link);
			$scope.LinkBuffer.Load();
		}
		else {
			$scope.ErrorMessage = 'Sorry, but link name is empty';
            $('#ErrorMessage').modal('show');					
		}
	}
	
	$scope.OpenAddLinkWindow=function(){ 
		if ($scope.CurrentEditor.SelectedMaterial!=''){ 
			$('#MaterialLinkToBuffer').modal('show');			
		}
		else {
			$scope.ErrorMessage = 'Sorry, but material is not selected';
            $('#ErrorMessage').modal('show');			
		}
	}

    $scope.GetServerPath = function () {

    }
    $scope.LoadMaterialGroups();
});

angular.module('GDYApp').filter("mydate", function () {
    var re = /\\\/Date\(([0-9]*)\)\\\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});


$(document).ready(function () {

});
