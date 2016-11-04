(function ($) {
    var defaultOptions = {
        imageFileTypes: ['image/jpeg', 'image/png', 'image/gif'],
        initMessage: "Hazırlanıyor...",
        uploadFinished: function (fileinstance) {
            alert("Upload Finished")
        },
        uploadAborted: function () { return true; },
        fileSelected: function (instance, file) { return true; },
        fileUploading: function (instance, file) { return true; },
        fileUploaded: function (instance, file, data) { },
        fileRemoved: function () { instance, file },
        allFilesUploaded :function(instance, group,file){},
        multiSelect:true,
        parameters: {},
        action: "/CKEditor/uploadfile",
        ResumeCheckUrl: "/CKEditor/GetFileLength",
        buttontext: "Gözat",
        chunkSize: 1 * 1024 * 1024,
        FileParamName: "File",
        headers: {},
        containerElementID: null,
        createItemFunction : null
    }
    var newOptions = null;
        
    var helper = {
        formatSize: function (size) {
            if (size < 1024) {
                return size + ' bytes';
            } else if (size < 1024 * 1024) {
                return (size / 1024.0).toFixed(0) + ' KB';
            } else if (size < 1024 * 1024 * 1024) {
                return (size / 1024.0 / 1024.0).toFixed(1) + ' MB';
            } else {
                return (size / 1024.0 / 1024.0 / 1024.0).toFixed(1) + ' GB';
            }
        },
        generateUniqueIdentifier: function (file) {
            
            //var func = (file.slice ? 'slice' : (file.mozSlice ? 'mozSlice' : (file.webkitSlice ? 'webkitSlice' : 'slice')));
            //var blob = file[func](0, file.size);
            //var x = new FileReaderSync().readAsArrayBuffer(file);


            ////fr.readAsArrayBuffer(blob);
            ////var x  = fr.result;
            //return CryptoJS.MD5(x)
            var relativePath = file.webkitRelativePath || file.fileName || file.name; // Some confusion in different versions of Firefox
            var size = file.size;
            return (size + '-' + relativePath.replace(/[^0-9a-zA-Z_-]/img, ''));
        },
        isJSon : function (o) {
            try {
                var json = JSON.parse(o);
                return true;
            }
            catch (e) {
                return false
            }
        }
    }

    var jDataFiles = function () {
        this.Files = [];
        this.AddFile = function (file) {
            this.Files.push(file);
        }
        this.RemoveFile = function (file) {
            var fileInstance = null;
            if (typeof file === "number") {
                fileInstance = this.Files[file];

                this.Files.splice(file, 1);
            }
            else {
                fileInstance = file;
                var index = this.Files.indexOf(file);
                if (index > -1) {
                    this.Files.splice(index, 1);
                }
            }
            if (fileInstance != null && fileInstance.CurrentOptions.fileRemoved) {
                fileInstance.CurrentOptions.fileRemoved(fileInstance.uploaderInstance, fileInstance);
            }
        }
    };
    var Chunk = function (offset, file, jdatafile, callback) {
        var _instance = this;
        var jDataFile = jdatafile;
        var chunkSize = jdatafile.CurrentOptions.chunkSize;
        var startByte = chunkSize * offset;
        var endByte = Math.min(file.size, (offset + 1) * chunkSize);
        this.InProgress = false;

        this.Offset = offset;
        this.Cancel = null;
        this.StartUpload = function () {
            jdatafile.LastChunkOffset = offset;
            var OnProgress = function (e) {
                if (e.lengthComputable) {
                    if (callback)
                        callback("percent", _instance, e.loaded);
                    //var percentage = Math.round((e.loaded * 100) / e.total);
                    //if (this.currentProgress !== percentage) {
                    //    //progressBarDiv.width((406 / 100) * percentage);
                    //    this.currentProgress = percentage;
                    //    if (callback)
                    //        callback("percent", _instance, e.loaded);
                    //}
                }
            };



            try {
                var xhr = new XMLHttpRequest();
                _instance.Cancel = function ()
                {
                    xhr.abort();
                }
                var upload = xhr.upload;
                upload.addEventListener('progress', OnProgress, false);
                //upload.currentProgress = 0;
                var chunkFinished = function () {
                    //*********************************
                    //UPLOAD FINISHED
                    //*********************************
                    upload.removeEventListener('progress', OnProgress, false);
                    upload = null;
                    _instance.InProgress = false;
                    var serverResponse = null;

                    if (xhr.responseText) {
                        try {
                            serverResponse = jQuery.parseJSON(xhr.responseText);
                        }
                        catch (e) {
                            serverResponse = xhr.responseText;
                        }

                        //HATA VAR MI?
                        if (xhr.status < 200 || xhr.status > 299) {
                            if (callback)
                                callback("error",_instance, serverResponse);
                        }
                        else {
                            if (callback)
                                callback("complete", _instance, serverResponse);
                        }
                    }
                };
                xhr.onload = chunkFinished;
                xhr.onabort = function ()
                {
                    if (callback)
                        callback("abort",_instance);
                }
                xhr.open("POST", jdatafile.CurrentOptions.action, true);

                var chunkParams = {
                    ChunkNumber: offset,
                    ChunkSize: jdatafile.CurrentOptions.chunkSize,
                    CurrentChunkSize: endByte - startByte,
                    FileSize: file.size,
                    FileType: file.type,
                    UniqueIdentifier: jdatafile.UniqueIdentifier,
                    FileName: jdatafile.FileName,
                    RelativePath: jdatafile.RelativePath
                };

                var formDataParams = $.extend({}, chunkParams, jdatafile.CurrentOptions.parameters);

                var fd = new FormData();

                var func = (file.slice ? 'slice' : (file.mozSlice ? 'mozSlice' : (file.webkitSlice ? 'webkitSlice' : 'slice')));
                var bytes = file[func](startByte, endByte);


                
                fd.append(jdatafile.CurrentOptions.FileParamName, bytes);
                
                $.each(formDataParams, function (key, val) {
                    var value = null;
                    if (typeof val === "function")
                        value = val(jdatafile,offset); //lazım olursa diye jdatafile ı ve offseti gönder.. ?:
                    else
                        value = val;
                    
                    fd.append(key, value);
                });
                
                $.each(jdatafile.CurrentOptions.headers, function (k, v) {
                    xhr.setRequestHeader(k, v);
                });
                _instance.InProgress = true;
                xhr.setRequestHeader("Accept", "*/*")
                xhr.send(fd);
            } catch (e) {
                if (callback)
                    callback("error", _instance);
            }
        };
    };
    var jDataFile = function (file,currentOptions,juploaderInstance) {

        var _instance = this;
        this.StatusChanged = null;
        this.uploaderInstance = juploaderInstance;
        this.Remove = function () {
            _instance.Paused = false;
            var lastChunk = chunks[_instance.LastChunkOffset];
            if (lastChunk.InProgress) {
                lastChunk.Cancel();
            }
            //_instance.CurrentOptions.uploadAborted(_instance);
            $(_instance.FileInfoDiv, $(document)).remove();
            _jDataFiles.RemoveFile(_instance);

        };
        var chunkCallBack = function () {
            var callType = arguments[0];
            var chunkInstance = arguments[1];
            
            switch (callType)
            {
                case "percent":
                    OnProgress(chunkInstance.Offset,arguments[2]);
                    break;
                case "error":
                    if (_instance.CurrentOptions.uploadError)
                        _instance.CurrentOptions.uploadError(_instance.uploaderInstance, _instance, arguments[2]);
                    _instance.Remove();
                    break;
                case "complete":
                    
                    if (chunkInstance.Offset + 1 < chunks.length) {
                        if (!_instance.Paused)
                            chunks[chunkInstance.Offset + 1].StartUpload();
                    }
                    else {                                                                          //Response
                        _instance.CurrentOptions.uploadFinished(_instance.uploaderInstance,_instance,arguments[2]);
                    }
                    break;
                case "abort":
                        //_instance.CurrentOptions.uploadFinished(_instance.uploaderInstance,_instance,arguments[2]);
                        _instance.CurrentOptions.uploadAborted(_instance.uploaderInstance,_instance);
                    break;
                default:
                    break;
            }
        }
        this.LastChunkOffset = 0;
        
        this.InProgress = false;
        this.UniqueIdentifier = helper.generateUniqueIdentifier(file);
        this.FileName = file.fileName || file.name;
        this.RelativePath = file.webkitRelativePath || this.FileName;
        this.File = file;
        this.FileSize = file.size;
        var fileSize = (file.size / 1024);
        var fileSizeMB = fileSize >= 1024;
        if (fileSizeMB)
            fileSize = fileSize / 1024;
        this.CurrentOptions = currentOptions;
        var chunks = [];

        for (var offset = 0; offset < Math.max(Math.ceil(file.size / this.CurrentOptions.chunkSize), 1) ; offset++) {
            chunks.push(new Chunk(offset, file,this,chunkCallBack));
        }


        var existsFileLength = function ()
        {
            var _data = null;
            var postData = {
                UniqueIdentifier: _instance.UniqueIdentifier,
                FileName : _instance.FileName
            };
            $.extend(postData, _instance.CurrentOptions.parameters);
            $.ajax({
                type: "GET",
                url: _instance.CurrentOptions.ResumeCheckUrl,
                async:false,
                data:postData,
                dataType:"json",
                success: function (data) {
                    _data = data;
                }
            });
            if (_data != null) {
                
                //$.extend(_instance.CurrentOptions.parameters, { Guid: _data.guid , MediaSuffix:_data.mediaSuffix });
                return _data.fileLength;
            }
            else
                return 0;
        }

        this.Kick = function () {
            var existsFileLen = existsFileLength();
            var startOffset = Math.ceil(existsFileLen / _instance.CurrentOptions.chunkSize);
            if (_instance.CurrentOptions.fileUploading)
                if (!_instance.CurrentOptions.fileUploading(_instance.uploaderInstance, _instance)) {
                    return;
                }
            if (startOffset < chunks.length)
                chunks[startOffset].StartUpload();
            else if (startOffset == chunks.length)
                chunks[0].StartUpload();
                
            else
                chunks[0].StartUpload();
            
        }



        //this.Paramters = parameters;
        this.CompletePercent = 0;
        this.Paused = false;
        var xhr = null;
        var _postAction;
        var previewImage = $("<img style='width:75px;height:100px'/>");
        var progressBarDiv = $("<div class='jDataUploaderProgressBar jDataUploaderBorderRadius'></div>");
        var fileNameSpan = $("<span>" + file.name + "</span>");
        var sizeSpan = $("<span>" + (fileSizeMB ? fileSize.toFixed(2) : fileSize.toFixed(0)) + " " + (fileSizeMB ? "MB" : "KB") + "</span>");
        var speedAndPercentSpan = $("<span></span>");
        this.Preview = function (onlyUrl) {
            var preview = false;
            if (this.CurrentOptions != null && this.CurrentOptions.imageFileTypes.push && this.CurrentOptions.imageFileTypes.length) {
                var imageFile = false;
                for (var i = 0; i < this.CurrentOptions.imageFileTypes.length; i++) {
                    if (this.CurrentOptions.imageFileTypes[i] === _instance.File.type) {
                        imageFile = true; break;
                    }
                }
                preview = imageFile && _instance.File.size <= 10485760;

            }
            if (preview) {
                //$parent.append(previewImage);
                blobURLref = window.URL.createObjectURL(_instance.File);
                if (onlyUrl === true)
                    return blobURLref;
                else
                    previewImage.attr('src', blobURLref);
                //previewImage.attr('src', blobURLref);
            }
            else return null;
            //return preview;
        };
        this.Cancel = function () {
            _instance.Paused = false;
            var lastChunk = chunks[_instance.LastChunkOffset];
            if (lastChunk.InProgress)
                lastChunk.Cancel();
            else {
                _instance.CurrentOptions.uploadAborted(_instance);
            }
        };
        this.PauseOrResume = function () {
            if (!_instance.Paused) {
                _instance.Paused = true;
               
            }
            else {
                _instance.Paused = false;
                var lastChunk = chunks[_instance.LastChunkOffset];
                if (!lastChunk.InProgress) {
                    lastChunk.StartUpload();
                }
            }
        };
        this.FileInfoDiv = !_instance.CurrentOptions.createItemFunction ? (function () {
            var pauseLink = $("<a href='#' class='cancelLink' >[Beklet]</a>");
            var cancelLink = $("<a href='#' class='pauseLink' >[İptal]</a>");
            cancelLink.click(_instance.Cancel);
            pauseLink.click(_instance.PauseOrResume);
            var containerDiv = $("<div class='jDataUploaderItemContainer jDataUploaderDropShadow jDataUploaderBorderRadius'></div>");
            var clearDiv = $("<div style='clear:both;'></div>")
            var imageContainerDiv = $("<div class='jDataUploaderPreviewImageContainer jDataImageBorder'></div>");
            var infoContainerDiv = $("<div class='jDataUploaderPreviewInfoContainer'></div>");
            infoContainerDiv.append(fileNameSpan);
            infoContainerDiv.append(sizeSpan);
            infoContainerDiv.append(speedAndPercentSpan);
            var buttonsContainerDiv = $("<div class='jDataUploaderPreviewButtonsContainer'></div>");
            buttonsContainerDiv.append(pauseLink);
            buttonsContainerDiv.append(cancelLink);
            imageContainerDiv.append(previewImage);
            containerDiv.append(imageContainerDiv);
            containerDiv.append(infoContainerDiv);
            containerDiv.append(buttonsContainerDiv);
            containerDiv.append(progressBarDiv);
            containerDiv.append(clearDiv);
            return containerDiv;
        })() : _instance.CurrentOptions.createItemFunction(_instance);
        //Returns true if it is a DOM node





        this.StartUpload = function (postAction) {
            _postAction = postAction;
            
            try {
                var xhr = new XMLHttpRequest();
                var upload = xhr.upload;
                //var boundary = "jDataUploaderBoundary" + (new Date()).getTime();
                upload.currentStart = (new Date()).getTime();
                upload.currentProgress = 0;
                upload.startData = 0;
                speedAndPercentSpan.css({ color: "black" });
                speedAndPercentSpan.text(this.CurrentOptions.initMessage);
                progressBarDiv.css("background-color", "#2699b9");
                upload.addEventListener('progress', OnProgress, false);

                xhr.onerror = function (e) {
                    alert("error");
                }
                xhr.onload = function () {
                    //*********************************
                    //UPLOAD FINISHED
                    //*********************************
                    upload.removeEventListener('progress', OnProgress, false);
                    upload = null;

                    var serverResponse = null;

                    if (xhr.responseText) {
                        try {
                            serverResponse = jQuery.parseJSON(xhr.responseText);
                        }
                        catch (e) {
                            serverResponse = xhr.responseText;
                        }

                        //HATA VAR MI?
                        if (xhr.status < 200 || xhr.status > 299) {
                            //opts.error(xhr.statusText, file, fileIndex, xhr.status);
                            progressBarDiv.css("background-color", "red");
                            speedAndPercentSpan.css({ color: "black" });
                            speedAndPercentSpan.text("Yükleme hatası...");
                        }
                        else {
                            var currentSpeed = speedAndPercentSpan.text();
                            if (currentSpeed === "" || currentSpeed == _instance.CurrentOptions.initMessage)
                                speedAndPercentSpan.text("100%");
                            speedAndPercentSpan.css({ color: "green" });
                            progressBarDiv.width(406);
                            _instance.CurrentOptions.uploadFinished(_instance);
                        }
                    }
                };

                xhr.open("POST", _postAction, true);
                var fd = new FormData();
                fd.append("x", new Blob([File], { type: File.type }), File.name);
                $.each(this.CurrentOptions.parameters, function (key, val) {
                    var value = null;
                    if (typeof val === "function")
                        value = val();
                    else
                        value = val;

                    fd.append(key, value);
                });
                xhr.send(fd);

            } catch (e) {
                alert(e.message);
            }

        }
        this.currentStart = (new Date()).getTime();
        this.currentProgress = 0;
        this.startData = 0;
        
        var OnProgress = function (offset, loaded) {
            var totalSize = file.size;
            var loaded = (offset * _instance.CurrentOptions.chunkSize) + (loaded);
            
                var percentage = Math.round(((loaded) * 100) / totalSize);
                if (this.currentProgress !== percentage) {
                    //progressBarDiv.width((406 / 100) * percentage);
                    this.currentProgress = percentage;

                    var elapsed = new Date().getTime();
                    var diffTime = elapsed - _instance.currentStart;
                    if (diffTime >= 1000) {
                        var diffData = loaded - _instance.startData;
                        var speed = diffData / diffTime; // KB per second
                        speedmb = false;
                        speed = (speedmb = speed > (1024)) ? speed / (1024 ) : (speed);
                        //console.log(this.currentProgress + "% - " + (speedmb ? speed.toFixed(2) : speed.toFixed(0)) + (speedmb ? " Mb/s" : " Kb/s"));
                        ////speedAndPercentSpan.text(this.currentProgress + "% - " + (speedmb ? speed.toFixed(2) : speed.toFixed(0)) + (speedmb ? " Mb/s" : " Kb/s"));
                        //speedAndPercentSpan.text(this.currentProgress + "% - " + loaded );//+ (speedmb ? speed.toFixed(2) : speed.toFixed(0)) + (speedmb ? " Mb/s" : " Kb/s"));
                        //opts.speedUpdated(this.index, this.file, speed);
                        _instance.startData = loaded;
                        _instance.currentStart = elapsed;
                        if (_instance.StatusChanged != null)
                            _instance.StatusChanged(this.currentProgress, (speedmb ? speed.toFixed(2) : speed.toFixed(0)) + (speedmb ? " Mb/s" : " Kb/s"));
                    }
                }
            
        };


    };
    var _jDataFiles = new jDataFiles();

    $.jDataUploader = function (options) {
        var _instance = this;
        var opts = $.extend({}, defaultOptions, options);
        opts.uploadFinished = opts.uploadAborted = function (uploadInstance,finstance) {
            $(finstance.FileInfoDiv, $(document)).remove();
            _jDataFiles.RemoveFile(finstance);
        }
        newOptions = opts;
        var multipleString = newOptions.multiSelect === true ? "multiple='multiple'" : "";
        var inputFile = $("<input type='file' style='display:none;' " + multipleString + " />");
        //var browseButton = $("<input type='button' value='" + opts.buttontext + "' />");
        //var startUploadButton = $("<input type='button' value='Start Upload'/>");
        
        //startUploadButton.click(function (dataitem) {
        //    for (var i = 0; i < dataitem.Files.Files.length ; i++) {
        //        dataitem.Files.Files[i].StartUpload(_instance.attr("action"));
        //    }

        //});
        var lastOptions = null;
        var _groups = {};
        this.ChangeGroupParemeters = function (groupName, options) {
            if (_groups.hasOwnProperty(groupName)) {
                _groups[groupName].allFilesUploaded = options.allFilesUploaded;
                for (var i = 0; i < _groups[groupName].files.length; i++) {
                    var file = _groups[groupName].files[i];
                    var extended = $.extend(true, {}, file.CurrentOptions, options);
                    file.CurrentOptions = extended;
                }
            }
        }
        var containerInBody = false;
        var containerDiv = null;
        this.Browse = function (options) {
            _groups[options.group] = {count:0, files : [], allFilesUploaded:options.allFilesUploaded};
            
            if (options != null) {
                var baseUpFinish = opts.uploadFinished;
                var baseUpAborted = opts.uploadAborted;
                //var baseRemove = opts.fileRemoved;
                lastOptions = $.extend({}, opts, options);
                var newUpAbort = lastOptions.uploadAborted;
                if (newUpAbort)
                    lastOptions.uploadAborted = function (uploaderInstance, finstance)
                    {

                        baseUpAborted(uploaderInstance, finstance);
                        newUpAbort(uploaderInstance, finstance);
                        
                        var index = _groups[finstance.CurrentOptions.group].files.indexOf(finstance);
                        if (index > -1) {
                            _groups[finstance.CurrentOptions.group].count--;
                            _groups[finstance.CurrentOptions.group].files.splice(index,1);
                        }
                    }
                var newUpFinish = lastOptions.uploadFinished;
                if (newUpFinish)
                    lastOptions.uploadFinished = function (uploaderInstance,finstance, data) {
                        baseUpFinish(uploaderInstance, finstance,  data);
                        newUpFinish(uploaderInstance, finstance, data);

                        var index = _groups[finstance.CurrentOptions.group].files.indexOf(finstance);
                        if (index > -1) {
                            _groups[finstance.CurrentOptions.group].count--;
                            _groups[finstance.CurrentOptions.group].files.splice(index, 1);
                        }
                        
                        if (_groups[finstance.CurrentOptions.group].count < 1 && _groups[finstance.CurrentOptions.group].allFilesUploaded) {
                            //alert("groupComplete :" + finstance.CurrentOptions.group);
                            if (_groups[finstance.CurrentOptions.group].allFilesUploaded)
                                _groups[finstance.CurrentOptions.group].allFilesUploaded(finstance.CurrentOptions.group, finstance.CurrentOptions)
                        }
                    };

            }
            else
                lastOptions = opts;


            containerDiv = lastOptions.containerElementID !== null ? $("#" + lastOptions.containerElementID) : (containerDiv ? containerDiv : $("<div class='jDataUploaderContainer'></div>"));
            if (lastOptions.containerElementID === null && !containerInBody) {
                $("body").append(containerDiv);
                containerInBody = true;
            }

            inputFile.click();
        }
        this.Clear = function (groupName)
        {
            var result = false;
            if (groupName && _groups[groupName] !== undefined) {
                //console.log(groupName + ":" + _groups[groupName].files.length);
                var files = [];
                files = files.concat(_groups[groupName].files);
                _groups[groupName].files = [];
                for (var i = 0; i < files.length; i++) {
                    //console.log("File:" + i);
                    
                    var file = files[i];
                    file.Remove();
                }
            }
            else {
                for (var i = 0; i < _jDataFiles.Files.length; i++) {
                    var file = files[i];
                    file.Remove();
                }

            }
            return result;
        }
        this.Upload=function(groupName)
        {
            var result = false;
            if (groupName && _groups[groupName] !== undefined) {
                console.log(groupName + ":" + _groups[groupName].files.length);
                var files = [];
                files = files.concat(_groups[groupName].files);
                for (var i = 0; i < files.length; i++) {
                    console.log("File:" + i);
                    
                    var file =files[i];
                    if (!file.InProgress) { // eğer bu grupta ki dosyalardan birisi işlemdeyse sırayla devam edecektir. o yüzden upload başlatılmaz.
                        file.Kick();
                        result = true;
                    }
                }
            }
            else if (!groupName) {
                for (var i = 0; i < _jDataFiles.Files.length; i++) {
                    if(!_jDataFiles.Files[i].InProgress){
                        _jDataFiles.Files[i].Kick();
                        result = true;
                    }
                }
                
            }
            return result;
        }
        inputFile.on('change', function (e) {
           _instance.FileHandler(this);
        });
        //var createPrivewContainer = function ()
        //{
        //    FilesPreviewContainer.css({position:"absolute", top:"5px", rigth:"5px", padding:"5px", maxWidth:"250px"});
        //}
        //var FilesPreviewContainer = $("<div></div>");
        //var FilePreviewContainer = $("<div style='float:rigth;width:100px;height:150px;'></div>");
        $("body").append(inputFile);
        //this.append(browseButton);
        //this.append(startUploadButton);
        //this.Files = new jDataFiles();
        //this.Files.AddFile(new $.jDataFile(new object, null));
        this.FileHandler = function (fileInput) {
            
            var files = fileInput.files;
            for (var i = 0; i < (!lastOptions.singleFile ? files.length: 1); i++) {
                var file = new jDataFile(files[i], lastOptions,_instance);
                //file.data = currentData;
               
                //file.CurrentOptions = opts;
                if (lastOptions.fileSelected)
                    if (!lastOptions.fileSelected(_instance, file))
                        return;
                containerDiv.append(file.FileInfoDiv);
                if (file.Preview.call(file, _instance)) {
                    //_instance.append(file.PreviewImage);
                }
                //this.Files.AddFile(file);
                if (lastOptions.singleFile)
                {
                    var _files = [];
                    _files = _groups[lastOptions.group].files;//_jDataFiles.Files;
                    for (var o = 0; o < _files.length; o++) {
                        $(_files[o].FileInfoDiv, $(document)).remove();
                        _jDataFiles.RemoveFile(_files[o]);
                    }
                    //_jDataFiles.Files.splice(0, _jDataFiles.Files.length);
                    _groups[lastOptions.group].files.splice(0, _groups[lastOptions.group].files.length);
                    _groups[lastOptions.group].count = 0;
                }
                _jDataFiles.AddFile(file);
                _groups[lastOptions.group].count++;
                _groups[lastOptions.group].files.push(file);
                //file.Kick();
                if (lastOptions.browseComplate)
                    lastOptions.browseComplate(lastOptions);
            }
            fileInput.value = "";
            //return file;
        }
    }
})(jQuery);

//<div class="file-bar">
//                                            <a href="#">
//                                                <div class="file-bar-icon">
//                                                    <i class="fa fa-file"></i>
//                                                </div>
//                                                <div class="file-bar-info">
//                                                    <h5>Bilmem ne.jpg </h5><div style="width:50%;background-color:#f2f2f2;height:100%"></div>
//                                                </div>
//                                            </a>
//                                        </div>