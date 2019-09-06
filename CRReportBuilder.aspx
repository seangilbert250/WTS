﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CRReportBuilder.aspx.cs" Inherits="CRReportBuilder" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2018 Infinite Technologies, Inc. -->
    <script type="text/javascript" src="Scripts/reports.js"></script>
    <link rel="stylesheet" type="text/css" href="Styles/tooltip.css" />
</asp:Content>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CR Report Builder</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
</asp:Content>

<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <table style="width: 100%;">
		<tr>
			<td>                
                <input type="button" id="btnClose" value="Back To Parameter Page" style="vertical-align: middle;" />
                <input type="button" id="btnSave" value="Save" style="vertical-align: middle;" />
			    <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
			</td>
		</tr>
	</table>
</asp:Content>

<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>

    <div id="divAddEdit">
        <table style="width: 100%; border-collapse: collapse;" cellpadding="0px">
            <tr>
                <td id="tdLeftPane" valign="top" style="width:60%;padding:3px;"> <!-- left side of page -->
                    <table style="width: 100%; border-collapse: collapse;" cellpadding="5px">                        
                        <tr> <!-- left top -->
                            <td style="width:800px;">
                                <table id="tblDDL" style="border-collapse: collapse;" cellpadding="5px">
                                    <tr>                                        
                                        <td style="text-align:left;font-weight:bold;width:150px">Release: <span style="color:red">*</span></td>
                                        <td style="text-align:left;font-weight:bold;width:150px">Contract: <span style="color:red">*</span></td>
                                        <td style="text-align:left;white-space:nowrap">&nbsp;</td>
                                    </tr>
                                    <tr style="">
                                        <td style="text-align:left;vertical-align:top;">
                                            <select id="ddlRelease" runat="server" style="background-color:#f5f6ce;width:150px;">
                                            </select>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <div id="divContractContainer" style="white-space:nowrap">
                                                <select id="ddlContract" runat="server" style="background-color:#f5f6ce;width:150px;">
                                                </select>
                                            </div>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <div id="divVisibleContainer" style="white-space:nowrap">
                                                <input type="checkbox" id="chkVisible" runat="server" checked/>
                                                <span>Visible to Customer </span>
                                            </div>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;white-space:nowrap;">
                                            <input type="button" id="btnClearDDLs" value="Clear">&nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr> <!-- left bottom -->
                            <td style="width:100%;">
                                <table style="width: 100%; border-collapse: collapse;" cellpadding="0px">
                                    <tr id="builderRow">
                                        <td colspan="2" style="padding-top:5px;border-top:1px solid #dddddd;position:relative;">
                                            <div id="lblSelected">
                                                <table style="width: 98%">
                                                    <tr>
                                                        <td style="width: 135px;">
                                                            <strong>Release: </strong><span id="spnBuilderRelease"></span>
                                                        </td>
                                                        <td>
                                                            <strong>Contract: </strong><span id="spnBuilderContract"></span>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div id="divCRReportBuilder" runat="server" style="height: 82vh; overflow: auto;">
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td id="waProgramMGMT" workloadallocation="ProgramMGMT" workloadallocationID="21" workloadallocationsort="1" class="droppableCR" colspan="2" style="height: 75px; vertical-align: text-top; border: 1px solid #ffffff;">
                                                            <strong>Workload Allocation: </strong>Program MGMT
                                                            <div id="divProgramMGMTDropCRs" style="padding-left: 5px;"></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="waDeployment" workloadallocation="Deployment" workloadallocationID="22" workloadallocationsort="2" class="droppableCR" colspan="2" style="height: 75px; vertical-align: text-top; border: 1px solid #ffffff;">
                                                            <strong>Workload Allocation: </strong>Deployment
                                                            <div id="divDeploymentDropCRs" style="padding-left: 5px;"></div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="waProduction" workloadallocation="Production" workloadallocationID="23" workloadallocationsort="3" class="droppableCR" colspan="2" style="height: 75px; vertical-align: text-top; border: 1px solid #ffffff;">
                                                            <strong>Workload Allocation: </strong>Production
                                                            <div id="divProductionDropCRs" style="padding-left: 5px;"></div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td id="tdtoggle" style="width:6px;background-color:#bbbbbb;border:1px solid #aaaaaa;text-align:center;vertical-align:top;cursor:pointer;"> <!-- divider -->
                    <div style="width:6px;vertical-align:middle;line-height:100vh;margin:0 auto;">
                        <img src="images/pageRollerOver.gif" style="vertical-align:middle;">
                    </div>
                </td>
                <td id="tdrightpane" valign="top" style="width:40%;padding:3px;position:relative;"> <!-- right side of page -->
                    <div id="divTabsContainer" class="" style="padding: 0px;overflow:hidden;">                        
                        <div id="divCRs" style="width:98%; padding:3px; border-bottom: 1px solid #aaaaaa;">
                            Available CRs - <span id="spnCRCount" style="padding-right: 52px;">0</span> Total CRs - <span id="spnCRTotal">0</span>
                            <div id="divLoadedCRs" style="overflow: auto; height: 35vh; padding-left: 15px;"></div>
                        </div>
                        <div id="divAORs" style="width:98%; padding:3px; border-bottom: 1px solid #aaaaaa;">
                            Available AORs - <span id="spnAORCount" style="padding-right: 55px;">0</span> Total AORs - <span id="spnAORTotal">0</span>
                            <div id="divLoadedAORs" style="overflow: auto; height: 35vh; padding-left: 15px;"></div>
                        </div>
                        <div id="divSystems" style="width:98%; padding:3px;">
                            Available Deployments  - <span id="spnDeploymentCount" style="padding-right: 18px;">0</span>Drag Deployments onto AORs
                            <div id="divLoadedDeployments" style="overflow: auto; height: 16vh; padding-left: 15px;"></div>
                        </div>
                        <div id="divTabsLoading" style="position:absolute;"></div>
                    </div> 
                </td>
            </tr>
        </table>
    </div>

    <div id="divMessagePlaceHolder"></div>

    <div id="divAttributeSelector" style="position:absolute;display:none;padding:5px;">
    </div>

    <iframe id="frmDownload" style="display: none;"></iframe>
    
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _changesMade;
        var _pageUrls;

        var _crColor;
        var _crColorBorder;
        var _crColorHighlight;
        var _aorColor;
        var _aorColorBorder;
        var _aorColorHighlight;
        var _deploymentColor;
        var _deploymentColorBorder;
        var _deploymentColorHighlight;

        var _dragItemTextErrorColor;
        var _dragItemBackgroundErrorColor;
        var _dragItemBorderErrorColor;

    </script>

    <script id="jsEvents" type="text/javascript">
        ///////////////////////////////////////////////////////
        // GENERAL FUNCTIONS
        ///////////////////////////////////////////////////////
        function imgRefresh_click() {
            refreshPage();
        }

        function btnClose_click() {
             QuestionBox('Confirmation', 'You are going to lose changes, do you want to save?', 'Yes,No', 'close_confirmed', 300, 300, this);
        }  

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'CR':
                    btnCR_click();
                    break;
                case 'AOR':
                    btnAOR_click();
                    break;
                case 'WorkTasks':
                    btnTasks_click();
                    break;
                case 'Deployments':
                    btnDeployments_click();
                    break;
                case 'CoverPage':
                    btnCoverPage_click();
                    break;
                case 'ReleaseBuilder':
                    btnReleaseBuilder_click();
                    break;
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function btnCR_click() {
            var nWindow = 'CRs';
            var nTitle = 'CR';
            var nHeight = 725, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORContainer + '?' + 'GridType=CR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAOR_click() {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORContainer + '?' + 'GridType=AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnTasks_click() {
            var nWindow = 'Tasks';
            var nTitle = 'Work Tasks';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + '?Type=Task List';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDeployments_click() {
            var nWindow = 'Deployments';
            var nTitle = 'Deployments';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORContainer + '?' + 'GridType=Deployments';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnCoverPage_click() {
            var nWindow = 'CRReportNarratives';
            var nTitle = 'CR Report Narratives';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.MasterData.MaintenanceContainer + '?' + 'MDType=Narrative';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnReleaseBuilder_click() {
            var nWindow = 'ReleaseBuilder';
            var nTitle = 'Release Builder';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORReleaseBuilder + window.location.search;
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

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    var builder = [];

                    $('[id$=divCRReportBuilder]').each(function () {
                        var $obj = $(this);

                        if ($obj.find('div[crid]').length > 0) {
                            $obj.find('div[crid]').each(function (crRowIndex, crRow) {
                                if ($(crRow).find('div[aorid]').length > 0) {
                                    $(crRow).find('div[aorid]').each(function (rowIndex, row) {
                                        builder.push({
                                            'workloadAllocation': $(crRow).attr('workloadallocation'),
                                            'workloadAllocationID': $(crRow).attr('workloadallocationid'),
                                            'crid': $(crRow).attr('crid'),
                                            'aorid': $(row).attr('aorid'),
                                            'deploymentid': $(row).attr('deploymentid')
                                        });
                                    });
                                }
                            });
                        }
                    });

                    var builderJSON = '{save:' + JSON.stringify(builder) + '}';

                    PageMethods.Save($('[id$=ddlRelease]').val(), $('[id$=ddlContract]').val(), builderJSON, save_done, on_error);
                } else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                clearLoadingMessage();
                MessageBox('An error has occurred. ' + e.message);
            }
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
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('Save Successful.');
                }
                else {
                    MessageBox('Failed to save. \n' + errorMsg);
                }
                ShowDimmer(false);
            }
            catch (e) {
                alert("Error in Save_Done. " + e.message);
                ShowDimmer(false);
            }
        }

        function close_confirmed(answer) {
            if (answer == 'Yes') {
               btnSave_click(true);
            }
            else {
                var nURL = _pageUrls.Reports.CR;

                window.location.href = 'Loading.aspx?Page=' + nURL;
            }
        }

        function on_error() {
            ShowDimmer(false);
            clearLoadingMessage();
            MessageBox('An error has occurred.');
        }

        function refreshPage() {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'ReleaseID', $('[id$=ddlRelease]').val());
            nURL = editQueryStringValue(nURL, 'ContractID', $('[id$=ddlContract]').val());
            nURL = editQueryStringValue(nURL, 'Visible', $('[id$=chkVisible]')[0].checked == true ? 1 : 0);

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function validate() {
            var validation = [];
            var blnExit = false;

            $('[id$=divCRReportBuilder]').find('div[crid]').each(function (crRowIndex, row) {
                if (blnExit) return false;

                if ($(row).find('div[aorid]').length == 0) {
                    validation.push('A CR cannot be empty.');
                }
                if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(row).attr('crid') + '"]').length > 1) {
                    validation.push('Duplicate CRs has been found.');
                    validation.push($(row).find('span:first').text());
                    blnExit = true;
                    return false;
                }
            });

            blnExit = false;
            $('[id$=divCRReportBuilder]').find('div[aorid]').each(function (crRowIndex, row) {
                if (blnExit) return false;

                if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(row).attr('aorid') + '"][deploymentid="' + $(row).attr('deploymentid') + '"]').length > 1) {
                    validation.push('Duplicate AORs has been found. ' + $(row).attr('aorid'));
                    blnExit = true;
                    return false;
                }
            });

            return validation.join('<br>');
        }

        function toggleRightPane() {
            $('#tdrightpane').toggle();

            if ($('#tdrightpane').is(':visible')) {
                $('#tdtoggle').css('width', '6px');
                $('#tdLeftPane').css('width', '65%');
            }
            else {                
                $('#tdtoggle').css('width', '8px');
                $('#tdLeftPane').css('width', '100%');
            }
        }

        ///////////////////////////////////////////////////////
        // UTILITIES / HELPERS
        ///////////////////////////////////////////////////////

        function showLoadingMessage(msg, autoClear, container, errorMode) {
            if (autoClear == null) autoClear = false;
            if (container == null) container = $('#divMessagePlaceHolder')[0];
            if (errorMode) {
                dangerMessage(msg, null, autoClear, container);
            }
            else {
                infoMessage(msg, null, autoClear, container);
            }
        }

        function clearLoadingMessage(container) {            
            if (container == null) container = $('#divMessagePlaceHolder');
            $(container).html('');
        }      

        ///////////////////////////////////////////////////////
        // Release, Contract DDLs
        ///////////////////////////////////////////////////////

        function input_changed() {
            var releaseID = $('[id$=ddlRelease]').val();
            var contractID = $('[id$=ddlContract]').val();
            var visible = $('[id$=chkVisible]')[0].checked;

            $('#spnBuilderRelease').text($('[id$=ddlRelease] option:selected').text());
            $('#spnBuilderContract').text($('[id$=ddlContract] option:selected').text());

            clearDataSet();

            if (releaseID > 0 && contractID > 0) {
                showLoadingMessage('Loading data...');
                PageMethods.GetCRReportData(releaseID, contractID, visible, loadCRReportData_done, on_error);
            }
        }
        
        function loadCRReportData_done(result) { 
            var dt = jQuery.parseJSON(result);
            if (dt.CR != null && dt.CR.length > 0) {
                var html = '';

                $.each(dt.CR, function (rowIndex, row) {
                    html += '<div style="background-color: ' + _crColor + '; padding: 5px;">';
                    html += '<span crid="' + row.CRID + '" sort="' + row.Sort + '" primarysr="' + row.PrimarySR + '" class="draggableCR" style="border-radius: 5px; padding: 2px; cursor:pointer;">'
                    html += '<a href="" onclick="openCR(' + row.CRID + '); return false;" style="color: blue;">CR</a> ' + (row.PrimarySR == 0 ? '' : row.PrimarySR + ' ') + decodeURIComponent(row.CRName);
                    html += '</span></div>';
                });
                $('#divLoadedCRs').html(html);
                $('#spnCRCount').text(dt.CR.length);
                $('#spnCRTotal').text(dt.CR.length);
            } else {
                $('#divLoadedCRs').html('');
                $('#spnCRCount').text('0');
                $('#spnCRTotal').text('0');
            }

            if (dt.AOR != null && dt.AOR.length > 0) {
                var html = '';

                $.each(dt.AOR, function (rowIndex, row) {
                    if (dt.Deployment != null && dt.Deployment.length > 0) {
                        var deploymentID = 0;

                        $.each(dt.Deployment, function (dRowIndex, dRow) {
                            if (deploymentID != dRow.DeploymentID && row.AORID == dRow.AORID && html.indexOf('aorid="' + row.AORID + '"') == -1) {
                                var date = dRow.ScheduledDate == 9999 ? "" : new Date(dRow.ScheduledDate);
                                if (date != "") date.setDate(date.getDate() + 1);
                                date = date != "" ? (date.getMonth() + 1) + '/' + ('0' + date.getDate()).slice(-2) + " " : "";
                                html += '<div style="background-color: ' + _aorColor + '; padding: 5px;">';
                                html += '<span aorid="' + row.AORID + '" deploymentid="' + dRow.DeploymentID + '" class="draggableAOR" style="border-radius: 5px; padding: 2px; cursor:pointer;">';
                                html += '<span>.' + dRow.ReleaseScheduleDeliverable + ' ' + date + '</span>';
                                html += 'AOR <a href="" onclick="openAOR(' + row.AORID + ', ' + row.AORReleaseID + '); return false;" style="color: blue;">' + row.AORID + '</a> ' + row.AORName;
                                html += '</span></div>';
                                deploymentID = dRow.DeploymentID;
                            }
                        });
                    }

                    if (html.indexOf('aorid="' + row.AORID + '"') == -1) {
                        html += '<div style="background-color: ' + _aorColor + '; padding: 5px;">';
                        html += '<span aorid="' + row.AORID + '" deploymentid="0" class="draggableAOR" style="border-radius: 5px; padding: 2px; cursor:pointer;">';
                        html += 'AOR <a href="" onclick="openAOR(' + row.AORID + ', ' + row.AORReleaseID + '); return false;" style="color: blue;">' + row.AORID + '</a> ' + row.AORName;
                        html += '</span></div>';
                    }

                });
                $('#divLoadedAORs').html(html);
                $('#spnAORCount').text(dt.AOR.length);
                $('#spnAORTotal').text(dt.AOR.length);
            } else {
                $('#divLoadedAORs').html('');
                $('#spnAORCount').text('0');
                $('#spnAORTotal').text('0');
            }
            
            if (dt.AOR != null && dt.AOR.length > 0) {
                var crid = 0;
                var crCount = 0;
                var aorCount = 0;
                $.each(dt.AOR, function (rowIndex, row) {
                    if (row.CRID > -9999 && row.VisibleToCustomer && ($('[id$=divCRReportBuilder]').find('div[crid]').length == 0 || 
                        ($('[id$=divCRReportBuilder]').find('div[crid="' + row.CRID + '"]').length == 0 || $('[id$=divCRReportBuilder]').find('div[crid="' + row.CRID + '"][workloadallocationid="' + row.WorkloadAllocationID + '"]').length == 0))) {
                        var html = '';

                        html += '<div crid="' + row.CRID +
                            '" sort="' + row.Sort + 
                            '" primarysr="' + row.PrimarySR +
                            '" workloadallocation="' + $('[id$=divCRReportBuilder]').find('td[workloadallocationid="' + row.WorkloadAllocationID + '"]').attr('workloadallocation') +
                            '" workloadallocationid="' + row.WorkloadAllocationID +
                            '" class="droppedCR" style="padding: 5px;">';
                        html += '<img src="Images/Icons/cross.png" onclick="removeItem($(this))" style="width: 12px; height: 12px; padding-right: 3px; visibility: hidden; cursor:pointer;"/>';
                        html += '<span style="border-radius: 3px; padding: 1px; background-color: ' + _crColor + '; text-decoration: underline; cursor:pointer;">CR ' + (row.PrimarySR == 0 ? '' : row.PrimarySR + ' ') + decodeURIComponent(row.CRName);
                        html += '</span>';
                        html += '<div id="div' + row.CRID + 'DropAORsEmpty" workloadallocationid="' + row.WorkloadAllocationID + '" style="padding-left: 15px; font-size: 25px; opacity: 0.25; display: none;">Drag AORs Here</div>';
                        html += '</div > ';

                        $('#div' + $('[id$=divCRReportBuilder]').find('td[workloadallocationid="' + row.WorkloadAllocationID + '"]').attr('workloadallocation') + 'DropCRs').append(html);
                        $('#divLoadedCRs').find('span[crid="' + row.CRID + '"]').css('background-color', _dragItemBackgroundErrorColor);
                        crid = row.CRID;
                        crCount++;
                    }

                    if (row.VisibleToCustomer && $('[id$=divCRReportBuilder]').find('div[crid="' + row.CRID + '"]').length > 0) {
                        var html = '';

                        if (dt.Deployment != null && dt.Deployment.length > 0) {
                            var deploymentID = 0;

                            $.each(dt.Deployment, function (dRowIndex, dRow) {
                                html = '';
                                if (deploymentID != dRow.DeploymentID && row.AORID == dRow.AORID) {
                                    var date = dRow.ScheduledDate == 9999 ? "" : new Date(dRow.ScheduledDate);
                                    if (date != "") date.setDate(date.getDate() + 1);
                                    date = date != "" ? (date.getMonth() + 1) + '/' + ('0' + date.getDate()).slice(-2) : "";

                                    html += '<div aorid="' + row.AORID + '" deploymentid="' + dRow.DeploymentID + '" class="droppedAOR">';
                                    html += $('[id$=divCRReportBuilder]').find('div[aorid="' + row.AORID + '"]').length == 0 ? '<img src="Images/Icons/cross.png" onclick="removeItem($(this))" style="width: 12px; height: 12px; padding-right: 3px; visibility: hidden; cursor:pointer;"/>' : "";
                                    html += '<span style="border-radius: 3px; padding: 1px; background-color: ' + _aorColor + '; cursor:pointer;';
                                    html += $('[id$=divCRReportBuilder]').find('div[aorid="' + row.AORID + '"]').length > 0 ? 'display:none;' : "";
                                    html += '">';
                                    html += '<span class="droppedDeployment" aorid="' + row.AORID + '" deploymentid="' + dRow.DeploymentID + '" style="background-color: ' + _deploymentColor + '; cursor:pointer;">.' + dRow.ReleaseScheduleDeliverable + ' ' + date + '</span>';
                                    html += (date != "" ? " " : "") + 'AOR ' + row.AORID + " " + row.AORName;
                                    html += '</span></div>';

                                    $('[id$=divCRReportBuilder]').find('div[crid="' + row.CRID + '"][workloadallocationid="' + row.WorkloadAllocationID+ '"]').append(html);
                                    $('#divLoadedAORs').find('span[aorid="' + row.AORID + '"]').css('background-color', _dragItemBackgroundErrorColor);
                                    deploymentID = dRow.DeploymentID;
                                }
                            });
                        }

                        if ($('[id$=divCRReportBuilder]').find('div[aorid="' + row.AORID + '"]').length == 0) {
                            html += '<div aorid="' + row.AORID + '" deploymentid="0" class="droppedAOR">';
                            html += '<img src="Images/Icons/cross.png" onclick="removeItem($(this))" style="width: 12px; height: 12px; padding-right: 3px; visibility: hidden; cursor:pointer;"/>';
                            html += '<span style="border-radius: 3px; padding: 1px; background-color: ' + _aorColor + '; cursor:pointer;">AOR ' + row.AORID + " " + row.AORName;
                            html += '</span></div>';

                            $('[id$=divCRReportBuilder]').find('div[crid="' + row.CRID + '"][workloadallocationid="' + row.WorkloadAllocationID + '"]').append(html);
                            $('#divLoadedAORs').find('span[aorid="' + row.AORID + '"]').css('background-color', _dragItemBackgroundErrorColor);
                        }
                        aorCount++;
                    }
                });
                $('#spnCRCount').text($('#spnCRCount').text() - crCount);
                $('#spnAORCount').text($('#spnAORCount').text() - aorCount);
            }

            // Sort CRs
            $('[id$=divCRReportBuilder]').find('td[workloadallocation]').each(function () {
                var waElement = $('#div' + $(this).attr('workloadallocation') + 'DropCRs');
                var crElements = waElement.find('div[crid]');
                crElements.sort(function (a, b) {
                    var compA = $(a).attr('crid');
                    var compB = $(b).attr('crid');
                    return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                });
                waElement.find('div[crid]').detach();
                $.each(crElements, function (index, item) {
                    waElement.append(item);
                });

                crElements = waElement.find('div[crid]');
                crElements.sort(function (a, b) {
                    var compA = $(a).attr('primarysr');
                    var compB = $(b).attr('primarysr');
                    return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                });
                waElement.find('div[crid]').detach();
                $.each(crElements, function (index, item) {
                    waElement.append(item);
                });

                crElements = waElement.find('div[crid]');
                crElements.sort(function (a, b) {
                    var compA = $(a).attr('sort');
                    var compB = $(b).attr('sort');
                    return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                });
                waElement.find('div[crid]').detach();
                $.each(crElements, function (index, item) {
                    waElement.append(item);
                });

                if ($(crElements).length == 0) {
                    $('#div' + $(this).attr('workloadallocation') + 'DropCRsEmpty').show();
                } else {
                    $('#div' + $(this).attr('workloadallocation') + 'DropCRsEmpty').hide();
                }
            });

            if (dt.Deployment != null && dt.Deployment.length > 0) {
                var html = '';
                var deploymentID = 0;
                var deploymentCount = 0;
                $.each(dt.Deployment, function (rowIndex, row) {
                    if (row.DeploymentID != deploymentID) {
                        var date = row.ScheduledDate == 9999 ? "" : new Date(row.ScheduledDate);
                        if (date != "") date.setDate(date.getDate() + 1);
                        date = date != "" ? (date.getMonth() + 1) + '/' + ('0' + date.getDate()).slice(-2) + " " : "";
                        html += '<div style="background-color: ' + _deploymentColor + '; padding: 5px;">';
                        html += '<span deploymentid="' + row.DeploymentID + '" class="draggableDeployment" style="border-radius: 5px; padding: 2px; cursor:pointer;">';
                        html += '<a href="" onclick="openDeployment(' + row.DeploymentID + '); return false;" style="color: blue;">.' + row.ReleaseScheduleDeliverable + '</a> ' + date + ' ' + row.Description;
                        html += '</span></div>';
                        deploymentID = row.DeploymentID;
                        deploymentCount++;
                    }
                });
                $('#divLoadedDeployments').html(html);
                $('#spnDeploymentCount').text(deploymentCount);
            } else {
                $('#divLoadedDeployments').html('');
                $('#spnDeploymentCount').text('0');
            }

            initDragControls();
            initDropControls();
            clearLoadingMessage();
        }  

        function clearDataSet() {
            $('#divLoadedCRs').html('');
            $('#divLoadedAORs').html('');
            $('#divLoadedDeployments').html('');
            $('#divProgramMGMTDropCRs').html('');
            $('#divDeploymentDropCRs').html('');
            $('#divProductionDropCRs').html('');
            $('#divProgramMGMTDropCRsEmpty').css('display', 'none');
            $('#divDeploymentDropCRsEmpty').css('display', 'none');
            $('#divProductionDropCRsEmpty').css('display', 'none');
            $('#spnCRCount').text('0');
            $('#spnCRTotal').text('0');
            $('#spnAORCount').text('0');
            $('#spnAORTotal').text('0');
            $('#spnDeploymentCount').text('0');
        }

        function btnClearDDLs_click() {
            $('[id$=ddlRelease]').val(0);
            $('[id$=ddlContract]').val(0);
            $('#spnBuilderRelease').text('');
            $('#spnBuilderContract').text('');
            clearDataSet();
        }

        function openCR(CRID) {
            var nWindow = 'CR';
            var nTitle = 'CR';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=false&CRID=' + CRID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openAOR(AORID, AORReleaseID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openDeployment(deploymentID) {
            var nWindow = 'Deployment';
            var nTitle = 'Deployment';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=false&DeliverableID=' + deploymentID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function initDragControls() {
            $('.draggableCR').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').length == 0) {
                        $(ui.helper).css('font-weight', 'bold');
                        $(ui.helper).css('color', _crColorBorder);
                        $(ui.helper).css('background-color', _crColor);
                        $(ui.helper).css('border', '1px solid ' + _crColorBorder);
                        $(".droppableCR").css('background-color', _crColor);
                    } else {
                        $(ui.helper).css('color', _dragItemTextErrorColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                    }
                },
                stop: function (e, ui) {
                    $(".droppableCR").css('background-color', 'transparent');
                    $("#waProgramMGMT, #waDeployment, #waProduction").css('border', '1px solid #ffffff');
                }
            });

            $('.draggableAOR').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').length == 0) {
                        $(ui.helper).css('font-weight', 'bold');
                        $(ui.helper).css('color', _aorColorBorder);
                        $(ui.helper).css('background-color', _aorColor);
                        $(ui.helper).css('border', '1px solid ' + _aorColorBorder);
                        $('[id$=divCRReportBuilder]').find('div[crid]').css('background-color', _aorColor);
                    } else {
                        $(ui.helper).css('color', _dragItemTextErrorColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                    }
                },
                stop: function (e, ui) {
                    $('[id$=divCRReportBuilder]').find('div[crid]').css('background-color', 'transparent');
                }
            });

            $('.draggableDeployment').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    $(ui.helper).css('font-weight', 'bold');
                    $(ui.helper).css('color', _deploymentColorBorder);
                    $(ui.helper).css('background-color', _deploymentColor);
                    $(ui.helper).css('border', '1px solid ' + _deploymentColorBorder);
                    $('[id$=divCRReportBuilder]').find('div[aorid]:not([deploymentid="' + $(ui.helper).attr('deploymentid') + '"])').css('background-color', _deploymentColor);
                },
                stop: function (e, ui) {
                    $('[id$=divCRReportBuilder]').find('div[aorid]:not([deploymentid="' + $(ui.helper).attr('deploymentid') + '"])').css('background-color', 'transparent');
                }
            });

            $('.droppedCR').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    $(ui.helper).css('font-weight', 'bold');
                    $(ui.helper).css('color', _crColorBorder);
                    $(ui.helper).css('background-color', _crColor);
                    $(ui.helper).css('border', '1px solid ' + _crColorBorder);
                    $(ui.helper).css('padding', '2px');
                    $(ui.helper).css('border-radius', '5px');
                    $(".droppableCR").css('background-color', _crColor);
                    $(ui.helper).find('img').css('visibility', 'hidden');
                },
                stop: function (e, ui) {
                    $(".droppableCR").css('background-color', '#ffffff');
                    $("#waProgramMGMT, #waDeployment, #waProduction").css('border', '1px solid #ffffff');
                }
            });

            $('.droppedAOR').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    $(ui.helper).css('font-weight', 'bold');
                    $(ui.helper).css('color', _aorColorBorder);
                    $(ui.helper).css('background-color', _aorColor);
                    $(ui.helper).css('border', '1px solid ' + _aorColorBorder);
                    $(ui.helper).css('padding', '2px');
                    $(ui.helper).css('border-radius', '5px');
                    $('[id$=divCRReportBuilder]').find('div[crid]').css('background-color', _aorColor);
                    $(ui.helper).find('img').css('visibility', 'hidden');
                },
                stop: function (e, ui) {
                    $('[id$=divCRReportBuilder]').find('div[crid]').css('background-color', 'transparent');
                }
            });

            $('.droppedDeployment').draggable({
                helper: 'clone',
                revert: 'invalid',
                appendTo: 'body',
                scroll: false,
                start: function (e, ui) {
                    $(ui.helper).css('font-weight', 'bold');
                    $(ui.helper).css('color', _deploymentColorBorder);
                    $(ui.helper).css('background-color', _deploymentColor);
                    $(ui.helper).css('border', '1px solid ' + _deploymentColorBorder);
                    $('[id$=divCRReportBuilder]').find('div[aorid]:not([deploymentid="' + $(ui.helper).attr('deploymentid') + '"])').css('background-color', _deploymentColor);
                },
                stop: function (e, ui) {
                    $('[id$=divCRReportBuilder]').find('div[aorid]:not([deploymentid="' + $(ui.helper).attr('deploymentid') + '"])').css('background-color', 'transparent');
                }
            });
        }

        function initDropControls() {
            $('.droppableCR').droppable({
                tolerance: "pointer",
                over: function (e, ui) {
                    if ($(ui.helper).attr('crid') != undefined) {
                        if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').length == 0 || $(ui.helper).hasClass('droppedCR')) {
                            $(e.target).css('background-color', _crColorHighlight);
                        }
                    }
                },
                out: function (e, ui) {
                    if ($(ui.helper).attr('crid') != undefined) {
                        if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').length == 0 || $(ui.helper).hasClass('droppedCR')) {
                            $(e.target).css('background-color', _crColor);
                        }
                    }
                },
                drop: function (e, ui) {
                    if ($(ui.helper).attr('crid') != undefined) {
                        $('[id$=divCRReportBuilder]').find('td[workloadallocation]').each(function () {
                            if ($(e.target).attr('workloadallocation') == $(this).attr('workloadallocation')) {
                                if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').length == 0) {
                                    var html = ''
                                    html += '<div crid="' + $(ui.helper).attr('crid') +
                                        '" sort="' + $(ui.helper).attr('sort') +
                                        '" primarysr="' + $(ui.helper).attr('primarysr') +
                                        '" workloadallocation="' + $(this).attr('workloadallocation') +
                                        '" workloadallocationid="' + $(this).attr('workloadallocationid') +
                                        '" class="droppedCR" style="padding: 5px;">';
                                    html += '<img src="Images/Icons/cross.png" onclick="removeItem($(this))" style="width: 12px; height: 12px; padding-right: 3px; visibility: hidden; cursor:pointer;"/>';
                                    html += '<span style="border-radius: 3px; padding: 1px; background-color: ' + _crColor + '; text-decoration: underline; cursor:pointer;">' + $(ui.helper).html();
                                    html += '</span>';
                                    html += '<div id="div' + $(ui.helper).attr('crid') + 'DropAORsEmpty" workloadallocationid="' + $(this).attr('workloadallocationid') + '"style="padding-left: 15px; font-size: 25px; opacity: 0.25;">Drag AORs Here</div>';
                                    html += '</div>';

                                    $('#div' + $(this).attr('workloadallocation') + 'DropCRs').append(html);
                                    $('#spnCRCount').text($('#spnCRCount').text() - 1);
                                } else if ($(ui.helper).hasClass('droppedCR')) {
                                    $('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').remove();
                                    $('#div' + $(ui.helper).attr('crid') + 'DropAORsEmpty[workloadallocationid="' + $(ui.helper).attr('workloadallocationid') + '"]').attr('workloadallocationid', $(this).attr('workloadallocationid'));

                                    var html = ''
                                    html += '<div crid="' + $(ui.helper).attr('crid') +
                                        '" sort="' + $(ui.helper).attr('sort') +
                                        '" primarysr="' + $(ui.helper).attr('primarysr') +
                                        '" workloadallocation="' + $(this).attr('workloadallocation') +
                                        '" workloadallocationid="' + $(this).attr('workloadallocationid') +
                                        '" class="droppedCR" style="padding: 5px;">';
                                    html += $(ui.helper).html();
                                    html += '</div>';

                                    $('#div' + $(this).attr('workloadallocation') + 'DropCRs').append(html);
                                }

                                if ($('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRs').find('div[crid]').length == 0) {
                                    $('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRsEmpty').show();
                                } else {
                                    $('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRsEmpty').hide();
                                }

                                if ($('#div' + $(e.target).attr('workloadallocation') + 'DropCRs').find('div[crid]').length == 0) {
                                    $('#div' + $(e.target).attr('workloadallocation') + 'DropCRsEmpty').show();
                                } else {
                                    $('#div' + $(e.target).attr('workloadallocation') + 'DropCRsEmpty').hide();
                                }
                            }
                        });

                        // Sort
                        var waElement = $('#div' + $(this).attr('workloadallocation') + 'DropCRs');
                        var crElements = waElement.find('div[crid]');
                        crElements.sort(function (a, b) {
                            var compA = $(a).attr('crid');
                            var compB = $(b).attr('crid');
                            return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                        });
                        waElement.find('div[crid]').detach();
                        $.each(crElements, function (index, item) {
                            waElement.append(item);
                        });

                        crElements = waElement.find('div[crid]');
                        crElements.sort(function (a, b) {
                            var compA = $(a).attr('primarysr');
                            var compB = $(b).attr('primarysr');
                            return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                        });
                        waElement.find('div[crid]').detach();
                        $.each(crElements, function (index, item) {
                            waElement.append(item);
                        });

                        crElements = waElement.find('div[crid]');
                        crElements.sort(function (a, b) {
                            var compA = $(a).attr('sort');
                            var compB = $(b).attr('sort');
                            return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                        });
                        waElement.find('div[crid]').detach();
                        $.each(crElements, function (index, item) {
                            waElement.append(item);
                        });

                        $('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').effect("highlight", { color: _crColorHighlight }, 1500);
                        $('#divLoadedCRs').find('span[crid="' + $(ui.helper).attr('crid') + '"]').css('background-color', _dragItemBackgroundErrorColor);
                        initDragControls();
                        initDropControls();
                    } 
                }
            });

            $('div[crid]').droppable({
                tolerance: "pointer",
                over: function (e, ui) {
                    if ($(ui.helper).attr('aorid') != undefined) {
                        if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').length == 0 || $(ui.helper).hasClass('droppedAOR')) {
                            $(e.target).css('background-color', _aorColorHighlight);
                        }
                    }
                },
                out: function (e, ui) {
                    if ($(ui.helper).attr('aorid') != undefined) {
                        if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').length == 0 || $(ui.helper).hasClass('droppedAOR')) {
                            $(e.target).css('background-color', _aorColor);
                        }
                    }
                },
                drop: function (e, ui) {
                    if ($(ui.helper).attr('aorid') != undefined) {
                        var crid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').closest('div[crid]').attr('crid');
                        var workloadallocationid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').closest('div[crid]').attr('workloadallocationid');

                        if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').length == 0) {
                            var deploymentDate = $(ui.helper).attr('deploymentid') > 0 ? '<span style="background-color:' + _deploymentColor + '">' + $(ui.helper).text().slice(0, $(ui.helper).text().indexOf('AOR') - 1) + '</span>' : "";
                            var aorText = $(ui.helper).text().slice($(ui.helper).text().indexOf('AOR'));

                            var html = ''
                            html += '<div aorid="' + $(ui.helper).attr('aorid') + '" deploymentid="' + $(ui.helper).attr('deploymentid') + '" class="droppedAOR">';
                            html += '<img src="Images/Icons/cross.png" onclick="removeItem($(this))" style="width: 12px; height: 12px; padding-right: 3px; visibility: hidden; cursor:pointer;"/>';
                            html += '<span style="border-radius: 3px; padding: 1px; background-color: ' + _aorColor + '; cursor:pointer;">';
                            html += deploymentDate + (deploymentDate != "" ? " " : "") + aorText;
                            html += '</span></div>';

                            $(this).append(html);
                            $('#spnAORCount').text($('#spnAORCount').text() - 1);
                        } else if ($(ui.helper).hasClass('droppedAOR')) {
                            $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').remove();

                            var html = ''
                            html += '<div aorid="' + $(ui.helper).attr('aorid') + '" deploymentid="' + $(ui.helper).attr('deploymentid') + '" class="droppedAOR">';
                            html += $(ui.helper).html();
                            html += '</div>';

                            $(this).append(html);
                        }

                        if ($('div[crid="' + $(e.target).attr('crid') + '"][workloadallocationid="' + $(e.target).attr('workloadallocationid') + '"]').find('div[aorid]').length == 0) {
                            $('#div' + $(e.target).attr('crid') + 'DropAORsEmpty[workloadallocationid="' + $(e.target).attr('workloadallocationid') + '"]').show();
                        } else {
                            $('#div' + $(e.target).attr('crid') + 'DropAORsEmpty[workloadallocationid="' + $(e.target).attr('workloadallocationid') + '"]').hide();
                        }

                        if ($('div[crid="' + crid + '"][workloadallocationid="' + workloadallocationid + '"]').find('div[aorid]').length == 0) {
                            $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').show();
                        } else {
                            $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').hide();
                        }

                        // Sort
                        var crElement = $(this);
                        var aorElements = crElement.find('div[aorid]');
                        aorElements.sort(function (a, b) {
                            var compA = $(a).attr('aorid');
                            var compB = $(b).attr('aorid');
                            return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
                        });
                        crElement.find('div[aorid]').detach();
                        $.each(aorElements, function (index, item) {
                            crElement.append(item);
                        });

                        $('[id$=divCRReportBuilder]').find('div[aorid]').css('background-color', 'transparent');
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').effect("highlight", { color: _aorColorHighlight}, 1500);
                        $('#divLoadedAORs').find('span[aorid="' + $(ui.helper).attr('aorid') + '"]').css('background-color', _dragItemBackgroundErrorColor);
                        initDragControls();
                        initDropControls();
                    }
                    $('[id$=divCRReportBuilder]').find('div[crid]').css('background-color', 'transparent');
                }
            });

            $('div[aorid]').droppable({
                tolerance: "pointer",
                over: function (e, ui) {
                    if ($(ui.helper).hasClass('draggableDeployment') || $(ui.helper).hasClass('droppedDeployment')) {
                        if ($(e.target).attr('deploymentid') != $(ui.helper).attr('deploymentid')) {
                            $(e.target).css('background-color', _deploymentColorHighlight);
                        }
                    }
                },
                out: function (e, ui) {
                    if ($(ui.helper).hasClass('draggableDeployment') || $(ui.helper).hasClass('droppedDeployment')) {
                        $('[id$=divCRReportBuilder]').find('div[aorid]:not([deploymentid="' + $(ui.helper).attr('deploymentid') + '"])').css('background-color', _deploymentColor);
                    }
                },
                drop: function (e, ui) {
                    if ($(ui.helper).hasClass('draggableDeployment')) {
                        $(e.target).attr('deploymentid', $(ui.helper).attr('deploymentid'));
                        var deploymentDate = '<span class="droppedDeployment" aorid="' + $(e.target).attr('aorid') + '" deploymentid="' + $(ui.helper).attr('deploymentid') + '" style="background-color:' + _deploymentColor + '; cursor: pointer;">' + $(ui.helper).text().slice(0, $(ui.helper).text().search('  ')) + '</span>';
                        var aorText = $(e.target).find('span').text().slice($(e.target).find('span').text().indexOf('AOR'));
                        $(e.target).find('span').text('');
                        $(e.target).find('span').append(deploymentDate + ' ' + aorText);
                        $('[id$=divCRReportBuilder]').find('div[aorid]').css('background-color', 'transparent');
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(e.target).attr('aorid') + '"]').effect("highlight", { color: _deploymentColorHighlight }, 1500);
                    } else if ($(ui.helper).hasClass('droppedDeployment')) {
                        var deploymentDate = '<span class="droppedDeployment" aorid="' + $(e.target).attr('aorid') + '" deploymentid="' + $(ui.helper).attr('deploymentid') + '" style="background-color:' + _deploymentColor + '; cursor: pointer;">' + $(ui.helper).text() + '</span>';
                        var aorText = $(e.target).find('span').text().slice($(e.target).find('span').text().indexOf('AOR'));
                        $(e.target).find('span').text('');
                        $(e.target).find('span').append(deploymentDate + ' ' + aorText);
                        $('[id$=divCRReportBuilder]').find('div[aorid]').css('background-color', 'transparent');
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(e.target).attr('aorid') + '"]').attr('deploymentid', $(ui.helper).attr('deploymentid'));
                        $('[id$=divCRReportBuilder]').find('span[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').remove();
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').find('span').text($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').find('span').text().slice(1));
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').attr('deploymentid', '0');
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(e.target).attr('aorid') + '"]').effect("highlight", { color: _deploymentColorHighlight }, 1500);
                    }
                    initDragControls();
                    initDropControls();
                }
            });

            $('#tdrightpane').droppable({
                tolerance: "pointer",
                drop: function (e, ui) {
                    var crid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').closest('div[crid]').attr('crid');
                    var workloadallocationid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').closest('div[crid]').attr('workloadallocationid');

                    if ($(ui.helper).attr('crid') != undefined && $(ui.helper).hasClass('droppedCR')) { //Return a CR from the Builder to the CR List
                        $('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"][workloadallocationid="' + $(ui.helper).attr('workloadallocationid') + '"]').remove();
                        $(ui.helper).find('div[aorid]').each(function () { //The CR in the builder has AORs under it, remove them first
                            if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(this).attr('aorid') + '"]').length == 0) {
                                $('#divLoadedAORs').find('span[aorid="' + $(this).attr('aorid') + '"]').css('background-color', 'transparent');
                                $('#spnAORCount').text(parseInt($('#spnAORCount').text()) + 1);
                            }
                        });
                        if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(ui.helper).attr('crid') + '"]').length == 0) {
                            $('#divLoadedCRs').find('span[crid="' + $(ui.helper).attr('crid') + '"]').css('background-color', 'transparent');
                            $('#spnCRCount').text(parseInt($('#spnCRCount').text()) + 1);
                        }
                    }
                    else if ($(ui.helper).attr('aorid') != undefined && $(ui.helper).hasClass('droppedAOR')) {
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"]').remove();
                        $('#divLoadedAORs').find('span[aorid="' + $(ui.helper).attr('aorid') + '"]').css('background-color', 'transparent');
                        $('#spnAORCount').text(parseInt($('#spnAORCount').text()) + 1);
                    }
                    else if ($(ui.helper).attr('deploymentid') != undefined && $(ui.helper).hasClass('droppedDeployment')) {
                        $('[id$=divCRReportBuilder]').find('span[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').remove();
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').find('span').text($('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').find('span').text().slice(1));
                        $('[id$=divCRReportBuilder]').find('div[aorid="' + $(ui.helper).attr('aorid') + '"][deploymentid="' + $(ui.helper).attr('deploymentid') + '"]').attr('deploymentid', '0');
                    }

                    if ($('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRs').find('div[crid]').length == 0) {
                        $('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRsEmpty').show();
                    } else {
                        $('#div' + $(ui.helper).attr('workloadallocation') + 'DropCRsEmpty').hide();
                    }

                    if ($('div[crid="' + crid + '"][workloadallocationid="' + workloadallocationid + '"]').find('div[aorid]').length == 0) {
                        $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').show();
                    } else {
                        $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').hide();
                    }
                }
            });

            $('div[crid], div[aorid]').find('img:first').css('visibility', 'hidden');

            $('div[crid], div[aorid]').hover(function () {
                $(this).find('img:first').css('visibility', 'visible');
            }, function () {
                $(this).find('img:first').css('visibility', 'hidden');
            });
        }

        function removeItem(element) {
            var removable = $(element).closest('div');
            var crid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(removable).attr('aorid') + '"]').closest('div[crid]').attr('crid');
            var workloadallocationid = $('[id$=divCRReportBuilder]').find('div[aorid="' + $(removable).attr('aorid') + '"]').closest('div[crid]').attr('workloadallocationid');

            if ($(removable).attr('crid') != undefined && $(removable).hasClass('droppedCR')) { //Return a CR from the Builder to the CR List
                $('[id$=divCRReportBuilder]').find('div[crid="' + $(removable).attr('crid') + '"][workloadallocationid="' + $(removable).attr('workloadallocationid') + '"]').remove();
                $(removable).find('div[aorid]').each(function () { //The CR in the builder has AORs under it, remove them first
                    if ($('[id$=divCRReportBuilder]').find('div[aorid="' + $(this).attr('aorid') + '"]').length == 0) {
                        $('#divLoadedAORs').find('span[aorid="' + $(this).attr('aorid') + '"]').css('background-color', 'transparent');
                        $('#spnAORCount').text(parseInt($('#spnAORCount').text()) + 1);
                    }
                });
                if ($('[id$=divCRReportBuilder]').find('div[crid="' + $(removable).attr('crid') + '"]').length == 0) {
                    $('#divLoadedCRs').find('span[crid="' + $(removable).attr('crid') + '"]').css('background-color', 'transparent');
                    $('#spnCRCount').text(parseInt($('#spnCRCount').text()) + 1);
                }
            }
            else if ($(removable).attr('aorid') != undefined && $(removable).hasClass('droppedAOR')) {
                $('[id$=divCRReportBuilder]').find('div[aorid="' + $(removable).attr('aorid') + '"]').remove();
                $('#divLoadedAORs').find('span[aorid="' + $(removable).attr('aorid') + '"]').css('background-color', 'transparent');
                $('#spnAORCount').text(parseInt($('#spnAORCount').text()) + 1);
            }

            if ($('#div' + $(removable).attr('workloadallocation') + 'DropCRs').find('div[crid]').length == 0) {
                $('#div' + $(removable).attr('workloadallocation') + 'DropCRsEmpty').show();
            } else {
                $('#div' + $(removable).attr('workloadallocation') + 'DropCRsEmpty').hide();
            }

            if ($('div[crid="' + crid + '"][workloadallocationid="' + workloadallocationid + '"]').find('div[aorid]').length == 0) {
                $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').show();
            } else {
                $('#div' + crid + 'DropAORsEmpty[workloadallocationid="' + workloadallocationid + '"]').hide();
            }
        }

    </script>

    <script id="jsInit" type="text/javascript">

        function initVariables() {
            // we escape the html to not break the html or javascript, but when reading into memory, we unescape it
            _changesMade = false;
            _type = '<%=this.Type%>';
            _pageUrls = new PageURLs();

            _crColor = '#E6FDFF'; 
            _crColorBorder = '#001517';
            _crColorHighlight = '#A3CED2';
            _aorColor = '#E7FFE6'; 
            _aorColorBorder = '#021F00';
            _aorColorHighlight = '#B7E9B4';
            _deploymentColor = '#FFE6E7';
            _deploymentColorBorder = '#260001';
            _deploymentColorHighlight = '#FEC4C5'; 

            _dragItemTextErrorColor = '#999999';
            _dragItemBackgroundErrorColor = '#dddddd';
            _dragItemBorderErrorColor = '#999999';
        }        

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
        }

        function initEvents() {           
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(false); return false; });

            $('[id$=ddlRelease]').on('change', function () { input_changed(); });
            $('[id$=ddlContract]').on('change', function () { input_changed(); });
            $('[id$=chkVisible]').on('change', function () { input_changed(); });

            $('#btnClearDDLs').on('click', function () { btnClearDDLs_click(); });
            $('#tdtoggle').on('click', function () { toggleRightPane(); });
        }

        function initDefaults() {   

        }

        $(document).ready(function () {
            initVariables();
            initDisplay();            
            initEvents();
            initDefaults();

            $('[id$=chkVisible]')[0].checked = '<%=this.Visible%>' == 1 ? true : false;
            if ('<%=this.ReleaseID%>' > 0 && '<%=this.ContractID%>' > 0){
                $('[id$=ddlRelease]').val('<%=this.ReleaseID%>');
                $('[id$=ddlContract]').val('<%=this.ContractID%>');
                input_changed();
            }
        });
    </script>
</asp:Content>