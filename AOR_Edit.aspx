﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Edit.aspx.cs" Inherits="AOR_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css" />
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="15" width="15" data-section="All" style="cursor: pointer;" />
    <img id="imgSettings" src="Images/Icons/cog.png" title="Grid Settings" alt="Grid Settings" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
    <asp:HiddenField ID="itisettings" runat="server" EnableViewState="True" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Details</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
    <input type="button" id="btnArchive" value="Archive" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divAOR" style="padding: 10px;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5px;">&nbsp;
                    </td>
                    <td style="width: 95px;">AOR #:
                    </td>
                    <td>
                        <span id="spnAOR" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span></div>
                    </td>
                    <td style="width: 5px;">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;">*</span>
                    </td>
                    <td>AOR Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtAORName" runat="server" MaxLength="150" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td style="vertical-align: top;">Label:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr style="display: none;">
                    <td>&nbsp;
                    </td>
                    <td style="vertical-align: top;">Progress Update:
                    </td>
                    <td>
                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td style="vertical-align: top;">Approved:
                    </td>
                    <td>
                        <asp:CheckBox ID="chkApproved" runat="server" Enabled="false" />&nbsp;&nbsp;
                        <asp:Label ID="lblApprovedBy" runat="server"></asp:Label>&nbsp;&nbsp;
                        <asp:Label ID="lblApprovedDate" runat="server"></asp:Label>
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div id="iti_sections">
            <div id="divSystemsContainer">
                <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="Systems" style="cursor: pointer;" />&nbsp;&nbsp;<span id="SystemsTitle">AOR Systems (0)</span>
                </div>
                <div id="divSystems" style="display: none;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <div id="divAORSystem" runat="server">
                                    <table style="border-collapse: collapse; padding-bottom: 10px; width: 350px;">
                                        <tr class="gridHeader">
                                            <th style="border-left: 1px solid grey;">Primary System
                                            </th>
                                        </tr>
                                        <tr class="gridBody">
                                            <td style="border-left: 1px solid grey; text-align: center;">
                                                <asp:DropDownList ID="ddlPrimarySystem" runat="server" Width="99%" Enabled="false"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr class="gridBody">
                                            <td style="border-left: 1px solid grey; text-align: center; padding: 0px;">
                                                <div id="divAORSystems" runat="server" style="width: 350px;"></div>
                                                <div id="divAORSystemsPopup" runat="server" style="position: absolute; display: none; background-color: #ffffff;">
                                                    <input type="button" id="btnSaveSystemSelections" value="Save Systems" style="margin-left: 3px; margin-top: 3px;" />
                                                    &nbsp;<input type="button" id="btnSaveResourceSelections" value="Save Resources" style="margin-top: 3px;" />
                                                    &nbsp;<input type="button" id="btnCloseSystemSelections" value="Close" style="margin-top: 3px;" />
                                                    <wts:MultiSelect runat="server" ID="msAORSystems"
                                                        ItemLabelColumnName="Resource"
                                                        ItemValueColumnName="WTS_RESOURCEID"
                                                        CustomAttributes="AORRoleID, Allocation"
                                                        OptionGroupLabelColumnName="WTS_SYSTEM"
                                                        OptionGroupValueColumnName="WTS_SYSTEMID"
                                                        ShowChildCount="true"
                                                        IsOpen="true"
                                                        KeepOpen="true"
                                                        Width="100%"
                                                        MaxHeight="330"
                                                        HideDDLButton="true" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="divDetailsContainer">
                <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="Details" style="cursor: pointer;" />&nbsp;&nbsp;AOR Details
                </div>
                <div id="divDetails" style="display: none;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <div id="divAORDetails" runat="server">
                                    <table style="border-collapse: collapse; width: 100%; padding-bottom: 10px; width: 850px;">
                                        <tr>
                                            <td style="width: 125px">AOR Workload Type: 
                                            </td>
                                            <td style="width: 185px; text-align: center; padding-right: 55px;">
                                                <asp:DropDownList ID="ddlWorkType" runat="server" Width="99%" Enabled="false"></asp:DropDownList>
                                            </td>
                                            <td style="width: 125px">Workload Allocation: 
                                            </td>
                                            <td style="width: 360px; text-align: center;">
                                                <asp:DropDownList ID="ddlWorkloadAllocation" runat="server" Width="99%" Enabled="false"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="wlAOR_NoChk">AOR Status: 
                                            </td>
                                            <td class="wlAOR_NoChk" style="text-align: center; padding-right: 55px;">
                                                <asp:DropDownList ID="ddlAORStatus" runat="server" Width="99%" Enabled="false"></asp:DropDownList>
                                            </td>
                                            <td>Visible To Customer: 
                                            </td>
                                            <td style="padding: 0px;">
                                                <asp:CheckBox ID="chkAORCustomerFlagship" runat="server" Enabled="false" Style="padding-right: 120px;" />
                                                <span class="wlAOR">Cascade AOR:
                                                    <asp:CheckBox ID="chkCascadeAOR" runat="server" Enabled="false" /></span>
                                            </td>
                                        </tr>
                                        <tr style="display: none;">
                                            <td>
                                                <span>Release: </span>
                                            </td>
                                            <td style="text-align: center; padding-right: 55px;">
                                                <asp:DropDownList ID="ddlProductVersion" runat="server" Width="99%" Enabled="false"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr class="missionDate" style="display: none;">
                                            <td>Planned Start: 
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPlannedStart" runat="server" Width="80px" ReadOnly="true" ForeColor="Gray" CssClass="date"></asp:TextBox>
                                                <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                            </td>
                                            <td>Planned End: 
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPlannedEnd" runat="server" Width="80px" ReadOnly="true" ForeColor="Gray" CssClass="date"></asp:TextBox>
                                                <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                            </td>
                                        </tr>
                                        <tr class="missionDate" style="display: none;">
                                            <td>Actual Start: 
                                            </td>
                                            <td>
                                                <span id="spnActualStart" runat="server"></span>
                                            </td>
                                            <td>Actual End: 
                                            </td>
                                            <td>
                                                <span id="spnActualEnd" runat="server"></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 8px">Contract(s): 
                                            </td>
                                            <td colspan="3" style="padding-top: 8px">
                                                <span id="spnContracts" runat="server"></span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>

                    </table>
                </div>
            </div>
            <div id="divAOREstimationContainer">
                <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="AOREstimation" style="cursor: pointer;" />&nbsp;&nbsp;<span id="AOREstimationTitle">AOR Estimation</span>
                </div>
                <div id="divAOREstimation" style="display: none;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <div id="divEstimation" runat="server">
                                    <table style="width: 99%" cellpadding="0" cellspacing="0">
                                        <tr class="pageContentHeader">
                                            <td colspan="5">
                                                <div style="float: left">
                                                    <table>
                                                        <tr cellpadding="0" cellspacing="0">
                                                            <td>
                                                                <img src="Images/icons/help.png" alt="Help" title="CR Report Options" width="15" height="15" style="cursor:pointer;" onclick="loadHelpText(); return"/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <div style="float: right">
                                                    <table>
                                                        <tr cellpadding="0" cellspacing="0">
                                                            <td>
                                                                <input type="button" id="btnAddEditAORAssoc" runat="server" value="View AOR Assoc" style="vertical-align: middle;" disabled="disabled" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id="divEstimationBody" runat="server">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr style="padding-bottom: 10px">
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <div id="divEstimationOverride">
                                    <table cellspacing="0" cellpadding="0" style="width: 98%;">
                                        <tr class="gridHeader">
                                            <td style="font-weight: bold">Override Estimation
                                            </td>
                                            <td id="tdOverrideJustification_Header" style="font-weight: bold; display: none">Justification
                                            </td>
                                            <td id="tdOverrideCreated_Header" style="font-weight: bold; display: none">Created/Updated
                                            </td>
                                            <td id="tdSignOffOverride_Header" style="font-weight: bold; display: none">Override Sign-Off
                                            </td>
                                            <td id="tdSignOffOverrideBy_Header" style="font-weight: bold; display: none">Sign-Off By
                                            </td>
                                            <td id="tdOverrideHistory_Header" style="font-weight: bold; display: none">Override History
                                            </td>
                                        </tr>
                                        <tr class="gridBody">
                                            <td style="width: 150px">
                                                <asp:DropDownList ID="ddlEstimationOverride" runat="server"></asp:DropDownList>
                                            </td>
                                            <td id="tdOverrideJustification_Body" style="width: 600px; display: none">
                                                <asp:TextBox ID="txtOverrideJustification" TextMode="MultiLine" Rows="3" Width="98%" runat="server"></asp:TextBox>
                                            </td>
                                            <td id="tdOverrideCreated_Body" style="text-align: center; display: none">
                                                <asp:Label ID="lblOverrideJustificationCreated" runat="server"></asp:Label>
                                                <br />
                                                <asp:Label ID="lblOverrideJustificationUpdated" runat="server"></asp:Label>
                                            </td>
                                            <td id="tdSignOffOverride_Body" style="text-align: center; display: none">
                                                <asp:CheckBox ID="chkOverrideSignOff" runat="server" Enabled="false" />
                                            </td>
                                            <td id="tdSignOffOverrideBy_Body" style="text-align: center; display: none">
                                                <asp:Label ID="lblOverrideSignOffBy" runat="server"></asp:Label>
                                            </td>
                                            <td id="tdOverrideHistory_Body" style="text-align: center; display: none">
                                                <span id="linkOverrideHistory" runat="server" style="cursor: pointer; text-decoration: underline; color: blue; font-weight: bold" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr style="padding-bottom: 10px">
                            <td></td>
                        </tr>
                        <tr class="pageContentInfo">
                            <td>
                                <div id="divEstimatedResources">
                                    <table cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td style="font-weight: bold">Estimated Net Resources: &nbsp; &nbsp; &nbsp;
                                            </td>
                                            <td>
                                                <input id="txtAOREst_NumResources" runat="server" type="number" maxlength="3" size="3" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="divAOREOverrideHistory" class="modal" style="display: none">
                <!-- Modal content -->
                <div class="modal-content" style="width: 1050px">
                    <div class="modal-header">
                        <img class="modal-close" src="Images/Icons/closeButtonRed.png" alt="close" onclick="closeModal()" />
                        <span id="spnOverrideHistory">Override Estimation History</span>
                    </div>
                    <div id="divOverrideHistory" class="modal-body">
                    </div>
                </div>
            </div>
            <div id="divCMMIContainer" data-index="1">
                <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="CMMI" style="cursor: pointer;" />&nbsp;&nbsp;PD2TDR (Release) Details
                    <div class="wlAOR" id="tdAORRequiresPD2TDR" style="float: right; vertical-align: top; padding-right: 10px;">
                        PD2TDR Required?
                        <asp:CheckBox ID="chkAORRequiresPD2TDR" runat="server" Enabled="false" />
                    </div>
                </div>

                <div id="divCMMI" style="padding: 10px; display: none;">
                    <table style="width: 100%;">
                        <tr style="display: none;">
                            <td style="padding-bottom: 10px;">
                                <table style="border-collapse: collapse; width: 100%;">
                                    <tr class="gridHeader">
                                        <th style="border-left: 1px solid grey;">ROI
                                        </th>
                                    </tr>
                                    <tr class="gridBody">
                                        <td style="border-left: 1px solid grey; text-align: center;">
                                            <asp:TextBox ID="txtROI" runat="server" TextMode="MultiLine" Rows="4" Width="99%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr style="display: none;">
                            <td style="padding-bottom: 10px;">
                                <table style="border-collapse: collapse;">
                                    <tr class="gridHeader">
                                        <th style="border-left: 1px solid grey; width: 100px;">Criticality
                                        </th>
                                        <th style="width: 100px;">Customer Value
                                        </th>
                                        <th style="width: 100px;">Risk
                                        </th>
                                        <th style="width: 135px;">Level of Effort (LOE)
                                        </th>
                                        <th style="width: 85px;">Hours to Fix
                                        </th>
                                        <th style="width: 80px;">Cyber/ISMT
                                        </th>
                                    </tr>
                                    <tr class="gridBody">
                                        <td style="border-left: 1px solid grey; text-align: center;">
                                            <asp:DropDownList ID="ddlCriticality" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlCustomerValue" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlRisk" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlLevelOfEffort" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:TextBox ID="txtHoursToFix" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" Style="text-align: center;"></asp:TextBox>
                                        </td>
                                        <td style="text-align: center; background-color: #F5F6CE;">
                                            <asp:CheckBox ID="chkCyberISMT" runat="server" Enabled="false" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr class="rel_wl_chk" style="display: none;">
                            <td style="padding-bottom: 10px;">
                                <table style="border-collapse: collapse; display: none;">
                                    <tr class="gridHeader">
                                        <th colspan="7" style="border-left: 1px solid grey;">Status
                                        </th>
                                    </tr>
                                    <tr class="gridHeader">
                                        <th style="border-left: 1px solid grey; width: 155px;">Investigation (Inv)
                                        </th>
                                        <th style="width: 155px;">Technical (TD)
                                        </th>
                                        <th style="width: 155px;">Customer Design (CD)
                                        </th>
                                        <th style="width: 155px;">Coding (C)
                                        </th>
                                        <th style="width: 155px;">Internal Testing (IT)
                                        </th>
                                        <th style="width: 215px;">Customer Validation Testing (CVT)
                                        </th>
                                        <th style="width: 155px;">Adoption (Adopt)
                                        </th>
                                    </tr>
                                    <tr class="gridBody">
                                        <td style="border-left: 1px solid grey; text-align: center;">
                                            <asp:DropDownList ID="ddlInvestigation" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlTechnical" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlCustomerDesign" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlCoding" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlInternalTesting" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlCustomerVerificationTesting" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center;">
                                            <asp:DropDownList ID="ddlAdoption" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <div id="divPD2TDRStatus" runat="server"></div>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-bottom: 10px;">
                                <table style="border-collapse: collapse;">
                                    <tr class="gridHeader">
                                        <th class="rel" colspan="2" style="visibility: hidden;"></th>
                                        <th class="rel" colspan="3" style="border-left: 1px solid grey; display: none;">Estimated Effort
                                        </th>
                                    </tr>
                                    <tr class="gridHeader">
                                        <th style="border-left: 1px solid grey; width: 155px; display: none;">Critical Path Team
                                        </th>
                                        <th style="border-left: 1px solid grey; width: 155px;">Cyber Review
                                        </th>
                                        <th class="rel" style="border-left: 1px solid grey; width: 155px; display: none;">CMMI
                                        </th>
                                        <th id="thCyberNarrative" style="width: 400px; display: none;">
                                            <span style="color: red;">*</span>&nbsp;Cyber Review Narrative
                                        </th>
                                        <th id="thCMMIStopLight" style="width: 165px; display: none;">Report Stop Light
                                        </th>
                                        <th class="rel" style="border-left: 1px solid grey; width: 155px; display: none;">Coding
                                        </th>
                                        <th class="rel" style="width: 155px; display: none;">Testing
                                        </th>
                                        <th class="rel" style="width: 155px; display: none;">Training/Support
                                        </th>
                                        <th style="width: 155px; display: none;">Release Status
                                        </th>
                                        <th style="width: 100px; display: none;">Stage Priority
                                        </th>
                                        <th style="width: 100px; display: none;">Tier
                                        </th>
                                        <th style="width: 100px; display: none;">Rank
                                        </th>
                                        <th class="rel" style="border-left: 1px solid grey; width: 100px; display: none;">IP1
                                        </th>
                                        <th class="rel" style="width: 100px; display: none;">IP2
                                        </th>
                                        <th class="rel" style="width: 100px; display: none;">IP3
                                        </th>
                                    </tr>
                                    <tr class="gridBody">
                                        <td style="border-left: 1px solid grey; text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlCriticalPathTeam" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="border-left: 1px solid grey; text-align: center;">
                                            <asp:DropDownList ID="ddlCyber" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="border-left: 1px solid grey; text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlCMMI" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td id="tdCyberNarrative" style="text-align: center; display: none;">
                                            <asp:TextBox ID="txtCyberNarrative" runat="server" TextMode="MultiLine" Rows="2" Width="99%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                                        </td>
                                        <td id="tdCMMIStopLight" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlStopLight" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="border-left: 1px solid grey; text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlCodingEffort" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlTestingEffort" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlTrainingSupportEffort" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td style="text-align: center; display: none;">
                                            <span id="spnStatus" runat="server"></span>
                                        </td>
                                        <td style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlStagePriority" runat="server" Width="95%" Enabled="false">
                                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                                <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlTier" runat="server" Width="95%" Enabled="false">
                                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                <asp:ListItem Value="1" Text="A"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="B"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="C"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td style="text-align: center; display: none;">
                                            <asp:TextBox ID="txtRank" runat="server" MaxLength="2" Width="95%" ReadOnly="true" ForeColor="Gray" Style="text-align: center;"></asp:TextBox>
                                        </td>
                                        <td class="rel" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlIP1" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlIP2" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                        <td class="rel" style="text-align: center; display: none;">
                                            <asp:DropDownList ID="ddlIP3" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divCRsContainer">
                    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                        <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="CRs" style="cursor: pointer;" />&nbsp;&nbsp;<span id="CRTitle">CRs (0)</span>
                    </div>
                    <div id="divCRs" style="display: none;">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <div id="divAORCRs" runat="server">
                                        <iframe id="frameCRs" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="divTasksContainer">
                    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                        <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="Tasks" style="cursor: pointer;" />&nbsp;&nbsp;<span id="TasksTitle">Work Tasks (0)</span>
                    </div>
                    <div id="divTasks" style="display: none;">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <div id="divAORTasks" runat="server">
                                        <iframe id="frameTasks" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="divResourcesContainer">
                    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                        <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="Resources" style="cursor: pointer;" />&nbsp;&nbsp;<span id="ResourcesTitle">Resources (0)</span>
                    </div>
                    <div id="divResources" style="display: none;">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <div id="divAORResources" runat="server">
                                        <iframe id="frameResources" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="divHistoryContainer" style="display: none;">
                    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
                        <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="History" style="cursor: pointer;" />&nbsp;&nbsp;History:
                    </div>
                    <div id="divHistory" style="padding: 10px; display: none;">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <div id="divAORHistory" runat="server"></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div id="divSettingsContainer" style="display: none;">
                <div id="divSettingsPopup" runat="server" style="position: absolute; display: none; background-color: #ffffff;">
                    <div style="font-size: 1.1em; font-family: Verdana,Arial,sans-serif; padding: 5px; padding-top: 5px; float: left">
                        Select sections to be expanded.
                    </div>
                    <table id="tableSettings" style="width: 99%; vertical-align: top; text-align: left; padding: 5px;" cellpadding="0" cellspacing="0">
                        <thead>
                        </thead>
                        <tbody>
                            <tr>
                                <td style="border: 1px solid black; padding: 5px;">
                                    <table id="tableSettingsAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px;">
                                        <tbody>
                                            <tr class="attributesRow">
                                                <td class="attributesValue" style="font-size: 1.4em;">
                                                    <ul>
                                                        <li id="settingsAORSystems" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsAORSystems" runat="server" type="checkbox" field="Systems" />
                                                            <label for="chkSettingsAORSystems">AOR Systems</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsAORDetails" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsAORDetails" runat="server" type="checkbox" field="Details" />
                                                            <label for="chkSettingsAORDetails">AOR Details</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsAOREstimation" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsAOREstimation" runat="server" type="checkbox" field="AOREstimation" />
                                                            <label for="chkSettingsAOREstimation">AOR Estimation</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsCMMIDetails" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsCMMIDetails" runat="server" type="checkbox" field="CMMI" />
                                                            <label for="chkSettingsCMMIDetails">PD2TDR (Release) Details</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsCRs" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsCRs" runat="server" type="checkbox" field="CRs" />
                                                            <label for="chkSettingsCRs">CRs</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsTasks" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsTasks" runat="server" type="checkbox" field="Tasks" />
                                                            <label for="chkSettingsTasks">Work Tasks</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsResources" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsResources" runat="server" type="checkbox" field="Resources" />
                                                            <label for="chkSettingsResources">Resources</label>
                                                        </li>
                                                    </ul>
                                                    <ul>
                                                        <li id="settingsHistory" class="ui-state-default" style="margin: 0px 3px 3px 3px; padding: 10px;">
                                                            <input id="chkSettingsHistory" runat="server" type="checkbox" field="History" />
                                                            <label for="chkSettingsHistory">History</label>
                                                        </li>
                                                    </ul>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="font-size: 1em; font-family: Verdana,Arial,sans-serif; text-align: right; padding-top: 10px;">
                                    <input id="btnSettingsExpand" type="button" value="Expand Sections" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <style type="text/css">
        .RiskLowColor {
            background-color: limegreen;
            color: #000000;
        }

        .RiskModerateColor {
            background-color: yellow;
            color: #000000;
        }

        .RiskHighColor {
            background-color: red;
            color: #000000;
        }
    </style>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _resourceCount;
        var _aorChanged = false;
    </script>

    <script id="jsEvents" type="text/javascript">
        $.extend({
            icompare: function (a, b) {
                return $(a).not(b).get().length === 0 && $(b).not(a).get().length === 0;
            }
        });

        function imgRefresh_click() {
            refreshPage();
        }

        function imgSettings_click() {
            openSettings();
        }

        function btnArchive_click() {
            var nWindow = 'ArchiveAOR';
            var nTitle = 'Archive AOR';
            var nHeight = 200, nWidth = 550;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Archive AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnCancel_click() {
            refreshPage();
        }

        function btnSettingsExpand_click() {
            top.$('#tableSettingsAttributes input').each(function () {
                if ($(this)[0].checked && $('#div' + $(this).attr('field') + ':hidden').length > 0) imgToggleSection_click($('.toggleSection[data-section="' + $(this).attr('field') + '"]'));
                else if (!$(this)[0].checked && $('#div' + $(this).attr('field') + ':hidden').length == 0) imgToggleSection_click($('.toggleSection[data-section="' + $(this).attr('field') + '"]'));
            });

            PageMethods.setAORSettingsSession(
                top.$('#<%=this.chkSettingsAORSystems.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsAORDetails.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsAOREstimation.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsCMMIDetails.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsCRs.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsTasks.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsResources.ClientID%>')[0].checked,
                top.$('#<%=this.chkSettingsHistory.ClientID%>')[0].checked, setAORSettingsSession_done, on_error
            );

        }

        function setAORSettingsSession_done() {
            var popup = popupManager.GetPopupByName('AORSectionSettings');

            if (popup != undefined) popup.Close();
        }

        function btnSave_click(close) {
            try {
                var validation = validate();

                if (validation.length === 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    var arrSystems = [];

                    $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function () {
                        var $obj = $(this);

                        if ($obj.find('td').text().indexOf('No Systems') === -1) arrSystems.push({ 'systemid': $obj.find('select[field="System"]').val(), 'primary': 0 });
                    });

                    if ($('#<%=this.ddlPrimarySystem.ClientID%>').val() > 0) {
                        arrSystems.push({ 'systemid': $('#<%=this.ddlPrimarySystem.ClientID%>').val(), 'primary': 1 });
                    }

                    var nSystemsJSON = '{save:' + JSON.stringify(arrSystems) + '}';

                    //AOR Estimation
                    var arrEstimation = [];
                    $('#<%=this.divEstimationBody.ClientID %>').find('.gridBody').each(function () {
                        if ($(this).closest("tr[id='trAORE_Total']").length == 0 && parseInt($(this).find("select[name^='ddlRisk'] option:selected").val()) != - 1) {
                            arrEstimation.push({
                                'estimation_id': $(this).find("input[name^='txtBOEID']").val(),
                                'weight_val': $(this).find("input[name^='txtWeight']").val(),
                                'risk_id': $(this).find("select[name^='ddlRisk'] option:selected").val(),
                                'details': $(this).find("input[name^='txtDetails']").val(),
                                'mitigation': $(this).find("input[name^='txtMitigation']").val()
                            });
                        }
                    });
                    var nEstimationJSON = '{save:' + JSON.stringify(arrEstimation) + '}';

                    PageMethods.Save('<%=this.NewAOR %>', '<%=this.AORID %>', '<%=this.AORReleaseID %>', $('#<%=this.txtAORName.ClientID %>').val(), $('#<%=this.txtDescription.ClientID %>').val(), $('#<%=this.txtNotes.ClientID %>').val(),
                        ($('#<%=this.chkApproved.ClientID %>').is(':checked') ? 1 : 0), $('#<%=this.ddlCodingEffort.ClientID %>').val(), $('#<%=this.ddlTestingEffort.ClientID %>').val(), $('#<%=this.ddlTrainingSupportEffort.ClientID %>').val(),
                        $('#<%=this.ddlStagePriority.ClientID %>').val(), $('#<%=this.ddlProductVersion.ClientID %>').val(), $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(),
                        $('#<%=this.ddlTier.ClientID %>').val(), $('#<%=this.txtRank.ClientID %>').val(), $('#<%=this.ddlIP1.ClientID %>').val(), $('#<%=this.ddlIP2.ClientID %>').val(), $('#<%=this.ddlIP3.ClientID %>').val(),
                        $('#<%=this.txtROI.ClientID %>').val(), $('#<%=this.ddlCMMI.ClientID %>').val(), $('#<%=this.ddlCyber.ClientID %>').val(), $('#<%=this.txtCyberNarrative.ClientID %>').val(),
                        $('#<%=this.ddlCriticalPathTeam.ClientID %>').val(), $('#<%=this.ddlWorkType.ClientID %>').val(), ($('#<%=this.chkCascadeAOR.ClientID %>').is(':checked') ? 1 : 0), ($('#<%=this.chkAORCustomerFlagship.ClientID %>').is(':checked') ? 1 : 0),
                        $('#<%=this.ddlInvestigation.ClientID %>').val(), $('#<%=this.ddlTechnical.ClientID %>').val(), $('#<%=this.ddlCustomerDesign.ClientID %>').val(), $('#<%=this.ddlCoding.ClientID %>').val(),
                        $('#<%=this.ddlInternalTesting.ClientID %>').val(), $('#<%=this.ddlCustomerVerificationTesting.ClientID %>').val(), $('#<%=this.ddlAdoption.ClientID %>').val(), $('#<%=this.ddlStopLight.ClientID %>').val(), $('#<%=this.ddlAORStatus.ClientID %>').val(), ($('#<%=this.chkAORRequiresPD2TDR.ClientID %>').is(':checked') ? 1 : 0),
                        $('#<%=this.ddlCriticality.ClientID %>').val(), $('#<%=this.ddlCustomerValue.ClientID %>').val(), $('#<%=this.ddlRisk.ClientID %>').val(), $('#<%=this.ddlLevelOfEffort.ClientID %>').val(), $('#<%=this.txtHoursToFix.ClientID %>').val(), ($('#<%=this.chkCyberISMT.ClientID %>').is(':checked') ? 1 : 0),
                        $('#<%=this.txtPlannedStart.ClientID %>').val(), $('#<%=this.txtPlannedEnd.ClientID %>').val(),
                        nEstimationJSON,
                        $('#<%=this.txtAOREst_NumResources.ClientID %>').val(), $('#<%=this.ddlEstimationOverride.ClientID %>').val(), $('#<%=this.txtOverrideJustification.ClientID %>').val(), ($('#<%=this.chkOverrideSignOff.ClientID %>').is(':checked') ? 1 : 0),
                        nSystemsJSON, save_done, on_error);
                    if (close) {
                        if (parent.parent.showFrameForGrid) parent.parent.showFrameForGrid('AOR', true);
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

            var blnSaved = false, blnExists = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
                if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                if (parent.parent._newItemCreated != undefined) parent.parent._newItemCreated = true;

                MessageBox('AOR has been saved.');
                savePrimarySystemResources();
                if (($('#<%=this.ddlProductVersion.ClientID %>').val() != '<%=this.ddlProductVersion.SelectedValue %>' || $('#<%=this.txtAORName.ClientID %>').val() != $('#<%=this.txtAORName.ClientID %>').attr('original_value')) && parent.refreshPage) {
                    parent.refreshPage(newID);
                }
                else {
                    parent._aorChanged = false;
                    refreshPage(newID);
                }
            }
            else if (blnExists) {
                MessageBox('AOR Name already exists.');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function imgToggleSection_click(obj, blnSectionClick) {
            var $obj = $(obj);
            var section = $obj.data('section');

            if (section == 'All') {
                $('.toggleSection:not(:first)').each(function () {
                    if ($obj.attr('title') == 'Show Section' && $(this).attr('title') == 'Show Section') {
                        $(this).attr('src', 'Images/Icons/minus_blue.png');
                        $(this).attr('title', 'Hide Section');
                        $(this).attr('alt', 'Hide Section');
                        $('#div' + $(this).data('section')).show();
                        if ($('#frame' + $(this).data('section')).length > 0) resizeFrame($(this).data('section'));
                    }
                    else if ($obj.attr('title') != 'Show Section' && $(this).attr('title') != 'Show Section') {
                        $(this).attr('src', 'Images/Icons/add_blue.png');
                        $(this).attr('title', 'Show Section');
                        $(this).attr('alt', 'Show Section');
                        $('#div' + $(this).data('section')).hide();
                    }
                });

                if ($obj.attr('title') == 'Show Section') {
                    $obj.attr('src', 'Images/Icons/minus_blue.png');
                    $obj.attr('title', 'Hide Section');
                    $obj.attr('alt', 'Hide Section');
                    if ($('#frame' + section).length > 0) resizeFrame(section);
                }
                else {
                    $obj.attr('src', 'Images/Icons/add_blue.png');
                    $obj.attr('title', 'Show Section');
                    $obj.attr('alt', 'Show Section');
                }
            } else {
                if ($obj.attr('title') == 'Show Section') {
                    $obj.attr('src', 'Images/Icons/minus_blue.png');
                    $obj.attr('title', 'Hide Section');
                    $obj.attr('alt', 'Hide Section');
                    $('#div' + section).show();
                    if ($('#frame' + section).length > 0) resizeFrame(section);
                }
                else {
                    $obj.attr('src', 'Images/Icons/add_blue.png');
                    $obj.attr('title', 'Show Section');
                    $obj.attr('alt', 'Show Section');
                    $('#div' + section).hide();
                }
            }

            resizePage();

            if (blnSectionClick) {
                $('#<%=this.chkSettingsAORSystems.ClientID%>').prop('checked', $('#divSystems').is(':visible'));
                $('#<%=this.chkSettingsAORDetails.ClientID%>').prop('checked', $('#divDetails').is(':visible'));
                $('#<%=this.chkSettingsAOREstimation.ClientID%>').prop('checked', $('#divAOREstimation').is(':visible'));
                $('#<%=this.chkSettingsCMMIDetails.ClientID%>').prop('checked', $('#divCMMI').is(':visible'));
                $('#<%=this.chkSettingsCRs.ClientID%>').prop('checked', $('#divCRs').is(':visible'));
                $('#<%=this.chkSettingsTasks.ClientID%>').prop('checked', $('#divTasks').is(':visible'));
                $('#<%=this.chkSettingsResources.ClientID%>').prop('checked', $('#divResources').is(':visible'));
                $('#<%=this.chkSettingsHistory.ClientID%>').prop('checked', $('#divHistory').is(':visible'));

                PageMethods.setAORSettingsSession(
                    $('#<%=this.chkSettingsAORSystems.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsAORDetails.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsAOREstimation.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsCMMIDetails.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsCRs.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsTasks.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsResources.ClientID%>').prop('checked'),
                    $('#<%=this.chkSettingsHistory.ClientID%>').prop('checked'),
                    setAORSettingsSession_done,
                    on_error
                );
            }
        }

        function ddlWorkType_change() {
            setupAORType(false);
            setupMissionDate(false);
        }

        function ddlPrimarySystem_change() {
            PageMethods.loadWorkloadAllocation($('#<%=this.ddlPrimarySystem.ClientID%>').val(), loadWorkloadAllocation_done, loadWorkloadAllocation_on_error);
        }

        function loadWorkloadAllocation_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 1) {
                var contractID = 0;
                if ($('#<%=this.ddlWorkloadAllocation.ClientID %> option:disabled').text() !== dt[1].CONTRACT) {
                    $('#<%=this.ddlWorkloadAllocation.ClientID %>').empty();
                    $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(dt[0].WorkloadAllocationID);
                    $.each(dt, function (rowIndex, row) {
                        if (contractID != this.ContractID) {
                            $('#<%=this.ddlWorkloadAllocation.ClientID %>').append($("<option disabled='disabled' style='background-color: white;'/>").text(this.CONTRACT));
                            contractID = this.ContractID;
                        }
                        $('#<%=this.ddlWorkloadAllocation.ClientID %>').append($("<option />").val(this.WorkloadAllocationID).text(this.WorkloadAllocation));
                    });
                }
            } else {
                $('#<%=this.ddlWorkloadAllocation.ClientID %>').empty();
            }
        }

        function loadWorkloadAllocation_on_error() {
            MessageBox('An error has occurred.');
        }

        function setupAORType(init) {
            var selected = $('#<%=this.ddlWorkType.ClientID %> option:selected').text();

            if (selected == 'Release/Deployment MGMT') {
                $('.rel').show();
                $('.rel_wl_chk').show();
                $('#<%=this.chkCascadeAOR.ClientID %>')[0].checked = "";
                $('#<%=this.chkAORRequiresPD2TDR.ClientID %>')[0].checked = "";
                $('#<%=this.ddlAORStatus.ClientID %>').prop('disabled', true);
                $('.wlAOR').hide();
            } else if (selected == 'PD2TDR Managed AORs') {
                $('.rel').show();
                $('.wlAOR').show();
            } else if (selected == 'Workload MGMT' && $('#<%=this.chkAORRequiresPD2TDR.ClientID %>')[0].checked) {
                $('.rel').hide();
                $('.wlAOR').show();
                $('.rel_wl_chk').show();
                $('#<%=this.ddlAORStatus.ClientID %>').val("");
            }
            else if (!init) {
                $('.rel').find('select').val('0');
                $('.rel').hide();
                $('.rel_wl_chk').hide();
                $('.wlAOR').show();
            }
        }

        function ddlCyber_change() {
            setupCyber(false);
        }

        function setupCyber(init) {
            var selected = $('#<%=this.ddlCyber.ClientID %> option:selected').text();

            if (selected == 'Reviewed/Concern' || selected == 'Not Required') {
                if (selected == 'Not Required') {
                    if (!init) $('#<%=this.txtCyberNarrative.ClientID %>').val('This workload does not deal with changes to the code and does not impact changes to the UI.');
                    $('#<%=this.txtCyberNarrative.ClientID %>').css('color', 'gray');
                    $('#<%=this.txtCyberNarrative.ClientID %>').attr('readonly', 'true');
                }
                else {
                    $('#<%=this.txtCyberNarrative.ClientID %>').removeAttr('readonly');
                    $('#<%=this.txtCyberNarrative.ClientID %>').css('color', 'black');
                    if (!init) $('#<%=this.txtCyberNarrative.ClientID %>').empty();
                }

                $('#thCyberNarrative').show();
                $('#tdCyberNarrative').show();
            }
            else {
                $('#thCyberNarrative').hide();
                $('#tdCyberNarrative').hide();
                $('#<%=this.txtCyberNarrative.ClientID %>').removeAttr('readonly');
                $('#<%=this.txtCyberNarrative.ClientID %>').css('color', 'black');
                if (!init) $('#<%=this.txtCyberNarrative.ClientID %>').empty();
            }
        }

        function setupMissionDate(init) {
            var selected = $('#<%=this.ddlWorkType.ClientID %> option:selected').text();

            if (selected == 'Workload MGMT') {
                $('.missionDate').show();
            }
            else {
                $('#<%=this.txtPlannedStart.ClientID %>').val('');
                $('#<%=this.txtPlannedEnd.ClientID %>').val('');
                $('.missionDate').hide();
            }
        }

        function openSettings() {
            var nWindow = 'AORSectionSettings';
            var nTitle = 'AOR Section Settings';
            var nHeight = 400, nWidth = 450;
            var nURL = null;

            // always reset to update selections from select lists
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divSettingsPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function reorderSectionList() {
            var items = $('#iti_sections').children();
            var data = JSON.parse($(defaultParentPage.itisettings).text());
            var newArray = [];
            var oldArray = [];

            $.each(items, function (i, v) {
                newArray.push('' + v.id.replace("div", "chk").replace("Container", ""));
            });

            $.each(data.sectionexpanded, function (i, v) {
                oldArray.push(i);
            });

            if (!$.icompare(newArray, oldArray)) {
                var newSectionOrder = [];
                var newSectionExpanded = {};

                $.each(items, function (i, v) {
                    var newItem = i + 1;
                    newSectionOrder.push(newItem.toString());
                    newSectionExpanded['' + v.id.replace("div", "chk").replace("Container", "") + ''] = true;
                });

                data.sectionorder = newSectionOrder;
                data.sectionexpanded = newSectionExpanded;
                $(defaultParentPage.itisettings).text(JSON.stringify(data));
            }

            var orderedItems = $.map(data.sectionorder, function (value) {
                return items.get(value - 1);
            });

            $('#iti_sections').empty().html(orderedItems);
        }

        function showSectionList() {
            var data = JSON.parse($(defaultParentPage.itisettings).text());
            var imgArray = $('#iti_sections').find('img');

            $.each(data.sectionexpanded, function (i, item) {
                $.each(imgArray, function (j, value) {
                    var $obj = $(imgArray[j]);
                    var section = $obj.data('section');

                    if (section === i.substring(3)) {
                        if (item === false) {
                            $obj.attr("src", "Images/Icons/minus_blue.png");
                            $obj.attr("title", "Hide Section");
                            $obj.attr("alt", "Hide Section");
                            $obj.data("section", i.substring(3));
                        } else {
                            $obj.attr("src", "Images/Icons/add_blue.png");
                            $obj.attr("title", "Show Section");
                            $obj.attr("alt", "Show Section");
                            $obj.data("section", i.substring(3));
                        }
                        imgToggleSection_click($obj);
                    }
                });
            });
        }

        function validate() {
            var validation = [];
            var $systemRows = $('#<%=this.divAORSystems.ClientID %> tr').not(':first');
            var blnExit = false;
            var allocationSum = 0;

            if ($('#<%=this.txtAORName.ClientID %>').val().length == 0) validation.push('AOR Name cannot be empty.');

            if ($('#<%=this.ddlCyber.ClientID %> option:selected').text() == 'Reviewed/Concern' && $('#<%=this.txtCyberNarrative.ClientID %>').val().length == 0) validation.push('Cyber Review Narrative cannot be empty.');

            $.each($systemRows, function () {
                if (blnExit) return false;

                var systemID = $(this).find('select[field="System"]').val();

                $.each($systemRows.not($(this)), function () {
                    if ($(this).find('select[field="System"]').val() == systemID
                        || $(this).find('select[field="System"]').val() == $('#<%=this.ddlPrimarySystem.ClientID%>').val()) {
                        validation.push('System cannot have duplicates.');
                        blnExit = true;
                        return false;
                    }
                });
            });

            if ($systemRows.length > 0 && $systemRows.find('td').text().indexOf('No Systems') == -1) {
                if ($('#<%=this.ddlPrimarySystem.ClientID%>').val() == 0) {
                    validation.push('Please select a primary system.');
                }
            }

            //Validate AOR Estimation Start
            if ($('#<%=this.ddlEstimationOverride.ClientID %>').val() != -1) {
                if ($('#<%=this.txtOverrideJustification.ClientID %>').val() == '') {
                    validation.push('Override Justification is required');
                }
            }

            var ddlRisk_High_Val = 47;
            var ddlRisk_Null_Val = -1;
            $('#ctl00_ContentPlaceHolderBody_divEstimationBody').find('.gridBody').each(function () {
                var mitigation = $(this).find("input[name^='txtMitigation']").val();
                var details = $(this).find("input[name^='txtDetails']").val();
                var weight = $(this).find("input[name^='txtWeight']").val();
                var risk = $(this).find("select[name^='ddlRisk'] option:selected").val();

                if ((risk == ddlRisk_Null_Val) && (mitigation != '' || details != '' || weight != '')) {
                    validation.push('A Risk value is required');
                    return false;
                }
                 if ((risk == ddlRisk_High_Val) && (details == '' || mitigation == '')) {
                    validation.push('If this is High Risk then Mitigation and Details are required');
                    return false;
                }
                if ((risk != ddlRisk_Null_Val) && (weight == '')) {
                    validation.push('If a Risk value is selected then a weight% is required');
                    return false;
                }         
            });
            //Validate AOR Estimation End


            if (weight_Total > 100) {
                validation.push('AOR Basis of Estimate total weight cannot exceed 100%');
            }
            return validation.join('<br>');
        }

        function addSystemLinkClicked() {
            var nWindow = 'AddSystemToAOR';
            var nTitle = 'Add System';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            // always reset to update selections from select lists
            var $tbl = $('#<%=this.divAORSystems.ClientID %> table');
            var selections = $tbl.find('select').map(function () { return $(this).val(); });
            selections = $.makeArray(selections).join(';');

            msAORSystems_setSelections(selections);

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divAORSystemsPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function closeSystemSelections() {
            popupManager.RemovePopupWindow('AddSystemToAOR');
        }

        function saveSystemSelections() {
            var arrSystems = msAORSystems_getSelections(true);
            //popupManager.RemovePopupWindow('AddSystemToAOR'); // uncomment to close window
            var $tbl = $('#<%=this.divAORSystems.ClientID %> table');

            // first clear any system rows that aren't checked
            var sysSelects = $tbl.find('select');
            for (var i = 0; i < sysSelects.length; i++) {
                var val = $(sysSelects[i]).val();

                if (arrSystems.indexOf(val) == -1) {
                    removeSystem(sysSelects[i]);
                }
            }

            // next add any system rows that are checked and aren't already added

            for (var i = 0; i < arrSystems.length; i++) {
                var sID = arrSystems[i];

                if ($tbl.find('select:has(option[value=' + sID + ']:selected)').length == 0) {
                    var nHTML = '';

                    nHTML += '<tr class=\"gridBody\" rowChanged="1"><td style=\"text-align: center;' + (i == 0 ? 'border-bottom: 0px;' : '') + '\">';
                    nHTML += '<a href=\"\" onclick=\"removeSystem(this); return false;\" style=\"color: blue;\">Remove</a>';
                    nHTML += '</td><td style=\"text-align: center;' + (i == 0 ? 'border-bottom: 0px; ' : '') + '\">';
                    nHTML += '<select field="System" original_value="' + sID + '" fieldChanged="1" style="width: 99%; background-color: #F5F6CE;">' + decodeURIComponent('<%=this.SystemOptions %>') + '</select>';
                    nHTML += '</td></tr>';

                    $tbl.find('tr:first').after(nHTML);

                    $tbl.find('select[original_value=' + sID + ']').val(sID);
                }
            }

            if ($tbl.find('td').text().indexOf('No Systems') > -1 && arrSystems.length > 0) {
                removeSystem($tbl.find('td:contains("No Systems")'));
            }

            $('#btnSave').prop('disabled', false);
            if (parent.aorChanged) parent.aorChanged(true);
        }

        function savePrimarySystemResources() {
            try {
                PageMethods.getPrimarySystemResources($('#<%=this.ddlPrimarySystem.ClientID %>').val(), $('#<%=this.ddlProductVersion.ClientID %>').val(), saveSystemResourceSelections);
            }
            catch (e) {
                dangerMessage('Failed to save Primary System Resources', null, true, this);
            }
        }

        function saveSystemResourceSelections(result) {
            try {
                var arrResourcesAdd = [];
                var arrResources = msAORSystems_getSelections(false, 'AORRoleID, Allocation');
                var frameResources = $('#frameResources').contents().find('.gridBody');

                frameResources.each(function () {
                    if ($(this).find('select[field="Resource"]').val() > 0) arrResourcesAdd.push({ 'resourceid': $(this).find('select[field="Resource"]').val(), 'allocation': $(this).find('input[field="Allocation"]').val(), 'aorroleid': $(this).find('select[field="ResourceRole"]').val() });
                });

                var dt;
                if (result) {
                    dt = jQuery.parseJSON(result);
                    $(dt).each(function () {
                        if ($(this)[0]['WTS_RESOURCEID'] > 0) arrResourcesAdd.push({ 'resourceid': $(this)[0]['WTS_RESOURCEID'], 'allocation': $(this)[0]['Allocation'], 'aorroleid': $(this)[0]['AORRoleID'] });
                    });
                }

                for (var i = 0; i < arrResources.length; i++) {
                    var rscArr = arrResources[i].split('___');
                    var rscID = rscArr[0];
                    var AORRoleID = rscArr[1];
                    var Allocation = rscArr[2];

                    if (rscID != "" && rscID != "MISSING") {
                        if (!_.contains(_.pluck(arrResourcesAdd, 'resourceid'), rscID)) {
                            arrResourcesAdd.push({ 'resourceid': rscID, 'allocation': Allocation, 'aorroleid': AORRoleID });
                        }
                    }
                }
                _resourceCount = arrResourcesAdd.length;
                var nResourcesJSON = '{save:' + JSON.stringify(arrResourcesAdd) + '}';

                PageMethods.SaveResources('<%=this.AORID %>', nResourcesJSON, saveResource_done, on_error);
            }
            catch (e) {
                if ($(popupManager.GetPopupByName('AddSystemToAOR')).length > 0) dangerMessage('An error has occurred', null, true, $(popupManager.GetPopupByName('AddSystemToAOR').Window).find('[id*=divAORSystemsPopup]')[0]);
            }
        }

        function saveResource_done(result) {
            var blnSaved = false, blnExists = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
                if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                if ($(popupManager.GetPopupByName('AddSystemToAOR')).length > 0) successMessage('Resources have been saved', null, true, $(popupManager.GetPopupByName('AddSystemToAOR').Window).find('[id*=divAORSystemsPopup]')[0]);
                $('#frameResources')[0].contentWindow.location.reload(true);
            }
            else {
                if ($(popupManager.GetPopupByName('AddSystemToAOR')).length > 0) dangerMessage('Failed to save Resources' + obj.error, null, true, $(popupManager.GetPopupByName('AddSystemToAOR').Window).find('[id*=divAORSystemsPopup]')[0]);
            }
        }

        function removeSystem(obj) {
            $(obj).closest('tr').remove();

            var $tbl = $('#<%=this.divAORSystems.ClientID %> table');

            if ($tbl.find('tr').not(':first').length == 0) $tbl.append('<tr class="gridBody"><td colspan="3" style="border-top: 1px solid grey; border-bottom: 0px; text-align: center;">No Systems</td></tr>');

            $('#btnSave').prop('disabled', false);
            if (parent.aorChanged) parent.aorChanged(true);
        }

        function updateTab(tabName, newCount, subtasks) {
            switch (tabName.toUpperCase()) {
                case 'SYSTEMS':
                    $('#SystemsTitle').text('AOR Systems (' + newCount + ')');
                    _resourceTeamCount = newCount;
                    break;
                case 'CRS':
                    $('#CRTitle').text('CRs (' + newCount + ')');
                    _CRCount = newCount;
                    break;
                case 'TASKS':
                    newCount += subtasks;
                    $('#TasksTitle').text('Work Tasks (' + newCount + ')');
                    _taskCount = newCount;
                    break;
                case 'RESOURCES':
                    $('#ResourcesTitle').text('Resources (' + newCount + ')');
                    _resourceTeamCount = newCount;
                    break;
            }
        }

        function input_change(obj) {
            var $obj = $(obj);

            if ($obj.attr('id') && $obj.attr('id').indexOf('Rank') != -1) {
                var nVal = $obj.val();

                $obj.val(nVal.replace(/[^\d]/g, ''));
            }

            switch ($obj.attr('field')) {
                case 'System':
                case 'Primary':
                    $obj.attr('fieldChanged', '1');
                    $obj.closest('tr').attr('rowChanged', '1');
                    break;
            }
            if ($obj.attr('id') && $obj.attr('id').indexOf('msAORSystems_ddlItems') == -1) {
                $('#btnSave').prop('disabled', false);
                if (parent.aorChanged) parent.aorChanged(true);
            }
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if ($obj.attr('id') && $obj.attr('id').indexOf('Rank') != -1) {
                if (nVal.length == 1) $obj.val('0' + nVal);
                return;
            }

            if ($obj.attr('field') == 'Allocation') {
                if (nVal == '') $obj.val('0');
                return;
            }

            $obj.val($.trim(nVal));
        }

        function txtDate_clear(obj) {
            var $obj = $(obj);

            $obj.prev().val('');
            $('#btnSave').prop('disabled', false);
        }

        function refreshPage(newID) {
            if (parent._aorChanged) {
                QuestionBox('Confirm Back', 'You have unsaved changes. Would you like to save or discard?', 'Save,Discard,Cancel', 'confirmClose', 400, 300, this);
            } else {
                var nURL = window.location.href;

                if (newID != undefined && parseInt(newID) > 0) {
                    if (parent.refreshPage) parent.refreshPage(newID);
                }
                window.location.href = 'Loading.aspx?Page=' + nURL;
            }
        }

        function confirmClose(answer) {
            switch ($.trim(answer).toUpperCase()) {
                case 'SAVE':
                    btnSave_click();
                    parent._aorChanged = false;
                    break;
                case 'DISCARD':
                    var nURL = window.location.href;
                    window.location.href = 'Loading.aspx?Page=' + nURL;
                    parent._aorChanged = false;
                    break;
                default:
                    return;
            }
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }

        function resizeFrame(section) {
            if (section == 'Resources') {
                $('#frame' + section)[0].height = $('#frame' + section).contents().find('[id$="AORResources"]').height() + $('#frame' + section).contents().find('[id$="AORActionTeam"]').height() + 144
            } else {
                if ($('#frame' + section).contents().find('table')[0]) $('#frame' + section)[0].height = $('#frame' + section).contents().find('table')[0].scrollHeight + $('#frame' + section).contents().find('[id$="Data_Grid"]').height() + $('#frame' + section).contents().find('[id$="PagerContainer"]').height();
            }
        }

        function togglePhase(obj) {
            var $obj = $(obj);

            if ($obj.attr('blnAll') == '1') {
                $('.phase').each(function () {
                    if ($(this).attr('title') == $obj.attr('title')) $(this).trigger('click');
                });
            }
            else {
                if ($obj.attr('title') == 'Show Work Activities') {
                    $obj.closest('tr').next().show();
                }
                else {
                    $obj.closest('tr').next().hide();
                }
            }

            if ($obj.attr('title') == 'Show Work Activities') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Hide Work Activities');
                $obj.attr('alt', 'Hide Work Activities');
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Show Work Activities');
                $obj.attr('alt', 'Show Work Activities');
            }
        }

        function btnAddEditAORAssoc_click() {
            //AOR Estimation
            var nWindow = 'AOREstimationAssoc';
            var nTitle = 'AOR Risk Estimation';
            var nHeight = 500, nWidth = 800;
            var AOREstimation_AORReleaseID = 0;

            $('#<%=this.divEstimationBody.ClientID %>').find('.gridBody').each(function () {
                if ($(this).find("td[id^='tdBOE_']").text() == 'Familiarity') {
                    AOREstimation_AORReleaseID = $(this).find("input[name='txtAOREstimation_AORReleaseID']").val();
                }
            });

            var nURL = 'AOREstimation_AORAssoc.aspx' + window.location.search + '&AOREstimation_AORReleaseID=' + AOREstimation_AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function AOREstimation_WeightValidation(obj) {
            var nVal = $(obj).val();

            //nVal = nVal.replace(/[^\d,]/g, '');

            if (nVal > 100) nVal = 100;

            $(obj).val(nVal);
        }

        var weight_Total = 0;
        function AOREstimation_CalcTotals() {

            var risk_high = 0;
            var risk_moderate = 0;
            var risk_low = 0;

            weight_Total = 0;
            var selectedOptionTotal = 0;
            $('#<%=this.divEstimationBody.ClientID %>').find('.gridBody').each(function () {
                var selectedOption = $(this).find("select[name^='ddlRisk'] option:selected").text();

                if ($(this).find("input[name^='txtWeight']").length > 0 && $(this).find("input[name^='txtWeight']").val().length > 0) {
                    weight_Total += parseInt($(this).find("input[name^='txtWeight']").val());
                }

                if (selectedOption == 'High (Emergency)') {
                    if ($(this).find("input[name^='txtWeight']").val().length > 0) {
                        risk_high += parseInt($(this).find("input[name^='txtWeight']").val());
                    }
                    selectedOptionTotal += 1;
                }
                if (selectedOption == 'Moderate (Acceptable)') {
                    if ($(this).find("input[name^='txtWeight']").val().length > 0) {
                        risk_moderate += parseInt($(this).find("input[name^='txtWeight']").val());
                    }
                    selectedOptionTotal += 1;
                }
                if (selectedOption == 'Low (Routine)') {
                    if ($(this).find("input[name^='txtWeight']").val().length > 0) {
                        risk_low += parseInt($(this).find("input[name^='txtWeight']").val());
                    }
                    selectedOptionTotal += 1;
                }
            });

            $('#<%=this.divEstimationBody.ClientID %>').find("label[for='lblWeight_Total']").text(weight_Total + '%');

            if (weight_Total > 100) {
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdWeight_Total']").css('background', 'red');
                //alert('Total weight cannot exceed 100%');
            }
            else if (weight_Total == 100) {
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdWeight_Total']").css('background', 'limegreen');
            } else {
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdWeight_Total']").css('background', 'yellow');
            }

            if (risk_high > 40) {
                $('#<%=this.divEstimationBody.ClientID %>').find("label[for='lblRisk_Total']").text('High (Emergency)');
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdRisk_Total']").css('background', 'red');
            }
            else if (risk_high + risk_moderate > 70) {
                $('#<%=this.divEstimationBody.ClientID %>').find("label[for='lblRisk_Total']").text('Moderate (Acceptable)');
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdRisk_Total']").css('background', 'yellow');
            }
            else {
                $('#<%=this.divEstimationBody.ClientID %>').find("label[for='lblRisk_Total']").text('Low (Routine)');
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdRisk_Total']").css('background', 'limegreen');
            }

            if (selectedOptionTotal == 0) {
                $('#<%=this.divEstimationBody.ClientID %>').find("label[for='lblRisk_Total']").text('Low (Not Maintained)');
                $('#<%=this.divEstimationBody.ClientID %>').find("td[id='tdRisk_Total']").css('background', 'limegreen');
            }

            //if (weight_Total > 100) {
            //    $('#btnSave').prop('disabled', true);
            //} else {
            $('#btnSave').prop('disabled', false);
            //}
        }

        function ddlSetClass(obj) {
            $(obj).removeClass();
            $(obj).addClass(obj.options[obj.selectedIndex].className);
        }

        function showOverride(show) {
            if (show == 1) {
                $('#divEstimationOverride').find("td[id^='tdOverride']").each(function () {
                    $(this).show();
                });

                if ('<%= this._showOverride %>' == '1') {
                    $('#divEstimationOverride').find("td[id^='tdSignOffOverride']").each(function () {
                        $(this).show();
                    });
                }
                $('#divEstimationOverride').css('width', '100%');
            } else {
                $('#divEstimationOverride').find("td[id^='tdOverride']").each(function () {
                    $(this).hide();
                });

                $('#divEstimationOverride').find("td[id^='tdSignOffOverride']").each(function () {
                    $(this).hide();
                });

                $('#divEstimationOverride').css('width', '150px');
            }
        }

        function enableOverrideJustification(obj) {
            if ($(obj).find('option:selected').val() == -1) {
                showOverride(0);
            } else {
                showOverride(1);
            }
        }

        function linkOverrideHistory_onClick(aorReleaseID) {
            PageMethods.GetOverrideHistory(aorReleaseID, GetOverrideHistory_done, on_error);
        }

        function GetOverrideHistory_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);

            if (dt == null || dt.length == 0) {
                nHTML = 'No History';
            }
            else {
                nHTML += '<table cellpadding="0" cellspacing="0">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<td style="font-weight: bold">Old Priority</td>';
                nHTML += '<td style="font-weight: bold">New Priority</td>';
                nHTML += '<td style="font-weight: bold">Old Justification</td>';
                nHTML += '<td style="font-weight: bold">New Justification</td>';
                nHTML += '<td style="font-weight: bold">Old Sign Off</td>';
                nHTML += '<td style="font-weight: bold">New Sign Off</td>';
                nHTML += '<td style="font-weight: bold">Archived?</td>';
                nHTML += '<td style="font-weight: bold">Created</td>';
                nHTML += '<td style="font-weight: bold">Updated</td>';
                nHTML += '</tr>';

                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';

                    nHTML += '<td style="text-align: center">' + row.Old_Priority + '</td>';
                    nHTML += '<td style="text-align: center">' + row.New_Priority + '</td>';
                    nHTML += '<td style="text-align: center">' + row.Old_Justification + '</td>';
                    nHTML += '<td style="text-align: center">' + row.New_Justification + '</td>';
                    nHTML += '<td style="text-align: center">' + row.Old_Bln_SignOff + '</td>';
                    nHTML += '<td style="text-align: center">' + row.New_Bln_SignOff + '</td>';
                    nHTML += '<td style="text-align: center">' + row.Bln_Archive + '</td>';
                    nHTML += '<td style="text-align: center">' + row.CreatedBy + ':' + '<br />' + row.CreatedDate + '</td>';
                    nHTML += '<td style="text-align: center">' + row.UpdatedBy + ':' + '<br />' + row.UpdatedDate + '</td>';

                    if (rowIndex == dt.length - 1) nHTML += '</tr>';
                });

                nHTML += '</table>';
            }

            $('#divOverrideHistory').html(nHTML);
            $('#divAOREOverrideHistory').show();
        }


        function closeModal() {
            try {
                if ($("#divAOREOverrideHistory:visible").length > 0) {
                    $("#divAOREOverrideHistory").hide();
                }
            } catch (ex) {

            }
        }


        function loadHelpText() {

            var nHTML = '';
            var pTitle = 'AOR Estimation';
            
            nHTML += '<br><ul class="helpList"><li><b>AOR Estimation</b></li>';
            nHTML += '<ul><li><b>Risks and Weights:</b> two variables used to measure total impact a particular area will have on the AOR. For example,';
            nHTML += '<br>having Low Weight and High Risk for Effort means that the AOR will require a lot of effort but since the weight is low -';
            nHTML += '<br>the overall impact on the AOR Scope will be low.Conversely, if both the risk and weight are high for Effort then it means that Effort ';
            nHTML += '<br>has a high impact on the AOR.Thus, the program manager may have consider de- scoping such that risk/ weight is lowered for Effort</li>';
            nHTML += '<br><li><b>Effort:</b> Highlights the Scope/ level of effort required to accomplish tasks for a particular AOR. ';
            nHTML += '<br>If Effort is rated as high, the program manager has to consider descoping given the PM has a set number of resources and a set schedule. </li>';
            nHTML += '<br><li><b>Familiarity:</b> This area measures how familiar the resources are with requirements of a particular AOR.  ';
            nHTML += '<br>For example: A senior developer, familiar with the way ITI codes will be more familiar with coding practices than a new developer. ';
            nHTML += '<br>Additionally, if we have done this type of work in the past, the developer team may be very familiar with it and can code this quickly. ';
            nHTML += '<br>If we have not done this work in the past, the developer team will spend more time investigating and working with the customer ';
            nHTML += '<br>and will not be able to produce as much code.If this area is marked high risk, the program manager must consider descoping in order ';
            nHTML += '<br>to manage expectations based on the level of familiarity the team has with it.Additionally, the number of resources the PM has and ';
            nHTML += '<br>the schedule is set and cannot be changed. </li>';
            nHTML += '<br><li><b>Non-Resource Personnel:</b> This area is used to measure risk related non-personnel resources such as use to external reporting systems,  ';
            nHTML += '<br>tools, documents, etc.If external products cannot be obtained timely, the program manager may need to descope by moving this work to a future deployment  ';
            nHTML += '<br>until the work can be accomplished.</li>';
            nHTML += '<br><li><b>Process Volatility:</b> This area is used to measure risk related with the volatility of the processes that an AOR is supporting.';  
            nHTML += '<br>For example, the change being made to the code could affect multiple websystems.Could impact inputs coming in or out of the system. </li>';
            nHTML += '<br><li><b>Timeline:</b> This field is used to predict whether an AOR will meet Schedule or not.   The PM can consider descoping if there is high risk ';
            nHTML += '<br> area as the schedule and resources are set and cannot be changed.Note: Schedule is set because of the process start date; it cannot be moved.';
            nHTML += '<br>It is fixed.Resources are set because we have a firm fixed price contract; therefore, we only estimate the scope and whether or not we can deliver ';
            nHTML += '<br>that scope.We will descope and move items to future deployments if high risk. </li>';
            nHTML += '<br><li><b>Override Estimation:</b> Allows user to manipulate the overall risk of an AOR - this override will supersede the estimated risk and will require a justification.'; 
            nHTML += '<br>This will be reflected on the DSE Report.</li>';
            nHTML += '<br><li><b>Estimated Net Resources:</b> Initial estimation of the resources that will be working on an AOR.</li></ul></ul><br>';
                           
            MessageBox(nHTML, pTitle);
        }

        function basisOfEstimateChanged(obj, type) {
            if (type == "RISK") {
                if ($(obj).attr('orig_val') != $(obj).find('option:selected').val()) {
                    parent._aorChanged = true;
                } else {
                    parent._aorChanged = false;
                }
            }
            if (type == "WEIGHT") {
                if ($(obj).attr('orig_val') != $(obj).val()) {
                    parent._aorChanged = true;
                } else {
                    parent._aorChanged = false;
                }
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _rscSelIdx = 10000;
            if (parent.aorChanged) parent.aorChanged(false);
        }

        function initControls() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('.date').datepicker({
                    dateFormat: 'm/d/yy'
                });
            }
        }

        function initDisplay() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('input[type="text"], textarea').css('color', 'black');
                $('input[type="text"], textarea').not('.date').removeAttr('readonly');
                $('select, input[type="checkbox"]').not('#<%=this.ddlProductVersion.ClientID %>, #<%=this.ddlWorkType.ClientID %> , #<%=this.ddlWorkloadAllocation.ClientID %>').removeAttr('disabled');
                $('.clearDate').show();
                $('#btnCancel').show();
                $('#btnSave').show();
            }

            if ('<%=this.NewAOR %>'.toUpperCase() == 'FALSE') {
                $('#divInfo').show();
                $('#trReleaseStatus').show();
                $('#divHistoryContainer').show();
            }

            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE' && '<%=this.NewAOR %>'.toUpperCase() == 'FALSE') $('#btnArchive').show();

            updateTab('Systems', <%=this.systemCount%>);

            if ($('#frameCRs').attr('src') == "javascript:'';") $('#frameCRs').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORCRs + window.location.search);
            if ($('#frameTasks').attr('src') == "javascript:'';") $('#frameTasks').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORTasks + window.location.search + '&ReleaseAOR=' + ($('#<%=this.ddlWorkType.ClientID %>').val() == 2));
            if ($('#frameResources').attr('src') == "javascript:'';") $('#frameResources').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORResourceTeam + window.location.search);

            if ('<%= this._showOverride %>' == '1') {
                showOverride(1)
                $('#divEstimationOverride').css('width', '100%');
            } else {
                $('#divEstimationOverride').css('width', '150px');
            }

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#imgSettings').click(function () { imgSettings_click(); });
            $('#btnArchive').click(function () { btnArchive_click(); return false; });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnSettingsExpand').click(function () { btnSettingsExpand_click(); return false; });
            $('.toggleSection').on('click', function () { imgToggleSection_click(this, true); });
            $('#<%=this.chkAORRequiresPD2TDR.ClientID %>').change(function () { ddlWorkType_change(); return false; });
            <%--$('#<%=this.ddlPrimarySystem.ClientID %>').change(function () { ddlPrimarySystem_change(); return false; });--%>
            $('#<%=this.ddlWorkType.ClientID %>').change(function () { ddlWorkType_change(); return false; });
            $('#<%=this.ddlCyber.ClientID %>').change(function () { ddlCyber_change(); return false; });
            $('input[type="text"], textarea').not('.date').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('.date').change(function () { input_change(this); });
            $('.clearDate').click(function () { txtDate_clear(this); });
            $('#btnSaveSystemSelections').on('click', function () { saveSystemSelections(); });
            $('#btnSaveResourceSelections').on('click', function () { saveSystemResourceSelections(); });
            $('#btnCloseSystemSelections').on('click', function () { closeSystemSelections(); });
            $('#<%=this.btnAddEditAORAssoc.ClientID %>').on('click', function () { btnAddEditAORAssoc_click(); return false; })

            //AOR Estimation checks and dynamic calculations
            $("select[name^='ddlRisk']").on('change', function () { AOREstimation_CalcTotals(); ddlSetClass(this); basisOfEstimateChanged(this, 'RISK'); });
            $("input[name^='txtWeight']").on('blur', function () { AOREstimation_WeightValidation(this); AOREstimation_CalcTotals(); basisOfEstimateChanged(this, 'WEIGHT'); });
            $("input[name^='txtDetails']").on('keyup paste', function () { parent.aorChanged(true); });
            $("input[name^='txtMitigation']").on('keyup paste', function () { parent.aorChanged(true); });
            $('#<%=this.ddlEstimationOverride.ClientID %>').change(function () { enableOverrideJustification(this); ddlSetClass(this); return false; });

            $(window).resize(resizePage);

            msAORSystems_init();
        }

        $(document).ready(function () {
	        <%--if($(defaultParentPage.itisettings).text() === "") $(defaultParentPage.itisettings).text($("#<%=itisettings.ClientID %>").val());
			reorderSectionList();
	        showSectionList();--%>
            initVariables();
            initControls();
            initDisplay();
            initEvents();

            setupAORType(true);
            setupCyber(true);
            setupMissionDate(true);

            AOREstimation_CalcTotals();
            $("select[name^='ddlRisk']").each(function () {
                ddlSetClass(this);
            });

            $('#<%=this.ddlEstimationOverride.ClientID %>').each(function () {
                ddlSetClass(this);
            });

            $('#tableSettingsAttributes input').each(function () {
                if ($(this)[0].checked) imgToggleSection_click($('.toggleSection[data-section="' + $(this).attr('field') + '"]'));
            });
        });
    </script>
</asp:Content>
