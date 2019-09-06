<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reset.aspx.cs" Inherits="Account_Reset" Theme="Default" %>
<%@ Register Src="~/Account/RequestPasswordReset.ascx" TagPrefix="uc" TagName="RequestPasswordReset" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="padding:0px; margin:0px;">
    <form id="form1" runat="server" novalidate="novalidate">
        <div id="divExpired" runat="server" class="attributes" style="padding:0px; display:none;">
            <p class="validation-summary-errors" style="color:red; padding-left:5px;">
                <asp:Literal runat="server" ID="RequestValidationFailureText" />
            </p>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="upResetContainer" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc:RequestPasswordReset ID="ucRPR" runat="server" EnableViewState="false" Username="" EmailAddress="" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div>
            <div id="divPasswordFields" runat="server" class="attributes" style="padding-top:10px; display:block;">
                <asp:HiddenField ID="hiddenUserId" runat="server" />
                <asp:HiddenField ID="hiddenResetCode" runat="server" />
                <p class="message-info" style="padding-left:5px;">
                    Passwords are required to be a minimum of <%: Membership.MinRequiredPasswordLength %> characters in length.
                </p>
                <p class="validation-summary-errors" style="color:red; padding-left:5px;">
                    <asp:Literal runat="server" ID="FailureText" />
                </p>
                <div id="divPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                    <div class="attributesRequired">*</div>
                    <div class="attributesLabel" style="width:150px;">New Password: </div>
                    <div class="attributesValue">
                        <asp:TextBox runat="server" ID="NewPassword" CssClass="passwordEntry" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NewPassword"
                            CssClass="field-validation-error" ErrorMessage="The new password is required." ForeColor="Red" />
                    </div>
                </div>
                <div id="divConfirmPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                    <div class="attributesRequired">*</div>
                    <div class="attributesLabel" style="width:150px;">Confirm New Password: </div>
                    <div class="attributesValue">
                        <asp:TextBox runat="server" ID="ConfirmNewPassword" CssClass="passwordEntry" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ConfirmNewPassword"
                            CssClass="field-validation-error" Display="Dynamic" ErrorMessage="Confirm new password is required." ForeColor="Red" />
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                            CssClass="field-validation-error" Display="Dynamic" ErrorMessage="The new password and confirmation password do not match." ForeColor="Red" />
                    </div>
                </div>
                <div id="divChangeButton" class="attributesRow" style="width:100%; padding-left:5px; padding-top:5px;">
                    <div class="attributesRequired">&nbsp;</div>
                    <div class="attributesLabel" style="width:150px;">&nbsp;</div>
                    <div class="attributesValue">
                        <asp:Button ID="buttonSetPassword" runat="server" Text="Set password" OnClick="buttonSetPassword_Click" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
