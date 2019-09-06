﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMTDefectsImpact_Grid.aspx.cs" Inherits="RQMTDefectsImpact_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <span id="spnView" style="display: none;">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Gridiew: 
        <asp:DropDownList ID="ddlGridview" runat="server" Style="font-size: 12px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
        <asp:HiddenField ID="itisettings" runat="server" />
    </span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="buttonNew" value="Add" style="vertical-align: middle;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle;" />
                <input type="button" id="btnClose" value="Close" style="vertical-align:middle;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <iframe id="frmDownload" style="display: none;"></iframe>
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsEvents" type="text/javascript">
        var _pageURLs = new PageURLs();
        var _idxDelete = 0, _idxID = 0, _idxDescription = 0, _idxVerified = 0, _idxResolved = 0, _idxContinueToReview = 0, _idxImpactID = 0;
        var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';

        function buttonNew_click() {
            var grdData = <%=this.grdData.ClientID%>;
			
            var nRow = grdData.Body.Rows[0].cloneNode(true);
            $(nRow.cells[_idxID]).text('0');//.innerText = '0';
            $(nRow.cells).each(function (i, td) {
                if ($(td).find('textarea').length > 0) {
                    $(td).find('textarea').attr('original_value', '');
                    $(td).find('textarea').text('');
                    $(td).find('textarea').val('');
                }
                else if ($(td).find('input:checkbox').length > 0) {
                    $(td).find('input:checkbox').attr('original_value', '');
                    $(td).find('input:checkbox').attr('checked', false);
                    $(td).find('input:checkbox').prop('checked', false);
                }
                else if ($(td).children('input').length > 0) {
                    $(td).find('input').attr('original_value', '');
                    $(td).find('input').text('');
                    $(td).find('input').val('');
                }
                else if ($(td).children('select').length > 0) {
                    $(td).find('select').attr('original_value', '');
                }
                else {
                    $(td).html('&nbsp;');
                }
            });

            $(nRow).attr('fieldChanged', true);
            $(nRow).find('span[dateinfo]').remove();
            $(nRow).find('td[srcontainerlinkcell=1]').css('cursor', 'default');
            $(nRow).find('td[taskcontainerlinkcell=1]').css('cursor', 'default');
            grdData.Body.Rows[0].parentNode.insertBefore(nRow, grdData.Body.Rows[0]);
            //add delete button
            $(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
            $(nRow).show();
            resizeFrame();
        }

        function deleteNewRow(img) {
            $(img).closest('tr').remove();
            resizeFrame();
        }

        function imgDelete_click(itemID) {
            if (!itemID || itemID == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            //if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
            //    deleteItem(itemID, item);
            //}
        }

        function row_click(row) {
            if ($(row).attr('itemID')) {
                _selectedId = $(row).attr('itemID');
            }
        }

        function resizeGrid() {
            setTimeout(function () { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }

        function activateSaveButton(sender) {
            if (_canEdit) {
                $('#btnSave').attr('disabled', false);
                $('#btnSave').prop('disabled', false);
                $(sender).closest('tr').attr('fieldChanged', true);
            }
        }

        function txt_change(sender) {
            var original_value = '', new_value = '';
            if ($(sender).attr('original_value')) {
                original_value = $(sender).attr('original_value');
            }

            new_value = $(sender).val();

            if (new_value != original_value) {
                activateSaveButton(sender);
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var changedRows = [];
                    var id = 0;
                    var original_value = '', name = '', description = '', sortOrder = '', archive = '';

                    $('.gridBody, .selectedRow', $('#<%=this.grdData.ClientID%>_Grid')).each(function (i, row) {
                    var changedRow = [];
                    var changed = false;

                    if (_dcc[0].length > 0 && $(this)[0].hasAttribute('fieldChanged')) {
                        for (var i = 0; i <= _dcc[0].length - 1; i++) {
                            var newval = GetColumnValue(row, i);
                            var oldval = GetColumnValue(row, i, true);
                            if (newval != oldval) {
                                changed = true;
                                break;
                            }
                        }
                        if (changed) {
                            for (var i = 0; i <= _dcc[0].length - 1; i++) {
                                changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + GetColumnValue(row, i) + '"');
                            }
                            var obj = '{' + changedRow.join(',') + '}';
                            changedRows.push(obj);
                        }
                    }
                    });

                    if (changedRows.length == 0) {
                        MessageBox('You have not made any changes');
                    }
                    else {
                        ShowDimmer(true, "Saving...", 1);
                        //var json = '{update:' + JSON.stringify(changedRows) + '}';
                        var json = '[' + changedRows.join(",") + ']';
                        PageMethods.SaveChanges(json, RQMT_ID, SYSTEM_ID, save_done, on_error);
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            try {
                ShowDimmer(false);

                var saved = false;
                var ids = '', errorMsg = '';

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.ids) {
                        ids = obj.ids;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('Items have been saved.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to save items. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function validate() {
            var validation = [];

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="Impact"]'), function () {
                var nText = $(this).val();

                if (nText.length == 0) {
                    if ($.inArray('Impact cannot be empty.', validation) == -1) validation.push('Impact cannot be empty.');
                }
            });

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table textarea[field="Description"]'), function () {
                var nText = $(this).val();
                if (nText.length == 0) {
                    if ($.inArray('Description cannot be empty.', validation) == -1) validation.push('Description cannot be empty.');
                }
            });

            return validation.join('<br>');
        }

        function imgDelete_click(itemID) {
            if (!itemID || itemID == '' || itemID == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
                deleteItem(itemID);
            }
        }

        function deleteItem(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);

                PageMethods.DeleteItem(itemId, deleteItem_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }

        function deleteItem_done(result) {
            ShowDimmer(false);

            var deleted = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
                        deleted = true;
                    }
                    if (obj.id) {
                        id = obj.id;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (deleted) {
                    MessageBox('Item has been deleted.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to delete item. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function on_error(result) {
            ShowDimmer(false);
            var resultText = 'An error occurred when communicating with the server';
            MessageBox('save error:  \n' + resultText);
        }

        function updateOpenNodes() {
            var open = '';
            var openNodes = $('[id$=_BodyContainer]').find('tr[srcontainerrow=1]:visible');
            for (var i = 0; i < openNodes.length; i++) {
                var node = $(openNodes[i]);
                if (open != '') open += ',';
                open += node.attr('itemID');
            }
            
            defaultParentPage._expandedGridNodes['rqmtdefectimpactgridsrs'] = open;

            open = '';
            openNodes = $('[id$=_BodyContainer]').find('tr[taskcontainerrow=1]:visible');
            for (var i = 0; i < openNodes.length; i++) {
                var node = $(openNodes[i]);
                if (open != '') open += ',';
                open += node.attr('itemID');
            }
            
            defaultParentPage._expandedGridNodes['rqmtdefectimpactgridtasks'] = open;
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            updateOpenNodes();

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function closeDialog() {
            var thePopup = popupManager.GetPopupByName('RQMTDefects');
            thePopup.Close();
        }

        function srHeaderCellClicked(headercell) {
            var itemID = $(headercell).attr('itemid');

            var srcell = $('[id$=_BodyContainer]').find('td[srcontainerlinkcell=1][itemid=' + itemID + ']');
            toggleSRContainerRow(srcell);            
        }

        function taskHeaderCellClicked(headercell) {
            var itemID = $(headercell).attr('itemid');

            var taskcell = $('[id$=_BodyContainer]').find('td[taskcontainerlinkcell=1][itemid=' + itemID + ']');
            toggleTaskContainerRow(taskcell);   
        }

        function toggleSRContainerRow(srcell, forceToggle) {
            srcell = $(srcell);

            if (srcell.text() == 'ADD' && (forceToggle == null || !forceToggle)) {
                var theTR = srcell.closest('tr');
                var theTable = theTR.closest('table');
                var srTR = theTR.next();

                if (srTR.is(':visible')) {
                    srTR.toggle();                    
                }

                openSRGridSearch(srcell);
            }
            else {
                var theTR = srcell.closest('tr');
                var theTable = theTR.closest('table');
                var srTR = theTR.next();

                srTR.toggle();
            }

            updateOpenNodes();
        }

        function toggleTaskContainerRow(taskcell, forceToggle) {
            taskcell = $(taskcell);

            if (taskcell.text() == 'ADD' && (forceToggle == null || !forceToggle)) {
                var theTR = taskcell.closest('tr');
                var theTable = theTR.closest('table');
                var taskTR = theTR.next().next(); // we are two rows after the defect row

                if (taskTR.is(':visible')) {
                    taskTR.toggle();                    
                }

                openTaskGridSearch(taskcell);
            }
            else {
                var theTR = taskcell.closest('tr');
                var theTable = theTR.closest('table');
                var taskTR = theTR.next().next(); // we are two rows after the defect row

                taskTR.toggle();
            }

            updateOpenNodes();
        }

        function openSRGridSearch(src) {
            var itemID = $(src).attr('itemid');
            if (itemID > 0) {
                activeSearchRSDID = itemID;
            }
            else {
                activeSearchRSDID = $(src).parent().attr('itemid');
            }
            
            var nTitle = 'SR Search';
            var nHeight = 650, nWidth = 1200;
            var nURL = 'SR_Grid.aspx?Mode=Search&Systems=<%=HttpUtility.UrlEncode(this.matchAORSRWebSystems)%>&StatusIDs=' + escape('122,123,124');
            var openPopup = popupManager.AddPopupWindow('SRSearch', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
            openPopup.onClose = SRGridSearchClosed;
        }

        function SRGridSearchClosed() {
            activeSearchRSDID = 0;
        }

        function SRSelected(srid, srexternal) {
            var rsdid = activeSearchRSDID;

            var open = defaultParentPage._expandedGridNodes['rqmtdefectimpactgridsrs'];

            if (open == null || open.indexOf(rsdid) == -1) {
                var srcell = $('[id$=_BodyContainer]').find('tr[itemid=' + rsdid + ']').find('td[srcontainerlinkcell=1]');
                toggleSRContainerRow(srcell, true);
            }

            PageMethods.SRSelected(rsdid, srid, srexternal, SRSelected_done, on_error)

            var thePopup = popupManager.GetPopupByName('SRSearch');
            thePopup.Close();
        }

        function SRSelected_done(result) {
            refreshPage();
        }

        function SRDelete(img) {
            img = $(img);
            var rsdsrid = img.attr('rsdsrid');
            
            QuestionBox('Confirm SR Delete', 'Are you sure you want to delete this SR?', 'Yes,No', 'SRDelete_confirmed', 300, 300, this, rsdsrid);
        }

        function SRDelete_confirmed(answer, params) {            
            if (answer == 'Yes') {
                PageMethods.SRDelete(params, SRDelete_done, on_error);
            }
        }

        function SRDelete_done(result) {
            refreshPage();
        }

        function SRNumber_Clicked(srid) {
            var obj = parent;

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('SR', false, srid, true);
            }
            else {
                var nWindow = 'SR';
                var nTitle = 'SR';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.SREdit + window.location.search + '&NewSR=false&SRID=' + srid;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function openTask(parentTaskID, subTaskID, subTaskNumber) {
            var nWindow;
            var h = 700, w = 850;
            if (subTaskID) {
                nWindow = 'WorkSubTask';
                var title = 'Subtask - [' + parentTaskID + ' - ' + subTaskNumber + ']';
                var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.TaskEdit
                    + window.location.search //will have WorkItemID
                    + '&workItemID=' + parentTaskID
                    + '&newTask=0'
                    + '&taskID=' + subTaskID;
            } else {
                nWindow = 'WorkTask';
                w = 1400;
                var title = 'Primary Task - [' + parentTaskID + ']';
                var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.WorkItemEditParent
                    + window.location.search //will have WorkItemID
                    + '&workItemID=' + parentTaskID
                    + '&UseLocal=True';
            }


            var openPopup = popupManager.AddPopupWindow(nWindow, title, url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

        function openTaskGridSearch(src) {
            var itemID = $(src).attr('itemid');
            if (itemID > 0) {
                activeSearchRSDID = itemID;
            }
            else {
                activeSearchRSDID = $(src).parent().attr('itemid');
            }

            selectTask(activeSearchRSDID);
        }

	    function selectTask(activeSearchRSDID) {
	        var editable = '<%=this.CanEdit %>'.toUpperCase() == 'TRUE';
            if (!editable) return;

	        var aorID = 0;
	        var aorName = '';

	        var nWindow = 'SelectSubTask';
	        var nTitle = 'Select Sub-Task';
	        var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + '?GridType=AOR&MyData=false&NewAOR=false&AORID=0&AORReleaseID=0&Type=SubTask&SubType=SelectSubTask&SelectCallback=subTaskSelected&SelectedTasks=&TaskID=&SelectedStatuses=0,1,2,3,4,5,6,7,8,9,11,12,13,64&HideAdd=true';
            nURL += '&SelectedSystems=<%=SYSTEM_ID%>';

	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
            openPopup.onClose = selectTask_closed;
        }

        function selectTask_closed() {            
            activeSearchRSDID = 0;
        }

        function subTaskSelected(data) {
            var rsdid = activeSearchRSDID;
            var taskarr = data[0].tasknumber.split('.'); // worktaskid.worktasknumber
                        
            var open = defaultParentPage._expandedGridNodes['rqmtdefectimpactgridtasks'];

            if (open == null || open.indexOf(rsdid) == -1) {
                var srcell = $('[id$=_BodyContainer]').find('tr[itemid=' + rsdid + ']').find('td[taskcontainerlinkcell=1]');
                toggleTaskContainerRow(srcell, true);
            }

            PageMethods.TaskAddedToDefect(rsdid, taskarr[0], subTaskSelected_done, on_error)
        }

        function subTaskSelected_done(result) {
            refreshPage();
        }

        function TaskDelete(img) {
            img = $(img);
            var rsdtaskid = img.attr('rsdtaskid');
            var tasknumber = img.closest('td').next().text();
            
            QuestionBox('Confirm Task Delete', 'Are you sure you want to delete task ' + tasknumber + ' from defect?', 'Yes,No', 'TaskDelete_confirmed', 300, 300, this, rsdtaskid);
        }

        function TaskDelete_confirmed(answer, params) {            
            if (answer == 'Yes') {
                PageMethods.DeleteTaskFromDefect(params, deleteTaskFromDefect_done, on_error);
            }
        }

        function deleteTaskFromDefect_done() {
            refreshPage();
        }

    </script>
    <script id="jsInit" type="text/javascript">
        var RQMT_ID = '<%= this.RQMT_ID %>';
        var SYSTEM_ID = '<%= this.SYSTEM_ID %>';
        var activeSearchRSDID = 0;
        var activeRSDSRID = 0;

        function initVariables() {
            try {
                _pageUrls = new PageURLs();

                _canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
                _canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
                _isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

                if (_dcc[0] && _dcc[0].length > 0) {
                    _idxDelete = parseInt('<%=this.DCC["X"].Ordinal %>');
                    _idxID = parseInt('<%=this.DCC["RQMTSystemDefectID"].Ordinal %>');
                    _idxDescription = parseInt('<%=this.DCC["Description"].Ordinal %>');
                    _idxVerified = parseInt('<%=this.DCC["Verified"].Ordinal %>');
                    _idxResolved = parseInt('<%=this.DCC["Resolved"].Ordinal %>');
                    _idxContinueToReview = parseInt('<%=this.DCC["ContinueToReview"].Ordinal %>');
                    _idxImpactID = parseInt('<%=this.DCC["ImpactID"].Ordinal %>');
                }
            } catch (e) {

            }

            var thePopup = popupManager.GetPopupByName('RQMTDefects');
            thePopup.SetTitle('RQMT Defect(s) & Impact - RQMT #' + RQMT_ID + ' (' + '<%=this.systemName.Replace("'", "")%>' + ')');
        }

        $(document).ready(function () {
            initVariables();

            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');

            $('#imgReport').hide();
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnClose').click(function () { closeDialog(); });

            $('#buttonNew').click(function (event) { buttonNew_click(); return false; });

            $('textarea').on('change keyup mouseup', function () { txt_change(this); });
            $('input:text').on('change keyup mouseup', function () { txt_change(this); });
            $('input:checkbox').on('change', function () { txt_change(this); });
            $('select').on('change', function () { txt_change(this); });
            $('input').on('change keyup mouseup', function () { txt_change(this); });
            $('td[srcontainerlinkcell=1]').on('click', function () { toggleSRContainerRow(this); });
            $('td[taskcontainerlinkcell=1]').on('click', function () { toggleTaskContainerRow(this); });
            $('input[addsrbutton]').click(function () { openSRGridSearch(this); });
            $('input[addtaskbutton]').click(function () { openTaskGridSearch(this); });
            $('img[imgsrdelete=1]').click(function () { SRDelete(this); });
            $('img[imgtaskdelete=1]').click(function () { TaskDelete(this); });

            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });
                        
            var open = defaultParentPage._expandedGridNodes['rqmtdefectimpactgridsrs'];
            if (open != null && open.length > 0) {
                open = open.split(',');

                for (var i = 0; i < open.length; i++) {
                    var theContainerRow = $('[id$=_BodyContainer]').find('tr[srcontainerrow=1][itemID=' + open[i] + ']');

                    if (theContainerRow.find('td:contains("NO SRs found")').length == 0) {
                        theContainerRow.toggle();
                    }
                }
            } 

            open = defaultParentPage._expandedGridNodes['rqmtdefectimpactgridtasks'];
            if (open != null && open.length > 0) {
                open = open.split(',');

                for (var i = 0; i < open.length; i++) {
                    var theContainerRow = $('[id$=_BodyContainer]').find('tr[taskcontainerrow=1][itemID=' + open[i] + ']');

                    if (theContainerRow.find('td:contains("NO tasks found")').length == 0) {
                        theContainerRow.toggle();
                    }
                }
            } 

            resizeFrame();
        });

    </script>

</asp:Content>