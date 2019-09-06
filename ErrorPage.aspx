<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorPage.aspx.cs" Inherits="ErrorPage" Theme="default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="errorDiv" style="margin:5px; overflow:no-display;">
        <table id="errorTable">
            <tr>
                <td style="font-weight:bold; white-space:nowrap; vertical-align:top;">
                    Page:
                </td>
                <td style="vertical-align:top;">
                    <asp:Label ID="lblPage" runat="server" style="white-space:nowrap"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="font-weight:bold; white-space:nowrap; vertical-align:top;">
                    Function:
                </td>
                <td style="vertical-align:top;">
                    <asp:Label ID="lblFunction" runat="server" style="white-space:nowrap"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="font-weight:bold; white-space:nowrap; vertical-align:top;">
                    Error Code:
                </td>
                <td style="vertical-align:top;">
                    <asp:Label ID="lblNumber" runat="server" style="white-space:nowrap"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="font-weight:bold; white-space:nowrap; vertical-align:top; padding-top:2px;">
                    Error Description:
                </td>
                <td style="vertical-align:top;">
                    <asp:TextBox ID="txtDescription" runat="server" BorderStyle="None" BackColor="Transparent" TextMode="MultiLine" ReadOnly="true" Style="margin:0px; padding:0px; overflow:auto; width:200px; height:40px; font-family:Arial; font-size:12px; color:#4C4C4C;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div id="popupFooter" class="PopupFooter">
        <table cellpadding="0" cellspacing="0" style="width:100%; height:100%;">
            <tr>
                <td style="width:100%; text-align:right;">
                    <button id="btnClose" class="button" onmouseover="this.className = 'buttonOver';" onmouseout="this.className = 'button';" onmousedown="this.className = 'buttonClick';" onmouseup="this.className = 'buttonOver';" onclick="closeWindow(); return false;">
                        <div>Close</div>
                    </button>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        //This sets the popup size based on the contents
        var errorTable = document.getElementById('errorTable');
        var popupFooter = document.getElementById('popupFooter');
        popupManager.GetPopupByName('Error').SetWidth(errorTable.offsetWidth + 10);
        popupManager.GetPopupByName('Error').SetHeight(errorTable.offsetHeight + popupFooter.offsetHeight + 10);

    </script>
    </form>
</body>
</html>
