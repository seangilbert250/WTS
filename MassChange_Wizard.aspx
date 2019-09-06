﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MassChange_Wizard.aspx.cs" Inherits="MassChange_Wizard" Theme="Default" %>

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
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css?=2" />
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
                <td id="tdEntity" style="height: 370px; width: 100px; vertical-align: top; border-right: 1px solid gray; ">
                    <div class="pageContentHeader" style="padding: 5px;">
                            Select Entity&nbsp;<span class="selectedAOR"></span>
			        </div>
                    <div id="divEntity" runat="server" style="border-collapse: collapse; width: 100%;">
                        <%--<tr>
                            <td>
                                <div class="pageContentHeader" style="padding: 5px; width: 96%;">
                                    Select Entity
			                    </div>
                            </td>
                        </tr>--%>
                        <%--<tr>
                            <td>
                                <div id="AOR" stepType="Entity" class="nav">
                                    AOR
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="CR" stepType="Entity" class="nav">
                                    CR
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="Task" stepType="Entity" class="nav">
                                    Task
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="Sub-Task" stepType="Entity" class="nav">
                                    Sub-Task
                                </div>
                            </td>
                        </tr>--%>
                    </div>
                </td>
                <td id="tdField" style="height: 370px; width: 100px; vertical-align: top; border-right: 1px solid gray; ">
                    <div class="pageContentHeader" style="padding: 5px;">
                            Select Field&nbsp;<span class="selectedAOR"></span>
			        </div>
                    <%--<div id="divField" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;">
                        
                    </div>--%>
                    <table id="tblField" style="border-collapse: collapse; width: 100%;">
                        
                    </table>
                </td>
                <td id="tdExistingValue" style="height: 370px; width: 200px; vertical-align: top; border-right: 1px solid gray;">
                    <div class="pageContentHeader" style="padding: 5px;">
                            Select Existing Value&nbsp;<span class="selectedAOR"></span>
			        </div>
                    <div id="divExistingValue" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;">
                        
                    </div>
                </td>
                <td id="tdNewValue" style="height: 370px; width: 200px; vertical-align: top; border-right: 1px solid gray; ">
                    <div class="pageContentHeader" style="padding: 5px;">
                            Select New Value&nbsp;<span class="selectedAOR"></span>
			        </div>
                    <div id="divNewValue" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;">
                        
                    </div>
                </td>
                <td id="tdEntities" style="height: 370px; width: 600px; vertical-align: top; border-right: 1px solid gray; ">
                    <div class="pageContentHeader" style="padding: 5px;">
                            Select Entities&nbsp;<span class="selectedAOR"></span>
			        </div>
                    <div id="divEntities" runat="server" style="height: 345px; overflow-x: hidden; overflow-y: auto;">
                        
                    </div>
                </td>
        <td style="vertical-align: top; display:none;">
	<div id="divStep1" class="step" nextstep="divStep2" prevstep="divStep1" >
            <div class="pageContentHeader" style="padding: 5px;">
                Step 1: Select Entity to Update
			</div>
            <div style="padding: 10px;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="width: 70px; text-align:left;">
                            Entity:
                            <select id="ddlEntity" runat="server" >
                                <option value="0">-Select-</option>
                                <option value="AOR">AOR</option>
                                <option value="CR">CR</option>
                                <option value="Primary Task">Primary Task</option>
                                <option value="Subtask">Subtask</option>
                            </select>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="divStep2" class="step" style=" overflow-x: hidden; overflow-y: auto;" nextstep="divStep3" prevstep="divStep1" >
            <div class="pageContentHeader" style="padding: 5px;">
                Step 2: Select Field to Change
			</div>
            <div style="padding: 10px;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="width: 70px; text-align:left;">
                            Field to Change:
                            <select id="ddlFieldChange" runat="server" ></select>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="divStep3" class="step" style=" overflow-x: hidden; overflow-y: auto;" nextstep="divStep4" prevstep="divStep2" >
            <div class="pageContentHeader" style="padding: 5px;">
                Step 3: Select Existing Value to change
			</div>
            <div style="padding: 10px;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="width: 70px; text-align:left;">
                            Existing Value:
                            <select id="ddlExistingValue" runat="server" multiple="true" ></select>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="divStep4" class="step" style="overflow-x: hidden; overflow-y: auto;" nextstep="divStep5" prevstep="divStep3" >
            <div class="pageContentHeader" style="padding: 5px;">
                Step 4: Select New Value:
			</div>
            <div style="padding: 10px;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="width: 70px;text-align:left;">
                            New Value:
                            <select id="ddlNewValue" runat="server"></select>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="divStep5" class="step" style="overflow-x: hidden; overflow-y: auto;" nextstep="divStep1" prevstep="divStep4" >
            <div class="pageContentHeader" style="padding: 5px;">
                Step 5: Limit Entities to Update
			</div>
            <div style="padding: 10px;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="width: 70px;text-align:left;">
                            Entities:
                            <select id="ddlSelEntities" runat="server" multiple="true" ></select>
                        </td>
                    </tr>
                </table>
            </div>
        </div>       
            </td>
            </tr>
        </table>  
     <%--   <table style="border-collapse: collapse; width: 100%;">
            <tr>
                <td id="tdEntity" style="height: 370px; width: 80px; vertical-align: top; border-right: 1px solid gray; ">
                    <table id="tblEntity" style="border-collapse: collapse; width: 100%;">
                        
                    </table>
                </td>
            </tr>
        </table>--%>
	<div id="divPageFooter" class="PopupFooter">
		<table cellpadding="0" cellspacing="0" style="width: 100%; text-align:right;">
			<tr>
				<td>&nbsp;</td>
                <td>
					<input type="button" id="btnBack" value="Back" style="display:none;"/>
				</td>
                <td>
					<input type="button" id="btnNext" value="Next" style="display:none;"/>
				</td>
                <td>
					<input type="button" id="btnFinish" value="Finish" disabled="disabled" />
				</td>
				<td>
					<input type="button" id="btnClose" value="Cancel" />
				</td>
			</tr>
		</table>
	</div>
	<div id="divPageDimmer" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: grey; filter: alpha(opacity=60); opacity: .60; display: none;"></div>
	<div id="divSaving" style="position: absolute; left: 35%; top: 15%; padding: 10px; background: white; border: 1px solid grey; font-size: 18px; text-align: center; display: none;">
		<table>
			<tr>
				<td>WTS is Saving Data... Please wait...</td>
			</tr>
			<tr>
				<td>
					<img alt='' src="Images/loaders/progress_bar_blue.gif" /></td>
			</tr>
		</table>
	</div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
</form>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAORID, _selectedAORReleaseID;
        var _selectedAORName = '';
        var filterBox = new filterContainer('divAppliedFilters');
        var _selectedCRStatusesQF = '';
        var arrCurrentEntity = [];
        var arrAORs = [];
        var arrCRs = [];
        var arrTasks = [];
        var arrSubTasks = [];
    </script>
<script id="jsEvents" type="text/javascript">

    function btnClose_click() {
        if ($('#btnFinish').is(':enabled')) {
            QuestionBox('Confirm Close', 'Are you sure you would like to close?', 'Yes,No', 'confirmClose', 300, 300, this);
        }
        else {
            closeWindow();
        }
    }

    function confirmClose(answer) {
        if ($.trim(answer).toUpperCase() == 'YES') closeWindow();
    }

    function btnBack_click() {
                stepFinished($('.step:visible'), false);
    }

    function setSelectedEntity(strEntity) {
        arrCurrentEntity = [];
        switch (strEntity) {
            case 'AOR':
                arrCurrentEntity = arrAORs;
                break;
            case 'CR':
                arrCurrentEntity = arrCRs;
                break;
            case 'Primary Task':
                arrCurrentEntity = arrTasks;
                break;
            case 'Subtask':
                arrCurrentEntity = arrSubTasks;
                break;
        }
        var idField = '';
        var columnName = '';
        var textField = ''

        var strEntity = '';
        var filterField = '';
        var existingValueFilter = [];
        if (arrCurrentEntity.length > 0) {

            $.each(arrCurrentEntity, function (index, value) {
                if (index == 0) {
                    idField = value.strField;
                    filterField = value.strField;
                    strEntity = value.strEntity;
                }
                if (strEntity == value.strEntity) {
                    existingValueFilter.push(value.strFieldID);
                }
            });
            ShowDimmer(true, "Loading " + strEntity + "...", 1);
            PageMethods.LoadFilteredEntity(strEntity, idField, columnName, textField, existingValueFilter.join(','), '', '', LoadFilteredEntity_done, on_error);
        }

        
    }

    function btnNext_onclick(obj) {
        try {
            if ($(obj).attr('id') != 'navStep1') {
                $('.navselected[stepType="' + $(obj).attr('stepType') + '"]').removeClass('navselected');
                    $(obj).addClass('navselected');
                }

                switch ($(obj).attr('stepType')) {
                    
                case 'Entity':
                    setSelectedEntity($(obj).attr('id'));
                    $('#divExistingValue').empty();
                    $('#divNewValue').empty();
                    $('#divEntities').empty();
                    var opt = $('#ddlEntity option:selected');
                    var columnName = $(opt).val();
                    var textField = $(opt).text();
                        
                    PageMethods.LoadEntityFields($(obj).attr('id'), LoadEntityFields_done, on_error);
                    
                    break;
                case 'Field':
                    
                    $('#divExistingValue').empty();
                    $('#divNewValue').empty();

                    if (arrCurrentEntity.length == 0) {
                        $('#divEntities').empty();
                    }

                    var opt = $('#ddlFieldChange option:selected');
			        var columnName = $(opt).val();
			        var textField = $(opt).text();
			        var idField = $(opt).attr('id_field');
                        
                    LoadExistingValues($('#tblEntity .navselected').attr('id'), $(obj).attr('id'), $(obj).attr('value'), $(obj).text());
                    break;
            }
        }
        catch (e) { }
    }

    function stepFinished(objStep, blnNext) {
        var nextStep = $('#' + objStep.attr('nextstep'));
        var prevStep = $('#' + objStep.attr('prevstep'));
        if (blnNext) {
            objStep.hide("slide", { direction: "left" }, 500, function () {
                nextStep.show("slide", { direction: "right" }, 500);
            });
        }
        else {
            objStep.hide("slide", { direction: "right" }, 500, function () {
                prevStep.show("slide", { direction: "left" }, 500);
            });
        }
    }

    function save_done(result) {
        try {
            ShowDimmer(false);

            var obj = jQuery.parseJSON(result);

            var saved = false;
            var count = 0;
            var errorMsg = '';

            if (obj) {
                if (obj.saved) {
                    saved = (obj.saved.toUpperCase() == 'TRUE');
                }
                if (obj.Count) {
                    count = parseInt(obj.Count);
                }
                if (obj.error) {
                    errorMsg = obj.error;
                }
            }

            var msg = '';
            if (saved) {
                msg = 'Updated ' + count + ' ' + $('#tblEntity .navselected').attr('id') + ' ' + $('.navselected[stepType="Field"]').text() + '.';
            }
            else {
                msg = 'Failed to update Entity ' + $('#tblEntity.navselected').attr('id') + ' ' + $('.navselected[stepType = "Field"]').text() + '.';
            }

            if (errorMsg.length > 0) {
                if (msg.length > 0) {
                    msg += '\n';
                }
                msg += errorMsg;
            }

            MessageBox(msg);

            if (opener && opener.refreshPage) {
                opener.refreshPage(true);
            }
            refreshPage();
        } catch (e) {
            ShowDimmer(false);
        }
    }

    function LoadFilteredEntity(entityType, idField, columnName, textField, existingValueFilter) {
        try {
            $('#<%=this.ddlSelEntities.ClientID %>').empty();
            ShowDimmer(true, "Loading " + entityType + "...", 1);

            var strEntity = '';
            var filterField = '';
            var filterFieldIDs = [];
                if (arrCurrentEntity.length > 0) {

                    $.each(arrCurrentEntity, function (index, value) {
                        if (index == 0) {
                            filterField = value.strField;
                            strEntity = value.strEntity;
                        }
                        if (strEntity == value.strEntity) {
                            filterFieldIDs.push(value.strFieldID);
                        }
                    });
                }

            PageMethods.LoadFilteredEntity(entityType, idField, columnName, textField, existingValueFilter, filterField, filterFieldIDs.join(','), LoadFilteredEntity_done, on_error);
        } catch (e) {
            ShowDimmer(false);
            MessageBox('There was an error gathering ' + entityType + ' list.\n' + e.message);
        }
    }
    function LoadFilteredEntity_done(result) {
    var nHTML = '';
            var dt = jQuery.parseJSON(result);
            
            nHTML += '<table style="border-collapse: collapse; width: 100%;">';
            nHTML += '<tr class="gridHeader">';
            nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 10px;">'
            nHTML += '<input fieldId="chkEntity_all" type="checkbox" onchange="chkEntity_change(this);" />';
            nHTML += '</th > ';
            nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 10px;">' + $('.navselected[stepType="Entity"]').text() + ' #</th>';
            nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">' + $('.navselected[stepType="Entity"]').text() + '</th>';
            nHTML += '</tr>';

            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {

                    var entityNumber = '';
                    if ($('.navselected[stepType="Entity"]').attr('id') == 'Subtask') {
                        entityNumber = row.WORKITEMID + ' - ' + row.TASK_NUMBER
                    }
                    else{
                            entityNumber = row.valueField;
                        }
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="text-align: center;">';
                    if ('<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') nHTML += '<input type="checkbox" fieldId="' + row.valueField + '" onchange="chkEntity_change(this);" />';

                    nHTML += '</td>';
                    nHTML += '<td style="text-align: center; width: 10px;" >' + entityNumber + '</td>';
                        nHTML += '<td>' + row.textField + '</td>';
                        nHTML += '</tr>';
                    });
                }
            nHTML += '</table>';
            $('#divEntities').html(nHTML);
            ShowDimmer(false);
        }
 
    function LoadEntityFields_done(result) {
            var dt = jQuery.parseJSON(result);
            var nHTML = '';
            if (dt != null && dt.length > 0) {
                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr>';
                    nHTML += '<td>';
                    nHTML += '<div id="' + row.id_field + '" value="' + row.valueField + '" stepType="Field" class="nav" >' + row.textField + '</div>';
                    nHTML += '</td>';
                    nHTML += '</tr>';
                });

            $('#tblField').html(nHTML);
            $('.nav').unbind();
            $('.nav').hover(function () {
                $(this).toggleClass('navhover');
            });
            $('.nav').click(function () { btnNext_onclick(this); });
             
            }
    }

    function on_error() {
        ShowDimmer(false);
        MessageBox('An error has occurred.');
    }

    function LoadExistingValues(entityType,idField, columnName, textField) {
        try {
            if (idField.length == 0 || textField.length == 0) {
                MessageBox('Unable to gather Exsisting Value list. Attribute selection is invalid.');
                return;
            }
            var strEntity = '';
            var filterField = '';
            var existingValueFilter = [];
                if (arrCurrentEntity.length > 0) {
                    
                    $.each(arrCurrentEntity, function (index, value) {
                        if (index == 0) {
                            filterField = value.strField;
                            strEntity = value.strEntity;
                        }
                        if (strEntity == value.strEntity) {
                            existingValueFilter.push(value.strFieldID);
                        }
                    });
                }
            ShowDimmer(true, "Loading New Values for " + textField + "...", 1);

            PageMethods.LoadExistingValues(entityType, idField, columnName, textField, filterField, existingValueFilter.join(','),LoadExistingValues_done, on_error);
        } catch (e) {
            ShowDimmer(false);
            MessageBox('There was an error gathering Existing Value list.\n' + e.message);
        }
    }
    function LoadExistingValues_done(result) {
        try {
            ShowDimmer(false);

            var obj = jQuery.parseJSON(result);
            var loaded = false;
            var currentCount = 0, newCount = 0;
            var errorMsg = '';
            var dtCurrentOptions = {}, dtNewOptions = {};

            if (obj) {
                if (obj.loaded && obj.loaded.toUpperCase() == 'TRUE') {
                    loaded = true;
                }
                if (obj.CurrentCount) {
                    currentCount = parseInt(obj.CurrentCount);
                }
                if (obj.NewCount) {
                    newCount = parseInt(obj.NewCount);
                }
                if (obj.error) {
                    errorMsg = obj.error;
                }

                if (currentCount > 0 && obj.CurrentOptions) {
                    dtCurrentOptions = jQuery.parseJSON(obj.CurrentOptions);
                }
                if (newCount > 0 && obj.NewOptions) {
                    dtNewOptions = jQuery.parseJSON(obj.NewOptions);
                }
            }

            var nHTML = '';
            if (dtCurrentOptions != null && dtCurrentOptions.length > 0) {
                
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 10px;">';
                nHTML += '<input fieldId="chkExisting_all" type="checkbox" onchange="chkExistingVal_change(this);" />';
                nHTML += '</th > ';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">' + $('.navselected[stepType="Field"]').text() + '</th>';
                nHTML += '</tr>';
                
                    $.each(dtCurrentOptions, function (rowIndex, row) {
                        nHTML += '<tr class="gridBody">';
                        nHTML += '<td style="text-align: center;">';
                        if ('<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') nHTML += '<input fieldId="' + row.valueField + '" type="checkbox" onchange="chkExistingVal_change(this);" />';

                        nHTML += '</td>';
                        nHTML += '<td>' + row.textField + '</td>';
                        nHTML += '</tr>';
                    });
                nHTML += '</table>';
                $('#divExistingValue').html(nHTML);
            }
            $('#tdExistingValue').fadeIn();
            $('.nav').unbind();
            $('.nav').hover(function () {
                $(this).toggleClass('navhover');
            });
            $('.nav').click(function () { btnNext_onclick(this); });

            nHTML = '';
            if (dtNewOptions != null && dtNewOptions.length > 0) {
                
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 10px;"></th > ';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">' + $('.navselected[stepType="Field"]').text() + '</th>';
                nHTML += '</tr>';
                
                    $.each(dtNewOptions, function (rowIndex, row) {
                        nHTML += '<tr class="gridBody">';
                        nHTML += '<td style="text-align: center;">';
                        if ('<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') nHTML += '<input fieldId="' + row.valueField + '" type="checkbox" onchange="chkNewVal_change(this);" />';

                        nHTML += '</td>';
                        nHTML += '<td>' + row.textField + '</td>';
                        nHTML += '</tr>';
                    });
                nHTML += '</table>';
                $('#divNewValue').html(nHTML);
            }
            $('#tdNewValue').fadeIn();
            $('.nav').unbind();
            $('.nav').hover(function () {
                $(this).toggleClass('navhover');
            });
            $('.nav').click(function () { btnNext_onclick(this); });

            $('#btnFinish').prop('disabled', true);
        } catch (e) {
            ShowDimmer(false);
        }
    }

    function btnFinish_OnClick() {
        var newValue = '';
        var existingValue = [];
        var entityFilter = [];

        $('#<%=this.divNewValue.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) newValue = $obj.attr('fieldId');
        });

        $('#<%=this.divExistingValue.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) existingValue.push($obj.attr('fieldId'));
        });

        $('#<%=this.divEntities.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) entityFilter.push($obj.attr('fieldId'));
        });

        var opt = $('.navselected[stepType="Field"]');
        var columnName = $(opt).text();
        var textField = $(opt).text();
        var idField = $(opt).attr('id_field');

        PageMethods.SaveChanges($('#tblEntity .navselected').attr('id'), columnName, existingValue.join(','), newValue, entityFilter.join(','), save_done, on_error);
    }
    function chkUpdate_ready() {
        $('#btnFinish').prop('disabled', true);
        var newValue = '';
        var existingValue = [];
        var entityFilter = [];

        $('#<%=this.divNewValue.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) newValue = $obj.attr('fieldId');
        });

        $('#<%=this.divExistingValue.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) existingValue.push($obj.attr('fieldId'));
        });

        $('#<%=this.divEntities.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');
            if ($obj.is(':checked')) entityFilter.push($obj.attr('fieldId'));
        });

        if (entityFilter.length > 0 && existingValue.length > 0 && newValue.length > 0) $('#btnFinish').prop('disabled', false);
    }
    function chkEntity_change(entityVal) {
        var checkAll;

        if ($(entityVal)[0].checked && $(entityVal).attr('fieldid') === 'chkEntity_all') {
            checkAll = true;
        } else if (!$(entityVal)[0].checked && $(entityVal).attr('fieldid') === 'chkEntity_all') {
            checkAll = false;
        }

        $('#<%=this.divEntities.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');

            if (checkAll) {
                $obj[0].checked = "True";
            } else if (!checkAll && checkAll !== undefined) {
                $obj[0].checked = "";
            } else {
                $('input[fieldid=chkEntity_all]')[0].checked = "";
            }
        });
        
        chkUpdate_ready();
    }
    function chkNewVal_change(obj) {
        if ($(obj).is(':checked')) {
            $('#<%=this.divNewValue.ClientID %>').find('input[type=checkbox]').not($(obj)).prop('checked', false).prop('disabled', true);
            }
            else {
                $('#<%=this.divNewValue.ClientID %>').find('input[type=checkbox]').not($(obj)).prop('disabled', false);
        }

        chkUpdate_ready();
    }

    function chkExistingVal_change(existingVal) {
        var opt = $('.navselected[stepType="Field"]');
        var columnName = $(opt).attr('value');
        var textField = $(opt).text();
        var idField = $(opt).attr('id');
        var existingValueFilter = [];
        var checkAll;

        if ($(existingVal)[0].checked && $(existingVal).attr('fieldid') === 'chkExisting_all') {
            checkAll = true;
        } else if (!$(existingVal)[0].checked && $(existingVal).attr('fieldid') === 'chkExisting_all') {
            checkAll = false;
        }

        $('#<%=this.divExistingValue.ClientID %> tr').not(':first').each(function () {
            var $obj = $(this).find('input[type=checkbox]');

            if (checkAll) {
                $obj[0].checked = "True";
            } else if (!checkAll && checkAll !== undefined) {
                $obj[0].checked = "";
            } else {
                $('input[fieldid=chkExisting_all]')[0].checked = "";
            }

            if ($obj.is(':checked')) existingValueFilter.push($obj.attr('fieldId'));
        });
        
        LoadFilteredEntity($('#tblEntity .navselected').attr('id'), idField, columnName, textField, existingValueFilter.join(','));
        
    }

    function populateOptions(dtCurrentOptions, dtNewOptions) {
        $('#<%=this.ddlExistingValue.ClientID %>').empty();
        $('#<%=this.ddlNewValue.ClientID %>').empty();

        var value = '', text = '';
        $.each(dtCurrentOptions, function (index, val) {
            value = '';
            text = '';

            if (val.valueField) {
                value = val.valueField;
            }
            if (val.textField) {
                text = val.textField;
            }

            $('#<%=this.ddlExistingValue.ClientID %>').append('<option value="' + value + '">' + text + '</option>');
        });

        $('#<%=ddlExistingValue.ClientID %>').multipleSelect({
            placeholder: 'Default'
				    , width: 'undefined'
					, onOpen: function () { }
					, onClose: function () { }
        }).change(function () { });


        $.each(dtNewOptions, function (index, val) {
            value = '';
            text = '';

            if (val.valueField) {
                value = val.valueField;
            }
            if (val.textField) {
                text = val.textField;
            }

            $('#<%=this.ddlNewValue.ClientID %>').append('<option value="' + value + '">' + text + '</option>');
        });
    }
    </script>
    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {
          
        }

        function initDisplay() {
            if (opener.arrSelectedEntity) {
                if (opener.arrSelectedEntity.length > 0) {
                    var strEntity = '';
                    var idField = '';
                    var columnName = '';
                    var textField = '';
                    var existingValueFilter = [];
                    $.each(opener.arrSelectedEntity, function (index, value) {
                        if (index == 0) {
                            idField = value.strField;
                            strEntity = value.strEntity;
                            $('#' + value.strEntity.replace(/\s/g, '')).click();
                        }
                        
                        switch (value.strEntity) {
                            case 'AOR':
                                arrAORs.push(value);
                                break;
                            case 'CR':
                                arrCRs.push(value);
                                break;
                            case 'Primary Task':
                                arrTasks.push(value);
                                break;
                            case 'Subtask':
                                arrSubTasks.push(value);
                                break;
                        }

                        //if(strEntity == value.strEntity) {
                        //    existingValueFilter.push(value.strFieldID);
                        //}
                    });
                    setSelectedEntity(strEntity);
                }
            }
        }
        function initEvents() {
                $('.nav').hover(function () {
                    $(this).toggleClass('navhover');
                });
                $('.nav').click(function () { btnNext_onclick(this); });
                $("#btnBack").click(function () { btnBack_click(); return false; });
                $("#btnFinish").click(function () { btnFinish_OnClick(); return false; });
                $("#btnSave").click(function () { btnSave_onclick(); return false; });
                $("#btnClose").click(function () { btnClose_click(); return false; });
        }

        $(document).ready(function () {
            initEvents();
            initVariables();
            initControls();
            initDisplay();
        });
    </script>
</body>
</html>