<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="UserProfile_Options.aspx.cs" Inherits="Admin_UserProfile_Options" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">User Custom Options</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<link rel="stylesheet" href="../Styles/jquery-ui.css" />
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
	<img src="../Images/Icons/pencil.png" alt="Review/Edit User Custom Options" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">User Custom Options</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server"></asp:Content>
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
						<tr id="trMainView" class="attributesRow" style="Display:none">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Main View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlMainView" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>						
						<tr id="trAORView" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Default AOR View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_AOR" runat="server" Style="font-size: 12px; padding-left: 0px; width: 135px" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
						<tr id="trCrosswalkView" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">Default Crosswalk View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_Crosswalk" runat="server" Style="font-size: 12px; padding-left: 0px; width: 135px" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>						
						<tr id="trSystemView" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">Default View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_Main" runat="server" Style="font-size: 12px; padding-left: 0px; width: 135px" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
						<tr id="trWorkloadView" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">Default Workload View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_Workload" runat="server" Style="font-size: 12px; padding-left: 0px; width: 135px" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>

						<tr id="trWorkRequestView" class="attributesRow" style="visibility:hidden">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">Default Work Request View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_Request" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
						<tr id="trHotlistView" class="attributesRow" style="visibility:hidden">
							<td class="attributesRequired">&nbsp;</td>
							<td name="FirstLabel" class="attributesLabel">Default Hotlist View:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlView_Hotlist" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
									<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
								</asp:DropDownList>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>

	<script id="jsVariables" type="text/javascript">
		var _userID = 0;
		var _membershipUserID = '';

		var _isAdmin = false, _isCurrentUser = false;
	</script>

	<script id="jsAJAX" type="text/javascript">

		function optionChanged(element) {
			var changed = false;

			var orig = '';
			if ($(element).attr('original_value')) {
				orig = $(element).attr('original_value');
			}

			if ($(element).val() != orig) {
				changed = true;
			}

			return changed;
		}

		function getOption(element) {
			var option = [];

			var userSettingID = "0", userSettingTypeID = "0", gridNameID = "0";

			if ($(element).attr('UserSettingID')) {
				userSettingID = $(element).attr('UserSettingID');
			}
			if ($(element).attr('UserSettingTypeID')) {
				userSettingTypeID = $(element).attr('UserSettingTypeID');
			}
			if ($(element).attr('GridNameID')) {
				gridNameID = $(element).attr('GridNameID');
			}

			option.push('"UserSettingID":"' + userSettingID + '"');
			option.push('"UserSettingTypeID":"' + userSettingTypeID + '"');
			option.push('"GridNameID":"' + gridNameID + '"');
			option.push('"SettingValue":"' + $(element).val() + '"');

			return '{' + option.join(',') + '}';
		}

		function save() {
			try {
				ShowDimmer(true, 'Saving...');

				var options = [];
				// 12-28-16 - Users will not have data saved yet, so send everything and let the stored procedure handle Insert/Update.
<%--					options.push(getOption($('#<%=this.ddlMainView.ClientID %>')));--%>

				options.push(getOption($('#<%=this.ddlView_Main.ClientID %>')));
				options.push(getOption($('#<%=this.ddlView_Workload.ClientID %>')));
				options.push(getOption($('#<%=this.ddlView_Crosswalk.ClientID %>')));
				options.push(getOption($('#<%=this.ddlView_AOR.ClientID %>')));


<%--					options.push(getOption($('#<%=this.ddlView_Request.ClientID %>')));
					options.push(getOption($('#<%=this.ddlView_Hotlist.ClientID %>')));--%>

<%--				if (optionChanged($('#<%=this.ddlView_Main.ClientID %>'))) {
					options.push(getOption($('#<%=this.ddlView_Main.ClientID %>')));
				}
				if (optionChanged($('#<%=this.ddlView_Workload.ClientID %>'))) {
					options.push(getOption($('#<%=this.ddlView_Workload.ClientID %>')));
				}
				if (optionChanged($('#<%=this.ddlView_Crosswalk.ClientID %>'))) {
					options.push(getOption($('#<%=this.ddlView_Crosswalk.ClientID %>')));
				}
				if (optionChanged($('#<%=this.ddlView_Request.ClientID %>'))) {
					options.push(getOption($('#<%=this.ddlView_Request.ClientID %>')));
				}
				if (optionChanged($('#<%=this.ddlView_Hotlist.ClientID %>'))) {
					options.push(getOption($('#<%=this.ddlView_Hotlist.ClientID %>')));
				}--%>

				var json = '[' + options.join(',') + ']';

				PageMethods.SaveUserOptions(_userID, json
					, save_done, on_error);

			} catch (e) {
				ShowDimmer(false);
			}
		}
		function save_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);
				var saved = 0, failed = 0;
				var errorMsg = '', ids = '', failedIds = '';

				if (obj) {
					if (obj.saved) {
						saved = parseInt(obj.saved);
					}
					if (obj.failed) {
						failed = parseInt(obj.failed);
					}
					if (obj.savedIds) {
						ids = obj.savedIds;
					}
					if (obj.failedIds) {
						failedIds = obj.failedIds;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				var msg = '';
				if (errorMsg.length > 0) {
					msg = 'An error occurred while saving: \n' + errorMsg;
				}

				if (saved > 0) {
					msg = 'Successfully saved user options. Module view will take affect next login.';
					//msg = 'Successfully saved ' + saved + ' user options.';
				}
				if (failed > 0) {
					msg += '\n' + 'Failed to save user options.';
					//msg += '\n' + 'Failed to save ' + failed + ' user options.';
				}
				MessageBox(msg);

				if (!opener) {
					opener = parent;
				}

				if (opener) {
					if (opener.refreshGrid) {
						opener.refreshGrid();
					}
				}

				refreshPage();
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

	<script id="jsEvents" type="text/javascript">

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
		}

		function buttonCancel_click() {
			document.location.href = document.location.href;
		}

		function buttonSave_click() {
			var valid = true;
			//TODO: perform validation

			if (valid) {
				save();
			}
		}

		function resizePage() {
			resizePageElement('divPageContainer', 5);
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initializeVariables() {

			try {
				_userID = parseInt('<%=this.UserId %>');
				_membershipUserID = '<%=this.MembershipUserId %>';
				_isAdmin = ('<%=this.IsUserAdmin %>'.toUpperCase() == 'TRUE');
				_isCurrentUser = ('<%=this.IsCurrentUser %>'.toUpperCase() == 'TRUE');

			} catch (e) {

			}

		}

		$(document).ready(function () {
			$('#imgRefresh').hide();
			$('[name="FirstLabel"]').width(160);

			initializeVariables();

			$('#buttonCancel').click(function () { buttonCancel_click(); return false; });

			if (_isAdmin || _isCurrentUser) {
				$('#buttonSave').attr('disabled', false);
				$('#buttonSave').prop('disabled', false);
				$('#buttonSave').click(function () { buttonSave_click(); return false; });
			}

			resizePage();
			$(window).resize(resizePage);

			$(document.body).bind('onbeforeunload', function () { closePage(); });
		});

	</script>
</asp:Content>