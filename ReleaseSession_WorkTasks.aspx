<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReleaseSession_WorkTasks.aspx.cs" Inherits="ReleaseSession_WorkTasks" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Session Work Tasks</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <asp:Label ID="lblHeader" runat="server"></asp:Label>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>

	<script id="jsEvents" type="text/javascript">
        function refreshPage() {
            document.location.href = document.location.href;
        }

        function openWorkTask(WORKITEM_TASKID, WORKITEMID, TASK_NUMBER) {
            var nWindow = 'PrimaryTask';
            var nTitle = 'Primary Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(WORKITEMID) > 0) {
                nTitle += ' - [' + WORKITEMID + ']';
                nURL += '?workItemID=' + WORKITEMID;
            }

            if (WORKITEM_TASKID != '') {
                nWindow = 'WorkSubTask';
                nTitle = 'Subtask - [' + WORKITEMID + ' - ' + TASK_NUMBER + ']';
                nHeight = 700, nWidth = 850;
                nURL = _pageUrls.Maintenance.TaskEdit + '?workItemID=' + WORKITEMID + '&taskID=' + WORKITEM_TASKID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {
          
        }

        function initEvents() {
            $('#imgRefresh').click(function () { refreshPage(); });
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
        }

        $(document).ready(function () {
            initVariables();
            initControls();            
            initEvents();
            initDisplay();
        });
    </script>
</asp:Content>