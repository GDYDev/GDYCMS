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

var SoftByTypeAndName = function (a, b) {
    if ((a.Type == "file") && (b.Type == "folder")) {
        return 1;
    }
    else {
        if ((a.Type == "folder") && (b.Type == "file")) {
            return -1;
        }
        else {
            var A = a.Name.toLowerCase();
            var B = b.Name.toLowerCase();
            if (A < B) {
                return -1;
            } else if (A > B) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}

    function ItemType(Name, Type, isChecked) {
        this.Name = Name;
        this.Type = Type;
        this.isChecked = isChecked;
    }


    function FileManagerInstance() {
        this.LeftPanelList=[];
        this.RightPanelList=[];
        this.LeftPanelAbsolutePath = "";
        this.LeftPanelHttpPath = "";
        this.RightPanelAbsolutePath = "";
        this.RightPanelHttpPath=""
        this.SelectedObject=null;
        this.NewObjectName = "";
        this.NewDirectoryName = "NewFolder";
        this.ActivePanel = "left";
        this.CurrentAction = null;        
    }


    angular.module('FileManager', ['ngCookies']).service('FileManagerInstance', function ($cookies) {

        var SaveActualPathToCookie = function (ActualPath) {
            var tmp = JSON.stringify(ActualPath);
            $cookies.put('GDYSimpleCMS_LAST_UPLOAD_PATH', tmp)
        }
        
        var _this = this;
        this.Instance = new FileManagerInstance();

        this.LeftPanelStack = [];
        this.RightPanelStack = [];
        this.LeftPanelStack.push('~/Content/Upload'); // Directory stack
        this.RightPanelStack.push('~/Content/Upload'); // Directory stack
        
        this.ReflectDirectoryStack = function () {
            var leftpath = "";
            _this.LeftPanelStack.forEach(function (item, idx, arr) {
                if (idx > 0) {
                    leftpath += "/";
                }                
                leftpath += item;
            });
            _this.Instance.LeftPanelAbsolutePath = leftpath;

            var rightpath = "";
            _this.RightPanelStack.forEach(function (item, idx, arr) {
                if (idx > 0) {
                    rightpath += "/";
                }
                rightpath += item;
            });
            _this.Instance.LeftPanelAbsolutePath = leftpath;
            _this.Instance.RightPanelAbsolutePath = rightpath;
            _this.Instance.LeftPanelHttpPath = location.protocol + "//" + location.host + leftpath.replace('~', '');
            _this.Instance.RightPanelHttpPath = location.protocol + "//" + location.host + rightpath.replace('~', '');
        }
        this.ReflectDirectoryStack();
        
        
        this.LoadDirectories = function () {
            _this.Instance.CurrentAction = "read";
            var Returned = ProcessAJAXRequest("/Admin/Filemanager", _this.Instance);

            _this.Instance.LeftPanelList = Returned.LeftPanelList;
            _this.Instance.RightPanelList = Returned.RightPanelList;
            //-------------------------------------ADD FULL PATH RELATIVE PATH INFORMATION-----------------------
            _this.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                item["WebPath"] = location.protocol + "//" + location.host + _this.Instance.LeftPanelAbsolutePath.replace('~', '') + "/" + item.Name;
                item["LocationFolder"] = _this.Instance.LeftPanelAbsolutePath.replace('~', '');
            });
            _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                item["WebPath"] = location.protocol + "//" + location.host + _this.Instance.RightPanelAbsolutePath.replace('~', '') + "/" + item.Name;
                item["LocationFolder"] = _this.Instance.RightPanelAbsolutePath.replace('~', '');
            });
        }
        this.LoadDirectories();

        this.Instance.LeftPanelList = this.Instance.LeftPanelList.sort(SoftByTypeAndName);
        this.Instance.RightPanelList = this.Instance.RightPanelList.sort(SoftByTypeAndName);
        this.LeftPanelSelectedName = null;
        this.RightPanelSelectedName = null;

        //--------------------------------------------BACK FORWARD OPERATIONS---------------------------------------------
        this.CreateStackPath = function (Stack) {
            var ret = '/';
            Stack.forEach (function (item, idx, arr) {
                ret += item;
                ret += '/';
            })
            ret = ret.substr(2);
            return ret;
        }
        SaveActualPathToCookie(_this.CreateStackPath(_this.LeftPanelStack));


        this.LeftPanelGoBack = function () {
            if (_this.LeftPanelStack.length > 1) {
                _this.LeftPanelStack.pop();
                _this.ReflectDirectoryStack();
                SaveActualPathToCookie(_this.CreateStackPath(_this.LeftPanelStack));
                _this.LoadDirectories();
            }
        }
        this.LeftPanelGoForward = function (item) {
            if (item.Type == "folder") {
                _this.LeftPanelStack.push(item.Name);
                _this.ReflectDirectoryStack();
                SaveActualPathToCookie(_this.CreateStackPath(_this.LeftPanelStack));
                _this.LoadDirectories();
            }
        }

        this.RightPanelGoBack = function () {
            if (_this.RightPanelStack.length > 1) {
                _this.RightPanelStack.pop();
                _this.ReflectDirectoryStack();
                SaveActualPathToCookie(_this.CreateStackPath(_this.RightPanelStack));
                _this.LoadDirectories();
            }
        }
        this.RightPanelGoForward = function (item) {
            if (item.Type == "folder") {
                _this.RightPanelStack.push(item.Name);
                _this.ReflectDirectoryStack();
                SaveActualPathToCookie(_this.CreateStackPath(_this.RightPanelStack));
                _this.LoadDirectories();
            }
        }
//--------------------------------------------EOF BACK FORWARD OPERATIONS-----------------------------------------------


        this.SetLeftPanelSelected = function (item) {
            _this.LeftPanelSelectedName = item.Name; // For highlighting
            _this.Instance.SelectedObject = item; //For JSON operations
            _this.RightPanelSelectedName = null;
            _this.Instance.ActivePanel = "left";            
        }

        this.SetRightPanelSelected = function (item) {
            _this.RightPanelSelectedName = item.Name; // For highlighting
            _this.Instance.SelectedObject = item; // For JSON operations
            _this.LeftPanelSelectedName = null;
            _this.Instance.ActivePanel = "right";             
        }

        this.ProcessRenameOperation = function () {
            if (_this.Instance.SelectedObject != null) {
                $('#RenameOperation').modal('show');
            }
            else {
                $('#NothingSelected').modal('show');
            }
        }

        this.DeleteOperation = function () {
            _this.Instance.CurrentAction = 'delete';
            var Returned = ProcessAJAXRequest("/Admin/Filemanager", _this.Instance);
            _this.LoadDirectories();
        }

        this.ProcessDeleteOperation = function () {
            var _somethingfound = false;
            if (_this.Instance.ActivePanel == 'left') {
                _this.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                    if (item.isChecked == true) {
                        _somethingfound = true;
                        _this.Instance.RightPanelAbsolutePath = _this.Instance.LeftPanelAbsolutePath;
                        _this.RightPanelStack = _this.LeftPanelStack.slice();
                    }
                });
            }
            else {
                if (_this.Instance.ActivePanel == 'right') {
                    _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                        if (item.isChecked == true) {
                            _somethingfound = true;
                            _this.Instance.LeftPanelAbsolutePath = _this.Instance.RightPanelAbsolutePath;
                            _this.LeftPanelStack = _this.RightPanelStack.slice();
                        }
                    });
                }
            }
            if (_somethingfound == false) {
                $('#NothingSelected').modal('show');
            }
            else {
                $('#EnsureDeleteOperation').modal('show');
            }
        }

        this.ProcessNewDirectoryOperation = function () {
            $('#NewDirectory').modal('show');
        }

        this.TryToRename = function () {
            if (_this.Instance.NewObjectName == "") {
                $('#NewNameEmpty').modal('show');
            }
            else {// Check, have this folder same element with same name that was entered by user
                if (_this.Instance.ActivePanel == 'left') {
                    _this.Instance.LeftPanelList.forEach(function (item,idx,arr) {
                        if (item.Type == _this.Instance.SelectedObject.Type) {
                            if (item.Name.toLowerCase() == _this.Instance.NewObjectName.toLowerCase()) {
                                $('#ElementExists').modal('show');
                            }
                            else {
                                _this.Rename();
                            }
                        }
                    });
                }
                else {
                    if (_this.Instance.ActivePanel == 'right') {
                        _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                            if (item.Type == _this.Instance.SelectedObject.Type) {
                                if (item.Name.toLowerCase() == _this.Instance.NewObjectName.toLowerCase()) {
                                    $('#ElementExists').modal('show');
                                }
                                else {
                                    _this.Rename();
                                }
                            }
                        });
                    }
                }
            }
        }
        this.Rename = function () {
            _this.Instance.CurrentAction = "rename";
            var Returned = ProcessAJAXRequest("/Admin/Filemanager", _this.Instance);
            _this.LoadDirectories();
        }

        this.TryToCreateDirectory = function () {
            if (_this.Instance.NewDirectoryName == '') {
                $('#NewNameEmpty').modal('show');
            }
            else { // Check, have this panel same directory
                var _samefound = false;
                if (_this.Instance.ActivePanel == 'left') {
                    _this.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                        if (item.Name.toLowerCase() == _this.Instance.NewDirectoryName.toLowerCase()) {
                            _samefound = true;                            
                        }
                    });
                }
                else {
                    if (_this.Instance.ActivePanel == 'right') {
                        _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                            if (item.Name.toLowerCase() == _this.Instance.NewDirectoryName.toLowerCase()) {
                                _samefound = true;
                            }
                        });
                    }
                }
                if (_samefound == false) {
                    _this.CreateDirectory();
                }
                else {
                    $('#DirectoryExists').modal('show');
                }

            }
        }

        this.CreateDirectory = function () {
            _this.Instance.CurrentAction = "newfolder";
             var Returned = ProcessAJAXRequest("/Admin/Filemanager", _this.Instance);
            _this.LoadDirectories();
        }

        //---------------------------------------------MOVE OPERATION-------------------------------------------------------
        this.ProcessMoveOperation = function () {
            var _LeftPanelItem = null;
            var _RightPanelItem = null;
            var _sameObjectsFounded = false; // Flag indicating that same objects(files or folder) was found
            var _selfCopyDetected = false; // Example LeftPanel:/Content/Upload/"directory" opened <------- RightPanel:/Content/Upload opened  "directory" is checked. Move object self move operation obhect
            if (_this.Instance.LeftPanelAbsolutePath == _this.Instance.RightPanelAbsolutePath) { // Path are same. Move operation impossible
                $('#SamePanels').modal('show');
            }
            else { //Panels path are not same. If that- one of panels is selected exactly
                if (_this.Instance.ActivePanel == 'left') {
                    _this.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                        _LeftPanelItem = item;
                        if (item.isChecked == true) {
                            if (_this.Instance.RightPanelHttpPath == item.WebPath) { // Self move protection
                                _selfCopyDetected = true;
                            }
                        }
                        _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                            if (item.Name == _LeftPanelItem.Name) {
                                if (item.Type == _LeftPanelItem.Type) {
                                    if (_LeftPanelItem.isChecked == true) {
                                        _sameObjectsFounded = true;
                                    }
                                }
                            }
                        });
                    });
                }
                else {
                    if (_this.Instance.ActivePanel == 'right') {
                        _this.Instance.RightPanelList.forEach(function (item, idx, arr) {
                            _RightPanelItem = item;
                            if (item.isChecked == true) {
                                if (_this.Instance.LeftPanelHttpPath == item.WebPath) { // Self move protection
                                    _selfCopyDetected = true;
                                }
                            }
                            _this.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                                if (item.Name == _RightPanelItem.Name) {
                                    if (item.Type == _RightPanelItem.Type) {
                                        if (_RightPanelItem.isChecked == true) {
                                            _sameObjectsFounded = true;
                                        }
                                    }
                                }
                            });
                        });
                    }
                }
                if (_sameObjectsFounded == true) {
                    $('#EqualElementsFound').modal('show');
                }
                else {
                    if (_selfCopyDetected == true) {
                        $('#SelfCopyDetected').modal('show');
                    }
                    else {
                        _this.Instance.CurrentAction = "move";
                        var Returned = ProcessAJAXRequest("/Admin/Filemanager", _this.Instance);
                        _this.LoadDirectories();
                    }
                }
            }
        }
    });

    angular.module('GDYApp', ['FileManager', 'angularFileUpload', 'LinkBuffer']);
    angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http, FileManagerInstance, FileUploader, LinkBufferModel) {
        var _scope = $scope

        $scope.LinkBuffer = LinkBufferModel;
        $scope.FileManager = FileManagerInstance;
        $scope.FileUploader = new FileUploader({ url: '/Admin/FileManager', removeAfterUpload: true });

        _scope.FileManager.Instance.ActivePanel = 'left';

        $scope.FileUploader.onSuccessItem = function () {
            _scope.FileManager.LoadDirectories();
            alert('FILE UPLOADED');
        }

        $scope.ToBuffer = function () {
            if (_scope.FileManager.Instance.ActivePanel == 'left') {
                var fullpath = _scope.FileManager.CreateStackPath(_scope.FileManager.LeftPanelStack);
                fullpath += _scope.FileManager.LeftPanelSelectedName;
                _scope.LinkBuffer.AddLink(_scope.FileManager.LeftPanelSelectedName, fullpath);
            }
            else {
                if (_scope.FileManager.Instance.ActivePanel == 'right') {
                    var fullpath = _scope.FileManager.CreateStackPath(_scope.FileManager.RightPanelStack);
                    fullpath += _scope.FileManager.RightPanelSelectedName;
                    _scope.LinkBuffer.AddLink(_scope.FileManager.RightPanelSelectedName, fullpath);
                }
            }
        }

        $scope.OpenBuffer = function () {
            $scope.LinkBuffer.Load();
            $('#LinkBufferModal').modal('show');
        }

        $scope.Upload = function () {
            var SameFound = false;
            if ($scope.FileUploader.queue[0] != null) {
                if ($scope.FileManager.Instance.ActivePanel == 'left') {
                    $scope.FileManager.Instance.LeftPanelList.forEach(function (item, idx, arr) {
                        if (item.Name == $scope.FileUploader.queue[0].file.name) {
                            if (item.Type == 'file') {
                                SameFound = true;
                                $('#ElementExists').modal('show');                                
                            }
                        }
                    });
                }
                else {
                    if ($scope.FileManager.Instance.ActivePanel == 'right') {
                        $scope.FileManager.Instance.RightPanelList.forEach(function (item, idx, arr) {
                            if (item.Name == $scope.FileUploader.queue[0].file.name) {
                                if (item.Type == 'file') {
                                    SameFound = true;
                                    $('#ElementExists').modal('show');
                                }
                            }
                        });
                    }
                }
            }
            else {
                SameFound = true;
                $('#FileEmpty').modal('show');
            }
            if (SameFound == false) {
                $scope.FileUploader.uploadAll();
            }


        }
    });



