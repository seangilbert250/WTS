<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConfigureHotlist.aspx.cs" Inherits="Admin_ConfigureHotlist" theme="Default"%>

<!DOCTYPE html>
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.2/jquery.min.js"></script>
<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
<script src="../Scripts/shell.js"></script>

    <style>
        *{
            font-family: Arial;
            font-size: 12px;
        }
        table{
            border-collapse: collapse; 
            width: 100%;
        }
        .header {
            background-size: contain;
            text-align: center;
            font-size: 14px;
        }
        .header th{
            border: 1px solid black;
            padding-bottom: 10px;
            padding-top: 10px;
            background: url("../Images/Headers/gridheaderblue.png");
            text-align: center;
        }
        .listBoxButton{
            width: 100px;
        }
        .columnBody{
            border: 1px solid grey;
            vertical-align: top;
        }
        .listBox {
            cursor:default;
            font-size:12px;
            border:solid 1px grey;
            overflow-x:hidden;
            overflow-y:auto;
            width: 180px;
            height: 100%;
            padding-top: 3px;
            padding-bottom: 3px;
            padding-left: 3px;
        }
        .icon{
             cursor: pointer; 
             position: relative; 
             top: 3px; 
        }
        .ListBoxHeader{
            vertical-align: top;
            text-align: center;
        }
        .listBoxContainer{
            text-align: center; 
            padding-top: 10px; 
            padding-left: 10px;
        }
        .listBoxButtonColumn{
            vertical-align: middle;
        }
        #headerTwo, #headerThree, #headerFour{
            width: 27%;
        }
        #headerOne{
            width: 20%;
        }
        #ConfigHeader{
            background-image : url("../Images/Headers/grey.gif");
            background-size: 100% 100%;
            padding-bottom: 10px;
            padding-left: 20px;
            padding-right: 20px;
        }
        #emailNow, #apply {
            width: 100px;
            padding-right: 5px;
        }

    </style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EMAIL HOSTLIST</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="ConfigHeader">
            <h2 class="header">EMAIL HOSTLIST</h2>
            <div id="buttonRow">        
                <asp:DropDownList ID="ddlConfig" runat="server" Font-Size="9" Width="245px">
                </asp:DropDownList>
                <img id="imgSaveConfig" src="../Images/icons/disk.png" title="Save Hotlist Configuration" alt="Save Configuration" class="icon" />
                <img id="imgDeleteConfig" src="../Images/icons/delete.png" title="Delete Hotlist Configuration" alt="Delete Configuration" class="icon" />
                	<div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
	                <div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
		                <table style="width: 100%;">
			                <tr>
				                <td class="pageContentInfo">
					                Hotlist Config:
				                </td>
			                </tr>
			                <tr>
				                <td>
					                <input type="text" id="txtViewName" />
				                </td>
			                </tr>
			                <tr>
				                <td>
					                <input type="button" id="buttonSaveView" value="Save" />&nbsp;<input type="button" id="buttonCancelView" value="Cancel" />
				                </td>
			                </tr>
		                </table>
	                </div>
                </div>
        </div>
        <div>
            <table id="paramsBody">
                <tr class="header">
                    <th id="headerOne">Production Settings</th><th id="headerTwo">Status</th><th id="headerThree">Assigned To</th><th id="headerFour">Recipients</th>
                </tr>
                <tr>
                    <td class="columnBody">
                        <table>
                            <tr>
                                <td>
                                    <br />
                                    <fieldset>
                                        <legend>Production Status</legend>
                                        <asp:Panel runat="server" id="prodStatus"></asp:Panel>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                    <fieldset>
                                        <legend>Tech Rank</legend>
                                        <br />
                                        Min <select id="ddlTechMin" width="50" runat="server"></select>&nbsp; 
                                        Max <select id="ddlTechMax" wdith="50" runat="server"></select>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                    <fieldset>
                                        <legend>Bus Rank</legend>
                                        <br />
                                        Min <select id="ddlBusMin" width="50" runat="server"></select> &nbsp;
                                        Max <select id="ddlBusMax" wdith="50" runat="server"></select>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td class="columnBody">
                        <div class="listBoxContainer">
			                <table>
				                <tr class="ListBoxHeader">
					                <td>Available Status</td>
					                <td>&nbsp;</td>
					                <td>Selected Stasus</td>
				                </tr>
				                <tr class="ListBoxHeader">
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxStatusAvailable" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>

					                </td>
					                <td class="listBoxButtonColumn">
									        <button id="btnAddStatus" class="listBoxButton">>></button><br />
									        <button id="btnRemoveStatus" class="listBoxButton"><<</button><br />
									        <button id="btnClearAllStatus" class="listBoxButton">Clear All</button><br />
					                </td>
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxStatusSelect" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>
					                </td>
				                </tr>
			                </table>
		                 </div>
                    </td>
                    <td class="columnBody">
                        <div class="listBoxContainer">
			                <table>
				                <tr class="ListBoxHeader">
					                <td>Available Assigned</td>
					                <td>&nbsp;</td>
					                <td>Selected Assigned</td>
				                </tr>
				                <tr class="ListBoxHeader">
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxAssignedAvailable" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>
					                </td>
					                <td class="listBoxButtonColumn">
									        <button id="btnAddAssigned" class="listBoxButton">>></button><br />
									        <button id="btnRemoveAssigned" class="listBoxButton"><<</button><br />
									        <button id="btnClearAllAssigned" class="listBoxButton">Clear All</button><br />
					                </td>
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxAssignedSelect" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>
					                </td>
				                </tr>
			                </table>
		                 </div>
                    </td>
                  <td class="columnBody">
                        <div class="listBoxContainer">
			                <table>
				                <tr class="ListBoxHeader">
					                <td>Available Recipients</td>
					                <td>&nbsp;</td>
					                <td>Selected Recipients</td>
				                </tr>
				                <tr class="ListBoxHeader">
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxRecipientAvailable" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>
					                </td>
					                <td class="listBoxButtonColumn">
									        <button id="btnAddRecipient" class="listBoxButton">>></button><br />
									        <button id="btnRemoveRecipient" class="listBoxButton"><<</button><br />
									        <button id="btnClearAllRecipient" class="listBoxButton">Clear All</button><br />
					                </td>
					                <td>
                                        <asp:ListBox runat="server" id="ListBoxRecipientSelect" CssClass="listBox" Rows="15" SelectionMode="Multiple"></asp:ListBox>
					                </td>
				                </tr>
			                </table>
		                 </div>
                    </td>
            </table>
        </div>
    </div>
    <br /> <br />
    <fieldset> 
        <legend>Message:</legend>
        <textarea id="emailMessage" rows="6" cols="315"></textarea>
    </fieldset>
        <br /> <br />
    <div id="submitButtons" align="left">
        <button type="button" id="emailNow">Send Now</button>
        <button type="button" id="apply">Set as Default</button>
<%--        <button type="button" id="TESTemailNow">TEST - Steve only</button>--%>
<%--        <button type="button" id="TESTemailNowSR">TEST SR Report - Steve only</button>--%>
    </div>

 <asp:ScriptManager ID="EmailHostlist" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script type="text/javascript">
        $(document).ready(function () {

            $('form').submit(function (event) {
                event.preventDefault();
            });

            $('#btnAddStatus').click(function () { btnAdd($('#ListBoxStatusAvailable'), $('#ListBoxStatusSelect')); });
            $('#btnAddAssigned').click(function () { btnAdd($('#ListBoxAssignedAvailable'), $('#ListBoxAssignedSelect')); });
            $('#btnAddRecipient').click(function () { btnAdd($('#ListBoxRecipientAvailable'), $('#ListBoxRecipientSelect')); });

            $('#btnRemoveStatus').click(function () { btnRemove($('#ListBoxStatusAvailable'), $('#ListBoxStatusSelect')); });
            $('#btnRemoveAssigned').click(function () { btnRemove($('#ListBoxAssignedAvailable'), $('#ListBoxAssignedSelect')); });
            $('#btnRemoveRecipient').click(function () { btnRemove($('#ListBoxRecipientAvailable'), $('#ListBoxRecipientSelect')); });

            $('#btnClearAllStatus').click(function () { btnClearAll($('#ListBoxStatusAvailable'), $('#ListBoxStatusSelect')); });
            $('#btnClearAllAssigned').click(function () { btnClearAll($('#ListBoxAssignedAvailable'), $('#ListBoxAssignedSelect')); });
            $('#btnClearAllRecipient').click(function () { btnClearAll($('#ListBoxRecipientAvailable'), $('#ListBoxRecipientSelect')); });
            $('#apply').click(function () { setActive() });


            $('#ddlConfig').change(function () {
                configID = $('#ddlConfig option:selected').val();
                if (configID) {
                    PageMethods.getConfig(configID,
                        function (config) {
                            config = JSON.parse(config);
                            if (!setPageTo(config)) {
                            MessageBox("There was an error applying this configuration", "ERROR");
                        }
                    }, onError);
                }
            });

            $('#ddlConfig').trigger("change"); //default configuration is the active configuration. Go to the server and get active config, then change the menus to reflect that config. 

            $('#imgSaveConfig').click(function () {
                $('#divDimmer').show();
                var pos = $(this).position();
                var width = $('#divViewName').outerWidth();
                $('#divViewName').css({
                    position: "absolute",
                    top: pos.top + "px",
                    left: (pos.left) + "px"
                }).slideDown(function () { $('#txtViewName').focus(); });
            });

            $('#txtViewName').on("keypress", function (e) {
                if (e.which == 13) {
                    $('#buttonSaveView').trigger('click');
                    return false;
                }
            });

            $('#buttonSaveView').click(function () {
                var name = $('#txtViewName').val();
                var config = gatherReportParameters();
                PageMethods.addConfig(config.ProdStatus, config.TechMin, config.BusMin, config.TechMax, config.BusMax, config.status, config.assigned, config.recipients, config.message, name,
                    function (result) {
                        $('#ddlConfig').append('<option value=\'' + result.Key.toString() + '\'>' + result.Value.toString() + '</option>');
                        MessageBox("Saved Successfully", "Saved");
                        $('#buttonCancelView').trigger("click");
                    }
                    , onError);
            });

            $('#emailNow').click(function () {
                var config = gatherReportParameters();
                MessageBox("Gathering data and sending message...", "");  // Use this here instead - There is no immediate confirmation to user.
                PageMethods.sendHotlist(config.ProdStatus, config.TechMin, config.BusMin, config.TechMax, config.BusMax, config.status, config.assigned, config.recipients, config.message,
                    function (result) {
                        //MessageBox("Message Sent", "");
                    }
                    , onError);
            });

            $('#TESTemailNow').click(function () {
                var config = gatherReportParameters();
                PageMethods.TESTsendHotlist(config.ProdStatus, config.TechMin, config.BusMin, config.TechMax, config.BusMax, config.status, config.assigned, config.recipients, config.message,
                    function (result) {
                        MessageBox("Message Sent to STEVE", "");
                    }
                    , onError);
            });

            $('#TESTemailNowSR').click(function () {
                PageMethods.TESTSendSrReport(
                    function (result) {
                        MessageBox("SR Message Sent to STEVE", "");
                    }
                    , onError);
            });


            $('#buttonCancelView').click(function () {
                $('#divViewName').slideUp(function () {
                    $('#divDimmer').hide();
                });
            });

            $('#imgDeleteConfig').click(function () {
                deleteID = $('#ddlConfig option:selected').val();
                PageMethods.deleteConfig(deleteID,
                   function () {
                       $("#ddlConfig option[value=\"" + deleteID + "\"]").remove();
                       MessageBox("Deleted Successfuly", "Success");
                   }, onError);
            });


            function btnAdd(listboxAvailable, listBoxSelected) {
                $(listboxAvailable).find('option:selected').each(function () {
                    $(listBoxSelected).append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                    $(this).remove();
                });
            };

            function btnRemove(listboxAvailable, listBoxSelected) {
                $(listBoxSelected).find('option:selected').each(function () {
                    $(listboxAvailable).append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                    $(this).remove();
                });
            };

            function btnClearAll(listboxAvailable, listBoxSelected) {
                $(listBoxSelected).find('option').each(function () {
                    $(listboxAvailable).append('<option value=\"' + $(this).val() + "\">" + $(this).text() + "</option>");
                    $(this).remove();
                });
            };

            function setActive() {
                selectedConfig = $('#ddlConfig option:selected');
                PageMethods.setActive(selectedConfig.val());
                $('#ddlConfig option').each(function () {
                    this.text = this.text.replace('(Active) ', '');
                });
                selectedConfig.html('(Active) ' + selectedConfig.html().toString());
            };

            function setPageTo(config) {
                try {
                    clearMenu();
                    setProdStatus(config.ProdStatus);
                    setTechMin(config.TechMin);
                    setTechMax(config.TechMax);
                    setBusMin(config.BusMin);
                    setBusMax(config.BusMax);
                    setStatus(config.status);
                    setAssigned(config.assigned);
                    setRecipients(config.recipients);
                    setMessage(config.message);
                }
                catch (e) {
                    return false;
                }
                return true;
            };

            function clearMenu() {
                $('#prodStatus').find('input').each(function () {
                    $(this).prop('checked', false);
                });

                $('#btnClearAllStatus').trigger('click');
                $('#btnClearAllAssigned').trigger('click');
                $('#btnClearAllRecipient').trigger('click');
                $('#ddlTechMin').val(1);
                $('#ddlBusMin').val(1);
                $('#ddlTechMax').val(1);
                $('#ddlBusMax').val(1);
                $('#emailMessage').val('');
            };

            function setProdStatus(ProdStatus) {
                try{
                    $('#prodStatus').find('input').each(function () {
                        var statusID = parseInt($(this).attr('StatusID'));
                        if (ProdStatus.indexOf(statusID) > -1){
                            $(this).prop("checked", true);
                        }
                    });
                }
                catch (e){}
            };

            function setTechMin(value) {
                $('#ddlTechMin').val(value);
            };

            function setTechMax(value) {
                $('#ddlBusMin').val(value);
            };

            function setBusMin(value) {
                $('#ddlTechMax').val(value);
            };

            function setBusMax(value) {
                $('#ddlBusMax').val(value);
            };

            function setStatus(status) {
                if (!status) return;

                status.forEach(function (value) {
                    $('#ListBoxStatusAvailable').val(value);
                    $('#btnAddStatus').trigger('click');
                });
            };

            function setAssigned(assigned) {
                if (!assigned) return;

                assigned.forEach(function (value) {
                    $('#ListBoxAssignedAvailable').val(value);
                    $('#btnAddAssigned').trigger('click');
                });
            };

            function setRecipients(recipients) {
                if (!recipients) return;

                recipients.forEach(function (value) {
                    $('#ListBoxRecipientAvailable').val(value);
                    $('#btnAddRecipient').trigger('click');
                });
            };

            function setMessage(message) {
                if (!message) return;

                $('#emailMessage').val(message.toString());
            };

            function gatherReportParameters() {
                var config = {};
                config.ProdStatus = getProdStatus();
                config.TechMin = getTechMin();
                config.TechMax = getTechMax();
                config.BusMin = getBusMin();
                config.BusMax = getBusMax();
                config.status = getStatus();
                config.assigned = getAssigned();
                config.recipients = getRecipients();
                config.message = getMessage();
                return config;
            };

            function getProdStatus() {
                var prodStatus = [];
                try {
                    $('#prodStatus').find('input:checked').each(function () {
                        prodStatus.push($(this).attr('StatusID'));
                    });
                }
                catch (e) { }

                return prodStatus.join();
            };

            function getTechMin() {
                var minValue, maxValue;
                try {
                    minValue = parseInt($('#ddlTechMin option:selected').val());
                    maxValue = parseInt($('#ddlTechMax option:selected').val());

                    if (minValue && maxValue && minValue > maxValue) {
                        $('#ddlTechMin').val(maxValue);
                        $('#ddlTechMax').val(minValue);
                        minValue = maxValue;
                    }
                }
                catch (e) {
                    return 1;
                }

                if (minValue) {
                    return minValue;
                }
                else return 1;
            };

            function getTechMax() {
                var minValue, maxValue;
                try {
                    minValue = parseInt($('#ddlTechMin option:selected').val());
                    maxValue = parseInt($('#ddlTechMax option:selected').val());

                    if (minValue && maxValue && maxValue < minValue) {
                        $('#ddlTechMin').val(maxValue);
                        $('#ddlTechMax').val(minValue);
                        maxValue = minValue;
                    }
                }
                catch (e) {
                    return 10;
                }

                if (minValue) {
                    return maxValue;
                }
                else return 10;
            };

            function getBusMin() {
                var minValue, maxValue;
                try {
                    minValue = parseInt($('#ddlBusMin option:selected').val());
                    maxValue = parseInt($('#ddlBusMax option:selected').val());

                    if (minValue && maxValue && minValue > maxValue) {
                        $('#ddlBusMin').val(maxValue);
                        $('#ddlBusMax').val(minValue);
                        minValue = maxValue;
                    }
                }
                catch (e) {
                    return 1;
                }

                if (minValue) {
                    return minValue;
                }
                else return 1;
            };

            function getBusMax() {
                var minValue, maxValue;
                try {
                    minValue = parseInt($('#ddlBusMin option:selected').val());
                    maxValue = parseInt($('#ddlBusMax option:selected').val());

                    if (minValue && maxValue && maxValue < minValue) {
                        $('#ddlBusMin').val(maxValue);
                        $('#ddlBusMax').val(minValue);
                        maxValue = minValue;
                    }
                }
                catch (e) {
                    return 10;
                }

                if (minValue) {
                    return maxValue;
                }
                else return 10;
            };

            function getStatus() {
                var status = [];
                try {
                    $('#ListBoxStatusSelect option').each(function () {
                        status.push($(this).val());
                    });
                }
                catch (e) { }
                return status.join();
            };

            function getAssigned() {
                var assigned = [];
                try {
                    $('#ListBoxAssignedSelect option').each(function () {
                        assigned.push($(this).val());
                    });
                }
                catch (e) { }
                return assigned.join();
            };

            function getRecipients() {
                var recipients = [];
                try {
                    $('#ListBoxRecipientSelect option').each(function () {
                        recipients.push($(this).val());
                    });
                }
                catch (e) { }
                return recipients.join(';') + ';';
            };

            function getMessage() {
                return $('#emailMessage').val();
            };

            function onError(ex) {
                MessageBox(ex.get_message(), "Error!");
            };

        });
    </script>
    </form>
</body>
</html>
