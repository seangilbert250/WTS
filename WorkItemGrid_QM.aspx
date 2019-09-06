﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="WorkItemGrid_QM.aspx.cs" Inherits="WorkItemGrid_QM" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Workload Task Quick Maintenance</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Primary Tasks (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<span id="lblMsg" style="display:none;">Loading list options...</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td style="text-align:right;">
				<label for="chkShowClosed">Show Closed</label><input type="checkbox" id="chkShowClosed" style="vertical-align:middle;" />
				&nbsp;
				<label for="chkShowArchived">Show Archived</label><input type="checkbox" id="chkShowArchived" style="vertical-align:middle;" />
				&nbsp;
				<input type="button" id="buttonGoToWorkItem" value="Go to Work Task #" />
				<input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="11" size="8" />
				&nbsp;
				<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" style="display: none;" />
				<input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" style="display: none;" />
				&nbsp;
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdWorkload" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
		<Services>
			<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
		</Services>
	</asp:ScriptManager>

	<script type="text/javascript">
		var _canEditWorkItem = false;
		var _idxStatus = 0;
		var _pageUrls;

		function refreshPage(preservePageNum) {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			if (preservePageNum && (preservePageNum == true || preservePageNum == 1)) {
				var pageIndex = '<%=this.grdWorkload.PageIndex %>';
				qs = editQueryStringValue(qs, 'PageIndex', pageIndex);
			}
			else {
				qs = editQueryStringValue(qs, 'PageIndex', 0);
			}
			qs = editQueryStringValue(qs, 'IncludeArchive', ($('#chkShowArchived').is(':checked') ? 1 : 0));
			qs = editQueryStringValue(qs, 'ShowClosed', $('#chkShowClosed').prop('checked'));

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {

		}

		function activateSaveButton(enable) {
			if (_canEditWorkItem) {
				$('#buttonSave').attr('disabled', false);
				$('#buttonSave').prop('disabled', false);
			}
		}

		function txt_change(sender) {
			var original_value = '', new_value = '';
			if ($(sender).attr('original_value')) {
				original_value = $(sender).attr('original_value');
			}

			new_value = $(sender).val();

			if (new_value != original_value) {
				$(sender).closest('tr').attr('changed', '1');
				activateSaveButton();
			}
		}

		function resizePage() {

		}

		function buttonSave_click() {
			save();
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

	</script>

	<script id="jsSort" type="text/javascript">

		function imgSort_click() {
			try {
				var sortableColumns = '<%=this.SortableColumns%>';
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

				var sURL = 'SortOptions.aspx?GridName=WORKITEMGRID_QM.ASPX&sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				//var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>;
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
				var pURL = window.location.href;
				pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
				pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

				window.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
			}
		}


		function lbEditWorkItem_click(recordId) {
			if (parent.ShowFrameForWorkloadItem) {
				parent.ShowFrameForWorkloadItem(false, recordId, recordId, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Primary Task - [' + recordId + ']';
				url = _pageUrls.Maintenance.WorkItemEditParent
					+ '?WorkItemID=' + recordId;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkloadTask', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
        }

        function editWorkTask(itemID, taskNumber, result) {
            var title = 'Subtask - [' + itemID + '-' + taskNumber + ']';
            var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
                + window.location.search
                + '&workItemID=' + itemID
                + '&newTask=0'
                + '&taskID=' + result;

            var h = 700, w = 850;

            var openPopup = popupManager.AddPopupWindow('WorkloadSubTask', title, url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

	</script>

	<script id="jsColOrder" type="text/javascript">

		function imgReorder_click() {
			openGridOrderer();
		}

		function openGridOrderer() {

			var sURL = 'Grid_Order.aspx';
			var nPopup = popupManager.AddPopupWindow("Orderer", "Order Grid Columns", sURL, 500, 600, "PopupWindow", this.self);
			if (nPopup) {
				nPopup.Open();
			}
		}

		function getSelectedColumnOrder(blnDefault) {
			try {
				var selectedColumnOrder = '<%=this.SelectedColumnOrder%>';
				var defaultColumnOrder = '<%=this.DefaultColumnOrder%>';

				if (blnDefault) {
					return defaultColumnOrder;
				}
				else {
					return selectedColumnOrder;
				}
			}
			catch (e) {
				return "";
			}
		}

		function updateColumnOrder(columnOrder) {
			try {

				var pURL = window.location.href;
				pURL = editQueryStringValue(pURL, 'columnOrder', escape(columnOrder));
				pURL = editQueryStringValue(pURL, 'columnOrderChanged', 'true');
				pURL = editQueryStringValue(pURL, 'RefData', '0');

				window.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
				MessageBox('updateColumnOrder:\n' + e.number + '\n' + e.message);
			}
		}

	</script>

	<script id="jsDropdown" type="text/javascript">

		function ddl_change(ddl) {
			var textField = $(ddl).attr('field');

			if (textField.toUpperCase().indexOf("WorkType") > -1) {
				//find status ddl and force reload
				var tr = $(ddl).closest('tr')[0];
				var workTypeID = $('option:selected', $(ddl)).val()
				var td = $(tr).find('td:eq(' + _idxStatus + ')');

				var ddlStatus = $(td).find('select');

				$(tr).attr('WorkTypeID', workTypeID);
				LoadList(ddlStatus);
			}

			var value = '', originalValue = '';
			value = $('option:selected', $(ddl)).val();
			if ($(ddl).attr("original_value")) {
				originalValue = $(ddl).attr("original_value");
			}

			if (value != originalValue) {
				$(ddl).closest('tr').attr('changed', '1');
				activateSaveButton(true);
			}
		}

		function chk_change(sender) {
			var value = '', originalValue = '';
			value = $(sender).prop('checked') ? 1 : 0;

			if ($(sender).attr("original_value")) {
				originalValue = $(sender).attr("original_value");
			}

			if (value != originalValue) {
				$(sender).closest('tr').attr('changed', '1');
				activateSaveButton(true);
			}
		}
		
		function showDropdown(ddl) {
			var event;

			try {
				event = new MouseEvent('click', {
					view: window,
					bubbles: false,
					cancelable: true
				});

				/* can be added for i.e. compatiblity. */
				ddl.focus();
				var WshShell = new ActiveXObject("WScript.Shell");
				WshShell.SendKeys("%{DOWN}");
				/**/

				ddl.dispatchEvent(event);

				return;
			} catch (e) {
				
			}
			
			try {
				event = document.createEvent('MouseEvents');
				event.initMouseEvent('mousedown', true, true, window,
					0, 0, 0, 80, 20,
					false, false, false, false,
					0, null);
				ddl.dispatchEvent(event);

				/* can be added for i.e. compatiblity. */
				ddl.focus();
				var WshShell = new ActiveXObject("WScript.Shell");
				WshShell.SendKeys("%{DOWN}");
				/**/
			} catch (e) {

			}
			
			try {
				event.stopPropagation();
			} catch (e) { };
			return false;
		}

		/***********************
		Drop Down List Loading
		**********************/
		function LoadList(ctl) {
			try {
				var ddl;
				if (ctl.length == 0) {
					ddl = $(ctl.context);
				}
				else {
					ddl = ctl;
				}

				var workTypeId = '', selectedVal = '', selectedText = '';
				var idField = '', textField = '';
				var data = {};
				selectedVal = $(ddl).find('option:selected').val();
				selectedText = $(ddl).find('option:selected').text();
				workTypeId = $(ddl).closest('tr').attr("WorkTypeID");

				$('#lblMsg').show();
				textField = $(ddl).attr('field');
				if (textField == undefined || textField == 'undefined') {
					return;
				}
				switch (textField.toUpperCase()) {
					case "WORKITEMTYPE":		//WORKITEMTYPEID
						idField = "WORKITEMTYPEID";
						data = _WorkItemTypeList;
						break;
					case "WORKREQUEST":			//WORKREQUESTID
						idField = "WORKREQUESTID";
						textField = "TITLE";
						data = _WorkRequestList;
						break;
					case "WEBSYSTEM":			//WTS_SYSTEMID
						idField = "WTS_SYSTEMID";
						textField = "WTS_SYSTEM";

						data = _SystemList;
						break;
					case "ALLOCATION":			//ALLOCATIONID
						idField = "ALLOCATIONID";
						data = removeDuplicates(_AllocationList, "ALLOCATION");
						break;
					case "PRODUCTVERSION":		//ProductVersionID
						idField = "ProductVersionID";
						textField = "ProductVersion";
						data = _ProductVersionList;
						break;
					case "PRIORITY":			//PRIORITYID
						idField = "PRIORITYID";
						data = _PriorityList;
						break;
					case "PRIMARY_DEVELOPER":	//PRIMARYRESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "USERNAME";
						data = _UserList;
						break;
					case "PRIORITY_RANK":		//PRIORITYID
						idField = "PRIORITYID";
						textField = "PRIORITY";
						data = _PriorityRankList;
						break;
					case "ASSIGNED":			//ASSIGNEDRESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "USERNAME";
						data = _UserList;
						break;
					case "WORKTYPE":		//WorkTypeID
						idField = "WorkTypeID";
						data = _WorkTypeList;
						break;
					case "STATUS":				//STATUSID
						idField = "STATUSID";
						data = _StatusList;
						break;
					case "PROGRESS":			//Percent
						idField = "Percent";
						textField = "Percent";
						data = _PercentList;
						break;
					case "MENUTYPE":			//MenuTypeID
						idField = "_MenuTypeList";
						data = _PercentList;
						break;
					case "MENU":				//MenuID
						idField = "MenuID";
						data = _MenuList;
						break;
					case "PRODUCTIONSTATUS":		//STATUSID
						idField = "STATUSID";
						textField = "STATUS";
						data = _ProductionStatusList;
						break;
				}
				
				if (typeof data === 'undefined') {
					return;
				}

				$(ddl).empty();

				$.each(data[0], function (rowindex, row) {
					var mg1 = textField;
					if (textField.toUpperCase() == "STATUS"
						&& row["WorkTypeID"]
						&& workTypeId != row["WorkTypeID"]) { //make sure WorkType of status record matches
						return true;
					}

					var $option = $("<option />");
					// Add value and text to option
					$option.attr("value", row[idField]).text(row[textField]);
					$option.attr('title', row[textField]);
					$option.css('font-size', '12px');
					$option.css('font-family', 'Arial');
					// Add option to drop down list
					$(ddl).append($option); //prepend?		
					
				});

				if (!$('option[value="' + selectedVal + '"]', $(ddl))) {
					var $o = $('<option />');
					$o.text('-SELECT-');
					$o.val('0');
					$o.css('font-size', '12px');
					$o.css('font-family', 'Arial');
					$(ddl).prepend($o);
					$(ddl).val('0');

				}
				else {
					$(ddl).val(selectedVal);
				}

				$(ddl).unbind('mousedown').unbind('focus').unbind('onclick');
				$(ddl).prop('onclick', null);

				$('#lblMsg').hide();

                //FilterStatuses
                if (textField.toUpperCase() === 'STATUS') {
                    if ($(ddl).val() != 1) {
                        $(ddl).children().each(function () {
                            if ($(this).text() === 'New') {
                                $(this).wrap('<span>');
                                $(this).hide();
                            }
                        });
                    }

                    if (!($(ddl).val() == 2  // Re-Opened
                        || $(ddl).val() == 7 // Un-Reproducible
                        || $(ddl).val() == 8 // Deployed
                        || $(ddl).val() == 9 // Checked In
                        || $(ddl).val() == 10 // Closed
                        || $(ddl).val() == 11)) { // Ready for Review
                        $(ddl).children().each(function () {
                            if ($(this).text() === 'Re-Opened') {
                                $(this).wrap('<span>');
                                $(this).hide();
                            }
                        });
                    }
                }

				//showDropdown($(ddl)[0]);
			} catch (e) {
				MessageBox(e.message);
				$('#lblMsg').hide();
			}
		}

		function removeDuplicates(list, variable) {
			for (i = 0; i < list[0].length; i++) {
				for (j = i + 1; j < list[0].length; j++) {
					if (list[0][i][variable].trim() == list[0][j][variable].trim()) {
						list[0].splice(j, 1);
						j = j - 1;
					}
				}
			}
			return list;
		}

	</script>

	<script id="jsAJAX" type="text/javascript">

		function verifyItemExists(itemID, taskNumber, type) {
			//PageMethods.ItemExists(itemID, type, function (result) { verifyItemExists_done(itemID, type, result); }, function (result) { verifyItemExists_done(itemID, type, false); });
            WorkloadWebmethods.ItemExists(+itemID, taskNumber, type, function (result) { verifyItemExists_done(itemID, taskNumber, type, result); }, function (result) { verifyItemExists_done(itemID, taskNumber, type, false); });
		}

        function verifyItemExists_done(itemID, taskNumber, type, exists) {
			if (exists && exists.toUpperCase() == "TRUE") {
				switch (type) {
					case 'Primary Task':
						lbEditWorkItem_click(itemID);
						break;
					case 'Work Request':
						lbEditRequest_click(itemID);
                        break;
                    case 'Subtask':
                        PageMethods.WorkItem_TaskID_Get(itemID, taskNumber, function (result) { editWorkTask(itemID, taskNumber, result); });
                        break;
				}
			}
            else {
                if (taskNumber > -1) {
                    MessageBox('Could not find ' + type + ' #' + itemID + '-' + taskNumber);
                } else {
                    MessageBox('Could not find ' + type + ' #' + itemID);
                }
			}
		}

		function save() {
			try {
				var changedRows = [];
				var id = 0;
				var original_value = '', name = '', description = '', sortOrder = '', archive = '';

				ShowDimmer(true, "Updating...", 1);

				var changed = false;
				$('.gridBody, .selectedRow', $('#<%=this.grdWorkload.ClientID%>_Grid')).each(function (i, row) {
					var changedRow = [];
					changed = false;

					if ($(this).attr('changed') && $(this).attr('changed') == '1') {
						changed = true;
					}

					if (changed) {
						for (var i = 0; i <= _dcc[0].length - 1; i++) {
							changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
						}
						var obj = '{' + changedRow.join(',') + '}';
						changedRows.push(obj);
					}
				});

				if (changedRows.length == 0) {
					ShowDimmer(false);
					MessageBox('No changes were detected.');
				}
				else {
					var json = '[' + changedRows.join(",") + ']';
					PageMethods.SaveChanges(json, save_done, on_error);
				}
			} catch (e) {
				ShowDimmer(false);
				MessageBox('There was an error gathering data to save.\n' + e.message);
			}
		}
		function save_done(result) {
			try {
				ShowDimmer(false);

				var saved = 0, failed = 0;
				var errorMsg = '', ids = '', failedIds = '';

				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.saved) {
						saved = parseInt(obj.saved);
					}
					if (obj.failed) {
						failed = parseInt(obj.failed);
					}
					if (obj.savedIds) {
						ids = obj.savedIds;
					}
					if (obj.failedIds) {
						failedIds = obj.failedIds;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				var msg = '';
				if (errorMsg.length > 0) {
					msg = 'An error occurred while saving: \n' + errorMsg;
				}

				if (saved > 0) {
					msg = 'Successfully saved ' + saved + ' Work Tasks.';
					if (opener && opener.refreshPage) {
						opener.refreshPage(true);
					}
				}
				if (failed > 0) {
					msg += '\n' + 'Failed to save ' + failed + ' Work Tasks.';
				}
				MessageBox(msg);

				refreshPage();
			} catch (e) { }
		}

		function on_error(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

	</script>


	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();
				_canEditWorkItem = ('<%=this.CanEditWorkItem.ToString().ToUpper()%>' == 'TRUE');

				_idxStatus = +'<%=this.DCC == null ? 0 : this.DCC["STATUS"].Ordinal %>';
			} catch (e) { }
		}

		$(document).ready(function () {

			initVariables();

			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');

			$('#imgReorder').show();
			$('#imgReport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
			$('#imgReorder').click(function () { imgReorder_click();});
			if (_canEditWorkItem) {
				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });

				$('select', $('#<%=this.grdWorkload.ClientID %>_Grid')).on('mousedown focus', function () { LoadList(this); });
				$('select').on('change', function () { ddl_change(this); return false; });
				$('input:checkbox').on('change', function () { chk_change(this); return false; });
			}

			$('#chkShowClosed').change(function () { $('#imgRefresh').trigger('click'); });
			$('#chkShowClosed').prop('checked', ('<%=this.ShowClosed.ToString().ToUpper() %>' == 'TRUE'));

			$('#chkShowArchived').click(function () { $('#imgRefresh').trigger('click'); });
			$('#chkShowArchived').prop('checked', '<%=this._includeArchive %>' == 'True');

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
            

		});
	</script>
</asp:Content>