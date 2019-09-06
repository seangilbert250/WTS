<%@ Page Language="C#" EnableViewState="false" AutoEventWireup="true" CodeFile="AOR_Wizard_Frame.aspx.cs" Inherits="AOR_Wizard_Frame" Theme="Default" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Wizard</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <style>
        * {
            font-family: Arial;
            font-size: 12px;
        }
    </style>
</head>
<body>
	<form id="form1" runat="server">
        <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	    </iti_Tools_Sharp:Grid>
        <asp:Button ID="btnLoadGrid" runat="server" style="display: none;" />
        <asp:TextBox ID="txtAppliedFilters" runat="server" style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtTaskSearch" runat="server" style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtPostBackType" runat="server" style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtCRStatusQF" runat="server" style="display: none;"></asp:TextBox>
        <asp:TextBox ID="txtCRContractQF" runat="server" Text="0" style="display: none;"></asp:TextBox>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>

    <script id="jsEvents" type="text/javascript">
        function input_change(obj) {
            var $obj = $(obj);

	        $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');

            switch ('<%=this.Type %>') {
                case 'CR':
                    if ($obj.find('input[type="checkbox"]').is(':checked')) {
                        if ($.inArray($obj.attr('cr_id'), parent.arrCRs) == -1) parent.arrCRs.push($obj.attr('cr_id'));
                    }
                    else {
                        if ($.inArray($obj.attr('cr_id'), parent.arrCRs) != -1) parent.arrCRs.splice($.inArray($obj.attr('cr_id'), parent.arrCRs), 1);
                    }
                    break;
                case 'Task':
                    if ($obj.find('input[type="checkbox"]').is(':checked')) {
                        if ($.inArray($obj.attr('task_id'), parent.arrTasks) == -1) parent.arrTasks.push($obj.attr('task_id'));
                    }
                    else {
                        if ($.inArray($obj.attr('task_id'), parent.arrTasks) != -1) parent.arrTasks.splice($.inArray($obj.attr('task_id'), parent.arrTasks), 1);
                    }
                    break;
            }
	        
            countSelected();
        }

        function loadGrid() {
            $('#<%=this.txtPostBackType.ClientID %>').val('LoadGrid');
            
            switch ('<%=this.Type %>') {
                case 'CR':
                    $('#<%=this.txtCRStatusQF.ClientID %>').val(parent._selectedCRStatusesQF);
                    $('#<%=this.txtCRContractQF.ClientID %>').val($('*[id*=ddlCRContractQF]', parent.document).val());
                    break;
                case 'Task':
                    var filters = parent.filterBox.toJSON({ groups: { ParentModule: 'Work' } });

                    $('#<%=this.txtAppliedFilters.ClientID %>').val(filters);
                    $('#<%=this.txtPostBackType.ClientID %>').val('LoadGrid');
                    if ($('*[id*=txtTaskSearch]', parent.document) != undefined) $('#<%=this.txtTaskSearch.ClientID %>').val($('*[id*=txtTaskSearch]', parent.document).val());
                    $('#<%=this.btnLoadGrid.ClientID %>').trigger('click');
                    break;
            }

            $('#<%=this.btnLoadGrid.ClientID %>').trigger('click');
        }

	    function checkSelected() {
	        $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
	            var $obj = $(this).parent();

                switch ('<%=this.Type %>') {
                    case 'CR':
                        if ($.inArray($obj.attr('cr_id'), parent.arrCRs) != -1) {
                            $(this).prop('checked', true);
                        }
                        else {
                            $(this).prop('checked', false);
                        }
                        break;
                    case 'Task':
                        if ($.inArray($obj.attr('task_id'), parent.arrTasks) != -1) {
                            $(this).prop('checked', true);
                        }
                        else {
                            $(this).prop('checked', false);
                        }
                        break;
                }
	        });

	        countSelected();
	    }

        function countSelected() {
            switch ('<%=this.Type %>') {
                case 'CR':
                    var selectedCount = parent.arrCRs.length;

                    $('#spnSelectedCountCR', parent.document).text(selectedCount + ' CR' + (selectedCount != 1 ? 's' : '') + ' Checked');
                    break;
                case 'Task':
                    var selectedCount = parent.arrTasks.length;

                    $('#spnSelectedCount', parent.document).text(selectedCount + ' Work Task' + (selectedCount != 1 ? 's' : '') + ' Checked');
                    break;
            }
            if (typeof parent.checkStepCompletion != "undefined") parent.checkStepCompletion();
        }

        function sortItems() {
            var sorted = true;
            var itemRows;

            do {
                sorted = true;

                itemRows = $('.gridBody, .selectedRow', $('#<%=this.grdData.ClientID%>_Grid'));

                for (var i = 0; i < itemRows.length - 1; i++) {
                    var firstRow = itemRows[i];
                    var secondRow = itemRows[i + 1];

                    var firstRowChecked = $(firstRow).find('input[type=checkbox]:checked').length > 0;
                    var secondRowChecked = $(secondRow).find('input[type=checkbox]:checked').length > 0;

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
                        itemRows[i] = secondRow;
                        itemRows[i + 1] = firstRow;
                        $(secondRow).insertBefore(firstRow);
                    }
                }

            } while (!sorted);

            $(itemRows).each(function (index) {
                if (index % 2 == 1) $(this).css('background-color', '#DFDFDF');
                else $(this).css('background-color', '#FFFFFF');
            });
        }

        function openTask(taskID) {
            var nWindow = 'WorkTask';
            var nTitle = 'Work Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(taskID) > 0) {
                nTitle += ' - [' + taskID + ']';
                nURL += '?workItemID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function resizePage() {
            <%=this.grdData.ClientID %>.ResizeGrid();
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        $(document).ready(function () {
            initVariables();
            checkSelected();
            sortItems();

            $(document).on('change', 'input[type=checkbox]', function () {
                sortItems();
            });
        });
    </script>
</body>
</html>
