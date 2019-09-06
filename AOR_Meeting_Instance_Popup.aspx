﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Instance_Popup.aspx.cs" Inherits="AOR_Meeting_Instance_Popup" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Meeting Instance</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" href="Styles/multiple-select.css" />
    <link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span><%=this.Type == "Note Type" ? "Note Breakout" : this.Type %></span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <span id="spnInstanceDate" style="padding-right: 5px; display: none;">Instance Date:&nbsp;<asp:DropDownList ID="ddlInstanceDateQF" runat="server" Width="170px"></asp:DropDownList></span>
                <span id="spnNoteType" style="display: none;">Note Breakout:&nbsp;<asp:DropDownList ID="ddlNoteTypeQF" runat="server" Width="205px"></asp:DropDownList></span>
                <span id="spnTxtSearch" style="display: none">Search by Name: <asp:TextBox ID="txtSearch" runat="server" MaxLength="255"></asp:TextBox></span>
                <input type="button" id="btnAdd" value="Save And Close" disabled="disabled" tabindex="1000" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" tabindex="1000" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnAddAnother" value="Save And Add Another" disabled="disabled" tabindex="1001" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnClose" value="Close" tabindex="1002" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="false" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <div id="divNote" style="padding: 10px; display: none;">
        <input type="checkbox" id="chkSelectAllNoteTypes" /><label for="chkSelectAllNoteTypes"><b>Check/Uncheck All</b></label>
        <asp:CheckBoxList ID="cblNoteType" runat="server"></asp:CheckBoxList>
    </div>
    <div id="divNoteDetail" style="padding: 10px; display: none;">
        <asp:HiddenField runat="server" ID="hdnNoteExtData" Value="" />
        <table style="width: 100%;border-collapse:collapse;">
            <tr id="trNoteDetailTypeRow">
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 85px;white-space:nowrap;">
                    Note Breakout:
                </td>
                <td>
                    <asp:DropDownList ID="ddlNoteType" runat="server" Width="150px" Enabled="false" TabIndex="1" style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
            </tr>
            <tr id="trNoteDetailAORRow">
                <td>
                    &nbsp;
                </td>
                <td>
                    AOR Name:
                </td>
                <td>
                    <asp:DropDownList ID="ddlAORName" runat="server" Width="250px" Enabled="false" TabIndex="2" style="background-color: #F5F6CE;"></asp:DropDownList>
                    <img id="imgAOROptionSettings" src="Images/Icons/cog.png" alt="AOR Option Settings" title="AOR Option Settings" width="15" height="15" style="cursor: pointer; margin-left: 3px; display: none;" />
                    <div id="divAOROptionSettings" style="text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none;">
                        <label class="aoroptionsettingsinput"><input type="checkbox" id="chkAORsIncluded" checked="checked" />AORs Included</label><br />
                        <label class="aoroptionsettingsinput"><input type="checkbox" id="chkAllAORs" />All (grouped by System)</label>
                    </div>
                </td>
            </tr>
            <tr id="trTaskRow" style="display:none">
                <td>
                    &nbsp;
                </td>
                <td>
                    Primary Task:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtTaskID" MaxLength="10" style="width:100px" disabled="disabled" />
                    &nbsp;<img src="Images/Icons/Find.png" id="imgSelectTask" style="position:relative;top:5px;width:15px;height:15px;margin-left:3px;cursor:pointer;">
                    &nbsp;&nbsp;&nbsp;<div id="divTaskTitle" class="smalltext" style="width:385px;color:#666666;white-space:nowrap;overflow:hidden;display:inline-block;"><asp:Literal runat="server" ID="ltTaskTitle"></asp:Literal></div>
                </td>
            </tr>
            <tr id="trSubTaskRow" style="display:none">
                <td>
                    &nbsp;
                </td>
                <td>
                    Subtask:
                </td>
                <td>
                     <asp:TextBox runat="server" id="txtSubTaskID" Maxlength="10" style="width:100px;" disabled="disabled" />
                    &nbsp;<img src="Images/Icons/Find.png" id="imgSelectSubTask" style="position:relative;top:5px;width:15px;height:15px;margin-left:3px;cursor:pointer;">
                    &nbsp;&nbsp;&nbsp;<div id="divSubTaskTitle" class="smalltext" style="width:385px;color:#666666;white-space:nowrap;overflow:hidden;display:inline-block;"><asp:Literal runat="server" ID="ltSubTaskTitle"></asp:Literal></div>
                </td>
            </tr>
            <tr id="trNoteDetailTitleRow">
                <td>
                    &nbsp;
                </td>
                <td>
                    Title:
                </td>
                <td>
                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="150" Width="99%" ReadOnly="true" TabIndex="3" ForeColor="Gray"></asp:TextBox>
                </td>
            </tr>
            <tr id="trNoteDetailDetailRow">
                <td style="vertical-align: top;">
                    <span style="color: red;">*</span>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="txtNoteDetail" runat="server" TextMode="MultiLine" Rows="24" Width="99%" ReadOnly="true" ForeColor="Gray" TabIndex="10"></asp:TextBox>
                </td>
            </tr>
            <tr id="trNoteDetailObjectivesRow" style="display:none">
                <td style="width:5px;padding-top:5px;" valign="top">
                    <span style="color: red;">*</span>
                </td>
                <td valign="top" style="padding-top:5px;">Objectives:</td>
                <td valign="top" style="padding-top:5px;position:relative;">
                    <div id="divObjectives" style="height:375px;position:relative;overflow-y:auto;">

                    </div>
                    <div id="divObjectivesDetail" style="position:absolute;display:none;left:0px;top:5px;width:100%;height:375px;background-color:white;" activerowidx="" activekey="">
                        <div style="width:100%;white-space:nowrap;">Objective Title: <input type="text" id="txtObjectivesDetailTitle" style="width:647px" tabindex="2000"></div>
                        <div style="margin-top:5px;"><textarea id="txtObjectivesDetail" rows="20" cols="100" style="" tabindex="2001"></textarea></div>
                        <div style="margin-top:5px;"><input type="button" id="btnSaveDetail" value="Save Detail" tabindex="2002">&nbsp;<input type="button" id="btnCloseDetail" value="Close Detail" tabindex="2003"></div>
                    </div>
                    <div id="divObjectivesHelp" style="position:absolute;display:none;padding:3px;top:-100px;left:25%;margin:0 auto;background-color:white;border:1px solid darkgray;box-shadow: 3px 3px 3px 0px #888888">
                        <table cellspacing="0" cellpadding="0" width="100%" style="">
                            <tr><td colspan="2" style="font-weight:bold;text-decoration:underline;">KEYBOARD SHORTCUTS</td></tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">ENTER</td>
                                <td width="50%" align="left">ADD ROW</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+ENTER</td>
                                <td width="50%" align="left" style="white-space:nowrap;">SAVE DETAIL (detail dialog only)</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+PLUS</td>
                                <td width="50%" align="left">ADD ROW</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+MINUS</td>
                                <td width="50%" align="left">DELETE ROW</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+LEFT</td>
                                <td width="50%" align="left">DECREASE LEVEL</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+RIGHT</td>
                                <td width="50%" align="left">INCREASE LEVEL</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+SHIFT+LEFT</td>
                                <td width="50%" align="left">SET TO LEVEL 1</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+SHIFT+RIGHT</td>
                                <td width="50%" align="left">SET TO LEVEL 10</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+UP</td>
                                <td width="50%" align="left">MOVE ROW UP</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+DOWN</td>
                                <td width="50%" align="left">MOVE ROW DOWN</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">CTRL+SHIFT+UP</td>
                                <td width="50%" align="left">MOVE ROW TO TOP</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;padding-right:5px;">CTRL+SHIFT+DOWN</td>
                                <td width="50%" align="left">MOVE ROW TO BOTTOM</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">UP</td>
                                <td width="50%" align="left">MOVE UP</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">DOWN</td>
                                <td width="50%" align="left">MOVE DOWN</td>
                            </tr>
                            <tr><td colspan="2">&nbsp;</td></tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">SHIFT+UP</td>
                                <td width="50%" align="left">MOVE TO TOP</td>
                            </tr>
                            <tr>
                                <td width="50%" align="left" style="font-weight:bold;">SHIFT+DOWN</td>
                                <td width="50%" align="left">MOVE TO BOTTOM</td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="divUnlockMeeting" style="padding: 10px; display: none;">
        <table style="border-collapse: collapse; width: 100%;">
            <tr>
                <td colspan="2">
                    This is a past meeting and is currently locked. If you are sure you would like to make changes, please enter a reason and click Save.<br /><br />
                </td>
            </tr>
            <tr>
                <td style="width: 5px; vertical-align: top;">
                    <span style="color: red;">*</span>
                </td>
                <td>
                    <asp:TextBox ID="txtUnlockReason" runat="server" TextMode="MultiLine" Rows="9" Width="99%"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
    <script src="Scripts/cleditor/jquery.cleditor.js"></script>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedSystemsQF = '';
        var _selectedReleasesQF = '';
        var _objectives = [];
        var _seqTypes = 'x,1,01,A,a,i,I,B'.split(',');
        var _suffixTypes = 'x,.,),-,_,s,.S,)S,-S,-S,_S,sS'.split(','); // there are two forms of each separator: always on, or separator only (used only if there is another item after it)
        var _seqDecorators = '.,)';
        var _editorObjectiveDetailInitialized = false;
        var _objectiveLevelSpacing = 35;
        var _objectiveLevelPadding = 35;
        var _deletedObjectiveKeys = [];
        var _refreshMessage = '<< Click to apply Quick Filter(s)';
        var _taskAORs = ''; // if a user adds a new task, this will contain the list of AORs that task is associated with
        var _origAORs = []; // if we are in action items and we select a task that isn't already included in the meeting, we prompt them to see if they want to add it (this list contains the default AORs for the meeting since we can't rely on the AOR dropdown because the cog icon allows them to add other AOR's to the list)
    </script>

	<script id="jsEvents" type="text/javascript">
        function Objective(level, idx, seqtype, title, notes, createnote) {
            this.level = level;
            this.idx = idx;
            this.key = generateUniqueID(15);
            this.seqtype = seqtype;
            this.title = title;
            this.notes = notes;
            this.createnote = createnote;

            // these two properties are calculated on the fly by the engine
            this.seqidx = 0;
            this.seqtext = '';
            this.seqoff = false; // true if the seqtype contains |x| (this flag is used for efficiency so we don't have to do substring searches)
        }

	    function imgRefresh_click() {
	        refreshPage();
	    }

        function ddlSystemQF_update() {
            var arrSystemQF = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');

            _selectedSystemsQF = arrSystemQF.join(',');
        }

	    function ddlSystemQF_close() {
            //refreshPage();
        }

        function ddlReleaseQF_update() {
            var arrReleaseQF = $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect('getSelects');

            _selectedReleasesQF = arrReleaseQF.join(',');
        }

	    function ddlReleaseQF_close() {
            //refreshPage();
        }

	    function ddlInstanceDateQF_change() {
	        refreshPage();
	    }

	    function ddlNoteTypeQF_change() {
	        refreshPage();
	    }

        function btnAdd_click(addAnother, showTaskAddPrompt, addAORID) {
            if (showTaskAddPrompt) {
                if ($('#<%=this.ddlNoteType.ClientID %>').val() == '<%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>') {
                    if (!processAddAORFromTask(addAnother ? 'addanother' : 'add')) {
                        return false;
                    }
                }
            }

            if (addAORID == null) {
                addAORID = 0;
            }

	        try {
                var arrAdditions = [];
                var options = '';

	            switch ('<%=this.Type %>') {
	                case 'AOR':
	                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
	                        var $obj = $(this).parent();

	                        arrAdditions.push({ 'aorreleaseid': $obj.attr('aorrelease_id') });
	                    });
	                    break;
	                case 'Resource':
	                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
	                        var $obj = $(this).parent();

	                        arrAdditions.push({ 'resourceid': $obj.attr('resource_id') });
	                    });
	                    break;
	                case 'Note Type':
	                    $('#<%=this.cblNoteType.ClientID %>  input[type="checkbox"]:checked').each(function () {
	                        arrAdditions.push({ 'aornotetypeid': $(this).val() });
	                    });
	                    break;
	                case 'Note Detail':
	                    if ($('#<%=this.ddlNoteType.ClientID %> option:selected').text() != '' && $('.cleditorMain iframe').contents().text().length > 0) {
                            options = $('#<%=this.ddlAORName.ClientID %>').val() + '|' + $('#<%=this.ddlNoteType.ClientID %>').val() + '|' + $('#<%=this.txtTitle.ClientID %>').val().replace('|', '<pipe>');
                            var taskid = $('[id$=txtTaskID]').val().length > 0 ? $('[id$=txtTaskID]').val() : '0';
                            var subtaskid = $('[id$=txtSubTaskID]').attr('subtaskid').length > 0 ? $('[id$=txtSubTaskID]').attr('subtaskid') : '0'; // the txt val contains the "number", but we really want the WORKITEM_TASKID which is stored in the attribute
                            arrAdditions.push({ 'aormeetingnotesidparent': '<%=this.AORMeetingNotesID_Parent %>', 'aornotetypeid': $('#<%=this.ddlNoteType.ClientID %>').val(), 'aorreleaseid': $('#<%=this.ddlAORName.ClientID %>').val(), 'title': $('#<%=this.txtTitle.ClientID %>').val(), 'notedetail': $('#<%=this.txtNoteDetail.ClientID %>').val(), 'taskid' : taskid, 'subtaskid' : subtaskid, 'extdata' : ''  });
                        break;
	                    }
                    case 'Add Note Objectives':
                        if (_objectives.length == 0) {
                            MessageBox('Please enter at least one objective.');
                            return;
                        }

                        var allObjectivesHaveTitles = true;
                        for (var i = 0; i < _objectives.length; i++) {
                            if (_objectives[i].title == null || _objectives[i].title.trim() == '') {
                                allObjectivesHaveTitles = false;
                                break;
                            }
                        }

                        if (!allObjectivesHaveTitles) {
                            MessageBox('All objectives must have titles.');
                            return;
                        }

                        // the backslash character is causing problems with the JSON javascript, .NET, Newtonsoft, and XML parsing, so we are removing it
                        for (var i = 0; i < _objectives.length; i++) {
                            if (_objectives[i].title != null) _objectives[i].title = _objectives[i].title.replace('\\', ' ');
                            if (_objectives[i].notes != null) _objectives[i].notes = _objectives[i].notes.replace('\\', ' ');
                        }

                        arrAdditions.push({ 'aormeetingnotesidparent': '<%=this.AORMeetingNotesID_Parent %>', 'aornotetypeid': $('#<%=this.ddlNoteType.ClientID %>').val(), 'aorreleaseid': $('#<%=this.ddlAORName.ClientID %>').val(), 'title': $('#<%=this.txtTitle.ClientID %>').val(), 'objectives': { 'objective': _objectives } });

                        break;
	            }

	            if (arrAdditions.length > 0) {
	                ShowDimmer(true, 'Saving...', 1);

	                var nJSON = '{save:' + JSON.stringify(arrAdditions) + '}';

                    PageMethods.Add('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', '<%=this.Type %>', nJSON, options, addAORID, function (result) { add_done(result, addAnother); }, on_error);
	            }
	            else {
	                MessageBox('<%=this.Type %>' == 'Note Detail' ? 'Please enter a note.' : 'Please check at least one.');
	            }
	        }
	        catch (e) {
	            ShowDimmer(false);
	            MessageBox('An error has occurred.');
	        }
	    }

	    function add_done(result, addAnother) {
	        ShowDimmer(false);

	        var blnSaved = false;
	        var errorMsg = '';
	        var obj = $.parseJSON(result);

	        if (obj) {
	            if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
	            if (obj.error) errorMsg = obj.error;
	        }

	        if (blnSaved) {
	            switch ('<%=this.Type %>') {
	                case 'AOR':
	                    if (opener.getAORs) opener.getAORs();
	                    if (opener.getResources) opener.getResources();
	                    if (opener.getSRs) opener.getSRs();
	                    if (opener.getAORProgress) opener.getAORProgress();
	                    break;
	                case 'Resource':
	                    if (opener.getResources) opener.getResources();
	                    break;
	                case 'Note Type':
	                    if (opener.getNotes) opener.getNotes();
	                    break;
                    case 'Note Detail':
	                    if (opener.getNotes) opener.getNotes();
	                    if (opener.getAORs) opener.getAORs();
	                    if (opener.getResources) opener.getResources();
	                    if (opener.getSRs) opener.getSRs();
                        if (opener.getAORProgress) opener.getAORProgress();
                        if (opener.RefreshTreeView) opener.RefreshTreeView();
                        if (obj.newnoteid != '' && opener.getSelectedNoteDetail) opener.getSelectedNoteDetail(obj.newnoteid);
                        break;
                    case 'Add Note Objectives':
                        if (opener.RefreshTreeView) opener.RefreshTreeView();
                        if (obj.newnoteid != '' && opener.getSelectedNoteDetail) opener.getSelectedNoteDetail(obj.newnoteid);
                        break;
	            }

	            if ('<%=this.Type %>' == 'Note Detail' && addAnother) {
	                $('#<%=this.txtTitle.ClientID %>').val('');
	                $('#<%=this.txtNoteDetail.ClientID %>').cleditor()[0].clear();
	                $('#<%=this.txtTitle.ClientID %>').focus();
	                $('#btnAdd').prop('disabled', true);
	                $('#btnSave').prop('disabled', true);
	                $('#btnAddAnother').prop('disabled', true);
	                $('[id$=txtTaskID]').val('');
	                $('#divTaskTitle').html('');
	                $('[id$=txtSubTaskID]').attr('taskid', '');
	                $('[id$=txtSubTaskID]').attr('tasknumber', '');
	                $('[id$=txtSubTaskID]').attr('subtaskid', '');
	                $('[id$=txtSubTaskID]').val('');
                    $('#divSubTaskTitle').html('');
                    $('[id$=ddlAORName]').val('0');

	                syncNoteTypeFields();

	            }
	            else {
	                setTimeout(closeWindow, 1);
	            }
	        }
	        else {
	            MessageBox('Failed to save. <br>' + errorMsg);
	        }
	    }

	    function on_error() {
	        ShowDimmer(false);
	        MessageBox('An error has occurred.');
	    }

        function taskAddPromptConfirmed(answer, promptData) {
            var tokens = promptData.split('.');
            var mode = tokens[0];
            var addAORID = tokens[1];

            if (mode == 'save') {
                btnSave_click(false, answer.toLowerCase() == 'yes' ? addAORID : null);
            }
            else if (mode == 'add' || mode == 'addanother') {
                btnAdd_click(mode == 'addanother', false, answer.toLowerCase() == 'yes' ? addAORID : null);
            }
        }

        function processAddAORFromTask(mode) {
	        var taskid = $('[id$=txtTaskID]').val().length > 0 ? $('[id$=txtTaskID]').val() : '0';

	        if (taskid > 0 && _taskAORs != '') {
	            // that means we have a task id AND we have changed it from it's original value
	            // if we don't change it from its original value we don't need to re-prompt the user about adding the aor task since we already did that previously
	            var aors = $.parseJSON(_taskAORs);

	            var foundMatch = false;

	            // a task may be associated with multiple aors (maybe?), but we only want to prompt to add one
	            var lastUnmatchedAORID = 0;
	            var lastUnmatchedAORName = '';

	            for (var aorid in aors) {
	                var aorname = aors[aorid];

                    var aorMatchTokens = _.find(_origAORs, function (tokens) { return tokens[0] == aorid });

	                if (aorMatchTokens == null) {
	                    lastUnmatchedAORID = aorid;
	                    lastUnmatchedAORName = aorname;
	                }
	                else {
	                    foundMatch = true;
	                }
	            }

	            if (!foundMatch && lastUnmatchedAORID != 0) {
                    QuestionBox('Add AOR?', 'The task you have selected is associated with an AOR that is not assigned to this meeting. Do you want to add [u][b]AOR ' + lastUnmatchedAORID + ': ' + lastUnmatchedAORName + '[/b][/u] to this meeting when saving?', 'Yes,No', 'taskAddPromptConfirmed', 500, 300, this, mode + '.' + lastUnmatchedAORID);
                    return false;
	            }
            }

            return true;
        }

        function btnSave_click(showTaskAddPrompt, addAORID) {
            if (showTaskAddPrompt) {
                if ($('#<%=this.ddlNoteType.ClientID %>').val() == '<%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>') {
                    if (!processAddAORFromTask('save')) {
                        return false;
                    }
                }
            }

            if (addAORID == null) {
                addAORID = 0;
            }

            try {
                var arrChanges = [];

	            switch ('<%=this.Type %>') {
	                case 'Edit Note Detail':
	                    if ($('#<%=this.ddlNoteType.ClientID %> option:selected').text() != '' && $('.cleditorMain iframe').contents().text().length > 0) {
	                        var taskid = $('[id$=txtTaskID]').val().length > 0 ? $('[id$=txtTaskID]').val() : '0';
                            var subtaskid = $('[id$=txtSubTaskID]').attr('subtaskid').length > 0 ? $('[id$=txtSubTaskID]').attr('subtaskid') : '0'; // the txt val contains the "number", but we really want the WORKITEM_TASKID which is stored in the attribute
                            var extData = $('[id$=hdnNoteExtData]').val();
	                        arrChanges.push({ 'aormeetingnotesid': '<%=this.AORMeetingNotesID_Parent %>', 'aornotetypeid': $('#<%=this.ddlNoteType.ClientID %>').val(), 'aorreleaseid': $('#<%=this.ddlAORName.ClientID %>').val(), 'title': $('#<%=this.txtTitle.ClientID %>').val(), 'notedetail': $('#<%=this.txtNoteDetail.ClientID %>').val(), 'taskid' : taskid, 'subtaskid' : subtaskid, 'extdata' : extData });
	                    }
	                    break;
	                case 'Unlock Meeting':
	                    if ($('#<%=this.txtUnlockReason.ClientID %>').val().length > 0) {
                            arrChanges.push({ 'unlockreason': $('#<%=this.txtUnlockReason.ClientID %>').val() });
	                    }
                        break;
                    case 'Edit Note Objectives':
                        if (_objectives.length == 0) {
                            MessageBox('Please enter at least one objective.');
                            return;
                        }

                        var allObjectivesHaveTitles = true;
                        for (var i = 0; i < _objectives.length; i++) {
                            if (_objectives[i].title == null || _objectives[i].title.trim() == '') {
                                allObjectivesHaveTitles = false;
                                break;
                            }
                        }

                        if (!allObjectivesHaveTitles) {
                            MessageBox('All objectives must have titles.');
                            return;
                        }

                        // the backslash character is causing problems with the JSON javascript, .NET, Newtonsoft, and XML parsing, so we are removing it
                        for (var i = 0; i < _objectives.length; i++) {
                            if (_objectives[i].title != null) _objectives[i].title = _objectives[i].title.replace('\\', ' ');
                            if (_objectives[i].notes != null) _objectives[i].notes = _objectives[i].notes.replace('\\', ' ');
                        }

                        arrChanges.push({ 'aormeetingnotesid': '<%=this.AORMeetingNotesID_Parent %>', 'aornotetypeid': $('#<%=this.ddlNoteType.ClientID %>').val(), 'aorreleaseid': $('#<%=this.ddlAORName.ClientID %>').val(), 'title': $('#<%=this.txtTitle.ClientID %>').val(), 'objectives': { 'objective': _objectives }, 'deletedobjectives' : _deletedObjectiveKeys.join(',') });

                        break;
	            }

	            if (arrChanges.length > 0) {
	                ShowDimmer(true, 'Saving...', 1);

	                var nJSON = '{save:' + JSON.stringify(arrChanges) + '}';

	                PageMethods.Add('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', '<%=this.Type %>', nJSON, '', addAORID, save_done, on_error);
	            }
	            else {
                    MessageBox('<%=this.Type %>' == 'Unlock Meeting' ? 'Please enter a reason.' : 'Please enter a note.');
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
	            switch ('<%=this.Type %>') {
	                case 'Edit Note Detail':
	                    <%--if (opener.getNotesDetail) opener.getNotesDetail('<%=this.AORMeetingNotesID_Parent %>');--%>
	                    if (opener.getNotes) opener.getNotes();
	                    if (opener.getAORs) opener.getAORs();
	                    if (opener.getResources) opener.getResources();
	                    if (opener.getSRs) opener.getSRs();
                        if (opener.getAORProgress) opener.getAORProgress();
                        if (opener.getSelectedNoteDetail) opener.getSelectedNoteDetail(<%=AORMeetingNotesID_Parent%>, true);
                        if (opener.RefreshTreeView) opener.RefreshTreeView();
                        break;
	                case 'Unlock Meeting':
	                    if (opener.refreshPage) opener.refreshPage();
                        break;
                    case 'Edit Note Objectives':
                        if (opener.RefreshTreeView) opener.RefreshTreeView();
                        if (opener.getSelectedNoteDetail) opener.getSelectedNoteDetail(<%=AORMeetingNotesID_Parent%>);
                        break;
	            }

	            setTimeout(closeWindow, 1);
	        }
	        else {
	            MessageBox('Failed to save. <br>' + errorMsg);
	        }
        }

	    function btnClose_click() {
	        if ($('#btnAdd').is(':enabled') || $('#btnSave').is(':enabled')) {
	            QuestionBox('Confirm Close', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmClose', 300, 300, this);
	        }
	        else {
	            closeWindow();
	        }
	    }

	    function confirmClose(answer) {
	        switch ($.trim(answer).toUpperCase()) {
	            case 'SAVE':
	                switch ('<%=this.Type %>') {
	                    case 'Note Detail':
	                        $('#btnAdd').trigger('click');
	                        break;
	                    case 'Edit Note Detail':
	                        $('#btnSave').trigger('click');
	                        break;
	                }
	                break;
	            case 'DISCARD':
	                closeWindow();
	                break;
	            default:
	                return;
	        }
	    }

	    function imgAOROptionSettings_click() {
	        var $pos = $('#imgAOROptionSettings').position();

	        $('#divAOROptionSettings').css({
	            top: ($pos.top - 3),
	            left: ($pos.left + 23)
	        }).slideToggle();
	    }

        function getAOROptions() {
            PageMethods.GetAOROptions('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', ($('#chkAORsIncluded').is(':checked') ? 1 : 0), ($('#chkAllAORs').is(':checked') ? 1 : 0), getAOROptions_done, getAOROptions_error);
        }

        function getAOROptions_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            var $ddl = $('#<%=this.ddlAORName.ClientID %>');
            var $opt = $ddl.find('option:selected');
            var currentSystem = '';
            var exists = false;

            $ddl.empty();
            nHTML += '<option value="0"></option>';

            $.each(dt, function (rowIndex, row) {
                if ($('#chkAllAORs').is(':checked') && row.WTS_SYSTEM != undefined && row.WTS_SYSTEM != currentSystem) {
                    currentSystem = row.WTS_SYSTEM;
                    nHTML += '<option style="background-color: white;" disabled>' + currentSystem + '</option>';
                }

                var nText = '';

                switch (row.WorkloadAllocation.toUpperCase()) {
                    case 'RELEASE CAFDEX':
                        nText = '(R) ';
                        break;
                    case 'PRODUCTION SUPPORT':
                        nText = '(P) ';
                        break;
                    default:
                        nText = '(O) ';
                        break;
                }

                nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '" aorname="' + row.AORName + '">' + nText + row.AORID + ' - ' + row.AORName + '</option>';
            });

            $ddl.append(nHTML);

            $('option', $ddl).each(function () {
                if ($(this).val() == $opt.val()) {
                    exists = true;
                    return false;
                }
            });

            if (exists) {
                $ddl.val($opt.val());
            }
            else {
                $ddl.prepend($opt);
            }
        }

        function getAOROptions_error() {

        }

	    function openAOR(AORID) {
	        var nWindow = 'AOR';
	        var nTitle = 'AOR';
	        var nHeight = 700, nWidth = 1400;
	        var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&Source=MI';
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function openNoteDetail(AORMeetingNotesID) {
	        var nWindow = 'NoteDetail';
	        var nTitle = 'Note Detail';
	        var nHeight = 500, nWidth = 650;
	        var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&AORMeetingNotesID=' + AORMeetingNotesID;

	        nURL = editQueryStringValue(nURL, 'Type', 'View Note Detail');

	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function showText(txt) {
	        alert(decodeURIComponent(txt));
	    }

        function input_change(obj) {
            if (obj != null) {
                var $obj = $(obj);

                switch ('<%=this.Type %>') {
                    case 'AOR':
                    case 'Resource':
                        $obj.attr('fieldChanged', '1');
                        $obj.closest('tr').attr('rowChanged', '1');
                        break;
                }

                if ($obj.attr('id') && $obj.attr('id').indexOf('SelectAllNoteTypes') != -1) $('input[id*=cblNoteType]').prop('checked', $obj.is(':checked'));
            }

	        $('#btnAdd').prop('disabled', false);
	        $('#btnSave').prop('disabled', false);
	        $('#btnAddAnother').prop('disabled', false);
	    }

	    function txtBox_blur(obj) {
	        var $obj = $(obj);
	        var nVal = $obj.val();

	        $obj.val($.trim(nVal));
	    }

	    function refreshPage() {
            var nURL = window.location.href;

	        switch ('<%=this.Type %>') {
	            case 'AOR':
	                ddlSystemQF_update();
                    ddlReleaseQF_update();

	                nURL = editQueryStringValue(nURL, 'SelectedSystems', _selectedSystemsQF);
	                nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
                    nURL = editQueryStringValue(nURL, 'txtSearch', $('#<%=txtSearch.ClientID %>').val());
	                break;
	            case 'Historical Notes':
                    nURL = editQueryStringValue(nURL, 'InstanceFilterID', $('#<%=this.ddlInstanceDateQF.ClientID %>').val());
	                nURL = editQueryStringValue(nURL, 'NoteTypeFilterID', $('#<%=this.ddlNoteTypeQF.ClientID %>').val());
	                break;
	        }

	        window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function UpdateObjectivesTable() {
            var container = GetObjectivesContainer();

            var html = '';

            html += '<table style="border-collapse:collapse;border:1px solid gray">';
            html += '  <tr class="gridHeader">';
            html += '    <th style="text-align:center;border:1px solid gray;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">#</th>';
            html += '    <th style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">Move/Add</th>'; // movement
            html += '    <th style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">Sequence</th>'; // seq
            html += '    <th style="text-align:left;width:99%;white-space:nowrap;padding-left:3px;padding-right:3px;">Title</th>'; // title
            html += '    <th style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;"><img src="Images/Icons/page_white_edit.png" alt="Note Details" title="Note Details"></th>'; // note details
            html += '    <th style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;"><img src="Images/Icons/pin.png" alt="Create Note" title="Create Note"></th>'; // create note
            html += '  </tr>';

            var prefixArr = _suffixTypes;
            var suffixArr = _suffixTypes;
            var typeArr = _seqTypes;

            var lastObjective = null;

            for (var i = 0; i < _objectives.length; i++) {
                var objective = _objectives[i];

                var level = objective.level;
                var key = objective.key;
                var title = objective.title;
                if (title == null) title = '';
                var seqtypeArr = objective.seqtype.split('|');
                var createNote = objective.createnote;
                var seqtext = objective.seqtext;
                var notes = objective.notes;

                html += '<tr class="gridBody" rowidx="' + i + '" key="' + key + '">';
                html += '  <td style="">' + (i + 1) + '</td>';
                html += '  <td style="text-align:center;border:1px solid gray;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">';
                html += '    <img src="Images/Icons/arrow_left_blue.png" dir="left" rowidx="' + i + '" key="' + key + '" style="padding-right:3px;cursor:' + (level > 1 ? 'pointer' : 'default') + ';opacity:' + (level > 1 ? '1.0' : '0.5') + ';" alt="Move Left" title="Move Left" onclick="MoveObjective(this, \'left\'); return false;">';
                html += '    <img src="Images/Icons/arrow_right_blue.png" dir="right" style="position:relative;top:1px;padding-right:3px;cursor:' + (i > 0 && level < 10 ? 'pointer' : 'default') + ';opacity:' + (i > 0 && level < 10 ? '1.0' : '0.5') + ';" rowidx="' + i + '" key="' + key + '" alt="Move Right" title="Move Right" onclick="MoveObjective(this, \'right\'); return false;">';
                html += '    <img src="Images/Icons/arrow_up_blue.png" dir="up" rowidx="' + i + '" key="' + key + '" style="padding-right:0px;cursor:' + (i > 0 ? 'pointer' : 'default') + ';opacity:' + (i > 0 ? '1.0' : '0.5') + ';" alt="Move Up" title="Move Up" onclick="MoveObjective(this, \'up\'); return false;">';
                html += '    <img src="Images/Icons/arrow_down_blue.png" dir="down" rowidx="' + i + '" key="' + key + '" style="padding-right:3px;cursor:' + (i < (_objectives.length - 1) ? 'pointer' : 'default') + ';opacity:' + (i < (_objectives.length - 1) ? '1.0' : '0.5') + '"  alt="Move Left" title="Move Down"  onclick="MoveObjective(this, \'down\'); return false;">';
                html += '    <img src="Images/Icons/add_blue.png" rowidx="' + i + '" key="' + key + '" style="padding-right:1px;cursor:pointer;" onclick="AddObjectiveRow(this); return false;" alt="Add Row" title="Add Row">';
                html += '    <img src="Images/Icons/cross_blue.png" rowidx="' + i + '" key="' + key + '" style="padding-right:3px;cursor:pointer;" onclick="DeleteObjectiveRow(this); return false;" alt="Delete Row" title="Delete Row">';
                html += '  </td >';

                html += '  <td style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">';

                html += '    <select name="ddlSeqType" rowidx="' + i + '" key="' + key + '" tabindex="' + ((i*5) + 10) + '" style="font-weight:bolder;" onchange="SequenceAdjusted(this); SaveObjectiveRow(this); return false;" onkeydown="ObjectiveSelectKeyDown(event, this);" onkeyup="ObjectiveRowKeyUp(event, this); return false;">';
                for (var x = 0; x < typeArr.length; x++) {
                    var txt = typeArr[x];
                    if (txt == 'B') {
                        txt = '&#x25CF;';
                    }
                    else if (txt == 'x') {
                        txt = 'NONE';
                    }
                    else if (txt == 'i') {
                        txt = 'iii';
                    }
                    else if (txt == 'I') {
                        txt = 'III';
                    }

                    html += '<option value="' + typeArr[x] + '"' + (typeArr[x] == seqtypeArr[1] ? ' selected' : '') + '>' + txt + '</option>';
                }
                html += '    </select>';

                html += '    <select name="ddlSeqSuffixType" rowidx="' + i + '" key="' + key + '" tabindex="' + ((i*5) + 11) + '" style="font-weight:bolder;" onchange="SequenceAdjusted(this); SaveObjectiveRow(this); return false;" onkeydown="ObjectiveSelectKeyDown(event, this);" onkeyup="ObjectiveRowKeyUp(event, this); return false;">';
                for (var x = 0; x < suffixArr.length; x++) {
                    var txt = suffixArr[x];
                    if (txt == 'x') txt = 'NONE';
                    if (txt == 's') txt = ' \' \'';
                    if (txt.length == 2) {
                        if (txt == 'sS') {
                            txt = 'SO: \' \''
                        }
                        else {
                            txt = 'SO: ' + txt.substring(0, 1);
                        }
                    }
                    html += '<option value="' + suffixArr[x] + '"' + (suffixArr[x] == seqtypeArr[2] ? ' selected' : '') + ' style="background-color:' + (suffixArr[x].length == 2 ? 'gainsboro' : 'white') + ';">' + txt + '</option>';
                }
                html += '    </select>';

                html += '  </td>';
                html += '  <td style="text-align:left;width:99%;white-space:nowrap;padding-left:3px;padding-right:3px;position:relative;">';
                html += '    <input type="text" name="txtSeqTitle" rowidx="' + i + '" key="' + key + '" tabindex="' + ((i*5) + 12) + '" value="' + s.replaceAll(title, '\"', '&quot;') + '" style="width:' + (400 - ((level - 1) * _objectiveLevelSpacing)) + 'px;height:100%;border:0px;padding-left:' + (((level - 1) * _objectiveLevelSpacing) + _objectiveLevelPadding) + 'px" onblur="SaveObjectiveRow(this); return false;" onchange="SaveObjectiveRow(this); return false;"  onkeypress="return ObjectiveKeyDown(event, this);" onkeydown="return ObjectiveKeyDown(event, this);" onkeyup="var ret = ObjectiveRowKeyUp(event, this); if (ret) RefreshRowUI(' + i + '); else return false;">';
                html += '    <div name="divSeqLevel" rowidx="' + i + '" key="' + key + '" style="position:absolute;left:' + (((level - 1) * _objectiveLevelSpacing) + 3) + 'px;bottom:5px;font-size:8px;color:black;display:block;">' + seqtext + '</div>';
                html += '  </td>';

                html += '    <td style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">';
                html += '       <img src="Images/Icons/' + (notes != null && notes.trim() != '' ? 'pencil' : 'pencil_add') + '.png" name="imgEditObjectiveDetail" alt="Edit Note Details" rowidx="' + i + '" key="' + key + '" tabindex="' + ((i*5) + 13) + '" title="Edit Note Details" style="cursor:' + (title != null && title.trim() != '' ? 'pointer' : 'default') + ';opacity:' + (title != null && title.trim() != '' ? '1.0' : '0.5') + ';" onkeydown="return ObjectiveDetailImgKeyDown(event, this);" onclick="OpenObjectiveDetail(this); return false;">';
                html += '    </td > '; // edit note details

                html += '    <td style="text-align:center;width:1%;white-space:nowrap;padding-left:3px;padding-right:3px;">';
                html += '      <input type="checkbox" name="chkCreateNote" rowidx="' + i + '" key="' + key + '"' + (createNote ? 'checked' : '') + ' tabindex="' + ((i*5) + 14) + '" onchange="SaveObjectiveRow(this); return false;" onkeyup="ObjectiveRowKeyUp(event, this);">';
                html += '    </td > '; // create note
                html += '</tr>';

                lastObjective = objective;
            }

            html += '</table>';
            html += '<div style="width:100%;text-align:right;padding-top:3px;"><img src="Images/Icons/help.png" style="cursor:pointer;" onmouseover="ShowObjectivesHelp();" onmouseout="HideObjectivesHelp();"></div>';

            container.html(html);
        }

        function RefreshObjectiveFromJSON(objective) {
            // this is needed because the primitive variables turn into strings after coming back from json, so we need to re-convert them
            // additionally, because of the multiple levels of packaging our objects go through to get to the database, we also tokenize the title/note text and need to undo it for use in the wizard
            objective.level = parseInt(objective.level);
            objective.idx = parseInt(objective.idx);
            objective.seqidx = parseInt(objective.seqidx);
            objective.seqoff = objective.seqoff.toLowerCase() == 'true';
            objective.createnote = objective.createnote == 'true';

            objective.title = UndoStrongEscape(objective.title);
            objective.notes = UndoStrongEscape(objective.notes);
        }

        function ShowObjectivesHelp() {
            $('#divObjectivesHelp').show();
        }

        function HideObjectivesHelp() {
            $('#divObjectivesHelp').hide();
        }

        function GetObjectivesContainer() {
            return $('#divObjectives');
        }

        function SequenceAdjusted(elem) {
            var container = GetObjectivesContainer();

            var idx = $(elem).attr('rowidx');
            var key = $(elem).attr('key');

            var objective = GetObjectiveForKey(key);
            var level = objective.level;

            var numberingType = container.find('select[name=ddlSeqType][key=' + key + ']').val();
            var suffixType = container.find('select[name=ddlSeqSuffixType][key=' + key + ']').val();

            objective.seqtype = 'x|' + numberingType + '|' + suffixType;
            objective.seqoff = numberingType == 'x';

            if (numberingType != 'x') {
                for (var i = 0; i < _objectives.length; i++) {
                    var iterObjective = _objectives[i];
                    if (iterObjective.level == level && iterObjective.key != objective.key) {
                        if (iterObjective.seqtype.indexOf('|x|') == -1) {
                            iterObjective.seqtype = 'x|' + numberingType + '|' + suffixType;
                            iterObjective.seqoff = numberingType == 'x';
                        }
                    }
                }
            }

            RefreshObjectiveArray();
            UpdateObjectivesTable();
            input_change();

        }

        function SaveObjectiveRow(elem) {
            var container = GetObjectivesContainer();

            var idx = $(elem).attr('rowidx');
            var key = $(elem).attr('key');

            var objective = GetObjectiveForKey(key);

            if (objective == null) {
                return;
            }

            var numberingType = container.find('select[name=ddlSeqType][key=' + key + ']').val();
            var suffixType = container.find('select[name=ddlSeqSuffixType][key=' + key + ']').val();
            var title = container.find('input[name=txtSeqTitle][key=' + key + ']').val();
            var createNote = container.find('input[name=chkCreateNote][key=' + key + ']').is(':checked');

            objective.seqtype = 'x|' + numberingType + '|' + suffixType;
            objective.seqoff = numberingType == 'x';

            objective.title = title;
            objective.createnote = createNote;
            //objective.notes = 'todo...';
        }

        function AddObjectiveRow(elem) {
            var currentObjective = GetObjectiveForKey($(elem).attr('key'));

            var objective = null;

            if (currentObjective != null) {
                // create new objective, but copy some of the key details from the previous objective
                objective = new Objective(currentObjective.level, currentObjective.idx + 1, currentObjective.seqtype, '', '', false);
                objective.seqoff = currentObjective.seqoff;
            }

            _objectives.splice(objective.idx, 0, objective);

            RefreshObjectiveArray();
            UpdateObjectivesTable();
            input_change();

            return objective;
        }

        function DeleteObjectiveRow(elem) {
            var currentObjective = GetObjectiveForKey($(elem).attr('key'));
            var idx = $(elem).attr('rowidx');

            if (opener.isAgendaItemKeyLinkedToUnremovedNote(currentObjective.key)) {
                if (currentObjective.createnote) {
                    QuestionBox('Delete linked item?', 'If you delete this linked agenda item, you will Remove the associated Note. Continue?', 'Yes,No', 'DeleteObjectiveRowConfirmed', 500, 300, this, currentObjective.key + '.' + idx);
                }
                else {
                    QuestionBox('Delete linked item?', 'You are deleting a linked agenda item, but have unchecked the Link Note option. The associated Note will not be Removed. Continue?', 'Yes,No', 'DeleteObjectiveRowConfirmed', 500, 300, this, currentObjective.key + '.' + idx);
                }
            }
            else {
                DeleteObjectiveRowConfirmed("Yes", currentObjective.key + '.' + idx);
            }
        }

        function DeleteObjectiveRowConfirmed(answer, rowdata) {
            if (answer == "No") {
                return;
            }

            rowdata = rowdata.split('.');
            var key = rowdata[0];
            var idx = rowdata[1];

            var currentObjective = GetObjectiveForKey(key);

            if (currentObjective.createnote) {
                _deletedObjectiveKeys.push(key);
            }

            _objectives.splice(idx, 1);

            if (_objectives.length == 0) {
                _objectives.push(new Objective(1, 0, 'x|1|.', '', '', false));
            }

            RefreshObjectiveArray();
            UpdateObjectivesTable();

            if (idx >= _objectives.length) {
                FocusOnObjectiveRow(_objectives.length - 1);
            }
            else {
                FocusOnObjectiveRow(idx);
            }

            input_change();
        }

        function FocusOnObjectiveRow(idx) {
            var objective = _objectives[idx];
            var container = GetObjectivesContainer();
            var txt = container.find('input[name=txtSeqTitle][key=' + objective.key + ']');
            $(txt).focus();
        }

        function GetObjectiveForKey(key) {
            return _.findWhere(_objectives, { key: key });
        }

        function RefreshObjectiveArray() {
            var lastLevel = -1;
            var seqidx = -1;

            var lastSeqIdxAtLevel = [];
            var lastSeqTextAtLevel = [];

            for (var i = 0; i < _objectives.length; i++) {
                var objective = _objectives[i];

                objective.idx = i;

                var currentLevel = objective.level;
                var currentSeqType = objective.seqtype;

                if (objective.seqoff) {
                    objective.seqtext = '';
                    continue;
                }

                if (seqidx == -1) {
                    seqidx = 1;
                    objective.seqidx = 1;
                    objective.seqtext = GetLetterForSequence(currentSeqType, 1);
                    lastSeqIdxAtLevel[currentLevel] = 1;
                    lastSeqTextAtLevel[currentLevel] = objective.seqtext;
                }
                else {
                    if (currentLevel > lastLevel) {
                        // we are going to a child level
                        seqidx = 1;
                        objective.seqidx = 1;
                        lastSeqIdxAtLevel[currentLevel] = 1;

                        var lastParentLevel = -1;
                        var parentSeqType = null;

                        for (var x = i; x >= 0; x--) {
                            if (_objectives[x].level < currentLevel && !_objectives[x].seqoff) {
                                lastParentLevel = _objectives[x].level;
                                parentSeqType = _objectives[x].seqtype;
                                break;
                            }
                        }

                        objective.seqtext = lastSeqTextAtLevel[lastParentLevel] + GetSeparatorForSequence(parentSeqType, true) + GetLetterForSequence(currentSeqType, 1);
                        lastSeqTextAtLevel[currentLevel] = objective.seqtext;
                    }
                    else if (currentLevel == lastLevel) {
                        seqidx = parseInt(seqidx) + 1;
                        objective.seqidx = seqidx;
                        lastSeqIdxAtLevel[currentLevel] = seqidx;

                        var lastParentLevel = -1;
                        var parentSeqType = null;

                        for (var x = i; x >= 0; x--) {
                            if (_objectives[x].level < currentLevel && !_objectives[x].seqoff) {
                                lastParentLevel = _objectives[x].level;
                                parentSeqType = _objectives[x].seqtype;
                                break;
                            }
                        }

                        if (lastParentLevel > 0) {
                            objective.seqtext = lastSeqTextAtLevel[lastParentLevel] + GetSeparatorForSequence(parentSeqType, true) + GetLetterForSequence(currentSeqType, seqidx);
                        }
                        else {
                            objective.seqtext = GetLetterForSequence(currentSeqType, seqidx);
                        }

                        lastSeqTextAtLevel[currentLevel] = objective.seqtext;
                    }
                    else if (currentLevel < lastLevel) {

                        var lastAtLevel = lastSeqIdxAtLevel[currentLevel];
                        if (lastAtLevel == null) lastAtLevel = 0;
                        seqidx = parseInt(lastAtLevel) + 1;
                        objective.seqidx = seqidx;
                        lastSeqIdxAtLevel[currentLevel] = seqidx;

                        var lastParentLevel = -1;
                        var parentSeqType = null;

                        for (var x = i; x >= 0; x--) {
                            if (_objectives[x].level < currentLevel && !_objectives[x].seqoff) {
                                lastParentLevel = _objectives[x].level;
                                parentSeqType = _objectives[x].seqtype;
                                break;
                            }
                        }

                        if (lastParentLevel > 0) {
                            objective.seqtext = lastSeqTextAtLevel[lastParentLevel] + GetSeparatorForSequence(parentSeqType, true) + GetLetterForSequence(currentSeqType, seqidx);
                        }
                        else {
                            objective.seqtext = GetLetterForSequence(currentSeqType, seqidx);
                        }

                        lastSeqTextAtLevel[currentLevel] = objective.seqtext;
                    }
                }

                lastLevel = currentLevel;
            }
        }

        function GetSeparatorForSequence(seqtype, omitIfAlwaysOn) {
            var seqarr = seqtype.split('|');
            var septype = seqarr[2];

            if (omitIfAlwaysOn && septype.length == 1) {
                return '';
            }

            if (septype == 'x') return '';
            if (septype == 's') return ' ';

            if (septype.length == 2) {
                return septype.substring(0, 1);
            }
            else {
                return septype;
            }
        }

        function GetLetterForSequence(seqtypetokens, idx) { // this idx value is 1-based
            //var suffixArr = 'x,.,),-,_,s'.split(',');
            //var typeArr = '1,01,a,A,i,I,B'.split(',');

            var seqarr = seqtypetokens.split('|');
            var seqtype = seqarr[1];
            var septype = seqarr[2];

            var result = '';

            if (seqtype == 'a' || seqtype == 'A') {
                var letters = 'abcdefghijklmnopqrstuvwxyz';

                var letterIdx = (idx - 1) % 26;
                var rpt = Math.floor(idx / 26);
                var newLetter = letters.substring(letterIdx, letterIdx + 1);

                for (var i = 0; i < rpt; i++) newLetter += newLetter;

                result = seqtype == 'a' ? newLetter : newLetter.toUpperCase();
            }
            else if (seqtype == '1' || seqtype == '01') {
                if (seqtype == '01' && idx < 10) {
                    result = '0';
                }

                result += idx + '';
            }
            else if (seqtype == 'i' || seqtype == 'I') {
                result = seqtype == 'I' ? romanize(idx).toUpperCase() : romanize(idx).toLowerCase();
            }
            else if (seqtype == 'B') {
                result = '&#x25CF;';
            }

            if (septype.length == 1 && septype != 'x') {
                result += GetSeparatorForSequence(seqtypetokens);
            }

            return result;
        }

        function romanize(num) {
          var lookup = {M:1000,CM:900,D:500,CD:400,C:100,XC:90,L:50,XL:40,X:10,IX:9,V:5,IV:4,I:1},roman = '',i;
          for ( i in lookup ) {
            while ( num >= lookup[i] ) {
              roman += i;
              num -= lookup[i];
            }
          }
          return roman;
        }

        function MoveObjective(elem, dir) {
            SaveObjectiveRow(elem);

            var container = GetObjectivesContainer();

            var idx = parseInt($(elem).attr('rowidx'));
            var key = $(elem).attr('key');

            var objective = GetObjectiveForKey(key);

            if (objective == null) {
                return;
            }

            var changeMade = false;

            if (dir == 'down') {
                if (idx < (_objectives.length - 1)) {
                    var nextObjective = _objectives[objective.idx + 1];

                    _objectives[idx] = nextObjective;
                    _objectives[idx + 1] = objective;
                    changeMade = true;
                }
            }
            else if (dir == 'downbottom') {
                if (idx < (_objectives.length - 1)) {
                    _objectives.splice(idx, 1);
                    _objectives.push(objective);
                    changeMade = true;
                }
            }
            else if (dir == 'up') {
                if (idx > 0) {
                    var prevObjective = _objectives[objective.idx - 1];

                    _objectives[idx] = prevObjective;
                    _objectives[idx - 1] = objective;
                    changeMade = true;
                }
            }
            else if (dir == 'uptop') {
                if (idx > 0) {
                    _objectives.splice(idx, 1);
                    _objectives.unshift(objective);
                    changeMade = true;
                }
            }
            else if (dir == 'right') {
                if (idx > 0 && objective.level < 10) { // first row is always level 1
                    objective.level = objective.level + 1;

                    var existingSeqType = GetSequenceTypeForLevel(objective.level);
                    if (existingSeqType != null) {
                        objective.seqtype = existingSeqType;
                        objective.seqoff = existingSeqType.indexOf('|x|') != -1;
                    }

                    RefreshObjectiveArray(); // need to recalc level sequences
                    RefreshRowUI(); // needed because we aren't redrawing the grid
                    input_change();
                }
                return; // for level moves, we don't redraw the grid
            }
            else if (dir == 'rightall') {
                if (idx > 0 && objective.level < 10) {
                    objective.level = 10;

                    var existingSeqType = GetSequenceTypeForLevel(objective.level);
                    if (existingSeqType != null) {
                        objective.seqtype = existingSeqType;
                        objective.seqoff = existingSeqType.indexOf('|x|') != -1;
                    }

                    RefreshObjectiveArray(); // need to recalc level sequences
                    RefreshRowUI(); // needed because we aren't redrawing the grid
                    input_change();
                }
                return; // for level moves, we don't redraw the grid
            }
            else if (dir == 'left') {
                if (objective.level > 1) {
                    objective.level = objective.level - 1;

                    var existingSeqType = GetSequenceTypeForLevel(objective.level);
                    if (existingSeqType != null) {
                        objective.seqtype = existingSeqType;
                        objective.seqoff = existingSeqType.indexOf('|x|') != -1;
                    }

                    RefreshObjectiveArray(); // need to recalc level sequences
                    RefreshRowUI();  // needed because we aren't redrawing the grid
                    input_change();
                }
                return; // for level moves, we don't redraw the grid
            }
            else if (dir == 'leftall') {
                if (objective.level > 1) {
                    objective.level = 1;

                    var existingSeqType = GetSequenceTypeForLevel(objective.level);
                    if (existingSeqType != null) {
                        objective.seqtype = existingSeqType;
                        objective.seqoff = existingSeqType.indexOf('|x|') != -1;
                    }

                    RefreshObjectiveArray(); // need to recalc level sequences
                    RefreshRowUI();  // needed because we aren't redrawing the grid
                    input_change();
                }
                return; // for level moves, we don't redraw the grid
            }

            RefreshObjectiveArray();
            UpdateObjectivesTable();
            if (changeMade) input_change();
        }

        function GetSequenceTypeForLevel(level) {
            for (var i = 0; i < _objectives.length; i++) {
                if (_objectives[i].level == level) {
                    return _objectives[i].seqtype;
                }
            }

            return null;
        }

        function ResizeObjectiveTitleForLevel(objective) {
            var container = GetObjectivesContainer();
            var txt = container.find('input[name=txtSeqTitle][key=' + objective.key + ']');
            var level = objective.level;
            var padding = (level - 1) * _objectiveLevelSpacing;

            $(txt).css('padding-left', (padding + _objectiveLevelPadding) + 'px');
            $(txt).css('width', (400 - padding) + 'px');
        }

        function RefreshRowUI(objIdx) {
            var container = GetObjectivesContainer();

            for (var i = 0; i < _objectives.length; i++) {
                if (objIdx != null && i != objIdx) continue;

                var objective = _objectives[i];
                var key = objective.key;
                var level = objective.level;
                var idx = objective.idx;
                var seqtype = objective.seqtype;
                var title = objective.title;
                var notes = objective.notes;
                var seqtypearr = seqtype.split('|');

                var leftArrow = container.find('img[dir=left][rowidx=' + i + ']');
                leftArrow.css('opacity', level > 1 ? '1.0' : '0.5');
                leftArrow.css('cursor', level > 1 ? 'pointer' : 'default');

                var rightArrow = container.find('img[dir=right][rowidx=' + i + ']');
                rightArrow.css('opacity', idx > 0 && level < 10 ? '1.0' : '0.5');
                rightArrow.css('cursor', idx > 0 &&  level < 10 ? 'pointer' : 'default');

                var upArrow = container.find('img[dir=up][rowidx=' + i + ']');
                upArrow.css('opacity', idx > 0 ? '1.0' : '0.5');
                upArrow.css('cursor', idx > 0 ? 'pointer' : 'default');

                var downArrow = container.find('img[dir=down][rowidx=' + i + ']');
                downArrow.css('opacity', idx < (_objectives.length - 1) ? '1.0' : '0.5');
                downArrow.css('cursor', idx < (_objectives.length - 1) ? 'pointer' : 'default');

                var seqLevel = container.find('div[name=divSeqLevel][rowidx=' + i + ']');
                seqLevel.css('left', (((level - 1) * _objectiveLevelSpacing) + 3));
                seqLevel.html(objective.seqtext);

                var ddlNumberingType = container.find('select[name=ddlSeqType][rowidx=' + i + ']');
                $(ddlNumberingType).val(seqtypearr[1]);

                var ddlSuffixType = container.find('select[name=ddlSeqSuffixType][rowidx=' + i + ']');
                $(ddlSuffixType).val(seqtypearr[2]);

                var txt = container.find('input[name=txtSeqTitle][key=' + objective.key + ']');

                var currentTitle = txt.val().trim(); // we need to use this instead of title var because the objective may not have been updated yet, so we use the current txt field contents instead
                var detailImg = container.find('img[name=imgEditObjectiveDetail][rowidx=' + i + ']');
                detailImg.css('opacity', (currentTitle != null && currentTitle != '') ? '1.0' : '0.5');
                detailImg.css('cursor', (currentTitle != null && currentTitle != '') ? 'pointer' : 'default');
                detailImg.attr('src', notes != null && notes.trim() != '' ? 'Images/Icons/pencil.png' : 'Images/Icons/pencil_add.png');

                var level = objective.level;
                var padding = ((level - 1) * _objectiveLevelSpacing);

                $(txt).css('padding-left', (padding + _objectiveLevelPadding) + 'px');
                $(txt).css('width', (400 - padding) + 'px');
            }
        }

        function ObjectiveSelectKeyDown(evt, elem) {
            var key = evt.keyCode;

            if (key == 13) return false; // for the selects, we disable the default enter behavior
        }

        function ObjectiveKeyDown(evt, elem) {
            var key = evt.keyCode;

            if (evt.ctrlKey && (key == 37 || key == 39 || key == 38 || key == 40 || key == 107 || key == 109)) {
                return false; // we don't want ctrl+arrow keys to do any default behavior (they move cursors, typically); note that ctrl+arrow fires in keydown/keyup, but not keypress
            }
            else if (key == 38 || key == 40) {
                return false;
            }
            else {
                return true;
            }
        }

        function ObjectiveRowKeyUp(evt, elem) {
            var key = evt.keyCode;

            var isTxtBox = $(elem).attr('name') == 'txtSeqTitle';
            var start = 10000;
            if (isTxtBox) start = $(elem)[0].selectionStart;

            if (key == 13) {
                SaveObjectiveRow(elem);
                var newObjective = AddObjectiveRow(elem);
                var container = GetObjectivesContainer();

                container.find('input[name=txtSeqTitle][key=' + newObjective.key + ']').focus();
                return false;
            }
            else if (evt.ctrlKey) {
                if (key == 37) { // left
                    if (evt.shiftKey) {
                        MoveObjective(elem, 'leftall');
                    }
                    else {
                        MoveObjective(elem, 'left');
                    }
                    $(elem).focus();

                    if (isTxtBox) $(elem)[0].setSelectionRange(start, start);

                    return false;
                }
                else if (key == 39) { // right
                    if (evt.shiftKey) {
                        MoveObjective(elem, 'rightall');
                    }
                    else {
                        MoveObjective(elem, 'right');
                    }
                    $(elem).focus();

                    if (isTxtBox) $(elem)[0].setSelectionRange(start, start);

                    return false;
                }
                else if (key == 38) { // up
                    if (evt.shiftKey) {
                        MoveObjective(elem, 'uptop'); // this redraws the grid

                        container = GetObjectivesContainer();
                        var newElem = container.find('[name=' + $(elem).attr('name') + '][key=' + $(elem).attr('key') + ']');
                        $(newElem).focus();
                        if (isTxtBox) $(newElem)[0].setSelectionRange(start, start);
                    }
                    else {
                        MoveObjective(elem, 'up'); // this redraws the grid

                        container = GetObjectivesContainer();
                        var newElem = container.find('[name=' + $(elem).attr('name') + '][key=' + $(elem).attr('key') + ']');
                        $(newElem).focus();
                        if (isTxtBox) $(newElem)[0].setSelectionRange(start, start);
                    }

                    return false;
                }
                else if (key == 40) { // down
                    if (evt.shiftKey) {
                        MoveObjective(elem, 'downbottom');  // this redraws the grid

                        container = GetObjectivesContainer();
                        var newElem = container.find('[name=' + $(elem).attr('name') + '][key=' + $(elem).attr('key') + ']');
                        $(newElem).focus();
                        if (isTxtBox) $(newElem)[0].setSelectionRange(start, start);
                    }
                    else {
                        MoveObjective(elem, 'down');  // this redraws the grid

                        container = GetObjectivesContainer();
                        var newElem = container.find('[name=' + $(elem).attr('name') + '][key=' + $(elem).attr('key') + ']');
                        $(newElem).focus();
                        if (isTxtBox) $(newElem)[0].setSelectionRange(start, start);
                    }

                    return false;
                }
                else if (key == 107) { // plus
                    SaveObjectiveRow(elem);

                    var newObjective = AddObjectiveRow(elem);

                    var container = GetObjectivesContainer();
                    container.find('input[name=txtSeqTitle][key=' + newObjective.key + ']').focus();
                    return false;
                }
                else if (key == 109) { // minus
                    DeleteObjectiveRow(elem);
                    return false;
                }
            }
            else if (key == 38) {
                var idx = $(elem).attr('rowidx');

                if (idx > 0) {
                    var idxToGoTo = 0;

                    if (evt.shiftKey) {
                        idxToGoTo = 0;
                    }
                    else {
                        idxToGoTo = parseInt(idx) - 1;
                    }

                    var container = GetObjectivesContainer();
                    container.find('input[name=txtSeqTitle][key=' + _objectives[idxToGoTo].key + ']').focus();
                    container.find('input[name=txtSeqTitle][key=' + _objectives[idxToGoTo].key + ']')[0].setSelectionRange(10000, 10000);
                }

                return false;
            }
            else if (key == 40) {
                var idx = $(elem).attr('rowidx');

                if (idx < (_objectives.length - 1)) {
                    var idxToGoTo = 0;

                    if (evt.shiftKey) {
                        idxToGoTo = _objectives.length - 1;
                    }
                    else {
                        idxToGoTo = parseInt(idx) + 1;
                    }

                    var container = GetObjectivesContainer();

                    container.find('input[name=txtSeqTitle][key=' + _objectives[idxToGoTo].key + ']').focus();
                    container.find('input[name=txtSeqTitle][key=' + _objectives[idxToGoTo].key + ']')[0].setSelectionRange(10000, 10000);
                }

                return false;
            }

            return true;
        }

        function ObjectiveDetailImgKeyDown(evt, elem) {
            var key = evt.keyCode;

            if (key == 13 || key == 32) {
                OpenObjectiveDetail(elem);
                return false;
            }

            return true;
        }

        function OpenObjectiveDetail(elem) {
            var container = GetObjectivesContainer();

            var idx = parseInt($(elem).attr('rowidx'));
            var key = $(elem).attr('key');

            // does element have a title
            var title = container.find('input[name=txtSeqTitle][key=' + key + ']').val();
            if (title == null || title.trim() == '') {
                return;
            }

            var objective = _objectives[parseInt(idx)];
            $('#txtObjectivesDetailTitle').val(title);
            $('#txtObjectivesDetail').val(objective.notes);

            $('#divObjectivesDetail').attr('activerowidx', idx);
            $('#divObjectivesDetail').attr('activekey', key);
            $('#divObjectivesDetail').show();

            if (_editorObjectiveDetailInitialized) {

                $('#txtObjectivesDetail').cleditor()[0].refresh().focus();
                $('.cleditorMain iframe').contents().on('keydown', function (event) { return ObjectiveDetailKeyUp(event); });
            }
            else {
                $('#txtObjectivesDetail').cleditor({
                    height: 325,
                    width: 728,
                    controls:
                            'bold italic underline strikethrough subscript superscript | font size ' +
                            'style | color highlight removeformat | bullets numbering | outdent ' +
                            'indent | alignleft center alignright justify | undo redo | ' +
                            'rule image link unlink | cut copy paste pastetext | print',
                    bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px; color: black;'
                }).focus();

                $('.cleditorMain iframe').contents().on('keydown', function (event) { return ObjectiveDetailKeyUp(event); } );
            }

            _editorObjectiveDetailInitialized = true;
        }

        function ObjectiveDetailKeyUp(evt) {
            var key = evt.keyCode;

            if (evt.ctrlKey && key == 13) {
                SaveObjectiveDetail();
                return false;
            }

            input_change();

            return true;
        }

        function SaveObjectiveDetail() {
            var container = GetObjectivesContainer();

            var activeRowIdx = $('#divObjectivesDetail').attr('activerowidx');
            var activeKey = $('#divObjectivesDetail').attr('activekey');

            var objective = _objectives[parseInt(activeRowIdx)];

            var newTitle = $('#txtObjectivesDetailTitle').val();
            var newNotes = $('#txtObjectivesDetail').val();

            if (newTitle == null || newTitle.trim() == '') {
                dangerMessage('Title is required.');
                $('#txtObjectivesDetailTitle').focus();
                return;
            }

            objective.title = newTitle;
            var title = container.find('input[name=txtSeqTitle][key=' + activeKey + ']');
            title.val(newTitle);
            objective.notes = newNotes;

            CloseObjectiveDetail();
        }

        function CloseObjectiveDetail() {
            var container = GetObjectivesContainer();

            var activeRowIdx = $('#divObjectivesDetail').attr('activerowidx');
            var activeKey = $('#divObjectivesDetail').attr('activekey');
            var title = container.find('input[name=txtSeqTitle][key=' + activeKey + ']');
            var detailImg = container.find('img[name=imgEditObjectiveDetail][rowidx=' + i + ']');

            $('#divObjectivesDetail').attr('activerowidx', '');
            $('#divObjectivesDetail').attr('activekey', '');
            $('#txtObjectivesDetail').cleditor()[0].clear();
            $('#divObjectivesDetail').hide();

            setTimeout(function () { title.focus(); }, 250); // settimeout needed to prevent the enter key from firing on the title field as well (if user saves using the ctrl+enter) feature

            RefreshRowUI(activeRowIdx);
        }

        function toggleQuickFilters_click() {
	        var $imgShowQuickFilters = $('#imgShowQuickFilters');
	        var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 50;

            $('#trClearAll').hide();

            switch ('<%=this.Type%>') {
                case 'AOR':
                    $('#trms_Item0').show();
                    $('#trms_Item10').show();
                    break;
                default:
                    break;
            }

	        if ($imgShowQuickFilters.is(':visible')) {
                var pos = $imgShowQuickFilters.position();

	            $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

	            $divQuickFilters.css({
                    width: '300px',
	                position: 'absolute',
	                top: (pos.top + addtop) + 'px',
	                left: '23px'
	            }).slideDown();
	        }
	        else if ($imgHideQuickFilters.is(':visible')) {
                var pos = $imgHideQuickFilters.position();

	            $imgHideQuickFilters.hide();
	            $imgShowQuickFilters.show();

	            $divQuickFilters.css({
                    width: '300px',
	                position: 'absolute',
	                top: (pos.top + addtop) + 'px',
	                left: '23px'
	            }).slideUp();
            }
        }

	    function showQuickFilterRefreshMessage() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
            lblMessage.show();
            lblMessage.text(_refreshMessage);
	    }

	    function syncNoteTypeFields() {
	        var selectedType = $('[id$=ddlNoteType]').val();

	        var actionItemSelected = selectedType == <%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>;
	        if (actionItemSelected) {
	            $('#trTaskRow').show();
	            $('#trSubTaskRow').show();

	            syncTaskRows();

	            popupManager.ActivePopup.SetHeight(535);
	        }
	        else {
	            $('#trTaskRow').hide();
	            $('#trSubTaskRow').hide();

	            popupManager.ActivePopup.SetHeight(500);
	        }
	    }

	    function syncTaskRows() {
	        var taskID = $('[id$=txtTaskID]').val();

	        var editable = '<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE';

	        if (editable) {
	            $('#trTaskRow input').prop('disabled', false);
	            $('#trTaskRow input').css('color', '');
	            $('#trTaskRow input').css('background-color', '');
	            $('#trTaskRow img').css('cursor', 'pointer');
	        }
	        else {
	            $('#trTaskRow input').prop('disabled', true);
	            $('#trTaskRow input').css('color', '#999999');
	            $('#trTaskRow input').css('background-color', '#ffffe6');
	            $('#trTaskRow img').css('cursor', 'default');
	        }

	        if (taskID == null || taskID.trim() == '' || !editable) {
	            $('#trSubTaskRow input').prop('disabled', true);
	            $('#trSubTaskRow input').css('color', '#999999');
	            $('#trSubTaskRow input').css('background-color', '#ffffe6');
	            $('#trSubTaskRow img').css('cursor', 'default');
	        }
	        else {
	            $('#trSubTaskRow input').prop('disabled', false);
	            $('#trSubTaskRow input').css('color', '');
	            $('#trSubTaskRow input').css('background-color', '');
	            $('#trSubTaskRow img').css('cursor', 'pointer');
	        }
	    }

	    function selectTask() {
	        var editable = '<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE';
	        if (!editable) return;

	        var relID = $('[id$=ddlAORName]').val();
	        var aorID = 0;
	        var aorName = '';
	        if (relID > 0) {
	            var option = $('[id$=ddlAORName] option[value=' + relID + ']');
	            aorID = option.attr('aorid');
	            aorName = option.attr('aorname');
	        }

	        var nWindow = 'SelectTask';
	        var nTitle = 'Select Primary Task';
	        var nHeight = 700, nWidth = 1000;
	        var nURL = _pageUrls.Maintenance.AORPopup + '?GridType=AOR&MyData=false&NewAOR=false&AORID=0&AORReleaseID=0&Type=Task&SubType=SelectTask&SelectCallback=taskSelected&HideAdd=true&SelectedTasks=' + $('[id$=txtTaskID]').val();
	        if (aorID > 0) {
	            nURL += '&Filters=' + encodeURI('{"AOR":{"value":"' + aorID + '","text":"' + escape(aorName) + '"}}');
	        }
	        else {
	            var aorOptions = $('[id$=ddlAORName] option');
	            var aorIDList = '';
	            var aorNameList = '';
	            for (var i = 0; i < aorOptions.length; i++) {

	                if (aorIDList.length > 0) aorIDList += ',';
	                aorID = $(aorOptions[i]).attr('aorid');

	                if (aorID != null) {
	                    aorIDList += $(aorOptions[i]).attr('aorid');

	                    if (aorNameList.length > 0) aorNameList += ',';
	                    aorNameList += encodeURI($(aorOptions[i]).attr('aorname')); // we need to escape the special characters in the name or they break the json
	                }
	            }

	            nURL += '&Filters=' + encodeURI('{"AOR":{"value":"' + aorIDList + '","text":"' + escape(aorNameList) + '"}}');
	        }

	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function taskSelected(result) {
	        var taskID = result[0]['taskid'];

	        $('[id$=txtTaskID]').val(taskID);

	        taskID_changed(); // we call this function to load the title info since parent popups only pass id info
	    }

	    function taskID_changed() {
	        var newTaskID = $('[id$=txtTaskID]').val();

	        if (newTaskID == null || newTaskID == '') {
	            $('#divTaskTitle').html('');
	            $('[id$=txtSubTaskID]').attr('taskid', '');
	            $('[id$=txtSubTaskID]').attr('tasknumber', '');
	            $('[id$=txtSubTaskID]').attr('subtaskid', '');
	            $('[id$=txtSubTaskID]').val('');
	            $('#divSubTaskTitle').html('');
	            return;
	        }

	        PageMethods.ValidateTaskID(newTaskID, validateTaskID_done, function() { dangerMessage('Unable to validate Task ID');  });
	    }

	    function validateTaskID_done(result) {
	        var obj = $.parseJSON(result);

	        if (obj != null && obj.success == "true") {
	            if (obj.exists == "true") {
	                var title = obj.title;

	                if (title != null && title.length > 0) {
	                    if (title.length > 80) title = title.substring(0, 80) + '...';
	                    $('#divTaskTitle').html('(' + title + ')');
	                }
	                else {
	                    $('#divTaskTitle').html('');
	                }

	                if (obj.aors != null) {
                        _taskAORs = obj.aors;

                        // if we haven't selected an aor yet, and we select a task that belongs to an aor that is displayed in the aor ddl,
                        // auto-select the aor
                        var relID = $('[id$=ddlAORName]').val();
                        if (relID == 0) {
                            var aors = $.parseJSON(_taskAORs);

                            for (var aorid in aors) {
	                            var aoroption = $('[id$=ddlAORName] option[aorid=' + aorid + ']');

	                            if (aoroption.length > 0) {
                                    $(aoroption[aoroption.length - 1]).prop('selected', true);
                                    break;
	                            }
                            }
                        }
	                }

	                var taskID = $('[id$=txtTaskID]').val();
	                var taskIDOnSubTaskField = $('[id$=txtSubTaskID]').attr('taskid');

	                if (taskID != taskIDOnSubTaskField) {
	                    $('[id$=txtSubTaskID]').attr('taskid', '');
	                    $('[id$=txtSubTaskID]').attr('tasknumber', '');
	                    $('[id$=txtSubTaskID]').attr('subtaskid', '');
	                    $('[id$=txtSubTaskID]').val('');
	                    $('#divSubTaskTitle').html('');
	                }
	            }
	            else {
	                _taskAORs = '';
	                dangerMessage('Invalid Task ID');
	                $('[id$=txtTaskID]').val('');
	                $('#divTaskTitle').html('');
	                $('[id$=txtSubTaskID]').attr('taskid', '');
	                $('[id$=txtSubTaskID]').attr('tasknumber', '');
	                $('[id$=txtSubTaskID]').attr('subtaskid', '');
	                $('[id$=txtSubTaskID]').val('');
	                $('#divSubTaskTitle').html('');
	            }
	        }
	        else {
	            dangerMessage('Unable to validate Task ID');
	        }

	        syncTaskRows();
	    }

	    function selectSubTask() {
	        var taskID = $('[id$=txtTaskID]').val();

	        var editable = '<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE';
	        if (!editable) return;

	        if (taskID == null || taskID.trim() == '') {
	            dangerMessage('A Primary Task is required before you can select a Subtask.');
	            return;
	        }

	        var nWindow = 'SelectSubTask';
	        var nTitle = 'Select Subtask';
	        var nHeight = 700, nWidth = 1000;
	        var nURL = _pageUrls.Maintenance.AORPopup + '?GridType=AOR&MyData=false&NewAOR=false&AORID=0&AORReleaseID=0&Type=SubTask&SubType=SelectSubTask&SelectCallback=subTaskSelected&HideAdd=true&SelectedTasks=' + $('[id$=txtTaskID]').val() + '&SelectedSubTasks=' + $('[id$=txtSubTaskID]').attr('tasknumber') + '&TaskID=' + $('[id$=txtTaskID]').val();
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();

	    }

	    function subTaskSelected(result) {
	        var parentTaskID = $('[id$=txtTaskID]').val();

	        var taskNumber = result[0]['tasknumber']; // this actually contains subtaskid.number

	        var taskTokens = taskNumber.split('.');

	        $('[id$=txtSubTaskID]').attr('taskid', parentTaskID); // we store this off so we can tell if the parent task changed
	        $('[id$=txtSubTaskID]').attr('tasknumber', taskNumber);
	        $('[id$=txtSubTaskID]').attr('subtaskid', taskTokens[0]);
	        $('[id$=txtSubTaskID]').val(taskTokens[1]);

	        subTaskID_changed();
	    }

	    function subTaskID_changed() {
	        var parentTaskID = $('[id$=txtTaskID]').val();

	        if (parentTaskID == '' || parentTaskID == '0') {
	            dangerMessage('Task is required');
	            return;
	        }

	        var taskID = $('[id$=txtTaskID]').val();
	        var newSubTaskNumber = $('[id$=txtSubTaskID]').val(); // the id in the text box is actually the user-friendly number; we will validate this number against the parent task

	        PageMethods.ValidateSubTaskNumber(newSubTaskNumber, parentTaskID, validateSubTaskNumber_done, function() { dangerMessage('Unable to validate Subtask Number');  });
	    }

	    function validateSubTaskNumber_done(result) {
	        var obj = $.parseJSON(result);

	        if (obj != null && obj.success == "true") {
	            if (obj.exists == "true") {
	                var title = obj.title;
	                var taskID = obj.WORKITEMID;
	                var subTaskID = obj.WORKITEM_TASKID;
	                var number = obj.TASK_NUMBER;

	                $('[id$=txtSubTaskID]').attr('tasknumber', subTaskID + '.' + number);
	                $('[id$=txtSubTaskID]').attr('subtaskid', subTaskID);
	                //$('[id$=txtSubTaskID]').val(number); // no need to overwrite number that was entered manually

	                if (title != null && title.length > 0) {
	                    if (title.length > 80) title = title.substring(0, 80) + '...';
	                    $('#divSubTaskTitle').html('(' + title + ')');
	                }
	                else {
	                    $('#divSubTaskTitle').html('');
	                }
	            }
	            else {
	                dangerMessage('Invalid Subtask Number');
	                $('[id$=txtSubTaskID]').attr('tasknumber', '');
	                $('[id$=txtSubTaskID]').attr('subtaskid', '');
	                $('[id$=txtSubTaskID]').val('');
	                $('#divSubTaskTitle').html('');
	            }
	        }
	        else {
	            dangerMessage('Unable to validate Subtask Number');
	        }

	        syncTaskRows();
	    }

	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _taskAORs = '';
            _origAORs = [];

            var aorOptions = $('[id$=ddlAORName] option');
            for (var i = 0; i < aorOptions.length; i++) {
                var opt = aorOptions[i];

                var aorid = $(opt).attr('aorid');
                var aorname = $(opt).attr('aorname');

                if (aorid != null && aorid > 0) {
                    var tokens = [];
                    tokens.push(aorid);
                    tokens.push(aorname);
                    _origAORs.push(tokens);
                }
            }

            var extData = $('[id$=hdnNoteExtData]').val();

            if ('<%=this.Type %>' == 'Edit Note Objectives' && extData.length > 0) {
                var extData = $('[id$=hdnNoteExtData]').val();

                var idx1 = extData.indexOf('<objectivesjson>');
                var idx2 = extData.indexOf('</objectivesjson>');
                if (idx1 != -1 && idx2 != -1) {
                    var json = extData.substring(idx1 + '<objectivesjson>'.length, idx2);
                    _objectives = jQuery.parseJSON(json);
                    for (var i = 0; i < _objectives.length; i++) {
                        RefreshObjectiveFromJSON(_objectives[i]);
                    }
                }
            }

            var lengthBeforeFilter = _objectives.length;

            // delete all the objectives with [REMOVED] keys
            _objectives = _.filter(_objectives, function (obj) { return obj.key != 'REMOVED' });

            if (lengthBeforeFilter != _objectives.length) {
                input_change();
            }

            if (_objectives.length == 0) {
                _objectives.push(new Objective(1, 0, 'x|1|.', '', '', false));
            }

            RefreshObjectiveArray();
        }

        function initControls() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');

            var suites = $("#<%=Master.FindControl("ms_Item0").ClientID %> option").map(function () {
                return $(this).attr('OptionGroup');
            }).get();

            var uniqueSuites = _.unique(suites, true);
            $(uniqueSuites).each(function (i, v) {
                $("#<%=Master.FindControl("ms_Item0").ClientID %> option[OptionGroup='" + v + "']").wrapAll("<optgroup label='" + v + "'>");
            });

            $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onOpen: function () { ddlSystemQF_update(); },
				onClose: function () { ddlSystemQF_close(); },
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlSystemQF_update(); });

            $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onOpen: function() { ddlReleaseQF_update(); },
				onClose: function () { ddlReleaseQF_close(); },
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlReleaseQF_update(); });

            switch ('<%=this.Type %>') {
                case 'Note Detail':
                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        $('#<%=this.txtNoteDetail.ClientID %>').cleditor({
                            height: 340,
                            controls:
                                'bold italic underline strikethrough subscript superscript | font size ' +
                                'style | color highlight removeformat | bullets numbering | outdent ' +
                                'indent | alignleft center alignright justify | undo redo | ' +
                                'rule image link unlink | cut copy paste pastetext | print',
                            bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px;'
                        })[0].change(function () { input_change(this); });

                        $('#<%=this.txtNoteDetail.ClientID %>').cleditor().focus(); //required to avoid cleditor error
                    }
                    else {
                        $('#<%=this.txtNoteDetail.ClientID %>').cleditor({
                            height: 340,
                            controls:
                                'bold italic underline strikethrough subscript superscript | font size ' +
                                'style | color highlight removeformat | bullets numbering | outdent ' +
                                'indent | alignleft center alignright justify | undo redo | ' +
                                'rule image link unlink | cut copy paste pastetext | print',
                            bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px; color: gray;'
                        })[0].disable(true);
                    }
                    break;
                case 'Edit Note Detail':
                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && '<%=this.AllowSave %>'.toUpperCase() == 'TRUE') {
                        $('#<%=this.txtNoteDetail.ClientID %>').cleditor({
                            height: 340,
                            controls:
                                'bold italic underline strikethrough subscript superscript | font size ' +
                                'style | color highlight removeformat | bullets numbering | outdent ' +
                                'indent | alignleft center alignright justify | undo redo | ' +
                                'rule image link unlink | cut copy paste pastetext | print',
                            bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px;'
                        })[0].change(function () { input_change(this); });
                    }
                    else {
                        $('#<%=this.txtNoteDetail.ClientID %>').cleditor({
                            height: 340,
                            controls:
                                'bold italic underline strikethrough subscript superscript | font size ' +
                                'style | color highlight removeformat | bullets numbering | outdent ' +
                                'indent | alignleft center alignright justify | undo redo | ' +
                                'rule image link unlink | cut copy paste pastetext | print',
                            bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px; color: gray;'
                        })[0].disable(true);
                    }
                    break;
                case 'View Note Detail':
                    $('#<%=this.txtNoteDetail.ClientID %>').cleditor({
                        height: 340,
                        controls:
                                'bold italic underline strikethrough subscript superscript | font size ' +
                                'style | color highlight removeformat | bullets numbering | outdent ' +
                                'indent | alignleft center alignright justify | undo redo | ' +
                                'rule image link unlink | cut copy paste pastetext | print',
                        bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px; color: gray;'
                    })[0].disable(true);
                    break;
                case 'Add Note Objectives':
                case 'Edit Note Objectives':
                    // the editor for the objectives has to be initialized later because the textarea starts out hidden
                    // and the cleditor doesn't initialize correctly on hidden elements
                    break;
            }
        }

        function initDisplay() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
            lblMessage.hide();


            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#spnSystem').hide();
            $('#spnRelease').hide();
            $('#<%=this.grdData.ClientID %>').hide();

            switch ('<%=this.Type %>') {
                case 'AOR':
                case 'Resource':
                    if ('<%=this.Type %>' == 'AOR') {
                        $('#tdQuickFilters').show();
                        $('#spnTxtSearch').show();
                    }

                    $('#<%=this.grdData.ClientID %>').show();

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') $('#btnAdd').show();
                    break;
                case 'Note Type':
                    $('#divNote').show();

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') $('#btnAdd').show();
                    break;
                case 'Add Note Objectives':
                case 'Edit Note Objectives':
                    UpdateObjectivesTable();

                    $('input[type="text"], select, textarea').css('color', 'black');
                    $('input[type="text"], select, textarea').removeAttr('readonly');

                    if ('<%=this.Type %>' == 'Add Note Objectives') {
                        $('#btnAdd').show();
                    }
                    else {
                        $('#btnSave').show();
                    }
                    $('#btnClose').show();

                    $('#trNoteDetailAORRow').hide();
                    $('#trNoteDetailDetailRow').hide();
                    $('#trNoteDetailObjectivesRow').show();
                    $('#divNoteDetail').show();
                    $('#<%=txtTitle.ClientID%>').focus();

                    break;
                case 'Note Detail':
                    $('#divNoteDetail').show();
                    $('#btnClose').show();

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        $('#btnAdd').show();
                        $('#btnAddAnother').show();
                        $('select').removeAttr('disabled');
                        $('input[type="text"], textarea').css('color', 'black');
                        $('input[type="text"], textarea').removeAttr('readonly');
                        $('[id$=ddlNoteType] option[value=<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>]').prop('disabled', true);
                        $('#imgAOROptionSettings').show();

                        setTimeout(function () { $('#<%=this.ddlNoteType.ClientID %>').focus(); }, 500);
                    }

                    syncNoteTypeFields();
                    break;
                case 'Edit Note Detail':
                    $('#divNoteDetail').show();
                    $('#btnClose').show();

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && '<%=this.AllowSave %>'.toUpperCase() == 'TRUE') {
                        $('#btnSave').show();
                        $('select').removeAttr('disabled');
                        $('input[type="text"], textarea').css('color', 'black');
                        $('input[type="text"], textarea').removeAttr('readonly');
                        $('[id$=ddlNoteType] option[value=<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>]').prop('disabled', true);
                        $('#imgAOROptionSettings').show();
                    }

                    syncNoteTypeFields();
                    break;
                case 'Historical Notes':
                    $('#<%=this.grdData.ClientID %>').show();
                    $('#spnInstanceDate').show();
                    $('#spnNoteType').show();
                    break;
                case 'View Note Detail':
                    $('#divNoteDetail').show();
                    break;
                case 'Unlock Meeting':
                    $('#divUnlockMeeting').show();
                    $('#btnSave').show();
                    break;
            }
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#<%=this.ddlInstanceDateQF.ClientID %>').on('change', function () { ddlInstanceDateQF_change(); });
            $('#<%=this.ddlNoteTypeQF.ClientID %>').on('change', function () { ddlNoteTypeQF_change(); });
            $('#btnAdd').click(function () { btnAdd_click(false, true); return false; });
            $('#btnSave').click(function () { btnSave_click(true); return false; });
            $('#btnAddAnother').click(function () { btnAdd_click(true, true); return false; });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#imgAOROptionSettings').click(function () { imgAOROptionSettings_click(); });
            $('#chkAORsIncluded, #chkAllAORs').on('change', function () { getAOROptions(); });
            $('input[type="checkbox"], select').not($('#<%=this.ddlInstanceDateQF.ClientID %>')).not($('#<%=this.ddlNoteTypeQF.ClientID %>')).not($('#chkAORsIncluded')).not($('#chkAllAORs')).on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('#btnSaveDetail').on('click', function() { SaveObjectiveDetail(); } );
            $('#btnCloseDetail').on('click', function () { CloseObjectiveDetail(); });
            $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });
            $('[id$=ddlNoteType]').on('change', function () { syncNoteTypeFields(); } );
            $('#imgSelectTask').on('click', function () { selectTask(); });
            $('#imgSelectSubTask').on('click', function () { selectSubTask(); });
            $('[id$=txtTaskID]').on('change', function () { taskID_changed(); });
            $('[id$=txtTaskID]').on('keyup', function (event) {
                this.value = stripAlpha(this.value);
                syncTaskRows();
            });
            $('[id$=txtSubTaskID]').on('change', function () { subTaskID_changed(); });
            $('[id$=txtSubTaskID]').on('keyup', function (event) {
                this.value = stripAlpha(this.value);
            });

            $('#<%=txtSearch.ClientID %>').on('keyup', function (event) {
                showQuickFilterRefreshMessage();
                if (event.keyCode === 13) { refreshPage(); }
            });

            $(document).click(function (e) {
                try {
                    var objID = $(e.target).attr('id');
                    var objClass = $(e.target).attr('class');

                    if (objID != 'imgAOROptionSettings' && objID != 'divAOROptionSettings' && objID != 'chkAORsIncluded' && objID != 'chkAllAORs' && objClass != 'aoroptionsettingsinput') $('#divAOROptionSettings').slideUp();
                }
                catch (e) { }
            });
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>