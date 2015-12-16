app.controller('HomeController', ['$scope', 'FileUploader',
    function ($scope, FileUploader) {

        $scope.redrawObject = {};

        $scope.uploader = new FileUploader({url: 'http://localhost:24226/api/recognizeImage', removeAfterUpload: true});
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
            $scope.words = response;
        };

        $scope.onlick = function() {
            $scope.uploader.uploadAll();
        };
    }]);
