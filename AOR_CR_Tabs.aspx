﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_CR_Tabs.aspx.cs" Inherits="AOR_CR_Tabs" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Tabs</title>
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
                        <asp:Label ID="lblCR" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right; padding: 3px;">
                        <input type="button" id="btnSearch" value="Go to CR #" />
				        <input type="text" id="txtCRSearch" name="Search" tabindex="5" maxlength="6" size="4" />
			            <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="vertical-align: middle; display: inline-block;"></iti_Tools_Sharp:Menu>
                        <input type="button" id="btnBackToGrid" value="Back To CR Grid" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divDetails">Details</a></li>
                <li style="display: none;"><a href="#divNarratives">Narratives</a></li>
                <li style="display: none;"><a href="#divAORs">AORs (<%=this.AORCount %>)</a></li>
                <li style="display: none;"><a href="#divSRsTasks">SRs/Primary Tasks (<%=this.SRCount %>)</a></li>
			</ul>
            <div id="divDetails">
                <iframe id="frameDetails" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divNarratives">
                <iframe id="frameNarratives" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divAORs">
                <iframe id="frameAORs" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divSRsTasks">
                <iframe id="frameSRsTasks" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
		</div>
        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	</form>

    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _AORCount = 0;
        var _SRCount = 0;
        var _selectedCRID = 0;
    </script>
    
	<script id="jsEvents" type="text/javascript">
	    function btnBackToGrid_click() {
	        if (parent.showFrameForGrid) parent.showFrameForGrid('CR', false);
	    }

        function tab_click(tabName) {
			switch (tabName.toUpperCase()) {
				case 'DETAILS':
				    if ($('#frameDetails').attr('src') == "javascript:'';") $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCREdit + window.location.search);
				    break;
			    case 'NARRATIVES':
			        if ($('#frameNarratives').attr('src') == "javascript:'';") $('#frameNarratives').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRNarratives + window.location.search);
                    break;
                case 'AORS (' + _AORCount + ')':
                    if ($('#frameAORs').attr('src') == "javascript:'';") $('#frameAORs').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRAORs + window.location.search);
                    break;
			    case 'SRS/PRIMARY TASKS (' + _SRCount + ')':
			        if ($('#frameSRsTasks').attr('src') == "javascript:'';") $('#frameSRsTasks').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRSRsTasks + window.location.search);
			        break;
			}

			resizePage();
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && (parseInt(newID) < 0 || parseInt(newID) > 0)) {
                nURL = editQueryStringValue(nURL, 'NewCR', 'false');
                nURL = editQueryStringValue(nURL, 'CRID', newID);
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function refreshNarratives() {
            $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCREdit + window.location.search);
            $('#frameNarratives').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRNarratives + window.location.search);
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
                case 'SRS/TASKS':
                    $('[href="#divSRsTasks"]').text('SRs/Tasks (' + newCount + ')');
                    _SRCount = newCount;
                    break;
            }
        }

        function loadCR() {
            var nURL = window.location.href;
            nURL = editQueryStringValue(nURL, 'CRID', _selectedCRID);

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function openMenuItem(url) {
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

        function btnSearch_click() {
            PageMethods.Search($('#txtCRSearch').val(), search_done, search_on_error);
        }

        function search_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length === 1) {
                _selectedCRID = dt[0].CRID;
                loadCR();
            } else {
                MessageBox('Could not find CR: ' + $('#txtCRSearch').val());
            }
        }

        function search_on_error() {
            MessageBox('Could not find CR: ' + $('#txtCRSearch').val());
        }

        function formatSearch(obj) {
            var text = $(obj).val();

            if (/[^0-9]|^0+(?!$)/g.test(text)) {
                $(obj).val(text.replace(/[^0-9]|^0+(?!$)/g, ''));
            }
        }

        function key_press(obj) {
            if (event.keyCode == 13 || event.keyCode == 144) {
                $('#btnSearch').trigger('click');
            }
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _AORCount = <%=this.AORCount %>;
            _SRCount = <%=this.SRCount %>;
        }

        function initControls() {
            if ('<%=this.NewCR %>'.toUpperCase() == 'FALSE') {
                $('[href="#divNarratives"]').closest('li').show();
                $('[href="#divAORs"]').closest('li').show();
                $('[href="#divSRsTasks"]').closest('li').show();
            }

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
            $(':text').bind('keydown', function (e) {
                if (e.keyCode === 13 || e.keyCode === 144) {
                    e.preventDefault();
                }
            });
            $('#txtCRSearch').on('keyup', function () {
                formatSearch(this);
                key_press(this);
            });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
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