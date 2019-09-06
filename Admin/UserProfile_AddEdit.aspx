﻿<%@ Page Title="" Language="C#" MasterPageFile="~/AddEdit.master" AutoEventWireup="true" CodeFile="UserProfile_AddEdit.aspx.cs" Inherits="UserProfile_AddEdit" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">User Profile</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>


    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" /> 

</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="server">
    User Profile
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server">
	<table id="tableChildHeader" style="width:100%; text-align:right;">
		<tr>
			<td style="height:30px; text-align:right;">
				<input type="button" id="buttonCancel" onclick="buttonCancel_click(); return false;" value="Cancel" title="Clear All Changes" style="padding: 2px;" />
				<input type="button" id="buttonSave" runat="server" onclick="buttonSave_click(); return false;" value="Save" style="padding: 2px;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<asp:HiddenField ID="txtUserId" runat="server" Value="0" />
    <asp:HiddenField ID="txtMembership_UserId" runat="server" Value="" />
	<div id="divPageContainer" class="pageContainer" style="overflow-y:auto;">
		<table id="tableAttributes" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
			<tr>
				<td id="tdLeftColumn" style="width: 50%; vertical-align: top;">
					<table style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trUsername" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Username:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtUsername" runat="server" Width="150" BackColor="#cccccc" onkeydown="return false;" />
							</td>
						</tr>

						<tr id="trPrefix" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Prefix:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtPrefix" runat="server" Width="150"></asp:TextBox>
							</td>
						</tr>
						<tr id="trFirstName" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">First Name:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtFirstName" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
							</td>
						</tr>
						<tr id="trMiddleName" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Middle Name:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtMiddleInit" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
							</td>
						</tr>
						<tr id="trLastName" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Last Name:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtLastName" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
							</td>
						</tr>
						<tr id="trSuffix" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Suffix:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtSuffix" runat="server" Width="150"></asp:TextBox>
							</td>
						</tr>

						<tr id="trPhoneOffice" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Phone:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtPhone" runat="server" Width="115"></asp:TextBox>
							</td>
						</tr>
						<tr id="trPhoneMobile" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Mobile:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtPhone_Mobile" runat="server" Width="115"></asp:TextBox>
							</td>
						</tr>
						<tr id="trPhoneMisc" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Misc:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtPhone_Misc" runat="server" Width="115"></asp:TextBox>
							</td>
						</tr>
						<tr id="trFax" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Fax:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtFax" runat="server" Width="115"></asp:TextBox>
							</td>
						</tr>
					</table>
				</td>
				<td id="tdRightColumn" style="vertical-align: top;">
					<table style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trAssignPassword" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">
								<a id="anchorAdvanced" type="text/html" href="#" class="btn_Link" style="text-align: left; display: none;">Advanced:</a>
							</td>
							<td class="attributesValue">
								<asp:UpdatePanel ID="upResetContainer" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
									<ContentTemplate>
										<div id="divAdvancedOptions" class="attributesValue" style="display: none;">
											<asp:TextBox ID="txtNewPassword" TextMode="Password" runat="server" Width="150"></asp:TextBox>
											<asp:Button ID="buttonSetPassword" runat="server" Text="Set Password" OnClick="buttonSetPassword_Click" />
										</div>
									</ContentTemplate>
								</asp:UpdatePanel>
							</td>
						</tr>

						<tr id="trEmail" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Email:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtEmail" runat="server" TextMode="Email" Width="250"></asp:TextBox>
								<span id="spanEmail" runat="server" style="display: none;"></span>
							</td>
						</tr>
						<tr id="trEmail2" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Email2:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtEmail2" runat="server" TextMode="Email" Width="250"></asp:TextBox>
								<span id="spanEmail2" runat="server" style="display: none;"></span>
							</td>
						</tr>

						<tr id="trAddress" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Address:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtAddress" runat="server" Width="250"></asp:TextBox>
							</td>
						</tr>
						<tr id="trAddress2" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Address2:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtAddress2" runat="server" Width="250"></asp:TextBox>
							</td>
						</tr>
						<tr id="trCity" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">City:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtCity" runat="server" Width="150"></asp:TextBox>
							</td>
						</tr>
						<tr id="trState" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">State:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtState" runat="server" Width="50"></asp:TextBox>
							</td>
						</tr>
						<tr id="trPostalCode" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Postal Code:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtPostalCode" runat="server" Width="50"></asp:TextBox>
							</td>
						</tr>
						<tr id="trCountry" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">Country:</td>
							<td class="attributesValue">
								<asp:TextBox ID="txtCountry" runat="server" Width="100"></asp:TextBox>
							</td>
						</tr>


                        <tr id="trTheme" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Theme:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlTheme" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true"></asp:DropDownList>
							</td>
                        </tr>

   						<tr id="trOptions" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">
								<input type="checkbox" id="chkAnimations" runat="server" title="Option for enabling/disabling page animations" style="vertical-align: middle;" /><asp:Label ID="labelAnimations" runat="server" AssociatedControlID="chkAnimations" Text="Enable Page Animations" ToolTip="Option for enabling/disabling page animations" />
							</td>
						</tr>

					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="text-align: left; vertical-align: top;">
					<table style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trSpacer1" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
						<tr id="trNotes" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel" style="vertical-align: top;">Notes:</td>
							<td class="attributesValue" style="vertical-align: top;">
								<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" Style="text-align: left; font-size: 11px; width: 98%;" Enabled="true"></asp:TextBox>
							</td>
						</tr>
						<tr id="trSpacer2" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">&nbsp;</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td id="tdLeftCol2" style="width: 100%; vertical-align: top;">
					<table style="width: 100%; vertical-align: top; text-align: left;">
                        <tr id="trResourceType" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Resource Type:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlResourceType" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true"></asp:DropDownList>
							</td>
						</tr>
						<tr id="trOrganization" class="attributesRow">
							<td class="attributesRequired">*</td>
							<td class="attributesLabel">Organization:</td>
							<td class="attributesValue">
								<asp:DropDownList ID="ddlOrganization" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true"></asp:DropDownList>
							</td>
						</tr>
						<tr id="trApprovedLocked" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">
								<input type="checkbox" id="chkApproved" runat="server" title="Flag for user registration and approval status" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="labelForChkApproved" runat="server" AssociatedControlID="chkApproved" Text="Approved" ToolTip="Flag for user registration and approval status" />
                            </td>
   							<td class="attributesValue">
								<input type="checkbox" id="chkLocked" runat="server" title="Flag if user has been locked out of the system" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="labelForChkLocked" runat="server" AssociatedControlID="chkLocked" Text="Locked" ToolTip="Flag if user has been locked out of the system" />
                            </td>
							<td class="attributesValue">
								<input type="checkbox" id="chkArchive" runat="server" title="User account archive status" value="Archive" style="text-align: left; vertical-align: middle;" /><asp:Label ID="labelForChkArchive" runat="server" AssociatedControlID="chkArchive" Text="Archive" ToolTip="User account archive status" />
							</td>
							<td class="attributesValue">
								<a id="buttonUnlock" runat="server" type="text/html" href="#" class="btn_Link" title="Unlock user account" style="padding-left: 5px; padding-right: 5px; display: none;">Unlock account</a>
                            </td>
						</tr>

                        <tr id="trUserTypes" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">
								<input type="checkbox" id="chkIsDeveloper" runat="server" title="Is this user a software developer" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="lblForchkIsDeveloper" runat="server" AssociatedControlID="chkIsDeveloper" Text="Developer" ToolTip="Is this person a developer" />
                            </td>
							<td class="attributesValue">
								<input type="checkbox" id="chkIsBusAnalyst" runat="server" title="Is this user a business analyst" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="lblForchkIsBusAnalyst" runat="server" AssociatedControlID="chkIsBusAnalyst" Text="Business Analyst" ToolTip="Is this person a business analyst" />
                            </td>
							<td class="attributesValue" style="display: none;">
								<input type="checkbox" id="chkReceiveSREMail" runat="server" title="Include in SR Report EMail distribution" style="vertical-align: middle;" /><asp:Label ID="labelForchkReceiveSREMail" runat="server" AssociatedControlID="chkReceiveSREMail" Text="SR Report EMails" ToolTip="Include SR Report EMail distribution" />  <%--disabled="disabled"--%>
                            </td>
							<td class="attributesValue" style="display: none;">
								<input type="checkbox" id="chkIncludeInSRCounts" runat="server" title="Include in SR Report Counts" style="vertical-align: middle;" /><asp:Label ID="labelForchkIncludeInSRCounts" runat="server" AssociatedControlID="chkIncludeInSRCounts" Text="Include in SR Counts" ToolTip="Include SR Report Counts" />  <%--disabled="disabled"--%>
							</td>
						</tr>

                        <tr id="trUserTypesAddl" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel">&nbsp;</td>
							<td class="attributesValue">
								<input type="checkbox" id="chkAMCGEO" runat="server" title="Is this user AMC GOE" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="lblForchkAMCGEO" runat="server" AssociatedControlID="chkAMCGEO" Text="AMC GEO" ToolTip="Is this person AMC GEO" />
                            </td>
							<td class="attributesValue">
								<input type="checkbox" id="chkCASUser" runat="server" title="Is this user a CAS User" disabled="disabled"  style="vertical-align: middle;" /><asp:Label ID="lblForchkCASUser" runat="server" AssociatedControlID="chkCASUser" Text="CAS User" ToolTip="Is this person a CAS User" />
                            </td>
							<td class="attributesValue">
								<input type="checkbox" id="chkALODUser" runat="server" title="Is this user an ALOD User" disabled="disabled" style="vertical-align: middle;" /><asp:Label ID="lblForchkALODUser" runat="server" AssociatedControlID="chkALODUser" Text="ALOD User" ToolTip="Is this person an ALOD User" />
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="text-align: left; vertical-align: top;">
					<table style="width: 100%; vertical-align: top; text-align: left;">
						<tr id="trAttributes" class="attributesRow">
							<td class="attributesRequired">&nbsp;</td>
							<td class="attributesLabel" style="vertical-align: top;">Attributes:</td>
							<td class="attributesValue" style="vertical-align: top;">
								<asp:CheckBoxList ID="chkListAttributes" runat="server" CssClass="attributes" RepeatDirection="Vertical" RepeatLayout="Table" RepeatColumns="4"></asp:CheckBoxList>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>

	<script id="jsSave" type="text/javascript">

        function buttonSave_click() {
            var membership_UserId = $('#<%=this.txtMembership_UserId.ClientID %>').val();
            if (membership_UserId.length == 0
                || membership_UserId == _emptyGuid) {
                findRegisteredUser();
                return false;
            }
            else {
                save(false);
            }
        }

		function getSelectedFlags() {
			var flags = new Array();
			$("#<%=this.chkListAttributes.ClientID%> input[type=checkbox]:checked").each(function () {
				flags.push($(this).val());
			});

			return flags;
		}

        function getProfileDetails(registerNew) {
            
            var profile = new Object();

            profile.userId = $('#<%=this.txtUserId.ClientID %>').val();
            profile.registerNew = registerNew;
            profile.membership_UserId = $('#<%=this.txtMembership_UserId.ClientID %>').val();
            profile.username = $('#<%=this.txtUsername.ClientID %>').val();
            profile.ResourceTypeId = parseInt($('#<%=this.ddlResourceType.ClientID %> option:selected').val());
            profile.OrganizationId = parseInt($('#<%=this.ddlOrganization.ClientID %> option:selected').val());
            profile.themeId = $('#<%=this.ddlTheme.ClientID %> option:selected').val();
            profile.enableAnimations = $('#<%=this.chkAnimations.ClientID %>').is(':checked');
            profile.prefix = $('#<%=this.txtPrefix.ClientID %>').val();
            profile.firstName = $('#<%=this.txtFirstName.ClientID %>').val();
            profile.middleName = $('#<%=this.txtMiddleInit.ClientID %>').val();
            profile.lastName = $('#<%=this.txtLastName.ClientID %>').val();
            profile.suffix = $('#<%=this.txtSuffix.ClientID %>').val();
            profile.phone_Office = $('#<%=this.txtPhone.ClientID %>').val();
            profile.phone_Mobile = $('#<%=this.txtPhone_Mobile.ClientID %>').val();
            profile.phone_Misc = $('#<%=this.txtPhone_Misc.ClientID %>').val();
            profile.fax = $('#<%=this.txtFax.ClientID %>').val();
            profile.email = $('#<%=this.txtEmail.ClientID %>').val();
            profile.email2 = $('#<%=this.txtEmail2.ClientID %>').val();
            profile.address = $('#<%=this.txtAddress.ClientID %>').val();
            profile.address2 = $('#<%=this.txtAddress2.ClientID %>').val();
            profile.city = $('#<%=this.txtCity.ClientID %>').val();
            profile.state = $('#<%=this.txtState.ClientID %>').val();
            profile.country = $('#<%=this.txtCountry.ClientID %>').val();
            profile.postalCode = $('#<%=this.txtPostalCode.ClientID %>').val();
        	profile.notes = $('#<%=this.txtNotes.ClientID %>').val();
        	profile.flags = getSelectedFlags().join(',');
        	profile.archive = $('#<%=this.chkArchive.ClientID %>').is(':checked');

            profile.receivesremail = $('#<%=this.chkReceiveSREMail.ClientID %>').is(':checked');
            profile.includeInSRCounts = $('#<%=this.chkIncludeInSRCounts.ClientID %>').is(':checked');

            // New as of 1-3-17:
            profile.IsDeveloper = $('#<%=this.chkIsDeveloper.ClientID %>').is(':checked');
            profile.IsBusAnalyst = $('#<%=this.chkIsBusAnalyst.ClientID %>').is(':checked');
            profile.IsAMCGEO = $('#<%=this.chkAMCGEO.ClientID %>').is(':checked');
            profile.IsCASUser = $('#<%=this.chkCASUser.ClientID %>').is(':checked');
            profile.IsALODUser = $('#<%=this.chkALODUser.ClientID %>').is(':checked');

            return profile;
        }

        function findRegisteredUser() {
            var email = $('#<%=this.txtEmail.ClientID %>').val();
        	var username = $('#<%=this.txtUsername.ClientID %>').val();

        	try {
        		ShowDimmer(true, 'Searching...');

        		PageMethods.GetRegisteredUser(email, username
					, findRegisteredUser_done, on_error);
        	} catch (e) {
        		ShowDimmer(false);
        	}
        }
		function findRegisteredUser_done(result) {
			try {
				var obj = jQuery.parseJSON(result);
				var exists = '', id = '', email = '', username = '', error = '';
				var fName = $('#<%=this.txtFirstName.ClientID %>').val();;
				var lName = $('#<%=this.txtLastName.ClientID %>').val();
				var username_ui = $('#<%=this.txtUsername.ClientID %>').val();

				ShowDimmer(false);

				$.each(obj, function (index, val) {
					//do something with data
					switch (index.toUpperCase()) {
						case 'EXISTS':
							exists = val;
							break;
						case 'ID':
							id = val;
							break;
						case 'EMAIL':
							email = val;
							break;
						case 'USERNAME':
							username = val;
							break;
						case 'ERROR':
							error = val;
							break;
					}
				});

				if (exists.toUpperCase() == 'TRUE') {
					if (email.toUpperCase() == $('#<%=this.txtEmail.ClientID %>').val().toUpperCase()) {
						var msg = 'A registered user already exists with the same email address. Please update and resubmit if you would like to continue.';
						//alert(msg);
						MessageBox(msg);
						return;
					}
					else if (username.toUpperCase() == $('#<%=this.txtUsername.ClientID %>').val().toUpperCase()) {
						//rebuild username
						var lastChar = userName.Substring(userName.Length - 1, 1);
						var num = 1;
						if (isNaN(lastChar)) {
							num = 1;
						}
						else {
							num = parseInt(lastChar) + 1;
							userName = userName.Substring(0, userName.Length - 1);
						}
						userName += num.ToString();

						save(true);
					}
				}
				else {
					//register user when saving profile
					save(true);
					return false;
				}
			} catch (e) {
				ShowDimmer(false);
			}
		}

        function save(registerNew) {
        	try {
        		ShowDimmer(true, 'Saving...');

                var profile = getProfileDetails(registerNew);

                PageMethods.SaveProfile(parseInt(profile.userId), registerNew
					, profile.membership_UserId, profile.username, parseInt(profile.ResourceTypeId)
					, parseInt(profile.OrganizationId), parseInt(profile.themeId), profile.enableAnimations
					, profile.prefix, profile.firstName, profile.middleName, profile.lastName, profile.suffix
					, profile.phone_Office, profile.phone_Mobile, profile.phone_Misc, profile.fax, profile.email, profile.email2
					, profile.address, profile.address2, profile.city, profile.state, profile.country, profile.postalCode
					, profile.notes, profile.flags, profile.archive, profile.receivesremail, profile.includeInSRCounts
                    , profile.IsDeveloper, profile.IsBusAnalyst, profile.IsAMCGEO, profile.IsCASUser, profile.IsALODUser
					, save_done, on_error);

            } catch (e) {
                ShowDimmer(false);
            }
        }
        function save_done(result) {
        	try {
        		var obj = jQuery.parseJSON(result);
        		var saved = '', errorMsg = '', id = '', username = '';
        		var changedTheme = 'false';

        		ShowDimmer(false);

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
        				case 'USERNAME':
        					username = val;
        					break;
        				case 'CHANGEDTHEME':
        					changedTheme = val;
        					break;
        			}
        		});

        		if (saved.toUpperCase() == 'TRUE') {
        			$('#<%=this.txtUserId.ClientID %>').val(id);

                	if (!opener) {
                		opener = parent;
                	}

                	if (opener) {
                		if (opener.refreshGrid) {
                			opener.refreshGrid();
                			refreshPage(id);
                		}
                		else if (top.themeChanged
                            && '<%=this.IsCurrentUser %>'.toLowerCase() == 'true') {
                        	//check for udpated theme for current profile
                            if (changedTheme != null
								&& changedTheme != undefined
								&& changedTheme.toLowerCase() == 'true') {

                            	QuestionBox('Theme Changed', 'Successfully saved profile. Your theme has changed. Refresh system to apply change?', 'Yes,No', 'refreshTheme', 300, 300, window.self);
                            }
                            else {
                            	MessageBox('Successfully saved profile.');

                            	refreshPage(id);
                            }
                        //refreshPage(id);
                        }
                        else {
                            MessageBox('Successfully saved profile.');

                            refreshPage(id);
                        }
					}
					refreshPage(id);
				}
				else {
                	MessageBox('Failed to save user profile. \n \n' + errorMsg);
					return false;
				}
			}
			catch (e) {
				//TODO: since the error is in the "done" procedure, log message instead of displaying
			}
		}
    
        function refreshTheme(answer) {
            if (answer == 'Yes') {
                if (top && defaultParentPage && defaultParentPage.themeChanged) {
                    defaultParentPage.themeChanged(true);
                }
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
            var membership_UserId = $('#<%=this.txtMembership_UserId.ClientID %>').val();
            
        	try {
        		ShowDimmer(true, 'Saving...');

        		PageMethods.ChangeUserApproval(membership_UserId, approved
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
            var membership_UserId = $('#<%=this.txtMembership_UserId.ClientID %>').val();
            
        	try {
        		ShowDimmer(true, 'Saving...');

        		PageMethods.UnlockUser(membership_UserId
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

	</script>

    <script type="text/javascript">
        var _emptyGuid = '00000000-0000-0000-0000-000000000000';

        function refreshPage(userId) {
            if (userId === undefined || userId === null) {
                userId = '';
            }
            var url = window.location.href;
            url = editQueryStringValue(url, 'UserID', userId);

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

        function buttonCancel_click() {
            document.location.href = document.location.href;
        }

        function resizePage() {
            resizePageElement('divPageContainer',5);
        }

        function buildUserName() {
            var userId = $('#<%=this.txtUserId.ClientID %>').val();
            if (userId.length > 0
                && userId != '0') {
                return false;
            }

            var fName = '', mName = '', lName = '', userName = '';

            fName = $('#<%=this.txtFirstName.ClientID %>').val().trim().replace(' ', '');
            mName = $('#<%=this.txtMiddleInit.ClientID %>').val().trim().replace(' ', '');
            lName = $('#<%=this.txtLastName.ClientID %>').val().trim().replace(' ', '');

            if (mName.length > 0) {
                mName += '.';
            }

            userName = fName + '.' + mName + lName;

            $('#<%=this.txtUsername.ClientID %>').val(userName);
        }

        $(document).ready(function () {
        	$('[id*="pageContentHeader"]').hide();
        	$('[id*="pageContentInfo"]').removeClass('pageContentInfo');
        	$('[id*="pageContentInfo"]').addClass('pageContentHeader');
        	
        	$('[name="FirstLabel"]').width(80);

            $('.UserNamePart').bind('keyup', function () {
                buildUserName();
                return false;
            });

            if('<%=this.IsAdmin %>'.toUpperCase() == 'TRUE') {
                $('#anchorAdvanced').show();
                $('#anchorAdvanced').click( function () { $('#divAdvancedOptions').toggle(); return false; });
            }
        	if ('<%=this.IsUserAdmin %>'.toUpperCase() == 'TRUE') {
        		$('#trApprovedLocked').prop('enabled', true);
        		$('#trApprovedLocked').prop('disabled', false);

        		$('#<%=this.chkApproved.ClientID %>').attr('enabled', true);
        		$('#<%=this.chkApproved.ClientID %>').bind('change', function () { chkApproved_change(); return false; });

        		$('#<%=this.buttonUnlock.ClientID %>').attr('disabled', '');
        		$('#<%=this.buttonUnlock.ClientID %>').show();
        		$('#<%=this.buttonUnlock.ClientID %>').click(function () { buttonUnlock_click(); return false; });

        		$('#<%=this.ddlOrganization.ClientID %>').bind('change', function () { ddlOrganization_change(); return false; });
        	}
        	else {
        		$('#trApprovedLocked').prop('enabled', false);
        		$('#trApprovedLocked').prop('disabled', true);
        	}

        	$('[id*="imgRefresh"]').click(function () { refreshPage(); return false; });
           
            resizePage();
            $(window).resize(resizePage);

            $(document.body).bind('onbeforeunload', function () { closePage(); });
        });

	</script>
</asp:Content>

