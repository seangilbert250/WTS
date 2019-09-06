<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QuestionBox.aspx.cs" Inherits="QuestionBox" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" src="scripts/shell.js"></script>
    <script type="text/javascript" src="scripts/common.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="qb_Body">
            <table style="width:100%;">
                <tr>
                    <td id="qb_Image" style="width:40px; vertical-align:top; height:100%; padding-top:10px; padding-left:10px;">  
                    </td>
                    <td id="qb_Question" style="vertical-align:middle; height:100%; padding-top:10px;">
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="qb_Footer" class="PopupFooter">
            <table cellpadding="0" cellspacing="0" style="width:100%;">
                <tr>
                    <td style="width:100%; text-align:right;">
                        <div id="qb_Buttons" style="width:100%;">
                        </div>
                    </td>
                </tr>
            </table>
            
        </div>
    <script type="text/javascript">
        var qb_Question = document.getElementById('qb_Question');
        var qb_Image = document.getElementById('qb_Image');
        var qb_Buttons = document.getElementById('qb_Buttons');
        var qb_Question = document.getElementById('qb_Question');

        var vQuestion = '<%=Request.QueryString["question"] %>';
        var vImage = '<%=Request.QueryString["image"] %>';
        var vTitle = '<%=Request.QueryString["title"] %>';
        var vButtons = '<%=Request.QueryString["buttons"] %>';
        var vFunction = '<%=Request.QueryString["function"] %>';
        var vName = '<%=Request.QueryString["name"] %>';
        var vParam = '<%=Request.QueryString["param"] %>';
        var vMode = '<%=Request.QueryString["mode"]%>';
        var vMaxLength = '<%=Request.QueryString["maxlength"]%>';
        var vReq = '<%=Request.QueryString["req"]%>';
        var vDefVal = '<%=Request.QueryString["defval"]%>';

        vQuestion = UndoStrongEscape(vQuestion);

        setupQuestionBox();

        function setupQuestionBox() {
            try {
                qb_Question.innerHTML = vQuestion + (vMode == 'textinput' ? '<br /><input id="qb_InputAnswer" type="text" maxlength="' + vMaxLength + '" value="' + vDefVal.replace("\"", "") + '" style="margin-top:5px;width:90%;border:1px solid #666666;">' : '');
                var image = document.createElement('img');
                if (vImage == '') {
                    vImage = 'Images/Icons/questionBox.gif';
                }
                image.src = vImage;
                qb_Image.appendChild(image);
                setupButtons();

                if (vMode == 'textinput') {
                    document.getElementById('qb_InputAnswer').focus();
                }
            }
            catch (e) {
            }
        }

        function setupButtons() {
            try {
                var splitButtons = vButtons.split(',');
                for (var i = 0; i <= splitButtons.length - 1; i++) {
                    var button = document.createElement('button');

                    button.id = 'QB_Button_' + splitButtons[i];
                    button.onclick = function () { return buttonClick(this.innerText); };
                    button.innerText = splitButtons[i];
                    qb_Buttons.appendChild(button);
                }
            }
            catch (e) {
                generateAppErrorMessage(e.number, 'setupButtons(' + e.message + ')');
            }
        }

        function buttonClick(pButton) {
            try {
                if (opener[vFunction]) {
                    var inputAnswer = null;
                    if (vMode == 'textinput') {
                        inputAnswer = document.getElementById('qb_InputAnswer').value.trim();
                        if (inputAnswer == '' && vReq == 'true') {
                            var pButtonLower = pButton.toLowerCase();
                            if (pButtonLower == 'yes' || pButtonLower == 'ok' || pButtonLower == 'save') {
                                document.getElementById('qb_InputAnswer').style.border = '1px solid red';
                                return false;
                            }
                        }
                    }

                    var otherInputAnswers = '';
                    var inputs = document.getElementsByTagName('input');
                    for (var i = 0; i < inputs.length; i++) {
                        if (inputs[i].type == 'checkbox') {                            
                            if (inputs[i].checked) {
                                if (otherInputAnswers.length > 0) otherInputAnswers += ',';
                                otherInputAnswers += inputs[i].id + '=' + inputs[i].value;
                            }
                        }
                        else if (inputs[i].type == 'text' && inputs[i].id != 'qb_InputAnswer') {
                            if (inputs[i].value != '') {
                                if (otherInputAnswers.length > 0) otherInputAnswers += ',';
                                otherInputAnswers += inputs[i].id + '=' + StrongEscape(inputs[i].value);
                            }
                        }
                    }

                    var selects = document.getElementsByTagName('select');
                    for (var i = 0; i < selects.length; i++) {
                        var val = selects[i].options[selects[i].selectedIndex].value;
                        if (otherInputAnswers.length > 0) otherInputAnswers += ',';
                        otherInputAnswers += selects[i].id + '=' + val;
                    }

                    var tas = document.getElementsByTagName('textarea');
                    for (var i = 0; i < tas.length; i++) {
                        var val = tas[i].value;
                        if (otherInputAnswers.length > 0) otherInputAnswers += ',';
                        otherInputAnswers += tas[i].id + '=' + val;
                    }

                    if (otherInputAnswers.length > 0 && vParam != null && vParam.length > 0) otherInputAnswers = '|' + otherInputAnswers; // prepend answers with a pipe if there is a vParam

                    if (vParam) {
                        if (vMode == 'textinput') {
                            opener[vFunction](pButton, inputAnswer, vParam + otherInputAnswers);
                        }
                        else {
                            opener[vFunction](pButton, vParam + otherInputAnswers);
                        }
                    }
                    else {
                        if (vMode == 'textinput') {
                            opener[vFunction](pButton, inputAnswer, otherInputAnswers);
                        }
                        else {
                            opener[vFunction](pButton, otherInputAnswers);
                        }
                    }
                }
                else {
                    MessageBox('No returning function.');
                }
                closeWindow();
            }
            catch (e) {
                DisplayErrorMessage('buttonClick', e.number, e.message);
            }

            return true;
        }

    </script>
    <script type="text/javascript">
        //This sets the popup size based on the contents
        var messageDiv = document.getElementById('qb_Body');
        var popupFooter = document.getElementById('qb_Footer');
        popupManager.GetPopupByName('Question').SetWidth(messageDiv.offsetWidth + 10);
        var messageDivHeight = messageDiv.offsetHeight;
        if (messageDivHeight < 43) messageDivHeight = 43;
        popupManager.GetPopupByName('Question').SetHeight(messageDivHeight + popupFooter.offsetHeight + 10);

    </script>
    </form>
</body>
</html>
