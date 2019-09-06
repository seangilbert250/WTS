﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_SystemSuite_WorkArea.aspx.cs" Inherits="MDGrid_SystemSuite_WorkArea" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Work Area Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
                 Work Area (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;" >
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                     height="15" style="cursor: pointer; margin-right: 10px; float: right;" />
            </td>   
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
			<td style="padding-left: 5px;">
                <asp:DropDownList ID="ddlTaskStatus" runat="server" TabIndex="1" Style="width: 150px;">               
                    <asp:ListItem Text="Open Work Tasks Only" Value="0" />
					<asp:ListItem Text="All Work Tasks" Value="1" />
				</asp:DropDownList>
            </td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonNew" value="Add" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonRemove" value="Delete" disabled="disabled"/>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

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
    <div id="divAddWorkAreasPopup" runat="server" style="position:absolute; display:none; background-color:#ffffff;">
        <div class="pageContentInfo" style="text-align: right;">
            <input type="button" id="btnSaveWorkAreaSelections" value="Save Work Area" style="margin-left:3px;margin-top:3px;" />
            &nbsp;<input type="button" id="btnCloseWorkAreaSelections" value="Close" style="margin-top:3px;" />
        </div>
        <div style="padding: 3px;">
            <span>Work Area: </span>
            <asp:DropDownList ID="ddlWorkArea" runat="server" Width="80%" style="padding: 3px; float: right; background-color: #f5f6ce;"></asp:DropDownList>
        </div>
        <div style="padding: 20px 3px;">
            <span>Systems: </span>
            <div style="width: 80%; float: right;">
                <wts:MultiSelect runat="server" ID="msWorkAreaSystems"
                ItemLabelColumnName="WTS_SYSTEM"
                ItemValueColumnName="WTS_SYSTEMID"
                IsOpen="true"
                KeepOpen="true"
                Width="100%"
                MaxHeight="330"
                HideDDLButton="true"
                SelectAll="true"
                />
            </div>
            
         </div>
    </div>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

        var _pageURLs = new PageURLs();
        var _idxDelete = 0, _idxID = 0, _currentLevel = 0, _selectedID = 0;
        var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';
        var imgHelp = document.getElementById("imgHelp");
	</script>

	<script id="jsAJAX" type="text/javascript">

        function GetColumnValue(row, ordinal, blnoriginal_value) {
            try {
                var tdval = $(row).find('td:eq(' + ordinal + ')');
                var val = '';
                if ($(tdval).length == 0) { return ''; }

                if ($(tdval).children.length > 0) {
                    if ($(tdval).find("select").length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find("select").attr('original_value');
                        }
                        else {
                            val = $(tdval).find("select").val();
                        }
                    }
                    else if ($(tdval).find('input[type=checkbox]').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('input[type=checkbox]').parent().attr("original_value");
                        }
                        else {
                            if ($(tdval).find('input[type=checkbox]').prop('checked')) { val = '1'; }
                            else { val = '0'; }
                        }
                    }
                    else if ($(tdval).find('input[type=text]').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('input[type=text]').attr('original_value');

                        }
                        else {
                            val = $(tdval).find('input[type=text]').val();
                        }
                    }
                    else if ($(tdval).find('input[type=number]').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('input[type=number]').attr('original_value');

                        }
                        else {
                            val = $(tdval).find('input[type=number]').val();
                        }
                    }
                    else if ($(tdval).find('input').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('input').attr('original_value');

                        }
                        else {
                            val = $(tdval).find('input').val();
                        }
                    }
                    else {
                        val = $(tdval).text();
                    }

                }
                else {
                    val = $(tdval).text();
                }
                return val;
            } catch (e) { return ''; }
        }

        function save() {
            try {
                var changedRows = [];
                var id = 0;
                var original_value = '', name = '', description = '', sortOrder = '', archive = '';

                $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
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
                                changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
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
                    ShowDimmer(true, "Updating...", 1);
                    var json = '[' + changedRows.join(",") + ']';
                    PageMethods.SaveChanges(json, '<%=this._qfSystemSuiteID%>', save_done, on_error);
                }
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
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
                    popupManager.RemovePopupWindow('AddWorkAreaToSuite');
                }
                else {
                    MessageBox('Failed to save items. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function removeItem(itemId) {
            try {
                ShowDimmer(true, "Removing Systems...", 1);

                PageMethods.RemoveItem(parseInt(itemId), <%=this._qfSystemSuiteID%>, removeItem_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to remove systems.\n' + e.message);
            }
        }
        function removeItem_done(result) {
            ShowDimmer(false);

            var removed = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
                        removed = true;
                    }
                    if (obj.id) {
                        id = obj.id;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (removed) {
                    MessageBox('All Systems have been removed from this item.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to remove systems from this item. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function on_error(result) {
            ShowDimmer(false);

            var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

            MessageBox('save error:  \n' + resultText);
        }

	</script>

	<script id="jsEvents" type="text/javascript">

        function refreshPage() {
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'RefData', 1);
            qs = editQueryStringValue(qs, 'TaskStatus', $('#<%=this.ddlTaskStatus.ClientID %> option:selected').val());

            document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function imgExport_click() {
            var url = window.location.href;
            url = editQueryStringValue(url, 'Export', true);

            window.open('Loading.aspx?Page=' + url);
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

        function buttonNew_click() {
            var nWindow = 'AddWorkAreaToSuite';
            var nTitle = 'Add Work Area';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divAddWorkAreasPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function btnSaveWorkAreaSelections_click() {
            var arrSystems = msWorkAreaSystems_getSelections(true);
            var workArea = top.$('#<%=this.ddlWorkArea.ClientID%>').val();

            if (workArea == 0) {
                dangerMessage('Please select a Work Area', null, true, $(popupManager.GetPopupByName('AddWorkAreaToSuite').Window).find('[id*=divAddWorkAreasPopup]')[0]);
            } else if (arrSystems.length == 0) {
                dangerMessage('Please select a least one System', null, true, $(popupManager.GetPopupByName('AddWorkAreaToSuite').Window).find('[id*=divAddWorkAreasPopup]')[0]);
            }
            else {
                ShowDimmer(true, "Saving...", 1);
                var json = arrSystems.join(",");
                PageMethods.SaveWorkArea(json, workArea, '<%=this._qfSystemSuiteID%>', save_done, on_error);
            }
        }

        function closeCloseWorkAreaSelections() {
            popupManager.RemovePopupWindow('AddWorkAreaToSuite');
        }

        function buttonRemove_click() {
            if (!_selectedId || _selectedId == '' || _selectedId == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will remove all systems from this item.' + '\n' + 'Do you wish to continue?')) {
                removeItem(_selectedId);
            }
        }

        function buttonSave_click() {
            save();
        }

        function openTaskGrid(workAreaID) {
            var filters = '{"System Suite":{"value":"' + <%=this._qfSystemSuiteID%>+'"},"Work Area":{"value":"' + workAreaID + '"}}';

            PageMethods.SetFilterSession(false, false, "Work", filters, false, function () {
                var nWindow = 'WorkTasks';
                var nTitle = 'Work Tasks';
                var nHeight = 800, nWidth = 1400;
                var nURL = _pageUrls.Maintenance.WorkItemContainer + '?&MyData=false&SelectedStatuses=80,72,1,2,3,4,5,6,7,8,9,11,12,13';
                if ($('#<%=this.ddlTaskStatus.ClientID %> option:selected').val() == 1) nURL += ',10';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }, on_error);
        }

        function openRQMTGrid(workAreaID) {
            var filters = '{"System Suite":{"value":"' + <%=this._qfSystemSuiteID%>+'"},"Work Area":{"value":"' + workAreaID + '"}}';

            PageMethods.SetFilterSession(false, false, "RQMT", filters, false, function () {
                var nWindow = 'RQMTs';
                var nTitle = 'RQMTs';
                var nHeight = 800, nWidth = 1425;
                var nURL = _pageUrls.Maintenance.RQMTContainer + '?&GridType=RQMT&MyData=true';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }, on_error);
        }

        function row_click(row) {
            if ($(row).attr('itemID')) {
                _selectedId = $(row).attr('itemID');
                $('#buttonRemove').attr('disabled', false);
            }
        }

        function activateSaveButton(sender) {
            if (_canEdit) {
                $('#buttonSave').attr('disabled', false);
                $('#buttonSave').prop('disabled', false);
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

        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

	</script>

	<script id="jsInit" type="text/javascript">

        function initVariables() {
            try {
                _pageUrls = new PageURLs();

                _canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
                _canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
                _isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

                if (_dcc[0] && _dcc[0].length > 0) {
                    _idxID = +'<%=this.DCC == null ? 0 : this.DCC["WorkAreaID"].Ordinal %>';
                }
            } catch (e) {

            }
        }

        $(document).ready(function () {

            initVariables();
            $('#buttonRemove').show();

            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');

            $('#imgReport').hide();
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
            if (_canEdit) {
                $('input:text').on('change keyup mouseup', function () { txt_change(this); });
                $('input:checkbox').on('change', function () { txt_change(this); });
                $('input').on('change keyup mouseup', function () { txt_change(this); });

                $('#buttonNew').attr('disabled', false);
                $('#buttonNew').click(function (event) { buttonNew_click(); return false; });

                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
                $('#buttonRemove').click(function (event) { buttonRemove_click(); return false; });

                $('#btnSaveWorkAreaSelections').click(function (event) { btnSaveWorkAreaSelections_click(); return false; });
                $('#btnCloseWorkAreaSelections').on('click', function () { closeCloseWorkAreaSelections(); });
            }

            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });

            $('#<%=this.ddlTaskStatus.ClientID %>').change(function () { refreshPage(); });

            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
            msWorkAreaSystems_init();
            $('.ms-drop, .bottom').css('width', '100%');
            resizeFrame();
        });
	</script>

</asp:Content>

