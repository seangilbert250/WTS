<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SortOptions.aspx.cs" Inherits="SortOptions" Theme="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Sort Grid</title>
    <script src="Scripts/shell.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.11.2.js" type="text/javascript"></script>
    <script src="Scripts/PopupWindow.js" type="text/javascript"></script>
</head>
<body>
    <form id="frmSorter" runat="server">
    <div id="divSort" style="padding-left: 10px; padding-top: 5px;">
         <table border="0" width="100%">
             <tr>
                 <td style="width: 50%; height: 23px; text-align: center">
                     &nbsp;FIELDS</td>
                 <td style="width: 50%; height: 23px; text-align: center">
                     &nbsp;SORT BY</td>
             </tr>
            <tr>
               <td style="width: 50%; height: 23px; text-align: left">
                  1
                  <asp:DropDownList ID="dlField1" runat="server" EnableViewState="False" TabIndex="2" Visible="True" Width="90%">
                  </asp:DropDownList></td>
               <td style="width: 50%; height: 23px; text-align: center;">
                  <asp:RadioButtonList ID="optSort1" runat="server" CellPadding="0" name="optSort1" CellSpacing="0" RepeatDirection="Horizontal" Width="180px">
                     <asp:ListItem Selected="True" Value="1" Text="Ascending"></asp:ListItem>
                     <asp:ListItem Value="2" Text="Descending"></asp:ListItem>
                  </asp:RadioButtonList>
               </td>
            </tr>
            <tr>
               <td style="height: 23px; text-align: left">
                  2
                  <asp:DropDownList ID="dlField2" runat="server" EnableViewState="False" TabIndex="2" Visible="True" Width="90%">
                  </asp:DropDownList></td>
               <td style="height: 23px; text-align: center;">
                  <asp:RadioButtonList ID="optSort2" runat="server" CellPadding="0" name="optSort2" CellSpacing="0" RepeatDirection="Horizontal" Width="180px">
                     <asp:ListItem Selected="True" Value="1" Text="Ascending"></asp:ListItem>
                     <asp:ListItem Value="2" Text="Descending"></asp:ListItem>
                  </asp:RadioButtonList>
               </td>
            </tr>
            <tr>
               <td style="height: 23px; text-align: left">
                  3
                  <asp:DropDownList ID="dlField3" runat="server" EnableViewState="False" TabIndex="2" Visible="True" Width="90%">
                  </asp:DropDownList></td>
               <td style="text-align: center;">
                  <asp:RadioButtonList ID="optSort3" runat="server" CellPadding="0" name="optSort3" CellSpacing="0" RepeatDirection="Horizontal" Width="180px">
                     <asp:ListItem Selected="True" Value="1" Text="Ascending"></asp:ListItem>
                     <asp:ListItem Value="2" Text="Descending"></asp:ListItem>
                  </asp:RadioButtonList>
               </td>
            </tr>
            <tr>
               <td style="height: 23px; text-align: left">
                  4
                  <asp:DropDownList ID="dlField4" runat="server" EnableViewState="False" Font-Names="Arial" Font-Size="8pt" TabIndex="2" Visible="True" Width="90%">
                  </asp:DropDownList></td>
               <td style="height: 23px; text-align: center;">
                  <asp:RadioButtonList ID="optSort4" runat="server" CellPadding="0" name="optSort4" CellSpacing="0" Height="8px" RepeatDirection="Horizontal" Width="180px">
                     <asp:ListItem Selected="True" Value="1" Text="Ascending"></asp:ListItem>
                     <asp:ListItem Value="2" Text="Descending"></asp:ListItem>
                  </asp:RadioButtonList>
               </td>
            </tr>
             <tr>
                <td style="height: 22px; text-align: left">
                   <asp:CheckBox ID="chkSortDefault" runat="server" Text="Use Default Sorting" />
                </td>
                <td style="height: 22px"></td>
             </tr>
         </table>
         <div id="popupFooter" class="PopupFooter" runat="server">
            <table cellpadding="0" cellspacing="0" style="width:100%; height:100%;">
                <tr>
                    <td style="width:100%; text-align:right;">
                        <input type="button" id="btnApply" value="Apply" />
                        <input type="button" id="btnCancel" value="Cancel" />
                    </td>
                </tr>
            </table>
        </div> 
     </div>



    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
		<Services>
			<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
		</Services>
	</asp:ScriptManager>


</form>


<script type="text/javascript">
    var dlField1 = document.getElementById('dlField1');
    var dlField2 = document.getElementById('dlField2');
    var dlField3 = document.getElementById('dlField3');
    var dlField4 = document.getElementById('dlField4');
    var optSort1 = document.getElementById('optSort1').getElementsByTagName('input');
    var optSort2 = document.getElementById('optSort2').getElementsByTagName('input');
    var optSort3 = document.getElementById('optSort3').getElementsByTagName('input');
    var optSort4 = document.getElementById('optSort4').getElementsByTagName('input');
    var chkSortDefault = document.getElementById('chkSortDefault');
    var openerPage;
    var defaultColumns = '<%=DefaultColumns %>';
    var gridName = "";

    var pageName = "";
    var sortOptions = "";

    function selectDefaultValues() {
        try {
            if (defaultColumns == '') //return;
            {
                $(dlField1).val('');
                $(dlField2).val('');
                $(dlField3).val('');
                $(dlField4).val('');

                var rbl1 = document.getElementsByName('optSort1');
                var rbl2 = document.getElementsByName('optSort2');
                var rbl3 = document.getElementsByName('optSort3');
                var rbl4 = document.getElementsByName('optSort4');

                rbl1[1].checked = true;
                rbl1[2].checked = false;
                rbl2[1].checked = true;
                rbl2[2].checked = false;
                rbl3[1].checked = true;
                rbl3[2].checked = false;
                rbl4[1].checked = true;
                rbl4[2].checked = false;

            }
            else
                {
                $(dlField1).val(defaultColumns.split(';')[0].split('|')[0]);
                $(dlField2).val(defaultColumns.split(';')[1].split('|')[0]);
                $(dlField3).val(defaultColumns.split(';')[2].split('|')[0]);
                $(dlField4).val(defaultColumns.split(';')[3].split('|')[0]);

                $('input[name="optSort1"][value="' + defaultColumns.split(';')[0].split('|')[1] + '"]')[0].checked = true;
                $('input[name="optSort2"][value="' + defaultColumns.split(';')[1].split('|')[1] + '"]')[0].checked = true;
                $('input[name="optSort3"][value="' + defaultColumns.split(';')[2].split('|')[1] + '"]')[0].checked = true;
                $('input[name="optSort4"][value="' + defaultColumns.split(';')[3].split('|')[1] + '"]')[0].checked = true;
            }

            gridName = '<%=gridName %>';

            PageMethods.DeleteSort(1, gridName);

        }
        catch (e) { DisplayErrorMessage('selectDefaultValues', e.number, e.message); }
    }

    function btnApplySort() {
        try {
            var sortValues = dlField1.value + '|' + $('#optSort1 input:checked').val() + '~' + dlField2.value + '|' + $('#optSort2 input:checked').val() + '~' +
                             dlField3.value + '|' + $('#optSort3 input:checked').val() + '~' + dlField4.value + '|' + $('#optSort4 input:checked').val() + '~' +
                             chkSortDefault.checked;

            gridName = '<%=gridName %>';

            PageMethods.ApplySortToDB(1, gridName, sortValues);

            if (chkSortDefault.checked) {
                sortValues = "";
            }
            if (openerPage.applySort) { openerPage.applySort(sortValues); }
            closeWindow();
        }
        catch (e) {
            DisplayErrorMessage('btnApplySort', e.number, e.message);
        }
    }

    function ApplySortValues(pageName) {

        if (pageName != "")
            sortOptions = PageMethods.GetSortOptionsFromDB(1, pageName, save_done, on_error);

    }

    function save_done(sortOptions) {

        if (sortOptions != null)
        {
            var columnParts = [];
            var checkedParts = [];

            columnParts = sortOptions.trim().split('|');
            checkedParts = sortOptions.trim().split('~');

            checkedParts[0] = checkedParts[0].replace(/[^0-9]/g, "");
            checkedParts[1] = checkedParts[1].replace(/[^0-9]/g, "");
            checkedParts[2] = checkedParts[2].replace(/[^0-9]/g, "");
            checkedParts[3] = checkedParts[3].replace(/[^0-9]/g, "");

            var rbl1 = document.getElementsByName('optSort1');
            var rbl2 = document.getElementsByName('optSort2');
            var rbl3 = document.getElementsByName('optSort3');
            var rbl4 = document.getElementsByName('optSort4');

            if (checkedParts[0] == "1")  // 1 = Asc, 2 = Desc
                {
                rbl1[1].checked = true;
                rbl1[2].checked = false;
            }
            else
            {
                rbl1[1].checked = false;
                rbl1[2].checked = true;
            }

            if (checkedParts[1] == "1")
            {
                rbl2[1].checked = true;
                rbl2[2].checked = false;
            }
            else {
                rbl2[1].checked = false;
                rbl2[2].checked = true;
            }

            if (checkedParts[2] == "1") {
                rbl3[1].checked = true;
                rbl3[2].checked = false;
            }
            else {
                rbl3[1].checked = false;
                rbl3[2].checked = true;
            }

            if (checkedParts[3] == "1") {
                rbl4[1].checked = true;
                rbl4[2].checked = false;
            }
            else {
                rbl4[1].checked = false;
                rbl4[2].checked = true;
            }

            columnParts[0] = columnParts[0].replace("1", "").replace("2", "").replace("~", "");
            columnParts[1] = columnParts[1].replace("1", "").replace("2", "").replace("~", "");
            columnParts[2] = columnParts[2].replace("1", "").replace("2", "").replace("~", "");
            columnParts[3] = columnParts[3].replace("1", "").replace("2", "").replace("~", "");

            $(dlField1).val(columnParts[0]);
            $(dlField2).val(columnParts[1]);
            $(dlField3).val(columnParts[2]);
            $(dlField4).val(columnParts[3]);

        }
    }
    function on_error() {

    }

    $(document).ready(function () {
        try {
            $('#btnApply').click(function () { btnApplySort(); });
            $('#btnCancel').click(function () {
            	if (closeWindow) {
            		closeWindow();
            	}
            });
            $('#chkSortDefault').change(function () { selectDefaultValues(); });
            $(dlField1).change(function () { $(chkSortDefault).attr('checked', false); });
            $(dlField2).change(function () { $(chkSortDefault).attr('checked', false); });
            $(dlField3).change(function () { $(chkSortDefault).attr('checked', false); });
            $(dlField4).change(function () { $(chkSortDefault).attr('checked', false); });

            gridName = '<%=gridName %>';

            try {
                openerPage = defaultParentPage.popupManager.ActivePopup.Opener;
                ApplySortValues(gridName); // openerPage.pageName
            }
            catch (e) {
                openerPage = opener;
                ApplySortValues(gridName);  //openerPage.pageName
            }
        }
        catch (e) {
        }
    });
</script>   
</body>
</html>
