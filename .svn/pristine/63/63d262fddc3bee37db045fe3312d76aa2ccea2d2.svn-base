<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">About</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div style="overflow-x: hidden; overflow-y: auto; height: 300px;">
        <div id="divMain" style="padding-top: 5px; text-align: center;">   
            <table style="width: 100%;">
                <tr>
                    <td style="text-align: center;">
                        <div>
                            <asp:Label ID="lblWebsysHeader" runat="server" style="font-weight: bold;"></asp:Label>
                        </div>
                        <div style="padding-top: 5px;">
                            <asp:Label ID="lblWebsysDesc" runat="server"></asp:Label>
                        </div>
                        <div style="padding-top: 5px;">
                            Version: <asp:Label ID="lblVersionNo" runat="server">Version: </asp:Label>
                        </div>
                    </td>
                    <td style="padding: 10px; padding-left: 25px;">
                        <div id="divLogo" runat="server">
                            <asp:Image ID="imgLogo" runat="server" src="Images/logo1.jpg" style="width: 100px; height: 50px;"/>
                        </div>   
                    </td>
                </tr>
            </table>
            <div class="pageContentHeader" style="height: 2px; text-align: center; padding-top: 2px;">
                <div style="padding: 15px;">
                    <a href="http://www.infintech.com" target="_blank" style="font-size: 10pt;">http://www.infintech.com</a>
                    <br />
                    <div id="supportEmailDiv" runat="server"></div>
                </div>
                <div style="text-align: left; padding-left: 15px; padding-top: 5px;">
                    <asp:Label ID="lblCopyright" runat="server"></asp:Label>
                </div> 
                <div style="padding: 15px; text-align: left;">
                    WARNING: 
                        <asp:Label id="lblWebsysAbbrev" runat="server"></asp:Label>
                    name and software program(s) is protected by copyright law and international treaties. 
                    Unauthorized reproduction or distribution of this program, or any portion of it, may result in severe civil and criminal penalties, 
                    and will be prosecuted to the maximum extent possible under the law.
                </div>
            </div>
        </div>
        <div id="popupFooter" class="PopupFooter" runat="server">
            <table cellpadding="0" cellspacing="0" style="width: 100%; height: 100%;">
                <tr>
                    <td style="width: 100%; text-align: right;">
                        <div class='buttonDiv' id="btnClose" runat="server" style="margin-right:5px; margin-right:5px; cursor:hand;" onclick="closeWindow()" onmouseover="highlightButton(this,1)" onmouseout="highlightButton(this,0)" onmousedown="highlightButton(this,2)" onmouseup="highlightButton(this,1)" onselectstart="return false">
                            <div class="buttonLeft"></div>
                            <div class="buttonMiddle"><div>Close</div></div>
                            <div class="buttonRight"></div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <script type="text/javascript">
        function initPage() {
            try {
                var popupHeight = document.getElementById('popupFooter').clientHeight;

                resizePageElement('divMain', popupHeight);
            }
            catch (e) { }
        }

        function qs(key) {
            //Get the full querystring
            fullQs = window.location.search.substring(1);
            //Break it down into an array of name-value pairs
            qsParamsArray = fullQs.split("&");
            //Loop through each name-value pair and
            //return value in there is a match for the given key
            for (i=0;i<qsParamsArray.length;i++) {
                strKey = qsParamsArray[i].split("=");
                if (strKey[0] == key) {
                    return strKey[1];
                }
            }
        }

        function supportEmail() {
            var system = 'wts';
            var supportDiv = document.getElementById("<%=supportEmailDiv.ClientID %>");
            var anchor = document.createElement("A");
            anchor.href = "mailto:" + system + ".support@infintech.com?subject=" + system + " help";
            anchor.innerText = system + ".support@infintech.com";
            supportDiv.appendChild(anchor);
        }

        initPage();
        supportEmail();
    </script>
</asp:Content>