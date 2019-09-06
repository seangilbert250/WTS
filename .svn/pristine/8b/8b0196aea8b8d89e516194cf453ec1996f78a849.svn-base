<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SR_Grid.aspx.cs" Inherits="SR_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">SR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table style="width: 100%; border-collapse: collapse;">
		<tr>
			<td>
                SR&nbsp;(<%=this.TotalCount %>):&nbsp;<%=this._srWorkloadPriority %>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="buttonGoToSR" value="Go to SR #" />
                <input type="text" id="txtSR" name="GoTo" tabindex="2" maxlength="11" size="8" style="margin-right: 5px;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnEdit" value="View/Edit" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <div id="divms" style="display:none;">
        <div>
            <div style="display:inline-block;width:75px;font-weight:bold;vertical-align:middle;">
                System:
            </div>
            <div style="display:inline-block;vertical-align:middle;">
                <wts:MultiSelect runat="server" ID="msWebsystems" Visible="false"
                    ItemLabelColumnName="Websystem"
                    ItemValueColumnName="Websystem"                        
                    CustomAttributes=""
                    IsOpen="false"
                    KeepOpen="false"
                    Width="100%"
                    MaxHeight="330"
                    HideDDLButton="false"
                    />
            </div>
        </div>
        <div style="margin-top:10px;">
            <div style="display:inline-block;width:75px;font-weight:bold;vertical-align:middle;">
                Status:
            </div>
            <div style="display:inline-block;vertical-align:middle;">
                <wts:MultiSelect runat="server" ID="msStatus" Visible="false"
                    ItemLabelColumnName="Status"
                    ItemValueColumnName="StatusID"                        
                    CustomAttributes=""
                    IsOpen="false"
                    KeepOpen="false"
                    Width="100%"
                    MaxHeight="330"
                    HideDDLButton="false"
                    IgnoreDefaultItems="true"
                    />                
            </div>
        </div>
        <div style="margin-top:10px;">
            <div style="display:inline-block;width:75px;font-weight:bold;vertical-align:middle;">
                Reasoning:
            </div>
            <div style="display:inline-block;vertical-align:middle;">
                <wts:MultiSelect runat="server" ID="msReasoning" Visible="false"
                    ItemLabelColumnName="SRType"
                    ItemValueColumnName="SRTypeID"                        
                    CustomAttributes=""
                    IsOpen="false"
                    KeepOpen="false"
                    Width="100%"
                    MaxHeight="330"
                    HideDDLButton="false"
                    IgnoreDefaultItems="true"
                    />                
            </div>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedSRID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnDelete_click() {
            QuestionBox('Confirm SR Delete', 'Are you sure you want to delete this SR?', 'Yes,No', 'confirmSRDelete', 300, 300, this);
        }

        function confirmSRDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteSR(_selectedSRID, delete_done, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox('SR has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnEdit_click() {
            var obj = parent;

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('SR', false, _selectedSRID, true);
            }
            else {
                var nWindow = 'SR';
                var nTitle = 'SR';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.SREdit + window.location.search + '&NewSR=false&SRID=' + _selectedSRID;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('input[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                        var $obj = $(this);

                        arrChanges.push({ 'typeName': $obj.attr('typeName'), 'typeID': $obj.attr('typeID'), 'field': $obj.attr('field'), 'value': $obj.val() });
                    });

                    if (arrChanges.length > 0) {
                        ShowDimmer(true, 'Saving...', 1);

                        var nJSON = '{update:' + JSON.stringify(arrChanges) + '}';

                        PageMethods.SaveChanges(nJSON, save_done, on_error);
                    }
                    else {
                        MessageBox('You have not made any changes.');
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
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                var fieldCount = $('input[fieldChanged="1"], select[fieldChanged="1"]').length;
                var rowCount = $('tr[rowChanged="1"]').length;

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function imgSort_click() {
            try {
                var sortableColumns = '<%=this.SortableColumns%>';
                while (sortableColumns.indexOf('<BR />') > -1) {
                    sortableColumns = sortableColumns.replace("<BR />", ' ');
                }
                while (sortableColumns.indexOf('<BR/>') > -1) {
                    sortableColumns = sortableColumns.replace("<BR/>", ' ');
                }
                while (sortableColumns.indexOf('<br />') > -1) {
                    sortableColumns = sortableColumns.replace("<br />", ' ');
                }
                while (sortableColumns.indexOf('<br/>') > -1) {
                    sortableColumns = sortableColumns.replace("<br/>", ' ');
                }

                while (sortableColumns.indexOf('...') > -1) {
                    sortableColumns = sortableColumns.replace('...', '');
                }

                while (sortableColumns.indexOf('<BR>') > -1) {
                    sortableColumns = sortableColumns.replace('<BR>', ' ');
                }
                while (sortableColumns.indexOf('<br>') > -1) {
                    sortableColumns = sortableColumns.replace('<br>', ' ');
                }

                var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
                var nPopup = popupManager.AddPopupWindow("Sorter", "Sort Grid", sURL, 200, 400, "PopupWindow", this.self);
                if (nPopup) {
                    nPopup.Open();
                }
            }
            catch (e) {
            }
        }

        function applySort(sortValue) {
            try {
                var pURL = window.location.href;
                pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
                pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

                window.location.href = 'Loading.aspx?Page=' + pURL;
            }
            catch (e) {
            }
        }

        function imgExport_click() {
            window.location.href += '&Export=1';
        }

        function validate() {
            var validation = [];

            if ($('#<%=this.grdData.ClientID %>_BodyContainer table select[field="Status"]:first option').length == 0 || $('#<%=this.grdData.ClientID %>_BodyContainer table select[field="Status"] option:selected[value="0"]').length > 0) validation.push('Please select a Status.');
            if ($('#<%=this.grdData.ClientID %>_BodyContainer table select[field="Reasoning"]:first option').length == 0 || $('#<%=this.grdData.ClientID %>_BodyContainer table select[field="Reasoning"] option:selected[value="0"]').length > 0) validation.push('Please select a Reasoning.');
            if ($('#<%=this.grdData.ClientID %>_BodyContainer table select[field="User\'s Priority"]:first option').length == 0 || $('#<%=this.grdData.ClientID %>_BodyContainer table select[field="User\'s Priority"] option:selected[value="0"]').length > 0) validation.push('Please select a User\'s Priority.');

            return validation.join('<br>');
        }

        function formatGoTo(obj) {
            var text = $(obj).val();

            if (/[^0-9]|^0+(?!$)/g.test(text)) {
                $(obj).val(text.replace(/[^0-9]|^0+(?!$)/g, ''));
            }
        }

        function buttonGoToSR_click() {
            var recordID = $('#txtSR').val();
            if (recordID.length > 0) {
                PageMethods.verifySRExists(recordID, verifySRExists_done);
            }
            else {
                MessageBox('Please enter a SR #.');
            }
        }

        function verifySRExists_done(result) {
            if (result) {
                _selectedSRID = $('#txtSR').val();

                if (<%=this.searchMode.ToString().ToLower()%>) {
                    refreshPage(true, _selectedSRID);
                }
                else {
                    btnEdit_click();
                }
            } else {
                MessageBox('Could not find SR # ' + $('#txtSR').val());
            }
        }

        function showText(txt) {
            alert(decodeURIComponent(txt));
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

        function row_click(obj) {
            if ($(obj).attr('sr_id')) {
                _selectedSRID = $(obj).attr('sr_id');
            }

            if ($(obj)[0].innerHTML.indexOf(">WTS<") > 0) {
                $('#btnDelete').prop('disabled', false);
                $('#btnEdit').prop('disabled', false);
            } else {
                $('#btnDelete').prop('disabled', true);
                $('#btnEdit').prop('disabled', true);
            }
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            switch($obj.attr('field')) {
                case 'Sort':
                    var nVal = $obj.val();
                    var blnNegative = nVal.indexOf('-') != -1 ? true : false;
                    
                    nVal = nVal.replace(/[^\d]/g, '');
                    
                    if (blnNegative) nVal = '-' + nVal;

                    $obj.val(nVal);
                    break;
            }

            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            switch($obj.attr('field')) {
                case 'Sort':
                    if (nVal == '-') $obj.val('');
                    return;
            }

            $obj.val($.trim(nVal));
        }

        function refreshPage(blnRetainPageIndex, goToSRID) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

            nURL = editQueryStringValue(nURL, 'ReasoningIDs', escape(msReasoning_getSelections().join(',')));

            var sys = msWebsystems_getSelections().join(',');
            if (sys.indexOf('R&D WTS') != -1) sys += ',WTS';
            nURL = editQueryStringValue(nURL, 'Systems', escape(sys));
            nURL = editQueryStringValue(nURL, 'StatusIDs', escape(msStatus_getSelections().join(',')));
            nURL = editQueryStringValue(nURL, 'GoToSRID', goToSRID != null ? goToSRID : '');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function ddl_change() {
            refreshPage(false);
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }

        function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 0;
            var addLeft = 0;
                addtop = 30;
                addLeft = 25;

            if ($imgShowQuickFilters.is(':visible')) {
                $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

                var pos = $imgShowQuickFilters.position();
                $divQuickFilters.css({
                    width: '425px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideDown();
            }
            else if ($imgHideQuickFilters.is(':visible')) {
                $imgHideQuickFilters.hide();
                $imgShowQuickFilters.show();

                var pos = $imgHideQuickFilters.position();
                $divQuickFilters.css({
                    width: '425px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideUp();
            }
        }

        function SRSelected(theTD) {
            var theTR = $(theTD).closest('tr');
            var srid = theTR.attr('sr_id');
            var srext = theTR.attr('sr_external');            

            var thePopup = popupManager.GetPopupByName('SRSearch');
            thePopup.Opener.SRSelected(srid, srext);
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            if ('<%=this.CanEditSR %>'.toUpperCase() == 'TRUE') {
                if ($('.saveable').length > 0) {
                    $('#btnDelete').show();
                    $('#btnSave').show();
                }
            }

            if ('<%=this.CanViewSR %>'.toUpperCase() == 'TRUE' && '<%=this.grdData.Rows.Count %>' != '0' && '<%=this.DCC.Contains("SR_ID") %>'.toUpperCase() == 'TRUE') $('#btnEdit').show();

            if (<%=this.searchMode.ToString().ToLower()%>) {
                $('#pageContentHeader').hide();
                $('#imgExport').hide();
                $('#btnDelete').hide();
                $('#btnSave').hide();
                //$('#buttonGoToSR').hide();
                //$('#txtSR').hide();

                msWebsystems_init();
                msStatus_init();
                msReasoning_init();

                var tdSearch = $('#divms');
                var tdQF = $('#trQFCustom').find('td');
                tdQF.html(tdSearch);  
                tdSearch.show();
                $('#trClearAll').hide();
                $('#trQFCustom').show();                
                $('#tdQuickFilters').show();                
            }

            $('#txtSR').val('<%=(this._goToSRID > 0 ? this._goToSRID.ToString() : "")%>');

            resizeGrid();
        }

        function initEvents() {
            $('#imgSort').click(function () { imgSort_click(); });
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#buttonGoToSR').click(function () { buttonGoToSR_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnEdit').click(function () { btnEdit_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });

            if (<%=this.searchMode.ToString().ToLower()%>) {
                $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });
                $('td[tdsrnum=1]').click(function () { SRSelected(this); });
            }

            $("#txtSR").keyup(function (event) {
                formatGoTo(this);

                if (event.keyCode === 13 || event.keyCode === 144) {
                    $('#buttonGoToSR').trigger('click');
                }
            });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>
