<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SR_Edit.aspx.cs" Inherits="SR_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">SR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Details</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divSR" style="padding: 10px;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                    <td style="width: 115px;">
                        SR #:
                    </td>
                    <td>
                        <span id="spnSR" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span></div>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        Status:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlStatus" runat="server" Width="175px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        Reasoning:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlType" runat="server" Width="175px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        User's Priority:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPriority" runat="server" Width="175px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;"></span>
                    </td>
                    <td>
                        Investigation Priority:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlINVPriority" runat="server" Width="175px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red;"></span>
                    </td>
                    <td>
                        SR Rank:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSRRank" runat="server" Width="175px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top;">
                        <span style="color: red;">*</span>
                    </td>
                    <td style="vertical-align: top;">
                        Description:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="8" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function btnCancel_click() {
            var thePopup = popupManager.GetPopupByName('SR');
            if (thePopup != null) {
                thePopup.Close();
            }            
        }

        function btnSave_click() {
            try {
                var validation = validate();
                var blnAltered = 0;

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    PageMethods.Save('<%=this.NewSR %>', '<%=this.SRID %>',
                        $('#<%=this.ddlStatus.ClientID %>').val(), $('#<%=this.ddlType.ClientID %>').val(),
                        $('#<%=this.ddlPriority.ClientID %>').val(), $('#<%=this.ddlINVPriority.ClientID %>').val(),
                        $('#<%=this.ddlSRRank.ClientID %>').val(), $('#<%=this.txtDescription.ClientID %>').val(),
                        save_done, on_error);
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
                if (parent.parent._newItemCreated != undefined) parent.parent._newItemCreated = true;

                MessageBox('SR has been saved.');
                refreshPage(newID);
            }
            else if (blnExists) {
                MessageBox('SR already exists.');
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

            if ($('#<%=this.ddlStatus.ClientID %> option').length == 0 || $('#<%=this.ddlStatus.ClientID %>').val() == '0') validation.push('Please select a Status.');
            if ($('#<%=this.ddlType.ClientID %> option').length == 0 || $('#<%=this.ddlType.ClientID %>').val() == '0') validation.push('Please select a Reasoning.');
            if ($('#<%=this.ddlPriority.ClientID %> option').length == 0 || $('#<%=this.ddlPriority.ClientID %>').val() == '0') validation.push('Please select a User\'s Priority.');
            if ($('#<%=this.txtDescription.ClientID %>').val().length == 0) validation.push('Description cannot be empty.');

            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if (!$obj.is('[readonly]')) $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && (parseInt(newID) < 0 || parseInt(newID) > 0)) {
                if (parent.refreshPage) parent.refreshPage(newID);
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            if ('<%=this.CanEditSR %>'.toUpperCase() == 'TRUE') {
                $('input[type="text"], textarea').not('#<%=this.txtDescription.ClientID %>').css('color', 'black');
                $('input[type="text"], textarea').not('#<%=this.txtDescription.ClientID %>').removeAttr('readonly');
                $('select, input[type="checkbox"]').removeAttr('disabled');
                $('#btnCancel').show();
                $('#btnSave').show();
            }

            if ('<%=this.NewSR %>'.toUpperCase() == 'FALSE') $('#divInfo').show();

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>