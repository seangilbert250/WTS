﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Instance_Edit.aspx.cs" Inherits="AOR_Meeting_Instance_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>
<%@ Register Src="~/Controls/SimpleTreeViewControl.ascx" TagPrefix="wts" TagName="SimpleTreeView" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Meeting</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" href="Styles/jquery-ui-timepicker-addon.css" />
    <link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Meeting Instance" width="15" height="15" />
    <style>
        div {
            font-family: Arial;
	        font-size: 12px;
        }

        .existingmeetingsepouter {
            position:relative;
            display:table;
            width:100%;
            height:25px;
            float:left;
            font-weight:bold;
        }

        .existingmeetingsepinner {
            display:table-cell;
            vertical-align:bottom;
        }

        .existingmeetingdivouter {
            position:relative;
            display:table;
            text-align:center;
            background-color: #d9edf7;
            color: #31708f;
            border: 1px solid #559fe0;
            border-radius:5px;
            width: 300px;
            height: 80px;
            float:left;
            margin:5px;
            cursor:pointer;
        }

        .existingmeetingdivouteralt {
            background-color:#c9e1ed;
        }

        .existingmeetingdivouterselected {
            box-shadow:0px 0px 0px 2px #559fe0 inset;
        }

        .existingmeetingdivouterutility {
            background-color:#f2dede;
        }

        .existingmeetingdivinner {
            display:table-cell;
            vertical-align:middle;
            text-align:center;
        }

    </style>
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">
    <span id="spnMeetingInstanceEditHeader">Meeting</span>
    <img id="imgDownloadPDF" src="Images/Icons/pdf_16.png" alt="Download PDF" title="Download PDF" width="15" height="15" style="cursor: pointer; margin-left: 3px; display: none;" />
    <img id="imgEmailPDF" src="Images/Icons/email.png" alt="E-Mail PDF" title="E-Mail PDF" width="15" height="15" style="cursor: pointer; margin-left: 3px; position:relative; display:none;" />
    <div id="divDownloadPDFSettings" style="text-align: left; border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none; z-index: 100;">
        <div id="divDownloadItemsContainer" style="text-align: left;">
                <div id="divEmailCustomNote" style="display:none;border-bottom:1px solid #aaaaaa;padding-bottom:3px;"><b>Custom Note:</b> &nbsp; <input type="text" id="txtEmailCustomNote" style="width:200px" maxlength="200"></div>
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
</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
    <div style="padding-right:5px;">
        <img id="imgMtgNotAccepted" src="Images/Icons/check_gray.png" alt="This meeting has not been accepted" title="This meeting has not been accepted" width="15" height="15" style="vertical-align: middle; padding-right: 5px; opacity:.5; display: none;" />
        <img id="imgMtgAccepted" src="Images/Icons/check.png" alt="This meeting has been accepted" title="This meeting has been accepted" width="15" height="15" style="vertical-align: middle; padding-right: 5px; display: none;" />
        <img id="imgMtgEnded" src="Images/Icons/small-clock.png" alt="This meeting has ended" title="This meeting has ended" width="15" height="15" style="vertical-align: middle; padding-right: 5px; display: none;" />
        <img id="imgLocked" src="Images/Icons/lock.png" alt="This meeting is locked" title="This meeting is locked" width="15" height="15" style="cursor: pointer; vertical-align: middle; padding-right: 5px; display: none;" />
        <img id="imgLockMeeting" src="Images/Icons/lock_unlock.png" alt="Lock this meeting" title="Lock this meeting" width="15" height="15" style="cursor: pointer; vertical-align: middle; padding-right: 5px; display: none;" />
        <input type="button" id="btnBackToGrid" value="Back To Meeting Grid" style="vertical-align: middle; display: none;" />
	    <input type="button" id="btnEndMeeting" value="End Meeting" style="vertical-align: middle; display: none;" />
        <input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
        <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
        <input type="button" id="btnAccept" value="Accept" style="vertical-align: middle; display: none;" />
        <input type="button" id="btnStartMeeting" value="Start Meeting" style="vertical-align: middle; display: none;" />
        <input type="button" id="btnClose" value="Close" style="vertical-align: middle; display: none;" />
    </div>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div id="divPageContainer" style="overflow: hidden;">
        <div id="divAORMeetingInstance" style="padding: 10px;display:none;">
            <table style="width: 100%;">
                <tr style="display:none;">
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                    <td style="width: 165px;">
                        Meeting Instance #:
                    </td>
                    <td>
                        <span id="spnAORMeetingInstance" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span></div>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr id="trMeetingInstanceName" style="display:none;">
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width: 165px;">
                        Meeting Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtAORMeetingInstanceName" runat="server" MaxLength="150" Width="75%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width:5px;"> <!-- we are putting width info here because other rows might be hidden and the table doesn't align properly without it -->
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width:165px;">
                        Meeting Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtInstanceDate" runat="server" Width="150px" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                        <div id="divInstanceDateConflict" style="display:none;color:red;">&nbsp;* Invalid date. This date conflicts with another meeting.</div>
                    </td>
                    <td style="width:5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Notes:
                    </td>
                    <td>
                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="75%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr id="trActualLength" style="display:none;">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        Actual Length:
                    </td>
                    <td>
                        <asp:TextBox ID="txtActualLength" runat="server" MaxLength="4" Width="50px" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>&nbsp;Minutes
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div id="divSelectMeeting" style="width:100%; text-align:left;display:none;padding:10px;border-top:1px solid gray;">
            <div style="float:left">
                <b>Search for meeting:</b><br />
                <input type="text=" id="txtMeetingSearch" maxlength="100" style="width:400px;">
                <img src="images/icons/cross.png" style="cursor:pointer;position:relative;top:3px;" onclick="$('#txtMeetingSearch').val(''); refreshExistingMeetings(); $('#txtMeetingSearch').focus();">
            </div>
            <div style="float:left;white-space:nowrap;padding-left:25px;">
                <b>Sort By:</b><br />
                <input type="radio" name="rdExistingMeetingSort" value="name">Name
                <input type="radio" name="rdExistingMeetingSort" value="last">Last Meeting
                <input type="radio" name="rdExistingMeetingSort" value="week" checked>Week
            </div>
            <div style="float:left;white-space:nowrap;padding-left:25px;">
                <b>Direction:</b><br />
                <input type="radio" name="rdExistingMeetingSortDirection" value="asc">Ascending
                <input type="radio" name="rdExistingMeetingSortDirection" value="desc" checked>Descending
            </div>
            <div id="divExistingMeetings" style="width:95%;height:600px;overflow-y:auto;padding-top:10px;margin-top:40px;">
                    Loading existing meetings...
            </div>
        </div>
        <div id="divToggleDetails" style="width: 100%; text-align: center; display: none;">
            <span id="spnAORMeetingInstanceHeader" runat="server" style="font-weight: bold; display: none;"></span><br />
            <img id="imgToggleDetails" src="Images/Icons/arrow_up_blue.png" title="Hide Details" alt="Hide Details" width="15" height="15" style="cursor: pointer; padding: 5px;" />
            <div id="divPreviousMeetingAccepted" style="position:absolute;top:47px;right:10px;width:32px;height:32px;display:none;cursor:pointer;">
                <img src="images/icons/redalert_square.png" style="width:32px;height:32px;" alt="Action Required" title="Action Required">
            </div>
        </div>
        <div id="divSubSection" style="display: none;">
            <div id="divTabsContainer" class="mainPageContainer" style="padding: 0px;">
			    <ul>
                    <li><a href="#divAgendaTab">Agenda</a></li>
				    <li><a href="#divAORTab">AORs Included</a></li>
                    <li><a href="#divResourceTab">Resources Attending</a></li>
                    <li><a href="#divHistoryTab">History</a></li>
                    <li><a href="#divMetricsTab">Metrics</a></li>
                    <li><a href="#divAttachmentsTab">Attachments</a></li>
			    </ul>
                <div id="divAgendaTab" class="tabDiv" style="height: 424px; overflow: auto;">
                    <div style="height: 30px; text-align: right;position:relative;">
                        <div style="position: absolute; height:25px; top: 2px; right: 5px; margin: -6px; width: 85%; padding: 5px; background-color: white; white-space:nowrap;">
                            <input type="button" id="btnGoToNote" value="Go to Note #">
                            <input type="text" id="txtGoToNote" value="" style="width:60px" maxlength="8">
                            &nbsp;&nbsp;
                            Note Breakout:
                            <asp:DropDownList ID="ddlNoteType" runat="server" Width="200px"></asp:DropDownList>
                            <input type="checkbox" id="chkShowRemovedNote" /><label for="chkShowRemovedNote">Show Removed Note Breakouts</label>&nbsp;
                            <input type="button" id="btnViewHistoricalNotes" value="View Historical Notes" />
                            <input type="button" id="btnAddNote" value="Add Note Breakout" style="display: none;" />
                            <input type="button" id="btnAddNoteDetail" value="Add Note Detail" style="display: none;" />
                            <input type="button" id="btnAddNoteObjectives" value="Add Objectives" style="display: none;" />
                        </div>
                    </div>
                    <%--<table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td>
                                <div id="divAORMINotes">

                                </div>
                            </td>
                        </tr>
                    </table>--%>
                    <table style="width:100%;">
                        <tr>
                            <td style="width:200px; vertical-align:top;position:relative;top:-30px;">
                                <table width="200px" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td width="20%"><img id="imgDisplayAllNotes" src="Images/Icons/expand.gif" title="Show All" alt="Show All" height="9" width="9" style="cursor: pointer; float: left; padding-left: 3px; padding-top: 5px;" /></td>
                                        <td width="40%" align="right">
                                            <input name="rdNoteTreeViewType" type="radio" value="aor" checked>By AOR
                                        </td>
                                        <td width="40%" align="right">
                                            <input name="rdNoteTreeViewType" type="radio" value="type">By Type
                                        </td>
                                    </tr>
                                </table>

                                <div id="divNoteByAOR" style="float: left; width: 205px; height:524px; border-style: solid; padding: 5px 0px; margin-left: 3px; margin-top: 3px; border:0px; overflow: auto;">
                                    <wts:SimpleTreeView runat="server" ID="tvNoteByAOR" />
                                </div>
                                <div id="divNoteByType" style="float: left; width: 205px; height:504px; border-style: solid; padding: 5px 0px; margin-left: 3px; margin-top: 3px; border:0px; overflow: auto; display:none;">
                                    <wts:SimpleTreeView runat="server" ID="tvNoteByType" />
                                </div>
                            </td >
					        <td id="noteSpacer" style="cursor:pointer;position:relative;top:-30px; width: 4px; background: url(Images/page_spacer_vertical.gif); text-align: center; vertical-align: middle; border-bottom: 1px solid grey;" title="Expand/Collapse Note List Pane">
						        <img id="imgPageSpacerVertical1" src="Images/pageRoller.gif" alt="Expand/Collapse Note List Pane" title="Expand/Collapse Note List Pane" style="cursor: pointer;position:absolute;left:1px;top:33%;display:none;" />
                                <img id="imgPageSpacerVertical2" src="Images/pageRoller.gif" alt="Expand/Collapse Note List Pane" title="Expand/Collapse Note List Pane" style="cursor: pointer;position:absolute;left:1px;top:50%;" />
                                <img id="imgPageSpacerVertical3" src="Images/pageRoller.gif" alt="Expand/Collapse Note List Pane" title="Expand/Collapse Note List Pane" style="cursor: pointer;position:absolute;left:1px;top:66%;display:none;" />
					        </td>

                            <td style="vertical-align:top;">
                                <div id="divAORMINotesDetail">
                                </div>
                                <div id="divAORMINotesDetailHistory" style="position:absolute;display:none;overflow-y:auto;;height:400px;background-color:#ffffff;">
                                    Loading...
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divAORTab" class="tabDiv" style="height: 424px; overflow: auto;">
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td style="text-align: right;">
                                <input type="checkbox" id="chkShowRemovedAOR" /><label for="chkShowRemovedAOR">Show Removed AORs</label>&nbsp;
                                <input type="button" id="btnAddAOR" value="Add AOR" style="display: none;" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="divAORMIAOR"></div>
                            </td>
                        </tr>
                    </table>
                    <div id="divAORProgress" style="padding: 10px 0px;"></div>
                </div>
                <div id="divResourceTab" class="tabDiv" style="height: 424px; overflow: auto;">
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td style="text-align: right;">
                                <input type="checkbox" id="chkShowRemovedResource" /><label for="chkShowRemovedResource">Show Removed Resources</label>&nbsp;
                                <input type="button" id="btnAddResource" value="Add Resource" style="display: none;" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="divAORMIResources"></div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divHistoryTab" class="tabDiv" style="height: 424px; overflow: auto;">
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td>
                                <div id="divAORMIHistory"></div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divAttachmentsTab" class="tabDiv" style="height: 424px; overflow: auto;">
                    <iframe id="frameAttachments" name="frameAttachments" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Meeting Instance Attachments</iframe>
                </div>
                <div id="divMetricsTab" class="tabDiv" style="height: 424px; overflow: auto; position:relative;">
                    <div class="simplemessage hcenter vcenter info" id="divmetricsloading">Loading statistics...</div>
                    <div class="simplemessage hcenter vcenter danger" id="divmetricsfailed" style="display:none">Metrics could not be loaded.</div>
                    <div id="divmetricscontainer" style="padding:5px;opacity:.5">
                        <table style="border-collapse: collapse; width: 400px;">
                            <thead>
                                <tr class="gridHeader gridFullBorder">
                                    <th style="text-align:left;width:300px;">Meeting Metrics</th>
                                    <th style="text-align:right;width:100px;">Values</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Total Meetings</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotalMeetings">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Average Length (minutes)</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgLength">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Average Attendees</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgAttendedCount">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Average Resources</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgResourcesCount">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">% Attended</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgAttendedPct">0 %</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">% Highest Attended</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsMaxAttendedPct">0 %</div></td>
                                </tr>
                            </tbody>
                        </table>
                        <br>
                        <table style="border-collapse: collapse; width: 400px;">
                            <thead>
                                <tr class="gridHeader gridFullBorder">
                                    <th style="text-align:left;width:300px;">Agenda Metrics</th>
                                    <th style="text-align:right;width:100px;">Counts</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Agenda/Objectives</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Action Items</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Burndown Overview</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%>">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Notes</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.Notes%>">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Questions/Discussion Points</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.QuestionsDiscussionPoints%>">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left">Stopping Conditions</td>
                                    <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.StoppingConditions%>">0</div></td>
                                </tr>
                            </tbody>
                        </table>
                        <br>
                        <table style="border-collapse: collapse; width: 400px;">
                            <thead>
                                <tr class="gridHeader gridFullBorder">
                                    <th style="text-align:left;width:200px;">Action Item Metrics</th>
                                    <th style="text-align:right;width:50px;">New</th>
                                    <th style="text-align:right;width:50px;">All</th>
                                    <th style="text-align:right;width:50px;">Open</th>
                                    <th style="text-align:right;width:50px;">Closed</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left"><div id="divlastmeetingtitle">Last Meeting</div></td>
                                    <td style="text-align:right"><div id="divlastmeetingnew">0</div></td>
                                    <td style="text-align:right"><div id="divlastmeetingall">0</div></td>
                                    <td style="text-align:right"><div id="divlastmeetingopen">0</div></td>
                                    <td style="text-align:right"><div id="divlastmeetingclosed">0</div></td>
                                </tr>
                                <tr class="gridBody gridFullBorder">
                                    <td style="text-align:left"><div id="divsecondtolastmeetingtitle">Previous Last Meeting</div></td>
                                    <td style="text-align:right"><div id="divsecondtolastmeetingnew">0</div></td>
                                    <td style="text-align:right"><div id="divsecondtolastmeetingall">0</div></td>
                                    <td style="text-align:right"><div id="divsecondtolastmeetingopen">0</div></td>
                                    <td style="text-align:right"><div id="divsecondtolastmeetingclosed">0</div></td>
                                </tr>
                            </tbody>
                        </table>
                        <br><br>
                        <b>NOTES:</b><br>
                        <ol>
                            <li>Total Meetings includes all meetings of the same parent meeting type</li>
                            <li>Average Length only includes meetings with a length value entered</li>
                            <li>Attendance / Resources totals and averages only count meetings with 2 or more attendees</li>
                            <li>Agenda Metrics only includes notes, action items, and agenda items created on or after 2/12/2018</li>
                        </ol>
                    </div>
                </div>
		    </div>
        </div>
    </div>

    <iframe id="frmDownload" style="display: none;"></iframe>



    <script src="Scripts/jquery-ui-timepicker-addon.js"></script>
    <script src="Scripts/cleditor/jquery.cleditor.js"></script>

    <script id="jsVariables" type="text/javascript">
        var _newMeetingInstance;
        var _pageUrls;
        var _oldNoteTypeQF;
        var _optNote, _optResource;
        var _refreshEditor = false;
        var _sidebarVisible;
        var _resizeTimeout;
        var _meetingHasEnded = <%=MeetingEnded.ToString().ToLower()%>;
        var _meetingIsAccepted = <%=MeetingAccepted.ToString().ToLower()%>;
        var _meetingIsLocked = <%=Locked.ToString().ToLower()%>;
        var _previousMeetingAsBeenAccepted = <%=PreviousMeetingAccepted.ToString().ToLower()%>;
        var _previousMeetingInstanceID = <%=PreviousMeetingInstanceID%>;
        var _previousMeetingDate = '<%=(PreviousMeetingDate != DateTime.MinValue ? PreviousMeetingDate.ToString("MM/dd/yyyy") : "")%>';
        var _allNoteToggleCollapse = false;
        var _noteListPaneExpanded = false;
        var _defaultNoteListPaneCollapsedWidth = 205;
        var _defaultNoteListPaneExpandedWidth = 550;
        var _treeViewRefreshing = false;
        var _displayingRemovedNotes = false;
        var _lastNotesDetailHistoryGroupID = -1;
        var _mtgJSON = null;
        var _saveInProgress = false;
        var _openNoteTriggered = 0; // when a user clicks on an open note link while a save is in progress, we store the note's id here and open the note when the save is done
        var _parsedExistingMeetings;
        var _darr = null;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            if ($('#btnSave').is(':enabled')) {
                QuestionBox('Confirm Refresh', 'You have unsaved changes. Are you sure you would like to refresh the page?', 'Yes,No', 'confirmRefresh', 300, 300, this);
            }
            else {
                refreshPage();
            }
        }

        function imgLocked_click() {
            var nWindow = 'UnlockMeeting';
            var nTitle = 'Unlock Meeting';
            var nHeight = 250, nWidth = 750;
            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Unlock Meeting';

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function imgLockMeeting_click() {
            QuestionBox('Confirm lock', 'Lock this meeting?', 'Yes,No', 'imgLockMeeting_confirmed', 300, 300, this);
        }

        function imgLockMeeting_confirmed(answer) {
            if (answer == 'Yes') {
                PageMethods.LockMeeting(<%=this.AORMeetingInstanceID%>, imgLockMeeting_done, on_error);
            }
        }

        function imgLockMeeting_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success) {
                $('#imgLocked').show();
                $('#imgLockMeeting').hide();
            }
        }

        function imgDownloadPDF_click() {
            showDownloadPDFSettings();
        }

        function imgEmailPDF_click() {
            showDownloadPDFSettings(true);
        }

        function getNoteTreeViewClientID(other) {
            var rdType = $('input[name=rdNoteTreeViewType]:checked').val();

            if (other) {
                if (rdType == 'type') {
                    return '<%=tvNoteByAOR.ClientID%>';
                }
                else {
                    return '<%=tvNoteByType.ClientID%>';
                }
            }
            else {
                if (rdType == 'aor') {
                    return '<%=tvNoteByAOR.ClientID%>';
                }
                else {
                    return '<%=tvNoteByType.ClientID%>';
                }
            }
        }

        function noteTreeViewTypeClicked() {
            var rdType = $('input[name=rdNoteTreeViewType]:checked').val();

            if (rdType == 'aor') {
                $('#divNoteByAOR').show();
                $('#divNoteByType').hide();
            }
            else {
                $('#divNoteByType').show();
                $('#divNoteByAOR').hide();
            }
        }

        function noteNodeClicked(level, idx, key, node) {
            if (level == 1) {
                if (key == 'ai' || key == 'ao') return; // agenda/action items nodes don't have burndowns
                if (key == '00') key = '-1'; // no associated aor is looked up by key -1

                if ($('#btnSave').is(':enabled') && false) {
                    var args = '0,' + key + ',' + <%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%> + ',' + (key != -1);
                    QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost. Would you like to proceed?', 'Yes,No', 'getNotesDetailConfirmed', 300, 300, this, args);
                }
                else {
                    getNotesDetail(0, key, <%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%>, key != '-1');
                }
            }
            else if (level == 2) {
                if ($('#btnSave').is(':enabled') && false) {
                    QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost. Would you like to proceed?', 'Yes,No', 'getSelectedNoteDetailConfirmed', 300, 300, this, key);
                }
                else {
                    getSelectedNoteDetail(key);
                }
            }
        }

        function getNotesDetailConfirmed(answer, args) {
            if (answer.toLowerCase() == "yes") {
                args = args.split(',');

                getNotesDetail(args[0], args[1], args[2], args[3] == "true");
            }
        }

        function getSelectedNoteDetailConfirmed(answer, args) {
            if (answer.toLowerCase() == "yes") {
                args = args.split(',');

                getSelectedNoteDetail(args[0]);
            }
        }

        function isAgendaItemKeyLinkedToUnremovedNote(key) {
            var node = <%=tvNoteByAOR.ClientID%>_GetNodeByAttribute('agendaitemkey', key);

            return node.length > 0;
        }

        function btnGoToNote_click(noteIDOverride) {
            var noteID = noteIDOverride != null ? noteIDOverride : $('#txtGoToNote').val();

            if (isNaN(noteID) || noteID.length == 0) {
                dangerMessage('Enter a Note ID');
                return;
            }

            getSelectedNoteDetail(noteID);
            openNodeAndParent(noteID);
        }

        function openNodeAndParent(noteID) {
            // make sure note node is open
            var noteNode = window[getNoteTreeViewClientID() + '_GetNodeByKey'](noteID, 2);
            var idx = $(noteNode).attr('idx');
            var key = $(noteNode).attr('key');

            window[getNoteTreeViewClientID() + '_OpenNodeAndAllParents'](2, idx, key);
            window[getNoteTreeViewClientID() + '_SetNodeColor'](2, idx, key, '', '#d7e8fc', true);

            // open second tree
            var noteNode = window[getNoteTreeViewClientID(true) + '_GetNodeByKey'](noteID, 2);
            idx = $(noteNode).attr('idx');
            key = $(noteNode).attr('key');

            window[getNoteTreeViewClientID(true) + '_OpenNodeAndAllParents'](2, idx, key);
            window[getNoteTreeViewClientID(true) + '_SetNodeColor'](2, idx, key, '', '#d7e8fc', true);
        }

        function ToggleNoteTreeViewNodes() {
            _allNoteToggleCollapse = !_allNoteToggleCollapse;

            $('#imgDisplayAllNotes').attr('src', 'Images/Icons/' + (_allNoteToggleCollapse ? 'collapse.gif' : 'expand.gif'));

            window[getNoteTreeViewClientID() + '_ToggleAllNodes'](_allNoteToggleCollapse);
            window[getNoteTreeViewClientID(true) + '_ToggleAllNodes'](_allNoteToggleCollapse);
        }

        function RefreshTreeView() {
            _treeViewRefreshing = true;
            PageMethods.RefreshTreeView('<%=tvNoteByAOR.ClientID%>', '<%=tvNoteByType.ClientID%>', <%=this.AORMeetingID%>, <%=this.AORMeetingInstanceID%>, $('#chkShowRemovedNote').is(':checked'), RefreshTreeView_done, on_error);
        }

        function RefreshTreeView_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null) {
                var visibleNodes = getVisibleNoteNodes();

                <%=tvNoteByAOR.ClientID%>_SetTreeViewHTML(dt.aorhtml);
                <%=tvNoteByType.ClientID%>_SetTreeViewHTML(dt.typehtml);

                reopenPreviouslyVisibleNoteNodes(visibleNodes);
            }

            _treeViewRefreshing = false;

            // just in case the current edit note had an aor change to a previously not-visible aor node, we make sure that node now opens as well
            var nd = $('#divAORMINotesDetail');
            var tbl = nd.find('table');

            // it's possible the note detail table hasn't finished loading from server side; so if it hasn't, we skip it and let the load detail function open the parent node instead
            // we could always let the load detail function open the parent note when it's done EXCEPT for the fact that sometimes the tree view nodes haven't finished refreshing when
            // the detail function is done; so both have to try to open parent of visible note when done
            if (tbl != null && tbl.length > 0) {
                var tr = nd.find('tr[aormeetingnotesid]');

                if (tr.length > 0) {
                    var noteid = tr.attr('aormeetingnotesid');
                    openNodeAndParent(noteid);
                }
            }

            var agendaObjectivesPresent = <%=tvNoteByAOR.ClientID%>_NodeHasChildren(null, 'ao', 1);
            if (agendaObjectivesPresent) {
                $('#btnAddNoteObjectives').val('Edit Objectives');
            }
            else {
                $('#btnAddNoteObjectives').val('Add Objectives');
            }
        }

        function getVisibleNoteNodes() {
            var visibleNoteNodes = window[getNoteTreeViewClientID() + '_GetAllVisibleNodes'](false, false, true);

            visibleNoteNodes += ',' + (_allNoteToggleCollapse ? 'atc' : 'ate'); // all toggle collapse, all toggle expand
            visibleNoteNodes += ',rd' + $('input[name=rdNoteTreeViewType]:checked').val();

            return visibleNoteNodes;
        }

        function reopenPreviouslyVisibleNoteNodes(nodes) {
            var openNoteNodes = nodes != null && nodes.length > 0 ? nodes : getQueryStringParameter('OpenNoteNodes');

            if (openNoteNodes.indexOf('rdtype') != -1) {
                $('input[name=rdNoteTreeViewType][value=type]').prop('checked', true);
                noteTreeViewTypeClicked();
            }

            if (openNoteNodes.length > 0 && openNoteNodes.indexOf('anc') == -1) {
                var noteArr = openNoteNodes.split(',');

                for (var i = 0; i < noteArr.length; i++) {
                    var aorid = noteArr[i];

                    var node = window[getNoteTreeViewClientID() + '_GetNodeByKey'](aorid);
                    var level = node.attr('level');
                    var idx = node.attr('idx');

                    window[getNoteTreeViewClientID() + '_ToggleNode'](level, idx, aorid, true);
                }
            }

            _allNoteToggleCollapse = openNoteNodes.indexOf('atc') != -1;
            $('#imgDisplayAllNotes').attr('src', 'Images/Icons/' + (_allNoteToggleCollapse ? 'collapse.gif' : 'expand.gif'));
            var mode = _allNoteToggleCollapse ? 'Hide' : 'Show';
            $('#imgDisplayAllNotes').attr('title', mode + ' All')
            $('#imgDisplayAllNotes').attr('alt', mode + 'All')
        }

        function setInitialNoteListPaneSize() {
            var agendaWidth = $('#divAgendaTab').width();

            if (agendaWidth > 1500) {
                _defaultNoteListPaneCollapsedWidth = 350; // for wider pages, we start the note pane bigger
            }

            _noteListPaneExpanded = true;
            toggleNoteListPane();
        }

        function toggleNoteListPane() {
            _noteListPaneExpanded = !_noteListPaneExpanded;
            $('#divNoteByAOR').css('width', _noteListPaneExpanded ? _defaultNoteListPaneExpandedWidth + 'px' : _defaultNoteListPaneCollapsedWidth + 'px');
            $('#divNoteByType').css('width', _noteListPaneExpanded ? _defaultNoteListPaneExpandedWidth + 'px' : _defaultNoteListPaneCollapsedWidth + 'px');

            var btnGoToNoteDivLeftPos = $('#btnGoToNote').parent().position().left;
            var btnGoToNoteLeftPos = $('#btnGoToNote').position().left;
            var buttonLeft = (btnGoToNoteDivLeftPos + btnGoToNoteLeftPos);

            $('#noteSpacer').css('top', _noteListPaneExpanded && buttonLeft < _defaultNoteListPaneExpandedWidth ? '0px' : '-30px');
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
                var $pageContainerPos = $('#divPageContainer').position();

                $objDiv.css({
                    top: ($pageContainerPos.top),
                    left: ($pos.left)
                }).slideDown();

                $('#<%=this.cblDownloadPDFSettings.ClientID %> input').prop('checked', false);

                PageMethods.GetAORs('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', '', getAORCount_done, function () { });
                PageMethods.GetNotes('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', '', $('#chkShowRemovedNote').is(':checked'), '', getNoteCounts_done, function () { });
                PageMethods.GetAttachments('<%=this.AORMeetingInstanceID %>', getAttachmentCounts_done, function () { });
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

        function getAORCount_done(result) {
            var dt = jQuery.parseJSON(result);

            var container = $('#<%=this.cblDownloadPDFSettings.ClientID %>');

            if (dt != null) {
                var cb = container.find('input[type=checkbox][value=\'AORs Included\']');
                var label = cb.next();

                var cnt = dt.length;
                var cntStr = cnt > 0 ? ' (' + cnt + ')' : '';

                label.text('AORs Included' + cntStr);
                cb.prop('checked', cnt > 0);

                cb = container.find('input[type=checkbox][value=\'Burndown Grid\']');
                label = cb.next();
                label.text('Grid' + cntStr);
                cb.prop('checked', cntStr != '');

                if (cnt > 0) {
                    cb = container.find('input[type=checkbox][value=\'BurndownOverviewParent\']');
                    cb.prop('checked', true);
                }
            }
        }

        function getNoteCounts_done(result) {
            var dt = jQuery.parseJSON(result);

            var container = $('#<%=this.cblDownloadPDFSettings.ClientID %>');

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    var noteType = row.AORNoteTypeName;
                    var cnt = parseInt(row.NoteDetailCount);
                    var cntStr = cnt > 0 ? ' (' + cnt + ')' : '';

                    var cb = container.find('input[type=checkbox][value=\'' + noteType + '\']');
                    var label = cb.next();

                    if (noteType == 'Burndown Overview') {
                        noteType = 'Notes';
                    }

                    label.text(noteType + cntStr);
                    cb.prop('checked', cnt > 0);
                });
            }
        }

        function getAttachments() {
            loadAttachments();
            PageMethods.GetAttachments('<%=this.AORMeetingInstanceID %>', getAttachmentCounts_done, function () { });
        }

        function getAttachmentCounts_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null) {
                if (dt == '') dt = '0';
                $('#<%=this.cblDownloadPDFSettings.ClientID %> label').each(function () {
                    if ($(this).text() == 'Attachments') {
                        $(this).text('Attachments (' + dt + ')');
                    }
                });
            }
        }

        function download(type) {
            var downloadSettings = '';
            var emailSettings = '';

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

                    downloadSettings = arrSelections.join(',');
                    break;
            }

            if (isEmailMode()) {
                var arrSelections = [];

                $('[id=cbResourceEmail]input:checked').each(function () {
                    arrSelections.push($(this).attr('resourceid'));
                });

                if (arrSelections.length == 0) {
                    MessageBox('You must select at least one e-mail recipient.');
                    return false;
                }

                var customNote = $('#txtEmailCustomNote').val();

                emailSettings = (customNote != null && customNote.length > 0 ? escape(customNote) : '[EMPTY]') + ',' + arrSelections.join(',');

                var bm = new bubbleMessage($('#divAORMeetingInstance')[0], '<div style="background-color:#ffffff;border:1px solid #aaaaaa;padding:10px;">Sending email... <img src="images/loaders/progress_bar_blue.gif" style="position:relative;top:5px"></div>');
                bm.verticalAlign = 'center';
                bm.horizontalAlign = 'center';
                bm.show();
                PageMethods.EmailMinutes(downloadSettings, emailSettings, '<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', EmailMinutes_done, EmailMinutes_error);
            }
            else {
                $('#frmDownload').attr('src', 'AOR_Meeting_Instance_Edit.aspx' + window.location.search + '&Download=' + type + '&DownloadSettings=' + downloadSettings + '&ShowRemoved=' + _displayingRemovedNotes);
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

        function EmailMinutes_error(result) {
            closeActiveBubbleMessagesFromPopup();
            MessageBox('Unable to send e-mail. An error has occurred.');
        }

        function btnCancel_click() {
            if ($('#btnSave').is(':enabled')) {
                QuestionBox('Confirm Cancel', 'You have unsaved changes. Are you sure you would like to cancel?', 'Yes,No', 'confirmRefresh', 300, 300, this);
            }
            else {
                refreshPage();
            }
        }

        function btnEndMeeting_click() {
            QuestionBox('Confirm End Meeting', 'Are you sure you would like to end this meeting?', 'Yes,No', 'confirmEndMeeting', 300, 300, this);
        }

        function confirmRefresh(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') refreshPage();
        }

        function confirmEndMeeting(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') btnSave_click(false, true);
        }

        function btnSave_click(closeAfterSave, endMeeting, returnJSONOnly, hideDimmer) {
            if (endMeeting == null) endMeeting = _meetingHasEnded; // if we don't specify an end/meeting value, preserve the status quo
            if (returnJSONOnly == null) returnJSONOnly = false;
            if (hideDimmer == null) hideDimmer = false;

            try {
                var validation = validate();

                if (validation.length == 0) {
                    if (!returnJSONOnly && !hideDimmer) {
                        ShowDimmer(true, (endMeeting && !_meetingHasEnded ? 'Saving data and ending meeting...' : 'Saving...'), 1);
                    }

                    if (!returnJSONOnly) {
                        _saveInProgress = true;
                    }

                    var arrResources = [], arrNotes = [], arrNoteDetails = [];

                    if ('<%=this.NewAORMeetingInstance %>'.toUpperCase() == 'FALSE') {
                        $('#divAORMIResources tr').not(':first').each(function () {
                            var $obj = $(this);

                            if ($obj.find('input[field="Attended"]').attr('resourceid')) {
                                var attended = $obj.find('input[field="Attended"]').is(':checked') ? '1' : '0';
                                var reasonForAttending = $obj.find('input[field="ReasonForAttending"]').val();

                                arrResources.push({ 'resourceid': $obj.find('input[field="Attended"]').attr('resourceid'), 'attended': attended, 'reasonforattending': reasonForAttending });
                            }
                        });

                        $('#divAORMINotesDetail table tbody tr:has(td select[field="Status"])').each(function () {
                            var $obj = $(this);

                            if ($obj.find('select[field="Status"]').attr('aormeetingnotesid')) {
                                var title = $obj.find('input[field="Title"]').val();
                                var noteDetails = $obj.next().find('textarea[field="Note Details"]').val();
                                var aorReleaseID = $obj.find('select[field="AOR Name"]').val();
                                var statusID = $obj.find('select[field="Status"]').val();
                                var sort = $obj.find('input[field="Sort"]').val();
                                var noteTypeID = $obj.find('select[field="AOR Note Type"]').val();
                                var extData = $obj.find('input[name=extdata]').val();

                                if (title == undefined) title = $obj.find('td:eq(' + ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' ? 2 : 1) + ')').text();
                                if (noteDetails == undefined) noteDetails = $obj.next().find('td:eq(' + ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' ? 2 : 1) + ')').text();

                                arrNoteDetails.push({ 'aormeetingnotesid': $obj.find('select[field="Status"]').attr('aormeetingnotesid'), 'aornotetypeid': noteTypeID, 'title': title, 'notedetails': noteDetails, 'aorreleaseid': aorReleaseID, 'statusid': statusID, 'sort': sort, 'extdata' : UndoStrongEscape(extData) });
                            }
                        });
                    }
                    else {

                    }

                    var meetingInstanceName = $('#<%=this.txtAORMeetingInstanceName.ClientID %>').val();
                    var meetingInstanceDate = $('#<%=this.txtInstanceDate.ClientID %>').val();
                    var meetingNotes = $('#<%=this.txtNotes.ClientID %>').val();
                    var meetingLength = $('#<%=this.txtActualLength.ClientID %>').val();

                    var nResourcesJSON = '{save:' + JSON.stringify(arrResources) + '}';
                    var nNotesJSON = '{save:' + JSON.stringify(arrNotes) + '}';
                    var nNoteDetailsJSON = '{save:' + JSON.stringify(arrNoteDetails) + '}';

                    var aorMeetingID = <%=this.AORMeetingID %>;
                    if (_newMeetingInstance) {
                        var mtg = getSelectedExistingMeeting();

                        if (mtg != null) {
                            aorMeetingID = mtg.AORMeetingID;
                        }
                        else {
                            aorMeetingID = 0;
                        }
                    }

                    if (returnJSONOnly) {
                        var globalMeetingValues = [meetingInstanceName, meetingInstanceDate, meetingNotes, meetingLength];
                        var globalMeetingValueJSON = '{save:' + JSON.stringify(globalMeetingValues) + '}'

                        return globalMeetingValueJSON + nNotesJSON + nNoteDetailsJSON + nResourcesJSON;
                    }
                    else {

                        PageMethods.Save('<%=this.NewAORMeetingInstance %>', aorMeetingID, '<%=this.AORMeetingInstanceID %>', meetingInstanceName,
                            meetingInstanceDate, meetingNotes, meetingLength, nResourcesJSON, nNotesJSON, nNoteDetailsJSON, endMeeting ? "true" : "false", (endMeeting && !_meetingHasEnded) || _meetingIsLocked ? "true" : "false", _meetingIsAccepted ? "true" : "false",
                        <%=this.OriginatingAORID%>, <%=this.OriginatingAORReleaseID%>, _meetingHasEnded ? "true" : "false",
                            function (result) { save_done(result, closeAfterSave, endMeeting); }, on_error);
                    }
                }
                else {
                    _saveInProgress = false;

                    if (returnJSONOnly) {
                        return '[VALIDATIONERROR]';
                    }
                    else {
                        MessageBox('Invalid entries: <br><br>' + validation);
                    }
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
                _saveInProgress = false;
            }
        }

        function save_done(result, closeAfterSave, endMeeting) {
            ShowDimmer(false);
            _saveInProgress = false;

            var blnSaved = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);
            var aorMeetingID = 0;

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
                if (obj.error) errorMsg = obj.error;
                if (obj.AORMeetingID && parseInt(obj.AORMeetingID) > 0) aorMeetingID = obj.AORMeetingID;
            }

            if (blnSaved) {
                if (parent._newItemCreated != undefined) parent._newItemCreated = true;
                if (opener && opener.refreshPage) opener.refreshPage(true);

                if (!closeAfterSave) successMessage(endMeeting && !_meetingHasEnded ? 'Meeting has been saved and ended.' : 'Meeting Instance has been saved.', 'bottom right');
                if (!closeAfterSave && endMeeting) loadAttachments();

                if (closeAfterSave) {
                    setTimeout(closeWindow, 1);
                }
                else if (parseInt(newID) > 0) {
                    setTimeout(refreshPage(newID, aorMeetingID), 1);
                }
                else {
                    if (_openNoteTriggered != 0) {
                        openPopup('Yes', _openNoteTriggered);
                        _openNoteTriggered = 0;
                    }

                    if (endMeeting) {
                        if (!_meetingHasEnded) {
                            refreshPage(); // we need a full refresh because we are locking the meeting
                            return;
                        }

                        $('#imgMtgEnded').show();
                        $('#btnEndMeeting').hide();
                    }

                    $('#btnSave').prop('disabled', true);

                    getAORs();
                    getResources();
                    getNotes();
                    getAORProgress();
                    getAttachments();
                    RefreshTreeView();

                    if ($('#divTabsContainer').tabs('option', 'active') != 0) _refreshEditor = true;
                }
            }
            else {
                MessageBox('Failed to save.<br>' + errorMsg);
            }

            _openNoteTriggered = 0;
        }

        function on_error() {
            _treeViewRefreshing = false;
            _saveInProgress = false;
            _openNoteTriggered = 0;
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function btnBackToGrid_click() {
            if ($('#btnSave').is(':enabled')) {
                QuestionBox('Confirm Back To Grid', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmBackToGrid', 300, 300, this);
            }
            else {
                if (parent.showFrameForGrid) parent.showFrameForGrid(false);
            }
        }

        function confirmBackToGrid(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    btnSave_click(true);
                    break;
                case 'DISCARD':
                    if (parent.showFrameForGrid) parent.showFrameForGrid(false);
                    break;
                default:
                    return;
            }
        }

        function btnClose_click() {
	        if ($('#btnSave').is(':enabled')) {
	            QuestionBox('Confirm Close', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmClose', 300, 300, this);
	        }
	        else {
	            closeWindow();
	        }
	    }

        function confirmClose(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    btnSave_click(true);
                    break;
                case 'DISCARD':
                    closeWindow();
                    break;
                default:
                    return;
            }
        }

        function setDetailsToggleState(detailsVisible) {
            if (detailsVisible) {
                var $obj = $('#imgToggleDetails');
                $obj.attr('src', 'Images/Icons/arrow_up_blue.png');
                $obj.attr('title', 'Hide Details');
                $obj.attr('alt', 'Hide Details');

                $('#divAORMeetingInstance').show();
                $('#<%=this.spnAORMeetingInstanceHeader.ClientID %>').hide();
            }
            else {
                var $obj = $('#imgToggleDetails');
                $obj.attr('src', 'Images/Icons/arrow_down_blue.png');
                $obj.attr('title', 'Show Details');
                $obj.attr('alt', 'Show Details');

                $('#divAORMeetingInstance').hide();
                $('#<%=this.spnAORMeetingInstanceHeader.ClientID %>').show();
            }
        }

        function imgToggleDetails_click(fast) {
            var $obj = $('#imgToggleDetails');

            if ($obj.attr('title') == 'Show Details') {
                $obj.attr('src', 'Images/Icons/arrow_up_blue.png');
                $obj.attr('title', 'Hide Details');
                $obj.attr('alt', 'Hide Details');
                $('#<%=this.spnAORMeetingInstanceHeader.ClientID %>').hide();

                if (fast) {
                    $('#divAORMeetingInstance').show();
                    //resizePage();
                }
                else {
                    $('#divAORMeetingInstance').slideDown('slow', resizePage);
                }
            }
            else {
                $obj.attr('src', 'Images/Icons/arrow_down_blue.png');
                $obj.attr('title', 'Show Details');
                $obj.attr('alt', 'Show Details');

                if (fast) {
                    $('#divAORMeetingInstance').hide();
                    //resizePage();
                }
                else {
                    $('#divAORMeetingInstance').slideUp('slow', function () { $('#<%=this.spnAORMeetingInstanceHeader.ClientID %>').show(); resizePage(); });
                }
            }
        }

        function tab_click(tabName) {
            switch (tabName.toUpperCase()) {
                case 'AGENDA':
                    if (_refreshEditor) refreshAllEditors(); //editor is blank when rebuilt and tab div is hidden
                break;
            }
        }

        function refreshAllEditors() {
            $('.editor').each(function () {
                $(this).cleditor()[0].refresh();
                resizeEditor(this);
            });

            _refreshEditor = false;
        }

        function validate() {
            var validation = [];

            if (_newMeetingInstance) {
                var mtg = getSelectedExistingMeeting();

                if (mtg == null) {
                    validation.push('Select an Existing Meeting.');
                }
            }
            else {
                if ($('#<%=this.txtAORMeetingInstanceName.ClientID %>').val().length == 0) validation.push('Meeting Instance Name cannot be empty.');
            }

            if ($('#<%=this.txtInstanceDate.ClientID %>').val().length == 0) validation.push('Meeting Date cannot be empty.');

            if ($('#divInstanceDateConflict').css('display') != 'none') validation.push('Meeting Date conflict must be resolved before saving.');


            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);

            var id = $obj.attr('id');
            var field = $obj.attr('field');

            if (field == 'ReasonForAttending' || field == 'Attended') {
                return;
            }

            if (id && id.indexOf('ActualLength') != -1) {
                var nVal = $obj.val();

                $obj.val(nVal.replace(/[^\d]/g, ''));
            }
            else if (id && id.indexOf('txtInstanceDate') != -1) {
                var aorMeetingID = <%=this.AORMeetingID %>;
                if (_newMeetingInstance) {
                    var mtg = getSelectedExistingMeeting();

                    if (mtg != null) {
                        aorMeetingID = mtg.AORMeetingID;
                    }
                    else {
                        aorMeetingID = 0;
                    }
                }

                if (aorMeetingID != 0) {
                    PageMethods.ValidateInstanceDate(aorMeetingID, '<%=this.AORMeetingInstanceID %>', $obj.val(), validateInstanceDate_done, on_error);
                }

                var dVal = $obj.val();
                var origVal = $obj.attr('origvalue');

                if (dVal == origVal) {
                    return;
                }
                else {
                    $obj.attr('origvalue', dVal);
                }
            }

            if ($obj.attr('field') && $obj.attr('field').indexOf('Sort') != -1) {
                var nVal = $obj.val();
                var blnNegative = nVal.indexOf('-') != -1 ? true : false;

                nVal = nVal.replace(/[^\d]/g, '');

                if (blnNegative) nVal = '-' + nVal;

                $obj.val(nVal);
            }

            var excludeArr = 'txtGoToNote'.split(',');
            if (excludeArr.indexOf(id) != -1) {
                return;
            }

            $('#btnSave').prop('disabled', false);
        }

        function input_blur() {
            if (!_newMeetingInstance && '<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                if (!$('#btnSave').prop('disabled')) { // we do this comparison first, trying to avoid calculating the json if it's not needed
                    var mtgJSON = btnSave_click(false, false, true);

                    if (_mtgJSON.length != mtgJSON.length && _mtgJSON != '[VALIDATIONERROR]' && _mtgJSON != mtgJSON) { // we do the string comparisons of the json LAST since comparing is the most expensive
                        _mtgJSON = mtgJSON;
                        btnSave_click(false, null, false, true);
                    }
                }
            }
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if ($obj.attr('field') && $obj.attr('field').indexOf('Sort') != -1) {
                if (nVal == '-') $obj.val('');
                return;
            }

            $obj.val($.trim(nVal));

            // we exclude some fields from auto-save (due to heavy refresh time, or non-trivial background-saves)
            if ($obj.attr('field') == 'ReasonForAttending') {
                return;
            }

            input_blur();
        }

        function refreshPage(newID, AORMeetingID) {
            var nURL = window.location.href;

            if (newID != undefined && parseInt(newID) > 0) {
                nURL = editQueryStringValue(nURL, 'NewAORMeetingInstance', 'false');
                nURL = editQueryStringValue(nURL, 'AORMeetingInstanceID', newID);
            }

            if (AORMeetingID != null) {
                nURL = editQueryStringValue(nURL, 'AORMeetingID', AORMeetingID);
            }

            nURL = editQueryStringValue(nURL, 'OpenNoteNodes', getVisibleNoteNodes());
            nURL = editQueryStringValue(nURL, 'DetailsVisible', $('#divAORMeetingInstance').is(':visible') ? '1' : '0');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            if (_newMeetingInstance) {
                var bodyHeight = $(window).height();
                $('#divExistingMeetings').height(bodyHeight - 225);
            }
            else {
                var bodyHeight = $(window).height();
                var visibleTabDivTop = $('.tabDiv:visible').offset().top + 5;

                $('.tabDiv').height(bodyHeight - visibleTabDivTop);

                var agendaHeight = $('#divAgendaTab').height();
                $('#divNoteByAOR').height(agendaHeight - 30);
            }
        }

        function resizeEditor(obj) {
            var editor = $(obj).cleditor()[0];
            var editorMain = editor.$main[0];
            var editorFrame = editor.$frame[0];
            var contentHeight = editorFrame.contentWindow.document.body.scrollHeight;
            var maxHeightForNote = $('#divAgendaTab').height() - 150; // account for buttons, toolbars, etc...

            if (contentHeight > maxHeightForNote) {
                contentHeight = maxHeightForNote;
                //editorFrame.css.overflowY = 'auto';
            }

            editorFrame.style.height = (contentHeight + 25) + 'px';
            editorMain.style.height = (contentHeight + 55) + 'px';
            editorFrame.style.width = '99.7%';
            editorMain.style.width = '99.7%';
        }

        function chkShowRemovedAOR_change() {
            getAORs();
        }

        function btnAddAOR_click() {
            var nWindow = 'AddAOR';
            var nTitle = 'Add AOR';
            var nHeight = 500, nWidth = 650;
            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function getAORs() {
            $('#divAORMIAOR').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetAORs('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', $('#chkShowRemovedAOR').is(':checked'), getAORs_done, getAORs_error);
        }

        function getAORs_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No AORs';
            }
            else {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 60px;">';
                nHTML += '<img src="Images/Icons/help.png" alt="(Number of open SRs associated with AOR via assigned CRs / Number of open Tasks associated with AOR)" title="(Number of open SRs associated with AOR via assigned CRs / Number of open Tasks associated with AOR)" width="12" height="12" onclick="MessageBox(\'' + '(Number of open SRs associated with AOR via assigned CRs / Number of open Tasks associated with AOR)' + '\');" style="cursor: pointer;" />';
                nHTML += '</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">AOR #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 250px;">AOR Name</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 150px;">AOR Type</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Description</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 150px;">Date Added</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="border-left: 1px solid grey; text-align: center;">';
                    nHTML += '<div style="padding: 3px;"><img id="img' + row.AORRelease_ID + '" src="Images/Icons/add_blue.png" title="Show" alt="Show" height="12" width="12" onclick="toggleSRsTasks(this); return false;" aorreleaseid="' + row.AORRelease_ID + '" style="cursor: pointer;" />&nbsp;(' + row.SRCount + '/' + row.TaskCount + ')</div>';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        if (row.Included == 1) {
                            nHTML += '<div style="padding: 3px; padding-bottom: 0px;"><a href="" onclick="toggleAOR(' + row.AORRelease_ID + ', 0); return false;" style="color: blue;">Remove</a></div>';
                        }
                        else if (row.Included == 0) {
                            nHTML += '<div style="padding: 3px; padding-bottom: 0px;"><a href="" onclick="toggleAOR(' + row.AORRelease_ID + ', 1); return false;" style="color: blue;">Re-Add</a></div>';
                        }
                    }

                    nHTML += '</td><td style="text-align: center;">';

                    if ('<%=this.CanViewAOR %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<a href="" onclick="openAOR(' + row.AOR_ID + ', ' + row.AORRelease_ID + ', 0); return false;" style="color: blue;">' + row.AOR_ID + '</a>';
                    }
                    else {
                        nHTML += row.AOR_ID;
                    }

                    nHTML += '</td><td>' + row.AOR + '</td>';
                    nHTML += '<td>' + row.AORWorkTypeName + '</td>';
					nHTML += '<td>' + row.Description + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.DateAddedString + '</td>';
                    nHTML += '</tr>';

                    if (rowIndex == dt.length - 1) {
                        nHTML += '<tr class="gridBody" style="display: none;"><td style="border-right: none; border-bottom: none;">&nbsp;</td><td colspan="6" style="border-right: none; border-bottom: none; padding-top: 10px; padding-bottom: 10px;">';
                    }
                    else {
                        nHTML += '<tr class="gridBody" style="display: none;"><td style="border-right: none;">&nbsp;</td><td colspan="6" style="border-right: none; padding-top: 10px; padding-bottom: 10px;">';
                    }

                    nHTML += '<div style="padding: 3px; background: url(Images/page_header_back.gif); border-bottom: 1px solid #d3d3d3;">';
                    nHTML += '<table style="border-collapse: collapse; width: 100%;"><tr>';
                    nHTML += '<td style="padding-left: 5px; border: none;">SRs</td>';
                    nHTML += '<td style="text-align: right; padding-right: 5px; border: none;">';
                    nHTML += '<label><input type="checkbox" id="chkShowClosedSRs' + row.AORRelease_ID + '" name="chkShowClosedSRs" onchange="chkShowClosedSRs_change(' + row.AORRelease_ID + ');"';

                    if (parent._checkedShowClosedSRs != undefined) {
                        if ($.inArray(row.AORRelease_ID, parent._checkedShowClosedSRs) != -1) nHTML += ' checked ';
                    }

                    nHTML += '/>Show Closed SRs</label>&nbsp;&nbsp;';
                    nHTML += '</td></tr></table></div>';
                    nHTML += '<div id="divAORMISRs' + row.AORRelease_ID + '"></div>';
                    nHTML += '<div style="padding: 3px; background: url(Images/page_header_back.gif); border-bottom: 1px solid #d3d3d3;">';
                    nHTML += '<table style="border-collapse: collapse; width: 100%;"><tr>';
                    nHTML += '<td style="padding-left: 5px; border: none;">Tasks</td>';
                    nHTML += '<td style="text-align: right; padding-right: 5px; border: none;">';
                    nHTML += '<label><input type="checkbox" id="chkShowClosedTasks' + row.AORRelease_ID + '" name="chkShowClosedTasks" onchange="chkShowClosedTasks_change(' + row.AORRelease_ID + ');"';

                    if (parent._checkedShowClosedTasks != undefined) {
                        if ($.inArray(row.AORRelease_ID, parent._checkedShowClosedTasks) != -1) nHTML += ' checked ';
                    }

                    nHTML += '/>Show Closed Tasks</label>&nbsp;&nbsp;';
                    nHTML += '</td></tr></table></div>';
                    nHTML += '<div id="divAORMITasks' + row.AORRelease_ID + '"></div>';
                    nHTML += '</td></tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORMIAOR').html(nHTML);
            displaySRsTasks();
        }

        function getAORs_error() {
            $('#divAORMIAOR').html('Error gathering data.');
        }

        function displaySRsTasks() {
            if (parent._expandedAORs != undefined) {
                $.each(parent._expandedAORs, function (index, item) {
                    var $img = $('#img' + item);

                    if ($img.attr('title') == 'Show') $img.trigger('click');
                });
            }
        }

        function toggleAOR(AORReleaseID, opt) {
            PageMethods.ToggleAOR('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', AORReleaseID, opt, toggleAOR_done, toggleAOR_error);
        }

        function toggleAOR_done() {
            getAORs();
            getResources();
            getAORProgress();
        }

        function toggleAOR_error() {
            getAORs();
            MessageBox('An error has occurred.');
        }

        function openAOR(AORID, AORReleaseID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&Source=MI&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function toggleSRsTasks(obj) {
            var $obj = $(obj);
            var AORReleaseID = $obj.attr('aorreleaseid');

            if ($obj.attr('title') == 'Show') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Hide')
                $obj.attr('alt', 'Hide')
                $obj.closest('tr').next().show();

                if ($('#divAORMISRs' + AORReleaseID).html() == '') getSRs(AORReleaseID);
                if ($('#divAORMITasks' + AORReleaseID).html() == '') getTasks(AORReleaseID);

                if (parent._expandedAORs != undefined) {
                    if ($.inArray(AORReleaseID, parent._expandedAORs) == -1) parent._expandedAORs.push(AORReleaseID);
                }
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Show')
                $obj.attr('alt', 'Show')
                $obj.closest('tr').next().hide();

                if (parent._expandedAORs != undefined) parent._expandedAORs.splice($.inArray(AORReleaseID, parent._expandedAORs), 1);
            }
        }

        function chkShowClosedSRs_change(AORReleaseID) {
            if ($('#chkShowClosedSRs' + AORReleaseID).is(':checked')) {
                if (parent._checkedShowClosedSRs != undefined) {
                    if ($.inArray(AORReleaseID, parent._checkedShowClosedSRs) == -1) parent._checkedShowClosedSRs.push(AORReleaseID);
                }
            }
            else {
                if (parent._checkedShowClosedSRs != undefined) parent._checkedShowClosedSRs.splice($.inArray(AORReleaseID, parent._checkedShowClosedSRs), 1);
            }

            getSRs(AORReleaseID);
        }

        function getSRs(AORReleaseID) {
            $('#divAORMISRs' + AORReleaseID).html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" style="padding: 10px;" />');

            PageMethods.GetSRs('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', AORReleaseID, $('#chkShowClosedSRs' + AORReleaseID).is(':checked'), function (result) { getSRs_done(result, AORReleaseID); }, function () { getSRs_error(AORReleaseID); });
        }

        function getSRs_done(result, AORReleaseID) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                $('#divAORMISRs' + AORReleaseID).css('padding', '10px');
                nHTML = 'No SRs';
            }
            else {
                $('#divAORMISRs' + AORReleaseID).css('padding', '');
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 40px; border-left: 1px solid grey;">SR #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 120px;">Submitted By</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 70px;">Submitted Date</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 120px;">Status</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">Priority</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Description</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 300px;">Last Reply</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">Task #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 100px;">Task Status</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Task Assigned To</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="border-left: 1px solid grey; text-align: center;">' + row.SRID + '</td>';
                    nHTML += '<td>' + row.SubmittedBy + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.SubmittedDate + '</td>';
                    nHTML += '<td>' + row.Status + '</td>';
                    nHTML += '<td>' + row.Priority + '</td>';

                    var description = decodeURIComponent(row.Description);

                    if (description.length > 125) {
                        nHTML += '<td><a href="" title="' + description + '" onclick="alert(' + JSON.stringify(description).replace(/"/g, '&quot;') + '); return false;" style="color: blue;">' + description.substring(0, 125) + '...' + '</a></td>';
                    }
                    else {
                        nHTML += '<td>' + description + '</td>';
                    }

                    var lastReply = decodeURIComponent(row.LastReply);

                    if (lastReply.length > 125) {
                        nHTML += '<td><a href="" title="' + lastReply + '" onclick="alert(' + JSON.stringify(lastReply).replace(/"/g, '&quot;') + '); return false;" style="color: blue;">' + lastReply.substring(0, 125) + '...' + '</a></td><td style="text-align: center;">';
                    }
                    else {
                        nHTML += '<td>' + lastReply + '</td><td style="text-align: center;">';
                    }

                    if (row.TaskNumber == 0) {
                        nHTML += '&nbsp';
                    }
                    else {
                        if ('<%=this.CanViewWorkItem %>'.toUpperCase() == 'TRUE') {
                            nHTML += '<a href="" onclick="openTask(' + row.TaskNumber + ', 0); return false;" style="color: blue;">' + row.TaskNumber + '</a>';
                        }
                        else {
                            nHTML += row.TaskNumber;
                        }
                    }

                    nHTML += '</td><td>' + row.TaskStatus + '</td>';
                    nHTML += '<td>' + row.TaskAssignedTo + '</td>';
                    nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORMISRs' + AORReleaseID).html(nHTML);
        }

        function getSRs_error(AORReleaseID) {
            $('#divAORMISRs' + AORReleaseID).css('padding', '10px');
            $('#divAORMISRs' + AORReleaseID).html('Error gathering data.');
        }

        function chkShowClosedTasks_change(AORReleaseID) {
            if ($('#chkShowClosedTasks' + AORReleaseID).is(':checked')) {
                if (parent._checkedShowClosedTasks != undefined) {
                    if ($.inArray(AORReleaseID, parent._checkedShowClosedTasks) == -1) parent._checkedShowClosedTasks.push(AORReleaseID);
                }
            }
            else {
                if (parent._checkedShowClosedTasks != undefined) parent._checkedShowClosedTasks.splice($.inArray(AORReleaseID, parent._checkedShowClosedTasks), 1);
            }

            getTasks(AORReleaseID);
        }

        function getTasks(AORReleaseID) {
            $('#divAORMITasks' + AORReleaseID).html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" style="padding: 10px;" />');

            PageMethods.GetTasks('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', AORReleaseID, $('#chkShowClosedTasks' + AORReleaseID).is(':checked'), function (result) { getTasks_done(result, AORReleaseID); }, function () { getTasks_error(AORReleaseID); });
        }

        function getTasks_done(result, AORReleaseID) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                $('#divAORMITasks' + AORReleaseID).css('padding', '10px');
                nHTML = 'No Tasks';
            }
            else {
                $('#divAORMITasks' + AORReleaseID).css('padding', '');
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px; border-left: 1px solid grey;">Task #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Title</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 100px;">System(Task)</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 60px;">Product Version</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 75px;">Production Status</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">Priority</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">SR Number</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Assigned To</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Primary Resource</th>';
                //nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Secondary Tech. Resource</th>';
                //nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Primary Bus. Resource</th>';
                //nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Secondary Bus. Resource</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 110px;">Status</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 65px;">Percent Complete</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="border-left: 1px solid grey; text-align: center;">';

                    if ('<%=this.CanViewWorkItem %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<a href="" onclick="openTask(' + row.WORKITEMID + ', 0); return false;" style="color: blue;">' + row.WORKITEMID + '</a></td>';
                    }
                    else {
                        nHTML += row.WORKITEMID + '</td>';
                    }

                    var title = decodeURIComponent(row.TITLE);

                    if (title.length > 125) {
                        nHTML += '<td><a href="" title="' + title + '" onclick="alert(' + JSON.stringify(title).replace(/"/g, '&quot;') + '); return false;" style="color: blue;">' + title.substring(0, 125) + '...' + '</a></td>';
                    }
                    else {
                        nHTML += '<td>' + title + '</td>';
                    }

                    nHTML += '<td>' + row.WTS_SYSTEM + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.ProductVersion + '</td>';
                    nHTML += '<td>' + row.WorkloadAllocation + '</td>';
                    nHTML += '<td>' + row.PRIORITY + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.SR_Number + '</td>';
                    nHTML += '<td>' + row.AssignedTo + '</td>';
                    nHTML += '<td>' + row.PrimaryTechResource + '</td>';
                    //nHTML += '<td>' + row.SecondaryTechResource + '</td>';
                    //nHTML += '<td>' + row.PrimaryBusResource + '</td>';
                    //nHTML += '<td>' + row.SecondaryBusResource + '</td>';
                    nHTML += '<td>' + row.STATUS + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.COMPLETIONPERCENT + '</td>';
                    nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORMITasks' + AORReleaseID).html(nHTML);
        }

        function getTasks_error(AORReleaseID) {
            $('#divAORMITasks' + AORReleaseID).css('padding', '10px');
            $('#divAORMITasks' + AORReleaseID).html('Error gathering data.');
        }

        function openTask(taskID) {
            var nWindow = 'WorkTask';
            var nTitle = 'Work Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(taskID) > 0) {
                nTitle += ' - [' + taskID + ']';
                nURL += '?workItemID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function showClosed(answer, type) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                if ($('#chkShowClosedNoteDetail' + type).is(':checked')) {
                    if (parent._checkedShowClosed != undefined) {
                        if ($.inArray(type, parent._checkedShowClosed) == -1) parent._checkedShowClosed.push(type);
                    }
                }
                else {
                    if (parent._checkedShowClosed != undefined) parent._checkedShowClosed.splice($.inArray(type, parent._checkedShowClosed), 1);
                }

                getNotesDetail(type);
            }
            else {
                $('#chkShowClosedNoteDetail' + type).prop('checked', !$('#chkShowClosedNoteDetail' + type).is(':checked'));
            }
        }

        function showRemoved(answer, type) {
            switch (type) {
                case 'Note':
                    var answerIsChecked = $.trim(answer).toUpperCase() == 'YES';
                    if (_displayingRemovedNotes != answerIsChecked) {
                        getNotes();
                        RefreshTreeView();
                    }
                    _displayingRemovedNotes = answerIsChecked;
                    break;
                case 'Resource':
                    if ($.trim(answer).toUpperCase() == 'YES') {
                        getResources();
                    }
                    else {
                        $('#chkShowRemovedResource').prop('checked', !$('#chkShowRemovedResource').is(':checked'));
                    }
                    break;
                default: //Note Detail (needs type = AORMeetingNotesID)
                    if ($.trim(answer).toUpperCase() == 'YES') {
                        if ($('#chkShowRemovedNoteDetail' + type).is(':checked')) {
                            if (parent._checkedShowRemoved != undefined) {
                                if ($.inArray(type, parent._checkedShowRemoved) == -1) parent._checkedShowRemoved.push(type);
                            }
                        }
                        else {
                            if (parent._checkedShowRemoved != undefined) parent._checkedShowRemoved.splice($.inArray(type, parent._checkedShowRemoved), 1);
                        }

                        getNotesDetail(type);
                    }
                    else {
                        $('#chkShowRemovedNoteDetail' + type).prop('checked', !$('#chkShowRemovedNoteDetail' + type).is(':checked'));
                    }
                    break;
            }
        }

        function openPopup(answer, type) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                switch (type) {
                    case 'Note':
                        var nWindow = 'AddNoteDetail';
                        var nTitle = 'Add Note Detail';
                        var nHeight = 500, nWidth = 650;
                        var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Note Detail&AORMeetingNotesID=0';
                        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                        if (openPopup) openPopup.Open();
                        break;
                    case 'EditObjectives':
                    case 'NoteObjectives':
                        // open the first (and only) agenda/objective note with extdata containing objectives json (legacy meetings might have more than one)
                        var agendaNoteID = 0;

                        var agendaNodes = <%=tvNoteByAOR.ClientID%>_GetChildNodes(null, 'ao', 1);
                        if (agendaNodes != null && agendaNodes.length > 0) {
                            var node = agendaNodes[0];
                            agendaNoteID = $(node).attr('key');
                        }

                        if (agendaNoteID == null || agendaNoteID == 0) {
                            var nWindow = 'AddNoteObjectives';
                            var nTitle = 'Add Objectives';
                            var nHeight = 500, nWidth = 895;
                            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Add Note Objectives&AORMeetingNotesID=0';
                            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                            if (openPopup) openPopup.Open();
                        }
                        else {
                            var nWindow = 'EditNoteObjectives';
                            var nTitle = 'Edit Objectives';
                            var nHeight = 500, nWidth = 895;
                            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Edit Note Objectives&AORMeetingNotesID=' + agendaNoteID;
                            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                            if (openPopup) openPopup.Open();
                        }
                        break;
                    case 'Resource':
                        var nWindow = 'AddResource';
                        var nTitle = 'Add Resource';
                        var nHeight = 500, nWidth = 650;
                        var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Resource';
                        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                        if (openPopup) openPopup.Open();
                        break;
                    default: // this section is reached from the noteid links; the 'type' is 'AORMeetingNotesID.NoteType'
                        var tokens = type.split('.');

                        if (tokens.length == 2 && tokens[1] == <%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>) {
                            var nWindow = 'EditNoteObjectives';
                            var nTitle = 'Edit Objectives';
                            var nHeight = 500, nWidth = 895;
                            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Edit Note Objectives&AORMeetingNotesID=' + tokens[0];
                            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                            if (openPopup) openPopup.Open();
                        }
                        else {
                            var nWindow = 'NoteDetail';
                            var nTitle = 'Note Detail';
                            var nHeight = 500, nWidth = 650;
                            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Edit Note Detail&AORMeetingNotesID=' + tokens[0];
                            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                            if (openPopup) openPopup.Open();
                        }
                        break;
                }
            }
        }

        function openAORMeetingInstance(AORMeetingID, AORMeetingInstanceID, forceUnlock) {
            var nWindow = 'PastAORMeetingInstance';
            var nTitle = 'Past Meeting Instance';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORMeetingInstanceEdit + window.location.search;

            nURL = editQueryStringValue(nURL, 'AORMeetingID', AORMeetingID);
            nURL = editQueryStringValue(nURL, 'AORMeetingInstanceID', AORMeetingInstanceID);
            if (forceUnlock) {
                nURL = editQueryStringValue(nURL, 'ForceUnlock', 'true');
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function chkShowRemovedResource_change() {
            if ($('#btnSave').is(':enabled')) {
                QuestionBox('Confirmation', 'Unsaved changes on the Resources Attending tab will be lost. Would you like to proceed?', 'Yes,No', 'showRemoved', 300, 300, this, 'Resource');
            }
            else {
                showRemoved('Yes', 'Resource');
            }
        }

        function btnAddResource_click() {
            if ($('#btnSave').is(':enabled')) {
                QuestionBox('Confirmation', 'Unsaved changes on the Resources Attending tab will be lost if a new resource is added. Would you like to proceed?', 'Yes,No', 'openPopup', 300, 300, this, 'Resource');
            }
            else {
                openPopup('Yes', 'Resource');
            }
        }

        function getResources() {
            $('#divAORMIResources').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetResources('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', $('#chkShowRemovedResource').is(':checked'), getResources_done, getResources_error);
        }

        function getResources_done(result) {
            var nHTML = '';
            var rHTML = '';
            var dt = jQuery.parseJSON(result);
            var emailResourcesAdded = 0;

            if (dt == null || dt.length == 0) {
                nHTML = 'No Resources';
                rHTML = nHTML;
            }
            else {
                var rCols = parseInt(dt.length / 20) + 1;
                if (rCols > 4) rCols = 4;

                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                rHTML = nHTML;
                nHTML += '<tr class="gridHeader">';

                if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                    nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 50px;"></th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 150px;">';
                }
                else {
                    nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 150px;">';
                }

                nHTML += 'Resource</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 250px;">Affiliated AOR</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 150px;">Last Meeting Attended</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 95px;">Attendance %</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 75px;">Attended</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Reason For Attending</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<td style="border-left: 1px solid grey; text-align: center;">';

                        if (row.Included == 1) {
                            nHTML += '<a href="" onclick="toggleResource(' + row.WTS_RESOURCEID + ', 0); return false;" style="color: blue;">Remove</a>';
                        }
                        else if (row.Included == 0) {
                            nHTML += '<a href="" onclick="toggleResource(' + row.WTS_RESOURCEID + ', 1); return false;" style="color: blue;">Re-Add</a>';
                        }

                        nHTML += '</td><td>' + row.Resource + '</td>';
                    }
                    else {
                        nHTML += '<td style="border-left: 1px solid grey;">' + row.Resource + '</td>';
                    }

                    nHTML += '<td>' + row.AffiliatedAOR + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.LastMeetingAttendedString + '</td>';
                    nHTML += '<td style="text-align: center;">' + row.AttendancePercentage + '</td>';
                    nHTML += '<td style="text-align: center;">';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.Included == 1) {
                        nHTML += '<input type="checkbox" resourceid="' + row.WTS_RESOURCEID + '" field="Attended" original_value="' + (row.Attended == 'Yes' ? '1' : '0') + '" ' + (row.Attended == 'Yes' ? 'checked' : '') + ' />';
                    }
                    else {
                        nHTML += row.Attended;
                    }

                    nHTML += '</td>';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.Included == 1) {
                        nHTML += '<td style="text-align: center;">';
                        nHTML += '<input type="text" value="' + row.ReasonForAttending.replace(/\"/g, '&quot;') + '" maxlength="500" resourceid="' + row.WTS_RESOURCEID + '" field="ReasonForAttending" style="width: 95%;" />';
                    }
                    else {
                        nHTML += '<td>' + row.ReasonForAttending;
                    }

                    nHTML += '</td></tr>';

                    // build e-mail meeting minutes div
                    if (row.WTS_RESOURCE_TYPEID != <%=(int)UserManagement.ResourceType.Not_People%>) {

                        rHTML += '<tr>';
                        rHTML += '<td><input id="cbResourceEmail" type="checkbox" resourceid="' + row.WTS_RESOURCEID + '" field="EmailDefault" original_value="' + row.EmailDefault + '" ' + (row.EmailDefault == '1' ? 'checked' : '') + ' origsort="' + emailResourcesAdded + '" />&nbsp;' + row.Resource.replace('\'', '&amp;').replace('<', '&lt;').replace('>', '&gt;') + '</td>';
                        rHTML += '</tr>';

                        emailResourcesAdded++;
                    }
                });

                nHTML += '</table>';
                rHTML += '</table>';
            }

            $('#divAORMIResources').html(nHTML);
            $('#divAORMIResources').find('input[field=Attended],input[field=ReasonForAttending]').on('change', function () { resourceUpdated($(this).attr('resourceid')) });

            // email meeting minutes
            $('#divRecipientListContainer').html(rHTML);
            $("[id=cbResourceEmail]").on('change', sortEmailResources);
            sortEmailResources();
            var maxRows = $('#divPageContainer').height() / 22;
            if (emailResourcesAdded > (maxRows * .5)) {
                $('#divRecipientListContainer').height($('#divPageContainer').height() * .3);
                $('#divRecipientListContainer').css('overflow-y', 'auto');
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

        function getResources_error() {
            $('#divAORMIResources').html('Error gathering data.');
        }

        function toggleResource(resourceID, opt) {
            _optResource = opt;

            if (_optResource == 0) {
                QuestionBox('Confirm Resource Hide', 'Are you sure you would like to hide this resource? Unsaved changes on the Resources Attending tab will be lost. Would you like to proceed?', 'Yes,No', 'confirmToggleResource', 300, 300, this, resourceID);
            }
            else {
                QuestionBox('Confirm Resource Unhide', 'Are you sure you would like to unhide this resource? Unsaved changes on the Resources Attending tab will be lost. Would you like to proceed?', 'Yes,No', 'confirmToggleResource', 300, 300, this, resourceID);
            }
        }

        function confirmToggleResource(answer, resourceID) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                PageMethods.ToggleResource('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', resourceID, _optResource, toggleResource_done, toggleResource_error);
            }
        }

        function toggleResource_done() {
            getResources();
        }

        function toggleResource_error() {
            getResources();
            MessageBox('An error has occurred.');
        }

        function resourceUpdated(resourceID) {
            var attended = $('#divAORMIResources').find('input[field=Attended][resourceid=' + resourceID + ']').is(':checked');
            var reasonForAttending = $('#divAORMIResources').find('input[field=ReasonForAttending][resourceid=' + resourceID + ']').val();

            PageMethods.ResourceUpdated('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', resourceID, attended, reasonForAttending, resourceUpdated_done, on_error)
        }

        function resourceUpdated_done(results) {
            var dt = $.parseJSON(results);

            if (dt.success) {
                infoMessage('Resource updated.', 'bottom right');
            }
            else {
                dangerMessage('Unable to update resource.');
            }
        }

        function showQF(answer, type) {
            switch (type) {
                case 'Note':
                    if ($.trim(answer).toUpperCase() == 'YES') {
                        getNotes();
                        RefreshTreeView();
                        _oldNoteTypeQF = $('#<%=this.ddlNoteType.ClientID %>').val();
                    }
                    else {
                        $('#<%=this.ddlNoteType.ClientID %>').val(_oldNoteTypeQF);
                    }
                    break;
            }
        }

        function imgDisplayAllNotes_click() {
            var $obj = $('#imgDisplayAllNotes');

            ToggleNoteTreeViewNodes();

            if ($obj.attr('title') == 'Show All') {
                //$obj.attr('src', 'Images/Icons/collapse.gif');
                $obj.attr('title', 'Hide All')
                $obj.attr('alt', 'Hide All')

                $('.noteType').each(function () {
                    if ($(this).attr('title') == 'Show') $(this).trigger('click');
                });
            }
            else {
                //$obj.attr('src', 'Images/Icons/expand.gif');
                $obj.attr('title', 'Show All')
                $obj.attr('alt', 'Show All')

                $('.noteType').each(function () {
                    if ($(this).attr('title') == 'Hide') $(this).trigger('click');
                });
            }
        }

        function ddlNoteType_change() {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost. Would you like to proceed?', 'Yes,No', 'showQF', 300, 300, this, 'Note');
            }
            else {
                showQF('Yes', 'Note');
            }
        }

        function chkShowRemovedNote_change() {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost. Would you like to proceed?', 'Yes,No', 'showRemoved', 300, 300, this, 'Note');
            }
            else {
                showRemoved($('#chkShowRemovedNote').is(':checked') ? 'Yes' : 'No', 'Note');
            }
        }

        function btnViewHistoricalNotes_click() {
            var nWindow = 'ViewHistoricalNotes';
            var nTitle = 'View Historical Notes';
            var nHeight = 500, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Historical Notes';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAddNote_click() {
            var nWindow = 'AddNote';
            var nTitle = 'Add Note Breakout';
            var nHeight = 500, nWidth = 650;
            var nURL = _pageUrls.Maintenance.AORMeetingInstancePopup + window.location.search + '&Type=Note Type';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function getNotes() {
            $('#divAORMINotes').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetNotes('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', $('#<%=this.ddlNoteType.ClientID %>').val(), $('#chkShowRemovedNote').is(':checked'), '', getNotes_done, getNotes_error);
        }

        function getNotes_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No Notes';
            }
            else {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';

                $.each(dt, function (rowIndex, row) {
                    var extdata = row.ExtData;
                    if (extdata == null) extdata = '';

                    nHTML += '<tr><td><div class="pageContentHeader" style="height: 25px;"><table style="width: 100%;"><tr><td style="width: 55px; padding-left: 10px;">';
                    nHTML += '<img id="img' + row.AORMeetingNotesID + '" class="noteType" src="Images/Icons/add_blue.png" title="Show" alt="Show" height="12" width="12" onclick="toggleNoteDetails(this); return false;" aormeetingnotesid="' + row.AORMeetingNotesID + '" style="cursor: pointer;" />&nbsp;(' + row.NoteDetailCount + ')</td><td>';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        if (row.Included == 1) {
                            nHTML += '<a href="" onclick="toggleNote(' + row.AORMeetingNotesID + ', 0, 1, ' + (extdata.indexOf('<agendaitemkey>') != -1 ? 1 : 0) + '); return false;" style="color: blue; padding-right: 10px;">Remove</a>';
                        }
                        else if (row.Included == 0) {
                            nHTML += '<a href="" onclick="toggleNote(' + row.AORMeetingNotesID + ', 1, 1, 0); return false;" style="color: blue; padding-right: 10px;">Re-Add</a>';
                        }
                    }

                    nHTML += row.AORNoteTypeName + ':</td>';
                    nHTML += '<td style="text-align: right; padding-right: 5px; border: none;">';

                    var excludedNoteTypes = ['Agenda/Objectives', 'Burndown Overview', 'Notes'];

                    if ($.inArray(row.AORNoteTypeName, excludedNoteTypes) == -1) {
                        nHTML += '<label><input type="checkbox" id="chkShowClosedNoteDetail' + row.AORMeetingNotesID + '" name="chkShowClosedNoteDetail" onchange="chkShowClosedNoteDetail_change(' + row.AORMeetingNotesID + ');"';

                        if (parent._checkedShowClosed != undefined) {
                            if ($.inArray(row.AORMeetingNotesID, parent._checkedShowClosed) != -1) nHTML += ' checked ';
                        }

                        nHTML += '/>Show Closed Note Details</label>&nbsp;&nbsp;';
                    }

                    nHTML += '<label><input type="checkbox" id="chkShowRemovedNoteDetail' + row.AORMeetingNotesID + '" name="chkShowRemovedNoteDetail" onchange="chkShowRemovedNoteDetail_change(' + row.AORMeetingNotesID + ');"';

                    if (parent._checkedShowRemoved != undefined) {
                        if ($.inArray(row.AORMeetingNotesID, parent._checkedShowRemoved) != -1) nHTML += ' checked ';
                    }

                    nHTML += '/>Show Removed Note Details</label>&nbsp;&nbsp;';
                    nHTML += '</td></tr></table></div></td></tr>';
                    nHTML += '<tr class="gridBody" style="display: none;">';
                    nHTML += '<div id="divAORMINotesDetail' + row.AORMeetingNotesID + '"></div>';
                    nHTML += '</td></tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORMINotes').html(nHTML);
            displayNoteDetails();
        }

        function getNotes_error() {
            $('#divAORMINotes').html('Error gathering data.');
        }

        function displayNoteDetails() {
            if (parent._expandedNotes != undefined) {
                $.each(parent._expandedNotes, function (index, item) {
                    var $img = $('#img' + item);

                    if ($img.attr('title') == 'Show') $img.trigger('click');
                });
            }
        }

        function toggleNote(AORMeetingNotesID, opt, blnNoteType, hasAgendaItemKey) {
            _optNote = opt;

            var agendaObjectiveSupplmentalText = '';

            if (hasAgendaItemKey == 1) {
                agendaObjectiveSupplmentalText = ' and delete it from the associated Agenda/Objective'
            }

            if (_optNote == 0) {
                QuestionBox('Confirm Note ' + (blnNoteType == 1 ? 'Breakout' : 'Detail') + ' Hide', 'Are you sure you would like to hide this note ' + (blnNoteType == 1 ? 'breakout' : 'detail') + agendaObjectiveSupplmentalText + '?', 'Yes,No', 'confirmToggleNote', 300, 300, this, AORMeetingNotesID);
            }
            else {
                QuestionBox('Confirm Note ' + (blnNoteType == 1 ? 'Breakout' : 'Detail') + ' Unhide', 'Are you sure you would like to unhide this note ' + (blnNoteType == 1 ? 'breakout' : 'detail') + '?', 'Yes,No', 'confirmToggleNote', 300, 300, this, AORMeetingNotesID);
            }
        }

        function confirmToggleNote(answer, AORMeetingNotesID) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                PageMethods.ToggleNote('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', AORMeetingNotesID, _optNote, function() { toggleNote_done(AORMeetingNotesID); }, toggleNote_error);
            }
        }

        function toggleNote_done(AORMeetingNotesID) {
            getSelectedNoteDetail(AORMeetingNotesID);
            getNotes();
            RefreshTreeView();
        }

        function toggleNote_error() {
            getNotes();
            MessageBox('An error has occurred.');
        }

        function toggleNoteDetails(obj) {
            var $obj = $(obj);
            var AORMeetingNotesID = $obj.attr('aormeetingnotesid');

            if ($obj.attr('title') == 'Show') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Hide')
                $obj.attr('alt', 'Hide')
                $obj.closest('table').parent().closest('tr').next().show();

                if ($('#divAORMINotesDetail' + AORMeetingNotesID).html() == '') getNotesDetail(AORMeetingNotesID);

                if (parent._expandedNotes != undefined) {
                    if ($.inArray(AORMeetingNotesID, parent._expandedNotes) == -1) parent._expandedNotes.push(AORMeetingNotesID);
                }
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Show')
                $obj.attr('alt', 'Show')
                $obj.closest('table').parent().closest('tr').next().hide();

                if (parent._expandedNotes != undefined) parent._expandedNotes.splice($.inArray(AORMeetingNotesID, parent._expandedNotes), 1);
            }
        }

        function chkShowRemovedNoteDetail_change(AORMeetingNotesID) {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on this note breakout will be lost. Would you like to proceed?', 'Yes,No', 'showRemoved', 300, 300, this, AORMeetingNotesID);
            }
            else {
                showRemoved('Yes', AORMeetingNotesID);
            }
        }

        function chkShowClosedNoteDetail_change(AORMeetingNotesID) {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on this note breakout will be lost. Would you like to proceed?', 'Yes,No', 'showClosed', 300, 300, this, AORMeetingNotesID);
            }
            else {
                showClosed('Yes', AORMeetingNotesID);
            }
        }

        function btnAddNoteDetail_click() {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost if a new note detail is added. Would you like to proceed?', 'Yes,No', 'openPopup', 300, 300, this, 'Note');
            }
            else {
                openPopup('Yes', 'Note');
            }
        }

        function btnAddNoteObjectives_click() {
            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on the Agenda tab will be lost if objectives are added or edited. Would you like to proceed?', 'Yes,No', 'openPopup', 300, 300, this, 'NoteObjectives');
            }
            else {
                var agendaObjectivesPresent = window[getNoteTreeViewClientID() + '_NodeHasChildren'](null, 'ao', 1);

                if (agendaObjectivesPresent) {
                    openPopup('Yes', 'EditObjectives');
                }
                else {
                    openPopup('Yes', 'NoteObjectives');
                }
            }
        }

        function editNoteDetail(AORMeetingNotesID, obj) {
            var type = 0;
            var tr = $(obj).closest('tr');
            var ddlType = tr.find('select[name=ddlNoteType]');
            if (ddlType.length > 0) { // read only views won't have a ddl
                type = ddlType.val();
            }

            AORMeetingNotesID += '.' + type;

            if ($('#btnSave').is(':enabled') && false) {
                QuestionBox('Confirmation', 'Unsaved changes on this note breakout will be lost if a note detail is updated. Would you like to proceed?', 'Yes,No', 'openPopup', 300, 300, this, AORMeetingNotesID);
            }
            else {
                if (_saveInProgress) {
                    _openNoteTriggered = AORMeetingNotesID += '.' + type;
                    return;
                }
                else {
                    _openNoteTriggered = 0;
                    openPopup('Yes', AORMeetingNotesID);
                }
            }
        }

        function getSelectedNoteDetail(AORMeetingNotesID, forceRefreshFromPopup) {
            if (forceRefreshFromPopup) {
                var displayedNoteCount = $('#divAORMINotesDetail').find('tr[aormeetingnotesid]').length;
                var burndownGridPresent = $('#burndowngridiframe').length > 0;
                if (displayedNoteCount > 1 || burndownGridPresent) {
                    // if we are showing more than one note, it means we are viewing burndown notes from clicking on an aor
                    // if this is the case, we refresh the entire aor click, not just the one note
                    var aorid = $($('#divAORMINotesDetail').find('tr[aormeetingnotesid]')[0]).attr('aorid');
                    if (aorid == null || aorid.length == 0) aorid = "-1";

                    if (aorid != null && aorid.length > 0) {
                        getNotesDetail(0, aorid, <%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%>, burndownGridPresent);
                        return;
                    }
                }
            }

            $('#divAORMINotesDetail').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" style="padding: 10px;" />');
            PageMethods.GetSelectedNoteDetail(AORMeetingNotesID, true, true, function (result) { getNotesDetail_done(result, AORMeetingNotesID); }, function () { getNotesDetail_error(AORMeetingNotesID); });
        }

        function getNotesDetail(AORMeetingNotesID, aorID, noteTypeID, showBurndownGrid) {
            if (aorID == null) aorID == 0;
            if (noteTypeID == null) noteTypeID == 0;

            $('#divAORMINotesDetail' + AORMeetingNotesID).html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" style="padding: 10px;" />');
            PageMethods.GetNotesDetail(AORMeetingNotesID, $('#chkShowRemovedNoteDetail' + AORMeetingNotesID).is(':checked'), $('#chkShowClosedNoteDetail' + AORMeetingNotesID).is(':checked'), aorID, noteTypeID, <%=AORMeetingInstanceID%>, function (result) { getNotesDetail_done(result, AORMeetingNotesID, aorID, showBurndownGrid); }, function () { getNotesDetail_error(AORMeetingNotesID); });
        }

        function getNotesDetail_done(result, AORMeetingNotesID, AORID, showBurndownGrid) {
            var nHTML = '';

            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                $('#divAORMINotesDetail').css('padding', '10px');
                nHTML = 'No Note Details';
            }
            else {
                $('#divAORMINotesDetail').css('padding', '');

                if (showBurndownGrid) {
                    var nURL = 'AOR_Grid.aspx?GridType=AOR&MyData=false&AORID_Filter_arr=' + AORID + '&GridPageSize=12&GridPageIndex=0&CurrentLevel=1&GridViewNameOverride=MeetingNoteAORGrid';
                    nHTML += '<iframe id="burndowngridiframe" src="' + nURL + '" width="100%" frameBorder="0"></iframe>';
                    nHTML += '<br />';
                }

                nHTML += '<table style="border-collapse: collapse; width: 100%;vertical-align:top">';
                nHTML += '<tr class="gridHeader">';

                if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                    nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 50px;"></th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 60px;">';
                }
                else {
                    nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 60px;">';
                }

                nHTML += 'Note #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 150px;">Note Type</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: left; padding-left: 10px;">Title</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 300px;">AOR Name</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 100px;">Date Added</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 65px;">Status</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 100px;">Status Date</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 40px;">Sort</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">History</th>';
                nHTML += '</tr>';

                var noteTypeOptions = '<%=NoteTypeOptions%>';

                $.each(dt, function (rowIndex, row) {
                    var extdata = row.ExtData;
                    if (extdata == null) extdata = '';

                    nHTML += '<tr class="gridBody" aormeetingnotesid="' + row.AORMeetingNotesID + '" aorid="' + row.AORID + '">';
                    nHTML += '<td style="border-left: 1px solid grey; text-align: center;">';
                    nHTML += '<input type="hidden" name="extdata" value="' + StrongEscape(extdata) + '">';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        if (row.Included == 1) {
                            nHTML += '<a href="" onclick="toggleNote(' + row.AORMeetingNotesID + ', 0, 0, ' + (extdata.indexOf('<agendaitemkey>') != -1 ? 1 : 0) + '); return false;" style="color: blue;">Remove</a>';
                        }
                        else if (row.Included == 0) {
                            nHTML += '<a href="" onclick="toggleNote(' + row.AORMeetingNotesID + ', 1, 0, 0); return false;" style="color: blue;">Re-Add</a>';
                        }

                        nHTML += '</td><td style="text-align: center;"><a href="" onclick="editNoteDetail(' + row.AORMeetingNotesID + ', this); return false;" style="color: blue;">' + row.AORMeetingNotesID + '</a></td>';
                    }
                    else {
                        nHTML += '<a href="" onclick="editNoteDetail(' + row.AORMeetingNotesID + ', this); return false;" style="color: blue;">' + row.AORMeetingNotesID + '</a></td>';
                    }

                    var selectedOption = row.AORNoteTypeName;
                    var nto = noteTypeOptions;

                    if (row.AORNoteTypeName == 'Agenda/Objectives') {
                        nto = '<option value="17" selected>Agenda/Objectives</option>';
                    }
                    else if (selectedOption.length > 0) {
                        nto = nto.replace('>' + selectedOption + '</option>', ' selected>' + selectedOption + '</option>');
                        nto = nto.replace('>Agenda/Objectives</option>', ' style="display:none">Agenda/Objectives</option>'); // users can no longer pick agenda/objectives direclty
                    }
                    else {
                        nto = nto.replace('>Agenda/Objectives</option>', ' style="display:none">Agenda/Objectives</option>'); // users can no longer pick agenda/objectives direclty
                    }

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.STATUS != 'Closed') {
                        nHTML += '<td style="text-align:center;"><select name="ddlNoteType" id="ddlNoteType" field="AOR Note Type" original_value="' + selectedOption + '" style="width:150px;">' + nto + '</select></td>';
                    }
                    else {
                        nHTML += '<td style="text-align:center;">' + selectedOption  + '</td>';
                    }

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.STATUS != 'Closed') {
                        if (row.Title == null) row.Title = '';
                        nHTML += '<td style="text-align: center;" name="notedetailtitlecol"><input type="text" value="' + row.Title.replace(/\"/g, '&quot;') + '" maxlength="150" aormeetingnotesid="' + row.AORMeetingNotesID + '" field="Title" style="width: 97%;" /></td>';
                    }
                    else {
                        nHTML += '<td name="notedetailtitlecol">' + row.Title + '</td>';
                    }

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        var nOptions = decodeURIComponent('<%=this.NoteAOROptions %>');

                        if (nOptions.indexOf('value="' + row.AORReleaseID + '"') == -1) {
                            var nText = '';
                            if (row.WorkloadAllocation == null) row.WorkloadAllocation = '';

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

                            nOptions = '<option value="' + row.AORReleaseID + '" selected>' + nText + row.AORID + ' - ' + row.AORName + '</option>' + nOptions;
                        }
                        else {
                            nOptions = nOptions.replace('value="' + row.AORReleaseID + '"', 'value="' + row.AORReleaseID + '" selected');
                        }

                        nHTML += '<td style="text-align: center;">';
                        nHTML += '<select aormeetingnotesid="' + row.AORMeetingNotesID + '" field="AOR Name" original_value="' + row.AORReleaseID + '" style="width: 97%; background-color: #F5F6CE;">' + nOptions + '</select></td>';
                    }
                    else {
                        nHTML += '<td>' + row.AORName + '</td>';
                    }

                    nHTML += '<td style="text-align: center;white-space:nowrap;">' + row.AddDateString + '</td>';

                    var excludedNoteTypes = ['Agenda/Objectives', 'Burndown Overview', 'Notes'];

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        var nOptions = decodeURIComponent('<%=this.NoteStatusOptions %>');

                        nOptions = nOptions.replace('value="' + row.STATUSID + '"', 'value="' + row.STATUSID + '" selected');
                        nHTML += '<td style="text-align: center;"><select ';

                        if ($.inArray(row.AORNoteTypeName, excludedNoteTypes) != -1) nHTML += 'disabled';

                        nHTML += ' aormeetingnotesid="' + row.AORMeetingNotesID + '" field="Status" original_value="' + row.STATUSID + '" style="width: 65px; background-color: #F5F6CE;">' + nOptions + '</select></td>';
                    }
                    else {
                        nHTML += '<td style="text-align: center;">' + row.STATUS + '</td>';
                    }

                    nHTML += '<td style="text-align: center;white-space:nowrap;">' + row.StatusDateString + '</td>';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<td style="text-align: center;"><input type="text" value="' + row.Sort + '" maxlength="5" aormeetingnotesid="' + row.AORMeetingNotesID + '" field="Sort" original_value="' + row.Sort + '" style="width: 35px; text-align: center;" /></td>';
                    }
                    else {
                        nHTML += '<td style="text-align: center;">' + row.Sort + '</td>';
                    }

                    nHTML += '<td style="text-align: center;"><img src="images/icons/newspaper.png" height="16" width="16" style="cursor:pointer;" onclick="getNotesDetailHistory(' + row.AORMeetingNotesID + ', ' + row.NoteGroupID + ', this);"></td>';

                    nHTML += '</tr>';

                    // note ext data
                    if (row.AORNoteTypeName == 'Action Items') {
                        if ((row.WORKITEMID != null && row.WORKITEMID != '') || (row.WORKITEM_TASKID != null && row.WORKITEM_TASKID != '')) {
                            nHTML += '<tr class="gridBody gridFullBorder">';
                            nHTML += '  <td colspan="10" style="text-align: left;padding:5px;padding-left:10px;">';
                            if (row.WORKITEMID != null && row.WORKITEMID != '') {
                                nHTML += '<span style="width:75px;display:inline-block;"><b>Task ID: </b></span>';
                                nHTML += '<span style="width:75px;display:inline-block;">' + row.WORKITEMID + '</span>';
                                nHTML += '<span>' + row.TaskTitle + '</span>';
                                nHTML += '<br>';
                            }

                            if (row.WORKITEMID != null && row.WORKITEMID != '' && row.WORKITEM_TASKID != null && row.WORKITEM_TASKID != '') {
                                nHTML += '<span style="width:75px;display:inline-block;"><b>Task #: </b></span>';
                                nHTML += '<span style="width:75px;display:inline-block;"><a href="#" onclick="openActionItemSubTask(' + row.WORKITEMID + ', ' + row.WORKITEM_TASKID + ')">' + row.WORKITEMID + '-' + row.TASK_NUMBER + '</a></span>';
                                nHTML += '<span>' + row.SubTaskTitle + '</span>';
                                nHTML += '<br>';
                                nHTML += '<span style="width:75px;display:inline-block;"><b>Status: </b></span>';
                                nHTML += '<span style="width:75px;display:inline-block;">' + row.TaskStatus + '</span>';
                                nHTML += '<br>';
                                nHTML += '<span style="width:75px;display:inline-block;"><b>Assigned: </b></span>';
                                nHTML += '<span style="width:150px;display:inline-block;">' + row.AssignedTo + '</span>';
                                nHTML += '<br>';
                                nHTML += '<span style="width:75px;display:inline-block;"><b>Comp %: </b></span>';
                                nHTML += '<span style="width:75px;display:inline-block;">' + row.COMPLETIONPERCENT + '%</span>';
                            }
                            nHTML += '  </td>';
                            nHTML += '</tr>';
                        }
                    }

                    //note detail
                    nHTML += '<tr class="gridBody">';

                    if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.STATUS != 'Closed') {
                        nHTML += '<td colspan="10" style="text-align: center;"><textarea class="editor" aormeetingnotesid="' + row.AORMeetingNotesID + '" field="Note Details" notestatus="' + row.STATUS + '" style="width: 99%;">' + row.Notes + '</textarea></td>';
                    }
                    else if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.STATUS == 'Closed') {
                        nHTML += '<td colspan="10" style="text-align: center;"><textarea class="editor" style="width: 99%; background-color: white; border: none;" readonly="readonly">' + row.Notes + '</textarea></td>';
                    }
                    else {
                        nHTML += '<td colspan="10" style="text-align: center;"><textarea class="editor" style="width: 99%; background-color: white; border: none;" readonly="readonly">' + row.Notes + '</textarea></td>';
                    }

                    nHTML += '</tr>';

                    //space
                    if (rowIndex != dt.length - 1) {
                        nHTML += '<tr class="gridBody">';

                        if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && row.STATUS != 'Closed') {
                            nHTML += '<td colspan="9" style="border-right: none; height: 20px;"></td>';
                        }
                        else {
                            nHTML += '<td colspan="8" style="border-right: none; height: 20px;"></td>';
                        }

                        nHTML += '</tr>';
                    }
                });

                nHTML += '</table>';
            }

            $('#divAORMINotesDetail').html(nHTML);
            $('#divAORMINotesDetail').show();
            $('#divAORMINotesDetail').find('select,input').on('blur', function () {
                input_blur();
            });


            $('#divAORMINotesDetail').find('textarea').each(function () {
                var obj = this;
                var tbl = $(obj).closest('table');
                var ddlNoteType = $(tbl).find('select[name=ddlNoteType]');
                var extdata = $(tbl).find('input[name=extdata]').val();

                var ddlNoteTypeSelectionIsWizardAgenda = ddlNoteType.length > 0 && $(ddlNoteType).val() == '<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>';
                // legacy agendas created before the objectives wizard still need to be editable
                if (ddlNoteTypeSelectionIsWizardAgenda) {
                    if (extdata == null || extdata.length == 0 || extdata.indexOf('[LT]objectivesjson[GT]') == -1) {
                        ddlNoteTypeSelectionIsWizardAgenda = false;
                    }
                }

                if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE' && $(this).attr('notestatus') != 'Closed' && !ddlNoteTypeSelectionIsWizardAgenda) {
                    $(this).cleditor({
                        controls:
                            'bold italic underline strikethrough subscript superscript | font size ' +
                            'style | color highlight removeformat | bullets numbering | outdent ' +
                            'indent | alignleft center alignright justify | undo redo | ' +
                            'rule image link unlink | cut copy paste pastetext | print',
                        bodyStyle: 'background-color: #F5F6CE; font-family: Arial; font-size: 12px;'
                    })[0].change(function () { input_change(this); resizeEditor(obj); });

                    $(this).cleditor().bind("blurred", function () { input_blur(); return false; })
                }
                else {
                    $(this).cleditor({
                        bodyStyle: 'font-family: Arial; font-size: 12px;'
                    })[0].disable(true);
                }

                resizeEditor(obj);
            });

            var detailHeight = $('#divAORMINotesDetail').height();
            if (detailHeight > 400) {
                $('#imgPageSpacerVertical1').show();
                $('#imgPageSpacerVertical2').hide();
                $('#imgPageSpacerVertical3').show();
            }
            else {
                $('#imgPageSpacerVertical1').hide();
                $('#imgPageSpacerVertical2').show();
                $('#imgPageSpacerVertical3').hide();
            }

            openNodeAndParent(AORMeetingNotesID);
        }

        function getNotesDetail_error(AORMeetingNotesID) {
            $('#divAORMINotesDetail').css('padding', '10px');
            $('#divAORMINotesDetail').html('Error gathering data.');
        }

        function openActionItemSubTask(taskID, subTaskID) {
            var nWindow = 'WorkloadSubTask';
            var nTitle = 'Subtask';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.TaskEdit;

            if (parseInt(taskID) > 0 && parseInt(subTaskID) > 0) {
                nTitle;
                nURL += '?workItemID=' + taskID + '&taskID=' + subTaskID + '&ReadOnly=true';

                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);
                if (openPopup) openPopup.Open();
            }
        }

        function resizeBurndownGridIFrame() {
            var frm = $('#burndowngridiframe');

            if (frm.length > 0) {
                var pc = frm.contents().find('#divPage');
                var bodyContainer = frm.contents().find('[id$=_grdData_BodyContainer]');
                var newHeight = 0;

                if (bodyContainer.length > 0) {
                    var bodyTableHeight = frm.contents().find('[id$=_grdData_BodyContainer] table').height();
                    var pagerHeight = frm.contents().find('[id$=_PagerContainer]').is(':visible') ? frm.contents().find('[id$=_PagerContainer]').height() : 0;
                    newHeight = bodyTableHeight + pagerHeight;
                }

                if (newHeight == 0 && pc.length > 0) {
                    var pcfrm = pc.find('iframe');

                    if (pcfrm.length > 0) {
                        newHeight = pcfrm.height();
                    }
                    else {
                        newHeight = pc.height();
                    }
                }

                $(frm).height(newHeight + 50);
            }
        }

        function resizeFrame() {
            resizeBurndownGridIFrame();
        }

        function showFrameForEdit(var1, var2, recordId) {
            // this is a hack for now; the aor grid doesn't launch tasks correctly when embedded on this page, so we use the page-level openTask function instead
            openTask(recordId);
        }

        function getNotesDetailHistory(AORMeetingNotesID, NoteGroupID, img) {
            var row = $(img).closest('tr');
            var txtTitle = $(row).find('input[field=Title]');
            var tdTitle = txtTitle.length == 0 ? $(row).find('td[name=notedetailtitlecol]') : null;

            var title = txtTitle.length > 0 ? txtTitle.val() : tdTitle.text();

            var nWindow = 'NoteDetailHistory';
            var nTitle = 'Note #' + AORMeetingNotesID + (title != null && title.length > 0 ? ' - ' + title : '');
            var nHeight = 400, nWidth = 1200;
            var nURL = null;

            if (_lastNotesDetailHistoryGroupID != NoteGroupID) {
                $('#divAORMINotesDetailHistory').html('Loading...');
                _lastNotesDetailHistoryGroupID = NoteGroupID;

                PageMethods.LoadNotesDetailHistory(NoteGroupID, <%=AORMeetingInstanceID%>, getNotesDetailHistory_done, on_error);
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, 'divAORMINotesDetailHistory');
            if (openPopup) openPopup.Open();
        }

        function getNotesDetailHistory_done(result) {
            var thePopup = popupManager.GetPopupByName('NoteDetailHistory');

            var dt = jQuery.parseJSON(result);

            if (dt.length > 0) {
                var firstNote = dt[0];
                var secondNote = dt.length > 1 ? dt[1] : null;

                var nHTML = '';

                if (firstNote.AORNoteTypeID == "<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>") {
                    nHTML += '<div style="text-align:center;color:#4a65a8;font-weight:bold;font-size:16px;width:100%;margin-top:5px;margin-bottom:2px;">Current/Previous Meeting</div>';
                    nHTML += '<table width="100%" cellpadding="1" cellspacing="0" style="border-collapse:collapse">';
                    nHTML += '  <thead>';
                    nHTML += '    <tr class="gridHeader">';
                    nHTML += '      <th colspan="2" width="50%" style="text-align:left;">Current Meeting Instance (#' + firstNote.AORMeetingInstanceID + ' - ' + firstNote.InstanceDateString + ')</th>';
                    nHTML += '      <th colspan="2" width="50%" style="text-align:left;">Previous Meeting Instance (' + (secondNote != null ? '#' + secondNote.AORMeetingInstanceID + ' - ' + secondNote.InstanceDateString : 'N/A') + ')</th>';
                    nHTML += '    </tr>';
                    nHTML += '    <tr class="gridHeader">';
                    nHTML += '      <th style="width:105px;text-align:left;padding-right:5px;white-space:nowrap;">Note #</th>';
                    nHTML += '      <th style="text-align:left;width:50%;">Notes</th>';
                    nHTML += '      <th style="width:105px;text-align:left;padding-right:5px;white-space:nowrap;">Note #</th>';
                    nHTML += '      <th style="text-align:left;width:50%;">Notes</th>';
                    nHTML += '    </tr>';
                    nHTML += '  </thead>';

                    nHTML += '  <tbody>';
                    nHTML += '    <tr class="gridBody">';
                    nHTML += '      <td style="width:105px;text-align:left;padding-right:5px;vertical-align:top;white-space:nowrap;"><b>' + firstNote.AORMeetingNotesID + '</b><br />' + firstNote.UpdatedDateString + '<br />' + firstNote.UpdatedBy + '</td>';
                    nHTML += '      <td style="text-align:left;vertical-align:top;width:50%;">' + firstNote.Notes + '</td>';

                    if (secondNote != null) {
                        nHTML += '      <td style="width:105px;text-align:left;padding-right:5px;vertical-align:top;white-space:nowrap;"><b>' + secondNote.AORMeetingNotesID + '</b><br />' + secondNote.UpdatedDateString + '<br />' + secondNote.UpdatedBy;
                        if (secondNote.MeetingAccepted) {
                            nHTML += '<br /><br /><div style="width:100%;text-align:center;padding-bottom:5px;"><input type="button" name="MeetingAcceptedButton" value="Meeting Accepted" disabled style="cursor:default;border:0px;background-color:#bbbbbb;color:#ffffff;border-radius:5px;1.25em"></div>';
                        }
                        else {
                            nHTML += '<br /><br /><div style="width:100%;text-align:center;padding-bottom:5px;"><input type="button" name="AcceptMeetingButton" value="Accept Meeting" style="cursor:pointer;border:0px;background-color:#5cb85c;color:#ffffff;border-radius:5px;1.25em" onclick="popupManager.GetPopupByName(\'NoteDetailHistory\').Opener.confirmAcceptMeeting(' + secondNote.AORMeetingInstanceID + ', true); return false;"></div>';
                        }
                        nHTML += '      </td>';
                        nHTML += '      <td style="text-align:left;vertical-align:top;width:50%;">' + secondNote.Notes + '</td>';
                    }
                    else {
                        nHTML += '      <td style="width:105px;text-align:left;padding-right:5px;vertical-align:top;white-space:nowrap;">N/A&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>';
                        nHTML += '      <td style="text-align:left;vertical-align:top;width:50%;">N/A</td>';
                    }
                    nHTML += '    </tr>';
                    nHTML += '  </tbody>';

                    nHTML += '</table>';

                    nHTML += '<hr />';
                }

                nHTML += '<div style="text-align:center;color:#4a65a8;font-weight:bold;font-size:16px;width:100%;margin-top:5px;margin-bottom:2px;">Note History</div>';
                nHTML += '<table width="100%" cellpadding="1" cellspacing="0" style="border-collapse:collapse">';

                nHTML += '<thead>';
                nHTML += '  <tr class="gridHeader">';
                nHTML += '    <th style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;">Note #</th>';
                nHTML += '    <th style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;">Mtg #</th>';
                nHTML += '    <th style="width:1%;white-space:nowrap;width:100px;text-align:center;padding-right:5px;">Updated</th>';
                nHTML += '    <th style="width:1%;white-space:nowrap;width:100px;text-align:center;padding-right:5px;">Updated By</th>';
                nHTML += '    <th style="width:1%;white-space:nowrap;text-align:left;padding-right:5px;">Title</th>';
                nHTML += '    <th style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;">Status</th>';
                nHTML += '    <th style="width:99%;text-align:left;">Notes</th>';
                nHTML += '  </tr>';
                nHTML += '</thead>';

                nHTML += '<tbody>';

                var lastNote = null;

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';

                    nHTML += '    <td style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;vertical-align:top;">' + (rowIndex == 0 ? '<b>' : '') + row.AORMeetingNotesID + (rowIndex == 0 ? '</b>' : '') + '</td>';
                    nHTML += '    <td style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;vertical-align:top;">' + (rowIndex == 0 ? '<b>' : '') + row.AORMeetingInstanceID + (rowIndex == 0 ? '</b>' : '') + '</td>';
                    nHTML += '    <td style="width:1%;white-space:nowrap;width:100px;text-align:center;padding-right:5px;vertical-align:top;">' + row.UpdatedDateString + '</td>';
                    nHTML += '    <td style="width:1%;white-space:nowrap;width:100px;text-align:center;padding-right:5px;vertical-align:top;">' + row.UpdatedBy + '</td>';
                    nHTML += '    <td style="width:1%;text-align:left;padding-right:5px;vertical-align:top;"><div style="width:200px">' + row.Title + '</div></td>';
                    nHTML += '    <td style="width:1%;white-space:nowrap;width:75px;text-align:center;padding-right:5px;vertical-align:top;">' + row.STATUS + '</td>';

                    var note = row.Notes;

                    if (note == lastNote) {
                        note = '[SAME]';
                    }
                    else {
                        lastNote = note;
                    }

                    nHTML += '    <td style="wwidth:99%;text-align:left;vertical-align:top;">' + note + '</td>';

                    nHTML += '</tr>';
                });
                nHTML += '</tbody>';

                nHTML += '</table>';

                $(thePopup.Body).children('#divAORMINotesDetailHistory').html(nHTML);
            }
            else {
                $(thePopup.Body).children('#divAORMINotesDetailHistory').html('No results found.');
            }
        }

        function getAORProgress() {
            $('#divAORProgress').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetAORProgress('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', getAORProgress_done, getAORProgress_error);
        }

        function getAORProgress_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                //nHTML = 'No AORs';
            }
            else {
                $.each(dt, function (rowIndex, row) {
                    if (rowIndex > 0) nHTML += '<br><br>';

                    nHTML += '<table>';
                    nHTML += '<tr><td colspan="7"><b>AOR Name:</b>&nbsp;&nbsp;' + row.AOR + '</td></tr>';
                    nHTML += '<tr><td colspan="7"><b>Threshold Met:</b>&nbsp;&nbsp;' + row.ThresholdMet + '</td></tr>';
                    nHTML += '<tr><td colspan="7">&nbsp;</td></tr>';
                    nHTML += '<tr><td style="width:25px;"></td><td colspan="2" style="text-align: center;"><b>Design</b></td><td style="width:25px;"></td><td colspan="2" style="text-align: center;"><b>Develop</b></td><td></td></tr>';
                    nHTML += '<tr><td>&nbsp;</td><td>Exit Criteria Met:</td><td>' + row.ExitCriteriaMet + '</td><td>&nbsp;</td><td>Entrance Criteria Met:</td><td>' + row.EntranceCriteriaMet + '</td><td></td></tr>';
                    nHTML += '<tr><td>&nbsp;</td><td style="width: 1px;">Exit&nbsp;Criteria&nbsp;Open:</td><td style="width: 1px;">' + row.ExitCriteriaOpen + '</td><td>&nbsp;</td><td style="width: 1px;">Entrance&nbsp;Criteria&nbsp;Open:</td><td style="width: 1px;">' + row.EntranceCriteriaOpen + '</td><td></td></tr>';
                    nHTML += '<tr><td>&nbsp;</td><td>Exit Criteria N/A:</td><td>' + row.ExitCriteriaNA + '</td><td>&nbsp;</td><td>Entrance Criteria N/A:</td><td>' + row.EntranceCriteriaNA + '</td><td></td></tr>';
                    nHTML += '</table>';
                });
            }

            $('#divAORProgress').html(nHTML);
        }

        function getAORProgress_error() {
            $('#divAORProgress').html('Error gathering data.');
        }

        function getHistory() {
            $('#divAORMIHistory').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetHistory('<%=this.AORMeetingID %>', '<%=this.AORMeetingInstanceID %>', getHistory_done, getHistory_error);
        }

        function getHistory_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No History';
            }
            else {
                nHTML += '<table style="border-collapse: collapse;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;">Meeting Date</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">View</th>';
                nHTML += '<th style="border-top: 1px solid grey;">Meeting Name</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Accepted</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Edit</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">Minutes</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    var m = new moment(new Date(row.InstanceDateString));

                    nHTML += '<tr class="gridBody" key="' + row.AORMeetingInstanceID + '">';
                    nHTML += '<td name="meetingdatecolumn" style="border-left: 1px solid grey; text-align: center;">' + row.InstanceDateString + '</td>';
                    nHTML += '<td style="text-align: center;">';
                    nHTML += '<img src="images/icons/find.png" onclick="openAORMeetingInstance(' + '<%=this.AORMeetingID %>' + ', ' + row.AORMeetingInstanceID + '); return false;" style="cursor:pointer;" alt="View Meeting Instance #' + row.AORMeetingInstanceID + '" title="View Meeting Instance #' + row.AORMeetingInstanceID + '">';
                    nHTML += '</td>';
                    nHTML += '<td>' + row.AORMeetingInstanceName + '</td>';
                    nHTML += '<td style="text-align: center;" name="meetingacceptancecolumn" key="' + row.AORMeetingInstanceID + '">';
                    if ('<%=UserManagement.UserCanEdit(WTSModuleOption.Meeting)%>'.toUpperCase() == 'TRUE') {
                        nHTML += (row.MeetingAccepted ? '<img src="Images/Icons/check.png" width="15" height="15" alt="This meeting has been accepted" title="This meeting has been accepted">' : '<a href="" onclick="confirmAcceptMeeting(' + row.AORMeetingInstanceID + '); return false;" style="color: blue;">Accept</a>');
                    }
                    else if (row.MeetingAccepted) {
                        nHTML += '<img src="Images/Icons/check.png" width="15" height="15" alt="This meeting has been accepted" title="This meeting has been accepted">';
                    }
                    nHTML += '</td>';
                    nHTML += '<td style="text-align: center;" name="meetingeditcolumn" key="' + row.AORMeetingInstanceID + '">';
                    if ('<%=UserManagement.UserCanEdit(WTSModuleOption.Meeting)%>'.toUpperCase() == 'TRUE') {
                        nHTML += '<img src="images/icons/pencil.png" onclick="openAORMeetingInstance(' + '<%=this.AORMeetingID %>' + ', ' + row.AORMeetingInstanceID + ', true); return false;" style="width:16px;height:16px;cursor:pointer;" alt="Edit Meeting Instance #' + row.AORMeetingInstanceID + '" title="Edit Meeting Instance #' + row.AORMeetingInstanceID + '">';
                    }
                    nHTML += '</td>';
                    nHTML += '<td style="text-align: center;" name="viewminutescolumn" key="' + row.AORMeetingInstanceID + '" attachmentid="' + (row.LastMeetingMinutesDocumentID != null && row.LastMeetingMinutesDocumentID > 0 ? row.LastMeetingMinutesDocumentID : '') + '">';
                    nHTML += '<img src="images/icons/pdf_16.png" style="width:16px;height:16px;cursor:pointer;" onclick="openMeetingInstanceAttachment(' + row.LastMeetingMinutesDocumentID + ', ' + row.AORMeetingInstanceID + ', \'generateifempty\', \'' + m.format('MM/DD/YYYY') + '\', ' + !row.MeetingAccepted + '); return false;" alt="Download" title="Download">';
                    nHTML += '</td>';
                    nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORMIHistory').html(nHTML);
        }

        function getHistory_error() {
            $('#divAORMIHistory').html('Error gathering data.');
        }

        function openMeetingInstanceAttachment(attachmentId, AORMeetingInstanceID, mode, dt, showAcceptButton) {
            try {
                if (attachmentId == null || attachmentId == 0) {
                    if (mode == 'generateifempty') {
                        if (AORMeetingInstanceID != 0) {
                            infoMessage('Generating PDF');
                            PageMethods.RegenerateMeetingMinutes(<%=AORMeetingID%>, AORMeetingInstanceID, function (result) { regenerateMeetingMinutes_done(result, true, AORMeetingInstanceID, dt, showAcceptButton) }, on_error);
                        }
                    }
                }
                else {
                    showMeetingMinutesInPDFViewer(attachmentId, AORMeetingInstanceID, dt, showAcceptButton);
                }
            }
			catch(e){ }
        }

        function regenerateMeetingMinutes_done(result, openAttachment, AORMeetingInstanceID, dt, showAcceptButton) {
            var dt = jQuery.parseJSON(result);

            if (dt.newattachmentid) {
                if (openAttachment) {
                    showMeetingMinutesInPDFViewer(dt.newattachmentid, AORMeetingInstanceID, '', showAcceptButton);

                    var td = $('[name=viewminutescolumn][key=' + AORMeetingInstanceID + ']');
                    td.attr('attachmentid', dt.newattachmentid);
                    td.html('<img src="Images/Icons/pdf_16.png" width="15" height="15" alt="Download" Title="Download" onclick="openMeetingInstanceAttachment(' + dt.newattachmentid + ');" style="cursor:pointer;">');
                }
            }
        }

        function confirmAcceptMeeting(AORMeetingInstanceID, fromNoteHistoryDialog) {
            QuestionBox('Confirmation', 'Accept Meeting Instance ' + AORMeetingInstanceID + '?', 'Yes,No', 'meetingAcceptanceConfirmed', 300, 250, this, <%=AORMeetingID%> + '_' + AORMeetingInstanceID + (fromNoteHistoryDialog ? '_true' : '_false'));
        }

        function meetingAcceptanceConfirmed(answer, AORMeetingInstanceTokens) {
            if (answer != null && answer.toLowerCase() == "yes") {
                var tokens = AORMeetingInstanceTokens.split('_');
                var close = tokens.length == 4 && tokens[3] == 'CLOSE';
                PageMethods.AcceptMeetingInstance(tokens[0], tokens[1], tokens[2], function (result) { meetingInstanceAcceptance_done(result, close) }, on_error);
            }
        }

        function meetingInstanceAcceptance_done(result, closeWindow) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt.success) {
                var AORMeetingInstanceID = dt.AORMeetingInstanceID;
                var fromNoteHistoryDialog = dt.fromnotehistorydialog.toLowerCase();

                if (closeWindow) {
                    var td = $('[name=meetingacceptancecolumn][key=' + AORMeetingInstanceID + ']', opener);
                    td.html('<img src="Images/Icons/check.png" width="15" height="15" alt="This meeting has been accepted" title="This meeting has been accepted">');

                    btnClose_click();
                }
                else if (fromNoteHistoryDialog == "true") {
                    // at this point, the popup is still displaying, so we have to get it through the popupmanager
                    var pm = popupManager.GetPopupByName('NoteDetailHistory');
                    var btn = pm.InlineContainer.find('input[name=AcceptMeetingButton]');

                    // update the button
                    btn.attr('value', 'Meeting Accepted');
                    btn.attr('disabled', '1');
                    btn.css('background-color', '#bbbbbb');
                    btn.css('cursor', 'default');

                    // update the history tab
                    var ht = $('#divAORMIHistory');
                    var td = $('[name=meetingacceptancecolumn][key=' + AORMeetingInstanceID + ']');
                    td.html('<img src="Images/Icons/check.png" width="15" height="15" alt="This meeting has been accepted" title="This meeting has been accepted">');

                    if (dt.newattachmentid > 0) {
                        td = $('[name=viewminutescolumn][key=' + AORMeetingInstanceID + ']');
                        td.attr('attachmentid', dt.newattachmentid);
                        td.html('<img src="Images/Icons/pdf_16.png" width="15" height="15" alt="Download" Title="Download" onclick="openMeetingInstanceAttachment(' + dt.newattachmentid + ', ' + AORMeetingInstanceID + ', \'\', \'date\', false);" style="cursor:pointer;">');
                    }
                }
                else {
                    var td = $('[name=meetingacceptancecolumn][key=' + AORMeetingInstanceID + ']');
                    td.html('<img src="Images/Icons/check.png" width="15" height="15" alt="This meeting has been accepted" title="This meeting has been accepted">');

                    if (dt.newattachmentid > 0) {
                        td = $('[name=viewminutescolumn][key=' + AORMeetingInstanceID + ']');
                        td.attr('attachmentid', dt.newattachmentid);
                        td.html('<img src="Images/Icons/pdf_16.png" width="15" height="15" alt="Download" Title="Download" onclick="openMeetingInstanceAttachment(' + dt.newattachmentid + ', ' + AORMeetingInstanceID + ', \'\', \'date\', false);" style="cursor:pointer;">');
                    }
                }

                successMessage('Meeting accepted');
                _lastNotesDetailHistoryGroupID = -1; // force refresh of any note history so the accept column is accurate next time it's loaded

                if (AORMeetingInstanceID == _previousMeetingInstanceID) {
                    $('#divPreviousMeetingAccepted').hide();
                    destroyNotificationOverlay();
                }
            }
        }

        function loadAttachments() {
            var url = window.location.search;
            url = editQueryStringValue(url, 'Saved', '0');
            url = 'Loading.aspx?Page=AOR_Meeting_Instance_Attachments.aspx' + url;

            $('#<%=this.frameAttachments.ClientID%>').attr('src', url);

        }

        function SetAttachmentQty(qty) {
            if (!qty) {
                qty = 0;
            }

            $('#divTabsContainer').find('li[aria-controls=divAttachments]').find('a').text('Attachments (' + qty + ')');
        }

        function getMeetingMetrics() {
            PageMethods.GetMeetingMetrics(<%=AORMeetingInstanceID%>,  getMeetingMetrics_done, on_error);
        }

        function getMeetingMetrics_done(result) {
            var dt = jQuery.parseJSON(result);

            $('#divmetricsloading').hide();

            if (dt.success == "true") {
                $('#divMetricsTotalMeetings').html(dt.totalmeetings);
                $('#divMetricsAvgLength').html(dt.avglength);
                $('#divMetricsAvgAttendedCount').html(dt.avgattendedcount);
                $('#divMetricsAvgResourcesCount').html(dt.avgresourcescount);
                $('#divMetricsAvgAttendedPct').html(dt.avgattendedpct + ' %');
                $('#divMetricsMaxAttendedPct').html(dt.maxattendedpct + ' %');

                for (var i = 10; i <= 17; i++) {
                    var noteData = dt['notetype_' + i];

                    if (noteData != null) {
                        var noteDataTokens = noteData.split('_');

                        $('#divMetricsTotal_' + i).html(noteDataTokens[0]);
                    }
                }

                $('#divlastmeetingtitle').html('Last Meeting (' + (dt.lastmeetingid > 0 ? dt.lastmeetingid : 'N/A') + ')');
                $('#divlastmeetingnew').html(dt.lastmeetingid > 0 ? dt.newitemslastmeeting : 'N/A');
                $('#divlastmeetingall').html(dt.lastmeetingid > 0 ? dt.lastallnotes : 'N/A');
                $('#divlastmeetingopen').html(dt.lastmeetingid > 0 ? dt.lastopennotes : 'N/A');
                $('#divlastmeetingclosed').html(dt.lastmeetingid > 0 ? dt.lastclosednotes : 'N/A');

                $('#divsecondtolastmeetingtitle').html('Previous Last Meeting (' + (dt.secondtolastmeetingid > 0 ? dt.secondtolastmeetingid : 'N/A') + ')');
                $('#divsecondtolastmeetingnew').html(dt.secondtolastmeetingid > 0 ? dt.newitemssecondtolastmeeting : 'N/A');
                $('#divsecondtolastmeetingall').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastallnotes : 'N/A');
                $('#divsecondtolastmeetingopen').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastopennotes : 'N/A');
                $('#divsecondtolastmeetingclosed').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastclosednotes : 'N/A');

                $('#divmetricscontainer').css('opacity', '1.0');
            }
            else {
                $('#divmetricsfailed').show();
            }
        }

        function validateInstanceDate_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt.success == 'true') {
                $('#divInstanceDateConflict').css('display', 'none');
            }
            else {
                if (dt.ConflictingAORMeetingInstanceID != null && dt.ConflictingAORMeetingInstanceID != '') {
                    $('#divInstanceDateConflict').html('&nbsp;*Invalid date. This date conflicts with another occurrence of the same meeting (<a href="javascript:openAORMeetingInstance(' + dt.AORMeetingID + ', ' + dt.ConflictingAORMeetingInstanceID + ');" style="color:red">Meeting Occurrence #' + dt.ConflictingAORMeetingInstanceID + '</a>).</div>');
                    $('#divInstanceDateConflict').css('display', 'inline');
                }
                else {
                    dangerMessage('Unable to validate date. Invalid date.');
                }
            }
        }

        function txtAORMeetingInstanceName_changed() {
            var mtgName = $('[id$=txtAORMeetingInstanceName]').val();
            if (mtgName != null) {
                var testName = mtgName.toLowerCase();
                if (s.endsWith(testName, ' meeting')) {
                    mtgName = mtgName.substring(0, mtgName.length - 8);
                }
                else if (s.endsWith(testName, 'meeting')) {
                    mtgName = mtgName.substring(0, mtgName.length - 7);
                }
            }
            $('#txtEmailCustomNote').val('Good afternoon all, thank you to those who attended today\'s' + (mtgName != null && mtgName.length > 0 ? (' ' + mtgName) : '') + ' Meeting.  Please find the attached Meeting Minutes and Burndown Overview for your reference.');
        }

        function refreshExistingMeetings() {
            if (_parsedExistingMeetings == null) {
                return;
            }

            var search = $('#txtMeetingSearch').val().toLowerCase();
            var sort = $('[name=rdExistingMeetingSort][value=name]').prop('checked') ? 'name' : $('[name=rdExistingMeetingSort][value=last]').prop('checked') ? 'last' : 'week';
            var dir = $('[name=rdExistingMeetingSortDirection][value=asc]').prop('checked') ? 'asc' : 'desc';

            var html = '';

            var mtgsToDisplay = _.filter(_parsedExistingMeetings, function (mtg) { return mtg.AORMeetingNameLower.indexOf(search) != -1 });

            if (sort == 'name') {
                mtgsToDisplay = _.sortBy(mtgsToDisplay, function (mtg) { return mtg.AORMeetingNameLower });
            }
            else if (sort == 'last') {
                mtgsToDisplay = _.sortBy(mtgsToDisplay, function (mtg) { return mtg.LastMeetingParsed });
            }
            else if (sort == 'week') {
                mtgsToDisplay = _.sortBy(mtgsToDisplay, function (mtg) { return mtg.Week + '_' + (mtg.LastMeetingParsed != null ? mtg.LastMeetingParsed.getTime() : mtg.AORMeetingNameLower) });
            }

            if (dir == 'desc') {
                mtgsToDisplay = mtgsToDisplay.reverse();
            }

            var lastWeekDisplayed = -1;
            var weekAlt = false;

            for (var i = 0; i < mtgsToDisplay.length; i++) {
                var mtg = mtgsToDisplay[i];

                if (sort == 'week' && mtg.Week != lastWeekDisplayed) {
                    var weekStr = _darr[mtg.Week];
                    lastWeekDisplayed = mtg.Week;
                    weekAlt = !weekAlt;

                    html += '<div class="existingmeetingsepouter"><div class="existingmeetingsepinner">';
                    html += weekStr != null ? weekStr : 'NOT SCHEDULED';
                    html += '</div></div>';
                }

                var row = parseInt(i / 5) + 1;
                var altFormatting = sort == 'week' ? weekAlt : (row % 2 == 1);

                if (i == 25) {
                    html += '<div id="divExistingMeetingListExpandButton" class="existingmeetingdivouter existingmeetingdivouterutility" onclick="showExpandedExistingMeetingList();"><div class="existingmeetingdivinner">';
                    html += '<b>' + (mtgsToDisplay.length - 25) + ' More Meetings Found...</b><br />';
                    html += '(click to show more)';
                    html += '</div></div>';

                    html += '<div id="divExistingMeetingsExpanded" style="width:100%;display:none;">';
                }

                html += '<div class="existingmeetingdivouter' + (altFormatting ? ' existingmeetingdivouteralt' : '') + (mtg.Selected ? ' existingmeetingdivouterselected' : '') + '" onclick="existingMeetingSelected(this, ' + mtg.AORMeetingID + ');" title="' + (mtg.Description != null && mtg.Description.length > 0 ? mtg.Description.replace('\'', '').replace('\"', '') : 'NO DESCRIPTION') + '"><div class="existingmeetingdivinner">';

                html += '<span style="font-size:larger;"><b>' + mtg.AORMeetingName + '</b></span><br />';
                html += 'Meeting #' + mtg.AORMeetingID;

                if (mtg.Frequency != null) {
                    html += '<div style="width:25px;height:25px;position:absolute;right:-5px;bottom:-10px;">' + mtg.Frequency + '</div>';
                }

                if (mtg.LastMeeting != null) {
                    html += '<br />' + mtg.LastMeeting;
                    html += ' (' + mtg.NumberOfInstances + ')';
                }
                else {
                    html += '<br />No Occurrences';
                }

                html += '<div style="width:25px;height:25px;position:absolute;left:-5px;top:2px;opacity:.5;">' + (i + 1) + '</div>';

                html += '</div></div>';
            }

            if (mtgsToDisplay.length > 25) {
                var row = parseInt(mtgsToDisplay.length / 5) + 1;
                var altFormatting = (row % 2 == 1);
                html += '<div id="divExistingMeetingListCollapseButton" class="existingmeetingdivouter existingmeetingdivouterutility" onclick="collapseExpandedExistingMeetingList();" style="margin-bottom:100px;"><div class="existingmeetingdivinner">';
                html += '<b>' + mtgsToDisplay.length + ' Meetings Found...</b><br />';
                html += '(click to show less)';
                html += '</div></div>';

                html += '</div>';
            }

            $('#divExistingMeetings').html(html);

        }

        function showExpandedExistingMeetingList() {
            $('#divExistingMeetingsExpanded').show();
            $('#divExistingMeetingListExpandButton').hide();
        }

        function collapseExpandedExistingMeetingList() {
            $('#divExistingMeetingsExpanded').hide();
            $('#divExistingMeetingListExpandButton').show();
        }

        function existingMeetingSelected(div, mtgID) {
            var mtg = getExistingMeetingByID(mtgID);

            if (mtg.Selected) {
                mtg.Selected = false;
                $('[id$=txtAORMeetingInstanceName]').val('');
            }
            else {
                _.each(_parsedExistingMeetings, function (mtg) { mtg.Selected = false });
                mtg.Selected = true;

                $('[id$=txtAORMeetingInstanceName]').val(mtg.AORMeetingName);
            }

            PageMethods.ValidateInstanceDate(mtg.AORMeetingID, 0, $('#<%=this.txtInstanceDate.ClientID %>').val(), validateInstanceDate_done, on_error);

            refreshExistingMeetings();
        }

        function getSelectedExistingMeeting() {
            return _.find(_parsedExistingMeetings, function (mtg) { return mtg.Selected });
        }

        function getExistingMeetingByID(mtgID) {
            return _.find(_parsedExistingMeetings, function (mtg) { return mtg.AORMeetingID == mtgID });
        }

        function loadExistingMeetings() {
            PageMethods.LoadExistingMeetings(loadExistingMeetings_done, on_error);
        }

        function loadExistingMeetings_done(result) {
            var dt = $.parseJSON(result);

            _darr = getDayArray();

            _parsedExistingMeetings = [];

            for (var i = 0; dt != null && i < dt.length; i++) {
                var row = dt[i];

                var mtg = {};

                mtg.AORMeetingID = row['AOR Meeting #'];
                mtg.AORMeetingName = row['AOR Meeting Name'];
                mtg.AORMeetingNameLower = mtg.AORMeetingName.toLowerCase();
                mtg.Description = row['Description'];

                if (row['Last Meeting'] != null && row['Last Meeting'].length > 0) {
                    mtg.LastMeeting = row['Last Meeting'];
                    mtg.LastMeetingParsed = mtg.LastMeeting != null ? new Date(mtg.LastMeeting) : null;
                    mtg.LastMeeting = moment(mtg.LastMeetingParsed).format('MM/DD/YYYY hh:mm A');
                    mtg.Week = _darr[moment(mtg.LastMeetingParsed).format('MM/DD/YYYY')];
                }
                else {
                    mtg.LastMeeting = null;
                    mtg.LastMeetingParsed = null;
                    mtg.Week = 0;
                }

                mtg.Selected = false;


                mtg.NumberOfInstances = row['# of Meeting Instances'];
                mtg.Frequency = row['AORFrequencyName'];
                if (mtg.Frequency != null && mtg.Frequency.length > 0) {
                    mtg.Frequency = mtg.Frequency.substring(0, 1).toUpperCase();
                }

                _parsedExistingMeetings.push(mtg);
            }

            if (<%=this.AORMeetingID%> > 0) {
                var mtg = getExistingMeetingByID(<%=this.AORMeetingID%>);
                mtg.Selected = true;
                $('#txtMeetingSearch').val(mtg.AORMeetingName);
            }

            refreshExistingMeetings();
        }

        function getDayArray() {
            var darr = [];

            var dt = new moment(new Date());

            var minDt = new moment('2017-01-01');
            var week = 100000;
            // start dt on a saturday (last day of the week)
            do {
                var day = dt.day();
                if (day == 0) {
                    break;
                }
                else {
                    dt.add(1, 'd');
                }
            } while (1==1);

            var day = 0;
            darr[week] = moment(dt).subtract(6, 'd').format('MM/DD/YYYY') + ' - ' + dt.format('MM/DD/YYYY');

            do {
                if (day == 0) {
                    week--;
                    darr[week] = moment(dt).subtract(6, 'd').format('MM/DD/YYYY') + ' - ' + dt.format('MM/DD/YYYY');
                }

                darr[dt.format('MM/DD/YYYY')] = week;

                dt.subtract(1, 'd');

                if (day == 0) {
                    day = 6;
                }
                else {
                    day--;
                }

                if (dt.isBefore(minDt)) {
                    break;
                }
            } while (1 == 1);

            return darr;
        }

        function btnStartMeeting_click() {
            btnSave_click();
        }

        function btnAcceptMeeting_click() {
            meetingAcceptanceConfirmed('Yes', <%=AORMeetingID%> + '_' + <%=AORMeetingInstanceID%> + '_false_CLOSE');
        }

        function showPreviousMeetingNotAcceptedAlert() {
            if ($('[id*=notificationoverlay_]').length == 0) {
                var msg = '';
                msg += '<div style="font-size:1.25em;font-weight:bold;padding-bottom:10px;">Action Required</div>';
                msg += 'Previous meeting (' + _previousMeetingDate + ') has not been Accepted/Ended.<br /><br />';
                msg += '<a href="javascript:showMeetingMinutesPDF(' + _previousMeetingInstanceID + ', \'' + _previousMeetingDate + '\', true);" style="color:blue;text-decoration:underline;">Review</a>&nbsp;<a href="javascript:confirmAcceptMeeting(' + _previousMeetingInstanceID + ');" style="color:blue;text-decoration:underline;">Accept</a>';
                showNotificationOverlay('Meeting Notification', msg, 'images/icons/redalert_square.png');
            }
        }

        function showMeetingMinutesPDF(AORMeetingInstanceID, date, showAcceptButton) {
            // find the attachment
            var attachmentID = 0;
            var meetingInstanceRow = $('#divAORMIHistory tr[key=' + AORMeetingInstanceID + ']');
            if (meetingInstanceRow != null) {
                //nHTML += '<td style="text-align: center;" name="viewminutescolumn" key="' + row.AORMeetingInstanceID + '" attachmentid="' + row.LastMeetingMinutesDocumentID + '">';
                attachmentID = meetingInstanceRow.find('td[name=viewminutescolumn]').attr('attachmentid');
            }

            if (attachmentID != null && attachmentID != 0 && attachmentID != 'null') {
                showMeetingMinutesInPDFViewer(attachmentID, AORMeetingInstanceID, date, showAcceptButton);
            }
            else {
                infoMessage('Generating PDF');
                PageMethods.RegenerateMeetingMinutes(<%=AORMeetingID%>, AORMeetingInstanceID, function (result) { regenerateMeetingMinutes_done(result, true, AORMeetingInstanceID, date, showAcceptButton) }, on_error);
            }
        }

        function showMeetingMinutesInPDFViewer(attachmentID, AORMeetingInstanceID, date, showAcceptButton) {
            var buttonLabels = [];
            var buttonFunctions = [];
            var buttonData = [];

            if (showAcceptButton) {
                buttonLabels.push('Accept');
                buttonFunctions.push('acceptMeetingFromPDFViewer');
                buttonData.push(AORMeetingInstanceID);
            }

            if (date == null || date.length == 0) {
                // see if we can find a date from the history table
                var row = $('#divAORMIHistory').find('tr[key=' + AORMeetingInstanceID + ']');
                var td = row.find('td[name=meetingdatecolumn]');
                var m = new moment(new Date(td.html()));
                date = m.format('MM/DD/YYYY');
            }

            showPDFViewer('Download_Attachment.aspx?attachmentID=' + attachmentID, 'Meeting Minutes - ' + date, null, null,
                showAcceptButton ? buttonLabels : null,
                showAcceptButton ? buttonFunctions : null,
                showAcceptButton ? buttonData : null
            );
        }

        function acceptMeetingFromPDFViewer(data) {
            meetingAcceptanceConfirmed('Yes', <%=AORMeetingID%> + '_' + data + '_false');

            var pm = popupManager.GetPopupByName('PDFVIEWER');
            pm.Close();
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _oldNoteTypeQF = $('#<%=this.ddlNoteType.ClientID %>').val();
            _newMeetingInstance = '<%=this.NewAORMeetingInstance %>'.toUpperCase() == 'TRUE';
            _parsedExistingMeetings = null;

            if (parent._expandedNotes != undefined) parent._expandedNotes = [];
            if (parent._checkedShowRemoved != undefined) parent._checkedShowRemoved = [];
            if (parent._checkedShowClosed != undefined) parent._checkedShowClosed = [];
            if (parent.showFrameForGrid) _sidebarVisible = $('#sidebar', defaultParentPage.document).is(':visible');
        }

        function initControls() {
            $('#divTabsContainer').tabs();

            if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                $('#<%=this.txtInstanceDate.ClientID %>').datetimepicker({
                    controlType: 'select',
                    dateFormat: 'm/d/yy',
                    timeFormat: 'h:mm TT'
                });
            }

            if (_newMeetingInstance) {
                var dt = new moment();
                var min = dt.minutes() % 10;

                if (min >= 1 && min <= 4) {
                    dt.subtract(min, 'minutes');
                }
                else if (min >= 6 && min <= 9) {
                    min += dt.add(10 - min, 'minutes');
                }

                $('#<%=this.txtInstanceDate.ClientID %>').val(dt.format('MM/DD/YYYY hh:mm A'));
            }
        }

        function initDisplay() {

            getMeetingMetrics(); // this contains metrics for the meeting TYPE, not the instance, so it displays for both new and old meetings

            if (!_newMeetingInstance) {

                // main details start out HIDDEN now since the toggle state is supposed to be kept between
                // refreshes; we start it hidden instead of visible to avoid screen flashing during rendering
                var detailsVisible = getQueryStringParameter('DetailsVisible', '0') == '1';
                setDetailsToggleState(detailsVisible);


                if ('<%=this.Locked %>'.toUpperCase() == 'TRUE') $('#imgLocked').show();
                if ('<%=this.MeetingEnded%>'.toUpperCase() == 'TRUE') $('#imgMtgEnded').show();
                if ('<%=this.MeetingEnded%>'.toUpperCase() == 'FALSE') $('#btnEndMeeting').show();
                if ('<%=this.MeetingAccepted%>'.toUpperCase() == 'FALSE') $('#imgMtgNotAccepted').show();
                if ('<%=this.MeetingAccepted%>'.toUpperCase() == 'TRUE') $('#imgMtgAccepted').show();
                if ('<%=this.MeetingEnded%>'.toUpperCase() == 'TRUE' && '<%=this.MeetingAccepted%>'.toUpperCase() == 'TRUE' && '<%=this.Locked %>'.toUpperCase() != 'TRUE') {
                    $('#imgLockMeeting').css('opacity', '0.5');
                    $('#imgLockMeeting').show();
                }

                var mtgName = $('[id$=txtAORMeetingInstanceName]').val();
                if (mtgName != null) {
                    var testName = mtgName.toLowerCase();
                    if (testName.endsWith(' meeting')) {
                        mtgName = mtgName.substring(0, mtgName.length - 8);
                    }
                    else if (testName.endsWith('meeting')) {
                        mtgName = mtgName.substring(0, mtgName.length - 7);
                    }
                }
                $('#txtEmailCustomNote').val('Good afternoon all, thank you to those who attended today\'s' + (mtgName != null && mtgName.length > 0 ? (' ' + mtgName) : '') + ' Meeting.  Please find the attached Meeting Minutes and Burndown Overview for your reference.');

                $('#imgDownloadPDF').show();
                $('#imgEmailPDF').show();
                $('#divInfo').show();
                $('#divToggleDetails').show();
                $('#divSubSection').show();
                $('#trActualLength').show();
                $('#trMeetingInstanceName').show();

                if (!_previousMeetingAsBeenAccepted && _previousMeetingInstanceID != 0) {
                    $('#divPreviousMeetingAccepted').show();
                    setTimeout(showPreviousMeetingNotAcceptedAlert, 500);
                }



                getAORs();
                getResources();
                //getNotes();
                getAORProgress();
                getHistory();
                loadAttachments();
                reopenPreviouslyVisibleNoteNodes();
            }
            else {
                setDetailsToggleState(true);
                $('#spnMeetingInstanceEditHeader').html('Use Existing Meeting');
                $('#divSelectMeeting').show();
                $('#btnStartMeeting').show();
            }

            setInitialNoteListPaneSize();

            if ('<%=this.CanEditAORMeetingInstance %>'.toUpperCase() == 'TRUE') {
                $('input[type="text"], textarea').css('color', 'black');
                $('input[type="text"], textarea').not('#<%=this.txtInstanceDate.ClientID %>').removeAttr('readonly');
                $('select').removeAttr('disabled');
                $('#btnCancel').show();

                if (!_newMeetingInstance) {
                    if ('<%=ForceUnlock%>'.toUpperCase() == 'TRUE') {
                        $('#btnAccept').show();
                    }

                    $('#btnSave').show();
                }
                $('#btnAddAOR').show();
                $('#btnAddResource').show();
                //$('#btnAddNote').show();
                $('#btnAddNoteDetail').show();

                var agendaObjectivesPresent = <%=tvNoteByAOR.ClientID%>_NodeHasChildren(null, 'ao', 1);
                if (agendaObjectivesPresent) {
                    $('#btnAddNoteObjectives').val('Edit Objectives');
                }

                $('#btnAddNoteObjectives').show();
            }

            if (parent.showFrameForGrid) {
                $('#btnBackToGrid').show();
            }
            else if (opener != undefined) {
                $('#btnClose').show();
            }

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });

            if ('<%=this.CanEditAORMeetingInstanceAlt %>'.toUpperCase() == 'TRUE') {
                $('#imgLocked').click(function () { imgLocked_click(); });
                $('#imgLockMeeting').click(function () { imgLockMeeting_click(); });
            }

            $('#imgDownloadPDF').click(function () { imgDownloadPDF_click(); });
            $('#imgEmailPDF').click(function () { imgEmailPDF_click(); });
            $('#btnSelectRecipients').click(function () { btnSelectRecipients_click(); })
            $('#btnSelectAllRecipients').click(function () { btnSelectAllRecipients_click(); });
            $('#btnClearAllRecipients').click(function () { btnClearAllRecipients_click(); });
            $('#btnResetRecipients').click(function () { btnResetRecipients_click(); });
            $('#btnDownloadPDF').click(function () { if (download('pdf')) { showDownloadPDFSettings(isEmailMode()); } return false; });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(false); return false; });
            $('#btnStartMeeting').click(function () { btnStartMeeting_click(); });
            $('#btnEndMeeting').click(function () { btnEndMeeting_click(); });
            $('#btnBackToGrid').click(function () { btnBackToGrid_click(); return false; });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#imgToggleDetails').click(function () { imgToggleDetails_click(); });
            $('#divTabsContainer ul li a').click(function () { tab_click($(this).text()); });
            $('#divPageContainer').on('keyup paste', 'input[type="text"]:not(#<%=this.txtInstanceDate.ClientID %>), textarea', function (event) {
                if (event.keyCode != 9 && event.keyCode != 16) input_change(this);
            });
            $('#divPageContainer').on('change', 'select:not(#<%=this.ddlNoteType.ClientID %>), input[type="checkbox"]:not(#chkShowRemovedAOR, [name="chkShowClosedSRs"], [name="chkShowClosedTasks"], #chkShowRemovedResource, #chkShowRemovedNote, [name="chkShowRemovedNoteDetail"], [name="chkShowClosedNoteDetail"])', function () { input_change(this); });
            $('#divPageContainer').on('blur', 'input[type="text"], textarea', function () { txtBox_blur(this); });
            $('#<%=this.txtInstanceDate.ClientID %>').change(function () { input_change(this); });
            $('#chkShowRemovedAOR').change(function () { chkShowRemovedAOR_change(); });
            $('#chkShowRemovedResource').change(function () { chkShowRemovedResource_change(); });
            $('#chkShowRemovedNote').change(function () { chkShowRemovedNote_change(); });
            $('#btnAddAOR').click(function () { btnAddAOR_click(); return false; });
            $('#btnAddResource').click(function () { btnAddResource_click(); return false; });
            $('#imgDisplayAllNotes').click(function () { imgDisplayAllNotes_click(); });
            $('#<%=this.ddlNoteType.ClientID %>').change(function () { ddlNoteType_change(); });
            $('#btnViewHistoricalNotes').click(function () { btnViewHistoricalNotes_click(); return false; });
            $('#btnAddNote').click(function () { btnAddNote_click(); return false; });
            $('#btnAddNoteDetail').click(function () { btnAddNoteDetail_click(); return false; });
            $('#btnAddNoteObjectives').click(function () { btnAddNoteObjectives_click(); return false; });
            $('#noteSpacer').on('click', function () { toggleNoteListPane(); });
            $('[id$=txtAORMeetingInstanceName]').on('change', function () { txtAORMeetingInstanceName_changed(); });
            $('#txtGoToNote').on('keyup', function (event) {
                this.value = stripAlpha(this.value);
                if (event.keyCode === 13) {
                    btnGoToNote_click();
                }
            });
            $('#btnGoToNote').on('click', function () { btnGoToNote_click(); });
            $('input[name=rdNoteTreeViewType]').on('click', function () { noteTreeViewTypeClicked(); });
            $('#txtMeetingSearch').on('keyup', function () { refreshExistingMeetings(); });
            $('[name=rdExistingMeetingSort]').on('change', function () { refreshExistingMeetings(); });
            $('[name=rdExistingMeetingSortDirection]').on('change', function () { refreshExistingMeetings(); });
            $('#btnAccept').on('click', function () { btnAcceptMeeting_click(); });
            $('#divPreviousMeetingAccepted').on('click', function () { showPreviousMeetingNotAcceptedAlert(); });

            $(window).resize(function () {
                resizePage();
                clearTimeout(_resizeTimeout);
                _resizeTimeout = setTimeout(function () {
                    refreshAllEditors();
                }, 500);
            });
            $(document).click(function (e) {
                try {
                    var objID = $(e.target).prop('id');
                    var objHtmlFor = $(e.target).prop('htmlFor');
                    var objFirstChildID = '';

                    if ($(e.target).children(':first').length > 0) {
                        objFirstChildID = $(e.target).children(':first').prop('id');
                    }

                    if (objHtmlFor == undefined) objHtmlFor = '';
                    if (objFirstChildID == undefined) objFirstChildID = '';

                    var excludedIDs = ['imgDownloadPDF', 'imgEmailPDF', 'btnSelectRecipients', 'btnSelectAllRecipients', 'btnResetRecipients', 'btnClearAllRecipients', 'divDownloadPDFSettings', 'cbResourceEmail', 'divEmailCustomNote', 'txtEmailCustomNote'];
                    if (excludedIDs.indexOf(objID) == -1 && objID.indexOf('cblDownloadPDFSettings') == -1 && objHtmlFor.indexOf('cblDownloadPDFSettings') == -1 && objFirstChildID.indexOf('cblDownloadPDFSettings') == -1 && objFirstChildID.indexOf('cbResourceEmail') == -1) $('#divDownloadPDFSettings').slideUp();
                }
                catch (e) { }
            });
            $(window).on('beforeunload', function () {
                if (opener != undefined) {
                    if (parent._expandedAORs != undefined) parent._expandedAORs = [];
                    if (parent._expandedNotes != undefined) parent._expandedNotes = [];
                    if (parent._checkedShowClosedSRs != undefined) parent._checkedShowClosedSRs = [];
                    if (parent._checkedShowClosedTasks != undefined) parent._checkedShowClosedTasks = [];
                    if (parent._checkedShowRemoved != undefined) parent._checkedShowRemoved = [];
                    if (parent._checkedShowClosed != undefined) parent._checkedShowClosed = [];
                }
            });

            if (parent.showFrameForGrid) {
                setInterval(function () {
                    if ($('#sidebar', defaultParentPage.document).is(':visible') != _sidebarVisible) {
                        refreshAllEditors();
                        _sidebarVisible = $('#sidebar', defaultParentPage.document).is(':visible');
                    }
                }, 1000);
            }
        }

        function initFinish() {
            _mtgJSON = btnSave_click(false, false, true); // set up initial state of meeting json
            if (_newMeetingInstance) {
                loadExistingMeetings();
            }
            else {
                // go to initial agenda note, if available
                if ($('input[name=rdNoteTreeViewType][value=aor]').is(':checked') && <%=tvNoteByAOR.ClientID%>_NodeHasChildren(null, 'ao', 1)) {
                    var agendaNodes = <%=tvNoteByAOR.ClientID%>_GetChildNodes(null, 'ao', 1)
                    if (agendaNodes != null && agendaNodes.length > 0) {
                        var agendaNode = agendaNodes[0];
                        var key = $(agendaNode).attr('key');
                        btnGoToNote_click(key);
                    }
                }
            }
        }

        $(document).ready(function () {
            if ('<%=this.Download %>' == '') {
                initVariables();
                initControls();
                initDisplay();
                initEvents();
                initFinish();
            }
        });
    </script>
</asp:Content>