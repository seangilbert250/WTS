﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_AORs.aspx.cs" Inherits="AOR_Scheduled_Deliverables_AORs" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CR AOR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>AORs</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" disabled="disabled" style="vertical-align: middle;" />
                <input type="button" id="btnDelete" value="Disassociate" disabled="disabled" style="vertical-align: middle;" />
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
        var _selectedAORReleaseDeliverableID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAdd_click() {
            var nWindow = 'AddCRAOR';
            var nTitle = 'Add AOR';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Release Schedule AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm AOR/Release Disassociation', 'Are you sure you want to disassociate this AOR/Release from this Deployment?', 'Yes,No', 'confirmDeliverableAORDelete', 300, 300, this);
        }

        function confirmDeliverableAORDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Disassociating...', 1);

                    PageMethods.DeleteDeliverableAOR(_selectedAORReleaseDeliverableID, delete_done, on_error);
                    if (parent.opener) parent.opener.refreshPage(true);
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
                MessageBox('AOR/Release has been disassociated.');
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

        function openAOR(AORID, AORReleaseID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function row_click(obj) {
            if (_canEditAOR) {
                var $obj = $(obj);

                if ($obj.attr('aorreleasedeliverable_id') && $obj.attr('aor_id') && $obj.attr('aorrelease_id')) {
                    _selectedAORReleaseDeliverableID = $obj.attr('aorreleasedeliverable_id');

                    PageMethods.CanEdit($obj.attr('aor_id'), $obj.attr('aorrelease_id'), canEdit_done, function () { $('#btnDelete').prop('disabled', false); });
                }
                else {
                    _selectedAORReleaseDeliverableID = 0;
                    $('#btnDelete').prop('disabled', false);
                }
            }
        }

        function canEdit_done(result) {
            var blnCanEdit = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.canEdit && obj.canEdit.toUpperCase() == 'TRUE') blnCanEdit = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnCanEdit) {
                $('#btnDelete').prop('disabled', false);
            }
            else {
                $('#btnDelete').prop('disabled', true);
            }
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);

            //iti_Tools ResizeGrid() doesn't work sometimes in certain environments
            var $grid = $('#<%=this.grdData.ClientID %>_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#<%=this.grdData.ClientID %>_BodyContainer table').height();
            var pagerHeight = $('#<%=this.grdData.ClientID %>_PagerContainer').is(':visible') ? $('#<%=this.grdData.ClientID %>_PagerContainer').height() : 0;
            var bodyHeight = $('#<%=this.grdData.ClientID %>_BodyContainer').height();
            if (bodyTableHeight < bodyHeight) bodyHeight = bodyTableHeight - pagerHeight + 3;
            var pagerTop = headerTop + bodyHeight + pagerHeight - 5;
            $('#<%=this.grdData.ClientID %>_PagerContainer').css('top', pagerTop + 'px');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _canEditAOR = ('<%=this.CanEditAOR.ToString().ToUpper()%>' == 'TRUE');
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            resizeGrid();
            parent.updateTab('AORs', <%=this.RowCount %>);
            if (parent.updateTab) parent.updateTab('AORs', <%=this.RowCount %>);
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            if (_canEditAOR) {
                $('#btnAdd').click(function () { btnAdd_click(); return false; });
                $('#btnDelete').click(function () { btnDelete_click(); return false; });
                $('#btnAdd').attr('disabled', false);
            }
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