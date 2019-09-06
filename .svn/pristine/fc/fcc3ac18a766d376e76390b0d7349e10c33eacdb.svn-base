<%@  Title="" Language="C#" EnableViewState="false" EnableEventValidation="false" AutoEventWireup="true" CodeFile="MDGrid_Narrative.aspx.cs" Inherits="MDGrid_Narrative" Theme="Default" MasterPageFile="~/Grids.master" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Narrative Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>CR Report Narrative (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;">
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                    height="15" style="cursor: pointer; margin-right: 10px; float: right; display: none;" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <table id="tableQuickFilters" cellpadding="0" cellspacing="0">
        <tr>
            <td style="padding-left: 5px;">Contract:
				<asp:DropDownList ID="ddlQF_Contract" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 175px;">
                    <asp:ListItem Text="ALL" Value="0" />
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
                <input type="button" id="buttonCopy" value="Copy" style="display: none" disabled="disabled" />
                <input type="button" id="buttonSave" value="Save" disabled="disabled" />
                <input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <asp:HiddenField ID="hdnShowArchive" runat="server" EnableViewState="True" />
    <iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" EmptyDataText="No data...">
    </iti_Tools_Sharp:Grid>
    <div id="divNarrativeCopyPopup" runat="server" style="position: absolute; display: none; background-color: #ffffff;">
        <table class="pageContentInfo" cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
            <tr>
                <td style="text-align: right;">
                    <input type="button" id="buttonCopySave" value="Copy" />
                </td>
            </tr>
        </table>
        <table style="width: 100%;">
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 155px;">From Product Version:
                </td>
                <td id="releaseFromElement">
                    <asp:DropDownList ID="ddlReleaseFrom" runat="server" Width="250px" enabled="false" Style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td style="width: 5px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <span style="color: red;">*</span>
                </td>
                <td>Contract Narratives:
                </td>
                <td id="contractElement">
                    <asp:DropDownList ID="ddlContractCopy" runat="server" Width="250px" enabled="false" Style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td>&nbsp;
                </td>
            </tr>
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 155px;">To Product Version:
                </td>
                <td id="releaseToElement">
                    <asp:DropDownList ID="ddlReleaseTo" runat="server" Width="250px" Style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td style="width: 5px;">
                    &nbsp;
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
    <script src="Scripts/autosize.js"></script>
    <script id="jsVariables" type="text/javascript">

        var _pageURLs = new PageURLs();
        var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
        var _selectedId = 0;
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
                    else if ($(tdval).find('textarea').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('textarea').attr('original_value');

                        }
                        else {
                            val = $(tdval).find('textarea').val();
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
                var valid = validate();
                if (valid.length == 0) {
                    var changedNarrRows = [];
                    var id = 0;
                    var original_value = '', name = '', description = '', sortOrder = '', archive = '';

                    $('[id*="frameChild"]').contents().find('iframe').contents().find('.gridBody, .selectedRow').each(function (i, row) {
                        var changedRow = [];
                        var changed = false;
                        
                        if (_narr_dcc[0].length > 0 && $(this)[0].hasAttribute('fieldChanged')) {
                            for (var i = 0; i <= _narr_dcc[0].length - 1; i++) {
                                var newval = GetColumnValue(row, i);
                                var oldval = GetColumnValue(row, i, true);
                                if (newval != oldval) {
                                    changed = true;
                                    break;
                                }
                            }
                            if (changed) {
                                for (var i = 0; i <= _narr_dcc[0].length - 1; i++) {
                                    if (_narr_dcc[0][i].ColumnName === "NarrativeID") {
                                        changedRow.push('"' + _narr_dcc[0][i].ColumnName + '":"' + $(row).attr('itemid') + '"');
                                    } else {
                                        changedRow.push('"' + _narr_dcc[0][i].ColumnName + '":"' + encodeURIComponent(GetColumnValue(row, i)) + '"');
                                    }
                                }
                                changedRow.push('"ProductVersionID":"' + $(this).attr('ProductVersionID') + '"');
                                changedRow.push('"ContractID":"' + $(this).attr('ContractID') + '"');

                                var obj = '{' + changedRow.join(',') + '}';
                                changedNarrRows.push(obj);
                            }
                        }
                    });

                    if (changedNarrRows.length == 0) {
                        MessageBox('You have not made any changes');
                    }
                    else {
                        ShowDimmer(true, "Updating...", 1);
                        var json = '[' + changedNarrRows.join(",") + ']';
                        PageMethods.SaveNarrativeChanges(json, save_done, on_error);
                    }
                } else {
                    MessageBox('Invalid entries: <br><br>' + valid);
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

                if (errorMsg.length > 0) {
                    MessageBox('Failed to save items. \n' + errorMsg);
                    refreshPage();
                }
                else {
                    MessageBox('Items have been saved.');
                    refreshPage(true);
                }
            } catch (e) { }
        }

        function validate() {
            var validation = [];
            var $contractSort = $('[id*="frameChild"]').contents().find('iframe').contents().find('iframe').contents().find('.gridBody:visible, .selectedRow:visible');
            var blnExit = false;

            $.each($contractSort, function () {
                if (blnExit) return false;
                if ($(this).find('select[id*="ddlCONTRACT"]').val()) {
                    var productVersion = $(this).attr('productversionid');
                    var contract = $(this).find('select[id*="ddlCONTRACT"]').val();
                    var contractSort = $(this).find('select[id*="ddlSort"]').val();

                    $.each($contractSort.not($(this)), function () {
                        if ($(this).attr('productversionid') === productVersion
                            && $(this).find('select[id*="ddlCONTRACT"]').val() === contract
                            && $(this).find('select[id*="ddlSort"]').val() == contractSort) {
                            validation.push("A contract's image location cannot have duplicates.");
                            blnExit = true;
                            return false;
                        }
                    });
                }
            });

            return validation.join('<br>');
        }

        function deleteItem(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);

                if (<%=_currentLevel%> == 1) {
                    PageMethods.DeleteItem(-1, parseInt(itemId), <%=this._productVersionID%>, deleteItem_done, on_error);
                } else {
                    PageMethods.DeleteItem(parseInt(itemId), <%=this._qfContractID%>, <%=this._productVersionID%>, deleteItem_done, on_error);
                }
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
                    if (typeof parent != undefined && $.isFunction(parent.refreshPage)) parent.refreshPage(true);
                    else refreshPage(true);
                }
                else {
                    MessageBox('Failed to delete item. \n' + errorMsg);
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

        function refreshPage(parent) {
            if (parent && typeof parent.refreshPage === 'function') {
                parent.refreshPage(true);
            }
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'ContractID', $('#<%=this.ddlQF_Contract.ClientID %> option:selected').val());
            qs = editQueryStringValue(qs, 'ShowArchive', $('#<%=Master.FindControl("chkBoxArchive").ClientID %>')[0].checked);
            document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function imgExport_click() {
            window.location.href += '&Export=1';
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

        function ddl_contract_change() {
            refreshPage();
        }

        function buttonNew_click() {
            var title = '', url = '';
            var h = 625, w = 450;

            title = 'Add CR Report Narrative';
            url = _pageUrls.Maintenance.NarrativeAdd
            url += '?ProductVersionID=' + <%=this._productVersionID%>
            url += '&ContractID=' + '<%=this._qfContractID%>';

            //open in a popup
            var openPopup = popupManager.AddPopupWindow('CRReportNarrative', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

        function buttonDelete_click() {
            if (!_selectedId || _selectedId == '' || _selectedId == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
                deleteItem(_selectedId);
            }
        }
        function imgDelete_click(itemID, item) {
            if (!itemID || itemID == '' || itemID == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
                deleteItem(itemID, item);
            }
        }

        function buttonSave_click() {
            save();
        }

        function buttonCopy_click() {
            var nWindow = 'CopyNarrative';
            var nTitle = 'Copy Narratives';
            var nHeight = 100, nWidth = 450;
            var nURL = null;

            $('#<%=this.ddlContractCopy.ClientID%>').val(_selectedId);
            $('#<%=this.ddlReleaseFrom.ClientID%>').val(<%=this._productVersionID%>);

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divNarrativeCopyPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function buttonCopySave_click() {
            try {
                var copyRelease = $(popupManager.GetPopupByName('CopyNarrative').Window).find('[id*=ddlReleaseTo]').val();
                var copyContract = $(popupManager.GetPopupByName('CopyNarrative').Window).find('[id*=ddlContractCopy]').val();

                if (copyRelease > 0 && copyContract.length > 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    PageMethods.Copy(<%=this._productVersionID%>, copyRelease, copyContract, copy_done, on_error);
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

        function copy_done(result) {
            try {
                ShowDimmer(false);

                var saved = false;
                var exists = false;
                var ids = '', errorMsg = '';

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.exists && obj.exists.toUpperCase() == 'TRUE') {
                        exists = true;
                    }
                    if (obj.ids) {
                        ids = obj.ids;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('Narratives has been copied.');
                    $(popupManager.RemovePopupWindow('CopyNarrative'));
                    parent.refreshPage(true);
                } else if (exists) {
                    MessageBox('Failed to copy Narrative. Narrative already exists in selected Release\n');
                }
                else {
                    MessageBox('Failed to copy Narrative. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function row_click(row) {
            if ($(row).attr('itemID')) {
                _selectedId = $(row).attr('itemID');
                $('#buttonDelete').attr('disabled', false);
                $('#buttonCopy').attr('disabled', false);
            }
        }

        function activateSaveButton(sender) {
            if (_canEdit) {
                if (parent.$('#buttonSave')) {
                    parent.$('#buttonSave').attr('disabled', false);
                    parent.$('#buttonSave').prop('disabled', false);
                }
                if (parent.parent.$('#buttonSave')) {
                    parent.parent.$('#buttonSave').attr('disabled', false);
                    parent.parent.$('#buttonSave').prop('disabled', false);
                }
                $(sender).closest('tr').attr('fieldChanged', true);
            }
        }

        function txt_change(sender) {
            autosize($(sender));
            var original_value = '', new_value = '';
            if ($(sender).attr('original_value')) {
                original_value = $(sender).attr('original_value');
            }

            new_value = $(sender).val();

            if (new_value != original_value) {
                activateSaveButton(sender);
            }
        }

        function ddl_change(ddl) {
            var value = '', originalValue = '';
            value = $('option:selected', $(ddl)).text();
            sysId = $('option:selected', $(ddl)).val();

            if ($(ddl).attr("original_value")) {
                originalValue = $(ddl).attr("original_value");
            }

            if (value != originalValue) {
                //$(ddl).attr("original_value", value);
                activateSaveButton(ddl);
            }
        }

        function imgShowHideChildren_click(sender, direction, id) {
            try {
                if (id == "0" || id == "ALL") {
                    var itemId = '0';

                    $('[Name="img' + direction + '"]').each(function () {
                        itemId = $(this).attr('itemId');
                        if (itemId != "ALL") imgShowHideChildren_click(this, direction, itemId);
                    });

                    $(sender).hide();
                    $(sender).siblings().show();
                } else {
                    if (direction.toUpperCase() == "SHOW") {
                        //show row/div with child grid frame
                        //get frame and pass url(if necessary)
                        var td;

                        $(sender).closest('tr').each(function () {
                            var currentRow = $(this);
                            var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
                            $(row).show();

                            if (<%=this._currentLevel%> === 0) {
                                td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("ProductVersion")) ? 0 : this.DCC["ProductVersion"].Ordinal %>)', row)[0];
                            } else {
                                td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("Contract")) ? 0 : this.DCC["Contract"].Ordinal %>)', row)[0];
                            }

                            loadChildGrid(td, id);
                        });
                    }
                    else {
                        $('tr[Name="gridChild_' + id + '"]').hide();
                    }

                    $(sender).hide();
                    $(sender).siblings().show();
                    resizeFrame();
                }
            } catch (e) {
                var msg = e.message;
            }
        }
        //TODO Handle code below with child
        function loadChildGrid(td, id) {
            var url = 'Loading.aspx?Page=';

            switch (<%=this._currentLevel%>) {
                case 0:
                    url += 'MDGrid_Narrative.aspx';
                    url += '?CurrentLevel=' + 1;
                    url += '&ProductVersionID=' + id;
                    url += '&ContractID=' + <%=this._qfContractID%>;
                    url += '&ShowArchive=' + $('#ctl00_chkBoxArchive')[0].checked;
                    break;
                case 1:
                    url += 'MDGrid_Narrative.aspx';
                    url += '?CurrentLevel=' + 2;
                    url += '&ProductVersionID=' + <%=this._productVersionID%>;
                    url += '&ContractID=' + encodeURIComponent(id);
                    url += '&ShowArchive=' + $('#ctl00_chkBoxArchive')[0].checked;
                    break;
            }


            $('iFrame', $(td)).each(function () {
                var src = $(this).attr('src');
                if (src == "javascript:''") {
                    $(this).attr('src', url);
                }

                $(this).show();
            });
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
            } catch (e) {

            }
        }

        function initializeEvents() {
            $('#<%=Master.FindControl("chkBoxArchive").ClientID %>')[0].checked = ('<%=_qfShowArchive%>' == 'True');
            $('#<%=Master.FindControl("chkBoxArchive").ClientID %>').on('click', function () {    
                refreshPage();
            });
        }

        $(document).ready(function () {

            initVariables();
            initializeEvents();
            if (<%=_currentLevel%> == 0) {
                $('#tdChkBoxArchive').show();
            }
            if (<%=_currentLevel%> == 1) {
                $('#buttonSave').hide();
                $('#buttonCopy').show();
            }
            if (<%=_currentLevel%> == 2) {
                $('#buttonSave').hide();
            }
            if (<%=_currentLevel%> > 0) {
                $('#tableQuickFilters').hide();
            }
            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');
            $('#imgReport').hide();
            //$('#imgExport').hide();
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
            if (_canEdit) {
                $('input:text, textarea, input[type="number"]').on('change keyup mouseup', function () { txt_change(this); });
                $('input:checkbox').on('change', function () { txt_change(this); });
                $('input').on('change keyup mouseup', function () { txt_change(this); });
                $('select', $('#<%=this.grdMD.ClientID %>_Grid')).on('change keyup mouseup', function () { ddl_change(this); });

                $('#buttonNew').attr('disabled', false);
                $('#buttonNew').click(function (event) { buttonNew_click(); return false; });
                $('#buttonCopy').click(function (event) { buttonCopy_click(); return false; });
                $('#buttonCopySave').click(function (event) { buttonCopySave_click(); return false; });
                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });
            }
            $('input:text, textarea, input[type="number"]').each(function () {
                autosize($(this));
            });
            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });
            $('#<%=this.ddlQF_Contract.ClientID %>').change(function () { ddl_contract_change(); return false; });
            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });

            resizeFrame();

        });
    </script>
</asp:Content>
