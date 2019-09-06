<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Account_Register" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request Access</title>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <link href="../App_Themes/Default/Default.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:CreateUserWizard runat="server" ID="RegisterUser"
        DuplicateEmailErrorMessage="A user already exists with the specified email address." FinishPreviousButtonType="Button" ViewStateMode="Enabled" 
        OnCreatedUser="RegisterUser_CreatedUser" MembershipProvider="WTSMembershipProvider" OnNextButtonClick="RegisterUser_NextButtonClick">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="wizardStepPlaceholder" />
            <asp:PlaceHolder runat="server" ID="navigationPlaceholder" />
        </LayoutTemplate>
        <WizardSteps>
            <asp:WizardStep>
                <div id="divProfileFields" class="attributes" style="width:100%; padding-left:10px;">
                    <h3 class="message-info">
                        Profile details
                    </h3>
                    <p class="message-info">&nbsp;</p>
                    <div id="divFindUsers" style="position:absolute; top:75px; left:375px; padding-bottom:10px; width:500px; border:1px solid gray; display:none;">
                        <div id="divUsernameFind" class="attributesRow" style="width:100%; padding-left:5px; padding-top:10px;">
                            <div Name="LeftColLabel" class="attributesLabel">Username: </div>
                            <div class="attributesValue">
                                <asp:HiddenField runat="server" ID="txtProfileUserId" />
                                <asp:TextBox runat="server" ID="txtUsername_Find" />
                            </div>
                        </div>
                        <div id="divEmailFind" class="attributesRow" style="width:100%; padding-left:5px;">
                            <div Name="LeftColLabel" class="attributesLabel">Email Address: </div>
                            <div class="attributesValue">
                                <asp:TextBox ID="txtEmail_Find" runat="server" TextMode="Email" Width="245"></asp:TextBox>
                            </div>
                        </div>
                        <div id="divSearchType" class="attributesRow" style="width:100%; padding-left:5px;">
                            <div Name="LeftColLabel" class="attributesLabel">Search Option: </div>
                            <div class="attributesValue" style="padding-left:5px;">
                                <asp:RadioButtonList ID="rblSearchType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Username" Selected="True" />
                                    <asp:ListItem Text="Email Address" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div id="divFind" class="attributesRow" style="width:100%; padding-right:5px;">
                            <div Name="LeftColLabel" class="attributesLabel">&nbsp;</div>
                            <div class="attributesValue" style="padding-left:5px;">
                                <button id="buttonSearch" onclick="buttonSearch_click();return false;">Search</button>
                            </div>
                        </div>
                        <div>&nbsp;</div>
                        <div id="divUserListLabel" style="width:100%;">
                            <div id="divDetailsSep" class="horizontalSpacer" style="width:100%; margin-bottom:5px;">&nbsp;</div>
                            <div class="attributesLabel" style="width:100%; padding-left:5px;">Available Users: </div>
                        </div>
                        <div id="divUserList" style="width:100%; padding-left:5px;">
                            <div Name="LeftColLabel" class="attributesLabel">&nbsp;</div>
                            <div class="attributesValue">
                                <select id="listUsers" style="width:400px; height:75px;" size="8" onchange="listUsers_change(this);return false;"></select>
                            </div>
                        </div>
                    </div>

                    <div id="divProfileUsername" class="attributesRow" style="width:100%; padding-left:5px;">
                        <div class="attributesRequired">&nbsp;</div>
                        <div Name="LeftColLabel" class="attributesLabel">Username: </div>
                        <div class="attributesValue">
                            <asp:TextBox runat="server" ID="txtProfileUserName" Name="ProfileUserName" BackColor="#cccccc" onkeydown="return false;"/>
                        </div>
                    </div>
                    <div id="divValidationErrors" class="attributesRow" style="width:100%; padding-left:5px;">
                        <div class="attributesRequired">&nbsp;</div>
                        <div Name="LeftColLabel" class="attributesLabel">&nbsp;</div>
                        <div class="attributesValue">
                            <asp:ValidationSummary ID="ProfileValidationSummary" runat="server" DisplayMode="List" ForeColor="Red" ShowMessageBox="true" ShowValidationErrors="true" ShowSummary="true" ShowModelStateErrors="true" ValidationGroup="Profile" />
                        </div>
                    </div>
                    <div id="divLeftColumn" class="attributes" style="width:300px; float:left;">
                        <div id="divNameContainer" class="attributesRow" style="width:100%;">
                            <div id="divPrefix" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Prefix: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtPrefix" runat="server" Width="150"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divFirstName" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">First Name: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtFirstName" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorFirstName" runat="server" ControlToValidate="txtFirstName" SetFocusOnError="true"
                                        CssClass="field-validation-error" ErrorMessage="First name is required." ToolTip="First name is required." Text="*" ForeColor="Red" ValidationGroup="Profile" />
                                </div>
                            </div>
                            <div id="divMiddleName" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Middle Name: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtMiddleInit" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divLastName" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Last Name: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtLastName" runat="server" Width="150" CssClass="UserNamePart"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorLastName" runat="server" ControlToValidate="txtLastName" SetFocusOnError="true"
                                        CssClass="field-validation-error" ErrorMessage="Last name is required." ToolTip="Last name is required." Text="*" ForeColor="Red" ValidationGroup="Profile" />
                                </div>
                            </div>
                            <div id="divSuffix" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Suffix: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtSuffix" runat="server" Width="150"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div id="divPhoneContainer" class="attributesRow" style="width:100%;">
                            <div id="divPhoneOffice" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Phone: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtPhone" runat="server" TextMode="Phone" Width="115"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divPhoneMobile" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Mobile: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtPhone_Mobile" runat="server" TextMode="Phone" Width="115"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divPhoneMisc" class="attributesRow" style="width:100%; padding-left:5px; display:none;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div name="LeftColLabel" class="attributesLabel">Misc:&nbsp;&nbsp;</div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtPhone_Misc" runat="server" Width="115"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divFax" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div name="LeftColLabel" class="attributesLabel">Fax: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtFax" runat="server" Width="115"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="divRightColumn" class="attributes" style="width:450px; float:left;">
                        <div id="divEmailContainer" class="attributesRow" style="width:100%;">
                            <div id="divEmail" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Email Address: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtProfileEmail" Name="ProfileEmail" runat="server" TextMode="Email" Width="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorProfileEmail" runat="server" ControlToValidate="txtProfileEmail" SetFocusOnError="true"
                                        CssClass="field-validation-error" ErrorMessage="The email address field is required." ToolTip="The email address field is required." ForeColor="Red" Text="*" ValidationGroup="Profile" />
                                    <asp:CustomValidator ID="CustomEmailValidator" runat="server" ControlToValidate="txtProfileEmail" SetFocusOnError="true" ForeColor="Red" OnServerValidate="CustomEmailValidator_ServerValidate" 
                                        ErrorMessage="A user already exists with the specified email address." ToolTip="A user already exists with the specified email address." Text="*" ValidationGroup="Profile"></asp:CustomValidator>

                                    <asp:HiddenField ID="txtExistingUsername" runat="server" />
                                </div>
                            </div>
                            <div id="divResetPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">&nbsp;</div>
                                <div class="attributesValue">
                                    <a id="anchorReset" type="text/html" href="#" class="btn_Link">Reset Password?</a>
                                </div>
                            </div>
                            <div id="divEmail2" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Email2: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtEmail2" runat="server" TextMode="Email" Width="250"></asp:TextBox>
                                </div>
                            </div>
                        </div>
        
                        <div id="divAddressContainer" class="attributesRow" style="width:100%;">
                            <div id="divAddress" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Address: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtAddress" runat="server" Width="250"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divAddress2" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Address2: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtAddress2" runat="server" Width="250"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divCity" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">City: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtCity" runat="server" Width="150"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divState" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">State: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtState" runat="server" Width="50"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divPostalCode" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Postal Code: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtPostalCode" runat="server" Width="50"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divCountry" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">Country: </div>
                                <div class="attributesValue">
                                    <asp:TextBox ID="txtCountry" runat="server" Width="100"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:CreateUserWizardStep runat="server" ID="RegisterUserWizardStep">
                <ContentTemplate>
                    <p class="message-info">
                        Passwords are required to be a minimum of <%: Membership.MinRequiredPasswordLength %> characters in length.
                    </p>
                    <p class="validation-summary-errors">
                        <asp:Literal runat="server" ID="ErrorMessage" />
                    </p>
                    <fieldset>
                        <legend>Register / Request Access</legend>
                        <div id="divRegistrationFields" class="attributes" style="width:100%; padding-left:10px; padding-right:5px;">
                            <h3 class="message-info">
                                Registration details
                            </h3>
                            <div id="divUsername" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Username: </div>
                                <div class="attributesValue">
                                    <asp:TextBox runat="server" ID="UserName" Name="UserName" BackColor="#cccccc" onkeydown="return false;"/>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName"
                                        CssClass="field-validation-error" ErrorMessage="The user name field is required." ForeColor="Red" />
                                </div>
                            </div>
                            <div id="divEmail" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Email address: </div>
                                <div class="attributesValue">
                                    <asp:TextBox runat="server" ID="Email" TextMode="Email" Width="250" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Email"
                                        CssClass="field-validation-error" ErrorMessage="The email address field is required." ForeColor="Red" />
                                </div>
                            </div>
                            <div id="divPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Password: </div>
                                <div class="attributesValue">
                                    <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Password"
                                        CssClass="field-validation-error" ErrorMessage="The password field is required." ForeColor="Red" />
                                </div>
                            </div>
                            <div id="divConfirmPassword" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Confirm Password: </div>
                                <div class="attributesValue">
                                    <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ConfirmPassword"
                                        CssClass="field-validation-error" ForeColor="Red" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                                            CssClass="field-validation-error" ForeColor="Red" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                                </div>
                            </div>
                            <div id="divQuestion" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Security Question: </div>
                                <div class="attributesValue">
                                    <asp:DropDownList runat="server" ID="Question"></asp:DropDownList>
                                </div>
                            </div>
                            <div id="divAnswer" class="attributesRow" style="width:100%; padding-left:5px;">
                                <div class="attributesRequired">*</div>
                                <div Name="LeftColLabel" class="attributesLabel">Answer: </div>
                                <div class="attributesValue">
                                    <asp:TextBox runat="server" ID="Answer"></asp:TextBox>
                                </div>
                            </div>
                            <div id="divRegister" class="attributesRow" style="width:100%; padding-left:5px; padding-top:5px;">
                                <div class="attributesRequired">&nbsp;</div>
                                <div Name="LeftColLabel" class="attributesLabel">&nbsp;</div>
                                <div class="attributesValue">
                                    <asp:Button ID="ButtonMoveNext" runat="server" CommandName="MoveNext" Text="Next" class="nextButton" />
                                </div>
                            </div>
                        </div>
                    </fieldset>
                </ContentTemplate>
                <CustomNavigationTemplate />
            </asp:CreateUserWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
    </div>
    <script type="text/javascript">
        function listUsers_change(list) {
            var option = $(list).find('option:selected');

            $('#<%=this.txtProfileUserId.ClientID %>').val(option.attr('userId'));
            $('#<%=this.txtProfileUserName.ClientID %>').val(option.attr('userName'));
            $('#<%=this.txtPrefix.ClientID %>').val(option.attr('prefix'));
            $('#<%=this.txtFirstName.ClientID %>').val(option.attr('first_name'));
            $('#<%=this.txtLastName.ClientID %>').val(option.attr('last_name'));
            $('#<%=this.txtSuffix.ClientID %>').val(option.attr('suffix'));
            $('#<%=this.txtMiddleInit.ClientID %>').val(option.attr('middle_name'));
            $('#<%=this.txtProfileEmail.ClientID %>').val(option.attr('email'));
            $('#<%=this.txtEmail2.ClientID %>').val(option.attr('email2'));
            $('#<%=this.txtPhone.ClientID %>').val(option.attr('phone'));
            $('#<%=this.txtPhone_Mobile.ClientID %>').val(option.attr('mobile'));
            $('#<%=this.txtAddress.ClientID %>').val(option.attr('address'));
            $('#<%=this.txtAddress2.ClientID %>').val(option.attr('address2'));
            $('#<%=this.txtCity.ClientID %>').val(option.attr('city'));
            $('#<%=this.txtState.ClientID %>').val(option.attr('state'));
            $('#<%=this.txtPostalCode.ClientID %>').val(option.attr('postalCode'));
            $('#<%=this.txtCountry.ClientID %>').val(option.attr('country'));

            $('#divFindUsers').css('display', 'none');
        }

        function buttonFindProfile_click() {
            $('#divFindUsers').css('display', 'block');
        }

        function buttonSearch_click() {
            var searchType = $('#<%=this.rblSearchType.ClientID %> input:checked').val();
            var userName = '', email = '';
            if (searchType.toUpperCase().indexOf('USERNAME') >= 0) {
                userName = $('#<%=this.txtUsername_Find.ClientID %>').val();
            }
            else if (searchType.toUpperCase().indexOf('EMAIL') >= 0) {
                email = $('#<%=this.txtEmail_Find.ClientID %>').val();
            }
            
            var data = '{' +
                '"userName:"' + userName +
                '",email:"' + email +
                '"}';

            var url = '//' +
                window.location.host + '/' +
                window.location.pathname +
                '.aspx/FindUnregisteredUsers ';

            $('#listUsers option').remove();

            $.ajax({
                type: "POST",
                url: url,
                data: data,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: search_done,
                error: on_error
            });
        }
        function search_done(result) {
            try {
                if (result == undefined || result == null || result == 'null'
                    || result.d == undefined || result.d == null || result.d == 'null') {
                    alert('No unregistered profiles were found for the specified search criteria.');
                    return false;
                }
                var obj = jQuery.parseJSON(result.d);
                
                $.each(obj, function (index, val) {
                    //add items to listbox if they exist
                    var optionHtml = '<option userId="' + val.Id + '" ' + 
                        'userName="' + val.Username + '" ' + 
                        'prefix="' + val.Prefix + '" ' + 
                        'first_name="' + val.First_Name + '" ' + 
                        'last_name="' + val.Last_Name + '" ' + 
                        'suffix="' + val.Suffix + '" ' + 
                        'middle_Name="' + val.Middle_Name + '" ' + 
                        'email="' + val.Email + '" ' + 
                        'emal2="' + val.Email2 + '" ' + 
                        'phone="' + val.Phone_Office + '" ' + 
                        'mobile="' + val.Phone_Mobile + '" ' + 
                        'address="' + val.Address + '" ' + 
                        'address2="' + val.Address2 + '" ' + 
                        'city="' + val.City + '" ' + 
                        'state="' + val.State + '" ' + 
                        'postalCode="' + val.PostalCode + '" ' + 
                        'country="' + val.Country + '" onclick="listUsers_click(this);return false;">' +
                        val.Username + ' : ' + val.Last_Name + ', ' + val.First_Name + ' : ' + val.Email +
                        '</option';

                    $('#listUsers').append(optionHtml);
                });
            } catch (e) {
                //log error?
            }
        }
        function on_error(result) {
            alert('Error searching for profile(s): ' + result.d);
        }

        function buildUserName() {
            var fName = '', mName = '', lName = '', userName = '';

            fName = $('#<%=this.txtFirstName.ClientID %>').val().replace(' ', '');
            mName = $('#<%=this.txtMiddleInit.ClientID %>').val().replace(' ', '');
            lName = $('#<%=this.txtLastName.ClientID %>').val().replace(' ', '');

            if (mName.length > 0) {
                mName += '.';
            }

            userName = fName + '.' + mName + lName;

            $('#<%=this.txtProfileUserName.ClientID %>').val(userName);
        }

        function textBox_readonly_keypress(element) {
            return false;
        }

        function setAttributeLabelWidth(name, width) {
            $('.attributesLabel[name="' + name + '"]').each(function () {
                $(this).width(width);
            });
        }

        $(document).ready(function () {
            setAttributeLabelWidth('LeftColLabel', 115);
            $('[name$="StartNextButton"]').css('margin-left', '130px');

            $('.UserNamePart').bind('keyup', function () {
                buildUserName();
                return false;
            });
            
            if ($('#<%=this.txtExistingUsername.ClientID %>').val().length > 0) {
                $('#divResetPassword').show();
            }
            else {
                $('#divResetPassword').hide();
            }
            $('#anchorReset').bind('click', function () {
                window.location.href = "Reset.aspx?requestReset=true&requestType=email"
                                    + '&email=' + $('#<%=this.txtProfileEmail.ClientID %>').val();
            });
        });
    </script>
    </form>
</body>
</html>
