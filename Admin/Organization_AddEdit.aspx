<%@ Page Title="" Language="C#" MasterPageFile="~/AddEdit.master" AutoEventWireup="true" CodeFile="Organization_AddEdit.aspx.cs" Inherits="Admin_Organization_AddEdit" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">Organization details</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="server">
	Organization Details
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server">
	<div id="divHeaderTop" style="width: 100%;">
		<input type="button" id="buttonCancel" onclick="buttonCancel_click(); return false;" value="Close" />
		<input type="button" id="buttonSave" runat="server" onclick="buttonSave_click(); return false;" value="Save" />
	</div>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
    <div id="divAttributes" class="pageSection" style="width:100%; padding-top:5px;">
		<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

		<asp:HiddenField ID="txtOrganizationId" runat="server" Value="0" />
		<div id="divPageContainer" class="pageContainer" style="overflow-y: scroll;">
			<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
				<tr id="trOrganization" class="attributesRow">
					<td class="attributesRequired">*</td>
					<td name="FirstLabel" class="attributesLabel">Organization: </td>
					<td class="attributesValue">
						<asp:TextBox ID="txtOrganization" runat="server" Width="150" Style="font-size: 11px;"></asp:TextBox>
						&nbsp;&nbsp;<input type="checkbox" id="chkArchive" runat="server" value="Archive" style="text-align: left; vertical-align: middle; display: none;" /><label id="labelForChkArchive" for="<%=this.chkArchive.ClientID %>" style="text-align: left; vertical-align: middle; display: none;">Archive</label>
					</td>
				</tr>
				<tr id="tdDescription" class="attributesRow">
					<td class="attributesRequired">&nbsp;</td>
					<td name="FirstLabel" class="attributesLabel" style="vertical-align:top;">Description:&nbsp;&nbsp;</td>
					<td class="attributesValue" style="vertical-align: top;">
						<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" Width="340" Style="text-align: left; font-size: 11px; font-family: Arial;" Enabled="true"></asp:TextBox>
					</td>
				</tr>
				<tr id="trSpacer" class="attributesRow">
					<td class="attributesRequired">&nbsp;</td>
					<td class="attributesLabel">&nbsp;</td>
					<td class="attributesValue">&nbsp;</td>
				</tr>
				<tr id="trRoles" class="attributesRow">
					<td class="attributesRequired">&nbsp;</td>
					<td name="FirstLabel" class="attributesLabel" style="vertical-align: top;">Roles:</td>
					<td class="attributesValue" style="vertical-align: top;">
						<asp:CheckBoxList ID="chkListRoles" runat="server" CssClass="attributes" RepeatDirection="Vertical" RepeatLayout="Table" RepeatColumns="4"></asp:CheckBoxList>
					</td>
				</tr>
				<tr id="trSpacer2" class="attributesRow">
					<td class="attributesRequired">&nbsp;</td>
					<td class="attributesLabel">&nbsp;</td>
					<td class="attributesValue">&nbsp;</td>
				</tr>
				<tr id="trUsers" class="attributesRow">
					<td class="attributesRequired">&nbsp;</td>
					<td name="FirstLabel" class="attributesLabel" style="width: 100%; vertical-align:top;">
						Associated Users: 
					</td>
					<td class="attributesValue" style="vertical-align:top;">
						<asp:ListBox ID="lstUsers" runat="server" CssClass="attributes" Height="70" Width="345"></asp:ListBox>
					</td>
				</tr>
			</table>
		</div>
    </div>
    
    <script type="text/javascript">
        function refreshPage(OrganizationId) {
            if (OrganizationId === undefined || OrganizationId === null) {
                OrganizationId = '';
            }
            var url = window.location.href;
            url = editQueryStringValue(url, 'OrganizationId', OrganizationId);

            window.location.href = url;
        }

        function closePage() {
            if (closeWindow) {
                closeWindow();
            }
            else {
                window.close();
            }
        } //end closePage()

        function getSelectedRoles() {
            var roles = new Array();
            $("#<%=this.chkListRoles.ClientID%> input[type=checkbox]:checked").each(function () {
                roles.push($(this).val());
            });

            return roles;
        }

        function buttonSave_click() {
            saveOrganization();

            return false;
        } //end btnSave_click()

        function saveOrganization() {
            var Organization = new Object();
            Organization.OrganizationId = $('#<%=this.txtOrganizationId.ClientID %>').val() == '' ? 0 : parseInt($('#<%=this.txtOrganizationId.ClientID %>').val());
            Organization.Organization = $('#<%=this.txtOrganization.ClientID %>').val();
            Organization.description = $('#<%=this.txtDescription.ClientID %>').val();
            Organization.archive = $('#<%=this.chkArchive.ClientID %>').is(':checked');
            Organization.roles = getSelectedRoles().join(',');

            PageMethods.SaveOrganization(Organization.OrganizationId
				, Organization.Organization
				, Organization.description
				, Organization.archive
				, Organization.roles
				, save_done, on_error);
        }

        function save_done(result) {
            var obj = jQuery.parseJSON(result);
            var saved = '', errorMsg = '', id = '', firstName = '', lastName = '';

            $.each(obj, function (index, val) {
                //do something with data
                switch (index.toUpperCase()) {
                    case 'SAVED':
                        saved = val;
                        break;
                    case 'ERROR':
                        errorMsg = val;
                        break;
                    case 'ID':
                        id = val;
                        break;
                    case 'Organization':
                        firstName = val;
                        break;
                }
            });

            if (saved.toUpperCase() == 'TRUE') {
                alert('Successfully saved Organization');
                $('#<%=this.txtOrganizationId.ClientID %>').val(id);
            }
            else {
                alert('Failed to save Organization.  '
                    + '\n' + '\n' + errorMsg);
                return false;
            }

            if (opener.refreshGrid) {
                opener.refreshGrid();
            }
            else if (opener.addOrganizationToDdl) {
                var OrganizationObj = new Object();
                OrganizationObj.ID = id;
                OrganizationObj.Name = firstName + ' ' + lastName;
                opener.addOrganizationToDdl(OrganizationObj);
            }

            refreshPage(id);
        }  //end save_done()

        function on_error(result) {
            var resultText = 'An error occurred when communicating with the server'
            + '\n' +
            'readyState = ' + result.readyState + '\n' +
            'responseText = ' + result.responseText + '\n' +
            'status = ' + result.status + '\n' +
            'statusText = ' + result.statusText;
            ;

            alert('save error:  \n' + resultText);
        }

        function buttonCancel_click() {
            //todo: confirmation dialog if changes have been made

            closePage();

            return false;
        } //end buttonCancel_click()

        function resizePage() {
        	resizePageElement('divPageContainer', 5);
        }

        $(document).ready(function () {
        	$('#imgRefresh').hide();
            $('[name="FirstLabel"]').width(85);

            $('#buttonSave').bind('click', function () { buttonSave_click(); });
            $('#buttonCancel').bind('click', function () { buttonCancel_click(); });

            if ('<%=this.ViewOnly %>'.toUpperCase() != 'TRUE') {
                $('#<%=this.chkArchive.ClientID %>').show();
                $('#labelForChkArchive').show();
            }
            else {
                $('#buttonSave').hide();
            }

        	resizePage();
        	$(window).resize(resizePage);

        	$(document.body).bind('onbeforeunload', function () { closePage(); });
        });
	</script>
</asp:Content>

