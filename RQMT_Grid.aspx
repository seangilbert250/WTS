<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMT_Grid.aspx.cs" Inherits="RQMT_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">RQMT</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" type="text/css" href="Styles/tooltip.css" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table style="width: 100%; border-collapse: collapse;">
		<tr>
			<td>
                Sys Rqmt Sets
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <span id="spnView" style="display: none;">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gridiew: 
        <asp:DropDownList ID="ddlGridview" runat="server" Style="font-size: 12px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
        <asp:HiddenField ID="itisettings" runat="server" />
    </span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnCopy" value="Copy" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnPaste" value="Paste" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" style="vertical-align: middle; display: none;" />                
                <input type="button" id="btnEdit" value="View/Edit" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnBuilder" value="Builder" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <iframe id="frmDownload" style="display: none;"></iframe>
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedRQMTID = 0;
        var _WTS_SYSTEMID = 0;
        var _WorkAreaID = 0;
        var _SystemSuiteID = 0;
        var _RQMTTypeID = 0;
        var _toggleAllExecuting = false;
        var _toggleAllExecutingOpen = false;
        var _gridPageIndex = <%=this.grdData.PageIndex%>;
        var _copiedRQMTSystems = '';
        var _rqmtSetUsageMonthCheckboxesExist = false;
        var _rqmtSetMonthCheckBoxLevel = <%=RQMTSetMonthCheckBoxLevel%>;
    </script>

    <script id="jsEvents" type="text/javascript">
        function ddlView_change() {
            refreshPage(false);
        }

        function ddlGridView_change() {
            var gridViewIndex = $("#<%=ddlGridview.ClientID %> option:selected").index();
            var gridViewText = $("#<%=ddlGridview.ClientID %> option:selected").text();
            var sectionsXml = $("#<%=ddlGridview.ClientID %> option:selected").attr("SectionsXML");

            PageMethods.UpdateSession(gridViewIndex, gridViewText, sectionsXml, '<%=SessionPageKey%>', ddlGridView_Change_Done, on_error);
        }

        function ddlGridView_Change_Done(results) {
            if(results) refreshPage(false);
        }

        function imgRefresh_click() {
            refreshPage(true, false);
        }

        function btnAdd_click() {
            openRQMTPopup(0, null, <%=FilterRQMTSetID%>, <%=FilterParentRQMTID%>);
        }

        function btnDelete_click() {
            var checkedCBs = getCheckedRQMTs();

            if (checkedCBs.length == 0) {
                warningMessage('Please check a RQMT');
                return;
            }
            
            var title = '';
            var msg = '';
            
            if (checkedCBs.indexOf(',') != -1) {
                title = 'Delete RQMTS';
                msg = 'Do you want to delete the checked RQMTS from this set only, or all the sets they are used?';
            }
            else {
                title = 'Delete RQMT';
                msg = 'Do you want to delete the checked RQMT from this set only, or all the sets it is used?';
            }

            QuestionBox(title, msg, 'This Set,All Sets,Cancel', 'confirmRQMTDelete', 300, 300, this, StrongEscape(checkedCBs)); // we need strong escape because the question box strips special characters when it passes the param on url
        }

        function confirmRQMTDelete(answer, checkedCBs) {
            if (answer == 'This Set' || answer == 'All Sets') {
                checkedCBs = UndoStrongEscape(checkedCBs);

                try {
                    ShowDimmer(true, 'Deleting...', 1);                

                    PageMethods.DeleteRQMTs(checkedCBs, answer == 'All Sets', function (result) { delete_done(result, checkedCBs.indexOf(',') != -1); }, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result, multiple) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                if (multiple) {
                    MessageBox('RQMTs have been deleted.');
                }
                else {
                    MessageBox('RQMT has been deleted.');
                }
                refreshPage(true, true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnBuilder_click() {
            var obj = parent;
            
            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            var suiteid = _SystemSuiteID;
            var sysid = _WTS_SYSTEMID;
            var waid = _WorkAreaID;
            var rtid = _RQMTTypeID;
            var rsetid = 0;
            var rqmtid = 0;

            if ($('.selectedRow').length == 1) {
                var selectedRow = $('.selectedRow');

                if (_SystemSuiteID == 0 && selectedRow.attr('WTS_SYSTEM_SUITEID') > 0) {
                    suiteid = selectedRow.attr('WTS_SYSTEM_SUITEID');
                }

                if (_WTS_SYSTEMID == 0 && selectedRow.attr('SYSTEM_ID') > 0) {
                    sysid = selectedRow.attr('SYSTEM_ID');
                }

                if (_WorkAreaID == 0 && selectedRow.attr('WORKAREA_ID') > 0) {
                    waid = selectedRow.attr('WORKAREA_ID');
                }

                if (_RQMTTypeID == 0 && selectedRow.attr('RQMTTYPE_ID') > 0) {
                    rtid = selectedRow.attr('RQMTTYPE_ID');
                }

                if (selectedRow.attr('RQMTSET_ID') > 0) {
                    rsetid = selectedRow.attr('RQMTSET_ID');
                }
            }

            // filters will look like SYSTEM_ID=31|WORKAREA_ID=133|RQMTTYPE_ID=2|RQMT_ID=13434
            var filters = '<%=this.Filter%>';
            if (filters.length > 0) {
                var filterArr = filters.split('|');

                if (suiteid == 0 && _.find(filterArr, function (f) { return f.split('=')[0] == 'WTS_SYSTEM_SUITEID' }) != null) {
                    suiteid = _.find(filterArr, function (f) { return f.split('=')[0] == 'WTS_SYSTEM_SUITEID' }).split('=')[1];
                }

                if (sysid == 0 && _.find(filterArr, function (f) { return f.split('=')[0] == 'SYSTEM_ID' }) != null) {
                    sysid = _.find(filterArr, function (f) { return f.split('=')[0] == 'SYSTEM_ID' }).split('=')[1];
                }

                if (waid == 0 && _.find(filterArr, function (f) { return f.split('=')[0] == 'WORKAREA_ID' }) != null) {
                    waid = _.find(filterArr, function (f) { return f.split('=')[0] == 'WORKAREA_ID' }).split('=')[1];
                }

                if (rtid == 0 && _.find(filterArr, function (f) { return f.split('=')[0] == 'RQMTTYPE_ID' }) != null) {
                    rtid = _.find(filterArr, function (f) { return f.split('=')[0] == 'RQMTTYPE_ID' }).split('=')[1];
                }

                if (rsetid == 0 && _.find(filterArr, function (f) { return f.split('=')[0] == 'RQMTSET_ID' }) != null) {
                    rsetid = _.find(filterArr, function (f) { return f.split('=')[0] == 'RQMTSET_ID' }).split('=')[1];
                }
            }

            var bldParams = '';
            bldParams += 'WTS_SYSTEM_SUITEID=' + _SystemSuiteID + '|';
            bldParams += 'WTS_SYSTEMID=' + sysid + '|';
            bldParams += 'WORKAREA_ID=' + waid + '|';
            bldParams += 'RQMTTYPE_ID=' + rtid + '|';
            bldParams += 'RSET_ID=' + rsetid + '|';
            bldParams += 'RQMT_ID=' + rqmtid;
            
            obj.showFrameForEdit('RQMTBUILDER', true, 0, true, bldParams);
        }

        function btnEdit_click() {
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('RQMT', false, _selectedRQMTSetRQMTSystemID, true);
            }
            else {
                var nWindow = 'RQMT';
                var nTitle = 'RQMT';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.RQMTEdit + window.location.search + '&NewRQMT=false&RQMTID=' + _selectedRQMTSetRQMTSystemID;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('input[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                        var $obj = $(this);

                        var val = $obj.val();

                        if ($obj.is(':checkbox')) {
                            val = $obj.is(':checked') ? 'true' : 'false';
                        }

                        arrChanges.push({
                            'typeName': $obj.attr('typeName'), 'typeID': $obj.attr('typeID'), 'field': $obj.attr('field'), 'value': val,
                            'rqmtid': $obj.attr('rqmtid'), 'systemid': $obj.attr('systemid'), 'workareaid': $obj.attr('workareaid'),
                            'rqmttypeid': $obj.attr('rqmttypeid'), 'rsetid': $obj.attr('rsetid')
                        });
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
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
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

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.' + (obj.output != null ? '<br>' + obj.output : ''));
                refreshPage(true);
            }
            else {                
                MessageBox('Failed to save. <br>' + errorMsg + '<br>' + obj.output);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            return '';

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="RQMT"]'), function() {
                var nText = $(this).val();
                
                if (nText.length == 0) {
                    if ($.inArray('RQMT cannot be empty.', validation) == -1) validation.push('RQMT cannot be empty.');
                }

                if ($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="RQMT"][value="' + nText + '"]').not($(this)).length > 0) {
                    if ($.inArray('RQMT cannot have duplicates.', validation) == -1) validation.push('RQMT cannot have duplicates.');
                }
            });

            return validation.join('<br>');
        }

        function displayAllRows(obj) {
            var $obj = $(obj);

            _toggleAllExecuting = true;

            if ($obj.attr('title') == 'Expand') {
                _toggleAllExecutingOpen = true; // we are collapsing the grid
            }
            else {
                _toggleAllExecutingOpen = false; // we are opening the grid
            }

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

            _toggleAllExecuting = false;
        }

        function displayNextRow(obj) {
            var $obj = $(obj);

            if ($obj.attr('title') != 'Loading...') {
                var $row = $obj.closest('tr');

                var rowIsCurrentlyVisible = $row.next().find('iframe').length > 0 && $row.next().find('iframe').is(':visible');

                var showRow = false;

                if (_toggleAllExecuting) {
                    showRow = _toggleAllExecutingOpen;
                }
                else {
                    showRow = !rowIsCurrentlyVisible;
                }

                var nURL = getURLForNode($row, false);                
                var nURLCleaned = getURLForNode($row, true);

                if (nURL == null) {
                    return;
                }
                
                var blnShow = true;

                if (showRow) {
                    if ($row.next().find('iframe').length == 0) { // if a frame already exists, we don't create it again (this would happen if they had already expanded a child node and a parent "open all" was clicked
                        $obj.attr('src', 'Images/Loaders/loader_2.gif');
                        $obj.attr('title', 'Loading...');
                        $obj.attr('alt', 'Loading...');
                        $obj.css('cursor', 'default');

                        var openRQMTNodes = defaultParentPage._expandedGridNodes['rqmt'];
                        if (openRQMTNodes == null) {
                            openRQMTNodes = [];
                            defaultParentPage._expandedGridNodes['rqmt'] = openRQMTNodes;
                        }

                        if (openRQMTNodes.indexOf(nURLCleaned) == -1) {
                            var cleanedWithNoUsageSetMonths = cleanURL(nURLCleaned, _rqmtSetUsageMonthCheckboxesExist);
                            openRQMTNodes.push(cleanedWithNoUsageSetMonths); // add the current url to the list of open nodes (we don't store RQMT SET usage month in the cached nodes due to complexities in syncing it)
                        }

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
                }
                else {
                    defaultParentPage._expandedGridNodes['rqmt'] = _.reject(defaultParentPage._expandedGridNodes['rqmt'], function (url) { return url == nURLCleaned }); // remove the current url from the list of open nodes
                    blnShow = false;
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

        function row_click(obj) {
            if ($(obj).attr('rqmtset_rqmtsystemid')) {
                $('#btnDelete').prop('disabled', false);
                $('#btnEdit').prop('disabled', false);
            }
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            switch($obj.attr('field')) {
                case 'Sort':
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

            $('#btnDelete').prop('disabled', $('input[field=rqmtdelete]:checked').length == 0);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            switch($obj.attr('field')) {
                case 'Sort':
                    if (nVal == '-') $obj.val('');
                    return;
            }

            $obj.val($.trim(nVal));
        }

        function refreshPage(blnRetainPageIndex, reloadFilters) {
            var nURL = window.location.href;
            
            nURL = editQueryStringValue(nURL, 'IsConfigured', '<%=this.IsConfigured.ToString() %>');
            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? getTopLevelGridPageIndex() : '0');
            
            if (_currentLevel > _rqmtSetMonthCheckBoxLevel && _rqmtSetUsageMonthCheckboxesExist) { // this section attempts to refresh the current version of the month parameters (since the parent month checkboxes can change in a parent frame before refreshing this page)
                // usagesetmonth could be set as a query string parameter OR as part of the filters
                var months = defaultParentPage._moduleCachedData['rqmtsetusagemonthselection']; 
                
                nURL = editQueryStringValue(nURL, 'USAGESETMONTH_ID', '0'); // replace query string USAGEMONTH_ID=1,5,8 with USAGEMONTH_ID=0 for easier string replacement                               
                nURL = nURL.replace('&USAGESETMONTH_ID=0', '');
                nURL = nURL.replace('USAGESETMONTH_ID=0', '');
                
                var filters = '<%=Filter%>';
                
                if (filters.indexOf('USAGESETMONTH_ID') != -1) { // remove usage set month from filters
                    var filterarr = filters.split('|');
                    filters = '';

                    for (var i = 0; i < filterarr.length; i++) {                        
                        if (filterarr[i].indexOf('USAGESETMONTH_ID') == -1) {
                            if (filters != '') filters += '|';
                            filters += filterarr[i];
                        }
                    }
                }
                
                if (months != null && months != '' && months != '0') { // it is possible for selections from a previous view to still be in the global cache, so we check to see if the checkboxes exist for THIS view
                    if (filters.length > 0) {
                        filters += '|';
                    }

                    filters += 'USAGESETMONTH_ID=' + months;
                }
                
                nURL = editQueryStringValue(nURL, 'Filter', filters);                
            }
            
            var gridViewText = $("#<%=ddlGridview.ClientID %> option:selected").text();

            if (!<%=string.IsNullOrWhiteSpace(this.View).ToString().ToLower()%>) {
                if ('<%=this.View%>' != gridViewText) {
                    nURL = editQueryStringValue(nURL, 'View', gridViewText);                    
                }
            }

            if (_currentLevel > 1) {
                var cleanedURL = cleanURL(nURL.substring(nURL.indexOf('RQMT_Grid.aspx')), _rqmtSetUsageMonthCheckboxesExist && _currentLevel > _rqmtSetMonthCheckBoxLevel);

                if (defaultParentPage._expandedGridNodes['rqmt'].indexOf(cleanedURL) == -1) {
                    defaultParentPage._expandedGridNodes['rqmt'].push(cleanedURL);
                }
            }

            if (reloadFilters == 1) { top.setFilterSession(true, false); return; }
            
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
                var nHeight = headerTop + bodyTableHeight + pagerHeight + 1;
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
            var $obj = $(nFrame).parents().eq(5).prev().find('td:eq(0) img'); //innerTD, innerTR, innerTBODY, innerTABLE, outerTD, outerTR, previousOuterTR
            
            $obj.attr('src', 'Images/Icons/minus_blue.png');
            $obj.attr('title', 'Collapse');
            $obj.attr('alt', 'Collapse');
            $obj.css('cursor', 'pointer');
        }

        function imgExport_click() {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'CurrentLevel', '1'); // for now, our export only ever has 1 level
            nURL = editQueryStringValue(nURL, 'Filter', '');
            nURL = editQueryStringValue(nURL, 'GridPageIndex', '0');
            nURL = editQueryStringValue(nURL, 'Export', 'true');

            $('#frmDownload').attr('src', nURL);
        }

        function export_done(results) {

        }

        function imgSettings_click() {
            openSettings();
        }

        function openSettings() {
            if (parent.openSettings) {
                parent.openSettings();
                return;
            }

            var nTitle = 'Sys RQMT Sets Parameters';
            var nHeight = 600, nWidth = 900;
            var nURL = _pageUrls.Maintenance.RQMTParameterPage;
            var openPopup = popupManager.AddPopupWindow('RQMTParameter', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function lblEdit_click(column, id) {                        
            if (column == 'Defects') {
                if (id == null || id == '') {
                    id = '0_0';
                }

                var RQMT_ID = id.split('_')[0];
                var SYSTEM_ID = id.split('_')[1];

                var nTitle = 'RQMT Defect(s) & Impact';
                var nHeight = 600, nWidth = 1250;
                var nURL = 'RQMTDefectsImpact_Grid.aspx?RQMT_ID=' + RQMT_ID + '&SYSTEM_ID=' + SYSTEM_ID;
                var openPopup = popupManager.AddPopupWindow('RQMTDefects', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
            else if (column == 'RQMT #' || column == 'RQMT Primary #' || column == 'Functionality' || column == 'Description') {
                var openSections = '_details_';
                var itemSubSection = null;
                if (column == 'Functionality') {
                    openSections += '_attributes_'; // note: the functionalities grid is in the attributes section
                    itemSubSection = '_functionalities_';
                }
                else if (column == 'Description') {
                    openSections += '_attributes_'; // note: the descriptions grid is in the attributes section
                    itemSubSection = '_descriptions_';
                }

                openRQMTPopup(id, openSections, null, null, null, openSections != '_details_', null, itemSubSection);
            }
        }

        function openRQMTPopup(RQMTID, openSections, RQMTSetID, ParentRQMTID, closeFn, hideNonOpenSections, itemID, itemSubSection) {    
            if (window.parent.openRQMTPopup) {
                if (closeFn == null) {
                    closeFn = function () {
                        var popup = popupManager.GetPopupByName('RQMTEdit');
                        if (popup != null) {
                            var frmurl = popup.Frame.contentDocument.location.href;
                            if (frmurl.indexOf('PageSaved=1') >= 0) { // if the user just opened the popup and didn't do anything, we don't force a refresh (note that adding defects if on another popup after this and so it wouldn't trigger a save on the dialog - if this is an issue, we can force the dialog to keep track of whether it has changed defects and needs to report a different save status)
                                refreshPage(true, true);
                            }
                        }
                    }
                }

                window.parent.openRQMTPopup(RQMTID, openSections, RQMTSetID, ParentRQMTID, closeFn, hideNonOpenSections, itemID, itemSubSection);
            }
        }

        function syncOpenNodes() {
            if (defaultParentPage._expandedGridNodes == null || defaultParentPage._expandedGridNodes['rqmt'] == null || defaultParentPage._expandedGridNodes['rqmt'].length == 0) {
                if (defaultParentPage._expandedGridNodes == null) {
                    defaultParentPage._expandedGridNodes = [];                    
                }

                defaultParentPage._expandedGridNodes['rqmt'] = [];

                return; // no nodes to open
            }            
                       
            var openRQMTNodes = defaultParentPage._expandedGridNodes['rqmt'];
            
            if (defaultParentPage._moduleCachedData['rqmtsetusagemonthselection'] != null && _rqmtSetUsageMonthCheckboxesExist) {
                var monthsarr = defaultParentPage._moduleCachedData['rqmtsetusagemonthselection'].split(',');
                
                for (var i = 1; i <= 12; i++) {
                    var cb = $('[id$=_grdData_HeaderContainer]').find('input[type=checkbox][rqmtsetusagemonth=' + i + ']');                    
                    
                    cb.prop('checked', monthsarr.indexOf(i + '') != -1); // the monthsarr contains strings, so we convert to string to make sure it finds it (int to string doesn't seem to match)
                }
            }

            $('#<%=this.grdData.ClientID %>_BodyContainer table tr.gridBody').each(function () {
                var row = $(this);
                var img = row.find('img');

                if (img.length > 0) {
                    var title = img.attr('title');

                    if (title == 'Expand' || title == 'Collapse') {
                        var nURL = cleanURL(getURLForNode(row, true), _rqmtSetUsageMonthCheckboxesExist && _currentLevel >= _rqmtSetMonthCheckBoxLevel); // geturlfornode will clean the node, but clean doesn't strip usagemonth by default so we call it again
                        
                        if (openRQMTNodes.indexOf(nURL) != -1) {
                            $(img).click();
                        }
                    }
                }
            });
        }

        function getURLForNode($row, cleanNonStandardParams) {
            var nURL = '';

            if (cleanNonStandardParams == null) cleanNonStandardParams = false;

            var rowBase = $row[0];

            var rqmtRow = false;

            for (var i = 0; i < rowBase.attributes.length; i++) {
                var attr = rowBase.attributes[i].nodeName.toLowerCase();                    
                if (attr == 'rqmt_id' || attr == 'rqmtname_id' || attr.indexOf('rqmtprimary') != -1 || attr.indexOf('rqmtnested') != -1) {
                    rqmtRow = true;
                    break;
                }
            }

            if (_currentLevel == '<%=this.LevelCount %>') {
                return;
            }
            else {
                nURL = 'RQMT_Grid.aspx' + window.location.search;
                nURL = editQueryStringValue(nURL, 'CurrentLevel', (parseInt(_currentLevel) + 1));
            }

            var filters = [];

            $.each($row.find('td'), function (i) {                            
                var nText = $('.gridHeader:eq(1) th:eq(' + i + ')').text();                         
                var nVal = encodeURIComponent($(this).text());
                // we don't pass down filters for attributes on rqmt rows since these are specific to that row (we WILL pass them down if we are drilling down on criticality outside of rqmts, however)                              
                // we exclude description id because we show multiple descriptions per row when we are viewing a RQMT row
                // we exclude functionality and defects for same reason as description
                var rqmtRowExcludes = ['RQMTCRITICALITY_ID', 'Criticality', 'RQMTACCEPTED_ID', 'Accepted', 'RQMTSTAGE_ID', 'RQMT Stage', 'RQMTSTATUS_ID', 'RQMT Status', 'DESCRIPTION_ID', 'RQMTSYSTEMDEFECT_ID', 'FUNCTIONALITY_ID', 'Functionality',
                'RQMTSYSTEMDEFECTNUMBER_ID', 'RQMTSYSTEMDEFECTDESCRIPTION_ID', 'RQMTSYSTEMDEFECTIMPACT_ID', 'RQMTSYSTEMDEFECTMITIGATION_ID', 'RQMTSYSTEMDEFECTNUMBER_ID', 'RQMTSYSTEMDEFECTRESOLVED_ID', 'RQMTSYSTEMDEFECTREVIEW_ID', 'RQMTSYSTEMDEFECTSTAGE_ID', 'RQMTSYSTEMDEFECTVERIFIED_ID'];
                if (nText.match(/_ID$/) && (!rqmtRow || (rqmtRow && rqmtRowExcludes.indexOf(nText) == -1))) {                                
                    filters.push(nText + '=' + nVal);
                }
            });

            // pass in custom month checkbox filter
            var cb = $('[id$=_grdData_HeaderContainer]').find('input[type=checkbox][rqmtsetusagemonth]:checked');                    
            if (cb.length > 0) {
                var months = '';
                for (var x = 0; x < cb.length; x++) {
                    if (months != '') months += ',';
                    months += $(cb[x]).attr('rqmtsetusagemonth');
                }
                filters.push('USAGESETMONTH_ID=' + months);
            }
            else if (defaultParentPage._moduleCachedData['rqmtsetusagemonthselection'] != null && defaultParentPage._moduleCachedData['rqmtsetusagemonthselection'] > 0 && _rqmtSetUsageMonthCheckboxesExist && _currentLevel >= _rqmtSetMonthCheckBoxLevel) {
                filters.push('USAGESETMONTH_ID=' + defaultParentPage._moduleCachedData['rqmtsetusagemonthselection']);
            }

            nURL = editQueryStringValue(nURL, 'Filter', ('<%=this.Filter %>' != '' ? encodeURIComponent('<%=this.Filter %>|') : '') + filters.join('|'));

            if (cleanNonStandardParams) {
                // because our refresh function adds in extra params, we strip those out to make sure the base url works
                nURL = cleanURL(nURL);
            }

            return nURL;
        }

        function cleanURL(nURL, removeUsageSetMonth) {
            // because our refresh function adds in extra params, we strip those out to make sure the base url works
            nURL = nURL.replace('&IsConfigured=True', '');
            nURL = nURL.replace('&IsConfigured=False', '');
            nURL = nURL.replace('&GridPageIndex=0', '');
            nURL = nURL.replace('&GridPageIndex=1', '');

            // remove usagesetmonth from query string AND filter string            
            if (removeUsageSetMonth && nURL.indexOf('USAGESETMONTH_ID') != -1) {
                nURL = editQueryStringValue(nURL, 'USAGESETMONTH_ID', '0'); // replace USAGESETMONTH_ID=1,5,8 with USAGEMONTH_ID=0 for easier string replacement
                nURL = nURL.replace('&USAGESETMONTH_ID=0', '');
                nURL = nURL.replace('USAGESETMONTH_ID=0', '');

                var filters = getQueryStringValue(nURL, 'Filter', '');
                
                if (filters.indexOf('USAGESETMONTH_ID') != -1) { // remove usage set month from filters (the query string version was replaced above)
                    var filterarr = filters.split('|');
                    filters = '';

                    for (var i = 0; i < filterarr.length; i++) {
                        if (filterarr[i].indexOf('USAGESETMONTH_ID') == -1) {
                            if (filters != '') filters += '|';
                            filters += filterarr[i];
                        }
                    }
                }

                nURL = editQueryStringValue(nURL, 'Filter', filters);
            }

            if (nURL.substring(nURL.length - 1) == '&') {
                nURL = nURL.substring(0, nURL.length - 1);
            }

            return nURL;
        }

        function toggleAllRQMTsForSet(chk, RQMTSetID) {
            var checked = $(chk).is(':checked');

            setRQMTSelectCheckboxState(RQMTSetID, checked);
        }

        function setRQMTSelectCheckboxState(RQMTSetID, checked) {
            $('[id$=grdData_BodyContainer] table tr').each(function () {
                // note, the trs here could also include trs for inner tables, so we specifically check to make sure that we are first child trs of the main grid table
                var theTable = $(this).closest('table');

                if (theTable.attr('id') != null && theTable.attr('id').length > 0) {
                    var cb = $(this).find('input[type=checkbox][typename=RQMTSELECT]');
                    var frame = $(this).find('iframe');

                    if (cb.length > 0) {
                        if ($(cb).attr('rsetid') == RQMTSetID) {
                            $(cb).prop('checked', checked);
                        }
                    }
                    else if (frame.length > 0) {
                        frame[0].contentWindow.setRQMTSelectCheckboxState(RQMTSetID, checked);
                    }
                }
            });            
        }

        function btnCopy_click() {
            var checkedCBs = getCheckedRQMTs();

            if (checkedCBs.length == 0) {
                warningMessage('Please check a RQMT');
            }
            else {                
                PageMethods.CopyRQMTs(checkedCBs, btnCopy_click_done, on_error);
            }
        }

        function btnCopy_click_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success) {
                _copiedRQMTSystems = dt.systemids != null ? dt.systemids : '';
                setTopLevelVariable('_copiedRQMTSystems', dt.systemids != null ? dt.systemids : '');
                showCopiedRQMTsToolTip(dt.rqmtidsstr);
            }
        }

        function showCopiedRQMTsToolTip(rqmts) {
            if (rqmts == null || rqmts.length == 0) {
                hideTopLevelToolTip('rqmtgrid_copiedrqmts');
                return;
            }

            if (rqmts.indexOf('*') != -1) {
                rqmts += '<br />* duplicate RQMT # from different RQMT Set';
            }

            hideTopLevelToolTip('rqmtgrid_copiedrqmts');
            showTopLevelToolTip('<b>Copied RQMTs:</b>&nbsp;' + rqmts + '&nbsp;&nbsp;&nbsp;<span style="font-size:smaller;cursor:pointer;" onclick="clearCopiedRQMTsClicked()">(<u>CLEAR</u>)', $('body')[0], 'rqmtgrid_copiedrqmts', false, 'info', null, 0, 0, 'bottom right');
        }

        function clearCopiedRQMTsClicked() {
            PageMethods.CopyRQMTs('', btnCopy_click_done, on_error);
            hideTopLevelToolTip('rqmtgrid_copiedrqmts');
            _copiedRQMTSystems = '';
            setTopLevelVariable('_copiedRQMTSystems', '');
        }

        function getCheckedRQMTs() {
            var checkedCBs = '';

            $('[id$=grdData_BodyContainer] table tr').each(function () {
                // note, the trs here could also include trs for inner tables, so we specifically check to make sure that we are first child trs of the main grid table
                var theTable = $(this).closest('table');

                if (theTable.attr('id') != null && theTable.attr('id').length > 0) {
                    var cb = $(this).find('input[type=checkbox][typename=RQMTSELECT]');
                    var frame = $(this).find('iframe');

                    if (cb.length > 0 && $(cb).is(':checked')) {
                        if (checkedCBs.length > 0) {
                            checkedCBs += ',';
                        }

                        checkedCBs += $(cb).attr('rqmtid') + '+' + $(cb).attr('systemid') + '+' + $(cb).attr('rsetid');
                    }
                    else if (frame.length > 0) {
                        var frmCheckedCBs = frame[0].contentWindow.getCheckedRQMTs();

                        if (frmCheckedCBs.length > 0) {
                            if (checkedCBs.length > 0) {
                                checkedCBs += ',';
                            }

                            checkedCBs += frmCheckedCBs;
                        }
                    }
                }
            });
            
            return checkedCBs;
        }

        function btnPaste_click() {
            var RQMTSetID = <%=FilterRQMTSetID%>;
            var SYSTEM_ID = <%=FilterSystemID%>;
            if (isTopLevelToolTipVisible('rqmtgrid_copiedrqmts')) {                
                if (RQMTSetID > 0) {
                    var sysarr = _copiedRQMTSystems.split('|');                    
                    var foundOtherSystems = false;
                    for (var i = 0; i < sysarr.length; i++) {
                        if (sysarr[i] != SYSTEM_ID) {
                            foundOtherSystems = true;
                            break;
                        }
                    }      

                    if (foundOtherSystems) {
                        QuestionBox('Paste RQMTs Special', StrongEscape('You are pasting one or more RQMTs from a different system. Attributes, Defects and Descriptions do not normally transfer between systems. To include these items, use the checkboxes below:<br /><br /><input type="checkbox" id="cball" value="all" onclick="document.getElementById(\'cbattr\').checked = this.checked; document.getElementById(\'cbdef\').checked = this.checked; document.getElementById(\'cbdesc\').checked = this.checked;">(all)<br /><input type="checkbox" id="cbattr" value="attr">Attributes<br /><input type="checkbox" id="cbdef" value="def">Defects<br /><input type="checkbox" id="cbdesc" value="desc">Descriptions'), 'Paste,Cancel', 'btnPaste_confirmed', 400, 500, this, RQMTSetID);
                    }
                    else {
                        QuestionBox('Paste RQMTs', 'Paste RQMTs into this set?', 'Yes,No', 'btnPaste_confirmed', 300, 300, this, RQMTSetID);
                    }
                }
            }
            else {
                warningMessage('No RQMTs on clipboard');
            }
        }

        function btnPaste_confirmed(answer, param) {
            if (answer == 'No' || answer == 'Cancel') {
                return;
            }                
            
            infoMessage('Pasting RQMTs...', null, false, null, 'rqmt_paste_msg');

            if (answer == 'Yes') {
                var RQMTSetID = param;                
                PageMethods.PasteRQMTs(RQMTSetID, '', function (result) { pasteRQMTs_done(result, RQMTSetID) }, on_error);
            }
            else if (answer == 'Paste') {
                var parr = param.split('|');
                var RQMTSetID = parr[0];

                PageMethods.PasteRQMTs(RQMTSetID, parr.length == 2 ? parr[1] : '', function (result) { pasteRQMTs_done(result, RQMTSetID) }, on_error);
            }
        }

        function pasteRQMTs_done(result, RQMTSetID) {    
            $('#rqmt_paste_msg').remove();
            refreshPage(true, true);
        }

        function toggleRQMTSetUsageMonth(month, on) {            
            var months = '';

            for (var i = 1; i <= 12; i++) {
                var cb = $('[id$=_grdData_HeaderContainer]').find('input[type=checkbox][rqmtsetusagemonth=' + i + ']');

                if ($(cb).is(':checked')) {
                    if (months != '') months += ',';
                    months += i;
                }
            }

            if (months == '') months = '0';

            defaultParentPage._moduleCachedData['rqmtsetusagemonthselection'] = months;

            $('#<%=this.grdData.ClientID %>_BodyContainer table tr.gridBody').each(function () {
                var row = $(this);
                var frm = row.next().find('iframe');
                
                if (frm.length > 0) {
                    for (var i = 0; i < frm.length; i++) {
                        var msg = $(frm[i]).contents().find('span[id$=_lblMessage]');
                        msg.html('<< Click refresh to apply updates');
                        msg.show();
                    }
                }
            });
        }

    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';

            _WTS_SYSTEMID = <%=this.WTS_SYSTEMID%>;
            _WorkAreaID = <%=this.WorkAreaID%>;
            _SystemSuiteID = <%=this.SystemSuiteID%>;
            _RQMTTypeID = <%=this.RQMTTypeID%>;

            if (defaultParentPage._expandedGridNodes == null) {
                defaultParentPage._expandedGridNodes = [];
            }

            if (defaultParentPage._moduleCachedData == null) {
                defaultParentPage._moduleCachedData = [];
            }

            _rqmtSetUsageMonthCheckboxesExist = <%=RQMTSetMonthCheckBoxLevel%> > 0;
        }

        function initDisplay() {
            $('#imgSort').hide();
             
            if ('<%=this.CanEditRQMT %>'.toUpperCase() == 'TRUE') {
                if ('<%=this.CanEditRQMSystemOrSet %>'.toUpperCase() == 'TRUE') {
                    $('#btnSave').show();
                }
            }
            else {
                $('input,select,textarea').not('select[id$=ddlGridview]').prop('disabled', true);
            }

            if (<%=this.CanViewRQMT.ToString().ToLower()%> && !<%=this.HideBuilderButton.ToString().ToLower()%>) {
                $('#btnBuilder').show();
            }

            if (<%=this.CanAddRQMT.ToString().ToLower()%>) {
                $('#btnAdd').show();
                $('#btnCopy').show();
                $('#btnPaste').show();
                $('#btnDelete').show();
            }
            
            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();

                resizeFrame();
                completeLoading();
            }
            else {
                $('#spnView').show();
                resizeGrid();
            }

            if (_currentLevel > 1 || !<%=this.ShowExport.ToString().ToLower()%>) {
                $('#imgExport').hide();
            }

            if (<%=this.ShowCOG.ToString().ToLower()%>) {
                $('#tdSettings').show();
            }

            if (!<%=this.ShowPageTitle.ToString().ToLower()%>) {
                $('[id$=pageContentHeader]').hide();
            }
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnEdit').click(function () { btnEdit_click(); return false; });
            $('#btnBuilder').click(function () { btnBuilder_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnCopy').click(function () { btnCopy_click(); return false; });
            $('#btnPaste').click(function () { btnPaste_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            $('#imgExport').on('click', function () { imgExport_click(); });
            $('#imgSettings').click(function () { imgSettings_click(); });

            $('#<%=ddlGridview.ClientID %>').on("change", function () { ddlGridView_change(); return false; });
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
            $('#<%=ddlGridview.ClientID %> option').filter(function () { return $.trim($(this).text()) === $("#<%= itisettings.ClientID %>").val(); }).prop('selected', true);
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();

            syncOpenNodes();

            if ('<%=this.IsConfigured.ToString().ToUpper() %>' == 'FALSE') {
                if ('<%=this.PageType %>'.toUpperCase() != 'TASK') {
                    openSettings();
                }
            }      

            if ('<%=CopiedRQMTs%>' != '') {
                showCopiedRQMTsToolTip('<%=CopiedRQMTs%>');
            }

            if ('<%=CopiedRQMTSystems%>' != '') {
                _copiedRQMTSystems = '<%=CopiedRQMTSystems%>';
                setTopLevelVariable('_copiedRQMTSystems', _copiedRQMTSystems);
            }
        });
    </script>
</asp:Content>
