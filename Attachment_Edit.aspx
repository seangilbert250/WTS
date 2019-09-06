﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Attachment_Edit.aspx.cs" Inherits="Attachment_Edit" enableViewState="true" Theme="Default"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Add Attachment</title>
    <script type="text/javascript" src="scripts/common.js"></script>
    <script type="text/javascript" src="scripts/shell.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<style type="text/css">
		select {
			background-color: #F5F6CE;
		}
	</style>
</head>
<body>
    <form id="form1" runat="server">  

    <div class="pageContainer" style="overflow-y:auto;">
        <asp:Label runat="server" id ="lblError"></asp:Label>
		<div id="divDetails" class="attributesValue" style="padding: 10px 0px 10px 20px;">
			<table id="tblAttachment" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
				<tr id="trFile" class="attributesRow">
					<td class="attributesRequired">*</td>
					<td id="tdFile" class="attributesLabel" style="width: 100px;">File:</td>
					<td id="fileUploader" class="attributesValue">
						<asp:FileUpload runat="server" ID="fileUpload1" AllowMultiple="true"  style="width:98%;" />
					</td>
				</tr>
				<tr id="trSingleAttachmentType" class="attributesRow" style="display: none;">
					<td class="attributesRequired">*</td>
					<td class="attributesLabel" style="width: 100px;">Attachment Type:</td>
					<td class="attributesValue">
						<asp:DropDownList ID="ddlAttachmentType" runat="server" style="width:205px;"></asp:DropDownList>
					</td>
				</tr>
				<tr id="trSingleFileDesc" class="attributesRow">
					<td class="attributesRequired" style="vertical-align:top;">&nbsp;</td>
					<td class="attributesLabel" style="width: 100px; vertical-align:top;">File Description:</td>
					<td class="attributesValue" style="vertical-align: top;">
						<asp:TextBox ID="txtDescription" runat="server" textMode="MultiLine" style="width:97%;"></asp:TextBox>
					</td>
				</tr>
			</table>
		</div>
        <div id="divGrid">
            <iti_Tools_Sharp:Grid ID="grdFiles" runat="server" AllowPaging="false" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
				CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf"></iti_Tools_Sharp:Grid>
        </div>
        
         <div id="pageCover" style="position:absolute; top:0px; left:0px; width:100%; height:100%; background-color:gainsboro; filter:alpha(opacity=50); -moz-opacity:0.5; -khtml-opacity: 0.5; opacity: 0.5; display:none;"></div>
    </div>

    <div id="hiddenFields" style="display:none;">
        <input id="inpHide" type="hidden" runat="server" />  
        <asp:Button ID="btnSubmit" runat="server" />
    </div>
    <div id="divLoading" style="position:absolute; left:38%; top:30%; padding:10px; background:white; border:solid 1px grey; font-size:18px;  text-align:center; display:block;">
        <table>
            <tr>
                <td>
                    WTS is Gathering Data...  Please wait...
                </td>
            </tr>
            <tr>
                <td>
                    <img alt="Loading" src="Images/loaders/progress_bar_blue.gif" />   
                </td>
            </tr>
        </table>
    </div>
    <div id="pageFooter" class="PopupFooter">
        <table cellpadding="0" cellspacing="0" style="width:100%; height:100%; ">
            <tr>
                <td style="width:100%; text-align:right;">
                    <span id="spnFileSize" style="padding-right: 25px;"></span>
                    <input type="button" id="btnClose" value="Close" style="cursor:pointer; float:right;"  />
                    <input type="button" id="btnAdd" value="Save" style="cursor:pointer; float:right;"  />
                    <input type="button" id="btnSave" value="Save" style="cursor:pointer; float:right;"  />
                </td>
            </tr>
        </table>
    </div>
   

    <div id="DivHidden" style="display: none;">
        <asp:TextBox ID="txtSelectedRow" runat="server"></asp:TextBox>
        <asp:TextBox ID="txtAttachmentID" runat="server"></asp:TextBox>
    </div>
    <script src="Scripts/jquery.json-2.4.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var btnSubmit = document.getElementById('btnSubmit');
        var txtDescription = document.getElementById('txtDescription');
        var ddlAttachmentType = document.getElementById('ddlAttachmentType');
        var txtModification = document.getElementById('txtModification');
        var file = document.getElementById('fileUpload1');
        var trOrigFile = document.getElementById('trOrigFile');
        var tdFile = document.getElementById('tdFile');
        var tblAttachment = document.getElementById('tblAttachment');
        var pageFooter = document.getElementById('pageFooter');
        var fileUpload1 = document.getElementById('fileUpload1');
        var isEdit = ('<%=this.Edit.ToString().ToUpper() %>' == "TRUE");
        var rowIndex = null;
        var hasUploaded = '<%=HasUploaded %>';
        var backupExists = false;

        function btnSave_Onclick() {
            positionDivLoading();
            $('#divLoading').show();
            $('#pageFooter').hide();
            $('#pageCover').show();
            var blnExit = false;
            if (fileUpload1.files) {
            	if (fileUpload1.files.length > 1) {
                    var arrFiles = [];
                    $('#grdFiles_Grid tr:gt(1)').each(function (index, file) {
                        if ($(file).find('td:eq(1) select').val() == '0' && '<%=Module.ToUpper() %>' != 'MDIMAGE') {
                            blnExit = true;
                            return false;
                        }
                        var arrFileAttr = [];
                        arrFileAttr.push('"name":"' + $(file).find('td:eq(0) input').val() + '"');
                        arrFileAttr.push('"type":"' + $(file).find('td:eq(1) select').val() + '"');
                        arrFileAttr.push('"size":"' + $(file).find('td:eq(2)').text() + '"');
                        arrFileAttr.push('"description":"' + $(file).find('td:eq(3) textarea').val() + '"');
                        arrFiles.push($.parseJSON('{' + arrFileAttr.join(',') + '}'));
                    });
                    if (blnExit) {
                        MessageBox('Please select an Attachment Type...');
                        divLoading.style.display = 'none';
                        pageFooter.style.display = 'block';
                        pageCover.style.display = 'none';
                        return;
                    }
                    var strFiles = $.toJSON(arrFiles);
                    $('#inpHide').val(strFiles);
                } else {
                    if (file.value == '' && isEdit != 1 && rowIndex == null) {
                        MessageBox('Please select a File to Upload...');
                        file.focus();
                    }

                    if (ddlAttachmentType.value == 0 && ('<%=Module.ToUpper() %>' != 'MDIMAGE') && ('<%=Module.ToUpper() %>' != 'NEWS')) {
                        MessageBox('Please select an Attachment Type...');
                        divLoading.style.display = 'none';
                        pageFooter.style.display = 'block';
                        pageCover.style.display = 'none';
                        return;
                    }
                }
                btnSubmit.click();
            } else {
                if (file.value == '' && isEdit != 1 && rowIndex == null) {
                    MessageBox('Please select a File to Upload...');
                    file.focus();
                }

                if (ddlAttachmentType.value == 0 && '<%=Module.ToUpper() %>' != 'MDIMAGE') {
                    MessageBox('Please select an Attachment Type...');
                    divLoading.style.display = 'none';
                    pageFooter.style.display = 'block';
                    pageCover.style.display = 'none';
                    return;
                }
                btnSubmit.click();
            }
        }

        function positionDivLoading() {
            var left = (document.body.clientWidth / 2) - (divLoading.clientWidth / 2);
            var top = (document.body.clientHeight / 2) - (divLoading.clientHeight / 2);

            divLoading.style.left = left + 'px';
            divLoading.style.top = top + 'px';
        }

        function refreshPage() {
        	window.location.href = 'Loading.aspx?Page=' + window.location.href;
        }
    
        function populateUploadGrid(obj) {
            var totalFileSize = 0;
            if (obj.files) {
                if (obj.files.length > 1) {
                    $('#divGrid').show();
                    $('#trSingleAttachmentType').hide();
                    $('#trSingleFileDesc').hide();
                    $('#grdFiles_Grid').find('tr:gt(0)').empty();

                    $(obj.files).each(function (index, file) {
                        var filesize = Math.round((file.size / 1024 / 1024) * 100) / 100;
                        totalFileSize += filesize;
                        var $tr = $('<tr>').addClass("gridBody");
                        var $tdFileName = $('<td>').append($('<input disabled>').attr('type', 'textbox').attr('id', 'txtFileName' + index).val(file.name));
                        var $tdType = $('<td' + ('<%=Module.ToUpper() %>' == 'MDIMAGE' ? ' style="display: none;"' : '') + '>').append($('#ddlAttachmentType').clone());
                        var $tdSize = $('<td>').text(filesize + ' MB');
                        var $tdDesc = $('<td>').append($('#txtDescription').clone());
                        if (file.type.indexOf("image") != -1) $tdType.find('select').val(11); //GRAPHICS
                        if (file.type.indexOf("pdf") != -1) $tdType.find('select').val(10); //DOCUMENTS
                        $tr.append($tdFileName);
                        $tr.append($tdType);
                        $tr.append($tdSize);
                        $tr.append($tdDesc);
                        $tr.find('input, textarea, select').css('width', '98%');
                        $('#grdFiles_Grid').append($tr);
                    });
                    totalFileSize = Math.round((totalFileSize) * 100) / 100;
                    if (totalFileSize > 20) {
                        $('#spnFileSize').text('Warning! Your total upload size is ' + totalFileSize + ' MB, it may take a while to complete this upload.').css('color', 'red');;
                    } else {
                        $('#spnFileSize').text('Total upload size: ' + totalFileSize + ' MB').css('color', 'black');;
                    }
                    $('#grdFiles_Grid').find('td:eq(1)').css('width', '100px');
                    grdFiles.HeightModifier = 25;
                    grdFiles.RedrawGrid();
                } else {
                    $('#divGrid').hide();
                    if (('<%=Module.ToUpper() %>' != 'MDIMAGE') && ('<%=Module.ToUpper() %>' != 'NEWS')) {
                        $('#trSingleAttachmentType').show();
                    }
                    $('#trSingleFileDesc').show();
                }
            } else {
                $('#divGrid').hide();
                if (('<%=Module.ToUpper() %>' != 'MDIMAGE') && ('<%=Module.ToUpper() %>' != 'NEWS')) {
                    $('#trSingleAttachmentType').show();
                }
                $('#trSingleFileDesc').show();
            }
            
        }

        $('#<%=this.fileUpload1.ClientID %>').on("change", function () {
            //No File Chosen / Cancel clicked: Show original File Select.
            if ($('#<%=this.fileUpload1.ClientID %>').val().length <= 0) {
                if (backupExists) {
                    chromeRestoreBackup();
                }
            }
        });

        $('#<%=this.fileUpload1.ClientID %>').on("click", function () {
            var filechooser = $('#<%=this.fileUpload1.ClientID %>');
            if (filechooser.val().length > 0) {
                backupExists = $('#<%=this.fileUpload1.ClientID %>').clone();
            }
        });
        function chromeRestoreBackup() {
            $('#<%=this.fileUpload1.ClientID %>').remove(); // Remove Old fileuploader 
            backupExists.prependTo($('#fileUploader')); // Append New fileuploader

            // Assign functions to the new fileuploader
            $('#<%=this.fileUpload1.ClientID %>').on("change", function () {
                // No File Chosen / Cancel clicked: Show original File Select.
                if ($('#<%=this.fileUpload1.ClientID %>').val().length <= 0) {
                    if (backupExists) {
                        chromeRestoreBackup();
                    }
                }
            });
            $('#<%=this.fileUpload1.ClientID %>').on("click", function () {
                var filechooser = $('#<%=this.fileUpload1.ClientID %>');
                if (filechooser.val().length > 0) {
                    backupExists = $('#<%=this.fileUpload1.ClientID %>').clone();
                }
            });
        }

        $(document).ready(function () {
        	$(':input').css('font-family', 'Arial');
        	$(':input').css('font-size', '12px');

            $('#trSingleAttachmentType').hide();
            $('#trSingleFileDesc').hide();
            $('#divLoading').hide();
            $('#divGrid').hide();
            var dontSetSizeFlag = false;
            if (hasUploaded == 1) {
                successMessage('Uploaded Successfully');
                if ('<%=Module.ToUpper() %>' == 'MDIMAGE') {
                    opener.refreshPage();
                }
                else if (opener.refreshGrid) {
                    opener.refreshGrid();
                }
                setTimeout(closeWindow, 10);
                return false;
            }
            $('#tdFile').text('File:');
            $('#btnSave').hide();
            $('#divLoading').hide();
            $('#pageFooter').show();
            $('#pageCover').hide();
            $('#btnClose').click(function () { closeWindow(); });
            $('#btnAdd').click(function () { btnSave_Onclick(); });
            $('#btnSave').click(function () { btnSave_Onclick(); });
            if (('<%=Module.ToUpper() %>' != 'MDIMAGE') && ('<%=Module.ToUpper() %>' != 'NEWS')){
                $('#trSingleAttachmentType').show();
            }
            $('#trSingleFileDesc').show();

            $('#fileUpload1').change(function () { populateUploadGrid(this); });
            if ($('[id*=txtAttachmentID]').val() != '0') $(tdFile).text('Replace File:');

        });

	</script>
    </form>
</body>
</html>

