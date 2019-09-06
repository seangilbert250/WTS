<%@ Page Language="C#" EnableViewState="false" AutoEventWireup="true" CodeFile="AOR_SubTasks_Add.aspx.cs" Inherits="AOR_SubTasks_Add" Theme="Default" %>

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
        <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	    </iti_Tools_Sharp:Grid>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _maxSelections;
    </script>

    <script id="jsEvents" type="text/javascript">
        function input_change(obj) {
            var $obj = $(obj);
            var subTaskID = $obj.attr('subtask_id');   
            var number = $obj.attr('subtask_number');
            var taskNumber = subTaskID + '.' + number;
            var taskChecked = $obj.find('input[type="checkbox"]').is(':checked');
            
            if (_maxSelections > 0 && parent.arrSubTasks.length >= _maxSelections && taskChecked) {
                if (_maxSelections == 1) { // just clear other selection and add this one
                    parent.arrSubTasks = [];

                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
                        var thisSubTaskID = $(this).parent().attr('subtask_id');

                        if (subTaskID != thisSubTaskID) {
                            $(this).prop('checked', false);
                        }
                    });
                }
                else { // don't allow any more selections
                    $obj.find('input[type="checkbox"]').prop('checked', false);
                    return;
                }
            }

	        $obj.attr('fieldChanged', '1');
	        $obj.closest('tr').attr('rowChanged', '1');
            
            if (taskChecked) {
                if ($.inArray(taskNumber, parent.arrSubTasks) == -1) parent.arrSubTasks.push(taskNumber);
            }
            else {
                if ($.inArray(taskNumber, parent.arrSubTasks) != -1) {                    
                    parent.arrSubTasks.splice($.inArray(taskNumber, parent.arrSubTasks), 1);
                }
            }
	        
            $('#btnAdd', parent.document).prop('disabled', false);
            countSelected();
        }

        function loadGrid() {
            parent.ShowDimmer(true, 'Loading...', 1);
        }

	    function checkSelected() {
	        $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
	            var $obj = $(this).parent();

	            var subTaskID = $obj.attr('subtask_id');   
	            var number = $obj.attr('subtask_number');
	            var taskNumber = subTaskID + '.' + number;

	            if ($.inArray(taskNumber, parent.arrSubTasks) != -1) {
	                $(this).prop('checked', true);
	            }
	            else {
	                $(this).prop('checked', false);
	            }
	        });

	        if (parent.arrSubTasks.length > 0) $('#btnAdd', parent.document).prop('disabled', false);
	        countSelected();
	    }

        function countSelected() {
            var selectedCount = parent.arrSubTasks.length;

            $('#spnSubTaskSelectedCount', parent.document).text(selectedCount + ' Subtask' + (selectedCount != 1 ? 's' : '') + ' Checked');
        }

        function openSubTask(subTaskID, workItemID) {
            var nWindow = 'WorkloadSubTask';
            var nTitle = 'Subtask';
            var nHeight = 700, nWidth = 850;
            var nURL = _pageUrls.Maintenance.TaskEdit;
            
            if (workItemID == null || workItemID == 0) {
                workItemID = <%=this.TaskID%>;
            }

            if (parseInt(subTaskID) > 0) {
                nTitle;
                nURL += '?workItemID=' + workItemID + '&taskID=' + subTaskID + '&ReadOnly=true';
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
            var grd = $('#<%=this.grdData.ClientID %>');

            if (grd.length > 0) {
                <%=this.grdData.ClientID %>.ResizeGrid();
            }
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            checkSelected();
        });
    </script>
</body>
</html>
