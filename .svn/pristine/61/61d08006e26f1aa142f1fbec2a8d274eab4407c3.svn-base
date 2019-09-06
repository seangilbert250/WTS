<%@ Page Title="" Language="C#" MasterPageFile="~/AddEdit.master" AutoEventWireup="true" CodeFile="GatherRegistrationDetails.aspx.cs" Inherits="Admin_GatherRegistrationDetails" Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headTitle" Runat="Server">Login Details</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHeaderText" Runat="Server">Login Details</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphPageContentInfo" Runat="Server"></asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
    <div id="divAttributes" class="attributes" style="width:100%; padding-top:5px;">
        <div id="divPassword" class="attributesRow" style="width:100%; padding-left:5px;">
            <div class="attributesRequired">*</div>
            <div class="attributesLabel" style="width:115px;">Password: </div>
            <div class="attributesValue">
                <asp:TextBox runat="server" ID="Password" TextMode="Password" />
            </div>
        </div>
        <div id="divConfirmPassword" class="attributesRow" style="width:100%; padding-left:5px;">
            <div class="attributesRequired">*</div>
            <div class="attributesLabel" style="width:115px;">Confirm Password: </div>
            <div class="attributesValue">
                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
            </div>
        </div>
        <div id="divQuestion" class="attributesRow" style="width:100%; padding-left:5px;">
            <div class="attributesRequired">*</div>
            <div class="attributesLabel" style="width:115px;">Security Question: </div>
            <div class="attributesValue">
                <asp:TextBox runat="server" ID="Question"></asp:TextBox>
            </div>
        </div>
        <div id="divAnswer" class="attributesRow" style="width:100%; padding-left:5px;">
            <div class="attributesRequired">*</div>
            <div class="attributesLabel" style="width:115px;">Answer: </div>
            <div class="attributesValue">
                <asp:TextBox runat="server" ID="Answer"></asp:TextBox>
            </div>
        </div>
        <div id="divErrors" class="attributesRow" style="width:100%; padding-left:5px;">
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Password"
                CssClass="field-validation-error" ErrorMessage="The password field is required." ForeColor="Red" /><br />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ConfirmPassword"
                CssClass="field-validation-error" ForeColor="Red" Display="Dynamic" ErrorMessage="The confirm password field is required." /><br />
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                CssClass="field-validation-error" ForeColor="Red" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
        </div>
    </div>
    <div id="divFooter" class="pageContentHeader" style="width:100%; position:absolute; bottom:0px;">
        <div id="divCancel" style="float:right; width:60px; padding-right:5px; vertical-align:middle; height:30px; text-align:right;">
            <button id="buttonCancel" onclick="buttonCancel_click();return false;" value="Cancel">Cancel</button>
        </div>
        <div id="divSave" runat="server" style="float:right; padding-right:5px; vertical-align:middle; height:30px; text-align:right;">
            <button id="buttonSave" onclick="buttonSave_click();return false;" value="Save">Save</button>
        </div>
    </div>
    <script type="text/javascript">

        function buttonCancel_click() {
            var dialogResult = null;
            if (window.opener) {
                window.opener.returnValue = 'cancel';
            }
            window.returnValue = 'cancel';

            window.close();
        } //end buttonCancel_click()

        function buttonSave_click() {
            //get entered values
            var password = '', question = '', answer = '';

            password = $('#<%=this.Password.ClientID %>').val();
            question = $('#<%=this.Question.ClientID %>').val();
            answer = $('#<%=this.Answer.ClientID %>').val();

            var dialogResult = new Object();
            dialogResult.password = password;
            dialogResult.question = question;
            dialogResult.answer = answer;

            if (window.opener) {
                window.opener.returnValue = dialogResult;
            }
            window.returnValue = dialogResult;

            window.close();
        } //end btnSave_click()

        $(document).ready(function () {
            $('#pageContentHeader').hide();
            $('#pageContentInfo').hide();

            $(document.body).bind('onbeforeunload', function () { buttonCancel_click(); });
        });
    </script>
</asp:Content>
