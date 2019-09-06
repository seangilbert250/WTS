﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.master" AutoEventWireup="true" CodeFile="CrosswalkParametersSections.aspx.cs" Inherits="CrosswalkParamContainer" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Crosswalk Parameters</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
    <style type="text/css">
        .ui-sortable-helper, .ui-draggable-dragging {
            cursor: url("Images/Cursor/closedhand.cur"), move;
            background-color: Window;
            display: table;
            width: 100px;
        }
    </style>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div id="divPage" class="pageContainer" style="overflow-y: auto">
        <div id="divTabsContainer" class="mainPageContainer">
            <div id="divBasic">
                <table id="tableBasicOptions" class="attributes" style="width: 99%; text-align: left; vertical-align: top; padding: 10px;">
                    <tr class="attributesRow">
                        <td class="attributesValue" style="border: 1px solid black; padding: 10px;">
                            <label for="ddlView" title="Grid View:" style="vertical-align: middle;">Grid View:</label>
                            <asp:DropDownList ID="ddlView" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
                            <img id="imgSaveView" src="Images/Icons/disk.png" title="Save View" alt="Save View" style="cursor: pointer;" />
                            <img id="imgDeleteView" src="Images/Icons/delete.png" title="Delete View" alt="Delete View" style="cursor: pointer;" />
                            <asp:HiddenField ID="txtXML" runat="server" />
                            <asp:HiddenField ID="txtDropDown" runat="server" />
                        </td>
                    </tr>
                    <tr style="display: none;">
                        <td style="padding: 5px;">Rollup/Sort Options:</td>
                    </tr>
                    <tr style="display: none;">
                        <td style="border: 1px solid black; padding: 5px;">
                            <table id="tableAttributes" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px 5px 0px;">
                                <tr class="attributesRow">
                                    <td class="attributesLabel" style="width: 60px;">Rollup By:</td>
                                    <td class="attributesValue" style="width: 100px;">
                                        <asp:DropDownList runat="server" ID="ddlRollupGroup" Enabled="false">
                                            <%--											<asp:ListItem Text="Priority" Value="Priority" />--%>
                                            <asp:ListItem Text="Status" Value="Status" />
                                        </asp:DropDownList>
                                    </td>
                                    <td class="attributesLabel" style="width: 105px;">Default Task Sort:</td>
                                    <td class="attributesValue">
                                        <asp:DropDownList runat="server" ID="ddlDefaultTaskSort">
                                            <asp:ListItem Text="Primary Tech. Rank" Value="Tech" />
                                            <asp:ListItem Text="Primary Bus. Rank" Value="Bus" />
                                            <asp:ListItem Text="Secondary Tech. Rank" Value="Secondary" />
                                            <asp:ListItem Text="Secondary Bus. Rank" Value="Secondary" />
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <table id="addSections">
                    <!--------------------------------------------------- Levels------------------------------------------->
                    <tr>
                        <td style="padding-left: 20px;">
                            <input type="button" id="btnAddSection" value="Add Level" style="padding-left: 1px;" />
                        </td>
                    </tr>
                </table>
                <div id="divAdvancedScroll" style="height: 580px;">
                    <table id="Sections">
                        <tbody id="tblBody">
                            <tr>
                                <td id="tdSections" style="vertical-align: top; height: 100px; padding-left: 20px;"></td>
                            </tr>
                            <!--------------------------------------------------- Levels------------------------------------------->
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="divFooter" class="pageContentHeader" style="width: 100%; position: absolute; bottom: 0px; left: 0px;">
        <table id="tableFooter" style="width: 100%; vertical-align: bottom;">
            <tr>
                <td style="text-align: right; height: 30px; padding: 2px 5px 2px 5px;">
                    <input type="button" id="buttonGetData" value="Get Data" />
                </td>
            </tr>
        </table>
    </div>
    <div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
    <div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
        <table style="width: 100%;">
            <tr>
                <td class="pageContentInfo">Grid View Name:
                </td>
            </tr>
            <tr>
                <td>
                    <select id="ddlSaveView" style="width: 255px;"></select>
                </td>
            </tr>
            <tr id="trViewName">
                <td>
                    <asp:TextBox ID="txtViewName" runat="server" MaxLength="50" Width="250"></asp:TextBox>
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

    <div id="divHidden" style="display: none;">
        <select id="ddlParameterTypes">
            <option value="-- SELECT LEVEL --">-- Select Level --</option>
            <option value="WORKLOAD PRIORITY">Workload Priority</option>
            <option value="RESOURCE COUNT (T.BA.PA.CT)">Resource Count (T.BA.PA.CT)</option>
            <option value="TASK.WORKLOAD.RELEASE STATUS">Task.Workload.Release Status</option>
            <option value="AFFILIATED">Affiliated</option>
            <%--<option value="AOR">AOR</option>--%>
            <option value="AOR RELEASE/DEPLOYMENT MGMT">AOR Release/Deployment MGMT</option>
            <option value="AOR WORKLOAD MGMT">AOR Workload MGMT</option>
            <%--<option value="CONTRACT ALLOCATION ASSIGNMENT">Contract Allocation Assignment</option>
            <option value="CONTRACT ALLOCATION GROUP">Contract Allocation Group</option>--%>
            <option value="ASSIGNED TO">Assigned To</option>
            <option value="FUNCTIONALITY">Functionality</option>
            <option value="WORK ACTIVITY">Work Activity</option>
            <option value="ORGANIZATION (ASSIGNED TO)">Organization (Assigned To)</option>
            <%--<option value="PDD TDR">PDD TDR</option>--%>
            <option value="PERCENT COMPLETE">Percent Complete</option>
            <%--<option value="BUS. RANK">Bus. Rank</option>--%>
            <%--<option value="PRIMARY BUS. RESOURCE">Primary Bus. Resource</option>--%>
            <%--<option value="TECH. RANK">Tech. Rank</option>--%>
            <option value="ASSIGNED TO RANK">Assigned To Rank</option>
            <option value="CUSTOMER RANK">Customer Rank</option>
            <option value="PRIMARY RESOURCE">Primary Resource</option>
            <option value="PRIORITY">Priority</option>
            <option value="PRODUCT VERSION">Product Version</option>
            <option value="Deployment">Deployment</option>
            <option value="Session">Session</option>
            <option value="PRODUCTION STATUS">Production Status</option>
            <%--<option value="SECONDARY BUS. RESOURCE">Secondary Bus. Resource</option>
            <option value="SECONDARY TECH. RESOURCE">Secondary Tech. Resource</option>--%>
            <option value="SR NUMBER">SR Number</option>
            <option value="STATUS">Status</option>
            <option value="SYSTEM(TASK)">System(Task)</option>
            <option value="SYSTEM SUITE">System Suite</option>
            <option value="WORK AREA">Work Area</option>
            <option value="PRIMARY TASK">Primary Task</option>     
            <option value="WORK TASK">Work Task</option>
			<%--<option value="PRIMARY TASK TITLE">Primary Task Title</option>
            <option value="WORK TASK TITLE">Work Task Title</option>--%>
            <%--<option value="WORK REQUEST">Work Request</option>--%>
            <option value="RESOURCE GROUP">Resource Group</option>
            <option value="WORKLOAD ALLOCATION">Workload Allocation</option>

            <%--            <option value="WORK ITEM">Work Item</option>
            <option value="WORKLOAD GROUP">Workload Group</option>--%>
            <option value="CONTRACT">Contract</option>
            <%-- <option value="SCOPE">Scope</option>
            <option value="VERSION">Version</option>
            <option value="PRIMARY DEVELOPER">Primary Developer</option>
            <option value="SECONDARY DEVELOPER">Secondary Developer</option>
            <option value="PRIMARY BUSINESS RANK">Primary Business Rank</option>
            <option value="SECONDARY BUSINESS RANK">Secondary Business Rank</option>
            <option value="(SUB) PRIMARY DEVELOPER">(SUB) Primary Developer</option>
            <option value="(SUB) SECONDARY DEVELOPER">(SUB) Secondary Developer</option>
            <option value="(SUB) PRIMARY BUSINESS RANK">(SUB) Primary Business Rank</option>
            <option value="(SUB) SECONDARY BUSINESS RANK">(SUB) Secondary Business Rank</option>--%>
            <option value="SUBMITTED BY">Submitted By</option>
            <option value="CLOSED DATE">Closed Date</option>
            <option value="CREATED DATE">Created Date</option>
            <option value="DEPLOYED DATE">Deployed Date</option>
            <option value="IN PROGRESS DATE">In Progress Date</option>
            <option value="READY FOR REVIEW DATE">Ready For Review Date</option>
            <option value="UPDATED DATE">Updated Date</option>
            <option value="UPDATED BY">Updated By</option>  
            <option value="NEEDED DATE">Needed Date</option>
            <option value="RQMT Risk">RQMT Risk</option>
        </select>
        <select id="ddlParameterSort">
            <option value="0">Ascending</option>
            <option value="1">Descending</option>
        </select>
    </div>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedRollupGroup = '', _selectedSortType = 'Tech';
        var strXML = '';
        var LastXML = '';
        var doDrop = 0;
        var gridType = '<%=GridType%>';

    </script>
    <script id="jsEvents" type="text/javascript">
        var ddlSection_Param_Count = 0;
        var ddlSection_Param_DefaultVal = $('#ddlParameterTypes').find('option').first().val();

        function closePage() {
            if (closeWindow) {
                closeWindow();
            }
            else {
                window.close();
            }
        }

        function loadLevels(lastXML) {

            if (lastXML != '' & lastXML != undefined) {
                loadSections($.parseXML(lastXML));
            }
            else {
                var levelsXML = $('#<%=this.ddlView.ClientID %> option:selected').attr("SectionsXML");
                loadSections($.parseXML(levelsXML));
            }
        }

        function loadSections(xmlOptions) {
            // -- remove all existing sections and add new ones
            var sections = $(xmlOptions).find("level");

            $("#tdSections").empty();
            for (var s = 0; s < sections.length; s++) {

                addSection();

                var breakouts = $(sections[s]).find("breakout");

                var breakoutParam = $(breakouts[0]).find("column").text();
                var breakoutSort = $(breakouts[0]).find("sort").text();

                if (breakoutSort == 'Ascending')
                    breakoutSort = 0;
                else
                    breakoutSort = 1;

                $("#tblSection" + (s + 1)).find("select:eq(0)").val(breakoutParam);
                $("#tblSection" + (s + 1)).find("select:eq(1)").val(breakoutSort);

                for (var b = 0; b < breakouts.length; b++) {

                    //add additional breakouts
                    var breakoutParam = $(breakouts[b]).find("column").text();
                    var breakoutSort = $(breakouts[b]).find("sort").text();

                    if (breakoutSort == 'Ascending')
                        breakoutSort = 0;
                    else
                        breakoutSort = 1;

                    addBreakout($("#tblSection" + (s + 1)), (s + 1), breakoutParam, breakoutSort);
                }
            }
        }

        function refreshPage() {
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'ddlChanged_ML', 'yes');

            document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function resizePage() {
            try {
                var heightModifier = 0;
                heightModifier += 10;

                resizePageElement('divPage', heightModifier + 2);
                resizePageElement('divTabsContainer', heightModifier + 3);
                resizePageElement('divBasic', heightModifier + 11);
            }
            catch (e) {
                var m = e.message;
            }
        }

        function ddlView_change() {

            loadLevels();
        }

        function ddlSaveView_change() {
            var $opt = $('#ddlSaveView option:selected');

            if ($opt.text() != '--Create New--') {
                $('#trViewName').hide();
                $('#chkProcessView').prop('checked', $opt.attr('OptionGroup') == 'Process Views');
            }
            else {
                $('#<%=txtViewName.ClientID %>').val('');
                $('#trViewName').show();
                $('#chkProcessView').prop('checked', false);
            }
        }

        function imgSaveView_click(obj) {
            $('#divDimmer').show();
            var pos = $(obj).position();

            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true);
            $('#trViewName').show();
            $('#chkProcessView').prop('checked', false);

            $('#divViewName').css({
                position: "absolute",
                top: pos.top + 31,
                left: 15
            }).slideDown(function () { $('#<%=this.txtViewName.ClientID %>').focus(); });
        }

        function buttonSaveView_click(option, name, checked) {
            var $opt = $('#ddlSaveView option:selected');
            if (option) $opt = option;

            var viewName = $('#<%=txtViewName.ClientID %>').val().trim();
            if (name) viewName = name;

            var processView = $('#chkProcessView').is(':checked') ? 1 : 0;
            if (checked) processView = checked;

            if ($opt.text() == '--Create New--') {
                if (viewName.toUpperCase() === 'DEFAULT') {
                    MessageBox('You cannot save with grid view name Default.');
                }
                else if (viewName !== '') {
                    var exists =
                        $('#<%=ddlView.ClientID %> option').filter(function () {
                            return $(this).text().trim().toUpperCase() === viewName;
                        }).length > 0;

                    if (!exists) {
                        confirmViewName('YES', $opt, viewName, checked);
                    }
                    else {
                        var myView = $('#<%=ddlView.ClientID %> option').filter(function () {
                            return $(this).text().trim().toUpperCase() === viewName;
                        }).first().attr('MyView');

                        if (myView === '1') {
                            QuestionBox('Confirm View Name', 'View name already exists. Would you like to overwrite?', 'Yes,No', 'confirmViewName', 300, 300, window.self, $opt, viewName, processView);
                        }
                        else {
                            MessageBox('View name already exists. You cannot overwrite view name which you did not create.');
                        }
                    }
                }
                else {
                    MessageBox('Please enter a view name.');
                }
            }
            else if ($opt.text().toUpperCase() == 'DEFAULT') {
                MessageBox('You cannot save with grid view name Default.');
            }
            else {
                var myView = $opt.attr('MyView');

                if (myView === '1') {
                    //QuestionBox('Confirm View Name', 'View name already exists. Would you like to overwrite?', 'Yes,No', 'confirmViewName', 300, 300, window.self);
                    confirmViewName('YES', $opt, viewName, checked);
                }
                else {
                    MessageBox('You cannot overwrite view name which you did not create.');
                }
            }
        }

        function confirmViewName(answer, option, name, checked) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                var gridViewID = 0;
                var viewName = '';
                var $opt = option;
                var processView = $('#chkProcessView').is(':checked') ? 1 : 0;

                if ($opt.text() != '--Create New--') {
                    gridViewID = $opt.val();
                    viewName = $opt.text();
                } else {
                    viewName = name;

                    var exists = $('#ddlSaveView option').filter(function () {
                        return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
                    }).length > 0;

                    if (exists) {
                        gridViewID = $('#<%=ddlView.ClientID %> option').filter(function () {
                            return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
                        }).val();
                    }
                }

                if (checked) processView = checked;

                try {
                    strXML = buildLevelsXML();
                    PageMethods.SaveView(gridViewID, viewName, processView, strXML, gridType, btnSaveView_Done, on_error);

                } catch (e) {
                    alert('Error in Save View. ' + e.message);
                }
            }
        }

        function on_error() {
            if (result.indexOf('True') > 0)
                MessageBox('Saved');
            else
                MessageBox('Error saving:' + result);

            refreshPage();

        }

        function btnSaveView_Done(result) {
            if (result.indexOf('True') > 0) {
                MessageBox('Saved');
            }
            else {
                MessageBox('Error saving:' + result);
            }

            refreshPage();
        }

        function buildLevelsXML() {

            try {

                // Section breakouts
                var strXML = "<crosswalkparameters>";

                var sortInt = 0;
                var sortText = '';
                
                $("[id^='tblSection']").each(function () {
                    strXML += "<level>";

                    $(this).find(".breakout").each(function () {
                        var val = $(this).val();

                        if (val != null && val != 0 &&  val != '' && val != '-- SELECT LEVEL --') {

                            strXML += "<breakout><column>" + $(this).val() + "</column>";
                            if ($(this).parent().next().find(".sort").val() == 0)
                                sortText = 'Ascending'
                            else
                                sortText = 'Descending';
                            strXML += "<sort>" + sortText + "</sort></breakout>";
                        }
                    });
                    strXML += "</level>";
                });

                var rollupGroup = $('#<%=this.ddlRollupGroup.ClientID %>').val();
                var defaultTaskSort = $('#<%=this.ddlDefaultTaskSort.ClientID %>').val();

                if (rollupGroup != null || defaultTaskSort != null) {
                    strXML += "<options>";
                    if (rollupGroup != null) strXML += "<rollupgroup>" + rollupGroup + "</rollupgroup>"
                    if (defaultTaskSort != null) strXML += "<defaulttasksort>" + defaultTaskSort + "</defaulttasksort>";
                    strXML += "</options>";
                }

                strXML += "</crosswalkparameters> ";

                PageMethods.UpdateSession(strXML, gridType);

            } catch (e) {
                return '';
            }
            return strXML;
        }

        function imgDeleteView_click() {
            var gv = $.trim($('#<%=this.ddlView.ClientID %> option:selected').text()).toUpperCase();

            if (gv === "DEFAULT" || gv === "-- NEW GRIDVIEW --") {
                MessageBox('You cannot delete this grid view.');
            }
            else if ($('#<%=this.ddlView.ClientID %> option:selected').attr('MyView') != '1') {
                MessageBox('You cannot delete a grid view which was not created by you.');
            }
            else {
                PageMethods.DeleteView($('#<%=this.ddlView.ClientID %>').val(), imgDeleteView_Done, on_error);
            }
        }

        function imgDeleteView_Done(result) {
            refreshPage();
        }

        function buttonGetData_click() {
            var x = buildLevelsXML();
            var ddlIndex = $('#<%=this.ddlView.ClientID %> option:selected').index();
            var ddlText = $('#<%=this.ddlView.ClientID %> option:selected').text();

            if (x.indexOf('<breakout>') == -1) {
                alert('At least one Level is required.');
            }
            else {
                PageMethods.SaveDefaultXML(x, ddlIndex, ddlText, gridType, function () {
                    if (opener == undefined) {
                        top.setFilterSession(false, false);
                        if (parent.opener.location.href.toUpperCase().indexOf('DEFAULT.ASPX') != -1 || parent.opener.location.href.toUpperCase().indexOf('.ASPX') == -1) {
                            defaultParentPage.$('#buttonGetData').trigger('click');
                        }
                        else {
                            if (parent.opener.refreshPage) parent.opener.refreshPage();
                        }
                        popupManager.ActivePopup.Close();
                    }
                    else {
                        if (opener.refreshPage) opener.refreshPage();
                        setTimeout(closePage, 500);
                    }
                    //window.close();  // If window.open is used, use this line, otherwise:
                });
            }
        }

        // Functions below and some other code came from Report_Workload.aspx & code behind ?
        function newBreakout(sectionArea) {
            var breakout = '<tr><td style="text-align: center"> <div style="padding-top: 10px"> <select class="breaktoutSelection" style="width: 180px, font-family: Arial, font-size=11px;"></select><select style="width: 60px, font-family: Arial, font-size=11px;"><option value=\'ASC\'>ASC</option><option value=\'DESC\'>DESC</option></select></div></td><td><img src="Images/Icons/cross.png" class="breakoutCancel" style="float: right"/></td></tr>';
            var html = $.parseHTML(breakout); //parsed to html to be appended to the breakout section area.
            var numberBreakouts = $('tr', sectionArea).length //counts the number of rows in the section area table.

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

        function setSectionList(select) { //this sets the options in the breakout drop down lists. This function is called when a new breakout is added, on a blur event for breakout, or when a breakout is removed via cancel button.
            var selectedValue = typeof $('option:selected', select).val() == null ? '' : $('option:selected', select).val();
            var selectedText = typeof $('option:selected', select).html() == 'undefined' ? '' : $('option:selected', select).html();
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

        function breakoutSelectionBlurEvent(selection) { //dynamically bound to .breakoutSection blur event
            var selectedValue = typeof $('option:selected', selection).val() == null ? '' : $('option:selected', selection).val(); //get currently selected value
            var selectedText = typeof $('option:selected', selection).html() == 'undefined' ? '' : $('option:selected', selection).html();
            var previousValue = typeof $(selection).attr('prevValue') == 'undefined' ? '' : $(selection).attr('prevValue'); //select box holds its previous text and values as two attributes. This allows you to compare the selected value to the previously selected value.
            var previousText = typeof $(selection).attr('prevText') == 'undefined' ? '' : $(selection).attr('prevText');
            var selectionChanged = previousValue !== selectedValue;
            $(selection).attr('prevValue', selectedValue);
            $(selection).attr('prevText', selectedText);

            if (previousValue !== '' && selectionChanged) { //if the select option was changed, and the previous value was not blank, then add the previous value back to the array so that it is available to the other select boxes.
                var addOption = {};
                addOption.text = previousText;
                addOption.value = previousValue;
            }

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

        function breakoutCancelClickEvent(cancel) {
            $(cancel).focus(); //forces focus to click event. This prevents duplicate values from existing in breakoutOptions array as the blur event might not have been triggered.
            var option = {};
            option.text = $(cancel).closest('tr').find('select option:selected')[0].text; //get the value of the first select.
            option.value = $(cancel).closest('tr').find('select option:selected')[0].value;
            if (option.text !== '') { //if selected option is the emptry string it is the default "non" value. By not removing it this allows for every select box to have the empty string/blank option.
                $(cancel).closest('tr').remove(); //need to remove the select before reseting the options in all select list, otherwise this removed select gets set as well.
                $('.breaktoutSelection').each(function () {
                    setSectionList(this);
                });
            }
            $(cancel).closest('tr').remove(); //in the 'else' case still need to remove the list, but don't need to do the other stuff (because it is the ''/empty option)>
        };


        // Not used any longer:
        function checkBreakoutLevels(sectionNumber) {
            try {
                // max 5 levels
                var breakoutCount = $("[id^='trSection_breakout" + sectionNumber + "']").length;
                if (breakoutCount == 5) {
                    $("#divAddBreakout" + sectionNumber).hide();
                }
                else {
                    $("#divAddBreakout" + sectionNumber).show();
                }

                //disableParameters(sectionNumber);
            }
            catch (e) { }
        }

        function removeBreakout(row, sectionNumber) {
            try {

                var delSelectedValue = $(row).find('select[id^="ddlSection_Param"] > option[selected="selected"]').val();
                //Un-disable the ddl_ParamSections selected value from all others in current table
                $(row).closest('table').find('select[id^="ddlSection_Param"]').each(function () {
                    $(this).find('option[value="' + delSelectedValue + '"]').prop('disabled', false);

                });



                $(row).remove();
                //checkBreakoutLevels(sectionNumber);
            }
            catch (e) { alert("Error " + e.number + ": removeBreakout()- " + e.message); }
        }

        //Create a breakout row and add it to the current table
        function addBreakout(tblSection, sectionNumber, selectedParam, selectedSort) {
            try {
                var breakoutNum = $("[id^='trSection_breakout" + sectionNumber + "']").length + 1;

                var nRow = $("<tr>").attr("id", "trSection_breakout" + sectionNumber + "_" + breakoutNum);
                var nCell0 = $("<td>");
                var nCell1 = $("<td>");
                var nCell2 = $("<td>");
                var nCell3 = $("<td>");
                nCell0.attr("id", "tdImgSection_Delete" + sectionNumber + "_" + breakoutNum);


                var imgMove = $("<img>").attr("src", "images/icons/Move.png").attr("alt", "Move").attr("title", "Move Breakout");
                nCell3.width("20px").height("20px");
                nCell3.append(imgMove);
                nRow.append(nCell3);

                //delete button
                var imgDelete = $("<img>").attr("src", "images/icons/delete.png").attr("alt", "Remove").attr("title", "Remove Breakout");//.hide();
                imgDelete.attr("id", "imgSection_Delete" + sectionNumber + "_" + breakoutNum);
                $(imgDelete).click(function () { removeBreakout(nRow, sectionNumber); });
                nCell0.width("20px").height("20px");
                nCell0.append(imgDelete);
                nRow.append(nCell0);

                //parameter DDL
                var paramDDL = $("#ddlParameterTypes").clone();
                var paramDDL_ID = "ddlSection_Param" + ddlSection_Param_Count;
                ddlSection_Param_Count += 1;
                paramDDL.attr("id", paramDDL_ID);
                paramDDL.addClass("breakout");

                if ($(paramDDL).find('option[value="-- SELECT LEVEL --"]').length > 0) {
                    $(paramDDL).val('-- SELECT LEVEL --');
                }
                else {
                    $(paramDDL).val('0'); // always add breakouts as EMPTY
                }

                disableOtherDDLOpts_FromCurrTBL(tblSection, paramDDL);
                
                nCell1.append(paramDDL);
                nRow.append(nCell1);

                //sort DDL
                var sortDDL = $("#ddlParameterSort").clone();
                var sortDDL_ID = "ddlParameterSort" + sectionNumber + "_" + breakoutNum;
                sortDDL.attr("id", sortDDL_ID);
                sortDDL.addClass("sort");

                nCell2.append(sortDDL);
                nRow.append(nCell2);

                $(tblSection).find('tbody').append(nRow);


                selectDLLAndAddEvents_param($('#' + sortDDL_ID), selectedSort);
                selectDLLAndAddEvents_param($('#' + paramDDL_ID), selectedParam);

                makeTableSortable();
            }
            catch (e) {
                alert("Error " + e.number + ": addBreakout()- " + e.message);
            }
        }

        function disableOtherDDLOpts_FromCurrTBL(currTbl, currDDL) {

            var allSelectedItems = [];
            $(currTbl).find('select[id^="ddlSection_Param"]').each(function () {
                if ($(this).find('option[selected="selected"]')[0] != null) {
                    allSelectedItems.push($(this).find('option[selected="selected"]')[0].value);
                }
            });

            for (i = 0; i < allSelectedItems.length; i++) {
                $(currDDL).find('option[value="' + allSelectedItems[i] + '"]').prop('disabled', true);
            }

        }


        function addSection() {
            try {
                var newSectionNum = 0;
                var newSectionLabelNumber = $("[id^='tblSection']").length + 1;

                ////max 5 sections
                //$("[id^='tblSection']").each(function () {
                //    var sectionID = $(this).attr("id");

                //    if (sectionID.substring(sectionID.length - 1) > newSectionNum) {
                //        newSectionNum = sectionID.substring(sectionID.length - 1);
                //    }
                //});
                //newSectionNum++;

                newSectionNum = newSectionLabelNumber;

                var divHeaderPad = $("<div>").attr("id", "divHeaderSection" + newSectionNum).css("padding-bottom", "10px");

                // Section Header
                var divHeader = $("<div>").addClass("pageContentHeader").css({ "text-align": "center", "padding-top": "3px", "border-top": "1px solid gainsboro", "height": "20px" });

                var headerTable = $("<thead>").css("width", "100%").attr("cellspacing", 0).attr("cellpadding", 0);
                var headerRow = $("<tr>");

                //section label
                var headerCell0 = $("<td>").css("padding", "0px");
                $(headerCell0).attr("id", "lblSection" + newSectionNum).append("Level " + newSectionLabelNumber);
                headerRow.append(headerCell0);

                //remove section button
                var imgRemove = $("<img>").attr("src", "images/icons/delete.png").attr("alt", "Remove").attr("title", "Remove Level").css("text-align", "right");
                imgRemove.click(function () { removeSection(imgRemove); });
                $(imgRemove).on("mouseover", function () { showToolTip("You cannot remove a level if there is only one left", imgRemove); });
                $(imgRemove).on("mouseout", function () { showToolTip("Remove Level", imgRemove) });

                var headerCell1 = $("<td>").css({ "padding": "0px 5px 0px 0px", "text-align": "right", "width": "25px" });

                headerCell1.append(imgRemove);
                headerRow.append(headerCell1);

                headerTable.append(headerRow);
                divHeader.append(headerTable);

                divHeaderPad.append(divHeader);

                //Section Body
                var nTable = $("<table>").attr("id", "tblSection" + newSectionNum).width("433px");
                var nRow = $("<tr>");
                var nCell = $("<td>");
                nCell.attr("colspan", 4);
                nCell.append(divHeaderPad);
                nRow.append(nCell);

                var tHead = $("<thead>");
                tHead.append(nRow);

                nTable.append(tHead);

                //Breakout button
                var divAdd = $("<td>").attr("id", "divAddBreakout" + newSectionNum);
                divAdd = $("<td>").attr("colspan", "3");
                var imgAdd = $("<img>").attr("src", "images/icons/add_blue.png").attr("alt", "Add").attr("title", "Add Breakout");
                imgAdd.css({ "width": "15px", "height": "15px", "padding-left": "5px" });
                imgAdd.click(function () {
                    addBreakout(nTable, newSectionNum);
                });
                divAdd.append(imgAdd).append("<span style=\"white-space: pre\"> Add Breakout</span>");

                //nTable.append(nRow);
                //nDiv.append(divSubtotal);
                //nDiv.append(nTable);

                var tBody = $("<tbody>").addClass("connectedSortable").attr("id", "tbody");

                nTable.append(tBody);

                var tFoot = $("<tfoot>");
                tFoot.append(divAdd);
                //Add to HTML source
                nTable.append(tFoot);

                //nTable.find('tbody').sortable();

                $("#tdSections").append(nTable);
                makeAllTablesSortable();
                resizePage();

            }
            catch (e) { alert("Error " + e.number + ": addSection()- " + e.message); }
        }

        function showToolTip(newTitle, imgRef) {
            if ($('table[id^="tblSection"]').length == 1) {
                $(imgRef).attr("title", newTitle);
            }

        }

        function removeSection(thisImg) {
            try {
                if ($('table[id^="tblSection"]').length > 1) {
                    $(thisImg).closest('table').remove();
                    reNumberTables();
                }
            }
            catch (e) { alert("Error " + e.number + ": removeSection()- " + e.message); }
        }

        function selectDLLAndAddEvents_param(thisDDL, selectedParam) {
            try {
                 //Bind the DDLs events if they dont exist
                if (!($._data($(thisDDL)[0]).events)) {
                    $(thisDDL).on("change", function () { paramDDLChange(thisDDL); });
                }

                //Find out if this is a breakoutDDL or sortDDL. Then set it accordingly

                if ($(thisDDL).filter('[id^="ddlParameterSort"]').length > 1) {
                    $(thisDDL).find('option').removeAttr("selected");
                    $(thisDDL).find('option[value="' + selectedParam + '"]').attr("selected", "selected");
                }
                else {

                    if (selectedParam) {
                        var itemExists = false;
                        var prevValue;
                        //If the current DDL has a selected option get it now so we can revert it if it already exists
                        if ($(thisDDL).find('option[selected="selected"]').length > 0) {
                            prevValue = $(thisDDL).find('option[selected="selected"]')[0].innerText.toUpperCase();
                        }

                        //Set the DDL option selected attribute
                        $(thisDDL).find('option').removeAttr("selected");
                        $(thisDDL).val(selectedParam);
                        $(thisDDL).find('option[value="' + selectedParam + '"]').attr("selected", "selected");

                        //Allow user to have multiple "select level" options
                        if (selectedParam != ddlSection_Param_DefaultVal) {

                            //Loop through all DDLS in current table except current DDL
                            $(thisDDL).closest('table').find('select[id^="ddlSection_Param"]').not(thisDDL).each(function () {
                                //UnHide PrevSelected
                                $(this).find('option[value="' + prevValue + '"]').prop('disabled', false);

                                //Hide CurrSelected
                                $(this).find('option[value="' + selectedParam + '"]').prop('disabled', true);

                            });
                        }

                    }
                    else {
                        //Regular DDL select
                        selectedParam = $(thisDDL)[0].value;
                        $(thisDDL).find('option').removeAttr("selected");
                        $(thisDDL).find('option[value="' + selectedParam + '"]').attr("selected", "selected");
                        $(thisDDL).val(selectedParam);
                    }
                }
            }
            catch (e) { alert("Error " + e.number + ": selectDLLAndAddEvents()- " + e.message); }
        }

        function paramDDLChange(thisDDL) {
            try {

                selectDLLAndAddEvents_param(thisDDL, $(thisDDL).val());
            }
            catch (e) { alert("Error " + e.number + ": paramDDLChange()- " + e.message); }
        }

        function buildSaveView() {
            $('#<%=txtViewName.ClientID %>').val('');
            $('#ddlSaveView').html($('#<%=ddlView.ClientID %>').html());
            $('#ddlSaveView').prepend('<option>--Create New--</option>');
            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true).change();
        }


        function makeTableSortable() {

            $('table[id^="tblSection"] tbody').sortable({
                connectWith: ".connectedSortable",
                cursor: 'move',
                opacity: 0.6,
                start: function (evt, ui) {
                    $(ui.item).find('select[id^="ddlSection_Param"]')[0].id = $(ui.item).find('select[id^="ddlSection_Param"]')[0].id.replace('ddlSection_Param', '');
                },
                stop: function (evt, ui) {
                    $(ui.item).find('select')[0].id = 'ddlSection_Param' + $(ui.item).find('select')[0].id;
                },


                receive: function (event, ui) { //this will prevent move if item is already in drop target table
                    var itemExists = false;

                    var currentValue = $(ui.item).find('select').val().toUpperCase();

                    if (currentValue != ddlSection_Param_DefaultVal) {
                        $(this).find('select[id^="ddlSection_Param"]').each(function () {
                            if ($(this).val() == $(ui.item).find('option[selected="selected"]').first().val()) {
                                itemExists = true;
                                return false;
                            }
                        });
                    }
                    if (itemExists) {//Then we will revert the move
                        $(ui.sender).sortable('cancel');
                        alert("You cannot drop an item that already exists in another level");
                    }
                    else {//Then we are moving the DDL to a new table
                        //1) Disable currentValue from DDls in going to table
                        $(this).closest('table').find('select[id^="ddlSection_Param"] option[value= "' + currentValue + '"]').each(function () {
                            $(this).prop('disabled', true);
                        });

                        //2) Un-disable currentValue from DDls in coming from table
                         $(ui.sender).closest('table').find('select[id^="ddlSection_Param"] option[value= "' + currentValue + '"]').each(function () {
                            $(this).prop('disabled', false);
                        });


                        //3A) Un-disable all values from current DDL
                        $(ui.item).find('option').prop('disabled', false);
                        //3B) Disable all values in current DDL from set of selected in going to table
                        var currDDL = $(ui.item);
                        $(this).closest('table').find('select[id^="ddlSection_Param"] option[selected="selected"]').each(function () {
                            $(currDDL).find('option[value="' + $(this).val() + '"]').prop('disabled', true);
                        });

                    }
                }
            });
        }

        function makeAllTablesSortable() {
            $('td[id^="tdSections"]').sortable({
                items: '> table[id^="tblSection"]',
                stop: function (evt, ui) {
                    reNumberTables();
                }
            });

        }

        function reNumberTables() {
            $('table[id^="tblSection"] td[id^="lblSection"').each(
                function (i) {
                    $(this)[0].innerText = $(this)[0].innerText.replace(/[0-9]/g, i + 1);
                }
            );
        }


    </script>

    <script id="jsInit" type="text/javascript">

        function initializeEvents() {
            $("#btnAddSection").click(function () { addSection(); return false; });
            $('#ddlSaveView').on('change', function () { ddlSaveView_change(); });
        }

        function initVariables() {
            try {
                _pageUrls = new PageURLs();
                _selectedRollupGroup = '<%=this.RollupGroup %>';
                _selectedSortType = '<%=this.DefaultSortType %>';

<%--			    _UseColumnOrdering = '<%=this.columnOrderPref %>';--%>
                if (_UseColumnOrdering == "True")
                    $('#chkUseColumnOrdering').attr('checked', true);
                else
                    $('#chkUseColumnOrdering').attr('checked', false);

                $('#newBreakoutOne').click(function () {
                    newBreakout($('#sectionOneArea'));
                });

                $('#newBreakoutTwo').click(function () {
                    newBreakout($('#sectionTwoArea'));
                });

            } catch (e) {

            }
        }

        function initControls() {
            var ddl = $('#ddlParameterTypes');

            if (gridType == 'RQMT Grid') {
                ddl.html('');

                var options = '';

                options += '<option value="RQMT Accepted">Accepted</option>';
                options += '<option value="Complexity">Complexity</option>';
                options += '<option value="RQMT Criticality">Criticality</option>';                

                options += '<option value="RQMT Defect Description">Defect Description</option>';
                options += '<option value="RQMT Defect Impact">Defect Impact</option>';
                options += '<option value="RQMT Defect Mitigation">Defect Mitigation</option>';
                options += '<option value="RQMT Defect Number">Defect #</option>';
                options += '<option value="RQMT Defect Stage">Defect PD2TDR</option>';
                options += '<option value="RQMT Defect Resolved">Defect Resolved</option>';
                options += '<option value="RQMT Defect Review">Defect Review</option>';
                options += '<option value="RQMT Defect Verified">Defect Verified</option>';
                options += '<option value="RQMT Defects">Defects</option>';

                options += '<option value="Description">Description</option>';
                options += '<option value="Functionality">Functionality</option>';
                options += '<option value="Justification">Justification</option>';                
                options += '<option value="Outline Index">Outline Index</option>';
                options += '<option value="Outline Index Child">Outline Index - Child</option>';
                options += '<option value="Outline Index Parent">Outline Index - Parent</option>';
                options += '<option value="RQMT Type">Purpose</option>';

                options += '<option value="RQMT">RQMT</option>';                
                options += '<option value="RQMT #">RQMT #</option>';
                options += '<option value="RQMT Metrics">RQMT Metrics</option>';
                options += '<option value="RQMT Primary">RQMT Primary</option>';    
                options += '<option value="RQMT Primary #">RQMT Primary #</option>';
                options += '<option value="RQMT Set">RQMT Set Name</option>';
                options += '<option value="RQMT Set Complexity">RQMT Set Complexity</option>';
                options += '<option value="RQMT Set Complexity Justification">RQMT Set Complexity Justification</option>';                
                options += '<option value="RQMT Set Usage">RQMT Set Usage</option>';                
                options += '<option value="RQMT Stage">RQMT Stage</option>';
                options += '<option value="RQMT Status">RQMT Status</option>';                
                options += '<option value="RQMT Usage">RQMT Usage</option>';
                options += '<option value="RQMT Usage Month">RQMT Usage - Month</option>';


                options += '<option value="System">System</option>';
                options += '<option value="SYSTEM SUITE">System Suite</option>';

                options += '<option value="Work Area">Work Area</option>';                

                ddl.html(options);

                $('#divFooter').hide();
                $('#divTabsContainer').css('padding', '0px');
            }
        }

        $(document).ready(function () {
            try {
                initVariables();
                initializeEvents();
                initControls();
                var selectedView = '<%=ddlView.SelectedValue%>';

                $('#<%=this.ddlView.ClientID %>').on("change", function () { ddlView_change(); return false; });
                $('#imgSaveView, #imgSaveViewAdv').click(function () { imgSaveView_click(this); });
                $('#<%=this.txtViewName.ClientID %>').on("keypress", function (e) { if (e.which == 13) { $('#buttonSaveView').trigger('click'); return false; } });
                $('#buttonSaveView').click(function () { buttonSaveView_click(); return false; });
                $('#buttonCancelView').click(function () { $('#divViewName').slideUp(function () { $('#divDimmer').hide(); }); return false; });
                $('#imgDeleteView, #imgDeleteViewAdv').click(function () { imgDeleteView_click(); });
                $('#imgRefresh').click(function () { refreshPage(); });
                $('#buttonGetData').click(function () { buttonGetData_click(); return false; });
                $(document.body).bind('onbeforeunload', function () { buttonGetData_click(); });

                var tab = parseInt('<%=ActiveTab %>');
                $('#divTabsContainer').tabs({
                    heightStyle: "fill"
                    , collapsible: false
                    , active: tab
                });

                $("#<%=this.ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
                $("#<%=this.ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");

                $('#<%=this.ddlView.ClientID %>').val(selectedView); //I don't know why this needs to be here. The page refreshes when the ddlviews are changed. For some reason the selected value is changed to an option other than what was selected. The effect of selecting that value is correct in that the selected columns change to the correct values, but the drop down list doesn't reads "dev standup" every time, for no apparant reason. The problem happens somewhere in the tab change code, but I couldn't figure it out so this hack fixes it. if and when you do figure it out, feel free to leave a comment explaining how you did it. I am curious.
                if ($.trim('<%=this.Grid_View %>') == '' && $('#<%=this.ddlView.ClientID %> > option').length > 0) ddlView_change();

                // If txtXML has last used XML, pass it into loadLevels, otherwise it'll use drop down
                LastXML = $('#<%=this.txtXML.ClientID %>').val();
                loadLevels(LastXML);
                buildSaveView();

                resizePage();

            } catch (e) {
                var m = e.message;
            }
        });
    </script>
</asp:Content>
