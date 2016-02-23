angular.module('GDYApp', ['GDYControls']);
angular.module('GDYApp').controller('GDYController', function ($scope, $sce, $log, $http) {
    $scope.ErrorMessage = "no error";
    var _scope = $scope;
    $scope.Submit = function () {
        var from = new Date(_scope.DateFrom);
        var to = new Date(_scope.DateTo);
        var delta = to - from;
        if (delta < 0) {
            _scope.ErrorMessage = 'Sorry but from date is later than to';
            $('#FieldError').modal('show');
        }
        else {
            if (delta == 0) {
                _scope.ErrorMessage = 'Sorry, but there is same date in two selectors'
                $('#FieldError').modal('show');
            }
            else {
                if (isNaN(from)) {
                    _scope.ErrorMessage = 'Sorry, but field FROM is incorrect'
                    $('#FieldError').modal('show');
                }
                else {
                    if (isNaN(to)) {
                        _scope.ErrorMessage = 'Sorry, but field TO is incorrect'
                        $('#FieldError').modal('show');
                    }
                    else {
                        $("#QueryStatForm").submit();
                    }
                }
            }
        }
    }
    
    $scope.ProcessClearStatistics = function () {
        $('#ActionName').val('ClearStatistics');// Action Name
        $("#QueryStatForm").submit(); //Submit
    }

    $scope.ClearStatisticsClick = function () {
        $('#ClearStatisticsDiallog').modal('show');
    }
    
    function CreateDateMatrix(tabdata, lowdate, highdate) {
        var ret = [];
        function treatAsUTC(date) {
            var result = new Date(date);
            result.setMinutes(result.getMinutes() - result.getTimezoneOffset());
            return result;
        }

        function daysBetween(startDate, endDate) {
            var millisecondsPerDay = 24 * 60 * 60 * 1000;
            return (treatAsUTC(endDate) - treatAsUTC(startDate)) / millisecondsPerDay;
        }


        var daysCount = Math.trunc(daysBetween(lowdate, highdate));
        var DayMatrix = new Array(daysCount+1);
        
        function addDays(date, days) {
            function DatFormat(date) {
                this.Date = date.toISOString().substring(0, 10);;
                this.Cnt = 0;
                return this;
            }
            var tmp = new Date(date);
            tmp.setDate(tmp.getDate() + days);            
            return new DatFormat(tmp);
        }

        for (i = 0; i < DayMatrix.length; i++) {
            DayMatrix[i]= addDays(lowdate,i);
        }

        // Fill DayMatrix with count
        DayMatrix.forEach(function (dayInMatrix, idxDay, arrDayMatrix) {
            tabdata.forEach(function (visit, idxVisitors, arrVisitors) {
                if (dayInMatrix.Date == visit.Date) {
                    dayInMatrix.Cnt++;
                }
            });
        });
        ret = DayMatrix;
        return ret;
    }
    $(document).ready(function () {
        if ((DateFrom!=null)&&(DateTo!=null)){
            _scope.tabledata = Visitors;
            var DateCnt = CreateDateMatrix(_scope.tabledata, DateFrom, DateTo);
            var MaxVisitorCnt = 0;
            DateCnt.forEach(function (item, idx, arr) {
                if (item["Cnt"] > MaxVisitorCnt) {
                    MaxVisitorCnt = item["Cnt"];
                }
            });

            _scope.XYdata={
                Xpoints:0,
                Ypoints:10,
                maxYValue: MaxVisitorCnt,
                id: "GDYGraph1",
                XYPairs:DateCnt
            }
            _scope.XYdata.Xpoints = Object.keys(DateCnt).length;
            _scope.tabledata = DateCnt;
            for (i = 0; i < _scope.XYdata.Xpoints; i++) {
                _scope.tabledata[i]["Date"] = _scope.tabledata[i]["Date"].toString();
            }
        }
    });
});