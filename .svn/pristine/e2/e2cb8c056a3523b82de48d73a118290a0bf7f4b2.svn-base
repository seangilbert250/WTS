<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Audit_History_Popup.aspx.cs" Inherits="Audit_History_Popup" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Audit History Popup</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span><%=this.Title %></span>
</asp:Content>
<asp:Content ID="cphGridSettings" ContentPlaceHolderID="cphGridSettings" runat="server">
    As of Date:&nbsp;<input type="text" id="txtAsOfDate" value="" style="width:75px;">
</asp:Content>
<asp:Content ID="cphPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="server">    
    <input type="button" id="btnClose" value="Close">
</asp:Content>

<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divHistoryContainer" style="padding:10px;width:97%;overflow-y:auto;">

    <asp:PlaceHolder ID="phAuditHistory" runat="server">
    </asp:PlaceHolder>

    </div>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _asOfDate;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function refreshPage() {
            var url = window.location.href;
            url = editQueryStringValue(url, 'asofdate', StrongEscape($('#txtAsOfDate').val()));
            window.location.href = url;
        }

        function closePage() {
            var thePopup = popupManager.GetPopupByName('AuditHistory');
            thePopup.Close();
        }

        function toggleAuditHistoryRow(section, rowIdx) {
            var expandImg = $('img[audithistoryrowexpandimg=' + (section + '_' + rowIdx) + ']');
            var rowHeader = $('tr[audithistoryrowheader=' + (section + '_' + rowIdx) + ']');
            var row = $('tr[audithistoryrow=' + (section + '_' + rowIdx) + ']');

            if (row.length > 0) {
                if (row.is(':visible')) {
                    expandImg.attr('src', 'images/icons/expand.gif');
                    expandImg.attr('alt', 'Expand History');
                    expandImg.attr('title', 'Expand History');
                    rowHeader.hide();
                    row.hide();
                }
                else {
                    expandImg.attr('src', 'images/icons/collapse.gif');
                    expandImg.attr('alt', 'Collapse History');
                    expandImg.attr('title', 'Collapse History');
                    rowHeader.show();
                    row.show();
                }
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _asOfDate = '<%=asOfDate.ToString("MM/dd/yyyy")%>';
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();

            var ht = $(window.frameElement).height();
            ht -= $('#pageContentInfo').height(); // gray bar
            ht -= 30; // padding

            $('#divHistoryContainer').height(ht);
            $('#txtAsOfDate').val(_asOfDate);
            $('#txtAsOfDate').datepicker();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnClose').click(function () { closePage(); });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>
