﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_SR_Grid.aspx.cs" Inherits="AOR_SR_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">SR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>SR</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Associate Primary Task" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Disassociate Primary Task" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedSRID = 0, _selectedTaskID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAdd_click() {
            var nWindow = 'AddTask';
            var nTitle = 'Add Primary Task';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=SR Task&SRID=' + _selectedSRID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm Task Disassociation', 'Are you sure you want to disassociate this Primary Task from the SR?', 'Yes,No', 'confirmTaskDelete', 300, 300, this);
        }

        function confirmTaskDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Disassociating...', 1);

                    PageMethods.DeleteTask(_selectedTaskID, delete_done, on_error);
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
                MessageBox('Task has been disassociated.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to disassociate. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
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
                        nURL = 'AOR_SR_Grid.aspx' + window.location.search;
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
            if ($(obj).attr('sr_id')) {
                _selectedSRID = $(obj).attr('sr_id');

                if (parseInt(_selectedSRID) > 0) $('#btnAdd').prop('disabled', false);
            }

            if ($(obj).attr('task_id')) {
                _selectedTaskID = $(obj).attr('task_id');

                if (parseInt(_selectedTaskID) > 0) $('#btnDelete').prop('disabled', false);
            }
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

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
                var nHeight = headerTop + bodyTableHeight + pagerHeight + 1;
                var nFrame = getMyFrameFromParent();

                $(nFrame).height(nHeight);
                resizeGrid();
                if (parent.resizeFrame) parent.resizeFrame();
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
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            if ('<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') {
                if ('<%=this.DCC.Contains("SR #") %>'.toUpperCase() == 'TRUE') $('#btnAdd').show();
                if ('<%=this.DCC.Contains("Primary Task") %>'.toUpperCase() == 'TRUE') $('#btnDelete').show();
            }

            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();

                resizeFrame();
                completeLoading();
            }
            else {
                resizeGrid();

                if (parent.updateTab) parent.updateTab('SRs/Primary Tasks', <%=this.RowCount %>);
            }
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>
