﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_CR_Grid.aspx.cs" Inherits="AOR_CR_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table style="width: 100%; border-collapse: collapse;">
		<tr>
			<td>
                CR&nbsp;(<%=this.TotalCount %>)
			</td>
            <td style="text-align: right;">
                <span id="spnLastImportDate" runat="server"></span>
            </td>
            <td style="width: 85px; text-align: right; padding-right: 9px;">
                <input type="button" id="btnToggleMetrics" value="Show Metrics" style="vertical-align: middle; display: none;" />
            </td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpMetrics" ContentPlaceHolderID="ContentPlaceHolderMetrics" runat="Server">
	<div id="divMetrics" style="padding: 5px; overflow: auto; display: none;"></div>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <table>
        <tr style="height: 30px;">
            <%--<td>
				<span id="spnSRStatus" style="display: none;">SR Status:&nbsp;<select id="ddlSRStatusQF" runat="server" multiple="true" style="width: 150px;"></select></span>
			</td>--%>
            <%--<td>
				<span id="spnCRRelatedRel" style="display: none;">Related Release:&nbsp;<select id="ddlCRRelatedRelQF" runat="server" multiple="true" style="width: 150px;"></select></span>
			</td>
            <td>
				<span id="spnCRStatus" style="display: none;">CR Status:&nbsp;<select id="ddlCRStatusQF" runat="server" multiple="true" style="width: 150px;"></select></span>
			</td>--%>
            <%--<td>
                <span id="spnCRWebsystem" style="display: none;">CR Websystem:&nbsp;<asp:DropDownList ID="ddlCRWebsystemQF" runat="server" Width="150px"></asp:DropDownList></span>
            </td>--%>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                
                <input type="button" id="btnSearch" value="CR Title Search" />
				<asp:TextBox ID="txtCRSearch" runat="server" MaxLength="255"></asp:TextBox>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnEdit" value="View/Edit" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			    <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iframe id="exportIframe" style="display:none;"></iframe>
    <button id="download_file" style="display:none;">Download</button>
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedCRRelatedRelQF = '';
        var _selectedCRStatusesQF = '';
        var _selectedSRStatusesQF = '';
        var _selectedCRContractsQF = '';
        var _selectedCRID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 0;
            var addLeft = 0;
            if (parseInt(_currentLevel) > 1) {
                addtop = 30;
                addLeft = 25;
            }else{
                addtop = 65;
                addLeft = 25;
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
                }).slideDown();
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
                }).slideUp();
            }
        }

        function getMetrics() {
            $('#divMetrics').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            ddlCRContractQF_update();

            PageMethods.GetMetrics(_selectedCRContractsQF, getMetrics_done, getMetrics_error);
        }

        function getMetrics_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No Metrics';
            }
            else {
                nHTML += '<table>';
                
                $.each(dt, function (rowIndex, row) {
                    if (rowIndex == 0) {
                        nHTML += '<tr>';
                    }
                    else if (rowIndex % 5 == 0) {
                        nHTML += '</tr><tr>';
                    }
                    
                    nHTML += '<td><b>' + row.Metric + ':</b></td>';
                    nHTML += '<td style="width: 50px;">' + row.Value + '</td>';

                    if (rowIndex == dt.length - 1) nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divMetrics').html(nHTML);
            resizeFrame();
        }

        function getMetrics_error() {
            $('#divMetrics').html('Error gathering data.');
        }

        function btnToggleMetrics_click() {
            $('#btnToggleMetrics').prop('value', ($('#divMetrics').is(':visible') ? 'Show Metrics' : 'Hide Metrics'));
            $('#divMetrics').slideToggle(function () {
                resizeFrame();

                if (parent._CRMetrics != undefined) parent._CRMetrics = $('#divMetrics').is(':visible');
            });
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function openMenuItem(url) {
            console.log(url);
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
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

        function ddlCRRelatedRelQF_update() {
            var arrCRRelatedRelQF = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');
			
            _selectedCRRelatedRelQF = arrCRRelatedRelQF.join(',');

            resizeFrame();
        }

        function ddlCRRelatedRelQF_close() {
            refreshPage(false);
        }

        function ddlCRStatusQF_update() {
            var arrCRStatusQF =$('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');
			
            _selectedCRStatusesQF = arrCRStatusQF.join(',');

            resizeFrame();
        }

        function ddlCRStatusQF_close() {
            refreshPage(false);
        }

        function ddlSRStatusQF_update() {
            var arrSRStatusQF = $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect('getSelects');
			
            _selectedSRStatusesQF = arrSRStatusQF.join(',');

            resizeFrame();
        }

        function ddlSRStatusQF_close() {
            refreshPage(false);
        }

        function ddlCRContractQF_update() {
            var arrCRContractQF = $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect('getSelects');

            _selectedCRContractsQF = arrCRContractQF.join(',');

            resizeFrame();
        }

        function ddlCRContractQF_close() {
            refreshPage(false);
        }

        function btnAdd_click() {
            if (parent.showFrameForEdit) {
                parent.showFrameForEdit('CR', true, 0, true);
            }
            else {
                var nWindow = 'CR';
                var nTitle = 'CR';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=true&CRID=0';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnDelete_click() {
            QuestionBox('Confirm CR Delete', 'Are you sure you want to delete this CR?', 'Yes,No', 'confirmCRDelete', 300, 300, this);
        }

        function confirmCRDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteCR(_selectedCRID, delete_done, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox('CR has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnEdit_click() {
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('CR', false, _selectedCRID, true);
            }
            else {
                var nWindow = 'CR';
                var nTitle = 'CR';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=false&CRID=' + _selectedCRID;
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

                        arrChanges.push({'crid': $obj.attr('cr_id'), 'field': $obj.attr('field'), 'value': $obj.val()});
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

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];
            
            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="CR Customer Title"]'), function() {
                var nText = $(this).val();
                
                if (nText.length == 0) {
                    if ($.inArray('CR Customer Title cannot be empty.', validation) == -1) validation.push('CR Customer Title cannot be empty.');
                }

                if ($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="CR Customer Title"][value="' + nText + '"]').not($(this)).length > 0) {
                    if ($.inArray('CR Customer Title cannot have duplicates.', validation) == -1) validation.push('CR Customer Title cannot have duplicates.');
                }
            });

            return validation.join('<br>');
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
                        nURL = 'AOR_CR_Grid.aspx' + window.location.search;
                        nURL = editQueryStringValue(nURL, 'CurrentLevel', (parseInt(_currentLevel) + 1));
                    }

                    var filters = [];
                
                    $.each($row.find('td'), function(i) {
                        var nText = $('.gridHeader:eq(1) th:eq(' + i + ')').text();
                        var nVal = encodeURIComponent($(this).text());

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

        function showText(txt) {
            alert(decodeURIComponent(txt));
        }

        function openAOR(AORID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openTask(taskID) {
            var nWindow = 'WorkTask';
            var nTitle = 'Primary Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(taskID) > 0) {
                nTitle += ' - [' + taskID + ']';
                nURL += '?workItemID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function row_click(obj) {
            if ($(obj).attr('cr_id')) {
                _selectedCRID = $(obj).attr('cr_id');

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

        function refreshPage(blnRetainPageIndex) {
            ddlCRRelatedRelQF_update();
            ddlCRStatusQF_update();
            ddlSRStatusQF_update();
            ddlCRContractQF_update();
            var nURL = document.location.href.replace('#', '');
            nURL = editQueryStringValue(nURL, 'SelectedCRRelatedRelQF', _selectedCRRelatedRelQF);
            nURL = editQueryStringValue(nURL, 'SelectedSRStatusesQF', _selectedSRStatusesQF);
            nURL = editQueryStringValue(nURL, 'SelectedCRStatusesQF', _selectedCRStatusesQF);
            nURL = editQueryStringValue(nURL, 'SelectedCRContractsQF', _selectedCRContractsQF);
            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

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
                    var qfHeight = $('.ms-drop :visible').height() + 68;

                    if (bodyTableHeight < qfHeight) bodyTableHeight = qfHeight;
                }

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

        function btnSearch_click() {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'txtSearch', $('#<%=txtCRSearch.ClientID %>').val());

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function btnExport_click() {
            ShowDimmer(true, 'Exporting...', 1);
            var url = window.location.href;
            url = editQueryStringValue(url, 'Export', true);
            $("#exportIframe").attr("src", url);
        }

        function search_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length === 1) {
                _selectedCRID = dt[0].CRID;
                btnEdit_click();
            } else {
                MessageBox('Could not find CR: ' + $('#<%=txtCRSearch.ClientID %>').val());
            }
        }

        function search_on_error() {
            MessageBox('Could not find CR: ' + $('#<%=txtCRSearch.ClientID %>').val());
        }

        function key_press(event) {
            if (event.keyCode == 13 || event.keyCode == 144) {
                $('#btnSearch').trigger('click');
                event.preventDefault();
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';
        }

        function initControls() {
            try {
                

            <%--$('#<%=this.ddlSRStatusQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
				    ,width: 'undefined'
					,onOpen: function() { ddlSRStatusQF_update(); }
					,onClose: function() { ddlSRStatusQF_close(); }
            }).change(function () { ddlSRStatusQF_update(); });--%>

                $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect({
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
					,onOpen: function() { ddlSRStatusQF_update(); }
					,onClose: function() { ddlSRStatusQF_update(); }
                }).change(function () { ddlSRStatusQF_update(); });

                $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect({
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
					,onOpen: function() { ddlCRStatusQF_update(); }
					,onClose: function() { ddlCRStatusQF_update(); }
            }).change(function () { ddlCRStatusQF_update(); });

                $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect({
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
					,onOpen: function() { ddlCRRelatedRelQF_update(); }
					,onClose: function() { ddlCRRelatedRelQF_update(); }
                }).change(function () { ddlCRRelatedRelQF_update(); });

                $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect({
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
                    , onOpen: function () { ddlCRContractQF_update(); }
                    , onClose: function () { ddlCRContractQF_update(); }
                }).change(function () { ddlCRContractQF_update(); });

            lblMessage.hide();

             }
            catch (e) {
                var strError = e.message;
            }
        }

        function initDisplay() {
            $('#imgSort').hide();
            
            //$('#imgExport').hide();

            if ('<%=this.CanEditCR %>'.toUpperCase() == 'TRUE') {
                if (parseInt(_currentLevel) == 1) $('#btnAdd').show();

                if ($('.saveable').length > 0) {
                    $('#btnDelete').show();
                    $('#btnSave').show();
                }
            }

            if ('<%=this.CanViewCR %>'.toUpperCase() == 'TRUE' && '<%=this.grdData.Rows.Count %>' != '0' && '<%=this.DCC.Contains("CR_ID") %>'.toUpperCase() == 'TRUE') $('#btnEdit').show();

            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();
                //$('#spnCRRelatedRel').hide();
                //$('#spnCRStatus').hide();
                //$('#spnSRStatus').hide();
                $('#tdQuickFilters').hide();
                $('#btnSearch').hide();
                $('#<%=txtCRSearch.ClientID %>').hide();
                if (parseInt(_currentLevel) == 2) {
                    $('#spnSRStatus').show();
                    $('#tdQuickFilters').show();
                    $('#trms_Item2').show();
                    $('#ms_Item2').show();
                    $('#trClearAll').hide();
                }
                resizeFrame();
                completeLoading();
            }
            else {
                $('#btnToggleMetrics').show();
                //$('#spnCRStatus').show();
                //$('#spnCRRelatedRel').show();
                //$('#spnSRStatus').hide();
                //$('#spnCRWebsystem').show();
                $('#tdQuickFilters').show();
                $('#trms_Item0').show();
                $('#trms_Item1').show();
                $('#trms_Item10').show();
                $('#trClearAll').hide();
                $('#<%=menuRelatedItems.ClientID %>').show();
                resizeGrid();
            }
        }

        function initEvents() {
 
            $('#imgExport').click(function () { btnExport_click();});
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnToggleMetrics').click(function () { btnToggleMetrics_click(); return false; });
            $('#<%=this.txtCRSearch.ClientID%>').on('keydown', function (e) {
                key_press(e);
            });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnEdit').click(function () { btnEdit_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            $('#btnQuickFilters').click(function() { toggleQuickFilters_click(); });
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();
            getMetrics();

            if (parent._CRMetrics != undefined && parent._CRMetrics) $('#btnToggleMetrics').trigger('click');
            resizeGrid();
        });
    </script>
</asp:Content>