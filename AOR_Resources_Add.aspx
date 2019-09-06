<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Resources_Add.aspx.cs" Theme="Default" Inherits="AOR_Resources_Add" %>

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
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#ffffff">
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
            parent.arrResources = [];
	        $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            var validation = validate();

            if (validation.length === 0) {
                $('#<%=this.grdData.ClientID %>_BodyContainer table tr').not(':first').each(function() {
                    var $obj = $(this);

                    if ($obj.find('td').text().indexOf('No Resources') === -1) parent.arrResources.push({ 'resourceid': $obj.find('select[field="Resource"]').val(), 'allocation': $obj.find('input[field="Allocation"]').val() });
                });
	        
                $('#btnAdd', parent.document).prop('disabled', false);
            }
            if (validation.length > 0) {
                MessageBox('Invalid entries: <br><br>' + validation);
            }
        }

        function validate(){
            var validation = [];
			var $resourceRows = $('#<%=this.grdData.ClientID %>_BodyContainer table tr').not(':first');
			var blnExit = false;
			var allocationSum = 0;
            
                    $.each($resourceRows, function () {
                        if (blnExit) return false;

                        var resourceID = $(this).find('select[field="Resource"]').val();

                        $.each($resourceRows.not($(this)), function () {
                            if ($(this).find('select[field="Resource"]').val() == resourceID) {
                                validation.push('Resource cannot have duplicates.');
                                blnExit = true;
                                return false;
                            }
                        });
                    });
                    $.each($resourceRows, function () {
                        allocationSum += parseInt($(this).find('input[field="Allocation"]').val());
                    });

                    if (allocationSum > 100) validation.push('Total Allocation % cannot exceed 100.');

                    return validation.join('<br>');
                    
        }

        function loadGrid() {
            parent.ShowDimmer(true, 'Loading...', 1);

            var filters = parent.filterBox.toJSON({ groups: { ParentModule: 'Work' } });

            $('#<%=this.txtAppliedFilters.ClientID %>').val(filters);
            $('#<%=this.txtPostBackType.ClientID %>').val('LoadGrid');
            $('#<%=this.btnLoadGrid.ClientID %>').trigger('click');
        }

        function addHeader() {
            var nHTML = '';
            var $tbl = $('#<%=this.grdData.ClientID %>_BodyContainer table');

            if ($tbl.length == 0){
                var $tbl = $('#<%=this.grdData.ClientID %>_BodyContainer');
                if ($tbl.find('td').text().indexOf('No Resources') != -1) $tbl.find('tr').not(':first').remove();

                nHTML += "<table style=\"border-collapse: collapse;\"><tr class=\"gridHeader\">";
                nHTML += "<th style=\"border-top: 1px solid grey; border-left: 1px solid grey; text-align: center; width: 200px;\"><a href=\"\" onclick=\"addResource(); return false;\" style=\"color: blue;\">Add</a>";
                nHTML += "</th><th style=\"border-top: 1px solid grey; text-align: center; width: 400px;\">";
                nHTML += "Resource</th><th style=\"border-top: 1px solid grey; text-align: center; width: 400px;\">Allocation %</th></tr></table>";
			
                $tbl.find('div').after(nHTML);
                <%=this.grdData.ClientID %>.ResizeGrid();
            }
        }

        function addResource() {
			var nHTML = '';
			var $tbl = $('#<%=this.grdData.ClientID %>_BodyContainer table');

			if ($tbl.find('td').text().indexOf('No Resources') != -1) $tbl.find('tr').not(':first').remove();

			nHTML += '<tr class=\"gridBody\" rowChanged="1"><td style=\" text-align: center;\">';
			nHTML += '<a href=\"\" onclick=\"removeResource(this); return false;\" style=\"color: blue;\">Remove</a>';
			nHTML += '</td><td>';
			nHTML += '<select field="Resource" original_value="0" fieldChanged="1" onchange=\"input_change(this); return false;\" style="width: 95%; background-color: #F5F6CE;">' + decodeURIComponent('<%=this.UsersOptions %>') + '</select>';
			nHTML += '</td><td style=\"text-align: center;\">';
			nHTML += '<input type="text" value="0" maxlength="3" field="Allocation" original_value="0" fieldChanged="1" onkeyup=\"input_change(this); return false;\" onpaste=\"input_change(this); return false;\" style="width: 95%; text-align: center;" />';
			nHTML += '</td></tr>';

			$tbl.find('tr:first').after(nHTML);
			$('#btnAdd', parent.document).prop('disabled', false);
            <%=this.grdData.ClientID %>.ResizeGrid();
            input_change(this);
		}

		function removeResource(obj) {
			$(obj).closest('tr').remove();

			var $tbl = $('#<%=this.grdData.ClientID %> table');

			if ($tbl.find('tr').not(':first').length == 0) $tbl.append('<tr class="gridBody"><td colspan="3" style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;">No Resources</td></tr>');

			$('#btnAdd', parent.document).prop('disabled', false);
            <%=this.grdData.ClientID %>.ResizeGrid();
		    input_change(this);
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
            addHeader();
        });
    </script>
</body>
</html>