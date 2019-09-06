﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Grid.aspx.cs" Inherits="AOR_Meeting_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Meeting</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Meeting</span>
    <img id="imgDownloadPDF" src="Images/Icons/doc_pdf.png" alt="Download PDF" title="Download PDF" width="15" height="15" style="cursor: pointer; margin-left: 3px;" />
    <img id="imgEmailPDF" src="Images/Icons/email.png" alt="E-Mail PDF" title="E-Mail PDF" width="15" height="15" style="cursor: pointer; margin-left: 3px; position:relative;" />
    <div id="divDownloadPDFSettings" style="text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none; z-index: 100;">
        <div id="divDlMiContainer" style="text-align:left;border-bottom:1px solid #aaaaaa;">
            <div id="divDlMiName" style="padding-bottom:5px;">
                <select id="ddlDlMiName" style="width:200px">
                    <option value="0">Meeting Name</option>
                </select>
            </div>
            <div id="divDlMiInstance" style="padding-bottom:5px;">
                <select id="ddlDlMiInstance" style="width:200px" disabled="true">
                    <option value="0">Meeting Instance</option>
                </select>
            </div>
        </div>
        <div id="divDownloadItemsContainer" style="text-align: left;">
            <div id="divEmailCustomNote" style="display:none;border-bottom:1px solid #aaaaaa;padding-bottom:3px;padding-top:3px;"><b>Custom Note:</b> &nbsp; <input type="text" id="txtEmailCustomNote" style="width:200px" maxlength="200"></div>
            <asp:CheckBoxList ID="cblDownloadPDFSettings" runat="server"></asp:CheckBoxList>
        </div>
        <div id="divRecipientListContainer" style="text-align: left; display: none;"></div>
        <br />
        <input type="button" id="btnDownloadPDF" value="Download" style="vertical-align: middle;" />
        <input type="button" id="btnSelectRecipients" value="Recipients" style="vertical-align: middle; display:none;" />
        <input type="button" id="btnSelectAllRecipients" emailbuttons="1" value="Select All" style="vertical-align: middle; display:none;" />
        <input type="button" id="btnClearAllRecipients" emailbuttons="1" value="Clear All" style="vertical-align: middle; display:none;" />
        <input type="button" id="btnResetRecipients" emailbuttons="1" value="Reset" style="vertical-align: middle; display:none;" />
    </div>
    <div style="position:absolute;right:5px;top:5px;">
        <input id="btnShowMetrics" type="button" value="Show Metrics">
    </div>
</asp:Content>
<asp:Content ID="cpMetrics" ContentPlaceHolderID="ContentPlaceHolderMetrics" runat="Server">
	<div id="divMetrics" style="padding: 5px; overflow: auto; display: none;">Loading metrics...</div>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <span id="spnView" style="display: none;">
        &nbsp;&nbsp;View: 
        <asp:DropDownList ID="ddlView" runat="server" Width="100px">
            <asp:ListItem Value="Week" Selected="True"></asp:ListItem>
            <asp:ListItem Value="Meeting"></asp:ListItem>
            <asp:ListItem Value="AOR"></asp:ListItem>
        </asp:DropDownList>
    </span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnStartMeeting" value="Start Meeting" style="vertical-align:middle; display: none;" />
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnEdit" value="View/Edit" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnAddMI" value="Add Meeting" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDeleteMI" value="Delete Meeting" disabled="disabled" style="vertical-align: middle; display: none;" />
			    <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <div id="divstartmeeting" style="display:none;padding:25px 5px 25px 5px;text-align:center;">
        <table cellpadding="0" cellspacing="0" style="width:390px;">
            <tr>
                <td align="center">
                    <div id="btnUseExistingMeeting" style="border:1px solid #4a65aa;background-color:#d5ddf1;color:#000000;border-radius:5px;width:150px;padding:3px;cursor:pointer;">Use Existing Meeting</div>
                </td>
                <td align="center">
                    <div id="btnCreateNewMeeting" style="border:1px solid #4a65aa;background-color:#d5ddf1;color:#000000;border-radius:5px;width:150px;padding:3px;cursor:pointer;">Create New Meeting</div>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnMeetings" Value="" />
    <iframe id="frmDownload" style="display: none;"></iframe>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedAORMeetingID = 0;
        var _selectedAORMeetingInstanceID = 0;
        var _toggleAllExecuting = false;
        var _toggleAllExecutingOpen = false;
    </script>

    <script id="jsEvents" type="text/javascript">
        function ddlView_change() {
            refreshPage(false);
        }

        function imgRefresh_click() {
            refreshPage(false);
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

        function btnStartMeeting_click() {
            var nHeight = 80;
            var nWidth = 400;
            var openPopup = popupManager.AddPopupWindow('wndStartMeeting', 'Start Meeting', null, nHeight, nWidth, 'PopupWindow', this, false, '#divstartmeeting');

            if (openPopup) openPopup.Open();
        }

        function btnUseExistingMeeting_click() {
            openAORMeetingInstance(0, 0);

            popupManager.GetPopupByName('wndStartMeeting').Close();
        }

        function btnCreateNewMeeting_click() {            
            btnAdd_click();
            popupManager.GetPopupByName('wndStartMeeting').Close();
        }

        function btnAdd_click() {
            if (parent.showFrameForEdit) {
                parent.showFrameForEdit('AORMeeting', true, 0, true);
            }
            else {
                var nWindow = 'AORMeeting';
                var nTitle = 'Meeting';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORMeetingEdit + window.location.search + '&NewAORMeeting=true&AORMeetingID=0';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnDelete_click() {
            QuestionBox('Confirm Meeting Delete', 'Are you sure you want to delete this Meeting?', 'Yes,No', 'confirmAORMeetingDelete', 300, 300, this);
        }

        function confirmAORMeetingDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteAORMeeting(_selectedAORMeetingID, delete_done, on_error);
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
                MessageBox('Meeting has been deleted.');
                setTimeout(refreshPage(true), 1);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnEdit_click() {
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('AORMeeting', false, _selectedAORMeetingID, true);
            }
            else {
                var nWindow = 'AORMeeting';
                var nTitle = 'Meeting';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORMeetingEdit + window.location.search + '&NewAORMeeting=false&AORMeetingID=' + _selectedAORMeetingID;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('input[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                        var $obj = $(this);

                        arrChanges.push({'aormeetingid': $obj.attr('aormeeting_id'), 'field': $obj.attr('field'), 'value': $obj.val()});
                    });

                    if (arrChanges.length > 0) {
                        ShowDimmer(true, 'Saving...', 1);

                        var nJSON = '{update:' + JSON.stringify(arrChanges) + '}';

                        PageMethods.SaveChanges(nJSON, save_done, on_error);
                    }
                    else {
                        MessageBox('You have not made any changes.');
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                var fieldCount = $('input[fieldChanged="1"], select[fieldChanged="1"]').length;
                var rowCount = $('tr[rowChanged="1"]').length;

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="Meeting Name"]'), function() {
                var nText = $(this).val();
                
                if (nText.length == 0) {
                    if ($.inArray('Meeting Name cannot be empty.', validation) == -1) validation.push('Meeting Name cannot be empty.');
                }

                if ($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="Meeting Name"][value="' + nText + '"]').not($(this)).length > 0) {
                    if ($.inArray('Meeting Name cannot have duplicates.', validation) == -1) validation.push('Meeting Name cannot have duplicates.');
                }
            });

            return validation.join('<br>');
        }

        function btnAddMI_click() {
            openAORMeetingInstance(<%=AORMeetingID%>, 0);
        }

        function btnDeleteMI_click() {
            QuestionBox('Confirm Meeting Instance Delete', 'Are you sure you want to delete this Meeting Instance?', 'Yes,No', 'confirmAORMeetingInstanceDelete', 300, 300, this);
        }

        function confirmAORMeetingInstanceDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteAORMeetingInstance(_selectedAORMeetingInstanceID, deleteMI_done, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function deleteMI_done(result) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox('Meeting Instance has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function openSettings() {
            return; //todo:

            if (parent.openSettings) {
                parent.openSettings();
                return;
            }

            var nTitle = 'Crosswalk Parameters';
            var nHeight = 800, nWidth = 420;
            var nURL = 'CrosswalkParametersSections.aspx?GridType=AORMeeting';
            var openPopup = popupManager.AddPopupWindow('CrosswalkParams', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function displayAllRows(obj) {
            var $obj = $(obj);

            _toggleAllExecuting = true;

            if ($obj.attr('title') == 'Expand') {
                _toggleAllExecutingOpen = true; // we are collapsing the grid
            }
            else {
                _toggleAllExecutingOpen = false; // we are opening the grid
            }

    	    $('#<%=this.grdData.ClientID %>_BodyContainer table td img').each(function () {
    	        $(this).click();
    	    });

    	    if ($obj.attr('title') == 'Expand') {
    	        $obj.attr('src', 'Images/Icons/minus_blue.png');
    	        $obj.attr('title', 'Collapse');
    	        $obj.attr('alt', 'Collapse');
    	    }
    	    else {
    	        $obj.attr('src', 'Images/Icons/add_blue.png');
    	        $obj.attr('title', 'Expand');
    	        $obj.attr('alt', 'Expand');
            }

            _toggleAllExecuting = false;
        }

        function displayNextRow(obj) {
            var $obj = $(obj);

            if ($obj.attr('title') != 'Loading...') {
                var $row = $obj.closest('tr');

                var rowIsCurrentlyVisible = $row.next().find('iframe').length > 0 && $row.next().find('iframe').is(':visible');

                var showRow = false;

                if (_toggleAllExecuting) {
                    showRow = _toggleAllExecutingOpen;
                }
                else {
                    showRow = !rowIsCurrentlyVisible;
                }

                var blnShow = true;

                if (showRow) {
                    if ($row.next().find('iframe').length == 0) { // if a frame already exists, we don't create it again (this would happen if they had already expanded a child node and a parent "open all" was clicked
                        $obj.attr('src', 'Images/Loaders/loader_2.gif');
                        $obj.attr('title', 'Loading...');
                        $obj.attr('alt', 'Loading...');
                        $obj.css('cursor', 'default');

                        var nURL = '';

                        if (_currentLevel == '<%=this.LevelCount %>') {
                            return;
                        }
                        else {
                            nURL = 'AOR_Meeting_Grid.aspx' + window.location.search;
                            nURL = editQueryStringValue(nURL, 'CurrentLevel', (parseInt(_currentLevel) + 1));
                        }

                        var filters = [];

                        $.each($row.find('td'), function (i) {
                            var nText = $('[id*=_grdData_]').find('.gridHeader:eq(1) th:eq(' + i + ')').text();
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
                }
                else {
                    blnShow = false;
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

        function openAORMeetingInstance(AORMeetingID, AORMeetingInstanceID) {
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('AORMeetingInstance', false, AORMeetingID, true, AORMeetingInstanceID);
            }
            else {
                var nWindow = 'AORMeetingInstance';
                var nTitle = 'Meeting Instance';
                var nHeight = 700, nWidth = 1400;
                var nURL = _pageUrls.Maintenance.AORMeetingInstanceEdit + window.location.search + '&AORMeetingID=' + AORMeetingID + '&NewAORMeetingInstance=false&AORMeetingInstanceID=' + AORMeetingInstanceID;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function openAOR(AORID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function row_click(obj) {
            if ($(obj).attr('aormeeting_id')) {
                _selectedAORMeetingID = $(obj).attr('aormeeting_id');

                $('#btnDelete').prop('disabled', false);
                $('#btnEdit').prop('disabled', false);
            }

            if ($(obj).attr('aormeetinginstance_id')) {
                _selectedAORMeetingInstanceID = $(obj).attr('aormeetinginstance_id');

                $('#btnDeleteMI').prop('disabled', false);
            }
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            switch($obj.attr('field')) {
                case 'Sort':
                    var nVal = $obj.val();
                    var blnNegative = nVal.indexOf('-') != -1 ? true : false;
                    
                    nVal = nVal.replace(/[^\d]/g, '');
                    
                    if (blnNegative) nVal = '-' + nVal;

                    $obj.val(nVal);
                    break;
                case 'AOR Meeting Name':
                    break;
            }

            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            switch($obj.attr('field')) {
                case 'Sort':
                    if (nVal == '-') $obj.val('');
                    return;
            }

            $obj.val($.trim(nVal));
        }

        function imgDownloadPDF_click() {
            showDownloadPDFSettings();
        }

        function imgEmailPDF_click() {
            showDownloadPDFSettings(true);
        }

        function isEmailMode() {
            return $('#btnDownloadPDF').attr('value') == 'Send E-Mail';
        }

        function isRecipientsMode() {
            return $('#divRecipientListContainer').is(':visible');
        }

        function showDownloadPDFSettings(email) {
            var $objDiv = $('#divDownloadPDFSettings');

            var currentModeEmail = isEmailMode();

            if ($objDiv.is(':visible')) {
                if (!email && currentModeEmail) {
                    // we are just swapping from email to download mode, so swap labels instead of close
                    $('#btnDownloadPDF').attr('value', 'Download');
                    $('#btnSelectRecipients').hide();
                    $('#divEmailCustomNote').hide();
                    $('[emailbuttons=1]').hide();
                    $('#divDownloadItemsContainer').show();
                    $('#divRecipientListContainer').hide();
                }
                else if (email && !currentModeEmail) {
                    // we are just swapping from download to email mode, so swap labels instead of close
                    $('#btnDownloadPDF').attr('value', 'Send E-Mail');
                    $('#btnSelectRecipients').show();
                    $('#divEmailCustomNote').show();
                    $('#btnSelectRecipients').attr('value', 'Recipients');
                    $('[emailbuttons=1]').show();
                    $('#divDownloadItemsContainer').show();
                    $('#divRecipientListContainer').hide();
                }
                else {
                    $objDiv.slideUp();
                }
            }
            else {
                if (email) {
                    $('#btnDownloadPDF').attr('value', 'Send E-Mail');
                    $('#btnSelectRecipients').show();
                    $('#divEmailCustomNote').show();
                    $('#btnSelectRecipients').attr('value', 'Recipients');
                    $('[emailbuttons=1]').hide();
                    $('#divDownloadItemsContainer').show();
                    $('#divRecipientListContainer').hide();
                }
                else {
                    $('#btnDownloadPDF').attr('value', 'Download');
                    $('#btnSelectRecipients').hide();
                    $('#divEmailCustomNote').hide();
                    $('[emailbuttons=1]').hide();
                    $('#divDownloadItemsContainer').show();
                    $('#divRecipientListContainer').hide();
                }

                var $pos = $('#imgDownloadPDF').position();
                var $pageContainerPos = $('#pageContentInfo').position();

                $objDiv.css({
                    top: ($pageContainerPos.top),
                    left: ($pos.left)
                }).slideDown();

                //$('#<%=this.cblDownloadPDFSettings.ClientID %> input').prop('checked', false);                
            }
        }

        function btnSelectRecipients_click() {
            var currentModeRecipients = isRecipientsMode();

            if (currentModeRecipients) {
                $('#divDownloadItemsContainer').show();
                $('#divRecipientListContainer').hide();
                $('[emailbuttons=1]').hide();
                $('#btnSelectRecipients').attr('value', 'Recipients');
            }
            else {
                $('#divDownloadItemsContainer').hide();
                $('#divRecipientListContainer').show();
                $('[emailbuttons=1]').show();
                $('#btnSelectRecipients').attr('value', 'Items');
            }
        }

        function btnSelectAllRecipients_click() {
            $("[id=cbResourceEmail]").prop("checked", true);
            sortEmailResources();
        }

        function btnClearAllRecipients_click() {
            $("[id=cbResourceEmail]").prop("checked", false);
            sortEmailResources();
        }

        function btnResetRecipients_click() {
            $("[id=cbResourceEmail]").prop("checked", false);
            $("[id=cbResourceEmail][original_value=1]").prop("checked", true);
            sortEmailResources();
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'View', $('#<%=this.ddlView.ClientID %>').val());
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

        function ddlDlMiName_change() {
            var mtgID = $('#ddlDlMiName').val();
            var ddl = $('#ddlDlMiInstance');

            clearNoteTypeCounts();

            if (mtgID > 0) {

                $(ddl).empty();
                $(ddl).append($('<option>', { value: '0', text: 'Loading Instances...' }));
                $(ddl).prop('disabled', true);   

                if (_meetingInstances['id_' + mtgID] != null) {
                    getMeetingInstances_done('[CACHED]' + _meetingInstances['id_' + mtgID]);
                }
                else {
                    PageMethods.GetMeetingInstances(mtgID, getMeetingInstances_done, on_error);
                }
            }
            else {
                ddl.prop('disabled', true);
                ddl.val('0');
            }            
        }

        function getMeetingInstances_done(result) {
            var ddl = $('#ddlDlMiInstance');

            if (result != null) {
                var instances = null;

                if (result.indexOf('[CACHED]') == 0) {
                    instances = result.substring(8);
                }
                else {
                    var obj = $.parseJSON(result);
                    instances = obj.instances;
                    _meetingInstances['id_' + obj.AORMeetingID] = instances;
                }

                if (instances != null && instances.length > 0) {
                    $(ddl).empty();
                    $(ddl).append($('<option>', { value: '0', text: 'Meeting Instance' }));

                    var formats = [];
                    formats.push('M/D/YYYY');
                    formats.push('M/D/YY');
                    formats.push('MM/DD/YYYY');
                    formats.push('MM/DD/YY');
                    formats.push('M/D');
                    formats.push('MM/DD');

                    _.each(instances.split(';'), function (ins) {
                        if (ins != null && ins.length > 0) {
                            var insArr = ins.split('=');
                            var text = '(' + insArr[1] + ') ' + unescape(insArr[0]).replace(/\+/g, ' ');

                            if (insArr.length == 3 && insArr[2].length > 0) { // if 2 (or blank), then the meeting hasn't occurred yet so don't add date
                                // a lot of meeting names have the date in the name itself; if that's the case, we try to be smart and not put
                                // it in a second time (e.g. 'FHP Developer Meeting 11/30' doesn't need to have (11/30/2017) added to it)
                                var m = moment(insArr[2], 'MM/DD/YYYY');

                                var found = false;

                                for (var i = 0; i < formats.length && !found; i++) {
                                    var f = formats[i];

                                    for (var j = 0; j < 3; j++) { // MM/DD/YYYY
                                        if (j == 1) f = f.replace(/\//g, '.') // MM.DD.YYYY
                                        else if (j == 2) f = f.replace(/\./g, '-'); // MM-DD-YYYY

                                        if (text.indexOf(m.format(f)) >= 0) {
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                if (!found) {
                                    text += ' (' + insArr[2] + ')';
                                }                                
                            }                                

                            $(ddl).append($('<option>', { value: insArr[1], text: text }));
                        }
                    });

                    $(ddl).prop('disabled', false);
                }
                else {
                    $(ddl).empty();
                    $(ddl).append($('<option>', { value: '0', text: 'Meeting Instance' }));
                    $(ddl).prop('disabled', true);
                }
            }
            else {
                $(ddl).empty();
                $(ddl).append($('<option>', { value: '0', text: 'Meeting Instance' }));
                $(ddl).prop('disabled', true);

                warningMessage('WARNING! Cannot parse meeting result.');
            }
        }

        function ddlDlMiInstance_change() {
            var mtgID = $('#ddlDlMiName').val();
            var insID = $('#ddlDlMiInstance').val();
                        
            clearNoteTypeCounts();

            if (mtgID > 0 && insID > 0) {
                for (var i = 1; i <= 4; i++) {
                    if (_meetingInstanceCounts['id_' + insID + '.' + i] != null) {
                        getMeetingInstanceCounts_done('[CACHED]' + _meetingInstanceCounts['id_' + insID + '.' + i], i);
                    }
                    else {
                        PageMethods.GetMeetingInstanceCounts(mtgID, insID, i, getMeetingInstanceCounts_done, on_error);
                    }
                }
            }
        }

        function clearNoteTypeCounts() {
            $('#<%=this.cblDownloadPDFSettings.ClientID %> label').each(function () {
                var cb = $(this).prev();

                var val = $(cb).attr('value');
                                
                if (val == 'Burndown Grid') val = 'Grid';
                if (val == 'Burndown Overview') val = 'Notes';
                if (val == 'BurndownOverviewParent') val = 'Burndown Overview';
                $(this).text(val);
                $(cb).prop('checked', false);
                
            });

            $('#divRecipientListContainer').html('');
        }

        function getMeetingInstanceCounts_done(result, idx) {
            if (result != null) {
                var counts;
                var countidx = 0;

                if (result.indexOf('[CACHED]') == 0) {
                    counts = result.substring(8);
                    countidx = idx;
                }
                else {
                    var obj = $.parseJSON(result);
                    counts = obj.counts;
                    countidx = obj.countidx;

                    _meetingInstanceCounts['id_' + obj.AORMeetingInstanceID + '.' + countidx] = counts;
                }

                if (counts != null && counts.length > 0) {
                    var typeCounts = [];

                    if (countidx == 1) { // aors
                        typeCounts['AORs Included'] = counts;
                    }
                    else if (countidx == 2) { // notes
                        // counts = Agenda Items=3;Action Items=0;Burndown Overview=8;...
                        var countArr = counts.split(';');

                        for (var i = 0; i < countArr.length; i++) {
                            if (countArr[i].length > 0) {
                                var noteArr = countArr[i].split('=');
                                typeCounts[noteArr[0]] = noteArr[1];
                            }
                        }
                    }
                    else if (countidx == 3) { // att
                        typeCounts['Attachments'] = counts;
                    }
                    else if (countidx == 4) { // resources attending
                        var resources = [];

                        var resourceList = counts.split(';');

                        if (resourceList != null && resourceList.length > 0) {
                            for (var i = 0; i < resourceList.length; i++) {
                                var rscArr = resourceList[i].split('=');

                                if (rscArr.length == 3) {
                                    var rsc = {};
                                    rsc.Resource = unescape(rscArr[0]).replace(/\+/g, ' ');
                                    rsc.WTS_RESOURCEID = rscArr[1];
                                    rsc.EmailDefault = rscArr[2];

                                    resources.push(rsc);
                                }
                            }
                        }

                        $('#divRecipientListContainer').html('');

                        if (resources.length > 0) {
                            var rHTML = '<table style="border-collapse: collapse; width: 100%;">';

                            for (var i = 0; i < resources.length; i++) {
                                var rsc = resources[i];

                                rHTML += '<tr>';
                                rHTML += '<td><input id="cbResourceEmail" type="checkbox" resourceid="' + rsc.WTS_RESOURCEID + '" field="EmailDefault" original_value="' + rsc.EmailDefault + '" ' + (rsc.EmailDefault == '1' ? 'checked' : '') + ' origsort="' + i + '" />&nbsp;' + rsc.Resource.replace('\'', '&amp;').replace('<', '&lt;').replace('>', '&gt;') + '</td>';
                                rHTML += '</tr>';
                            }

                            rHTML += '</table>';

                            $('#divRecipientListContainer').html(rHTML);

                            $("[id=cbResourceEmail]").on('change', sortEmailResources);
                            sortEmailResources();
                            var maxRows = $('#divPage').height() / 22;
                            if (resources.length > (maxRows * .5)) {
                                $('#divRecipientListContainer').height($('#divPage').height() * .3);
                                $('#divRecipientListContainer').css('overflow-y', 'auto');
                            }  
                        }

                        return; // this type doesn't use the typecounts array from the other 3 items so it just returns here
                    }                    
                    
                    var container = $('#<%=this.cblDownloadPDFSettings.ClientID %>');

                    for (var key in typeCounts) {
                        if (key == 'AORs Included') {
                            var aorCount = typeCounts[key] != null ? parseInt(typeCounts[key]) : 0; // the aors will be used for both aors included AND the grid option below the burndown overview section

                            // aors included
                            var cb = container.find('input[type=checkbox][value=\'AORs Included\']');
                            var label = cb.next();
                            var countStr = aorCount > 0 ? ' (' + aorCount + ')' : '';
                            label.text('AORs Included' + countStr);
                            cb.prop('checked', countStr != '');

                            // burndown grid (aors included)
                            cb = container.find('input[type=checkbox][value=\'Burndown Grid\']');
                            label = cb.next();
                            countStr = aorCount > 0 ? ' (' + aorCount + ')' : '';
                            label.text('Grid' + countStr);
                            cb.prop('checked', countStr != '');

                            if (aorCount > 0) {
                                cb = container.find('input[type=checkbox][value=\'BurndownOverviewParent\']');
                                cb.prop('checked', true);
                            }

                        }
                        else if (key == 'Burndown Overview') {
                            var burndownNoteCount = typeCounts[key] != null ? parseInt(typeCounts[key]) : 0; // these are the normal burndown notes

                            // burndown notes (burndown notes)
                            cb = container.find('input[type=checkbox][value=\'Burndown Overview\']');
                            label = cb.next();
                            countStr = burndownNoteCount > 0 ? ' (' + burndownNoteCount + ')' : '';
                            label.text('Notes' + countStr);
                            cb.prop('checked', countStr != '');

                            if (burndownNoteCount > 0) {
                                cb = container.find('input[type=checkbox][value=\'BurndownOverviewParent\']');
                                cb.prop('checked', true);
                            }
                        }
                        else if (key != 'BurndownOverviewParent') {
                            var count = typeCounts[key];
                            var countStr = parseInt(count) > 0 ? ' (' + count + ')' : '';
                            
                            var cb = container.find('input[type=checkbox][value=\'' + key + '\']');
                            var label = cb.next();

                            label.text(key + countStr);
                            cb.prop('checked', key != 'Attachments' && countStr != '');
                        }
                    }
                }
            }
        }

        function sortEmailResources() {
            var sorted = true;

            var emailResources = $('[id=cbResourceEmail]');

            do {
                sorted = true;

                for (var i = 0; i < emailResources.length - 1; i++) {
                    var check1 = emailResources[i];
                    var check2 = emailResources[i + 1];

                    var email1Row = $(check1).closest('tr');
                    var email2Row = $(check2).closest('tr');

                    var email1Checked = $(check1).is(':checked');
                    var email2Checked = $(check2).is(':checked');

                    var email1OrigSort = parseInt($(check1).attr('origsort'));
                    var email2OrigSort = parseInt($(check2).attr('origsort'));

                    var swap = false;

                    if ((!email1Checked && email2Checked) ||
                        (email1Checked && email2Checked && email1OrigSort > email2OrigSort) ||
                        (!email1Checked && !email2Checked && email1OrigSort > email2OrigSort)) {
                        swap = true;
                        sorted = false;
                    }

                    if (swap) {
                        emailResources[i] = check2;
                        emailResources[i + 1] = check1;
                        $(email2Row).insertBefore(email1Row);
                    }
                }

            } while (!sorted);
        }

        function download(type) {
            var downloadSettings = '';
            var emailSettings = '';

            var mtgID = $('#ddlDlMiName').val();
            var insID = $('#ddlDlMiInstance').val();

            if (mtgID == 0 || insID == 0) {
                warningMessage('<b>Please select both a meeting and an instance.</b>');
                return;
            }

            switch (type) {
                case 'pdf':
                    var arrSelections = [];

                    $('#<%=this.cblDownloadPDFSettings.ClientID %> input:checked').each(function () {
                        arrSelections.push($(this).attr('value'));
                    });

                    var bopIdx = arrSelections.indexOf('BurndownOverviewParent');

                    if (bopIdx != -1) {
                        // this option just enables the grid/overview to be included; it's not needed any more so we remove it from the settings
                        arrSelections.splice(bopIdx, 1);    
                    }
                    else {
                        var bgIdx = arrSelections.indexOf('Burndown Grid');
                        if (bgIdx != -1) {
                            arrSelections.splice(bgIdx, 1);
                        }

                        var boIdx = arrSelections.indexOf('Burndown Overview');
                        if (boIdx != -1) {
                            arrSelections.splice(boIdx, 1);
                        }
                    }

                    downloadSettings = mtgID + ',' + insID + ',' + arrSelections.join(',');

                    break;
            }

            if (isEmailMode()) {
                var arrSelections = [];

                $('[id=cbResourceEmail]input:checked').each(function () {
                    arrSelections.push($(this).attr('resourceid'));
                });

                if (arrSelections.length == 0) {
                    dangerMessage('<b>You must select at least one e-mail recipient.</b>');
                    return false;
                }

                var customNote = $('#txtEmailCustomNote').val();

                emailSettings = (customNote != null && customNote.length > 0 ? escape(customNote) : '[EMPTY]') + ',' + arrSelections.join(',');

                var bm = new bubbleMessage($('.grid')[0], '<div style="background-color:#ffffff;border:1px solid #aaaaaa;padding:10px;">Sending email... <img src="images/loaders/progress_bar_blue.gif" style="position:relative;top:5px"></div>');
                bm.verticalAlign = 'center';
                bm.horizontalAlign = 'center';
                bm.show();
                PageMethods.EmailMinutes(downloadSettings, emailSettings, EmailMinutes_done, on_error);
            }
            else {
                $('#frmDownload').attr('src', 'AOR_Meeting_Grid.aspx' + window.location.search + '&Download=' + type + '&DownloadSettings=' + downloadSettings);
            }

            return true;
        }

        function EmailMinutes_done(result) {
            closeActiveBubbleMessagesFromPopup();

            var dt = jQuery.parseJSON(result);

            if (dt.success == "true") {
                successMessage('<b>E-Mail sent.</b>');
            }
            else {
                MessageBox('Unable to send e-mail. An error has occurred. Check the logs.');
            }
        }

        function btnShowMetrics_click() {
            $('#btnShowMetrics').prop('value', ($('#divMetrics').is(':visible') ? 'Show Metrics' : 'Hide Metrics'));
            $('#divMetrics').slideToggle(function () {
                resizeFrame();                
            });
        }

        function loadMetrics() {
            PageMethods.LoadMeetingMetrics(loadMetrics_done, on_error);
        }

        function loadMetrics_done(results) {
            var dt = $.parseJSON(results);

            var html = '';

            html += '<table>';
            html += '<tr>';

            // left side
            html += '<td valign="top">';
            html += '<table style="border-collapse: collapse;">';
            html += '  <tr class="gridHeader">';
            html += '    <th style="border-top: 1px solid grey; border-left: 1px solid grey;text-align:left;">Meeting Metrics</th>';
            html += '    <th style="border-top: 1px solid grey; text-align:right;">Values</th>';                
            html += '  </tr>';
            
            var arr = [];
            arr.push('Total Meetings=totalmeetings');
            arr.push('Average Length (minutes)=avglength');
            arr.push('Average Attendees=avgattendedcount');
            arr.push('Average Resources=avgresourcescount');
            arr.push('% Attended=avgattendedpct');
            arr.push('% Highest Attended=maxattendedpct');

            var tarr = [];
            tarr.push('Agenda/Objectives=<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>');
            tarr.push('Action Items=<%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>');
            tarr.push('Burndown Overview=<%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%>');
            tarr.push('Notes=<%=(int)WTS.Enums.NoteTypeEnum.Notes%>');
            tarr.push('Questions/Discussion Points=<%=(int)WTS.Enums.NoteTypeEnum.QuestionsDiscussionPoints%>');
            tarr.push('Stopping Conditions=<%=(int)WTS.Enums.NoteTypeEnum.StoppingConditions%>');

            for (var i = 0; i < arr.length; i ++) {
                var tokens = arr[i].split('=');
                html += '<tr class="gridBody gridFullBorder"><td style="text-align:left;">' + tokens[0] + '</td><td style="text-align:right;">' + dt[tokens[1]] + (tokens[0].indexOf('%') != -1 ? '%' : '') + '</td></tr>';
            }

            html += '</table>';
            html += '</td>';

            // right side
            html += '<td valign="top">';
            html += '<table style="border-collapse: collapse;">';
            html += '  <tr class="gridHeader">';
            html += '    <th style="border-top: 1px solid grey; border-left: 1px solid grey;text-align:left;">Agenda Metrics</th>';
            html += '    <th style="border-top: 1px solid grey; text-align:right;">Counts</th>';                
            html += '  </tr>';

            for (var i = 0; i < tarr.length; i ++) {
                var tokens = tarr[i].split('=');
                var type = tokens[0];
                var typeid = tokens[1];
                var counts = dt['notetype_' + typeid].split('_');                
                html += '<tr class="gridBody gridFullBorder"><td style="text-align:left;">' + type + '</td><td style="text-align:right;">' + counts[0] + '</td></tr>';
            }

            html += '</table>';
            html += '</td>'

            html += '</tr>';
            html += '</table>'
            
            $('#divMetrics').html(html);
        }

    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';
            _meetings = [];
            _meetingInstances = []; // for caching
            _meetingInstanceCounts = []; // for caching
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            if ('<%=this.CanEditAORMeeting %>'.toUpperCase() == 'TRUE') {
                if (parseInt(_currentLevel) == 1) {
                    $('#btnStartMeeting').show();
                }
                else {
                    $('#<%=menuRelatedItems.ClientID %>').hide();
                }
                //if (parseInt(_currentLevel) == 1) $('#btnAdd').show();

                if ($('.saveable').length > 0) {
                    $('#btnDelete').show();
                    $('#btnSave').show();
                }
            }

            if ('<%=this.CanViewAORMeeting %>'.toUpperCase() == 'TRUE' && '<%=this.grdData.Rows.Count %>' != '0' && '<%=this.DCC.Contains("Meeting #") %>'.toUpperCase() == 'TRUE') $('#btnEdit').show();
            if ('<%=this.CanViewAORMeeting %>'.toUpperCase() == 'TRUE') {
                //$('#imgDownloadPDF').show();
            }

            if ('<%=this.DCC.Contains("Meeting Instance #") %>'.toUpperCase() == 'TRUE' && '<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                $('#btnAddMI').show();

                if ('<%=this.grdData.Rows.Count %>' != '0') $('#btnDeleteMI').show();
            }

            var ddlMtg = $('#ddlDlMiName');
            var mtgs = $('[id*=hdnMeetings]').val();            
            if (mtgs != null && mtgs.length > 0) {
                _meetings = mtgs.split(';');

                _.each(_meetings, function (mtg) {
                    var mtgArr = mtg.split('=');
                    $(ddlMtg).append($('<option>', { value: mtgArr[1], text: unescape(mtgArr[0]).replace(/\+/g, ' ') }));
                });
            }

            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();

                resizeFrame();
                completeLoading();
            }
            else {
                $('#pageContentHeader')
                $('#spnView').show();
                //$('#tdSettings').show();

                resizeGrid();

                //if (parent.updateTab) parent.updateTab('Meetings', 0); //todo: get correct meeting count 
            }
        }

        function initEvents() {
            $('#<%=this.ddlView.ClientID %>').on('change', function () { ddlView_change(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnStartMeeting').click(function () { btnStartMeeting_click(); return false; });
            $('#btnUseExistingMeeting').click(function () { popupManager.GetPopupByName('wndStartMeeting').Opener.btnUseExistingMeeting_click(); return false; });
            $('#btnCreateNewMeeting').click(function () { popupManager.GetPopupByName('wndStartMeeting').Opener.btnCreateNewMeeting_click(); return false; });

            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnEdit').click(function () { btnEdit_click(); return false; });
            $('#btnAddMI').click(function () { btnAddMI_click(); return false; });
            $('#btnDeleteMI').click(function () { btnDeleteMI_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            $('#imgDownloadPDF').click(function () { imgDownloadPDF_click(); });
            $('#imgEmailPDF').click(function () { imgEmailPDF_click(); });
            $('#ddlDlMiName').on('change', function () { ddlDlMiName_change(); });
            $('#ddlDlMiInstance').on('change', function () { ddlDlMiInstance_change(); });
            $('#btnDownloadPDF').click(function () { if (download('pdf')) { showDownloadPDFSettings(isEmailMode()); } return false; });
            $('#btnSelectRecipients').click(function () { btnSelectRecipients_click(); })
            $('#btnSelectAllRecipients').click(function () { btnSelectAllRecipients_click(); });
            $('#btnClearAllRecipients').click(function () { btnClearAllRecipients_click(); });
            $('#btnResetRecipients').click(function () { btnResetRecipients_click(); });         
            $('#btnShowMetrics').click(function () { btnShowMetrics_click(); });

            $(document).click(function (e) {
                try {                    
                    var objID = $(e.target).prop('id');
                    if (objID == '') {
                        // for some reason, in IE, select option elements don't trigger an objID for their select, causing the popup to close when selected, so we use
                        // the approach below to force-get the select id
                        if ($(e.originalEvent.srcElement).parent().is('select')) {
                            objID = $(e.originalEvent.srcElement).parent().prop('id');
                        }
                    }
                    var objHtmlFor = $(e.target).prop('htmlFor');
                    var objFirstChildID = '';

                    if ($(e.target).children(':first').length > 0) {
                        objFirstChildID = $(e.target).children(':first').prop('id');
                    }

                    if (objHtmlFor == undefined) objHtmlFor = '';
                    if (objFirstChildID == undefined) objFirstChildID = '';

                    var excludedIDs = ['imgDownloadPDF', 'imgEmailPDF', 'btnSelectRecipients', 'btnSelectAllRecipients', 'btnResetRecipients', 'btnClearAllRecipients', 'divDownloadPDFSettings', 'cbResourceEmail', 'divDlMiContainer', 'divDlMiName', 'divDlMiInstance', 'ddlDlMiName', 'ddlDlMiInstance', , 'divEmailCustomNote', 'txtEmailCustomNote'];
                    if (excludedIDs.indexOf(objID) == -1 && objID.indexOf('cblDownloadPDFSettings') == -1 && objHtmlFor.indexOf('cblDownloadPDFSettings') == -1 && objFirstChildID.indexOf('cblDownloadPDFSettings') == -1 && objFirstChildID.indexOf('cbResourceEmail') == -1) {
                        $('#divDownloadPDFSettings').slideUp();
                    }
                }
                catch (e) {
                }
            });
        }

        $(document).ready(function () {
            if ('<%=this.IsConfigured.ToString().ToUpper() %>' == 'FALSE') {
                openSettings();
            }

            initVariables();
            initDisplay();
            initEvents();
            if (_currentLevel == 1) {
                loadMetrics();
            }
        });
    </script>
</asp:Content>