<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Release_Assessment_Deployment.aspx.cs" Inherits="AOR_Release_Assessment_Deployment" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Release Assessment Deployments</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Deployments</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Disassociate" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAdd_click() {
            var nWindow = 'AddReleaseAssessmentDeployment';
            var nTitle = 'Add Deployment';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Deployment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            var checkedCount = $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').length;

            if (checkedCount > 0) {
                QuestionBox('Confirm Deployment Disassociation', 'Are you sure you want to disassociate the checked Deployment' + (checkedCount > 1 ? 's' : '') + ' from the Release Assessment?', 'Yes,No', 'confirmReleaseAssessmentDeploymentDelete', 300, 300, this);
            }
            else {
                MessageBox('Please check at least one.');
            }
        }

        function confirmReleaseAssessmentDeploymentDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    var arrReleaseAssessmentDeployment = [];

                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
	                    var $obj = $(this).parent();

                        arrReleaseAssessmentDeployment.push({ 'releaseassessmentdeploymentid': $obj.attr('releaseassessmentdeploymentid') });
                    });

                    ShowDimmer(true, 'Disassociating...', 1);

                    var nJSON = JSON.stringify(arrReleaseAssessmentDeployment);

                    PageMethods.DeleteReleaseAssessmentDeployment(nJSON, function (result) { delete_done(result, arrReleaseAssessmentDeployment.length); }, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result, checkedCount) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox((checkedCount > 1 ? 'Deployments have' : 'Deployment has') + ' been disassociated.');
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

        function checkbox_click() {
            if ($('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').length > 0) $('#btnDelete').prop('disabled', false);
            else $('#btnDelete').prop('disabled', true);
        }

        function openDeployment(deploymentID) {
            var nWindow = 'Deployment';
            var nTitle = 'Deployment';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=false&DeliverableID=' + deploymentID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#pageContentHeader').hide();

            if ('<%=this.CanEdit %>'.toUpperCase() == 'TRUE') {
                $('#btnAdd').show();

                if (true) $('#btnDelete').show();
            }

            resizeGrid();

            if (parent.updateTab) parent.updateTab('Deployments', <%=this.RowCount %>);
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnViewReleaseHistory').click(function () { btnViewReleaseHistory_click(); return false; });
            $('#btnViewChangeHistory').click(function () { btnViewChangeHistory_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            if (true) $('input[type="checkbox"]').click(function () { checkbox_click(this); });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();

            if (parent.resizeFrame) {
                parent.resizeFrame('Deployments');
            }

        });
    </script>
</asp:Content>
