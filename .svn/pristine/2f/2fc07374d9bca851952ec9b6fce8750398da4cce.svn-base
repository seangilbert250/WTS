<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="WorkItem_Tasks.aspx.cs" Inherits="WorkItem_Tasks" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
	<title>Sub Tasks</title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divPage" class="pageContainer" style="overflow-y:visible; border-bottom:1px solid grey;">
			<div id="divHeader" class="pageContentHeader">
				<table style="width: 100%; text-align: left;" border-collapse: collapse;>
					<tr>
                        <td id="tdSort" style="text-align: left; padding-top:2px; height:30px; width:20px;">
							<img id="imgSort" alt="Sort Items" title="Sort Items" src="images/icons/page_sort_a_z.png" width="15" height="15" style="cursor:pointer; margin-left:4px;" />
						</td>
						<td style="text-align:left; padding-left: 4px; width: 245px;">
							<div>
								Status:
								<select id="ddlStatus" runat="server" multiple="true" class="statusSelect" style="display: none;"></select>   <%--  Removed >>  class="statusSelect"--%>
							</div>
						</td>
						<td style="text-align:left; width:20px;">
							<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="images/icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer;" />
						</td>
						<td style="height:30px; text-align: right; padding-right: 5px; white-space: nowrap;">
                            <label id="lblShowBacklog" style="display: none;">Show Backlog</label><input type="checkbox" id="chkShowBacklog" style="display: none;"/>
							<input type="button" id="buttonNew" value="Add" disabled="disabled" style="padding: 2px; display: none;" />
							<input type="button" id="buttonSave" value="Save" disabled="disabled" style="padding: 2px; display: none;" />
                            <input type="button" id="buttonMove" value="Move" disabled="disabled" style="padding: 2px; display: none;" />
						</td>
					</tr>
				</table>
			</div>
            <div id="divSubTasks"></div>
		</div>

		<script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
		<link rel="stylesheet" href="Styles/multiple-select.css" />

		<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

		<script type="text/javascript" src="Scripts/jquery-ui.js"></script>

		<script type="text/javascript">
			var _pageUrls = {};
			var _displayedStatuses = '', _selectedStatuses = '';
			var _canEdit = false;
			var _originalHeight = 0;
			var arrSubTasks = [];

			function refreshPage(opt) {
				try {
					if (parent.location.href.indexOf('WorkItem_Details') > -1 && opt != 1) {
						parent.refreshPage();
						return;
					}

					if ('<%=this.Parent.ToUpper() %>' == 'GRID') parent.parent.loadMetrics();
				} catch (e) { }

				var qs = document.location.href;

				updateSelectedStatuses();
				qs = editQueryStringValue(qs, 'SelectedStatuses', _selectedStatuses);
				qs = editQueryStringValue(qs, 'ReloadStatusSession', true);

				document.location.href = 'Loading.aspx?Page=' + qs;
			}

			function enableDelete(enable) {

			}

			function addRemoveRow_Done() {
			    resizeFrame();
			}

			function resizeFrame(visibleDatepicker) {
				if (visibleDatepicker == null || visibleDatepicker == undefined) {
					visibleDatepicker = false;
				}
				if (typeof visibleDatepicker != "boolean" && typeof visibleDatepicker != "string") {
					return;
				}

				var frame = getMyFrameFromParent();
				var fPageHeight = 0;

				if (frame
					&& frame.contentWindow
					&& frame.contentWindow.document
					&& frame.contentWindow.document.body) {
                    var header = $('#divHeader', frame.contentWindow.document.body).height();
                    var grid = $('#divSubTasks', frame.contentWindow.document.body).height();

					fPageHeight = header + grid + 20;
					if (_originalHeight == 0) {
						_originalHeight = fPageHeight;
					}
				}

				frame.style.height = fPageHeight + 'px';

				if (parent.resizeFrame) {
					parent.resizeFrame();
				}
			}

			function resizePage(visibleDatepicker) {
				resizeFrame(visibleDatepicker);
			}

		</script>


		<script id="jsAJAX" type="text/javascript">

			function GetColumnValue(row, ordinal, blnoriginal_value) {
				try {
					var tdval = $(row).find('td:eq(' + ordinal + ')');
					var val = '';
					if ($(tdval).length === 0) { return ''; }

					if ($(tdval).children.length > 0) {
						if ($(tdval).find("select").length > 0) {
							if (blnoriginal_value) {
								val = $(tdval).find("select").attr('original_value');
							}
							else {
								val = $(tdval).find("select option:selected").val();
							}
						}
						else if ($(tdval).find('input[type=checkbox]').length > 0) {
							if (blnoriginal_value) {
								val = $(tdval).find('input[type=checkbox]').parent().attr("original_value");
							}
							else {
								if ($(tdval).find('input[type=checkbox]').prop('checked')) { val = '1'; }
								else { val = '0'; }
							}
						}
						else if ($(tdval).find('input[type=text]').length > 0) {
							if (blnoriginal_value) {
								val = $(tdval).find('input[type=text]').attr('original_value');

							}
							else {
								val = $(tdval).find('input[type=text]').val();
							}
						}
						else if ($(tdval).find('input[type=number]').length > 0) {
							if (blnoriginal_value) {
								val = $(tdval).find('input[type=number]').attr('original_value');

							}
							else {
								val = $(tdval).find('input[type=number]').val();
							}
						}
						else if ($(tdval).find('input').length > 0) {
							if (blnoriginal_value) {
								val = $(tdval).find('input').attr('original_value');

							}
							else {
								val = $(tdval).find('input').val();
							}
						}
						else {
							val = $(tdval).text();
						}

					}
					else {
						val = $(tdval).text();
					}
					return val;
				} catch (e) { return ''; }
			}

			function save() {
				try {
					var changedRows = [];
					var id = 0;
					var original_value = '', name = '', description = '', sortOrder = '', archive = '';

					var changed = false;
                    $('#divSubTasks table tr').each(function (i, row) {
						var changedRow = [];
						changed = false;

						if ($(this).attr('changed') && $(this).attr('changed') == '1') {
							changed = true;
						}

						if (changed) {
                            changedRow.push('"WORKITEM_TASKID":"' + $(row).attr('workitem_taskid') + '"');
                            changedRow.push('"BusinessRank":"' + GetColumnValue(row, 3) + '"');
                            changedRow.push('"AssignedToRank":"' + GetColumnValue(row, 4) + '"');
                            changedRow.push('"PRIORITY":"' + GetColumnValue(row, 5) + '"');
                            changedRow.push('"Title":"' + encodeURIComponent(GetColumnValue(row, 6)) + '"');
                            changedRow.push('"AssignedResource":"' + GetColumnValue(row, 8) + '"');
                            changedRow.push('"PrimaryResource":"' + GetColumnValue(row, 9) + '"');
                            changedRow.push('"COMPLETIONPERCENT":"' + GetColumnValue(row, 10) + '"');
                            changedRow.push('"STATUS":"' + GetColumnValue(row, 11) + '"');

							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					});

					if (changedRows.length === 0) {
						MessageBox('You have not made any changes');
					}
					else {
						ShowDimmer(true, "Updating...", 1);
						var json = '[' + changedRows.join(",") + ']';
						window.PageMethods.SaveChanges(json, save_done, on_error);
					}
				} catch (e) {
					ShowDimmer(false);
					MessageBox('There was an error gathering data to save.\n' + e.message);
				}
			}
			function save_done(result) {
				try {
					ShowDimmer(false);

					var saved = false;
					var ids = '', errorMsg = '';

					var obj = jQuery.parseJSON(result);

					if (obj) {
						if (obj.saved && obj.saved.toUpperCase() === 'TRUE') {
							saved = true;
						}
						if (obj.ids) {
							ids = obj.ids;
						}
						if (obj.error) {
							errorMsg = obj.error;
						}
					}

					if (saved) {
						MessageBox('Items have been saved.');
						refreshPage(1);
					}
					else {
						MessageBox('Failed to save items. \n' + errorMsg);
					}
				} catch (e) { }
			}

            function parentSave() {
                var changedRows = [];
                var id = 0;
                var original_value = '', name = '', description = '', sortOrder = '', archive = '';

                var changed = false;
                $('#divSubTasks table tr').each(function (i, row) {
                        var changedRow = [];
                        changed = false;

                        if ($(this).attr('changed') && $(this).attr('changed') == '1') {
                            changed = true;
                        }

                        if (changed) {
                            changedRow.push('"WORKITEM_TASKID":"' + $(row).attr('workitem_taskid') + '"');
                            changedRow.push('"BusinessRank":"' + GetColumnValue(row, 3) + '"');
                            changedRow.push('"AssignedToRank":"' + GetColumnValue(row, 4) + '"');
                            changedRow.push('"PRIORITY":"' + GetColumnValue(row, 5) + '"');
                            changedRow.push('"Title":"' + encodeURIComponent(GetColumnValue(row, 6)) + '"');
                            changedRow.push('"AssignedResource":"' + GetColumnValue(row, 8) + '"');
                            changedRow.push('"PrimaryResource":"' + GetColumnValue(row, 9) + '"');
                            changedRow.push('"COMPLETIONPERCENT":"' + GetColumnValue(row, 10) + '"');
                            changedRow.push('"STATUS":"' + GetColumnValue(row, 11) + '"');

                            var obj = '{' + changedRow.join(',') + '}';
                            changedRows.push(obj);
                        }
                    });

                if (changedRows.length === 0) {
                    return false;
                }
                return true;
            }

			function deleteTask(taskID) {
				try {
					ShowDimmer(true, "Deleting...", 1);

					window.PageMethods.DeleteTask(taskID, deleteTask_done, on_error);
				} catch (e) {
					ShowDimmer(false);
					MessageBox('Error communicating with the server.\n' + e.message);
				}
			}
			function deleteTask_done(result) {
				ShowDimmer(false);

				var deleted = false;
				var id = 0;
				var errorMsg = '';

				try {
					var obj = jQuery.parseJSON(result);

					if (obj) {
						if (obj.deleted && obj.deleted.toUpperCase() === 'TRUE') {
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
						MessageBox('Task has been deleted.');
						addRemoveRow_Done();
						refreshPage();
					}
					else {
						MessageBox('Failed to delete Subtask. \n' + errorMsg);
					}
				}
				catch (e) { }
			}

			function on_error(result) {
				ShowDimmer(false);

				var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

				MessageBox('save error:  \n' + resultText);
			}
		</script>

		<script id="jsQuickFilters" type="text/javascript">

			function ddlStatus_change() {
				updateSelectedStatuses();
			}

			function ddlStatus_open() {
				var arrSelections = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');

				_selectedStatuses = arrSelections.join(',');
				_displayedStatuses = _selectedStatuses;
				resizePage(true);
			}

			function ddlStatus_close() {
				if (_displayedStatuses !== _selectedStatuses) {
					_displayedStatuses = _selectedStatuses;
				}

				refreshPage(1);
			}

			function updateSelectedStatuses() {
				var arrSelections = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');

				_selectedStatuses = arrSelections.join(',');
			}

		</script>

		<script id="jsEvents" type="text/javascript">

			function activateSaveButton() {
				if (_canEdit) {
					$('#buttonSave').attr('disabled', false);
					$('#buttonSave').prop('disabled', false);

					$('[id*="imgSave"]').attr('disabled', false);
					$('[id*="imgSave"]').prop('disabled', false);
					$('[id*="imgSave"]').css('cursor', 'pointer');
					$('[id*="imgSave"]').attr('title', 'Save Subtask Updates');
					$('[id*="imgSave"]').unbind('click').click(function (event) { buttonSave_click(); return false; });
				}
			}

			function txt_change(sender) {
				var original_value = '', new_value = '';
				if ($(sender).attr('original_value')) {
					original_value = $(sender).attr('original_value');
				}

				new_value = $(sender).val();

				if (new_value !== original_value) {
					$(sender).closest('tr').attr('changed', '1');
					activateSaveButton();
				}
			}

			function ddl_change(sender) {
				var original_value = '', new_value = '';
				if ($(sender).attr('original_value')) {
					original_value = $(sender).attr('original_value');
				}

				new_value = $('option:selected', $(sender)).val();

				if (new_value !== original_value) {
					$(sender).closest('tr').attr('changed', '1');
					activateSaveButton();
				}
            }

            function parentSaveCheck() {
                var statusID = 0;
                var UnclosedSRTask = 0;
                var displayPrompt = false;
                $('#divSubTasks table tr').each(function (i, row) {
                    changed = false;
                    statusID = 0;
                    UnclosedSRTask = 0;

                    if ($(this).attr('changed') && $(this).attr('changed') == '1') {
                        changed = true;
                    }

                    if (changed) {
                        statusID = GetColumnValue(row, 11);
                        UnclosedSRTask = GetColumnValue(row, 13);

                        if (statusID == 10 && UnclosedSRTask == 1) displayPrompt = true;
                    }
                });
                return displayPrompt;
            }

			function buttonSave_click(taskID) {
                if (_canEdit) {
                    var statusID = 0;
                    var UnclosedSRTask = 0;
                    var displayPrompt = false;

                    $('#divSubTasks table tr').each(function (i, row) {
                        changed = false;
                        statusID = 0;
                        UnclosedSRTask = 0;

                        if ($(this).attr('changed') && $(this).attr('changed') == '1') {
                            changed = true;
                        }

                        if (changed) {
                            statusID = GetColumnValue(row, 11);
                            UnclosedSRTask = GetColumnValue(row, 13);

                            if (statusID == 10 && UnclosedSRTask == 1) displayPrompt = true;
                        }
                    });

                    if (displayPrompt) {
                        QuestionBox('Confirm SR Closed', 'All Work Tasks associated to a SR will be in a Closed Status. This will cause the SR to be set to Resolved. Would you like to proceed?', 'Save,Cancel', 'confirmSRUpdate', 300, 300, this);
                    } else {
                        save();
                    }
				}
            }

            function confirmSRUpdate(answer) {
                switch ($.trim(answer).toUpperCase()) {
                    case 'SAVE':
                        save();
                        break;
                    default:
                        return;
                }
            }

			function lbEditTask_click(taskID, taskNumber) {
                var title = 'Subtask - [' + taskNumber + ']';
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
							+ window.location.search //will have WorkItemID
							+ '&newTask=0'
							+ '&taskID=' + taskID;

				var h = 700, w = 850;

				var openPopup = popupManager.AddPopupWindow('Subtask', title, url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}

			function imgAddTask_click() {
				if (!_canEdit) {
					MessageBox('You do not have permissions to edit this Subtask.');
					return;
				}
				addTask();
			}

			function addTask() {
				var title = 'New Subtask';
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
							+ window.location.search //will have WorkItemID
							+ '&newTask=1'
							+ '&taskID=0'
				;

				var h = 700, w = 850;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('NewSubtask', title, url, h, w, 'PopupWindow', this);
				if (openPopup) {
				    openPopup.Open();
				}
			}
			function addTask_done(result) {
				var added = true;

				//TODO: check if added
				if (added) {
					addRemoveRow_Done();
					refreshPage();
				}
			}

			function buttonMove_click() {
			    arrSubTasks.length = 0;
			    var $objGrid = $('#divSubTasks');
			    $objGrid.find('input[type="checkbox"]:checked').each(function () {
			        var $obj = $(this);
			        if ($obj.attr('WorkItemTaskId') != 'WORKITEM_TASKID'){
			            if ($.inArray($obj.attr('WorkItemTaskId'), arrSubTasks) == -1) arrSubTasks.push($obj.attr('WorkItemTaskId'));
			        }
			    });

			    var nWindow = 'MoveSubTask';
			    var nTitle = 'Move Subtask';
			    var nHeight = 700, nWidth = 1000;
			    var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=MoveSubTask';
			    var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

			    if (openPopup) openPopup.Open();
			}

			function chkAll(obj) {
			    var blnChecked = $(obj).is(':checked')
			    $('#divSubTasks').find('input[type=checkbox]').prop('checked', blnChecked);

			    if ($(obj).is(':checked')) blnAllChecked = true;
			    else blnAllChecked = false;

			    $('#buttonMove').prop('disabled', false);
			}

			function input_change(obj) {
			    var $obj = $(obj);

			    $obj.attr('fieldChanged', '1');
			    $obj.closest('tr').attr('rowChanged', '1');

			    $('#buttonMove').prop('disabled', false);
			}

            function imgDeleteTask_click(taskID) {
                if (!_canEdit) {
                    MessageBox('You do not have permissions to edit this Subtask.');
                    return;
                }
				var msg = 'Unable to delete Subtask.';
				//TODO: do validation
				var valid = true;
				if (taskID === '' || taskID === 0) {
					valid = false;
					msg += '\n' + 'Please select a Subtask to delete.';
				}

				if (valid) {
					if (confirm('This will permanently delete this Subtask.' + '\n' + 'Do you wish to continue?')) {
						deleteTask(taskID);
					}
				}
				else {
					MessageBox(msg);
					return;
				}
			}


			function chkShowBacklog_change() {
			    url = window.location.href;
			    url = editQueryStringValue(url, 'ShowBacklog', ($('#chkShowBacklog').is(':checked') ? 1 : 0));
			    url = editQueryStringValue(url, 'RefData', 1);
			    window.location.href = url;
			}

			function imgSort_click() {
			    try {
                    var sortableColumns = 'Subtask;Customer Rank;Assigned To Rank;Priority;Version;Assigned To;Primary Resource;% Comp;Status;SR Number;Re-Opened';
				    while (sortableColumns.indexOf('<BR />') > -1) {
					    sortableColumns = sortableColumns.replace("<BR />", ' ');
				    }
				    while (sortableColumns.indexOf('<BR/>') > -1) {
					    sortableColumns = sortableColumns.replace("<BR/>", ' ');
				    }
				    while (sortableColumns.indexOf('<br />') > -1) {
					    sortableColumns = sortableColumns.replace("<br />", ' ');
				    }
				    while (sortableColumns.indexOf('<br/>') > -1) {
					    sortableColumns = sortableColumns.replace("<br/>", ' ');
				    }

				    while (sortableColumns.indexOf('...') > -1) {
					    sortableColumns = sortableColumns.replace('...', '');
				    }

				    while (sortableColumns.indexOf('<BR>') > -1) {
					    sortableColumns = sortableColumns.replace('<BR>', ' ');
				    }
				    while (sortableColumns.indexOf('<br>') > -1) {
					    sortableColumns = sortableColumns.replace('<br>', ' ');
				    }

			        var sURL = 'SortOptions.aspx?GridName=WorkItem_Tasks.ASPX&sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				    var nPopup = popupManager.AddPopupWindow("Sorter", "Sort Grid", sURL, 200, 400, "PopupWindow", this.self);
				    if (nPopup) {
					    nPopup.Open();
				    }
			    }
			    catch (e) {
			    }
			}

		    function applySort(sortValue) {
		        try {
		            var pURL = document.location.href;
		            pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
		            pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

		            document.location.href = 'Loading.aspx?Page=' + pURL;
		        }
		        catch (e) {
		        }
		    }

            function getSubTasks() {
                $('#divSubTasks').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

                updateSelectedStatuses();

                PageMethods.GetSubTasks('<%=this.WorkItemID %>' == '0' ? '<%=this.sourceWorkItemID %>' : '<%=this.WorkItemID %>', '<%=this._showArchived %>', '<%=this.ShowBacklog %>', '<%=this.SelectedStatuses != null ? string.Join(",", this.SelectedStatuses) : "" %>', '<%=this.ShowClosed %>', '<%=this.SelectedAssigned != null ? string.Join(",", this.SelectedAssigned) : "" %>', '<%=this.BusinessReview%>'.toLowerCase(), '<%=this.SortOrder %>', getSubTasks_done, getSubTasks_error);
            }

            function getSubTasks_done(result) {
                var nHTML = '';
                var dt = jQuery.parseJSON(result);

                if (dt != null && parent.SetTaskQty) {
                    parent.SetTaskQty('<%=this.WorkItemID %>', dt.length);
                }

                if (dt == null || dt.length == 0) {
                    nHTML = 'No Subtasks';
                }
                else {
                    nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                    nHTML += '<tr class="gridHeader">';
                    if (_canEdit) nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 18px;"><input type="checkbox" WorkItemId="WORKITEMID" WorkItemTaskId="WORKITEM_TASKID" onchange="chkAll(this);" /></th>';
                    if (_canEdit) nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 18px;"><a href="" onclick="imgAddTask_click(); return false;" style="color: blue;">New</a></th>';
                    nHTML += '<th style="border-top: 1px solid grey;' + (!_canEdit ? ' border-left: 1px solid grey;' : '') + ' text-align: center; width: 70px;">Subtask</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 60px;">Customer Rank</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 85px;">Assigned To Rank</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 45px;">Priority</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Title</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">Version</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 95px;">Assigned To</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 95px;">Primary Resource</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 50px;">% Comp</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 45px;">Status</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 45px;">SR Number</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 45px; display: none;">Unclosed SR Tasks</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 45px;">Re-Opened</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 45px;">In Progress Date</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 45px;">Ready For Review Date</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 45px;">Deployed Date</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; text-align: center; width: 45px;">Closed Date</th>';
                    if (_canEdit) nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 18px;"><img src="Images/Icons/disk.png" title="Save" alt="Save" height="12" width="12" onclick="buttonSave_click(); return false;" style="cursor: pointer;" /></th>';
                    nHTML += '</tr>';

                    $.each(dt, function (rowIndex, row) {
                        var selectedOption = '', options = '';
                        var blnEvenRow = rowIndex % 2 == 0;

                        nHTML += '<tr class="gridBody"' + (!blnEvenRow ? ' style="background-color: gainsboro;"' : "") + ' workitem_taskid="' + row.WORKITEM_TASKID + '">';
                        if (_canEdit) nHTML += '<td style="border-left: 1px solid grey; text-align: center;"><input type="checkbox" WorkItemId="' + row.WORKITEMID + '" WorkItemTaskId="' + row.WORKITEM_TASKID + '" onchange="input_change(this);" /></td>';
                        if (_canEdit) nHTML += '<td style="text-align: center;"><img src="Images/Icons/delete.png" title="Delete" alt="Delete" height="12" width="12" onclick="imgDeleteTask_click(' + row.WORKITEM_TASKID + ', ' + row.TASK_NUMBER + '); return false;" style="cursor: pointer;" /></td>'; //New
                        nHTML += '<td' + (!_canEdit ? ' style="border-left: 1px solid grey;' : '') + '><a href="" onclick="lbEditTask_click(' + row.WORKITEM_TASKID + ', &quot;' + row.WORKITEMID + ' - ' + row.TASK_NUMBER + '&quot;); return false;" style="color: blue;">' + row.WORKITEMID + ' - ' + row.TASK_NUMBER + '</a></td>'; //Sub-Task
                        nHTML += '<td style="text-align: center;"><input type="text" value="' + row.BusinessRank + '" style="text-align: center; width: 60px;"></td>'; //Customer Rank

                        selectedOption = row.AssignedToRank;
                        options = decodeURIComponent('<%=this.AssignedToRankOptions %>');
                        if (selectedOption != null && selectedOption.length > 0) {
                            options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td><select style="background-color: #F5F6CE; width: 85px;">' + options + '</select></td>'; //Assigned To Rank

                        selectedOption = row.PRIORITY;
                        options = decodeURIComponent('<%=this.PriorityOptions %>');
                        if (selectedOption != null && selectedOption.length > 0) {
                            options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td><select style="background-color: #F5F6CE;">' + options + '</select></td>'; //Priority

                        nHTML += '<td><input type="text" value="' + row.TITLE + '" style="width: 98%;"></td>'; //Title
                        nHTML += '<td>' + row.Version + '</td>'; //Version

                        selectedOption = row.AssignedResource;
                        options = decodeURIComponent('<%=this.ResourceOptions %>');
                        if (selectedOption != null && selectedOption.length > 0) {
                            if (selectedOption.indexOf('Action Team') != -1) {
                                options = '<option value="' + row.ASSIGNEDRESOURCEID + '" selected>' + selectedOption + '</option>' + options;
                            }
                            else {
                                if (options.indexOf(selectedOption) == -1) {
                                    options = '<option value="' + row.ASSIGNEDRESOURCEID + '" selected>' + selectedOption + '</option>' + options;
                                }
                                else {
                                    options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                                }
                            }
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td><select style="background-color: #F5F6CE; width: 95px;">' + options + '</select></td>'; //Assigned To

                        selectedOption = row.PrimaryResource;
                        options = decodeURIComponent('<%=this.ResourceOptions %>');
                        if (selectedOption != null && selectedOption.length > 0) {
                            if (options.indexOf(selectedOption) == -1) {
                                options = '<option value="' + row.PRIMARYRESOURCEID + '" selected>' + selectedOption + '</option>' + options;
                            }
                            else {
                                options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                            }
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td><select style="background-color: #F5F6CE; width: 95px;">' + options + '</select></td>'; //Primary Resource

                        selectedOption = row.COMPLETIONPERCENT;
                        options = decodeURIComponent('<%=this.CompletionOptions %>');
                        if (selectedOption != null) {
                            options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td style="text-align: center;"><select style="background-color: #F5F6CE;">' + options + '</select></td>'; //% Comp

                        selectedOption = row.STATUS;
                        options = decodeURIComponent('<%=this.StatusOptions %>');
                        if (selectedOption != null && selectedOption.length > 0) {
                            options = filterStatusDDL(selectedOption, options);
                            options = options.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                        }
                        else {
                            options = '<option value="0" selected>-Select-</option>' + options;
                        }
                        nHTML += '<td><select style="background-color: #F5F6CE;">' + options + '</select></td>'; //Status
                        nHTML += '<td>' + (row.SRNumber == null ? '' : row.SRNumber) + '</td>'; //SR Number
                        nHTML += '<td style="display: none;">' + (row["Unclosed SR Tasks"] == null ? '' : row["Unclosed SR Tasks"]) + '</td>'; //SR Number
                        nHTML += '<td style="text-align: center;">' + row.ReOpenedCount + '</td>'; //Re-Opened
                        nHTML += '<td style="text-align: center;">' + row.INPROGRESSDATE + '</td>'; //InProgressDate
                        nHTML += '<td style="text-align: center;">' + row.READYFORREVIEWDATE + '</td>'; //ReadyForReviewDate
                        nHTML += '<td style="text-align: center;">' + row.DEPLOYEDDATE + '</td>'; //DeployedDate
                        nHTML += '<td style="text-align: center;">' + row.CLOSEDDATE + '</td>'; //ClosedDate
                        if (_canEdit) nHTML += '<td></td>';
                        nHTML += '</tr>';
                    });

                    nHTML += '</table>';
                }

                $('#divSubTasks').html(nHTML);
                resizeFrame();
            }

            function filterStatusDDL(selected, statuses) {
                if (selected !== 'New') {
                    statuses = statuses.replace('<option value="1">New</option>', '');
                }

                if (!(selected === 'Re-Opened'
                    || selected === 'Closed'
                    || selected === 'Un-Reproducible'
                    || selected === 'Checked In'
                    || selected === 'Deployed'
                    || selected === 'Ready for Review')) {
                    statuses = statuses.replace('<option value="2">Re-Opened</option>', '');
                }

                return statuses;
            }

            function getSubTasks_error() {
                $('#divSubTasks').html('Error gathering data.');
            }
		</script>
	</form>

	<script id="jsInit" type="text/javascript">

		function initControls() {
			try {
				$('.statusSelect').multipleSelect({
					placeholder: 'Default(NOT Closed)'
					, width: 'undefined'
					, onOpen: function () {
						ddlStatus_open();
					}
					, onClose: function () {
						ddlStatus_close();
					}
				}).change(function () { ddlStatus_change(); });
			} catch (e) {

			}
		}

		$(document).ready(function () {
			initControls();

			_pageUrls = new PageURLs();
			if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
				_canEdit = true;
			}
			if ('<%=this.Parent.ToUpper() %>' !== 'GRID') {
				$('#buttonNew').show();
				$('#buttonSave').show();
				$('#chkShowBacklog').show();
				$('#lblShowBacklog').show();
			}

		    if (_canEdit) {
		        $('#buttonMove').show();
				$('#buttonNew').click(function () { imgAddTask_click(); return false; });
				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
				$('#buttonMove').click(function () { buttonMove_click(); return false; });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').prop('disabled', false);
				$('#chkShowBacklog').change(function () { chkShowBacklog_change(); return false; });
                $('#chkShowBacklog').prop('checked', ('<%=this.ShowBacklog.ToString().ToUpper() %>' == 'TRUE'));
                $('#divPage').on('change keyup mouseup', 'input:text', function () { txt_change(this); });
                $('#divPage').on('change', 'input:checkbox', function () { txt_change(this); });
                $('#divPage').on('change keyup mouseup', 'input', function () { txt_change(this); });
                $('#divPage').on('change keyup mouseup', 'select', function () { ddl_change(this); });

				$('#imgSort').click(function (event) { imgSort_click(); return false; });

				$('.date').each(function () {
					$(this).datepicker({
						changeMonth: true
						, showOtherMonths: true
						, selectOtherMonths: true
						, changeYear: true
						, onSelect: function () { resizePage(true); }
						, onClose: function () { resizePage(false); }
					});
				}).click(function () { resizePage(true); }).focus(function () { resizePage(true); });
			}

			$('#imgRefresh').click(function () { refreshPage(1); });

			$(window).resize(resizePage);

			resizeFrame(false);

            getSubTasks();
		});
	</script>
</body>
</html>
