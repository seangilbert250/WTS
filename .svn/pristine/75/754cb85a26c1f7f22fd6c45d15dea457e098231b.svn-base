<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMTDescription_Grid.aspx.cs" Inherits="RQMTDescription_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">RQMT Description</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table style="width: 100%; border-collapse: collapse;">
		<tr>
			<td>
                RQMT Description&nbsp;(<%=this.TotalCount %>)
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedRQMTDescriptionID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAdd_click() {
            var nWindow = 'RQMTDescription';
            var nTitle = 'RQMT Description';
            var nHeight = 400, nWidth = 600;
            var nURL = _pageUrls.Maintenance.RQMTDescriptionPopup + window.location.search + '&NewRQMTDescription=true&RQMTDescriptionID=0&Type=Add';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm RQMT Description Delete', 'Are you sure you want to delete this RQMT Description?', 'Yes,No', 'confirmRQMTDescriptionDelete', 300, 300, this);
        }

        function confirmRQMTDescriptionDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteRQMTDescription(_selectedRQMTDescriptionID, delete_done, on_error);
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
                MessageBox('RQMT Description has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('input[fieldChanged="1"], textarea[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
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
                var fieldCount = $('input[fieldChanged="1"], textarea[fieldChanged="1"], select[fieldChanged="1"]').length;
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

        function validate() {
            var validation = [];

            if ($('#<%=this.grdData.ClientID %>_BodyContainer table select[field="RQMT Description Type"]:first option').length == 0 || $('#<%=this.grdData.ClientID %>_BodyContainer table select[field="RQMT Description Type"] option:selected[value="0"]').length > 0) validation.push('Please select a RQMT Description Type.');
            if ($('#<%=this.grdData.ClientID %>_BodyContainer table textarea[field="RQMT Description"]:empty').length > 0) validation.push('RQMT Description cannot be empty.');

            return validation.join('<br>');
        }

        function row_click(obj) {
            if ($(obj).attr('rqmtdescription_id')) {
                _selectedRQMTDescriptionID = $(obj).attr('rqmtdescription_id');

                $('#btnDelete').prop('disabled', false);
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

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            if ('<%=this.CanEditRQMTDescription %>'.toUpperCase() == 'TRUE') {
                $('#btnAdd').show();

                if ($('.saveable').length > 0) {
                    $('#btnDelete').show();
                    $('#btnSave').show();
                }
            }

            resizeGrid();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>
