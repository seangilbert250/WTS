﻿<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" EnableViewState="false" AutoEventWireup="true" CodeFile="Task_Edit.aspx.cs" Inherits="Task_Edit" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Subtask</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <style type="text/css">
        .auto-style1 {
            width: 93px;
        }

        .auto-style2 {
            width: 130px;
        }

        .auto-style3 {
            width: 42px;
        }
    </style>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="images/icons/pencil.png" alt="Review/Edit Task" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Subtask</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server">
    <span id="labelMessage" style="color: red; padding-left: 15px;"></span>
</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
    <img src="Images/Icons/help.png" id="helpButton" style="cursor: pointer" />
	<iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="vertical-align: bottom; display: inline-block;"></iti_Tools_Sharp:Menu>
    <input type="button" id="buttonMetrics" value="Metrics" />
    <input type="button" id="buttonClose" value="Close" />
    <input type="button" id="buttonSave" runat="server" value="Save" />
    <input type="button" id="buttonSaveAndClose" value="Save and Close" style="display: none;" />
    <input type="button" id="buttonSaveAndClear" value="Save and Add Another" style="display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <div id="divPageContainer" class="pageContainer">
        <div id="divTabsContainer" class="mainPageContainer">
            <div id="divParentDetailsBar">
                <div id="divMetrics" style="display:none;padding:5px;border-bottom:1px solid #d3d3d3;"><span style="color:#aaaaaa;">Loading metrics...</span></div>
                <div id="divParentDetailsContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
                    <div id="divParentDetailsHeader" class="pageContentHeader" style="padding: 5px;">
                        <div class="attributesRequired" style="width: 10px; display: inline;">
                            <img id="imgHideParentDetails" class="hideSection" sectionname="ParentDetails" alt="Hide Inherited Details" title="Hide Inherited Details" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
                            <img id="imgShowParentDetails" class="showSection" sectionname="ParentDetails" alt="Show Inherited Details" title="Show Inherited Details" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                        </div>
                        <div class="attributesLabel" style="padding-left: 5px; display: inline;">Inherited Details:</div>
                    </div>
                    <table id="tableParentDetails" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 5px;">
                        <tr>
                            <td colspan="2" style="text-align: left; vertical-align: top;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                                    <tr id="trParentTitle" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td class="attributesLabel">Primary Task Title:</td>
                                        <td class="attributesValue">
                                            <asp:TextBox ID="txtParentTitle" runat="server" Width="98%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>Resource Group:</td>
                                        <td style="padding: 0px 2px;">
                                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                                                <tr id="trResourceGroup">
                                                    <td class="attributesValue" style="padding: 0px;">
                                                        <asp:TextBox ID="txtResourceGroup" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td style="width: 80px; text-align: left; position: relative;">System(Task):</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtSystemTask" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td id="lblReleaseAOR" style="width: 110px; text-align: left; position: relative;">Release/Deployment MGMT AOR:</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtReleaseAOR" runat="server" Width="93%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>Work Area:</td>
                                        <td style="padding: 0px 2px;">
                                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                                                <tr id="trWorkArea">
                                                    <td class="attributesValue" style="padding: 0px;">
                                                        <asp:TextBox ID="txtWorkArea" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td style="width: 80px; text-align: left; position: relative;">Contract:</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtContract" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td style="width: 110px; text-align: left; position: relative;">Workload Allocation:</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtWorkloadAllocation" runat="server" Width="93%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>Production Status:</td>
                                        <td style="padding: 0px 2px;">
                                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                                                <tr id="trProductionStatus">
                                                    <td class="attributesValue" style="padding: 0px;">
                                                        <asp:TextBox ID="txtProductionStatus" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td style="width: 80px; text-align: left; position: relative;">Functionality:</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtFunctionality" runat="server" Width="95%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                    <td id="lblWorkloadAOR" style="width: 110px; text-align: left; position: relative;">Workload MGMT AOR:</td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 10px">
                                                        <asp:TextBox ID="txtWorkloadAOR" runat="server" Width="93%" BackColor="#cccccc" ReadOnly="true" onkeydown="return false;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <ul id="Tabs">
                <li><a id="tabDetails" href="#divDetailsTab" onclick="Tab_click('details');">Details</a></li>
                <li><a id="tabNotes" href="#divNotes" onclick="Tab_click('notes');">Notes</a></li>
                <li><a id="tabAttachments" href="#divAttachments" onclick="Tab_click('attachments');">Attachments</a></li>
                <li><a id="tabResources" href="#divResources" onclick="Tab_click('resources');">Resources</a></li>
            </ul>
            <div id="divDetailsTab">
                <div id="divDetailsContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
                    <table id="tableDetails" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                        <tr>
                            <td id="tdLeftCol2" style="vertical-align: top;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                                    <tr id="trWorkItem" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td name="FirstLabel" class="attributesLabel">Subtask:</td>
                                        <td class="attributesValue">
                                            <asp:TextBox ID="txtWorkloadNumber" runat="server" Width="75" BackColor="#cccccc" ReadOnly="true" Style="text-align: right; padding-right: 3px;" onkeydown="return false;" />
                                        </td>
                                    </tr>
                                    <tr id="trTitle" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="FirstLabel" class="attributesLabel">Title:</td>
                                        <td class="attributesValue">
                                            <asp:TextBox ID="txtTitle" runat="server" Width="98%" />
                                        </td>
                                    </tr>
                                    <tr id="trAuditing">
                                        <td colspan="3" style="text-align: left; vertical-align: top;">
                                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                                                <tr>
                                                    <td class="attributesValue" style="padding-left: 160px;">Submitted:&nbsp;
										                <asp:Label ID="labelCreated" runat="server" />
                                                    </td>
                                                    <td class="attributesValue" style="text-align: right; padding-right: 13px;">Updated:&nbsp;
										                <asp:Label ID="labelUpdated" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdLeftCol2" style="vertical-align: top;">
                                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                                                <tr id="trSecondaryResource" class="attributesRow" style="display: none;">
                                                    <td class="attributesRequired"></td>
                                                    <td name="FirstLabel" class="attributesLabel">Secondary Tech Resource:
                                                    </td>
                                                    <td class="attributesValue">
                                                        <asp:DropDownList ID="ddlSecondaryResource" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr id="trPrimaryBusResource" class="attributesRow" style="display: none;">
                                                    <td class="attributesRequired"></td>
                                                    <td name="FirstLabel" class="attributesLabel">Primary Bus Resource:
                                                    </td>
                                                    <td class="attributesValue">
                                                        <asp:DropDownList ID="ddlPrimaryBusResource" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr id="trSecondaryBusResource" class="attributesRow" style="display: none;">
                                                    <td class="attributesRequired"></td>
                                                    <td name="FirstLabel" class="attributesLabel">Secondary Bus Resource:
                                                    </td>
                                                    <td class="attributesValue">
                                                        <asp:DropDownList ID="ddlSecondaryBusResource" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr class="attributesRow">
                                        <td class="attributesRequired" style="padding: 0px 2px; min-width: 10px"></td>
                                        <td name="FirstLabel" class="attributesLabel" style="padding: 0px 2px; min-width: 140px;">
                                            <asp:Label ID="aorLabel" runat="server"></asp:Label>
                                        </td>
                                        <td class="attributesValue" style="padding: 0px 2px;">
                                            <div id="divWorkloadAORs" runat="server" style="display: inline-block;"></div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table id="tableDetails" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                        <tr>
                            <td id="tdLeftCol2" style="vertical-align: top; width: 350px;">
                                <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                                    <tr id="trWorkActivity" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="LeftColumn" class="attributesLabel">Work Activity:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlWorkItemType" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trPriority" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="FirstLabel" class="attributesLabel">Priority:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlPriority" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trStatus" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="LeftColumn" class="attributesLabel">Status:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlStatus" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdProductVersionRequired" class="attributesRequired">&nbsp;</td>
                                        <td class="attributesLabel">Product Version:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:Label ID="lblProductVersion" runat="server" />
                                            <asp:DropDownList ID="ddlProductVersion" runat="server" Style="font-size: 12px; width: 160px; display: none;" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                            <a id="aEdit" href="" style="color: blue; padding-left: 5px; display: none;">Edit</a>
                                        </td>
                                    </tr>
                                    <tr id="trPercentComplete" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td name="FirstLabel" class="attributesLabel">Percent Complete:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlPercentComplete" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trSRNumber" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td class="attributesLabel">SR Number:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:TextBox ID="txtSRNumber" runat="server" TextMode="Number" Style="width: 75px;"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="tdRightCol2" style="vertical-align: top;">
                                <table cellpadding="0" cellspacing="0" style="width: 415px; vertical-align: top; text-align: left;">
                                    <tr id="trPlannedStart" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="FirstLabel" class="auto-style2">Planned Start Date:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtStartDate_Planned" runat="server" Style="width: 75px;" />
                                        </td>
                                    </tr>
                                    <tr id="trActualStart" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="FirstLabel" class="auto-style2">Actual Start Date:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtStartDate_Actual" runat="server" Style="width: 75px;" />
                                        </td>
                                    </tr>
                                    <tr id="trPlannedHours" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="FirstLabel" class="auto-style2">Estimated Effort:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:DropDownList ID="ddlHours_Planned" runat="server" Style="font-size: 12px; width: 80px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="auto-style6" style="text-align: left;">
                                            <img id="imgEffortHelp" runat="server" src="Images/Icons/help.png" alt="Help" title="Help" width="15" height="15" style="cursor: pointer; display: block;" />
                                        </td>
                                    </tr>
                                    <tr id="trActualHours" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="FirstLabel" class="auto-style2">Actual Effort:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:DropDownList ID="ddlHours_Actual" runat="server" Style="font-size: 12px; width: 80px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trActualEnd" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="FirstLabel" class="auto-style2">Actual End Date:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtEndDate_Actual" runat="server" Style="width: 75px;" />
                                        </td>
                                    </tr>



                                    <tr id="trBusinessRank" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="LeftColumn" class="auto-style2">Bus. Rank:
                                        </td>

                                    </tr>
                                    <tr id="trSort" class="attributesRow" style="display: none;">
                                        <td class="auto-style3">&nbsp;</td>
                                        <td name="LeftColumn" class="auto-style2">Tech. Rank:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtSortOrder" runat="server" TextMode="Number" Style="width: 75px;" />
                                        </td>
                                    </tr>
                                    <tr id="trAssignedTo" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="FirstLabel" class="attributesLabel">Assigned To:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlAssignedTo" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                            <input type="button" id="btnAORResourceTeam" value="..." style="display: none;" />
                                        </td>
                                    </tr>
                                    <tr id="trPrimaryResource" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="FirstLabel" class="attributesLabel">Primary Resource:
                                        </td>
                                        <td class="attributesValue">
                                            <asp:DropDownList ID="ddlPrimaryResource" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trAssignedToRank" class="attributesRow">
                                        <td class="attributesRequired">*</td>
                                        <td name="LeftColumn" class="auto-style2">Assigned To Rank:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:DropDownList ID="ddlAssignedToRank" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                                <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="trCustomerRank" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td name="LeftColumn" class="auto-style2">Customer Rank:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtBusinessRank" runat="server" TextMode="Number" Style="width: 75px;" />
                                        </td>
                                    </tr>
                                    <tr id="trDateNeeded" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td name="LeftColumn" class="auto-style2">Date Needed:
                                        </td>
                                        <td class="auto-style1">
                                            <asp:TextBox ID="txtDateNeeded" runat="server" Width="75" />
                                        </td>
                                    </tr>
                                    <tr id="trBusinessReview" class="attributesRow">
                                        <td class="attributesRequired">&nbsp;</td>
                                        <td name="LeftColumn" colspan="2" class="auto-style2">
                                            <asp:CheckBox ID="chkBusinessReview" runat="server" Style="padding: 0px;" />
                                            <label for="<%=this.chkBusinessReview.ClientID %>">Requested to be reviewed by BA or SME</label>
                                            <img id="imgHelp" src="Images/Icons/help.png" alt="When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'. Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA. Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the subtask is ready to be 'Checked-In'." title="When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'. Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA. Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the subtask is ready to be 'Checked-In'." width="15" height="15" style="cursor: pointer; vertical-align: middle; padding: 0px 5px 5px 2px;" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divMilestonesContainer" class="attributesRow" style="vertical-align: top; width: 99%;">
                    <div id="divMilestonesHeader" class="pageContentHeader" style="padding: 5px;">
                        <div class="attributesRequired" style="width: 10px; display: inline;">
                            <img id="imgHideMilestones" class="hideSection" sectionname="Milestones" alt="Hide Milestones" title="Hide Milestones" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                            <img id="imgShowMilestones" class="showSection" sectionname="Milestones" alt="Show Milestones" title="Show Milestones" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; " />
                        </div>
                        <div class="attributesLabel" style="padding-left: 5px; display: inline;">Subtask Milestones:</div>
                    </div>
                    <div id="divMilestones" class="attributesValue" style="padding: 10px 0px 10px 20px; display: none;">
                        <table id="tableMileStoneDetails" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-bottom: 10px;">
                            <tr>
                                <td id="tdLeftCol2" style="vertical-align: top; width: 350px;">
                                    <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
                                        <tr id="trTotalDaysOpened" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel">Total Days Opened(#):</td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalDaysOpened" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trInProgressDate" class="attributesRow">
                                             <td name="FirstLabel" class="attributesLabel">In Progress Date:</td>
                                              <td class="attributesValue">
                                                  <asp:Label ID="labelInProgressDate" runat="server" />
                                            </td>
                                            <td class="attributesValue">Time:
                                                  <asp:Label ID="labelInProgressTime" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trDeployedDate" class="attributesRow">
                                             <td name="FirstLabel" class="attributesLabel">Deployed Date:</td>
                                              <td class="attributesValue">
                                                 <asp:Label ID="labelDeployedDate" runat="server" />
                                            </td>
                                            <td class="attributesValue">Time:
                                                 <asp:Label ID="labelDeployedTime" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trReadyForReviewDate" class="attributesRow">
                                             <td name="FirstLabel" class="attributesLabel">Ready For Review Date:</td>
                                              <td class="attributesValue">
                                                  <asp:Label ID="labelReadyForReviewDate" runat="server" />
                                            </td>
                                            <td class="attributesValue">Time:
                                                  <asp:Label ID="labelReadyForReviewTime" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trClosedDate" class="attributesRow">
                                             <td name="FirstLabel" class="attributesLabel">Closed Date:</td>
                                              <td class="attributesValue">
                                                  <asp:Label ID="labelClosedDate" runat="server" />
                                            </td>
                                            <td class="attributesValue">Time:
                                                  <asp:Label ID="labelClosedTime" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td id="tdRightCol2" style="vertical-align: top;">
                                    <table cellpadding="0" cellspacing="0" style="width: 375px; vertical-align: top; text-align: left;">
                                        <tr id="trTotalBuisnessDaysOpened" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel" style="white-space:nowrap">Total Business Days Opened(#):
                                            </td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalBusinessDaysOpened" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trInProgressDays" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel">Days in Progress(#):
                                            </td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalDaysInProgress" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trDeployedDays" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel">Days Waiting(#):
                                            </td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalDaysDeployed" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trReadyForReviewDays" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel">Days Waiting(#):
                                            </td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalDaysReadyForReview" runat="server" />
                                            </td>
                                        </tr>
                                        <tr id="trClosedDays" class="attributesRow">
                                            <td name="FirstLabel" class="attributesLabel">Days Waiting(#):
                                            </td>
                                            <td class="attributesValue">
                                                <asp:Label ID="labelTotalDaysClosed" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="divDescriptionContainer" class="attributesRow" style="vertical-align: top; width: 99%;">
                    <div id="divDescriptionHeader" class="pageContentHeader" style="padding: 5px;">
                        <div class="attributesRequired" style="width: 10px; display: inline;">
                            <img id="imgHideDescription" class="hideSection" sectionname="Description" alt="Hide Description" title="Hide Description" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
                            <img id="imgShowDescription" class="showSection" sectionname="Description" alt="Show Description" title="Show Description" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                        </div>
                        <div class="attributesLabel" style="padding-left: 5px; display: inline;">Description:</div>
                    </div>
                    <div id="divDescription" class="attributesValue" style="padding: 10px 0px 10px 20px;">
                        <textarea id="textAreaDescription" runat="server" rows="18" style="width: 98%;"></textarea>
                    </div>
                </div>
                <div id="divRequirementsContainer" class="attributesRow" style="vertical-align: top; width: 99%; height: 99%;">
                    <div id="divRequirementsHeader" class="pageContentHeader" style="padding: 5px;">
                        <div class="attributesRequired" style="width: 10px; display: inline;">
                            <img id="imgHideRequirements" class="hideSection" sectionname="Requirements" alt="Hide Requirements" title="Hide Requirements" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                            <img id="imgShowRequirements" class="showSection" sectionname="Requirements" alt="Show Requirements" title="Show Requirements" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
                        </div>
                        <div class="attributesLabel" style="padding-left: 5px; display: inline;">Requirements:</div>
                    </div>
                    <div id="divRequirements" class="attributesValue" style="padding: 0px 0px 0px 0px; height: 280px; display: none;">
                        <iframe id="frameRQMTs" name="frameRQMTs" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 99%; margin: 0px; padding: 0px;" title="">Requirements</iframe>
                    </div>
                </div>
            </div>
            <div id="divNotes">
                <iframe id="frameComments" src="javascript:'';" frameborder="0" scrolling="no" style="display: block; width: 100%;"></iframe>
            </div>
            <div id="divAttachments">
                <iframe id="frameAttachments" name="frameAttachments" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 99%; margin: 0px; padding: 0px;" title="">Work Item Attachments</iframe>
            </div>
            <div id="divResources">
                <div id="divAffiliated" style="text-align: center; width: 98%; height: 99%; overflow: auto;"></div>
            </div>
        </div>
    </div>

    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script src="Scripts/cleditor/jquery.cleditor.js"></script>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _canEdit = false;
        var _taskID = 0, _workItemID = 0, _commentQty = 0;
    </script>

    <script id="jsEvents" type="text/javascript">

        function SetCommentQty(qty) {
            if (!qty) {
                qty = 0;
            }

            $('#tabNotes').text('Notes (' + qty + ')');
        }

        function SetAttachmentQty(qty) {
            if (!qty) {
                qty = 0;
            }

            $('#tabAttachments').text('Attachments (' + qty + ')');
        }

        function SetResourceQty(qty) {
            if (!qty) {
                qty = 0;
            }

            $('#tabResources').text('Resources (' + qty + ')');
        }

        function refreshPage(newID, saved) {
            var url = window.location.href;
            if (newID > 0) {
                url = editQueryStringValue(url, 'taskID', newID);
            }
            else {
                url = editQueryStringValue(url, 'taskID', 0);
            }

            if (!saved) {
                url = editQueryStringValue(url, 'Saved', '0');
            }
            else {
                url = editQueryStringValue(url, 'Saved', '1');
            }

            window.location.href = 'Loading.aspx?Page=' + url;
        }

        function clearMessage() {
            $('#labelMessage').text('');
            $('#labelMessage').hide();
        }

        function buttonHistory_click() {
            var title = '', url = '';
            var h = 600, w = 1000;

            title = 'Subtask History';
            url = _pageUrls.Maintenance.WorkItemHistory
                + '?type=WorkItemTask'
                + '&workItemTaskID=' + _taskID
                ;

            //open in a popup
            var openPopup = popupManager.AddPopupWindow('WorkItemTaskHistory', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

        function closePage() {
            if (closeWindow) {
                closeWindow();
            }
            else {
                window.close();
            }
        } //end closePage()

        function buttonClose_click() {
            clearMessage();
            if ($('input[fieldChanged="1"], select[fieldChanged="1"], textarea[fieldChanged="1"]').length > 0) {
                QuestionBox('Confirm Close', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmClose', 300, 300, this);
            } else {
                closePage();
            }
        }

        function confirmClose(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    buttonSave_click('close');
                    break;
                case 'DISCARD':
                    closePage();
                    break;
                default:
                    return;
            }
        }

        function buttonSave_click(pageOption) {
            clearMessage();
            var msg = validateFields();

            if (msg.length > 0) {
                MessageBox('Please update the following fields:<br>' + msg);
                return;
            }
            if (document.getElementById("frameComments").contentWindow.SaveComments) {
                document.getElementById("frameComments").contentWindow.SaveComments();
            }

            var statusID = parseInt($('#<%=this.ddlStatus.ClientID %> option:selected').val());
            var statusChanged = parseInt($('#<%=this.ddlStatus.ClientID %>').attr('fieldchanged'));

            if (statusID == 10 && statusChanged == 1 && '<%=this.UnclosedSRTasks%>' == 1) {
                var openPopup = top.popupManager.GetPopupByName('Question');
                if (openPopup) top.popupManager.RemovePopupWindow('Question');

                openPopup = popupManager.AddPopupWindow('ConfirmSR', 'Confirm SR Closed', 'QuestionBox.aspx?question=All Work Tasks associated with this SR will be in a Closed Status. This will cause the SR to be set to Resolved. Would you like to proceed?&buttons=Save,Cancel&function=confirmSRUpdate&param=undefined', 100, 375, 'PopupWindow', this);
                if (openPopup) openPopup.Open();
            } else {
                save(pageOption);
            }
        }

        function confirmSRUpdate(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    save();
                    break;
                default:
                    return;
            }
        }

        function validateFields() {
            var invalidFields = [];
            var msg = '';

            if ($('#<%=this.txtTitle.ClientID %>').val() == '') {
                invalidFields.push('Title is required');
            }
            if ($('#<%=this.ddlAssignedTo.ClientID %>').val() == 0) {
                invalidFields.push('Assigned To is required');
            }
            //todo: add date validation

            if ($('#<%=this.ddlPriority.ClientID %>').val() == 0) {
                invalidFields.push('Priority is required');
            }

            if ($('#<%=this.ddlStatus.ClientID %>').val() == 0) {
                invalidFields.push('Status is required');
            }

            if ($('#<%=this.ddlPrimaryResource.ClientID %>').val() == 0) {
                invalidFields.push('Primary Resource');
            }

            if ($('#<%=this.ddlProductVersion.ClientID %>').val() == 0) {
                invalidFields.push('Product Version');
            }

            if ($('#<%=this.ddlAssignedToRank.ClientID %>').val() == 0) {
                invalidFields.push('Assigned To Rank is required');
            }

            if (invalidFields.length > 0) {
                msg = invalidFields.join('<br>');
            }

            return msg;
        }

        function resizePage() {
            var heightModifier = 0;

            resizePageElement('divPageContainer', heightModifier + 2);
            resizePageElement('divTabsContainer', heightModifier + 3);
            resizePageElement('frameComments', heightModifier + 12);
            resizePageElement('frameAttachments', heightModifier);
            resizePageElement('frameRQMTs', heightModifier);

            resizePageElement('divDetailsTab', heightModifier);
            resizePageElement('divNotes', heightModifier);
            resizePageElement('divAttachments', heightModifier);
            resizePageElement('divResources', heightModifier);
            //resizePageElement('divDescriptionContainer', heightModifier);
            resizePageElement('divRequirementsContainer', heightModifier);

            resizeFrames();
        }

        function resizeFrames() {
            var frame;
            var fPageHeight = 0;

            frame = $('#frameComments')[0];
            if (frame.contentWindow
                && frame.contentWindow.document
                && frame.contentWindow.document.body
                && frame.contentWindow.document.body.offsetHeight) {
                fPageHeight = frame.contentWindow.document.body.offsetHeight;
            }
            frame.style.height = fPageHeight + 'px';
        }

        function showHideSection_click(imgId, show, sectionName) {
            clearMessage();

            if (show) {
                $('#div' + sectionName).show();
                $('#table' + sectionName).show();
                $('#' + imgId).hide();
                $('#' + imgId.replace('Show', 'Hide')).show();
                $('tr[section="' + sectionName + '"]').show();

                switch (sectionName) {
                    case "Comments":
                        if ($('#frameComments').attr('src') == "javascript:'';") {
                            loadComments();
                        }
                        break;
                }
            }
            else {
                $('#div' + sectionName).hide();
                $('#table' + sectionName).hide();
                $('#' + imgId).hide();
                $('#' + imgId.replace('Hide', 'Show')).show();
                $('tr[section="' + sectionName + '"]').hide();
            }

            resizePage();
        }

        function input_change(obj) {
            clearMessage();

            $(obj).attr('fieldChanged', '1');
        }

        function activateSaveButton() {
            clearMessage();

            if (_canEdit) {
                $('#buttonSave').prop('disabled', false);
            }
        }

        function loadComments() {
            var url = window.location.search;
            url = editQueryStringValue(url, 'Saved', '0');
            url = 'Loading.aspx?Page=Task_Comments.aspx' + url;

            $('#frameComments').attr('src', url);
        }

        function loadAttachments() {
            var url = window.location.search;
            url = editQueryStringValue(url, 'Saved', '0');
            url = 'Loading.aspx?Page=Task_Attachments.aspx' + url;

            $('#<%=this.frameAttachments.ClientID%>').attr('src', url);

        }

        function loadRequirements() {
            var systemID = $('[id$=txtSystemTask]').attr('WTS_SYSTEMID');
            var workAreaID = $('[id$=txtWorkArea]').attr('WorkAreaID');

            var url = 'Loading.aspx?Page=' + 'RQMT_Grid.aspx?GridType=RQMT&MyData=true&View=RQMT%20Defect%20Metrics&GridPageIndex=0&IsConfigured=True&CurrentLevel=1&Filter=SYSTEM_ID=' + systemID + '|WORKAREA_ID=' + workAreaID + '&SessionPageKey=TaskEdit&IgnoreUserFilters=true&ShowExport=false&ShowCOG=false&HideBuilderButton=true&ReadOnly=true&ShowSelectCheckboxes=false&ShowPageTitle=false&HideColumns=' + escape('RQMT Defect Metrics=RQMT Primary #,RQMT Defect Metrics=RQMT Primary');

            $('#<%=this.frameRQMTs.ClientID%>').attr('src', url);

        }



        function Tab_click(tabName) {
            $('#divTabsContainer').children('div').hide();
            $('#divParentDetailsBar').show();
            switch (tabName.toUpperCase()) {
                case 'DETAILS':
                    $('#divDetailsTab').show();

                    break;
                case 'NOTES':
                    if ($('#frameComments') && $('#frameComments').attr('src') == "javascript:'';") {
                        loadComments();
                    }
                    $('#divNotes').show();

                    break;
                case 'ATTACHMENTS':
                    if ($('#<%=this.frameAttachments.ClientID%>') && $('#<%=this.frameAttachments.ClientID%>').attr('src') == "javascript:'';") {
                        loadComments();
                    }
                    $('#divAttachments').show();

                    break;
                case 'RESOURCES':
                    $('#divResources').show();
            }

            resizePage();
        }

        function aEdit_click() {
            $('#aEdit').hide();
            $('#tdProductVersionRequired').text('*');
            $('#<%=this.lblProductVersion.ClientID %>').hide();
            $('#<%=this.ddlProductVersion.ClientID %>').show();

            MessageBox('Warning - Manually changing the Product Version should be done carefully as this will affect the tracking from AOR.');
        }
    </script>

    <script id="jsAJAX" type="text/javascript">

        function save(pageOption) {
            var title = '', description = '', plannedStart = '', actualStart = '', actualEnd = '', dateNeeded = '';
            var estimatedEffortID = 0, actualEffortID = 0, percentComplete = 0, assignedToID = 0, primaryResourceID = 0, secondaryResourceID = 0, workItemTypeID = 0
                , primaryBusResourceID, secondaryBusResourceID, priorityID = 0, statusID = 0, busRank = 0, sortOrder = 0, srNumber = 0, assignedToRankID = 0, productVersionID = 0;
            var businessReview = false;

            if (!pageOption || pageOption == undefined || pageOption == null) {
                pageOption = '';
            }

            priorityID = parseInt($('#<%=this.ddlPriority.ClientID %> option:selected').val());
            title = $('#<%=this.txtTitle.ClientID %>').val();
            description = encodeURIComponent($('#<%=this.textAreaDescription.ClientID %>').val());
            plannedStart = $('#<%=this.txtStartDate_Planned.ClientID %>').val();
            actualStart = $('#<%=this.txtStartDate_Actual.ClientID %>').val();
            actualEnd = $('#<%=this.txtEndDate_Actual.ClientID %>').val();
            estimatedEffortID = parseInt($('#<%=this.ddlHours_Planned.ClientID %> option:selected').val());
            actualEffortID = parseInt($('#<%=this.ddlHours_Actual.ClientID %> option:selected').val());
            percentComplete = parseInt($('#<%=this.ddlPercentComplete.ClientID %> option:selected').val());
            assignedToID = parseInt($('#<%=this.ddlAssignedTo.ClientID %> option:selected').val());
            primaryResourceID = parseInt($('#<%=this.ddlPrimaryResource.ClientID %> option:selected').val());
            secondaryResourceID = parseInt($('#<%=this.ddlSecondaryResource.ClientID %> option:selected').val());
            primaryBusResourceID = parseInt($('#<%=this.ddlPrimaryBusResource.ClientID %> option:selected').val());
            secondaryBusResourceID = parseInt($('#<%=this.ddlSecondaryBusResource.ClientID %> option:selected').val());
            statusID = parseInt($('#<%=this.ddlStatus.ClientID %> option:selected').val());
            workItemTypeID = parseInt($('#<%=this.ddlWorkItemType.ClientID %> option:selected').val());
            busRank = $('#<%=this.txtBusinessRank.ClientID %>').val() == '' ? 0 : parseInt($('#<%=this.txtBusinessRank.ClientID %>').val());
            sortOrder = $('#<%=this.txtSortOrder.ClientID %>').val() == '' ? 0 : parseInt($('#<%=this.txtSortOrder.ClientID %>').val());
            srNumber = $('#<%=this.txtSRNumber.ClientID %>').val();
            assignedToRankID = parseInt($('#<%=this.ddlAssignedToRank.ClientID %> option:selected').val());
            productVersionID = parseInt($('#<%=this.ddlProductVersion.ClientID %> option:selected').val());
            dateNeeded = $('#<%=this.txtDateNeeded.ClientID %>').val();
            businessReview = $('#<%=this.chkBusinessReview.ClientID %>').prop('checked');

            var arrAORs = [];

            $('#<%=this.divWorkloadAORs.ClientID %> tr').each(function () {
                var $obj = $(this);

                if ($obj.find('select').val() > 0) {
                    arrAORs.push({ 'aorreleaseid': $obj.find('select').val() });
                } else if ($('#aorReleaseID').attr('value')) {
                    arrAORs.push({ 'aorreleaseid': $('#aorReleaseID').attr('value') });
                }
            });

            var nAORsJSON = '{save:' + JSON.stringify(arrAORs) + '}';

            PageMethods.SaveTask(_taskID, _workItemID, priorityID
                , title, description, assignedToID, primaryResourceID, secondaryResourceID, primaryBusResourceID, secondaryBusResourceID
                , plannedStart, actualStart, estimatedEffortID, actualEffortID, actualEnd
                , percentComplete, statusID, workItemTypeID, busRank, sortOrder, pageOption, srNumber, assignedToRankID, productVersionID, dateNeeded, businessReview, nAORsJSON
                , save_done, on_error);
        }

        function save_done(result) {
            var saved = false;
            var id = 0;
            var errorMsg = '', pageOption = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.id) {
                        id = obj.id;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                    if (obj.pageOption) {
                        pageOption = obj.pageOption;
                    }
                }

                if (saved) {
                    if ('<%=AltRefreshAfterCloseFunction%>' != '') {
                        if (opener.<%=(AltRefreshAfterCloseFunction != "" ? AltRefreshAfterCloseFunction : "NOFUNCTION")%>) {
                            opener.<%=(AltRefreshAfterCloseFunction != "" ? AltRefreshAfterCloseFunction : "NOFUNCTION")%>();
                        }
                    }
                    else {
                        if (window && window.refreshPage) {
                            if (window.location.href.indexOf('WorkItemGrid.aspx') != -1) {
                                window.refreshPage(true);
                            }
                            else {
                                opener.refreshPage(1);
                            }
                        }
                    }

                    if (pageOption.toUpperCase() == 'CLOSE') {
                        closePage();
                    }
                    else if (pageOption.toUpperCase() == 'CLEAR') {
                        refreshPage(0, true);
                    }
                    else {
                        refreshPage(id, true);
                    }
                }
                else {
                    MessageBox('Failed to save Task. \n' + errorMsg);
                }
            }
            catch (e) {
                //alert("Error in Save_Done. " + e.message);
            }
        }

        function on_error(result) {
            var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

            MessageBox('save error:  \n' + resultText);
        }

        function getAffiliated() {
            $('#divAffiliated').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            var arrAORs = [];

            if (_canEdit) {
                <%--$('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function() {
				    var $obj = $(this);

                    if ($obj.find('select').val() > 0) arrAORs.push($obj.find('select').val());
                });--%>

                $('#MgmtWorkloadAORs').each(function () {
                    var $obj = $(this);

                    if ($obj.find('select').val() > 0) arrAORs.push($obj.find('select').val());
                });
            }
            else {
                arrAORs.push('-1');
            }

            PageMethods.GetAffiliated(_workItemID, <%=this.SystemID%>, <%=this.ProductVersionID%>, $('#<%=this.txtResourceGroup.ClientID%>').attr('resourcegroupid'), $('#<%=this.ddlWorkItemType.ClientID %>').val(), "", getAffiliated_done, getAffiliated_error);
        }

        function getAffiliated_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            var resourceCount = 0;

            if (dt == null || dt.length == 0) {
                nHTML = 'No Affiliated Resources';
            }
            else {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th rowspan="2" style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 155px;">Resource</th>';
                nHTML += '<th rowspan="2" style="border-top: 1px solid grey; text-align: center; width: 155px;">Resource Type</th>';
                nHTML += '<th colspan="2" style="border-top: 1px solid grey; text-align: center; width: 55px;">Affiliated ';
                nHTML += '<img id="imgAffiliatedHelp" src="Images/Icons/help.png" alt="Text Here" width="15" height="15" style="cursor: pointer; vertical-align: middle; padding: 0px 5px 5px 2px;" />';
                nHTML += '</th>';
                nHTML += '</tr>';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">AOR</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 55px;">System</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="border-left: 1px solid grey;">' + row.USERNAME + '</td>';
                    nHTML += '<td>' + row.AORRoleName + '</td>';
                    nHTML += '<td style="text-align: center;"><input type="checkbox" ' + (row.AORResource == 1 ? 'checked' : '') + ' disabled /></td>';
                    nHTML += '<td style="text-align: center;"><input type="checkbox" ' + (row.SystemResource == 1 ? 'checked' : '') + ' disabled /></td>';
                    nHTML += '</tr>';
                    resourceCount++;
                });

                nHTML += '</table>';
            }

            $('#divAffiliated').html(nHTML);
            $('#imgAffiliatedHelp').click(function () { imgAffiliatedHelp_click(); });
            SetResourceQty(resourceCount);
        }

        function getAffiliated_error() {
            $('#divAffiliated').html('Error gathering affiliated resources.');
        }

        function imgAffiliatedHelp_click() {
            var message = "Affiliated Resources are gathered by the following rules: <br /><br />";
            message += "AOR: <br />Resources associated to Work Tasks with the same AOR as this Work Task, whose Resource Type is associated with this Work Task's Work Activity. <br /><br />";
            message += "System: <br />Resources associated with the system suite this Work Task's associated system is a part of, whose Resource Type is associated with this Work Task's Work Activity, and is associated with this Work Task's Resource Group";
            MessageBox(message);
        }

        function showAOROptionSettings(obj) {
            var $objDiv = $(obj).closest('td').find('.aoroptionsettings');

            if ($objDiv.is(':visible')) {
                $objDiv.slideUp();
            }
            else {
                $('.aoroptionsettings').slideUp();

                var $pos = $(obj).position();

                $objDiv.css({
                    top: ($pos.top - 3),
                    left: ($pos.left + 23)
                }).slideDown();
            }
        }

        function getAOROptions(obj) {
            var systemAffiliated = 1, resourceAffiliated = 0, aorTypeAffiliated = 0, all = 0;
            var $objCheckboxes = $(obj).closest('td').find('.aoroptionsettings input[type=checkbox]');

            $.each($objCheckboxes, function () {
                switch ($(this).parent().text()) {
                    case 'Affiliated by selected System':
                        systemAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'Affiliated by selected Assigned To/Primary Resource':
                        resourceAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'Affiliated by AOR Workload Type':
                        aorTypeAffiliated = $(this).is(':checked') ? 1 : 0;
                        break;
                    case 'All (grouped by System)':
                        all = $(this).is(':checked') ? 1 : 0;
                        break;
                }
            });

            PageMethods.GetAOROptions($('#<%=this.ddlAssignedTo.ClientID %>').val(), $('#<%=this.ddlPrimaryResource.ClientID %>').val(), <%=this.SystemID %>, systemAffiliated, resourceAffiliated, $('#<%=this.ddlAssignedToRank.ClientID %>').val(), all, function (result) { getAOROptions_done(result, obj, aorTypeAffiliated); }, getAOROptions_error);
        }

        function getAOROptions_done(result, obj, aorTypeAffiliated) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            var $ddl = $(obj).closest('td').find('select:first');
            var $opt = $ddl.find('option:selected');
            var currentSystem = '';
            var currentAORType = ' ';
            var exists = false;

            $ddl.empty();
            nHTML += '<option value="0"></option>';

            $.each(dt, function (rowIndex, row) {
                if (row.AORType != "Release/Deployment MGMT" && row.AORType != currentAORType && $ddl.attr('field') !== 'Release/Deployment MGMT') {
                    if (aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                        currentAORType = row.AORType;
                        nHTML += '<option style="background-color: white" disabled>' + currentAORType + '</option>';
                    } else if (aorTypeAffiliated === 0) {
                        currentAORType = row.AORType;
                        nHTML += '<option style="background-color: white" disabled>' + currentAORType + '</option>';
                    }
                }

                if (row.WTS_SYSTEM != undefined && row.WTS_SYSTEM != currentSystem && aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                    currentSystem = row.WTS_SYSTEM;
                    nHTML += '<option style="background-color: ghostwhite;" disabled>' + currentSystem + '</option>';
                } else if ((row.WTS_SYSTEM != undefined && row.WTS_SYSTEM != currentSystem && (($ddl.attr('field') === 'Release/Deployment MGMT' && row.AORType === 'Release/Deployment MGMT')
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT'))
                    && aorTypeAffiliated === 0)) {
                    currentSystem = row.WTS_SYSTEM;
                    nHTML += '<option style="background-color: ghostwhite;" disabled>' + currentSystem + '</option>';
                }

                if (aorTypeAffiliated === 1 && $ddl.attr('field') === row.AORType) {
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
                } else if ((($ddl.attr('field') === 'Release/Deployment MGMT' && row.AORType === 'Release/Deployment MGMT')
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT' && $('#<%=this.ddlAssignedToRank.ClientID %>').val() != 31 && (row.AORID == 341 || row.AORID == 356 || row.AORID == 357))
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT' && $('#<%=this.ddlAssignedToRank.ClientID %>').val() == 31 && (row.AORID != 341 || row.AORID != 356 || row.AORID != 357)))
                    && aorTypeAffiliated === 0) {
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
                }
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

            if (<%=this.WorkItem_TaskID%> == 0 && dt[2]["AORID"] == 341) $ddl.val(dt[2]["AORReleaseID"]);
        }

        function getAOROptions_error() {

        }

        function getAORResourceTeamUser() {
            PageMethods.GetAORResourceTeamUser('<%=this.ParentRelAORReleaseID %>', getAORResourceTeamUser_done, on_error);
        }

        function getAORResourceTeamUser_done(result) {
            //remove all unselected resource team users
            $('#<%=this.ddlAssignedTo.ClientID %> option[og="Action Team"]').not(':selected').remove();

            try {
                var obj = jQuery.parseJSON(result);
                var opt = '<option value="' + obj[0].ResourceTeamUserID + '" og="Action Team" aorid="' + obj[0].AORID + '">' + obj[0].ResourceTeamUser + '</option>';

                //add new if it doesn't exist
                if ($('#<%=this.ddlAssignedTo.ClientID %> option[value="' + obj[0].ResourceTeamUserID + '"]').length == 0) {
                    $('#<%=this.ddlAssignedTo.ClientID %> option:first').after(opt);
                }
            }
            catch (e) { }

            verifyAORResourceTeamUser();
        }

        function btnAORResourceTeam_click() {
            var aorID = $('#<%=this.ddlAssignedTo.ClientID %> option:selected').attr('aorid');

            if (parseInt(aorID) > 0) {
                var nWindow = 'AOR';
                var nTitle = 'AOR';
                var nHeight = 700, nWidth = 1400;
                var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + aorID + '&AORReleaseID=' + '<%=this.ParentRelAORReleaseID %>' + '&Tab=ResourceTeam';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function verifyAORResourceTeamUser() {
            if ($('#<%=this.ddlAssignedTo.ClientID %> option:selected').attr('og') == 'Action Team') {
                $('#btnAORResourceTeam').show();
            }
            else {
                $('#btnAORResourceTeam').hide();
            }
        }

        function ddlAssignedToRank_change() {
            clearMessage();

            $('#<%=this.divWorkloadAORs.ClientID %> tr').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });
        }

        function displayBusinessReview() {
            var checked = $('#<%=this.chkBusinessReview.ClientID %>').is(':checked');

            $("label[for='<%=this.chkBusinessReview.ClientID %>']").css({
                'font-weight': checked ? 'bold' : 'normal',
                'background-color': checked ? 'red' : 'white',
                'color': checked ? 'white' : 'black',
                'padding': checked ? '5px' : '0px',
                'border-radius': checked ? '5px' : '0px'
            });
        }

        function imgHelp_click() {
            MessageBox("When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'.<br />Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA.<br />Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the subtask is ready to be 'Checked-In'.");
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'History':
                    buttonHistory_click();
                    break;
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

        function toggleMetrics() {
            if ($('#divMetrics').text().indexOf('Loading metrics') != -1) {
                PageMethods.LoadMetrics(_taskID, loadMetrics_done, on_error);
            }

            $('#divMetrics').slideToggle(function () {
                resizeFrames();                
            });
        }
        
        function getColorForName(name) {
            var color = '#000000';

            var red = 'Fail,Work Stoppage,Major,Deficiencies,Error,Missing,Deficient';
            var blue = 'Resolved,Accepted,Pass,Yes';
            var orange = 'Verified,Minor,Not Tested';

            if ((',' + red + ',').indexOf(',' + name + ',') != -1) {
                color = 'red';
            }
            else if ((',' + blue + ',').indexOf(',' + name + ',') != -1) {
                color = 'blue';
            }
            else if ((',' + orange + ',').indexOf(',' + name + ',') != -1) {
                color = 'orangered';
            }

            return color;
        }

        function loadMetrics_done(result) {
            //"RQMTS", "ACCEPTED", "CRITICALITY", "STAGE", "STATUS", "DEFECTS", "DEFECTSTATUS", "DEFECTIMPACT", "DEFECTSTAGE"            
            ds = $.parseJSON(result);
            if (ds == null) {
                $('#divMetrics').html('Metrics could not be loaded.');
                return;
            }

            var sys = $('[id$=txtSystemTask]').val();
            var wa = $('[id$=txtWorkArea]').val();
            if (wa != null) wa = wa.replace('0 - ', '');

            var dts = 'RQMTS,ACCEPTED,CRITICALITY,STAGE,STATUS,DEFECTS,DEFECTSTATUS,DEFECTIMPACT,DEFECTSTAGE'.split(',');

            var tables = [];
            tables.push('RQMTs (' + sys + '/' + wa + ')=RQMTS,ACCEPTED,CRITICALITY,STAGE,STATUS');
            tables.push('RQMT Defects (' + sys + '/' + wa + ')=DEFECTS,DEFECTSTATUS,DEFECTIMPACT,DEFECTSTAGE');

            var configs = [];
            configs['RQMTS'] = 'TotalRQMTs=Total';
            configs['ACCEPTED'] = '[SUBHEAD=Accepted]Yes,No';
            configs['CRITICALITY'] = '[SUBHEAD=Criticality][TR]Criticality,Total';
            configs['STAGE'] = '[SUBHEAD=PD2TDR][TR]RQMTStage,Total';
            configs['STATUS'] = '[SUBHEAD=Status][TR]RQMTStatus,Total';
            configs['DEFECTS'] = 'Defects=Total';
            configs['DEFECTSTATUS'] = '[SUBHEAD=Status]Verified,Resolved,Review';
            configs['DEFECTIMPACT'] = '[SUBHEAD=Impact][TR]DefectImpact,Total';
            configs['DEFECTSTAGE'] = '[SUBHEAD=PD2TDR][TR]DefectStage,Total';    

            var html = '';

            html += '<table>';

            for (var i = 0; i < tables.length; i++) {
                html += '<tr><td>';

                var tblList = tables[i];
                var idx = tblList.indexOf('=');
                var idx1 = 0;
                var idx2 = 0;
                var tblName = tblList.substring(0, idx);
                tblList = tblList.substring(idx + 1).split(',');

                html += '<span style="font-size:smaller;"><b>' + tblName + ':</b></span><br />';

                html += '<table cellspacing="0" cellpadding="1" border="1" style="border-collapse:collapse;border:1px solid grey;">';
                html += '  <tbody>';

                var headerTop = '<tr class="gridHeader">'
                var header = '<tr class="gridHeader">';
                var data = '<tr>'

                for (var x = 0; x < tblList.length; x++) {
                    var tblConfig = configs[tblList[x]];
                    var dt = ds[tblList[x]];

                    var subhead = '';
                    idx1 = tblConfig.indexOf('[SUBHEAD=');
                    if (idx1 != -1) {
                        idx2 = tblConfig.indexOf(']', idx1);
                        subhead = tblConfig.substring(idx1 + '[SUBHEAD='.length, idx2);
                        tblConfig = tblConfig.replace(tblConfig.substring(idx1, idx2 + 1), '');
                    }

                    var transpose = false;
                    if (tblConfig.indexOf('[TR]') != -1) {
                        transpose = true;
                        tblConfig = tblConfig.replace('[TR]', '');
                    }

                    var tblConfigCols = tblConfig.split(',');

                    if (transpose) {
                        var lblCol = tblConfigCols[0];
                        var dataCol = tblConfigCols[1];

                        for (var z = 0; z < dt.length; z++) {
                            var row = dt[z];

                            var l = row[lblCol];
                            var d = row[dataCol];

                            if (l == null) l = 'None';
                            if (d == null) d = '0';

                            if (z == 0) {
                                headerTop += '<th colspan="' + dt.length + '" align="center" style="font-size:smaller;border:1px solid grey;">' + subhead + '</th>';
                            }

                            var color = getColorForName(l);

                            header += '<th align="center" style="font-size:smaller;border:1px solid grey;">' + l + '</th>';
                            data += '<td align="center" style="font-size:smaller;border:1px solid grey;color:' + color + ';">' + d + '</th>';
                        }
                    }
                    else {
                        for (var y = 0; y < tblConfigCols.length; y++) {
                            var colConfig = tblConfigCols[y];
                            var colArr = null;
                            if (colConfig.indexOf('=') != -1) {
                                colArr = colConfig.split('=');
                            }
                            else {
                                colArr = (colConfig + '=' + colConfig).split('=');
                            }                        

                            var row = dt[0];

                            var color = getColorForName(colArr[1]);

                            if (subhead != '') {
                                if (y == 0) {
                                    headerTop += '<th colspan="' + tblConfigCols.length + '" align="center" style="font-size:smaller;border:1px solid grey;">' + subhead + '</th>';
                                }
                                header += '<th align="center" style="font-size:smaller;border:1px solid grey;">' + colArr[1] + '</th>';
                            }
                            else {
                                headerTop += '<th rowspan="2" valign="bottom" align="center" style="font-size:smaller;border:1px solid grey;">' + colArr[1] + '</th>';
                            }

                            var d = row[colArr[0]];
                            if (d == null) d = '0';

                            data += '<td align="center" style="font-size:smaller;border:1px solid grey;color:' + color + ';">' + d + '</td>';
                        }
                    }
                }                

                headerTop += '</tr>';
                header += '</tr>';
                data += '</tr>';

                html += headerTop;
                html += header;
                html += data;

                html += '  </tbody>';
                html += '</table>';
                
                html += '</td></tr>';
            }


            html += '</table>';

            $('#divMetrics').html(html);
        }

	</script>

	<script id="jsInit" type="text/javascript">

        function initializeControls() {

            $('.pageContainer').css('background-color', '#FAFAFA');
            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');
            $('[name="FirstLabel"]').width(140);

            //date pickers
            $('#<%=this.txtStartDate_Planned.ClientID %>').datepicker();
            $('#<%=this.txtStartDate_Actual.ClientID %>').datepicker();
            $('#<%=this.txtEndDate_Actual.ClientID %>').datepicker();
            $('#<%=this.txtDateNeeded.ClientID %>').datepicker();

            //html editor
            var w = '99%';// $('#<%=this.textAreaDescription.ClientID %>').width();
            var h = 150;
            if (($('#<%=this.textAreaDescription.ClientID %>').height() + 30) > 150) {
                h = $('#<%=this.textAreaDescription.ClientID %>').height() + 30;
            };

            $('#<%=this.txtTitle.ClientID %>').change(function () { input_change(this); });
            $('input').change(function () { input_change(this); });
            $('select').change(function () { input_change(this); });
            $('textarea').change(function () { input_change(this); });

            $('#<%=this.textAreaDescription.ClientID %>').cleditor({ width: w, height: h });

            $('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].change(function () { input_change($('#<%=this.textAreaDescription.ClientID %>')); });
            $($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc).css('height', (h - 28) + 'px');
            $($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc.body).css('height', (h - 45) + 'px');
            $($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc.body).css('background-color', '#F5F6CE');

            $('#<%=this.imgEffortHelp.ClientID %>').click(function () { MessageBox('XS = 0-4 Hours,S = 4-8 Hours, M = 8-24 Hours, L = 24-40 Hours, XL = 41-80 Hours'); });


            if (!_canEdit) {
                $('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].disable(true);
                $('[id$=buttonSave]').hide();
                $('#buttonSaveAndClose').hide();
                $('#buttonSaveAndClear').hide();

                $('input[type=text]').prop('disabled', true);
                $('select').prop('disabled', true);
            }

        }

        function initializeEvents() {

            $(window).resize(resizePage);

            $('[id$="imgRefresh"]').click(function () { refreshPage(_taskID, false); return false; });
            $('#<%=this.buttonSave.ClientID %>').click(function () { buttonSave_click(''); return false; });
            $('#buttonSaveAndClose').click(function () { buttonSave_click('close'); return false; });
            $('#buttonSaveAndClear').click(function () { buttonSave_click('clear'); return false; });
            $('#buttonClose').click(function () { buttonClose_click(); return false; });
            $('#aEdit').click(function () { aEdit_click(); return false; });
            $('#btnAORResourceTeam').click(function () { btnAORResourceTeam_click(); return false; });
            $('#<%=this.ddlAssignedTo.ClientID %>').change(function () { verifyAORResourceTeamUser(); });
            $('#<%=this.chkBusinessReview.ClientID %>').change(function () { displayBusinessReview(); return false; });
            $('#<%=this.ddlAssignedToRank.ClientID %>').change(function () { ddlAssignedToRank_change(); return false; });
            $('#imgHelp').click(function () { imgHelp_click(); });

            $('.hideSection').click(function (event) {
                showHideSection_click($(this).attr('id'), false, $(this).attr('sectionName'));
            });
            $('.showSection').click(function (event) {
                showHideSection_click($(this).attr('id'), true, $(this).attr('sectionName'));
            });

            $(document.body).bind('onbeforeunload', function () { closePage(); });
        }

        function initializeSections() {
            try {
                if (_taskID != 0) {
                    loadComments();
                    loadAttachments();
                    loadRequirements();
                    $('#imgShowComments').trigger('click');
                }
                else {
                    $('#tabNotes').prop('onclick', null).off('click');
                    $('#tabAttachments').prop('onclick', null).off('click');
                }

                return;
            } catch (e) {

            }
        }

        $(document).ready(function () {
            _pageUrls = new PageURLs();
            _taskID = parseInt('<%=this.WorkItem_TaskID %>');
            _workItemID = parseInt('<%=this.WorkItemID %>');
            if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
                _canEdit = true;
            }

            if ('<%=this.IsNew.ToString().ToUpper() %>' != 'TRUE' && _canEdit && '<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') $('#aEdit').show();

            initializeControls();

            $('#divTabsContainer').tabs({
                heightStyle: "fill"
                , collapsible: false
                , active: 0
            });

            if ('<%=this.IsNew.ToString().ToUpper() %>' == 'TRUE') {
                if (_canEdit) {
                    $('#buttonSaveAndClose').show();
                    $('#buttonSaveAndClear').show();
                }
                $("#divTabsContainer").tabs("option", "disabled", [1, 2]);
            }

            if ('<%=this.IsNew %>'.toUpperCase() == 'TRUE') $('#<%=menuRelatedItems.ClientID%>').css('display', 'none');

            initializeEvents();
            initializeSections();

            if ('<%=this.SaveComplete %>' == '1') {
                $('#labelMessage').text('Item updates have been saved.');
                $('#labelMessage').show();
            }
            else {
                clearMessage();
            }

            Tab_click('details');

            $('#<%=this.divWorkloadAORs.ClientID %> tr').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });

            resizePage();

            $("#helpButton").click(function () {
                clearMessage();

                var title = '', url = '';
                var h = 600, w = 1000;

                title = 'Subtask Attributes';
                url = _pageUrls.Maintenance.WorkItemDetailsHelp;

                //open in a popup
                var openPopup = popupManager.AddPopupWindow('Help', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
                if (openPopup) {
			        openPopup.Open();
			    }
            });

            $('#buttonMetrics').click(function () {
                toggleMetrics();
            });

            getAffiliated();
            getAORResourceTeamUser();
            displayBusinessReview();
		});
	</script>

</asp:Content>