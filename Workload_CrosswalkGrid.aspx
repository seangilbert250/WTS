﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" EnableViewState="false" CodeFile="Workload_CrosswalkGrid.aspx.cs" Inherits="Workload_CrosswalkGrid" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Workload Crosswalk Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
	<table style="width: 100%; padding-right: 7px;">
		<tr>
			<td>Rollup (<span id="spanRowCount" runat="server">0</span>)
			</td>
			<td>Grid View (<span id="spanGridView" runat="server">0</span>)
			</td>
            <td>
  				<input type="button" id="buttonGoToWorkItem" value="Go to Task #" />
				<input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="6" size="3" />
				&nbsp;
				<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" style="display: none;" />
				<input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" style="display: none;" />
				&nbsp;
				<input type="button" id="buttonNewWorkItem" value="Add Task" disabled="disabled" />
				<input type="button" id="buttonNewRequest" value="Add Work Request" disabled="disabled" style="display: none;" />
				&nbsp;
   				<input type="button" id="buttonNewSR" value="Add SR"/>
            </td>
            <td>
  				<iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related Items <img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="padding: 0px; margin: 0px; display: inline-block; float: left;"></iti_Tools_Sharp:Menu>
            </td>
			<td style="text-align: right;">
				<input type="button" id="buttonToggleMetrics" value="Show Metrics" />
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
			<td style="text-align: left; height: 30px; width: 16px;">
				<img id="imgSettings" alt="Grid Settings" title="Select grid columns" src="Images/Icons/cog.png" width="15" height="15" 
					style="cursor: pointer; margin-left: 2px;" />
			</td>
			<td style="text-align:left; padding-left: 10px; height: 30px;">
				<div>
					Status: 
					<select id="ddlStatus" runat="server" multiple="true" class="statusSelect" style="display: none;"></select>
				</div>
			</td>
            <td style="text-align:left; padding-left: 10px; height: 30px; vertical-align:middle;">
				<div>
					 Affiliated: 
					<select id="ddlAssigned" runat="server" multiple="true" class="AssignedSelect" style="display: none;"></select>
				</div>
			</td>
            <td style="text-align:left; padding-left: 10px; height: 30px; vertical-align:middle; visibility:visible">
				<div>
					 User Types: 
					<select id="ddlUserTypeFilters" runat="server" multiple="true" class="UserTypeSelected" style="display: none;"></select>  
				</div>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="float: left; padding-right: 2px;">
		<tr>
			<td>
				<input type="checkbox" id="chkShowBacklog" style="vertical-align: middle;" />
                <label for="chkBacklog" style="vertical-align: middle;">Show Backlog</label>
				&nbsp;
				<input type="checkbox" id="chkShowClosed" style="vertical-align: middle;" />
				<label for="chkShowClosed" style="vertical-align: middle;">Show Closed</label>
				&nbsp;
				<input type="checkbox" id="chkShowArchived" style="vertical-align: middle;" />
				<label for="chkShowArchived" style="vertical-align: middle;">Show Archived</label>
				&nbsp;
			</td>





<%--            <td>
  				<input type="button" id="buttonGoToWorkItem" value="Go to Task #" />
				<input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="6" size="3" />
				&nbsp;
				<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" />
				<input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" />
				&nbsp;
				<input type="button" id="buttonNewWorkItem" value="Add Task" disabled="disabled" />
				<input type="button" id="buttonNewRequest" value="Add Work Request" disabled="disabled" style="display: none;" />
				&nbsp;
   				<input type="button" id="buttonNewSR" value="Add SR"/>
            </td>--%>



		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdWorkload" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" HeightModifier="68" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
		<Services>
			<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
		</Services>
	</asp:ScriptManager>

	<script id="jsAJAX" type="text/javascript">

		function verifyItemExists(itemID, type) {
			//PageMethods.ItemExists(itemID, type, function (result) { verifyItemExists_done(itemID, type, result); }, function (result) { verifyItemExists_done(itemID, type, false); });
			WorkloadWebmethods.ItemExists(+itemID, type, function (result) { verifyItemExists_done(itemID, type, result); }, function (result) { verifyItemExists_done(itemID, type, false); });
		}

		function verifyItemExists_done(itemID, type, exists) {
			if (exists && exists.toUpperCase() == "TRUE") {
				switch (type) {
					case 'Work Item':
						editWorkItem(itemID);
						break;
					case 'Work Request':
						editWorkRequest(itemID);
						break;
				}
			}
			else {
				MessageBox('Could not find ' + type + ' # ' + itemID);
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
	</script>
	<script id="jsEvents" type="text/javascript">
		var _pageUrls;
		var _selectedId = 0, _myView = 0;
		var _canViewRequest = false, _canViewSR = false, _canViewWorkItem = false;
		var _canEditRequest = false, _canEditSR = false, _canEditWorkItem = false;
		var _displayedStatuses = '', _selectedStatuses = '';
		var _displayAssigned = '', _selectedAssigned = '';

		var _selectedUserTypes = '', _UserDDLChange = '';

		var _parentColumns = '', _childColumns = '';
		var _selectedColumnOrder = '', _defaultColumnOrder = ''
			, _selectedColumnOrder_Child = '', _defaultColumnOrder_Child = ''
			, _selectedRollupGroup = 'Priority'
			, _selectedRankSortType = 'Tech';

		function refreshPage(preservePageNum) {
			updateSelectedStatuses();
			updateSelectedAssigned();

			updateUserTypes();

			var qs = document.location.href;

			qs = editQueryStringValue(qs, 'RefData', 1); 
			qs = editQueryStringValue(qs, 'ShowArchived', ($('#chkShowArchived').is(':checked') ? 1 : 0));
			qs = editQueryStringValue(qs, 'ShowClosed', ($('#chkShowClosed').is(':checked') ? 1 : 0));
			qs = editQueryStringValue(qs, 'ShowBacklog', ($('#chkShowBacklog').is(':checked') ? 1 : 0));
			qs = editQueryStringValue(qs, 'SelectedStatuses', _selectedStatuses);
			//qs = editQueryStringValue(qs, 'SelectedWorkTypes', _selectedWorkTypes);
			qs = editQueryStringValue(qs, 'SelectedAssigned', _selectedAssigned);

			qs = editQueryStringValue(qs, 'SelectedUserTypes', _selectedUserTypes);

			qs = editQueryStringValue(qs, 'UserDDLChange', _UserDDLChange);

			_UserDDLChange = 'no';


			if (preservePageNum && (preservePageNum == true || preservePageNum == 1)) {
				var pageIndex = '<%=this.grdWorkload.PageIndex %>';
				qs = editQueryStringValue(qs, 'PageIndex', pageIndex);
			}
			else {
				qs = editQueryStringValue(qs, 'PageIndex', 0);
			}

			qs = editQueryStringValue(qs, 'ShowMetrics', $('#divMetrics').is(':visible'));

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function loadMetrics() {
			$('#divMetrics').text('Loading metrics...');

			var SelectedStatus = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');  // Will look like > 80,72,1,2,3,4... So "wrap" in stored proc.
  			var SelectedUsers = $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('getSelects');

		    PageMethods.LoadMetrics("'" + SelectedStatus + "'" , "'" + SelectedUsers + "'", '<%=this._showArchived %>', '<%=this._myData %>', loadMetrics_done, function () { $('#divMetrics').text('Failed to load metrics.'); });
		}

	    function loadMetrics_done(result) {
			var dt = jQuery.parseJSON(result);

			if (dt == null || dt.length == 0) {
				$('#divMetrics').text('No data found.');
			}
			else {
				var totalOnHold = 0, totalOpen = 0, totalAwaitingClosure = 0, totalClosed = 0;
				var totalTaskOnHold = 0, totalTaskOpen = 0, totalTaskAwaitingClosure = 0, totalTaskClosed = 0;
				var tbl = '<table cellpadding="3" cellspacing="0" style="display:inline-block; border-top:1px solid grey; border-left:1px solid grey;">';
				var tblTask = '<table cellpadding="3" cellspacing="0" style="display:inline-block; border-top:1px solid grey; border-left:1px solid grey; vertical-align:top;">';
				$.each(dt, function (rowIndex, row) {
					if (rowIndex == 0) {
						tbl += '<tr>';
						tblTask += '<tr>';
						$.each(this, function (column, value) {
							if (column == 'Priority') {
								tbl += '<td class="metricsHeader" style="border-bottom:1px solid grey; border-right:1px solid grey;"><b>Tasks</b></td>';
								tblTask += '<td class="metricsHeader" style="border-bottom:1px solid grey; border-right:1px solid grey;"><b>Sub-Tasks</b></td>';
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
								tblTask += '<td ' + span + ' class="metricsHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;' + display + '"><b>' + title + '</b></td>';
							}
						})
						tbl += '</tr><tr>';
						tblTask += '</tr><tr>';
						$.each(this, function (column, value) {
							tbl += '<td class="metricsSubHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;"><b>' + column + '</b></td>';
							tblTask += '<td class="metricsSubHeader" style="text-align:center; border-bottom:1px solid grey; border-right:1px solid grey;"><b>' + (column == 'Priority' ? '' : column) + '</b></td>';
						})
						tbl += '</tr>';
						tblTask += '</tr>';
					}

					tbl += '<tr>';
					if (row.Priority.indexOf('Task TOTAL') > -1) tblTask += '<tr>';
					$.each(this, function (column, value) {
						if (row.Priority.indexOf('Task TOTAL') > -1) {
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
						}
						else {
							if (row.Priority.indexOf('TOTAL') > -1) {
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
					if (row.Priority.indexOf('Task TOTAL') > -1) tblTask += '</tr>';
					tbl += '</tr>';
				});
				tbl += '</table>';

				tbl = tbl.replace('On Hold (0)', 'On Hold (' + totalOnHold + ')');
				tbl = tbl.replace('Open (0)', 'Open (' + totalOpen + ')');
				tbl = tbl.replace('Awaiting Closure (0)', 'Awaiting Closure (' + totalAwaitingClosure + ')');
				tbl = tbl.replace('Closed (0)', 'Closed (' + totalClosed + ')');
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

		function buttonToggleMetrics_click() {
			$('#buttonToggleMetrics').prop('value', ($('#divMetrics').is(':visible') ? 'Show Metrics' : 'Hide Metrics'));
			$('#divMetrics').slideToggle(function() { 
				resizeGrid();
				//positionStatusOptions(); 
			});
			
		}

		function resizeGrid() {
			try { resizePageElement($('#<%=this.grdWorkload.ClientID %>_BodyContainer').attr('id'), 23); } catch (e) { }
		}

	    function reloadChildGrids() {

			var frame = {};
			var d = {};
			var src = '', qs = '';

			$('#<%=this.grdWorkload.ClientID %>_Grid').find('iframe[name*="frameWorkItem"]').each(function () {
				frame = $(this)[0];
				d = {};
				qs = '';

				try {
					src = $(frame).attr('src');
					if (src == "javascript:''") {
						return true;
					}
					
					if (frame) {
						if (frame.refreshPage) {
							d = frame.refreshPage(_selectedStatuses, _selectedAssigned, _selectedUserTypes, /*_selectedWorkTypes,*/ ($('#chkShowClosed').is(':checked') ? 1 : 0));
						}
						else if (frame.contentWindow.refreshPage) {
						    d = frame.contentWindow.refreshPage(_selectedStatuses, _selectedAssigned, _selectedUserTypes, /*_selectedWorkTypes,*/ ($('#chkShowClosed').is(':checked') ? 1 : 0));
						}
					}
				} catch (e) {
					var msg = e.message;
				}
			});
		}

		function chkShowClosed_change() {
			var show = $('#chkShowClosed').prop('checked');

			var arrStatuses = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');
			var idxClosed = $.inArray('10', arrStatuses); //temporary - id of Closed = 10
			var idxApprovedClosed = $.inArray('70', arrStatuses); //temporary - id of Approved/Closed = 70

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

			$('#<%=this.ddlStatus.ClientID %>').multipleSelect('setSelects', arrStatuses);
			$('#<%=this.ddlStatus.ClientID %>').multipleSelect('refresh');
		    updateSelectedStatuses();
		    updateSelectedAssigned();

		    refreshPage();
		}
		
	    function chkShowBacklog_change() {
	        var checked = $('#chkShowBacklog').prop('checked');
	        var arrAssigned = $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('getSelects');

	        var arrUserTypes = $('#<%=this.ddlUserTypeFilters.ClientID %>').multipleSelect('getSelects');


	        var idxBacklog = $.inArray('69', arrAssigned); //temporary - id of backlog = 69
	        if (idxBacklog == -1) {
	            arrAssigned.push('69');
	        }
	        else if (idxBacklog > -1) {
	            arrAssigned.splice(idxBacklog, 1);
	        }
	        $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('setSelects', arrAssigned);
            $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('refresh');

	        $('#<%=this.ddlUserTypeFilters.ClientID %>').multipleSelect('setSelects', arrUserTypes);
            $('#<%=this.ddlUserTypeFilters.ClientID %>').multipleSelect('refresh');


	        updateSelectedStatuses();
	        updateSelectedAssigned();
	        refreshPage();
		}

		function imgExport_click() {
			var url = window.location.href;
			url = editQueryStringValue(url, 'Export', true);
			url = editQueryStringValue(url, 'ShowClosed', ($('#chkShowClosed').is(':checked') ? 1 : 0));
			url = editQueryStringValue(url, 'ShowBacklog', ($('#chkShowBacklog').is(':checked') ? 1 : 0));

			alert('Generating report. Watch for Save dialog on bottom of page.');

			window.open('Loading.aspx?Page=' + url);
		}

		function buttonNewWorkItem_click() {
			if (parent.ShowFrameForWorkloadItem) {
				parent.ShowFrameForWorkloadItem(true, 0, 0, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Work Item';
				url = _pageUrls.Maintenance.WorkItemEditParent;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
			    _selectedId = $(row).attr('itemID');  // Always a 0 
			}
		}

		function imgShowHideWorkItems_click(sender, direction, id) {
			try {

			    if (id == "0" || id == "ALL") {
					var rowNum = '0';

					$('[Name="img' + direction + '"]').each(function () {
						rowNum = $(this).attr('rowNum');
						if (rowNum && +rowNum > 0) {
							imgShowHideChildren_click(this, direction, rowNum);
						}
					});
				}

				if (direction.toUpperCase() == "SHOW") {
					//show row/div with child grid frame
					//get frame and pass url(if necessary)
					var td;

					$(sender).closest('tr').each(function () {
						var currentRow = $(this);
						var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
						var childType = 'WorkItem';
						$(row).show();

						td = $('td:eq(<%=this.DCC == null || this.DCC["X"] == null ? 0 : this.DCC["X"].Ordinal + 1%>)', row)[0];
					    childType = $(row).attr('childType');
						loadChildGrid(td, id, childType, row);
					});
       			    <%=this.grdWorkload.ClientID %>.RedrawGrid();

				}
				else {
					$('tr[Name="gridChild_' + id + '"]').hide();
				}

				$(sender).hide();
				$(sender).siblings().show();

			} catch (ex) {
			    //alert("Error in drill in. " + ex.message);
			}
		}

	    function loadChildGrid(td, id, childType, row) {
	        var url = '';

			if (childType == 'SR') {
				url += _pageUrls.Maintenance.WorkloadGrid_SRs
					+ window.location.search;
			}
			else {url += 'Workload_CrosswalkGrid_Items.aspx'+ window.location.search;}

			updateSelectedStatuses();
			updateSelectedAssigned()

			updateUserTypes();

		    //updateSelectedWorkTypes();

			url = editQueryStringValue(url, 'columnOrder', _selectedColumnOrder_Child);
			url = editQueryStringValue(url, 'childColumns', _childColumns);
			url = editQueryStringValue(url, 'parentColumns', _parentColumns); 
			url = editQueryStringValue(url, 'statuses', _selectedStatuses);
			//url = editQueryStringValue(url, 'workTypes', _selectedWorkTypes);
			url = editQueryStringValue(url, 'Assigned', _selectedAssigned);
			url = editQueryStringValue(url, 'ShowClosed', ($('#chkShowClosed').is(':checked') ? 1 : 0));
			url = editQueryStringValue(url, 'ShowBacklog', ($('#chkShowBacklog').is(':checked') ? 1 : 0));

			url = editQueryStringValue(url, 'userTypes', _selectedUserTypes);

			url = editQueryStringValue(url, 'RefData', 0);

			var filters = '';
			if (row.attr('filters')) {
				filters = row.attr('filters');
			}
			url += '&rowFilters=' + filters;

			$('iFrame', $(td)).each(function () {
			    var src = $(this).attr('src');
                // Removed this check.  Was casuing, at times, no sub grid to load.  12-15-2016 - Put back in.
				if (src == "javascript:''") {
					$(this).attr('src', 'Loading.aspx?Page=' + url);
				}
				$(this).show();
			});
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
				frame.style.height = fPageHeight + 'px';
			});
		}


		function formatGoTo(obj) {
			var text = $(obj).val();

			if (/[^0-9]|^0+(?!$)/g.test(text)) {
				$(obj).val(text.replace(/[^0-9]|^0+(?!$)/g, ''));
			}
		}


		function buttonGoToWorkItem_click() {
			var recordID = $('#txtWorkItem').val();

			if (recordID > 0) {
				verifyItemExists(recordID, 'Work Item');
			}
			else {
				MessageBox('Please enter a Work Item #.');
			}
		}

		function buttonGoToWorkRequest_click() {
			var requestID = $('#txtWorkRequest').val();

			if (requestID > 0) {
				verifyItemExists(requestID, 'Work Request');
			}
			else {
				MessageBox('Please enter a Work Request #.');
			}
		}


		function editWorkItem(workItemID) {

		    // 4-14-2017 - Added these
		    updateSelectedStatuses();
		    updateSelectedAssigned();

			if (parent.ShowFrameForWorkloadItem) {
			    parent.ShowFrameForWorkloadItem(false, workItemID, workItemID, true, _selectedStatuses, _selectedAssigned);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Work Item - [' + workItemID + ']';
				url = _pageUrls.Maintenance.WorkItemEditParent
					+ '?WorkItemID=' + workItemID

                    // 4-14-2017 - Added these
		            + '&SelectedStatuses=' + _selectedStatuses
		            + '&SelectedAssigned=' + _selectedAssigned;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function editWorkRequest(requestID) {
			if (parent.ShowFrameForWorkRequest) {
				parent.ShowFrameForWorkRequest(false, requestID, requestID, true);
			}
			else {
				var title = '', url = '';
				var h = 768, w = 1024;

				title = 'Work Request - [' + requestID + ']';
				url = _pageUrls.Maintenance.WorkRequestEditParent
					+ '?WorkRequestID=' + requestID;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
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
		}

		function ddlStatus_close() {
		    <%--			if('<%=this.ParentColumns.ToUpper().Contains("STATUS")%>'.toUpperCase() == "TRUE") {
				refreshPage();
			}

			if(_displayedStatuses != _selectedStatuses) {
				_displayedStatuses = _selectedStatuses;

				var arrStatuses = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');
				var idxClosed = $.inArray('10', arrStatuses); //temporary - id of Closed = 10
				var idxApprovedClosed = $.inArray('70', arrStatuses); //temporary - id of Approved/Closed = 70

				if (idxClosed != -1 && idxApprovedClosed != -1) $('#chkShowClosed').prop('checked', true);

				reloadChildGrids();
			}--%>
		    refreshPage();
		}

		function updateSelectedStatuses() {
			var arrSelections = $('#<%=this.ddlStatus.ClientID %>').multipleSelect('getSelects');
			
			_selectedStatuses = arrSelections.join(',');
		}


<%--		//function ddlWorkType_change() {
		//	updateSelectedWorkTypes();
		//}
		

		function ddlWorkType_open() {
			var arrSelections = $('#<%=this.ddlWorkType.ClientID %>').multipleSelect('getSelects');
			
			_selectedWorkTypes = arrSelections.join(',');
			_displayedWorkTypes = _selectedWorkTypes;
		}

		function ddlWorkType_close() {
			if('<%=this.ParentColumns.ToUpper().Contains("WORKTYPE")%>'.toUpperCase() == "TRUE") {
				refreshPage();
			}
			if(_displayedWorkTypes != _selectedWorkTypes) {
				_displayedWorkTypes = _selectedWorkTypes;

				reloadChildGrids();
			}
		}

		function updateSelectedWorkTypes() {
			var arrSelections = $('#<%=this.ddlWorkType.ClientID %>').multipleSelect('getSelects');
			
			_selectedWorkTypes = arrSelections.join(',');
		}--%>

	    function ddlAssigned_change() {
	        updateSelectedAssigned();
	    }

	    function ddlAssigned_open() {
			var arrSelections = $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('getSelects');
			
			_selectedAssigned = arrSelections.join(',');
			_displayedAssigned = _selectedAssigned;
		}

	    function ddlAssigned_close() {
	        <%--			if('<%=this.ParentColumns.ToUpper().Contains("AssignedID")%>'.toUpperCase() == "TRUE") {
				refreshPage();
			}
	        if (_displayedAssigned != _selectedAssigned) {
	            _displayedAssigned = _selectedAssigned;

				reloadChildGrids();
			}--%>
	        refreshPage();
		}

		function updateSelectedAssigned() {
			var arrSelections = $('#<%=this.ddlAssigned.ClientID %>').multipleSelect('getSelects');
		    _selectedAssigned = arrSelections.join(',');

		    _UserDDLChange = 'yes';
		}




	    function ddlUserTypeFilters_change() {
	        updateUserTypes();
	    }

	    function ddlUserTypeFilters_open() {
	        var arrUserTypes = $('#<%=this.ddlUserTypeFilters.ClientID %>').multipleSelect('getSelects');
	        _selectedUserTypes = arrUserTypes.join(',');
        }

	    function ddlUserTypeFilters_close() {
	        updateUserTypes();
	        refreshPage();
	    }

		function updateUserTypes() {
			var arrUserTypes = $('#<%=this.ddlUserTypeFilters.ClientID %>').multipleSelect('getSelects');
		    _selectedUserTypes = arrUserTypes.join(',');
		}


	</script>

	<script id="jsMenus" type="text/javascript">

		function relatedItems_click(action, type) {
			var h = 800, w = 1500;
			var name = "", title = "";
			var url = "";

			if (action.toUpperCase() == 'QM') {
				url = "";
				name = "QM";
				title = "QM";
				switch (type.toUpperCase()) {
					case "ATTRIBUTE":
						name += type;
						title += " " + type;
						url = "WorkItemGrid_QM.aspx";
						break;
				}
			}
			else if (action.toUpperCase() == 'MASSCHANGE') {
				url = "WorkItem_MassChange.aspx" + window.location.search;
				name = "WorkItemMassChange";
				title = "Work Item Mass Change";
				url = editQueryStringValue(url, 'FieldName', type);

				h = 200;
				w = 500;
			}

			var url = 'Loading.aspx?Page=' + url;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow(name, title, url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}

			//var w = window.open(url);
		}

		function openMenuItem(url) {
			if (url.indexOf("relatedItems") > 0) {
				var str = url.split("'");
				relatedItems_click(str[1], str[3]);
			}
		}

	</script>

	<script id="jsSettings" type="text/javascript">

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

				var sURL = 'SortOptions.aspx?GridName=WORKLOAD_CROSSWALKGRID.ASPX&sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				//var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
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
				updateSelectedStatuses();
				updateSelectedAssigned();

				updateUserTypes();
				                                                                                                                                                                                                                                    
				var qs = document.location.href;
				qs = editQueryStringValue(qs, 'sortOrder', sortValue);
				qs = editQueryStringValue(qs, 'sortChanged', 'true');
				qs = editQueryStringValue(qs, 'RefData', 1);
				qs = editQueryStringValue(qs, 'ShowArchived', ($('#chkShowArchived').is(':checked') ? 1 : 0));
				qs = editQueryStringValue(qs, 'ShowClosed', ($('#chkShowClosed').is(':checked') ? 1 : 0));
				qs = editQueryStringValue(qs, 'SelectedStatuses', _selectedStatuses);
			    qs = editQueryStringValue(qs, 'SelectedAssigned', _selectedAssigned);
				//qs = editQueryStringValue(qs, 'SelectedWorkTypes', _selectedWorkTypes);
				qs = editQueryStringValue(qs, 'PageIndex', 0);
				qs = editQueryStringValue(qs, 'ShowMetrics', $('#divMetrics').is(':visible'));

				qs = editQueryStringValue(qs, 'UserDDLChange', _UserDDLChange);

				_UserDDLChange = 'no';

				document.location.href = 'Loading.aspx?Page=' + qs;
			}
			catch (e) {
			}
		}

		function imgSettings_click() {

			openSettings();
		}

		function imgSettingsAdv_click(){

		    openSettingsAdv();

		}

		function openSettings() {
			var url = '', title = 'Crosswalk Parameters';
			var h = 650, w = 750;

			url = 'CrosswalkParamContainer.aspx?parentColumns=' + _parentColumns
				+ '&childColumns=' + _childColumns
				+ '&rollupGroup=' + _selectedRollupGroup
				+ '&rankSortType=' + _selectedRankSortType;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('CrosswalkParams', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function openSettingsAdv() {
		    var url = '', title = 'Crosswalk Parameters';
		    var h = 830, w = 800;

		    url = 'CrosswalkParametersSections.aspx?parentColumns=' + _parentColumns
				+ '&childColumns=' + _childColumns
				+ '&rollupGroup=' + _selectedRollupGroup
				+ '&rankSortType=' + _selectedRankSortType;

		    //open in a popup
		    var openPopup = popupManager.AddPopupWindow('CrosswalkParams', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
		    if (openPopup) {
		        openPopup.Open();
		    }

            // Using this allows for the window to be gragged outside of parent AND onto other monitors.
		    //var reportWindow = window.open(url, 'CrosswalkParams', 'scrollbars=no, width=800, height=900, top=10, left=400')
		}

		function ApplySettings() {
			var qs = document.location.href;

			qs = editQueryStringValue(qs, 'RefData', 1);
			qs = editQueryStringValue(qs, 'parentColumns', _parentColumns);
			qs = editQueryStringValue(qs, 'rollupGroup', _selectedRollupGroup);
			qs = editQueryStringValue(qs, 'childColumns', _childColumns);
			qs = editQueryStringValue(qs, 'rankSortType', _selectedRankSortType);

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function parseColumnString(columnOrder) {
			var columnObjects = [];
			var columnParts = [];
			var columnNames = [];
			var show = false;

			//dbname|displayname|visible|orderable|groupname|viewable
			columnObjects = columnOrder.trim().split('~');
			for (var i = 0; i < columnObjects.length; i++) {
				if (columnObjects[i].trim() == '') {
					continue;
				}

				columnParts = columnObjects[i].trim().split('|');
				if (columnParts.length >= 3) {
					if (columnParts[2] == 'true' && columnParts[1].trim() != '' && columnParts[1].trim() != '&nbsp;') {
						columnNames.push(columnParts[1].replace(' ', ''));
					}
				}
			}

			return columnNames.join(',');
		}

		function getSelectedColumnOrder(blnDefault) {
			try {
				if (blnDefault) {
					return _defaultColumnOrder;
				}
				else {
					return _selectedColumnOrder;
				}
			}
			catch (e) {
				return "";
			}
		}

		function getSelectedColumnOrderSub(blnDefault) {
			try {
				if (blnDefault) {
					return _defaultColumnOrder_Child;
				}
				else {
					return _selectedColumnOrder_Child;
				}
			}
			catch (e) {
				return "";
			}
		}

		function updateColumnOrder(columnOrder, columnOrderSub, rollupGroup, rankSortType) {
			try {

				_selectedColumnOrder = columnOrder;
				_parentColumns = parseColumnString(_selectedColumnOrder);
				_selectedColumnOrder_Child = columnOrderSub;
				_childColumns = parseColumnString(_selectedColumnOrder_Child);
				if (rollupGroup != '') {
					_selectedRollupGroup = rollupGroup;
				}
				if (rankSortType == '') {
					_selectedRankSortType = 'Tech';
				}
				else {
					_selectedRankSortType = rankSortType;
				}

				_columnsChanged = true;

			}
			catch (e) {
				MessageBox('updateColumnOrder:\n' + e.number + '\n' + e.message);
			}
		}

		function getSelectedRollupGroup() {
			return _selectedRollupGroup;
		}

		function updateRollupColumns(columns) {
			try {
				_selectedRollupGroup = columns;

				_columnsChanged = true;
			}
			catch (e) {
				MessageBox('updateRollupColumns:\n' + e.number + '\n' + e.message);
			}
		}
		
		function getSelectedRankSortType() {
			return _selectedRankSortType;
		}

		function updateDefaultRankSortType(sortType) {
			try {
				if (!sortType || sortType == '') {
					_selectedRankSortType = 'Tech';
				}
				else {
					_selectedRankSortType = sortType;
				}

				_columnsChanged = true;
			}
			catch (e) {
				MessageBox('updateDefaultRankSortType:\n' + e.number + '\n' + e.message);
			}
		}

	</script>
	
	<script id="jsInit" type="text/javascript">

		function resizePage() {
		    positionStatusOptions();
		}

		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				_canEditSR = ('<%=this.CanEditSR.ToString().ToUpper()%>' == 'TRUE');
				_canEditWorkItem = ('<%=this.CanEditWorkItem.ToString().ToUpper()%>' == 'TRUE');
				_canViewSR = (_canEditSR || ('<%=this.CanViewSR.ToString().ToUpper()%>' == 'TRUE'));
				_canViewWorkItem = (_canEditWorkItem || ('<%=this.CanViewWorkItem.ToString().ToUpper()%>' == 'TRUE'));

				_selectedColumnOrder = '<%=this.SelectedColumnOrder %>';
				_defaultColumnOrder = '<%=this.DefaultColumnOrder %>';
				_selectedColumnOrder_Child = '<%=this.SelectedColumnOrder_Child %>';
				_defaultColumnOrder_Child = '<%=this.DefaultColumnOrder_Child %>';
				_selectedRollupGroup = '<%=this.RollupGroup %>';
				_selectedRankSortType = '<%=this.DefaultSortType %>';

				_parentColumns = '<%=this.ParentColumns %>';
				_childColumns = '<%=this.ChildColumns %>';

			} catch(e) { 
			    //alert("Error in initVariables = " + e.message);
			}
		}

	    function buttonNewSR_click() {
	        _selectedId = localStorage.getItem("_selectedId");
	        
	        //if (_selectedId > 0) {
	            var title = 'New SR';
	            var url = 'Loading.aspx?Page=SR_Add.aspx'
		                + '?workItemID=' + _selectedId;

	            var h = 700, w = 850;

	            //open in a popup
	            var openPopup = popupManager.AddPopupWindow('NewSR', title, url, h, w, 'PopupWindow', this);
	            if (openPopup) {
	                openPopup.Open();
	            }
	        //}
	        //else {
	        //    MessageBox('Please select a task.');
            //}
        }


	    function initEvents(){

			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(false); });
			$('#imgSort').click(function () { imgSort_click(); });
			$('#imgSettings').click(function () { imgSettings_click(); });

			$('#imgSettingsAdv').click(function () { imgSettingsAdv_click(); });

			//$('#buttonNewSR').attr('disabled', true);

			if (_canEditSR) {
				$('#buttonNewSR').attr('disabled', false);
				$('#buttonNewSR').click(function (event) { buttonNewSR_click(); return false; });
			}

			if (_canEditRequest) {
				$('#buttonNewRequest').attr('disabled', false);
				$('#buttonNewRequest').click(function (event) { buttonNewRequest_click(); return false; });
			}
			if (_canEditWorkItem) {
				$('#buttonNewWorkItem').attr('disabled', false);
				$('#buttonNewWorkItem').click(function (event) { buttonNewWorkItem_click(); return false; });
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			$('#buttonToggleMetrics').click(function () { buttonToggleMetrics_click(); return false; });

			$('#chkShowClosed').change(function () { chkShowClosed_change(); return false; });
			$('#chkShowClosed').prop('checked', ('<%=this.ShowClosed.ToString().ToUpper() %>' == 'TRUE'));

		    $('#chkShowBacklog').change(function () { chkShowBacklog_change(); return false; });
            $('#chkShowBacklog').prop('checked', ('<%=this.ShowBacklog.ToString().ToUpper() %>' == 'TRUE'));

			$('#chkShowArchived').click(function () { $('#imgRefresh').trigger('click'); });
			$('#chkShowArchived').prop('checked', '<%=this._showArchived %>' == '1');

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
		}

		function initControls() {
			try {
				$('.statusSelect').multipleSelect( {
					placeholder: 'Default(NOT Closed)'
					,width: 'undefined'
					,onOpen: function() {
						ddlStatus_open();
					}
					,onClose: function() {
						ddlStatus_close();
					}
				} ).change(function () { ddlStatus_change(); });

				//$('.workTypeSelect').multipleSelect( {
				//	placeholder: 'Default(ALL)'
				//	,width: 'undefined'
				//	,onOpen: function() {
				//		ddlWorkType_open();
				//	}
				//	,onClose: function() {
				//		ddlWorkType_close();
				//	}
				//});

			$('.AssignedSelect').multipleSelect( {
			    placeholder: 'Default(ALL)'
                ,width: 'undefined'
                ,onOpen: function() {
                    ddlAssigned_open();
                }
                ,onClose: function() {
                    ddlAssigned_close();
                 }
			}).change(function () { ddlAssigned_change(); });

			$('.UserTypeSelected').multipleSelect( {
			    placeholder: 'Default(ALL)'
                ,width: 'undefined'
                ,onOpen: function() {
                    ddlUserTypeFilters_open();
                }
                ,onClose: function() {
                    ddlUserTypeFilters_close();
                }
			}).change(function () { ddlUserTypeFilters_change(); });



        } catch (e) {
	
			}
		}

		$(document).ready(function () {
			initVariables();
			initControls();
			initEvents();

			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');

			
			if ('<%=this.IsParentConfigured.ToString().ToUpper() %>' == 'FALSE') {
				openSettings(_parentColumns, _childColumns, _selectedRollupGroup);
			}

			if ('<%=this.ShowMetrics.ToString().ToUpper() %>' == 'FALSE') {
				$('#buttonToggleMetrics').prop('value', 'Show Metrics');
				$('#divMetrics').hide();
			}
			else {
				$('#buttonToggleMetrics').prop('value', 'Hide Metrics');
				$('#divMetrics').show();
			}

		    loadMetrics();

		    $('.gridPager').css({
		        'position': 'fixed',
		        'bottom': '0'
		    });
		});
	</script>

</asp:Content>