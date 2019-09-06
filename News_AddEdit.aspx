﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="News_AddEdit.aspx.cs" Inherits="News_AddEdit" Theme="Default" EnableEventValidation = "false"  %>

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
        <div style="height: 98%">
            <table cellpadding="0" cellspacing="0" style="padding: 10px;">
                <tr id="trArticle">
                    <td style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                    </td>
                    <td style="padding-bottom: 10px">
                        <asp:Label ID="lblArticleTitle" runat="server" Text="Article Title: "></asp:Label>
                    </td>
                    <td style="padding-bottom: 10px">
                        <asp:TextBox ID="txtArticleTitle" MaxLength="300" runat="server" Width="300"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                    </td>
                    <td style="padding-right: 10px; padding-bottom: 10px">
                        <asp:Label ID="lblNotificationType" runat="server" Text="Notification Type: "></asp:Label>
                    </td>

                    <td style="padding-bottom: 10px">
                        <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server"></asp:UpdatePanel>
                            <asp:DropDownList ID="ddlNotificationType" runat="server">
                            </asp:DropDownList>
                        
                        
                    </td>

                </tr>
                <tr>
                    <td style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                    </td>
                    <td id="tdLblStartDate" style="display: block;">
                        <asp:Label ID="lblStartDate" runat="server" Text="Start Date"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDateStart" runat="server" Font-Size="8pt" Width="100px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                    </td>
                    <td id="tdLblEndDate" style="display: block;">
                        <asp:Label ID="lblEndDate" runat="server" Text="End Date"></asp:Label>
                    </td>
                    <td id="tdDPEndDate">
                        <asp:TextBox ID="txtDateEnd" runat="server" Font-Size="8pt" Width="100px"></asp:TextBox>
                    </td>
                    <td id="tdAttachments">
                        <table>
                            <tr>
                                <%--<td id="tdRequiredAttachments" style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                                </td>--%>
                                <td id="tdLblAttachments" style="display: block;">
                                    <asp:Label ID="lblAttachments" runat="server" Text="Attachments"></asp:Label>
                                </td>
                                <td>
                                    <%--<asp:FileUpload ID="fileUpload" runat="server" Width="100%" AllowMultiple="True" />--%>
                                </td>
                            </tr>
                        </table>
                    </td>

                </tr>
                <tr style="display: none;" id="trTimeZone">
                    <td style="text-align: right; color: Red; padding-right: 5px; padding-bottom: 10px">* 
                    </td>
                    <td style="padding-bottom: 10px">
                        <asp:Label ID="lblTimeZone" runat="server" Text="Time Zone"></asp:Label>
                    </td>
                    <td style="padding-bottom: 10px">
                        <asp:DropDownList ID="ddlTimeZone" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="padding-bottom: 10px" style="padding-bottom: 10px"></td>
                    <td>
                        <asp:Label ID="lblActive" runat="server" Text="Active"></asp:Label>
                        <asp:CheckBox ID="cbActive" runat="server" Checked="true" />
                    </td>
                    <td style="padding-bottom: 10px"></td>
                </tr>
            </table>

            <div id="divNewsBodyContainer" class="attributesRow" style="vertical-align: top;">
                <div id="divNewsBodyHeader" class="pageContentHeader" style="padding: 5px;">
                    <div class="attributesLabel" style="padding-left: 5px; display: inline;">
                        Body:
                    </div>
                    <span id="labelMessage" style="color: red; padding-left: 15px;"></span>
                </div>
                <div id="divNewsBody" class="attributesValue" style="padding: 10px 0px 10px 20px;">
                    <textarea id="textNewsBody" runat="server" rows="12" style="width: 98%;"></textarea>
                </div>
            </div>
            <div>
                <iframe id="frameNewsAttachments" src="javascript:'';" frameborder="0" scrolling="no" style="display: block; width: 100%;"></iframe>
                <%--Grid:
                <iti_Tools_Sharp:Grid ID="gridAttachments" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		            CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	            </iti_Tools_Sharp:Grid>--%>
            </div>

            <div style="display: none;">
                <asp:TextBox ID="txtNews" runat="server"></asp:TextBox>
            </div>
        </div>
        <div id="popupFooter" class="PopupFooter">
            <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                <tr>
                    <td style="width: 100%; text-align: right;">
                        <input type="button" id="btnOther" class="button" style="width: 60px; height: 24px;" value="other" />
                        <input type="button" id="btnSave" class="button" style="width: 60px; height: 24px;" onmouseover="this.className = 'buttonOver';" onmouseout="this.className = 'button';" onmousedown="this.className = 'buttonClick';" onmouseup="this.className = 'buttonOver';" value="Save" />
                        <asp:Button ID="btnSubmit" runat="server" Style="display: none;" />
                        <button id="btnCancel" class="button" onmouseover="this.className = 'buttonOver';" onmouseout="this.className = 'button';" onmousedown="this.className = 'buttonClick';" onmouseup="this.className = 'buttonOver';" onclick="closeThisWindow(); return false;">
                            <div>Close</div>
                        </button>
                    </td>
                </tr>
            </table>
        </div>
        <div id="pageDimmer" style="position: absolute; top: 0px; left: 0px; width: 100%; height: 100%; background-color: grey; filter: alpha(opacity = 60); display: none;"></div>
        <div id="divSaving" style="width: 250px; position: absolute; left: 30%; top: 30%; background-color: white; border: 1px solid #000000; display: none;">
            <table style="width: 100%;">
                <tr>
                    <td style="height: 30px; vertical-align: middle; padding-left: 10px;">
                        <img src="images/loaders/loader_2.gif" alt="Saving" />
                    </td>
                    <td id="tdMessage" style="vertical-align: middle; white-space: nowrap; height: 100%; width: 98%;">Saving...
                    </td>
                </tr>
            </table>
        </div>

        

        <script type="text/javascript">
            var _newsId = -1;
            var _newsTypeID;
            var _editType;
            var _serverAttributes;
            //var _idxID = 0, _idxFileName = 0, _idxAttachmentType = 0, _idxTitle = 0;

            function complete() {
                //var blnSaved = false;
                //var errorMsg = '';

                //var obj = $.parseJSON(result);

                //if (obj) {
                //    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                //    if (obj.error) errorMsg = obj.error;
                //}

                //if (blnSaved) {
                    MessageBox('Your News Has Been Submitted!! :)');
                    opener.refreshGrid();
                    closeWindow();
                //}
                //else {
                //    MessageBox('Failed to submit. <br>' + errorMsg);
                //}

            }

            function lbEditAttachment_click(attachmentID) {
			<%--var url = '';
			//var _selectedId == attachmentID;

			try {
				var fileName = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxFileName].innerText;

				var selAttachmentType = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxAttachmentType].innerText;
				var selDescription = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxTitle].innerText;

				url = 'Attachment_Edit.aspx' 
					+ window.location.search 
					+ '&edit=1' 
					+ '&attachmentID=' + attachmentID 
					+ '&fileName=' + fileName 
					+ '&attachmentType=' + selAttachmentType 
					+ '&description=' + selDescription 
					+ '&random=' + new Date().getTime();

				var popup = popupManager.AddPopupWindow('EditAttachment', 'Edit Attachment - [' + fileName + ']'
					, 'Loading.aspx?Page=' + url, 200, 550, 'PopupWindow', window.self);
				
				if (popup) {
					popup.Open();
				}
			} catch (e) { }--%>
		}
		
		function buttonDownload_onclick(attachmentId) {
			try{
				window.open('Download_Attachment.aspx?attachmentID=' + attachmentId);
            }
			catch(e){ }
		}

            function closeThisWindow() {
                popupManager.ActivePopup.Close();
            }

            function validate() {

                var objNews = {};// new NewsItem();

                var descr = encodeURIComponent($($('#textNewsBody').cleditor()[0].doc.body)[0].innerHTML);

                objNews.ArticleTitle = $('#<%=this.txtArticleTitle.ClientID %>').val();
                objNews.NotificationType = parseInt($('#<%=this.ddlNotificationType.ClientID %> option:selected').val());
                objNews.StartDate = $('#<%=this.txtDateStart.ClientID %>').val();
                objNews.EndDate = $('#<%=this.txtDateEnd.ClientID %>').val();
                objNews.Active = $('#<%=this.cbActive.ClientID %>').prop('checked');
                <%--objNews.Attachment = $('#<%=this.fileUpload.ClientID %>')[0].value;--%>
                objNews.Description = descr;



                var validationHeader = '<table><tr><td colspan="2" style="font-weight: bold">Required fields missing:</td></tr>';
                var validationBody = '';

                <%--if (_newsTypeID == <%= (int)WTS.Enums.NewsTypeEnum.NewsOverview%>) {
                    objNews.ArticleTitle = "AUTOGENERATED";
                    objNews.Description = "AUTOGENERATED";
                    if (objNews.Attachment == '') { validationBody += '<tr><td style="text-align: right; color: Red; padding-right: 5px;">*</td><td>Attachment needed.</td></tr>'; }
                }--%>
                if (objNews.ArticleTitle == '') { validationBody += '<tr><td style="text-align: right; color: Red; padding-right: 5px;">*</td><td>Article Title needed.</td></tr>'; }
                if (objNews.NotificationType == -1) { validationBody += '<tr><td style="text-align: right; color: Red; padding-right: 5px;">*</td><td>Notification Type needed.</td></tr>'; }
                if (objNews.StartDate == '') { validationBody += '<tr><td style="text-align: right; color: Red; padding-right: 5px;">*</td><td>Start Date needed.</td></tr>'; }
                if (objNews.EndDate == '') { validationBody += '<tr><td style="text-align: right; color: Red; padding-right: 5px;">*</td><td>End Date needed.</td></tr>'; }
                if (validationBody.length > 0) {
                    validationBody += '</table>';
                    MessageBox(validationHeader + validationBody);
                    return 'INVALID';
                }

                return objNews;

            }



            function txt_change(sender) {
                clearMessage();

                var original_value = '', new_value = '';
                if ($(sender).attr('original_value')) {
                    original_value = $(sender).attr('original_value');
                }

                new_value = $(sender).val();

                if (new_value != original_value) {
                    //activateSaveButton();
                }
            }

            function clearMessage() {
                $('#labelMessage').text('');
                $('#labelMessage').hide();
            }

            function btnOther_Click() {
                var a = 2;

            }


            function btnSave_click() {
                try {
                    var objNews = validate();

                    if (objNews != 'INVALID') {
                        ShowDimmer(true, 'Submitting...', 1);
                        $('#<%=this.txtArticleTitle.ClientID %>')[0].value = encodeURIComponent($('#<%=this.txtArticleTitle.ClientID %>').val());
                        $('#textNewsBody')[0].value = encodeURIComponent($($('#textNewsBody').cleditor()[0].doc.body)[0].innerHTML);
                        $('#<%=this.btnSubmit.ClientID %>').trigger('click');

                    }
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }

            function on_error(result) {
                var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

                MessageBox('save error:  \n' + resultText, 'Server Error');
            }

            function setEditControls(serverAttributes) {
                //debugger;
                $('#txtArticleTitle')[0].value = decodeURIComponent(serverAttributes.txtArticleTitle);
                $('#<%=this.ddlNotificationType.ClientID %> option:selected').val(serverAttributes.ddlNotificationType);
                $('#<%=this.txtDateStart.ClientID %>').val(serverAttributes.txtDateStart);
                $('#<%=this.txtDateEnd.ClientID %>').val(serverAttributes.txtDateEnd);
                $('#<%=this.cbActive.ClientID %>').prop('checked', serverAttributes.Bln_Active);
                $('#<%=this.cbActive.ClientID %>').prop('checked', serverAttributes.Bln_Active);
                //$($('#textNewsBody').cleditor()[0].doc.body)[0].innerHTML = decodeURIComponent(serverAttributes.textNewsBody);
                $($('#textNewsBody').cleditor()[0].doc.body)[0].innerHTML = decodeURIComponent(serverAttributes.textNewsBody);
                <%--$('#<%=this.fileUpload.ClientID %>')[0].value = decodeURIComponent(serverAttributes.fileUpload);--%>
            }

            function loadNewsAttachments() {
			    //var url = window.location.search;
			    //url = editQueryStringValue(url, 'Saved', '0');
                var url = 'Loading.aspx?Page=NewsAttachments.aspx?newsID=' + _newsId;
                //debugger;
                $('#frameNewsAttachments').attr('src', url);
                
		    }

            $(document).ready(function () {
                $('#txtDateStart').datepicker();
                $('#txtDateEnd').datepicker();

                var w = '99%';// $('#<%=this.textNewsBody.ClientID %>').width();
                var h = 150;
                if (($('#<%=this.textNewsBody.ClientID %>').height() + 30) > 150) {
                    h = $('#<%=this.textNewsBody.ClientID %>').height() + 30;
                };

                //debugger;
                _newsId = '<%= this._newsID %>';
                _newsTypeID = '<%= this._newsTypeID %>';
                _serverAttributes = '<%= this.JsonSeverAttributes %>';
                _editType = '<%= this._editType %>';
                $('#<%=this.textNewsBody.ClientID %>').cleditor({ width: w, height: h });

                $('#<%=this.textNewsBody.ClientID %>').cleditor()[0].change(function () { txt_change($('#<%=this.textNewsBody.ClientID %>')); });
                $($('#<%=this.textNewsBody.ClientID %>').cleditor()[0].doc).css('height', (h - 28) + 'px');
                $($('#<%=this.textNewsBody.ClientID %>').cleditor()[0].doc.body).css('height', (h - 26) + 'px');
                $($('#<%=this.textNewsBody.ClientID %>').cleditor()[0].doc.body).css('background-color', '#F5F6CE');

                if (_editType == 'edit') {
                    setEditControls(JSON.parse(_serverAttributes));
                }

                $('#btnSave').click(function () { btnSave_click(); return false; });
                $('#btnOther').click(function () { btnOther_click(); return false; });
               <%-- _idxID = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTID"].Ordinal%>';
				_idxFileName = '<%=this.dc == null ? 0 : this.dc["FILENAME"].Ordinal%>';
				_idxAttachmentType = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTTYPE"].Ordinal%>';
                _idxTitle = '<%=this.dc == null ? 0 : this.dc["TITLE"].Ordinal%>';--%>
                loadNewsAttachments();
                //debugger;
               <%-- if (_newsTypeID == <%= (int)WTS.Enums.NewsTypeEnum.NewsArticle%>) {
                    $('#<%=this.fileUpload.ClientID %>').hide();
                    $('#tdAttachments').hide();
                }
                else {
                    //If this is a newsOverviewType then these fields are auto-generated
                    $('#divNewsBodyContainer').hide();
                    $('#trArticle').hide();
                }--%>
            });
        </script>

    </form>
</body>
</html>
