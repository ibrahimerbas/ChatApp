var uploader = new $.jDataUploader(null);

function CreateFileItem(finstance) {
    //var _fileModel = new fileModel(finstance.UniqueIdentifier, finstance.FileName, 0, "0 kb/s");
    //viewModel.FileViewModel().Add(_fileModel);

    
    var itemDiv = $("<div class='file-bar'></div>");
    var a = $("<a href='#_'></a>");
    var fileBarIcon = $("<div class='file-bar-icon'></div>");
    var previewUrl = finstance.Preview(true); //onlyURL true;
    var icon = previewUrl !== null ? $("<i class='fa'><img style='width:24px; height:24px;' src='" + previewUrl + "'/></i>") : $("<i class='fa fa-file'></i>");
    itemDiv.append(a);
    a.append(fileBarIcon);
    fileBarIcon.append(icon);
    var fileBarInfo = $("<div class='file-bar-info'></div>");
    var fileLabel = $("<h5 style='overflow:hidden;width:110px;'></h5>");
    var trash = $("<span></span>");
    var trashIcon = $("<i class='fa fa-trash-o'>X</i>")
    trash.append(trashIcon);
    trashIcon.bind("click", function () {
        finstance.Cancel(); //Upload Abort
        finstance.Remove();

    });
    fileLabel.text(finstance.FileName);
    fileBarInfo.append(fileLabel);
    fileBarInfo.append(trash);
    var percentDiv = $("<div style='width:0%;height:100%;background-color:#f2f2f2'></div>");
    fileBarInfo.append(percentDiv);
    a.append(fileBarInfo);
    finstance.StatusChanged = function (percent, speedText) {
        //_fileModel.Percent(percent);
        //_fileModel.Speed(speedText);
        console.log("percent: " + percent);
        console.log("Speed: " + speedText);
    };
    finstance.uploadAborted = function (uploaderInstance,fileInstance)
    {
        console.log("File Aborted");
        console.log(fileInstance.CurrentOptions);
    }
    return itemDiv;
   
}

function UploaderFileRemoved(uploaderInstance, finstance) {
    //viewModel.FileViewModel().Remove(finstance.UniqueIdentifier);
}
function ClearUploader(messageClientID)
{
    uploader.Clear(messageClientID);
}
function SelectFiles(messageClientID) {
    if (uploader) {
        var parameters = { action: '/Chat/UploadFile', singleFile: false, group: messageClientID, browseComplate: function (options) { console.log(options); }, allFilesUploaded: function (groupName, options) { alert("Dosyalar Tamamlandı..") }, uploadFinished: function (instance, file, data) { $('#thumbImageThumb' + file.CurrentOptions.parameters.id).attr('src', data.imagePath + '?' + Math.random()); }, createItemFunction: CreateFileItem, fileRemoved: UploaderFileRemoved, containerElementID: 'fileContainer', parameters: { MessageID: null }, ResumeCheckUrl: '/Chat/ExistsFileLength' };
        uploader.Browse(parameters);
    }
}

function StartUpload(messageClientID, MessageID, allFilesUploadedCallback, uploadErrorCallback)
{
    
    var params = { MessageID: MessageID || "0000000-0000-0000-0000-000000000000" };
    
    uploader.ChangeGroupParemeters(messageClientID, {
        parameters: params, allFilesUploaded: allFilesUploadedCallback || function (groupName, options) {
            console.log("Dosyaların yüklenmesi tamamlandı.");
        },
        uploadError:uploadErrorCallback || function (uploaderInstance, finstance, error) {
            console.log("Dosya yüklenirken hata oluştu!!!")
        }
    });
    var upResult = uploader.Upload(messageClientID);
    if (upResult) {console.log("Dosyaların yüklenmesi başlatıldı.");}
}