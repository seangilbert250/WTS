<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WTS_Login.aspx.cs" Inherits="Account_WTS_Login" Theme="Default" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Login</title>
	<link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
	<link href="../App_Themes/Default/Default.css" rel="stylesheet" type="text/css" />
	<link href="../Styles/openid.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/openid-jquery.js"></script>
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<style type="text/css">
        .AppTitle
        {
            font-size:100px;
            text-shadow:0px 2px rgba(255, 255, 255,1);
            color:black;
            width:auto;
        }

        .CompanyName
        {
            padding-left:100px;
            padding-top:20px;
            font-size:50px;
            text-shadow:0px 2px rgba(255, 255, 255,1);
            color:#222629;
        }
    </style>
</head>
<body>
	<form id="form1" runat="server">
		<div class="loginPanel">
			<div id="divInternalLogin" style="padding-left: 5px; padding-right: 5px;">
				<section id="loginForm" style="text-align: center;">
					<h2>Use a local account to log in.</h2>
					<asp:Login runat="server" ID="LoginCtrl" ViewStateMode="Disabled" RenderOuterTable="false"
						FailureTextStyle-Font-Bold="true" FailureTextStyle-ForeColor="Red" FailureText="Unable to find a user with the specified username/password combination."
						MembershipProvider="WTSMembershipProvider">
						<LayoutTemplate>
							<p class="validation-summary-errors" style="color: red;">
								<asp:Literal runat="server" ID="FailureText" />
							</p>
							<fieldset style="text-align: left;">
								<legend>Signin / Register</legend>
								<div id="divSigninFields" class="attributes" style="padding-top: 10px;">
									<div id="divUsername" class="attributesRow" style="width: 100%; padding-left: 5px;">
										<div class="attributesRequired">*</div>
										<div class="attributesLabel">Username: </div>
										<div class="attributesValue">
											<asp:TextBox runat="server" ID="UserName" />
											<asp:RequiredFieldValidator ID="RequiredFieldValidatorUsername" runat="server" ControlToValidate="UserName" CssClass="field-validation-error" ErrorMessage="The user name field is required." ForeColor="Red" />
										</div>
									</div>
									<div id="divPassword" class="attributesRow" style="width: 100%; padding-left: 5px;">
										<div class="attributesRequired">*</div>
										<div class="attributesLabel">Password: </div>
										<div class="attributesValue">
											<asp:TextBox runat="server" ID="Password" TextMode="Password" />
											<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="field-validation-error" ErrorMessage="The password field is required." ForeColor="Red" />
										</div>
									</div>
									<div id="divRememberMe" class="attributesRow" style="width: 100%; padding-left: 5px;">
										<div class="attributesRequired">&nbsp;</div>
										<div class="attributesLabel">&nbsp;</div>
										<div class="attributesValue">
											<asp:CheckBox runat="server" ID="RememberMe" />
											<asp:Label ID="Label1" runat="server" AssociatedControlID="RememberMe" CssClass="checkbox">Remember me?</asp:Label>
										</div>
									</div>
									<div id="divLogin" class="attributesRow" style="width: 100%; padding-left: 5px;">
										<div class="attributesRequired">&nbsp;</div>
										<div class="attributesLabel">&nbsp;</div>
										<div class="attributesValue">
											<asp:Button runat="server" ID="Login" CommandName="Login" Text="Sign In" />
										</div>
									</div>
								</div>
							</fieldset>
							<br />
							<fieldset style="text-align: left; display: none;">
								<legend>Login using OpenID</legend>
								<div id="divOpenIdSigninFields" class="attributes" style="padding-top: 10px;">
									<div id="divHeader" class="attributesRow" style="width: 100%;">
										Please click your account provider:
									</div>
									<div class="attributesRow" style="width: 100%; padding-left: 5px;">
										<div class="attributesRequired">&nbsp;</div>
										<div class="attributesLabel">&nbsp;</div>
										<div id="openid_btns" class="attributesValue">
											<asp:ListView runat="server" ID="providerDetails" ItemType="Microsoft.AspNet.Membership.OpenAuth.ProviderDetails"
												SelectMethod="GetProviderNames" ViewStateMode="Disabled">
												<ItemTemplate>
													<button type="submit" name="provider" value="<%#: Item.ProviderName %>" title="Log in using your <%#: Item.ProviderDisplayName %> account.">
														<%#: Item.ProviderDisplayName %>
													</button>
												</ItemTemplate>
												<EmptyDataTemplate>
													<p>There are no external authentication services configured. </p>
												</EmptyDataTemplate>
											</asp:ListView>
										</div>
									</div>
									<asp:HiddenField runat="server" ID="hdnProvider" Value="" ViewStateMode="Enabled" />
									<div class="oid-msg">
										<asp:Literal ID="litOpenIdMsg" runat="server"></asp:Literal>
									</div>
								</div>
							</fieldset>
						</LayoutTemplate>
					</asp:Login>

					<div id="divPassword_Register" class="attributes" style="padding-top: 5px;">
						<div id="divResetPasswordLink" class="attributesRow" style="width: 100%; padding-left: 5px;">
							<div class="attributesRequired">&nbsp;</div>
							<div class="attributesLabel">&nbsp;</div>
							<div class="attributesValue">
								<a id="anchorReset" type="text/html" href="#" class="btn_Link">Forgot Password?</a>
								&nbsp;&nbsp;|&nbsp;&nbsp;
								<asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Register</asp:HyperLink>
								if you don't have an account.
							</div>
						</div>
					</div>

				</section>
			</div>
		</div>
		<div id="appTitle" class="footerPanel" style="text-align: center; font-weight:bold; font-size:24px;">
			<span style="display: block;">WTS</span>
		</div>
		<div id="footerPanel" class="footerPanel" onselectstart="return false;" style="padding-left: 190px">
			<table style="width: 100%;">
				<tr>
					<td>Copyright ©
						<asp:Label ID="lblCopyRight" runat="server">2015</asp:Label>
						-
						<asp:Label ID="lblContractorNm" runat="server">Infinite Technologies, Inc.</asp:Label>
					</td>
					<td id="testSystemIndicator" runat="server" style="width: 100px; text-align: right; padding-right: 4px; display:none;">Test System
					</td>
				</tr>
			</table>
		</div>
		<script type="text/javascript">
			$(document).ready(function () {
				$('.attributesLabel').width(75);

				$('#divResetPassword').hide();
				$('#anchorReset').click( function () {
					window.location.href = "Reset.aspx?requestReset=true";
				});
			});
		</script>
	</form>
</body>
</html>
