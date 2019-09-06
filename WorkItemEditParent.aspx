﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkItemEditParent.aspx.cs" Inherits="WorkItemEditParent" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Review/Edit Task</title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divPage" class="pageContainer">
			<div id="divWorkItemHeader" class="pageContentHeader" style="font-size: 13px;">
				<table cellpadding="0" cellspacing="0" style="width: 100%;">
					<tr>
						<td style="text-align: left; padding-left: 4px; font-size: 14px; height: 30px;">Primary Task - <span id="labelWorkItemNumber" runat="server" style="font-size: 14px;"></span>
						</td>
						<td style="text-align: right; width: 70px;">
							<input type="button" id="buttonAdd" value="Add New" style="padding: 1px 2px 1px 2px; width: 65px; height: 20px;" disabled="disabled" />
						</td>
						<td style="text-align:right; width: 47px;">
							<input type="button" id="buttonCopy" value="Copy" style="padding: 1px 2px 1px 2px; width: 42px; height: 20px;" disabled="disabled" />
						</td>
						<td style="text-align: right; padding-right: 0px; width: 95px;">
							<input type="button" id="buttonBackToGrid" runat="server" value="Back to Grid" style="padding: 1px 2px 1px 2px; width: 88px; height: 20px;" />
						</td>
					</tr>
				</table>
			</div>
			<div id="divWorkItemHeaderButtons" class="pageContentInfo" style="height: auto; padding: 2px; border-bottom: 1px solid gainsboro;display:none;">
				<table cellpadding="0" cellspacing="0" style="width: 100%; text-align: left;">
					<tr>
						<td>
							<table cellpadding="0" cellspacing="0" style="white-space: nowrap;">
								<tr>
									<td style="text-align: left; padding-top: 2px;">
										<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="images/icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
									</td>
								</tr>
							</table>
						</td>
						<td style="text-align:right; white-space: nowrap;">
							<input type="button" id="buttonGoToWorkItem" value="Go to Work Task #" />
							<input type="text" id="txtWorkItem" name="GoTo" tabindex="5" maxlength="11" size="8" />
							&nbsp;
							<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" style="display: none;" />
							<input type="text" id="txtWorkRequest" name="GoTo" tabindex="4" maxlength="6" size="3" style="display: none;" />
                            &nbsp;&nbsp;&nbsp;
                            <input type="button" id="buttonPrevious" value="&nbsp;&nbsp;<<Prev&nbsp;&nbsp;&nbsp;" disabled="disabled" tabindex="2" style="width: 65px"/>
                            <input type="button" id="buttonNext" value="&nbsp;&nbsp;Next>>&nbsp;&nbsp;" disabled="disabled" tabindex="1" style="width: 65px"/>
							&nbsp;
						</td>
						<td style="text-align: right; white-space: nowrap;">
							<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
								<tr>
									<td style="display: none;">
										<input type="button" id="buttonSave" value="Save" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>

			<div id="divTabsContainer" class="mainPageContainer">
				<ul id="Tabs" runat="server">
					<li><a id="tabDetails" href="#divDetails" onclick="Tab_click('details');">Details</a></li>
					<li><a id="tabNotes" href="#divNotes" onclick="Tab_click('notes');">Notes</a></li>
					<li><a id="tabAttachments" href="#divAttachments" onclick="Tab_click('attachments');">Attachments</a></li>
					<li><a id="tabWRAttachments" href="#divRequestAttachments" onclick="Tab_click('requestattachments');" style="display: none;">Work Request Attachments</a></li>
				</ul>
				<div id="divDetails">
					<iframe id="frameDetails" name="frameDetails" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Primary Task Details</iframe>
				</div>
				<div id="divNotes">
					<iframe id="frameNotes" name="frameNotes" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Primary Task Notes</iframe>
				</div>
				<div id="divAttachments">
					<iframe id="frameAttachments" name="frameAttachments" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Primary Task Attachments</iframe>
				</div>
				<div id="divRequestAttachments">
					<iframe id="frameRequestAttachments" name="frameRequestAttachments" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Work Request Attachments</iframe>
				</div>
			</div>
		</div>

		<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
			<Services>
				<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
			</Services>
		</asp:ScriptManager>

		<script id="jsVariables" type="text/javascript">
			var _pageUrls;
			var _canEdit = false;
			var _workRequestID = 0, _workItemID = 0, _sourceWorkItemID = 0;
			var _commentQty = 0, _attachmentQty = 0, _wrAttachmentQty = 0;
			var nextItem = undefined; var previousItem = undefined; var UseLocal = false;
			var listItems = undefined;
            var _selectedAssigned = '<%=SelectedAssigned%>';
			var _selectedStatuses = '<%=SelectedStatuses%>';
		</script>

		<script id="jsAJAX" type="text/javascript">



			function verifyItemExists(itemID, taskNumber, type) {
                WorkloadWebmethods.ItemExists(itemID, taskNumber, type, function (result) { verifyItemExists_done(itemID, taskNumber, type, result); }, function (result) { verifyItemExists_done(itemID, taskNumber, type, false); });
			}

            function verifyItemExists_done(itemID, taskNumber, type, exists) {
				if (exists && exists.toUpperCase() == "TRUE") {
					switch (type) {
						case 'Primary Task':
							gotoWorkItem(itemID);
							break;
						case 'Work Request':
							gotoWorkRequest(itemID);
                            break;
                        case 'Subtask':
                            PageMethods.WorkItem_TaskID_Get(itemID, taskNumber, function (result) { editWorkTask(itemID, taskNumber, result); });
                            break;
					}
				}
                else {
                    if (taskNumber > -1) {
                        MessageBox('Could not find ' + type + ' # ' + itemID + '-' + taskNumber);
                    } else {
                        MessageBox('Could not find ' + type + ' # ' + itemID);
                    }
				}
			}

			function on_error(result) {
				var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

				MessageBox('save error:  \n' + resultText);
			}

			function initNextPrevious() {
			        if ('<%=this.UseLocal == true%>' === 'True'){
			            if (window.localStorage && typeof (Storage) !== "undefined") {
			                nextItem = parseInt(window.localStorage.getItem("nextItem"), 10);
			                previousItem = parseInt(window.localStorage.getItem("previousItem"), 10);
			                listItems = window.localStorage.getItem("itemList").split(",");
			                if (nextItem > -1 && nextItem <= listItems.length - 1) {
			                    $('#buttonNext').prop('disabled', false);
			                }
			                if (previousItem > -1) {
			                    $('#buttonPrevious').prop('disabled', false);
			                }
		                }
			            UseLocal = true;
                    }
			    }

			function gotoWorkItem(recordId) {
				if (parent.ShowFrameForWorkloadItem) {
                    parent.ShowFrameForWorkloadItem(false, recordId, recordId, true, UseLocal, _selectedStatuses, _selectedAssigned);
				}
				else {
					var url = '';
					url = _pageURLs.Maintenance.WorkItemEditParent
						+ '?WorkItemID=' + recordId;
                    if (popupManager.ActivePopup) popupManager.ActivePopup.SetTitle('PRIMARY TASK - [' + recordId + ']');
                    window.location.href = url;
				}
            }

            function editWorkTask(itemID, taskNumber, result) {
                var title = 'Subtask - [' + itemID + '-' + taskNumber + ']';
                var url = 'Loading.aspx?Page=' + _pageURLs.Maintenance.TaskEdit
                    + '?WorkItemID=' + itemID
                    + '&newTask=0'
                    + '&taskID=' + result;

                var h = 700, w = 850;

                var openPopup = popupManager.AddPopupWindow('WorkloadSubTask', title, url, h, w, 'PopupWindow', this);
                if (openPopup) {
                    openPopup.Open();
                }
            }

			function gotoWorkRequest(requestID) {
				if (parent.ShowFrameForWorkRequest) {
					parent.ShowFrameForWorkRequest(false, requestID, requestID, true, UseLocal);
				}
				else {
					var title = '', url = '';
					var h = 700, w = 1000;

					title = 'Work Request - [' + requestID + ']';
					url = _pageURLs.Maintenance.WorkRequestEditParent
						+ '?WorkRequestID=' + requestID;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
			}

		</script>

		<script id="jsEvents" type="text/javascript">

			function SetCommentQty(qty) {
				if (!qty) {
					qty = 0;
				}

				$('#tabNotes').text('Notes (' + qty + ')');
			}
			function SetAttachmentQty(qty) {
				if (!qty) {
					qty = 0;
				}
				
				$('#tabAttachments').text('Attachments (' + qty + ')');
			}
			function SetWRAttachmentQty(qty) {
				if (!qty) {
					qty = 0;
				}
				
				$('#tabWRAttachments').text('Work Request Attachments (' + qty + ')');
			}
			
			function continueMove(blnIsFromTabClick) {
				var cont = false;

				//TODO: check if need to change
				cont = true;

				return cont;
			}

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function resizePage() {
				try {
					var heightModifier = 0;
					heightModifier += 10; //$('#mainPageFooter').height();

					resizePageElement('divPage', heightModifier + 2);
					resizePageElement('divTabsContainer', heightModifier + 3);

					resizePageElement('<%=this.frameDetails.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameNotes.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameAttachments.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameRequestAttachments.ClientID%>', heightModifier + 12);

					resizePageElement('divDetails', heightModifier + 11);
					resizePageElement('divNotes', heightModifier + 11);
					resizePageElement('divAttachments', heightModifier + 11);
					resizePageElement('divRequestAttachments', heightModifier + 11);

				}
				catch (e) {
					var m = e.message;
				}
			}

			function Tab_click(tabName) {
				$('div', $('#divTabsContainer')).hide();

				switch (tabName.toUpperCase()) {
					case 'DETAILS':
						if ($('#<%=this.frameDetails.ClientID%>') && $('#<%=this.frameDetails.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameDetails.ClientID%>').attr('src', 'WorkItem_Details.aspx' + window.location.search);
						}
						$('#divDetails').show();

						break;
					case 'NOTES':
						if ($('#<%=this.frameNotes.ClientID%>') && $('#<%=this.frameNotes.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameNotes.ClientID%>').attr('src', 'WorkItem_Comments.aspx' + window.location.search);
						}
						$('#divNotes').show();

						break;
					case 'ATTACHMENTS':
						if ($('#<%=this.frameAttachments.ClientID%>') && $('#<%=this.frameAttachments.ClientID%>').attr('src') == "javascript:'';") {
							var url = 'WorkItem_Attachments.aspx' + window.location.search;
					    	$('#<%=this.frameAttachments.ClientID%>').attr('src', url);
					    }
						$('#divAttachments').show();

						break;
					case 'REQUESTATTACHMENTS':
						if ($('#<%=this.frameRequestAttachments.ClientID%>') && $('#<%=this.frameRequestAttachments.ClientID%>').attr('src') == "javascript:'';") {
							var url = 'WorkRequest_Attachments.aspx' + window.location.search + '&workRequestID=' + _workRequestID;
							$('#<%=this.frameRequestAttachments.ClientID%>').attr('src', url);
						}
						$('#divRequestAttachments').show();

						break;
				}

				resizePage();
			}


			function gotoId(id) {
				var contin = continueMove();
				if (!contin || contin == 'cancel') {
					return;
				}

				if (id != 0) {
					if (parent.ShowFrameForWorkItem) {
						var newWorkItem = false;
						var workItemId = id;
						var sourceWorkItemId = 0;
						var workItemNum = '';
						var sortValue = '';
						parent.ShowFrameForWorkItem(newWorkItem, workItemId, sourceWorkItemId, workItemNum, true);
					}
				}
			}
			function buttonFirst_click() {
				gotoId(+'<%=this.FirstWorkItemID %>');
			}
			function buttonPrevious_click() {
				gotoId(+'<%=this.PreviousWorkItemID %>');
            }
            function buttonNext_click() {
            	gotoId(+'<%=this.NextWorkItemID %>');
            }
            function buttonLast_click() {
            	gotoId(+'<%=this.LastWorkItemID %>');
            }

            function buttonBackToGrid_click() {
            	var contin = continueMove();
            	if (contin == 'cancel') {
            		return;
            	}

                if (parent.ShowFrameForGrid) {
                    parent.refreshChild();
            		parent.ShowFrameForGrid(false);
            	}
            	else if (parent.showFrameForGrid) {
            	    parent.showFrameForGrid('AOR', false);
            	}
            	else if (parent.ShowFrame) {
            	    parent.ShowFrame('Grid');
            	}
            	else if (closeWindow) {
            	    closeWindow();
            	}
            }


            function formatGoTo(obj) {
            	var text = $(obj).val();

            	if (/[^0-9-]|^0+(?!$)/g.test(text)) {
            		$(obj).val(text.replace(/[^0-9-]|^0+(?!$)/g, ''));
            	}
            }

            function buttonGoToWorkItem_click() {
            	var recordID = $('#txtWorkItem').val();

            	if (recordID.length > 0) {
                    if (recordID.indexOf('-') > -1) {
                        var taskNumber = recordID.slice(recordID.indexOf('-') + 1);
                        recordID = recordID.slice(0, recordID.indexOf('-'));
                        verifyItemExists(recordID, taskNumber, 'Subtask');
                    } else {
                        verifyItemExists(recordID, -1, 'Primary Task');
                    }
            	}
            	else {
            		MessageBox('Please enter a Work Task #.');
            	}
            }

            function buttonGoToWorkRequest_click() {
            	var requestID = $('#txtWorkRequest').val();

            	if (requestID > 0) {
            		verifyItemExists(requestID, -1, 'Work Request');
            	}
            	else {
            		MessageBox('Please enter a Work Request #.');
            	}
            }


            function buttonAdd_click() {
            	var url = '';

            	url = _pageURLs.Maintenance.WorkItemEditParent
					+ '?workItemID=0'
					+ '&newWorkItem=true'
                    + '&sourceWorkItemID=0'
                    + '&ShowPageContentInfo=<%=ShowPageContentInfo.ToString().ToLower()%>'
            	;

            	if (parent.ShowFrameForWorkloadItem) {
            		parent.ShowFrameForWorkloadItem(true, 0, 0, true);
            	}
            	else {
            		window.location.href = url;
            	}
            }

            function buttonCopy_click() {
            	var url = '';

            	url = _pageURLs.Maintenance.WorkItemEditParent
					+ '?workItemID=0'
					+ '&newWorkItem=true'
					+ '&sourceWorkItemID=' + _workItemID
            	;

            	if (parent.ShowFrameForWorkloadItem) {
            		parent.ShowFrameForWorkloadItem(true, 0, _workItemID, true);
            	}
            	else {
            		window.location.href = url;
            	}
            }

            function buttonNext_click() {
                try{
                    if (UseLocal && listItems) { //check that UseLocal is true and that listItems is not undefined
                        if (listItems.length && !isNaN(nextItem) && !isNaN(previousItem)) { //make sure all our variables are defined
                            if (nextItem + 1 <= listItems.length) { //make sure nextItem index is within the bounds of the list array
                                $('#txtWorkItem').val(listItems[nextItem]); //set the value of the text box
                                nextItem++; //increment the pointers
                                previousItem++;
                                window.localStorage.setItem("nextItem", nextItem); //store them for later
                                window.localStorage.setItem("previousItem", previousItem);
                                buttonGoToWorkItem_click(); //trigger the click event
                            }
                        }
                    }
                }
                catch (ex) {
                    MessageBox("An error has occured.");
                    $('#buttonNext').prop('disabled', true);
                    $('#buttonPrevious').prop('disabled', true);
                    UseLocal = false;
                }
            }

            function buttonPrevious_click() {
                try {
                    if (UseLocal && listItems) { //check that UseLocal is true and that listItems is not undefined
                        if (listItems.length && !isNaN(nextItem) && !isNaN(previousItem)) { //make sure all our variables are defined
                            if (previousItem >= 0) { //make sure nextItem index is within the bounds of the list array
                                $('#txtWorkItem').val(listItems[previousItem]); //set the value of the text box
                                nextItem--; //increment the pointers
                                previousItem--;
                                window.localStorage.setItem("nextItem", nextItem); //store them for later
                                window.localStorage.setItem("previousItem", previousItem);
                                buttonGoToWorkItem_click(); //trigger the click event
                            }
                        }
                    }
                }
                catch (ex) {
                    MessageBox("An error has occured.");
                    $('#buttonNext').prop('disabled', true);
                    $('#buttonPrevious').prop('disabled', true);
                    UseLocal = false;
                }
            }

		</script>
    </form>

	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript">

		$(document).ready(function () {
		    try {
		        _pageURLs = new PageURLs();
		        _workRequestID = parseInt('<%=this.WorkRequestID %>');
		        _workItemID = parseInt('<%=this.WorkItemID %>');
		        if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
		            _canEdit = true;
		            if (_workItemID > 0) {
		                $('#buttonCopy').click(function () { buttonCopy_click(); return false; });
		                $('#buttonCopy').prop('disabled', false);
		            }
		            $('#buttonAdd').click(function () { buttonAdd_click(); return false; });
		            $('#buttonAdd').prop('disabled', false);
		        }

                initNextPrevious();

                if ('<%=ShowPageContentInfo.ToString().ToUpper()%>' == 'TRUE') {
                    $('#divWorkItemHeaderButtons').show();
                }

				$(window).resize(resizePage);

				$('#imgRefresh').click(function () { refreshPage(); });
				$('#<%=this.buttonBackToGrid.ClientID %>').click(function () { buttonBackToGrid_click(); return false; });

				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: 0
				});

				if ('<%=this.WorkItemID %>' == '0') {
					$("#divTabsContainer").tabs("option", "disabled", [1, 2, 3]);
				}
				else {
					_commentQty = +'<%=this.Comment_Count %>';
					_attachmentQty = +'<%=this.Attachment_Count %>';
					_wrAttachmentQty = +'<%=this.WR_Attachment_Count %>';

					SetCommentQty(_commentQty);
					SetAttachmentQty(_attachmentQty);
					SetWRAttachmentQty(_wrAttachmentQty);
				}

				Tab_click('details');

				$('#buttonGoToWorkItem').click(function (event) { buttonGoToWorkItem_click(); return false; });
				$('#buttonGoToWorkRequest').click(function (event) { buttonGoToWorkRequest_click(); return false; });
				$("#txtWorkItem").keyup(function (event) {
					formatGoTo(this);

					if (event.keyCode == 13 || event.keyCode == 144) {
						$('#buttonGoToWorkItem').trigger('click');
					}
				});
				$("#txtWorkRequest").keyup(function (event) {
					formatGoTo(this);

					if (event.keyCode == 13 || event.keyCode == 144) {
						$('#buttonGoToWorkRequest').trigger('click');
					}
				});
				$("input:text[name='GoTo']").bind('paste', null, function () {
					formatGoTo(this);
				});

				$('#buttonNext').click(function (event) { buttonNext_click(); return false; });
				$('#buttonPrevious').click(function (event) { buttonPrevious_click(); return false; });
				resizePage();
			} catch (e) {
				var m = e.message;
			}
		});
	</script>
</body>
</html>
