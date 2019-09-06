﻿<%@ Page Language="C#" EnableViewState="false" AutoEventWireup="true" CodeFile="AOR_Tasks_Add.aspx.cs" Inherits="AOR_Tasks_Add" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <title>AOR</title>
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        
        <div id="selectedDataDiv"style=" max-height: 150px; overflow: auto; display: none">
                <iti_Tools_Sharp:Grid ID="selectedData" runat="server" AllowPaging="true" PageSize="3" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="false"
                    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
                </iti_Tools_Sharp:Grid>
        </div>
        <div style="overflow:auto">
            <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="20" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
                CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
            </iti_Tools_Sharp:Grid>
        </div>
        <asp:Button ID="btnLoadGrid" runat="server" Style="display: none;" />
        <asp:TextBox ID="txtAppliedFilters" runat="server" Style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtTaskSearch" runat="server" Style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtPostBackType" runat="server" Style="display: none;"></asp:TextBox>
    </form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _maxSelections;
        var parent_arrSelectedTaskRows;
    </script>

    <script id="jsEvents" type="text/javascript">
        function input_change(obj) {
            var $rowSpan = $(obj);
            var taskID = $rowSpan.attr('task_id');
            var taskChecked = $rowSpan.find('input[type="checkbox"]').is(':checked');
            var $row = $rowSpan.closest('tr');

            if (_maxSelections > 0 && parent.arrTasks.length >= _maxSelections && taskChecked) {
                if (_maxSelections == 1) { // just clear other selection and add this one
                    parent.arrTasks = [];
                    parent.arrSelectedTaskRows = JSON.stringify([]);
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
                        var thisTaskID = $(this).parent().attr('task_id');

                        if (taskID != thisTaskID) {
                            $(this).prop('checked', false);
                        }
                    });
                }
                else { // don't allow any more selections
                    $rowSpan.find('input[type="checkbox"]').prop('checked', false);
                    return;
                }
            }

            $rowSpan.attr('fieldChanged', '1');
            $row.attr('rowChanged', '1');
            

            if (taskChecked) {
                insertSelectedRow($row, taskID);
            }
            else {
                deleteSelectedRow(taskID);
            }
		    $('#btnAdd', parent.document).prop('disabled', false);
		    countSelected();
        }

        function insertSelectedRow(row, taskID) {
            if ($.inArray(taskID, parent.arrTasks) == -1) {
                parent.arrTasks.push(taskID);

                if ('<%=this.Type%>' == 'Task') {
                    var insertRow = $(row).clone();
                    $(insertRow).find('td').first().remove();
                    $(insertRow).find('span').attr("disabled", true);
                    $(insertRow).insertAfter(selectedData.Body.Rows[0]);
                    $(insertRow).attr('class', 'gridBody');

                    parent_arrSelectedTaskRows = getParent_arrSelectedTaskRows();

                    parent_arrSelectedTaskRows.push({ id: taskID, row: insertRow[0].outerHTML });

                    setParent_arrSelectedTaskRows();
                }
                
            }
        }

        function deleteSelectedRow(taskID) {
            if ($.inArray(taskID, parent.arrTasks) != -1) {
                parent.arrTasks.splice($.inArray(taskID, parent.arrTasks), 1);

                if ('<%=this.Type%>' == 'Task') {
                    //Get parent.arrSelectedTaskRows
                    parent_arrSelectedTaskRows = getParent_arrSelectedTaskRows();

                    $(selectedData.Body.Grid).find('a:contains("' + taskID + '")').closest('tr').remove();
                    parent_arrSelectedTaskRows = $.grep($(parent_arrSelectedTaskRows), function (row) {
                        return row.id != taskID;
                    });

                    setParent_arrSelectedTaskRows();
                }                
             }
        }

        function getParent_arrSelectedTaskRows() {
            return  JSON.parse(parent.arrSelectedTaskRows);
        }

        function setParent_arrSelectedTaskRows() {
            parent.arrSelectedTaskRows = JSON.stringify(parent_arrSelectedTaskRows);
        }


        function chkTask_change(obj) {
            var $obj = $(obj);
            if ($obj.find('input[type="checkbox"]').is(':checked')) {
                if ($.inArray($obj.attr('task_id'), parent.arrTasks) == -1) parent.arrTasks.push($obj.attr('task_id'));
                $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').not($obj.find('input[type="checkbox"]')).prop('checked', false).prop('disabled', true);
                $('#btnAdd', parent.document).prop('disabled', false);
            }
            else {
                if ($.inArray($obj.attr('task_id'), parent.arrTasks) != -1) parent.arrTasks.splice($.inArray($obj.attr('task_id'), parent.arrTasks), 1);
                $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').not($obj.find('input[type="checkbox"]')).prop('disabled', false);
                $('#btnAdd', parent.document).prop('disabled', true);
            }
            countSelected();
        }

        function loadGrid() {
            parent.ShowDimmer(true, 'Loading...', 1);

            var filters = parent.filterBox.toJSON({ groups: { ParentModule: 'Work' } });

            $('#<%=this.txtAppliedFilters.ClientID %>').val(filters);
            $('#<%=this.txtPostBackType.ClientID %>').val('LoadGrid');
            if ($('*[id*=txtTaskSearch]', parent.document) != undefined) $('#<%=this.txtTaskSearch.ClientID %>').val($('*[id*=txtTaskSearch]', parent.document).val());
            $('#<%=this.btnLoadGrid.ClientID %>').trigger('click');
        }

        function checkSelected() {
            //Check selected chkboxes
            $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
                var $obj = $(this).parent();

                if ($.inArray($obj.attr('task_id'), parent.arrTasks) != -1) {
                    $(this).prop('checked', true);
                }
                else {
                    $(this).prop('checked', false);
                }
            });

            //Fill in selected Rows
            parent_arrSelectedTaskRows = getParent_arrSelectedTaskRows();

            $.each(parent_arrSelectedTaskRows, function () {
                $($(this)[0].row).insertAfter(selectedData.Body.Rows[0]);
            });

            if (parent.arrTasks.length > 0) {
                $('#btnAdd', parent.document).prop('disabled', false);
            }
            countSelected();
        }

        function countSelected() {
            var selectedCount = parent.arrTasks.length;

            if ('<%= this.Type%>' == 'Task') {
                if (selectedCount == 0) {
                    $('#selectedDataDiv').hide();
                }
                else {
                    $('#selectedDataDiv').show();
                }
            }
            $('#spnSelectedCount', parent.document).text(selectedCount + ' Work Task' + (selectedCount != 1 ? 's' : '') + ' Checked');
            <%=this.selectedData.ClientID %>.ResizeGrid();
            <%=this.grdData.ClientID %>.ResizeGrid();
        }

        function openTask(workItemID, taskNumber, taskID, blnSubTask) {
            var nWindow = 'WorkTask';
            var nTitle = 'Primary Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(workItemID) > 0) {
                nTitle += ' - [' + workItemID + ']';
                nURL += '?workItemID=' + workItemID;
            }

            if (blnSubTask == '1') {
                nWindow = 'WorkSubTask';
                nTitle = 'Subtask - [' + workItemID + ' - ' + taskNumber + ']';
                nHeight = 700, nWidth = 850;
                nURL = _pageUrls.Maintenance.TaskEdit + '?workItemID=' + workItemID + '&taskID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _maxSelections = <%=MaxSelections%>;
        }

        function initDisplay() {
            parent.ShowDimmer(false);
            <%=this.grdData.ClientID %>.ResizeGrid();
            <%=this.selectedData.ClientID %>.ResizeGrid();

        }

        function initDefaultFilters() {
            var qsFilters = '<%=Filters%>';

            if (qsFilters != null && qsFilters.length > 0) {
                var filters = $.parseJSON(qsFilters);

                for (var fname in filters) {
                    var filter = filters[fname];

                    if (fname == 'AOR') {
                        var aorids = filter.value.split(',');
                        var aornames = filter.text.split(',');

                        for (var i = 0; i < aorids.length; i++) {
                            var aorid = aorids[i];
                            var aorname = decodeURI(aornames[i]);

                            parent.filterBox.filters.add({ name: 'AOR', groups: { ParentModule: 'Work', Module: "Custom" } }).parameters.add(aorid, aorname);
                        }
                    }
                }
            }

            $('#spnFilterCount', parent.document).text(parent.filterBox.filters.length + ' Filter' + (parent.filterBox.filters.length != 1 ? 's' : '') + ' Applied');
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            checkSelected();
            initDefaultFilters();
            if (parent.filterBox.filters.length > 0 && '<%=!IsPostBack%>' == 'True') {
                loadGrid();
            }
        });
    </script>
</body>
</html>