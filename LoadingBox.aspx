<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LoadingBox.aspx.cs" Inherits="LoadingBox" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="scripts/shell.js"></script>
    <script type="text/javascript" src="scripts/common.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="messageDiv" style="margin:5px; overflow:hidden;">
        <table style="width:100%;">
            <tr>
                <td style="height:30px; vertical-align:middle;">
                    <img src="images/loaders/loader_2.gif" alt="Loading" />   
                </td>
                <td id="tdMessage" style="vertical-align:middle; white-space:nowrap; height:100%; width:98%;">
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        //Note - If MessageBox is requested from Javascript
        //------ In order to pass a line break you must use '\\n'
        //------ Simply using '\n' will be lost due to being
        //------ passed through url. Similarly any other
        //------ will require an extra '\'.
        var tdMessage = document.getElementById('tdMessage');
        var theMessage = '<%=Request.QueryString["strLoading"] %>';
        tdMessage.innerText = unescape(theMessage);

    </script>
    <script type="text/javascript">
        //This sets the popup size based on the contents
        var tdMessage = document.getElementById('tdMessage');
        if (tdMessage.offsetWidth >= 1024) {
            tdMessage.style.whiteSpace = 'normal';
        }
        if (popupManager) {
            if (popupManager.GetPopupByName('Loading')) {
                popupManager.GetPopupByName('Loading').SetWidth(tdMessage.offsetWidth + 10);
                popupManager.GetPopupByName('Loading').SetHeight(tdMessage.offsetHeight + 10);
            }
        }
    </script>
    </form>
</body>
</html>