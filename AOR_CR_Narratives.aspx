﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_CR_Narratives.aspx.cs" Inherits="AOR_CR_Narratives" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Narratives</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divCR" style="padding: 10px;">
            <table style="width: 100%;">
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Description:
					</td>
					<td>
						<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray" CssClass="importLock"></asp:TextBox>
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                    <td style="width: 110px; vertical-align: top;">
                        Basis of Risk:
                    </td>
                    <td>
                        <asp:TextBox ID="txtBasisOfRisk" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Basis of Urgency:
                    </td>
                    <td>
                        <asp:TextBox ID="txtBasisOfUrgency" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Customer Impact:
                    </td>
                    <td>
                        <asp:TextBox ID="txtCustomerImpact" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Issue:
                    </td>
                    <td>
                        <asp:TextBox ID="txtIssue" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Proposed Solution:
                    </td>
                    <td>
                        <asp:TextBox ID="txtProposedSolution" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Rationale:
                    </td>
                    <td>
                        <asp:TextBox ID="txtRationale" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Workload Priority:
                    </td>
                    <td>
                        <asp:TextBox ID="txtWorkloadPriority" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
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
            refreshPage();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);
                    
                    PageMethods.Save('<%=this.CRID %>', $('#<%=this.txtNotes.ClientID %>').val(), $('#<%=this.txtBasisOfRisk.ClientID %>').val(), $('#<%=this.txtBasisOfUrgency.ClientID %>').val(),
                        $('#<%=this.txtCustomerImpact.ClientID %>').val(), $('#<%=this.txtIssue.ClientID %>').val(),
                        $('#<%=this.txtProposedSolution.ClientID %>').val(), $('#<%=this.txtRationale.ClientID %>').val(), $('#<%=this.txtWorkloadPriority.ClientID %>').val(), save_done, on_error);
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

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                MessageBox('CR Narratives have been saved.');
                parent.refreshNarratives();
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
            window.location.href = 'Loading.aspx?Page=' + window.location.href;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            if ('<%=this.CanEditCR %>'.toUpperCase() == 'TRUE') {
                $('textarea').css('color', 'black');
                $('textarea').removeAttr('readonly');
                $('#btnCancel').show();
                $('#btnSave').show();
            }

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('textarea').on('keyup paste', function () { input_change(this); });
            $('textarea').on('blur', function () { txtBox_blur(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>