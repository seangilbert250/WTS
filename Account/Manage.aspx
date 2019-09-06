<%@ Page Title="Manage Account" Language="C#" AutoEventWireup="true" CodeFile="Manage.aspx.cs" Inherits="Account_Manage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Request Access</title>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <link href="../App_Themes/Default/Default.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <hgroup class="title" style="padding-left:5px;">
        <h1><%: Title %>.</h1>
    </hgroup>
    <div style="padding-left:5px;">
        <section id="passwordForm">
            <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
                <p class="message-success"><%: SuccessMessage %></p>
            </asp:PlaceHolder>

            <p>You are logged in as <strong><%: User.Identity.Name %></strong>.</p>

            <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
                <p>
                    You do not have a local password for this site. Add a local
                    password so you can log in without an external login.
                </p>
                <fieldset>
                    <legend>Set Password</legend>
                    <ol>
                        <li>
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="password">Password</asp:Label>
                            <asp:TextBox runat="server" ID="password" TextMode="Password" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="password"
                                CssClass="field-validation-error" ErrorMessage="The password field is required."
                                Display="Dynamic" ValidationGroup="SetPassword" />
                        
                            <asp:ModelErrorMessage ID="ModelErrorMessage1" runat="server" ModelStateKey="NewPassword" AssociatedControlID="password"
                                CssClass="field-validation-error" SetFocusOnError="true" />
                        </li>
                        <li>
                            <asp:Label ID="Label2" runat="server" AssociatedControlID="confirmPassword">Confirm password</asp:Label>
                            <asp:TextBox runat="server" ID="confirmPassword" TextMode="Password" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="confirmPassword"
                                CssClass="field-validation-error" Display="Dynamic" ErrorMessage="The confirm password field is required."
                                ValidationGroup="SetPassword" />
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password" ControlToValidate="confirmPassword"
                                CssClass="field-validation-error" Display="Dynamic" ErrorMessage="The password and confirmation password do not match."
                                ValidationGroup="SetPassword" />
                        </li>
                    </ol>
                    <asp:Button ID="Button1" runat="server" Text="Set Password" ValidationGroup="SetPassword" OnClick="setPassword_Click" />
                </fieldset>
            </asp:PlaceHolder>

            <asp:PlaceHolder runat="server" ID="changePassword" Visible="false">
                <h3>Change password</h3>
                <asp:ChangePassword ID="ChangePassword1" runat="server" MembershipProvider="WTSMembershipProvider" CancelDestinationPageUrl="~/" ViewStateMode="Disabled" RenderOuterTable="false" SuccessPageUrl="Manage.aspx?m=ChangePwdSuccess">
                    <ChangePasswordTemplate>
                        <p class="validation-summary-errors">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                        <fieldset class="changePassword">
                            <legend>Change password details</legend>
                            <div id="divPasswordFields" class="attributes" style="padding-top:10px;">
                                <div id="divCurrentContainer" class="attributes" style="padding-left:5px; border:1px solid none;">
                                    <div id="divCurrentPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">*</div>
                                        <div class="attributesLabel">Current password: </div>
                                        <div class="attributesValue">
                                            <asp:TextBox runat="server" ID="CurrentPassword" CssClass="passwordEntry" TextMode="Password" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CurrentPassword" ValidationGroup="ValidatePassword"
                                                CssClass="field-validation-error" ErrorMessage="* The current password field is required." ForeColor="Red" Text="*" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="CurrentPassword" ValidationGroup="ValidateQuestion"
                                                CssClass="field-validation-error" ErrorMessage="* The current password field is required." ForeColor="Red" Text="*" />
                                        </div>
                                    </div>
                                </div>
                                <div id="divChangePassword" class="attributes" style="margin-top:10px; padding:5px; border:1px solid gray;">
                                    <div id="divPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">*</div>
                                        <div class="attributesLabel">New Password: </div>
                                        <div class="attributesValue">
                                            <asp:TextBox runat="server" ID="NewPassword" CssClass="passwordEntry" TextMode="Password" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NewPassword" ValidationGroup="ValidatePassword"
                                                CssClass="field-validation-error" ErrorMessage="* New password is required." ForeColor="Red" Text="*" />
                                        </div>
                                    </div>
                                    <div id="divConfirmPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">*</div>
                                        <div class="attributesLabel">Confirm New Password: </div>
                                        <div class="attributesValue">
                                            <asp:TextBox runat="server" ID="ConfirmNewPassword" CssClass="passwordEntry" TextMode="Password" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ConfirmNewPassword" ValidationGroup="ValidatePassword"
                                                CssClass="field-validation-error" Display="Dynamic" ErrorMessage="* Confirm new password is required." Text="*" ForeColor="Red" />
                                            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" ValidationGroup="ValidatePassword"
                                                CssClass="field-validation-error" Display="Dynamic" ErrorMessage="* The new password and confirmation password do not match." Text="*" ForeColor="Red" />
                                        </div>
                                    </div>
                                    <div id="divValidationSummary1" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div class="attributesLabel">&nbsp;</div>
                                        <div class="attributesValue">
                                            <asp:ValidationSummary ID="PasswordValidationSummary" runat="server" ValidationGroup="ValidatePassword" DisplayMode="List" ForeColor="Red" CssClass="field-validation-error" />
                                        </div>
                                    </div>
                                    <div id="divChangeButton" class="attributesRow" style="width:100%; padding-left:5px; padding-top:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div class="attributesLabel">&nbsp;</div>
                                        <div class="attributesValue">
                                            <asp:Button ID="Button2" runat="server" CommandName="ChangePassword" Text="Change Password" ValidationGroup="ValidatePassword" />
                                        </div>
                                    </div>
                                </div>
                                <div id="divQuestionAnswer" class="attributes" style="margin-top:10px; padding:5px; border:1px solid gray;">
                                    <div id="divQuestion" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div Name="LeftColLabel" class="attributesLabel">Security Question: </div>
                                        <div class="attributesValue">
                                            <asp:HiddenField runat="server" ID="txtQuestion" Value="" />
                                            <asp:DropDownList runat="server" ID="Question" AppendDataBoundItems="true"><asp:ListItem Text="--Select--" Value=""></asp:ListItem></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="Question" ValidationGroup="ValidateQuestion"
                                                CssClass="field-validation-error" ErrorMessage="* Security Question is required." ForeColor="Red" Text="*" />
                                        </div>
                                    </div>
                                    <div id="divAnswer" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div Name="LeftColLabel" class="attributesLabel">Answer: </div>
                                        <div class="attributesValue">
                                            <asp:TextBox runat="server" ID="Answer"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="Answer" ValidationGroup="ValidateQuestion"
                                                CssClass="field-validation-error" ErrorMessage="* Answer is required." ForeColor="Red" Text="*" />
                                        </div>
                                    </div>
                                    <div id="divValidationSummary2" class="attributesRow" style="width:100%; padding-left:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div class="attributesLabel">&nbsp;</div>
                                        <div class="attributesValue">
                                            <asp:ValidationSummary ID="ValidationSummaryQuestion" runat="server" ValidationGroup="ValidateQuestion" DisplayMode="List" ForeColor="Red" CssClass="field-validation-error" />
                                        </div>
                                    </div>
                                    <div id="divChangeQuestionButton" class="attributesRow" style="width:100%; padding-left:5px; padding-top:5px;">
                                        <div class="attributesRequired">&nbsp;</div>
                                        <div class="attributesLabel">&nbsp;</div>
                                        <div class="attributesValue">
                                            <asp:Button ID="buttonChangeQuestionAnswer" runat="server" CommandName="ChangeQuestion" Text="Change Question/Answer" ValidationGroup="ValidateQuestion" OnClick="buttonChangeQuestionAnswer_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </ChangePasswordTemplate>
                </asp:ChangePassword>
            </asp:PlaceHolder>
        </section>

        <%--<section id="externalLoginsForm">
            <asp:ListView ID="ListView1" runat="server"
                ItemType="Microsoft.AspNet.Membership.OpenAuth.OpenAuthAccountData"
                SelectMethod="GetExternalLogins" DeleteMethod="RemoveExternalLogin" DataKeyNames="ProviderName,ProviderUserId">
        
                <LayoutTemplate>
                    <h3>Registered external logins</h3>
                    <table>
                        <thead><tr><th>Service</th><th>User Name</th><th>Last Used</th><th>&nbsp;</th></tr></thead>
                        <tbody>
                            <tr runat="server" id="itemPlaceholder"></tr>
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#: Item.ProviderDisplayName %></td>
                        <td><%#: Item.ProviderUserName %></td>
                        <td><%#: ConvertToDisplayDateTime(Item.LastUsedUtc) %></td>
                        <td>
                            <asp:Button ID="Button3" runat="server" Text="Remove" CommandName="Delete" CausesValidation="false" 
                                ToolTip='<%# "Remove this " + Item.ProviderDisplayName + " login from your account" %>'
                                Visible="<%# CanRemoveExternalLogins %>" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </section>--%>
    </div>
    <script type="text/javascript">

        $(document).ready(function () {
            $('.attributesLabel').width(150);
            
            var question = '';
            $('#divQuestion select').change(function () {
                question = $(this).find('option:selected').val();
                $('#divQuestion input[type="hidden"]').val(question);
            });
        });
    </script>
    </form>
</body>
</html>