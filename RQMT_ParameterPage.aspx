﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMT_ParameterPage.aspx.cs" Inherits="RQMT_ParameterPage" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>RQMT - Parameters</title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" /> 

</head>
<body>
	<form id="form1" runat="server">
		<div id="divPage" class="pageContainer">
			<div id="divTabsContainer" class="mainPageContainer">
				<ul id="HomePageTabs" runat="server">
					<li><a href="#divRQMTAdvParams" onclick="HomeTab_click('Parameters');">RQMT Parameters</a></li>
					<li><a href="#divRQMTFilters" onclick="HomeTab_click('Filters');">Filters</a></li>
				</ul>
				<div id="divRQMTFilters">
					<iframe id="frameFilters" name="frameFilters" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 98%; height: 95%; margin: 0px; padding: 0px;" title="">Dashboard</iframe>
				</div>
				<div id="divRQMTAdvParams">
					<iframe id="frameParams" name="frameParams" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 98%; height: 95%; margin: 0px; padding: 0px;" title="">WTS News</iframe>
				</div>
			</div>
            <div id="paramFooter" class="PopupFooter">
			    <table>
				    <tr>
                        <td>
						    <button id="buttonSave" style="float: left;">Save</button>
					    </td>
					    <td>
						    <button id="buttonGetData" style="width: 80px">Get Data</button>
					    </td>
					    <td>
						    <button id="btnCancel" onclick="closeWindow(); return false;">Cancel</button>
					    </td>
				    </tr>
			    </table>
		    </div>
		</div>

        <div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
        <div id="divViewName" style="width: 220px; height: 160px; background-color: white; z-index: 999; display: none;">
            <div class="PopupWindow_Header" style="text-align: center; vertical-align: middle; width: 100%;">
                <table style="width: 100%;">
				    <tr>
                        <td>SAVE RQMT GRIDVIEW / FILTERS</td>
				    </tr>
			    </table>
            </div>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td style="width: 155px;">
                        Filter Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtFilterName" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        Grid View:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlSaveView" runat="server" Width="104px" style="background-color: #F5F6CE;"></asp:DropDownList>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr id="trViewName">
                    <td style="width: 5px;">
                        <span style="color: red;">*</span>
                    </td>
                    <td>
                        Grid View Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtViewName" runat="server" MaxLength="50" Width="100"></asp:TextBox>
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="width: 5px;">
                        <span style="color: red;">&nbsp;</span>
                    </td>
                    <td>
                        Process View:
                    </td>
                    <td>
                        <input type="checkbox" id="chkProcessView" style="vertical-align: middle; margin: 0px;" />
                    </td>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                </tr>
            </table>
            <div class="PopupFooter">
                <table>
				    <tr>
                        <td>
						    <input type="button" id="buttonSaveView" value="Save" />
					    </td>
					    <td>
						    <input type="button" id="buttonCancelView" value="Cancel" />
					    </td>
				    </tr>
			    </table>
            </div>
        </div>

		<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
			<Services>
				<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
			</Services>
		</asp:ScriptManager>

	</form>

	<script type="text/javascript">

		$(document).ready(function () {
			try {
                _tabToLoad = '<%=this.TabToLoad %>';
                $('#buttonGetData').click(function () { buttonGetData_click(); return false; });
                $('#buttonSave').click(function () { buttonSave_click(this); return false; });
                $('#buttonSaveView').click(function () { buttonSaveView_click(); return false; });
                $('#buttonCancelView').click(function () { $('#divDimmer').hide(); $('#divViewName').hide(); });
                $('#ddlSaveView').on('change', function () { ddlSaveView_change(); });
                
                $(window).resize(resizePage);

                switch (_tabToLoad) {
                    case 'Filters':
                        activeTab = 1;
                        HomeTab_click('Parameters', true); // we need BOTH tabs to be loaded for get data to work from left menu
                        break;
                    case 'Parameters':
                        activeTab = 0;
                        break;
                    default:
                        _tabToLoad = 'Parameters';
                        activeTab = 0;
                        HomeTab_click('Filters', true); // we need BOTH tabs to be loaded for get data to work from left menu
                        break;
                }

                $('#divTabsContainer').tabs({
                    heightStyle: "fill"
                    , collapsible: false
                    , active: activeTab
                });

                activeTab = 0;
                HomeTab_click(_tabToLoad);
			} catch (e) {
				var m = e.message;
			}
        });

        function HomeTab_click(tabName, silent) {
            $('div', $('#divTabsContainer')).hide();

            switch (tabName.toUpperCase()) {
                case 'FILTERS':
                    ShowDimmer(false);
                    var strURL = 'Loading.aspx?Page=FilterPage.aspx?random=' + new Date().getTime()
                        + '&parentModule=RQMT'
                        + '&MyData=true'
                        + '&Source=RQMTParams';

                    if ($('#<%=this.frameFilters.ClientID%>') && $('#<%=this.frameFilters.ClientID%>').attr('src') == "javascript:'';") {
                        $('#<%=this.frameFilters.ClientID%>').attr('src', strURL);
                    }

                    if (silent == null || !silent) {
                        $('#divRQMTFilters').show();
                    }

                    break;
                case 'PARAMETERS':
                    ShowDimmer(false);
                    var strURL = 'Loading.aspx?Page=CrosswalkParametersSections.aspx?GridType=RQMT%20Grid';
                    if ($('#<%=this.frameParams.ClientID%>') && $('#<%=this.frameParams.ClientID%>').attr('src') == "javascript:'';") {
                        $('#<%=this.frameParams.ClientID%>').attr('src', strURL);
                    }

                    if (silent == null || !silent) {
                        $('#divRQMTAdvParams').show();
                    
                        if (typeof $('#frameFilters')[0] != 'undefined' && $.isFunction($('#frameFilters')[0].contentWindow.btnSave_Click)) {
                            $('#frameFilters')[0].contentWindow.btnSave_Click();
                        }
                    }
                    break;
                default:
                    ShowDimmer(false);
                    if ($('#<%=this.frameParams.ClientID%>') && $('#<%=this.frameParams.ClientID%>').attr('src') == "javascript:'';") {
                        $('#<%=this.frameParams.ClientID%>').attr('src', strURL);
                    }
                    $('#divRQMTAdvParams').show();

                    break;
            }
            resizePage();
        }

        function ddlSaveView_change() {
            var $opt = $('#ddlSaveView option:selected');

            if ($opt.text() != '--Create New--') {
                $('#trViewName').hide();
                $('#chkProcessView').prop('checked', $opt.attr('OptionGroup') == 'Process Views');
            }
            else {
                $('#<%=txtViewName.ClientID %>').val('');
                $('#trViewName').show();
                $('#chkProcessView').prop('checked', false);
            }
        }

        function buildSaveView() {
            $('#<%=txtViewName.ClientID %>').val('');
            $('#ddlSaveView').html($('select[id*="ddlView"]', $('#frameParams').contents()).html());
            $('#ddlSaveView').prepend('<option>--Create New--</option>');
            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true).change();
        }

        function buttonGetData_click() {                       
            if (typeof $('#frameFilters')[0] != 'undefined' && $.isFunction($('#frameFilters')[0].contentWindow.btnSave_Click)) {
                $('#frameFilters')[0].contentWindow.btnSave_Click();
            }            

            if (typeof $('#frameParams')[0] != 'undefined' && $.isFunction($('#frameParams')[0].contentWindow.buttonGetData_click)) {
                $('#frameParams')[0].contentWindow.buttonGetData_click();
            }
        }

        function buttonSave_click(obj) {
            buildSaveView();
            $('#divDimmer').show();
            var pos = $(obj).position();

            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true);
            $('#trViewName').show();
            $('#chkProcessView').prop('checked', false);

            $('#divViewName').css({
                position: "absolute",
                top: ($(this).height() / 2) - 31,
                left: ($(this).width() / 2) - 110
            });
            $('#divViewName').show();
        }

        function buttonSaveView_click() {
            if ($('#<%=txtFilterName.ClientID%>').val() !== '' && !($('#<%=txtViewName.ClientID%>').val().trim() === '' && $('#<%=ddlSaveView.ClientID%> option:selected').text() == '--Create New--')) {
                if (typeof $('#frameFilters')[0] != 'undefined' && $.isFunction($('#frameFilters')[0].contentWindow.saveCustomFilters)) {
                    $('#frameFilters')[0].contentWindow.saveCustomFilters($('#<%=txtFilterName.ClientID%>').val());
                    $('input[id*="ddlSavedFilters_Text"]', $('#frameFilters').contents()).val($('#<%=txtFilterName.ClientID%>').val());
                }

                if (typeof $('#frameParams')[0] != 'undefined' && $.isFunction($('#frameParams')[0].contentWindow.buttonSaveView_click)) {
                    $('#frameParams')[0].contentWindow.buttonSaveView_click($('#<%=ddlSaveView.ClientID%> option:selected'), $('#<%=txtViewName.ClientID%>').val().trim(), $('#chkProcessView').is(':checked') ? 1 : 0);
                }
            } else {
                MessageBox('Please enter values for required fields.');
            }
        }

        function resizePage() {
            try {
                var heightModifier = 15;

                resizePageElement('divPage', heightModifier + 2);
                resizePageElement('divTabsContainer', heightModifier + 3);

                if (typeof $('#<%=this.frameFilters.ClientID%>')[0] != 'undefined' && $.isFunction($('#<%=this.frameFilters.ClientID%>')[0].contentWindow.resizePage)) {
                    $('#<%=this.frameFilters.ClientID%>')[0].contentWindow.resizePage();
                }

                if (typeof $('#<%=this.frameParams.ClientID%>')[0] != 'undefined' && $.isFunction($('#<%=this.frameParams.ClientID%>')[0].contentWindow.resizePage)) {
                    $('#<%=this.frameParams.ClientID%>')[0].contentWindow.resizePage();
                }

                resizePageElement('divRQMTFilters', heightModifier + 11);
                resizePageElement('divRQMTAdvParams', heightModifier + 11);

            }
            catch (e) {
                var m = e.message;
            }
        }

        function closeWindow() {
            var popup = popupManager.GetPopupByName('RQMTParameter');
            if (popup != null) popup.Close();

        }
	</script>
</body>
</html>