<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMT_Tabs.aspx.cs" Inherits="RQMT_Tabs" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>RQMT Tabs</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
</head>
<body>
	<form id="form1" runat="server">
        <div class="pageContentHeader">
            <table style="width: 100%;">
                <tr>
                    <td style="padding-left: 3px;">
                        <asp:Label ID="lblRQMT" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right; padding: 3px;">
                        <input type="button" id="btnBackToGrid" value="Back To RQMT Grid" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divDetails">Details</a></li>
			</ul>
            <div id="divDetails">
                <iframe id="frameDetails" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
		</div>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>
    
	<script id="jsEvents" type="text/javascript">
	    function btnBackToGrid_click() {
	        if (parent.showFrameForGrid) parent.showFrameForGrid('RQMT', false);
	    }

        function tab_click(tabName) {
			switch (tabName.toUpperCase()) {
				case 'DETAILS':
				    if ($('#frameDetails').attr('src') == "javascript:'';") $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.RQMTEdit + window.location.search);
				    break;
			}

			resizePage();
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && (parseInt(newID) < 0 || parseInt(newID) > 0)) {
                nURL = editQueryStringValue(nURL, 'NewRQMT', 'false');
                nURL = editQueryStringValue(nURL, 'RQMTID', newID);
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
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {
            $('#divTabsContainer').tabs({
                heightStyle: "fill",
				collapsible: false,
                active: 0
            });
        }

        function initDisplay() {
            if (parent.showFrameForGrid) $('#btnBackToGrid').show();

            tab_click('Details');
            resizePage();
        }

        function initEvents() {
            $('#btnBackToGrid').click(function () { btnBackToGrid_click(); return false; });
            $('#divTabsContainer ul li a').click(function () { tab_click($(this).text()); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();
        });
    </script>
</body>
</html>
