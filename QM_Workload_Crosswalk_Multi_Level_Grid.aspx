﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QM_Workload_Crosswalk_Multi_Level_Grid.aspx.cs" Inherits="QM_Workload_Crosswalk_Multi_Level_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Multi-Level Crosswalk</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <asp:HiddenField ID="itisettings" runat="server" />
    <span>Multi-Level Crosswalk</span>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <table id="tblGridView" runat="server">
        <tr>
            <td>
                <span style="margin-left: 10px">Gridview: </span>
                <asp:DropDownList ID="ddlGridview" runat="server" Style="font-size: 12px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnOpenWorkItem" value="Go to Work Task #" style="vertical-align: middle; display: none;" />
				<input type="text" id="txtWorkItem" maxlength="11" size="8" style="display: none;" />
                <input type="button" id="btnAddTask" value="Add Primary Task" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnAddSubTask" value="Add Subtask" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
            
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true" EmptyDataText="No Data"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <script src="Scripts/multiselect/jquery.multiple.select.js?=2" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css?=2" />

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
        <Services>
			<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
		</Services>
    </asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedStatuses = '';
        var _selectedAssigned = '';
        var _selectedWorkItemID = '';
        var _UserDDLChange = '';
        var arrSelectedEntity = [];
        var viewDataChanged = false;
        var _businessReview = false;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function imgSettings_click() {
            openSettings();
        }

        function imgExport_click() {
            var url = window.location.href;
            url = editQueryStringValue(url, 'Export', true);
            window.location.href = url;
        }

        function chkAll(obj) {
            var $obj = $(obj);
			    var blnChecked = $(obj).find('input').is(':checked')
                $('.masschange').find('input[type=checkbox]').prop('checked', blnChecked);
                //$('.masschange').find('input[type=checkbox]').parent().attr('fieldChanged', '1');
                
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

        function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 0;
            var addLeft = 0;
            if (parseInt(_currentLevel) > 1) {
                addtop = 30;
                addLeft = 5;
            }else{
                addtop = 57;
                addLeft = 364;
            }

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

        function relatedItems_click(action, type) {
            switch (action) {
                case 'MassChange':
                    btnMassChange_click();
                    break;
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function btnMassChange_click() {
            arrSelectedEntity.length = 0;
            $('.masschange').each(function () {
                var $obj = $(this);
                var objVal = '';
                if ($obj.find('input[type=checkbox]').is(':checked')) {
                    var $obj = $(this);
                    if ($obj.attr('strFieldID').indexOf("ID") <= 0) {
                        if ($.inArray($obj.attr('strFieldID'), arrSelectedEntity) == -1) arrSelectedEntity.push({ 'strEntity': $obj.attr('strEntity'), 'strField': $obj.attr('strField'), 'strFieldID': $obj.attr('strFieldID') });
                    }

                }
            });

            var nWindow = 'EntityMassChange';
            var nTitle = 'Mass Change';
            var nHeight = 400, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.MassChangeWizard + window.location.search;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function ddlGridView_change() {
            var gridViewIndex = $("#<%=ddlGridview.ClientID %> option:selected").index();
            var gridViewText = $("#<%=ddlGridview.ClientID %> option:selected").text();
            var sectionsXml = $("#<%=ddlGridview.ClientID %> option:selected").attr("SectionsXML");

            PageMethods.UpdateSession(gridViewIndex, gridViewText, sectionsXml, ddlGridView_Change_Done, on_error);
        }

        function ddlGridView_Change_Done(results) {
            if(results) refreshPage(false);
        }

        function verifyItemExists(itemID, taskNumber, type) {
            WorkloadWebmethods.ItemExists(itemID, taskNumber, type, function (result) { verifyItemExists_done(itemID, taskNumber, type, result); }, function (result) { verifyItemExists_done(itemID, taskNumber, type, false); });
        }

        function verifyItemExists_done(itemID, taskNumber, type, exists) {
            if (exists && exists.toUpperCase() == 'TRUE') {
                switch (type) {
                    case 'Primary Task':
                        openWorkItem(itemID, '', '', '0');
                        break;
                    case 'Subtask':
                        PageMethods.WorkItem_TaskID_Get(itemID, taskNumber, function (result) { openWorkItem(itemID, taskNumber, result, 1); });
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

        function btnOpenWorkItem_click() {
            var workItemID = $('#txtWorkItem').val();
            if (workItemID.length > 0) {
                if (workItemID.indexOf('-') > -1) {
                    var taskNumber = workItemID.slice(workItemID.indexOf('-') + 1);
                    workItemID = workItemID.slice(0, workItemID.indexOf('-'));
                    verifyItemExists(workItemID, taskNumber, 'Subtask');
                } else {
                    verifyItemExists(workItemID, -1, 'Primary Task');
                }
            }
            else {
                MessageBox('Please enter a Work Task #.');
            }
        }

        function btnAddTask_click() {
            openWorkItem(0, '', '', '0');
        }

        function btnAddSubTask_click() {
            var nWindow = 'WorkSubTask';
            var nTitle = 'Subtask';
            var nHeight = 700, nWidth = 850;
            var nURL = _pageUrls.Maintenance.TaskEdit + '?workItemID=' + _selectedWorkItemID.split('-')[0] + '&taskID=0';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnSave_click() {
            var statusID = 0;
            var UnclosedSRTask = 0;
            var displayPrompt = false;

            $('table[id$=grdData_Grid] tr').each(function (i, row) {
                changed = false;
                statusID = 0;
                UnclosedSRTask = 0;

                if ($(this).attr('rowchanged') && $(this).attr('rowchanged') == '1') {
                    changed = true;
                }

                if (changed) {
                    statusID = $(this).find('select[fieldChanged="1"][field="Status"]').val();
                    UnclosedSRTask = $(this).find('td[field="Unclosed SR Tasks"]').text();

                    if (statusID == 10 && UnclosedSRTask == 1) displayPrompt = true;
                }
            });

            if (displayPrompt) {
                QuestionBox('Confirm SR Closed', 'All Work Tasks associated to a SR will be in a Closed Status. This will cause the SR to be set to Resolved. Would you like to proceed?', 'Save,Cancel', 'confirmSRUpdate', 300, 300, this);
            } else {
                save();
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

        function save() {
            try {
                var arrChanges = [];

                $('select[fieldChanged="1"][field="Status"]').each(function () {
                    var $obj = $(this);
                    if ($obj.val() === '10') {
                        $('select[work_item_id="' + $obj.attr("work_item_id") + '"][field="Assigned To Rank"]').val('31');
                        $('select[work_item_id="' + $obj.attr("work_item_id") + '"][field="Assigned To Rank"]').attr('fieldChanged', '1');
                        $('input[work_item_id="' + $obj.attr("work_item_id") + '"][field="Customer Rank"]').val(99);
                        $('input[work_item_id="' + $obj.attr("work_item_id") + '"][field="Customer Rank"]').attr('fieldChanged', '1');
                    }
                });

                $('input[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                    var $obj = $(this);

                    arrChanges.push({ 'workitemid': $obj.attr('work_item_id'), 'blnsubtask': $obj.attr('bln_sub_task'), 'field': $obj.attr('field'), 'value': $obj.val() });
                });

                if (arrChanges.length > 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    var nJSON = '{update:' + JSON.stringify(arrChanges) + '}';

                    PageMethods.SaveChanges(nJSON, save_done, on_error);
                }
                else {
                    MessageBox('You have not made any changes.');
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                var fieldCount = $('input[fieldChanged="1"], select[fieldChanged="1"]').length;
                var rowCount = $('tr[rowChanged="1"]').length;

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                setTimeout(refreshPage(true), 1);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function openSettings() {
            if (parent.openSettings) {
                parent.openSettings();
                return;
            }

            var nTitle = 'Crosswalk Parameters';
            var nHeight = 800, nWidth = 500;
            var nURL = 'CrosswalkParametersSections.aspx';
            var openPopup = popupManager.AddPopupWindow('CrosswalkParams', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function ddlStatus_update() {
            var arrStatus = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');
			
            _selectedStatuses = arrStatus.join(',');

            resizeFrame();
        }

        function ddlStatus_close() {
            refreshPage(false);
        }

        function ddlAffiliated_update() {
            var arrAssigned = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');
			
            _selectedAssigned = arrAssigned.join(',');
            _UserDDLChange = 'yes';
            resizeFrame();
        }

        function ddlAffiliated_close() {
            refreshPage(false);
        }

        function chkBusinessReview_change(obj) {
            var $obj = $(obj);

            _businessReview = $obj[0].checked;
            lblMessage.show();
            lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
        }

        function displayAllRows(obj) {
    	    var $obj = $(obj);

    	    $('#<%=this.grdData.ClientID %>_BodyContainer table td img').each(function () {
    	        $(this).click();
    	    });

    	    if ($obj.attr('title') == 'Expand') {
    	        $obj.attr('src', 'Images/Icons/minus_blue.png');
    	        $obj.attr('title', 'Collapse');
    	        $obj.attr('alt', 'Collapse');
    	    }
    	    else {
    	        $obj.attr('src', 'Images/Icons/add_blue.png');
    	        $obj.attr('title', 'Expand');
    	        $obj.attr('alt', 'Expand');

    	    }
        }

        function displayNextRow(obj) {
            var $obj = $(obj);

            if ($obj.attr('title') != 'Loading...') {
                var $row = $obj.closest('tr');
                var blnShow = true;

                if ($row.next().find('iframe').length == 0) {
                    $obj.attr('src', 'Images/Loaders/loader_2.gif');
                    $obj.attr('title', 'Loading...');
                    $obj.attr('alt', 'Loading...');
                    $obj.css('cursor', 'default');

                    var nURL = '';

                    if (_currentLevel == '<%=this.LevelCount %>') {
                        return;
                    }
                    else {
                        ddlStatus_update();
                        ddlAffiliated_update();
                        nURL = 'QM_Workload_Crosswalk_Multi_Level_Grid.aspx' + window.location.search;
                        nURL = editQueryStringValue(nURL, 'CurrentLevel', (parseInt(_currentLevel) + 1));
                        nURL = editQueryStringValue(nURL, 'SelectedStatuses', _selectedStatuses);
                        nURL = editQueryStringValue(nURL, 'SelectedAssigned', _selectedAssigned);
                        nURL = editQueryStringValue(nURL, 'GridPageSize', $('#<%=Master.FindControl("ddlItem5").ClientID %>').val());
                        nURL = editQueryStringValue(nURL, 'UserDDLChange', 'no');
                    }

                    var filters = [];
                
                    $.each($row.find('td'), function(i) {
                        var nText = $('.gridHeader:eq(1) th:eq(' + i + ')').text();
                        var nVal = encodeURIComponent($(this).text());

                        if (nText == 'WorkTask_ID' && $(this).next().text().indexOf(' - ') != -1) nVal = '-' + nVal;
                        if (nText.match(/_ID$/)) filters.push(nText + '=' + nVal);
                    });

                    nURL = editQueryStringValue(nURL, 'Filter', ('<%=this.Filter %>' != '' ? encodeURIComponent('<%=this.Filter %>|') : '') + filters.join('|'));
                    
                    var nHTML = '<tr>';
                    nHTML += '<td colspan=' + $row.find('td:visible').length + ' style="padding-top: 5px; border: none; border-bottom: 1px solid grey;">';
                    nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                    nHTML += '<tr>';
                    nHTML += '<td style="width: 15px; vertical-align: top;">';
                    nHTML += '<img src="Images/Icons/tree_branch.gif" alt="Child Grid" />';
                    nHTML += '</td>';
                    nHTML += '<td>';
                    nHTML += '<iframe src=' + nURL + ' width="100%" height="' + $row.height() + 'px" frameBorder="0"></iframe>';
                    nHTML += '</td>';
                    nHTML += '</tr>';
                    nHTML += '</table>';
                    nHTML += '</td>';
                    nHTML += '</tr>';

                    $(nHTML).insertAfter($row);
                }
                else {
                    if ($row.next().is(':visible')) blnShow = false;
                }

                if (blnShow) {
                    if ($obj.attr('title') == 'Expand') {
                        $obj.attr('src', 'Images/Icons/minus_blue.png');
                        $obj.attr('title', 'Collapse');
                        $obj.attr('alt', 'Collapse');
                    }

                    $row.next().show();
                }
                else {
                    $obj.attr('src', 'Images/Icons/add_blue.png');
                    $obj.attr('title', 'Expand');
                    $obj.attr('alt', 'Expand');
                    $row.next().hide();
                }

                resizeFrame();
            }
        }

        function lbEditWorkItem_click(recordId) {
            var UseLocal = false; //to use local storage enabling next/previous on task edit page, or to not, that is the question. 
            
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.storeChildFrame($('iframe'));
                obj.showFrameForEdit('TASK', false, recordId, true);
            }
            else {
                var title = '', url = '';
                var h = 700, w = 1400;

                title = 'Primary Task - [' + recordId + ']';
                url = _pageUrls.Maintenance.WorkItemEditParent
					+ '?WorkItemID=' + recordId
                    + '&UseLocal=True';

                var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
                if (openPopup) {
                    openPopup.Open();
                }
            }
        }

        function openWorkItem(workItemID, taskNumber, taskID, blnSubTask) {
            var nWindow = 'WorkTask';
            var nTitle = 'Primary Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;
            if (parseInt(workItemID) > 0) {
                nTitle += ' - [' + workItemID + ']';
                nURL += '?workItemID=' + workItemID;
            }

            if (blnSubTask == '1') {
                nWindow = 'WorkSubTask';
                nTitle = 'Subtask - [' + workItemID + ' - ' + taskNumber + ']';
                nHeight = 700, nWidth = 850;
                nURL = _pageUrls.Maintenance.TaskEdit + '?workItemID=' + workItemID + '&taskID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function row_click(obj) {
            if ($(obj).attr('workitem_id')) {
                _selectedWorkItemID = $(obj).attr('workitem_id');

                if (_selectedWorkItemID.indexOf(' - ') == -1 && parseInt(_selectedWorkItemID) > 0) {
                    $('#btnAddSubTask').prop('disabled', false);
                }
                else {
                    $('#btnAddSubTask').prop('disabled', false);
                }
            }
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            switch($obj.attr('field')) {
                case 'Customer Rank':
                case 'Bus. Rank':
                case 'Tech. Rank':
                    var nVal = $obj.val();
                    var blnNegative = nVal.indexOf('-') != -1 ? true : false;
                    
                    nVal = nVal.replace(/[^\d]/g, '');
                    
                    if (blnNegative) nVal = '-' + nVal;

                    $obj.val(nVal);
                    break;
            }

            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            switch($obj.attr('field')) {
                case 'Customer Rank':
                case 'Bus. Rank':
                case 'Tech. Rank':
                    if (nVal == '-') $obj.val('');
                    return;
            }

            $obj.val($.trim(nVal));
        }

        function refreshPage(blnRetainPageIndex) {
            ddlStatus_update();
            if (!viewDataChanged) ddlAffiliated_update();
            var nURL = document.location.href.replace('#', '');
            nURL = editQueryStringValue(nURL, 'MyData', parent.parent.$('#ddlView_Work option:selected').text() == 'My Data' ? true : false);
            nURL = editQueryStringValue(nURL, 'SelectedStatuses', _selectedStatuses);
            nURL = editQueryStringValue(nURL, 'SelectedAssigned', _selectedAssigned);
            nURL = editQueryStringValue(nURL, 'GridPageSize', $('#<%=Master.FindControl("ddlItem5").ClientID %>').val());
            nURL = editQueryStringValue(nURL, 'BusinessReview', _businessReview);
            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');
            nURL = editQueryStringValue(nURL, 'UserDDLChange', _UserDDLChange);
            _UserDDLChange = 'no';
            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }

        function resizeFrame() {
            var $grid = $('#<%=this.grdData.ClientID %>_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#<%=this.grdData.ClientID %>_BodyContainer table').height();
            var pagerHeight = $('#<%=this.grdData.ClientID %>_PagerContainer').is(':visible') ? $('#<%=this.grdData.ClientID %>_PagerContainer').height() : 0;

            if (parseInt(_currentLevel) > 1) {
                if ($('.ms-drop').is(':visible')) {
                    var qfHeight = $('.ms-drop').height() + 75;

                    if (bodyTableHeight < qfHeight) bodyTableHeight = qfHeight;
                }
                
                var nHeight = headerTop + bodyTableHeight + pagerHeight + 15;

                //Use quick filter container height if more than page height
                var $divQuickFilters = $('#divQuickFilters');
                if ($divQuickFilters.is(':visible')) {
                    var quickFilterHeight = $divQuickFilters.height();
                    if (quickFilterHeight > nHeight) {
                        nHeight = headerTop + quickFilterHeight + 10;
                    }
                }

                var nFrame = getMyFrameFromParent();

                $(nFrame).height(nHeight);
                resizeGrid();
                parent.resizeFrame();
            }
            else {
                resizeGrid();
                
                //iti_Tools ResizeGrid() doesn't work sometimes in certain environments
                var bodyHeight = $('#<%=this.grdData.ClientID %>_BodyContainer').height(); 
                if (bodyTableHeight < bodyHeight) bodyHeight = bodyTableHeight - pagerHeight + 3;
                var pagerTop = headerTop + bodyHeight + pagerHeight - 5;
                $('#<%=this.grdData.ClientID %>_PagerContainer').css('top', pagerTop + 'px');
            }
        }

        function completeLoading() {
            var nFrame = getMyFrameFromParent();
            var $obj = $(nFrame).parents().eq(5).prev().find('td:eq(1) img'); //innerTD, innerTR, innerTBODY, innerTABLE, outerTD, outerTR, previousOuterTR

            $obj.attr('src', 'Images/Icons/minus_blue.png');
            $obj.attr('title', 'Collapse');
            $obj.attr('alt', 'Collapse');
            $obj.css('cursor', 'pointer');
        }

        function formatGoTo(obj) {
            var text = $(obj).val();

            if (/[^0-9-]|^0+(?!$)/g.test(text)) {
                $(obj).val(text.replace(/[^0-9-]|^0+(?!$)/g, ''));
            }
        }

        function ddlAssignedTo_change(obj) {

            var assignedTo_ID = $(obj).val();
            
            if ($(obj).closest("tr").find("td select[field='Release/Deployment MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Release/Deployment MGMT']");

                if (ddlObj.text().indexOf('No Release/Deployment MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
            if ($(obj).closest("tr").find("td select[field='Workload MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Workload MGMT']");

                if (ddlObj.text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
        }

        function ddlPrimaryResource_change(obj) {

            var primaryResource_ID = $(obj).val();
            
            if ($(obj).closest("tr").find("td select[field='Release/Deployment MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Release/Deployment MGMT']");

                if (ddlObj.text().indexOf('No Release/Deployment MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
            if ($(obj).closest("tr").find("td select[field='Workload MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Workload MGMT']");

                if (ddlObj.text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
        }

        function ddlSystem_change(obj) {
            
            var systemID = $(obj).val();

            updateWorkAreaFromSystem(obj,systemID);

            if ($(obj).closest("tr").find("td select[field='Release/Deployment MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Release/Deployment MGMT']");
                
                if (ddlObj.text().indexOf('No Release/Deployment MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
            if ($(obj).closest("tr").find("td select[field='Workload MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Workload MGMT']");

                if (ddlObj.text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
        }

        function ddlAssignedToRank_change(obj) {
            if ($(obj).closest("tr").find("td select[field='Workload MGMT']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Workload MGMT']");

                if (ddlObj.text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($(ddlObj));
                }
            }
        }

        function AOR_change(obj) {
            var $obj = $(obj);

            if ($obj.attr('field') == 'Release/Deployment MGMT') {
                if ($(obj).closest("tr").find("td select[field='Work Activity']").length > 0) {
                    loadWorkActivity($(obj).closest("tr").find("td select[field='Work Activity']"), $obj.find('option:selected').attr('workloadAllocationID'));
                }
            }
        }

        function loadWorkActivity(obj, workloadAllocationID) {
            var workActivityID = $(obj).val();
            var html;
            var optExists = false;
            //var currentPhaseID = 0;
            var currentWorkActivityGroup = '';
            
            $(obj).empty();
            
            $.each(arrWorkActivity[0], function (rowIndex, row) {
                if (/*row.PDDTDR_PHASEID == null || */row.WorkloadAllocationID == null || row.WorkloadAllocationID == workloadAllocationID) {
                    /*if (currentPhaseID != row.PDDTDR_PHASEID && row.PDDTDR_PHASE != null) {
                        html += '<option style="background-color: white;" disabled>' + row.PDDTDR_PHASE + '</option>';
                        currentPhaseID = row.PDDTDR_PHASEID;
                    }*/

                    if (currentWorkActivityGroup != row.WorkActivityGroup && row.WorkActivityGroup != null) {
                        html += '<option style="background-color: white;" disabled>' + row.WorkActivityGroup + '</option>';
                        currentWorkActivityGroup = row.WorkActivityGroup;
                    }
            
                    html += '<option value="' + row.WORKITEMTYPEID + '">' + row.WORKITEMTYPE + '</option>';
            
                    if (row.WORKITEMTYPEID == workActivityID) optExists = true;
                }
            });
            
            $(obj).append(html);
            
            if (optExists) $(obj).val(workActivityID);
        }

        function updateWorkAreaFromSystem(obj,systemID) {
            try {
                var optExists = false;
                var addedCount = 0;
                var ddlObj = '';
                if ($(obj).closest("tr").find("td select[field='WORK AREA']").length > 0) {
                    ddlObj = $(obj).closest("tr").find("td select[field='WORK AREA']");
                    ddlObj.empty();

                    var newOption = {};
                    $.each(arrWorkArea[0], function (rowindex, row) {
                        if (row.WTS_SYSTEMID == null
                            || row.WTS_SYSTEMID == systemID) {
                            newOption = '<option value="' + row.WorkAreaID + '" WTS_SYSTEMID="' + row.WTS_SYSTEMID + '">' + row.WorkArea + '</option>';
                            ddlObj.append(newOption);
                            addedCount += 1;
                        }
                    });
                    // Changing to some Systems returned no rows, so put General Support in:
                    if (addedCount == 0) {
                        newOption = '<option value=185 WTS_SYSTEMID=1>0 - General Support</option>';
                        ddlObj.append(newOption);
                    }
                    input_change(ddlObj);
                    input_change(obj);
                }
            } catch (e) {
                var strError = e.message;
            }
        }

        function showAOROptionSettings(obj) {
            var $objDiv = $(obj).closest('td').find('.aoroptionsettings');

            if ($objDiv.is(':visible')) {
                $objDiv.slideUp();
            }
            else {
                $('.aoroptionsettings').slideUp();

                var $pos = $(obj).position();

                $objDiv.css({
                    top: ($pos.top - 3),
                    left: ($pos.left + 23)
                }).slideDown();
            }
        }

        function getAOROptions(obj) {
            var systemAffiliated = 1, resourceAffiliated = 0, aorTypeAffiliated = 0, all = 0, assignedTo_ID = 0, primaryResource_ID = 0, system_ID = 0, assignedToRank_ID = 0;
            var $objCheckboxes = $(obj).closest('td').find('.aoroptionsettings input[type=checkbox]');

            $.each($objCheckboxes, function () {
                switch ($(this).parent().text()) {
                    case 'Affiliated by selected System':
                        systemAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'Affiliated by selected Assigned To/Primary Resource':
                        resourceAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'Affiliated by AOR Workload Type':
                        aorTypeAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'All (grouped by System)':
                        all = $(this).is(':checked') ? 1 : 0;
                        break;
                }
            });
            if ($(obj).closest("tr").find("td select[field='Assigned Resource']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Assigned Resource']");

                assignedTo_ID = $(ddlObj).val();
            } else {
                assignedTo_ID = $(obj).closest("td").find('select').attr('assignedTo_ID');
            }
            if ($(obj).closest("tr").find("td select[field='Primary Resource']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Primary Resource']");

                primaryResource_ID = $(ddlObj).val();
            } else {
                primaryResource_ID = $(obj).closest("td").find('select').attr('primaryResource_ID');
            }
            if ($(obj).closest("tr").find("td select[field='SYSTEM(TASK)']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='SYSTEM(TASK)']");

                system_ID = $(ddlObj).val();
            } else {
                system_ID = $(obj).closest("td").find('select').attr('system_id');
            }
            if ($(obj).closest("tr").find("td select[field='Assigned To Rank']").length > 0) {
                ddlObj = $(obj).closest("tr").find("td select[field='Assigned To Rank']");

                assignedToRank_ID = $(ddlObj).val();
            } else {
                assignedToRank_ID = $(obj).closest("td").find('select').attr('assignedToRank_ID');
            }
            
            PageMethods.GetAOROptions(assignedTo_ID, primaryResource_ID, system_ID, systemAffiliated, resourceAffiliated, assignedToRank_ID, all, function (result) { getAOROptions_done(result, obj, aorTypeAffiliated); }, getAOROptions_error);
        }

        function getAOROptions_done(result, obj, aorTypeAffiliated) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            var $ddl = $(obj).closest('td').find('select:first');
            var $opt = $ddl.find('option:selected');
            var currentSystem = '';
            var currentAORType = ' ';
            var exists = false;

            $ddl.empty();
            nHTML += '<option value="0"></option>';

            $.each(dt, function (rowIndex, row) {
                if (row.AORType != "Release/Deployment MGMT" && row.AORType != currentAORType && $ddl.attr('field') !== 'Release/Deployment MGMT') {
                    if (aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                        currentAORType = row.AORType;
                        nHTML += '<option style="background-color: white" disabled>' + currentAORType + '</option>';
                    } else if (aorTypeAffiliated === 0) {
                        currentAORType = row.AORType;
                        nHTML += '<option style="background-color: white" disabled>' + currentAORType + '</option>';
                    }
                }

                if (row.WTS_SYSTEM != undefined && row.WTS_SYSTEM != currentSystem && aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                    currentSystem = row.WTS_SYSTEM;
                    nHTML += '<option style="background-color: ghostwhite;" disabled>' + currentSystem + '</option>';
                } else if ((row.WTS_SYSTEM != undefined && row.WTS_SYSTEM != currentSystem && (($ddl.attr('field') === 'Release/Deployment MGMT' && row.AORType === 'Release/Deployment MGMT')
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT'))
                    && aorTypeAffiliated === 0)) {
                    currentSystem = row.WTS_SYSTEM;
                    nHTML += '<option style="background-color: ghostwhite;" disabled>' + currentSystem + '</option>';
                }

                if (aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
                } else if ((($ddl.attr('field') === 'Release/Deployment MGMT' && row.AORType === 'Release/Deployment MGMT')
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT'))
                    && aorTypeAffiliated === 0) {
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
                }
            });

            $ddl.append(nHTML);

            $('option', $ddl).each(function () {
                if ($(this).val() == $opt.val()) {
                    exists = true;
                    return false;
                }
            });

            if (exists) {
                $ddl.val($opt.val());
            }
            else {
                $ddl.prepend($opt);
            }
        }

        function getAOROptions_error() {

        }

        function clearSelectedAssigned(data) {
            _selectedAssigned = data;
            viewDataChanged = true;
        }
    </script>

    <script id="jsInit" type="text/javascript">
        var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
        var ddlStatus = $('#<%=Master.FindControl("ms_Item0").ClientID %>');
        var ddlAffiliated = $('#<%=Master.FindControl("ms_Item1").ClientID %>');
        var chkBusinessReview = $('#<%=Master.FindControl("chk_Item11").ClientID %>');

        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';
            _businessReview = '<%=this.QFBusinessReview%>'.toLowerCase();
        }

        function initControls() {
            ddlStatus.multipleSelect({
                placeholder: 'Default'
				    ,width: 'undefined'
                    ,onClick: function () {
                        lblMessage.show();
                        lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                    },
                    onCheckAll: function () {
                        lblMessage.show();
                        lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                    }
					,onOpen: function() { ddlStatus_update(); }
                ,onClose: function() { ddlStatus_update(); }
            }).change(function () { ddlStatus_update(); });
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Non-Resources']").wrapAll("<optgroup label='Non-Resources'>");
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Resources']").wrapAll("<optgroup label='Resources'>");
            ddlAffiliated.multipleSelect({
                placeholder: 'Default'
				    ,width: 'undefined'
                    ,onClick: function () {
                        lblMessage.show();
                        lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                    },
                    onCheckAll: function () {
                        lblMessage.show();
                        lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                    }
					,onOpen: function() { ddlAffiliated_update(); }
                ,onClose: function() { ddlAffiliated_update(); }
            }).change(function () { ddlAffiliated_update(); });

            chkBusinessReview.on('change', function () { chkBusinessReview_change(this); });

            $('td select[field="Workload MGMT"]').each(function () {
                if ($(this).text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($(this));
                }
            });

            //FilterStatuses
            $('td select[field="Status"]').each(function () {
                if ($(this).val() != 1) {
                    $(this).children().each(function () {
                        if ($(this).text() === 'New') {
                            $(this).wrap('<span>');
                            $(this).hide();
                        }
                    });
                }

                if (!($(this).val() == 2  // Re-Opened
                    || $(this).val() == 7 // Un-Reproducible
                    || $(this).val() == 8 // Deployed
                    || $(this).val() == 9 // Checked In
                    || $(this).val() == 10 // Closed
                    || $(this).val() == 11)) { // Ready for Review
                    $(this).children().each(function () {
                        if ($(this).text() === 'Re-Opened') {
                            $(this).wrap('<span>');
                            $(this).hide();
                        }
                    });
                }
            });

            lblMessage.hide();
        }

        function initDisplay() {
            $('#imgSort').hide();
            //$('#imgExport').hide();

            if ('<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') {
                $('#btnAddTask').show();
                $('#btnAddSubTask').show();

                if ($('.saveable').length > 0) $('#btnSave').show();
            }

            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();
                $('#tdPageSize').show();
                $('#imgExport').hide();
                $('#tdQuickFilters').show();
                $('#trItem5').show();
                $('#trms_Item0').show();
                $('#trms_Item1').show();
                $('#trchk_Item11').show();
                $('#trClearAll').hide();
                //$('#<%=menuRelatedItems.ClientID %>').hide();
                <%--if(<%=gridRowCnt%> <= 12){
                    $('#trItem5').hide();
                }--%>

                resizeFrame();
                completeLoading();
            }
            else {
                $('#tdSettings').show();
                $('#btnOpenWorkItem').show();
                $('#txtWorkItem').show();
                $('#imgExport').show();
                $('#tdQuickFilters').show();
                $('#trItem5').show();
                $('#trms_Item0').show();
                $('#trms_Item1').show();
                $('#trchk_Item11').show();
                $('#trClearAll').hide();
                resizeGrid();
            }
        }

        function initEvents() {
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#imgSettings').click(function () { imgSettings_click(); });
            $('#btnOpenWorkItem').click(function () { btnOpenWorkItem_click(); return false; });
            $('#txtWorkItem').keydown(function(e) {
                if (e.keyCode == 13 || e.keyCode == 144) {
                    e.preventDefault();
                    return false;
                }
            });
            $('#txtWorkItem').on('keyup paste', function(e) {
                formatGoTo(this);
                
                if (e.keyCode == 13 || e.keyCode == 144) $('#btnOpenWorkItem').trigger('click');
            });
            $('#txtWorkItem').blur(function() { txtBox_blur(this); });
            $('#btnAddTask').click(function () { btnAddTask_click(); return false; });
            $('#btnAddSubTask').click(function () { btnAddSubTask_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });

            $('#<%=Master.FindControl("ddlItem5").ClientID %>').on('change', function () { 
               
                lblMessage.show();
                lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
            });

            $('#btnQuickFilters').click(function() { toggleQuickFilters_click(); });
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();

            $('select').on('change keyup mouseup', function () { input_change(this); });
            $(".gridHeader th").css("border-right","1px solid grey");
            $(".gridHeader th").css("border-bottom","1px solid grey");
            $(".gridHeader th").css("border-top","none");
            $(".gridHeader th").css("outline", "none");
      
            //Fix for chrome issue changing selected value when using wrapall
            var oldValue = $('#<%=ddlGridview.ClientID %>').val();
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
            $('#<%=ddlGridview.ClientID %>').val(oldValue);

            $('#<%=ddlGridview.ClientID %>').on("change", function () { ddlGridView_change(); return false; });
            $('#<%=ddlGridview.ClientID %> option').filter(function () { return $.trim($(this).text()) === $("#<%= itisettings.ClientID %>").val(); }).prop('selected', true);

            resizeGrid();
        });
    </script>
</asp:Content>