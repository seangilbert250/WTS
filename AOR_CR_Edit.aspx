﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_CR_Edit.aspx.cs" Inherits="AOR_CR_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    <img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Details</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
    <img id="imgHelp" src="Images/Icons/help.png" alt="Details section cannot be edited on imported CRs" title="Details section cannot be edited on imported CRs" width="15" height="15" style="cursor: pointer; vertical-align: middle; padding-right: 5px; display: none;" />
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divCR" style="padding: 10px;">
            <table style="width: 100%;">
                <tr id="trInfo" style="display: none;">
                    <td colspan="4" style="text-align: right;">
                        <span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span>
                    </td>
                </tr>
                <tr>
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width: 125px;">
                        CR Customer Title:
                    </td>
                    <td>
                        <asp:TextBox ID="txtCRName" runat="server" MaxLength="255" Width="100%" ReadOnly="true" ForeColor="Gray" CssClass="importLock"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        CR Internal Title:
                    </td>
                    <td>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="255" Width="100%" ReadOnly="true" ForeColor="Gray" CssClass="importLock"></asp:TextBox>
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
						<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray" CssClass="importLock"></asp:TextBox>
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
                        Websystem:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlWebsystem" runat="server" Width="175px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        CSD Required Now:
                    </td>
                    <td>
                        <asp:CheckBox ID="chkCSDRequiredNow" runat="server" Enabled="false" CssClass="importLock" />
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
                        Related Release:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlRelatedRelease" runat="server" Width="175px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        Subgroup:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSubgroup" runat="server" Width="175px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        Design Review:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlDesignReview" runat="server" Width="75px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        ITI POC:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlITIPOC" runat="server" Width="175px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        Customer Priority List:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCPList" runat="server" Width="175px" Enabled="false" CssClass="importLock"></asp:DropDownList>
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
                        Government CSRD #:
                    </td>
                    <td>
                        <asp:TextBox ID="txtGovernmentCSRD" runat="server" MaxLength="6" Width="50px" ReadOnly="true" ForeColor="Gray" CssClass="importLock" style="text-align: center;"></asp:TextBox>
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
                        Primary SR #:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlPrimarySR" runat="server" Width="75px" Enabled="false" CssClass="importLock"></asp:DropDownList>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
        <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
			<img class="toggleSection" src="Images/Icons/minus_blue.png" title="Hide Section" alt="Hide Section" height="12" width="12" data-section="AssessmentPriority" style="cursor: pointer;" />&nbsp;&nbsp;Assessment/Priority:
		</div>
        <div id="divAssessmentPriority" style="padding: 10px;">
            <table style="width: 100%;">
				<tr>
					<td>
						<table style="border-collapse: collapse;">
                            <tr class="gridHeader">
                                <th style="border-left: 1px solid grey; width: 100px;">
                                    CAM Priority
                                </th>
                                <th style="width: 100px;">
                                    LCMB Priority
                                </th>
                                <th style="width: 100px;">
                                    Airstaff Priority
                                </th>
                                <th style="width: 115px;">
                                    Customer Priority
                                </th>
                                <th style="width: 100px;">
                                    ITI Priority
                                </th>
                                <th style="width: 145px;">
                                    Risk of PTS (Urgency)
                                </th>
                            </tr>
                            <tr class="gridBody">
                                <td style="border-left: 1px solid grey; text-align: center;">
                                    <asp:TextBox ID="txtCAMPriority" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtLCMBPriority" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtAirstaffPriority" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtCustomerPriority" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtITIPriority" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtRiskOfPTS" runat="server" MaxLength="4" Width="95%" ReadOnly="true" ForeColor="Gray" style="text-align: center;"></asp:TextBox>
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
        </div>
        <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
			<img class="toggleSection" src="Images/Icons/minus_blue.png" title="Hide Section" alt="Hide Section" height="12" width="12" data-section="Status" style="cursor: pointer;" />&nbsp;&nbsp;Status:
		</div>
        <div id="divStatus" style="padding: 10px;">
            <table style="width: 100%;">
                <tr>
                    <td style="padding-bottom: 10px;">
                        <table style="border-collapse: collapse;">
                            <tr class="gridHeader">
                                <th style="border-left: 1px solid grey; width: 115px;">
                                    CR Coordination
                                </th>
                            </tr>
                            <tr class="gridBody">
                                <td style="border-left: 1px solid grey; text-align: center;">
                                    <asp:DropDownList ID="ddlStatus" runat="server" Width="95%" Enabled="false"></asp:DropDownList>
                                </td>
                            </tr>
						</table>
                    </td>
                </tr>
                <tr>
                    <td>
						<table style="border-collapse: collapse;">
                            <tr class="gridHeader">
                                <th style="border-left: 1px solid grey; width: 125px;">
                                    LCMB Submitted
                                    <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                </th>
                                <th style="width: 125px;">
                                    LCMB Approved
                                    <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                </th>
                                <th style="width: 155px;">
                                    ERB (ISMT) Submitted
                                    <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                </th>
                                <th style="width: 155px;">
                                    ERB (ISMT) Approved
                                    <img src="Images/Icons/delete.png" class="clearDate" title="Clear Date" alt="Clear Date" width="12" height="12" style="cursor: pointer; display: none;" />
                                </th>
                            </tr>
                            <tr class="gridBody">
                                <td style="border-left: 1px solid grey; text-align: center;">
                                    <asp:TextBox ID="txtLCMBSubmitted" runat="server" Width="95%" ReadOnly="true" ForeColor="Gray" CssClass="date" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtLCMBApproved" runat="server" Width="95%" ReadOnly="true" ForeColor="Gray" CssClass="date" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtERBISMTSubmitted" runat="server" Width="95%" ReadOnly="true" ForeColor="Gray" CssClass="date" style="text-align: center;"></asp:TextBox>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox ID="txtERBISMTApproved" runat="server" Width="95%" ReadOnly="true" ForeColor="Gray" CssClass="date" style="text-align: center;"></asp:TextBox>
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
			</table>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsEvents" type="text/javascript">
        var blnImportAltered = 0;
        function imgRefresh_click() {
            refreshPage();
        }

        function imgHelp_click() {
            MessageBox('Details section cannot be edited on imported CRs');
        }

        function btnCancel_click() {
            refreshPage();
        }

        function btnSave_click() {
            try {
                var validation = validate();
                var blnAltered = 0;

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    if ('<%=this.Imported %>'.toUpperCase() == 'TRUE') {
                        blnAltered = blnImportAltered;
                    }
                    
                    PageMethods.Save(blnAltered,'<%=this.NewCR %>', '<%=this.CRID %>', $('#<%=this.txtCRName.ClientID %>').val(), $('#<%=this.txtTitle.ClientID %>').val(), $('#<%=this.txtNotes.ClientID %>').val(),
                        $('#<%=this.ddlWebsystem.ClientID %>').val(), ($('#<%=this.chkCSDRequiredNow.ClientID %>').is(':checked') ? 1 : 0), $('#<%=this.ddlRelatedRelease.ClientID %>').val(),
                        $('#<%=this.ddlSubgroup.ClientID %>').val(), $('#<%=this.ddlDesignReview.ClientID %>').val(), $('#<%=this.ddlITIPOC.ClientID %>').val(), $('#<%=this.ddlCPList.ClientID %>').val(),
                        $('#<%=this.txtGovernmentCSRD.ClientID %>').val(), $('#<%=this.ddlPrimarySR.ClientID %>').val(), $('#<%=this.txtCAMPriority.ClientID %>').val(),
                        $('#<%=this.txtLCMBPriority.ClientID %>').val(), $('#<%=this.txtAirstaffPriority.ClientID %>').val(), $('#<%=this.txtCustomerPriority.ClientID %>').val(), $('#<%=this.txtITIPriority.ClientID %>').val(),
                        $('#<%=this.txtRiskOfPTS.ClientID %>').val(), $('#<%=this.ddlStatus.ClientID %>').val(), $('#<%=this.txtLCMBSubmitted.ClientID %>').val(), $('#<%=this.txtLCMBApproved.ClientID %>').val(),
                        $('#<%=this.txtERBISMTSubmitted.ClientID %>').val(), $('#<%=this.txtERBISMTApproved.ClientID %>').val(), save_done, on_error);
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

                MessageBox('CR has been saved.');

                if ($('#<%=this.txtCRName.ClientID %>').val() != $('#<%=this.txtCRName.ClientID %>').attr('original_value') && parent.refreshPage) {
				    parent.refreshPage(newID);
				}
				else {
				    refreshPage(newID);
				}
            }
            else if (blnExists) {
                MessageBox('CR Customer Title already exists.');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function imgToggleSection_click(obj) {
            var $obj = $(obj);
            var section = $obj.data('section');

            if ($obj.attr('title') == 'Show Section') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Hide Section');
                $obj.attr('alt', 'Hide Section');
                $('#div' + section).show();
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Show Section');
                $obj.attr('alt', 'Show Section');
                $('#div' + section).hide();
            }
        }

        function validate() {
            var validation = [];

            if ($('#<%=this.txtCRName.ClientID %>').val().length == 0) validation.push('CR Customer Title cannot be empty.');

            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if ($obj.attr('id') && ($obj.attr('id').indexOf('GovernmentCSRD') != -1 ||
                $obj.attr('id').indexOf('HoursToFix') != -1)) {
                $obj.val(nVal.replace(/[^\d]/g, ''));
            }

            if ($obj.attr('id') && ($obj.attr('id').indexOf('CAMPriority') != -1 ||
                $obj.attr('id').indexOf('LCMBPriority') != -1 ||
                $obj.attr('id').indexOf('AirstaffPriority') != -1 ||
                $obj.attr('id').indexOf('CustomerPriority') != -1 ||
                $obj.attr('id').indexOf('ITIPriority') != -1 ||
                $obj.attr('id').indexOf('RiskOfPTS') != -1)) {
                var blnNegative = nVal.indexOf('-') != -1 ? true : false;

                nVal = nVal.replace(/[^\d]/g, '');

                if (blnNegative) nVal = '-' + nVal;

                $obj.val(nVal);
            }

            if ($obj.attr('id') && (($obj.attr('id').indexOf('CRName') != -1 ||
                $obj.attr('id').indexOf('Title') != -1) ||
                $obj.attr('id').indexOf('Notes')) != -1) {
                blnImportAltered = 1;
            }

            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if ($obj.attr('id') && ($obj.attr('id').indexOf('CAMPriority') != -1 ||
                $obj.attr('id').indexOf('LCMBPriority') != -1 ||
                $obj.attr('id').indexOf('AirstaffPriority') != -1 ||
                $obj.attr('id').indexOf('CustomerPriority') != -1 ||
                $obj.attr('id').indexOf('ITIPriority') != -1 ||
                $obj.attr('id').indexOf('RiskOfPTS') != -1)) {
                if (nVal == '-') $obj.val('');
                return;
            }

            $obj.val($.trim(nVal));
        }

        function txtDate_clear(obj) {
            var $obj = $(obj);
            var tdIndex = $obj.parent().index();

            $obj.parent().parent().next().find('td:eq(' + tdIndex + ') input[type="text"]').val('');
            $('#btnSave').prop('disabled', false);
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
        function initControls() {
            if ('<%=this.CanEditCR %>'.toUpperCase() == 'TRUE') {
                $('.date').datepicker({
                    dateFormat: 'm/d/yy'
                });
            }
        }

        function initDisplay() {
            if ('<%=this.CanEditCR %>'.toUpperCase() == 'TRUE') {
                if ('<%=this.Imported %>'.toUpperCase() == 'TRUE') {
                    $('#imgHelp').show();
                    $('input[type="text"], textarea').not('.importLock').css('color', 'black');
                    $('input[type="text"], textarea').not('.importLock, .date').removeAttr('readonly');

                    $('input[type="text"], textarea').css('color', 'black');
                    $('input[type="text"], textarea').removeAttr('readonly');

                    $('select').not('.importLock').removeAttr('disabled');
                    $('input[type="checkbox"]').parent().not('.importLock').children().removeAttr('disabled');
                }
                else {
                    $('input[type="text"], textarea').css('color', 'black');
                    $('input[type="text"], textarea').not('.date').removeAttr('readonly');
                    $('select, input[type="checkbox"]').removeAttr('disabled');
                }
                
                $('.clearDate').show();
                $('#btnCancel').show();
                $('#btnSave').show();
            }

            if ('<%=this.NewCR %>'.toUpperCase() == 'FALSE') $('#trInfo').show();

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#imgHelp').click(function () { imgHelp_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.toggleSection').on('click', function () { imgToggleSection_click(this); });
            //$('input[type="text"], textarea').not('<%=this.Imported %>'.toUpperCase() == 'TRUE' ? '.importLock, .date' : '.date').on('keyup paste', function () { input_change(this); });

            $('input[type="text"], textarea').not('<%=this.Imported %>'.toUpperCase() == 'TRUE' ? '.date' : '.date').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('.date').change(function () { input_change(this); });
            $('.clearDate').click(function () { txtDate_clear(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initControls();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>