<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="UserProfile_Roles.aspx.cs" Inherits="Admin_UserProfile_Roles" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">User Roles</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
	<img src="../Images/Icons/pencil.png" alt="Review/Edit User Roles" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">User Roles</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server">
</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="buttonCancel" value="Cancel" style="padding: 2px; width: 47px;" />
	<input type="button" id="buttonSave" value="Save" disabled="disabled" style="padding: 2px; width: 42px;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	
	<div id="divPageContainer" class="pageContainer" style="overflow-y: scroll;">
		<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
			<tr>
				<td id="tdLeftCol" style="width: 40%; vertical-align: top;">
					<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trOrganization" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td name="FirstLabel" class="attributesLabel">Organization:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlOrganization" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true"></asp:DropDownList>
							</td>
						</tr>
						<tr id="trSpacer3" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
						<tr id="trApprovedLocked" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">
								<input type="checkbox" id="chkApproved" runat="server" title="Flag for user registration and approval status" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="labelForChkApproved" runat="server" AssociatedControlID="chkApproved" Text="Approved" ToolTip="Flag for user registration and approval status" />
								<input type="checkbox" id="chkLocked" runat="server" title="Flag if user has been locked out of the system" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="labelForChkLocked" runat="server" AssociatedControlID="chkLocked" Text="Locked" ToolTip="Flag if user has been locked out of the system" />
								<a id="buttonUnlock" runat="server" type="text/html" href="#" class="btn_Link" title="Unlock user account" style="padding-left: 5px; padding-right: 5px; display: none;">Unlock account</a>
								<input type="checkbox" id="chkArchive" runat="server" title="User account archive status" value="Archive" style="text-align: left; vertical-align: middle;" /><asp:Label ID="labelForChkArchive" runat="server" AssociatedControlID="chkArchive" Text="Archive" ToolTip="User account archive status" />
							</td>
						</tr>
						<tr id="trSpacer5" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
					</table>
				</td>
				<td id="tdRightCol" style="vertical-align: top;">
					<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trTheme" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
						<tr id="trSpacer4" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
						<tr id="trOptions" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
						<tr id="trSpacer6" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="text-align: left; vertical-align: top;">
					<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trRoles" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel" style="vertical-align: top;">Roles:</td>
							<td class="attributesValue" style="vertical-align: top;">
								<asp:CheckBoxList ID="chkListRoles" runat="server" CssClass="attributes" RepeatDirection="Vertical" RepeatLayout="Table" RepeatColumns="4"></asp:CheckBoxList>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>

	</div>

	<script id="jsSave" type="text/javascript">

		function buttonSave_click() {
			save();
        }

        function getSelectedRoles() {
        	var roles = new Array();
        	$("#<%=this.chkListRoles.ClientID%> input[type=checkbox]:checked").each(function () {
            	roles.push($(this).val());
            });

            return roles;
		}
		
		function save() {
			try {
				ShowDimmer(true, 'Saving...');

				var organizationId = 0;
				var roles = '';
				var archive = false;

				organizationId = parseInt($('#<%=this.ddlOrganization.ClientID %> option:selected').val());
				roles = getSelectedRoles().join(',');
				archive = $('#<%=this.chkArchive.ClientID %>').is(':checked');

				PageMethods.SaveProfile(_userID, _membershipUserID
					, organizationId, roles, archive
					, save_done, on_error);

			} catch (e) {
				ShowDimmer(false);
			}
		}
		function save_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);
				var saved = '', errorMsg = '', id = '', username = '';
				var changedTheme = 'false';

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
					}
				});

				if (saved.toUpperCase() == 'TRUE') {
					MessageBox('Successfully saved user roles.');

        			if (!opener) {
        				opener = parent;
        			}

        			if (opener) {
        				if (opener.refreshGrid) {
        					opener.refreshGrid();
        				}
        			}

					refreshPage(id);
				}
				else {
					MessageBox('Failed to save user roles. \n \n' + errorMsg);
					return false;
				}
			}
			catch (e) {
				//TODO: since the error is in the "done" procedure, log message instead of displaying
			}
		}

		function on_error(result) {
			ShowDimmer(false);

			var resultText = 'An error occurred when communicating with the server';/*\n' +
				'readyState = ' + result.readyState + '\n' +
				'responseText = ' + result.responseText + '\n' +
				'status = ' + result.status + '\n' +
				'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

	</script>

	<script id="jsMembership" type="text/javascript">
		function chkApproved_change() {
			var approved = $('#<%=this.chkApproved.ClientID %>').is(':checked');

			try {
				ShowDimmer(true, 'Saving...');

				PageMethods.ChangeUserApproval(_membershipUserID, approved
					, changeApproval_done, on_error);
			} catch (e) {
				ShowDimmer(false);
			}
		}
		function changeApproval_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);
				var success = '', approved = '', error = '';

				$.each(obj, function (index, val) {
					//do something with data
					switch (index.toUpperCase()) {
						case 'SUCCESS':
							success = val;
							break;
						case 'APPROVED':
							approved = val;
							break;
						case 'ERROR':
							error = val;
							break;
					}
				});

				if (success.toUpperCase() == 'TRUE') {
					//done
				}
				else {
					//TODO: log or show error
				}
			} catch (e) {
				ShowDimmer(false);
			}
		}

		function chkLocked_change() {
			//no implementation needed
			return false;
		}

		function buttonUnlock_click() {
			try {
				ShowDimmer(true, 'Saving...');

				PageMethods.UnlockUser(_membershipUserID
					, unlock_done, on_error);
			} catch (e) {
				ShowDimmer(false);
			}
		}
		function unlock_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);
				var success = '', locked = '', error = '';

				$.each(obj, function (index, val) {
					//do something with data
					switch (index.toUpperCase()) {
						case 'SUCCESS':
							success = val;
							break;
						case 'LOCKED':
							locked = val;
							break;
						case 'ERROR':
							error = val;
							break;
					}
				});

				if (success.toUpperCase() == 'TRUE') {
					$('#<%=this.chkLocked.ClientID %>').prop('checked', false);
				}
				else {
        				//TODO: log or show error
				}
			} catch (e) {
				ShowDimmer(false);
			}
		}

		function ddlOrganization_change() {
			//load and select default roles
			var OrganizationId = parseInt($('#<%=this.ddlOrganization.ClientID %> option:selected').val());

			try {
				ShowDimmer(true, 'Loading default roles...');

				PageMethods.GetOrganizationDefaultRoles(OrganizationId
					, getOrganizationDefaultRoles_done, on_error);
			} catch (e) {
				ShowDimmer(false);
			}

			return false;
		}
		function getOrganizationDefaultRoles_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);
				var success = '', rolesComma = '', error = '';

				$.each(obj, function (index, val) {
					//do something with data
					switch (index.toUpperCase()) {
						case 'SUCCESS':
							success = val;
							break;
						case 'ROLES':
							rolesComma = val;
							break;
						case 'ERROR':
							error = val;
							break;
					}
				});

				if (success.toUpperCase() == 'TRUE') {
					$('#<%=this.chkListRoles.ClientID%> input[type="checkbox"]').prop('checked', false);

					var roles = new Array();
					roles = rolesComma.split(',');
					var len = roles.length;
					$('#<%=this.chkListRoles.ClientID%> input[type="checkbox"]').each(function () {
        				if ($.inArray($(this).val(), roles) > -1) {
        					$(this).prop('checked', true);
        				}
        			});
				}
				else {
					//ignore. the user admin can just select the desired roles
				}
			} catch (e) {
				ShowDimmer(false);
			}
		}

	</script>

	<script id="jsInit" type="text/javascript">
		var _emptyGuid = '00000000-0000-0000-0000-000000000000';
		var _userID = 0;
		var _membershipUserID = '';

		function refreshPage() {
			window.location.href = window.location.href;
		}

		function closePage() {
			if (closeWindow) {
				closeWindow();
			}
			else {
				window.close();
			}
		} //end closePage()

		function buttonCancel_click() {
			document.location.href = document.location.href;
		}

		function resizePage() {
			resizePageElement('divPageContainer', 5);
		}

		$(document).ready(function () {
			$('#imgRefresh').hide();
			$('[name="FirstLabel"]').width(80);

			try {
				_userID = parseInt('<%=this.UserId %>');
				_membershipUserID = '<%=this.MembershipUserId %>';
			} catch (e) {

			}

			$('#buttonCancel').click(function () { buttonCancel_click(); return false; });

            if ('<%=this.IsUserAdmin %>'.toUpperCase() == 'TRUE') {
        		$('#trApprovedLocked').prop('enabled', true);
        		$('#trApprovedLocked').prop('disabled', false);

        		$('#<%=this.chkApproved.ClientID %>').attr('enabled', true);
        		$('#<%=this.chkApproved.ClientID %>').bind('change', function () { chkApproved_change(); return false; });

        		$('#<%=this.buttonUnlock.ClientID %>').attr('disabled', '');
        		$('#<%=this.buttonUnlock.ClientID %>').show();
        		$('#<%=this.buttonUnlock.ClientID %>').click(function () { buttonUnlock_click(); return false; });

            	$('#<%=this.ddlOrganization.ClientID %>').bind('change', function () { ddlOrganization_change(); return false; });

            	$('#buttonSave').attr('disabled', false);
            	$('#buttonSave').prop('disabled', false);
            	$('#buttonSave').click(function () { buttonSave_click(); return false; });
        	}
        	else {
        		$('#trApprovedLocked').prop('enabled', false);
        		$('#trApprovedLocked').prop('disabled', true);
        	}

        	resizePage();
        	$(window).resize(resizePage);

        	$(document.body).bind('onbeforeunload', function () { closePage(); });
        });

	</script>
</asp:Content>