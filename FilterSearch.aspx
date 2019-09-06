﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FilterSearch.aspx.cs" Inherits="FilterSearch" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WTS Search</title>
    <script type="text/javascript" src="scripts/shell.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
    <script type="text/javascript" src="scripts/jquery-1.11.2.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 4px;">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblSearch" runat="server" Style="white-space: nowrap;"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtSearch" runat="server" Style="font-size: 11px; width: 300px;" onPaste="txtSearch_OnChange();" onkeypress="txtSearch_OnChange();"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ID="chkClear" runat="server" Text="Clear all search parameters." />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divFooter" class="PopupFooter">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <button id="btnSave" disabled="disabled" onclick="saveSearch(); return false;">Save</button>
                    </td>
                    <td>
                        <button id="btnClose" onclick="closeWindow();">Cancel</button>
                    </td>
                </tr>
            </table>
        </div>

        <script type="text/javascript">
            var txtSearch = document.getElementById('txtSearch');
            var chkClear = document.getElementById('chkClear');

            var btnSave = document.getElementById('btnSave');

            function saveSearch() {
                var filterValues = txtSearch.value;

                if (filterValues == '') {
                    txtSearch.focus();
                    MessageBox('Please enter a search value to continue.');

                    return false;
                }

                if (parent.addSearchToFilter) {
                    parent.addSearchToFilter(filterValues, chkClear.checked);
                }
                closeWindow();
            }

            function txtSearch_OnChange() {
                var e = event;
                if (e) {
                    btnSave.disabled = false;
                    if (e.keyCode == 13 || e.keyCode == 144) {
                        saveSearch();
                    }
                }
            }

            function initEvents() {
                //This fixes a Chrome issue bug-Auto fill is not an event that can be captured. 
                //So we must instead create an event to keep looking if the text has changed in order to save the search.
                setInterval(function () {
                    // get the password field
                    var pwd = document.getElementById('txtSearch');
                    var numberOfChars = pwd.selectionEnd;
                    if (numberOfChars > 0) {
                        document.getElementById('btnSave').disabled = false;
                    }
                }, 500);
            }


            $(document).ready(function () {
                initEvents();

                $('#<%=txtSearch.ClientID %>').focus();
            });
           

        </script>
    </form>
</body>
</html>
