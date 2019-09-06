<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_Tabs.aspx.cs" Inherits="AOR_Scheduled_Deliverables_Tabs" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Tabs</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
</head>
<body>
	<form id="form1" runat="server">
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divDetails">Details</a></li>
                <li style="display: none;"><a href="#divAORs">AORs (<%=this.AORCount %>)</a></li>
                <li style="display: none;"><a href="#divContracts">Contracts (<%=this.ContractCount %>)</a></li>
			</ul>
            <div id="divDetails">
                <iframe id="frameDetails" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divAORs">
                <iframe id="frameAORs" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divContracts">
                <iframe id="frameContracts" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
		</div>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _attachmentCount = 0, _meetingCount = 0, _CRCount = 0, _taskCount = 0, _AORsCount = 0, _ContractCount = 0;
    </script>
    
	<script id="jsEvents" type="text/javascript">
        function imgAlert_click() {
            var nWindow = 'AORAlerts';
            var nTitle = 'AOR Alert';
            var nHeight = 500, nWidth = 650;
            var nURL = _pageUrls.AORSummaryPopup + window.location.search + '&Type=AOR Alert';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnArchive_click() {
            var nWindow = 'ArchiveAOR';
            var nTitle = 'Archive AOR';
            var nHeight = 200, nWidth = 550;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Archive AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnBackToGrid_click() {
            if (parent.showFrameForGrid) parent.showFrameForGrid('AOR', false);
        }

        function tab_click(tabName) {
            switch (tabName.toUpperCase()) {
                case 'DETAILS':
                    if ($('#frameDetails').attr('src') == "javascript:'';") $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORScheduledDeliverablesEdit + window.location.search);
                    break;
                case 'AORS (' + _AORsCount + ')':
                    if ($('#frameAORs').attr('src') == "javascript:'';") $('#frameAORs').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORScheduledDeliverablesAORs + window.location.search);
                    break;
                case 'CONTRACTS (' + _ContractCount + ')':
                    if ($('#frameContracts').attr('src') == "javascript:'';") $('#frameContracts').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORScheduledDeliverablesContracts + window.location.search);
                    break;
            }

            resizePage();
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && parseInt(newID) > 0) {
                nURL = editQueryStringValue(nURL, 'NewAOR', 'false');
                nURL = editQueryStringValue(nURL, 'AORID', newID);
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            $('#divTabsContainer div iframe').each(function () {
                resizePageElement($(this).attr('id'), 0);
            });

            $('#divTabsContainer div').each(function () {
                resizePageElement($(this).attr('id'), -1);
            });
        }

        function updateTab(tabName, newCount) {
            switch (tabName.toUpperCase()) {
                case 'AORS':
                    $('[href="#divAORs"]').text('AORs (' + newCount + ')');
                    _AORCount = newCount;
                    break;
                case 'CONTRACTS':
                    $('[href="#divContracts"]').text('Contracts (' + newCount + ')');
                    _ContractCount = newCount;
                    break;
            }
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _AORsCount = <%=this.AORCount %>;
            _ContractCount = <%=this.ContractCount %>;
        }

        function initControls() {
            if ('<%=this.NewDeliverable %>'.toUpperCase() == 'FALSE') {
                $('[href="#divAORs"]').closest('li').show();
                $('[href="#divContracts"]').closest('li').show();
            }

            $('#divTabsContainer').tabs({
                heightStyle: "fill",
                collapsible: false,
                active: 0
            });
        }

        function initDisplay() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE' && '<%=this.NewDeliverable %>'.toUpperCase() == 'FALSE') $('#btnArchive').show();
            if (parent.showFrameForGrid) $('#btnBackToGrid').show();

            if ('<%=this.Tab %>' == 'Contracts') {
                $('#divTabsContainer a[href="#divContracts"]').trigger('click');
            }
            else {
                tab_click('Details');
            }

            resizePage();
        }

        function initEvents() {
            $('#imgAlert').click(function () { imgAlert_click(); });
            $('#btnArchive').click(function () { btnArchive_click(); return false; });
            $('#btnBackToGrid').click(function () { btnBackToGrid_click(); return false; });
            $('#divTabsContainer ul li a').click(function () { tab_click($(this).text()); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initEvents();
            initDisplay();
        });
    </script>
</body>
</html>
