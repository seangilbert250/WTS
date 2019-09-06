﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" EnableViewState="false" CodeFile="WorkItemGrid.aspx.cs" Inherits="WorkItemGrid" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Workload Task Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table id="tableAddSearch" style="border-collapse: collapse; width: 100%;">
	    <tr id="trAddSearch">
            <td>
                Primary Tasks/Subtasks (<span id="spanRowCount" runat="server">0</span>)
		    </td>
            <td style="text-align: right;">
                <input type="button" id="buttonGoToWorkItem" value="Go to Work Task #" />
                <input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="11" size="8" style="margin-right: 5px;" />
                <input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" style="display: none;" />
                <input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" style="margin-right: 5px; display: none;" />
                <input type="button" id="buttonNewWorkItem" value="Add Primary Task" disabled="disabled" style="margin-right: 5px;" />
                <input type="button" id="buttonNewSubTask" value="Add Subtask" disabled="disabled" style="margin-right: 5px; display:none;" />
                <input type="button" id="buttonNewRequest" value="Add Work Request" disabled="disabled" style="margin-right: 5px; display:none;" />
                <input type="button" id="buttonNewSR" value="Add SR" disabled="disabled" style="margin-right: 5px;" />
                <input type="button" id="buttonMetrics" value="Show Metrics" style="margin-right: 5px;" />
                <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpMetrics" ContentPlaceHolderID="ContentPlaceHolderMetrics" runat="Server">
	<div id="divMetrics" style="padding: 5px; overflow: auto; display:none;">Loading metrics...</div>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters">
		<tr id="trQuickFilters">
			<td style="padding-left: 10px;">
				View:
				<asp:DropDownList ID="ddlView" runat="server" TabIndex="1" AppendDataBoundItems="true">
					<%--<asp:ListItem Text="Work Request" Value="0" />--%>
					<asp:ListItem Text="Workload" Value="1" />
					<asp:ListItem Text="SR" Value="2" />
				</asp:DropDownList>
			</td>
		</tr>
	</table>
</asp:Content>

<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">

	<iti_Tools_Sharp:Grid ID="grdWorkload" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" HeightModifier="0" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />

    <iframe id="frmDownload" style="display: none;"></iframe>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
		<Services>
			<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
		</Services>
	</asp:ScriptManager>

	<script id="jsAJAX" type="text/javascript">

	    var sortValue = "";
	    var _selectedAssigned = '';
	    var ddlAssignedChanged = 'no';
        var _businessReview = false;
        var chkBacklog = $('#<%=Master.FindControl("chk_Item13").ClientID %>');
        var chkClosed = $('#<%=Master.FindControl("chk_Item14").ClientID %>');
        var chkArchived = $('#<%=Master.FindControl("chk_Item15").ClientID %>');

        function verifyItemExists(itemID, taskNumber, type) {
		    // 4-14-2017 - Added these 2:
		    updateSelectedStatuses();
            updateSelectedAssigned();
            window.WorkloadWebmethods.ItemExists(itemID, taskNumber, type, function (result) { verifyItemExists_done(itemID, taskNumber, type, result); }, function (result) { verifyItemExists_done(itemID, taskNumber, type, false); });
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
                }
				MessageBox('Could not find ' + type + ' #' + itemID);
			}
		}

		function on_error(result) {
		    var resultText = 'An error occurred when communicating with the server';
			MessageBox('save error:  \n' + resultText);
		}

	</script>

	<script id="jsWorkRequest" type="text/javascript">

		function buttonNewRequest_click() {
			var title = '', url = '';
			var h = 700, w = 1000;

			title = 'New Work Request';
			url = _pageUrls.Maintenance.WorkRequestEditParent;

			var openPopup = popupManager.AddPopupWindow('NewWorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function lbEditRequest_click(requestID) {
			if (parent.ShowFrameForWorkRequest) {
				parent.ShowFrameForWorkRequest(false, requestID, requestID, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'Work Request - [' + requestID + ']';
				url = _pageUrls.Maintenance.WorkRequestEditParent
					+ '?WorkRequestID=' + requestID;

				var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function imgShowHideChildren_click(sender, direction, id) {
			try {
				if (id == "0" || id == "ALL") {
					var requestId = '0';

					$('[Name="img' + direction + '"]').each(function () {
						requestId = $(this).attr('requestId');
						if (requestId && +requestId > 0) {
							imgShowHideChildren_click(this, direction, requestId);
						}
					});
				}

				if (direction.toUpperCase() === "SHOW") {
					//show row/div with child grid frame
					//get frame and pass url(if necessary)
					var td;

					$(sender).closest('tr').each(function () {
						var currentRow = $(this);
						var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
						var childType = 'WorkItem';
						$(row).show();

						td = $('td:eq(<%=(this.DCC == null || !this.DCC.Contains("WORKREQUESTID")) ? 0 : this.DCC["WORKREQUESTID"].Ordinal %>)', row)[0];
						childType = $(row).attr('childType');
						loadChildGrid(td, id, childType);
					});
				}
				else {
					$('tr[Name="gridChild_' + id + '"]').hide();
				}

				$(sender).hide();
				$(sender).siblings().show();
			} catch (e) {
				var msg = e.message;
			}
		}

		function loadChildGrid(td, id, childType) {
			var url = 'Loading.aspx?Page=';

			if (childType === 'SR') {
				url += _pageUrls.Maintenance.WorkloadGrid_SRs;
			}
			else {
				url += _pageUrls.Maintenance.WorkloadGrid_WorkItems;
			}

			url += '?requestId=' + id;

            if (childType === 'WorkItem') url += '&ShowArchived=' + (chkArchived.is(':checked') ? 1 : 0);
            if (childType === 'WorkItem') url += '&ShowBacklog=' + (chkBacklog.is(':checked') ? 1 : 0);
			url += '&SelectedStatuses=' + _selectedStatuses;
			url += '&SelectedAssigned=' + _selectedAssigned;

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src === "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}

	</script>

	<script id="jsSR" type="text/javascript">

	</script>

	<script id="jsWorkItem" type="text/javascript">

	    var _idxDescription;
	    var _displayedAssigned;

	    function lbEditWorkItem_click(recordId) {
	        var UseLocal = false; //to use local storage enabling next/previous on task edit page, or to not, that is the question.
	        UseLocal = setItemList(recordId);

	        // 4-14-2017 - Added these
	        updateSelectedStatuses();
	        updateSelectedAssigned();

			if (parent.ShowFrameForWorkloadItem) {
			    parent.ShowFrameForWorkloadItem(false, recordId, recordId, true, UseLocal, _selectedStatuses, _selectedAssigned);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Primary Task - [' + recordId + ']';
				url = _pageUrls.Maintenance.WorkItemEditParent
					+ '?WorkItemID=' + recordId
                    + '&UseLocal=True';

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
        }

        function editWorkTask(itemID, taskNumber, result) {
            var title = 'Subtask - [' + itemID + '-' + taskNumber + ']';
            var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
                + window.location.search //will have WorkItemID
                + '&workItemID=' + itemID
                + '&newTask=0'
                + '&taskID=' + result;

            var h = 700, w = 850;

            var openPopup = popupManager.AddPopupWindow('WorkloadSubTask', title, url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

		function imgShowHideChildTasks_click(sender, direction, id) {
			try {
				if (id == "0") {
					var itemId = '0';

					$('[Name="img' + direction + '"]').each(function () {
						itemId = $(this).attr('workItemId');
						if (itemId && +itemId > 0) {
							imgShowHideChildTasks_click(this, direction, itemId);
						}
					});
				}
				else {

					if (direction.toUpperCase() === "SHOW") {
						//show row/div with child grid frame
						//get frame and pass url(if necessary)
						var td;

						$(sender).closest('tr').each(function () {
							var currentRow = $(this);
							var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
							$(row).show();

							td = $('td:eq(' + _idxDescription + ')', row)[0];
							loadChildGrid_Tasks(td, id);
						});
					}
					else {
						$('tr[Name="gridChild_' + id + '"]').hide();
					}
				}

				$(sender).hide();
				$(sender).siblings().show();
			} catch (e) {
				var msg = e.message;
			}
		}

        function loadChildGrid_Tasks(td, id) {
            updateSelectedStatuses();
            updateSelectedAssigned();
		    var url = 'Loading.aspx?Page='
					//+ _pageUrls.Maintenance.WorkItemGrid_Tasks;
					+ 'WorkItem_Tasks.aspx'
					+ '?workItemID=' + id
		            + '&SelectedStatuses=' + _selectedStatuses
		            + '&SelectedAssigned=' + _selectedAssigned
                    + '&BusinessReview=' + _businessReview
					+ '&parent=grid'
                    + '&ShowClosed=' + (chkClosed.is(':checked') ? 1 : 0)
                    + '&ShowArchived=' + (chkArchived.is(':checked') ? 1 : 0)
		            + '&ShowBacklog=' + '<%=this.ShowBacklog%>';

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}

		function buttonNewSubTask_click() {
			if (_selectedId > 0) {
				var title = 'Subtask';
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
							+ '?workItemID=' + _selectedId
							+ '&newTask=1'
							+ '&taskID=0'
				;

				var h = 700, w = 850;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('NewWorkloadSubTask', title, url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
			else {
				MessageBox('Please select a task.');
			}
		}

	</script>

	<script id="jsEvents" type="text/javascript">
		var _pageUrls;
		var _selectedId = 0, _myView = 0;
		var _canViewRequest = false, _canViewSR = false, _canViewWorkItem = false;
		var _canEditRequest = false, _canEditSR = false, _canEditWorkItem = false;
		var _displayedStatuses = '', _selectedStatuses = '';

		function refreshPage(preservePageNum, showClosed) {
            var qs = document.location.href;
            updateSelectedStatuses();
            updateSelectedAssigned();
			qs = editQueryStringValue(qs, 'RefData', 1);
            qs = editQueryStringValue(qs, 'MyData', parent.$('#ddlView_Work option:selected').text() == 'My Data' ? true : false);
			qs = editQueryStringValue(qs, 'View', $('#<%=this.ddlView.ClientID %> option:selected').val());
		    qs = editQueryStringValue(qs, 'SelectedStatuses',_selectedStatuses);
		    qs = editQueryStringValue(qs, 'SelectedAssigned', _selectedAssigned);
            qs = editQueryStringValue(qs, 'BusinessReview', _businessReview);
		    qs = editQueryStringValue(qs, 'ddlChanged', 'yes');
		    qs = editQueryStringValue(qs, 'ddlAssignedChanged', ddlAssignedChanged);
			var view = $('#<%=this.ddlView.ClientID %> option:selected').text();
			if (preservePageNum && (preservePageNum == true || preservePageNum == 1)) {
				var pageIndex = '<%=this.grdWorkload.PageIndex %>';
				qs = editQueryStringValue(qs, 'PageIndex', pageIndex);
			}
			else {
				qs = editQueryStringValue(qs, 'PageIndex', 0);
			}

            qs = editQueryStringValue(qs, 'ShowClosed', (chkClosed.is(':checked') ? 1 : 0));
            qs = editQueryStringValue(qs, 'ShowBacklog', (chkBacklog.is(':checked') ? 1 : 0));
            qs = editQueryStringValue(qs, 'ShowArchived', (chkArchived.is(':checked') ? 1 : 0));

		    // Added 12-6-16
			qs = editQueryStringValue(qs, 'sortOrder', sortValue);
			qs = editQueryStringValue(qs, 'sortChanged', 'true');

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {
			var url = window.location.href;
			url = editQueryStringValue(url, 'Export', true);

			alert('Generating report. Watch for Save dialog on bottom of page.');

            $('#frmDownload').attr('src', url);
		}

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

			    var sURL = 'SortOptions.aspx?GridName=WORKITEMGRID.ASPX&sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
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

        function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 65;
            var addLeft = 181;

            if ($imgShowQuickFilters.is(':visible')) {
                $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

                var pos = $imgShowQuickFilters.position();
                $divQuickFilters.css({
                    width: '325px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideDown(function () { resizeFrame(); });
            }
            else if ($imgHideQuickFilters.is(':visible')) {
                $imgHideQuickFilters.hide();
                $imgShowQuickFilters.show();

                var pos = $imgHideQuickFilters.position();
                $divQuickFilters.css({
                    width: '325px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideUp(function () { resizeFrame(); });
            }
        }

		function ddlView_change() {
			refreshPage();
		}

		function chkShowClosed_change() {

            var show = chkClosed.prop('checked');

            var arrStatuses = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');
			var idxClosed = $.inArray('10', arrStatuses); //temporary - id of Closed = 10
			var idxApprovedClosed = $.inArray('70', arrStatuses); //temporary - id of Approved/Closed = 70
            var arrAssigned = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

			if(show && idxClosed == -1) {
				arrStatuses.push('10');
			}
			else if(!show && idxClosed > -1) {
				arrStatuses.splice(idxClosed, 1);
			}

			if (show && idxApprovedClosed == -1) {
				arrStatuses.push('70');
			}
			else if (!show && idxApprovedClosed > -1) {
				arrStatuses.splice(idxApprovedClosed, 1);
			}

            $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('setSelects', arrStatuses);
            $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('refresh');

            $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('setSelects', arrAssigned);
            $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('refresh');

		    updateSelectedStatuses();
            updateSelectedAssigned();

            $('#<%=Master.FindControl("lblMessage").ClientID %>').show();
            $('#<%=Master.FindControl("lblMessage").ClientID %>').text('<< Click Refresh icon to apply Quick Filter(s)');
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
                // 4-14-2017 - Added these 2:
			    updateSelectedStatuses();
			    updateSelectedAssigned();
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

        function loadMetrics() {
			$('#divMetrics').text('Loading metrics...');

            var SelectedStatus = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');  // Will look like > 80,72,1,2,3,4... So "wrap" in stored proc.
            var SelectedUsers = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

		    PageMethods.LoadMetrics("'" + SelectedStatus + "'" , "'" + SelectedUsers + "'", '<%=this._showArchived %>', '<%=this._myData %>', loadMetrics_done, function () { $('#divMetrics').text('Failed to load metrics.'); });
		}

	    function loadMetrics_done(result) {
			var ds = jQuery.parseJSON(result);

			if (ds == null || ds.length == 0) {
				$('#divMetrics').text('No data found.');
			}
			else {
				var totalOnHold = 0, totalOpen = 0, totalAwaitingClosure = 0, totalClosed = 0;
				var totalTaskOnHold = 0, totalTaskOpen = 0, totalTaskAwaitingClosure = 0, totalTaskClosed = 0;
				var tbl = '<table cellpadding="3" cellspacing="0" style="display:inline-block; border-top:1px solid grey; border-left:1px solid grey;">';
				$.each(ds.WorkloadMetrics, function (rowIndex, row) {
					if (rowIndex == 0) {
						tbl += '<tr>';
						$.each(this, function (column, value) {
							if (column == 'Priority') {
								tbl += '<td class="metricsHeader" style="border-bottom:1px solid grey; border-right:1px solid grey;"><b>Primary Tasks</b></td>';
							}
							else {
								var title = '', span = '', display = '';
								switch (column) {
									case 'On Hold':
									case 'Info Requested':
										title = (column == 'On Hold' ? 'On Hold (0)' : 'On Hold');
										span = (column == 'On Hold' ? 'colspan="2"' : '');
										display = (column != 'On Hold' ? 'display:none;' : '');
										break;
									case 'New':
									case 'In Progress':
									case 'Re-Opened':
									case 'Info Provided':
									case 'Un-Reproducible':
										title = (column == 'New' ? 'Open (0)' : 'Open');
										span = (column == 'New' ? 'colspan="5"' : '');
										display = (column != 'New' ? 'display:none;' : '');
										break;
									case 'Checked In':
									case 'Deployed':
										title = (column == 'Checked In' ? 'Awaiting Closure (0)' : 'Awaiting Closure');
										span = (column == 'Checked In' ? 'colspan="2"' : '');
										display = (column != 'Checked In' ? 'display:none;' : '');
										break;
									case 'Closed':
										title = 'Closed (0)';
										break;
								}
								tbl += '<td ' + span + ' class="metricsHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey; ' + display + '"><b>' + title + '</b></td>';
							}
						})
						tbl += '</tr><tr>';
						$.each(this, function (column, value) {
							tbl += '<td class="metricsSubHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;"><b>' + column + '</b></td>';
						})
						tbl += '</tr>';
					}

					tbl += '<tr>';
					$.each(this, function (column, value) {
                        if (row.Priority && row.Priority.indexOf('Task TOTAL') > -1) {

					    }
					    else {
                            if (row.Priority && row.Priority.indexOf('TOTAL') > -1) {
					            tbl += '<td style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey; background-color:#E6E6E6;">' + value + '</td>';

					            try {
					                switch (column) {
					                    case 'On Hold':
					                    case 'Info Requested':
					                        totalOnHold += parseInt(value);
					                        break;
					                    case 'New':
					                    case 'In Progress':
					                    case 'Re-Opened':
					                    case 'Info Provided':
					                    case 'Un-Reproducible':
					                        totalOpen += parseInt(value);
					                        break;
					                    case 'Checked In':
					                    case 'Deployed':
					                        totalAwaitingClosure += parseInt(value);
					                        break;
					                    case 'Closed':
					                        totalClosed += parseInt(value);
					                        break;
					                }
					            }
					            catch (e) { }
					        }
					        else {
					            tbl += '<td style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;">' + value + '</td>';
					        }
					    }
					})
					tbl += '</tr>';
				});
				tbl += '</table>';

				tbl = tbl.replace('On Hold (0)', 'On Hold (' + totalOnHold + ')');
				tbl = tbl.replace('Open (0)', 'Open (' + totalOpen + ')');
				tbl = tbl.replace('Awaiting Closure (0)', 'Awaiting Closure (' + totalAwaitingClosure + ')');
				tbl = tbl.replace('Closed (0)', 'Closed (' + totalClosed + ')');

				var tblTask = '<table cellpadding="3" cellspacing="0" style="display:inline-block; border-top:1px solid grey; border-left:1px solid grey; vertical-align:top;">';
				$.each(ds.WorkloadMetricsSub, function (rowIndex, row) {
				    if (rowIndex == 0) {
				        tblTask += '<tr>';
				        $.each(this, function (column, value) {
				            if (column == 'Priority') {
				                tblTask += '<td class="metricsHeader" style="border-bottom:1px solid grey; border-right:1px solid grey;"><b>Subtasks</b></td>';
				            }
				            else {
				                var title = '', span = '', display = '';
				                switch (column) {
				                    case 'On Hold':
				                    case 'Info Requested':
				                        title = (column == 'On Hold' ? 'On Hold (0)' : 'On Hold');
				                        span = (column == 'On Hold' ? 'colspan="2"' : '');
				                        display = (column != 'On Hold' ? 'display:none;' : '');
				                        break;
				                    case 'New':
				                    case 'In Progress':
				                    case 'Re-Opened':
				                    case 'Info Provided':
				                    case 'Un-Reproducible':
				                        title = (column == 'New' ? 'Open (0)' : 'Open');
				                        span = (column == 'New' ? 'colspan="5"' : '');
				                        display = (column != 'New' ? 'display:none;' : '');
				                        break;
				                    case 'Checked In':
				                    case 'Deployed':
				                        title = (column == 'Checked In' ? 'Awaiting Closure (0)' : 'Awaiting Closure');
				                        span = (column == 'Checked In' ? 'colspan="2"' : '');
				                        display = (column != 'Checked In' ? 'display:none;' : '');
				                        break;
				                    case 'Closed':
				                        title = 'Closed (0)';
				                        break;
				                }
				                tblTask += '<td ' + span + ' class="metricsHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;' + display + '"><b>' + title + '</b></td>';
				            }
				        })
				        tblTask += '</tr><tr>';
				        $.each(this, function (column, value) {
				            tblTask += '<td class="metricsSubHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;"><b>' + (column == 'Priority' ? '' : column) + '</b></td>';
				        })
				        tblTask += '</tr>';
				    }

				    tblTask += '<tr>';
				    $.each(this, function (column, value) {
				        if (row.Priority && row.Priority.indexOf('Task TOTAL') > -1) {

				        }
				        else {
                            if (row.Priority && row.Priority.indexOf('TOTAL') > -1) {
				                tblTask += '<td style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey; background-color:#E6E6E6;">' + value.replace('Task ', '') + '</td>';

				                try {
				                    switch (column) {
				                        case 'On Hold':
				                        case 'Info Requested':
				                            totalTaskOnHold += parseInt(value);
				                            break;
				                        case 'New':
				                        case 'In Progress':
				                        case 'Re-Opened':
				                        case 'Info Provided':
				                        case 'Un-Reproducible':
				                            totalTaskOpen += parseInt(value);
				                            break;
				                        case 'Checked In':
				                        case 'Deployed':
				                            totalTaskAwaitingClosure += parseInt(value);
				                            break;
				                        case 'Closed':
				                            totalTaskClosed += parseInt(value);
				                            break;
				                    }
				                }
				                catch (e) { }
				            } else {
				                tblTask += '<td style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;">' + value + '</td>';
				            }
				        }

				    })
				    tblTask += '</tr>';
				});
				tblTask += '</table>';
			    tblTask = tblTask.replace('On Hold (0)', 'On Hold (' + totalTaskOnHold + ')');
				tblTask = tblTask.replace('Open (0)', 'Open (' + totalTaskOpen + ')');
				tblTask = tblTask.replace('Awaiting Closure (0)', 'Awaiting Closure (' + totalTaskAwaitingClosure + ')');
				tblTask = tblTask.replace('Closed (0)', 'Closed (' + totalTaskClosed + ')');

				$('#divMetrics').html(tbl + '&nbsp;&nbsp;' + tblTask);

				if ($('#divMetrics').height() > 242) {
					$('#divMetrics').height(242);
				}
				else {
					$('#divMetrics').height('auto');
				}
			}

			resizeGrid();
		}

		function buttonMetrics_click() {
		    $('#buttonMetrics').prop('value', ($('#divMetrics').is(':visible') ? 'Show Metrics' : 'Hide Metrics'));
		    $('#divMetrics').slideToggle(function () {
		        resizeGrid();
		        //positionStatusOptions();
		    });
		}

        function resizeGrid() {
			try { resizePageElement($('#<%=this.grdWorkload.ClientID %>_BodyContainer').attr('id'), 23); } catch (e) { }
		}

		function buttonNewSR_click() {

		    var title = 'New SR';
		    var url = 'Loading.aspx?Page=SR_Add.aspx'
		            + '?workItemID=' + _selectedId;

		    var h = 700, w = 850;

		    //open in a popup
		    var openPopup = popupManager.AddPopupWindow('NewSR', title, url, h, w, 'PopupWindow', this);
		    if (openPopup) {
		        openPopup.Open();
		    }
        }

		function buttonNewWorkItem_click() {
			if (parent.ShowFrameForWorkloadItem) {
				parent.ShowFrameForWorkloadItem(true, 0, 0, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Primary Task';
				url = _pageUrls.Maintenance.WorkItemEditParent;

				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function reports_click(actionType, report) {

		}

		function activateAddSRButton() {
		    $('#buttonNewSR').attr('disabled', false);
		    $('#buttonNewSR').prop('disabled', false);
		}


		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
				activateAddSRButton();

				if (_canEditWorkItem) $('#buttonNewSubTask').attr('disabled', false);
				$('#buttonAddSR').attr('disabled', false);
			}
		}

		function relatedItems_click(action, type) {
			var h = 800, w = 1500;
			var name = "", title = "";
			var url = "";

            switch (action.toUpperCase()) {
                case 'QM':
                    url = "";
                    name = "QM";
                    title = "QM";
                    switch (type.toUpperCase()) {
                        case "ATTRIBUTE":
                            name += type;
                            title += " " + type;

                            url = "WorkItemGrid_QM.aspx" + window.location.search;
                            break;
                    }

                    url = 'Loading.aspx?Page=' + url;
                    var openPopup = popupManager.AddPopupWindow(name, title, url, h, w, 'PopupWindow', this);
                    if (openPopup) {
                        openPopup.Open();
                    }
                    break;
                case 'MASSCHANGE':
                    url = "WorkItem_MassChange.aspx" + window.location.search;
                    name = "WorkloadTaskMassChange";
                    title = "Primary Task Mass Change";

                    url = editQueryStringValue(url, 'FieldName', type);

                    // SCB 2-7--2017 - Added these for Mass Change
                    var arrAssigned = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');
                    var arrStatuses = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');

                    url = editQueryStringValue(url, 'SelectedAssigned', arrAssigned);
                    url = editQueryStringValue(url, 'SelectedStatuses', arrStatuses);

                    h = 200;
                    w = 500;

                    url = 'Loading.aspx?Page=' + url;
                    var openPopup = popupManager.AddPopupWindow(name, title, url, h, w, 'PopupWindow', this);
                    if (openPopup) {
                        openPopup.Open();
                    }
                    break;
                case 'SR':
                    url = "WorkItem_SR_Status.aspx" + window.location.search;
                    name = "WorkloadTask_SR_Status";
                    title = "Work Task SR Status";

                    h = 600;
                    w = 500;

                    url = 'Loading.aspx?Page=' + url;
                    var openPopup = popupManager.AddPopupWindow(name, title, url, h, w, 'PopupWindow', this);
                    if (openPopup) {
                        openPopup.Open();
                    }
                    break;
                case 'RELEASEASSESSMENT':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

		function openMenuItem(url) {
			if (url.indexOf("relatedItems") > 0) {
				var str = url.split("'");
				relatedItems_click(str[1], str[3]);
			}
		}


		function resizeFrames() {
			var frame;
			var fPageHeight = 0;

			$('iFrame').each(function () {
				frame = $(this)[0];
				fPageHeight = 0;

				if (frame.contentWindow
					&& frame.contentWindow.document
					&& frame.contentWindow.document.body
					&& frame.contentWindow.document.body.offsetHeight) {
					fPageHeight = frame.contentWindow.document.body.offsetHeight;
				}
				frame.style.height = fPageHeight + 8 + 'px';
			});

			resizePageElement($('#<%=this.grdWorkload.ClientID %>').attr('id'), 0);
			//<%=this.grdWorkload.ClientID %>.RedrawGrid();
		}

	    function ddlStatus_change() {
	        updateSelectedStatuses();
	    }

		function updateSelectedStatuses() {
            var arrSelections = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');

			_selectedStatuses = arrSelections.join(',');
		}

	</script>

	<script id="jsColOrder" type="text/javascript">

		function imgReorder_click() {
			openGridOrderer();
		}

		function openGridOrderer() {

			var sURL = 'Grid_Order.aspx';
			var nPopup = popupManager.AddPopupWindow("Orderer", "Order Grid Columns", sURL, 525, 500, "PopupWindow", this.self);
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

				var pURL = document.location.href;
				pURL = editQueryStringValue(pURL, 'columnOrder', escape(columnOrder));
				pURL = editQueryStringValue(pURL, 'columnOrderChanged', 'true');
				pURL = editQueryStringValue(pURL, 'RefData', '0');
				pURL = editQueryStringValue(pURL, 'View', $('#<%=this.ddlView.ClientID %> option:selected').val());

				var view = $('#<%=this.ddlView.ClientID %> option:selected').text();
				if (view == 'Workload' || view === 'Work Request') {
                    pURL = editQueryStringValue(pURL, 'ShowArchived', (chkArchived.is(':checked') ? 1 : 0));
				}

				document.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
				MessageBox('updateColumnOrder:\n' + e.number + '\n' + e.message);
			}
		}

		function SetTaskQty(workItemID, taskCount) {
			$('.taskCount_' + workItemID).text('(' + taskCount + ')');
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();
				_myView = parseInt('<%=this.MyView %>');

				_canEditRequest = ('<%=this.CanEditRequest.ToString().ToUpper()%>' == 'TRUE');
				_canEditSR = ('<%=this.CanEditSR.ToString().ToUpper()%>' == 'TRUE');
				_canEditWorkItem = ('<%=this.CanEditWorkItem.ToString().ToUpper()%>' == 'TRUE');
				_canViewRequest = (_canEditRequest || ('<%=this.CanViewRequest.ToString().ToUpper()%>' == 'TRUE'));
				_canViewSR = (_canEditSR || ('<%=this.CanViewSR.ToString().ToUpper()%>' == 'TRUE'));
				_canViewWorkItem = (_canEditWorkItem || ('<%=this.CanViewWorkItem.ToString().ToUpper()%>' == 'TRUE'));
                _businessReview = '<%=this.QFBusinessReview%>'.toLowerCase();

			} catch (e) { }

			try {
				_idxDescription = '<%=this.DCC != null && this.DCC["DESCRIPTION"] != null ? this.DCC["DESCRIPTION"].Ordinal : 0%>';
			} catch (e) { }
		}

	    function setItemList(recordID) {
	        try {
	            if (window.localStorage && typeof(Storage) !== "undefined") { //cheap check if browser supports local storage
	                var listArray = "<%=this.itemList%>".split(',');
	                var nextItem; var previousItem;
	                for (var i = 0; i < listArray.length; i++) {
	                    if (listArray[i] === recordID.toString()) {
	                        nextItem = i+1;
	                        previousItem = i - 1;
	                        break;
	                    }
	                }
	                if (!nextItem || nextItem >= listArray.length) {
	                    nextItem = -1; //-1 means the end of the list has been reached (the current item is the last item in the list). If current item is the first item in the list, previousItem is set to -1, so that kind of takes care of itself.
	                }
	                window.localStorage.setItem("itemList", "<%=this.itemList%>");
	                window.localStorage.setItem("nextItem", nextItem);
	                window.localStorage.setItem("previousItem", previousItem);
	                return true; //everything A-OK. No exceptions thrown. Everything is probably fine...right?
	            }
	        }
	        catch (ex) { //an error happened. Something went kaboom. Let the caller know to abort the mission.
	            return false;
            }
	    }

  	    function ddlAssigned_change() {
	        updateSelectedAssigned();
	    }

	    function updateSelectedAssigned() {

            var arrSelections = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

		    _selectedAssigned = arrSelections.join(',');
        }

        function clearSelectedAssigned(data) {
            _selectedAssigned = data;
            ddlAssignedChanged = true;
        }

        function chkBusinessReview_change(obj) {
            var $obj = $(obj);

            _businessReview = $obj[0].checked;
            $('#<%=Master.FindControl("lblMessage").ClientID %>').show();
            $('#<%=Master.FindControl("lblMessage").ClientID %>').text('<< Click Refresh icon to apply Quick Filter(s)');
        }

        function chkShowBacklog_change() {
            $('#<%=Master.FindControl("lblMessage").ClientID %>').show();
            $('#<%=Master.FindControl("lblMessage").ClientID %>').text('<< Click Refresh icon to apply Quick Filter(s)');
        }

	    function chkShowArchived_change()
        {
            $('#<%=Master.FindControl("lblMessage").ClientID %>').show();
            $('#<%=Master.FindControl("lblMessage").ClientID %>').text('<< Click Refresh icon to apply Quick Filter(s)');
	    }

		$(document).ready(function () {

			initVariables();
			initControls();
			var view = $('#<%=this.ddlView.ClientID %> option:selected').text();

			$('#imgReorder').show();
            $('#imgReport').hide();
            $('#tdQuickFilters').show();
            $('#trms_Item0').show();
            $('#trms_Item1').show();
            $('#trchk_Item12').show();

            $('#trClearAll').hide();

			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
            $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });

			$('#imgReorder').click(function () { imgReorder_click(); });
			$('#buttonGoToWorkItem').click(function (event) { buttonGoToWorkItem_click(); return false; });
			$('#buttonGoToWorkRequest').click(function (event) { buttonGoToWorkRequest_click(); return false; });

			if (_canEditRequest) {
				$('#buttonNewRequest').attr('disabled', false);
				$('#buttonNewRequest').click(function (event) { buttonNewRequest_click(); return false; });
			}
			if (_canEditSR) {
				$('#buttonNewSR').attr('disabled', false);
				$('#buttonNewSR').click(function (event) { buttonNewSR_click(); return false; });
			}

			$('#buttonMetrics').click(function (event) { buttonMetrics_click(); return false; });

			loadMetrics();

			if (_canEditWorkItem) {
				$('#buttonNewWorkItem').attr('disabled', false);
				$('#buttonNewWorkItem').click(function (event) { buttonNewWorkItem_click(); return false; });
				$('#buttonNewSubTask').click(function (event) { buttonNewSubTask_click(); return false; });

				if (view === 'Workload') $('#buttonNewSubTask').show();
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			$('#<%=this.ddlView.ClientID %>').change(function () { ddlView_change(); return false; });

            chkBacklog.change(function () { chkShowBacklog_change(); return false; });
            chkClosed.change(function () { chkShowClosed_change(); return false; });
            chkArchived.change(function () { chkShowArchived_change(); return false; });

		    $('#txtWorkItem, #txtWorkRequest').keydown(function (e) {
		        if (e.keyCode == 13 || e.keyCode == 144) {
		            e.preventDefault();
		            return false;
		        }
		    });
			$("#txtWorkItem").keyup(function (event) {
				formatGoTo(this);

				if (event.keyCode === 13 || event.keyCode === 144) {
					$('#buttonGoToWorkItem').trigger('click');
				}
			});
			$("#txtWorkRequest").keyup(function (event) {
				formatGoTo(this);

				if (event.keyCode === 13 || event.keyCode === 144) {
					$('#buttonGoToWorkRequest').trigger('click');
				}
			});
			$("input:text[name='GoTo']").bind('paste', null, function () {
				formatGoTo(this);
			});

		    $('.gridPager').css({
		        'position': 'fixed',
		        'bottom': '0'
		    });
		});

        function initControls() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
            var ddlStatus = $('#<%=Master.FindControl("ms_Item0").ClientID %>');
            var ddlAffiliated = $('#<%=Master.FindControl("ms_Item1").ClientID %>');
            var chkBusinessReview = $('#<%=Master.FindControl("chk_Item12").ClientID %>');

            ddlStatus.multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                }
                , onOpen: function () { ddlStatus_change(); }
                , onClose: function () { ddlStatus_change(); }
            }).change(function () { ddlStatus_change(); });
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Non-Resources']").wrapAll("<optgroup label='Non-Resources'>");
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Resources']").wrapAll("<optgroup label='Resources'>");
            ddlAffiliated.multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                }
                , onOpen: function () { ddlAssigned_change(); }
                , onClose: function () { ddlAssigned_change(); }
            }).change(function () { ddlAssigned_change(); });

            chkBusinessReview.on('change', function () { chkBusinessReview_change(this); });
            lblMessage.hide();
	    }

	</script>
</asp:Content>