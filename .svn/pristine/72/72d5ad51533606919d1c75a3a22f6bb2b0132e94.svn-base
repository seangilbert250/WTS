<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RegisterExternalLogin.aspx.cs" Inherits="Account_RegisterExternalLogin" Theme="Default" %>

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
        <div>
            <hgroup class="title">
                <h1>Register with your <%: ProviderDisplayName %> account</h1>
                <h2><%: ProviderUserName %>.</h2>
            </hgroup>
            
            <asp:ModelErrorMessage runat="server" ModelStateKey="Provider" CssClass="field-validation-error" />
    
            <asp:PlaceHolder runat="server" ID="userNameForm">
                <fieldset>
                    <legend>Association Form</legend>
                    <p>
                        You've authenticated with <strong><%: ProviderDisplayName %></strong> as
                        <strong><%: ProviderUserName %></strong>. Please enter a user name below for the current site
                        and click the Log in button.
                    </p>
                    <ol>
                        <li class="email">
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="userName">User name</asp:Label>
                            <asp:TextBox runat="server" ID="userName" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="userName"
                                Display="Dynamic" ErrorMessage="User name is required" ValidationGroup="NewUser" />
                    
                            <asp:ModelErrorMessage ID="ModelErrorMessage1" runat="server" ModelStateKey="UserName" CssClass="field-validation-error" />
                    
                        </li>
                    </ol>
                    <asp:Button ID="Button1" runat="server" Text="Log in" ValidationGroup="NewUser" OnClick="logIn_Click" />
                    <asp:Button ID="Button2" runat="server" Text="Cancel" CausesValidation="false" OnClick="cancel_Click" />
                </fieldset>
            </asp:PlaceHolder>
        </div>
    </form>
</body>
</html>