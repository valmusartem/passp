app.controller('HomeController', ['$scope', 'FileUploader', 'FilterService', '$filter',
    function ($scope, FileUploader, FilterService, $filter) {

        $scope.redrawObject = {};

        $scope.uploader = new FileUploader({url: '/upload', removeAfterUpload: true});
        $scope.uploader.filters.push(
            {
                name: 'imageFilter',
                fn: function (item /*{File|FileLikeObject}*/, options) {
                    var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                    return '|jpg|png|jpeg|gif|'.indexOf(type) !== -1;
                }
            });

        $scope.uploader.onAfterAddingFile = function (item) {
            while ($scope.uploader.queue.length > 1) {
                $scope.uploader.queue[0].remove();
            }
        };

        $scope.uploader.onCompleteItem = function(item, response, status, headers) {

        };

        $scope.onlick = function() {
            $scope.uploader.uploadAll();
        };

        $scope.filters = [
            {
                name: "Linear Correction",
                function: "linearCorrection"
            },
            {
                name: "Median",
                function: "median"
            },
            {
                name: "Sharpen",
                function: "sharpen"
            },
            {
                name: "Blur",
                function: "blur"
            },
            {
                name: "Edge Detection",
                function: "edgeDetection"
            },
            {
                name: "Otsu",
                function: "otsu"
            },
            {
                name: "Dilation",
                function: "dilation"
            }
        ];

        $scope.appliedFilters = [];

        $scope.changeFilters = function () {
            redraw($scope.appliedFilters);
        };

        $scope.selectBorders = function () {
            redraw(true, true, [255, 0, 0, 255]);
        };

        function redraw(filters, applySelection, selectionColor) {
            if ($scope.redrawObject.redraw) {
                $scope.redrawObject.redraw(filters, applySelection, selectionColor);
            }
        }

    }]);
