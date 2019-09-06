<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Edit.aspx.cs" Inherits="AOR_Meeting_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Meeting</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" href="Styles/jquery-ui-timepicker-addon.css" />
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server"><span id="spnMeetingEditHeader">Details</span></asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnStartMeeting" value="Start Meeting" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divAORMeeting" style="padding: 10px;">
            <table style="width: 100%;">
                <tr id="trMeetingNumber" style="display:none;">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        Meeting #:
                    </td>
                    <td>
                        <span id="spnAORMeeting" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span></div>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 5px;"> <!-- NOTE: WIDTHS ARE ON THE SECOND TR INSTEAD OF THE FIRST BECAUSE THE FIRST IS SOMETIMES HIDDEN -->
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width: 125px;">
                        Meeting Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtAORMeetingName" runat="server" MaxLength="150" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr id="trInstanceDateRow" style="display:none";>
                    <td>
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        Meeting Date:
                    </td>
                    <td>
                        <asp:TextBox ID="txtInstanceDate" runat="server" Width="150px" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                        <div id="divInstanceDateConflict" style="display:none;color:red;">&nbsp;* Invalid date. This date conflicts with another meeting.</div>
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
                        Description:
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" MaxLength="500" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr id="trMeetingNotes" style="display:none;">
                    <td>
                        &nbsp;
                    </td>
                    <td style="vertical-align: top;">
                        Notes:
                    </td>
                    <td>
                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
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
                        Frequency:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlFrequency" runat="server" Width="150px" Enabled="false"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr id="trAutoCreateMeetings" style="display: none;">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        Auto Create Meetings:
                    </td>
                    <td>
                        <asp:CheckBox ID="chkAutoCreateMeetings" runat="server" Enabled="false" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr style="display: none;">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        Private:
                    </td>
                    <td>
                        <asp:CheckBox ID="chkPrivate" runat="server" Enabled="false" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script src="Scripts/jquery-ui-timepicker-addon.js"></script>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _newMeeting;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function btnCancel_click() {
            refreshPage();
        }

        function btnStartMeeting_click() {
            btnSave_click();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);
                    
                    PageMethods.Save('<%=this.NewAORMeeting %>', '<%=this.AORMeetingID %>', $('#<%=this.txtAORMeetingName.ClientID %>').val(), $('#<%=this.txtDescription.ClientID %>').val(),
                        $('#<%=this.txtNotes.ClientID %>').val(), $('#<%=this.ddlFrequency.ClientID %>').val(), ($('#<%=this.chkAutoCreateMeetings.ClientID %>').is(':checked') ? 1 : 0),
                        ($('#<%=this.chkPrivate.ClientID %>').is(':checked') ? 1 : 0), $('#<%=this.txtInstanceDate.ClientID %>').val(), save_done, on_error);
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
            var newMeetingInstanceID = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
                if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
                if (obj.newMeetingInstanceID && parseInt(obj.newMeetingInstanceID) > 0) newMeetingInstanceID = obj.newMeetingInstanceID;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                if (parent.parent._newItemCreated != undefined) parent.parent._newItemCreated = true;

                if (_newMeeting) {
                    // re-direct to the meeting start
                    var nURL = _pageUrls.Maintenance.AORMeetingInstanceEdit + window.location.search;

                    nURL = editQueryStringValue(nURL, 'AORMeetingID', newID);
                    nURL = editQueryStringValue(nURL, 'NewAORMeeting', 'false');
                    nURL = editQueryStringValue(nURL, 'NewAORMeetingInstance', 'false');
                    nURL = editQueryStringValue(nURL, 'AORMeetingInstanceID', newMeetingInstanceID);

                    parent.window.location = nURL;
                }
                else {
                    MessageBox('Meeting has been saved.');

                    if ($('#<%=this.txtAORMeetingName.ClientID %>').val() != $('#<%=this.txtAORMeetingName.ClientID %>').attr('original_value') && parent.refreshPage) {
                        parent.refreshPage(newID);
                    }
                    else {
                        refreshPage(newID);
                    }
                }
            }
            else if (blnExists) {
                MessageBox('Meeting Name already exists.');
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

            if ($('#<%=this.txtAORMeetingName.ClientID %>').val().length == 0) validation.push('Meeting Name cannot be empty.');

            if (_newMeeting) {
                if ($('#<%=this.txtInstanceDate.ClientID %>').val().length == 0) validation.push('Meeting Date cannot be empty.');
            }

            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            <%--if ($obj.attr('id') && $obj.attr('id').indexOf('Frequency') != -1) {
                var nVal = $obj.find('option:selected').text();

                if (nVal == 'Daily' || nVal == 'Weekly') {
                    $('#trAutoCreateMeetings').show();
                }
                else {
                    $('#<%=this.chkAutoCreateMeetings.ClientID %>').prop('checked', false);
                    $('#trAutoCreateMeetings').hide();
                }
            }--%>

            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && parseInt(newID) > 0) {
                if (parent.refreshPage) parent.refreshPage(newID);
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _newMeeting = ('<%=this.NewAORMeeting %>'.toUpperCase() == 'TRUE');
        }

        function initDisplay() {
            if ('<%=this.CanEditAORMeeting %>'.toUpperCase() == 'TRUE') {
                $('input[type="text"], textarea').css('color', 'black');
                $('input[type="text"], textarea').removeAttr('readonly');
                $('select, input[type="checkbox"]').removeAttr('disabled');
                $('#btnCancel').show();                
            }

            var frequency = $('#<%=this.ddlFrequency.ClientID %> option:selected').text();

            //if (frequency == 'Daily' || frequency == 'Weekly') $('#trAutoCreateMeetings').show();

            if (_newMeeting) {
                $('#spnMeetingEditHeader').html('Create New Meeting');
                $('#trMeetingNumber').hide();
                $('#trMeetingNotes').hide();
                $('#trInstanceDateRow').show();
                $('#btnStartMeeting').show();

                $('#<%=this.txtInstanceDate.ClientID %>').datetimepicker({
                    controlType: 'select',
                    dateFormat: 'm/d/yy',
                    timeFormat: 'h:mm TT'
                });

                var dt = new moment();
                var min = dt.minutes() % 10;
                
                if (min >= 1 && min <= 4) {
                    dt.subtract(min, 'minutes');
                }
                else if (min >= 6 && min <= 9) {
                    min += dt.add(10 - min, 'minutes');
                }

                $('#<%=this.txtInstanceDate.ClientID %>').val(dt.format('MM/DD/YYYY hh:mm A'));
            }
            else {
                $('#divInfo').show();
                $('#btnSave').show();
            }

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnStartMeeting').click(function () { btnStartMeeting_click(); return false; });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>