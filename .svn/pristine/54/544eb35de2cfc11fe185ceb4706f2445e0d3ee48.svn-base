<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Home_Tabs.aspx.cs" Inherits="AOR_Home_Tabs" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Home Tabs</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
</head>
<body>
	<form id="form1" runat="server">
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divSummary">Release Metrics by Deployments</a></li>
                <li><a href="#divSettings">Settings</a></li>
			</ul>
            <div id="divSummary">
                <iframe id="frameSummary" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divSettings">
                <iframe id="frameSettings" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
		</div>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>
    
	<script id="jsEvents" type="text/javascript">
        function tab_click(tabName) {
			switch (tabName.toUpperCase()) {
				case 'SUMMARY':
				    if ($('#frameSummary').attr('src') == "javascript:'';") $('#frameSummary').attr('src', 'Loading.aspx?Page=' + _pageUrls.AORSummary);
				    break;
			    case 'SETTINGS':
			        if ($('#frameSettings').attr('src') == "javascript:'';") $('#frameSettings').attr('src', 'Loading.aspx?Page=' + _pageUrls.AORHome);
			        break;
			}

			resizePage();
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
            tab_click('Summary');
            resizePage();
        }

        function initEvents() {
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
