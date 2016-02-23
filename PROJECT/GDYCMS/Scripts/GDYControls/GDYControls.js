angular.module('GDYControls', []);

angular.module('GDYControls').directive('gdyStatgraph', ['$document', function ($document) {
    return {
        templateUrl: '/Scripts/GDYControls/HTMLTemplates/GDYControls_graph.html',
        translude: true,
        scope: {
            paramstruct: '=',
        },
        link: function (scope, element, attrs) {

            var GraphPane = {
                Width: 0,
                Height: 0
            }

            function RefreshSize() {
                GraphPane.Width = element[0].children[0].clientWidth;
                GraphPane.Height = element[0].children[0].clientHeight;
            }

            var pane = document.getElementById(scope.paramstruct.id);
            var ctx = pane.getContext("2d");
            



            function DrawGrid() {
                RefreshSize();
                pane.width = GraphPane.Width;
                pane.height = GraphPane.Height;
                var xPadding = 20;
                var yPadding = 20;

                var ActualXSize = GraphPane.Width - 2 * xPadding; // Actual size on x Coordinate
                var ActualYSize = GraphPane.Height - 2 * yPadding; // Actual size on y Coordinate

                if (scope.paramstruct.maxYValue == 0) {
                    scope.paramstruct.maxYValue = 1;
                }

                var dxCord = Math.floor(ActualXSize / scope.paramstruct.Xpoints); // xAdditive
                var dyCord = Math.floor(ActualYSize / scope.paramstruct.maxYValue); // yAdditive

                var RoundedXMaxCord = (scope.paramstruct.Xpoints-1) * dxCord;
                var RoundedYMaxCord = scope.paramstruct.maxYValue * dyCord;

                ctx.rect(xPadding, yPadding, RoundedXMaxCord, RoundedYMaxCord);

                for (var xLineCnt = 0; xLineCnt < scope.paramstruct.Xpoints; xLineCnt++) {
                    var xCord = xLineCnt * dxCord;
                    ctx.moveTo(xCord + xPadding, 0 + yPadding);
                    ctx.lineTo(xCord + xPadding, RoundedYMaxCord + yPadding);
                    
                    if (xLineCnt == 0) { // Start of day scale
                        ctx.font = "italic 10pt Arial";
                        ctx.fillText(scope.paramstruct.XYPairs[0]["Date"], xCord + xPadding, RoundedYMaxCord + 1.8 * yPadding);
                    }
                    else {
                        if (xLineCnt == scope.paramstruct.Xpoints - 1) {
                            ctx.font = "italic 10pt Arial";
                            ctx.fillText(scope.paramstruct.XYPairs[xLineCnt]["Date"], xCord + xPadding, RoundedYMaxCord + 1.8 * yPadding);
                        }
                    }
                    
                }

                for (var yLineCnt = 0; yLineCnt < scope.paramstruct.Ypoints; yLineCnt++) {
                    var yCord = Math.floor((scope.paramstruct.maxYValue / scope.paramstruct.Ypoints) * dyCord * yLineCnt);
                    if (yLineCnt == 0) { // Start of day scale
                        ctx.font = "italic 10pt Arial";
                        ctx.fillText("0", 0.4*xPadding, RoundedYMaxCord + yPadding);
                    }
                    if (yLineCnt == scope.paramstruct.Ypoints - 1) {
                        ctx.font = "italic 10pt Arial";
                        ctx.fillText(scope.paramstruct.maxYValue, 0.4 * xPadding, yPadding);
                    }
                    ctx.moveTo(xPadding, yCord + yPadding);
                    ctx.lineTo(RoundedXMaxCord + xPadding, yCord + yPadding);
                }
                ctx.stroke();
                ctx.closePath();

                var XYPairs = scope.paramstruct.XYPairs;
                var gtx = pane.getContext("2d");
                gtx.beginPath();
                gtx.lineWidth = 5;
                gtx.moveTo(xPadding, RoundedYMaxCord + 1 * yPadding);

                function DrawXYPairs() {
                    for (idx = 0; idx < Object.keys(XYPairs).length; idx++) {
                        gtx.lineTo(idx * dxCord + xPadding, RoundedYMaxCord - XYPairs[idx]["Cnt"] * dyCord + yPadding);
                    }
                }

                DrawXYPairs();
                gtx.strokeStyle = 'blue';
                gtx.stroke();                
            }
            DrawGrid();                       
        }
    }
}]);

angular.module('GDYControls').directive('gdyGraph', ['$document', function ($document) {
    return {
        templateUrl: '/Scripts/GDYControls/HTMLTemplates/GDYControls_graph.html',
        translude: true,
        scope: {
            paramstruct: '=',
        },
        link: function (scope, element, attrs) {

            var GraphPane = {
                Width: 0,
                Height: 0
            }
            //	If creation structure is empty
            if (scope.paramstruct == null) {
                scope.paramstruct = {
                    id: "GDYGraph1",
                    minX: 0,
                    maxX: 30,
                    minY: 0,
                    maxY: 30,
                    XStep: 30,
                    YStep: 30,
                    value: 0
                }
            }

            function RefreshSize() {
                GraphPane.Width = element[0].children[0].clientWidth;
                GraphPane.Height = element[0].children[0].clientHeight;
            }

            var pane = document.getElementById(scope.paramstruct.id);
            var ctx = pane.getContext("2d");



            function DrawGrid() {
                RefreshSize();


                pane.width = GraphPane.Width;
                pane.height = GraphPane.Height;
                var xPadding = 20;
                var yPadding = 20;

                var ActualXSize = GraphPane.Width - 2 * xPadding; // Actual size on x Coordinate
                var ActualYSize = GraphPane.Height - 2 * yPadding; // Actual size on y Coordinate

                var dxCord = Math.floor(ActualXSize / scope.paramstruct.XStep); // xAdditive
                var dyCord = Math.floor(ActualYSize / scope.paramstruct.YStep); // yAdditive

                var RoundedXMaxCord = scope.paramstruct.XStep * dxCord;
                var RoundedYMaxCord = scope.paramstruct.YStep * dyCord;

                ctx.rect(xPadding, yPadding, RoundedXMaxCord, RoundedYMaxCord);

                for (var xLineCnt = 0; xLineCnt < scope.paramstruct.XStep; xLineCnt++) {
                    var xCord = xLineCnt * dxCord;
                    ctx.moveTo(xCord + xPadding, 0 + yPadding);
                    ctx.lineTo(xCord + xPadding, RoundedYMaxCord + yPadding);

                }

                for (var yLineCnt = 0; yLineCnt < scope.paramstruct.YStep; yLineCnt++) {
                    var yCord = yLineCnt * dyCord;
                    ctx.moveTo(xPadding, yCord + yPadding);
                    ctx.lineTo(RoundedXMaxCord + xPadding, yCord + yPadding);
                }

                ctx.moveTo(xPadding, RoundedYMaxCord + 1 * yPadding);
                var dxOnOne = RoundedXMaxCord / (scope.paramstruct.maxX - scope.paramstruct.minX);
                var dyOnOne = RoundedYMaxCord / (scope.paramstruct.maxY - scope.paramstruct.minY);

                var XYPairs = {
                    "0": { "x": 0, y: "0" },
                    "1": { "x": 5, y: "10" },
                    "2": { "x": 10, y: "4" },
                    "3": { "x": 15, y: "30" },
                    "4": { "x": 21, y: "7" },
                    "5": { "x": 22, y: "3" },
                }

                function DrawLine(XYPairs) {
                    for (idx = 0; idx < Object.keys(XYPairs).length; idx++) {
                        ctx.lineTo(XYPairs[idx].x * dxOnOne + xPadding, RoundedYMaxCord - (XYPairs[idx].y * dyOnOne) + yPadding);
                    }
                }
                DrawLine(XYPairs, dxOnOne, dyOnOne);
                //----------------------Creating Graph--------------------------
                ctx.stroke();
                ctx.closePath();
            }

            window.onresize = function (event) {
                RefreshSize();
            }


            RefreshSize();
            DrawGrid();
            //drawBoard();

        }

    }
}]);

angular.module('GDYControls').directive('gdyTable', ['$document', function ($document) {
    return {
        templateUrl: '/Scripts/GDYControls/HTMLTemplates/GDYControls_table.html',
        transclude: true,
        scope: {
            paramstruct: "="
        },
        link: function (scope, element, attrs) {
            // Определяем ключи по первому объекту
            var keysCount = Object.keys(scope.paramstruct[0]).length;
            var KeysArr = [];
            for (i = 0; i < keysCount; i++) {
                KeysArr.push(Object.keys(scope.paramstruct[0])[i]);
            }

            scope.tableHeader = KeysArr; // Массив ключей будет служить заголовком талблицы
            scope.tabledata = new Array(scope.paramstruct.length); // Создаем непосредственно тело таблицы
            for (i = 0; i < scope.paramstruct.length; i++) {
                scope.tabledata[i] = new Array(keysCount);
                for (j = 0; j < keysCount; j++) {
                    scope.tabledata[i][j] = scope.paramstruct[i][KeysArr[j]];
                }
            }

            scope.selectedIndex = null;
            scope.SelectItem = function ($index) {
                scope.selectedIndex = $index;
            }
        }
    }

}]);

