<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css" />
    <title></title>
</head>
<script runat="server">
    protected void Button1_Click(object sender, System.EventArgs e)
    {
        DropDownList1.Enabled = false;
    }
    protected void Button2_Click(object sender, System.EventArgs e)
    {
        DropDownList1.Enabled = true;
    }
</script>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList 
             ID="DropDownList1"
             runat="server"
            Enabled="false"
             >
             <asp:ListItem>ConnectionsZone</asp:ListItem>
             <asp:ListItem>LoginView</asp:ListItem>
             <asp:ListItem>SiteMapPath</asp:ListItem>
             <asp:ListItem>ListBox</asp:ListItem>
             <asp:ListItem>LayoutEditorPart</asp:ListItem>
        </asp:DropDownList>
        <br /><br />
        <asp:Button 
             ID="Button1"
             runat="server"
             Font-Bold="true"
             ForeColor="IndianRed"
             Text="Disable DropDownList"
             OnClick="Button1_Click"
             />
        <asp:Button 
             ID="Button2"
             runat="server"
             Font-Bold="true"
             ForeColor="IndianRed"
             Text="Enable DropDownList"
             OnClick="Button2_Click"
             />
    </div>
    </form>
</body>
</html>
