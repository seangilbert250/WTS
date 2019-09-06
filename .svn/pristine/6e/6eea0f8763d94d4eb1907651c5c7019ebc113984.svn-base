<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report_WorkLoad.aspx.cs" Inherits="Report_WorkLoad" theme="Default"%>

<!DOCTYPE html>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.2/jquery.min.js"></script>
<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
<script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
<script src="Scripts/shell.js"></script>

    <style>
        *{
            font-family: Arial;
            font-size: 12px;
        }
        table{
            border-collapse: collapse; 
            width: 100%;
        }
        .breakoutButton{
            padding-left: 10px;
            padding-bottom: 20px;
            padding-top: 10px;
        }
        .header {
            background-size: contain;
            text-align: center;
            font-size: 14px;
        }
        .header th{
            border: 1px solid black;
            padding-bottom: 10px;
            padding-top: 10px;
            background: url("Images/Headers/gridheaderblue.png");
            text-align: center;
        }
        .subHeader {
            background: url("Images/Headers/gridheaderblue.png");
            text-align: center;
            font-weight: bold;
            border: 1px solid #bbbbbb;
        }
        .sectionButton {
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            border-radius: 4px;
            background-color: #e6e6e6;
            font-size: smaller;
            height: 20px;
        }
        .workLoadOverviewButton{
            width: 100px;
        }
        .columnBody{
            border: 1px solid grey;
            vertical-align: top;
        }
        .listBox {
            cursor:default;
            font-size:12px;
            border:solid 1px grey;
            overflow-x:hidden;
            overflow-y:auto;
            width: 180px;
            height: 100%;
            padding-top: 3px;
            padding-bottom: 3px;
            padding-left: 3px;
        }
        .icon{
             cursor: pointer; 
             position: relative; 
             top: 3px; 
        }
        .workLoadOverviewListColumn{
            vertical-align: top;
            text-align: center;
        }
         #generateReport {
            float: right;
            -moz-border-radius: 4px;
            -webkit-border-radius: 4px;
            border-radius: 4px;
        }
        #buttonRow{
            padding-bottom: 10px;
            padding-left: 20px;
            padding-right: 20px;
        }
        #headerOne, #headerThree{
            width: 40%;
        }
        #headerTwo{
            width: 20%;
        }
        #sectionButtonContainer{
             float: right; 
             padding-top: 10px; 
             padding-right: 10px;
             padding-bottom: 25px;
        }
        #resourceCheckBoxContainer{
            padding : 5px;
        }
        #workLoadOverviewContainer{
            text-align: center; 
            padding-top: 10px; 
            padding-left: 10px;
        }
        #workLoadOverviewTable{
            width: 100%;
        }
        #workLoadOverviewButtonColumn{
            vertical-align: middle;
        }
        #resourceCheckBoxContainer{
            padding: 5px;
        }
        #reportHeader{
            background-image : url("Images/Headers/grey.gif");
            background-size: 100% 100%;
            padding-bottom: 10px;
            padding-left: 20px;
            padding-right: 20px;
        }
    </style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WORKLOAD SUMMARY REPORT PARAMETERS</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="reportHeader">
            <h2 class="header">WORKLOAD SUMMARY REPORT PARAMETERS</h2>
            <div id="buttonRow">        
                <asp:DropDownList ID="ddlParameters" runat="server" Font-Size="9" Width="120px">
                </asp:DropDownList>
                <img id="imgSaveParameters" src="Images/icons/disk.png" title="Save Custom Parameters Set" alt="Save Parameters" class="icon" />
                <img id="imgDeleteParameters" src="Images/icons/delete.png" title="Delete Custom Parameters Set" alt="Delete Parameters" class="icon" />
                	<div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
	                <div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
		                <table style="width: 100%;">
			                <tr>
				                <td class="pageContentInfo">
					                Grid View Name:
				                </td>
			                </tr>
			                <tr>
				                <td>
					                <input type="text" id="txtViewName" />
				                </td>
			                </tr>
			                <tr>
				                <td>
					                <input type="checkbox" id="chkProcessView" style="vertical-align: middle;" />
					                <label for="chkProcessView" style="vertical-align: middle;">Process View</label>
				                </td>
			                </tr>
			                <tr>
				                <td>
					                <input type="button" id="buttonSaveView" value="Save" />&nbsp;<input type="button" id="buttonCancelView" value="Cancel" />
				                </td>
			                </tr>
		                </table>
	                </div>
                <input type="checkbox" id="excelExport" value="ExportToExcel"/>Export to Excel<br />
                <button type="button" id="generateReport">Generate Report</button>
             </div>
        </div>
        <div id="parameterTableContainer">
            <table id="paramsBody">
                <tr class="header">
                    <th id="headerOne">Summary Overview</th><th id="headerTwo">Resource Overview</th><th id="headerThree">Workload Summary</th>
                </tr>
                <tr>
                    <td class="columnBody">
                        <table id="Summary Body">
                            <tr>
                                <td colspan="2">
                                    <div id="sectionButtonContainer"> 
                                        <button class="sectionButton" type="button" id="addSection">Add Section</button>
                                        <button type="button" class="sectionButton" id="removeSection">Remove Section</button>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="sectionOne">
                                        <tr>
                                            <td class="subHeader" colspan="2">
                                                Section 1
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table id="sectionOneArea">
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img id="newBreakoutOne" alt="Add Level" src="Images/Icons/add_blue.png" class="breakoutButton"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="sectionTwo">
                                        <tr>
                                            <td class="subHeader" colspan="2">
                                                Section 2
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table id="sectionTwoArea">
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <img id="newBreakoutTwo" alt="Add Level" src="Images/Icons/add_blue.png" class="breakoutButton"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="columnBody">
                        <div id="resourceCheckBoxContainer">
                            <input type="checkbox" id="resourceBus" value="Bus"/>Bus<br />
                            <input type="checkbox" id="resourceDev" value="Dev"/>Dev<br />
                        </div>
                    </td>
                    <td class="columnBody">
                        <div class="workLoadOverviewContainer">
			                <table>
				                <tr class="workLoadOverviewListColumn">
					                <td>Available Columns</td>
					                <td>&nbsp;</td>
					                <td>Selected Columns</td>
				                </tr>
				                <tr class="workLoadOverviewListColumn">
					                <td>
                                        <select multiple="multiple" size="20" id="columnAvailable" class="listBox"></select>
					                </td>
					                <td class="workLoadOverviewButtonColumn">
									        <button id="btnAdd" class="workLoadOverviewButton">>></button><br />
									        <button id="btnRemove" class="workLoadOverviewButton"><<</button><br />
									        <button id="btnClearAll" class="workLoadOverviewButton">Clear All</button><br />
					                </td>
					                <td>
                                         <select multiple="multiple" size="20" id="columnSelect" class="listBox"></select>
					                </td>
				                </tr>
			                </table>
		                 </div>
                    </td>
            </table>
        </div>
    </div>
 <asp:ScriptManager ID="totallyunique" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script type="text/javascript">
        $(document).ready(function () {
            var maxBreakouts = 3;
            var maxColumns = 6;

            var breakoutOptions = [     //section groupings drop down list initial values. Items are removed from the array in the event they are selected in a drop down list, and added back to the array when deselected 
            , { text: "", value: "" }   //these values are used in group by clause for major order counts (section 1) and child order counts (section 1 + section 2). 
            //, { text: "Work Request", value: "[Work Request]" } //
            , { text: "Priority", value: "[Priority]" } //
            , { text: "Work Area", value: "[Work Area]" } //
            //, { text: "Allocation Group", value: "[Allocation Group]" }//
            //, { text: "Allocation Assign", value: "[Allocation Assignment]" } //
            , { text: "System", value: "[System]" } //
            , { text: "Workload Group", value: "[Workload Group]" } //
            , { text: "Item", value: "[Item Type]" } //
            , { text: 'Work Type', value: "[Work Type]" } //
            , { text: "Contract", value: "[Contract]" } //
            //, { text: "PDD TDR Phase", value: "[PDD TDR Phase]" } //
            , { text: "Scope", value: "[Scope]" } //
            , { text: "Release Version", value: "[Release Version]" } //
            ].sort(function (a, b) {
                return a.text.toString().localeCompare(b.text.toString());
            });

            function getAvailableColumns() {
                var availableColumns = [                //values for available columns multi selection. These are the columns and column names displayed in the report task detail.
               , { text: "Assigned To", value: "Task Assigned Resource,Sub-Task Assigned Resource" } //
               , { text: "Estimated Start Date", value: "Sub-Task Estimated Start Date" } //
               , { text: "Actual Start Date", value: "Sub-Task Actual Start Date" } //
               , { text: "Actual End Date", value: "Sub-Task Actual End Date" } //
               , { text: "Status", value: "Task Status,Sub-Task Status" } //
               , { text: "Title", value: "Task Title,Sub-Task Title" } //
               , { text: "Created By", value: "Task Created By,Sub-Task Created By" } //
               , { text: "Created Date", value: "Task Created Date,Sub-Task Created Date" } //
               , { text: "Updated By", value: "Task Updated By,Sub-Task Updated By" } //
               , { text: "Updated Date", value: "Task Updated Date,Sub-Task Updated Date" } //
               , { text: "Submitted by", value: "Task Submitted By,Sub-Task Submitted By" } //
               , { text: "Primary Tech Resource", value: "Task Primary Tech Resource,Sub-Task Primary Tech Resource" } //
               , { text: "Secondary Tech Resource", value: "Task Secondary Tech Resource" } //
               , { text: "Primary Business Resource", value: "Task Primary Business Resource" } //
               , { text: "Task Number", value: "Task #,Sub-Task #" } //
               , { text: "Primary Tech Rank", value: "Task Primary Tech Rank,Sub-Task Primary Tech Rank" } //
               , { text: "Primary Bus Rank", value: "Task Primary Bus Rank, Sub-Task Primary Bus Rank" } //
               , { text: "Priority", value: "Task Priority,Sub-Task Priority" } //
               , { text: "Percent Complete", value: "Task Percent Complete,Sub-Task Percent Complete" } //
               , { text: "Work Area", value: "Task Work Area" } //
               , { text: "Functionality", value: "Task Functionality" } //
               , { text: "Date Needeed", value: "Task Date Needed" } //
               //, {text: "PDDTDR Phase", value: "Task Phase"}
                ].sort(function (a, b) {
                    return a.text.toString().localeCompare(b.text.toString());
                });
                return availableColumns;
            };
            $("#ddlParameters option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
            $("#ddlParameters option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
            setAvailableColumnsList(); //itialize available columns multi select.
            $('#sectionOne').hide(); //the tables are hidden until 'add section' button is pressed, and rehidden when 'remove section' button is pressed.
            $('#sectionTwo').hide();
            bindEvents();
            setPagetoDefault();

            function setPagetoDefault() {
                resetParameters();
                $('#addSection').trigger('click');
                $('#addSection').trigger('click');
                newBreakout($('#sectionOneArea'));
                $($('#sectionOneArea').find('select')[0]).val('[Allocation Group]').trigger('blur');
                $($('#sectionOneArea').find('select')[1]).val('ASC');
                newBreakout($('#sectionTwoArea'));
                $($('#sectionTwoArea').find('select')[0]).val('[Allocation Assignment]').trigger('blur');
                $($('#sectionTwoArea').find('select')[1]).val('ASC');
                $('#resourceBus').prop('checked', true);
                $('#resourceDev').prop('checked', true);
                $('#columnAvailable').val(['Task #,Sub-Task #']);
                $('#btnAdd').trigger('click');
                $('#columnAvailable').val(['Task Assigned Resource,Sub-Task Assigned Resource']);
                $('#btnAdd').trigger('click');
                $('#columnAvailable').val(['Task Status,Sub-Task Status']);
                $('#btnAdd').trigger('click');
                $('#columnAvailable').val(['Task Title,Sub-Task Title']);
                $('#btnAdd').trigger('click');
            }

            function resetParameters() {
                $('#btnClearAll').trigger('click');
                $('#removeSection').trigger('click');
                $('#removeSection').trigger('click');
                $('#resourceBus').prop('checked', false);
                $('#resourceDev').prop('checked', false);
                breakoutOptions.sort(function (a, b) {
                    return a.text.toString().localeCompare(b.text.toString());
                });
                setAvailableColumnsList();
            }

            function setAvailableColumnsList() {
                var availableColumns = getAvailableColumns();
                $('#columnAvailable').find('option').remove(); //reset available columns select list
                availableColumns.forEach(function (option) {   //bind available columns to the list
                    $('#columnAvailable').append('<option value=\"' + option.value + "\">" + option.text + "</option>");
                });
            }

            function setSectionList(select) { //this sets the options in the breakout drop down lists. This function is called when a new breakout is added, on a blur event for breakout, or when a breakout is removed via cancel button.
                var selectedValue = typeof $('option:selected', select).val() == null ? '' : $('option:selected', select).val();
                var selectedText = typeof $('option:selected', select).html() == 'undefined' ? '' : $('option:selected', select).html();
                var availableOptions = breakoutOptions;
                availableOptions.sort(function (a, b) { //resort the list, because it looks nice.
                    return a.text.toString().localeCompare(b.text.toString());
                });
                if (selectedText !== '') { //flush all options and rebound the selected value as the first option (first option = selected)
                    $(select)
                        .find('option')
                        .remove()
                        .end()
                        .append('<option value=\'' + selectedValue + '\'>' + selectedText + '</option>')
                    ;
                }
                else { //in the event the select option is "", then either there was no selected option (new select) or the option was set to blank. In either case the selected option should not be preserved, as by default the first item in the select is blank.
                    $(select).find('option').remove();
                }
                availableOptions.forEach(function (option) { //append all available options (options not currently selected) to the select drop down list. 
                    $(select).append('<option value=\"' + option.value + "\">" + option.text + "</option>");
                });
            };

            function bindEvents() {

                $('#ddlParameters').change(function (){
                    var value = $('#ddlParameters option:selected').val();
                    var parameters;
                    if (value === "Default") {
                        setPagetoDefault();
                    }
                    else if (value === "Default (Backlog)") {
                        setPagetoDefault();
                    }
                    else{
                        getParameters(value);
                    }
                });

                $("#generateReport").click(function () {
                    var SummaryOverviewsSection1 = getSummaryOverviews($('#sectionOneArea')).join();
                    var SummaryOverviewsSection2 = getSummaryOverviews($('#sectionTwoArea')).join();
                    var Organziation = getOrganization();
                    var selectedColumns = getSelectedColumns().join();
                    var filters = JSON.stringify(getFilters());
                    var Delimeter = ',';

                    var Excel = ''; 
                    if ($('#excelExport').is(':checked'))
                        Excel = 'Yes';
                    else
                        Excel = 'No';

                    // 13419 - 7
                    var ddlValue = $('#ddlParameters option:selected').val();

                    PageMethods.saveReportData(SummaryOverviewsSection1, SummaryOverviewsSection2, Organziation, selectedColumns, filters, Delimeter, ddlValue, Excel); //get the data from the database and put it in the session
                    __doPostBack('btnGenerateReport', true); //trigger a postback which will generate the report. 
                });

                $('#imgSaveParameters').click(function () {
                        
                $('#divDimmer').show();
			    var pos = $(this).position();
			    var width = $('#divViewName').outerWidth();
			    $('#divViewName').css({
			        position: "absolute",
			        top: pos.top + "px",
			        left: (pos.left) + "px"
			    }).slideDown(function () { $('#txtViewName').focus(); });
                });

                $('#txtViewName').on("keypress", function (e) { 
                    if (e.which == 13) { 
                        $('#buttonSaveView').trigger('click'); 
                        return false; 
                    } 
                });

                $('#buttonSaveView').click(function () { 
                    var name = $('#txtViewName').val();
                    var process = $('#chkProcessView').is(':checked');
                    if (name === null || name === '' || name === 'Default') {
                        return false;
                    }
                    else if (name.length > 255) {
                        MessageBox("The parameters preset name must be less than 255 characters!", "Error!");
                        return false;
                    }
                    addNewParametersPreset(name, process);
                    $('#buttonCancelView').trigger("click");
                    return false;
                });

                $('#buttonCancelView').click(function () { 
                    $('#divViewName').slideUp(function () { $('#divDimmer').hide(); 
                    });
                    $('#chkProcessView').prop('checked', false);
                    $('#txtViewName').val('');
                    return false; 
                });

                $('#imgDeleteParameters').click(function () {
                    var value = $('#ddlParameters option:selected').val();
                    if (value === "Default") {
                        return false;
                        }
                    deleteParametersPreset(value);
                    return false;
                });


                $('#removeSection').click(function () {
                    if ($('#sectionTwoArea').is(':visible')) { //if the section is visible then remove each breaktout option and hide the section area. If the section is not visible the section has not been added via the "add section" button. 
                        $('#sectionTwoArea').find('.breakoutCancel').each(function () {
                            breakoutCancelClickEvent(this);
                        });
                        $('#sectionTwo').hide();
                    }
                    else if ($('#sectionOneArea').is(':visible')) {
                        $('#sectionOneArea').find('.breakoutCancel').each(function () {
                            breakoutCancelClickEvent(this);
                        });
                        $('#sectionOne').hide();
                    }
                    breakoutOptions.sort(function (a, b) { //resort the list, because it looks nice.
                        return a.text.toString().localeCompare(b.text.toString());
                    });
                });

                $('#addSection').click(function () { //set the section visible, which allows uses to add or remove breakouts. 
                    if (!$('#sectionOneArea').is(':visible')) {
                        $('#sectionOne').show();
                    }
                    else if (!$('#sectionTwoArea').is(':visible')) {
                        $('#sectionTwo').show();
                    }
                });

                $('#btnAdd').click(function () {        //remove options from available in columns list and append them to selected
                    var numberColumnsSelected = $('#columnSelect option').length; //maximum number of columns limited for report readability.
                    var inserted = 0;
                    $('#columnAvailable').find('option:selected').each(function () {
                        if (numberColumnsSelected + inserted >= maxColumns) {
                            return false;
                        }
                        $('#columnSelect').append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                        $(this).remove();
                        inserted++;
                    });
                    sortAvailableColumns();
                    return false;
                });

                $('#btnRemove').click(function () {        //remove options from available and append them to selected
                    $('#columnSelect').find('option:selected').each(function () {
                        $('#columnAvailable').append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                        $(this).remove();
                    });
                    sortAvailableColumns();
                    return false;
                });

                $('#btnClearAll').click(function () {        //remove all options from selected and append them to available
                    $('#columnSelect').find('option').each(function () {
                        $('#columnAvailable').append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                        $(this).remove();
                    });
                    sortAvailableColumns();
                    return false;
                });

                $('#newBreakoutOne').click(function () {
                    newBreakout($('#sectionOneArea'));
                });

                $('#newBreakoutTwo').click(function () {
                    newBreakout($('#sectionTwoArea'));
                });
            }

            function addNewParametersPreset(name, process){
                params = gatherReportParameters();
                var paramsID = 0; 
                PageMethods.addReportParameters(params, name, <%=this.reportID%>,<%=this.userID%>,process, 
                    function(result){
                        if(process === true){
                            $('#ddlParameters').append('<option OptionGroup=\'Process Views\' value=\'' + result.Key.toString() + '\'>' + result.Value.toString() + '</option>');
                        }
                        else{
                            $('#ddlParameters').append('<option OptionGroup=\'Custom Views\' value=\'' + result.Key.toString() + '\'>' + result.Value.toString() + '</option>');
                        }
                        sortddlList();
                        $('#ddlParameters').val(result.Key.toString());
                    }
                    ,onError);
            }

            function updateParametersPreset(key){
                var JSON = gatherReportParameters();
                PageMethods.updateReportParameters(JSON, key, function(){ MessageBox("Report parameters saved", "Success");}, onError);
            }

            function deleteParametersPreset(key) {
                PageMethods.deleteReportParameters(key, 
                   function(){
                       $("#ddlParameters option[value=\"" + key + "\"]").remove();
                       MessageBox("Report parameters deleted", "Success");
                   }, onError);
            }

            function getParameters(key) {
                var ParamsObject;
                PageMethods.getReportParameters(key, 
                    function(result){
                        if (result){
                            var parameters = JSON.parse(result);
                            setParameters(parameters);
                        }else{
                            MessageBox("A malformed object was returned. These parameters will be deleted. Please try recreating the parameters and saving them again.", "Error!");
                            $('#imgDeleteParameters').trigger('click');
                        }
                    }, onError);
            }

            function setParameters(paramObject){
                var somekindofError = false; //if an exception is thrown in any step this will be set to true. 
                var temp = false; //holds the return value of each function. 
                resetParameters();
                if (typeof paramObject.SummaryOverviewsSection1 != 'undefined'){
                    var temp = setSummaryOverviews(paramObject.SummaryOverviewsSection1, $('#sectionOneArea'));
                    if (temp === true) 
                        somekindofError = true;
                    temp = false;
                }
                if (typeof paramObject.SummaryOverviewsSection2 != 'undefined'){
                    temp = setSummaryOverviews(paramObject.SummaryOverviewsSection2, $('#sectionTwoArea'));
                    if (temp === true) 
                        somekindofError = true;
                    temp = false;
                }
                if (typeof paramObject.resourceBus != 'undefined' && paramObject.resourceDev != 'undefined'){
                    temp = setResourceOverviews(paramObject.resourceBus, paramObject.resourceDev);
                    if (temp === true) 
                        somekindofError = true;
                    temp = false;
                }
                if (typeof paramObject.selectedColumns != 'undefined'){
                    temp = setWorkloadSummary(paramObject.selectedColumns);
                    if (temp === true) 
                        somekindofError = true;
                    temp = false;
                }

                if (somekindofError){
                    MessageBox("An error occured while applying one or more of your settings. This can occur do to a saving error, or do to changes in the structure of this page. It is recommended that you overwrite or delete these settings, so that you do not get this error again.", "Error!");
                }
                return;
            }

            function setSummaryOverviews(overviews, sectionArea){
                var error = false;
                if (!overviews.length || overviews.length  === 0) return error;

                try{
                    $('#addSection').trigger('click');
                }
                catch (e){
                    error = true;
                }
                for(var i = 0, j = 0; i < overviews.length; i++){
                    try{
                        newBreakout(sectionArea);
                        var breakout = overviews[i].toString();
                        var sortOrder = breakout.substr(breakout.length-4) === 'DESC' ? 'DESC' : 'ASC';
                        var value = breakout.replace('ASC', '').replace('DESC', '').trim();
                        $($(sectionArea).find('select')[j]).val(value).trigger('blur'); //jquery black magic. Overviews are always added in pairs. This loop iterates an i variable, which is the index into the overviews array, and the j varaible, which increments once for each select list added. The value of these selects are changed and then triggered a blur event which takes care of other detials. 
                        j++;
                        $($(sectionArea).find('select')[j]).val(sortOrder);
                        j++;
                    }
                    catch (e){
                        error = true;
                    }
                }
                return error;
            }

            function setResourceOverviews(Bus, Dev){
                var error = false;
                try{
                    $('#resourceBus').prop('checked', Bus);
                    $('#resourceDev').prop('checked', Dev);
                }
                catch(e){
                    error = true;
                }
                return error;
            }

            function setWorkloadSummary(selectedColumns){
                var error = false;
                for(var i = 0; i < selectedColumns.length; i++){
                    try{
                        $('#columnAvailable').val(selectedColumns[i]);
                        $('#btnAdd').trigger('click');
                    }
                    catch(e){
                        error = true;
                    }
                }
                return error;
            }

            function onError(e){
                MessageBox(e.get_message(), "Error!");
            }

            function gatherReportParameters(){
                reportParameters = {}; 
                reportParameters.SummaryOverviewsSection1 = getSummaryOverviews($('#sectionOneArea'));
                reportParameters.SummaryOverviewsSection2 = getSummaryOverviews($('#sectionTwoArea'));
                reportParameters.resourceBus = $('#resourceBus').is(':checked');
                reportParameters.resourceDev = $('#resourceDev').is(':checked');
                reportParameters.selectedColumns = getSelectedColumns();
                return JSON.stringify(reportParameters);
            }
            function newBreakout(sectionArea) {
                var breakout = '<tr><td style="text-align: center"> <div style="padding-top: 10px"> <select class="breaktoutSelection" style="width: 180px, font-family: Arial, font-size=11px;"></select><select style="width: 60px, font-family: Arial, font-size=11px;"><option value=\'ASC\'>ASC</option><option value=\'DESC\'>DESC</option></select></div></td><td><img src="Images/Icons/cross.png" class="breakoutCancel" style="float: right"/></td></tr>';
                var html = $.parseHTML(breakout); //parsed to html to be appended to the breakout section area. 
                var numberBreakouts = $('tr', sectionArea).length //counts the number of rows in the section area table. 

                if (numberBreakouts >= maxBreakouts) { //maximum breakouts is limited to a global value. 
                    return false;
                }
                $(sectionArea).append(html);
                $('.breakoutCancel').bind('click', function () {
                    breakoutCancelClickEvent(this);
                });
                $('.breaktoutSelection').each(function () { //set selection list and bind blur event. Uses blur instead of change because setSectionList triggers a 'change' event for each drop down list (which causes infinite loop). 
                    $(this).bind('blur', function () {
                        breakoutSelectionBlurEvent(this);
                    });
                    setSectionList(this);
                });
            };

            function breakoutCancelClickEvent(cancel) {
                $(cancel).focus(); //forces focus to click event. This prevents duplicate values from existing in breakoutOptions array as the blur event might not have been triggered. 
                var option = {};
                option.text = $(cancel).closest('tr').find('select option:selected')[0].text; //get the value of the first select. 
                option.value = $(cancel).closest('tr').find('select option:selected')[0].value;
                if (option.text !== '') { //if selected option is the emptry string it is the default "non" value. By not removing it this allows for every select box to have the empty string/blank option. 
                    removeOption(breakoutOptions, 'value', option.value); //makes sure the option  is removed before adding it back in. In some scenarios the option can still be in the array even when it should have been removed by a blur event. 
                    breakoutOptions.push(option);
                    $(cancel).closest('tr').remove(); //need to remove the select before reseting the options in all select list, otherwise this removed select gets set as well. 
                    $('.breaktoutSelection').each(function () {
                        setSectionList(this);
                    });
                }
                $(cancel).closest('tr').remove(); //in the 'else' case still need to remove the list, but don't need to do the other stuff (because it is the ''/empty option)> 
            };

            function sortAvailableColumns() {
               var sortedOptions = $('#columnAvailable option').sort(function (a, b) {
                    return $(a).html().toString().localeCompare($(b).html().toString());
               })
               $('#columnAvailable').find('option').remove(); //reset available columns select list
               sortedOptions.each(function () {
                   $('#columnAvailable').append(this);
               });
            }

            function sortddlList() {
                $("#ddlParameters option[value=\"" + "Default" + "\"]").remove();
                var sortedOptions = $('#ddlParameters option').sort(function (a, b) {
                    return $(a).html().toString().localeCompare($(b).html().toString());
                })
                $('#ddlParameters').find('option').remove(); //reset available columns select list
                $('#ddlParameters').find('optgroup').remove();
                sortedOptions.each(function () {
                    $('#ddlParameters').append(this);
                });
                $('#ddlParameters').prepend('<option OptionGroup=\'Process Views\' value=\'Default\'>Default</option>');

                
                
                $('#ddlParameters').append('<option OptionGroup=\'Process Views\' value=\'Default (Backlog)\'>Default (Backlog)</option>');



                $("#ddlParameters option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
                $("#ddlParameters option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
            }

            function breakoutSelectionBlurEvent(selection) { //dynamically bound to .breakoutSection blur event
                var selectedValue = typeof $('option:selected', selection).val() == null ? '' : $('option:selected', selection).val(); //get currently selected value
                var selectedText = typeof $('option:selected', selection).html() == 'undefined' ? '' : $('option:selected', selection).html(); 
                var previousValue = typeof $(selection).attr('prevValue') == 'undefined' ? '' : $(selection).attr('prevValue'); //select box holds its previous text and values as two attributes. This allows you to compare the selected value to the previously selected value.
                var previousText = typeof $(selection).attr('prevText') == 'undefined' ? '' : $(selection).attr('prevText');
                var selectionChanged = previousValue !== selectedValue;
                $(selection).attr('prevValue', selectedValue); 
                $(selection).attr('prevText', selectedText);

                if (selectedValue !== '' && selectionChanged) { //if not blank and the selection is different from its previous value, remove the option from the array so that only this select box can select it. This prevents duplicate select options. 
                    removeOption(breakoutOptions, 'value', selectedValue);
                }

                if (previousValue !== '' && selectionChanged) { //if the select option was changed, and the previous value was not blank, then add the previous value back to the array so that it is available to the other select boxes. 
                    var addOption = {};
                    addOption.text = previousText;
                    addOption.value = previousValue;
                    breakoutOptions.push(addOption);
                }

                breakoutOptions.sort(function (a, b) { //resort the list, because it looks nice.
                    return a.text.toString().localeCompare(b.text.toString());
                });

                $('.breaktoutSelection').each(function () { //update the value of the select boxes to reflect the changes. 
                    setSectionList(this);
                });
            };

            function removeOption(arr, propName, propValue) { //simple utility function for finding a value in the JSON array and removing it. 
                arr.forEach(function (option, index) {
                    if (option[propName] == propValue) {
                        arr.splice(index, 1);
                        return;
                    }
                });
            };

            function getSummaryOverviews(sectionArea) { //get the values of breakout selection and return them as a comma deliminated list. 
                var SummaryOverviews = [];
                $('.breaktoutSelection', sectionArea).each(function () { //update the value of the select boxes to reflect the changes. 
                    var selectedValue = typeof $('option:selected', this).val() == null ? '' : $('option:selected', this).val();
                    var order = $(this).next();
                    var orderDirection = $('option:selected', order).val(); //all orderby boxes should have ASC by default, so value can't be null or undefined. 
                    if (selectedValue !== '' && selectedValue) { //if the selected value is blank or undefined then we don't append anything
                        SummaryOverviews.push(selectedValue + ' ' + orderDirection);
                    }
                });
                return SummaryOverviews;
            };

            function getOrganization() {
                var business = $('#resourceBus').is(':checked');
                var dev = $('#resourceDev').is(':checked');
                if (business && dev) {
                    return 'ALL';
                }
                else if (business) {
                    return 'Business Team';
                }
                else if (dev) {
                    return 'Folsom Dev';
                }
                else
                    return '';
            }
        });

        function getSelectedColumns() {
            var SelectedColumns = [];
            $('#columnSelect').find('option').each(function () {
                SelectedColumns.push($(this).val());
            });
            return SelectedColumns;
        };

        function getFilters() {
            var filterBox = top.filterBox.filters;
            var filters = [];
            for (var i = 0; i < filterBox.length; i++) {
                var filter = new Object();
                if (filterBox[i].groups.ParentModule == "Reports") {
                    filter.text = filterBox[i].name;
                    filter.value = unpackFilterParameters(filterBox[i].parameters);
                    filters.push(filter);
                }
            }
            return filters;
        };

        function unpackFilterParameters(parameters) {
            var parametersList = '';
            for (var i = 0; i < parameters.length; i++) {
                parametersList += parameters[i].text + ',';
            }
            return parametersList.substring(0, parametersList.length - 1); //this method always appends one extra comma. This removes it. 
        };
    </script>
    </form>
</body>
</html>
