<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RequestPasswordReset.ascx.cs" Inherits="Account_RequestPasswordReset" %>

<h3>Reset Password</h3>
<div id="divResetContainer" class="attributes" style="border:1px solid gray; padding: 10px 5px 10px 5px;">
    <div id="divFormDisabler" runat="server" class="disabler" style="position:absolute; padding:0px; margin:0px; width:100%; height:100%; background-color:#ebe9e9; opacity:.85; text-align:center; vertical-align:middle;">
        <table style="height:100%; width:100%;">
            <tr>
                <td style="height:100%; width:100%;">Processing your request...</td>
            </tr>
        </table>
    </div>
    <p class="message-info" style="padding-left:5px;">
        Request Password reset.  A message will be sent to the registered email on your account.
    </p>
    <p class="validation-summary-errors" style="color:red; padding-left:5px;">
        <asp:Literal runat="server" ID="FailureText" />
    </p>
    <div id="divResetType" class="attributesRow" style="width:100%; padding-left:5px; display:none;">
        <div class="attributesRequired">*</div>
        <div class="attributesLabel" style="width:75px;">Lookup: </div>
        <div class="attributesValue">
            <asp:DropDownList runat="server" ID="ddlRequestType" ViewStateMode="Enabled" EnableViewState="true" onchange="ddlRequestType_change(this);return false;">
                <asp:ListItem Text="Username" Value="Username"></asp:ListItem>
                <asp:ListItem Text="EmailAddress" Value="EmailAddress"></asp:ListItem>
            </asp:DropDownList>
            <asp:HiddenField ID="txtRequestType" runat="server" Value="Username" />
        </div>
    </div>
    <div id="divResetUsername" class="attributesRow" style="width:100%; padding-left:5px;">
        <div class="attributesRequired">*</div>
        <div class="attributesLabel" style="width:75px;">Username: </div>
        <div class="attributesValue">
            <asp:TextBox runat="server" ID="txtResetUsername" />
        </div>
    </div>
    <div id="divResetEmail" class="attributesRow" style="width:100%; padding-left:5px; display:none;">
        <div class="attributesRequired">*</div>
        <div class="attributesLabel" style="width:75px;">Email: </div>
        <div class="attributesValue">
            <asp:TextBox runat="server" ID="txtResetEmail" TextMode="Email" Width="250" />
        </div>
    </div>
    <div id="divResetButton" class="attributesRow" style="width:100%; padding-left:5px; padding-top:5px;">
        <div class="attributesRequired">&nbsp;</div>
        <div class="attributesLabel" style="width:75px;">&nbsp;</div>
        <div class="attributesValue">
            <asp:Button ID="buttonRequestReset" runat="server" Text="Reset" OnClientClick="buttonRequestReset_clientClick();return true;" OnClick="buttonRequestReset_Click" />
        </div>
    </div>
    <div id="divMessages" runat="server" class="attributesRow" style="width:100%; padding-left:5px; padding-top:10px;">
        <asp:Label ID="labelResultMessage" runat="server"></asp:Label>
    </div>
</div>

<script type="text/javascript">
    function ddlRequestType_change(sender) {
        var requestType = $('#' + sender.id + ' option:selected').val();
        $('#<%=this.txtRequestType.ClientID %>').val(requestType);
        switch (requestType.toUpperCase()) {
            case 'USERNAME':
                $('#divResetUsername').show();
                $('#divResetEmail').hide();
                break;
            default:
                $('#divResetUsername').hide();
                $('#divResetEmail').show();
                break;
        }
    }

    function buttonRequestReset_clientClick() {
        $('.disabler').first().css('height', $('#divResetContainer').css('height'));
        $('.disabler').first().css('width', $('#divResetContainer').css('width'));
        $('.disabler').bind('click', function () { $(this).hide(); });

        $('#<%=this.divFormDisabler.ClientID %>').show();

        return true;
    }

    $(document).ready(function () {
        $('.disabler').hide();
        if ('<%=this.RequestType.ToString() %>'.toUpperCase() == 'EMAILADDRESS') {
            $('#divResetUsername').hide();
            $('#divResetEmail').show();
            return false;
        }
    });
</script>