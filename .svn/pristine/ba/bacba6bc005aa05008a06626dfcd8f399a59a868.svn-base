<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SR_Add.aspx.cs" Inherits="SR_Add" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Sustainment Request</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Sustainment Request</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle;" />
    <input type="button" id="btnSave" value="Submit" disabled="disabled" style="vertical-align: middle;" />
    &nbsp;
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divSR" style="padding: 10px;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width: 100px;">
                        Submitted By:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSubmittedBy" runat="server" Width="175px"></asp:DropDownList>
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
                        Reasoning:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlType" runat="server" Width="175px"></asp:DropDownList>
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
                        <asp:DropDownList ID="ddlPriority" runat="server" Width="175px"></asp:DropDownList>
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
						<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" Width="99%"></asp:TextBox>
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        Attachments:
                    </td>
                    <td>
                        <asp:FileUpload ID="fileUpload" runat="server" Width="100%" AllowMultiple="True" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
            <asp:Button ID="btnSubmit" runat="server" style="display: none;" />
        </div>
    </div>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function btnCancel_click() {
            closeWindow();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Submitting...', 1);

                    $('#<%=this.btnSubmit.ClientID %>').trigger('click');
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

        function complete(result) {
            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                MessageBox('SR has been submitted.');
                setTimeout(closeWindow, 1);
            }
            else {
                MessageBox('Failed to submit. <br>' + errorMsg);
            }
        }

        function validate() {
            var validation = [];

            if ($('#<%=this.ddlSubmittedBy.ClientID %> option').length == 0 || $('#<%=this.ddlSubmittedBy.ClientID %>').val() == '0') validation.push('Please select a Submitted By.');
            if ($('#<%=this.ddlType.ClientID %> option').length == 0 || $('#<%=this.ddlType.ClientID %>').val() == '0') validation.push('Please select a Reasoning.');
            if ($('#<%=this.ddlPriority.ClientID %> option').length == 0 || $('#<%=this.ddlPriority.ClientID %>').val() == '0') validation.push('Please select a User\'s Priority.');
            if ($('#<%=this.txtDescription.ClientID %>').val().length == 0) validation.push('Description cannot be empty.');

            return validation.join('<br>');
        }

        function input_change(obj) {
            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage() {
            var nURL = window.location.href;

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
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