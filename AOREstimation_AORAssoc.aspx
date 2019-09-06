﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Grids.master" CodeFile="AOREstimation_AORAssoc.aspx.cs" Inherits="AOREstimation_AORAssoc" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Familiarity AOR Association</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Familiarity AOR Association</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle;" />
                <input type="button" id="btnDelete" value="Disassociate" disabled="disabled" style="vertical-align: middle;" />
                <input type="button" id="btnPrimary" value="Set as Primary" disabled="disabled" style="vertical-align: middle;" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" EmptyDataText="No AORS Associated">
	</iti_Tools_Sharp:Grid>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAOREstimation_AssocID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function buttonSave_click() {
            save();
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

                $('.gridBody, .selectedRow', $('#<%=this.grdData.ClientID%>_Grid')).each(function (i, row) {
                    var changedRow = [];
                    var changed = false;

                    if (_dcc[0].length > 0) {
                        for (var i = 0; i <= _dcc[0].length - 1; i++) {
                            if (i == 4) { //Check only the notes column for changes
                                var newval = GetColumnValue(row, i);
                                var oldval = GetColumnValue(row, i, true);
                                if (newval != oldval) {
                                    changed = true;
                                    break;
                                }
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
                    PageMethods.SaveChanges(json, save_done, on_error);
                }
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }

        function save_done(result) {
            try {
                ShowDimmer(false);

                var saved = 0, failed = 0;
                var errorMsg = '', ids = '', failedIds = '';

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved) {
                        saved = parseInt(obj.saved);
                    }
                    if (obj.failed) {
                        failed = parseInt(obj.failed);
                    }
                    if (obj.savedIds) {
                        ids = obj.savedIds;
                    }
                    if (obj.failedIds) {
                        failedIds = obj.failedIds;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                var msg = '';
                if (errorMsg.length > 0) {
                    msg = 'An error occurred while saving: \n' + errorMsg;
                }

                if (saved > 0) {
                    msg = 'Successfully saved ' + saved + ' record(s).';
                    //if (opener && opener.refreshPage) {
                    //    opener.refreshPage(true);
                    //}
                }
                if (failed > 0) {
                    msg += '\n' + 'Failed to save ' + failed + ' record(s).';
                }
                MessageBox(msg);

                if (saved > 0) {
                    refreshPage();
                }
            } catch (e) { }
        }

        function btnAdd_click() {
            var nWindow = 'AddCRAOR';
            var nTitle = 'Add AOR';
            var nHeight = 700, nWidth = 1000;
            var nURL = 'AOREstimation_AORAssoc_Add.aspx' + window.location.search + '&Type=Release Schedule AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm AOR Estimation Disassociation', 'Are you sure you want to disassociate this AOR Estimation?', 'Yes,No', 'confirmAORDelete', 300, 300, this);
        }

        function confirmAORDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Disassociating...', 1);

                    PageMethods.DeleteAORAssoc(_selectedAOREstimation_AssocID, delete_done, on_error);
                    if (parent.opener) parent.opener.refreshPage(true);
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
                MessageBox('AOR Estimation has been disassociated.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to disassociate. <br>' + errorMsg);
            }
        }
        
        function btnPrimary_click() {
            QuestionBox('Confirm AOR Estimation Primary', 'Are you sure you want to set this AOR as the Primary?', 'Yes,No', 'confirmAORPrimary', 300, 300, this);
        }

        function confirmAORPrimary(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Setting AOR as Primary...', 1);

                    PageMethods.SetPrimaryAORAssoc(_selectedAOREstimation_AssocID, primary_done, on_error);
                    if (parent.opener) parent.opener.refreshPage(true);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function primary_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                MessageBox('AOR has been set as Primary.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to Set as Primary. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function openAOR(AORID, AORReleaseID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function row_click(obj) {
            var $obj = $(obj);

            if ($obj.attr('AOREstimation_AORAssocID')) {
                _selectedAOREstimation_AssocID = $obj.attr('AOREstimation_AORAssocID');
                $('#btnDelete').prop('disabled', false);
                $('#btnPrimary').prop('disabled', false);
            }
            else {
                _selectedAOREstimation_AssocID = 0;
                $('#btnDelete').prop('disabled', false);
                $('#btnPrimary').prop('disabled', false);
            }
        }

        function canEdit_done(result) {
            var blnCanEdit = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.canEdit && obj.canEdit.toUpperCase() == 'TRUE') blnCanEdit = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnCanEdit) {
                $('#btnDelete').prop('disabled', false);
                $('#btnPrimary').prop('disabled', false);
            }
            else {
                $('#btnDelete').prop('disabled', true);
                $('#btnPrimary').prop('disabled', true);
            }
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {

        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _canEdit = ('<%=this.CanEditAOR.ToString().ToUpper()%>' == 'TRUE');

            if (_dcc[0] && _dcc[0].length > 0) {
                _idxID = parseInt('<%=this.DCC["AOREstimation_AORAssocID"].Ordinal %>');
                _idxNotes = parseInt('<%=this.DCC["Notes"].Ordinal %>');
            }
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            resizeGrid();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnPrimary').click(function () { btnPrimary_click(); return false; });
            $('input:text').on('change keyup mouseup', function () { txt_change(this); });

            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            if (_canEdit) {
                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
            }
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>