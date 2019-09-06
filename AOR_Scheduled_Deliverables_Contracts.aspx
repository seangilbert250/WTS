<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_Contracts.aspx.cs" Inherits="AOR_Scheduled_Deliverables_Contracts" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Contract</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Contracts</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle;" />
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
        var _selectedDeploymentContractID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAdd_click() {
            var nWindow = 'AddContract';
            var nTitle = 'Add Contract';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Contract';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm Contract Disassociation', 'Are you sure you want to disassociate this Contract from this Deployment?', 'Yes,No', 'confirmDelete', 300, 300, this);
        }

        function confirmDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Disassociating...', 1);

                    PageMethods.DeleteDeploymentContract(_selectedDeploymentContractID, delete_done, on_error);
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
                MessageBox('Contract has been disassociated.');
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

        function row_click(obj) {
            var $obj = $(obj);

            if ($obj.attr('deploymentcontract_id')) {
                _selectedDeploymentContractID = $obj.attr('deploymentcontract_id');
            }
            else {
                _selectedDeploymentContractID = 0;
            }
            $('#btnDelete').prop('disabled', false);
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
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            resizeGrid();
            parent.updateTab('Contracts', <%=this.RowCount %>);
            if (parent.updateTab) parent.updateTab('Contracts', <%=this.RowCount %>);
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
