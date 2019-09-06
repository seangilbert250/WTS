﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMTBuilder.aspx.cs" Inherits="RQMTBuilder" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" type="text/css" href="Styles/tooltip.css" />    
</asp:Content>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">RQMT Description</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span><%=this.Title %></span>
</asp:Content>

<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <table style="width: 100%;">
		<tr>
			<td>                
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnClose" value="Back To RQMTs Grid" style="vertical-align: middle;" />
			</td>
		</tr>
	</table>
</asp:Content>

<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>

    <div id="divAddEdit" style="display: none;">
        <table style="width: 100%; border-collapse: collapse;" cellpadding="0px">
            <tr>
                <td valign="top" style="width:100%;padding:3px;"> <!-- left side of page -->
                    <table style="width: 100%; border-collapse: collapse;" cellpadding="5px">                        
                        <tr> <!-- left top -->
                            <td style="width:800px;">
                                <table id="tblDDL" style="border-collapse: collapse;" cellpadding="5px">
                                    <tr>                                        
                                        <td style="text-align:left;font-weight:bold;width:150px">Suite: <span style="color:red">*</span></td>
                                        <td style="text-align:left;font-weight:bold;width:150px">Systems: <span style="color:red">*</span></td>
                                        <td style="text-align:left;font-weight:bold;width:150px">Work Area: <span style="color:red">*</span></td>
                                        <td style="text-align:left;font-weight:bold;width:150px">Purpose: <span style="color:red">*</span></td>                                        
                                        <td style="text-align:left;font-weight:bold;width:150px">Set Name: <span style="color:red">*</span></td>
                                        <td style="text-align:left;white-space:nowrap">&nbsp;</td>
                                    </tr>
                                    <tr style="">
                                        <td style="text-align:left;vertical-align:top;">
                                            <select id="ddlSuite" runat="server" style="background-color:#f5f6ce;width:150px;">
                                            </select>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <div id="divSystemsContainer" style="white-space:nowrap">
                                                <select id="ddlSystem" runat="server" style="background-color:#f5f6ce;width:150px;">
                                                </select>
                                            </div>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <div id="divWorkAreaContainer" style="white-space:nowrap">
                                                <select id="ddlWorkArea" runat="server" style="background-color:#f5f6ce;width:150px;">
                                                </select>
                                            </div>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <div id="divRQMTTypeContainer" style="white-space:nowrap">
                                                <select id="ddlRQMTType" runat="server" style="background-color:#f5f6ce;width:150px;">
                                                </select>
                                            </div>
                                        </td>
                                        <td style="text-align:left;vertical-align:top;">
                                            <input type="text" id="txtRQMTSetName" maxlength="100" style="background-color:#f5f6ce;border:1px solid #a9a9a9;width:150px">
                                        </td>
                                        <td style="text-align:left;vertical-align:top;white-space:nowrap;">
                                            <input type="button" id="btnClearDDLs" value="Clear">&nbsp;&nbsp;
                                            <input type="button" id="btnAddNewRQMTSet" value="Add New Set" style="" onclick="return false;">
                                            <input type="button" id="btnRefreshRQMTs" value="Refresh RQMTs">
                                            <div style="display:inline-block;border:0px;" class="tooltip">
                                                <img src="images/icons/help.png" width="12" height="12">
                                                <div class="tooltiptext tooltip-bottom-noarrow" style="width:400px;left:-100px;text-align:left;">
                                                    <b>Set Name:</b> Allows a RQMT to be defined by a name that makes sense to the user.<br /><br />
                                                    <b>Suite:</b> Filters down the list of Systems available to select for a RQMT Set.<br /><br />
                                                    <b>*Systems:</b> A list of Systems to be selected for a RQMT Set and is required for a RQMT Set.<br /><br />
                                                    <b>*Work Area:</b> A list of Work Areas in the selected system for the RQMT Set.<br /><br />
                                                    <b>*Purpose:</b> A list of the available purposes for the RQMT Set.<br /><br />
                                                    System, Work Area, Purpose, and a RQMT are what is required to define a RQMT Set.<br /><br />
                                                    These values are progressive. As you select each item the page will filter the others. All are
                                                    intended to filter down to a list of RQMT Sets or to Create a New Set using the Add New Set button.
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr> <!-- left bottom -->
                            <td style="width:100%;">
                                <table style="width: 100%; border-collapse: collapse;" cellpadding="0px">
                                    <tr id="rqmtsGridRow">
                                        <td colspan="2" style="padding-top:20px;border-top:1px solid #dddddd;position:relative;">
                                            <div id="divRequiredFields" style="position:absolute;right:0px;top:5px;color:red;">* required to add new set</div>
                                            <div id="divCopiedRQMTs" style="display:none;background-color:#d9edf7;color:#31708f;border:1px solid #559fe0;border-radius:3px;padding:5px;margin-bottom:3px;">
                                            </div>
                                            <div id="divCopiedRQMTsDivider" style="width:100%;display:none;border-bottom:1px solid #dddddd;height:1px;margin-bottom:2px;"></div>
                                            <div id="divRQMTsGrid" style="height:550px;overflow:scroll;">                                                
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td id="tdtoggle" style="width:6px;height:2000px;background-color:#bbbbbb;border:1px solid #aaaaaa;text-align:center;vertical-align:top;cursor:pointer;"> <!-- divider -->
                    <div style="width:6px;height:800px;vertical-align:middle;line-height:800px;margin:0 auto;">
                        <img src="images/pageRollerOver.gif" style="vertical-align:middle;">
                    </div>
                    <div style="width:6px;height:800px;vertical-align:middle;line-height:800px;margin:0 auto;">
                        <img src="images/pageRollerOver.gif" style="vertical-align:middle;">
                    </div>
                    <div style="width:6px;height:800px;vertical-align:middle;line-height:800px;margin:0 auto;">
                        <img src="images/pageRollerOver.gif" style="vertical-align:middle;">
                    </div>
                    <div style="width:6px;height:800px;vertical-align:middle;line-height:800px;margin:0 auto;">
                        <img src="images/pageRollerOver.gif" style="vertical-align:middle;">
                    </div>
                </td>
                <td id="tdrightpane" valign="top" style="width:550px;padding:3px;position:relative;"> <!-- right side of page -->
                    <div id="divTabsContainer" class="" style="width:550px;padding: 0px;overflow:hidden;">                        
                        <div id="divTabsContainerTabRow" style="width:2000px;overflow:hidden;">
			                <ul style="background:none;border:0px;padding:0px;position:relative;">
				                <li><a id="comptab_rqmt" href="#divRQMTsTab">RQMTs</a></li>                        
			                </ul>                            
                            <div style="position:absolute;top:30px;left:505px;width:40px;height:16px;z-index:10000;padding-top:2px;display:none;">
                                <img src="images/icons/arrowleftgrey.png" style="cursor:pointer;opacity:.75" onclick="tabScrollClicked('left');">
                                <img src="images/icons/arrowrightgrey.png" style="cursor:pointer;opacity:.75" onclick="tabScrollClicked('right');">
                            </div>
                        </div>                        
                        <div id="divRQMTsTab" class="tabDiv" comptab="true" comptype="rqmt" style="width:98%;overflow: auto;border:1px solid #aaaaaa;padding:3px;"></div>
                        <div id="divTabsLoading" style="position:absolute;"></div>
                    </div> 
                </td>
            </tr>
        </table>
    </div>

    <div id="divRQMTSetEdit" style="display:none;padding:10px;">
        <div style="width:95%;">
            <div class="gridHeader gridFullBorder" style="font-size:larger;font-weight:bold;overflow:hidden;width:100%;margin-bottom:10px;">Rename RQMT Set:</div>
            <select id="ddlRenameRQMTSet" style="width:40%;" onchange="popupManager.GetPopupByName('EditRQMTSet').Opener.editRQMTSet_ddlRenameRQMTSet_changed()"></select>&nbsp;
            <input type="text" id="txtRenameRQMTSet" style="width:55%;border:1px solid #a9a9a9;margin-top:5px;">

            <br><br><br>

            <div class="gridHeader gridFullBorder" style="font-size:larger;font-weight:bold;overflow:hidden;width:100%;margin-bottom:10px;">Recategorize RQMT Set:</div>

            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td><b>Suite:</b></td>
                    <td><b>System: *</b></td>
                    <td><b>Work Area: *</b></td>
                    <td><b>Purpose: *</b></td>
                </tr>
                <tr>
                    <td style="text-align:left;padding-right:20px;"><select id="ddlRecategorizeSetSuite" style="width:125px;" disabled></select></td>
                    <td style="text-align:left;padding-right:20px;"><select id="ddlRecategorizeSetSystem" style="width:125px;" disabled></select></td>
                    <td style="text-align:left;padding-right:20px;"><select id="ddlRecategorizeSetWorkArea" style="width:125px;" disabled></select></td>
                    <td style="text-align:left;padding-right:20px;"><select id="ddlRecategorizeSetRQMTType" style="width:125px;" disabled></select></td>
                </tr>
            </table>

            <br><br><br>

            <div class="gridHeader gridFullBorder" style="font-size:larger;font-weight:bold;overflow:hidden;width:100%;margin-bottom:10px;">RQMT Set Properties:</div>

            <table cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td><b>Complexity: *</b></td>
                    <td><b>Justification:</b>&nbsp;<span style="font-size:smaller;">(REQUIRED FOR <b>L</b>, <b>XL</b>, <b>XXL</b>)</span><span id="spnRecategorizeSetJustificationNeeded">&nbsp;*</span></td>
                </tr>
                <tr>
                    <td style="text-align:left;vertical-align:top;padding-right:20px;white-space:nowrap;width:1%;"><select id="ddlRecategorizeSetComplexity" style="width:125px;" onchange="popupManager.GetPopupByName('EditRQMTSet').Opener.recategorizeSetComplexityChanged();"></select></td>
                    <td style="text-align:left;vertical-align:top;width:99%;"><textarea id="txtRecategorizeSetJustification" maxlength="1000" style="width:600px;height:50px;border:1px solid #a9a9a9;"></textarea></td>                    
                </tr>
            </table>

            <br><br>
            <div style="text-align:right;width:100%;">
                <input type="button" id="btnRecategorizeSetSave" value="Save">&nbsp;<input type="button" id="btnRecategorizeSetCancel" value="Cancel">
            </div>
        </div>
        <div id="divRecategorizeMessagePlaceHolder"></div>        
    </div>

    <div id="divRQMTDescriptionEdit" style="display:block;padding:10px;">
        <div style="width:95%;">
            <textarea id="txtRQMTDescriptionEdit" style="width:100%;height:150px;" onkeyup="popupManager.GetPopupByName('EditRQMTDescription').Opener._debouncedExistingDescriptionsSearch()"></textarea>
            <br /><br />
            <div style="width:100%;">
                <div style="float:left">
                    <select id="ddlRQMTDescriptionEditType" style="background-color:#f5f6ce;">
                        <%=DescriptionTypeOptions%>
                    </select>                    
                </div>
                <div style="text-align:right;">
                    <input type="button" id="btnRQMTDescriptionEditSave" value="Save">&nbsp;<input type="button" id="btnRQMTDescriptionEditDelete" value="Delete">&nbsp;<input type="button" id="btnRQMTDescriptionEditCancel" value="Cancel">
                </div>
            </div>
        </div>
        <div style="width:95%;height:1px;background-color:#cdcdcd;margin-top:10px;margin-bottom:10px;">
        </div>
        <div style="width:95%;">
            <b>Existing / Similar Descriptions:</b>
            <br />
            <div id="divRQMTDescriptionEditResults" style="margin-top:10px;width:100%;height:225px;overflow-y:auto;overflow-x:hidden;">
                No results found.
            </div>
        </div>
    </div>

    <div id="divRQMTFuncEdit" style="display:none;padding:10px;">
        <div style="width:95%;margin-bottom:15px;position:relative;z-index:10;">
            <div style="display:inline-block;width:100px;vertical-align:top;"><b>Functionality:</b></div>
            <div id="divSelectedFunctionalities" style="display:inline-block;width:350px;vertical-align:top;"></div> <!-- list for previously selected fnc -->
            <div id="divAvailableFunctionalitiesGlass" style="position:absolute;display:none;width:800px;height:500px;left:-100px;top:-100px;z-index:-5;"></div>
            <div id="divAvailableFunctionalities" style="position:absolute;display:none;width:200px;height:250px;overflow-y:scroll;vertical-align:top;border:1px solid #000000;background-color:#ffffff;"> <!-- option popup to edit/change fncs -->
            </div>
            <select id="ddlSelectedFunctionality"></select> <!-- for RQMT sets, we use a DDL and only edit one at a time -->
        </div>
        <div id="divRQMTFuncComplexityRow" style="position:relative;width:95%;margin-bottom:15px;white-space:nowrap;z-index:4;">
            <div style="display:inline-block;vertical-align:top;width:100px;"><b>Complexity:</b></div>
            <select id="ddlRQMTFuncComplexity"></select>&nbsp;<span style="font-size:smaller;color:#bbbbbb">(L, XL, XXL requires Justification)</span>
            <br /><br />
            <div style="display:inline-block;vertical-align:top;width:100px;"><b>Justification:<span id="spanRQMTFuncEditJustificationReq">&nbsp;*</span></b></div>
            <textarea id="txtRQMTFuncJustification" style="width:350px;height:50px;" maxlength="500"></textarea>
        </div>
        <div id="divRQMTFuncEditQuickSelect" style="position:relative;width:95%;margin-bottom:30px;z-index:4;">

        </div>
        <div style="width:95%;text-align:left;position:absolute;height:25px;bottom:60px;z-index:5">
            <input type="button" id="btnRQMTFuncEditSave" value="Save">&nbsp;<input type="button" id="btnRQMTFuncEditDelete" value="Delete">&nbsp;<input type="button" id="btnRQMTFuncEditCancel" value="Cancel">
            <span id="spanRQMTFuncEditDeleteExplanation" style="display:none;font-size:smaller;color:#aaaaaa;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(this item cannot be deleted because it is in use by a RQMT)</span>
        </div>
    </div>

    <div id="divRQMTEdit" style="display:none;padding:10px;"> <!-- THIS DIV IS NO LONGER USED - THIS FUNCTIONALITY HAS BEEN MOVED OVER TO THE RQMT_EDIT.ASPX POPUP PAGE -->
        <div style="width:95%;height:510px;"> <!-- height = 40px less than popup height (550) -->
            <div id="divRQMTEditTabRow" style="display:block;padding-left:3px;position:relative;top:3px;z-index:0;">
                <div id="divRQMTEditTabRQMT" style="background-color:#ffffff;display:table-cell;border:1px solid #a9a9a9;border-radius:3px;padding:5px;cursor:pointer;font-weight:bold;position:relative;" onclick="popupManager.GetPopupByName('EditRQMT').Opener.editRQMTTabClicked('RQMT', 'RQMTSets')">RQMT</div>
                <div id="divRQMTEditTabRQMTSets" style="color:#666666;background-color:#dedede;display:table-cell;border:1px solid #a9a9a9;border-radius:3px;padding:5px;cursor:pointer;font-weight:normal;position:relative;left:2px;top:2px;" onclick="popupManager.GetPopupByName('EditRQMT').Opener.editRQMTTabClicked('RQMTSets', 'RQMT')">RQMT Sets</div>
                <div style="position:absolute;right:0px;top:0px;">
                    <input id="btnRQMTEditSave" type="button" value="Save">&nbsp;
                    <input id="btnRQMTEditCancel" type="button" value="Cancel">
                </div>
            </div>
            <div id="divRQMTEditRQMT" style="display:block;position:relative;width:100%;height:485px;border:1px solid #a9a9a9;background-color:#ffffff;padding:10px;box-sizing: border-box;">
                <textarea id="txtRQMTEdit" style="width:99%;height:300px;"></textarea>
            </div>
            <div id="divRQMTEditRQMTSets" style="display:none;position:relative;width:100%;height:485px;border:1px solid #a9a9a9;background-color:#ffffff;padding:10px;box-sizing: border-box;">
                Loading sets...
            </div>            
        </div>
        <div id="divRQMTEditMessagePlaceHolder"></div>
    </div>
    
    <div id="divRQMTBaseEdit" style="display:none;padding:10px;">
        <div style="width:95%;margin-bottom:5px;">
            <textarea id="txtRQMTBaseEdit" style="width:100%;height:220px;"></textarea>
        </div>
        <div style="width:95%;text-align:right;">
            <input type="button" id="btnRQMTBaseEditSave" value="Save">&nbsp;<input type="button" id="btnRQMTBaseEditDelete" value="Delete">&nbsp;<input type="button" id="btnRQMTBaseEditCancel" value="Cancel">
        </div>
    </div>

    <div id="divRQMTAttrEdit" style="display:none;padding:10px;">
        <div style="width:95%;margin-bottom:5px;">
            <table style="width:95%">                
                <tr>
                    <td style="width:75px;font-weight:bold;">PD2TDR:</td>
                    <td style="text-align:right;">
                        <select id="ddlRQMTAttrEditStage" style="width:150px;">
                            <%=AttributeOptions["stage"]%>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width:75px;font-weight:bold;">Criticality:</td>
                    <td style="text-align:right;">
                        <select id="ddlRQMTAttrEditCriticality" style="width:150px;">
                            <%=AttributeOptions["criticality"]%>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width:75px;font-weight:bold;">Status:</td>
                    <td style="text-align:right;">
                        <select id="ddlRQMTAttrEditStatus" style="width:150px;">
                            <%=AttributeOptions["status"]%>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td style="width:75px;font-weight:bold;">Accepted:</td>
                    <td style="text-align:right;">
                        <input type="radio" id="rdoRQMTAttrEditAccepted" name="rdoRQMTAttrEditAccepted" value="1">Yes&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="radio" id="rdoRQMTAttrEditAccepted" name="rdoRQMTAttrEditAccepted" value="0">No
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:right;">
                        <br />
                        <input type="button" id="btnRQMTAttrEditSave" value="Save">&nbsp;<input type="button" id="btnRQMTAttrEditCancel" value="Cancel">
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="divMessagePlaceHolder"></div>

    <div id="divAttributeSelector" style="position:absolute;display:none;padding:5px;">
    </div>

    <img id="imgSortArrow" src="images/icons/sortarrowright.png" style="position:absolute;top:0px;left:0px;display:none;width:32px;height:31px;">
    <img id="imgSortArrowIndented" src="images/icons/sortarrowindented.png" style="position:absolute;top:0px;left:0px;display:none;width:32px;height:31px;">
    <iframe id="frmDownload" style="display: none;"></iframe>
        
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _debugMode = false;

        var _changesMade;
        var _customListCache; // dictionary containing various cached lists
        var _pageUrls;
        var _cachedRQMTSetList; // stores the list of RQMT Sets for the RQMT edit grid (and refreshes when new sets are added or changed)
        var _systemNames;

        var _dtSystems;
        var _dtWorkAreaSystems;

        var _loadedRQMTs; // the main rqmts on the page are stored here
        var _loadedRQMTSets; // rqmt sets can have their own meta data, so we store that here
        var _loadedRQMTSetViewModes; // tracks which sets are in which view mode (must be outside of the set cache because that cache is refreshed upon save)

        var _compTypes;
        var _displayedSearchResults;
        var _activeComponentType;
        var _rqmtSetNames;
        var _activeRQMTSetRQMTSystemID; // the RQMT item that is currently being edited within a set
        var _activeRQMTSetID; // the RQMT Set that is currently being edited, or added to (in case of the Add RQMT button)
        var _lastRQMTHoveredOver; // contains the row the draggable rqmts are on
        var _hasDefaultValues;
        var _initComplete;
        var _draggingInProcess;

        var _debouncedFilterRQMTSets;
        var _debouncedExistingDescriptionsSearch;

        var _rowAltColor;
        var _dragHoverColor;
        var _checkHighlightColor;
        var _newComponentTextColor;
        var _newComponentBackgroundColor;
        var _newComponentBorderColor;
        var _rowErrorColor;
        var _dragItemTextColor;
        var _dragItemBackgroundColor;
        var _dragItemBorderColor;

        var _dragItemTextErrorColor;
        var _dragItemBackgroundErrorColor;
        var _dragItemBorderErrorColor;

        var _funcSelectedBackgroundColor;

        var _sortArrow;
        var _sortArrowIndented;
        var _sortArrowSrcID;
        var _sortArrowTgtID;
        var _sortArrowTop;
        var _sortArrowIsIndented;
        var _sortingInProgress;

        var _defaultSystemSuiteID;
        var _defaultSystemID;
        var _defaultWorkAreaID;
        var _defaultRQMTTypeID;
        var _defaultRQMTSetID;
        var _defaultRQMTSetName;
        var _defaultRQMTID;

        var _showRQMTAssociations;
        var _suiteOptions;
        var _systemOptions;
        var _workAreaOptions;
        var _rqmtTypeOptions;
        var _complexityOptions;
        var _functionalitySelectOptions;
        var _functionalityCheckBoxOptions;

        var _draggingFunctionalityInProgress;
        var _usageSelectionInProgress;

        var _wideMode;

        var _rqmtSetColspan;
        var _rqmtColspan;
        var _rqmtColspanWideMode;

        var _copiedRQMTSystems;
        var _quickAddWarningEnabled;
        
    </script>

    <script id="jsEvents" type="text/javascript">
        ///////////////////////////////////////////////////////
        // GENERAL FUNCTIONS
        ///////////////////////////////////////////////////////
        function imgRefresh_click() {
            refreshPage();
        }

        function btnClose_click() {
            if (_changesMade) {
                QuestionBox('Confirmation', 'You are going to lose changes, do you want to save?', 'Yes,No', 'close_confirmed', 300, 300, this);
            }
            else {
                close_confirmed('No');
            }
        }        

        function close_confirmed(answer) {
            if (answer == 'Yes') {
                btnSave_click(true);
            }
            else {
                top.setFilterSession(true, false);
               // if (parent.refreshGrid) parent.refreshGrid(0, 1);
                parent.showFrameForGrid('RQMTBUILDER', false);
            }
        }

        function btnSave_click(closeAfterSave) {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    switch ('<%=this.Type %>') {
                        case 'Add':
                            
                        break;
                    }

                    var nRQMTsJSON = '{ "save" : ' + JSON.stringify(_newRQMTSystems) + '}';

                    PageMethods.Save(nRQMTsJSON, function (result) { save_done(result, closeAfterSave); }, on_error);

                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred. ' + e.message);
            }
        }
        
        function save_done(result, closeAfterSave) {
            ShowDimmer(false);

            var blnSaved = false, blnExists = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);
            
            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;                
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                if (parent.refreshGrid) parent.refreshGrid(0,1);

                if (closeAfterSave) {
                    setTimeout(function () { parent.showFrameForGrid('RQMTBUILDER', false) }, 1);
                }

                _changesMade = false;
                updateButtonStatuses();
                refreshRQMTTypesDDL();
                reloadNewRQMTSystems(obj.rqmtsystemidmappings);
            }
            else if (blnExists) {
                MessageBox('RQMT already exists');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function input_change(obj) {            
            $('#btnSave').prop('disabled', !_changesMade);
            $('#btnSaveAndClose').prop('disabled', !_changesMade);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage() {
            window.location.href = 'Loading.aspx?Page=' + window.location.href;
        }

        function validate() {
            var validation = [];

            switch ('<%=this.Type %>') {
                case 'Add':
                    var allRQMTs = true;
                    for (var i = 0; i < _newRQMTSystems.length; i++) {
                        var rqmtsys = _newRQMTSystems[i];
                        rqmtsys.validationPerformed = true;

                        if (_.filter(rqmtsys.dtRQMTs, function (comp) { return comp.deleted == null || !comp.deleted }).length == 0) {
                            allRQMTs = false;
                            markSystemWithError(rqmtsys);
                        }
                    }

                    if (!allRQMTs) {
                        validation.push('Systems must have at least one RQMT.');
                    }

                    break;
            }

            return validation.join('<br>');
        }

        function tabScrollClicked(dir) {
            var scrollGridUL = $('#divTabsContainerTabRow ul');

            var x = dir == 'left' ? 100 : -100;
            var currentX = scrollGridUL.position().left;

            currentX += x;
            if (currentX > 0) currentX = 0;
            if (currentX < -1000) currentX = -1000;

            scrollGridUL.animate({ left: currentX + "px"}, 100 );
        }

        function initCustomLists() {
            var customLists = '<%=WTS.Util.StringUtil.StrongEscape(CustomLists)%>';
            customLists = $.parseJSON(UndoStrongEscape(customLists));

            if (customLists != null) {
                _customListCache['RQMTPriority'] = customLists.RQMTPriority;             
            }
        }

        function imgExport_click() {
            var nURL = 'RQMT_Grid.aspx?GridType=RQMT&MyData=true';

            nURL = editQueryStringValue(nURL, 'CurrentLevel', '1'); // for now, our export only ever has 1 level
            nURL = editQueryStringValue(nURL, 'Filter', '');
            nURL = editQueryStringValue(nURL, 'View', 'System');
            nURL = editQueryStringValue(nURL, 'GridPageIndex', '0');
            nURL = editQueryStringValue(nURL, 'Export', 'true');

            var sets = '';
            for (var i = 0; i < _loadedRQMTSets.length; i++) {
                if (i > 0) sets += ',';
                sets += _loadedRQMTSets[i].RQMTSetID;
            }
            nURL = editQueryStringValue(nURL, 'FilterToSets', sets);

            $('#frmDownload').attr('src', nURL);
        }

        function toggleRightPane() {
            $('#tdrightpane').toggle();

            if ($('#tdrightpane').is(':visible')) {
                $('#tdtoggle').css('width', '6px');
                _wideMode = false;
            }
            else {                
                $('#tdtoggle').css('width', '8px');
                _wideMode = true;
            }

            refreshRQMTs(false);
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

        function updateButtonStatuses() {
            var rqmtSetName = $('[id$=txtRQMTSetName]').val();
            var system = $('[id$=ddlSystem]').val();
            var workArea = $('[id$=ddlWorkArea]').val();
            var rqmtType = $('[id$=ddlRQMTType]').val();
            
            if (system == null || system.length == 0) system = '0';
            if (workArea == null || workArea.length == 0) workArea = '0';
            if (rqmtType == null || rqmtType.length == 0) rqmtType = '0';

            var addNewSetButtonEnabled = true;
            if (system != 0 && workArea != 0 && rqmtType != 0 && rqmtSetName != null && rqmtSetName.length > 0) {
                if (_.find(_loadedRQMTs, function (rqmt) { return rqmt.WTS_SYSTEMID == system && rqmt.WorkAreaID == workArea && rqmt.RQMTTypeID == rqmtType && rqmt.RQMTSetName == rqmtSetName; }) != null) {
                    addNewSetButtonEnabled = false;
                }
            }
            else {
                addNewSetButtonEnabled = false;
            }
            
            $('#btnAddNewRQMTSet').prop('disabled', !addNewSetButtonEnabled);

            for (var i = 0; i < _compTypes.length; i++) {
                var compTypeObj = _compTypes[i];

                var addNewButton = $('#btnaddnewcomp_' + compTypeObj.type);
                var cancelAddButton = $('#btncancelnewcomp_' + compTypeObj.type);
                var clearButton = $('#btnclearnewcomp_' + compTypeObj.type);
                var txtCompSearch = $('#txtcompsearch_' + compTypeObj.type).val();

                addNewButton.prop('disabled', txtCompSearch == null || txtCompSearch.trim().length == 0);

                if (_activeRQMTSetID == 0) {
                    clearButton.show();
                    cancelAddButton.hide();
                }
                else {
                    clearButton.hide();
                    cancelAddButton.show();
                }
            }
        }

        ///////////////////////////////////////////////////////
        // SYSTEM SUITES, SYSTEMS, WORK AREA, AND RQMT TYPE DDLS
        ///////////////////////////////////////////////////////

        function ddlSuite_changed(recatSetMode) { // recatSetMode is true when we are using the recategorize popup ddls for rqmt sets rather than the page ddls
            if (recatSetMode == null) recatSetMode = false;

            var ic = null;

            if (recatSetMode) {
                var thePopup = popupManager.GetPopupByName('EditRQMTSet');
                ic = thePopup.InlineContainer;
            }

            var suiteID = recatSetMode ? $('#ddlRecategorizeSetSuite', ic).val() : $('[id$=ddlSuite]').val();

            if (!recatSetMode) {
                clearActiveRQMTSet();
            }
            
            if (suiteID > '0') {
                var systemOptions = getSystemOptionsForSuite(suiteID);

                loadSuites_done(systemOptions, recatSetMode); // NOTE: WE USED TO DO THIS LOADING VIA AJAX CALL, BUT MOVED IT TO INLINE ON PAGE           
            }
            else {                
                if (recatSetMode) {
                    $('#ddlRecategorizeSetSystem', ic).html('');
                }
                else {
                    $('[id$=ddlSystem]').html('');
                }
            }

            if (!recatSetMode) {
                updateButtonStatuses();
                clearActiveRQMTSet();
            }
        }
        
        function loadSuites_done(systemOptions, recatSetMode) { // recatSetMode is true when we are using the recategorize popup ddls for rqmt sets rather than the page ddls
            if (recatSetMode == null) recatSetMode = false;
            
            var ic = null;

            if (recatSetMode) {
                var thePopup = popupManager.GetPopupByName('EditRQMTSet');
                ic = thePopup.InlineContainer;
            }

            var suiteID = recatSetMode ? $('#ddlRecategorizeSetSuite', ic).val() : $('[id$=ddlSuite]').val();

            if (systemOptions != null && systemOptions.length > 0) {
                var html = '';

                html = '<option value="0"></option>' + systemOptions;

                if (recatSetMode) {
                    $('#ddlRecategorizeSetSystem', ic).html(html);
                }
                else {
                    $('[id$=ddlSystem]').html(html);
                }
            }
            else {
                if (recatSetMode) {
                    $('#ddlRecategorizeSetSystem', ic).html('');
                }
                else {
                    $('[id$=ddlSystem]').html('');
                }
            }

            clearLoadingMessage(recatSetMode ? $(ic).find('#divRecategorizeMessagePlaceHolder') : null);

            if (!recatSetMode) {
                if (!_initComplete && _hasDefaultValues && _defaultSystemID != 0) {
                    $('[id$=ddlSystem]').val(_defaultSystemID);
                }

                refreshWorkAreaDDL();
            }
        }  

        function ddlSystem_changed(recatSetMode) {            
            if (!recatSetMode) {
                clearActiveRQMTSet();
            }

            refreshWorkAreaDDL(recatSetMode);
        }

        function refreshRQMTTypesDDL() {
            PageMethods.GetRequirementTypes(refreshRQMTTypesDDL_done, on_error);
        }

        function refreshRQMTTypesDDL_done(result) {
            var ds = jQuery.parseJSON(result);
            
            if (ds != null && ds.length > 0) {
                var selectedOption = $('[id$=ddlRQMTType]').val();

                var html = '';

                $('[id$=ddlRQMTType] option').remove();

                var lastGroup = null;

                for (var i = 0; i < ds.length; i++) {
                    var row = ds[i];

                    if (row.RQMTType != '') {
                        var group = row.InternalType;

                        if (group != lastGroup) {
                            html += '<option value="0" style="background-color:#ffffff;color:#aaaaaa;" disabled>' + group + '</option>';
                        }

                        lastGroup = group;
                    }

                    html += '<option value="' + row.RQMTTypeID + '">' + row.RQMTType + '</option>';
                }                

                $('[id$=ddlRQMTType]').html(html);
                $('[id$=ddlRQMTType]').val(selectedOption);
            }
        }

        function refreshWorkAreaDDL(recatSetMode) {
            if (recatSetMode == null) recatSetMode = false;

            var ic = null;

            if (recatSetMode) {
                var thePopup = popupManager.GetPopupByName('EditRQMTSet');
                ic = thePopup.InlineContainer;
            }

            var WTS_SYSTEMID = recatSetMode ? $('#ddlRecategorizeSetSystem', ic).val() : $('[id$=ddlSystem]').val();

            var workAreaOptions = getWorkAreaOptionsForSystem(WTS_SYSTEMID);            

            refreshWorkAreaDDL_done(workAreaOptions, WTS_SYSTEMID, recatSetMode);  // NOTE: WE USED TO DO THIS LOADING VIA AJAX CALL, BUT MOVED IT TO INLINE ON PAGE
        }

        function refreshWorkAreaDDL_done(workAreaOptions, WTS_SYSTEMID, recatSetMode) {
            if (recatSetMode == null) recatSetMode = false;

            var ic = null;

            if (recatSetMode) {
                var thePopup = popupManager.GetPopupByName('EditRQMTSet');
                ic = thePopup.InlineContainer;
            }

            var html = '';

            if (recatSetMode) {
                var currentSelection = $('#ddlRecategorizeSetWorkArea', ic).val();
                $('#ddlRecategorizeSetWorkArea', ic).html(workAreaOptions);
                $('#ddlRecategorizeSetWorkArea', ic).val(currentSelection);
            }
            else {
                var currentSelection = $('[id$=ddlWorkArea]').val();

                if (!_initComplete && _hasDefaultValues && _defaultWorkAreaID != 0) {
                    // we'd only be here before init complete is done if we are setting default values for the DDL's
                    currentSelection = _defaultWorkAreaID;
                }

                $('[id$=ddlWorkArea]').html(workAreaOptions);
                $('[id$=ddlWorkArea]').val(currentSelection);

                if (!_initComplete) {
                    if (_defaultRQMTTypeID != 0) {
                        $('[id$=ddlRQMTType]').val(_defaultRQMTTypeID);
                    }

                    if (_defaultRQMTSetName != '') {
                        $('[id$=txtRQMTName]').val(_defaultRQMTSetName);
                    }
                }

                // all ddl's selected
                var allDefaultsSelected = $('#tblDDL').find('select:has(option[value=0]:selected)').length == 0;                
                filterRQMTSets(_initComplete ? 0 : ((!_initComplete && allDefaultsSelected) ? -1 : 0)); // 0 is loading all, -1 is loading all and opening all
            }

            clearLoadingMessage(recatSetMode ? $(ic).find('#divRecategorizeMessagePlaceHolder') : null);            
        }

        function btnClearDDLs_click() {
            $('[id$=txtRQMTSetName]').val('');
            $('[id$=ddlSuite]').val(0);
            $('[id$=ddlSystem]').val(0);
            $('[id$=ddlWorkArea]').val(0);
            $('[id$=ddlRQMTType]').val(0);

            updateButtonStatuses();
        }



        ///////////////////////////////////////////////////////
        // RQMT SETS
        /////////////////////////////////////////////////////// 

        function refreshRQMTs(reload) {
            if (reload == null) reload = true;

            if (reload) {
                filterRQMTSets();
            }
            else {
                filterRQMTSets_done(_loadedRQMTs, 0, true);
            }
        }

        function rqmtSetsDisplayed() {
            return $('#divRQMTsGrid').find('table[id=tblRQMTSet]').length > 0;
        }

        function clearRQMTsGrid() {
            _loadedRQMTs = [];
            _loadedRQMTSets = [];
            $('#divRQMTsGrid').html('No results found.');
        }

        function clearCachedRQMTSetList() {
            _cachedRQMTSetList = null;
        }

        function getRQMTSetGroupName(RQMTSetNameID) {
            var rqmt = _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSetNameID == RQMTSetNameID });

            if (rqmt != null) {
                return rqmt.RQMTSetName;
            }
        }

        function getRQMTSetName(RQMTSetID) {
            var rqmt = _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSetID == RQMTSetID });

            if (rqmt != null) {
                return rqmt.RQMTSetName;
            }

            return null;
        }

        function RQMTExists(RQMTID) {
            return _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTID == RQMTID }) != null;
        }

        function getRQMTFromRQMTSet(RQMTSetID, RQMTID) {
            return _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSetID == RQMTSetID && rqmt.RQMTID == RQMTID });
        }

        function getAllRQMTsForRQMTSet(RQMTSetID) {
            return _.filter(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSetID == RQMTSetID });
        }

        function getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID) {
            return _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == RQMTSet_RQMTSystemID });
        }

        function getRQMTSet(RQMTSetID) {
            return _.find(_loadedRQMTSets, function (set) { return set.RQMTSetID == RQMTSetID });
        }

        function getCheckedRQMTsForRQMTSet(RQMTSetID) {
            var theTable = $('table[rqmtsetrqmttable=1][rqmtsetid=' + RQMTSetID + ']');
            var allRQMTs = theTable.find('input[cbRQMT=1]');
            var checkedRQMTs = theTable.find('input[cbRQMT=1]:checked');

            return checkedRQMTs;
        }

        function getSystemOptionsForSuite(WTS_SystemSuiteID) {
            var str = '';

            for (var i = 0; i < _dtSystems.length; i++) {
                var sys = _dtSystems[i];

                if (sys['WTS_SystemSuiteID'] == WTS_SystemSuiteID) {
                    str += '<option value="' + sys['WTS_SystemID'] + '">' + sys['WTS_SYSTEM'] + '</option>';
                }
            }

            return str;
        }

        function getWorkAreaOptionsForSystem(WTS_SYSTEMID) {
            var str = '';
            
            for (var i = 0; i < _dtWorkAreaSystems.length; i++) {
                var wa = _dtWorkAreaSystems[i];

                if (wa['WTS_SYSTEMID'] == WTS_SYSTEMID) {
                    str += '<option value="' + wa['WorkAreaID'] + '">' + wa['WorkArea'] + '</option>';
                }
            }
            
            return str;
        }

        function isRQMTOnClipboard() {
            return $('#divCopiedRQMTs').is(':visible');
        }

        function filterRQMTSets(RQMTSetID) { // if RQMTSetID is specified, that RQMTSetID will be marked as "open" when the filtering is done; if -1 is passed, ALL RQMTSETS will be opened
            if (RQMTSetID == null) RQMTSetID = 0;

            clearActiveRQMTSet();

            var rqmtSetName = $('[id$=txtRQMTSetName]').val();
            var system = $('[id$=ddlSystem]').val();
            var workArea = $('[id$=ddlWorkArea]').val();
            var rqmtType = $('[id$=ddlRQMTType]').val();
            
            if (system == null || system.length == 0) system = '0';
            if (workArea == null || workArea.length == 0) workArea = '0';
            if (rqmtType == null || rqmtType.length == 0) rqmtType = '0';

            var forceOpenAll = system > 0 && workArea > 0 && rqmtType > 0;

            if (rqmtSetName.length > 0 || system != 0 || workArea != 0 || rqmtType != 0 || (!_initComplete && _defaultRQMTID > 0)) {
                PageMethods.FilterRQMTSets(RQMTSetID, rqmtSetName, system, workArea, rqmtType, function (results) { filterRQMTSets_done(results, RQMTSetID, false, forceOpenAll) }, on_error);

                showLoadingMessage('Refreshing RQMTs...');
            }
            else {
                clearRQMTsGrid();
            }
        }

        function filterRQMTSets_done(results, RQMTSetID, resultsAlreadyParsed, forceOpenAll) { // if RQMTSetID is 0/NULL, we are refreshing the entire page, otherwise we are updating just one table on the page
            if (resultsAlreadyParsed == null) resultsAlreadyParsed = false;
            if (RQMTSetID == null) RQMTSetID = 0;

            var dtrqmt;
            var dtrqmtset;

            if (resultsAlreadyParsed) {
                dtrqmt = results;

                if (RQMTSetID > 0) {
                    dtrqmtset = [];
                    dtrqmtset.push(_.find(_loadedRQMTSets, function (set) { return set.RQMTSetID == RQMTSetID }));
                }
                else {
                    dtrqmtset = _loadedRQMTSets;
                }
            }
            else {
                // when results from back from the server, they come back as a data set containing the following tables:
                // [0] = RQMT
                // [1] = DESC
                // [2] = FUNC
                // [3] = DEFECT
                // [4] = RQMTSETTASK

                var ds = $.parseJSON(results);
                dtrqmt = ds['RQMT'];
                dtdesc = ds['DESC'];
                dtfunc = ds['FUNC'];
                dtdefect = ds['DEFECT'];
                dtrqtmsettask = ds['RQMTSETTASK'];
                
                // parse the sets and create the set cache, and intialize base rqmt objects (arrays)
                dtrqmtset = [];

                for (var i = 0; i < dtrqmt.length; i++) {
                    var rqmt = dtrqmt[i];

                    rqmt.RQMTDescriptionArray = [];
                    rqmt.Functionalities = [];
                    rqmt.Defects = [];

                    var setID = rqmt.RQMTSetID;

                    var set = _.find(dtrqmtset, function (s) { return s.RQMTSetID == setID });
                    if (set == null) {
                        set = {};
                        set.Functionalities = [];
                        set.Tasks = [];

                        set.RQMTSetID = setID;
                        set.WorkAreaID = rqmt.WorkAreadID;
                        set.WorkArea = rqmt.WorkArea;
                        set.WTS_SYSTEMID = rqmt.WTS_SYSTEMID;
                        set.WTS_SYSTEM = rqmt.WTS_SYSTEM;
                        set.WTS_SYSTEM_SUITEID = rqmt.WTS_SYSTEM_SUITEID;
                        set.WTS_SYSTEM_SUITE = rqmt.WTS_SYSTEM_SUITE;
                        set.RQMTSetNameID = rqmt.RQMTSetNameID;
                        set.RQMTSetName = rqmt.RQMTSetName;
                        set.RQMTSetTypeID = rqmt.RQMTSetTypeID;
                        set.RQMTSetType = rqmt.RQMTSetType;

                        // we are loading a new set, but just in case it was already visible in the UI, we try to carry over a few properties such as the quickaddenabled property
                        var previouslyLoadedSet = getRQMTSet(setID);
                        if (previouslyLoadedSet != null) {
                            set.QuickAddEnabled = previouslyLoadedSet.QuickAddEnabled;
                        }
                        else {
                            set.QuickAddEnabled = true;
                        }

                        dtrqmtset.push(set);
                    }
                }

                for (var i = 0; i < dtdesc.length; i++) {
                    var desc = dtdesc[i];
                    var RQMTSystemID = desc.RQMTSystemID;

                    var matchingRQMTSystems = _.filter(dtrqmt, function (r) { return r.RQMTSystemID == RQMTSystemID });
                    for (var x = 0; matchingRQMTSystems != null && x < matchingRQMTSystems.length; x++) {
                        var rqmt = matchingRQMTSystems[x];

                        var matchingRQMTDescription = _.find(rqmt.RQMTDescriptionArray, function (d) { return d.RQMTDescriptionID == desc.RQMTDescriptionID });

                        if (matchingRQMTDescription != null) {
                            if (desc.RQMTDescriptionAttachmentID != null && desc.RQMTDescriptionAttachmentID > 0) {
                                var attachment = {};
                                attachment.RQMTDescriptionAttachmentID = desc.RQMTDescriptionAttachmentID;
                                attachment.AttachmentId = desc.AttachmentId;
                                attachment.FileName = desc.FileName;

                                matchingRQMTDescription.Attachments.push(attachment);
                            }
                        }
                        else {
                            desc.Attachments = [];

                            if (desc.RQMTDescriptionAttachmentID != null && desc.RQMTDescriptionAttachmentID > 0) {
                                var attachment = {};
                                attachment.RQMTDescriptionAttachmentID = desc.RQMTDescriptionAttachmentID;
                                attachment.AttachmentId = desc.AttachmentId;
                                attachment.FileName = desc.FileName;

                                desc.Attachments.push(attachment);
                            }

                            rqmt.RQMTDescriptionArray.push(desc);
                        }
                    }
                }

                for (var i = 0; i < dtfunc.length; i++) {
                    var func = dtfunc[i];

                    var setID = func.RQMTSetID;
                    var RQMTSetFunctionalityID = func.RQMTSetFunctionalityID;
                    var RQMTSet_RQMTSystemID = func.RQMTSet_RQMTSystemID;

                    var set = _.find(dtrqmtset, function (s) { return s.RQMTSetID == setID });
                    if (set != null) {
                        if (set.Functionalities == null) set.Functionalities = [];
                        
                        if (_.find(set.Functionalities, function (f) { return f.RQMTSetFunctionalityID == RQMTSetFunctionalityID }) == null) {
                            var setfunc = {};

                            setfunc.RQMTSetFunctionalityID = RQMTSetFunctionalityID;
                            setfunc.FunctionalityID = func.FunctionalityID;
                            setfunc.Functionality = func.Functionality;
                            setfunc.RQMTComplexityID = func.RQMTComplexityID;
                            setfunc.RQMTComplexity = func.RQMTComplexity;
                            setfunc.Points = func.Points; 
                            setfunc.Justification = func.Justification;

                            set.Functionalities.push(setfunc);
                        }                       
                    }

                    if (RQMTSet_RQMTSystemID != null) {                        
                        var rqmt = _.find(dtrqmt, function (r) { return r.RQMTSet_RQMTSystemID == RQMTSet_RQMTSystemID });
                        
                        if (_.find(rqmt.Functionalities, function (f) { return f.RQMTSetFunctionalityID == RQMTSetFunctionalityID }) == null) {
                            var rqmtfunc = {};

                            rqmtfunc.RQMTSet_RQMTSystemID = RQMTSet_RQMTSystemID;
                            rqmtfunc.RQMTSetFunctionalityID = RQMTSetFunctionalityID;

                            rqmt.Functionalities.push(rqmtfunc);
                        }
                    }
                }

                for (var i = 0; i < dtdefect.length; i++) {
                    var defect = dtdefect[i];
                    var RQMTSystemID = defect.RQMTSystemID;

                    var matchingRQMTSystems = _.filter(dtrqmt, function (r) { return r.RQMTSystemID == RQMTSystemID });
                    for (var x = 0; matchingRQMTSystems != null && x < matchingRQMTSystems.length; x++) {
                        var rqmt = matchingRQMTSystems[x];

                        rqmt.Defects.push(defect);
                    }
                }

                for (var i = 0; i < dtrqtmsettask.length; i++) {
                    var task = dtrqtmsettask[i];

                    var setID = task.RQMTSetID;

                    var set = _.find(dtrqmtset, function (s) { return s.RQMTSetID == setID });

                    set.Tasks.push(task);
                }
            }
            
            if (RQMTSetID > 0) {
                // we refreshed just one set, so remove the old values for the set, and add in the newer values
                _loadedRQMTs = _.reject(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSetID == RQMTSetID });
                _loadedRQMTSets = _.reject(_loadedRQMTSets, function (set) { return set.RQMTSetID == RQMTSetID });

                if (dtrqmt != null && dtrqmt.length > 0) {
                    _loadedRQMTs = _loadedRQMTs.concat(dtrqmt);
                }

                if (dtrqmtset != null && dtrqmtset.length > 0) {
                    _loadedRQMTSets = _loadedRQMTSets.concat(dtrqmtset);
                }
            }
            else {
                _loadedRQMTs = dtrqmt;
                _loadedRQMTSets = dtrqmtset;
            }            
            
            if (dtrqmt != null && dtrqmt.length > 0) {
                var lastRQMTSetGroupName = '';
                var lastRQMTSetID = -1;
                var lastGroupHadSet = false;
                var setCount = 0;
                var lastRQMTSetNameID = -1;

                var html = '';

                if (RQMTSetID <= 0) { // 0 is loading all, -1 is loading all and opening all
                    // during initial page load, we might be coming in with a specific RQMTID from the grid; in this case, we filter out any sets from the dtrqmt that do not contain the RQMTID
                    if (!_initComplete && _defaultRQMTID > 0) {
                        var setsContainingRQMTID = _.filter(dtrqmt, function (d) { return d.RQMTID == _defaultRQMTID });
                        setsContainingRQMTID = _.map(setsContainingRQMTID, function (d) { return d.RQMTSetID });

                        // keep all rows where the rqmtsetid is in the setsContainingRQMTID array
                        dtrqmt = _.filter(dtrqmt, function (row) { return setsContainingRQMTID.indexOf(row.RQMTSetID) != -1 });
                    }

                    // we start full page updates by building a list of sets that are already visible so that as we rebuild the HTML we can preserve the open/closed states of items
                    var openSets = [];    
                    var openSetRows = $('tr[id=trRQMTSet]:visible');                                         
                    for (var i = 0; i < openSetRows.length; i++) {
                        openSets.push($(openSetRows[i]).attr('rqmtsetid'));
                    }

                    // similar to RQMTSets, we try to preserve view states; although groups are defaulted to open (unlike RQMT Sets)
                    var closedGroups = [];
                    var closedRQMTSetNameGroups = $('div[id=divrqmtsetnamegroup]:hidden');                    
                    for (var i = 0; i < closedRQMTSetNameGroups.length; i++) {
                        closedGroups.push($(closedRQMTSetNameGroups[i]).attr('rqmtsetnameid'));
                    }     

                    var setsWithOpenTasks = [];
                    var openSetTaskRows = $('tr[id=trRQMTSetTask]:visible');
                    for (var i = 0; i < openSetTaskRows.length; i++) {
                        setsWithOpenTasks.push($(openSetTaskRows[i]).attr('rqmtsetid'));
                    }

                    for (var i = 0; i < dtrqmt.length; i++) {
                        var row = dtrqmt[i];

                        if (row.RQMTSetNameID != lastRQMTSetNameID) {
                            html += getHTMLForRQMTSetNameGroup(row, _.filter(dtrqmt, function (r) { return row.RQMTSetNameID == r.RQMTSetNameID }), closedGroups, openSets, setsWithOpenTasks, RQMTSetID == -1 || forceOpenAll);
                        }

                        lastRQMTSetNameID = row.RQMTSetNameID;
                    }

                    var prevFuncSetSelections = $('[id=divrqmtsetfunctionality][rqmtsetid][funcselected=1]');

                    $('#divRQMTsGrid').html(html);

                    for (var f = 0; f < prevFuncSetSelections.length; f++) {
                        var prevSelection = $(prevFuncSetSelections[f]);
                        var rsid = prevSelection.attr('rqmtsetid');
                        
                        var newSelection = $('[id=divrqmtsetfunctionality][rqmtsetid=' + rsid + '][rqmtsetfunctionalityid=' + prevSelection.attr('rqmtsetfunctionalityid') + ']');
                        newSelection.attr('funcselected', '1');
                        newSelection.css('background-color', _funcSelectedBackgroundColor);

                        $('[id=divrqmtsetselectedfunctionalitieslabel][rqmtsetid=' + rsid + ']').css('display', 'none');
                        $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + rsid + ']').css('display', 'inline-block');
                    }
                }
                else {
                    var setRows = _.filter(dtrqmt, function (r) { return r.RQMTSetID == RQMTSetID });

                    // preserve highlighted set-level functionalities
                    var prevFuncSetSelections = $('[id=divrqmtsetfunctionality][rqmtsetid=' + RQMTSetID + '][funcselected=1]');
                    var tasksOpen = $('tr[id=trRQMTSetTask][rqmtsetid=' + RQMTSetID + ']').is(':visible');                    

                    if (setRows.length > 0) {
                        html = getHTMLForRQMTSet(setRows[0], dtrqmt, $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + RQMTSetID + ']').attr('altrow') == 'true', true, tasksOpen);

                        $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + RQMTSetID + ']').remove();                        
                        $('#divRQMTsGrid').find('tr[id=trRQMTSetTask][rqmtsetid=' + RQMTSetID + ']').remove();
                        $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + RQMTSetID + ']').replaceWith(html);
                    }
                    else {
                        $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + RQMTSetID + '],tr[id=trRQMTSet][rqmtsetid=' + RQMTSetID + ']').remove();
                        // todo: recolor all RQMT SET rows (alt row coloring)
                    }

                    for (var f = 0; f < prevFuncSetSelections.length; f++) {
                        var prevSelection = $(prevFuncSetSelections[f]);

                        var newSelection = $('[id=divrqmtsetfunctionality][rqmtsetid=' + RQMTSetID + '][rqmtsetfunctionalityid=' + prevSelection.attr('rqmtsetfunctionalityid') + ']');
                        newSelection.attr('funcselected', '1');
                        newSelection.css('background-color', _funcSelectedBackgroundColor);

                        $('[id=divrqmtsetselectedfunctionalitieslabel][rqmtsetid=' + RQMTSetID + ']').css('display', 'none');
                        $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + RQMTSetID + ']').css('display', 'inline-block');
                    }
                }
            }
            else {
                if (RQMTSetID <= 0) {
                    clearRQMTsGrid();
                }
                else {              
                    $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + RQMTSetID + '],tr[id=trRQMTSet][rqmtsetid=' + RQMTSetID + ']').remove();
                    // todo: recolor all RQMT SET rows (alt row coloring)
                }
            }

            // dropping of components (from right side of page) onto the grid
            var doesSetExist = $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow]').length > 0;            
            if (doesSetExist) {
                $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow],tr[id=trRQMTSet]').droppable({
                    over: function (e, ui) {
                        if ($(ui.helper).attr('rqmtnametd') == 'true' && $(this).attr('rqmtsetid') != $(ui.helper).attr('rqmtsetid')) {
                            $(ui.helper).css('color', _dragItemTextErrorColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                            _sortArrow.hide();
                            return;
                        }

                        var tgtRQMTSetID = $(this).attr('rqmtsetid');
                        
                        var srcFKID = $(ui.helper).attr('compfkid');
                        var existingRQMT = getRQMTFromRQMTSet(tgtRQMTSetID, srcFKID);

                        if (existingRQMT != null) {
                            $(ui.helper).css('color', _dragItemTextErrorColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                        }
                        else {
                            $(ui.helper).css('color', _dragItemTextColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);
                        }

                        var highlightColor = existingRQMT != null ? _rowErrorColor : _dragHoverColor;

                        if ($(this).attr('id') == 'trRQMTSetHeaderRow') {                                
                            $(this).find('td').css('background-color', highlightColor);
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', highlightColor);
                        }
                        else {
                            $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + tgtRQMTSetID + ']').find('td').css('background-color', highlightColor);
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', highlightColor);                            
                        }                      
                    },
                    out: function (e, ui) {
                        if ($(ui.helper).attr('rqmtnametd') == 'true') {
                            $(ui.helper).css('color', _dragItemTextErrorColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                            _sortArrow.hide();
                            return;
                        }

                        var tgtRQMTSetID = $(this).attr('rqmtsetid');

                        if ($(this).attr('id') == 'trRQMTSetHeaderRow') {                                
                            $(this).find('td').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : $('[id=trRQMTSetHeaderRow][rqmtsetid=' + tgtRQMTSetID + ']').attr('altrow') == 'true' ? _rowAltColor : '#ffffff');
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : '#ffffff');
                        }
                        else {
                            $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + tgtRQMTSetID + ']').find('td').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : $('[id=trRQMTSetHeaderRow][rqmtsetid=' + _activeRQMTSetID + ']').attr('altrow') == 'true' ? _rowAltColor : '#ffffff');
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : '#ffffff');
                            
                        } 

                        $(ui.helper).css('color', _dragItemTextColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);

                        //$(this).children('td[name=tdRQMTSystemCheckCell]').css('background-color', isChecked ? _checkHighlightColor : '');
                    },
                    drop: function (e, ui) {   
                        var srcCompType = $(ui.helper).attr('comptype');
                        var srcCompID = $(ui.helper).attr('compid');
                        var srcFKID = $(ui.helper).attr('compfkid');
                        var tgtRQMTSetID = $(this).attr('rqmtsetid');
                        
                        if ($(ui.helper).attr('name') == 'divsearchresultcomp') {
                            componentDroppedInRQMTSet(tgtRQMTSetID, srcCompType, srcCompID, srcFKID);
                        }                        

                        if ($(this).attr('id') == 'trRQMTSetHeaderRow') {                                
                            $(this).find('td').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : $('[id=trRQMTSetHeaderRow][rqmtsetid=' + tgtRQMTSetID + ']').attr('altrow') == 'true' ? _rowAltColor : '#ffffff');
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : '#ffffff');
                        }
                        else {
                            $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow][rqmtsetid=' + tgtRQMTSetID + ']').find('td').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : $('[id=trRQMTSetHeaderRow][rqmtsetid=' + _activeRQMTSetID + ']').attr('altrow') == 'true' ? _rowAltColor : '#ffffff');
                            $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + tgtRQMTSetID + ']').find('td:first').css('background-color', tgtRQMTSetID == _activeRQMTSetID ? _activeRQMTSetBackground : '#ffffff');
                            
                        } 
 
                    }
                });

                // dragging to resort and indent
                $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true]').draggable({
                    helper: 'clone',
                    revert: 'invalid',
                    appendTo: 'body',
                    start: function (e, ui) {
                        if (_loadedRQMTSetViewModes[$(this).attr('rqmtsetid')] == 'functionality') {
                            return false;
                        }

                        var groupMoveMode = e.ctrlKey;

                        var isChildRQMT = $(this).attr('parentrqmtsetrqmtsystemid') > 0;                        

                        if (groupMoveMode) {
                            if (isChildRQMT) {
                                _sortArrowSrcID = $(this).attr('parentrqmtsetrqmtsystemid');
                            }
                            else {
                                _sortArrowSrcID = $(this).attr('rqmtsetrqmtsystemid');
                            }
                        }
                        else {
                            _sortArrowSrcID = $(this).attr('rqmtsetrqmtsystemid');
                            $(this).css('color', '#bbbbbb');
                        }

                        _lastRQMTHoveredOver = null;                        
                        _sortArrowTgtID = -1;
                        _sortArrowTop = false;
                        _sortArrowIsIndented = false;
                        _sortingInProgress = true;
                        _draggingInProcess = true;                        
                        $(ui.helper).css('font-weight', 'bold');
                        $(ui.helper).css('color', _dragItemTextColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);
                        $(ui.helper).css('width', '150px');
                        $(ui.helper).css('white-space', 'nowrap');

                        var txt = '';

                        if (groupMoveMode) {
                            var parentRQMT = _.find(_loadedRQMTs, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == _sortArrowSrcID }); // srcid will contain parent id
                            $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true][rqmtsetrqmtsystemid=' + _sortArrowSrcID + ']').css('color', '#bbbbbb');

                            txt = parentRQMT.RQMT.trim();
                            if (txt.length > 20) txt = txt.substring(0, 20).trim() + '...';

                            var childRQMTArr = _.filter(_loadedRQMTs, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == _sortArrowSrcID });

                            if (childRQMTArr != null && childRQMTArr.length > 0) {
                                for (var i = 0; i < childRQMTArr.length; i++) {
                                    if (i < 5) {
                                        var childTxt = childRQMTArr[i].RQMT;
                                        if (childTxt.length > 20) childTxt = childTxt.substring(0, 20).trim() + '...';

                                        txt += '<br />&nbsp;&nbsp;' + childTxt;
                                    }
                                    else if (i == 5) {
                                        txt += '<br />&nbsp;&nbsp;+' + (childRQMTArr.length - 5) + ' more...';
                                    }

                                    $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true][rqmtsetrqmtsystemid=' + childRQMTArr[i].RQMTSet_RQMTSystemID + ']').css('color', '#bbbbbb');
                                }
                            }
                        }
                        else {
                            txt = $(ui.helper).text().trim().replace('(CLICK TO TOGGLE INDENT)', '');
                            if (txt.length > 20) txt = txt.substring(0, 20).trim() + '...';
                        }

                        $(ui.helper).html(txt);
                    },
                    stop: function (e, ui) {                        
                        var groupMoveMode = e.ctrlKey;
                        var isChildRQMT = $(this).attr('parentrqmtsetrqmtsystemid') > 0; 
                        var parentSrc = isChildRQMT ? $(this).attr('parentrqmtsetrqmtsystemid') : $(this).attr('rqmtsetrqmtsystemid'); // the _sortArrowSrcID is cleared by the drop call, which happens before the stop call
                        
                        $(this).css('color', '');

                        if (groupMoveMode) {                            
                            $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true][rqmtsetrqmtsystemid=' + parentSrc + ']').css('color', '');
                            var childRQMTArr = _.filter(_loadedRQMTs, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == parentSrc });

                            if (childRQMTArr != null && childRQMTArr.length > 0) {
                                for (var i = 0; i < childRQMTArr.length; i++) {
                                    $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true][rqmtsetrqmtsystemid=' + childRQMTArr[i].RQMTSet_RQMTSystemID + ']').css('color', '');
                                }
                            }
                        }

                        _sortArrow.hide();
                        _sortArrowIndented.hide();

                        clearActiveRQMTSet($(this).attr('rqmtsetid'));

                        _sortArrowSrcID = -1;
                        _sortArrowTgtID = -1;
                        _sortArrowTop = false;
                        _sortArrowIsIndented = false;
                        _sortingInProgress = false;
                        setTimeout(function () { _draggingInProcess = false; }, 100); // _draggingInProcess allows us to prevent unwanted click events from firing when a drop occurs or ends; we use the timer to make sure the drag value is still on until after those other functions have evaluated
                    },
                    drag: function (e, ui) {
                        if (_lastRQMTHoveredOver != null) {
                            var setViewMode = _loadedRQMTSetViewModes[$(this).attr('rqmtsetid')];   
                            if (setViewMode == null) setViewMode = 'normal';

                            if (setViewMode == 'functionality') {
                                return false;
                            }

                            var groupMoveMode = e.ctrlKey;

                            // we are hovering over an illegal spot (the previous hover spot would be green if we were hovering correctly), so hide the sortarrow
                            // NOTE: RIGHT NOW, THIS IS FIRING WHEN THE DRAGGED TD GOES MORE THAN 50% OUT OF TD COLUMN, WHICH IS EASY TO DO WHEN THERE ARE MANY COLUMNS ON THE PAGE WITH SMALL WIDTHS
                            if ($(_lastRQMTHoveredOver).css('color') != 'rgb(0, 128, 0)') {
                                _sortArrow.hide();
                                _sortArrowIndented.hide();   
                                $(ui.helper).css('color', 'gray');
                                $(ui.helper).css('border-color', 'gray');
                                return;
                            }                            

                            // lastrqmthovered values are page absolute, whereas ui.position.top is not

                            var scrollTop = $('#divRQMTsGrid').scrollTop();                            

                            var diffFromTop = (ui.position.top + scrollTop) - _lastRQMTHoveredOver.y;
                            var diffFromLeft = ui.position.left - _lastRQMTHoveredOver.x;

                            //console.log('ui.position.top:' + ui.position.top + '/' + _lastRQMTHoveredOver.y + ' diff:' + diffFromTop + ' scrolltop:' + scrollTop);
                            
                            if (diffFromTop > (_lastRQMTHoveredOver.half - 8)) { // bottom half
                                _sortArrowTop = false;

                                if (diffFromLeft > 70 && false) { // indent [INDENT IS DISABLED IN FAVOR OF SINGLE CLICK ON ROW]
                                    _sortArrowIsIndented = true;
                                    _sortArrow.css('left', (_lastRQMTHoveredOver.x + 8) + 'px');                                    
                                }
                                else { // sort
                                    _sortArrowIsIndented = false;
                                    _sortArrow.css('left', (_lastRQMTHoveredOver.x + -30 + 8) + 'px');
                                }

                                var y = ((_lastRQMTHoveredOver.y + _lastRQMTHoveredOver.ht) - 11 - scrollTop); // top of current cell + height of current cell;  the -11 accounts for the arrow image height (we need about 1/2 height offset); scroll top accounts for the div scrolling

                                // if the rqmt has children, put the arrow at the bottom of those children instead of the bottom of the parent rqmt
                                // only move arrow to bottom of children in 'normal' view mode
                                if (groupMoveMode && setViewMode == 'normal') {
                                    var childRQMTArr = _.filter(_loadedRQMTs, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == _sortArrowTgtID });

                                    for (var i = 0; childRQMTArr != null && i < childRQMTArr.length; i++) {
                                        var childRQMT = childRQMTArr[i];
                                        var childTD = $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true][rqmtsetrqmtsystemid=' + childRQMT.RQMTSet_RQMTSystemID + ']');

                                        y += childTD.height() + 4; // account for 2px padding on top and bottom
                                        if (i == 0) y += 4; // we do a one-time bump of 4... not sure why, but the childTD's always end up 4 short
                                    }
                                }

                                _sortArrow.css('top', y + 'px');
                                //console.log('BOTTOM HALF: ' + _lastRQMTHoveredOver.y + '/' + _lastRQMTHoveredOver.ht + '/' + scrollTop + '/' + _sortArrow.css('top'));
                            }
                            else { // top half
                                _sortArrowTop = true;

                                if (diffFromLeft > 70 && false) { // indent [INDENT IS DISABLED IN FAVOR OF SINGLE CLICK ON ROW]
                                    _sortArrowIsIndented = true;
                                    _sortArrow.css('left', (_lastRQMTHoveredOver.x + 8) + 'px');
                                }
                                else { // sort
                                    _sortArrowIsIndented = false;
                                    _sortArrow.css('left', (_lastRQMTHoveredOver.x + -30 + 8) + 'px');
                                }

                                _sortArrow.css('top', (_lastRQMTHoveredOver.y - 13 - scrollTop) + 'px');                                
                               // console.log('TOP HALF: ' + _lastRQMTHoveredOver.y + '/' + _lastRQMTHoveredOver.ht + '/' + scrollTop + '/' + _sortArrow.css('top'));
                            }                            
                            
                            _sortArrowIndented.css('left', _sortArrow.css('left'));
                            _sortArrowIndented.css('top', _sortArrow.css('top'));

                            if (_sortArrowIsIndented) {
                                _sortArrow.hide();
                                _sortArrowIndented.show();
                            }
                            else {
                                _sortArrowIndented.hide();
                                _sortArrow.show();
                            }
                        }
                    },
                    distance: 3,
                    delay: 100
                });

                // dropping after resorting / indenting
                $('#divRQMTsGrid').find('td[id=tdRQMTName][rqmtnametd=true]').droppable({
                    over: function (e, ui) {
                        if ($(ui.helper).attr('name') == 'divsearchresultcomp' || $(ui.helper).attr('id') == 'divrqmtsetfunctionality') {
                            return;
                        }

                        var groupMoveMode = e.ctrlKey;

                        var draggedRQMTSetID = $(ui.helper).attr('rqmtsetid');
                        var tgtRQMTSetID = $(this).attr('rqmtsetid');                        
                        var isDroppableRow = $(ui.helper).attr('rqmtnametd') == 'true';
                        var parentRQMTSet_RQMTSystemID = $(this).attr('parentrqmtsetrqmtsystemid');
                        var isChildRQMT = parentRQMTSet_RQMTSystemID > 0;

                        if (draggedRQMTSetID != tgtRQMTSetID || !isDroppableRow || (groupMoveMode && isChildRQMT)) {                            
                            $(ui.helper).css('color', _dragItemTextErrorColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                            _sortArrow.hide();
                            _sortArrowIndented.hide();
                            _lastRQMTHoveredOver = null;   
                        }
                        else {
                            $(ui.helper).css('color', _dragItemTextColor);
                            $(ui.helper).css('background-color', _dragItemBackgroundColor);
                            $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);
                            _sortArrow.hide();
                            _sortArrowIndented.hide();
                            _sortArrow.appendTo($(this).closest('body'));    
                            _sortArrowIndented.appendTo($(this).closest('body'));    
                            $(this).css('color', 'green');

                            _lastRQMTHoveredOver = this;
                            _lastRQMTHoveredOver.ht = $(this).height() + 4; // account for 2px padding on top and bottom
                            _lastRQMTHoveredOver.half = _lastRQMTHoveredOver.ht / 2 + (groupMoveMode ? -10 : 0);
                            
                            _lastRQMTHoveredOver.y = getAbsoluteTop($(this)[0]); // gets y position from top of ENTIRE page (not just the scroll window)
                            _lastRQMTHoveredOver.x = $(this).position().left;   
                            
                            _sortArrowTgtID = $(this).attr('rqmtsetrqmtsystemid');
                        }
                    },
                    out: function (e, ui) {
                        var groupMoveMode = e.ctrlKey;

                        if (groupMoveMode) {
                            if ($(this).attr('parentrqmtsetrqmtsystemid') == _sortArrowSrcID) {
                                
                            }
                            else {
                                $(this).css('color', '');
                            }
                        }
                        else {
                            $(this).css('color', '');
                        }                        
                    },
                    drop: function (e, ui) {
                        if ($(ui.helper).attr('rqmtnametd') == 'true') {                            
                            var tgtRQMTSetID = $(this).attr('rqmtsetid');
                            $(this).css('color', '');
                            
                            if (_debugMode) {
                                console.log(_sortArrowSrcID + ' => ' + _sortArrowTgtID + ' (TOP=' + _sortArrowTop + ' INDENT=' + _sortArrowIsIndented + ')');
                            }

                            var groupMoveMode = e.ctrlKey;

                            var parentRQMTSet_RQMTSystemID = $(this).attr('parentrqmtsetrqmtsystemid');
                            var isChildRQMT = parentRQMTSet_RQMTSystemID > 0;

                            if (groupMoveMode && isChildRQMT) {                                
                                return false;
                            }

                            if (_sortArrowSrcID != -1 && _sortArrowTgtID != -1 && _sortArrowSrcID != _sortArrowTgtID && _loadedRQMTSetViewModes[tgtRQMTSetID] != 'functionality') {
                                // the drag/drop feature needs to finish the drop before we rewrite the dom, so we use a timeout so the drop operation can complete
                                // before we do the reordering functionality (we are currently updating the dom in javascript only; but if we use an ajax call to save to database
                                // first the settimeout won't be necessary since it will be async by default)
                                // NOTE: because we reset the sort vars at end of the drop function, we need to copy off the values we are going to use because they will all by nulled out by the time
                                // the timeout triggers;
                                var p1 = tgtRQMTSetID;
                                var p2 = _sortArrowSrcID;
                                var p3 = _sortArrowTgtID;
                                var p4 = _sortArrowTop;
                                var p5 = _sortArrowIsIndented;
                                var p6 = _loadedRQMTSetViewModes[tgtRQMTSetID] == 'normalcollapsed' || e.ctrlKey;
                                setTimeout(function () { RQMTSetReOrdered(p1, p2, p3, p4, p5, p6); }, 100);
                            }

                            _sortArrowSrcID = -1;
                            _sortArrowTgtID = -1;
                            _sortArrowTop = false;
                            _sortArrowIsIndented = false;
                            _sortingInProgress = false;
                        }
                    }
                });

                // dragging from set functionality divs
                $('#divRQMTsGrid').find('div[id=divrqmtsetfunctionality]').draggable({
                    helper: 'clone',
                    revert: 'invalid',
                    appendTo: 'body',
                    start: function (e, ui) {
                        _draggingInProcess = true;
                        _draggingFunctionalityInProgress = false;
                        $(this).css('color', '#bbbbbb');
                        $(ui.helper).find('div .tooltiptext').remove();
                        $(ui.helper).attr('id', 'divrqmtsetfunctionality');
                        $(ui.helper).attr('rqmtsetid', $(this).attr('rqmtsetid'));
                        $(ui.helper).attr('rqmtsetfunctionalityid', $(this).attr('rqmtsetfunctionalityid'));
                        $(ui.helper).attr('functionalityid', $(this).attr('functionalityid'));
                        $(ui.helper).css('font-weight', 'bold');
                        $(ui.helper).css('color', _dragItemTextColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);

                        var clearSetFuncSelections = $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + $(this).attr('rqmtsetid') + ']');
                        if (clearSetFuncSelections.is(':visible')) {
                            var theTD = clearSetFuncSelections.closest('td');
                            var isTheDraggedItemSelected = $(this).attr('funcselected') == '1';
                            var selCount = theTD.find('[id=divrqmtsetfunctionality][funcselected=1]').length + (isTheDraggedItemSelected ? 0 : 1); // make sure that the dragged item is counted even if it wasn't previously right-clicked
                            if (selCount > 1) {
                                $(ui.helper).html($(ui.helper).html() + '&nbsp;<b>(+' + (selCount - 1) + ')</b>');
                            }
                        }
                    },
                    stop: function (e, ui) {
                        $(this).css('color', _newComponentTextColor);

                        if ($(this).attr('funcselected') == '1') {
                            $(this).css('background-color', _funcSelectedBackgroundColor);
                        }
                        else {
                            $(this).css('background-color', _newComponentBackgroundColor);
                        }

                        clearActiveRQMTSet($(this).attr('rqmtsetid'));
                        _draggingFunctionalityInProgress = false;
                        setTimeout(function () { _draggingInProcess = false; }, 100); // _draggingInProcess allows us to prevent unwanted click events from firing when a drop occurs or ends; we use the timer to make sure the drag value is still on until after those other functions have evaluated
                    },
                    distance: 3,
                    delay: 100
                });

                $('#divRQMTsGrid').find('td[id=tdRQMTFunctionality]').droppable({
                    over: function (e, ui) {                        
                        if ($(ui.helper).attr('id') == 'divrqmtsetfunctionality') {                            
                            if ($(ui.helper).attr('rqmtsetid') == $(this).attr('rqmtsetid')) {
                                $(ui.helper).css('font-weight', 'bold');
                                $(ui.helper).css('color', _dragItemTextColor);
                                $(ui.helper).css('background-color', _dragItemBackgroundColor);
                                $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);
                                $(this).css('background-color', _dragHoverColor);

                                $(this).parent().find('td[id=tdRQMTName][rqmtnametd=true]').css('color', 'green');
                            }
                            else {
                                $(ui.helper).css('color', _dragItemTextErrorColor);
                                $(ui.helper).css('background-color', _dragItemBackgroundErrorColor);
                                $(ui.helper).css('border', '1px solid ' + _dragItemBorderErrorColor);
                            }
                        }
                    },
                    out: function (e, ui) {
                        if ($(ui.helper).attr('id') == 'divrqmtsetfunctionality' && $(ui.helper).attr('rqmtsetid') == $(this).attr('rqmtsetid')) {                            
                            var tr = $(this).closest('tr');
                            var altRow = tr.attr('altrow');

                            var bg = altRow ? '#eeeeee' : '#ffffff';
                            $(this).css('background-color', altRow == 'true' ? '#eeeeee' : '#ffffff');

                            $(this).parent().find('td[id=tdRQMTName][rqmtnametd=true]').css('color', '#000000');
                        }
                    },
                    drop: function (e, ui) {
                        if ($(ui.helper).attr('id') == 'divrqmtsetfunctionality' && $(ui.helper).attr('rqmtsetid') == $(this).attr('rqmtsetid')) {                            
                            var tr = $(this).closest('tr');
                            var altRow = tr.attr('altrow');

                            var bg = altRow ? '#eeeeee' : '#ffffff';
                            $(this).css('background-color', altRow == 'true' ? '#eeeeee' : '#ffffff');

                            var dropRow = $(this).closest('tr');
                            var RQMTSetID = dropRow.attr('rqmtsetid');
                            var RQMTSet_RQMTSystemID = dropRow.attr('rqmtsetrqmtsystemid');

                            var RQMTSetFunctionalityID = $(ui.helper).attr('rqmtsetfunctionalityid');
                            var FunctionalityID = $(ui.helper).attr('functionalityid');
                            var RQMTFunctionalities = '';

                            // add the new dropped func
                            var rqmtSet = getRQMTSet(RQMTSetID);
                            var rqmt = RQMTSet_RQMTSystemID > 0 ? getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID) : null;
                            RQMTFunctionalities += FunctionalityID + '';

                            // add any extra selections that were right-click highlighted
                            var selections = $('[id=divrqmtsetfunctionality][rqmtsetid=' + RQMTSetID + '][funcselected=1]');
                            for (var i = 0; i < selections.length; i++) {
                                var sel = $(selections[i]);
                                RQMTFunctionalities += ',' + sel.attr('functionalityid');
                            }

                            // add remaining funcs that already existed in the rqmt                                                      
                            for (var i = 0; i < rqmt.Functionalities.length; i++) {
                                var func = rqmt.Functionalities[i];
                                var theFunctionality = _.find(rqmtSet.Functionalities, function (f) { return f.RQMTSetFunctionalityID == func.RQMTSetFunctionalityID });

                                RQMTFunctionalities += ',' + theFunctionality.FunctionalityID;
                            }

                            PageMethods.SaveRQMTFunctionality(RQMTSetID, RQMTSet_RQMTSystemID, RQMTFunctionalities, RQMTSetFunctionalityID, 0, 0, null, editRQMTFunc_Save_done, on_error);
                        }
                    }
                });
            }


            $('.tabDiv[comptab]').find('[name=divsearchresultcomp]').draggable("option", "distance", doesSetExist ? 3 : 100000); 

            clearActiveRQMTSet();
            clearLoadingMessage();
            updateButtonStatuses();

            _initComplete = true; // if we are initializing the page, filter rqmt sets is always the last thing that gets done, so we always set it here
        }

        function getHTMLForRQMTSetNameGroup(theRow, dt, closedGroups, openSets, setsWithOpenTasks, forceOpenAll) {
            var html = '';

            var dataFound = false;
            var lastRQMTSetID = -1;
            var altRow = false;
            var setCount = 0;

            var groupIsClosed = _.find(closedGroups, function (setNameID) { return setNameID == theRow.RQMTSetNameID }) != null && !forceOpenAll;

            html += '<table cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;margin-top:10px;">';
            html += '  <tr>';
            html += '    <td style="padding:3px;"><span style="font-size:16px;font-weight:bold;" id="spanRQMTSetGroupName" rqmtsetnameid="' + theRow.RQMTSetNameID + '">' + theRow.RQMTSetID + ' - ' + theRow.RQMTSetName + '</span>&nbsp;&nbsp;&nbsp;<span style="font-size:smaller;color:#000000;">(<a href="javascript:void()" onclick="toggleRQMTSetGroup(' + theRow.RQMTSetNameID + '); $(this).text($(this).text() == \'COLLAPSE\' ? \'SHOW\' : \'COLLAPSE\'); return false;" style="color:#000000;">' + (groupIsClosed ? 'SHOW' : 'COLLAPSE') + '</a> / <a href="javascript:void();" onclick="renameSetGroup(' + theRow.RQMTSetNameID  + '); return false;" style="color:#000000;">RENAME</a>)<span></td>';
            html += '  </tr>';
            html += '  <tr>';
            html += '    <td>';
            html += '      <div id="divrqmtsetnamegroup" rqmtsetnameid="' + theRow.RQMTSetNameID + '" style="display:' + (groupIsClosed ? 'none' : 'block') + '">'; // we are using a div inside the cell to allow allow for show/hide of the set grid; this works cleaner than hiding tr's and td's
            html += '        <table cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">'; // this table will hold the grid of sets in this rqmtsetname group
            html += '          <tr class="gridHeader">';
            html += '            <td style="border:1px solid grey;text-align:center;width:20px;font-weight:bold;">&nbsp;</td>';            
            html += '            <td style="border:1px solid grey;text-align:left;width:200px;font-weight:bold;">Work Area</td>';
            html += '            <td style="border:1px solid grey;text-align:left;width:200px;font-weight:bold;">System</td>';
            html += '            <td style="border:1px solid grey;text-align:left;width:200px;font-weight:bold;">Purpose</td>';
            html += '            <td style="border:1px solid grey;text-align:center;width:20px;font-weight:bold;white-space:nowrap;">RQMTs</td>';
            html += '            <td style="border:1px solid grey;text-align:center;width:50px;font-weight:bold;white-space:nowrap;">Tasks</td>';
            html += '            <td style="border:1px solid grey;text-align:center;width:20px;font-weight:bold;white-space:nowrap;">';
            html += '              <div class="tooltip" style="border-right:0px;border-bottom:1px dotted #888888;">Complexity';
            html += '<div class="tooltiptext">';
            html += '<b><u>Complexity</u></b><br />';
            html += 'XXS<br />XS<br />S<br />M<br />L<br />XL<br />XXL<br />TBD';
            html += '</div >';
            html += '              </div >';
            html += '            </td > ';
            html += '            <td style="border:1px solid grey;text-align:center;width:20px;font-weight:bold;white-space:nowrap;">Updated</td>';
            html += '            <td style="border:1px solid grey;text-align:left;font-weight:bold;width:1%;white-space:nowrap;">Options</td>';
            html += '          </tr>';

            for (var i = 0; dt != null && i < dt.length; i++) {
                var row = dt[i];

                var RQMTSetID = row.RQMTSetID;

                if (RQMTSetID != lastRQMTSetID) {                                        
                    html += getHTMLForRQMTSet(row, _.filter(dt, function (r) { return r.RQMTSetID == RQMTSetID }), setCount % 2 == 1, _.find(openSets, function (setID) { return setID == RQMTSetID }) != null || forceOpenAll, _.find(setsWithOpenTasks, function (setID) { return setID == RQMTSetID }) != null);

                    dataFound = true;
                    setCount++;
                    lastRQMTSetID = RQMTSetID;
                }
            }

            if (!dataFound) {
                html += '<tr><td colspan="' + _rqmtSetColspan + '">No data found.</td></tr>';
            }

            html += '        </table>'; 
            html += '      </div>';
            html += '    </td>';
            html += '  </tr>';
            html += '</table>';

            return html;
        }

        function getHTMLForRQMTSet(theRow, dt, altRow, startOpen, tasksOpen) {
            var html = '';

            var RQMTSetID = theRow.RQMTSetID;
            var rqmtSet = getRQMTSet(RQMTSetID);
            var functionalityViewMode = _loadedRQMTSetViewModes[RQMTSetID] == 'functionality';
            var collapsedViewMode = _loadedRQMTSetViewModes[RQMTSetID] == 'normalcollapsed';
            var count = dt.length;
            var rqmtsInSet = dt != null && dt.length > 0 && dt[0].RQMTID > 0; // the dt[0].RQMTID check is needed because we could have sets without rqmts in them, but we still get one row for the set

            // if we have a set with no requirements, we still get a count of 1 (the calling query is a left join on set -> rqmt)
            // so for counts of 1, we verify that there is at least 1 rqmt, and if not, we set the count to 0
            if (dt[0].RQMTID == null || dt[0].RQMTID == 0) {
                count--;
            }

            var bg = altRow ? '#eeeeee' : '#ffffff';

            html += '<tr id="trRQMTSetHeaderRow" rqmtsetid="' + RQMTSetID + '" class="gridBody" altrow="' + (altRow ? 'true' : 'false') + '" onmouseover="if (_sortingInProgress) return; $(this).find(\'div[id=divRQMTSetHeaderRowButtons][rqmtsetid=' + RQMTSetID + ']\').css(\'opacity\', 1.0)" onmouseout="if (_sortingInProgress) return; $(this).find(\'div[id=divRQMTSetHeaderRowButtons][rqmtsetid=' + RQMTSetID + ']\').css(\'opacity\', .15);">';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';border-left:1px solid grey;text-align:center;width:20px;font-weight:bold;padding:3px;"><img src="images/icons/' + (startOpen ? 'minus_blue.png' : 'add_blue.png') + '" style="width:16px;height:16px;cursor:pointer;" name="imgToggleSet" rqmtsetid="' + RQMTSetID + '" onclick="toggleRQMTSet(' + RQMTSetID + ', this);"></td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:left;width:200px;font-weight:bold;">' + theRow.WorkArea + (_debugMode ? ' <span style="font-weight:normal;font-size:smaller;">(' + RQMTSetID + ')</span>' : '') + '</td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:left;width:200px;font-weight:bold;">' + theRow.WTS_SYSTEM + '</td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:left;width:200px;font-weight:bold;">' + theRow.RQMTType + '</td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:center;width:20px;font-weight:bold;white-space:nowrap"><div style="display:inline-block;border-bottom:1px dotted #000000;cursor:pointer;" onclick="toggleRQMTSet(' + RQMTSetID + ', this);">(' + count + ')</div></td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:center;width:50px;font-weight:bold;white-space:nowrap;cursor:pointer;" onclick="RQMTSetTasksClicked(' + RQMTSetID + ')" alt="' + (rqmtSet.Tasks.length > 0 ? 'Show/Hide Tasks' : 'Click to add Tasks') + '" title="' + (rqmtSet.Tasks.length > 0 ? 'Show/Hide Tasks' : 'Click to add Tasks') + '"><div style="display:inline-block;border-bottom:1px dotted #000000;">(' + rqmtSet.Tasks.length + ')</div></td>';
            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:center;width:20px;font-weight:bold;white-space:nowrap">';
            if (theRow.Justification != null && theRow.Justification.length > 0) {
                // note for devs copying and/or modifying the tooltip code: the default width is 120px; if you change the width, you must change the margin left as well to be -50% of the new width
                html += '    <div class="tooltip">(' + theRow.RQMTComplexity + ')<div class="tooltiptext tooltip-bottom" style="width:250px;margin-left:-125px;">' + StripHTML(theRow.Justification) + '</div></div>';
            }
            else {
                html += '(' + theRow.RQMTComplexity + ')';
            }
            html += '  </td>';

            var m = moment(theRow.UpdatedDate);
            var upd = m.format('MM/DD/YYYY');
            var updTooltip = 'Created:<br />' + theRow.CreatedBy + '<br /><br />Updated:<br />' + theRow.UpdatedBy;

            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:center;white-space:nowrap;font-weight:bold;"><div class="tooltip"><u>' + upd + '</u><div class="tooltiptext tooltip-bottom-noarrow" style="width:225px;left:-50px;">' + updTooltip + '</div></div></td>';

            html += '  <td class="gridFullBorder" style="background-color:' + bg + ';text-align:left;width:1%;white-space:nowrap;">';
            html += '    <div id="divRQMTSetHeaderRowButtons" rqmtsetid="' + RQMTSetID + '" style="opacity:.15;text-align:center;">';
            html += '      <img src="images/icons/check' + (rqmtSet.QuickAddEnabled ? '' : '_gray') + '.png" alt="Quick Add ' + (rqmtSet.QuickAddEnabled ? 'Enabled' : 'Disabled') + ' (click to toggle)" title="Quick Add ' + (rqmtSet.QuickAddEnabled ? 'Enabled' : 'Disabled') + ' (click to toggle)" onclick="editRQMTSetQuickAddClicked(' + RQMTSetID + ')" style="cursor:pointer">&nbsp;';
            html += '      <img src="images/icons/pencil.png" alt="Edit Set" title="Edit Set" onclick="editRQMTSetButtonClicked(' + RQMTSetID + ')" style="cursor:pointer">&nbsp;';
            html += '      <img src="images/icons/cross.png" alt="Delete Set" title="Delete Set" onclick="deleteRQMTSetClicked(' + RQMTSetID + ')" style="cursor:pointer">';
            html += '      <img src="images/icons/newspaper.png" alt="View RQMT Set History" title="View RQMT Set History" onclick="viewRQMTSetHistory(' + RQMTSetID + ')" style="cursor:pointer">';
            html += '    </div>';
            html += '  </td>';
            html += '</tr>';

            html += '<tr id="trRQMTSetTask" rqmtsetid="' + RQMTSetID + '" style="display:' + (tasksOpen && rqmtSet.Tasks.length > 0 ? 'table-row' : 'none') + ';">';
            html += '  <td colspan="' + _rqmtSetColspan + '" style="border:1px solid grey;background-color:#ffffff;text-align:left;width:100%;padding:5px;position:relative;">';
                        
            html += '<table celpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">';

            if (rqmtSet.Tasks.length > 0) {
                html += '<tr>';
                html += '  <td colspan="5" style="border-width:0px;text-align:left;padding-left:0px;"><i>RQMT Set Tasks:</i></td>';
                html += '  <td colspan="1" style="border-width:0px;text-align:right;padding-right:0px;"><input type="button" value="Add" onclick="selectTask(' + RQMTSetID + ')"></td>';
                html += '</tr>';
                html += '<tr>';
                html += '  <td class="gridBodyFullBorder" style="width:15px;text-align:center;background-color:#dddddd;font-weight:bold;"></td>';
                html += '  <td class="gridBodyFullBorder" style="width:100px;text-align:center;background-color:#dddddd;font-weight:bold;">Work Task</td>';
                html += '  <td class="gridBodyFullBorder" style="width:250px;text-align:left;background-color:#dddddd;font-weight:bold;">Title</td>';
                html += '  <td class="gridBodyFullBorder" style="width:150px;text-align:center;background-color:#dddddd;font-weight:bold;">Assigned To</td>';
                html += '  <td class="gridBodyFullBorder" style="width:75px;text-align:center;background-color:#dddddd;font-weight:bold;">% Comp</td>';
                html += '  <td class="gridBodyFullBorder" style="width:125px;text-align:center;background-color:#dddddd;font-weight:bold;">Status</td>';
                html += '</tr>';

                for (var i = 0; i < rqmtSet.Tasks.length; i++) {
                    var task = rqmtSet.Tasks[i];

                    var rstaskid = task.RQMTSetTaskID;
                    var wid = task.WORKITEMID;
                    var tn = task.TASK_NUMBER;
                    var witid = task.WORKITEM_TASKID;

                    html += '<tr>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:center;"><img imgtaskdelete="1" rqmtsetid="' + RQMTSetID + '" rstaskid="' + rstaskid + '" witid="' + witid + '" wid="' + wid + '" tn="' + tn + '" src="images/icons/delete.png" width="12" height="12" style="cursor:pointer;" onclick="deleteTask(' + RQMTSetID + ', ' + rstaskid + ', \'' + wid + '-' + tn + '\')"></td>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:center;"><span style="text-decoration:underline;color:blue;cursor:pointer;" onclick="openTask(' + wid + ', ' + witid + ', ' + tn + ')">' + wid + '-' + tn + '</span></td>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:left;">' + task.TITLE + '</td>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:center;">' + task.USERNAME + '</td>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:center;">' + task.COMPLETIONPERCENT + '</td>';
                    html += '  <td class="gridBodyFullBorder" style="text-align:center;">' + task.STATUS + '</td>';
                    html += '</tr>';
                }
            }
            else {
                html += '<tr><td>No tasks found.</td></tr>';
            }

            html += '</table>';

            html += '  </td>';
            html += '</tr>';

            html += '<tr id="trRQMTSet" rqmtsetid="' + RQMTSetID + '" viewmode="normal" style="' + (startOpen ? '' : 'display:none;') + '" onmouseover="if (_sortingInProgress || _draggingFunctionalityInProgress) return; $(this).prev().find(\'div[id=divRQMTSetHeaderRowButtons][rqmtsetid=' + RQMTSetID + ']\').css(\'opacity\', 1.0)" onmouseout="if (_sortingInProgress || _draggingFunctionalityInProgress) return; $(this).prev().find(\'div[id=divRQMTSetHeaderRowButtons][rqmtsetid=' + RQMTSetID + ']\').css(\'opacity\', .15);">';
            html += '  <td colspan="' + _rqmtSetColspan + '" style="border:1px solid grey;background-color:#ffffff;text-align:left;width:100%;padding:5px;position:relative;">';

            // quick add label
            var quickAddColor = rqmtSet.QuickAddEnabled ? '#4f6d10' : '#666666';
            html += '<div style="color:' + quickAddColor + ';position:absolute;right:25px;font-size:smaller;">';
            html += '(QUICK ADD ' + (rqmtSet.QuickAddEnabled ? 'ENABLED' : 'DISABLED') + ')';
            html += '</div>';

            html += '<div rqmtaddedoverlay="1" rqmtsetid="' + RQMTSetID + '" style="width:100%;height:100%;position:absolute;vertical-align:middle;z-index:1000;display:none;">';
            html += '  <div style="width:100%;height:100%;position:absolute;background-color:white;opacity:.7;z-index:1;"></div>';
            html += '  <div style="width:100%;height:100%;position:absolute;background-color:blue;opacity:.15;z-index:1;"></div>';
            html += '  <div style="position:absolute;top:50%;left:50%;z-index:2;font-size:24px;opacity:.5;transform: rotate(20deg);">RQMT ADDED</div>';
            html += '</div>';

            // usage
            html += '    <table cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;margin-bottom:5px;">';
            html += '      <tr id="trRQMTSetUsage" rqmtsetid="' + RQMTSetID + '">';
            html += '        <td style="width:1%;white-space:nowrap;text-align:left;padding-right:10px;" valign="top"><b>Usage (months):</b></td>';
            html += '        <td monthcell="1" style="width:99%;white-space_nowrap;text-align:left;padding-right:10px;" valign="top">';
            html += GetRQMTSetUsageMonthString(RQMTSetID, dt);
            html += '        </td>';
            html += '      </tr>';
            html += '    </table>';

            // functionalities
            html += '    <table cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;margin-bottom:5px;">';
            html += '      <tr id="trRQMTSetFunctionality" rqmtsetid="' + RQMTSetID + '">';
            html += '        <td style="width:1%;white-space:nowrap;text-align:left;padding-right:10px;" valign="top"><div style="line-height:28px;"><b>Functionalities:</b></div></td>';
            html += '        <td style="width:60%;text-align:left;padding-right:10px;" valign="top">';
            if (rqmtSet.Functionalities.length > 0) {
                for (var i = 0; i < rqmtSet.Functionalities.length; i++) {
                    var func = rqmtSet.Functionalities[i];

                    html += '<div id="divrqmtsetfunctionality" rqmtsetid="' + RQMTSetID + '" rqmtsetfunctionalityid="' + func.RQMTSetFunctionalityID + '" functionalityid="' + func.FunctionalityID + '" style="padding:3px;display:inline-block;border:1px solid ' + _newComponentBorderColor + ';background-color:' + _newComponentBackgroundColor + ';color:' + _newComponentTextColor + ';margin:3px;border-radius:5px;cursor:pointer;white-space:nowrap;" onmouseover="this.style.border=\'1px solid gray\'; this.style.color=\'gray\';" onmouseout="this.style.border=\'1px solid ' + _newComponentBorderColor + '\'; this.style.color=\'' + _newComponentTextColor + '\'" onclick="editRQMTFunctionalityClicked(' + RQMTSetID + ', ' + 0 + ', ' + func.RQMTSetFunctionalityID + ', ' + func.FunctionalityID + ');" oncontextmenu="editRQMTFunctionalityContextClicked(this); window.event.preventDefault(); window.event.stopPropagation();">';
                    html += func.Functionality + '&nbsp;(';
                    if (func.Justification != null && func.Justification.trim().length > 0) {
                        html += '<div class="tooltip" style="display:inline-block;">';
                        html += func.RQMTComplexity != null ? func.RQMTComplexity : 'TBD';
                        html += '  <div class="tooltiptext tooltip-bottom-noarrow" style="width:300px">';
                        html += StripHTML(func.Justification);
                        html += '  </div>';
                        html += '</div>';
                    }
                    else {
                        html += func.RQMTComplexity != null ? func.RQMTComplexity : 'TBD';
                    }
                    html += ')';
                    html += '</div>';
                    if (i >= 3 && i % 4 == 3) {
                        html += '<br />';
                    }
                }

                html += '<div id="divrqmtsetselectedfunctionalitieslabel" rqmtsetid="' + RQMTSetID + '" style="margin-left:5px;display:inline-block;font-size:smaller;position:relative;top:3px;">(RIGHT CLICK TO SELECT MULTIPLE)</div>';
                html += '<div id="divclearrqmtsetselectedfunctionalities" rqmtsetid="' + RQMTSetID + '" style="cursor:pointer;margin-left:5px;display:none;font-size:smaller;position:relative;top:3px;" onclick="clearRQMTSetFunctionalitySelectedItems(this);">(<u>CLEAR SELECTIONS</u>)</div>';
            }
            else {
                html += '<div style="padding-top:3px;margin:3px;">No Functionalities assigned.</div>';
            }
            html += '        </td>';
            html += '        <td style="width:1%;padding-left:3px;white-space:nowrap;text-align:right;" valign="bottom">';     
                        

            if (rqmtsInSet) { 
                html += '          <div style="display:inline-block;"><input type="button" value="Copy" onclick="copyRQMTsClicked(' + RQMTSetID + ')"></div>';
            }

            html += '          <div style="display:inline-block;margin-left:0px;"><input type="button" rqmtpastebutton="1" value="Paste"' + (isRQMTOnClipboard() ? '' : ' disabled') + ' onclick="pasteRQMTsClicked(' + RQMTSetID + ', \'' + theRow.WTS_SYSTEMID + '\')"></div>'; // we leave this line in at all times so we can paste rqmts in there

            if (rqmtsInSet) {
                html += '          <div style="display:inline-block;margin-left:0px;margin-right:20px;"><input type="button" value="Delete" onclick="removeRQMTFromSetClicked(' + RQMTSetID + ')"></div>';
                html += '          <div style="display:inline-block;border:1px solid ' + (functionalityViewMode || collapsedViewMode ? '#aaaaaa' : '#000000') + ';cursor:pointer;border-radius:3px;position:relative;top:5px;width:20px;height:20px;" onmouseover="' + (functionalityViewMode || collapsedViewMode ? 'this.style.border=\'1px solid #000000\';' : '') + '" onmouseout="' + (functionalityViewMode || collapsedViewMode ? 'this.style.border=\'1px solid #aaaaaa\';' : '') + '" onclick="' + (functionalityViewMode || collapsedViewMode ? 'toggleRQMTSetViewMode(' + RQMTSetID + ', \'normal\');' : '') + '"><img src="images/icons/listorderingwithchildren.png" style="width:20px;height:20px;opacity:' + (functionalityViewMode || collapsedViewMode ? '.66' : '1.0') + ';" alt="Standard View" title="Standard View"></div>';
                html += '          <div style="display:inline-block;border:1px solid ' + (functionalityViewMode || !collapsedViewMode ? '#aaaaaa' : '#000000') + ';cursor:pointer;border-radius:3px;position:relative;top:5px;width:20px;height:20px;" onmouseover="' + (functionalityViewMode || !collapsedViewMode ? 'this.style.border=\'1px solid #000000\';' : '') + '" onmouseout="' + (functionalityViewMode || !collapsedViewMode ? 'this.style.border=\'1px solid #aaaaaa\';' : '') + '" onclick="' + (functionalityViewMode || !collapsedViewMode ? 'toggleRQMTSetViewMode(' + RQMTSetID + ', \'normalcollapsed\');' : '') + '"><img src="images/icons/listordering.png" style="width:20px;height:20px;opacity:' + (functionalityViewMode || !collapsedViewMode ? '.66' : '1.0') + ';" alt="Standard View - Parents Only" title="Standard View - Parents Only"></div>';
                html += '          <div style="display:inline-block;border:1px solid ' + (!functionalityViewMode ? '#aaaaaa' : '#000000') + ';cursor:pointer;border-radius:3px;position:relative;top:5px;width:20px;height:20px;" onmouseover="' + (!functionalityViewMode ? 'this.style.border=\'1px solid #000000\';' : '') + '" onmouseout="' + (!functionalityViewMode ? 'this.style.border=\'1px solid #aaaaaa\';' : '') + '" onclick="' + (!functionalityViewMode ? 'toggleRQMTSetViewMode(' + RQMTSetID + ', \'functionality\');' : '') + '"><img src="images/icons/fnclistordering.png" style="width:20px;height:20px;opacity:' + (!functionalityViewMode ? '.66' : '1.0') + ';" alt="Functionality View" title="Functionality View"></div>';
            }

            html += '        </td>';
            html += '      </tr>';
            html += '    </table>';

            html += '    <table rqmtsetrqmttable="1" rqmtsetid="' + RQMTSetID + '" cellpadding="0" cellspacing="0" width="100%" style="border-collapse:collapse;">';

            var modeRQMTColSpan = _wideMode ? _rqmtColspanWideMode : _rqmtColspan;

            if (dt != null && dt.length > 0 && dt[0].RQMTID > 0) { // the dt[0].RQMTID check is needed because we could have sets without rqmts in them, but we still get one row for the set
                if (functionalityViewMode) {
                    var addedRQMTS = false;

                    for (var f = -1; f < rqmtSet.Functionalities.length; f++) { // we start with -1; this tells us to pull UNASSIGNED first
                        var unassignedMode = f == -1;

                        var func = unassignedMode ? null : rqmtSet.Functionalities[f];

                        if (addedRQMTS) {
                            html += '<tr><td colspan="' + modeRQMTColSpan + '" style="height:8px;"></td></tr>';
                        }

                        var rqmtsWithFunc = null;

                        if (unassignedMode) {
                            rqmtsWithFunc = _.filter(dt, function (rqmt) { return rqmt.Functionalities.length == 0 });                            
                        }
                        else {
                            rqmtsWithFunc = _.filter(dt, function (rqmt) { return rqmt.Functionalities.length > 0 && _.find(rqmt.Functionalities, function (f) { return f.RQMTSetFunctionalityID == func.RQMTSetFunctionalityID }) != null });
                        }
                        
                        if (rqmtsWithFunc != null && rqmtsWithFunc.length > 0) {
                            html += '<tr>';
                            html += '  <td colspan="' + modeRQMTColSpan + '" style="text-align:left;width:99%;white-space:nowrap;font-size:larger;font-weight:bold;text-decoration:underline;">' + (func != null ? func.Functionality.toUpperCase() : 'UNASSIGNED') + '</td>';
                            html += '</tr>';

                            for (var i = 0; i < rqmtsWithFunc.length; i++) {
                                var row = rqmtsWithFunc[i];
                                html += getHTMLForRQMT(row, i, i + 1, 0, rqmtSet, true, false);
                            }

                            addedRQMTS = true;
                        }
                        else {
                            // we don't show "no results found" for the unassigned group
                            if (!unassignedMode) {
                                html += '<tr>';
                                html += '  <td colspan="' + modeRQMTColSpan + '" style="text-align:left;width:99%;white-space:nowrap;font-size:larger;font-weight:bold;text-decoration:underline;">' + (func != null ? func.Functionality.toUpperCase() : 'UNASSIGNED') + '</td>';
                                html += '</tr>';

                                html += '<tr id="trRQMT" altrow="false" colspan="' + modeRQMTColSpan + '"><td colspan="' + modeRQMTColSpan + '">No RQMTs found.</td></tr>';
                            }
                        }
                    }
                }
                else {
                    var lastParentID = -1;
                    var currentParentIdx = 0;
                    var currentChildIdx = 0;

                    for (var i = 0; i < dt.length; i++) {
                        var row = dt[i];
                        if (row.ParentRQMTSet_RQMTSystemID != 0) { // child
                            currentChildIdx++;

                            if (collapsedViewMode) {
                                continue;
                            }
                        }
                        else { // parent
                            if (row.RQMTSet_RQMTSystemID != lastParentID) {
                                currentParentIdx++;
                                currentChildIdx = 0;
                            }

                            lastParentID = row.RQMTSet_RQMTSystemID;
                        }
                        html += getHTMLForRQMT(dt[i], i, currentParentIdx, currentChildIdx, rqmtSet, false, collapsedViewMode);
                    }
                }
            }
            else {
                html += '<tr id="trRQMT" altrow="false" colspan="' + modeRQMTColSpan + '"><td>No RQMTs found.</td></tr>';
            }

            html += '    </table>';
            html += '  </td>';
            html += '</tr>';            

            return html;
        }

        function getHTMLForRQMT(theRow, idx, currentParentIdx, currentChildIdx, rqmtSet, functionalityViewMode, collapsedViewMode) {
            var html = '';

            if (idx == 0) {
                html += '<tr rqmtsetid="' + theRow.RQMTSetID + '" class="gridHeader gridFullBorder">';
                html += '  <th style="border:1px solid grey;width:1%;white-space:nowrap;"><input type="checkbox" toggleall="1" onclick="rqmtSetToggleAllRQMTsClicked(' + theRow.RQMTSetID + ', this)"></th>';
                html += '  <th style="width:1%;white-space:nowrap;text-align:center;">&nbsp;IDX&nbsp;</th>';
                html += '  <th style="width:1%;white-space:nowrap;text-align:center;">&nbsp;#&nbsp;</th>';
                html += '  <th style="text-align:left;width:99%;white-space:nowrap;">RQMT</th>';
                html += '  <th style="text-align:left;width:200px;white-space:nowrap;">FUNCTIONALITY&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th>';
                html += '  <th style="text-align:left;width:150px;white-space:nowrap;">USAGE (MONTHS)</th>';
                html += '  <th style="text-align:left;width:150px;white-space:nowrap;">DESC</th>';
                html += '  <th style="text-align:left;width:150px;white-space:nowrap;">ATTRIBUTES</th>';
                html += '  <th style="text-align:left;width:150px;white-space:nowrap;">DEFECTS</th>';
                html += '  <th style="text-align:center;width:1px;white-space:nowrap;">UPDATED</th>';
                html == '</tr>';
            }

            var bgColor = idx % 2 == 0 ? '#ffffff' : _rowAltColor;

            html += '<tr id="trRQMT" rqmtrow="true" rqmtsetid="' + theRow.RQMTSetID + '" rqmtsetrqmtsystemid="' + theRow.RQMTSet_RQMTSystemID + '" rqmtsystemid="' + theRow.RQMTSystemID + '" rqmtid="' + theRow.RQMTID + '" class="gridBody gridFullBorder" altrow="' + (idx % 2 == 0 ? 'false' : 'true') + '">';
            html += '  <td style="border:1px solid grey;width:1%;padding:4px;text-align:center;vertical-align:top;background-color:' + bgColor + ';">';
            html += '    <input type="checkbox" cbRQMT="1" rqmtsetid="' + theRow.RQMTSetID + '" systemid="' + theRow.WTS_SYSTEMID + '" rqmtsetrqmtsystemid="' + theRow.RQMTSet_RQMTSystemID + '" rqmtsystemid="' + theRow.RQMTSystemID + '" rqmtid="' + theRow.RQMTID + '">';
            //html += '    <img src="images/icons/cross.png" style="width:16px;height:16px;padding:3px;cursor:pointer;" onclick="removeRQMTFromSetClicked(' + theRow.RQMTSetID + ', ' + theRow.RQMTID + '); return false;">';
            html += '  </td > ';

            var childRQMTs = 0;
            if (collapsedViewMode) {
                var childArr = _.filter(_loadedRQMTs, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == theRow.RQMTSet_RQMTSystemID });

                childRQMTs = childArr != null ? childArr.length : 0;
            }

            html += '  <td style="width:1%;text-align:center;vertical-align:top;white-space:nowrap;background-color:' + bgColor + ';">&nbsp;' + currentParentIdx + '.' + currentChildIdx + (collapsedViewMode ? '&nbsp;(' + childRQMTs + ')' : '') + '&nbsp;</td>';
            html += '  <td style="width:1%;text-align:center;vertical-align:top;white-space:nowrap;background-color:' + bgColor + ';">&nbsp;<a href="javascript:openRQMTPopupFromBuilder(' + theRow.RQMTID + ',' + theRow.RQMTSet_RQMTSystemID + ')">' + theRow.RQMTID + '</a>&nbsp;</td>';
            html += '  <td id="tdRQMTName" rqmtnametd="true" rqmtsetid="' + theRow.RQMTSetID + '" rqmtsetrqmtsystemid="' + theRow.RQMTSet_RQMTSystemID + '" rqmtid="' + theRow.RQMTID + '" parentrqmtsetrqmtsystemid="' + theRow.ParentRQMTSet_RQMTSystemID + '" style="position:relative;text-align:left;vertical-align:top;width:200px;background-color:' + bgColor + ';border:1px double grey;cursor:pointer;' + (theRow.ParentRQMTSet_RQMTSystemID > 0 && !functionalityViewMode ? 'padding-left:15px;' : '') + '" onmouseover="$(this).find(\'[toggleindenthelper]\').show();" onmouseout="$(this).find(\'[toggleindenthelper]\').hide();" onclick="if (!_draggingInProcess) toggleRQMTIndent(' + theRow.RQMTSet_RQMTSystemID + ');">' + theRow.RQMT + (_debugMode ? ' <span style="font-size:smaller">(RSRS' + theRow.RQMTSet_RQMTSystemID + ' RS' + theRow.RQMTSystemID + ' RQMT' + theRow.RQMTID + '/PAR' + theRow.ParentRQMTSet_RQMTSystemID + ' IDX' + theRow.OutlineIndex + ')</span>' : '') + ' <div toggleindenthelper="1" style="position:absolute;text-align:right;bottom:0px;right:2px;height:12px;width:95%;font-size:10px;color:#888888;display:none;cursor:pointer;">(CLICK TO TOGGLE INDENT)</div></td>';

            // functionality
            html += '  <td id="tdRQMTFunctionality" rqmtsetid="' + theRow.RQMTSetID + '" rqmtsetrqmtsystemid="' + theRow.RQMTSet_RQMTSystemID + '" class="gridFullBorder" style="text-align:left;vertical-align:top;width:200px;white-space:nowrap;background-color:' + bgColor + ';" onmouseover="$(this).find(\'img\').css(\'opacity\', 1.0);" onmouseout="$(this).find(\'img\').css(\'opacity\', 0.4);">';
            var funcToolTip = '';

            for (var i = 0; i < theRow.Functionalities.length; i++) {
                var func = theRow.Functionalities[i];
                var RQMTSetFunctionalityID = func.RQMTSetFunctionalityID;
                var theFunctionality = _.find(rqmtSet.Functionalities, function (f) { return f.RQMTSetFunctionalityID == RQMTSetFunctionalityID });

                if (theFunctionality != null) {            
                    if (funcToolTip.length > 0) funcToolTip += '<br />';
                    funcToolTip += theFunctionality.Functionality;                    
                }                
            }

            html += '  <div class="tooltip" style="display:inline;margin-right:5px;' + (theRow.Functionalities.length == 0 ? 'visibility:hidden;' : '') + '">';
            html += '    <span style="cursor:pointer;" onclick="editRQMTFunctionalityClicked(' + theRow.RQMTSetID + ', ' + theRow.RQMTSet_RQMTSystemID + '); return false;">FUNCTIONALITIES (' + theRow.Functionalities.length + ')</span>&nbsp;';
            if (theRow.Functionalities.length > 0) {
                html += '<div class="tooltiptext tooltip-right-noarrow" style="width:200px;white-space:normal;">' + funcToolTip + '</div>';
            }
            html += '  </div>';
            html += '  <div style="display:inline;">';
            html += '    <img src="images/icons/pencil_add.png" alt="Add New Functionality" title="Add New Functionality" style="cursor:pointer;width:14px;height:14px;opacity:.4;" onclick="editRQMTFunctionalityClicked(' + theRow.RQMTSetID + ', ' + theRow.RQMTSet_RQMTSystemID + '); return false;">';
            html += '  </div>';

            html += '  </td > ';

            // usage
            html += '  <td id="tdRQMTUsage" rqmtsetid="' + theRow.RQMTSetID + '" rqmtsetrqmtsystemid="' + theRow.RQMTSet_RQMTSystemID + '" class="gridFullBorder" style="position:relative;text-align:center;vertical-align:top;width:150px;white-space:nowrap;background-color:' + bgColor + ';" onmouseover="$(this).find(\'div[togglealllabel=1]\').show();" onmouseleave="_usageSelectionInProgress=false; $(this).find(\'div[togglealllabel=1]\').hide();" onmousedown="">';
            for (var m = 1; m <= 12; m++) {
                var month = new moment(new Date((m < 10 ? '0' : '') + m + '/01/2000'));
                var monthWidth = _wideMode ? '28px' : '12px';
                var monthSelected = theRow['Month_' + m] != null && (theRow['Month_' + m] + '').toLowerCase() == 'true';
                var monthName = _wideMode ? month.format('MMMM').substring(0, 3).toUpperCase() : month.format('MMM').substring(0, 1).toUpperCase();
                var monthBG = monthSelected ? _newComponentBackgroundColor : '#ffffff';
                var monthColor = monthSelected ? _newComponentTextColor : '#999999';
                var monthBorder = monthSelected ? _newComponentBorderColor : '#999999';
                var monthOpacity = monthSelected ? .85 : .66;

                html += '<div month="' + m + '" style="-webkit-user-select: none; -moz-user-select: none; -ms-user-select: none; color:' + monthColor + ';background-color:' + monthBG + ';border:1px solid ' + monthBorder + ';opacity:' + monthOpacity + ';border-radius:3px;cursor:pointer;display:inline-block;width:' + monthWidth + ';height:12px;line-height:14px;font-size:10px;font-weight:bold;margin-right:1px;" onmouseover="this.style.opacity=1.0; if (_usageSelectionInProgress) RQMTFunctionalityUsageClicked(' + m + ', ' + theRow.RQMTSet_RQMTSystemID + ', this);" onmouseout="' + (monthSelected ? 'this.style.opacity=.85;' : 'this.style.opacity=.66;') + '" onmousedown="_usageSelectionInProgress = true; RQMTFunctionalityUsageClicked(' + m + ', ' + theRow.RQMTSet_RQMTSystemID + ', this);" onmouseup="_usageSelectionInProgress=false; return false;">' + monthName + '</div>';                
            }
            html += '<div togglealllabel="1" style="position:absolute;text-align:right;bottom:0px;height:12px;width:98%;font-size:10px;color:#888888;display:none;cursor:pointer;" onclick="if (!_usageSelectionInProgress) ToggleAllRQMTFunctionalityUsage(' + theRow.RQMTSet_RQMTSystemID + ', this);">(CLICK TO TOGGLE ALL)</div>';
            html += '  </td>';

            // description
            html += '  <td class="gridFullBorder" style="text-align:center;vertical-align:top;width:' + (_wideMode ? '400px' : '150px') + ';white-space:nowrap;background-color:' + bgColor + ';" onmouseover="$(this).find(\'img\').css(\'opacity\', 1.0);" onmouseout="$(this).find(\'img\').css(\'opacity\', 0.4);">';                                    
          
            if (theRow.RQMTDescriptionArray.length > 0) {
                html += '<div class="tooltip">';
                html += '<div style="display:inline-block;border-bottom:1px dotted #000000;cursor:pointer;" onclick="editRQMTDescription(' + theRow.RQMTSet_RQMTSystemID + ', ' + -1 + '); return false;"><nobr>(' + theRow.RQMTDescriptionArray.length + ')</nobr></div>';
                html += '<div class="tooltiptext tooltip-noarrow" style="width:500px;left:-475px;top:5px;">';
                html += '<table cellpadding="0" cellspacing="0" border="0" style="border:0px;width:500px">';
                html += '  <tr>';
                html += '    <td style="width:25px;text-align:center;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;border:0px;">#</td>';
                html += '    <td style="width:325px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;">Description</td>';
                html += '    <td style="width:150px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;">Type</td>';
                html += '    <td style="width:150px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;"><div style="cursor:pointer;display:inline-block;" alt="Add/Edit" title="Add/Edit" onclick="editRQMTDescription(' + theRow.RQMTSet_RQMTSystemID + ', ' + 0 + ')" onmouseover="$(this).css(\'text-decoration\', \'underline\')" onmouseout="$(this).css(\'text-decoration\', \'none\')"><img src="images/icons/attach.png" width="12" height="12" style="border-bottom:1px solid black"></div></td>';
                html += '  </tr>';

                for (var d = 0; d < theRow.RQMTDescriptionArray.length; d++) {
                    var desc = theRow.RQMTDescriptionArray[d];

                    var descText = desc.RQMTDescription.length > 300 ? desc.RQMTDescription.substring(0, 300) + '...' : desc.RQMTDescription;

                    html += '  <tr>';
                    html += '    <td style="width:25px;text-align:center;vertical-align:top;border:0px;">' + (d + 1) + '</td>';
                    html += '    <td style="width:300px;text-align:left;vertical-align:top;border:0px;cursor:pointer;" onclick="editRQMTDescription(' + theRow.RQMTSet_RQMTSystemID + ', ' + desc.RQMTSystemRQMTDescriptionID + ')" onmouseover="$(this).css(\'text-decoration\', \'underline\')" onmouseout="$(this).css(\'text-decoration\', \'none\')">' + StripHTML(descText) + '</td>';
                    html += '    <td style="width:100px;text-align:left;vertical-align:top;border:0px;white-space:nowrap;cursor:pointer;" onclick="editRQMTDescription(' + theRow.RQMTSet_RQMTSystemID + ', ' + desc.RQMTSystemRQMTDescriptionID + ')" onmouseover="$(this).css(\'text-decoration\', \'underline\')" onmouseout="$(this).css(\'text-decoration\', \'none\')">' + desc.RQMTDescriptionType + '</td>';
                    html += '    <td style="width:150px;text-align:left;vertical-align:top;border:0px;">';
                    if (desc.Attachments != null && desc.Attachments.length > 0) {
                        for (var i = 0; i < desc.Attachments.length; i++) {
                            var att = desc.Attachments[i];
                            html += '<div style="display:block;cursor:pointer;" alt="View Attachment" title="View Attachment" onclick="openDescriptionAttachment(' + att.AttachmentId + ')" onmouseover="$(this).css(\'text-decoration\', \'underline\')" onmouseout="$(this).css(\'text-decoration\', \'none\')">' + att.FileName + '</div>';
                        }
                    }
                    else {
                        html += '&nbsp;';
                    }
                    html += '    </td > ';
                    html += '  </tr>';
                }

                html += '</table>';
                html += '</div>';                
                html += '</div>';                
            }
            else {
                html += '<div style="display:inline-block;border-bottom:1px dotted #000000;cursor:pointer;" onclick="editRQMTDescription(' + theRow.RQMTSet_RQMTSystemID + ', ' + -1 + '); return false;"><nobr>(0)</nobr></div>'
            }                       
            html += '  </td>';

            // attributes
            html += '  <td class="gridFullBorder" style="text-align:left;vertical-align:top;width:150px;background-color:' + bgColor + ';">';

            var attrIDs = ['RQMTStageID', 'CriticalityID', 'RQMTStatusID', 'RQMTAccepted'];            
            var attrVals = ['RQMTStage', 'Criticality', 'RQMTStatus', 'RQMTAccepted'];
            var attrTitles = ['PD2TDR', 'Criticality', 'Status', 'Accepted'];

            var attrHTML = '';
            var popupHTML = '';
            
            for (var a = 0; a < attrIDs.length; a++) {                
                var id = theRow[attrIDs[a]];
                var val = theRow[attrVals[a]];
                var title = attrTitles[a];

                var skipAttr = false;

                if (id == null) id = 0;
                if (val == null) {
                    val = 'None';
                    skipAttr = true;
                }

                val = val + ''; // some of the vals come over as straight booleans or ints, so we convert them to text

                var valLower = val.toLowerCase();

                if (valLower == 'false') {
                    val = 'No';
                    skipAttr = true;
                }
                else if (valLower == 'true') {
                    val = 'Yes';
                }

                valLower = val.toLowerCase();
                
                var color = '#000000';
                if (valLower == 'yes' || valLower == 'pass') {
                    color = '#3c763d';
                }
                else if (valLower == 'no' || valLower == 'fail' || valLower == 'deficient' || valLower == 'major' || valLower == 'critical') {
                    color = '#a94442';
                }

                val = '<span style="color:' + color + '">' + val + '</span>';

                if (!skipAttr) {
                    if (attrHTML.length > 0) attrHTML += '/';
                    attrHTML += val;
                }

                if (popupHTML != '') {
                    popupHTML += '<br />';
                }
                else {
                    //popupHTML += '<div style="width:75px;text-align:left;"><b><u>Attributes</u></b></div>';
                }
                popupHTML += '<span style="display:inline-block;width:75px;"><b>' + title + ':</b></span>&nbsp;' + val;
            }

            if (attrHTML.length > 0) {
                html += '<div style="width:95%;margin-right:3px;cursor:pointer;border-radius:3px;padding:2px;border:1px solid ' + bgColor + ';" onmouseover="this.style.color=\'' + _newComponentTextColor + '\'; this.style.border=\'1px solid ' + _newComponentBorderColor + '\'; this.style.backgroundColor=\'' + _newComponentBackgroundColor + '\';" onmouseout="this.style.color=\'\'; this.style.backgroundColor=\'\'; this.style.border=\'1px solid ' + bgColor + '\';" onclick="editRQMTAttributeClicked(' + theRow.RQMTSet_RQMTSystemID + '); return false;" class="tooltip">';
                html += attrHTML;
                html += '<div class="tooltiptext" style="width:200px;text-align:left;left:-70px;width:200px;">' + popupHTML + '</div>';
                html += '</div>';
            }
            else {
                html += '    <div style="float:right;"><img src="images/icons/pencil_add.png" alt="Add Attributes" title="Add Attributes" style="cursor:pointer;width:14px;height:14px;opacity:.4;" onmouseover="this.style.opacity=1.0" onmouseout="this.style.opacity=.4" onclick="editRQMTAttributeClicked(' + theRow.RQMTSet_RQMTSystemID + '); return false;"></div>';
            }

            html += '  </td>';

            // defects
            html += '  <td style="text-align:center;vertical-align:top;width:1px;white-space:no-wrap;background-color:' + bgColor + ';cursor:pointer;" onclick="DefectLinkClicked(' + theRow.RQMTSetID + ', ' + theRow.RQMTID + ', ' + theRow.WTS_SYSTEMID + ')">'

            if (theRow.Defects.length > 0) {
                html += '<div class="tooltip">';
                html += '<div style="display:inline-block;border-bottom:1px dotted #000000;cursor:pointer;"><nobr>(' + theRow.Defects.length + ')</nobr></div>';
                html += '<div class="tooltiptext tooltip-bottom-noarrow" style="width:525px;left:-400px;">';
                html += '<table cellpadding="0" cellspacing="0" border="0" style="border:0px;width:525px">';
                html += '  <tr>';
                html += '    <td style="width:25px;text-align:center;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;border:0px;">#</td>';
                html += '    <td style="width:300px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;">Defect</td>';
                html += '    <td style="width:100px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;">Impact</td>';
                html += '    <td style="width:100px;text-align:left;vertical-align:top;font-weight:bold;border:0px;color:black;text-decoration:underline;">PD2TDR</td>';
                html += '  </tr>';

                for (var d = 0; d < theRow.Defects.length; d++) {
                    var defect = theRow.Defects[d];

                    var defectColor = defect.Impact == 'Work Stoppage' ? 'color:red;' : '';
                    html += '  <tr>';
                    html += '    <td style="width:25px;text-align:center;vertical-align:top;border:0px;' + defectColor + '">' + (d + 1) + '</td>';
                    html += '    <td style="width:300px;text-align:left;vertical-align:top;border:0px;' + defectColor + '">' + defect.Description + '</td>';
                    html += '    <td style="width:100px;text-align:left;vertical-align:top;border:0px;' + defectColor + '">' + defect.Impact + '</td>';
                    html += '    <td style="width:100px;text-align:left;vertical-align:top;border:0px;' + defectColor + '">' + (defect.RQMTStage != null ? defect.RQMTStage : 'None') + '</td>';
                    html += '  </tr>';
                }

                html += '</table>';
                html += '</div>';
                html += '</div>';
            }
            else {
                html += '<div style="display:inline-block;border-bottom:1px dotted #000000;cursor:pointer;"><nobr>(0)</nobr></div>'
            }

            html += '  </td > ';

            // updated
            var m = moment(theRow.UpdatedDate);
            var upd = m.format('MM/DD/YYYY');
            var updTooltip = 'Created:<br />' + theRow.CreatedBy + '<br /><br />Updated:<br />' + theRow.UpdatedBy;            
            html += '  <td style="text-align:center;vertical-align:top;width:1px;white-space:no-wrap;background-color:' + bgColor + ';">' + '<div class="tooltip"><u>' + upd + '</u><div class="tooltiptext tooltip-bottom-noarrow" style="width:225px;left:-115px;font-weight:bold;">' + updTooltip + '</div></div></td>';
            
            html += '</tr>';

            return html;
        }

        function toggleRQMTSetGroup(RQMTSetNameID) {
            var groupDiv = $('[id=divrqmtsetnamegroup][rqmtsetnameid=' + RQMTSetNameID + ']');

            if (groupDiv.is(':visible')) {
                groupDiv.hide();
            }
            else {
                groupDiv.show();
            }

            clearActiveRQMTSet();
        }

        function toggleRQMTSet(RQMTSetID, imgToggle) {
            
            var setTR = $('[id=trRQMTSet][rqmtsetid=' + RQMTSetID + ']');
            var setTaskTR = $('[id=trRQMTSetTask][rqmtsetid=' + RQMTSetID + ']');

            if (setTR.is(':visible')) {
                setTR.hide();
                setTaskTR.hide();
                $(imgToggle).attr('src', 'images/icons/add_blue.png');
            }
            else {
                setTR.show();
                $(imgToggle).attr('src', 'images/icons/minus_blue.png');
            }
        }

        function toggleRQMTSetViewMode(RQMTSetID, mode) {
            _loadedRQMTSetViewModes[RQMTSetID] = mode;
            
            redrawRQMTSetTable(RQMTSetID);
        }

        function refreshRQMTSetTable(RQMTSetID) { // refreshes table with fresh data from the database
            PageMethods.FilterRQMTSets(RQMTSetID, null, 0, 0, 0, function (results) { filterRQMTSets_done(results, RQMTSetID) }, on_error);
        }

        function redrawRQMTSetTable(RQMTSetID) { // refreshes the table but does NOT load data from the database (use this for small view changes that don't involve data updates)
            var rqmts = getAllRQMTsForRQMTSet(RQMTSetID);

            filterRQMTSets_done(rqmts, RQMTSetID, true);
        }

        function clearActiveRQMTSet(RQMTSetID) {
            var setToClear = RQMTSetID != null ? RQMTSetID : _activeRQMTSetID;
            $('[id=trRQMTSetHeaderRow][rqmtsetid=' + setToClear + ']').find('td').css('background-color', $('[id=trRQMTSetHeaderRow][rqmtsetid=' + setToClear + ']').attr('altrow') == 'true' ? _rowAltColor : '#ffffff');
            $('[id=trRQMTSet][rqmtsetid=' + setToClear + ']').find('td:first').css('background-color', '#ffffff');

            _activeRQMTSetID = 0;
        }

        function setActiveRQMTSet(RQMTSetID) {
            _activeRQMTSetID = RQMTSetID;
            $('[id=trRQMTSetHeaderRow][rqmtsetid=' + _activeRQMTSetID + ']').find('td').css('background-color', _activeRQMTSetBackground);
            $('[id=trRQMTSet][rqmtsetid=' + _activeRQMTSetID + ']').find('td:first').css('background-color', _activeRQMTSetBackground);
        }

        function editRQMTSetButtonClicked(RQMTSetID) {          
            _activeRQMTSetID = RQMTSetID;

            var rqmtset = getRQMTSet(RQMTSetID);
            var rqmts = getAllRQMTsForRQMTSet(RQMTSetID);
            var rqmt = rqmts[0];

            // set up popup            
            var selectHTML = '<option value="0">--- CREATE A NEW NAME ---</option>';
            for (var i = 0; _rqmtSetNames != null && i < _rqmtSetNames.length; i++) {
                selectHTML += '<option value="' + _rqmtSetNames[i].RQMTSetNameID + '" ' + (_rqmtSetNames[i].RQMTSetNameID == rqmt.RQMTSetNameID ? 'selected' : '') + '>' + _rqmtSetNames[i].RQMTSetName + '</option>';
            }
            $('#ddlRenameRQMTSet').html(selectHTML);
            $('#txtRenameRQMTSet').val(rqmt.RQMTSetName);
            $('#txtRenameRQMTSet').prop('disabled', true);

            var ddlSuiteSource = $('[id$=ddlSuite]');
            var ddlSuiteRecat = $('#ddlRecategorizeSetSuite');
            ddlSuiteRecat.html('');
            var suiteSourceOptions = ddlSuiteSource.find('option');            
            $.each(suiteSourceOptions, function (idx, opt) { ddlSuiteRecat.append($(opt).clone()) });
            ddlSuiteRecat.val(rqmtset.WTS_SYSTEM_SUITEID);
            ddlSuiteRecat.on('change', function () { ddlSuite_changed(true); });

            var ddlSystemRecat = $('#ddlRecategorizeSetSystem');
            ddlSystemRecat.html('');
            var SystemSourceOptions = getSystemOptionsForSuite(rqmtset.WTS_SYSTEM_SUITEID);
            ddlSystemRecat.html(SystemSourceOptions);
            ddlSystemRecat.val(rqmt.WTS_SYSTEMID);
            ddlSystemRecat.on('change', function () { ddlSystem_changed(true); });

            var ddlWorkAreaRecat = $('#ddlRecategorizeSetWorkArea');
            ddlWorkAreaRecat.html('');
            var WorkAreaSourceOptions = getWorkAreaOptionsForSystem(rqmtset.WTS_SYSTEMID);
            ddlWorkAreaRecat.html(WorkAreaSourceOptions);            
            ddlWorkAreaRecat.val(rqmt.WorkAreaID);

            var ddlRQMTTypeSource = $('[id$=ddlRQMTType]');
            var ddlRQMTTypeRecat = $('#ddlRecategorizeSetRQMTType');
            ddlRQMTTypeRecat.html('');
            var RQMTTypeSourceOptions = ddlRQMTTypeSource.find('option');            
            $.each(RQMTTypeSourceOptions, function (idx, opt) { ddlRQMTTypeRecat.append($(opt).clone()) });
            ddlRQMTTypeRecat.val(rqmt.RQMTTypeID);

            var ddlRQMTComplexityRecat = $('#ddlRecategorizeSetComplexity');
            ddlRQMTComplexityRecat.html(_complexityOptions);            
            ddlRQMTComplexityRecat.val(rqmt.RQMTComplexityID);

            var justNeeded = rqmt.RQMTComplexity == 'L' || rqmt.RQMTComplexity == 'XL' || rqmt.RQMTComplexity == 'XXL';
            $('#txtRecategorizeSetJustification').prop('disabled', !justNeeded);
            $('#txtRecategorizeSetJustification').css('opacity', justNeeded ? 1.0 : .666);

            if (justNeeded) {
                $('#spnRecategorizeSetJustificationNeeded').show();
                $('#txtRecategorizeSetJustification').val(rqmt.Justification);
            }
            else {
                $('#spnRecategorizeSetJustificationNeeded').hide();
                $('#txtRecategorizeSetJustification').val('');
            }
            

            // make sure the button doesn't have same event twice
            $('#btnRecategorizeSetCancel').off('click').on('click', editRQMTSet_Cancel);
            $('#btnRecategorizeSetSave').off('click').on('click', editRQMTSet_Save);
            
            var openPopup = popupManager.AddPopupWindow('EditRQMTSet', 'Edit RQMT Set', null, 360, 800, 'PopupWindow', this, false, 'divRQMTSetEdit');
            openPopup.onClose = editRQMTSet_ClearActiveRQMTSet;
            if (openPopup) openPopup.Open();
        }

        function editRQMTSetQuickAddClicked(RQMTSetID) {
            var set = getRQMTSet(RQMTSetID);
            set.QuickAddEnabled = !set.QuickAddEnabled;
            
            redrawRQMTSetTable(RQMTSetID);
        }

        function viewRQMTSetHistory(RQMTSetID) {
            var nTitle = 'RQMT Set History (RQMT Set #' + RQMTSetID + ')';
            var nHeight = 500, nWidth = 1000;
            var nURL = 'Audit_History_Popup.aspx?viewtype=RQMTSET&itemid=' + RQMTSetID;
            var openPopup = popupManager.AddPopupWindow('AuditHistory', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);
            if (openPopup) openPopup.Open();
        }

        function editRQMTSet_ClearActiveRQMTSet() {
            _activeRQMTSetID = 0;
        }

        function recategorizeSetComplexityChanged() {
            var thePopup = popupManager.GetPopupByName('EditRQMTSet');
            var ic = thePopup.InlineContainer;

            var ddl = $('#ddlRecategorizeSetComplexity', ic);
            var txt = $('#txtRecategorizeSetJustification', ic);
            var req = $('#spnRecategorizeSetJustificationNeeded', ic);

            var ddlval = ddl.find('option:selected').text();
            var justNeeded = ddlval == 'L' || ddlval == 'XL' || ddlval == 'XXL';
            if (justNeeded) {
                req.show();
            }
            else {
                req.hide();
                txt.val('');
            }
            txt.prop('disabled', !justNeeded);
            txt.css('opacity', justNeeded ? 1.0 : .666);
        }

        function editRQMTSet_Cancel() {            
            var thePopup = popupManager.GetPopupByName('EditRQMTSet');
            thePopup.Opener.editRQMTSet_ClearActiveRQMTSet();
            thePopup.Close();
        }

        function editRQMTSet_Save() {
            var thePopup = popupManager.GetPopupByName('EditRQMTSet');
            var ic = thePopup.InlineContainer;

            var rqmts = getAllRQMTsForRQMTSet(_activeRQMTSetID);
            var rqmt = rqmts[0];

            var nameID = $('#ddlRenameRQMTSet', ic).val();
            var nameTxt = $('#txtRenameRQMTSet', ic).val();

            var cmpDDL = $('#ddlRecategorizeSetComplexity', ic);
            var cmpDDLTxtVal = cmpDDL.find('option:selected').text();
            var cmpDDLVal = cmpDDL.val();
            var justTxt = $('#txtRecategorizeSetJustification', ic);
            var justNeeded = cmpDDLTxtVal == 'L' || cmpDDLTxtVal == 'XL' || cmpDDLTxtVal == 'XXL';

            var errors = [];
            
            if (nameTxt.trim().length == 0) {
                $('#txtRenameRQMTSet', ic).css('border', '1px solid red');                
                errors.push('Name cannot be blank.');
                return;
            }
            else {
                $('#txtRenameRQMTSet', ic).css('border', '1px solid #a9a9a9');
            }

            if ($('#ddlRecategorizeSetSystem', ic).val() == 0) {
                $('#ddlRecategorizeSetSystem', ic).css('border', '1px solid red');
                errors.push('System is required.');
            }
            else {
                $('#ddlRecategorizeSetSystem', ic).css('border', '1px solid #a9a9a9');
            }

            if ($('#ddlRecategorizeSetWorkArea', ic).val() == 0) {
                $('#ddlRecategorizeSetWorkArea', ic).css('border', '1px solid red');
                errors.push('Work Area is required.');
            }
            else {
                $('#ddlRecategorizeSetWorkArea', ic).css('border', '1px solid #a9a9a9');
            }

            if ($('#ddlRecategorizeSetRQMTType', ic).val() == 0) {
                $('#ddlRecategorizeSetRQMTType', ic).css('border', '1px solid red');
                errors.push('Purpose is required.');
            }
            else {
                $('#ddlRecategorizeSetRQMTType', ic).css('border', '1px solid #a9a9a9');
            }

            if (justNeeded && justTxt.val().trim().length == 0) {
                justTxt.css('border', '1px solid red');
                errors.push('Justification is required.');
            }
            else {
                justTxt.css('border', '1px solid #a9a9a9');
            }

            if (errors.length > 0) {
                dangerMessage(errors.join('<br>'), null, null, ic[0]);
                return;
            }

            if (nameID == rqmt.RQMTSetNameID) {
                nameTxt = '';
            }

            showLoadingMessage('Saving RQMT Set...', false, $(ic).find('#divRecategorizeMessagePlaceHolder')[0]);

            PageMethods.SaveRQMTSet(_activeRQMTSetID, nameTxt, $('#ddlRecategorizeSetSystem', ic).val(), $('#ddlRecategorizeSetWorkArea', ic).val(), $('#ddlRecategorizeSetRQMTType', ic).val(), cmpDDLVal, justTxt.val().trim(), function (result) { saveRQMTSet_done(result, true) }, on_error);
        }

        function saveRQMTSet_done(result, recatSetMode) {
            if (recatSetMode == null) recatSetMode = false;

            var ic = null;

            if (recatSetMode) {
                var thePopup = popupManager.GetPopupByName('EditRQMTSet');
                ic = thePopup.InlineContainer;
            }

            var dt = $.parseJSON(result);

            clearLoadingMessage(recatSetMode ? $(ic).find('#divRecategorizeMessagePlaceHolder') : null); 

            if (dt.success == 'true') {    
                $('[id$=txtRQMTSetName]').val($('#txtRenameRQMTSet', ic).val());
                $('[id$=ddlSystem]').val($('#ddlRecategorizeSetSystem', ic).val());
                $('[id$=ddlWorkArea]').val($('#ddlRecategorizeSetWorkArea', ic).val());
                $('[id$=ddlRQMTType]').val($('#ddlRecategorizeSetRQMTType', ic).val());

                thePopup.Opener.editRQMTSet_ClearActiveRQMTSet();
                thePopup.Opener.clearCachedRQMTSetList();
                thePopup.Close();
                
                filterRQMTSets();
            }
            else {
                if (dt.exists == 'true') {
                    showLoadingMessage('A RMQT Set with the specified configuration already exists. Please select different values.', true, $(ic).find('#divRecategorizeMessagePlaceHolder')[0], true);
                }
                else {
                    showLoadingMessage(dt.error, true, $(ic).find('#divRecategorizeMessagePlaceHolder')[0], true);
                }
            }
        }

        function editRQMTSet_ddlRenameRQMTSet_changed() {
            var thePopup = popupManager.GetPopupByName('EditRQMTSet');
            var ic = thePopup.InlineContainer;

            var val = $('#ddlRenameRQMTSet', ic).val();
            var txt = $('#ddlRenameRQMTSet', ic).find('option:selected').html();

            if (val == 0) {
                var rqmts = getAllRQMTsForRQMTSet(_activeRQMTSetID);
                var rqmt = rqmts[0];

                $('#txtRenameRQMTSet', ic).val(rqmt.RQMTSetName);
                $('#txtRenameRQMTSet', ic).prop('disabled', false);
                $('#txtRenameRQMTSet', ic).focus();
            }
            else {
                $('#txtRenameRQMTSet', ic).val(txt);
                $('#txtRenameRQMTSet', ic).prop('disabled', true);
            }
        }
        
        function btnAddNewRQMTSet_click() {
            var rqmtSetName = $('[id$=txtRQMTSetName]').val();
            var system = $('[id$=ddlSystem]').val();
            var workArea = $('[id$=ddlWorkArea]').val();
            var rqmtType = $('[id$=ddlRQMTType]').val();

            clearActiveRQMTSet();
            
            if (system == null || system.length == 0) system = '0';
            if (workArea == null || workArea.length == 0) workArea = '0';
            if (rqmtType == null || rqmtType.length == 0) rqmtType = '0';

            if (system != 0 || workArea != 0 || rqmtType != 0) {
                if (rqmtSetName != null && rqmtSetName.length > 0) {
                    addNewRQMTSet_confirmed('OK', rqmtSetName);
                }
                else {
                    /*
                    var selectHTML = '<br><br /><select onchange="if (this.selectedIndex > 0) { document.getElementById(\'qb_InputAnswer\').value=this.options[this.selectedIndex].text;document.getElementById(\'qb_InputAnswer\').disabled=true; document.getElementById(\'qb_InputAnswer\').style.backgroundColor=\'#dddddd\'; } else {  document.getElementById(\'qb_InputAnswer\').value=\'\';document.getElementById(\'qb_InputAnswer\').disabled=false; document.getElementById(\'qb_InputAnswer\').style.backgroundColor=\'\'; } " style="width:250px"><option value="0">--- CREATE A NEW NAME ---</option>';
                    for (var i = 0; _rqmtSetNames != null && i < _rqmtSetNames.length; i++) {
                        selectHTML += '<option value="1">' + _rqmtSetNames[i].RQMTSetName + '</option>';
                    }
                    selectHTML += '</select > ';

                    var question = 'Enter a name for this RQMT Set, or select an existing name from the list below:' + selectHTML;
                    question = escape(StrongEscape(question));

                    InputBox('Add RQMT Set', question, 'OK,Cancel', 'addNewRQMTSet_confirmed', 400, 300, this);
                    */
                }
            }
        }

        function addNewRQMTSet_confirmed(btnAnswer, inputAnswer) {
            if (btnAnswer == 'OK') {
                $('[id$=txtRQMTSetName]').val(inputAnswer);

                var system = $('[id$=ddlSystem]').val();
                var workArea = $('[id$=ddlWorkArea]').val();
                var rqmtType = $('[id$=ddlRQMTType]').val();

                PageMethods.AddRQMTSet(inputAnswer, system, workArea, rqmtType, addNewRQMTSet_done, on_error);

                showLoadingMessage('Creating RQMT Set...');
            }
        }

        function addNewRQMTSet_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success) {
                clearCachedRQMTSetList();
                filterRQMTSets();
            }

            clearLoadingMessage();
        }

        function renameSetGroup(RQMTSetNameID) {
            clearActiveRQMTSet();
            InputBox('Rename RQMT Set Group', 'Enter a name for this RQMT Set Group:', 'OK,Cancel', 'renameRQMTSetGroup_confirmed', 400, 300, this, RQMTSetNameID, null, null, getRQMTSetGroupName(RQMTSetNameID));
        }

        function renameRQMTSetGroup_confirmed(btnAnswer, inputAnswer, param) {
            if (btnAnswer == 'OK') {
                showLoadingMessage('Saving RQMT Set Name...');
                PageMethods.RenameRQMTSetGroup(param, inputAnswer, function (result) { renameRQMTSetGroup_done(result, param, inputAnswer) }, on_error);
            }
        }

        function renameRQMTSetGroup_done(result, RQMTSetNameID, RQMTSetName) {
            var dt = $.parseJSON(result);

            clearLoadingMessage();

            if (dt.success == "true") {     
                clearCachedRQMTSetList();
                infoMessage('RQMT Set Group renamed.', 'bottom right');
                $('span[id=spanRQMTSetGroupName][rqmtsetnameid=' + RQMTSetNameID + ']').html(RQMTSetName);

                refreshRQMTs();
            }
            else {
                dangerMessage(dt.error);
            }
        }
        
        function rqmtSetToggleAllRQMTsClicked(RQMTSetID, cb) {
            cb = $(cb);
            var theTable = cb.closest('table');
            var cbIsChecked = cb.is(':checked');
            theTable.find('input[cbRQMT=1]').prop('checked', cbIsChecked);
        }

        function componentDroppedInRQMTSet(tgtRQMTSetID, srcCompType, srcCompID, srcFKID) {
            var allowAdd = true;

            if (srcCompType == 'rqmt') {
                var existingRQMT = getRQMTFromRQMTSet(tgtRQMTSetID, srcFKID);

                if (existingRQMT != null) {
                    allowAdd = false;
                    dangerMessage('Cannot add duplicate RQMTs to RQMT Set.');
                }
                else {
                    showLoadingMessage('Adding RQMT...');
                    PageMethods.AddRQMTToRQMTSet(tgtRQMTSetID, srcFKID, null, false, function (result) { componentDroppedInRQMTSet_done(result, tgtRQMTSetID, srcCompType); }, on_error);
                }
            }
        }

        function componentDroppedInRQMTSet_done(result, RQMTSetID, compType) {
            var dt = $.parseJSON(result);

            if (dt.success) {
                if (compType == 'rqmt') {
                    refreshRQMTSetTable(RQMTSetID);
                }
            }
        }

        function RQMTSetTasksClicked(RQMTSetID) {
            var rqmtSet = getRQMTSet(RQMTSetID);

            if (rqmtSet.Tasks != null && rqmtSet.Tasks.length > 0) {
                // toggle task container
                var taskContainer = $('tr[id=trRQMTSetTask][rqmtsetid=' + RQMTSetID + ']');

                if (taskContainer.is(':visible')) {
                    taskContainer.css('display', 'none');
                }
                else {
                    taskContainer.css('display', 'table-row');
                }
            }
            else {
                selectTask(RQMTSetID);
            }
        }

        function selectTask(RQMTSetID, RQMTSetRQMTSystemID) { // we can select a task for a RQMTSet or a specific RQMT
            // NOTE: CURRENTLY, WE DON'T SUPPORT ADDING TASK FOR A RQMT, BUT THE SECOND PARAMETER IS HERE AS A HOOK FOR FUTURE
            _activeRQMTSetID = RQMTSetID;
            _activeRQMTSetRQMTSystemID = RQMTSetRQMTSystemID;

            var set = getRQMTSet(RQMTSetID);
            var rqmt = RQMTSetRQMTSystemID != null && RQMTSetRQMTSystemID > 0 ? getRQMTFromRQMTSetRQMTSystemID(RQMTSetRQMTSystemID) : null;

	        var nWindow = 'SelectSubTask';
	        var nTitle = 'Select Sub-Task';
	        var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + '?GridType=AOR&MyData=false&NewAOR=false&AORID=0&AORReleaseID=0&Type=SubTask&SubType=SelectSubTask&SelectCallback=subTaskSelected&SelectedTasks=&TaskID=&SelectedStatuses=0,1,2,3,4,5,6,7,8,9,11,12,13,64&HideAdd=true';
            nURL += '&SelectedSystems=' + set.WTS_SYSTEMID;

	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
            openPopup.onClose = selectTask_closed;
        }

        function selectTask_closed() {            
            _activeRQMTSetID = 0;
            _activeRQMTSetRQMTSystemID = 0;
        }

        function subTaskSelected(data) {
            var rsid = _activeRQMTSetID;
            var rsrsid = _activeRQMTSetRQMTSystemID;

            var taskarr = data[0].tasknumber.split('.'); // worktaskid.worktasknumber

            var taskContainer = $('tr[id=trRQMTSetTask][rqmtsetid=' + rsid + ']');
            taskContainer.css('display', 'table-row');
            
            showLoadingMessage('Adding task...');
            PageMethods.TaskAddedToRQMTSet(rsid, taskarr[0], function (result) { subTaskSelected_done(result, rsid) }, on_error)
        }

        function subTaskSelected_done(result, RQMTSetID) {
            clearLoadingMessage();
            refreshRQMTSetTable(RQMTSetID);
        }

        function deleteTask(RQMTSetID, RQMTSetTaskID, desc) {
            QuestionBox('Confirm Task Delete', 'Delete task ' + desc + '?', 'Yes,No', 'deleteTask_confirmed', 300, 300, this, RQMTSetID + ',' + RQMTSetTaskID);
        }

        function deleteTask_confirmed(answer, params) {            
            if (answer == 'Yes') {
                showLoadingMessage('Deleting task...');
                var RQMTSetID = params.split(',')[0];
                var RQMTSetTaskID = params.split(',')[1];

                PageMethods.DeleteTaskFromRQMTSet(RQMTSetID, RQMTSetTaskID, function (result) { deleteTask_done(result, RQMTSetID) }, on_error);
            }
        }

        function deleteTask_done(result, RQMTSetID) {
            clearLoadingMessage();
            refreshRQMTSetTable(RQMTSetID);
        }

        function clearCopiedRQMTsClicked() {
            PageMethods.CopyRQMTs('', '', '', copyRQMTsClicked_done, on_error);
            _copiedRQMTSystems = '';
        }

        function copyRQMTsClicked(RQMTSetID) {
            var checkedRQMTs = getCheckedRQMTsForRQMTSet(RQMTSetID);

            if (checkedRQMTs.length == 0) {
                warningMessage('Please check a RQMT');
                return;
            }

            var rqmtids = '';
            var systemids = '';
            var rqmtsetrqmtsystemids = '';
            for (var i = 0; i < checkedRQMTs.length; i++) {
                if (i > 0) {
                    rqmtids += '|';
                    systemids += '|';
                    rqmtsetrqmtsystemids += '|';
                }

                rqmtids += $(checkedRQMTs[i]).attr('rqmtid');
                systemids += $(checkedRQMTs[i]).attr('systemid');
                rqmtsetrqmtsystemids += $(checkedRQMTs[i]).attr('rqmtsetrqmtsystemid');
            }

            PageMethods.CopyRQMTs(rqmtids, systemids, rqmtsetrqmtsystemids, copyRQMTsClicked_done, on_error);
        }

        function copyRQMTsClicked_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                var rqmtids = dt.rqmtids;
                var systemids = dt.systemids;

                if (rqmtids != null && rqmtids.length > 0) {
                    _copiedRQMTSystems = systemids;                    
                    $('#divCopiedRQMTs').html('<b>Copied RQMTs:</b> ' + dt.rqmtidsstr + '&nbsp;&nbsp;&nbsp;<span style="font-size:smaller;cursor:pointer;" onclick="clearCopiedRQMTsClicked()">(<u>CLEAR</u>)' + (dt.rqmtidsstr.indexOf('*') != -1 ? '<br />* duplicate RQMT # from different RQMT Set' : '') + '</span>');
                    $('#divCopiedRQMTs').css('display', 'inline-block');
                    $('#divCopiedRQMTsDivider').show();
                    $('input[rqmtpastebutton=1]').prop('disabled', false);
                }
                else {
                    _copiedRQMTSystems = '';
                    $('#divCopiedRQMTs').hide();
                    $('#divCopiedRQMTsDivider').hide();
                    $('input[rqmtpastebutton=1]').prop('disabled', true);
                }
            }

            setHeight();
        }

        function pasteRQMTsClicked(RQMTSetID, WTS_SYSTEMID) {
            if (isRQMTOnClipboard()) {
                var sysarr = _copiedRQMTSystems.split('|');
                var foundOtherSystems = false;
                for (var i = 0; i < sysarr.length; i++) {
                    if (sysarr[i] != WTS_SYSTEMID) {
                        foundOtherSystems = true;
                        break;
                    }
                }

                if (foundOtherSystems) {
                    QuestionBox('Paste RQMTs Special', StrongEscape('You are pasting one or more RQMTs from a different system. Attributes, Defects and Descriptions do not normally transfer between systems. To include these items, use the checkboxes below:<br /><br /><input type="checkbox" id="cbattr" value="attr">Attributes<br /><input type="checkbox" id="cbdef" value="def">Defects<br /><input type="checkbox" id="cbdesc" value="desc">Descriptions'), 'Paste,Cancel', 'pasteRQMTsConfirmed', 400, 500, this, RQMTSetID);
                }
                else {
                    QuestionBox('Paste RQMTs', 'Paste RQMTs into this set?', 'Yes,No', 'pasteRQMTsConfirmed', 300, 300, this, RQMTSetID);
                }
            }
        }

        function pasteRQMTsConfirmed(answer, param) {
            if (answer == 'No' || answer == 'Cancel') {
                return;
            }

            showLoadingMessage('Pasting RQMTs...');

            if (answer == 'Yes') {
                var RQMTSetID = param;
                PageMethods.PasteRQMTs(RQMTSetID, '', function (result) { pasteRQMTs_done(result, RQMTSetID) }, on_error);
            }
            else if (answer == 'Paste') {
                var parr = param.split('|');
                var RQMTSetID = parr[0];
                PageMethods.PasteRQMTs(RQMTSetID, parr.length == 2 ? parr[1] : '', function (result) { pasteRQMTs_done(result, RQMTSetID) }, on_error);
            }
        }

        function pasteRQMTs_done(result, RQMTSetID) {            
            clearLoadingMessage();
            refreshRQMTSetTable(RQMTSetID);
        }

        function removeRQMTFromSetClicked(RQMTSetID) {
            var theTable = $('table[rqmtsetrqmttable=1][rqmtsetid=' + RQMTSetID + ']');
            var allRQMTs = theTable.find('input[cbRQMT=1]');
            var checkedRQMTs = getCheckedRQMTsForRQMTSet(RQMTSetID);

            if (checkedRQMTs.length == 0) {
                warningMessage('Please check a RQMT');
                return;
            }

            var multiple = checkedRQMTs.length > 1;
            var all = checkedRQMTs.length == allRQMTs.length;

            var title = '';
            var msg = '';

            if (all) {
                title = 'Delete All RQMTS';
                msg = 'Do you want to delete all RQMTS from this set only, or all the sets they are used?';
            }
            else if (multiple) {
                title = 'Delete RQMTS';
                msg = 'Do you want to delete the checked RQMTS from this set only, or all the sets they are used?';
            }
            else {
                title = 'Delete RQMT';
                msg = 'Do you want to delete the checked RQMT from this set only, or all the sets it is used?';
            }

            var cbs = '';
            for (var i = 0; i < checkedRQMTs.length; i++) {
                if (i > 0) cbs += '|';
                cbs += $(checkedRQMTs[i]).attr('rqmtid');
            }

            QuestionBox(title, msg, 'This Set,All Sets,Cancel', 'removeRQMTFromSetConfirmed', 300, 300, this, RQMTSetID + ',' + cbs);
            clearActiveRQMTSet();
        }

        function removeRQMTFromSetConfirmed(answer, param) {
            if (answer == 'This Set' || answer == 'All Sets') {
                var tokens = param.split(',');

                var rqmtsInSet = getAllRQMTsForRQMTSet(tokens[0]);

                var rqmtBeingDeleted = getRQMTFromRQMTSet(tokens[0], tokens[1]);

                showLoadingMessage('Deleting RQMT' + (tokens[1].indexOf('|') != -1 ? 's' : '') + '...');

                PageMethods.DeleteRQMTFromSet(tokens[0], tokens[1], answer == 'All Sets', function (result) { removeRQMTFromSet_done(result, tokens[0], answer == 'All Sets') }, on_error);
            }
        }

        function removeRQMTFromSet_done(result, RQMTSetID, globalDelete) {
            var dt = $.parseJSON(result);

            clearLoadingMessage();

            if (dt.success) {
                if (globalDelete) {
                    refreshRQMTs();
                }
                else {
                    refreshRQMTSetTable(RQMTSetID);
                }
            }
        }

        function deleteRQMTSetClicked(RQMTSetID) {
            var rqmtRows = $('#divRQMTsGrid').find('tr[id=trRQMTSet][rqmtsetid=' + RQMTSetID + ']');

            if (rqmtRows.length > 0) {
                QuestionBox('Delete RQMT Set', 'WARNING! This RQMT Set contains RQMTs. Deleting this RQMT Set will also delete its RQMTs. Continue?', 'Yes,No', 'deleteRQMTSetConfirmed', 300, 300, this, RQMTSetID);
            }
            else {
                QuestionBox('Delete RQMT Set', 'Are you sure you want to delete this RQMT Set?', 'Yes,No', 'deleteRQMTSetConfirmed', 300, 300, this, RQMTSetID);
            }

            clearActiveRQMTSet();
        }

        function deleteRQMTSetConfirmed(answer, RQMTSetID) {
            if (answer == 'Yes') {
                PageMethods.DeleteRQMTSet(RQMTSetID, function (result) { deleteRQMTSet_done(result, RQMTSetID) }, on_error);
            }
        }

        function deleteRQMTSet_done(result, RQMTSetID) {
            clearCachedRQMTSetList();
            refreshRQMTSetTable(RQMTSetID);
        }        

        function refreshRQMTSetNames() {
            PageMethods.GetRQMTSetNames(refreshRQMTSetNames_done, on_error);
        }

        function refreshRQMTSetNames_done(result) {
            var dt = $.parseJSON(result);

            _rqmtSetNames = dt;
        }

        function editRQMT(RQMTSet_RQMTSystemID) { // NOTE: THIS FUNCTION IS NO LONGER USED - WE USED TO POP UP A RQMT TEXT EDIT AND ASSOCIATIONS EDITOR, BUT NOW WE DEFER THAT TO THE EDITRQMT POPUP PAGE USING THE RQMT # LINK
            _activeRQMTSetRQMTSystemID = RQMTSet_RQMTSystemID;

            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);

            // we call off before on to make sure we don't ever double-up the click event
            $('#btnRQMTEditCancel').off('click').on('click', editRQMT_CancelClicked);                       
            $('#btnRQMTEditSave').off('click').on('click', editRQMT_SaveClicked);

            $('#txtRQMTEdit').val(rqmt.RQMT);

            var openPopup = popupManager.AddPopupWindow('EditRQMT', 'Edit RQMT - ' + (rqmt.RQMT.length > 50 ? rqmt.RQMT.substring(0, 50) + '...' : rqmt.RQMT), null, 530, 900, 'PopupWindow', this, false, 'divRQMTEdit');
            openPopup.onClose = editRQMT_ClearActiveRQMT;
            if (openPopup) openPopup.Open();
            editRQMTTabClicked('RQMT', 'RQMTSets');

            if (_cachedRQMTSetList != null) {
                editRQMT_LoadAllRQMTSets_done(_cachedRQMTSetList, rqmt)
            }
            else {
                PageMethods.LoadAllRQMTSets(function (results) { editRQMT_LoadAllRQMTSets_done(results, rqmt) }, on_error);
            }
        }

        function toggleRQMTIndent(RQMTSet_RQMTSystemID) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);
            var allRQMTsInSet = getAllRQMTsForRQMTSet(rqmt.RQMTSetID);
            var prevRQMT = null;

            for (var i = allRQMTsInSet.length - 1; i >= 0; i--) {
                var rqmtFound = allRQMTsInSet[i];

                if (rqmtFound.RQMTSet_RQMTSystemID == rqmt.RQMTSet_RQMTSystemID) {
                    if (i > 0) {
                        prevRQMT = allRQMTsInSet[i - 1];
                    }

                    break;
                }
            }

            if (prevRQMT != null) {
                RQMTSetReOrdered(rqmt.RQMTSetID, RQMTSet_RQMTSystemID, prevRQMT.RQMTSet_RQMTSystemID, false, rqmt.ParentRQMTSet_RQMTSystemID == 0, false);
            }
        }

        function editRQMT_LoadAllRQMTSets_done(results, rqmt) {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            _cachedRQMTSetList = results;

            var dt = $.parseJSON(_cachedRQMTSetList);

            var html = '';
                        
            html += '<table cellpadding="1" cellspacing="0" width="100%">';
            html += '    <tr><td style="font-weight:bold;width:1%;white-space:nowrap;">Current RQMT:&nbsp;&nbsp;&nbsp;</td><td style="width:99%;white-space:nowrap;font-weight:bold;">' + (rqmt.RQMT.length > 50 ? rqmt.RQMT.substring(0, 50) + '...' : rqmt.RQMT) + '</td></tr>';
            html += '    <tr><td style="width:1%;white-space:nowrap;">&nbsp;&nbsp;System Suite:&nbsp;&nbsp;&nbsp;</td><td style="width:99%;white-space:nowrap;font-weight:bold;">' + rqmt.WTS_SYSTEM_SUITE + '</td></tr>';
            html += '    <tr><td style="width:1%;white-space:nowrap;">&nbsp;&nbsp;System:&nbsp;&nbsp;&nbsp;</td><td style="width:99%;white-space:nowrap;font-weight:bold;">' + rqmt.WTS_SYSTEM + '</td></tr>';
            html += '    <tr><td style="width:1%;white-space:nowrap;">&nbsp;&nbsp;Work Area:&nbsp;&nbsp;&nbsp;</td><td style="width:99%;white-space:nowrap;font-weight:bold;">' + rqmt.WorkArea + '</td></tr>';
            html += '    <tr><td style="width:1%;white-space:nowrap;">&nbsp;&nbsp;Purpose:&nbsp;&nbsp;&nbsp;</td><td style="width:99%;white-space:nowrap;font-weight:bold;">' + rqmt.RQMTType + '</td></tr>';
            html += '    <tr style="height:15px;"><td colspan="2" style="text-align:right;height:15px;font-size:smaller;"><div id="divRQMTEditRQMTSetsLegend" style="display:none;">(<span style="color:red">DELETING RQMT</span> | <span style="color:blue">ADDING RQMT</span>)</div></td></tr>';
            html += '</table>';
            html += '<div style="width:100%;height:370px;overflow-y:auto;position:relative;">';
            html += '  <table width="100%" cellpadding="1" cellspacing="0" style="border-collapse:collapse;">';
            html += '    <tr>';
            html += '      <td class="gridHeaderFullBorder" style="text-align:center;width:20px;">&nbsp;</td>';
            html += '      <td class="gridHeaderFullBorder" style="text-align:left;white-space:nowrap;font-weight:bold;">Set Name</td>';
            html += '      <td class="gridHeaderFullBorder" style="text-align:left;white-space:nowrap;font-weight:bold;">Work Area</td>';
            html += '      <td class="gridHeaderFullBorder" style="text-align:left;white-space:nowrap;font-weight:bold;">System</td>';
            html += '      <td class="gridHeaderFullBorder" style="text-align:left;white-space:nowrap;font-weight:bold;">Purpose</td>';
            html += '    </tr>';

            for (var i = 0; i < dt.length; i++) {
                var set = dt[i];

                var cls = i % 2 == 0 ? 'gridBodyFullBorder' : 'gridBodyFullBorderAlt';

                html += '<tr>';
                html += '  <td class="' + cls + '" style="text-align:center;width:20px;border-left:1px solid grey;"><input type="checkbox" name="cbEditRQMT_RQMTSet" rqmtsetid="' + set.RQMTSetID + '" origsort="' + i + '" origchecked="false" onchange="popupManager.GetPopupByName(\'EditRQMT\').Opener.editRQMT_SortRQMTAssociations();"></td>';
                html += '  <td class="' + cls + '" style="text-align:left;white-space:nowrap;">' + set.RQMTSetName + '</td>';
                html += '  <td class="' + cls + '" style="text-align:left;white-space:nowrap;">' + set.WorkArea + '</td>';
                html += '  <td class="' + cls + '" style="text-align:left;white-space:nowrap;">' + set.WTS_SYSTEM + '</td>';
                html += '  <td class="' + cls + '" style="text-align:left;white-space:nowrap;">' + set.RQMTType + '</td>';
                html += '</tr>';
            }

            html += '  </table>';
            html += '</div>';

            $('#divRQMTEditRQMTSets', ic).html(html);

            editRQMT_LoadRQMTAssociations(_activeRQMTSetRQMTSystemID);
        }

        function editRQMT_LoadRQMTAssociations(RQMTSet_RQMTSystemID) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);

            PageMethods.LoadAllRQMTSetsForRQMT(rqmt.RQMTID, editRQMT_LoadRQMTAssociations_done, on_error);
        }

        function editRQMT_LoadRQMTAssociations_done(results) {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;
            var opener = thePopup.Opener;

            var dt = $.parseJSON(results);

            for (var i = 0; i < dt.length; i++) {
                var row = dt[i];

                var RQMTID = row.RQMTID;
                var RQMTSetID = row.RQMTSetID;

                var cb = $('input[name=cbEditRQMT_RQMTSet][rqmtsetid=' + RQMTSetID + ']', ic);
                cb.prop('checked', true);
                cb.attr('origchecked', 'true');
                cb.closest('tr').find('td').css('font-weight', 'bold');
            }

            editRQMT_SortRQMTAssociations();
        }

        function editRQMT_SortRQMTAssociations() {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            var cbs = $('input[name=cbEditRQMT_RQMTSet]', ic);

            var sorted = true;

            do {
                sorted = true;

                for (var i = 0; i < cbs.length - 1; i++) {
                    var cb1 = cbs[i];
                    var cb2 = cbs[i + 1];

                    var cb1checked = $(cb1).is(':checked');
                    var cb2checked = $(cb2).is(':checked');

                    var cb1origsort = parseInt($(cb1).attr('origsort'));
                    var cb2origsort = parseInt($(cb2).attr('origsort'));

                    var reverse = false;

                    if ((cb1checked && cb2checked) || (!cb1checked && !cb2checked)) { // both have same check status
                        if (cb2origsort < cb1origsort) {
                            reverse = true;
                        }
                    }
                    else if (cb2checked && !cb1checked) { // cb2 is checked but cb1 is not
                        reverse = true;
                    }

                    if (reverse) {
                        sorted = false;

                        cbs[i] = cb2;
                        cbs[i + 1] = cb1;
                        
                        var cb1tr = $(cb1).closest('tr');
                        var cb2tr = $(cb2).closest('tr');

                        cb2tr.remove();                        
                        cb2tr.insertBefore(cb1tr);
                    }
                }
            } while (!sorted);

            var changeFound = false;

            for (var i = 0; i < cbs.length; i++) {
                var cb = cbs[i];
                var tr = $(cb).closest('tr');

                var origChecked = $(cb).attr('origchecked') == 'true';
                var checkedNow = $(cb).is(':checked');

                if (checkedNow) {
                    if (origChecked) {
                        $(tr).find('td').css('color', '#000000');
                    }
                    else { // we are adding a new item
                        $(tr).find('td').css('color', 'blue');
                        changeFound = true;
                    }
                }
                else {
                    if (origChecked) { // we are about to delete an item from a set
                        $(tr).find('td').css('color', 'red');
                        changeFound = true;
                    }
                    else {
                        $(tr).find('td').css('color', '#000000');
                    }
                }
            }

            if (changeFound) {
                $('#divRQMTEditRQMTSetsLegend', ic).show();
            }
            else {
                $('#divRQMTEditRQMTSetsLegend', ic).hide();
            }
        }

        function editRQMTTabClicked(tab1name, tab2name) {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            var tab1 = $('#divRQMTEditTab' + tab1name, ic);
            var tab2 = $('#divRQMTEditTab' + tab2name, ic);
            var div1 = $('#divRQMTEdit' + tab1name, ic);
            var div2 = $('#divRQMTEdit' + tab2name, ic);

            tab1.css('font-weight', 'bold');
            tab2.css('font-weight', 'normal');

            tab1.css('background-color', '#ffffff');
            tab2.css('background-color', '#dedede');

            tab1.css('color', '#000000');
            tab2.css('color', '#666666');

            tab1.css('top', '0px');
            tab2.css('top', '2px');

            div2.hide();
            div1.show();
        }

        function editRQMT_CancelClicked() {            
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            thePopup.Opener.editRQMT_ClearActiveRQMT();
            thePopup.Close();
        }

        function editRQMT_SaveClicked() {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            var changesFound = false;

            var rqmt = getRQMTFromRQMTSetRQMTSystemID(_activeRQMTSetRQMTSystemID);

            changesFound |= $('input[name=cbEditRQMT_RQMTSet][origchecked=true]', ic).not(':checked').length > 0;
            changesFound |= $('input[name=cbEditRQMT_RQMTSet][origchecked=false]:checked', ic).length > 0;
            var txtChanges = rqmt.RQMT != $('#txtRQMTEdit', ic).val();
            changesFound |= txtChanges;

            if (txtChanges) {
                if ($('input[name=cbEditRQMT_RQMTSet]:checked', ic).length > 1) {
                    QuestionBox('Confirm Save', 'This RQMT is used in more than one RQMT Set. Continue save?', 'Yes,No', 'editRQMT_SaveConfirmed', 300, 300, thePopup.Opener);
                }
                else {
                    editRQMT_SaveConfirmed('Yes');
                }
            }
            else if (changesFound) {                
                editRQMT_SaveConfirmed('Yes');
            }
            else {
                editRQMT_CancelClicked();
            }            
        }

        function editRQMT_SaveConfirmed(answer) {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            var rqmt = getRQMTFromRQMTSetRQMTSystemID(_activeRQMTSetRQMTSystemID);

            if (answer == 'Yes') {
                var adds = $('input[name=cbEditRQMT_RQMTSet][origchecked=false]:checked', ic);
                var deletes = $('input[name=cbEditRQMT_RQMTSet][origchecked=true]', ic).not(':checked');                

                var rsAdds = '';
                for (var i = 0; i < adds.length; i++) {
                    if (i > 0) rsAdds += ',';
                    rsAdds += $(adds[i]).attr('rqmtsetid');
                }

                var rsDeletes = '';
                for (var i = 0; i < deletes.length; i++) {
                    if (i > 0) rsDeletes += ',';
                    rsDeletes += $(deletes[i]).attr('rqmtsetid');
                }

                var rqmtChanges = {};
                rqmtChanges.RQMT = $('#txtRQMTEdit', ic).val();
                rqmtChanges.RQMTID = rqmt.RQMTID;
                rqmtChanges.RQMTSetID = rqmt.RQMTSetID;
                rqmtChanges.RQMTSet_RQMTSystemID = rqmt.RQMTSet_RQMTSystemID;

                rqmtChanges.adds = rsAdds;
                rqmtChanges.deletes = rsDeletes;

                showLoadingMessage('Saving RQMT changes...', false, $(ic).find('#divRQMTEditMessagePlaceHolder')[0]);
                $('#btnRQMTEditSave').prop('disabled', true);

                rqmtChanges = JSON.stringify(rqmtChanges);

                PageMethods.SaveRQMTChanges(rqmtChanges, editRQMT_Save_done, on_error);
            }            
        }

        function editRQMT_Save_done(result) {
            var thePopup = popupManager.GetPopupByName('EditRQMT');
            var ic = thePopup.InlineContainer;

            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                editRQMT_CancelClicked();
                clearLoadingMessage($(ic).find('#divRQMTEditMessagePlaceHolder'));
                filterRQMTSets();
                executeComponentTabSearch('rqmt');
            }
            else {
                editRQMT_CancelClicked();
                clearLoadingMessage($(ic).find('#divRQMTEditMessagePlaceHolder'));
                dangerMessage('Error saving RQMT changes. ' + dt.error);
            }
        }

        function editRQMT_ClearActiveRQMT() {
            _activeRQMTSetRQMTSystemID = 0;
        }

        function openRQMTPopupFromBuilder(RQMTID, RQMTSet_RQMTSystemID, openSections, hideNonOpenSections, itemID, itemSubSection) {                        
            if (window.parent.openRQMTPopup) {
                window.parent.openRQMTPopup(RQMTID, openSections, null, null,
                    function () {
                        refreshRQMTs(true);
                    },
                    hideNonOpenSections, itemID, itemSubSection);
            }
        }

        function RQMTSetReOrdered(RQMTSetID, srcRQMTSetRQMTSystemID, tgtRQMTSetRQMTSystemID, top, indent, moveChildrenWithParent) {
            var rqmtsInSet = getAllRQMTsForRQMTSet(RQMTSetID);
            var srcIdx = _.findIndex(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID });
            var tgtIdx = _.findIndex(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID });
            var srcRQMT = _.find(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID });
            var tgtRQMT = _.find(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID });

            var srcIsParent = _.find(rqmtsInSet, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID && rqmt.RQMTSet_RQMTSystemID != srcRQMTSetRQMTSystemID });
            var tgtIsParent = _.find(rqmtsInSet, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID && rqmt.RQMTSet_RQMTSystemID != tgtRQMTSetRQMTSystemID });

            if (moveChildrenWithParent && srcIsParent) {
                var srcChildren = _.filter(rqmtsInSet, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID && rqmt.RQMTSet_RQMTSystemID != srcRQMTSetRQMTSystemID });
                var tgtChildren = _.filter(rqmtsInSet, function (rqmt) { return rqmt.ParentRQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID && rqmt.RQMTSet_RQMTSystemID != tgtRQMTSetRQMTSystemID });

                // remove the src item and all it's children
                rqmtsInSet = _.reject(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID || rqmt.ParentRQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID });
                tgtIdx = _.findIndex(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID }); // get tgt idx again if item is removed

                if (top) {
                    // move all the src + children before this item
                    rqmtsInSet.splice(tgtIdx, 0, srcRQMT);

                    for (var i = 0; srcChildren != null && i < srcChildren.length; i++) {
                        rqmtsInSet.splice(tgtIdx + (i + 1), 0, srcChildren[i]);
                    }
                }
                else {
                    tgtIdx += tgtChildren != null ? tgtChildren.length : 0;

                    rqmtsInSet.splice(tgtIdx + 1, 0, srcRQMT);

                    for (var i = 0; srcChildren != null && i < srcChildren.length; i++) {
                        rqmtsInSet.splice((tgtIdx + 1) + (i + 1), 0, srcChildren[i]);
                    }
                }
            }
            else {
                // first sort items by removing the moving item and then placing it in the correct spot
                rqmtsInSet = _.reject(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == srcRQMTSetRQMTSystemID });
                tgtIdx = _.findIndex(rqmtsInSet, function (rqmt) { return rqmt.RQMTSet_RQMTSystemID == tgtRQMTSetRQMTSystemID }); // get tgt idx again if item is removed
                if (top) {
                    rqmtsInSet.splice(tgtIdx, 0, srcRQMT);
                }
                else {
                    rqmtsInSet.splice(tgtIdx + 1, 0, srcRQMT);
                }

                // now set the source item's indent status
                srcRQMT.ParentRQMTSet_RQMTSystemID = indent ? 10000 : 0;
            }

            // now reset all the parents and children based on where they are in the list
            var order = refreshRQMTSetOrdering(rqmtsInSet);

            PageMethods.SaveRQMTSetOrdering(order, function (result) { }, on_error);

            filterRQMTSets_done(rqmtsInSet, RQMTSetID, true);
        }

        function refreshRQMTSetOrdering(rqmtsInSet) {
            var lastParentLevelID = -1;
            var currentParentIdx = 1;
            var currentChildIdx = 1;
            for (var i = 0; i < rqmtsInSet.length; i++) {
                var rqmt = rqmtsInSet[i];
                if (i == 0) rqmt.ParentRQMTSet_RQMTSystemID = 0; // the first item in the set cannot be a child item
                var isParentLevel = rqmt.ParentRQMTSet_RQMTSystemID == 0;

                if (isParentLevel) {
                    rqmt.OutlineIndex = currentParentIdx++;
                    lastParentLevelID = rqmt.RQMTSet_RQMTSystemID;
                    currentChildIdx = 1;

                    rqmt.OutlineIndexString = rqmt.OutlineIndex + '.0';
                }
                else {
                    rqmt.ParentRQMTSet_RQMTSystemID = lastParentLevelID;
                    rqmt.OutlineIndex = currentChildIdx++;
                    rqmt.OutlineIndexString = currentParentIdx + '.' + rqmt.OutlineIndex;
                }
            }

            var order = '';

            for (var i = 0; i < rqmtsInSet.length; i++) {
                var rqmt = rqmtsInSet[i];

                if (i > 0) {
                    order += ',';
                }

                order += rqmt.RQMTSet_RQMTSystemID + '|' + rqmt.ParentRQMTSet_RQMTSystemID + '|' + rqmt.OutlineIndex;
            }

            return order;
        }

        function editRQMTDescription(RQMTSet_RQMTSystemID, RQMTSystemRQMTDescriptionID) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);
            var desc = _.find(rqmt.RQMTDescriptionArray, function (d) { return d.RQMTSystemRQMTDescriptionID == RQMTSystemRQMTDescriptionID });

            /*
            var divDescEdit = $('#divRQMTDescriptionEdit');
            divDescEdit.attr('rqmtsetid', rqmt.RQMTSetID);
            divDescEdit.attr('rqmtsetrqmtsystemid', RQMTSet_RQMTSystemID);
            divDescEdit.attr('rqmtid', rqmt.RQMTID);
            divDescEdit.attr('rqmtsystemrqmtdescriptionid', RQMTSystemRQMTDescriptionID);
            divDescEdit.attr('editmode', RQMTSystemRQMTDescriptionID != -1 ? '1' : '0');

            var txt = divDescEdit.find('#txtRQMTDescriptionEdit');
            txt.val(desc != null ? desc.RQMTDescription : '');

            var ddl = divDescEdit.find('#ddlRQMTDescriptionEditType');
            if (desc != null) {
                ddl.val(desc.RQMTDescriptionTypeID);
            }
            else {
                ddl[0].selectedIndex = 0;
            }           

            // we call off first to make sure we aren't duplicating the event
            $('#btnRQMTDescriptionEditCancel').off('click').on('click', editRQMTDescription_Cancel);
            $('#btnRQMTDescriptionEditSave').off('click').on('click', editRQMTDescription_Save);
            $('#btnRQMTDescriptionEditDelete').off('click').on('click', editRQMTDescription_Delete);

            */
            openRQMTPopupFromBuilder(rqmt.RQMTID, RQMTSet_RQMTSystemID, '_attributes_', true, desc != null ? desc.RQMTSystemRQMTDescriptionID : null, '_descriptions_'); // note: the descriptions grid is in the attributes section
        }

        function editRQMTDescription_Delete() {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
            var ic = $(thePopup.InlineContainer);

            var RQMTSystemRQMTDescriptionID = ic.attr('rqmtsystemrqmtdescriptionid');

            QuestionBox('Confirm Delete', 'Delete this description?', 'Yes,No', 'deleteDescriptionConfirmed', 300, 300, thePopup.Opener, RQMTSystemRQMTDescriptionID);
        }

        function deleteDescriptionConfirmed(answer, RQMTSystemRQMTDescriptionID) {
            if (answer == 'Yes') {
                PageMethods.DeleteRQMTDescription(RQMTSystemRQMTDescriptionID, deleteDescription_done, on_error);
            }
        }

        function deleteDescription_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
                var ic = $(thePopup.InlineContainer);
            
                var RQMTSetID = ic.attr('rqmtsetid');            

                filterRQMTSets(RQMTSetID);
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }

            editRQMTDescription_Cancel();
        }

        function editRQMTDescription_Cancel() {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');            
            thePopup.Close();

            var divDescEdit = $('#divRQMTDescriptionEdit');
            divDescEdit.attr('rqmtsetid', '');
            divDescEdit.attr('rqmtsetrqmtsystemid', '');
            divDescEdit.attr('rqmtid', '');
            divDescEdit.attr('rqmtsystemrqmtdescriptionid', '');
            divDescEdit.attr('editmode', '');

            divDescEdit.find('#txtRQMTDescriptionEdit').val('');
            divDescEdit.find('#ddlRQMTDescriptionEditType')[0].selectedIndex = 0;
            divDescEdit.find('#divRQMTDescriptionEditResults').val('');
        }

        function editRQMTDescription_Save() {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
            var ic = $(thePopup.InlineContainer);
            
            var RQMTSetID = ic.attr('rqmtsetid');            
            var RQMTID = ic.attr('rqmtid');
            var editMode = ic.attr('editmode') == '1';
            var RQMTSet_RQMTSystemID = ic.attr('rqmtsetrqmtsystemid');
            var RQMTSystemRQMTDescriptionID = ic.attr('rqmtsystemrqmtdescriptionid');

            var txt = ic.find('#txtRQMTDescriptionEdit');
            var ddl = ic.find('#ddlRQMTDescriptionEditType');
            
            PageMethods.SaveRQMTDescription(RQMTSet_RQMTSystemID, RQMTSystemRQMTDescriptionID, txt.val(), ddl.val(), editMode, editRQMTDescription_Save_done, on_error);            
        }

        function editRQMTDescription_Save_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                var RQMTDescriptionID = dt.RQMTDescriptionID;

                var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
                var ic = $(thePopup.InlineContainer);

                var RQMTSet_RQMTSystemID = ic.attr('rqmtsetrqmtsystemid');
                var txt = ic.find('#txtRQMTDescriptionEdit');

                // find out if we have a shared rqmt description on this page; if not, just update the current rqmt set, otherwise refresh entire page
                var rqmtCount = $('[rqmtdescriptionid=' + RQMTDescriptionID + ']');

                if (rqmtCount.length == 1) {
                    var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);
                    rqmt.RQMTDescription = txt.val();
                    rqmt.RQMTDescriptionID = RQMTDescriptionID;

                    $('[rqmtdescriptionid=' + RQMTDescriptionID + ']').val(txt.val());
                }
                else {
                    filterRQMTSets();
                }
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }

            editRQMTDescription_Cancel();
        }

        function openDescriptionAttachment(AttachmentID) {
            window.open('Download_Attachment.aspx?attachmentID=' + AttachmentID);
        }

        // executes a description search in the edit description popup
        function searchDescriptions() {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
            var ic = $(thePopup.InlineContainer);

            var txt = ic.find('#txtRQMTDescriptionEdit').val();

            if (txt != null && txt.trim().length > 0) {
                PageMethods.ExecuteComponentSearch('desc', txt, function (result) { searchDescriptions_done(result); }, on_error);                        
            }
            else {
                ic.find('#divRQMTDescriptionEditResults').html('No results found.');
            }
        }

        function searchDescriptions_done(result) {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
            var ic = $(thePopup.InlineContainer);

            var dt = $.parseJSON(result);
            
            if (dt.length > 0) {
                var html = '';

                for (var i = 0; i < dt.length; i++) {
                    var desc = dt[i];

                    html += '<div style="background-color:' + _newComponentBackgroundColor + ';border:1px solid ' + _newComponentBorderColor + ';color:' + _newComponentTextColor + ';border-radius:5px;margin-bottom:5px;padding:5px;cursor:pointer;opacity:.9" onmouseover="this.style.opacity=1.0" onmouseout="this.style.opacity=.9" onclick="popupManager.GetPopupByName(\'EditRQMTDescription\').Opener.description_clicked(this)">';
                    html += StripHTML('<p>' + desc.RQMTDescription + '</p>'); // the strip html function uses the jquery function $(str).text(); however if there are no tags at all, the str itself is interpreted as an html selector and stripped out, so we add some default tags to make sure it works correctly
                    html += '</div>';
                }

                ic.find('#divRQMTDescriptionEditResults').html(html);
            }
            else {
                ic.find('#divRQMTDescriptionEditResults').html('No results found.');
            }
        }

        function description_clicked(div) {
            var thePopup = popupManager.GetPopupByName('EditRQMTDescription');
            var ic = $(thePopup.InlineContainer);

            ic.find('#txtRQMTDescriptionEdit').val($(div).html());
        }

        function editRQMTAttributeClicked(RQMTSet_RQMTSystemID) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);

            var divDescEdit = $('#divRQMTAttrEdit');
            divDescEdit.attr('RQMTSet_RQMTSystemID', RQMTSet_RQMTSystemID);

            $('#ddlRQMTAttrEditStage').val(rqmt.RQMTStageID != null ? rqmt.RQMTStageID : 0);
            $('#ddlRQMTAttrEditCriticality').val(rqmt.CriticalityID != null ? rqmt.CriticalityID : 0);
            $('#ddlRQMTAttrEditStatus').val(rqmt.RQMTStatusID != null ? rqmt.RQMTStatusID : 0);
            
            $('[name=rdoRQMTAttrEditAccepted][value=1]').prop('checked', rqmt.RQMTAccepted);
            $('[name=rdoRQMTAttrEditAccepted][value=0]').prop('checked', !rqmt.RQMTAccepted);

            $('#btnRQMTAttrEditCancel').off('click').on('click', editRQMTAttribute_Cancel);
            $('#btnRQMTAttrEditSave').off('click').on('click', editRQMTAttribute_Save);

            var openPopup = popupManager.AddPopupWindow('EditRQMTAttr', 'Edit RQMT Attribute' + ' - ' + (rqmt.RQMT.length > 50 ? rqmt.RQMT.substring(0, 50) + '...' : rqmt.RQMT), null, 200, 300, 'PopupWindow', this, false, 'divRQMTAttrEdit');            
            if (openPopup) openPopup.Open();            
        }

        function editRQMTAttribute_Cancel() {
            var thePopup = popupManager.GetPopupByName('EditRQMTAttr');            
            thePopup.Close();

            var divDescEdit = $('#divRQMTAttrEdit');
            divDescEdit.attr('RQMTSet_RQMTSystemID', '0');
        }

        function editRQMTAttribute_Save() {
            var thePopup = popupManager.GetPopupByName('EditRQMTAttr');            
            var ic = $(thePopup.InlineContainer);

            var RQMTSet_RQMTSystemID = ic.attr('RQMTSet_RQMTSystemID');

            var RQMTStageID = $('#ddlRQMTAttrEditStage', ic).val();
            var CriticalityID = $('#ddlRQMTAttrEditCriticality', ic).val();
            var RQMTStatusID = $('#ddlRQMTAttrEditStatus', ic).val();
            var RQMTAccepted = $('[name=rdoRQMTAttrEditAccepted][value=1]', ic).is(':checked');                       

            PageMethods.SaveRQMTAttributes(RQMTSet_RQMTSystemID, RQMTStageID, CriticalityID, RQMTStatusID, RQMTAccepted, editRQMTAttribute_Save_done, on_error);
        }

        function editRQMTAttribute_Save_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                var thePopup = popupManager.GetPopupByName('EditRQMTAttr');
                var ic = $(thePopup.InlineContainer);

                var RQMTSet_RQMTSystemID = ic.attr('RQMTSet_RQMTSystemID');

                var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);
                var RQMTSetID = rqmt.RQMTSetID;
                var RQMTSystemID = rqmt.RQMTSystemID;

                // find out if the RQMTSystem is used anywhere else on the page; if not, just update the current rqmt set, otherwise refresh entire page
                var matchingSystems = $('tr[id=trRQMT][rqmtsystemid=' + RQMTSystemID + ']')

                if (matchingSystems.length == 1) {
                    filterRQMTSets(RQMTSetID);
                }
                else {
                    filterRQMTSets();
                }
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }

            editRQMTAttribute_Cancel();
        }

        function editRQMTBase(RQMTID) {
            var rqmt = getComponentFromSearchResults('rqmt', RQMTID);

            if (rqmt == null) return;

            $('#txtRQMTBaseEdit').attr('rqmtid', RQMTID);
            $('#txtRQMTBaseEdit').val(rqmt.RQMT);

            $('#btnRQMTBaseEditSave').off('click').on('click', editRQMTBase_Save);
            $('#btnRQMTBaseEditDelete').off('click').on('click', editRQMTBase_Delete);
            $('#btnRQMTBaseEditCancel').off('click').on('click', editRQMTBase_Cancel);

            var openPopup = popupManager.AddPopupWindow('EditRQMTBase', 'Edit RQMT', null, 300, 700, 'PopupWindow', this, false, 'divRQMTBaseEdit');            
            if (openPopup) openPopup.Open();              
        }

        function editRQMTBase_Save() {
            var thePopup = popupManager.GetPopupByName('EditRQMTBase');
            var ic = $(thePopup.InlineContainer);

            var RQMTID = ic.find('#txtRQMTBaseEdit').attr('rqmtid');
            var txt = ic.find('#txtRQMTBaseEdit').val();

            PageMethods.SaveRQMTBase(RQMTID, txt, editRQMTBase_Save_done, on_error);
        }

        function editRQMTBase_Save_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                executeComponentTabSearch('rqmt');
                filterRQMTSets();
                editRQMTBase_Cancel();   
            }
            else if (dt.exists == 'true') {
                MessageBox('A RQMT with this name already exists.');
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }            
        }

        function editRQMTBase_Cancel() {
            var thePopup = popupManager.GetPopupByName('EditRQMTBase');
            thePopup.Close();
        }

        function editRQMTBase_Delete() {
            var thePopup = popupManager.GetPopupByName('EditRQMTBase');
            var ic = $(thePopup.InlineContainer);

            var RQMTID = ic.find('#txtRQMTBaseEdit').attr('rqmtid');

            QuestionBox('Confirm Delete', 'Delete this RQMT?', 'Yes,No', 'editRQMTBase_Delete_confirmed', 300, 300, thePopup.Opener, RQMTID);
        }

        function editRQMTBase_Delete_confirmed(answer, RQMTID) {
            if (answer == 'Yes') {
                PageMethods.DeleteRQMTBase(RQMTID, editRQMTBase_Delete_done, on_error);
            }
        }

        function editRQMTBase_Delete_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                executeComponentTabSearch('rqmt');
                filterRQMTSets();
                editRQMTBase_Cancel();
            }
            else if (dt.hasdependencies == 'true') {
                MessageBox('This RQMT is used in one or more RQMT Sets and cannot be deleted.');
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }
        }

        function filterRQMTBase() {
            $('#divRQMTBaseFilter').slideToggle(100);          
        }

        function rqmtAssociationClicked(WTS_SYSTEM_SUITEID, WTS_SYSTEMID, WorkAreaID, RQMTTypeID) {
            _defaultSystemSuiteID = WTS_SYSTEM_SUITEID;
            _defaultSystemID = WTS_SYSTEMID;
            _defaultWorkAreaID = WorkAreaID;
            _defaultRQMTTypeID = RQMTTypeID;
            
            $('#txtRQMTSetName').val('');

            _initComplete = false;
           
            initDefaults();
        }

        function rqmtToggleAssociations(i) {
            var div = $('#divRQMTAssociations_' + i);

            if (div.is(':visible')) {
                div.hide();
            }
            else {
                div.show();
            }
        }

        function editRQMTFunctionalityContextClicked(div) {
            div = $(div);
            var selected = div.attr('funcselected') == '1';

            if (selected) {
                div.attr('funcselected', '0');
                div.css('background-color', _newComponentBackgroundColor);

            }
            else {
                div.attr('funcselected', '1');
                div.css('background-color', _funcSelectedBackgroundColor);
            }

            var theTD = div.closest('td');
            var selections = theTD.find('[id=divrqmtsetfunctionality][funcselected=1]');

            if (selections.length > 0) {
                $('[id=divrqmtsetselectedfunctionalitieslabel][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'none');
                $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'inline-block');                
            }
            else {
                $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'none');
                $('[id=divrqmtsetselectedfunctionalitieslabel][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'inline-block');
            }
        }

        function clearRQMTSetFunctionalitySelectedItems(div) {
            div = $(div);
            var theTD = div.closest('td');
            theTD.find('[id=divrqmtsetfunctionality]').attr('funcselected', '0');
            theTD.find('[id=divrqmtsetfunctionality]').css('background-color', _newComponentBackgroundColor);
            theTD.find('[id=divrqmtsetfunctionality]').css('color', _newComponentTextColor);
            $('[id=divclearrqmtsetselectedfunctionalities][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'none');
            $('[id=divrqmtsetselectedfunctionalitieslabel][rqmtsetid=' + div.attr('RQMTSetID') + ']').css('display', 'inline-block');
        }

        function editRQMTFunctionalityClicked(RQMTSetID, RQMTSet_RQMTSystemID, RQMTSetFunctionalityID, OriginalFunctionalityID) {
            // we have two modes, first editing functionality at the set level, and second adding/editing functionality at the rqmt level
            // when editing at the set level, we can only set complexity and cannot change the functionality type or select a new one
            // (functionality is only available at the set level AFTER it has been chosen at the rqmt level)
            var rqmtFuncEditMode = RQMTSet_RQMTSystemID != null && RQMTSet_RQMTSystemID > 0;

            var rqmt = RQMTSet_RQMTSystemID > 0 ? getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID) : null;
            var rqmtSet = getRQMTSet(RQMTSetID);
            var rqmtSetFunc = rqmtFuncEditMode ? null : _.find(rqmtSet.Functionalities, function (f) { return f.RQMTSetFunctionalityID == RQMTSetFunctionalityID });

            var divRQMTFuncEdit = $('#divRQMTFuncEdit');            

            var ddlSelectedFunctionality = $('#ddlSelectedFunctionality'); // ddl for set selection
            var divSelectedFunctionalities = $('#divSelectedFunctionalities'); // list of all funcs chosen by the rqmt
            var divAvailableFunctionalities = $('#divAvailableFunctionalities'); // checkbox container to select func
            var divAvailableFunctionalitiesGlass = $('#divAvailableFunctionalitiesGlass');
            var divRQMTFuncEditQuickSelect = $('#divRQMTFuncEditQuickSelect'); // quick select for adding funcs

            divRQMTFuncEdit.attr('RQMTSetID', RQMTSetID);
            divRQMTFuncEdit.attr('RQMTSet_RQMTSystemID', RQMTSet_RQMTSystemID != null ? RQMTSet_RQMTSystemID : 0);
            divRQMTFuncEdit.attr('RQMTSetFunctionalityID', RQMTSetFunctionalityID != null ? RQMTSetFunctionalityID : 0);

            // ddl used by the set-level func edit
            ddlSelectedFunctionality.html(_functionalitySelectOptions);
            if (OriginalFunctionalityID != null) {
                ddlSelectedFunctionality.val(OriginalFunctionalityID);
            }

            // text list and checkboxes used by rqmt func edit
            var availableBtns = '<div style="position:fixed;width:200px;border-bottom:1px solid #000000;text-align:right;background-color:#eeeeee;"><img src="images/icons/close_button_red.png" style="margin:2px;cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities(); return false;"></div><br />';
            divAvailableFunctionalities.html(availableBtns + _functionalityCheckBoxOptions);
            divAvailableFunctionalities.hide();
            divAvailableFunctionalitiesGlass.hide();
            
            if (rqmtFuncEditMode) {
                // populate the select functionalities list and also check the correct checkboxes for the available list
                if (rqmt.Functionalities.length > 0) {
                    var prevhtml = '';

                    for (var i = 0; i < rqmt.Functionalities.length; i++) {
                        var func = rqmt.Functionalities[i];
                        
                        if (prevhtml.length > 0) prevhtml += ', ';

                        // the rqmt functionalities don't contain the full fnc name, so we get that from the set
                        var theFunctionality = _.find(rqmtSet.Functionalities, function (f) { return f.RQMTSetFunctionalityID == func.RQMTSetFunctionalityID });

                        prevhtml += '<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities(); return false;"><u>' + theFunctionality.Functionality + '</u></span>';

                        divAvailableFunctionalities.find('input[value=' + theFunctionality.FunctionalityID + ']').prop('checked', true);
                    }

                    prevhtml += '&nbsp;<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ClearAllAvailableFunctionalities();"><u>(CLEAR ALL)</u></span>';                

                    divSelectedFunctionalities.html(prevhtml);
                }
                else {
                    divSelectedFunctionalities.html('<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities();"><u>NONE</u> (click to set)</span>');
                }                

                // quick select list
                if (rqmtSet.Functionalities.length > 0) {
                    var qshtml = '';

                    for (var i = 0; i < rqmtSet.Functionalities.length; i++) {
                        var func = rqmtSet.Functionalities[i];

                        qshtml += '<div style="padding:3px;display:inline-block;border:1px solid ' + _newComponentBorderColor + ';background-color:' + _newComponentBackgroundColor + ';color:' + _newComponentTextColor + ';margin:3px;border-radius:5px;cursor:pointer;white-space:nowrap;" onmouseover="this.style.border=\'1px solid gray\'; this.style.color=\'gray\';" onmouseout="this.style.border=\'1px solid ' + _newComponentBorderColor + '\'; this.style.color=\'' + _newComponentTextColor + '\'" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_QuickSelectionClicked(' + func.FunctionalityID + ');">';
                        qshtml += func.Functionality;
                        qshtml += '</div>';
                    }

                    qshtml = '<div style="display:inline-block;vertical-align:top;width:100px;"><b>Previous<br />Set Selections:</b></div><div style="display:inline-block;vertical-align:top;width:350px;">' + qshtml + '</div>';
                    divRQMTFuncEditQuickSelect.html(qshtml);
                    divRQMTFuncEditQuickSelect.show();
                }
                else {
                    divRQMTFuncEditQuickSelect.hide();
                }

                divSelectedFunctionalities.show();

                // hide things needed only for the set-level functionality popup
                $('#btnRQMTFuncEditDelete').hide();
                ddlSelectedFunctionality.hide();
                $('#divRQMTFuncComplexityRow').hide();
                $('#spanRQMTFuncEditDeleteExplanation').hide();
                var tbd = $('#ddlRQMTFuncComplexity option:contains(TBD)').attr('value');
                $('#ddlRQMTFuncComplexity').val(tbd);
                $('#txtRQMTFuncJustification').val('');
            }
            else {
                ddlSelectedFunctionality.show();
                ddlSelectedFunctionality.prop('disabled', true); // at set level, we don't allow changing of func
                divSelectedFunctionalities.hide();
                divRQMTFuncEditQuickSelect.hide();

                $('#divRQMTFuncComplexityRow').show();                

                // only allow delete if the functionality is not in use in this set
                var funcInUse = false;                    
                var rqmts = getAllRQMTsForRQMTSet(RQMTSetID);                    
                for (var i = 0; !funcInUse && i < rqmts.length; i++) {
                    for (var f = 0; f < rqmts[i].Functionalities.length; f++) {
                        if (rqmts[i].Functionalities[f].RQMTSetFunctionalityID == RQMTSetFunctionalityID) {
                            funcInUse = true;
                            break;
                        }
                    }
                }

                $('#btnRQMTFuncEditDelete').show();

                if (funcInUse) {
                    $('#btnRQMTFuncEditDelete').prop('disabled', true);
                    $('#spanRQMTFuncEditDeleteExplanation').show();
                }
                else {
                    $('#btnRQMTFuncEditDelete').prop('disabled', false);
                    $('#spanRQMTFuncEditDeleteExplanation').hide();
                }

                $('#ddlRQMTFuncComplexity').html(_complexityOptions);
                if (rqmtSetFunc.RQMTComplexityID > 0) {
                    $('#ddlRQMTFuncComplexity').val(rqmtSetFunc.RQMTComplexityID);
                    $('#txtRQMTFuncJustification').val(rqmtSetFunc.Justification);
                }
                else {
                    var tbd = $('#ddlRQMTFuncComplexity option:contains(TBD)').attr('value');
                    $('#ddlRQMTFuncComplexity').val(tbd);
                    $('#txtRQMTFuncJustification').val('');
                }

                var compText = $('#ddlRQMTFuncComplexity').find('option:selected').text();
                if (compText == 'L' || compText == 'XL' || compText == 'XXL') {
                    $('#txtRQMTFuncJustification').css('opacity', '1.0');
                    $('#txtRQMTFuncJustification').prop('disabled', false);
                    $('#spanRQMTFuncEditJustificationReq').show();
                }
                else {
                    $('#txtRQMTFuncJustification').css('opacity', '.66');
                    $('#txtRQMTFuncJustification').prop('disabled', true);
                    $('#spanRQMTFuncEditJustificationReq').hide();
                }
                
            }           

            $('#btnRQMTFuncEditCancel').off('click').on('click', editRQMTFunc_Cancel);
            $('#btnRQMTFuncEditSave').off('click').on('click', editRQMTFunc_Save);
            $('#btnRQMTFuncEditDelete').off('click').on('click', editRQMTFunc_Delete);
            $('#ddlRQMTFuncComplexity').off('change').on('change', editRQMTFunc_Complexity_Changed);
            divAvailableFunctionalitiesGlass.off('click').on('click', editRQMTFunc_ToggleAvailableFunctionalities);
            divAvailableFunctionalities.find('input:checkbox').off('click').on('click', editRQMTFunc_AvailableFunctionalityClicked);

            // try to account for 4 per row, 50px per row
            var ht = 125 + (parseInt(1 + rqmtSet.Functionalities.length / 4) * 60);
            if (!rqmtFuncEditMode) {
                ht = 110 + 150; // allow for complexity ddl and text box for justification (2 extra rows)
            }

            var title = '';

            if (rqmtFuncEditMode) {
                title = RQMTSetFunctionalityID != null ? 'Edit' : 'Add';
                title += ' RQMT Functionality - ';
                title += (rqmt.RQMT.length > 30 ? rqmt.RQMT.substring(0, 30) + '...' : rqmt.RQMT);
            }
            else {
                title = 'Edit RQMT Set Functionality - ' + $('#ddlRQMTFuncEdit').find('option[value=' + OriginalFunctionalityID + ']').text();
            }

            var openPopup = popupManager.AddPopupWindow('EditRQMTFunc', title, null, ht, 500, 'PopupWindow', this, false, 'divRQMTFuncEdit');            
            if (openPopup) openPopup.Open();            
        }

        function editRQMTFunc_ToggleAvailableFunctionalities() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var divAvailableFunctionalities = $('#divAvailableFunctionalities', ic);            
            var theGlass = $('#divAvailableFunctionalitiesGlass', ic);
            var divSelectedFunctionalities = $('#divSelectedFunctionalities', ic);

            if (divAvailableFunctionalities.is(':visible')) {
                divAvailableFunctionalities.hide();
                theGlass.hide();
            }
            else {
                divAvailableFunctionalities.show();
                theGlass.show();
            }
        }

        function editRQMTFunc_AvailableFunctionalityClicked() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var divAvailableFunctionalities = $('#divAvailableFunctionalities', ic);
            var cbs = divAvailableFunctionalities.find('input:checkbox');

            var divSelectedFunctionalities = $('#divSelectedFunctionalities', ic); // list of all funcs chosen by the rqmt

            var prevhtml = '';

            if (cbs.length > 0) {
                for (var i = 0; i < cbs.length; i++) {
                    var cb = $(cbs[i]);

                    if (cb.is(':checked')) {
                        if (prevhtml != '') prevhtml += ', ';

                        var func = cb.next('span').html();
                        prevhtml += '<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities(); return false;"><u>' + func + '</u></span>';
                    }                    
                }
            }

            if (prevhtml == '') {
                prevhtml = '<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities();"><u>NONE</u> (click to set)</span>';
            }
            else {
                prevhtml += '&nbsp;<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ClearAllAvailableFunctionalities();"><u>(CLEAR ALL)</u></span>';                
            }

            divSelectedFunctionalities.html(prevhtml);
        }

        function editRQMTFunc_ClearAllAvailableFunctionalities() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var divAvailableFunctionalities = $('#divAvailableFunctionalities', ic);
            var cbs = divAvailableFunctionalities.find('input:checkbox');

            var divSelectedFunctionalities = $('#divSelectedFunctionalities', ic); // list of all funcs chosen by the rqmt

            var prevhtml = '';

            if (cbs.length > 0) {
                for (var i = 0; i < cbs.length; i++) {
                    $(cbs[i]).prop('checked', false);                 
                }
            }

            if (prevhtml == '') {
                prevhtml = '<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities();"><u>NONE</u> (click to set)</span>';
            }       

            divSelectedFunctionalities.html(prevhtml);
        }

        function editRQMTFunc_QuickSelectionClicked(funcID) {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var divAvailableFunctionalities = $('#divAvailableFunctionalities', ic);
            var divSelectedFunctionalities = $('#divSelectedFunctionalities', ic); // list of all funcs chosen by the rqmt

            var cbs = divAvailableFunctionalities.find('input:checkbox');
            var cb = divAvailableFunctionalities.find('input:checkbox[value=' + funcID + ']');
            cb.prop('checked', true);

            var prevhtml = '';

            if (cbs.length > 0) {
                for (var i = 0; i < cbs.length; i++) {
                    var cb = $(cbs[i]);

                    if (cb.is(':checked')) {
                        if (prevhtml != '') prevhtml += ', ';

                        var func = cb.next('span').html();
                        prevhtml += '<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ToggleAvailableFunctionalities(); return false;"><u>' + func + '</u></span>';
                    }                    
                }
            }    

            prevhtml += '&nbsp;<span style="cursor:pointer;" onclick="popupManager.GetPopupByName(\'EditRQMTFunc\').Opener.editRQMTFunc_ClearAllAvailableFunctionalities();"><u>(CLEAR ALL)</u></span>';                

            divSelectedFunctionalities.html(prevhtml);
        }

        function editRQMTFunc_Complexity_Changed() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var compText = $('#ddlRQMTFuncComplexity', ic).find('option:selected').text(); 

            if (compText == 'L' || compText == 'XL' || compText == 'XXL') {
                $('#txtRQMTFuncJustification', ic).css('opacity', '1.0');
                $('#txtRQMTFuncJustification', ic).prop('disabled', false);
                $('#spanRQMTFuncEditJustificationReq', ic).show();
            }
            else {
                $('#txtRQMTFuncJustification', ic).css('opacity', '.66');
                $('#txtRQMTFuncJustification', ic).prop('disabled', true);
                $('#txtRQMTFuncJustification', ic).val('');
                $('#spanRQMTFuncEditJustificationReq', ic).hide();
            }
        }

        function editRQMTFunc_Cancel() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            thePopup.Close();

            var divRQMTFuncEdit = $('#divRQMTFuncEdit');
            divRQMTFuncEdit.attr('RQMTSetID', '0');
            divRQMTFuncEdit.attr('RQMTSet_RQMTSystemID', '0');
            divRQMTFuncEdit.attr('RQMTSetFunctionalityID', '0');
            divRQMTFuncEdit.attr('OriginalFunctionalityID', '0');

            var divRQMTFuncEditQuickSelect = $('#divRQMTFuncEditQuickSelect');
            divRQMTFuncEditQuickSelect.html('');
        }

        function editRQMTFunc_Save() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var RQMTSetID = ic.attr('RQMTSetID');
            var RQMTSet_RQMTSystemID = ic.attr('RQMTSet_RQMTSystemID');
            var RQMTSetFunctionalityID = ic.attr('RQMTSetFunctionalityID');                      

            var ComplexityID = $('#ddlRQMTFuncComplexity', ic).val();
            var Justification = $('#txtRQMTFuncJustification', ic).val();
            var divAvailableFunctionalities = $('#divAvailableFunctionalities', ic);
            var selectedRQMTFunctionalities = '';
            if (RQMTSet_RQMTSystemID > 0) { // individual rqmt popup
                FunctionalityID = 0;
                ComplexityID = 0;
                Justification = null;

                var cbs = divAvailableFunctionalities.find('input:checkbox:checked');
                for (var i = 0; i < cbs.length; i++) {
                    if (i > 0) selectedRQMTFunctionalities += ',';
                    selectedRQMTFunctionalities += $(cbs[i]).attr('value');
                }
            }
            else { // rqmt set popup
                var compText = $('#ddlRQMTFuncComplexity', ic).find('option:selected').text();
                if ((compText == 'L' || compText == 'XL' || compText == 'XXL') && (Justification == null || Justification.trim().length == 0)) {
                    $('#txtRQMTFuncJustification', ic).css('border', '1px solid red');
                    if (errors.length > 0) errors += ' ';
                    errors += 'Justification is required.';
                }
            }

            var errors = '';

            if (errors.length > 0) {
                dangerMessage(errors, null, null, ic[0]);
                return;
            }

            PageMethods.SaveRQMTFunctionality(RQMTSetID, RQMTSet_RQMTSystemID, selectedRQMTFunctionalities, RQMTSetFunctionalityID, 0, ComplexityID, Justification, editRQMTFunc_Save_done, on_error);
        }

        function editRQMTFunc_Delete() {
            var thePopup = popupManager.GetPopupByName('EditRQMTFunc');            
            var ic = $(thePopup.InlineContainer);

            var RQMTSetID = ic.attr('RQMTSetID');
            var RQMTSet_RQMTSystemID = ic.attr('RQMTSet_RQMTSystemID');
            var RQMTSetFunctionalityID = ic.attr('RQMTSetFunctionalityID');

            PageMethods.DeleteRQMTFunctionality(RQMTSetID, RQMTSet_RQMTSystemID, RQMTSetFunctionalityID, editRQMTFunc_Save_done, on_error);
        }

        function editRQMTFunc_Save_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success == 'true') {
                var thePopup = popupManager.GetPopupByName('EditRQMTFunc');

                if (thePopup != null) {
                    var ic = $(thePopup.InlineContainer);

                    var RQMTSet_RQMTSystemID = ic.attr('RQMTSet_RQMTSystemID');
                    var RQMTSetID = ic.attr('RQMTSetID');

                    filterRQMTSets(RQMTSetID);

                    editRQMTFunc_Cancel();
                }
                else { //this means we dragged a function from one row to another
                    filterRQMTSets(dt.RQMTSetID);
                }                
            }
            else if (dt.error != '') {
                MessageBox(dt.error);
            }            
        }

        function RQMTFunctionalityUsageClicked(month, RQMTSet_RQMTSystemID, theDiv) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);

            var monthSelected = rqmt['Month_' + month] != null && (rqmt['Month_' + month] + '').toLowerCase() == 'true';
            monthSelected = !monthSelected;

            rqmt['Month_' + month] = monthSelected;
            

            if (theDiv == null) { // if we call this programmatically, the div could be null
                // not implemented yet
            }

            // normally we redraw the grid after updates, but since this is such a simple toggle, we just do it here and skip the full redraw
            if (rqmt['Month_' + month]) {
                $(theDiv).css('background-color', _newComponentBackgroundColor);
                $(theDiv).css('color', _newComponentTextColor);
                $(theDiv).css('border', '1px solid ' + _newComponentBorderColor);
                $(theDiv).css('font-weight', 'bold');
                $(theDiv).css('opacity', '.85');
                theDiv.onmouseout = function () { this.style.opacity = .85; };
            }
            else {
                $(theDiv).css('background-color', '#ffffff');
                $(theDiv).css('color', '#999999');
                $(theDiv).css('border', '1px solid #999999');
                $(theDiv).css('font-weight', 'bold');
                $(theDiv).css('opacity', '.66');
                theDiv.onmouseout = function () { this.style.opacity = .66; };
            }

            var td = $('tr[id=trRQMTSetUsage][rqmtsetid=' + rqmt.RQMTSetID + ']').find('td[monthcell=1]');
            if (td != null) {
                $(td).html(GetRQMTSetUsageMonthString(rqmt.RQMTSetID, null));
            }

            PageMethods.UpdateRQMTSetRQMTSystemUsage(RQMTSet_RQMTSystemID, month, monthSelected, UpdateRQMTSetRQMTSystemUsage_done, on_error);
        }

        function ToggleAllRQMTFunctionalityUsage(RQMTSet_RQMTSystemID, theToggleAllDiv) {
            var rqmt = getRQMTFromRQMTSetRQMTSystemID(RQMTSet_RQMTSystemID);

            var firstMonthSelected = rqmt['Month_1'] != null && (rqmt['Month_1'] + '').toLowerCase() == 'true';
            
            var allYearSelected = !firstMonthSelected;

            for (var m = 1; m <= 12; m++) {
                var monthSelected = rqmt['Month_' + m] != null && (rqmt['Month_' + m] + '').toLowerCase() == 'true';

                if (monthSelected != allYearSelected) {
                    RQMTFunctionalityUsageClicked(m, RQMTSet_RQMTSystemID, $(theToggleAllDiv).parent().find('div[month=' + m + ']'));
                }
            }
        }

        function UpdateRQMTSetRQMTSystemUsage_done(result) {
            var dt = $.parseJSON(result);

            if (dt.success != 'true') {
                dangerMessage('Error updating RQMT usage.');
            }
        }

        function GetRQMTSetUsageMonthString(RQMTSetID, dt) {
            if (dt == null) {
                dt = getAllRQMTsForRQMTSet(RQMTSetID);
            }

            var html = '';

            for (var m = 1; m <= 12; m++) {
                var month = new moment(new Date((m < 10 ? '0' : '') + m + '/01/2000'));
                var monthWidth = true ? '28px' : '12px';
                var monthSelected = _.find(dt, function (rqmt) { return (rqmt['Month_' + m] != null && (rqmt['Month_' + m] + '').toLowerCase() == 'true') }) != null;
                var monthName = true ? month.format('MMMM').substring(0, 3).toUpperCase() : month.format('MMM').substring(0, 1).toUpperCase();
                var monthBG = monthSelected ? _newComponentBackgroundColor : '#ffffff';
                var monthColor = monthSelected ? _newComponentTextColor : '#999999';
                var monthBorder = monthSelected ? _newComponentBorderColor : '#999999';
                var monthOpacity = monthSelected ? .85 : .66;

                html += '<div month="' + m + '" style="color:' + monthColor + '; background-color:' + monthBG + '; border:1px solid ' + monthBorder + ';opacity:' + monthOpacity + ';text-align:center;line-height:14px;border-radius:3px;cursor:pointer;display:inline-block;width:' + monthWidth + ';height:12px;font-size:10px;font-weight:bold;margin-right:1px;">' + monthName + '</div>';
            }

            return html;
        }

        function DefectLinkClicked(RQMTSetID, RQMTID, WTS_SYSTEMID) {
            var nTitle = 'RQMT Defect(s) & Impact';
            var nHeight = 600, nWidth = 1250;
            var nURL = 'RQMTDefectsImpact_Grid.aspx?RQMT_ID=' + RQMTID + '&SYSTEM_ID=' + WTS_SYSTEMID;
            var openPopup = popupManager.AddPopupWindow('RQMTDefects', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);
            openPopup.onClose = function () { refreshRQMTs(); };

            if (openPopup) openPopup.Open();
        }
        
        ///////////////////////////////////////////////////////
        // COMPONENTS SEARCH / ADD NEW
        //
        // NOTES: Components were designed as "things from the right pane that could be dragged onto the RQMT's on the left". For example, if one wanted to drag systems, work areas, descriptions, etc... Each
        // of these were components with different search properties, display properties, and behaviors, reflected in the ComponentType obj. This design changed slightly when the new RQMTBuilder page was built;
        // now only RQMTs are on the right, and the need for components is less. Moreover, a few custom behaviors have been added to the search results that need to be placed in the ComponentType model rather
        // than handled "inline" in the search results (they are behaviors that are specific to RQMTs and wouldn't be applicable to other types of components). If we add more components, this custom behavior
        // needs to be addressed.
        /////////////////////////////////////////////////////// 
        function ComponentType(type, name, ds, dt, txtProp, idProp, showInNewGrid, showInExistingGrid, fkIdProp, allowNewCreation, maxAllowed,
            searchResultHeaders, searchResultColumns, ddlLabel, ddlOptions, ddlIdProp, ddlTextProp, ddlShowFirstLetterWithTxtProp, allowBlankSearch,
            keepInSync, alwaysConfirmNewEntries) {
            this.type = type;
            this.name = name;
            this.ds = ds;
            this.dt = dt;
            this.txtProp = txtProp;
            this.idProp = idProp;
            this.showInNewGrid = showInNewGrid;
            this.showInExistingGrid = showInExistingGrid;
            this.fkIdProp = fkIdProp;
            this.allowNewCreation = allowNewCreation;
            this.maxAllowed = maxAllowed;
            this.searchResultHeaders = searchResultHeaders;
            this.searchResultColumns = searchResultColumns;
            this.ddlLabel = ddlLabel;
            this.ddlOptions = ddlOptions;
            this.ddlIdProp = ddlIdProp;
            this.ddlTextProp = ddlTextProp;
            this.ddlShowFirstLetterWithTxtProp = ddlShowFirstLetterWithTxtProp;
            this.allowBlankSearch = allowBlankSearch; // if true, the list will be searched even when nothing is entered in the text box (all items will be displayed - use for master-data items)
            this.keepInSync = keepInSync; // if true, causes page to check other rqmtsystems for use of the same item, and if so, makes sure the text matches between them if one is edited
            this.alwaysConfirmNewEntries = alwaysConfirmNewEntries;
            
            this.isAttribute = false;
            this.hasSearchTab = searchResultHeaders != null;
            this.allowAddInNewRQMTs = true;            
            this.componentSearchOnClick = null;
            this.filterOnClick = null;
            this.detailColumnOnClick = null; // if not null, a detail icon column will display in the search results, allowing users to click on it
            this.componentSearchTextLabel = name;
            this.componentSearchTextRightLabel = null;
        }

        function initDefaultComponents() {
            _compTypes = [];

            var rqmtCompType = new ComponentType('rqmt', 'RQMT', 'RQMT', 'dtRQMTs', 'RQMT', 'RQMTID', true, true, 'RQMTID', true, 1,
                '#,RQMT #,RQMT', '#,RQMT_ID,RQMT', null, null, null, null, false, false,
                true, false);
            //rqmtCompType.componentSearchOnClick = 'editRQMTBase';
            rqmtCompType.filterOnClick = 'filterRQMTBase';
            rqmtCompType.detailColumnOnClick = 'showRQMTBaseDetail';
            rqmtCompType.componentSearchTextLabel = 'Search for existing RQMTs or create new:&nbsp;' +
                '<div class="tooltip" style="border-bottom:0px;cursor:pointer;">' +
                '<img src="images/icons/help.png" width="12" height="12">' +

                '<div class="tooltiptext" style="width:500px;left:-300px;text-align:left;font-weight:normal;">' +
                '<b>To search:</b><br />' +
                '  <ol>' +
                '    <li>Enter one or more RQMTs</li>' +
                '    <li style="margin-top:5px;">Max 500 characters per RQMT</li>' +
                '    <li style="margin-top:5px;">May search by text <i>or</i> number (#)</li>' +
                '    <li style="margin-top:5px;">May search for multiple RQMTs at once by separating search strings with a carriage return</li>' +
                '  </ol><br />' +
                '<b>To create new:</b><br />' +
                '  <ol>' +
                '    <li>Enter one or more RQMTs</li>' +
                '    <li style="margin-top:5px;">Separate multiple RQMTs with a carriage return</li>' +
                '    <li style="margin-top:5px;">Max 500 characters per RQMT</li>' +
                '    <li style="margin-top:5px;">Newly added RQMTs will be immediately added to visible RQMT Sets unles the "Quick add to set" checkbox is <i>unchecked</i>, or "Quick add" is disabled in a set by clicking the green check icon (<img src="images/icons/check.png" width="10" height="10">) in the RQMT Set options.</li>' +
                '    <li style="margin-top:5px;">Add a ">" character before a RQMT to indicate that the RQMT should be indented under the RQMT above it (note that only one indent level is allowed for RQMTs)</li>' +
                '  </ol>' +
                '</div>' +

                '</div>';
            rqmtCompType.componentSearchTextRightLabel = '';


            _compTypes.push(rqmtCompType);          

            /*
            var compType = new ComponentType('attr', 'Attributes');
            compType.isAttribute = true;
            compType.showInNewGrid = true;
            _compTypes.push(compType);
            */
        }

        function initComponentTabs() {
            var wm = new WindowManager();
            var ht = wm.getHeight();
            // account for header bar, button bar, and tabs
            ht -= 105;
            
            $('.tabDiv').height(ht);
            //$('#divExistingRQMTSystemsGrid').height(ht);

            var tabdivs = $('.tabDiv[comptab]');

            $.each(tabdivs, function (idx, tab) {
                var compType = $(tab).attr('comptype');
                var compTypeObj = getComponentType(compType);

                var html = '';

                // wrapper
                html += '<div style="padding:3px;text-align:left;">';

                // search / new entry box
                html += '<div id="divcompsearch_' + compType + '" style="position:relative;margin-bottom:5px;">';
                html += '  <b>' + compTypeObj.componentSearchTextLabel + '</b><br />';
                html += '<div id="divcompsearchrightlabel_' + compType + '" style="position:absolute;top:0px;right:5%;">' + compTypeObj.componentSearchTextRightLabel + '</div>';             
                html += '  <textarea id="txtcompsearch_' + compType + '" compsearch="true" style="background-color:#f5f6ce;width:95%;height:100px;margin-bottom:5px;" onkeyup="_debouncedExecuteComponentTabSearch(\'' + compType + '\'); parseComponentSearchEntry(\'' + compType + '\'); return false;"></textarea>';
                if (compTypeObj.ddlLabel != null) {
                    html += '<div id="divcompddl_' + compType + '">';
                    html += '<b>' + compTypeObj.ddlLabel + ':</b><br />';
                    html += '<select id="ddlcompddl_' + compType + '" style="background-color:#f5f6ce;width:95%;">';
                    html += compTypeObj.ddlOptions;
                    html += '</select>';
                    html += '</div><br />';
                }
                                
                html += '  <input id="btnaddnewcomp_' + compType + '" type="button" value="Create New" disabled="true" onclick="createNewComponentButtonClicked(\'' + compType + '\'); return false;">';
                html += '  <input id="btnclearnewcomp_' + compType + '" type="button" value="Clear" disabled="true" onclick="clearNewComponentSearch(\'' + compType + '\'); return false;">';
                html += '  <input id="btncancelnewcomp_' + compType + '" type="button" value="Cancel" style="display:none;" onclick="cancelNewComponentSearch(\'' + compType + '\'); return false;">';

                html += '  <div style="position:absolute;bottom:0px;right:5%;">';
                html += '    <span style="white-space:nowrap;padding-right:10px;"><input id="cbquickaddcomp_' + compType + '" type="checkbox" value="1" checked>Quick add to set</span>';
                html += '    <input id="btnfiltercomp_' + compType + '" type="button" value="Filters" onclick="filterComponent(\'' + compType + '\'); return false;">';
                html += '  </div > ';

                html += '</div>';

                html += '<div id="divRQMTBaseFilter" style="border-top:1px solid #dddddd;margin-bottom:5px;display:none;">';
                html += '<table>';

                html += '  <tr>';                
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;font-size:smaller;"><b>Suite:</b></td>';
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;font-size:smaller;"><b>System:</b></td>';
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;font-size:smaller;"><b>Work Area:</b></td>';
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;font-size:smaller;"><b>Purpose:</b></td>';
                html += '    <td style="width:99%;white-space:nowrap;font-size:smaller;">&nbsp;</td>';

                html += '  </tr>';
                html += '  <tr>';                
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;"><select id="compfilter_rqmt_suite" style="width:90px;font-size:smaller;" onchange="componentFiltersChanged(\'rqmt\');">' + _suiteOptions + '</select></td>';
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;"><select id="compfilter_rqmt_system" style="width:90px;font-size:smaller;" onchange="componentFiltersChanged(\'rqmt\');">' + _systemOptions + '</select></td>';
                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;"><select id="compfilter_rqmt_workarea" style="width:90px;font-size:smaller;" onchange="componentFiltersChanged(\'rqmt\');">' + _workAreaOptions + '</select></td>';                
                                html += '    <td style="width:90px;padding-right:5px;white-space:nowrap;"><select id="compfilter_rqmt_rqmttype" style="width:90px;font-size:smaller;" onchange="componentFiltersChanged(\'rqmt\');">' + _rqmtTypeOptions + '</select></td>';
                html += '    <td style="width:99%;text-align:left;cursor:pointer;"><img src="images/icons/cross.png" style="width:16px;height:16px;" onclick="$(this).closest(\'tr\').find(\'select\').val(\'0\'); componentFiltersChanged(\'rqmt\');"></td>';
                html += '  </tr>';

                html += '</table>';
                html += '</div>';

                // results
                html += '<div id="divcompsearchresults_' + compType + '" style="border-top:1px solid #dddddd;padding-top:5px;">';
                html += '</div>';              

                html += '</div>';

                $(tab).html(html);
            });
        }

        function componentFiltersChanged(compType) {
            var searchTxt = $('#txtcompsearch_' + compType).val();
            var existingResults = _displayedSearchResults['rqmt'];

            var filterCount = $('#divRQMTBaseFilter').find('select').length;
            var filtersSelected = $('#divRQMTBaseFilter').find('select:has(option[value=0]:selected)').length < filterCount;

            if (compType == 'rqmt') {
                if (searchTxt == null || searchTxt.trim().length == 0) {
                    executeComponentTabSearch('rqmt', searchTxt);
                }
                else {
                    if (existingResults != null) {
                        componentSearch_done('rqmt', null, existingResults);
                    }
                    else {
                        executeComponentTabSearch('rqmt', searchTxt);
                    }
                }
            }
        }

        function parseComponentSearchEntry(compType) {            
            var length = 0;

            var txt = $('#txtcompsearch_' + compType).val();
            var txtarr = null;            
            if (txt.trim().length > 0) {
                txt = txt.replace('\r\n', '\n');
                txtarr = txt.split('\n');
                txtarr = _.reject(txtarr, function (t) { return t == null || t.length == 0 || t == '' || t == '\n' });
                length = txtarr.length;
            }

            var label = $('#divcompsearchrightlabel_' + compType);

            if (length == 1) {
                label.html('<span style="font-size:smaller;">(1 RQMT Entry)</span>');
            }
            else if (length > 1) {
                label.html('<span style="font-size:smaller;color:red;">(' + length + ' RQMT Entries)</span>');
            }
            else {
                label.html('');
            }
        }

        function executeComponentTabSearch(compType, txt) {
            if (compType == null) {
                compType = $('[comptab]:visible').attr('comptype');
            }

            var compTypeObj = getComponentType(compType);

            if (txt == null) {
                txt = $('#txtcompsearch_' + compType).val();
            }

            if (txt != null) {
                txt = txt.trim();

                if (txt.substring(0, 1) == '>') {
                    txt = txt.substring(1).trim();
                }
            }

            if (txt != null && txt.length > 2000) {
                txt = txt.substring(0, 2000);
            }            

            //$('#btnaddtosystemcomp_' + compType).prop('disabled', txt == null || txt.length == 0 || !rqmtSetsDisplayed());
            $('#btnclearnewcomp_' + compType).prop('disabled', txt == null || txt.length == 0);

            var filterCount = $('#divRQMTBaseFilter').find('select').length;
            var filtersSelected = $('#divRQMTBaseFilter').find('select:has(option[value=0]:selected)').length < filterCount;

            if ((txt == null || txt.length == 0) && (!compTypeObj.allowBlankSearch && !filtersSelected)) {
                _displayedSearchResults[compType] = null;
                $('#divcompsearchresults_' + compType).html('');
                updateButtonStatuses();
                return;
            }
                        
            $('#lblsearchresults_' + compType).html('Search Results: (UPDATING...)');                   

            PageMethods.ExecuteComponentSearch(compType, txt, function (result) { componentSearch_done(compType, result); }, on_error);                        
        }

        function componentSearch_done(compType, results, parsedResults) {
            var compTypeObj = getComponentType(compType);
            var idProp = compTypeObj.idProp;
            var fkIdProp = compTypeObj.fkIdProp;
            var txt = $('#txtcompsearch_' + compType).val();
            var divCompSearchResults = $('#divcompsearchresults_' + compType);
            var searchResultHeaders = compTypeObj.searchResultHeaders.split(',');
            var searchResultColumns = compTypeObj.searchResultColumns.split(',');
            var cols = searchResultHeaders.length;
            var checkedNewRQMTs = '';// getCheckedNewRQMTs();

            var componentTextColor = '#000000';
            var componentBackgroundColor = '#eeeeee';
            var componentBorderColor = '#999999';

            var filterSuite = $('#compfilter_rqmt_suite').val();
            var filterSystem = $('#compfilter_rqmt_system').val();
            var filterWorkArea = $('#compfilter_rqmt_workarea').val();
            var filterRQMTType = $('#compfilter_rqmt_rqmttype').val();
            var hasFilters = filterSuite != '0' || filterSystem != '0' || filterWorkArea != '0' || filterRQMTType != '0';

            var html = '<div style="margin-bottom:5px;width:95%;">';
            html += '<b id="lblsearchresults_' + compType + '">Search Results:</b> <span style="font-size:smaller;">(* assigned to a displayed set)</span>';
            html += '<div style="float:right"><input type="checkbox" onclick="_showRQMTAssociations=$(this).is(\':checked\'); componentSearch_done(\'rqmt\', null, _displayedSearchResults[\'rqmt\']);" ' + (_showRQMTAssociations ? 'checked' : '') + '>Show associations</div>';
            html += '</div>';
            
            html += '<table style="width:100%;border-collapse:collapse;">';
            html += '  <thead>';
            html += '    <tr class="gridHeader gridFullBorder">';
            for (var x = 0; x < searchResultHeaders.length; x++) {
                var col = searchResultHeaders[x].trim();
                var dataCol = searchResultColumns[x].trim();
                var align = 'text-align:left;';
                var width = '';
                var wrap = '';

                if (col == '' || col.indexOf('#') >= 0 || col.indexOf(' ID') >= 0 || dataCol.indexOf('.') >= 0) {
                    align = 'text-align:center;';
                    width = 'width:1%;';
                    wrap = 'white-space:nowrap;';

                    if (col == '#') {
                        col = '';
                    }
                }

                html += '<th style="' + align + width + wrap + '">' + col + '</th>';
            }
            html += '    </tr>';
            html += '  </thead>';

            html += '<tbody>';

            var dt = parsedResults;

            if (dt == null) {
                dt = jQuery.parseJSON(results);

                // we don't want blanks, so remove them
                if (dt != null) {
                    dt = _.filter(dt, function (row) { return row[compTypeObj.fkIdProp] > 0; });
                }

                // because the return results contain similar word searches in addition to exact matches (and then sorted by alpha), but don't have a "match value", 
                // we try to sort the list based on best match using the following ordering:
                //    1) exact matches (Execution Plan)
                //    2) substring match for entire search phrase (First Execution Plan In January)
                //    3) substring match for individual words if there is more than one (First Test, The Plan)
                //    4) all the rest in alpha order (the original results are in alpha order)
                var searchArray = txt.trim().replace('\r\n', '\n').split('\n');
                for (var x = 0; x < searchArray.length; x++) {
                    var s = searchArray[x];
                    s = s.trim();
                    if (s.substring(0, 1) == '>') {
                        s = s.substring(1).trim();
                    }
                    searchArray[x] = s;
                }      

                var searchArrayUpper = [];
                for (var x = 0; x < searchArray.length; x++) {
                    var s = searchArray[x];
                    searchArrayUpper.push(s.toUpperCase());
                }

                var searchArrayUpperSplit = [];
                for (var x = 0; x < searchArray.length; x++) {
                    var s = searchArray[x];
                    searchArrayUpperSplit.push(s.toUpperCase().split(' '));
                }
                
                var fixedArr = [];
                fixedArr = fixedArr.concat(_.filter(dt, function (row) { return row[compTypeObj.txtProp] != null && _.find(searchArrayUpper, function (s) { return s == row[compTypeObj.txtProp].trim().toUpperCase() }) != null || s == row[compTypeObj.idProp] }));
                fixedArr = fixedArr.concat(_.filter(dt, function (row) { return row[compTypeObj.txtProp] != null && fixedArr.indexOf(row) == -1 && _.find(searchArrayUpper, function (s) { return row[compTypeObj.txtProp].trim().toUpperCase().indexOf(s) != -1 }) != null }));
                                
                if (_.find(searchArrayUpperSplit, function (arr) { return arr.length > 1 }) != null) {
                    for (var i = 0; i < searchArrayUpperSplit.length; i++) {
                        var wordArr = searchArrayUpperSplit[i]

                        for (var x = 0; x < wordArr.length; x++) {
                            var word = wordArr[x];

                            fixedArr = fixedArr.concat(_.filter(dt, function (row) { // this function is broken into separate lines and result assignments to make it easier for debugging
                                if (row[compTypeObj.txtProp] != null && fixedArr.indexOf(row) == -1) {                                    
                                    var testRQMT = row[compTypeObj.txtProp].trim().toUpperCase();                                    
                                    var match = testRQMT.indexOf(word) >= 0;                                    
                                    return match;
                                }
                                else {
                                    return false;
                                }
                            }));
                        }
                    }
                }

                fixedArr = fixedArr.concat(_.reject(dt, function (row) { return fixedArr.indexOf(row) >= 0 }));
                dt = fixedArr;
                _displayedSearchResults[compType] = dt;
            }

            var compsSeen = [];
            
            if (dt != null && dt.length > 0) {
                var altBackground = false;

                var idx = 0;

                for (var i = 0; i < dt.length; i++) {
                    var compFkID = dt[i][fkIdProp];

                    if (compsSeen.indexOf(compFkID) != -1) {
                        continue;
                    }

                    if (hasFilters) {
                        if (filterSuite != '0' && dt[i]['WTS_SYSTEM_SUITEID'] != filterSuite) {
                            continue;
                        }

                        if (filterSystem != '0' && dt[i]['WTS_SYSTEMID'] != filterSystem) {
                            continue;
                        }

                        if (filterWorkArea != '0' && dt[i]['WorkAreaID'] != filterWorkArea) {
                            continue;
                        }

                        if (filterRQMTType != '0' && dt[i]['RQMTTypeID'] != filterRQMTType) {
                            continue;
                        }
                    }

                    html += '<tr>';

                    compsSeen.push(compFkID);
                    idx++;
                    
                    var bgColor = altBackground ? '#eeeeee' : '#ffffff';

                    for (var x = 0; x < searchResultColumns.length; x++) {
                        var header = searchResultHeaders[x].trim();
                        var col = searchResultColumns[x].trim();
                        var draggable = true;

                        var val = null;
                        var align = 'text-align:left;';
                        var width = '';
                        var wrap = '';
                        var bg = 'background-color:' + bgColor + ';';
                        var leftCount = 0;

                        var leftIdx = col.indexOf('.');                        
                        if (leftIdx != -1) {                            
                            leftCount = parseInt(col.substring(leftIdx + 1));
                            col = col.substring(0, leftIdx);
                        }

                        if (col == '#') {
                            val = idx + '.';
                            align = 'text-align:center;';
                            width = 'width:1%;';
                            wrap = 'white-space:nowrap;';
                            draggable = false;
                        }
                        else {
                            val = dt[i][col];

                            if (header == '' || header.indexOf('#') >= 0 || header.indexOf(' ID') >= 0) {
                                align = 'text-align:center;';
                                width = 'width:1%;';
                                wrap = 'white-space:nowrap;';
                                draggable = false;
                            }

                            if (leftCount > 0) {
                                align = 'text-align:center;';
                            }
                        }

                        if (val == null) {
                            val = '';
                        }
                        else if (val.length > 100) {
                            val = val.substring(0, 100) + '...';
                        }
                        
                        if (leftCount > 0 && col != '#') {
                            if (val.length > leftCount) {
                                val = val.substring(0, leftCount);
                            }                            
                        }

                        html += '  <td valign="top" class="gridFullBorder" style="padding:3px;' + align + width + wrap + bg + '">';

                        if (draggable) {
                            // note: compid on this page refers to the comprqmtsystemid value; but the comp search results aren't tied to systems the way they are with the existing rqmt systems grids, so compid is 0
                            if (RQMTExists(compFkID)) {
                                val += '*';
                            }

                            html += '<div name="divsearchresultcomp" rqmtsystemid="-99999" comptype="' + compType + '" compid="0" compfkid="' + dt[i][fkIdProp] + '" compkey="0" '
                                + (compTypeObj.componentSearchOnClick != null ? 'onclick="if (!_draggingInProcess) ' + compTypeObj.componentSearchOnClick + '(' + dt[i][fkIdProp] + ');" ' : '')
                                + 'onmouseover="if (!_draggingInProcess) highlightComponentUsage($(this).attr(\'compfkid\'))" onmouseout="if (!_draggingInProcess) clearComponentUsageHighlight($(this).attr(\'compfkid\'))" '
                                + 'style="position:relative;padding:3px;padding-right:16px;background-color:' + componentBackgroundColor + ';border:1px solid ' + componentBorderColor + ';color:' + componentTextColor + ';border-radius:5px;margin-bottom:3px;cursor:pointer;" '                                
                                + '">';
                        }
                        
                        if (col == 'RQMT_ID') { // NOTE: THIS IS ANOTHER PLACE WHERE THE COMPONENTS ARE NOT ABSTRACTED THE WAY THEY WERE INTENDED - CUSTOM TAB VIEW BEHAVIOR SHOULD BE HANDLED IN THE COMPONNET OBJ PROPERTIES, NOT IN THIS FUNCTION
                            html += '<a href="javascript:openRQMTPopupFromBuilder(' + val + ',' + 0 + ')">' + val + '</a>';
                        }
                        else {
                            html += val;
                        }

                        if (draggable) {
                            var compClose = '</div>';

                            var assocHeader = '';
                            var assocBody = '';
                            var assocFooter = '';

                            // NOTE: THIS IS ANOTHER PLACE WHERE THE COMPONENTS ARE NOT ABSTRACTED THE WAY THEY WERE INTENDED - CUSTOM TAB VIEW BEHAVIOR SHOULD BE HANDLED IN THE COMPONNET OBJ PROPERTIES, NOT IN THIS FUNCTION                                                        
                            assocHeader += '<div id="divRQMTAssociations_' + i + '" style="display:' + (_showRQMTAssociations ? 'block' : 'none') + ';">';
                            assocHeader += '  <table>';
                            assocHeader += '    <tr>';
                            assocHeader += '      <td style="font-size:smaller;color:#999999;text-align:left;"><b>Work Area</b></td>';
                            assocHeader += '      <td style="font-size:smaller;color:#999999;text-align:left;"><b>System</b></td>';
                            assocHeader += '      <td style="font-size:smaller;color:#999999;text-align:left;"><b>Purpose</b></td>';
                            assocHeader += '    </tr>';

                            assocFooter += '</table></div>';

                            for (var a = i; a < dt.length; a++) { // starting with current row, and continuing with all the dups, add assoc data (we do the same going BACKWARDS, see below)
                                if (dt[a][fkIdProp] != compFkID) { // we found a different main comp, so stop
                                    break;
                                }

                                var rqmtid = dt[a]['RQMTID'];
                                var wa = dt[a]['WorkArea'];
                                var sys = dt[a]['WTS_SYSTEM'];
                                var p = dt[a]['RQMTType'];

                                if (wa != null || sys != null || p != null) {
                                    assocBody += '    <tr onmouseover="$(this).children().css(\'color\', \'' + _dragItemTextColor + '\');" onmouseout="$(this).children().css(\'color\', \'\');" onclick="rqmtAssociationClicked(' + dt[a]['WTS_SYSTEM_SUITEID'] + ', ' + dt[a]['WTS_SYSTEMID'] + ',' + dt[a]['WorkAreaID'] + ',' + dt[a]['RQMTTypeID'] + ');">';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (wa != null ? wa : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (sys != null ? sys : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (p != null ? p : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '    </tr>';
                                }
                            }
                            
                            for (var a = i - 1; i >= 0; a--) { // if we are filtering, then the first items from a set of matching comps could be filtered out before we display the first one; therefore, to pull associated values, we scan backwards as well as forwards
                                if (a == -1) break; // if i was a 0, then we can't go backwards

                                if (dt[a][fkIdProp] != compFkID) { // we found a different main comp, so stop
                                    break;
                                }

                                var rqmtid = dt[a]['RQMTID'];
                                var wa = dt[a]['WorkArea'];
                                var sys = dt[a]['WTS_SYSTEM'];
                                var p = dt[a]['RQMTType'];

                                if (wa != null || sys != null || p != null) {
                                    assocBody += '    <tr onmouseover="$(this).children().css(\'color\', \'' + _dragItemTextColor + '\');" onmouseout="$(this).children().css(\'color\', \'\');" onclick="rqmtAssociationClicked(' + dt[a]['WTS_SYSTEM_SUITEID'] + ', ' + dt[a]['WTS_SYSTEMID'] + ',' + dt[a]['WorkAreaID'] + ',' + dt[a]['RQMTTypeID'] + ');">';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (wa != null ? wa : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (sys != null ? sys : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '      <td style="cursor:pointer;font-size:smaller;text-align:left;white-space:nowrap;">' + (p != null ? p : 'NONE') + '&nbsp;&nbsp;</td>';
                                    assocBody += '    </tr>';
                                }
                            }

                            if (!_showRQMTAssociations) {
                                compClose = '<div style="position:absolute;right:0px;top:2px;width:16px;height:16px;' + (assocBody != '' ? '' : 'opacity:.25;') + '" onclick="' + (assocBody != '' ? 'rqmtToggleAssociations(' + i + ');' : '') + ' try { event.stopPropagation(); } catch { } try { window.event.cancelBubble = true; } catch { }"><img src="images/icons/layout.png" style="width:14px;height:14px;"></div>' + compClose;
                            }

                            if (assocBody != '') {
                                html += compClose + assocHeader + assocBody + assocFooter;
                            }
                            else {
                                html += compClose;
                            }
                        }

                        html += '  </td>';
                    }

                    html += '</tr>';

                    altBackground = !altBackground;
                }                
            }
            else {
                html += '<tr>';

                if (txt != null && txt.length > 0) {
                    html += '  <td colspan="' + cols + '">No results found.<br /><br /><i>Clicking "Add to System' + (checkedNewRQMTs != null && checkedNewRQMTs.indexOf(',') > 0 ? 's' : '') + '" will create a <u>new entry</u> after save is complete.</i></td>';
                }
                else {
                    html += '  <td colspan="' + cols + '">Enter a search string.</td>';
                }
                html += '</tr>';
            }

            html += '</tbody>';
            html += '</table>';

            $(divCompSearchResults).html(html);

            // add events to new components
            if (dt.length > 0) {
                $(divCompSearchResults).find('[name=divsearchresultcomp]').draggable({
                    helper: 'clone',
                    revert: 'invalid',
                    appendTo: 'body',
                    start: function (e, ui) {
                        $(ui.helper).css('font-weight', 'bold');
                        $(ui.helper).css('color', _dragItemTextColor);
                        $(ui.helper).css('background-color', _dragItemBackgroundColor);
                        $(ui.helper).css('border', '1px solid ' + _dragItemBorderColor);
                        _draggingInProcess = true;

                        for (var i = 0; i < _loadedRQMTSets.length; i++) {
                            var set = _loadedRQMTSets[i];

                            var srcFKID = $(ui.helper).attr('compfkid');
                            var existingRQMT = getRQMTFromRQMTSet(set.RQMTSetID, srcFKID);

                            if (existingRQMT != null) {
                                var trRQMTSetHeaderRow = $('[id=trRQMTSetHeaderRow][rqmtsetid=' + set.RQMTSetID + ']');
                                var trRQMTSet = $('[id=trRQMTSet][rqmtsetid=' + set.RQMTSetID + ']');

                                trRQMTSetHeaderRow.css('opacity', '0.3');
                                trRQMTSet.css('opacity', '0.3');                                
                            }
                            
                        }
                    },
                    stop: function (e, ui) {
                        $('[id=trRQMTSetHeaderRow]').css('opacity', '1.0');
                        $('[id=trRQMTSet]').css('opacity', '1.0');
                        $('[rqmtaddedoverlay=1]').hide();

                        setTimeout(function () { _draggingInProcess = false; }, 100); // _draggingInProcess allows us to prevent unwanted click events from firing when a drop occurs or ends; we use the timer to make sure the drag value is still on until after those other functions have evaluated
                    },
                    distance: 3,
                    delay: 100
                });

                $(divCompSearchResults).find('[name=divsearchresultcomp]').draggable( "option", "distance", $('#divRQMTsGrid').find('tr[id=trRQMTSetHeaderRow]').length > 0 ? 3 : 10000);              
            }

            updateButtonStatuses();
        }

        function highlightComponentUsage(rqmtid) {
            for (var i = 0; i < _loadedRQMTSets.length; i++) {
                var set = _loadedRQMTSets[i];

                var existingRQMT = getRQMTFromRQMTSet(set.RQMTSetID, rqmtid);

                if (existingRQMT != null) {
                    var trRQMTSetHeaderRow = $('[id=trRQMTSetHeaderRow][rqmtsetid=' + set.RQMTSetID + ']');
                    var overlay = $('[rqmtaddedoverlay=1][rqmtsetid=' + set.RQMTSetID + ']');

                    trRQMTSetHeaderRow.css('opacity', '0.3');
                    overlay.show();
                }

            }
        }

        function clearComponentUsageHighlight(div) {
            $('[id=trRQMTSetHeaderRow]').css('opacity', '1.0');
            $('[rqmtaddedoverlay=1]').hide();
        }

        function createNewComponentButtonClicked(compType) {
            var compTypeObj = getComponentType(compType);

            if (_displayedSearchResults != null && _displayedSearchResults[compType] != null) {
                var displayedResultsForCompType = _displayedSearchResults[compType];

                var txt = $('#txtcompsearch_' + compType).val();

                if (txt == null || txt.length == 0) {
                    var errorMessage = 'Invalid entry. ';

                    if (txt == null || txt.trim().length == 0) {
                        errorMessage += 'Text value is required. ';
                    }

                    dangerMessage(errorMessage.trim());

                    return;
                }

                if ($('#cbquickaddcomp_' + compType).is(':checked')) {
                    var setsWithQuickAddEnabled = _.filter(_loadedRQMTSets, function (set) { return set.QuickAddEnabled });

                    if (setsWithQuickAddEnabled.length > 1 && _quickAddWarningEnabled) {
                        QuestionBox('Confirm Quick Add', StrongEscape('You are attempting to quick add a RQMT to <u>multiple</u> sets at once. Continue?<br /><br /><input id=\"cbhidedquickaddwarning\" type=\"checkbox\" value=\"1\">Do not show this warning in the future'), 'Yes,No', 'createNewComponentButtonConfirmed', 300, 300, this, compType);
                        return;
                    }
                }

                txt = txt.replace('\r', '');

                if (displayedResultsForCompType != null && displayedResultsForCompType.length > 0) {
                    txt = txt.trim().toUpperCase();
                    var comp = _.find(displayedResultsForCompType, function (row) { return row[compTypeObj.txtProp] != null && row[compTypeObj.txtProp].trim().toUpperCase() == txt; });
                                        
                    if (comp != null) {
                        QuestionBox('Confirm Duplicate', 'One or more similar ' + compTypeObj.name + ' values already exist. Are you sure you want to create a new entry?', 'Yes,No', 'createNewComponentButtonConfirmed', 300, 300, this, compType);
                    }
                    else {
                        if (compTypeObj.alwaysConfirmNewEntries) {
                            QuestionBox('Confirm New Entry', 'Are you sure you want to create a new ' + compTypeObj.name + ' entry?', 'Yes,No', 'createNewComponentButtonConfirmed', 300, 300, this, compType);
                        }
                        else {
                            createNewComponentButtonConfirmed('Yes', compType);
                        }
                    }
                }
                else {
                    if (compTypeObj.alwaysConfirmNewEntries) {
                        QuestionBox('Confirm New Entry', 'Are you sure you want to create a new ' + compTypeObj.name + ' entry?', 'Yes,No', 'createNewComponentButtonConfirmed', 300, 300, this, compType);
                    }
                    else {
                        createNewComponentButtonConfirmed('Yes', compType);
                    }
                }
            }            
        }

        function createNewComponentButtonConfirmed(answer, compType) {
            if (compType != null && compType.indexOf('|') != -1) {
                if (compType.indexOf('cbhidedquickaddwarning=1') != -1) {
                    PageMethods.HideQuickAddWarning(function (result) { }, on_error);
                    _quickAddWarningEnabled = false;
                }

                compType = compType.split('|')[0];
            }

            if (answer == 'Yes') {
                var txt = $('#txtcompsearch_' + compType).val();

                if (txt != null && txt.length > 0) {
                    PageMethods.CreateNewComponent(compType, txt, function (result) { createNewComponent_done(result, compType) }, on_error);
                }
            }
            
            $('#txtcompsearch_' + compType).focus();
        }

        function createNewComponent_done(result, compType) {
            var dt = $.parseJSON(result);

            if (dt.success) {
                // we've saved the result (or didn't save due to duplicate). now re-run the search with the new results (including the new ID)
                executeComponentTabSearch(compType, $('#txtcompsearch_' + compType).val());

                if ($('#cbquickaddcomp_' + compType).is(':checked')) {
                    var setsWithQuickAddEnabled = _.filter(_loadedRQMTSets, function (set) { return set.QuickAddEnabled });
                    // the _loadedRQMTSets collection is getting updated as the rqmts are being added, so the collection could change on the fly so we store props offline
                    var setsLength = setsWithQuickAddEnabled != null ? setsWithQuickAddEnabled.length : 0;
                    var sets = [];
                    for (var i = 0; i < setsLength; i++) {
                        sets.push(setsWithQuickAddEnabled[i].RQMTSetID);
                    }
                    
                    let rqmtarr = [];
                    let rqmtid = dt.rqmtid;
                    let rqmtids = dt.rqmtids;
                    if (rqmtids != null && rqmtids.length > 0) {
                        rqmtarr = rqmtids.split(',');
                    }
                    else if (rqmtid > 0) {
                        rqmtarr.push(rqmtid);
                    }
                    let rqmtsneedingparents = dt.rqmtsneedingparents.split(',');


                    // NOTE: THE CALL BELOW SETS OFF A CHAIN OF ADDS, ONE RQMT AND ONE SET AT A TIME. AS EACH ADD FINISHES, THE NEXT STARTS; WE DO THIS INSTEAD OF USING A LOOP
                    // THAT CALLS ALL OF THE ADDS SIMULTANEOUSLY; THIS CAUSED ITEMS TO BE ADDED OUT OF ORDER SINCE SOME CALLS FINISHED FASTER THAN OTHERS EVEN THOUGH THEY WERE
                    // SUPPOSED TO OCCUR SECOND; THE METHOD WE USE MIGHT BE A BIT SLOWER, BUT IT IS GUARANTEEING THE ADD ORDER
                    if (sets.length > 0 && rqmtarr.length > 0) {
                        showLoadingMessage('Adding RQMT...');
                        compAddRQMTToRQMTSet(sets.join(','), rqmtarr.join(','), rqmtsneedingparents, 0, 0);
                    }
                    else {
                        $('#txtcompsearch_rqmt').val('');
                        $('#txtcompsearch_rqmt').focus();
                        $('#divcompsearchrightlabel_rqmt').html('');
                        updateButtonStatuses();
                        refreshRQMTs(); 
                    }
                }
            }
        }

        function compAddRQMTToRQMTSet(setsList, rqmtsList, rqmtsNeedingParents, currentSetIdx, currentRQMTIdx) {
            var sets = setsList.split(',');
            var rqmts = rqmtsList.split(',');

            var setid = sets[currentSetIdx];
            var rqmtid = rqmts[currentRQMTIdx];

            //console.log('set=' + currentSet + ' rqmt=' + currentRQMT + ' setid=' + setid + ' rqmtid=' + rqmtid);

            var addAsChild = rqmtsNeedingParents.length > 0 && rqmtsNeedingParents[currentRQMTIdx] == '1';

            PageMethods.AddRQMTToRQMTSet(setid, rqmtid, null, addAsChild,
                function (result) {
                    if (currentRQMTIdx < (rqmts.length - 1)) {
                        setTimeout(function () { compAddRQMTToRQMTSet(setsList, rqmtsList, rqmtsNeedingParents, currentSetIdx, currentRQMTIdx + 1) }, 0);
                    }
                    else {
                        if (currentSetIdx < (sets.length - 1)) {
                            setTimeout(function () { compAddRQMTToRQMTSet(setsList, rqmtsList, rqmtsNeedingParents, currentSetIdx + 1, 0) }, 0);
                        }
                        else {
                            // we're done
                            $('#txtcompsearch_rqmt').val('');
                            $('#txtcompsearch_rqmt').focus();
                            $('#divcompsearchrightlabel_rqmt').html('');
                            updateButtonStatuses();
                            refreshRQMTs(); 
                        }
                    }
                }
                , on_error);
        }

        function clearNewComponentSearch(compType) {
            $('#divcompsearchrightlabel_' + compType).html('');                        
            $('#txtcompsearch_' + compType).val('');
            executeComponentTabSearch(compType, '');
            $('#txtcompsearch_' + compType).focus();            
            updateButtonStatuses();
        }

        function cancelNewComponentSearch(compType) {
            clearNewComponentSearch(compType);
            clearActiveRQMTSet();
            updateButtonStatuses();
        }


        function getComponentType(type) {
            return _.find(_compTypes, function (ct) { return ct.type == type });
        }

        function getComponentFromSearchResults(compType, compFKID) {
            var comp = null;

            var compTypeObj = getComponentType(compType);

            if (_displayedSearchResults != null && _displayedSearchResults[compType] != null) {
                var dt = _displayedSearchResults[compType];

                comp = _.find(dt, function (row) { return row[compTypeObj.fkIdProp] == compFKID; });
            }

            return comp;
        }

        function filterComponent(compType) {
            var compTypeObj = getComponentType(compType);

            if (compTypeObj.filterOnClick != null) {
                window[compTypeObj.filterOnClick]();
            }
        }

        ///////////////////////////////////////////////////////
        // ATTRIBUTES
        /////////////////////////////////////////////////////// 


    </script>

    <script id="jsInit" type="text/javascript">

        function initVariables() {
            _defaultSystemSuiteID = <%=DefaultSystemSuiteID%>;
            _defaultSystemID = <%=DefaultSystemID%>;
            _defaultWorkAreaID = <%=DefaultWorkAreaID%>;
            _defaultRQMTTypeID = <%=DefaultRQMTTypeID%>;
            _defaultRQMTSetID = <%=DefaultRQMTSetID%>;
            _defaultRQMTSetName = '<%=DefaultRQMTSetName.Replace("'", "\'")%>';
            _defaultRQMTID = <%=DefaultRQMTID%>;
            _dtSystems = $.parseJSON(UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(dtSystemsJSON)%>'));
            _dtWorkAreaSystems = $.parseJSON(UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(dtWorkAreaSystemsJSON)%>'));

            // we escape the html to not break the html or javascript, but when reading into memory, we unescape it
            _suiteOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(SuiteOptions)%>');
            _systemOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(SystemOptions)%>');
            _workAreaOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(WorkAreaOptions)%>');
            _rqmtTypeOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(RQMTTypeOptions)%>');
            _complexityOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(ComplexityOptions)%>');
            _functionalitySelectOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(FunctionalitySelectOptions)%>');
            _functionalityCheckBoxOptions = UndoStrongEscape('<%=WTS.Util.StringUtil.StrongEscape(FunctionalityCheckBoxOptions)%>');

            _hasDefaultValues = _defaultSystemSuiteID != 0 || _defaultSystemID != 0 || _defaultWorkAreaID != 0 || _defaultRQMTTypeID != 0;
            _initComplete = false;
            _changesMade = false;
            _type = '<%=this.Type%>';
            _pageUrls = new PageURLs();
            _customListCache = [];
            _loadedRQMTs = [];
            _loadedRQMTSets = [];
            _loadedRQMTSetViewModes = [];
            _debouncedFilterRQMTSets = _.debounce(filterRQMTSets, 350);
            _debouncedExistingDescriptionsSearch = _.debounce(searchDescriptions, 350);
            _debouncedExecuteComponentTabSearch = _.debounce(executeComponentTabSearch, 200);
            _displayedSearchResults = [];
            _rqmtSetNames = [];
            _systemNames = [];

            _activeRQMTSetID = 0;
            _activeRQMTSetBackground = '#85b2c8';

            _rowAltColor = '#eeeeee';

            _dragItemTextColor = '#3c763d';
            _dragItemBackgroundColor = '#dff0d8';
            _dragItemBorderColor = '#3c763d';

            _dragHoverColor = '#dff0d8';
            _checkHighlightColor = '#85b2c8';
            _newComponentTextColor = '#31708f';
            _newComponentBackgroundColor = '#d9edf7';
            _newComponentBorderColor = '#559fe0';
            _rowErrorColor = '#f2dede';

            _dragItemTextErrorColor = '#999999';
            _dragItemBackgroundErrorColor = '#dddddd';
            _dragItemBorderErrorColor = '#999999';

            _funcSelectedBackgroundColor = '#8ad3f7';

            _showRQMTAssociations = false;
            _draggingInProcess = false;
            _draggingFunctionalityInProgress = false;
            _quickAddWarningEnabled = <%=QuickAddWarningEnabled.ToString().ToLower()%>;

            initDefaultComponents();

            _sortArrow = $('#imgSortArrow');
            _sortArrowIndented = $('#imgSortArrowIndented');

            _wideMode = false;

            _rqmtSetColspan = 9;
            _rqmtColspan = 10;
            _rqmtColspanWideMode = 10;

            _copiedRQMTSystems = '';
        }        

        function initControls() {
            $('#divTabsContainer').tabs();            
        }

        function initDisplay() {
            $('#imgSort').hide();           
       
            setHeight();

            switch ('<%=this.Type %>') {
                case 'Add':
                case 'Edit':
                    //$('#btnSave').show();
                    $('#btnSaveAndClose').show();
                    $('#divAddEdit').show();
                    break;
            }

            initComponentTabs();
        }

        function setHeight() {
            var wm = new WindowManager();
            var ht = wm.getHeight();
            // account for header bar, button bar, and top selectors, bottom bar (30, 30, 70, 30)            
            ht -= 160;

            if ($('#divCopiedRQMTs').is(':visible')) {
                ht -= $('#divCopiedRQMTs').height();
                ht -= 20; // account for extra padding and spacer rows
            }

            $('#divRQMTsGrid').css('height', ht + 'px');   
        }

        function initEvents() {           

            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(false); return false; });
            $('#imgExport').click(function () { imgExport_click(); });

            $('[id$=txtRQMTSetName]').on('keyup', function () { _debouncedFilterRQMTSets(); });
            $('[id$=ddlSuite]').on('change', function () { ddlSuite_changed(); });
            $('[id$=ddlSystem]').on('change', function () { ddlSystem_changed(); });
            $('[id$=ddlWorkArea]').on('change', function () { filterRQMTSets(); });
            $('[id$=ddlRQMTType]').on('change', function () { filterRQMTSets(); });

            $('#btnAddNewRQMTSet').on('click', function () { btnAddNewRQMTSet_click(); });
            $('#btnClearDDLs').on('click', function () { btnClearDDLs_click(); });
            $('#btnRefreshRQMTs').on('click', function () { refreshRQMTs(); });
            $('#tdtoggle').on('click', function () { toggleRightPane(); });
            $(window).on('resize', function () { setHeight(); });
            
        }

        function initDefaults() {   
            _hasDefaultValues = _defaultSystemSuiteID != 0 || _defaultSystemID != 0 || _defaultWorkAreaID != 0 || _defaultRQMTTypeID != 0;

            if (_defaultSystemSuiteID > 0) { // this will be > 0 if suite is set OR if system is set (from here, the workflow will attempt to set all 5(suite,system,wa,wg,rt) values in order)
                $('[id$=ddlSuite]').val(_defaultSystemSuiteID);
                ddlSuite_changed(); // the suite_changed function leads to system changed fn, wa changed fn, and finally filterRQMTSets at the end
            }
            else if (_defaultWorkAreaID > 0 || _defaultRQMTTypeID > 0) { // we did not set suite or system
                if (_defaultWorkAreaID > 0) {
                    $('[id$=ddlWorkArea]').val(_defaultWorkAreaID);
                }

                if (_defaultRQMTTypeID > 0) {
                    $('[id$=ddlRQMTType]').val(_defaultRQMTTypeID);
                }

                filterRQMTSets(_defaultRQMTID > 0 ? -1 : 0); // since wa / rt by themselves can show a ton of rows, we close them by default, unless a specific RQMTID was chosen
            }
            else if (_defaultRQMTID > 0) {
                filterRQMTSets(-1); // force open all sets with the rqmt in it
            }
            else {
                _initComplete = true; // we had no init to do
            }

            if ('<%=CopiedRQMTs%>' != '') {
                $('#divCopiedRQMTs').html('<b>Copied RQMTs:</b> ' + '<%=CopiedRQMTs%>' + '&nbsp;&nbsp;&nbsp;<span style="font-size:smaller;cursor:pointer;" onclick="clearCopiedRQMTsClicked()">(<u>CLEAR</u>)' + ('<%=CopiedRQMTs%>'.indexOf('*') != -1 ? '<br />* duplicate RQMT # from different RQMT Set' : '') + '</span>');
                $('#divCopiedRQMTs').css('display', 'inline-block');
                $('#divCopiedRQMTsDivider').show();
            }

            if ('<%=CopiedRQMTSystems%>' != '') {
                _copiedRQMTSystems = '<%=CopiedRQMTSystems%>';
            }
            
            refreshRQMTSetNames();
            updateButtonStatuses();
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();            
            initEvents();
            initDefaults();

            refreshRQMTTypesDDL();
        });
    </script>
</asp:Content>