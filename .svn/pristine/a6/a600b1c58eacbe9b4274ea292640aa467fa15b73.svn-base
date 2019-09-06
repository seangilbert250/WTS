<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMTDescription_Popup.aspx.cs" Inherits="RQMTDescription_Popup" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">RQMT Description</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span><%=this.Title %></span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnClose" value="Close" style="vertical-align: middle;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <div id="divAdd" style="padding: 10px; display: none;">
        <table style="width: 100%; border-collapse: collapse;">
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 140px;">
                    RQMT Description Type:
                </td>
                <td>
                    <asp:DropDownList ID="ddlRQMTDescriptionType" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding-top: 10px;">
                    <asp:TextBox ID="txtRQMTDescription" runat="server" TextMode="MultiLine" Rows="20" Width="99%"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function btnClose_click() {
            closeWindow();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    switch ('<%=this.Type %>') {
                        case 'Add':
                            PageMethods.Save($('#<%=this.ddlRQMTDescriptionType.ClientID %>').val(), $('#<%=this.txtRQMTDescription.ClientID %>').val(), save_done, on_error);
                        break;
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false, blnExists = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
                if (obj.newID && (parseInt(obj.newID) < 0 || parseInt(obj.newID) > 0)) newID = obj.newID;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                opener.refreshPage(true);
                setTimeout(closeWindow, 1);
            }
            else if (blnExists) {
                MessageBox('RQMT Description already exists.');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            switch ('<%=this.Type %>') {
                case 'Add':
                    if ($('#<%=this.ddlRQMTDescriptionType.ClientID %> option').length == 0 || $('#<%=this.ddlRQMTDescriptionType.ClientID %>').val() == '0') validation.push('Please select a RQMT Description Type.');
                    if ($('#<%=this.txtRQMTDescription.ClientID %>').val().length == 0) validation.push('RQMT Description cannot be empty.');
                    break;
            }

            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);

            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage() {
            window.location.href = 'Loading.aspx?Page=' + window.location.href;
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#<%=this.grdData.ClientID %>').hide();

            switch ('<%=this.Type %>') {
                case 'Add':
                    $('#btnSave').show();
                    $('#divAdd').show();
                    break;
            }
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('input[type="checkbox"], select').on('change', function () { input_change(this); });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>
