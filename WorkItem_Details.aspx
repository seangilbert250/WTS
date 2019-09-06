﻿<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="WorkItem_Details.aspx.cs" Inherits="WorkItem_Details" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Task</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->        
    <script type="text/javascript" src="Scripts/workload.js"></script>
	<link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
    <style type="text/css">
        .auto-style3 {
            width: 82px;
        }
        .auto-style4 {
            width: 100%;
        }
        .auto-style5 {
            width: 200px;
            height: 11px;
        }
        .auto-style6 {
            width: 108px;
            height: 11px;
        }
        .auto-style7 {
            width: 82px;
            height: 11px;
        }
        .auto-style8 {
            width: 119px;
        }
        .auto-style9 {
            width: 25px;
        }
        </style>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="images/icons/pencil.png" alt="Review/Edit Task" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Primary Task</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server">
    <span id="labelMessage" style="color:red; padding-left:15px;"></span>
</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">

    <div id="CopySubTasks" style="display:none;">
	    <label for="<%=this.chkCopySubTasks.ClientID %>">Copy Subtasks</label>
	    <asp:CheckBox ID="chkCopySubTasks" runat="server" style="padding: 0px;" />
    </div>

    <img src="Images/Icons/help.png" id="helpButton" style="cursor: pointer"/>
	<iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="vertical-align: bottom; display: inline-block;"></iti_Tools_Sharp:Menu>
	<input type="button" id="buttonCancel" value="Cancel" style="padding: 1px 2px 1px 2px; width: 47px;" />
	<input type="button" id="buttonSave" value="Save" disabled="disabled" style="padding: 1px 2px 1px 2px; width: 42px;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<div id="divPageContainer" class="pageContainer" style="overflow-y: scroll; overflow-x: hidden;">
		<div id="divDetailsContainer" class="attributesRow" style="vertical-align:top;">
			<div id="divDetailsHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideDetails" class="hideSection" sectionname="Details" alt="Hide Task Details" title="Hide Task Details" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
					<img id="imgShowDetails" class="showSection" sectionname="Details" alt="Show Task Details" title="Show Task Details" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;">Primary Task Details:</div>
			</div>
			<div id="divDetails" class="attributesValue" style="padding: 10px 0px 10px 20px;">
				<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
					<tr id="trWorkRequestParent" style="display: none;">
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trWorkRequest" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Work Request:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlWorkRequest" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr id="trTitle">
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table CellPadding="0" CellSpacing="0" Style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trItemNumber" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">Primary Task:</td>
									<td class="attributesValue" style="width:50px;">
										<asp:TextBox ID="txtWorkloadNumber" runat="server" Width="40" BackColor="#cccccc" ReadOnly="true" Style="text-align: right;" />
									</td>
									<td class="attributesRequired">*</td>
									<td colspan="2" class="attributesValue" style="text-align:right;">
										<asp:TextBox ID="txtTitle" runat="server" Width="99%" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr id="trAuditing">
                        <td colspan="2" style="text-align: left; vertical-align: top;">
                            <table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr>
									<td class="attributesValue" style="padding-left: 140px;">Submitted:&nbsp;
										<asp:Label ID="labelCreated" runat="server" />
									</td>
                                    <td class="auto-style9">
    	    							<img id="imgAffiliated" runat="server" visible="false" src="Images/Icons/help.png" alt="Affiliated" title="Affiliated" width="15" height="15" style="cursor: pointer; display: block;" />
                                        </td>
                                    <td class="attributesValue" style="text-align: right; padding-right: 3px;">
										Updated:&nbsp;
										<asp:Label ID="labelUpdated" runat="server" />
									</td>
								</tr>
							</table>
                        </td>
					</tr>
                    <tr>
                        <td style="padding-bottom: 10px;">
                            <table cellpadding="0" cellspacing="0" style="vertical-align: top; text-align: left;">
                                <tr>
                                    <td class="attributesRequired">*</td>
						            <td name="LeftColumn" class="attributesLabel">
							            System(Task):
						            </td>
						            <td class="attributesValue">
							            <asp:DropDownList ID="ddlSystem" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
								            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
							            </asp:DropDownList>
						            </td>
                                </tr>
                                <tr>
                                    <td class="attributesRequired">&nbsp;</td>
						            <td name="LeftColumn" class="attributesLabel">
							            Contract:
						            </td>
						            <td class="attributesValue">
							            <asp:Label ID="spnContract" runat="server"></asp:Label>
						            </td>
                                </tr>
                            </table>
                        </td>
                        <td>
							<table style="border-collapse: collapse; width: 100%;">
						        <tr class="attributesRow">
                                    <td class="attributesRequired" style="vertical-align: top;"></td>
							        <td style="width: 750px;">
								        <div id="divReleaseAORs" runat="server" style="display: inline-block;"></div>&nbsp;&nbsp;
								        <div id="divWorkloadAORs" runat="server" style="display: inline-block; vertical-align: top;"></div>
							        </td>
                                    <td>
                                        <span id="lblWorkloadPriority" runat="server" style="font-weight: bold; vertical-align: top;">Workload Priority of Subtasks:<br /></span>
								        <asp:Label ID="spnWorkloadPriority" runat="server" style="text-align: center; vertical-align: top;">0.0.0.0.0.0 (0, 0%)</asp:Label>
                                    </td>
						        </tr>
					        </table>
						</td>
                    </tr>
					<tr>
						<td id="tdLeftColumn" style="vertical-align: top; width: 325px;">
							<table cellpadding="0" cellspacing="0" style="vertical-align: top; text-align: left;">
								<tr style="display: none;">
                                    <td class="attributesRequired">*</td>
                                    <td name="LeftColumn" class="attributesLabel">
								        PD<sup>2</sup>TDR Phase:
									</td>
                                    <td class="attributesValue">
										<asp:DropDownList ID="ddlPDDTDRPhase" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
										</asp:DropDownList>
                                    </td>
								</tr>
                                <tr id="trWorkType" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Resource Group:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlWorkType" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trWorkItemType" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Work Activity:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlWorkItemType" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trPriority" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Priority:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlPriority" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trStatus" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Status:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlStatus" runat="server" Style="font-size: 12px;  width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
										<img id="imgStatusHelp" runat="server" src="Images/Icons/help.png" alt="Help" title="Help" width="15" height="15" style="cursor: pointer; display: none;" />
									</td>
								</tr>
                                <tr id="trWorkArea" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Work Area:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlWorkArea" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
                                <tr id="trWorkloadGroup" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Functionality:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlWorkloadGroup" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
                                    <td class="attributesLabel" style="padding: 5px; display: none;">
										Bug Tracker ID:
									</td>
									<td class="attributesValue" style="display: none;">
										<asp:TextBox ID="txtBugTrackerID" runat="server" Style="width: 75px;" />
									</td>
								</tr>
                                <tr id="trProductVersion" class="attributesRow">
                                    <td id="tdProductVersionRequired" class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">
										Product Version:
									</td>
									<td class="attributesValue">
                                        <asp:Label ID="lblProductVersion" runat="server" />
										<asp:DropDownList ID="ddlProductVersion" runat="server" Style="font-size: 12px; width: 160px; display: none;" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
										<a id="aEdit" href="" style="color: blue; padding-left: 5px; display: none;">Edit</a>
										<asp:CheckBox ID="chkProduction" runat="server" style="padding: 0px; display: none;" />
										<label for="<%=this.chkProduction.ClientID %>" style="display: none;">Production</label>
									</td>
                                </tr>
                                <tr id="trProductionStatus" class="attributesRow">
                                    <td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Production Status:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlProductionStatus" runat="server" Style="font-size: 12px; padding-left: 0px; width: 160px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
                                    </td>
                                </tr>
								<tr id="trAllocationGroup" class="attributesRow" style="display: none;">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">
										Contract Allocation Group:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlAllocationGroup" runat="server" Style="font-size: 12px; width: 396px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
<%--										<asp:TextBox ID="txtAllocationGroup" runat="server" Style="width: 390px;" Enabled="false"/>--%>
									</td>
								</tr>
                                <tr id="trAllocation" class="attributesRow" style="display: none;">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">
										Contract Allocation Assign:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlAllocation" runat="server" Style="font-size: 12px; width: 396px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
						<td id="tdRightColumn">
							<table cellpadding="0" cellspacing="0" style=" width: 813px; text-align: left;">
								<tr id="trAssignedTo" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">
										Assigned To:
									</td>
									<td class="attributesValue" colspan="3" style="width: 190px;">
										<asp:DropDownList ID="ddlAssignedTo" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
                                        <input type="button" id="btnAORResourceTeam" value="..." style="display: none;" />
                                        <img id="imgAssignedToWarning" src="Images/Icons/exclamation_mark_red.png" title="Warning - Selected Assigned To resource is not affiliated with the task" alt="Warning - Selected Assigned To resource is not affiliated with the task" height="17" width="17" style="vertical-align: middle; cursor: pointer; display: none;" />
									</td>
                                    <td rowspan="8" style="width: 450px;">
                                        <div id="divAffiliated" style="text-align: center; width: 100%; height: 113px; overflow: auto; padding-left: 25px;"></div>
                                        <div id="divAORHistory" style="text-align: center; width: 100%; height: 60px; overflow: auto; padding-left: 25px; padding-top: 5px;"></div>
                                    </td>
								</tr>
								<tr id="trPrimaryResource" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">
										Primary Resource:
									</td>
									<td class="attributesValue" colspan="4">
										<asp:DropDownList ID="ddlPrimaryResource" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
                                        <img id="imgPrimaryResourceWarning" src="Images/Icons/exclamation_mark_red.png" title="Warning - Selected Primary Resource is not affiliated with the task" alt="Warning - Selected Primary Resource is not affiliated with the task" height="17" width="17" style="vertical-align: middle; cursor: pointer; display: none;" />
									</td>
									<td class="attributesLabel" style="padding: 5px; display:none;">
										Tech. Rank:
									</td>
									<td class="attributesValue">
										<asp:TextBox ID="txtResourcePriorityRank" runat="server" Style="width: 75px;display:none;" />
									</td>
								</tr>
                                <tr id="trTechRank" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel" style="display:none;">
										Primary Resource:
									</td>
									<td class="attributesValue" style="width:100px;display:none;">
										<asp:DropDownList ID="DropDownList1" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesLabel" >
										Assigned To Rank:
									</td>
									<td class="attributesValue" colspan="2">
										<asp:DropDownList ID="ddlAssignedToRank" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trSecondaryResource" class="attributesRow" style="display:none;">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">
										Secondary Tech. Resource:
									</td>
									<td class="attributesValue" style="width: 100px;">
										<asp:DropDownList ID="ddlSecondaryResource" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesLabel" style="padding: 5px">
										Secondary Tech. Rank:
									</td>
									<td class="attributesValue" colspan="2">
										<asp:TextBox ID="txtSecondaryTechRank" runat="server" Style="width: 75px;" />
									</td>
								</tr>
								<tr id="trPrimaryBusinessResource" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel" style="display:none;">
										Primary Bus. Resource:
									</td>
									<td class="attributesValue" style="width: 100px;display:none;">
										<asp:DropDownList ID="ddlPrimaryBusResource" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesLabel" >
										Customer Rank:
									</td>
									<td class="attributesValue" style="width: 100px;" colspan="2">
										<asp:TextBox ID="txtPrimaryBusRank" runat="server" Style="width: 75px;" />
									</td>
								</tr>
								<tr id="tr2" class="attributesRow" style="display:none;">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">
                                        Secondary Business Resource:
									</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlSecondaryBusResource" runat="server" Style="font-size: 12px; width: 160px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesLabel" style="padding: 5px">
										Secondary Bus. Rank:
									</td>
									<td class="attributesValue" colspan="2">
										<asp:TextBox ID="txtSecondaryBusRank" runat="server" Style="width: 75px;" />
									</td>
								</tr>
								<tr id="trDateNeeded" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">
										Date Needed:
									</td>
									<td colspan="4" class="attributesValue">
										<asp:TextBox ID="txtDateNeeded" runat="server" Width="75" />
									</td>
								</tr>
								<tr id="trSignedOff" class="attributesRow" style="display: none;">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">Signed Off:</td>
									<td class="attributesValue" colspan="4">
										<asp:CheckBox ID="chkSigned_Bus" runat="server" style="padding: 0px;" />
										<label id="lblSigned_Bus" runat="server" for="<%=this.chkSigned_Bus.ClientID %>">Bus</label>
										<br />
										<asp:CheckBox ID="chkSigned_Dev" runat="server" style="padding: 0px;" />
										<label id="lblSigned_Dev" runat="server" for="<%=this.chkSigned_Dev.ClientID %>">Dev</label>
									</td>
								</tr>
                                <tr id="trRecurring" class="attributesRow" style="display: none;">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">
										<asp:CheckBox ID="chkRecurring" runat="server" style="padding: 0px;"/>
									    <label for="<%=this.chkRecurring.ClientID %>">Recurring</label>
									</td>
									<td colspan="4" class="attributesValue"></td>
								</tr>
                                <tr id="trIVT" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">
										<asp:CheckBox ID="chkIVTRequired" runat="server" style="padding: 0px;" />
										<label for="<%=this.chkIVTRequired.ClientID %>">IVT Required</label>
									</td>
									<td colspan="4" class="attributesValue" style="vertical-align: bottom;">
										<label id="labelConnections" for="spanNumDependencies" style="cursor: pointer; text-align: left; padding: 0px; margin: 0px;">Dependent Items (<span id="spanNumDependencies" runat="server">0</span>)</label>
                                        <%--<img id="imgConnections" src="Images/Icons/find.png" alt="Task Connections" title="Task Connections" style="cursor:pointer; height:10px; width:10px;" />--%>
									</td>
								</tr>
                                <tr id="trCascadeAOR" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">
										<asp:CheckBox ID="chkCascadeAOR" runat="server" style="padding: 0px;"/>
									    <label for="<%=this.chkCascadeAOR.ClientID %>">Cascade AOR</label>
									</td>
									<td colspan="4" class="attributesValue">
                                        <img id="imgCascadeAORWarning" src="Images/Icons/exclamation_mark_red.png" title="Warning - No Workload MGMT AOR Selected" alt="Warning - No Workload MGMT AOR Selected" height="17" width="17" style="vertical-align: middle; cursor: pointer; display: none;" />
									</td>
								</tr>
                                <tr id="trBusinessReview" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" colspan="5" class="attributesLabel">
										<asp:CheckBox ID="chkBusinessReview" runat="server" style="padding: 0px;" />
							            <label for="<%=this.chkBusinessReview.ClientID %>">Requested to be reviewed by BA or SME</label>
                                        <img id="imgHelp" src="Images/Icons/help.png" alt="When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'. Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA. Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the task is ready to be 'Checked-In'." title="When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'. Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA. Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the task is ready to be 'Checked-In'." width="15" height="15" style="cursor: pointer; vertical-align: middle; padding: 0px 5px 5px 2px;" />
									</td>
								</tr>
                            </table>
						</td>
					</tr>
					<tr id="trPlanning" style="display: none;">
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="border-style: solid; border-width: thin; vertical-align: top; text-align: left;" class="auto-style4";>
								<tr id="trPlan" class="attributesRow">
									<td rowspan="4" class="attributesRequired" style="vertical-align: top;">
										<img id="imgHidePlanning" class="hideSection" sectionname="Planning" alt="Hide Planning" title="Hide Planning" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
										<img id="imgShowPlanning" class="showSection" sectionname="Planning" alt="Show Planning" title="Show Planning" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
									</td>
									<td rowspan="4" name="LeftColumn" class="attributesLabel" style="vertical-align: top;">
										Planning:
									</td>
									<td colspan="4" class="attributesValue">&nbsp;</td>
								</tr>
								<tr id="trPlanned" class="attributesRow" section="Planning" style="display: none;">
									<td class="attributesValue" style="width: 200px; text-align: left; padding: 0px;">
										Planned Design Start:
									</td>
									<td class="auto-style3" style="text-align: left;">
										<asp:TextBox ID="txtDesignStart_Planned" runat="server" Style="width: 75px;" />
									</td>
									<td class="attributesValue" style="width: 200px; text-align: left; padding-left: 20px;">
										Planned Dev Start:
									</td>
									<td class="auto-style8">
										<asp:TextBox ID="txtDevStart_Planned" runat="server" Style="width: 75px;" />
									</td>
									<td class="attributesValue" style="width: 200px; text-align: left; padding-left: 20px;">
										Estimated Completion:
									</td>
									<td class="attributesValue">
										<asp:TextBox ID="txtEstimatedCompletion" runat="server" Style="width: 75px;" />
									</td>
								</tr>
								<tr id="trActual" class="attributesRow" section="Planning" style="display: none;">
									<td class="attributesValue" style="width: 200px; text-align: left; padding: 0px;">
										Actual Design Start:
									</td>
									<td class="auto-style3" style="text-align: left;">
										<asp:TextBox ID="txtDesignStart_Actual" runat="server" Style="width: 75px;" />
									</td>
									<td class="attributesValue" style="width: 200px; text-align: left; padding-left: 20px;">
										Actual Dev Start:
									</td>
									<td class="auto-style8">
										<asp:TextBox ID="txtDevStart_Actual" runat="server" Style="width: 75px;" />
									</td>
   									<td class="attributesValue" style="width: 200px; text-align: left; padding-left: 20px;">
										Actual Completion:
									</td>
									<td class="attributesValue">
										<asp:TextBox ID="txtActualCompletion" runat="server" Style="width: 75px;" />
									</td>
								</tr>
								<tr id="trEstimated" class="attributesRow" section="Planning" style="display: none;">
									<td class="auto-style5" style="text-align: left; padding-left: 0px;">
										Estimated Effort:
									</td>
									<td class="auto-style7" style="text-align: left;">
										<asp:DropDownList ID="ddlHours_Planned" runat="server" Style="font-size: 12px; width: 80px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
	    							<td class="auto-style6" style="text-align: left;">
    	    							<img id="imgEffortHelp" runat="server" src="Images/Icons/help.png" alt="Help" title="Help" width="15" height="15" style="cursor: pointer; display: block;" />
    								</td>
								</tr>
							</table>
						</td>
					</tr>
                    <tr id="trDevelopment">
                        <td colspan="2" style="text-align: left; vertical-align: top;">
                            <table cellpadding="0" cellspacing="0" style="border-style: solid; border-width: thin; width: 100%; vertical-align: top; text-align: left;">
                                <tr id="trDev" class="attributesRow">
                                    <td rowspan="6" class="attributesRequired" style="vertical-align: top;">
                                        <img id="imgHideDevelopment" class="hideSection" sectionname="Development" alt="Hide Development" title="Hide Development" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
                                        <img id="imgShowDevelopment" class="showSection" sectionname="Development" alt="Show Development" title="Show Development" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                                    </td>
                                    <td rowspan="6" name="LeftColumn" class="attributesLabel" style="vertical-align: top;">Development:
                                    </td>
                                    <td colspan="4" class="attributesValue">&nbsp;</td>
                                </tr>
                                <tr id="trPercentComplete" class="attributesRow" section="Development">
                                    <td class="attributesLabel" style="width: 125px; text-align: left; padding: 0px;">Percent Complete:
                                    </td>
                                    <td class="attributesValue">
                                        <asp:DropDownList ID="ddlPercentComplete" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="trSR" class="attributesRow" section="Development">
                                    <td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">SR Number:
                                    </td>
                                    <td class="attributesValue" colspan="3">
                                        <asp:TextBox ID="txtSRNumber" runat="server" TextMode="Number" Style="width: 75px;" />
                                    </td>
                                </tr>
                                <tr id="trReproduced" class="attributesRow" style="display: none;">
                                    <td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">Reproduced:
                                    </td>
                                    <td class="attributesValue" style="width: 90px; text-align: left;">
                                        <asp:CheckBox ID="chkReproduced_Bus" runat="server" />
                                        <label for="<%=this.chkReproduced_Bus.ClientID %>" style="padding-left: 3px;">Business</label>
                                    </td>
                                    <td colspan="2" class="attributesValue" style="text-align: left; padding: 0px;">
                                        <asp:CheckBox ID="chkReproduced_Dev" runat="server" Style="padding: 0px;" />
                                        <label for="<%=this.chkReproduced_Dev.ClientID %>" style="padding-left: 3px;">Dev</label>
                                    </td>
                                </tr>
                                <tr id="trMenuType" class="attributesRow" style="display: none;">
                                    <td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">Menu Type:
                                    </td>
                                    <td class="attributesValue" colspan="3">
                                        <asp:DropDownList ID="ddlMenuType" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="trMenuName" class="attributesRow" style="display: none;">
                                    <td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">Menu Name:
                                    </td>
                                    <td class="attributesValue" colspan="3">
                                        <asp:DropDownList ID="ddlMenuName" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="trMilestones">
                        <td colspan="2" style="text-align: left; vertical-align: top;">
                            <table cellpadding="0" cellspacing="0" style="border-style: solid; border-width: thin; width: 100%; vertical-align: top; text-align: left;">
                                <tr id="trMilestones" style="white-space: nowrap; width: 100%">
                                    <td style="vertical-align: top; white-space: nowrap; width: 50%">
                                        <div>
                                            <img id="imgHideMilestones" class="hideSection" sectionname="Milestones" alt="Hide Milestones" title="Hide Milestones" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
                                            <img id="imgShowMilestones" class="showSection" sectionname="Milestones" alt="Show Milestones" title="Show Milestones" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
                                            <span id="spanMilestoneLabel" class="attributesLabel">Primary Task Milestones:
                                            </span>
                                        </div>
                                    </td>
                                    <td class="attributesLabel" style="vertical-align: top; width: 50%">Deployment:
                                    </td>
                                </tr>
                                <tr id="trMilestonesData" style="width: 100%; display: none" section="Milestones">
                                    <td id="tdLeftColumn" style="width: 50%; white-space: nowrap; vertical-align: top;">
                                        <table style="vertical-align: top; text-align: left; width: 100%">
                                            <tr style="width: 100%">
                                                <td style="width: 50%">
                                                    <table cellpadding="0" cellspacing="0" style="vertical-align: top; text-align: left;">
                                                        <tr id="trTotalDaysOpened" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Total Days Opened (#):
                                                                <asp:Label ID="labelTotalDaysOpened" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr id="trInProgressDate" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">In Progress Date:
                                                                <asp:Label ID="labelInProgressDate" runat="server" />
                                                            </td>
                                                             <td class="attributesValue">Time:
                                                                <asp:Label ID="labelInProgressTime" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr id="trReadyForReviewDate" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Ready For Review Date:
                                                                <asp:Label ID="labelReadyForReviewDate" runat="server" />
                                                            </td>
                                                            <td class="attributesValue">Time:
                                                                <asp:Label ID="labelReadyForReviewTime" runat="server" />
                                                             </td>
                                                        </tr>
                                                         <tr id="trClosedDate" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Closed Date:
                                                                <asp:Label ID="labelClosedDate" runat="server" />
                                                            </td>
                                                             <td class="attributesValue">Time:
                                                                <asp:Label ID="labelClosedTime" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="width: 50%">
                                                    <table cellpadding="0" cellspacing="0" style="vertical-align: top; text-align: left;">
                                                        <tr id="trTotalBusinessDaysOpened" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Total Business Days Opened (#):
                                                                <asp:Label ID="labelTotalBusinessDaysOpened" runat="server" />
                                                            </td>
                                                        </tr>

                                                        <tr id="trTotalDaysInProgress" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Days In Progress (#):
                                                                <asp:Label ID="labelTotalDaysInProgress" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr id="trTotalDaysReadyForReview" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Days Waiting (#):
                                                                <asp:Label ID="labelTotalDaysReadyForReview" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr id="trTotalDaysClosed" class="attributesRow">
                                                            <td class="attributesValue" style="width: 90px;">Days Waiting (#):
                                                                <asp:Label ID="labelTotalDaysClosed" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="tdRightColumn" style="width: 50%; border-left: solid; border-width: thin;">
                                        <table>
                                            <tr id="trDeployed_Comm" class="attributesRow" section="Deployment" style="text-align: right;">
                                                <td class="attributesValue" style="width: 10px; text-align: left; padding: 0px;">
                                                    <asp:CheckBox ID="chkDeployed_Comm" runat="server" Style="padding: 0px;" />
                                                </td>
                                                <td class="attributesValue" style="width: 90px;">
                                                    <label for="<%=this.chkDeployed_Comm.ClientID %>">Commercial</label>
                                                </td>
                                                <td class="attributesValue" style="padding-left: 15px; width: 215px;">By:
										<asp:DropDownList ID="ddlDeployedBy_Comm" runat="server">
                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                                </td>
                                                <td class="attributesValue">On:
										<asp:TextBox ID="txtDeployedDate_Comm" runat="server" Style="width: 75px;" />
                                                </td>
                                            </tr>
                                            <tr id="trDeployed_Test" class="attributesRow" section="Deployment" style="text-align: right;">
                                                <td class="attributesValue" style="width: 10px; text-align: left; padding: 0px;">
                                                    <asp:CheckBox ID="chkDeployed_Test" runat="server" Style="padding: 0px;" />
                                                </td>
                                                <td class="attributesValue" style="width: 90px;">
                                                    <label for="<%=this.chkDeployed_Test.ClientID %>">.mil Test</label>
                                                </td>
                                                <td class="attributesValue" style="padding-left: 15px; width: 215px;">By:
										<asp:DropDownList ID="ddlDeployedBy_Test" runat="server">
                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                                </td>
                                                <td class="attributesValue">On:
										<asp:TextBox ID="txtDeployedDate_Test" runat="server" Style="width: 75px;" />
                                                </td>
                                            </tr>
                                            <tr id="trDeployed_Prod" class="attributesRow" section="Deployment" style="text-align: right;">
                                                <td class="attributesValue" style="width: 10px; text-align: left; padding: 0px;">
                                                    <asp:CheckBox ID="chkDeployed_Prod" runat="server" Style="padding: 0px;" />
                                                </td>
                                                <td class="attributesValue" style="width: 90px;">
                                                    <label for="<%=this.chkDeployed_Prod.ClientID %>">Production</label>
                                                </td>
                                                <td class="attributesValue" style="padding-left: 15px; width: 215px;">By:
										<asp:DropDownList ID="ddlDeployedBy_Prod" runat="server">
                                            <asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                                </td>
                                                <td class="attributesValue">On:
										<asp:TextBox ID="txtDeployedDate_Prod" runat="server" Style="width: 75px;" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>

                                </tr>
                            </table>
                        </td>
                    </tr>
					<tr id="trTestingReview" style="display: none;">
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="border-style: solid; border-width: thin; width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
								<tr id="trTesting" class="attributesRow">
									<td rowspan="4" class="attributesRequired" style="vertical-align: top;">
										<img id="imgHideTesting" class="hideSection" sectionname="Testing" alt="Hide Testing" title="Hide Testing" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
										<img id="imgShowTesting" class="showSection" sectionname="Testing" alt="Show Testing" title="Show Testing" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
									</td>
									<td rowspan="4" name="LeftColumn" class="attributesLabel" style="vertical-align: top;">
										Testing/Review:
									</td>
									<td colspan="3" class="attributesValue">&nbsp;</td>
								</tr>
								<tr id="trCVTStep" class="attributesRow" section="Testing" style="display: none;">
									<td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">CVT Step:
									</td>
									<td class="attributesValue" style=" width:125px; text-align: left;">
										<asp:TextBox ID="txtCVTStep" runat="server" Style="width: 100px;" />
									</td>
									<td class="attributesValue">&nbsp;</td>
								</tr>
								<tr id="trCVTStatus" class="attributesRow" section="Testing" style="display: none;">
									<td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">CVT Status:
									</td>
									<td class="attributesValue" style="width: 125px; text-align: left;">
										<asp:DropDownList ID="ddlCVTStatus" runat="server" Style="width:105px; font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="Not Ready" Value="0"></asp:ListItem>
											<asp:ListItem Text="Pass" Value="1"></asp:ListItem>
											<asp:ListItem Text="Fail" Value="2"></asp:ListItem>
											<asp:ListItem Text="Accept" Value="3"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesValue">&nbsp;</td>
								</tr>
								<tr id="trTester" class="attributesRow" section="Testing" style="display: none;">
									<td class="attributesValue" style="width: 125px; text-align: left; padding: 0px;">Tester:
									</td>
									<td class="attributesValue" style="width: 125px; text-align: left;">
										<asp:DropDownList ID="ddlTester" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td class="attributesValue">&nbsp;</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>
		</div>
		<div id="divDescriptionContainer" class="attributesRow" style="vertical-align: top;">
			<div id="divDescriptionHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideDescription" class="hideSection" sectionname="Description" alt="Hide Primary Task Description" title="Hide Primary Task Description" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
					<img id="imgShowDescription" class="showSection" sectionname="Description" alt="Show Primary Task Description" title="Show Primary Task Description" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;">
					Description:
				</div>
			</div>
			<div id="divDescription" class="attributesValue" style="padding: 10px 0px 10px 20px;">
				<textarea id="textAreaDescription" runat="server" rows="12" style="width:98%;"></textarea>
			</div>
		</div>
		<div id="divCommentsContainer" class="attributesRow" style="vertical-align: top;">
			<div id="divCommentsHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideComments" class="hideSection" sectionname="Comments" alt="Hide Primary Task Comments" title="Hide Primary Task Comments" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
					<img id="imgShowComments" class="showSection" sectionname="Comments" alt="Show Primary Task Comments" title="Show Primary Task Comments" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;">
					<span id="spanCommentsLabel">
						Comments:
					</span>
				</div>
			</div>
			<div id="divComments" class="attributesValue" style="padding: 10px 0px 10px 20px; display: none;">
				<iframe id="frameComments" src="javascript:'';" frameborder="0" scrolling="no" style="display: block; width: 100%;"></iframe>
			</div>
		</div>
		<div id="divTaskContainer" class="attributesRow" style="vertical-align: top;">
			<div id="divTasksHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideTasks" class="hideSection" sectionname="Tasks" alt="Hide Subtasks" title="Hide Subtasks" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
					<img id="imgShowTasks" class="showSection" sectionname="Tasks" alt="Show Subtasks" title="Show Subtasks" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;"><span id="spanTaskQtyLabel">Subtasks:</span></div>
				<img id="imgAddTask" alt="Add Task" title="Add Task" src="Images/Icons/add_blue.png" height="10" width="10" style="display: none;" />
			</div>
			<div id="divTasks" class="attributesValue" style="padding: 10px 0px 10px 20px;">
				<iframe id="frameTasks" src="javascript:'';" frameborder="0" scrolling="no" style="display: block; width: 100%;"></iframe>
			</div>
		</div>
        <div id="divRequirementsContainer" class="attributesRow" style="vertical-align: top;height:99%">
			<div id="divRequirementsHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideRequirements" class="hideSection" sectionname="Requirements" alt="Hide Requirements" title="Hide Requirements" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;display: none;" />
					<img id="imgShowRequirements" class="showSection" sectionname="Requirements" alt="Show Requirements" title="Show Requirements" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;  " />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;">Requirements:</div>
			</div>
			<div id="divRequirements" class="attributesValue" style="padding: 0px 0px 0px 0px;height:500px;">
				<iframe id="frameRQMTs" name="frameRQMTs" src="javascript:'';" scrolling="no" frameborder="0"  style="display: block; width: 100%;height:500px;"></iframe>
			</div>
		</div>
	</div>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script src="Scripts/cleditor/jquery.cleditor.js"></script>
	
	<script id="jsVariables" type="text/javascript">
		var _canEdit = false;
		var _pageUrls;
		var _id = 0;
		var _isNew = false;
		var _userID = 0;
		var _statusID = 1, _workAreaID = 0, _allocationID = 0, _AllocationGroupId = 0, _RequestedID = 0;
		var _commentQty = 0, _attachmentQty = 0, _wrAttachmentQty = 0, _subTaskQty = 0;
		var _sourceWorkItemID, copySubTasks;
        var _selectedAssigned = '<%=SelectedAssigned%>';
		var _selectedStatuses = '<%=SelectedStatuses%>';
	</script>

	<script id="jsAJAX" type="text/javascript">

	    function isNumeric(input)  // Using this for ranks, which are OK if blank, otherwise check for garbage
	    {
            return (input - 0) == input && ('' + input).trim().length > 0;
	    }

	    function buildWorkItemObject() {
			var wi = {};// new WorkloadItem();

			try {
				var descr = encodeURIComponent($('#<%=this.textAreaDescription.ClientID %>').val());
				wi.WorkItemID = _id;
				wi.BugTracker_ID = _id;
				wi.WorkRequestID = parseInt($('#<%=this.ddlWorkRequest.ClientID %> option:selected').val());
				wi.WorkItemTypeID = parseInt($('#<%=this.ddlWorkItemType.ClientID %> option:selected').val()); 
			    wi.WTS_SystemID = parseInt($('#<%=this.ddlSystem.ClientID %> option:selected').val()); 
				wi.ProductVersionID = parseInt($('#<%=this.ddlProductVersion.ClientID %> option:selected').val());
				wi.Production = $('#<%=this.chkProduction.ClientID %>').prop('checked');
				wi.Recurring = $('#<%=this.chkRecurring.ClientID %>').prop('checked');
				wi.SR_Number = $('#<%=this.txtSRNumber.ClientID %>').val() == '' ? 0 : $('#<%=this.txtSRNumber.ClientID %>').val();
				wi.Reproduced_Biz = $('#<%=this.chkReproduced_Bus.ClientID %>').prop('checked');
				wi.Reproduced_Dev = $('#<%=this.chkReproduced_Dev.ClientID %>').prop('checked');
			    wi.PriorityID = parseInt($('#<%=this.ddlPriority.ClientID %> option:selected').val()); 
				wi.AllocationID = parseInt($('#<%=this.ddlAllocation.ClientID %> option:selected').val()); 
				wi.MenuTypeID = parseInt($('#<%=this.ddlMenuType.ClientID %> option:selected').val());
				wi.MenuNameID = parseInt($('#<%=this.ddlMenuName.ClientID %> option:selected').val());
				wi.AssignedResourceID = parseInt($('#<%=this.ddlAssignedTo.ClientID %> option:selected').val());
			    wi.PrimaryResourceID = parseInt($('#<%=this.ddlPrimaryResource.ClientID %> option:selected').val());
			    wi.ResourcePriorityRank = $('#<%=this.txtResourcePriorityRank.ClientID %>').val() == '' ? 10 : $('#<%=this.txtResourcePriorityRank.ClientID %>').val();
			    //wi.ResourcePriorityRank = $('#<%=this.txtResourcePriorityRank.ClientID %>').val();
			    if (!isNumeric(wi.ResourcePriorityRank))
			    {
			        MessageBox('Ranks must be numeric');
			        return;
			    }
			    wi.SecondaryResourceID = parseInt($('#<%=this.ddlSecondaryResource.ClientID %> option:selected').val());
			    wi.SecondaryResourceRank = $('#<%=this.txtSecondaryTechRank.ClientID %>').val() == '' ? 10 : $('#<%=this.txtSecondaryTechRank.ClientID %>').val();
			    //wi.SecondaryResourceRank = $('#<%=this.txtSecondaryTechRank.ClientID %>').val();
			    if (!isNumeric(wi.SecondaryResourceRank))
			    {
			        MessageBox('Ranks must be numeric');
			        return;
			    }
			    wi.PrimaryBusinessResourceID = parseInt($('#<%=this.ddlPrimaryBusResource.ClientID %> option:selected').val());
			    wi.PrimaryBusinessRank = $('#<%=this.txtPrimaryBusRank.ClientID %>').val() == '' ? 10 : $('#<%=this.txtPrimaryBusRank.ClientID %>').val();
			    //wi.PrimaryBusinessRank = $('#<%=this.txtPrimaryBusRank.ClientID %>').val();
			    if (!isNumeric(wi.PrimaryBusinessRank))
    	        {
    	            MessageBox('Ranks must be numeric');
			        return;
			    }
			    wi.SecondaryBusinessResourceID = parseInt($('#<%=this.ddlSecondaryBusResource.ClientID %> option:selected').val());
			    wi.SecondaryBusinessRank = $('#<%=this.txtSecondaryBusRank.ClientID %>').val() == '' ? 10 : $('#<%=this.txtSecondaryBusRank.ClientID %>').val();
			    //wi.SecondaryBusinessRank = $('#<%=this.txtSecondaryBusRank.ClientID %>').val();
			    if (!isNumeric(wi.SecondaryBusinessRank))
                {
    	            MessageBox('Ranks must be numeric');
			        return;
			    }
			    wi.WorkTypeID = parseInt($('#<%=this.ddlWorkType.ClientID %> option:selected').val());
			    wi.StatusID = parseInt($('#<%=this.ddlStatus.ClientID %> option:selected').val());
			    wi.IVTRequired = $('#<%=this.chkIVTRequired.ClientID %>').prop('checked');
				wi.NeedDate = $('#<%=this.txtDateNeeded.ClientID %>').val();
				wi.EstimatedEffortID = $('#<%=this.ddlHours_Planned.ClientID %>').val();
				wi.EstimatedCompletionDate = $('#<%=this.txtEstimatedCompletion.ClientID %>').val()
				wi.CompletionPercent = parseInt($('#<%=this.ddlPercentComplete.ClientID %> option:selected').val());
				wi.WorkloadGroupID = parseInt($('#<%=this.ddlWorkloadGroup.ClientID %> option:selected').val());
				wi.WorkAreaID = parseInt($('#<%=this.ddlWorkArea.ClientID %> option:selected').val());
			    wi.Title = $('#<%=this.txtTitle.ClientID %>').val(); 
				wi.Description = descr;
				wi.Archive = false;
				wi.Deployed_Comm = $('#<%=this.chkDeployed_Comm.ClientID %>').prop('checked');
				wi.Deployed_Test = $('#<%=this.chkDeployed_Test.ClientID %>').prop('checked');
				wi.Deployed_Prod = $('#<%=this.chkDeployed_Prod.ClientID %>').prop('checked');
				wi.DeployedBy_CommID = parseInt($('#<%=this.ddlDeployedBy_Comm.ClientID %> option:selected').val());
				wi.DeployedBy_TestID = parseInt($('#<%=this.ddlDeployedBy_Test.ClientID %> option:selected').val());
				wi.DeployedBy_ProdID = parseInt($('#<%=this.ddlDeployedBy_Prod.ClientID %> option:selected').val());
				wi.DeployedDate_Comm = $('#<%=this.txtDeployedDate_Comm.ClientID %>').val();
				wi.DeployedDate_Test = $('#<%=this.txtDeployedDate_Test.ClientID %>').val();
				wi.DeployedDate_Prod = $('#<%=this.txtDeployedDate_Prod.ClientID %>').val();
				wi.PlannedDesignStart = $('#<%=this.txtDesignStart_Planned.ClientID %>').val();
				wi.PlannedDevStart = $('#<%=this.txtDevStart_Planned.ClientID %>').val();
				wi.ActualDesignStart = $('#<%=this.txtDesignStart_Actual.ClientID %>').val();
			    wi.ActualDevStart = $('#<%=this.txtDevStart_Actual.ClientID %>').val();
			    wi.ActualCompletionDate = $('#<%=this.txtActualCompletion.ClientID %>').val();
				wi.CVTStep = $('#<%=this.txtCVTStep.ClientID %>').val();
				wi.CVTStatus = $('#<%=this.ddlCVTStatus.ClientID %> option:selected').text();
				wi.TesterID = parseInt($('#<%=this.ddlTester.ClientID %> option:selected').val());
				wi.Signed_Bus = $('#<%=this.chkSigned_Bus.ClientID %>').prop('checked');
				wi.Signed_Dev = $('#<%=this.chkSigned_Dev.ClientID %>').prop('checked');
			    wi.ProductionStatusID = parseInt($('#<%=this.ddlProductionStatus.ClientID %> option:selected').val());
			    wi.PDDTDR_PHASEID = parseInt($('#<%=this.ddlPDDTDRPhase.ClientID %> option:selected').val());

			    wi.AssignedToRankID = parseInt($('#<%=this.ddlAssignedToRank.ClientID %> option:selected').val());
                wi.BusinessReview = $('#<%=this.chkBusinessReview.ClientID %>').prop('checked');
			} catch (e) {
			    //alert("Error building WorkItemObject. " + e.message);
			}

			return wi;
		}

        function save() {
            try {
                if ($('#frameTasks')[0].contentWindow.parentSave()) {
                    $('#frameTasks')[0].contentWindow.save();
                }
            } catch (e) {}
            
			var workItem = buildWorkItemObject();

            copySubTasks  = $('#<%=this.chkCopySubTasks.ClientID %>').prop('checked');
		    _sourceWorkItemID = +'<%=this.SourceWorkItemID%>';

            var arrAORs = [];
            var cascadeAOR = false;
            
            $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('select').val() > 0) { 
                    arrAORs.push({ 'aorreleaseid': $obj.find('select').val(), 'aorworktypeid': '2' });
                }
                else {
                    arrAORs.push({ 'aorreleaseid': '0', 'aorworktypeid': '2' });
                }
            });
            $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('select').val() > 0) {
                    arrAORs.push({ 'aorreleaseid': $obj.find('select').val(), 'aorworktypeid': '1' });
                    cascadeAOR = $('#<%=this.chkCascadeAOR.ClientID %>').prop('checked');
                }
                else {
                    arrAORs.push({ 'aorreleaseid': '0', 'aorworktypeid': '1' });
                    cascadeAOR = $('#<%=this.chkCascadeAOR.ClientID %>').prop('checked');
                }
            });
            var nAORsJSON = '{save:' + JSON.stringify(arrAORs)  + '}';

			try {
				PageMethods.SaveWorkItem(
                    workItem, copySubTasks, _sourceWorkItemID, nAORsJSON, cascadeAOR, save_done, on_error);
			} catch (e) {
				MessageBox('An error occurred gathering data to save.' + '\n' + e.message, 'Save Error');
			}
		}
		
		function save_done(result) {
			var saved = false;
			var id = 0;
			var errorMsg = '';

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
				}

				if (saved) {
					$('#labelMessage').text('Item updates have been saved.');
					$('#labelMessage').show();


					if (parent) {
						if (parent.opener && parent.opener.refreshPage) {
							parent.opener.refreshPage(true);
						}
						else if (parent.parent && parent.parent.ShowFrameForWorkloadItem) {
						    parent.parent.ShowFrameForWorkloadItem(false, id, id, true, false, _selectedStatuses, _selectedAssigned);
						}

						if (_id == 0) {
							var url = parent.location.href;
							url = editQueryStringValue(url, 'workItemID', id);

							parent.location.href = url;
						}
					}

					refreshPage(id);
				}
				else {
					MessageBox('Failed to save Workload Item. \n' + errorMsg, 'Save Failed');
				}
			}
			catch(e) { }
		}

		function on_error(result) {
			/*var resultText = 'An error occurred when communicating with the server';\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

			//MessageBox('save error:  \n' + resultText, 'Server Error');
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

            PageMethods.GetAOROptions($('#<%=this.ddlAssignedTo.ClientID %>').val(), $('#<%=this.ddlPrimaryResource.ClientID %>').val(), $('#<%=this.ddlSystem.ClientID %>').val(), systemAffiliated, resourceAffiliated, $('#<%=this.ddlAssignedToRank.ClientID %>').val(), all, function (result) { getAOROptions_done(result, obj, aorTypeAffiliated); }, getAOROptions_error);
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
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '" workloadAllocationID="' + row.WorkloadAllocationID + '" workloadAllocation="' + row.WorkloadAllocation + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
                } else if ((($ddl.attr('field') === 'Release/Deployment MGMT' && row.AORType === 'Release/Deployment MGMT')
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT' && $('#<%=this.ddlAssignedToRank.ClientID %>').val() != 31 && (row.AORID == 341 || row.AORID == 356 || row.AORID == 357))
                    || ($ddl.attr('field') !== 'Release/Deployment MGMT' && row.AORType !== 'Release/Deployment MGMT' && $('#<%=this.ddlAssignedToRank.ClientID %>').val() == 31 && (row.AORID != 341 || row.AORID != 356 || row.AORID != 357)))
                    && aorTypeAffiliated === 0) {
                    nHTML += '<option value="' + row.AORReleaseID + '" aorid="' + row.AORID + '" workloadAllocationID="' + row.WorkloadAllocationID + '" workloadAllocation="' + row.WorkloadAllocation + '">' + row.AORID + ' (' + row.WorkloadAllocationAbbreviation + ') - ' + row.AORName + '</option>';
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
                $ddl.prepend('<option value="0"></option>');
                $ddl.val($opt.val());
            }
            else {
                $ddl.prepend($opt);
                if ($opt.val() > 0) {
                    $ddl.prepend('<option value="0"></option>');
                }
                else {
                    $ddl.val($opt.val());
                }
            }

            if (<%=this.WorkItemID%> == 0) {
                if (dt[2]["AORID"] == 341) {
                    $ddl.val(dt[2]["AORReleaseID"]);
                } else {
                    $ddl.val(0);
                }
            }

            chkCascadeAOR_change();
        }

	    function getAOROptions_error() {
	        
	    }

		function getAffiliated() {
		    $('#divAffiliated').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

		    var arrAORs = [];

		    if (_canEdit) {
                $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function() {
				    var $obj = $(this);

                    if ($obj.find('select').val() > 0) arrAORs.push($obj.find('select').val());
                });
		    }
		    else {
		        arrAORs.push('-1');
		    }
                
            PageMethods.GetAffiliated(_id, $('#<%=this.ddlSystem.ClientID %>').val(), $('#<%=this.ddlProductVersion.ClientID %>').val(), $('#<%=this.ddlWorkType.ClientID %>').val(), $('#<%=this.ddlWorkItemType.ClientID %>').val(), arrAORs.join(','), getAffiliated_done, getAffiliated_error);
        }

	    function getAffiliated_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

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
                });

                nHTML += '</table>';
            }

            $('#divAffiliated').html(nHTML);
            $('#imgAffiliatedHelp').click(function () { imgAffiliatedHelp_click(); });
            verifyResources();
        }

	    function getAffiliated_error() {
	        $('#divAffiliated').html('Error gathering affiliated resources.');
	        verifyResources();
        }

        function getAORHistory() {
            $('#divAORHistory').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.GetAORHistory(_id, getAORHistory_done, getAORHistory_error);
        }

        function getAORHistory_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No AOR History';
            }
            else {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 100px;">Product Version</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center;">AOR</th>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="border-left: 1px solid grey; text-align: center;">' + row.ProductVersion + '</td>';
                    nHTML += '<td>' + row.AOR + '</td>';
                    nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divAORHistory').html(nHTML);
        }

        function getAORHistory_error() {
            $('#divAORHistory').html('Error gathering AOR history.');
        }
	</script>

	<script id="jsSubsections" type="text/javascript">

		function imgAddTask_click() {
			var frame = $('#frameTasks')[0];

			if (frame && frame.contentWindow
				&& frame.contentWindow.addTask) {
				frame.contentWindow.addTask();
			}
		}

		function loadTasks() {
			var url = window.location.search;
			url = editQueryStringValue(url, 'Saved', '0');
			url = 'Loading.aspx?Page=WorkItem_Tasks.aspx' + url;

			$('#frameTasks').attr('src', url);
		}

		function loadComments() {
			var url = window.location.search;
			url = editQueryStringValue(url, 'Saved', '0');
			url = 'Loading.aspx?Page=WorkItem_Comments.aspx' + url;

			$('#frameComments').attr('src', url);
		}

        function loadRequirements() {
            var systemID = $('#<%=this.ddlSystem.ClientID %>').val();
            var workAreaID = $('#<%=this.ddlWorkArea.ClientID %>').val();

            if (systemID > 0 && workAreaID > 0) {
                var url = 'Loading.aspx?Page=' + 'RQMT_Grid.aspx?GridType=RQMT&MyData=true&View=RQMT%20Defect%20Metrics&GridPageIndex=0&IsConfigured=True&CurrentLevel=1&Filter=SYSTEM_ID=' + systemID + '|WORKAREA_ID=' + workAreaID + '&SessionPageKey=TaskEdit&IgnoreUserFilters=true&ShowExport=false&ShowCOG=false&HideBuilderButton=true&ReadOnly=true&ShowSelectCheckboxes=false&ShowPageTitle=false&HideColumns=' + escape('RQMT Defect Metrics=RQMT Primary #,RQMT Defect Metrics=RQMT Primary');

                $('#frameRQMTs').attr('src', url);
            }
        }


		function showSubsections() {
			<%--var phaseId = $('#<%=this.ddlPDDTDRPhase.ClientID %> option:selected').val();
			var phase = $('#<%=this.ddlPDDTDRPhase.ClientID %> option:selected').text();
			
			switch (phase.toUpperCase()) {
				case "INVESTIGATION":
				case "PLANNING":
					$('#imgShowPlanning').trigger('click');
					break;
				case "DESIGN":
					$('#imgShowPlanning').trigger('click');
					$('#imgShowDevelopment').trigger('click');
					break;
				case "DEVELOP":
					//$('#imgShowPlanning').trigger('click');
					$('#imgShowDevelopment').trigger('click');
					$('#imgShowDeployment').trigger('click');
					break;
				case "TESTING":
					$('#imgShowDevelopment').trigger('click');
					$('#imgShowDeployment').trigger('click');
					$('#imgShowTesting').trigger('click');
					break;
				case "DEPLOY":
					$('#imgShowDevelopment').trigger('click');
					$('#imgShowDeployment').trigger('click');
					$('#imgShowTesting').trigger('click');
					break;
				case "REVIEW":
					$('#imgShowDevelopment').trigger('click');
					$('#imgShowTesting').trigger('click');
					break;
			}--%>

			$('#imgShowDevelopment').trigger('click');
            $('#imgShowDeployment').trigger('click');
		}

	</script>

	<script id="jsEvents" type="text/javascript">

		function SetCommentQty(qty) {
			if (!qty) {
				qty = 0;
			}

			$('#spanCommentsLabel').text('Comments (' + qty + '):');
			
			if (parent.SetCommentQty) {
				parent.SetCommentQty(qty);
			}
		}
		function SetAttachmentQty(qty) {
			//if (!qty) {
			//	$('#tabAttachments').text('Attachments (0)');
			//}
			//else {
			//	$('#tabAttachments').text('Attachments (' + qty + ')');
			//}

			if (parent.SetAttachmentQty) {
				parent.SetAttachmentQty(qty);
			}
		}
		function SetWRAttachmentQty(qty) {
			//if (!qty) {
			//	$('#tabWRAttachments').text('Work Request Attachments (0)');
			//}
			//else {
			//	$('#tabWRAttachments').text('Work Request Attachments (' + qty + ')');
			//}

			if (parent.SetWRAttachmentQty) {
				parent.SetWRAttachmentQty(qty);
			}
		}
		function SetTaskQty(workItemID, qty) {
			if (!qty) {
				qty = 0;
			}

			$('#spanTaskQtyLabel').text('Subtasks (' + qty + '):');

			if (parent.SetTaskQty) {
				parent.SetTaskQty(workItemID, qty);
			}
		}

		function refreshPage(newID) {
			var url = document.location.href;
			if (newID > 0) {
				url = editQueryStringValue(url, 'workItemID', newID);
				url = editQueryStringValue(url, 'Saved', '1');
			}
			else {
				url = editQueryStringValue(url, 'Saved', '0');
			}

			document.location.href = 'Loading.aspx?Page=' + url;
		}

		function resizePage() {
			resizePageElement('divPageContainer');
            resizeFrames();
		}

		function resizeFrames() {
			var frame;
			var fPageHeight = 0;

			$('iframe').each(function () {
				frame = $(this)[0];
                fPageHeight = 0;
                if ($(frame).attr('id') != undefined) {
                    if ($(frame).attr('id').indexOf('frameRQMTs') == -1) {
                        if (frame.contentWindow
                            && frame.contentWindow.document
                            && frame.contentWindow.document.body
                            && frame.contentWindow.document.body.offsetHeight) {
                            fPageHeight = frame.contentWindow.document.body.offsetHeight;
                            //if ($(frame).attr('id').indexOf('frameTasks') > -1 && fPageHeight < 250) {
                            //	fPageHeight = 300; //make sure tasks frame is tall enough for datepickers
                            //}
                        }
                        frame.style.height = fPageHeight + 'px';
                    }
                }
			});
		}

		function showHideSection_click(imgId, show, sectionName) {
			clearMessage();

			if (show) {
				$('#div' + sectionName).show();
				$('#' + imgId).hide();
				$('#' + imgId.replace('Show', 'Hide')).show();
				$('tr[section="' + sectionName + '"]').show();

				switch (sectionName) {
					case "Tasks":
						if ($('#frameTasks').attr('src') == "javascript:'';") {
							loadTasks();
						}
						break;
					case "Comments":
						if ($('#frameComments').attr('src') == "javascript:'';") {
							loadComments();
                        }
                    case "Requirements":
                        if ($('#frameRQMTs').attr('src') == "javascript:'';") {
                            loadRequirements();
                        }
						break;
				}
			}
			else {
				$('#div' + sectionName).hide();
				$('#' + imgId).hide();
				$('#' + imgId.replace('Hide', 'Show')).show();
				$('tr[section="' + sectionName + '"]').hide();
			}

            resizePage();
		}

		function clearMessage() {
			$('#labelMessage').text('');
			$('#labelMessage').hide();
		}

		function activateSaveButton() {
			clearMessage();

			if (_canEdit) {
				$('#buttonSave').removeAttr('disabled');
			}
		}

		function ddl_change(sender) {
			activateSaveButton();

			clearMessage();

			var $obj = $(sender);
			if ($obj.attr('field')) {
			    $obj.attr('fieldChanged', '1');
                $obj.closest('tr').attr('rowChanged', '1');
            }
            <%--if ($('#<%=this.ddlProductionStatus.ClientID %>').val() == 78
                || $('#<%=this.ddlProductionStatus.ClientID %>').val() == 108
                || $('#<%=this.ddlProductionStatus.ClientID %>').val() == 117) {
                $('#MgmtReleaseAORs').hide();
                $('#MgmtReleaseAORsMsg').show();
            } else {--%>
                $('#MgmtReleaseAORs').show();
                $('#MgmtReleaseAORsMsg').hide();
            //}
		}

		function txt_change(sender) {
			clearMessage();

			var original_value = '', new_value = '';
			if ($(sender).attr('original_value')) {
				original_value = $(sender).attr('original_value');
			}

			new_value = $(sender).val();

			if (new_value != original_value) {
				activateSaveButton();
			}
		}

		function chkDeployed_change(sender, environment) {
			clearMessage();

			//chkDeployed_<environment> : ddlDeployedBy_<environment> : txtDeployedDate_<environment>
			
			var checked = $(sender).prop('checked');
			var ddl = $('[id*="ddlDeployedBy_' + environment + '"]');
			var txt = $('[id*="txtDeployedDate_' + environment + '"]');

			if (checked) {
				if ($(ddl).val() == 0) {
					$(ddl).find('option').each(function () {
						if ($(this).val() == _userID) {
							$(ddl).val(_userID);
							return false;
						}
					});
				}

				var today = new Date();

				$(txt).val(today.getMonth() + 1 + '/' + today.getDate() + '/' + today.getFullYear());
			}
			else {
				$(ddl).val('0');
				$(txt).val('');
			}
		}

		/*
		Load Resource Group dropdown with available options
		*/
		function ddlWorkRequest_change() {
			clearMessage();
		}

		function ddlPDDTDRPhase_change() {
		    var optExists = false;
		    var PhaseID = $('#<%=ddlPDDTDRPhase.ClientID %> option:selected').val();
		    var workTypeId = $('#<%=ddlWorkType.ClientID %> option:selected').val();

			var selectOpt = {};
			$('#<%=this.ddlWorkType.ClientID %> option').each(function () {
				if ($(this).val() == '0') {
					selectOpt = this;
				}
				$(this).remove();
			});

            $.each(arrWorkType[0], function (rowindex, row) {
					var status = [];
					if (row.PhaseID == PhaseID) {
						newOption = '<option value="' + row.WorkTypeID + '"PhaseID="' + row.PhaseID + '">' + row.WorkType + '</option>';
						if (+row.WorkTypeID == +workTypeId) {
							optExists = true;
						}
						$('#<%=this.ddlWorkType.ClientID %>').append(newOption);
					}
				});

				if (optExists) {
					$('#<%=this.ddlWorkType.ClientID %>').val(workTypeId);
				}
				else {
					ddlWorkType_change();
				}

				if ($('#<%=this.ddlWorkType.ClientID %> option').length == 0) {
					$('#<%=this.ddlWorkType.ClientID %>').append(selectOpt);
				}
		}

	    function ddlWorkType_change() {
			clearMessage();

			var workTypeId = $('#<%=ddlWorkType.ClientID %> option:selected').val();
			var optExists = false;

			try {
				$('#<%=this.ddlStatus.ClientID %> option').each(function () {
					if ($(this).val() != '0') {
						$(this).remove();
					}
				});

			    var newOption = {};
				$.each(arrStatus[0], function (rowindex, row) {
					var status = [];
					if (row.WorkTypeID == workTypeId) {
						newOption = '<option value="' + row.STATUSID + '" WorkTypeID="' + row.WorkTypeID + '">' + row.STATUS + '</option>';
						if (+row.STATUSID == +_statusId) {
							optExists = true;
						}
						$('#<%=this.ddlStatus.ClientID %>').append(newOption);
					}
				});

				if (optExists) {
					$('#<%=this.ddlStatus.ClientID %>').val(_statusId);
				}

			    chkSigned_change();
			} catch (e) {

			}

			return false;
		}

        function ddlWorkItemType_change() {
            var workItemType = $('#<%=this.ddlWorkItemType.ClientID %> option:selected').text();

            if (workItemType == 'T2A - IVT/Peer Review') {
                $('#<%=this.chkIVTRequired.ClientID %>').prop('disabled', true);
            }
            else {
                $('#<%=this.chkIVTRequired.ClientID %>').prop('disabled', false);
            }
        }

	    function ddlAllocationGroup_change() {
	        clearMessage();
	        var AllocationGroupID = $('#<%=this.ddlAllocationGroup.ClientID %>').val();

	        updateAllocationFromGroup( AllocationGroupID);

	    }

		function ddlSystem_change() {
			clearMessage();

			var systemID = $('#<%=this.ddlSystem.ClientID %>').val();
			
			updateWorkAreaFromSystem(systemID);
			//updateAllocationGroupFromSystem(systemID);

			//updateAllocationFromSystem(systemID);

            var contract = $('#<%=this.ddlSystem.ClientID %>').find('option:selected').attr('contract');
            if (contract != undefined) {
                $('#<%=this.spnContract.ClientID%>').text(contract);
            }
            else {
                $('#<%=this.spnContract.ClientID%>').text('');
            }

            $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function() {
				var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Release/Deployment MGMT AORs') == -1) {
				    getAOROptions($obj.find('input[type=checkbox]:first'));
				}
            });

            $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });

            getAffiliated();
        }

        function ddlAssignedToRank_change() {
            clearMessage();

            $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });
        }

	    function ddlProductVersion_change() {
	        getAffiliated();
	    }

		function updateWorkAreaFromSystem(systemID) {
			try {
			    var optExists = false;
			    var addedCount = 0;

				$('#<%=this.ddlWorkArea.ClientID %>').empty();

				var newOption = {};
				$.each(arrWorkArea[0], function (rowindex, row) {
					if (row.WTS_SYSTEMID == null
						|| row.WTS_SYSTEMID == systemID) {
						newOption = '<option value="' + row.WorkAreaID + '" WTS_SYSTEMID="' + row.WTS_SYSTEMID + '">' + row.WorkArea + '</option>';
						if (+row.WorkAreaID == +_workAreaID) {
							optExists = true;
						}
						$('#<%=this.ddlWorkArea.ClientID %>').append(newOption);
					    addedCount += 1;
					}
				});
                // Changing to some Systems returned no rows, so put General Support in:
			    if (addedCount == 0)
			    {
			        newOption = '<option value=185 WTS_SYSTEMID=1>0 - General Support</option>';
					$('#<%=this.ddlWorkArea.ClientID %>').append(newOption);
			    }

				if (optExists) {
					$('#<%=this.ddlWorkArea.ClientID %>').val(_workAreaID);
				}
			} catch (e) { }
		}

	    function contains(a, obj) {
	        for (var i = 0; i < a.length; i++) {
	            if (a[i] === obj) {
	                return true;
	            }
	        }
	        return false;
	    }
  		function updateAllocationGroupFromSystem(systemID) {
			try {
			    var optExists = false;
			    var arrAdded = [];

				$('#<%=ddlAllocationGroup.ClientID %>').empty();
				$('#<%=ddlAllocation.ClientID %>').empty();

			    var newOption = {};
				$.each(arrAllocation[0], function (rowindex, row) {
				    if (row.WTS_SYSTEMID == null | row.WTS_SYSTEMID == systemID) {
				        if (!contains(arrAdded, row.ALLOCATIONGROUPID) & row.ALLOCATIONGROUP != null)
				        {
				            arrAdded.push(row.ALLOCATIONGROUPID);
				            newOption = '<option value="' + row.ALLOCATIONGROUPID + '" ALLOCATIONGROUPID="' + row.ALLOCATIONGROUPID + '">' + row.ALLOCATIONGROUP + '</option>';
							$('#<%=this.ddlAllocationGroup.ClientID %>').append(newOption);
				        }
					}
				});

				var $o = $('<option />');
				$o.text('-Select-');
				$o.val('0');
				$o.css('font-size', '12px');
				$o.css('font-family', 'Arial');
				$('#<%=this.ddlAllocationGroup.ClientID %>').prepend($o);
			    $('#<%=this.ddlAllocationGroup.ClientID %>').val(_allocationGroupID);
			} catch (e) { }
		}


		function updateAllocationFromSystem(systemID) {
			try {
				var optExists = false;

				$('#<%=this.ddlAllocation.ClientID %>').empty();

				var newOption = {};
				$.each(arrAllocation[0], function (rowindex, row) {
					if (row.WTS_SYSTEMID == null
						|| row.WTS_SYSTEMID == systemID) {
						newOption = '<option value="' + row.ALLOCATIONID + '" WTS_SYSTEMID="' + row.WTS_SYSTEMID + '">' + row.ALLOCATION + '</option>';
						if (+row.ALLOCATIONID == +_allocationID) {
							optExists = true;
						}
						else {
							$('#<%=this.ddlAllocation.ClientID %>').append(newOption);
						}
					}
				});

				var $o = $('<option />');
				$o.text('-Select-');
				$o.val('0');
				$o.css('font-size', '12px');
				$o.css('font-family', 'Arial');
				$('#<%=this.ddlAllocation.ClientID %>').prepend($o);
				
				if (optExists) {
					$('#<%=this.ddlAllocation.ClientID %>').val(_allocationID);
				}
				else {
					$('#<%=this.ddlAllocation.ClientID %>').val('0');
				}
			} catch (e) { }
		}

	    function updateAllocationFromGroup(allocationGroupId) {
			try {
			    var optExists = false;
			    var allocationIdArr = [];

				$('#<%=this.ddlAllocation.ClientID %>').empty();

				var newOption = {};
				$.each(arrAllocation[0], function (rowindex, row) {
				    if (row.ALLOCATIONGROUPID == allocationGroupId) {  //(row.WTS_SYSTEMID == null || row.WTS_SYSTEMID == systemID) && 
				        newOption = '<option value="' + row.ALLOCATIONID + '" ALLOCATIONGROUPID="' + row.ALLOCATIONGROUPID + '">' + row.ALLOCATION + '</option>';
						if (+row.ALLOCATIONID == +_allocationID) {
							optExists = true;
						}
						else {
						    if (allocationIdArr.indexOf(row.ALLOCATIONID) == -1)
                            {
						        allocationIdArr.push(row.ALLOCATIONID);
						        $('#<%=this.ddlAllocation.ClientID %>').append(newOption);
						    }
						}
					}
				});

				var $o = $('<option />');
				$o.text('-Select-');
				$o.val('0');
				$o.css('font-size', '12px');
				$o.css('font-family', 'Arial');
				$('#<%=this.ddlAllocation.ClientID %>').prepend($o);
				
				if (optExists) {
					$('#<%=this.ddlAllocation.ClientID %>').val(_allocationID);
				}
				else {
					$('#<%=this.ddlAllocation.ClientID %>').val('0');
				}
			} catch (e) { }

	    }

		function ddlAllocation_change() {
			//set default resources
			if (!_isNew
				|| !arrAllocation 
				|| !arrAllocation[0]
				) {
				return;
			}

			try {
				var allocationID = 0;
				var defaultAssignedToID = 0, defaultPrimaryTechID = 0, defaultPrimaryBusID = 0;

				allocationID = $('#<%=this.ddlAllocation.ClientID %>').val();

				$.each(arrAllocation[0], function (rowindex, row) {
					if (row.ALLOCATIONID == allocationID) {
						defaultAssignedToID = row.DefaultAssignedToID;
						defaultPrimaryTechID = row.DefaultTechnicalResourceID;
						defaultPrimaryBusID = row.DefaultBusinessResourceID;
						return false;
					}
				});

				//Uncomment the checks for value = 0 below if don't want to change once already set
				var ddl = $('#<%=this.ddlAssignedTo.ClientID %>')
				//if ($(ddl).val() == 0) {
					if ($('option[value="' + defaultAssignedToID + '"]', $(ddl))
						&& $('option[value="' + defaultAssignedToID + '"]', $(ddl)).length > 0) {
						$(ddl).val(defaultAssignedToID);
					}
					else {

					}
				//}

				ddl = $('#<%=this.ddlPrimaryResource.ClientID %>');
				//if ($(ddl).val() == 0) {
					if ($('option[value="' + defaultPrimaryTechID + '"]', $(ddl))
						&& $('option[value="' + defaultPrimaryTechID + '"]', $(ddl)).length > 0) {
						$(ddl).val(defaultPrimaryTechID);
					}
					else {
						$(ddl).val('0');
					}
				//}

				ddl = $('#<%=this.ddlPrimaryBusResource.ClientID %>');
				//if ($(ddl).val() == 0) {
					if ($('option[value="' + defaultPrimaryBusID + '"]', $(ddl))
						&& $('option[value="' + defaultPrimaryBusID + '"]', $(ddl)).length > 0) {
						$(ddl).val(defaultPrimaryBusID);
					}
					else {
						$(ddl).val('0');
					}
			    //}

			} catch (e) { }
		}

		function chkSigned_change() {
			<%--var currentStatus = '<%=this.CurrentStatus.Trim().ToUpper() %>';

			if ($('#<%=this.chkSigned_Bus.ClientID %>').is(':checked') && $('#<%=this.chkSigned_Dev.ClientID %>').is(':checked')) {
				$('#<%=this.ddlStatus.ClientID %>').prop('disabled', false);
				$('#<%=this.imgStatusHelp.ClientID %>').hide();

				if (currentStatus == '' || currentStatus == 'REQUESTED') {
					$('#<%=this.ddlStatus.ClientID %> option').filter(function () {
							return $(this).text() == "New";
					}).prop('selected', true);
				}
			}
			else --%>
            if ('<%=this.IsNewWorkItem %>'.toUpperCase() == 'TRUE') {
				<%--$('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
				$('#<%=this.imgStatusHelp.ClientID %>').show();--%>

				$('#<%=this.ddlStatus.ClientID %> option').filter(function () {
					return $(this).text() == "New";
				}).prop('selected', true);
			}
			<%--else if (currentStatus == 'REQUESTED') {
				$('#<%=this.ddlStatus.ClientID %>').val('<%=this.CurrentStatusID %>');
				$('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
			    $('#<%=this.imgStatusHelp.ClientID %>').show();
			}--%>

			setupStatus();
		}

	    function ShowAffiliated() {
	        var assignedTo = $('#<%=this.ddlAssignedTo.ClientID %>').val();
	        MessageBox('Assigned to ID: ' + assignedTo);
	    }

		function buttonHistory_click() {
			clearMessage();

			var title = '', url = '';
			var h = 600, w = 1000;

			title = 'Primary Task History';
			url = _pageUrls.Maintenance.WorkItemHistory
				+ '?type=WorkItem'
				+ '&workItemID=' + _id
			;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('WorkItemHistory', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function buttonSave_click() {
			clearMessage();

			var msg = validateFields();

			if (msg.length > 0) {
				MessageBox(msg, 'Validation Error');
			}
            else {
                if (($('#<%=this.ddlStatus.ClientID %> option:selected').val() == 10 && '<%=this.UnclosedSRTasks%>' == "1") || (typeof $('#frameTasks')[0].contentWindow.parentSaveCheck != "undefined" && $('#frameTasks')[0].contentWindow.parentSaveCheck())) {
                    QuestionBox('Confirm SR Closed', 'All Work Tasks associated to a SR will be in a Closed Status. This will cause the SR to be set to Resolved. Would you like to proceed?', 'Save,Cancel', 'confirmSRUpdate', 300, 300, this);
                } else {
                    save();
                }
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
			var emptyFields = [];
			var msg = '';

			<%--if ($('#<%=this.ddlWorkRequest.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Work Request');
			}--%>
			if ($('#<%=this.txtTitle.ClientID %>').val() == '') {
				emptyFields.push('Title');
			}
			if ($('#<%=this.ddlWorkType.ClientID %>').val() == 0) {
			    emptyFields.push('Resource Group');
			}
			if ($('#<%=this.ddlStatus.ClientID %>').val() == 0 || $('#<%=this.ddlStatus.ClientID %>').val() == null) {
				emptyFields.push('Status');
			}
			if ($('#<%=this.ddlSystem.ClientID %>').val() == 0) {
				emptyFields.push('System');
			}
			if ($('#<%=this.ddlProductVersion.ClientID %>').val() == 0) {
				emptyFields.push('Product Version');
			}
			if ($('#<%=this.ddlWorkItemType.ClientID %>').val() == 0) {
				emptyFields.push('Work Activity');
			}
			if ($('#<%=this.ddlPriority.ClientID %>').val() == 0) {
				emptyFields.push('Priority');
			}
			if ($('#<%=this.ddlAssignedTo.ClientID %>').val() == 0) {
				emptyFields.push('Assigned To');
			}
			//don't do primary resource anymore
		    //confusing if item hasn't been distributed yet

   			<%--if ($('#<%=this.ddlAllocationGroup.ClientID %>').val() == 0 || $('#<%=this.ddlAllocationGroup.ClientID %>').val() == null) {
				emptyFields.push('Allocation Group');
			}
			if ($('#<%=this.ddlAllocation.ClientID %>').val() == 0 || $('#<%=this.ddlAllocation.ClientID %>').val() == null) {
				emptyFields.push('Allocation Assignment');
			}--%>
			if ($('#<%=this.ddlWorkArea.ClientID %>').val() == 0 || $('#<%=this.ddlWorkArea.ClientID %>').val() == null) {
				emptyFields.push('Work Area');
			}
			if ($('#<%=this.ddlWorkloadGroup.ClientID %>').val() == 0) {
				emptyFields.push('Functionality');
			}
			if ($('#<%=this.ddlProductionStatus.ClientID %>').val() == 0) {
				emptyFields.push('Production Status');
			}
            if ($('#<%=this.ddlAssignedToRank.ClientID %>').val() == 0) {
				emptyFields.push('Assigned To Rank');
            }

		    if (emptyFields.length > 0) {
			    msg += 'Please provide values for the following fields: <br>' + emptyFields.join(', ');
			}

			return msg;
		}

		function showConnections() {
			var title = '', url = '';
			var h = 400, w = 1400;

			title = 'Primary Task - [' + $('#<%=this.txtWorkloadNumber.ClientID %>').val() + '] Connections - ' + $('#<%=this.txtTitle.ClientID %>').val();
			url = 'WorkItem_ConnectionGrid.aspx?';

			var workItemType = $('#<%=this.ddlWorkItemType.ClientID %> option:selected').text();
            if (workItemType == 'T2A - IVT/Peer Review') {
				url += 'TestItemID=' + _id;
			}
			else {
				url += 'WorkItemID=' + _id;
			}

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('WorkItemConnections', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

	    function setupStatus() {
	        var count = parseInt('<%=this.Sub_Task_Count %>');
	        var ClosedOrCheckedInCount = parseInt('<%=this.Sub_Task_Closed_Count %>')
                + parseInt('<%=this.Sub_Task_CheckedIn_Count %>')
                + parseInt('<%=this.Sub_Task_Deployed_Count %>')
                + parseInt('<%=this.Sub_Task_Complete_Count %>');

		    // 14133 - Added check for IVT and don't disable Status drop down if IVT:
    		var workItemType = $('#<%=this.ddlWorkItemType.ClientID %> option:selected').text();
            if (workItemType != 'T2A - IVT/Peer Review')
                {
		        if (count > 0) {
				    var currentStatus = '<%=this.CurrentStatus.Trim().ToUpper() %>';

				    if (currentStatus == 'NEW' && count == parseInt('<%=this.Sub_Task_New_Count %>')) {
					    $('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
				    }
				    else if (currentStatus == 'ON HOLD' && count == parseInt('<%=this.Sub_Task_OnHold_Count %>')) {
					    $('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
				    }
                    // Not just Closed but total of Checked In, Deployed, Complete & Closed.
				    else if (currentStatus == 'IN PROGRESS' && count != ClosedOrCheckedInCount) {
					    $('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
				    }
		            <%-- else if (currentStatus == 'IN PROGRESS' && count != parseInt('<%=this.Sub_Task_Closed_Count %>')) {
					    $('#<%=this.ddlStatus.ClientID %>').prop('disabled', true);
				    }--%>
			    }
            }
            filterStatusDDL(_isNew);
		}

	    function verifyResources() {
	        var assignedTo = $('#<%=this.ddlAssignedTo.ClientID %> option:selected').text().replace(' ', '.').toUpperCase();
	        var primaryResource = $('#<%=this.ddlPrimaryResource.ClientID %> option:selected').text().replace(' ', '.').toUpperCase();
	        var assignedToAffiliated = false, primaryResourceAffiliated = false;

	        if (assignedTo == 'BUSINESS.COMPLETE') assignedToAffiliated = true;
            if ($('#<%=this.ddlAssignedTo.ClientID %> option:selected').attr('og') == 'Action Team') assignedToAffiliated = true;

	        $('#divAffiliated table tr').not(':first').each(function () {
	            if (!(assignedToAffiliated && primaryResourceAffiliated)) {
	                var nText = $(this).find('td:eq(0)').text().toUpperCase();

	                if (!assignedToAffiliated && nText == assignedTo) assignedToAffiliated = true;
	                if (!primaryResourceAffiliated && nText == primaryResource) primaryResourceAffiliated = true;
	            }
	        });

	        if (assignedTo == '-SELECT-' || assignedToAffiliated) {
	            $('#imgAssignedToWarning').hide();
	        }
	        else {
	            $('#imgAssignedToWarning').show();
	        }

	        if (primaryResource == '-SELECT-' || primaryResourceAffiliated) {
	            $('#imgPrimaryResourceWarning').hide();
	        }
	        else {
	            $('#imgPrimaryResourceWarning').show();
	        }
	    }

	    function aEdit_click() {
	        $('#aEdit').hide();
	        $('#tdProductVersionRequired').text('*');
	        $('#<%=this.lblProductVersion.ClientID %>').hide();
	        $('#<%=this.ddlProductVersion.ClientID %>').show();
	        
	        MessageBox('Warning - Manually changing the Product Version should be done carefully as this will affect the tracking from AOR.');
        }

        function getAORResourceTeamUser() {
            var aorReleaseID = $('#<%=this.divReleaseAORs.ClientID %> select:first').val();

            if (aorReleaseID != undefined) PageMethods.GetAORResourceTeamUser(aorReleaseID, getAORResourceTeamUser_done, on_error);
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
                var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + aorID + '&AORReleaseID= ' + $('#<%=this.divReleaseAORs.ClientID %> select:first').val() + '&Tab=ResourceTeam';
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

        function chkCascadeAOR_change() {
            var checked = $('#<%=this.chkCascadeAOR.ClientID %>').is(':checked');
            var blnWorkloadAORSelected = false;
            $('#imgCascadeAORWarning').hide();

            $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('select').val() > 0) {
                    $('#imgCascadeAORWarning').hide();
                    blnWorkloadAORSelected = true;
                }
            });

            if (!blnWorkloadAORSelected && checked) $('#imgCascadeAORWarning').show();
        }

        function filterStatusDDL(newTask) {
            var statusDDL = $('#<%=this.ddlStatus.ClientID %> option:selected');

            if (newTask) {
                $('#<%=this.ddlStatus.ClientID %> option').each(function () {
                    if ($(this).text() === 'Re-Opened' || $(this).text() === 'Closed') {
                        $(this).wrap('<span>');
                        $(this).hide();
                    }
                });
            } else {
                if (statusDDL.text() !== 'New') {
                    $('#<%=this.ddlStatus.ClientID %> option').each(function () {
                        if ($(this).text() === 'New') {
                            $(this).wrap('<span>');
                            $(this).hide();
                        }
                    });
                }

                if (!(statusDDL.text() === 'Re-Opened'
                    || statusDDL.text() === 'Closed'
                    || statusDDL.text() === 'Un-Reproducible'
                    || statusDDL.text() === 'Checked In'
                    || statusDDL.text() === 'Deployed'
                    || statusDDL.text() === 'Ready for Review')) {
                    $('#<%=this.ddlStatus.ClientID %> option').each(function () {
                        if ($(this).text() === 'Re-Opened') {
                            $(this).wrap('<span>');
                            $(this).hide();
                        }
                    });
                }
            }
        }

        function imgAffiliatedHelp_click() {
            var message = "Affiliated Resources are gathered by the following rules: <br /><br />";
            message += "AOR: <br />Resources associated to Work Tasks with the same AOR as this Work Task, whose Resource Type is associated with this Work Task's Work Activity. <br /><br />";
            message += "System: <br />Resources associated with the system suite this Work Task's associated system is a part of, whose Resource Type is associated with this Work Task's Work Activity, and is associated with this Work Task's Resource Group";
            MessageBox(message);
        }

        function imgHelp_click() {
            MessageBox("When this is checked, additional testing by the Business Analyst (BA) is required before the 'Status' becomes 'Checked-In'.<br />Please follow the following process: The Developer's (DEV) fix will be posted to the PROD-TEST staging environment with 'Ready for Review' selected and assigned to the BA.<br />Once the item is successfully tested, the BA will assign the item back to the DEV with 'Review Complete' selected to indicate that the task is ready to be 'Checked-In'.");
        }

        function AOR_change(obj) {
            var $obj = $(obj);

            chkCascadeAOR_change();
            getAffiliated();

            if ($obj.attr('field') == 'Release/Deployment MGMT') {
                getAORResourceTeamUser();
                loadWorkActivity($obj.find('option:selected').attr('workloadAllocationID'));

                var wa = $obj.find('option:selected').attr('workloadAllocation');
                if (wa != undefined) {
                    $('#spnWorkloadAllocation').text(wa);
                }
                else {
                    $('#spnWorkloadAllocation').text('');
                }
            }
        }

        function loadWorkActivity(workloadAllocationID) {
            var workActivityID = $('#<%=this.ddlWorkItemType.ClientID %>').val();
            var html = '<option value="0">-Select-</option>';
            var optExists = false;
            //var currentPhaseID = 0;
            var currentWorkActivityGroup = '';

            $('#<%=this.ddlWorkItemType.ClientID %>').empty();

            $.each(arrWorkActivity[0], function (rowIndex, row) {
                if (/*row.PDDTDR_PHASEID == null || */row.WorkloadAllocationID == null || row.WorkloadAllocationID == workloadAllocationID) {
                    /*if (currentPhaseID != row.PDDTDR_PHASEID && row.PDDTDR_PHASE != null) {
                        html += '<option style="background-color: white;" disabled>' + row.PDDTDR_PHASE + '</option>';
                        currentPhaseID = row.PDDTDR_PHASEID;
                    }*/

                    if (currentWorkActivityGroup != row.WorkActivityGroup && row.WorkActivityGroup != null) {
                        html += '<option style="background-color: white;" disabled>' + row.WorkActivityGroup + '</option>';
                        currentWorkActivityGroup = row.WorkActivityGroup;
                    }

                    html += '<option value="' + row.WORKITEMTYPEID + '">' + row.WORKITEMTYPE + '</option>';

                    if (row.WORKITEMTYPEID == workActivityID) optExists = true;
                }
            });

            $('#<%=this.ddlWorkItemType.ClientID %>').append(html);

            if (optExists) $('#<%=this.ddlWorkItemType.ClientID %>').val(workActivityID);
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
	</script>

	<script id="jsInitialize" type="text/javascript">
		
		function initializeSections() {
			try {
				//if (_canEdit) {
				//	$('#imgAddTask').show();
				//	$('#imgAddTask').click(function () { imgAddTask_click(); return false; });
				//}
				if (_id != 0) {
					loadTasks();
                }
                
			    showSubsections();

				var itemType = $('#<%=this.ddlWorkItemType.ClientID %> option:selected').text();
				if (itemType.toUpperCase().indexOf('SUSTAINMENT') > -1) {
					//hide the subsections

					$('#imgHidePlanning').trigger('click');
					$('#imgHideDevelopment').trigger('click');
					$('#imgHideDeployment').trigger('click');
					$('#imgHideTesting').trigger('click');
				}
			} catch (e) {

			}
		}

		function initializeEvents() {

			$(window).resize(resizePage);
			$('select').change(function () { ddl_change(this); });
			$('input:text').bind('change', function () { txt_change(this); });
			
			$('#imgRefresh').click(function () { refreshPage(); });
			if (_canEdit) {
				$('#buttonSave').click(function () { buttonSave_click(); return false; });
			}
			$('#buttonCancel').click(function () { refreshPage(); return false; });

			$('#<%=this.ddlWorkRequest.ClientID %>').change(function () { ddlWorkRequest_change(); return false; });
		    $('#<%=this.ddlPDDTDRPhase.ClientID %>').change(function () { ddlPDDTDRPhase_change(); return false; });
            $('#<%=this.ddlWorkType.ClientID %>').change(function () { ddlWorkType_change(); return false; });
            $('#<%=this.ddlWorkItemType.ClientID %>').change(function () { ddlWorkItemType_change(); return false; });
		    $('#<%=this.ddlAllocationGroup.ClientID %>').change(function () { ddlAllocationGroup_change(); return false; });

			$('#<%=this.ddlStatus.ClientID %>').change(function () {
				_statusId = $('#<%=this.ddlStatus.ClientID %> option:selected').val();
				return false;
			});
		    $('#<%=this.ddlSystem.ClientID %>').change(function () { ddlSystem_change(); return false; });
		    $('#<%=this.ddlProductVersion.ClientID %>').change(function () { ddlProductVersion_change(); return false; });
			$('#<%=this.ddlWorkArea.ClientID %>').change(function () {
				_workAreaID = $('#<%=this.ddlWorkArea.ClientID %> option:selected').val();
				return false;
            });
            $('#<%=this.ddlAssignedToRank.ClientID %>').change(function () { ddlAssignedToRank_change(); return false; });
            $('#<%=this.ddlWorkType.ClientID %>').change(function () { getAffiliated(); return false; });
            $('#<%=this.ddlWorkItemType.ClientID %>').change(function () { getAffiliated(); return false; });

<%--  			$('#<%=this.ddlAllocationGroup.ClientID %>').change(function () {
				_AllocationGroupId = $('#<%=this.ddlAllocationGroup.ClientID %> option:selected').val();
				return false;
			});

		    $('#<%=this.ddlAllocationGroup.ClientID %>').change(function () { ddlAllocationGroup_change(); return false; });--%>

            $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Release/Deployment MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });

            $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this);

                if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                    getAOROptions($obj.find('input[type=checkbox]:first'));
                }
            });

		    $('#<%=this.ddlAllocation.ClientID %>').change(function () { ddlAllocation_change(); return false; });

		    $('#<%=this.ddlAssignedTo.ClientID %>').change(function () {
                $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function() {
				    var $obj = $(this);

                    if ($obj.find('td').text().indexOf('No Release/Deployment MGMT AORs') == -1) {
				        getAOROptions($obj.find('input[type=checkbox]:first'));
				    }
                });

                $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                    var $obj = $(this);

                    if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                        getAOROptions($obj.find('input[type=checkbox]:first'));
                    }
                });

		        verifyResources();
                verifyAORResourceTeamUser();
		        return false;
		    });
		    $('#<%=this.ddlPrimaryResource.ClientID %>').change(function () {
		        $('#<%=this.divReleaseAORs.ClientID %> tr').not(':first').each(function () {
				    var $obj = $(this);

                    if ($obj.find('td').text().indexOf('No Release/Deployment MGMT AORs') == -1) {
				        getAOROptions($obj.find('input[type=checkbox]:first'));
				    }
                });

                $('#<%=this.divWorkloadAORs.ClientID %> tr').not(':first').each(function () {
                    var $obj = $(this);

                    if ($obj.find('td').text().indexOf('No Workload MGMT AORs') == -1) {
                        getAOROptions($obj.find('input[type=checkbox]:first'));
                    }
                });

		        verifyResources();
		        return false;
		    });

			if (_id > 0) {
				$('#labelConnections').addClass('btn_Link');
				$('#imgConnections').click(function () { showConnections(); return false; });
				$('#labelConnections').click(function () { showConnections(); return false; });
			}
			else {
				$('#labelConnections').css('cursor', 'default');
				$('#imgConnections').css('cursor', 'default');
				_RequestedID = +'<%=this.RequestedID %>';
				//_statusId = _RequestedID;  //requested status is now default on new task
			}

			var workItemType = $('#<%=this.ddlWorkItemType.ClientID %> option:selected').text();
            if (workItemType == 'T2A - IVT/Peer Review') {
				$('#<%=this.chkIVTRequired.ClientID %>').prop('disabled', true);
			}

			$('.hideSection').click(function (event) {
				showHideSection_click($(this).attr('id'), false, $(this).attr('sectionName'));
			});
			$('.showSection').click(function (event) {
				showHideSection_click($(this).attr('id'), true, $(this).attr('sectionName'));
			});

			$('#<%=this.chkDeployed_Comm.ClientID %>').change(function () { chkDeployed_change(this, 'Comm'); return false; });
			$('#<%=this.chkDeployed_Test.ClientID %>').change(function () { chkDeployed_change(this, 'Test'); return false; });
			$('#<%=this.chkDeployed_Prod.ClientID %>').change(function () { chkDeployed_change(this, 'Prod'); return false; });

			$('#<%=this.chkSigned_Bus.ClientID %>').change(function () { chkSigned_change(); return false; });
			$('#<%=this.chkSigned_Dev.ClientID %>').change(function () { chkSigned_change(); return false; });
			$('#<%=this.imgStatusHelp.ClientID %>').click(function () { MessageBox('Tasks must be signed off by a Business Team user as well as a Dev team user before the Status can be changed from Requested.'); });

		    $('#<%=this.imgEffortHelp.ClientID %>').click(function () { MessageBox('S = 4-8 Hours, M = 8-24 Hours, L = 24-40 Hours, XL = 41-80 Hours, XXL = 80+ Hours'); });

		    $('#<%=this.imgAffiliated.ClientID %>').click(function () { ShowAffiliated(); return false; });

		    $('#imgAssignedToWarning').click(function () { MessageBox('Warning - Selected Assigned To resource is not affiliated with the task'); return false; });
            $('#imgPrimaryResourceWarning').click(function () { MessageBox('Warning - Selected Primary Resource is not affiliated with the task'); return false; });
            $('#imgCascadeAORWarning').click(function () { MessageBox('Warning - No Workload MGMT AOR Selected'); return false; });
		    $('#aEdit').click(function () { aEdit_click(); return false; });
            $('#btnAORResourceTeam').click(function () { btnAORResourceTeam_click(); return false; });
            $('#<%=this.chkBusinessReview.ClientID %>').change(function () { displayBusinessReview(); return false; });
            $('#<%=this.chkCascadeAOR.ClientID %>').change(function () { chkCascadeAOR_change(); return false; });
            $('#imgHelp').click(function () { imgHelp_click(); });

		    $(document).click(function (e) {
		        try {
		            var objClass = $(e.target).attr('class');
		            var objName = $(e.target).attr('name');

		            if (objClass != 'showaoroptionsettings' && objClass != 'aoroptionsettings' && objName != 'aoroptionsettingsinput') $('.aoroptionsettings').slideUp();
		        }
		        catch (e) { }
		    });
		}

		$(document).ready(function () {
			_pageUrls = new PageURLs();
			_id = +'<%=this.WorkItemID%>';
			if (_id > 0) {
				_commentQty = +'<%=this.Comment_Count %>';
				_attachmentQty = +'<%=this.Attachment_Count %>';
				_wrAttachmentQty = +'<%=this.WR_Attachment_Count %>';
				_subTaskQty = +'<%=this.Task_Count %>';

				SetCommentQty(_commentQty);
				SetAttachmentQty(_attachmentQty);
				SetWRAttachmentQty(_wrAttachmentQty);
				SetTaskQty(_id, _subTaskQty);
			}
			else {
				_isNew = true;
			}

			_statusId = +'<%=this.CurrentStatusID %>';
			if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
				_canEdit = true;
			}
		    _userID = +'<%=this.UserID %>';

            if ('<%=this.IsNewWorkItem.ToString().ToUpper() %>' != 'TRUE' && _canEdit && '<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('#aEdit').show();
            }

			activateSaveButton();

			initializeEvents();
			initializeSections();
            resizePage();
			$('.pageContainer').css('background-color', '#FAFAFA');
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			$('.attributesLabel').css('width', 120);
			$('.attributesLabel[name="LeftColumn"]').css('width', 120);
			$('.attributesLabel[name="Rank"]').css('width', 100);

            if ('<%=this.IsNewWorkItem %>'.toUpperCase() == 'TRUE'
                && '<%=this.SourceWorkItemID %>' != '0') {
                activateSaveButton();
            } 

            //Hide and disable these items if the current workItem is new
            if ('<%=this.IsNewWorkItem %>'.toUpperCase() == 'TRUE') {
                $('#<%=menuRelatedItems.ClientID%>').css('display', 'none');
                $('#divTaskContainer').hide();
                $('#divRequirementsContainer').hide();
                $(parent.document.body).find('#tabNotes').removeAttr('onClick');
                $(parent.document.body).find('#tabAttachments').removeAttr('onClick');
            }

			$('#<%=this.txtDateNeeded.ClientID %>').datepicker();

			//planning date pickers
			$('#<%=this.txtDesignStart_Planned.ClientID %>').datepicker();
			$('#<%=this.txtDevStart_Planned.ClientID %>').datepicker();
			$('#<%=this.txtDesignStart_Actual.ClientID %>').datepicker();
			$('#<%=this.txtDevStart_Actual.ClientID %>').datepicker();
			$('#<%=this.txtEstimatedCompletion.ClientID %>').datepicker();
			$('#<%=this.txtActualCompletion.ClientID %>').datepicker();
			//deployment date pickers
			$('#<%=this.txtDeployedDate_Comm.ClientID %>').datepicker();
			$('#<%=this.txtDeployedDate_Test.ClientID %>').datepicker();
			$('#<%=this.txtDeployedDate_Prod.ClientID %>').datepicker();

            ddlWorkType_change();

			var w = '99%';// $('#<%=this.textAreaDescription.ClientID %>').width();
			var h = 150;
			if(($('#<%=this.textAreaDescription.ClientID %>').height() + 30) > 150){
				h = $('#<%=this.textAreaDescription.ClientID %>').height() + 30;
			};

			$('#<%=this.textAreaDescription.ClientID %>').cleditor({width:w, height:h});

			$('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].change(function () { txt_change($('#<%=this.textAreaDescription.ClientID %>')); });
			$($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc).css('height', (h - 28) + 'px');
			$($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc.body).css('height', (h - 26) + 'px');
			$($('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].doc.body).css('background-color', '#F5F6CE');

			if (!_canEdit) {
				$('#<%=this.textAreaDescription.ClientID %>').cleditor()[0].disable(true);
			}

			//setupStatus();

			_allocationID = $('#<%=this.ddlAllocation.ClientID %>').val();
			_workAreaID = $('#<%=this.ddlWorkArea.ClientID %>').val();

			if ('<%=this.SaveComplete %>' == '1') {
				$('#labelMessage').text('Item updates have been saved.');
				$('#labelMessage').show();
			}
			else {
				clearMessage();
			}

			resizePage();

            // 12817 - 7:
		    _sourceWorkItemID = parseInt('<%=this.SourceWorkItemID %>');
		    if (_id == 0 & _sourceWorkItemID != 0)
		    {
		        $(CopySubTasks).show();
		    }


			$("#helpButton").click(function () {
			    clearMessage();

			    var title = '', url = '';
			    var h = 600, w = 1000;

			    title = 'Primary Tasks Attributes';
			    url = _pageUrls.Maintenance.WorkItemDetailsHelp;

			    //open in a popup
			    var openPopup = popupManager.AddPopupWindow('Help', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			    if (openPopup) {
			        openPopup.Open();
			    }
			});

            getAffiliated();
            getAORHistory();
            getAORResourceTeamUser();
            displayBusinessReview();
            chkCascadeAOR_change();
            filterStatusDDL(_isNew);
		});
	</script>
</asp:Content>