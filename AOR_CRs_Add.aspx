﻿<%@ Page Language="C#" EnableViewState="false" AutoEventWireup="true" CodeFile="AOR_CRs_Add.aspx.cs" Inherits="AOR_CRs_Add" Theme="Default" %>

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
        <asp:Button ID="btnLoadGrid" runat="server" style="display: none;" />
        <asp:TextBox ID="txtAppliedFilters" runat="server" style="display: none;"></asp:TextBox>
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

            if ($obj.find('input[type="checkbox"]').is(':checked')) {
                if ($.inArray($obj.attr('cr_id'), parent.arrCRs) == -1) parent.arrCRs.push($obj.attr('cr_id'));
            }
            else {
                if ($.inArray($obj.attr('cr_id'), parent.arrCRs) != -1) parent.arrCRs.splice($.inArray($obj.attr('cr_id'), parent.arrCRs), 1);
            }
	        
            $('#btnAdd', parent.document).prop('disabled', false);
            countSelected();
        }

        function loadGrid() {
            parent.ShowDimmer(true, 'Loading...', 1);

            var filters = parent.filterBox.toJSON({ groups: { ParentModule: 'Work' } });

            $('#<%=this.txtAppliedFilters.ClientID %>').val(filters);
            $('#<%=this.txtPostBackType.ClientID %>').val('LoadGrid');
            $('#<%=this.txtCRStatusQF.ClientID %>').val(parent._selectedCRStatusesQF);
            $('#<%=this.txtCRContractQF.ClientID %>').val($('*[id*=ms_Item5]', parent.document).val());
            $('#<%=this.btnLoadGrid.ClientID %>').trigger('click');
        }

	    function checkSelected() {
	        $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]').each(function () {
	            var $obj = $(this).parent();

	            if ($.inArray($obj.attr('cr_id'), parent.arrCRs) != -1) {
	                $(this).prop('checked', true);
	            }
	            else {
	                $(this).prop('checked', false);
	            }
	        });

	        if (parent.arrCRs.length > 0) $('#btnAdd', parent.document).prop('disabled', false);
	        countSelected();
	    }

        function countSelected() {
            var selectedCount = parent.arrCRs.length;

            $('#spnSelectedCountCR', parent.document).text(selectedCount + ' CR' + (selectedCount != 1 ? 's' : '') + ' Checked');
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            parent.ShowDimmer(false);
            <%=this.grdData.ClientID %>.ResizeGrid();
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            checkSelected();
        });
    </script>
</body>
</html>
