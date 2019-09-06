﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Tabs.aspx.cs" Inherits="AOR_Tabs" Theme="Default" %>

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
                        <asp:Label ID="lblAOR" runat="server"></asp:Label>
                        <span id="spnRelease" style="padding-left: 25px; display: none;">Release:&nbsp;<asp:DropDownList ID="ddlRelease" runat="server" Width="200px"></asp:DropDownList></span>
                    </td>
                    <td style="text-align: right; padding: 3px;">
                        <img id="imgAlert" runat="server" src="Images/Icons/exclamation_mark_red.png" title="Alert" alt="Alert" height="17" width="17" style="padding-left: 5px; vertical-align: middle; cursor: pointer; display: none;" />&nbsp;
                        <input type="button" id="btnSearch" value="Go to AOR #" />
                        <input type="text" id="txtAORSearch" name="Search" tabindex="5" maxlength="4" size="2" />
			            <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="vertical-align: middle; display: none;"></iti_Tools_Sharp:Menu>
                        <input type="button" id="btnBackToGrid" value="Back To AOR Grid" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divDetails">Details</a></li>
                <li style="display: none;"><a href="#divAttachments">Attachments (<%=this.AttachmentCount %>)</a></li>
                <li style="display: none;"><a href="#divMeetings">Meetings (<%=this.MeetingCount %>)</a></li>
			</ul>
            <div id="divDetails">
                <iframe id="frameDetails" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divAttachments">
                <iframe id="frameAttachments" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divMeetings">
                <iframe id="frameMeetings" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
		</div>
        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	</form>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _attachmentCount = 0, _meetingCount = 0, _CRCount = 0, _taskCount = 0, _resourceTeamCount = 0, _actionTeamCount = 0, _selectedAORID = 0, _selectedAORReleaseID = 0;
        var _aorChanged = false;
    </script>
    
	<script id="jsEvents" type="text/javascript">
	    function ddlRelease_change() {
	        var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'AORReleaseID', $('#<%=this.ddlRelease.ClientID %>').val());

            window.location.href = 'Loading.aspx?Page=' + nURL;
	    }

	    function imgAlert_click() {
	        var nWindow = 'AORAlerts';
	        var nTitle = 'AOR Alert';
	        var nHeight = 500, nWidth = 650;
	        var nURL = _pageUrls.AORSummaryPopup + window.location.search + '&Type=AOR Alert';
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
        }

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseBuilder':
                    btnReleaseBuilder_click();
                    break;
                case 'MassChange':
                    btnMassChange_click();
                    break;
                case 'CRReport':
                    btnCRReport_click();
                    break;
                case 'DSEReport':
                    btnDSEReport_click();
                    break;
                case 'MoveTask':
                    btnMoveWorkTask_click();
                    break;
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function btnReleaseBuilder_click() {
            var nWindow = 'ReleaseBuilder';
            var nTitle = 'Release Builder';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORReleaseBuilder + window.location.search;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnMassChange_click() {
            var nWindow = 'EntityMassChange';
            var nTitle = 'Mass Change';
            var nHeight = 400, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.MassChangeWizard + window.location.search;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnCRReport_click() {
            var nWindow = 'CRReport';
            var nTitle = 'CR Report';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Reports.CR;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDSEReport_click() {
            var nWindow = 'DSEReport';
            var nTitle = 'DSE Report';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Reports.Release_DSE;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnMoveWorkTask_click() {
            var nWindow = 'MoveWorkTask';
            var nTitle = 'Move Work Task';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=MoveWorkTask';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnBackToGrid_click() {
            if (_aorChanged) {
                QuestionBox('Confirm Back', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmClose', 400, 300, this);
            } else {
                if (parent.showFrameForGrid) parent.showFrameForGrid('AOR', false);
            }
        }

        function confirmClose(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    if ($('#frameDetails')[0].contentWindow.btnSave_click) $('#frameDetails')[0].contentWindow.btnSave_click(true);
                    break;
                case 'DISCARD':
                    if (parent.showFrameForGrid) parent.showFrameForGrid('AOR', false);
                    break;
                default:
                    return;
            }
        }

        function aorChanged(change) {
            _aorChanged = change;
        }

        function tab_click(tabName) {
			switch (tabName.toUpperCase()) {
				case 'DETAILS':
				    if ($('#frameDetails').attr('src') == "javascript:'';") $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AOREdit + window.location.search);
				    break;
			    case 'ATTACHMENTS (' + _attachmentCount + ')':
			        if ($('#frameAttachments').attr('src') == "javascript:'';") $('#frameAttachments').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORAttachments + window.location.search);
			        break;
			    case 'MEETINGS (' + _meetingCount + ')':
			        var nURL = _pageUrls.Maintenance.AORContainer + window.location.search;

                    nURL = editQueryStringValue(nURL, 'GridType', 'AORMeeting');
                    nURL = editQueryStringValue(nURL, 'MenuType', 'AOR');

			        if ($('#frameMeetings').attr('src') == "javascript:'';") $('#frameMeetings').attr('src', 'Loading.aspx?Page=' + nURL);
			        break;
			    case 'CRS (' + _CRCount + ')':
			        if ($('#frameCRs').attr('src') == "javascript:'';") $('#frameCRs').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRs + window.location.search);
			        break;
			    case 'TASKS (' + _taskCount + ')':
			        if ($('#frameTasks').attr('src') == "javascript:'';") $('#frameTasks').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORTasks + window.location.search);
                    break;
                case 'RESOURCES (' + _resourceTeamCount + ')':
                    if ($('#frameResourceTeam').attr('src') == "javascript:'';") $('#frameResourceTeam').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORResourceTeam + window.location.search);
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
                case 'ATTACHMENTS':
                    $('[href="#divAttachments"]').text('Attachments (' + newCount + ')');
                    _attachmentCount = newCount;
                    break;
                case 'MEETINGS':
                    $('[href="#divMeetings"]').text('Meetings (' + newCount + ')');
                    _meetingCount = newCount;
                    break;
                case 'CRS':
                    $('[href="#divCRs"]').text('CRs (' + newCount + ')');
                    _CRCount = newCount;
                    break;
                case 'TASKS':
                    $('[href="#divTasks"]').text('Tasks (' + newCount + ')');
                    _taskCount = newCount;
                    break;
                case 'RESOURCES':
                    $('[href="#divResourceTeam"]').text('Resources (' + newCount + ')');
                    _resourceTeamCount = newCount;
                    break;
            }
        }

        function loadAOR() {
            var nURL = window.location.href;
            nURL = editQueryStringValue(nURL, 'AORID', _selectedAORID);
            nURL = editQueryStringValue(nURL, 'AORReleaseID', _selectedAORReleaseID);

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function btnSearch_click() {
            PageMethods.Search($('#txtAORSearch').val(), search_done, search_on_error);
        }

        function search_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length === 1) {
                _selectedAORID = dt[0].AORID;
                _selectedAORReleaseID = dt[0].AORRelease_ID;
                loadAOR();
            } else {
                MessageBox('Could not find AOR: ' + $('#txtAORSearch').val());
            }
        }

        function search_on_error() {
            MessageBox('Could not find AOR: ' + $('#txtAORSearch').val());
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
            _attachmentCount = <%=this.AttachmentCount %>;
            _meetingCount = <%=this.MeetingCount %>;
            _CRCount = <%=this.CRCount %>;
            _taskCount = <%=this.TaskCount %>;
            _resourceTeamCount = <%=this.ResourceTeamCount %>;
            _actionTeamCount = <%=this.ActionTeamCount %>;
        }

        function initControls() {
            if ('<%=this.NewAOR %>'.toUpperCase() == 'FALSE') {
                $('#spnRelease').show();
                $('[href="#divAttachments"]').closest('li').show();
                $('[href="#divCRs"]').closest('li').show();
                $('[href="#divTasks"]').closest('li').show();
                $('[href="#divResourceTeam"]').closest('li').show();

                if ('<%=this.Source %>' != 'MI') $('[href="#divMeetings"]').closest('li').show();
            }

            $('#divTabsContainer').tabs({
                heightStyle: "fill",
				collapsible: false,
				active: 0
            });
        }

        function initDisplay() {
            if (parent.showFrameForGrid) $('#btnBackToGrid').show();
            if ('<%=this.CanEditAOR%>' === 'True') $('#<%=menuRelatedItems.ClientID%>').css('display', 'inline-block');
            if ('<%=this.Tab %>' == 'Tasks') {
                $('#divTabsContainer a[href="#divTasks"]').trigger('click');
            }
            <%--else if ('<%=this.Tab %>' == 'ResourceTeam') {
                $('#divTabsContainer a[href="#divResourceTeam"]').trigger('click');
            }--%>
            else {
                tab_click('Details');
            }
            
            resizePage();
        }

        function initEvents() {
            $('#<%=this.ddlRelease.ClientID %>').change(function () { ddlRelease_change(); return false; });
            $('#imgAlert').click(function () { imgAlert_click(); });
            $(':text').bind('keydown', function (e) {
                if (e.keyCode === 13 || e.keyCode === 144) {
                    e.preventDefault();
                }
            });
            $('#txtAORSearch').on('keyup', function () {
                formatSearch(this);
                key_press(this);
            });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
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