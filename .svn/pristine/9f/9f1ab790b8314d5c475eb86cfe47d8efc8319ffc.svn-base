<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MDAddEdit_AORResource.aspx.cs" Inherits="MDAddEdit_AORResource" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Resource Allocation</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" href="Styles/multiple-select.css" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span id="spnTitle" runat="server">Resource Allocation</span>
    <span id="spnRelease" style="padding-left: 25px;">Release:&nbsp;<asp:DropDownList ID="ddlReleaseQF" runat="server" multiple="true" Width="200px"></asp:DropDownList></span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
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

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
    
    <script id="jsVariables" type="text/javascript">
        var _selectedReleasesQF = '';
    </script>

	<script id="jsEvents" type="text/javascript">
	    function imgRefresh_click() {
	        refreshPage();
        }

        function ddlReleaseQF_update() {
            var arrReleaseQF = $('#<%=this.ddlReleaseQF.ClientID %>').multipleSelect('getSelects');

            _selectedReleasesQF = arrReleaseQF.join(',');
        }

        function ddlReleaseQF_close() {
            refreshPage();
        }

	    function btnSave_click() {
            try {
                var arrChanges = [];

                $('input[fieldChanged="1"]').each(function() {
                    var $obj = $(this);

                    arrChanges.push({ 'aorreleaseresourceid': $obj.attr('aorreleaseresource_id'), 'field': $obj.attr('field'), 'value': $obj.val()});
                });

                if (arrChanges.length > 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    validate(arrChanges);
                }
                else {
                    MessageBox('You have not made any changes.');
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
	            var fieldCount = $('input[fieldChanged="1"]').length;
	            var rowCount = $('tr[rowChanged="1"]').length;

	            MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');

                if (opener.parent.refreshChildren) opener.parent.refreshChildren();
                else if (opener.refreshPage) opener.refreshPage();

                refreshPage();
	        }
	        else {
	            MessageBox('Failed to save. <br>' + errorMsg);
	        }
        }

	    function on_error() {
	        ShowDimmer(false);
	        MessageBox('An error has occurred.');
	    }

        function validate(changes) {
            var validation = [];
            var json = JSON.stringify(changes);

            try {
                PageMethods.VerifyAllocation(<%=this.ResourceID%>, json, function (result) {
                    if (result) {
                        var nJSON = '{update:' + JSON.stringify(changes) + '}';

                        PageMethods.SaveChanges(nJSON, save_done, on_error);
                    } else {
                        MessageBox('Invalid entries: <br><br> Resource allocation cannot exceed 100%.');
                        ShowDimmer(false);
                    }
                });
            } catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
            
        }

        function input_change(obj) {
            var $obj = $(obj);

	        $obj.attr('fieldChanged', '1');
	        $obj.closest('tr').attr('rowChanged', '1');

	        if ($obj.attr('field') == 'Allocation %') {
	            var nVal = $obj.val();

	            $obj.val(nVal.replace(/[^\d]/g, ''));
	        }

	        $('#btnSave').prop('disabled', false);
	    }

	    function txtBox_blur(obj) {
	        var $obj = $(obj);
	        var nVal = $obj.val();

	        if ($obj.attr('field') == 'Allocation %') {
	            if (nVal == '') $obj.val('0');
	            return;
	        }

	        $obj.val($.trim(nVal));
	    }

        function refreshPage() {
            ddlReleaseQF_update();

            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);

	        window.location.href = 'Loading.aspx?Page=' + nURL;
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            if ('<%=this.CanEdit %>'.toUpperCase() == 'TRUE') $('#btnSave').show();
            $('#<%=this.grdData.ClientID%>_BodyContainer').css('height', '376');
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('input[type="text"]').on('keyup paste', function () { input_change(this); });
            $('input[type="text"]').on('blur', function () { txtBox_blur(this); });
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();

            $('#<%=this.ddlReleaseQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onOpen: function () { ddlReleaseQF_update(); }
                , onClose: function () { ddlReleaseQF_close(); }
            }).change(function () { ddlReleaseQF_update(); });

            $('#<%=this.ddlReleaseQF.ClientID %>').multipleSelect('setSelects', <%=this.Releases%>);
        });
    </script>
</asp:Content>
