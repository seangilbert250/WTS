<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Tasks.aspx.cs" Inherits="AOR_Tasks" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Task</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Tasks</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnViewReleaseHistory" value="View Release History" disabled="disabled" style="vertical-align: middle;" />
                <input type="button" id="btnViewChangeHistory" value="View Change History" disabled="disabled" style="vertical-align: middle;" />
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Disassociate" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAORReleaseTaskID = 0;
        var _selectedTaskID = 0;
        var _selectedAssigned = '';
        var _selectedStatuses = '';
        var _currentLevel = 0;
        
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnViewReleaseHistory_click() {
            var nWindow = 'ViewReleaseHistory';
            var nTitle = 'View Release History';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Release History&TaskID=' + _selectedTaskID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnViewChangeHistory_click() {
            var nWindow = 'ViewChangeHistory';
            var nTitle = 'View Change History';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Change History&TaskID=' + _selectedTaskID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAdd_click() {
            var nWindow = 'AddAORTask';
            var nTitle = 'Add Work Task';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Task';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            var checkedCount = $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').length;

            if (checkedCount > 0) {
                QuestionBox('Confirm Task Disassociation', 'Are you sure you want to disassociate the checked Task' + (checkedCount > 1 ? 's' : '') + ' from the AOR?', 'Yes,No', 'confirmAORTaskDelete', 300, 300, this);
            }
            else {
                MessageBox('Please check at least one.');
            }
        }

        function confirmAORTaskDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    var arrReleaseTask = [];

                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
	                    var $obj = $(this).parent();

                        arrReleaseTask.push({ 'aorreleasetaskid': $obj.attr('aorreleasetask_id'), 'workitem': $obj.attr('workitem') });
                    });

                    ShowDimmer(true, 'Disassociating...', 1);

                    var nJSON = JSON.stringify(arrReleaseTask);

                    PageMethods.DeleteAORTask(nJSON, '<%=this.ReleaseAOR%>', function(result) { delete_done(result, arrReleaseTask.length); }, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result, checkedCount) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox((checkedCount > 1 ? 'Tasks have' : 'Task has') + ' been disassociated.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to disassociate. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function openTask(workItemID, taskNumber, taskID, blnSubTask) {
            var nWindow = 'WorkTask';
            var nTitle = 'Work Task';
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

        function row_click(obj) {
            if ($(obj).attr('aorreleasetask_id')) {
                _selectedAORReleaseTaskID = $(obj).attr('aorreleasetask_id');

                $('#btnDelete').prop('disabled', false);
            }

            if ($(obj).attr('task_id')) {
                _selectedTaskID = $(obj).attr('task_id');

                $('#btnViewReleaseHistory').prop('disabled', false);
                $('#btnViewChangeHistory').prop('disabled', false);
            }
        }

        function checkbox_click(obj) {
            if ($(obj).closest('span').attr('workitem')) {
                var workitem = $(obj).closest('span').attr('workitem').indexOf(' - ') > -1 ? $(obj).closest('span').attr('workitem').slice(0, $(obj).closest('span').attr('workitem').indexOf(' - ')) : $(obj).closest('span').attr('workitem');
                $('span[workitem*="' + workitem + '"]').find('input').each(function () {
                    $(this)[0].checked = obj.checked;
                });
            }
        }

        function imgShowHideChildren_click(sender, direction, id) {
            try {
                if (id == "0" || id == "ALL") {
                    var itemId = '0';
                    $('[Name="img' + direction + '"]').each(function () {
                        itemId = $(this).attr('itemId');
                        if (requestId && +requestId > 0) {
                            imgShowHideChildren_click(this, direction, itemId);
                        }
                    });
                }

                if (direction.toUpperCase() == "SHOW") {
                    //show row/div with child grid frame
                    //get frame and pass url(if necessary)
                    var td;

                    $(sender).closest('tr').each(function () {
                        var currentRow = $(this);
                        var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
                        $(row).show();
                        $(row).attr('nohighlight', 'true');

                        td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("Task_ID")) ? 0 : this.DCC["Task_ID"].Ordinal %>)', row)[0];
                        loadChildGrid(td, id);
                    });
                }
                else {
                    $('tr[Name="gridChild_' + id + '"]').hide();
                }

                $(sender).hide();
                $(sender).siblings().show();
                resizeFrame();
            } catch (e) {
                var msg = e.message;
            }
        }

        function loadChildGrid(td, id) {
            var url = window.location.href;
            url += '&WorkItemID=' + id;


            $('iFrame', $(td)).each(function () {
                var src = $(this).attr('src');
                if (src == "javascript:''") {
                    $(this).attr('src', url);
                }

                $(this).show();
            });
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            updateSelectedStatuses();
            updateSelectedAssigned();

            nURL = editQueryStringValue(nURL, 'ddlChanged', 'yes');
            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');
            nURL = editQueryStringValue(nURL, 'SelectedStatuses',_selectedStatuses);
		    nURL = editQueryStringValue(nURL, 'SelectedAssigned', _selectedAssigned);

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }

        function resizeFrame() {
            $('.pageContentHeader').hide();
            var $grid = $('#ctl00_ContentPlaceHolderBody_grdData_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#ctl00_ContentPlaceHolderBody_grdData_BodyContainer table').height();
            if ($('.ms-drop').is(':visible')) {
                var qfHeight = $('.ms-drop').height() + 75;

                if (bodyTableHeight < qfHeight) {
                    bodyTableHeight = qfHeight;
                }
            }

            var nHeight = headerTop + bodyTableHeight + 1;

            var nFrame = getMyFrameFromParent();
            $(nFrame).height(nHeight);
            resizeGrid();
            parent.resizeFrame('Tasks');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#pageContentHeader').hide();

            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('#btnAdd').show();

                if ('<%=this.RowCount %>' != '0') $('#btnDelete').show();
            }

            resizeGrid();

            if (parent.updateTab) parent.updateTab('Tasks', <%=this.RowCount %>, <%=this.SubtaskCount%>);
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnViewReleaseHistory').click(function () { btnViewReleaseHistory_click(); return false; });
            $('#btnViewChangeHistory').click(function () { btnViewChangeHistory_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            if ('<%=this.ReleaseAOR%>' === 'True') $('input[type="checkbox"]').click(function () { checkbox_click(this); });
        }

         function ddlStatus_change() {
	        updateSelectedStatuses();
	    }

		function updateSelectedStatuses() {
            var arrSelections = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');

			_selectedStatuses = arrSelections.join(',');
		}

        function updateSelectedAssigned() {

            var arrSelections = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

		    _selectedAssigned = arrSelections.join(',');
        }

        function clearSelectedAssigned(data) {
            _selectedAssigned = data;
            ddlAssignedChanged = true;
        }

        function ddlAssigned_change() {
	        updateSelectedAssigned();
	    }

	    function updateSelectedAssigned() {

            var arrSelections = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

		    _selectedAssigned = arrSelections.join(',');
        }

         function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 30;
            var addLeft = 0;

            if ($imgShowQuickFilters.is(':visible')) {
                $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

                var pos = $imgShowQuickFilters.position();
                $divQuickFilters.css({
                    width: '325px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideDown(function () { resizeFrame(); }).css('overflow','');
            }
            else if ($imgHideQuickFilters.is(':visible')) {
                $imgHideQuickFilters.hide();
                $imgShowQuickFilters.show();

                var pos = $imgHideQuickFilters.position();
                $divQuickFilters.css({
                    width: '325px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideUp(function () { resizeFrame(); }).css('overflow','');
            }
        }

         function initControls() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
            var ddlStatus = $('#<%=Master.FindControl("ms_Item0").ClientID %>');
            var ddlAssigned = $('#<%=Master.FindControl("ms_Item1").ClientID %>');

            ddlStatus.multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                }
                , onOpen: function () { ddlStatus_change(); }
                , onClose: function () { ddlStatus_change(); }
            }).change(function () { ddlStatus_change(); });
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Non-Resources']").wrapAll("<optgroup label='Non-Resources'>");
            $("#<%=Master.FindControl("ms_Item1").ClientID %> option[OptionGroup='Resources']").wrapAll("<optgroup label='Resources'>");
            ddlAssigned.multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
                }
                , onOpen: function () { ddlAssigned_change(); }
                , onClose: function () { ddlAssigned_change(); }
            }).change(function () { ddlAssigned_change(); });

            //chkBusinessReview.on('change', function () { chkBusinessReview_change(this); });
            lblMessage.hide();
	    }


        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
            initControls();
            $('#tdQuickFilters').show();
            $('#trms_Item0').show();
            $('#trms_Item1').show();
            $('#trClearAll').hide();

            $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });
            if (parent.resizeFrame) {
                parent.resizeFrame('Tasks');
            }
            if ('<%=this.WorkItemID%>' != "0") {
                resizeFrame();
                $('#btnViewReleaseHistory').hide();
                $('#btnViewChangeHistory').hide();
                $('#btnAdd').hide();
                $('#btnDelete').hide();
            }
        });
    </script>
</asp:Content>
