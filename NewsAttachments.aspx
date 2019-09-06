<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewsAttachments.aspx.cs" Inherits="NewsAttachments" Theme="Default" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
    <link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
    <script src="Scripts/cleditor/jquery.cleditor.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <div>
            <input type="button" id="btnAdd" value="Add" />
            <input type="button" id="btnDelete" value="Delete" />
        </div>
        <div>
            <iti_Tools_Sharp:Grid ID="gridAttachments" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
                CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
            </iti_Tools_Sharp:Grid>
        </div>

        <script type="text/javascript">
            var _newsId = -1;
            var _newsTypeID;
            var _editType;
            var _serverAttributes;
            var _idxID = 0, _idxFileName = 0, _idxAttachmentType = 0, _idxTitle = 0;
            var _newsAttachmentId = 0;

            function lbEditAttachment_click(attachmentID) {
                var url = '';
                //var _selectedId == attachmentID;

                try {
                    var fileName = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxFileName].innerText;

                var selAttachmentType = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxAttachmentType].innerText;
                var selDescription = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxTitle].innerText;

                    url = 'Attachment_Edit.aspx'
                        //+ window.location.search
                        + '?newsID=' + <%= this._newsID%>
                        + '&Module=News' 
                        + '&edit=1'
                        + '&attachmentID=' + attachmentID
                        + '&fileName=' + fileName
                        + '&attachmentType=' + selAttachmentType
                        + '&description=' + selDescription
                        + '&attachmentTypeID=' + '5'
                        + '&random=' + new Date().getTime();

                    var popup = popupManager.AddPopupWindow('EditAttachment', 'Edit Attachment - [' + fileName + ']'
                        , 'Loading.aspx?Page=' + url, 200, 550, 'PopupWindow', window.self);

                    if (popup) {
                        popup.Open();
                    }
                } catch (e) { }
            }

            function refreshGrid() {
			    document.location.href = 'Loading.aspx?Page=' + document.location.href;
		    }

            function buttonDownload_onclick(attachmentId) {
                try {
                    window.open('Download_Attachment.aspx?attachmentID=' + attachmentId);
                }
                catch (e) { }
            }

            function buttonAdd_onclick() {
                try {
                    //debugger;
                   <%-- var selAttachmentType = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxAttachmentType].innerText;--%>
                    
                    var url = 'Attachment_Edit.aspx'
                        //+ window.location.search
                        + '?newsID=' + <%= this._newsID%>
                        + '&random=' + new Date().getTime()
                        + '&Module=News'
                        + '&attachmentTypeID=' + '5'
                        + '&attachmentType=News';//+ selAttachmentType;

                    var filterPopup = popupManager.AddPopupWindow('AddNewAttachment', 'Add New Attachment'
                        , 'Loading.aspx?Page=' + url, 300, 600, 'PopupWindow', window.self);
                    if (filterPopup) {
                        filterPopup.Open();
                    }
                }
                catch (e) { }
            }
            function btnDelete_click() {
                if (_newsAttachmentId == null || _newsAttachmentId == 0) {
                    MessageBox('You must select a file to delete first.');
                }
                else {
                    var selectedFileName = $(<%=this.gridAttachments.ClientID %>.SelectedRow).find('td:eq(' + _idxFileName + ')').text();

                    if (confirm('Are you sure you want to delete: ' + selectedFileName + '?')) {
                        PageMethods.DeleteAttachment(_newsAttachmentId
                            , deleteAttachment_Done, OnError);
                    }
                }
            }
            function deleteAttachment_Done(result) {
                var deleted = false;
                var id = '', errorMsg = '';

                try {
                    var obj = jQuery.parseJSON(result);

                    if (obj) {
                        if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
                            deleted = true;
                        }
                        if (obj.id) {
                            id = obj.id;
                        }
                        if (obj.error) {
                            errorMsg = obj.error;
                        }
                    }

                    if (deleted) {
                        MessageBox('Attachment has been deleted.');
                        refreshGrid();
                    }
                    else {
                        MessageBox('Failed to delete attachment. \n' + errorMsg);
                    }
                } catch (e) {

                }
            }
            function OnError(result) {
                var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

                MessageBox('save error:  \n' + resultText);
            }


            function row_click(row) {
                if ($(row).attr('AttachmentID')) {
                    _newsAttachmentId = $(row).attr('AttachmentID');
                }

                //if (_canEdit) {
                //    $('#btnDelete').attr('disabled', false);
                //    $('#btnDelete').prop('disabled', false);
                //}
            }

            $(document).ready(function () {
                _idxID = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTID"].Ordinal%>';
                _idxFileName = '<%=this.dc == null ? 0 : this.dc["FILENAME"].Ordinal%>';
                _idxAttachmentType = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTTYPE"].Ordinal%>';
                _idxTitle = '<%=this.dc == null ? 0 : this.dc["TITLE"].Ordinal%>';

                $('.gridBody').click(function (event) { row_click(this); });
			    $('.selectedRow').click(function (event) { row_click(this); });

                $('#btnAdd').click(function (event) { buttonAdd_onclick(); });
                //debugger;
                $('#btnDelete').click(function (event) { btnDelete_click(); });

            });
        </script>
    </form>
</body>
</html>
