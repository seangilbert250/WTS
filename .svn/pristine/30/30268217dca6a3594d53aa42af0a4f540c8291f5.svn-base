<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReleaseSession_Wizard.aspx.cs" Inherits="ReleaseSession_Wizard" MasterPageFile="~/Popup.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Session Wizard</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2017 Infinite Technologies, Inc. -->        
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divSessionWizard" runat="server" style="position: absolute; width: 100%; background-color: #ffffff;">
            <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td class="pageContentInfo" style="text-align: right; padding-right: 5px;">
                        <input type="button" id="buttonCreate" value="Create Sessions" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="padding: 5px; width: 100%;">
                        
                            <tr>
                                <td style="width: 120px;">
                                    Number of Sessions:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSessionsAmount" runat="server" type="number" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Session Start Date:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStartDateWizard" runat="server" autocomplete="off" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Session End Date:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtEndDateWizard" runat="server" autocomplete="off" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Session Duration:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDurationWizard" runat="server" type="number" Width="100px"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>

    <script id="jsVariables" type="text/javascript">

    </script>
    <script id="jsEvents" type="text/javascript">
        function buttonCreate_click() {
            var startDate = new Date($('#<%=this.txtStartDateWizard.ClientID%>').val());
            var endDate = new Date($('#<%=this.txtEndDateWizard.ClientID%>').val());
            var duration = $('#<%=this.txtDurationWizard.ClientID%>').val();
            var amount = $('#<%=this.txtSessionsAmount.ClientID%>').val();

            if (startDate instanceof Date && !isNaN(startDate) && endDate instanceof Date && !isNaN(endDate) && duration && duration > 0 && amount && amount > 0) {
                for (var i = 0; i < amount; i++) {
                    opener.buttonNew_click($.datepicker.formatDate('mm/dd/yy', startDate), $.datepicker.formatDate('mm/dd/yy', endDate), duration, i === 0, (i !== 0 && i === amount - 1));
                    startDate.setDate(startDate.getDate() + parseInt(duration));
                    endDate.setDate(endDate.getDate() + parseInt(duration));
                }

                closeWindow();
                opener.save();
            } else if (duration && duration <= 0){
                MessageBox('Please enter a valid duration.');
            } else if (amount && amount <= 0) {
                MessageBox('Please enter a valid number of sessions.');
            } else {
                MessageBox('All fields are required.');
            }
        }

        function updateDuration() {
            var startDate = $('#<%=this.txtStartDateWizard.ClientID%>').datepicker('getDate');
            var endDate = $('#<%=this.txtEndDateWizard.ClientID%>').datepicker('getDate');
            var day = 86400000;
            var duration = 0;

            if (startDate && endDate && startDate < endDate) duration = Math.round(Math.abs((endDate.getTime() - startDate.getTime()) / (day)));

            $('#<%=this.txtDurationWizard.ClientID%>').val(duration);
            updateEndDate($('#<%=this.txtDurationWizard.ClientID%>'));
        }

        function updateEndDate() {
            var startDate = $('#<%=this.txtStartDateWizard.ClientID%>').datepicker('getDate');
            var duration = $('#<%=this.txtDurationWizard.ClientID%>').val();
            var endDate = new Date(startDate);

            if (startDate) {
                endDate.setDate(startDate.getDate() + parseInt(duration));
                $('#<%=this.txtEndDateWizard.ClientID%>').val($.datepicker.formatDate('mm/dd/yy', endDate));
            }
        }
    </script>
    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {
            $('#<%=this.txtStartDateWizard.ClientID%>').datepicker({
                showAnim: ""
                , changeMonth: true
                , showOtherMonths: true
                , selectOtherMonths: true
                , changeYear: true
                , onSelect: function () { updateDuration(); }
            });

            $('#<%=this.txtEndDateWizard.ClientID%>').datepicker({
                showAnim: ""
                , changeMonth: true
                , showOtherMonths: true
                , selectOtherMonths: true
                , changeYear: true
                , onSelect: function () { updateDuration(); }
            });

            $('#<%=this.txtDurationWizard.ClientID%>').on('change', function () { updateEndDate($(this)); });
        }

        function initEvents() {
            $('#buttonCreate').click(function (event) { buttonCreate_click(); return false; });
        }

        $(document).ready(function () {
            initEvents();
            initVariables();
            initControls();
        });
    </script>
</asp:Content>