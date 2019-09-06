﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Wizard.aspx.cs" Inherits="AOR_Wizard" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Wizard</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <script type="text/javascript" src="Scripts/filter.js"></script>
    <style type="text/css">
        .nav {
            padding: 7px 3px;
            cursor: pointer;
            border-bottom: 1px solid gray;
        }
        .navhover {
            background-color: #d7e8fc;
        }
        .navselected {
            background-color: #d7e8fc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table style="border-collapse: collapse; width: 100%;">
            <tr>
                <td id="tdNav" style="height: 370px; width: 120px; vertical-align: top; border-right: 1px solid gray; display: none;">
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td>
                                <div class="pageContentHeader" style="padding: 7px 3px; width: 97%;">
                                    AOR Navigation
			                    </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep1" class="nav">
                                    Select AOR
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep2" class="nav">
                                    Select AOR Attributes
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep3" class="nav">
                                    Select AOR Release Attributes
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep4" class="nav">
                                    Select AOR Release Systems
                                </div>
                            </td>
                        </tr>
                        <tr style="display: none;">
                            <td>
                                <div id="navStep5" class="nav">
                                    Select AOR Release Resources
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep6" class="nav">
                                    Select AOR Release CRs
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="navStep7" class="nav">
                                    Select AOR Release Work Tasks
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="vertical-align: top;">
                    <div id="divStep1" class="step">
                        <div class="pageContentHeader" style="padding: 5px;">
                            Select AOR
			            </div>
                        <div style="padding: 10px;">
                            <table style="border-collapse: collapse; width: 100%;">
                                <tr>
                                    <td style="width: 70px;">
                                        AOR Name:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAORName" runat="server" MaxLength="150" Width="98%"></asp:TextBox>
                                    </td>
                                    <td style="width: 95px;">
                                        <input type="button" id="btnSearch" value="Search" />
                                        <input type="button" id="btnClear" value="Clear" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divAOR" style="height: 298px; overflow-x: hidden; overflow-y: auto; text-align: center;">Please click the Search button to select an AOR.</div>
                    </div>
                    <div id="divStep2" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;">
                            Select AOR Attributes&nbsp;<span class="selectedAOR"></span>
			            </div>
                        <div style="padding: 10px;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 5px;">
						    
					                </td>
                                    <td style="width: 65px;">
						                Description:
					                </td>
                                    <td>
                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" MaxLength="500" Width="98%"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div id="divStep3" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;">
                            Select AOR Release Attributes&nbsp;<span class="selectedAOR"></span>
			            </div>
                        <div style="padding: 10px;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 5px;">
						    
					                </td>
                                    <td style="width: 120px;">
						                Release:
					                </td>
                                    <td>
                                        <asp:DropDownList ID="ddlProductVersion" runat="server" Width="185px" BackColor="#F5F6CE"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
						    
					                </td>
                                    <td>
						                Primary System:
					                </td>
                                    <td>
                                        <asp:DropDownList ID="ddlPrimarySystem" runat="server" Width="185px" BackColor="#F5F6CE"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
						    
					                </td>
                                    <td>
						                Workload Allocation:
					                </td>
                                    <td>
                                        <asp:DropDownList ID="ddlWorkloadAllocation" runat="server" Width="185px" BackColor="#F5F6CE"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
						    
					                </td>
                                    <td>
						                AOR Workload Type:
					                </td>
                                    <td>
                                        <asp:DropDownList ID="ddlWorkType" runat="server" Width="185px" BackColor="#F5F6CE" Enabled="false"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div id="divStep4" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;width: 90%;">
                            <div style="width:49%;display:inline-block;vertical-align:top;">Select AOR Release Systems&nbsp;<span class="selectedAOR"></span></div>
                            <div style="width:50%;display:inline-block;text-align:right;vertical-align:top;">
                                System Name:&nbsp;<input id="txtSystemNameSearch" type="text" style="width:250px;" />&nbsp;<input type="button" value="Clear" id="btnClearSystemNameSearch" />
                            </div>
			            </div>
                        <div id="divAORSystems" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;"></div>
                    </div>
                    <div id="divStep5" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;">
                            <div style="width:49%;display:inline-block;vertical-align:top;">Select AOR Release Resources&nbsp;<span class="selectedAOR"></span></div>
                            <div style="width:50%;display:inline-block;text-align:right;vertical-align:top;">
                                Resource Name:&nbsp;<input id="txtResourceNameSearch" type="text" style="width:250px;" />&nbsp;<input type="button" value="Clear" id="btnClearResourceNameSearch" />
                            </div>
			            </div>
                        <div id="divAORResources" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;"></div>
                    </div>
                    <div id="divStep6" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;">
                            Select AOR Release CRs&nbsp;<span class="selectedAOR"></span>
			            </div>
                        <div id="divAORCRs" style="padding: 2px;">
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <span id="spnSelectedCountCR">0 CRs Checked</span>
                                    </td>
                                    <td style="text-align: right;">
                                        <span id="spnCRStatus">CR Coordination:&nbsp;<select id="ddlCRStatusQF" runat="server" multiple="true" style="width: 150px;"></select></span>
                                        <span id="spnCRContract" style="padding-right: 5px;">CR Contract:&nbsp;<select id="ddlCRContractQF" runat="server" multiple="true" style="width: 150px;"></select></span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <iframe id="frmCRs" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 310px;"></iframe>
                    </div>
                    <div id="divStep7" class="step" style="display: none;">
                        <div class="pageContentHeader" style="padding: 5px;">
                            Select AOR Release Tasks&nbsp;<span class="selectedAOR"></span>
			            </div>
                        <div id="divAORTasks" style="padding: 2px;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 25px;">
                                        <img id="imgShowFilters" src="Images/Icons/funnel.png" title="Assign Filters" alt="Assign Filters" width="15" height="15" style="cursor: pointer; margin: 4px;" />
                                    </td>
                                    <td style="width: 160px;">
                                        <span id="spnFilterCount">0 Filters Applied</span>
                                    </td>
                                    <td>
                                        Work Task #:&nbsp;<input type="text" id="txtTaskSearch" placeholder="Search for multiple work tasks by comma separating work task #s." style="width: 350px;" />&nbsp;<input type="button" id="btnTaskSearch" value="Search" />
                                    </td>
                                    <td style="text-align: right; padding-right: 5px;">
                                        <span id="spnSelectedCount">0 Work Tasks Checked</span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <iframe id="frmTasks" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 310px;"></iframe>
                    </div>
                </td>
            </tr>
        </table>
        <div id="divFooter" class="PopupFooter">
            <div id="wizardStatusMessage" style="position:absolute;left:5px;top:2px;line-height:25px;display:none;"></div>
            <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td style="text-align: right;">
                        <input type="button" id="btnClose" value="Close" />
                        <input type="button" id="btnSave" value="Save And Close" style="display: none;" />
                        <input type="button" id="btnSaveContinue" value="Save And Select Another AOR" style="display: none;" />
                        <input type="button" id="btnBack" value="Back" style="display: none;" />
                        <input type="button" id="btnNext" value="Next" disabled="disabled" style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divAppliedFilters" class="filterContainer" style="display: none;"></div>

        <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	    <link rel="stylesheet" href="Styles/multiple-select.css" />

        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    </form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAORID, _selectedAORReleaseID;
        var _selectedAORName = '';
        var _blnAORChanged = false;
        var filterBox = new filterContainer('divAppliedFilters');
        var _selectedCRStatusesQF = '';
        var _selectedCRContractsQF = '';
        var arrCRs = [];
        var arrTasks = [];
        var _statusMsgTimer;
    </script>

    <script id="jsEvents" type="text/javascript">
        function nav_click(obj) {
            if ($(obj).attr('id') != 'navStep1') {
                $('.navselected').removeClass('navselected');
                if ($(obj).find('img[title="Visited"]').length == 0) $(obj).prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $(obj).addClass('navselected');
            }

            switch ($(obj).attr('id')) {
                case 'navStep1':
                    QuestionBox('Confirmation', 'Are you sure you would like to re-select an AOR? Unsaved changes will be lost if you select a different AOR.', 'Yes,No', 'confirmNavBack', 300, 300, this);
                    break;
                case 'navStep2':
                    $('.step:visible').fadeOut(function () { $('#divStep2').fadeIn(); });
                    break;
                case 'navStep3':
                    $('.step:visible').fadeOut(function () { $('#divStep3').fadeIn(); });
                    break;
                case 'navStep4':
                    $('.step:visible').fadeOut(function () { $('#divStep4').fadeIn(); });
                    break;
                case 'navStep5':
                    $('.step:visible').fadeOut(function () { $('#divStep5').fadeIn(); });
                    break;
                case 'navStep6':
                    $('.step:visible').fadeOut(function () { $('#divStep6').fadeIn(function () { $('#frmCRs')[0].contentWindow.resizePage(); }); });
                    break;
                case 'navStep7':
                    $('.step:visible').fadeOut(function () { $('#divStep7').fadeIn(function () { $('#frmTasks')[0].contentWindow.resizePage(); }); });
                    break;
            }
            checkStepCompletion();
        }

        function confirmNavBack(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                $('#tdNav').hide();
                $('#btnSave').hide();
                $('#btnSaveContinue').hide();
                $('#btnBack').hide();
                $('.step:visible').fadeOut(function () { $('#divStep1').fadeIn(); })
            }
        }

        function btnSearch_click() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') $('#btnNext').prop('disabled', false);

            $('#divAOR').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');

            PageMethods.Search($('#<%=this.txtAORName.ClientID %>').val(), search_done, search_on_error);
        }

        function btnClear_click() {
            $('#<%=this.txtAORName.ClientID %>').val('');
            $('#divAOR').html('Please click the Search button to select an AOR.');
        }

        function search_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            if (dt != null && dt.length > 0) {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 25px;"></th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">AOR #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; border-right: none;">AOR Name</th>';
                nHTML += '</tr>';

                var uniqueName = true;
                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="text-align: center;">';

                    if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') nHTML += '<input type="checkbox" aorid="' + row.AORID + '" aorreleaseid="' + row.AORRelease_ID + '" field="AOR" onchange="chkAOR_change(this);" />';
                    
                    nHTML += '</td><td style="text-align: center;">';

                    if ('<%=this.CanViewAOR %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<a href="" onclick="openAOR(' + row.AORID + ', 0); return false;" style="color: blue;">' + row.AORID + '</a>';
                    }
                    else {
                        nHTML += row.AORID;
                    }

                    nHTML += '</td><td style="border-right: none;">' + row.AORName + '</td>';
                    nHTML += '</tr>';

                    if ($('#<%=this.txtAORName.ClientID %>').val().toUpperCase() === row.AORName.toUpperCase()) uniqueName = false;
                });

                nHTML += '</table>';

                if (uniqueName) nHTML += '<br /><br /><input type="button" id="btnAddNewAOR" value="Add New AOR" onclick="addNewAOR(); return false;" />';
            }
            else {
                $('#btnNext').prop('disabled', true);
                nHTML += 'No results found.<br /><br /><input type="button" id="btnAddNewAOR" value="Add New AOR" onclick="addNewAOR(); return false;" />';
            }

            $('#divAOR').html(nHTML);
        }

        function search_on_error() {
            $('#divAOR').html('Error gathering data.');
        }

        function addNewAOR() {
            _selectedAORID = 0;
            _selectedAORName = 'New AOR';
            _selectedAORReleaseID = 0;

            $('#btnNext').prop('disabled', false).trigger('click');
            $('input[field=AOR').prop('checked', false).prop('disabled', false);
        }

        function chkAOR_change(obj) {
            if ($(obj).is(':checked')) {
                _selectedAORID = parseInt($(obj).attr('aorid'));
                _selectedAORName = $(obj).closest('tr').find('td:eq(2)').text();
                _selectedAORReleaseID = parseInt($(obj).attr('aorreleaseid'));
                $('#divAOR').find('input[type=checkbox][field=AOR]').not($(obj)).prop('checked', false).prop('disabled', true);
            }
            else {
                _selectedAORID = undefined;
                _selectedAORName = '';
                _selectedAORReleaseID = undefined;
                $('#divAOR').find('input[type=checkbox][field=AOR]').not($(obj)).prop('disabled', false);
            }
            _blnAORChanged = true;
        }

        function openAOR(AORID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&Source=MI';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnClose_click() {
            if ($('#btnNext').is(':enabled')) {
                QuestionBox('Confirm Close', 'Are you sure you would like to close?', 'Yes,No', 'confirmClose', 300, 300, this);
            }
            else {
                closeWindow();
            }
        }

        function confirmClose(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') closeWindow();
        }

        function btnSave_click(saveAndContinue) {
            var validation = validate();

            if (validation.length == 0) {
                QuestionBox('Confirm Save', 'Are you sure you would like to save?', 'Yes,No', 'confirmSave', 300, 300, this, saveAndContinue);
            }
            else {
                MessageBox('Invalid entries: <br><br>' + validation);
            }
        }

        function validate() {
            var validation = [];
            var $systemRows = $('#<%=this.divAORSystems.ClientID %> tr').not(':first');

            if ($systemRows.find('input[type=checkbox][field="System"]:checked').length > 0 && $('#<%=this.ddlPrimarySystem.ClientID%>').val() == 0) {
                validation.push('Please select a primary system.');
            }

            $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function () {
                var $obj = $(this).find('input[type=checkbox][field="System"]');

                if ($obj.is(':checked')) {
                    if ($obj.attr('systemid') == $('#<%=this.ddlPrimarySystem.ClientID%>').val()) {
                        validation.push('Unable to select duplicate systems.');
                    }
                }
            });

            return validation.join('<br>');
        }

        function confirmSave(answer, saveAndContinue) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
				    ShowDimmer(true, 'Saving...', 1);
					
					var arrSystems = [], arrResources = [], arrCRsFinal = [], arrTasksFinal = [];

                    $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function() {
                        var $obj = $(this).find('input[type=checkbox][field="System"]');

                        if ($obj.is(':checked')) arrSystems.push({ 'systemid': $obj.attr('systemid'), 'primary': 0 });
                    });

                    if ($('#<%=this.ddlPrimarySystem.ClientID%>').val() > 0) {
                        arrSystems.push({ 'systemid': $('#<%=this.ddlPrimarySystem.ClientID%>').val(), 'primary': 1 });
                    }

					$('#<%=this.divAORResources.ClientID %> tr').not(':first').each(function() {
					    var $obj = $(this).find('input[type=checkbox][field="Resource"]');

					    if ($obj.is(':checked')) arrResources.push({ 'resourceid': $obj.attr('resourceid') });
					});

                    $.each(arrCRs, function (index, value) {
                        arrCRsFinal.push({ 'crid': value });
                    });

                    $.each(arrTasks, function (index, value) {
                        arrTasksFinal.push({ 'taskid': value });
                    });

				    var nSystemsJSON = '{save:' + JSON.stringify(arrSystems) + '}';
				    var nResourcesJSON = '{save:' + JSON.stringify(arrResources) + '}';
				    var nCRsJSON = '{save:' + JSON.stringify(arrCRsFinal) + '}';
				    var nTasksJSON = '{save:' + JSON.stringify(arrTasksFinal) + '}';
					
					PageMethods.Save(_selectedAORID, (_selectedAORID == 0 ? $('#<%=this.txtAORName.ClientID %>').val() : ''), $('#<%=this.txtDescription.ClientID %>').val(),
                        $('#<%=this.ddlProductVersion.ClientID %>').val(), $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(), $('#<%=this.ddlWorkType.ClientID %>').val(),
						nSystemsJSON, nResourcesJSON, nCRsJSON, nTasksJSON, function (result) { save_done(result, saveAndContinue); }, save_on_error);
			    }
			    catch (e) {
				    ShowDimmer(false);
				    MessageBox('An error has occurred.');
			    }
            }
        }

        function save_done(result, saveAndContinue) {
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
			    if (opener.refreshPage) opener.refreshPage(false);

				MessageBox('AOR has been saved.');

				if ($.trim(saveAndContinue).toUpperCase() == 'TRUE') {
				    setTimeout(refreshPage(), 1);
                }
                else {
				    setTimeout(closeWindow, 1);
                }
			}
			else if (blnExists) {
				MessageBox('AOR Name already exists.');
			}
			else {
				MessageBox('Failed to save. <br>' + errorMsg);
			}
        }

        function save_on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function btnBack_click() {
            switch ($('.step:visible').attr('id')) {
                case 'divStep2':
                    QuestionBox('Confirmation', 'Are you sure you would like to re-select an AOR? Unsaved changes will be lost if you select a different AOR.', 'Yes,No', 'confirmBack', 300, 300, this);
                    break;
                case 'divStep3':
                    step3_done(true);
                    break;
                case 'divStep4':
                    step4_done(true);
                    break;
                case 'divStep5':
                    step5_done(true);
                    break;
                case 'divStep6':
                    step6_done(true);
                    break;
                case 'divStep7':
                    step7_done(true);
                    break;
            }
        }

        function confirmBack(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') step2_done(true);
        }

        function btnNext_click() {
            switch ($('.step:visible').attr('id')) {
                case 'divStep1':
                    if (_selectedAORReleaseID == 0 && $('#btnAddNewAOR').is(':visible')) {
                        var aorName = $('#<%=this.txtAORName.ClientID %>').val();

                        if (aorName.length > 0) {
                            PageMethods.AORExists($('#<%=this.txtAORName.ClientID %>').val(), step1_done, step1_on_error);
                        }
                        else {
                            MessageBox('Please enter an AOR Name.');
                        }
                    }
                    else if (_selectedAORReleaseID > 0) {
                        step1_done(false);
                    }
                    else {
                        MessageBox('Please select an AOR.');
                    }
                    break;
                case 'divStep2':
                    step2_done(false);
                    break;
                case 'divStep3':
                    step3_done(false);
                    break;
                case 'divStep4':
                    step4_done(false);
                    break;
                case 'divStep5':
                    step5_done(false);
                    break;
                case 'divStep6':
                    step6_done(false);
                    break;
            }
            checkStepCompletion();
        }

        function step1_done(newAORExists) {
            if (!newAORExists) {
                $('.navselected').removeClass('navselected');
                $('#divStep1').fadeOut(function () { $('#divStep2').fadeIn(function () { $('#tdNav').show(); }); })
                $('#btnSave').show();
                $('#btnSaveContinue').show();
                $('#btnBack').show();

                //default AOR Workload Type to Release/Deployment MGMT AOR
                $('#<%=this.ddlWorkType.ClientID %>').val('2');

                if (_blnAORChanged) {
                    clearAll();
                }

                if ($('#navStep1').find('img[title="Visited"]').length == 0) $('#navStep1').prepend('<img src="Images/Icons/check.png" title="Visited" alt="Visited" width="15" height="15" />');
                if ($('#navStep2').find('img[title="Visited"]').length == 0) $('#navStep2').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep2').addClass('navselected');

                if (_blnAORChanged) {
                    _blnAORChanged = false;
                    if (_selectedAORReleaseID > 0) {
                        getAOR();
                        $('.selectedAOR').text('(' + _selectedAORName + ')');
                    }
                    else {
                        prepopulateAOR();
                        $('.selectedAOR').text('(' + $('#<%=this.txtAORName.ClientID %>').val() + ')');
                        $('#<%=this.ddlProductVersion.ClientID %>').prop('disabled', false);
                        if ($('#<%=this.ddlProductVersion.ClientID %>').val() == null) $('#<%=this.ddlProductVersion.ClientID %>').val('<%=currentReleaseID %>');
                    }
                }
            }
            else {
                MessageBox('This AOR Name already exists.');
            }
        }

        function step1_on_error() {
            MessageBox('An error has occurred.');
        }

        function clearAll() {
            $('img[title="Visited"]').remove();
            $('#<%=this.txtDescription.ClientID %>').val('');
            $('#<%=this.ddlProductVersion.ClientID %>').val('');
            $('#<%=this.ddlProductVersion.ClientID %>').prop('disabled', true);
            <%--$('#<%=this.ddlWorkloadAllocation.ClientID %>').empty();--%>
            $('#<%=this.ddlWorkloadAllocation.ClientID %>').val('');
            $('#<%=this.ddlWorkType.ClientID %>').val('2');

            $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function() {
                $(this).find('input[type=checkbox][field="System"]').prop('checked', false);
            });

            $('#<%=this.divAORResources.ClientID %> tr').not(':first').each(function() {
                $(this).find('input[type=checkbox][field="Resource"]').prop('checked', false);
            });

            arrCRs = [];
            arrTasks = [];

            loadCRGrid();
            loadGrid();
        }

        function getAOR() {
            $('#frmTasks').attr('src', 'AOR_Wizard_Frame.aspx' + window.location.search + '&AORID=' + _selectedAORID + '&Type=Task');
            //PageMethods.GetWorkloadAllocation(_selectedAORID, _selectedAORReleaseID, getWorkloadAllocation_done, getWorkloadAllocation_on_error);
            PageMethods.GetAOR(_selectedAORID, getAOR_done, getAOR_on_error);
            PageMethods.GetSystems(_selectedAORID, getSystems_done, getSystems_on_error);
            PageMethods.GetResources(_selectedAORID, getResources_done, getResources_on_error);
            PageMethods.GetCRs(_selectedAORID, getCRs_done, getCRs_on_error);
            PageMethods.GetTasks(_selectedAORID, getTasks_done, getTasks_on_error);
        }

        function getAOR_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                $('#<%=this.txtDescription.ClientID %>').val(dt[0].Description);
                $('#<%=this.ddlProductVersion.ClientID %>').val(dt[0].ProductVersion_ID);
                $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(dt[0].WorkloadAllocation_ID);
                <%--if ($('#<%=this.ddlWorkloadAllocation.ClientID %>').val() != dt[0].WorkloadAllocation_ID) {
                    $('#<%=this.ddlWorkloadAllocation.ClientID %>').prepend($("<option />").val(dt[0].WorkloadAllocation_ID).text(dt[0]["Workload Allocation"]));
                    $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(dt[0].WorkloadAllocation_ID);
                }--%>
                $('#<%=this.ddlWorkType.ClientID %>').val(dt[0].AORWorkType_ID);
            }
            checkStepCompletion();
        }

        function getAOR_on_error() {
            MessageBox('An error has occurred.');
        }

        function getWorkloadAllocation_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                var contractID = 0;
                $.each(dt, function (rowIndex, row) {
                    if (contractID != this.ContractID) {
                        $('#<%=this.ddlWorkloadAllocation.ClientID %>').append($("<option disabled='disabled' style='background-color: white;'/>").text(this.CONTRACT));
                        contractID = this.ContractID;
                    }
                    $('#<%=this.ddlWorkloadAllocation.ClientID %>').append($("<option />").val(this.WorkloadAllocationID).text(this.WorkloadAllocation));
                });
                $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(dt[0].WorkloadAllocationID);
            }
        }

        function getWorkloadAllocation_on_error() {
            MessageBox('An error has occurred.');
        }


        function ddlPrimarySystem_change() {
            PageMethods.loadWorkloadAllocation($('#<%=this.ddlPrimarySystem.ClientID%>').val(), loadWorkloadAllocation_done, loadWorkloadAllocation_on_error);
        }

        function ddlWorkType_change() {
            $('#frmTasks').attr('src', 'AOR_Wizard_Frame.aspx' + window.location.search + '&AORID=' + _selectedAORID + '&Type=Task' + '&WorkloadTypeID=' + $('#<%=this.ddlWorkType.ClientID%>').val());
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

        function getSystems_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function () {
                        var $obj = $(this).find('input[type=checkbox][field="System"]');

                        if (parseInt($obj.attr('systemid')) == row.WTS_SYSTEM_ID) {
                            $obj.prop('checked', true);

                            if (row.Primary) {
                                $('#<%=this.ddlPrimarySystem.ClientID%>').val(row.WTS_SYSTEM_ID);
                                $(this).remove();
                            }
                            return;
                        }
				    });
                });

                sortSystems();
            }
            checkStepCompletion();
        }

        function getSystems_on_error() {
            MessageBox('An error has occurred.');
        }

        function sortSystems() {
            var sorted = true;

            do {
                sorted = true;

                var systemRows = $('[sysrow=1]');

                for (var i = 0; i < systemRows.length - 1; i++) {
                    var firstRow = systemRows[i];
                    var secondRow = systemRows[i + 1];

                    var firstRowChecked = $(firstRow).find('input[type=checkbox][field="System"]:checked').length > 0;
                    var secondRowChecked = $(secondRow).find('input[type=checkbox][field="System"]:checked').length > 0;

                    var firstSort = parseInt($(firstRow).attr('origsort'));
                    var secondSort = parseInt($(secondRow).attr('origsort'));

                    var swap = false;
                    
                    if ((!firstRowChecked && secondRowChecked) ||
                        (firstRowChecked && secondRowChecked && firstSort > secondSort) ||
                        (!firstRowChecked && !secondRowChecked && firstSort > secondSort)) {
                        swap = true;
                        sorted = false;
                    }
                        
                    if (swap) {
                        systemRows[i] = secondRow;
                        systemRows[i + 1] = firstRow;
                        $(secondRow).insertBefore(firstRow);
                    }                    
                }

            } while (!sorted);
        }

        function getResources_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    $('#<%=this.divAORResources.ClientID %> tr').not(':first').each(function() {
                        var $obj = $(this).find('input[type=checkbox][field="Resource"]');

                        if (parseInt($obj.attr('resourceid')) == row.WTS_RESOURCE_ID) {
                            $obj.prop('checked', true);
                            return;
                        }
				    });
                });

                sortResources();
            }
            checkStepCompletion();
        }

        function getResources_on_error() {
            MessageBox('An error has occurred.');
        }

        function sortResources() {
            var sorted = true;

            do {
                sorted = true;

                var rscRows = $('[rscrow=1]');

                for (var i = 0; i < rscRows.length - 1; i++) {
                    var firstRow = rscRows[i];
                    var secondRow = rscRows[i + 1];

                    var firstRowChecked = $(firstRow).find('input[type=checkbox][field="Resource"]:checked').length > 0;
                    var secondRowChecked = $(secondRow).find('input[type=checkbox][field="Resource"]:checked').length > 0;

                    var firstSort = parseInt($(firstRow).attr('origsort'));
                    var secondSort = parseInt($(secondRow).attr('origsort'));

                    var swap = false;

                    if ((!firstRowChecked && secondRowChecked) ||
                        (firstRowChecked && secondRowChecked && firstSort > secondSort) ||
                        (!firstRowChecked && !secondRowChecked && firstSort > secondSort)) {
                        swap = true;
                        sorted = false;
                    }

                    if (swap) {
                        $(secondRow).insertBefore(firstRow);
                    }
                }

            } while (!sorted);
        }

        function getCRs_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    arrCRs.push(row.CR_ID.toString());
                });
            }

            loadCRGrid();
            checkStepCompletion();
        }

        function getCRs_on_error() {
            MessageBox('An error has occurred.');
        }

        function getTasks_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    arrTasks.push(row.Task_ID.toString());
                });
            }

            loadGrid();
            checkStepCompletion();
        }

        function getTasks_on_error() {
            MessageBox('An error has occurred.');
        }

        function prepopulateAOR() {
            var nFilter = decodeURIComponent('<%=this.Filter %>').split('|');

            for (var i = 0; i < nFilter.length; i++) {
                var nItem = nFilter[i].split('=');

                switch (nItem[0].toUpperCase()) {
                    case 'RELEASE_ID':
                        $('#<%=this.ddlProductVersion.ClientID %>').val(nItem[1]);
                        break;
                    case 'AOR_PRODUCTION_STATUS_ID':
                        $('#<%=this.ddlWorkloadAllocation.ClientID %>').val(nItem[1]);
                        break;
                    <%--case 'AORTYPE_ID':
                        $('#<%=this.ddlWorkType.ClientID %>').val(nItem[1]);
                        break;--%>
                    case 'AOR_SYSTEM_ID':
                        $('#<%=this.divAORSystems.ClientID %> tr').not(':first').each(function () {                            
                            var $obj = $(this).find('input[type=checkbox][field="System"]');

                            if ($obj.attr('systemid') == nItem[1]) {
                                $obj.prop('checked', true);
                                return;
                            }
                        });
                        break;
                    case 'RESOURCES_ID':
                        $('#<%=this.divAORResources.ClientID %> tr').not(':first').each(function () {
                            var $obj = $(this).find('input[type=checkbox][field="Resource"]');

                            if ($obj.attr('resourceid') == nItem[1]) {
                                $obj.prop('checked', true);
                                return;
                            }
                        });
                        break;
                    case 'CRNAME_ID':
                        arrCRs.push(nItem[1]);
                        loadCRGrid();
                        break;
                    case 'TASK_ID':
                        arrTasks.push(nItem[1]);
                        loadGrid();
                        break;
                }
            }
        }

        function step2_done(back) {
            if (back) {
                $('#tdNav').hide();
                $('#btnSave').hide();
                $('#btnSaveContinue').hide();
                $('#btnBack').hide();
                $('#divStep2').fadeOut(function () { $('#divStep1').fadeIn(); });
                if (_selectedAORID === 0) {
                    _selectedAORID = undefined;
                    _selectedAORName = '';
                    _selectedAORReleaseID = undefined;
                }
            }
            else {
                $('#navStep2').removeClass('navselected');
                if ($('#navStep3').find('img[title="Visited"]').length == 0) $('#navStep3').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep3').addClass('navselected');
                $('#divStep2').fadeOut(function () { $('#divStep3').fadeIn(); });
            }
        }

        function step3_done(back) {
            if (back) {
                $('#navStep3').removeClass('navselected');
                if ($('#navStep2').find('img[title="Visited"]').length == 0) $('#navStep2').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep2').addClass('navselected');
                $('#divStep3').fadeOut(function () { $('#divStep2').fadeIn(); });
            }
            else {
                $('#navStep3').removeClass('navselected');
                if ($('#navStep4').find('img[title="Visited"]').length == 0) $('#navStep4').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep4').addClass('navselected');
                $('#divStep3').fadeOut(function () { $('#divStep4').fadeIn(); });
            }
        }

        function step4_done(back) {
            if (back) {
                $('#navStep4').removeClass('navselected');
                if ($('#navStep3').find('img[title="Visited"]').length == 0) $('#navStep3').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep3').addClass('navselected');
                $('#divStep4').fadeOut(function () { $('#divStep3').fadeIn(); });
            }
            else {
                $('#navStep4').removeClass('navselected');
                if ($('#navStep6').find('img[title="Visited"]').length == 0) $('#navStep6').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep6').addClass('navselected');
                $('#divStep4').fadeOut(function () { $('#divStep6').fadeIn(); });
            }
        }

        function step5_done(back) {
            if (back) {
                $('#navStep5').removeClass('navselected');
                if ($('#navStep4').find('img[title="Visited"]').length == 0) $('#navStep4').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep4').addClass('navselected');
                $('#divStep5').fadeOut(function () { $('#divStep4').fadeIn(); });
            }
            else {
                $('#navStep5').removeClass('navselected');
                if ($('#navStep6').find('img[title="Visited"]').length == 0) $('#navStep6').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep6').addClass('navselected');
                $('#divStep5').fadeOut(function () { $('#divStep6').fadeIn(function () { $('#frmCRs')[0].contentWindow.resizePage(); }); });
            }
        }

        function step6_done(back) {
            if (back) {
                $('#navStep6').removeClass('navselected');
                if ($('#navStep4').find('img[title="Visited"]').length == 0) $('#navStep4').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep4').addClass('navselected');
                $('#divStep6').fadeOut(function () { $('#divStep4').fadeIn(); });
            }
            else {
                $('#navStep6').removeClass('navselected');
                if ($('#navStep7').find('img[title="Visited"]').length == 0) $('#navStep7').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep7').addClass('navselected');
                $('#btnNext').hide();
                $('#divStep6').fadeOut(function () { $('#divStep7').fadeIn(function () { $('#frmTasks')[0].contentWindow.resizePage(); }); });
            }
        }

        function step7_done(back) {
            if (back) {
                $('#navStep7').removeClass('navselected');
                if ($('#navStep6').find('img[title="Visited"]').length == 0) $('#navStep6').prepend('<img src="Images/Icons/check_gray.png" title="Visited" alt="Visited" width="15" height="15" />');
                $('#navStep6').addClass('navselected');
                $('#btnNext').show();
                $('#divStep7').fadeOut(function () { $('#divStep6').fadeIn(); });
            }
        }

        function input_change(obj) {
            var $obj = $(obj);

            if ($obj.attr('id') && $obj.attr('id').indexOf('AORName') != -1) {
                if (event.keyCode == 13 || event.keyCode == 144) $('#btnSearch').trigger('click');

                $('#divAOR').html('Please click the Search button to select an AOR.');
            }

            if ($obj.attr('id') && $obj.attr('id').indexOf('TaskSearch') != -1) {
                var nVal = $obj.val();

                //$obj.val(nVal.replace(/[^\d,]/g, ''));

                var text = $(obj).val();

                if (/[^0-9-,]|^0+(?!$)/g.test(text)) {
                    $(obj).val(text.replace(/[^0-9-,]|^0+(?!$)/g, ''));
                }
            }

            checkStepCompletion();
        }

        function checkStepCompletion() {
            if ($('#txtDescription').val().length > 0) $('#navStep2').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep2').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');

            if ($('#ddlPrimarySystem').val() > 0 && $('#ddlWorkloadAllocation').val() > 0 && $('#ddlWorkType').val() > 0) $('#navStep3').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep3').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');

            var systemSelected = false;
            $('#divAORSystems').find('input[type="checkbox"]').each(function () {
                if ($(this)[0].checked) systemSelected = true;
            });
            if (systemSelected) $('#navStep4').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep4').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');

            var resourceSelected = false;
            $('#divAORResources').find('input[type="checkbox"]').each(function () {
                if ($(this)[0].checked) resourceSelected = true;
            });
            if (resourceSelected) $('#navStep5').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep5').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');

            if ($('#spnSelectedCountCR').text() != "0 CRs Checked") $('#navStep6').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep6').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');

            if ($('#spnSelectedCount').text() != "0 Tasks Checked") $('#navStep7').find('img[title="Visited"]').attr('src', 'Images/Icons/check.png');
            else $('#navStep7').find('img[title="Visited"]').attr('src', 'Images/Icons/check_gray.png');
        }

        function ddlCRStatusQF_update() {
            var arrCRStatusQF = $('#<%=this.ddlCRStatusQF.ClientID %>').multipleSelect('getSelects');
			
            _selectedCRStatusesQF = arrCRStatusQF.join(',');
        }

        function ddlCRStatusQF_close() {
            loadCRGrid();
        }

        function ddlCRContractQF_update() {
            var arrCRContractQF = $('#<%=this.ddlCRContractQF.ClientID %>').multipleSelect('getSelects');

            _selectedCRContractsQF = arrCRContractQF.join(',');
        }

        function ddlCRContractQF_close() {
            loadCRGrid();
        }

	    function imgShowFilters_click() {
	        var nWindow = 'FilterPage';
	        var nTitle = 'Filter and Criteria';
	        var nHeight = 450, nWidth = 900;
	        var nURL = 'FilterPage.aspx?parentModule=Work&MyData=False&Source=AOR';
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function btnTaskSearch_click() {
	        loadGrid();
	    }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage() {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'AORName', '');
            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function loadCRGrid() {
            if (typeof $('#frmCRs')[0].contentWindow.loadGrid != "undefined") {
                setTimeout(function () { $('#frmCRs')[0].contentWindow.loadGrid(); }, 1);
            }
        }

        function loadGrid() {
            var filterCount = filterBox.filters.length;

            $('#spnFilterCount').text(filterCount + ' Filter' + (filterCount != 1 ? 's' : '') + ' Applied');

            if (typeof $('#frmTasks')[0].contentWindow.loadGrid != "undefined") {
                setTimeout(function () { $('#frmTasks')[0].contentWindow.loadGrid(); }, 1);
            }
        }

        function showWizardStatusMessage(msg) {
            var msgDiv = $('#wizardStatusMessage');
            
            if (msgDiv.is(':visible')) {
                clearTimeout(_statusMsgTimer);

                msgDiv.html(msg);
            }
            else {
                msgDiv.html(msg);
                msgDiv.show();
            }

            _statusMsgTimer = setTimeout(hideWizardStatusMessage, 500);
        }

        function hideWizardStatusMessage() {
            var msgDiv = $('#wizardStatusMessage');

            $(msgDiv).hide();
            $(msgDiv).html('');
        }

        function systemNameSearch() {
            showWizardStatusMessage('Filtering...');

            var searchStr = $('#txtSystemNameSearch').val().toLowerCase();
            var searchBlank = searchStr == '';

            var systemRows = $('[sysrow=1]');

            $.each(systemRows, function (rowIdx, row) {
                var nameTD = $(row).find('td')[1];

                if (searchBlank || $(nameTD).text().toLowerCase().indexOf(searchStr) != -1) {
                    $(row).show();
                }
                else {
                    $(row).hide();
                }
            });
        }

        function resourceNameSearch() {
            showWizardStatusMessage('Filtering...');

            var searchStr = $('#txtResourceNameSearch').val().toLowerCase();
            var searchBlank = searchStr == '';

            var systemRows = $('[rscrow=1]');

            $.each(systemRows, function (rowIdx, row) {
                var nameTD = $(row).find('td')[1];

                if (searchBlank || $(nameTD).text().toLowerCase().indexOf(searchStr) != -1) {
                    $(row).show();
                }
                else {
                    $(row).hide();
                }
            });
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {
            $('#<%=this.ddlCRStatusQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
				    ,width: 'undefined'
					,onOpen: function() { ddlCRStatusQF_update(); }
					,onClose: function() { ddlCRStatusQF_close(); }
            }).change(function () { ddlCRStatusQF_update(); });

            $('#<%=this.ddlCRContractQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onOpen: function () { ddlCRContractQF_update(); }
                , onClose: function () { ddlCRContractQF_close(); }
            }).change(function () { ddlCRContractQF_update(); });
        }

        function initDisplay() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') $('#btnNext').show();

            $('#<%=this.txtAORName.ClientID %>').focus();
            $('#frmCRs').attr('src', 'AOR_Wizard_Frame.aspx' + window.location.search + '&Type=CR');
        }

        function initEvents() {
            $('.nav').hover(function () {
                $(this).toggleClass('navhover');
            });
            $('.nav').click(function () { nav_click(this); });
            $('#<%=this.txtAORName.ClientID %>').on('keydown', function (event) {
                if (event.keyCode == 13 || event.keyCode == 144) {
                    event.preventDefault();
                    return false;
                }
                else {
                    $('#btnNext').prop('disabled', true);
                }
            });
            $('#imgShowFilters').click(function () { imgShowFilters_click(); });
            $('#btnTaskSearch').click(function () { btnTaskSearch_click(); });
            $('#txtTaskSearch').on('keydown', function (event) {
                if (event.keyCode == 13 || event.keyCode == 144) {
                    event.preventDefault();
                    return false;
                }
            });
            $('#txtTaskSearch').on('keyup', function (event) {
                if (event.keyCode == 13 || event.keyCode == 144) $('#btnTaskSearch').trigger('click');
            });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            <%--$('#<%=this.ddlPrimarySystem.ClientID %>').change(function () { ddlPrimarySystem_change(); return false; });--%>
            $('#<%=this.ddlWorkType.ClientID %>').change(function () { ddlWorkType_change(); return false; });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
            $('#btnClear').click(function () { btnClear_click(); return false; });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(false); return false; });
            $('#btnSaveContinue').click(function () { btnSave_click(true); return false; });
            $('#btnBack').click(function () { btnBack_click(); return false; });
            $('#btnNext').click(function () { btnNext_click(); return false; });
            $('#txtSystemNameSearch').on('keyup', systemNameSearch);
            $('#txtSystemNameSearch').on('change', systemNameSearch);
            $('#txtResourceNameSearch').on('keyup', resourceNameSearch);
            $('#txtResourceNameSearch').on('change', resourceNameSearch);
            $('#btnClearSystemNameSearch').click(function () { $('#txtSystemNameSearch').val(''); systemNameSearch(); });
            $('#btnClearResourceNameSearch').click(function () { $('#txtResourceNameSearch').val(''); resourceNameSearch(); });

            $(document).on('change', 'input[type=checkbox][field="System"]', function () {
                if ($(this).is(':checked')) showWizardStatusMessage('Moving to top...');
                sortSystems();

                var numChecked = $('input[type=checkbox][field="System"]:checked').length;
                if (!$(this).is(':checked') && $($($(this)[0].parentElement.parentElement)[0].lastChild)[0].lastChild.checked === true) {
                    $($($(this)[0].parentElement.parentElement)[0].lastChild)[0].lastChild.checked = false;
                }
                checkStepCompletion();
            });

            $(document).on('change', 'input[type=checkbox][field="Resource"]', function () {
                if ($(this).is(':checked')) showWizardStatusMessage('Moving to top...');
                sortResources();
                checkStepCompletion();
            });
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();

            if ($.trim($('#<%=this.txtAORName.ClientID %>').val()).length > 0) $('#btnSearch').trigger('click');
        });
    </script>
</body>
</html>