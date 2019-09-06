﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WTS - Workload Tracking System</title>
    <link rel="shortcut icon" href="Images/fav_icon.ico" type="image/x-icon" />
    <link href="Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" />
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="scripts/filter.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
    <script type="text/javascript" src="Scripts/iti_FilterContainer.js"></script>
    <script type="text/javascript" src="Scripts/SmoothMovement.js"></script>
</head>
<body style="overflow:hidden;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
            <Services>
                <asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
            </Services>
        </asp:ScriptManager>
        <div id="defaultPageContainer" style="overflow: hidden; vertical-align: top;">
            <div id="itisettings" runat="server" style="display: none"></div>
            <div id="mainPageHeader" style="width: 100%; border-bottom: 1px solid grey; padding: 0px;">
                <%-- Removing cellpadding="0" cellspacing="0" here messes up page, leave them. --%>
                <table cellpadding="0" cellspacing="0" style="width: 100%;">
                    <tr>
                        <td id="headerLeftMain" style="background: url('Images/Headers/gridheaderbluedark.png'); width: 100%; height: 65px;">
                            <span style="padding-left:5px; font-size:24px; color:whitesmoke;">WTS - </span><span style="font-size:14px; color:whitesmoke;">Workload Tracking System</span>
                            <div id="divTestSystemIndicator" runat="server" style="display: none; position: absolute; top: 5px; left: 45%; border: solid 1px grey; background: #ffe8a0; padding: 4px; color: Red; font-weight: bold;">
                                <%=FromServer%>
                            </div>
                        </td>
                    </tr>
                </table>
                <div id="appUserInfo" style="position: absolute; top: 0px; right: 10px;">
                    <table>
                        <tr>
                            <td style="vertical-align:middle; height:30px;">
                                <asp:Label ID="lblUser" runat="server" Style="padding-right: 8px; font-size:14px; color:white; height:30px;"></asp:Label>
                                <asp:LoginName ID="LoginNameControl" runat="server" ToolTip="View/Edit Profile" Style="cursor: pointer; font-size:14px; color:whitesmoke;" />
                                <asp:LoginStatus ID="LoginStatusControl" runat="server" LoginText="Sign In/Register" LogoutText="Logout" LogoutAction="Refresh" Style="font-size: 14px; color: whitesmoke;" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="MaintenanceMenu" style="position: absolute; top: 35px; right: 5px; text-align: right; color: white;">
                    <div id="btnCVT" style="float: right; margin-right: 15px;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgCVT" alt="Open/Close CVT Menu" style="width: 22px; height: 22px;" src="Images/appheader/CVT.png" />
                                </td>
                                <td id="tdCVT" style="cursor: pointer;">
                                    <iti_Tools_Sharp:Menu ID="menuCVT" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="CVT" Style="display: inline-block; float: left;"></iti_Tools_Sharp:Menu>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="btnHelp" style="float: right; margin-right: 15px;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgHelp" alt="Open/Close Help Menu" style="width: 22px; height: 22px;" src="Images/appheader/help2.png" />
                                </td>
                                <td id="tdHelp" style="cursor: pointer;">
                                    <iti_Tools_Sharp:Menu ID="menuHelp" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Help" Style="display: inline-block; float: left;">

                                    </iti_Tools_Sharp:Menu>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="buttonSR" style="float: right; margin-right: 15px; cursor: pointer;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgSR" alt="Submit Sustainment Request" style="width: 22px; height: 22px;" src="Images/appheader/sr.png" />
                                </td>
                                <td style="padding-left: 2px;">SR
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="buttonRequestedReports" style="float: right; margin-right: 15px; cursor: pointer;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgRequestedReports" alt="View Requested Reports" style="width: 22px; height: 22px;" src="Images/appheader/req_report.png" />
                                </td>
                                <td style="padding-left: 2px;">Requested Reports
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="buttonSRHotList" style="float: right; margin-right: 15px; cursor: pointer; display: none;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgSRHotList" alt="Service Request Hotlist" style="width: 22px; height: 22px;" src="Images/appheader/req_report.png" />
                                </td>
                                <td style="padding-left: 2px;">EMail SR Report
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="buttonEmailHotList" style="float: right; margin-right: 15px; cursor: pointer; display: none;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgEmailHotList" alt="EMail Hotlist" style="width: 22px; height: 22px;" src="Images/appheader/req_report.png" />
                                </td>
                                <td style="padding-left: 2px;">EMail Hotlist
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="buttonMyProfile" style="float: right; margin-right: 15px; cursor: pointer;">
                        <table style="text-align: center;">
                            <tr>
                                <td>
                                    <img id="imgMyProfile" alt="Open/Close Profile Menu" style="width: 22px; height: 22px;" src="Images/appheader/myProfile.png" />
                                </td>
                                <td id="tdMyProfile" style="padding-left: 2px;">
                                    <iti_Tools_Sharp:Menu ID="menuMyProfile" runat="server" Align="Right" ClientClickEvent="myProfile_Select" Text="My Profile" Style="display: inline-block; float: left;">
                                        <Menus>
                                            <iti_Tools_Sharp:itiMenu>
                                                <Items>
                                                    <iti_Tools_Sharp:itiMenuItem Text="My Profile" Value="My Profile" Imagesource="Images/infintech_black.png"></iti_Tools_Sharp:itiMenuItem>
                                                    <iti_Tools_Sharp:itiMenuItem Text="Change Password / Security Question" Value="PasswordQuestion" ImageSource="Images/infintech_black.png"></iti_Tools_Sharp:itiMenuItem>
                                                </Items>
                                            </iti_Tools_Sharp:itiMenu>
                                        </Menus>
                                    </iti_Tools_Sharp:Menu>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
<%--            Removing cellpadding="0" cellspacing="0" here messes page up.--%>
            <table id="mainPageContent" cellpadding="0" cellspacing="0" style="width: 100%; border-bottom: 1px solid grey; border-top: 1px solid grey; text-align: left; vertical-align: top;">
                <tr>
                    <td id="sidebar" style="width: 225px; vertical-align: top; border-bottom: 1px solid grey;">
<%--            Removing cellpadding="0" cellspacing="0" here messes page up.--%>
                        <table id="tablePageManager" cellpadding="0" cellspacing="0" style="width: 100%; border-left: 1px solid #9A9A9A;">
                            <tr>
                                <td style="width: 100%;">
                                    <div id="pageManager" style="width: 100%;"></div>
                                    <div id="pageManagerHeader" style="width: 100%; border-bottom: 1px solid #9A9A9A; border-right: 1px solid #9A9A9A;">
                                        <table id="tableSelectedModule" class="modules" cellpadding="0" cellspacing="0" style="width: 100%; border-right: 1px solid #9A9A9A;">
                                            <tr id="trSelectedModule" class="moduleRow" direction="EXPAND" moduleid="0" modulename="">
                                                <td id="tdSelectedModuleImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img id="imgSelectedModule" src="Images/icons/blank.png" style="width: 16px; height: 16px;" alt="" />
                                                </td>
                                                <td id="tdSelectedModuleText" class="moduleText" style="height: 18px;">&nbsp;</td>
                                                <td id="tdSelectedModuleArrow" class="moduleArrow" style="text-align: right; padding-right: 4px; height: 18px;">
                                                    <img id="imgModuleMenu" alt="Expand Module Selections Menu" title="Expand Module Selections Menu" src="Images/fatArrowDown.png" style="width: 14px; height: 14px;" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table id="tableAvailableModules" class="modules" cellpadding="0" cellspacing="0" style="width: 100%; display: none; border-right: 1px solid #9A9A9A;">
                                            <tr id="trWork" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="1" modulename="Work">
                                                <td id="tdDailyWorkloadImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/page_white_edit.png" style="width: 16px; height: 16px;" alt="Daily Workload" /></td>
                                                <td id="tdDailyWorkloadText" class="moduleText">Work Tasks</td>
                                            </tr>

                                            <tr id="trDeveloperReview" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="5" modulename="DeveloperReview">
                                                <td id="tdDeveloperReviewImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/page_white_edit.png" style="width: 16px; height: 16px;" alt="Developer Review" /></td>
                                                <td id="tdDeveloperReviewText" class="moduleText">Developer Review</td>
                                            </tr>
                                            <tr id="trDailyReview" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="6" modulename="DailyReview">
                                                <td id="tdDailyReviewImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/page_white_edit.png" style="width: 16px; height: 16px;" alt="Daily Review" /></td>
                                                <td id="tdDailyReviewText" class="moduleText">Daily Review</td>
                                            </tr>

                                            <tr id="trMeetingModule" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="10" modulename="MeetingModule">
                                                <td id="tdMeetingModuleImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/calendar_2.png" style="width: 16px; height: 16px;" alt="Meeting Module" /></td>
                                                <td id="tdMeetingModuleText" class="moduleText">Meeting Module</td>
                                            </tr>
                                            <tr id="trAoR" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="7" modulename="AoR">
                                                <td id="tdAoRImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/page_white_edit.png" style="width: 16px; height: 16px;" alt="PMO (Program Management Operations)" /></td>
                                                <td id="tdAoRText" class="moduleText">Projects</td>
                                            </tr>
                                            <tr id="trRQMT" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="8" modulename="RQMT">
                                                <td id="tdRQMTImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/Icons/check_gray.png" style="width: 16px; height: 16px;" alt="RQMT MGMT" /></td>
                                                <td id="tdRQMTText" class="moduleText">RQMT Traceability</td>
                                            </tr>
                                            <tr id="trReports" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="2" modulename="Reports">
                                                <td id="tdReportsImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/newspaper.png" style="width: 16px; height: 16px;" alt="Reports" />
                                                </td>
                                                <td id="tdReportsText" class="moduleText">Reports</td>
                                            </tr>
                                            <tr id="trSR" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="9" modulename="SR">
                                                <td id="tdSRImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/Icons/page_white_wrench.png" style="width: 16px; height: 16px;" alt="Sustainment Request" /></td>
                                                <td id="tdSRText" class="moduleText">Sustainment Request</td>
                                            </tr>
                                            <tr id="trMasterData" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="3" modulename="MasterData">
                                                <td id="tdMasterDataImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/page_white_edit.png" style="width: 16px; height: 16px;" alt="Master Data" /></td>
                                                <td id="tdMasterDataText" class="moduleText">Master Data</td>
                                            </tr>
                                            <tr id="trAdmin" runat="server" disabled="false" visible="false" class="moduleRow" moduleid="4" modulename="Administration">
                                                <td id="tdAdminImage" class="moduleImage" style="width: 20px; padding-left: 4px; padding-right: 3px;">
                                                    <img src="Images/icons/lock_with_key.png" style="width: 16px; height: 16px;" alt="Administration" />
                                                </td>
                                                <td id="tdAdminText" class="moduleText">Administration</td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="pageManagerBody" style="width: 100%; border-right: 1px solid #9A9A9A; overflow: hidden;">
                                        <div id="divModuleOptions" style="height: 160px; width: 100%; border-bottom: 1px solid #B9B9B9; border-right: 1px solid #9A9A9A; overflow-y:scroll; overflow-x: hidden;">
                                            <table id="tableModuleContainer" cellpadding="1" cellspacing="0" style="width: 100%; cursor: default;">
                                            </table>
                                        </div>
                                        <div id="divModuleQuickFilters" class="pageContentInfo" style="width: 100%; height: auto;">
                                            <table id="tableModuleOptions" style="width: 100%; border-bottom: 1px solid white; border-right: 1px solid #9A9A9A;">
                                                <tr id="trWorkView" tag="Workload,QMWorkload,MultiLevelCrosswalk,WorkloadCrosswalk,WorkRequest,Hotlist">
                                                    <td style="padding-left: 5px; width: 40px; vertical-align: middle;">View
                                                    </td>
                                                    <td style="text-align: left; padding: 3px; padding-right: 3px;">
                                                        <select id="ddlView_Work" runat="server" style="width: 120px; font-size: 11px;"></select>
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: right; padding: 3px;">
                                                        <img id="imgViewHelp" src="Images/Icons/help.png" title="Help" alt="Help" style="cursor: pointer;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr id="trFilterManager">
                                <td style="width: 100%;">
                                    <div id="divFilterManager" style="width: 100%;">
                                        <div id="divFilterManagerHeader" style="display: none;">
                                            <table id="tableSearch" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A;">
                                                <tr>
                                                    <td style="padding-left: 5px; height: 25px; width:40px; vertical-align: middle;">Search
                                                    </td>
                                                    <td style="text-align: left; padding-top: 2px; padding-left: 3px; height: 25px;">
                                                        <select id="ddlSearch" runat="server" style="width: 120px; font-size: 11px;">
                                                            <option>- Select -</option>
                                                        </select>
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: right; height: 25px; padding-top: 2px; padding-right: 3px;">
                                                        <img id="imgShowSearch" src="Images/icons/find.png" alt="Open Search" style="cursor: pointer;" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tableFilter" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A; display: none;">
                                                <tr>
                                                    <td style="padding-left: 5px; height: 25px; width: 40px; vertical-align: middle;">Filters
                                                    </td>
                                                    <td style="text-align: left; padding-top: 0px; padding-left: 0px; height: 23px;">
                                                        <iti_Tools_Sharp:DropDownList ID="ddlSavedFilters" BackColor="White" runat="server" Font-Size="11px" Width="114px" onchange="setCustomFilter();" />
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: right; height: 25px; padding-top: 2px; padding-right: 2px;">
                                                        <img id="imgSaveFilter" src="Images/icons/disk.png" title="Save Custom Filter Set" alt="Save Filter" style="cursor: pointer;" />
                                                    </td>
                                                    <td style="vertical-align: middle; text-align: right; height: 25px; padding-top: 2px; padding-right: 3px;">
                                                        <img id="imgDeleteFilter" src="Images/icons/delete.png" title="Delete Custom Filter Set" alt="Delete Filter" style="cursor: pointer;" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div id="divPageManagerFooter" style="width: 100%; background-color: white; display: none;">
                                            <table id="tblSpacer" style="width: 100%; height: 4px; border-right: 1px solid #9A9A9A;">
                                                <tr>
                                                    <td style="background-image: url(Images/page_spacer_horizontal.gif); height: 4px; overflow: hidden;"></td>
                                                </tr>
                                            </table>
                                            <table id="tablePageManagerButtons" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A; border-bottom: 1px solid #9A9A9A;">
                                                <tr>
                                                    <td style="vertical-align: middle; text-align: left; height: 30px;">
                                                        <img id="imgShowFilters" src="Images/icons/funnel.png" title="Assign Filters" alt="Assign Filters" style="cursor: pointer; padding-left: 5px;" />
                                                        <img id="imgClearFilters" src="Images/icons/eraser.png" title="Clear Filters" alt="Clear Filters" style="cursor: pointer; padding-left: 3px;" />
                                                    </td>
                                                    <td style="text-align: right; height: 30px;">
                                                        <img id="imgAORHome" src="Images/Icons/house.png" title="AOR Home" alt="AOR Home" style="cursor: pointer; padding-left: 3px; display: none;" />
                                                        <img id="imgNews" src="Images/icons/newspaper.png" title="Home Tabs" alt="Home Tabs" style="cursor: pointer; padding-left: 3px;" />
                                                        <img id="imgMetrics" src="Images/icons/report.png" title="View Metrics" alt="View Metrics" style="cursor: pointer; padding-left: 3px; padding-right: 5px; visibility: hidden" />
                                                        <input type="button" id="buttonGetData" value="Get Data" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>


                                        <div id="divFilterManagerBody">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td id="tdAppliedFilters" style="width: 100%; vertical-align: top; font-size: 12px;">
                                                        <div id="divAppliedFilters" class="filterContainer" style="width: 216px;">
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>


<%--                                        <div id="divFilterManagerBody" style="width: 209px; border-bottom: 1px solid #9A9A9A; background-color: white;">
                                            <div style="overflow-x: hidden; overflow-y: auto; height: 100%; width: 208px; border-right: 1px solid #9A9A9A;">
                                                <table cellpadding="0" cellspacing="0" style="width: 100%;">
                                                    <tr>
                                                        <td id="tdAppliedFilters" style="width: 100%; vertical-align: top; font-size: 12px;">
                                                            <div id="divAppliedFilters" class="filterContainer" style="width: 208px; overflow-x: hidden; overflow-y: auto; margin: 0px;">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>--%>


                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td id="pageSpacer" style="width: 6px; background: url(Images/page_spacer_vertical.gif); text-align: center; vertical-align: middle; border-bottom: 1px solid grey;" title="Show/Hide Navigation Pane">
                        <img id="imgPageSpacerVertical" src="Images/pageRoller.gif" alt="Show/Hide Navigation Pane" title="Show/Hide Navigation Pane" style="cursor: pointer;" />
                    </td>
                    <td id="content" style="color: Black; vertical-align: top; border-left: 1px solid #9A9A9A; border-right: 1px solid #9A9A9A; border-bottom: 1px solid grey;">
                        <div id="pageContentDiv" style="width: 100%; height: 100%; text-align: center; background-color: White;">
                            <iframe id="pageContentFrame" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;"></iframe>
                        </div>
                    </td>
                </tr>
            </table>
        </div>

        <div id="divSystemNotification" runat="server" style="position: absolute; bottom: 0px; overflow: hidden; width: 300px; height: 30px;">
            <div style="border: 1px solid grey; border-bottom: 0px;">
                <div>
                    <table id="Table2" cellpadding="0" cellspacing="0" style="font-size: 8pt; font-family: Arial; width: 100%; border-bottom: solid 1px grey;">
                        <tr id="trNotification" style="background-image: url(Images/Headers/main_header_background_grey.jpg); cursor: pointer;" title="Open Notification">
                            <td id="tdNotification" style="text-align: center; font-size: 14px; height: 26px;">System Notification
                            </td>
                            <td style="width: 16px; padding-right: 6px; padding-top: 3px; height: 26px;">

                                <img src="images/fatArrowDown.png" id="imgNotificationArrow" alt="Open Notification" style="cursor: pointer;" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="divSystemNoteInfo" runat="server" style="background: white; overflow: auto; padding-right: 4px; padding-left: 4px; height: 268px;">
                </div>

            </div>
        </div>

        <div id="mainPageFooter" style="width: 100%; height: 30px; background-color: #333333; position: absolute; bottom: 0px; left: 0px; background-image: url(Images/Headers/main_header_background_grey.jpg);">
            <table style="width: 100%; height: 100%;">
                <tr>
                    <td style="padding-left: 5px; vertical-align: middle; color: grey; border-bottom: solid 1px grey; border-right: solid 1px grey;">
                        Copyright © <asp:Label runat="server" ID="lblCopyRight"></asp:Label> - <asp:Label runat="server" ID="lblContractorNm"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>

        <div id="popupContainer" class="popupPageContainer"></div>
        <div id="divPageDimmer" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: grey; filter: alpha(opacity=60); opacity: .60; display: none;"></div>
        <div id="divFilterSaver" style="position: absolute; left: 38%; top: 30%; padding: 10px; background: white; border: solid 1px grey; font-size: 18px; text-align: center; display: none;">
            <table>
                <tr>
                    <td>WTS is Applying your Filters...  Please wait...
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;">
                        <img alt='' src="Images/loaders/progress_bar_blue.gif" />
                    </td>
                </tr>
            </table>
        </div>

        <div id="divHiddenFields" style="display:none;">
            <asp:ListBox ID="lstWork" runat="server" moduleName="Work">
                <asp:ListItem Text="Multi-Level Crosswalk" Value="MultiLevelCrosswalk" />
                <%--<asp:ListItem Text="QM Workload Crosswalk" Value="WorkloadCrosswalk" />--%>
                <asp:ListItem Text="Workload" Value="Workload" />
                <%--<asp:ListItem Text="QM Attribute" Value="QMWorkload" />--%>
            </asp:ListBox>
            <asp:ListBox ID="lstMeetingModule" runat="server" moduleName="MeetingModule">
                <asp:ListItem Text="Meeting" Value="AORMeeting" />
            </asp:ListBox>
            <asp:ListBox ID="lstDeveloperReview" runat="server" moduleName="DeveloperReview">
                <asp:ListItem Text="Workload" Value="Workload" />
            </asp:ListBox>
            <asp:ListBox ID="lstDailyReview" runat="server" moduleName="DailyReview">
                <asp:ListItem Text="Workload" Value="Workload" />
            </asp:ListBox>
            <asp:ListBox ID="lstAoR" runat="server" moduleName="AoR">
                <asp:ListItem Text="AOR" Value="AOR" />
                <asp:ListItem Text="CR" Value="CR" />
                <asp:ListItem Text="Deployments" Value="Deployments" />
                <asp:ListItem Text="Release Assessment" Value="ReleaseAssessment" />
            </asp:ListBox>
            <asp:ListBox ID="lstRQMT" runat="server" moduleName="RQMT">
                <asp:ListItem Text="Sys Rqmt Sets" Value="RQMT" />
                <%--<asp:ListItem Text="RQMT Description" Value="RQMTDescription" />--%>
            </asp:ListBox>
            <asp:ListBox ID="lstReports" runat="server" moduleName="Reports">
                <%--<asp:ListItem Text="Area of Responsibility(AOR)" Value="AOR"></asp:ListItem>--%>
                <asp:ListItem Text="Workload Summary Report" Value="Workload_Summary"></asp:ListItem>
                <asp:ListItem Text="CR" Value="CR"></asp:ListItem>
                <asp:ListItem Text="Task" Value="Task"></asp:ListItem>
                <asp:ListItem Text="Release DSE" Value="Release_DSE"></asp:ListItem>
            </asp:ListBox>
            <asp:ListBox ID="lstSR" runat="server" moduleName="SR">
                <asp:ListItem Text="SR" Value="SR" />
            </asp:ListBox>
            <asp:ListBox ID="lstMasterData" runat="server" moduleName="MasterData">
                <%--<asp:ListItem Text="Allocation Group" Value="AllocationGroup"></asp:ListItem>
                <asp:ListItem Text="Allocation Assignment" Value="Allocation"></asp:ListItem>--%>
                <%--<asp:ListItem Text="AOR Workload Type" Value="AORType"></asp:ListItem>--%>
                <%--<asp:ListItem Text="Area/Size of Effort" Value="EffortArea"></asp:ListItem>--%>
                <asp:ListItem Text="AOR Estimation" Value="AOREstimation"></asp:ListItem>
                <asp:ListItem Text="Contract" Value="Contract"></asp:ListItem>
                <asp:ListItem Text="CR Report Narrative" Value="Narrative"></asp:ListItem>
                <asp:ListItem Text="Image" Value="Image"></asp:ListItem>
                <asp:ListItem Text="PD2TDR Phase" Value="Phase"></asp:ListItem>
                <asp:ListItem Text="Priority" Value="Priority"></asp:ListItem>
                <asp:ListItem Text="Progress" Value="Progress" Enabled="false"></asp:ListItem>
                <asp:ListItem Text="Release Version" Value="Version"></asp:ListItem>
                <asp:ListItem Text="Resource Group" Value="ResourceGroup"></asp:ListItem>
                <asp:ListItem Text="Requirement Type" Value="RQMTType"></asp:ListItem>
                <%--<asp:ListItem Text="Requirement Description Type" Value="RQMTDescriptionType"></asp:ListItem>--%>
                <%--<asp:ListItem Text="Scope" Value="Scope"></asp:ListItem>--%>
                <asp:ListItem Text="Status" Value="Status"></asp:ListItem>
                <asp:ListItem Text="System Suite" Value="SystemSuite"></asp:ListItem>
                <asp:ListItem Text="System(Task)" Value="SystemProcess"></asp:ListItem>
                <asp:ListItem Text="Work Activity" Value="WorkActivity"></asp:ListItem>
                <asp:ListItem Text="Work Activity Group" Value="WorkActivityGroup"></asp:ListItem>
                <asp:ListItem Text="Work Area" Value="WorkArea"></asp:ListItem>
                <%--<asp:ListItem Text="Work Request Group" Value="RequestGroup"></asp:ListItem>--%>
                <asp:ListItem Text="Workload Allocation" Value="WorkloadAllocation"></asp:ListItem>
                <asp:ListItem Text="Workload Group (Functionality)" Value="WorkloadGroup"></asp:ListItem>
            </asp:ListBox>
            <asp:ListBox ID="lstAdministration" runat="server" moduleName="Administration">
                <asp:ListItem Text="Resource MGMT" Value="Resource_MGMT"></asp:ListItem>
                <asp:ListItem Text="Organization" Value="Organization"></asp:ListItem>
                <asp:ListItem Text="Roles" Value="Roles"></asp:ListItem>
                <%--<asp:ListItem Text="Email Hotlist" Value="Hotlist"></asp:ListItem>--%>
                <%--<asp:ListItem Text="SR Configuration" Value="SRConfiguration"></asp:ListItem>--%>
            </asp:ListBox>
            <asp:ListBox ID="lstCustomFilters" runat="server"></asp:ListBox>
            <asp:ListBox ID="lstSearch" runat="server"></asp:ListBox>

            <iframe id="frameApplyFilter" name="frameApplyFilter"></iframe>
            <iframe id="frameSaveFilter" name="frameSaveFilter"></iframe>

        </div>

        <script id="jsPopup" type="text/javascript">
            var popupContainer = document.getElementById('popupContainer');
            var popupManager = new PopupWindowManager(popupContainer);
        </script>

        <script id="js" type="text/javascript">
            var _pageUrls = new PageURLs();
            var _userId = 0;

            //Meeting popup
            var _expandedAORs = [];
            var _expandedNotes = [];
            var _checkedShowClosedSRs = [];
            var _checkedShowClosedTasks = [];
            var _checkedShowRemoved = [];
            var _checkedShowClosed = [];

            // use this for general grid node tracking to maintain open nodes when things are clicked
            // this array should be keyed by grid type, and each type should contain an array that has identifiers for what nodes are open (urls, ids, etc...)
            // NOTE: this array is reset every time the user hits Get Data
            var _expandedGridNodes = [];
            var _moduleCachedData = []; // contains a cache for each module type, such as 'rqmt'; it is reset when the module hits get data
        </script>

        <script id="jsSidebar" type="text/javascript">

            function openWorkPage(optionName, forceReload) {
                var url = '';
                var usePopup = false;
                var parentModule = $('#trSelectedModule').attr('moduleName');
                var parentModuleFilter = parentModule;
                if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModuleFilter = 'Work';

                switch (optionName.toUpperCase()) {
                    case 'AOR':
                        url = _pageUrls.Maintenance.AORContainer
                            + '?'
                            + 'GridType=AOR';
                        break;
                    case 'AORMEETING':
                        url = _pageUrls.Maintenance.AORContainer
                            + '?'
                            + 'GridType=AORMeeting';
                        break;
                    case 'CR':
                        url = _pageUrls.Maintenance.AORContainer
                            + '?'
                            + 'GridType=CR';
                        break;
                    case 'DEPLOYMENTS':
                        url = _pageUrls.Maintenance.AORContainer
                            + '?'
                            + 'GridType=Deployments';
                        break;
                    case 'RELEASEASSESSMENT':
                        url = _pageUrls.Maintenance.AORContainer
                            + '?'
                            + 'GridType=ReleaseAssessment';
                        break;
                    case 'RQMT':
                        url = _pageUrls.Maintenance.RQMTContainer
                            + '?'
                            + 'GridType=RQMT';
                        break;
                    case 'RQMTDESCRIPTION':
                        url = _pageUrls.Maintenance.RQMTContainer
                            + '?'
                            + 'GridType=RQMTDescription';
                        break;
                    case 'SR':
                        url = _pageUrls.Maintenance.SRContainer
                            + '?'
                            + 'GridType=SR';
                        url += '&SubmittedBy=';
                        if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Submitted By' }).length > 0) {
                            url += filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Submitted By' })[0].parameters.toString(',', true);
                        }
                        url += '&StatusIDs=';
                        if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Status' }).length > 0) {
                            url += filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Status' })[0].parameters.toString(',', false);
                        }
                        url += '&ReasoningIDs=';
                        if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Reasoning' }).length > 0) {
                            url += filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Reasoning' })[0].parameters.toString(',', false);
                        }
                        url += '&Systems=';
                        if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'System' }).length > 0) {
                            url += filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'System' })[0].parameters.toString(',,', true);
                        }
                        break;
                    case 'WORKREQUEST':
                        url = _pageUrls.Maintenance.WorkRequestContainer
                            + '?'
                            + 'GridType=Request';
                        break;
                    case 'WORKLOAD':
                        url = _pageUrls.Maintenance.WorkItemContainer
                            + '?';
                        break;
                    case 'MULTILEVELCROSSWALK':
                        url = _pageUrls.Maintenance.CrosswalkContainer
                            + '?'
                            + 'GridType=MultiLevel';
                        break;
                    case 'WORKLOADCROSSWALK':
                        url = _pageUrls.Maintenance.CrosswalkContainer
                            + '?';
                        break;
                    case 'QMWORKLOAD':
                        url = _pageUrls.Maintenance.WorkItemGrid_QM
                            + '?';
                        break;
                    case 'HOTLIST':
                        url = _pageUrls.Maintenance.WorkRequestContainer
                            + '?'
                            + 'GridType=Hotlist';
                        break;
                }

                var myData = true;
                if ($('#<%=this.ddlView_Work.ClientID %> option:selected').text() != "My Data") {
                    myData = false;
                }

                var filterSelectedStatuses = '';
                var filterSelectedAssigned = '';
                if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Workload Status' }).length > 0) {
                    filterSelectedStatuses = filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Workload Status' })[0].parameters.toString(',', false);
                }
                if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Workload Assigned To' }).length > 0) {
                    filterSelectedAssigned = filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Workload Assigned To' })[0].parameters.toString(',', false);
                }
                if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Primary Bus Resource' }).length > 0) {
                    filterSelectedAssigned = filterSelectedAssigned + ',' + filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Primary Bus Resource' })[0].parameters.toString(',', false);
                }
                if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Primary Resource' }).length > 0) {
                    filterSelectedAssigned = filterSelectedAssigned + ',' + filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Primary Resource' })[0].parameters.toString(',', false);
                }
                if (filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Affiliated' }).length > 0) {
                    filterSelectedAssigned = filterSelectedAssigned + ',' + filterBox.filters.find({ groups: { ParentModule: parentModuleFilter }, name: 'Affiliated' })[0].parameters.toString(',', false);
                }
                if (filterSelectedStatuses != '') {
                    filterSelectedStatuses = '&SelectedStatuses=' + filterSelectedStatuses;
                }
                if (filterSelectedAssigned != '') {
                    filterSelectedAssigned = '&filterSelectedAssigned=' + filterSelectedAssigned;
                }
                url += '&MyData=' + myData + filterSelectedStatuses + filterSelectedAssigned;

                if (!usePopup) {
                    loadContentFrame(url, forceReload);
                }
                else {
                    //setup popup window

                }
            }

            function openReportsPage(optionName, forceReload) {
                var url = '';
                var usePopup = false;

                switch (optionName.toUpperCase()) {
                    case 'AOR':
                        url = _pageUrls.Reports.ReportParameter + '?ReportName=' + optionName;
                        break;
                    case 'WORKLOAD_SUMMARY':
                        url = _pageUrls.Reports.WorkLoad;
                        break;
                    case 'CR':
                        url = _pageUrls.Reports.CR;
                        forceReload = false;
                        break;
                    case 'TASK':
                        url = _pageUrls.Reports.Task;
                        forceReload = false;
                        break;
                    case 'RELEASE_DSE':
                        url = _pageUrls.Reports.Release_DSE;
                        forceReload = false;
                        break;
                }

                if (!usePopup) {
                    loadContentFrame(url, forceReload);
                }
                else {
                    //setup popup window

                }
            }

            function buttonEmailHotlist_done(result) {
                MessageBox('Email has been sent.');
            }

            function buttonSRHotlist_done(result) {
                MessageBox('Email has been sent.');
            }

            function openMasterDataPage(optionName, forceReload) {
                var url = _pageUrls.MasterData.MaintenanceContainer + '?';
                var mdType = '';
                var usePopup = false;

                switch (optionName.toUpperCase()) {
                    case 'ALLOCATIONCATEGORY':
                        mdType = _pageUrls.MasterData.MDType.AllocationCategory;
                        break;
                    case 'ALLOCATIONGROUP':
                        mdType = _pageUrls.MasterData.MDType.AllocationGroup;
                        break;
                    case 'ALLOCATION':
                        mdType = _pageUrls.MasterData.MDType.Allocation;
                        break;
                    case 'AORESTIMATION':
                        mdType = _pageUrls.MasterData.MDType.AOREstimation;
                        break;
                    case 'AORTYPE':
                        mdType = _pageUrls.MasterData.MDType.AOR_Type;
                        break;
                    case 'CONTRACT':
                        mdType = _pageUrls.MasterData.MDType.Contract;
                        break;
                    case 'CVT':
                        mdType = 'CVT';
                        break;
                    case 'EFFORT':
                        mdType = _pageUrls.MasterData.MDType.Effort;
                        break;
                    case 'EFFORTAREA':
                        mdType = _pageUrls.MasterData.MDType.EffortArea;
                        break;
                    case 'EFFORTSIZE':
                        mdType = _pageUrls.MasterData.MDType.EffortSize;
                        break;
                    case 'IMAGE':
                        mdType = _pageUrls.MasterData.MDType.Image;
                        break;
                    case 'NARRATIVE':
                        mdType = _pageUrls.MasterData.MDType.Narrative;
                        break;
                    case 'PRIORITY':
                        mdType = _pageUrls.MasterData.MDType.Priority;
                        break;
                    case 'PHASE':
                        mdType = _pageUrls.MasterData.MDType.PDD_TDR_Phase;
                        break;
                    case 'PHASEWORKTYPE':
                        mdType = _pageUrls.MasterData.MDType.PhaseWorkType;
                        break;
                    case 'STATUS':
                        mdType = _pageUrls.MasterData.MDType.PDD_TDR_Status;
                        break;
                    case 'VERSION':
                        mdType = _pageUrls.MasterData.MDType.ProductVersion;
                        break;
                    case 'PROGRESS':
                        mdType = _pageUrls.MasterData.MDType.PDD_TDR_Progress;
                        break;
                    case 'SCOPE':
                        mdType = _pageUrls.MasterData.MDType.Scope;
                        break;
                    case 'SYSTEMSUITE':
                        mdType = _pageUrls.MasterData.MDType.SystemSuite;
                        break;
                    case 'SYSTEMPROCESS':
                        mdType = _pageUrls.MasterData.MDType.System;
                        break;
                    case 'REQUESTGROUP':
                        mdType = _pageUrls.MasterData.MDType.RequestGroup;
                        break;
                    case 'RESOURCEGROUP':
                        mdType = _pageUrls.MasterData.MDType.WorkType;
                        break;
                    case 'RQMTTYPE':
                        mdType = _pageUrls.MasterData.MDType.RQMT_Type;
                        break;
                    case 'RQMTDESCRIPTIONTYPE':
                        mdType = _pageUrls.MasterData.MDType.RQMT_Description_Type;
                        break;
                    case 'WORKAREA':
                        mdType = _pageUrls.MasterData.MDType.WorkArea;
                        break;
                    case 'WORKLOADALLOCATION':
                        mdType = _pageUrls.MasterData.MDType.WorkloadAllocation;
                        break;
                    case 'WORKLOADGROUP':
                        mdType = _pageUrls.MasterData.MDType.WorkloadGroup;
                        break;
                    case 'WORKTYPEPHASE':
                        mdType = _pageUrls.MasterData.MDType.WorkTypePhase;
                        break;
                    case 'WORKTYPESTATUS':
                        mdType = _pageUrls.MasterData.MDType.WorkTypeStatus;
                        break;
                    case 'WORKACTIVITY':
                        mdType = _pageUrls.MasterData.MDType.ItemType;
                        break;
                    case 'WORKACTIVITYGROUP':
                        mdType = _pageUrls.MasterData.MDType.WorkActivityGroup;
                        break;
                }

                url += '&MDType=' + mdType;

                loadContentFrame(url, forceReload);
            }

            function openAdministrationPage(optionName, forceReload) {
                var url = '';
                var usePopup = false;

                switch (optionName.toUpperCase()) {
                    case 'RESOURCE_MGMT':
                        url = _pageUrls.Administration.ResourceMGMTContainer;
                        url = 'Admin/User_Grid.aspx';
                        break;
                    case 'ORGANIZATION':
                        url = _pageUrls.Administration.OrganizationContainer;
                        url = 'Admin/Organization_Grid.aspx';
                        break;
                    case 'ROLES':
                        url = _pageUrls.Administration.OrganizationContainer;
                        url = 'Admin/ManageRoles.aspx';
                        break;
                    case 'HOTLIST':
                        url = _pageUrls.Administration.OrganizationContainer;
                        url = 'Admin/ConfigureHotlist.aspx';
                        break;
                    case 'SRCONFIGURATION':
                        url = _pageUrls.Administration.OrganizationContainer;
                        url = 'Admin/ConfigureSR.aspx';
                        break;
                }

                if (!usePopup) {
                    loadContentFrame(url, forceReload);
                }
                else {
                    //setup popup window

                }
            }


            function save_done(result) {
                try {
                    if (result.d.indexOf('Error') != -1) {
                        MessageBox(result.d);
                    }
                } catch (e) {

                }
            }
            function on_error(result) {
                MessageBox('save error:  \n' + result);
            }

            function loadViewOptions(module, option) {
                var tag = '';
                //load appropriate "View" options
                $('tr', $('#tableModuleOptions')).hide();
                $('tr', $('#tableModuleOptions')).each(function () {
                    tag = $(this).attr('tag');
                    if (tag.indexOf(option) > -1) {
                        $(this).show();
                        //if (tag == 'Inventory') {
                        //  showSubOptions(tag, selectedParentOption).text());
                        //}
                    }
                });

                if (module === 'AoR') {
                    $('#divFilterManagerHeader').hide();
                    $('#tblSpacer').hide();
                    $('#imgShowFilters').hide();
                    $('#imgClearFilters').hide();
                    $('#imgAORHome').show();
                    $('#imgMetrics').hide();
                    $('#divFilterManagerBody').hide();
                }
                else if (module === 'RQMT') {
                    $('#divModuleQuickFilters').show();
                    $('#divFilterManagerHeader').show();
                    $('#tblSpacer').show();
                    $('#imgShowFilters').show();
                    $('#imgClearFilters').show();
                    $('#divFilterManagerBody').show();
                }
                else if (module == 'SR') {
                    $('#divFilterManagerHeader').hide();
                    $('#tblSpacer').hide();
                    $('#imgShowFilters').show();
                    $('#imgClearFilters').show();
                    $('#imgMetrics').hide();
                    $('#divFilterManagerBody').show();
                }
                else if (module === 'Reports' && (option === 'CR' || option == 'Task' || option == 'Release_DSE')) {
                    $('#divFilterManagerHeader').hide();
                    $('#tblSpacer').hide();
                    $('#imgShowFilters').show();
                    $('#imgClearFilters').show();
                    $('#imgAORHome').hide();
                    $('#imgMetrics').hide();
                    $('#divFilterManagerBody').show();
                }
                else if (module === 'Administration'
                    || module === 'MasterData'
                    || (module === 'Reports' && option === 'Workload_Summary')) {
                    $('#divFilterManagerHeader').hide();
                    $('#divModuleQuickFilters').hide();
                    $('#tblSpacer').hide();
                    $('#imgShowFilters').hide();
                    $('#imgClearFilters').hide();
                    $('#imgAORHome').hide();
                    $('#imgMetrics').hide();
                    $('#divFilterManagerBody').hide();
                }
                else {
                    $('#divModuleQuickFilters').show();
                    $('#divFilterManagerHeader').show();
                    $('#tblSpacer').show();
                    $('#imgShowFilters').show();
                    $('#imgClearFilters').show();
                    $('#imgAORHome').hide();
                    $('#imgMetrics').show();
                    $('#divFilterManagerBody').show();
                }
                pageResize();

            }

            function moduleOption_click(module, option) {
                $('#tableModuleContainer tr').removeClass('moduleOption_Selected');
                $('#tableModuleContainer tr[optionValue=' + option + ']').addClass('moduleOption_Selected');

                loadViewOptions(module, option);
                loadSearchFields();
                reApplyFilters();
            }
            function showSubOptions(option, subOption) {
                $('tr[parent="' + option + '"]', $('#tableModuleOptions')).hide();
                $('tr[parent="' + option + '"]', $('#tableModuleOptions')).each(function () {
                    tag = $(this).attr('tag');
                    if (tag.indexOf(subOption) > -1) {
                        $(this).show();
                    }
                    else {
                        $(this).hide();
                    }
                });
            }

            function loadSearchOptions(selectedModuleName) {

            }

            function setupModuleFilterSection(selectedModuleName) {
                $('#divFilterManagerHeader').show();
                $('#imgShowFilters').show();
                $('#imgClearFilters').show();
                $('#imgAORHome').hide();
                $('#tableFilter').show();
                $('#divPageManagerFooter').show();
                $('tr', $('#tableModuleOptions')).hide();
                var option = '';
                if ($('.moduleOption_Selected').attr('optionValue')) {
                    option = $('.moduleOption_Selected').attr('optionValue');
                }

                switch (selectedModuleName.toUpperCase()) {
                    case 'WORK':
                    case 'DEVELOPERREVIEW':
                    case 'DAILYREVIEW':
                    case 'AOR':
                    case 'RQMT':
                    case 'SR':
                        $('#divFilterManager').show();
                        if (selectedModuleName.toUpperCase() == 'AOR') {
                            $('#imgAORHome').show();
                            $('#imgAORHome').trigger('click');
                        }
                        $('#imgMetrics').show();
                        loadViewOptions(selectedModuleName, option);
                        loadSearchFields();
                        reApplyFilters();
                        break;
                    case 'MEETINGMODULE':
                    case 'REPORTS':
                    case 'MASTERDATA':
                    case 'ADMINISTRATION':
                        $('#divFilterManager').show();
                        $('#divFilterManagerHeader').hide();
                        $('#divModuleQuickFilters').hide();
                        $('#tblSpacer').hide();
                        $('#imgShowFilters').hide();
                        $('#imgClearFilters').hide();
                        $('#imgAORHome').hide();
                        $('#imgMetrics').hide();
                        $('#divFilterManagerBody').hide();
                        break;
                }

                if (selectedModuleName.toUpperCase() != 'AOR' && selectedModuleName.toUpperCase() != 'RQMT' && selectedModuleName.toUpperCase() != 'SR') showStartPage('Dashboard');
            }

            function setupModuleSections(selectedModuleName) {

                setupModuleFilterSection(selectedModuleName);

                loadSearchFields();
                loadCustomFilters();
            }

            function setupModuleOptions(selectedModuleName) {
                $('#tableModuleContainer').empty();

                var listbox = null;
                switch (selectedModuleName.toUpperCase()) {
                    case 'WORK':
                        listbox = $('#<%=this.lstWork.ClientID %>');
                        break;
                    case 'MEETINGMODULE':
                        listbox = $('#<%=this.lstMeetingModule.ClientID %>');
                        break;
                    case 'DEVELOPERREVIEW':
                        listbox = $('#<%=this.lstDeveloperReview.ClientID %>');
                        break;
                    case 'DAILYREVIEW':
                        listbox = $('#<%=this.lstDailyReview.ClientID %>');
                        break;
                    case 'AOR':
                        listbox = $('#<%=this.lstAoR.ClientID %>');
                        break;
                    case 'RQMT':
                        listbox = $('#<%=this.lstRQMT.ClientID %>');
                        break;
                    case 'REPORTS':
                        listbox = $('#<%=this.lstReports.ClientID %>');
                        break;
                    case 'SR':
                        listbox = $('#<%=this.lstSR.ClientID %>');
                        break;
                    case 'MASTERDATA':
                        listbox = $('#<%=this.lstMasterData.ClientID %>');
                        break;
                    case 'ADMINISTRATION':
                        listbox = $('#<%=this.lstAdministration.ClientID %>');
                        break;
                }

                var itemCount = listbox.children('option').length;
                var tableRow = null, textCell = null;
                var text = '', value = '';
                var selected = false;
                for (var i = 0; i < itemCount; i++) {
                    var itemText = listbox.children('option')[i].innerText;

                    tableRow = tableModuleContainer.insertRow(tableModuleContainer.rows.length);
                    textCell = tableRow.insertCell(0);
                    text = itemText;
                    value = listbox.children('option')[i].value;
                    $(textCell).addClass('moduleOption');
                    $(textCell).text(text);
                    $(tableRow).attr('optionName', text);
                    $(tableRow).attr('optionValue', value);
                    $(tableRow).click(function () {
                        moduleOption_click(selectedModuleName, $(this).attr('optionValue'));
                    });

                    if (i == 0 || selected == false) {
                        $(tableRow).addClass('moduleOption_Selected');
                        selected = true;
                    }
                }

                $('#divModuleOptions').show();
            }
            function setupSelectedModule(selectedModuleName) {
                var moduleRow = $('#tableAvailableModules tr[moduleName=' + selectedModuleName + ']');
                var moduleText = $(moduleRow).children('.moduleText').text();
                var Imagesource = $(moduleRow).find('img').attr('src');

                $('#trSelectedModule').attr('moduleId', moduleRow.attr('moduleId'));
                $('#trSelectedModule').attr('moduleName', moduleRow.attr('moduleName'));

                $('#imgSelectedModule').attr('src', Imagesource);
                $('#imgSelectedModule').attr('alt', moduleText);

                $('#tdSelectedModuleText').text(moduleText);

                setupModuleOptions(selectedModuleName);
                setupModuleSections(selectedModuleName);
            }

            function setModuleExpandCollapseImage(direction, selectedModule) {
                $('#trSelectedModule').attr('direction', direction.toUpperCase());

                if (direction.toUpperCase() == 'EXPAND') {
                    $('#imgModuleMenu').attr('src', 'Images/fatArrowDown.png');
                    $('#imgModuleMenu').attr('alt', 'Expand Module Selections Menu');
                    $('#imgModuleMenu').attr('title', 'Expand Module Selections Menu');
                }
                else {
                    $('#imgModuleMenu').attr('src', 'Images/fatArrowUp.png');
                    $('#imgModuleMenu').attr('alt', 'Collapse Module Selections Menu');
                    $('#imgModuleMenu').attr('title', 'Collapse Module Selections Menu');
                }
            }

            function menuSelection_click(direction, selectedModuleName) {
                if (direction.toUpperCase() == 'EXPAND') {
                    $('#tableAvailableModules tr').show();
                    $('#divFilterManager').hide();
                    if (selectedModuleName == null || selectedModuleName == '') {
                        $('#trSelectedModule').hide();
                    }
                    else {
                        $('#trSelectedModule').show();
                        $('#tableAvailableModules tr[moduleName=' + selectedModuleName + ']').hide();
                    }
                    $('#tableAvailableModules').show();

                    $('#divModuleOptions').hide();

                    setModuleExpandCollapseImage('COLLAPSE', selectedModuleName);
                }
                else {
                    $('#trSelectedModule').show();
                    $('#tableAvailableModules').hide();

                    setModuleExpandCollapseImage('EXPAND', selectedModuleName);

                    //only setup selected module and its options if it is not already selected
                    if ($('#trSelectedModule').attr('moduleName') == selectedModuleName) {
                        $('#divModuleOptions').show();
                        setupModuleFilterSection(selectedModuleName)
                        setupSelectedModule(selectedModuleName);
                    }
                    else {
                        setupSelectedModule(selectedModuleName);
                    }
                }
            }

        </script>

        <script id="jsFilters" type="text/javascript">
            var filterBox = new filterContainer('divAppliedFilters');

            function saveFilters(blnMenuItem, strMenuItem, blnSave) {
                try {
                    if (!blnSave) {
                        var frame = $('#<%=this.pageContentFrame.ClientID %>')[0];

                        if (frame && frame.contentWindow
                            && frame.contentWindow.showUpdateMetrics) {
                            frame.contentWindow.showUpdateMetrics();
                        }
                        else {
                            //$('#imgMetrics').trigger('click');
                            //setTimeout(function () { frame.contentWindow.showUpdateMetrics(); }, 3000);
                        }

                        return;
                    }

                    var parentModule = $('#trSelectedModule').attr('moduleName');

                    //setup applying filters here
                    //Custom
                    var parentModuleFilter = parentModule;
                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModuleFilter = 'Work';

                    var filters = filterBox.toJSON({ groups: { ParentModule: parentModuleFilter } }); //, "Module"
                    var myData = false;
                    myData = ($('#<%=this.ddlView_Work.ClientID %> option:selected').text() == 'My Data') ? true : false;

                    PageMethods.SetFilterSession(false, true, parentModule, filters, myData, setFilterSession_Done, on_error);
                } catch (e) {

                }
            }

            function saveFilters_done(results) {
                try {
                    var blnMenuItem = results.blnMenuItem;
                    var strMenuItem = results.strMenuItem;

                    switch (strMenuItem) {
                        /***********************
                        Other
                        ************************/
                        case "Load_Summary":
                        case "Load_Filters":
                        case "Load_HomePage":
                            GoToPage(strMenuItem);
                            break;

                        default:
                            MessageBox('Under Developement');
                            break;
                    }

                } catch (e) {

                }
            }

            function reApplyFilters() {
                var parentModule = $('#trSelectedModule').attr('moduleName');

                if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                filterBox.toTable({ groups: { ParentModule: parentModule } }, "Module");
            }

            function imgShowSearch_click() {
                var searchField = $('#<%=this.ddlSearch.ClientID %> option:selected').text();
                if (searchField != '- Select -') {
                    var url = 'FilterSearch.aspx?field=' + searchField + '&random=' + new Date().getTime();
                    var openPopup = popupManager.AddPopupWindow('Search', 'Search'
                        , url, 90, 475
                        , 'PopupWindow', this);

                    if (openPopup) openPopup.Open();
                }
                else {
                    MessageBox('Please select a search option.');
                }
            }

            function setSearchValue(search) {
                if (!search || search == undefined || $.trim(search) == '') {
                    MessageBox('Please enter a search value.');
                    return;
                }

                addSearchToFilter($('#ddlSearch option:selected').text(), search, search);
            }

            function loadSearchFields() {
                $('#<%=this.ddlSearch.ClientID %>').empty();

                var parentModule = $('#trSelectedModule').attr('moduleName');
                var searchType = 'All';
                var option = {};

                if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                $('#<%=this.lstSearch.ClientID %> option').each(function () {
                    searchType = $(this).attr('type');
                    if (searchType == 'All' || searchType == parentModule) {
                        $('#<%=this.ddlSearch.ClientID %>').append($('<option></option>').val($(this).val()).html($(this).text()));
                    }
                });
            }

            function addSearchToFilter(values, clearAll) {
                var selector = 'Contains';
                var searchField = $('#<%=this.ddlSearch.ClientID %> option:selected').text().toUpperCase() + ' ' + selector;

                var parentModule = $('#trSelectedModule').attr('moduleName');

                if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                var filterValues = values.split(",");
                if (clearAll) {
                    var searchFilter = filterBox.filters.find({ name: searchField, groups: { ParentModule: parentModule, Module: "Custom" } })[0];
                    if (searchFilter) {
                        searchFilter.remove();
                    }
                }

                for (var i = 0; i <= filterValues.length - 1; i++) {
                    var searchValue = filterValues[i].trim();

                    if (searchValue != '') filterBox.filters.add({ name: searchField, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(searchValue, searchValue);
                }

                filterBox.toTable({ groups: { ParentModule: parentModule } }, "Module");
                saveFilters(false, "Load_HomePage");
            }

            function gatherFilters() {
                try {
                    var parentModule = $('#trSelectedModule').attr('moduleName');
                    var strFilters = '', userMgmtFilter = '';
                    var filterTypeName = '', filterAffiliation = '', filterAffiliationName = '';
                    var filterRole = '', filterRoleName = '', filterParameter = '';
                    var filterType = '';
                    var filterValueIDs = '';
                    var filterValueNames = '';
                    var splitFilterValueIDs = [];
                    var splitFilterValueNames = [];

                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                    if (DefaultSystemFilters) {
                        var curDefFilters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Default" } });
                        if (curDefFilters) {
                            curDefFilters.clear();
                            filterBox.toTable('', 'Module');
                        }

                        strFilters = DefaultSystemFilters.split('`');
                        userMgmtFilter = '';

                        for (var i = 0; i <= strFilters.length - 1; i++) {
                            filterTypeName = strFilters[i].split('|')[0];
                            filterAffiliation = strFilters[i].split('|')[1];
                            filterAffiliationName = strFilters[i].split('|')[2];
                            filterRole = strFilters[i].split('|')[3];
                            filterRoleName = strFilters[i].split('|')[4];
                            filterParameter = strFilters[i].split('|')[5];
                            filterType = strFilters[i].split('|')[6];
                            filterValueIDs = strFilters[i].split('|')[7];
                            filterValueNames = strFilters[i].split('|')[8];
                            splitFilterValueIDs = filterValueIDs.split(',');
                            splitFilterValueNames = filterValueNames.split(',');

                            filterType = filterType.split(",")[0];

                            for (var y = 0; y <= splitFilterValueIDs.length - 1; y++) {
                                if (filterAffiliationName == 'Default') {
                                    filterAffiliationName = splitFilterValueNames[y];
                                    filterAffiliation = splitFilterValueIDs[y];
                                }

                                if (splitFilterValueIDs[y] != '') {
                                    filterBox.filters.add({ name: filterParameter, groups: { ParentModule: parentModule, Module: "Default" } }).parameters.add(splitFilterValueIDs[y], splitFilterValueNames[y]);
                                    userMgmtFilter = filterBox.filters.add({ name: filterParameter, groups: { ParentModule: parentModule, Module: "Default" } });
                                    userMgmtFilter.removable = true;
                                }
                            }
                        }
                        filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                    }

                    if (WTSFilters) {
                        var curFilters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "User Management" } });
                        if (curFilters) {
                            curFilters.clear();
                            filterBox.toTable('', 'Module');
                        }

                        strFilters = WTSFilters.split('`');
                        userMgmtFilter = '';

                        for (var i = 0; i <= strFilters.length - 1; i++) {
                            filterTypeName = strFilters[i].split('|')[0];
                            filterAffiliation = strFilters[i].split('|')[1];
                            filterAffiliationName = strFilters[i].split('|')[2];
                            filterRole = strFilters[i].split('|')[3];
                            filterRoleName = strFilters[i].split('|')[4];
                            filterParameter = strFilters[i].split('|')[5];
                            filterType = strFilters[i].split('|')[6];
                            filterValueIDs = strFilters[i].split('|')[7];
                            filterValueNames = strFilters[i].split('|')[8];
                            splitFilterValueIDs = filterValueIDs.split(',');
                            splitFilterValueNames = filterValueNames.split(',');

                            filterType = filterType.split(",")[0];

                            for (var y = 0; y <= splitFilterValueIDs.length - 1; y++) {
                                if (filterAffiliationName == 'CUSTOM') {
                                    filterAffiliationName = splitFilterValueNames[y];
                                    filterAffiliation = splitFilterValueIDs[y];
                                }

                                if (splitFilterValueIDs[y] != '') {
                                    filterBox.filters.add({ name: filterParameter, groups: { ParentModule: parentModule, Module: "User Management" } }).parameters.add(splitFilterValueIDs[y], splitFilterValueNames[y]);
                                    userMgmtFilter = filterBox.filters.add({ name: filterParameter, groups: { ParentModule: parentModule, Module: "User Management" } });
                                    userMgmtFilter.removable = false;
                                }
                            }
                        }
                        filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                    }
                }
                catch (e) {
                    MessageBox('Error: gatherFilters - ' + e.number + ' : ' + e.message);
                }
            }

			function imgEMailHotlist_click()
			{
                WorkloadWebmethods.EmailHotlist(buttonEmailHotlist_done, function (result) { });
                MessageBox('EMail sent.');
            }

            function imgSRHotlist_click() {
                WorkloadWebmethods.SRHotlist(buttonSRHotlist_done, function (result) { });
                MessageBox('EMail sent.');
            }

            function imgClearFilters_click() {
                try {
                    var parentModule = $('#trSelectedModule').attr('moduleName');

                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                    if (confirm('You are about to clear the Applied Custom Filters! Are you sure you want to continue?')) {
                        var filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
                        if (filters) {
                            filters.clear();
                            filterBox.toTable('', 'Module');
                            if (parentModule !== "Reports") {
                                saveFilters(false, "Load_HomePage");
                            }
                        }

                        filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Default" } });
                        if (filters) {
                            filters.clear();
                            filterBox.toTable('', 'Module');
                            if (parentModule !== "Reports") {
                                saveFilters(false, "Load_HomePage");
                            }
                        }

                        PageMethods.ClearFilterSession(parentModule, null, null);
                    }
                }
                catch (e) {
                    MessageBox('Error: btnClearFilters_Onclick - ' + e.number + ' : ' + e.message);
                }
            }

            function saveCustomFilters() {
                try {
                    var collectionName = ddlSavedFilters.SelectedText;
                    if (collectionName == '' || collectionName == '- Add New -') {
                        MessageBox("Please enter or select a name to save the custom filter.");
                        return false;
                    }

                    var parentModule = $('#trSelectedModule').attr('moduleName');
                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                    var filterCount = filterBox.filters.find({ groups: { ParentModule: parentModule } });
                    if (filterCount && filterCount.length > 0) {
                        if (confirm('Save custom filters with the name of ' + collectionName + '?')) {
                            var filters = filterBox.toJSON({ groups: { ParentModule: parentModule } });
                            if (filters) PageMethods.SaveCustomFilter(collectionName, false, parentModule, filters, saveCustomFilters_done, on_error);
                        }
                    }
                    else {
                        MessageBox('Please select at least one filter to save.');
                        return false;
                    }
                }
                catch (e) { }
            }
            function saveCustomFilters_done(result) {
                var saved = false;
                var collectionName = '';
                var customFilter = '';
                var errorMsg = '';

                try {
                    var obj = jQuery.parseJSON(result);

                    if (obj) {
                        if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                            saved = true;
                        }
                        if (obj.collectionName) {
                            collectionName = obj.collectionName;
                        }
                        if (obj.customFilter) {
                            customFilter = obj.customFilter;
                        }
                        if (obj.error) {
                            errorMsg = obj.error;
                        }
                    }

                    if (saved) {
                        dlFilters = ddlSavedFilters.DropDownList;

                        var option = '';
                        for (var i = 0; i <= dlFilters.options.length - 1; i++) {
                            if (dlFilters.options[i].text == collectionName) {
                                option = dlFilters.options[i];
                                dlFilters.selectedIndex = i;
                                break;
                            }
                        }
                        if (!option) {
                            option = document.createElement('option');
                            option.text = collectionName;
                            dlFilters.options.add(option);
                            dlFilters.selectedIndex = dlFilters.options.length - 1;
                        }
                        option.value = customFilter;
                        currentCustomFilter = '';

                        MessageBox("Custom filter saved successfully...");

                        setCustomFilter();
                    }
                    else {
                        MessageBox("Error: Filters were not saved...");
                    }
                }
                catch (e) { }
            }

            function deleteCustomFilters() {
                try {
                    var collectionName = ddlSavedFilters.SelectedText;
                    if (collectionName == '' || collectionName == '- Add New -') {
                        MessageBox("Please select a filter to delete.");
                        return false;
                    }

                    var parentModule = $('#trSelectedModule').attr('moduleName');
                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                    if (confirm('You are about to delete the ' + collectionName + ' custom filter, are you sure you wish to continue?')) {
                        var filters = filterBox.toJSON({ groups: { ParentModule: parentModule, Module: "Custom" } });
                        if (filters) PageMethods.SaveCustomFilter(collectionName, true, parentModule, filters, deleteCustomFilters_done, on_error);
                    }
                }
                catch (e) { }
            }
            function deleteCustomFilters_done(result) {
                var saved = false;
                var collectionName = '';
                var customFilter = '';
                var errorMsg = '';

                try {
                    var obj = jQuery.parseJSON(result);

                    if (obj) {
                        if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                            saved = true;
                        }
                        if (obj.collectionName) {
                            collectionName = obj.collectionName;
                        }
                        if (obj.customFilter) {
                            customFilter = obj.customFilter;
                        }
                        if (obj.error) {
                            errorMsg = obj.error;
                        }
                    }

                    if (saved) {
                        // 13888 - Removed this call, can't get handle on lstCustomFilters and so errors (Chrome)
                        //removeCustomFiltersFromDDL(collectionName);

                        saveFilters(false, "Load_HomePage");

                        // 13888 11-28-2016 - Added this, removed MessageBox below.
                        document.location.href = 'Loading.aspx?Page=' + document.location.href;

                        //MessageBox("Custom filter deleted...");
                    }
                    else {
                        MessageBox("There was an error and the filter set was not deleted");
                    }
                }
                catch (e) {
                    alert("Error in deleteCustomFilters_done. " + e.message);
                }
            }

            function loadCustomFilters() {
                try {
                    var lstCustomFilters = document.getElementById('lstCustomFilters');
                    ddlSavedFilters.DropDownList.options.length = 0;

                    var bOpt = document.createElement('option');
                    bOpt.value = '';
                    bOpt.text = '';
                    ddlSavedFilters.DropDownList.options.add(bOpt);

                    var sOpt = document.createElement('option');
                    sOpt.value = '0';
                    sOpt.text = '- Add New -';
                    ddlSavedFilters.DropDownList.options.add(sOpt);

                    if (lstCustomFilters && ddlSavedFilters.DropDownList) {
                        for (var i = 0; i <= lstCustomFilters.options.length - 1; i++) {
                            var opt = document.createElement('option');
                            opt.value = lstCustomFilters.options[i].value;
                            opt.text = lstCustomFilters.options[i].text;
                            ddlSavedFilters.DropDownList.options.add(opt);
                        }
                    }
                }
                catch (e) {
                }
            }

            var currentCustomFilter = '';

            function setCustomFilter() {
                if (currentCustomFilter != ddlSavedFilters.SelectedText && ddlSavedFilters.SelectedValue !== "0") {
                    var parentModule = $('#trSelectedModule').attr('moduleName');

                    if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR')
                        parentModule = 'Work';

                    var curFilters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
                    if (curFilters) {
                        curFilters.clear();
                        filterBox.toTable('', 'Module');
                    }
                    if (ddlSavedFilters.DropDownList.value != -1) {
                        var filters = ddlSavedFilters.DropDownList.value.split("`");

                        for (var i = 0; i <= filters.length - 1; i++) {
                            var filterName = filters[i].split('|')[0];
                            var filterField = filters[i].split('|')[1];
                            if (filterField) {
                                var parameterID = filters[i].split('|')[2].split(',');
                                var parameterName = filters[i].split('|')[3].split(',');

                                for (var y = 0; y <= parameterID.length - 1; y++) {
                                    if (parameterID[y] != '' && parameterName[y] != '') {
                                        filterBox.filters.add({ name: filterName, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(parameterID[y], parameterName[y]);
                                    }
                                }
                            }
                        }
                    }
                    filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                }
                currentCustomFilter = ddlSavedFilters.SelectedText;
                saveFilters(false, "Load_HomePage");
            }
        </script>

        <script id="jsEvents" type="text/javascript">

            function page_keyup() {
                if (window.event) {
                    var key = window.event.keyCode;
                    if (window.event.ctrlKey && window.event.altKey && key == 123) {
                        // CTRL+ALT+F12
                        var changeUserWindow = popupManager.AddPopupWindow('ChangeUser_Auth', 'Change User', 'ChangeUser_Auth.aspx?random=' + new Date().getTime(), 120, 270, 'PopupWindow');
                        if (changeUserWindow) {
                            changeUserWindow.Open();
                        }
                    }
                }
            }

            function showStartPage(tabToLoad, module, sysNotification) {
                //default is news but when all read or overridden in options it should go to metrics page
                if (tabToLoad == null || tabToLoad == undefined || tabToLoad == '') {
                    tabToLoad = 'News';
                }
                var url = _pageUrls.Home + '?TabToLoad=' + tabToLoad + '&Module=' + module + '&Random=' + new Date().getTime();
                var forceReload = false;
                if (sysNotification == 1) { forceReload = true;}
                loadContentFrame(url, forceReload);
            }

            function loadContentFrame(url, forceReload) {

                if (!forceReload) {
                    //check if the requested page is already open in frame
                    var currentPage = $('#<%=this.pageContentFrame.ClientID %>').attr('src');
                    var newPage = url;
                    idxSearch = url.indexOf('?');
                    if (idxSearch > 0) {
                        newPage = newPage.substr(0, idxSearch);
                    }

                    //handle shared report page
                    if (newPage == 'Report_ParameterSelection.aspx' && url != currentPage.replace('Loading.aspx?Page=', '')) forceReload = true;

                    if (currentPage.toUpperCase().indexOf(newPage.toUpperCase()) > -1
                        && !forceReload) {
                        return;
                    }
                }
                if ($('#<%=this.pageContentFrame.ClientID %>').attr('src').indexOf(url + '&') !== -1
                    && document.getElementById("pageContentFrame").contentWindow.frameGrid != undefined
                    && document.getElementById("pageContentFrame").src.indexOf(url, document.getElementById("pageContentFrame").src.length - url.length) !== -1) {
                    try {
                        document.getElementById("pageContentFrame").contentWindow.frameGrid.contentWindow.refreshPage();
                    }
                    catch (e) {
                        document.getElementById("pageContentFrame").contentWindow.frameGrid.refreshPage();
                    }
                }
                else {
                    $('#<%=this.pageContentFrame.ClientID %>').attr('src', 'Loading.aspx?Page=' + url);
                }
            }

            function imgShowFilters_click() {
                var module = $('#trSelectedModule').attr('moduleName');
                var option = $('.moduleOption_Selected').attr('optionValue');

                if (module == 'DeveloperReview' || module == 'DailyReview' || module == 'AoR') module = 'Work';

                var myData = true;
                if ($('#<%=this.ddlView_Work.ClientID %> option:selected').text() != "My Data") {
                    myData = false;
                }

                if (option == "Workload_Summary") {
                    myData = false;
                }

                var reportTypeID = -1;
                if (option == "CR") {
                    reportTypeID = <%=(int)WTS.Reports.ReportTypeEnum.CRReport%>;
                }
                else if (option == "Task") {
                    reportTypeID = <%=(int)WTS.Reports.ReportTypeEnum.TaskReport%>;
                }
                else if (option == "Release_DSE") {
                    reportTypeID = <%=(int)WTS.Reports.ReportTypeEnum.ReleaseDSEReport%>;
                }

                ShowDimmer(false);

                var strURL;
                var h = 450, w = 900;
                var window = 'FilterPage';
                var title = 'Filter and Criteria';

                if (module == 'RQMT') {
                    strURL = _pageUrls.Maintenance.RQMTParameterPage + '?TabToLoad=Filters';
                    h = 600, w = 900;
                    window = 'RQMTParameter';
                    title = 'Sys RQMT Sets Parameters';
                } else {
                    strURL = 'Loading.aspx?Page=FilterPage.aspx?random=' + new Date().getTime()
                        + '&parentModule=' + module
                        + '&MyData=' + myData;
                }

                if (reportTypeID > 0) {
                    strURL += '&Source=Report' + '&Options=' + reportTypeID;
                }

                var openPopup = popupManager.AddPopupWindow(window, title, strURL, h, w, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }

            function imgAORHome_click() {
                loadContentFrame(_pageUrls.AORHomeTabs, false);
            }

            function imgNews_click() {
                showStartPage('Dashboard', '');
                //showStartPage('News', '');
            }

            function imgMetrics_click() {
                var module = '';
                try {
                    module = $('#trSelectedModule').attr('moduleName');

                    showStartPage('Metrics', module);
                } catch (e) {

                }
            }

            function imgViewHelp_click() {
                MessageBox('<%=this.ViewInfo %>');
            }

            function buttonGetData_click() {
                if ('<%=this.IsAuthorized %>'.toUpperCase() == 'FALSE') {
                    MessageBox('You do not have any active roles and/or filters for this websystem.  Please contact your System Administrator/Program Group RQMT Manager for assignment of Roles/Filters to allow maintenance.');
                    return;
                }
                blnReloadFilters = false;

                // when we click getdata, we reset all open nodes
                _expandedGridNodes = []; 
                _moduleCachedData = [];

                var option = $('.moduleOption_Selected').attr('optionValue');
                var skipFilters = ['AOR', 'AORMeeting', 'CR', 'ScheduledDeliverables', 'ReleaseAssessment', 'RQMTDescription', 'SR', 'Task', 'Release_DSE'];

                if ($.inArray(option, skipFilters) != -1) {
                    getDataFiltered();
                }
                else {
                    setFilterSession(true, false);
                }

                //applyFilters();
            }

            function getDataFiltered() {
                var module = $('#trSelectedModule').attr('moduleName');
                var option = $('.moduleOption_Selected').attr('optionValue');

                if (!module
                    || module === ''
                    || !option
                    || option === '') {
                    MessageBox('Please select an option to load.');
                    return;
                }
                switch (module.toUpperCase()) {
                    case 'WORK':
                    case 'MEETINGMODULE':
                    case 'DEVELOPERREVIEW':
                    case 'DAILYREVIEW':
                    case 'AOR':
                    case 'SR':
                    case 'RQMT':
                        openWorkPage(option, true);
                        break;
                    case 'REPORTS':
                        openReportsPage(option, true);
                        break;
                    case 'MASTERDATA':
                        openMasterDataPage(option, true);
                        break;
                    case 'ADMINISTRATION':
                        openAdministrationPage(option, true);
                        break;
                }
            }

            var DefaultSystemFilters = '<%=this.DefaultSystemFilters%>';
            var WTSFilters = '<%=this.WTSFilters%>';

            var filtersLoaded = false;

            function applyFilters(loadMetrics, blnSkipLoad, blnRefTaskRep) {
                blnSkipLoad = typeof blnSkipLoad !== 'undefined' ? blnSkipLoad : false;
                try {
                    filtersLoaded = false;

                    $('#divFilterSaver').show();
                    $('#divPageDimmer').show();

                    var filterURL = 'Apply_Filters.aspx';
                    var useFilter = '1';
                    if (loadMetrics) {
                        useFilter = '0';
                    }

                    postToURL('frameApplyFilter', filterURL, { strFilter: '', strUseFilter: useFilter, metrics: loadMetrics, blnSkipLoad: blnSkipLoad, skipClear: 'false' });
                }
                catch (e) {
                    MessageBox('Error: applyFilters - ' + e.number + ' : ' + e.message);
                    $('#divFilterSaver').hide();
                    $('#divPageDimmer').hide();
                }
            }
            function applyFilters_Done(metrics, blnSkipLoad) {
                try {
                    filtersLoaded = true;

                    $('#divFilterSaver').hide();
                    $('#divPageDimmer').hide();

                    if (metrics || blnSkipLoad) {
                        return;
                    } else {
                        getDataFiltered();
                    }
                }
                catch (e) {
                    $('#divFilterSaver').hide();
                    $('#divPageDimmer').hide();
                }
            }

            function clearViewDataSession() {
                try {
                    PageMethods.ClearViewDataSession();
                    if (typeof $('#pageContentFrame').contents($('#frameGrid')).find('body form #divFrameContainer #frameGrid')[0] != 'undefined'
                        && $.isFunction($('#pageContentFrame').contents($('#frameGrid')).find('body form #divFrameContainer #frameGrid')[0].contentWindow.clearSelectedAssigned)) {
                        $('#pageContentFrame').contents($('#frameGrid')).find('body form #divFrameContainer #frameGrid')[0].contentWindow.clearSelectedAssigned(($('#<%=this.ddlView_Work.ClientID %> option:selected').text() == 'My Data') ? _userId : '');
                    }
                } catch (e) {
                    MessageBox('Error: Could not clear View Data Session - ' + e.message);
                }
            }

            function clearFilterSession(module) {
                $('#divFilterSaver').show();
                $('#divPageDimmer').show();

                try {
                    PageMethods.ClearFilterSession(module, clearFilterSession_done, on_error);
                } catch (e) {
                    $('#divFilterSaver').hide();
                    $('#divPageDimmer').hide();
                }
            }
            function clearFilterSession_done(result) {
                $('#divFilterSaver').hide();
                $('#divPageDimmer').hide();

                var obj = jQuery.parseJSON(result);

                var module = '';
                if (obj) {
                    if (obj.module) {
                        module = obj.module;
                    }
                }

                showStartPage('Metrics', module);
            }

            function setFilterSession(getData, loadStartPage) {
                blnSkipLoad = typeof blnSkipLoad !== 'undefined' ? blnSkipLoad : false;
                loadStartPage = typeof loadStartPage !== 'undefined' ? loadStartPage : true;

                try {
                    $('#divFilterSaver').show();
                    $('#divPageDimmer').show();

                    var module = $('#trSelectedModule').attr('moduleName');
                    var option = $('.moduleOption_Selected').attr('optionValue');

                    var moduleFilter = module;
                    if (module == 'DeveloperReview' || module == 'DailyReview' || module == 'AoR') moduleFilter = 'Work';

                    var filters = null;

                    if (filterBox && filterBox.filters && filterBox.filters.length > 0) {
                        filters = filterBox.toJSON({ groups: { ParentModule: moduleFilter } });
                    }

                    myData = ($('#<%=this.ddlView_Work.ClientID %> option:selected').text() == 'My Data') ? true : false;

                    PageMethods.SetFilterSession(getData, loadStartPage, module, filters, myData, setFilterSession_Done, on_error);
                } catch (e) {
                    MessageBox('Error: setFilterSession - ' + e.number + ' : ' + e.message);
                    $('#divFilterSaver').hide();
                    $('#divPageDimmer').hide();
                }
            }
            function setFilterSession_Done(result) {
                $('#divFilterSaver').hide();
                $('#divPageDimmer').hide();

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.getData && obj.getData.toUpperCase() == "TRUE") {
                        getDataFiltered();
                    }
                    else {
                        var module = '';
                        if (obj.module) {
                            module = obj.module;
                        }
                        if (obj.loadStartPage && obj.loadStartPage.toUpperCase() == 'TRUE') {
                            showStartPage('Metrics', module);
                        }
                    }
                }
            }

            function reportExtraInfo() {
                return ';' + _base_year + "-" + (parseInt(_base_year) + 7) + " RQMT Report";
            }
            function getArrayItemIndex(arr, item) {
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i] == item) {
                        return i;
                    }
                }
                return -1;
            }

        </script>

        <script id="jsMenuButtons" type="text/javascript">
            function myProfile_Select(value) {
                if (value == "My Profile") {
                    showUserProfile();
                }
                else if (value == 'PasswordQuestion') {
                    changeSecurity();
                }
                else if (value == "Options") {
                    showOptionsPage();
                }
            }

            function openMenuItem(url) {
                if (url) {
                    var nWindow;
                    var w = 0, h = 0;
                    var qsValue = '', strTitle = '';

                    //get query string params
                    for (var i = 0; i < url.split("&").length; i++) {
                        qsValue = url.split("&")[i];

                        if (qsValue.indexOf('width=') > -1) {
                            qsValue = qsValue.substring(qsValue.indexOf('width='));
                            w = qsValue.substring(qsValue.indexOf('width=') + 6);
                        }

                        if (qsValue.indexOf('height=') > -1) {
                            qsValue = qsValue.substring(qsValue.indexOf('height='));
                            h = qsValue.substring(qsValue.indexOf('height=') + 7);
                        }

                        if (qsValue.indexOf('Title=') > -1) {
                            qsValue = qsValue.substring(qsValue.indexOf('Title='));
                            strTitle = qsValue.substring(qsValue.indexOf('Title=') + 6);
                        }
                    }

                    if (url.indexOf('.aspx?') > -1) {
                        //open in a popup
                        if (h > document.body.clientHeight - 40) {
                            h = document.body.clientHeight - 40;
                        }

                        var openPopup = popupManager.AddPopupWindow('MenuItem', strTitle, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
                        if (openPopup) openPopup.Open();
                    }
                    else {

                        //open in a window
                        if (w > 0 && h > 0) {
                            nWindow = window.open(url, 'nWindow', 'status=0,menubar=0,resizable=0,scrollbars=0,height=' + h + ',width=' + w);
                        }
                        else {
                            nWindow = window.open(url);
                        }

                        if (!nWindow) MessageBox('A new window was not able to open. This could be because you have a popup blocker installed and running.\nEither disable the popup blocker, allow for this site, or hold down the Ctrl key when you click this button.');
                    }
                }
            }

            function showUserProfile() {
                var url = 'Admin/UserProfileEditParent.aspx?'
                    + 'New=false'
                    + '&UserID=' + _userId
                    + '&CurrentUser=' + true
                    + '&popup=true'
                    + '&random=' + new Date().getTime();

                var nPopup = popupManager.AddPopupWindow('MyProfile', 'User Profile', url, 650, 1050, 'PopupWindow', this);
                if (nPopup) {
                    nPopup.Open();
                }

                return false;
            }

            function changeSecurity() {
                var url = 'Account/Manage.aspx?random=' + new Date().getTime();

                var nPopup = popupManager.AddPopupWindow('ChangeSecurity', 'Change Password / Security Question', url, 625, 580, 'PopupWindow', this);
                if (nPopup) {
                    nPopup.Open();
                }

                return false;
            }

            function buttonRequestedReports_click() {
                var nTitle = 'Manage Reports';
                var nHeight = 650, nWidth = 1100;
                var nURL = 'Reports_Grid.aspx';

                var left = (screen.width / 2) - (nWidth / 2);
                var top = (screen.height / 2) - (nHeight / 2);

                window.open(nURL, 'ManageReportsWindow', 'status=0,menubar=0,resizable=0,scrollbars=0,height=' + nHeight + ',width=' + nWidth + ',left=' + left + ',top=' + top);
            }

            function buttonSR_click() {
                try {
                    var url = 'Loading.aspx?Page=SR_Add.aspx?random=' + new Date().getTime();

                    var srPopup = popupManager.AddPopupWindow('AddSR', 'Sustainment Request', url, 250, 600, 'PopupWindow', window.self);
                    if (srPopup) {
                        srPopup.Open();
                    }
                } catch (e) {
                    MessageBox('Error: buttonSR_click - ' + e.number + ' : ' + e.message);
                }
            }

            function btnTools_click() {
                try {
                    var url = ''
                        + '&random=' + Math.random();

                    var AddAttachmentPopup = popupManager.AddPopupWindow('adminToolsWindow', 'Admin Tools', url, 600, 1200, "PopupWindow", this.self);
                    if (AddAttachmentPopup) {
                        AddAttachmentPopup.Open();
                    }

                } catch (e) {
                    MessageBox('Error: btnTools_click(): ' + e.number + ' : ' + e.message);
                }
            }

            function showOptionsPage() {
                var url = 'Loading.aspx?Page=' + _pageUrls.UserOptionsContainer + '?'
                    + 'UserId=<%=this.UserId %>';

                var openPopup = popupManager.AddPopupWindow('My Options', 'My Options', url + '?userNMID=' + '<%=this.UserId %>', 550, 790, 'PopupWindow', this);
                if (openPopup) {
                    openPopup.Open();
                }
            }

        </script>
    </form>

    <script type="text/javascript">
            function refreshPage() {
                window.location.href = window.location.href;
            }

            function keepSessionAlive() {
                $.ajax({
                    type: "POST",
                    url: "SessionMethods.asmx/KeepSessionAlive",
                    data: "{}",
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: null,
                    error: null
                });
            }

            function pageResize() {
                try {
                    var heightModifier = $('#mainPageFooter').height();

                    resizePageElement('sidebar', heightModifier + 1);
                    resizePageElement('sidebarExpander', heightModifier + 1);
                    resizePageElement('pageContentFrame', heightModifier + 2);
                    resizePageElement('divAppliedFilters', heightModifier + 2);
                }
                catch (e) {
                    MessageBox('pageResize', e.number, e.message);
                }
            }

            function initializeModules() {
                try {
                    $('#trSelectedModule').click(function () {
                        menuSelection_click($(this).attr('direction'), $('#trSelectedModule').attr('moduleName'));
                    });
                    $('#tableAvailableModules tr').click(function () {
                        menuSelection_click('COLLAPSE', $(this).attr('moduleName'));
                    });
                    $('#tableAvailableModules tr').mouseover(function () {
                        $(this).css('background-image', 'url(Images/Headers/gridheaderblue.png)');
                    });
                    $('#tableAvailableModules tr').mouseout(function () {
                        $(this).css('background-image', 'url(Images/page_header_back.gif)');
                    });

                    menuSelection_click('EXPAND', '');
                } catch (e) {

                }
            }

            function showHideNavPane() {
                var visible = ($('#sidebar').css('display') != 'none'); //$('#sidebar').is(':visible');
                if (!visible) {
                    $('#sidebar').show();
                    $('#pageSpacer').width(6);
                }
                else {
                    $('#sidebar').hide();
                    $('#pageSpacer').width(12);
                }

                pageResize();
            }

            function initializeVariables() {
                try {
                    _userId = parseInt('<%=this.UserId %>');
                _pageUrls = new PageURLs();
            } catch (e) {

            }
        }

        var divSystemNotification = document.getElementById('divSystemNotification');

        function showSystem_Notification(firstOpen) {
            // create a new instance of SmoothMovement to show System Notification
            if (divSystemNotification.offsetHeight == 30) {
                moveSystemNotification();
                var sysNotOpener = new SmoothMovement(30, 300);
                addEventHandler(window, "resize", moveSystemNotification);
                sysNotOpener.animate(30, function (position) { resizeSystemNotificationPane(position) });
                document.getElementById("imgNotificationArrow").src = "Images/fatArrowDown.png";
                document.getElementById("trNotification").title = "Close Notification";
                document.getElementById("imgNotificationArrow").alt = "Close Notification";
            } else {
                if (divSystemNotification.offsetHeight == 300) {
                    hideSystem_Notification();
                    document.getElementById("imgNotificationArrow").src = "Images/fatArrowUp.png";
                    document.getElementById("trNotification").title = "Open Notification";
                    document.getElementById("imgNotificationArrow").alt = "Open Notification";
                }
            }
        }

        function moveSystemNotification() {
            divSystemNotification.style.left = parseInt(document.body.clientWidth) - divSystemNotification.offsetWidth + 'px';
        }

        function resizeSystemNotificationPane(nPosition) {
            divSystemNotification.style.height = nPosition + 'px';
        }

        function hideSystem_Notification() {
            var sysNotCloser = new SmoothMovement(300, 30);
            sysNotCloser.animate(30, function (position) { resizeSystemNotificationPane(position) });
        }

        function showNews() {
            showStartPage('', '', 1);
            showSystem_Notification(false);
        }

        function initializeEvents() {
            try {
                $(document).keyup(function () { page_keyup(); });

                $('#imgAORHome').click(function () { imgAORHome_click(); return false; });
                $('#imgNews').click(function () { imgNews_click(); return false; });
                $('#imgMetrics').click(function () { imgMetrics_click(); return false; });
                $('#buttonSR').click(function () { buttonSR_click(); return false; });
                $('#buttonRequestedReports').click(function () { buttonRequestedReports_click(); return false; });
                $('#buttonGetData').click(function () { buttonGetData_click(); return false; });
                $('#<%=this.ddlView_Work.ClientID %>').change(function () { clearViewDataSession(); return false; });
                $('#imgViewHelp').click(function () { imgViewHelp_click(); });
                $('#imgShowSearch').click(function () { imgShowSearch_click(); });
                $('#imgSaveFilter').click(function () { saveCustomFilters(); });
                $('#imgDeleteFilter').click(function () { deleteCustomFilters(); });
                $('#imgShowFilters').click(function () { imgShowFilters_click(); });
                $('#imgClearFilters').click(function () { imgClearFilters_click(); });
                $('#buttonEmailHotList').click(function () { imgEMailHotlist_click(); });
                $('#buttonSRHotList').click(function () { imgSRHotlist_click(); });
                $('#trNotification').click(function () { showSystem_Notification(false); return false; });

                if ('<%= showSysNotification %>' == 'True'){
                    showSystem_Notification(true);
                    $('#divSystemNoteInfo').click(function () { showNews(); });
                }

                $('#pageSpacer').click(function () { showHideNavPane(); });
            } catch (e) { }
        }

        $(document).ready(function () {
            setInterval(keepSessionAlive, 300000); //5 minutes
            $(window).resize(pageResize);
            document.body.style.overflow = 'hidden';

            initializeVariables();
            initializeEvents();
            initializeModules();

            $('#trWork').trigger('click');

            var startPage = +'<%=this.StartupPageId %>' == 2 ? 'Metrics' : 'News';
            //showStartPage(startPage);
            showStartPage('Dashboard');

            if (ddlSavedFilters) {
                ddlSavedFilters.Reposition();
            }

            $("a").attr("target", "_blank");

            gatherFilters();
            saveFilters(false, "Load_HomePage", true);

            pageResize();
        });
    </script>
</body>
</html>