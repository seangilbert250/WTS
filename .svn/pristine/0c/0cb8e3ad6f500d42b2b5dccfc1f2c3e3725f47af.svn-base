<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Home.aspx.cs" Inherits="AOR_Home" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Home</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divCurrentRelease" style="padding: 10px;">
            <table>
                <tr>
                    <td>
                        Current Release:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCurrentRelease" runat="server" Width="125px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsEvents" type="text/javascript">
        function btnSave_click() {
            try {
                ShowDimmer(true, 'Saving...', 1);
                    
                PageMethods.Save($('#<%=this.ddlCurrentRelease.ClientID %>').val(), save_done, on_error);
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                $('#btnSave').prop('disabled', true);
                MessageBox('Current Release has been saved.');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
                refreshPage();
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function input_change(obj) {
            $('#btnSave').prop('disabled', false);
        }

        function refreshPage() {
            window.location.href = 'Loading.aspx?Page=' + window.location.href;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            $('#divPage').hide();

            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('select').removeAttr('disabled');
                $('#btnSave').show();
            }

            resizePage();
        }

        function initEvents() {
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('select').on('change', function () { input_change(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>