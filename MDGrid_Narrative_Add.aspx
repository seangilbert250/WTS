<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_Narrative_Add.aspx.cs" Inherits="MDGrid_Narrative_Add" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Add CR Report Narrative</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server"></asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server"></asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
        <table style="width: 98%;">
        <tr>
	        <td>
                <input type="button" id="btnSave" value="Add" style="vertical-align:middle;" />
	        </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divMain" style="padding: 10px;">
        <table style="width: 100%;">
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 155px;">
                    Product Version:
                </td>
                <td id="releaseElement">
                    <asp:DropDownList ID="ddlRelease" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td style="width: 5px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 135px;">
                    Contract:
                </td>
                <td>
                    <asp:DropDownList ID="ddlContract" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td style="width: 5px;">
                    &nbsp;
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSubmit" runat="server" style="display: none;" />
    </div>
    <div id="divMissionContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
        <div id="divMissionHeader" class="pageContentHeader" style="padding: 5px;">
			<div class="attributesRequired" style="width: 10px; display: inline;">
				<img id="imgHideMission" class="hideSection" sectionname="Mission" alt="Hide Mission" title="Hide Mission" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
				<img id="imgShowMission" class="showSection" sectionname="Mission" alt="Show Mission" title="Show Mission" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
			</div>
			<div class="attributesLabel" style="padding-left: 5px; display: inline;">Workload Allocation Type: Mission</div>
		</div>
        <div id="divMission" style="padding: 10px;">
			<table id="tableMission" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
				<tr>
					<td colspan="2" style="text-align: left; vertical-align: top;">
						<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                            <tr>
                                <td>
                                    <span style="color: red;">*</span>
                                </td>
                                <td style="width: 155px;">
                                    CR Report Narrative:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMission" TextMode="Multiline" Rows="7" runat="server" Width="244px"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5px;">
                                    <span style="color: red;">&nbsp;</span>
                                </td>
                                <td style="width: 135px;">
                                    Image:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMissionImage" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                                </td>
                                <td style="width: 5px;">
                                    &nbsp;
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
        </div>
    </div>
    <div id="divProgramMGMTContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
        <div id="divProgramMGMTHeader" class="pageContentHeader" style="padding: 5px;">
			<div class="attributesRequired" style="width: 10px; display: inline;">
				<img id="imgHideProgramMGMT" class="hideSection" sectionname="ProgramMGMT" alt="Hide Program MGMT" title="Hide Program MGMT" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				<img id="imgShowProgramMGMT" class="showSection" sectionname="ProgramMGMT" alt="Show Program MGMT" title="Show Program MGMT" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
			</div>
			<div class="attributesLabel" style="padding-left: 5px; display: inline;">Workload Allocation Type: Program MGMT</div>
		</div>
        <div id="divProgramMGMT" style="padding: 10px; display: none;">
			<table id="tableProgramMGMT" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
				<tr>
					<td colspan="2" style="text-align: left; vertical-align: top;">
						<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                            <tr>
                                <td>
                                    <span style="color: red;">*</span>
                                </td>
                                <td style="width: 155px;">
                                    CR Report Narrative:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProgramMGMT" TextMode="Multiline" Rows="7" runat="server" Width="244px"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5px;">
                                    <span style="color: red;">&nbsp;</span>
                                </td>
                                <td style="width: 135px;">
                                    Image:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlProgramMGMTImage" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                                </td>
                                <td style="width: 5px;">
                                    &nbsp;
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
        </div>
    </div>
    <div id="divDeploymentContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
        <div id="divDeploymentHeader" class="pageContentHeader" style="padding: 5px;">
			<div class="attributesRequired" style="width: 10px; display: inline;">
				<img id="imgHideDeployment" class="hideSection" sectionname="Deployment" alt="Hide Deployment" title="Hide Deployment" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				<img id="imgShowDeployment" class="showSection" sectionname="Deployment" alt="Show Deployment" title="Show Deployment" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
			</div>
			<div class="attributesLabel" style="padding-left: 5px; display: inline;">Workload Allocation Type: Deployment</div>
		</div>
        <div id="divDeployment" style="padding: 10px; display: none;">
			<table id="tableDeployment" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
				<tr>
					<td colspan="2" style="text-align: left; vertical-align: top;">
						<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                            <tr>
                                <td>
                                    <span style="color: red;">*</span>
                                </td>
                                <td style="width: 155px;">
                                    CR Report Narrative:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtDeployment" TextMode="Multiline" Rows="7" runat="server" Width="244px"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5px;">
                                    <span style="color: red;">&nbsp;</span>
                                </td>
                                <td style="width: 135px;">
                                    Image:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlDeploymentImage" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                                </td>
                                <td style="width: 5px;">
                                    &nbsp;
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
        </div>
    </div>
    <div id="divProductionContainer" class="attributesRow" style="vertical-align: top; overflow-x: hidden;">
        <div id="divProductionHeader" class="pageContentHeader" style="padding: 5px;">
			<div class="attributesRequired" style="width: 10px; display: inline;">
				<img id="imgHideProduction" class="hideSection" sectionname="Production" alt="Hide Production" title="Hide Production" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				<img id="imgShowProduction" class="showSection" sectionname="Production" alt="Show Production" title="Show Production" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer;" />
			</div>
			<div class="attributesLabel" style="padding-left: 5px; display: inline;">Workload Allocation Type: Production</div>
		</div>
        <div id="divProduction" style="padding: 10px; display: none;">
			<table id="tableProduction" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
				<tr>
					<td colspan="2" style="text-align: left; vertical-align: top;">
						<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
                            <tr>
                                <td>
                                    <span style="color: red;">*</span>
                                </td>
                                <td style="width: 155px;">
                                    CR Report Narrative:
                                </td>
                                <td>
                                    <asp:TextBox ID="txtProduction" TextMode="Multiline" Rows="7" runat="server" Width="244px"></asp:TextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5px;">
                                    <span style="color: red;">&nbsp;</span>
                                </td>
                                <td style="width: 135px;">
                                    Image:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlProductionImage" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                                </td>
                                <td style="width: 5px;">
                                    &nbsp;
                                </td>
                            </tr>
						</table>
					</td>
				</tr>
            </table>
        </div>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _selectedHistoryID = 0;
    </script>

    <script type="text/javascript">
        function refreshPage() {
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'RefData', 1);

            document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function btnSave_click() {
            try {
                var validation = validate();
                var blnAltered = 0;

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    PageMethods.Save($('#<%=this.ddlRelease.ClientID %>').val(), $('#<%=this.ddlContract.ClientID %>').val(),
                        <%=this.missionNarrativeID%>, $('#<%=this.txtMission.ClientID %>').val(), $('#<%=this.ddlMissionImage.ClientID %>').val(),
                        <%=this.programMGMTNarrativeID%>, $('#<%=this.txtProgramMGMT.ClientID %>').val(), $('#<%=this.ddlProgramMGMTImage.ClientID %>').val(),
                        <%=this.deploymentNarrativeID%>, $('#<%=this.txtDeployment.ClientID %>').val(), $('#<%=this.ddlDeploymentImage.ClientID %>').val(),
                        <%=this.productionNarrativeID%>, $('#<%=this.txtProduction.ClientID %>').val(), $('#<%=this.ddlProductionImage.ClientID %>').val(),
                        save_done, on_error);
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            try {
                ShowDimmer(false);

                var saved = false;
                var ids = '', errorMsg = '';

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.ids) {
                        ids = obj.ids;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('Items have been saved.');
                    if (opener.refreshPage) opener.refreshPage();
                    setTimeout(closeWindow, 1);
                }
                else {
                    MessageBox('Failed to save items. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            if ($('#<%=this.ddlRelease.ClientID %>').val() == 0) validation.push('Product Version cannot be empty.');
            if ($('#<%=this.txtMission.ClientID %>').val().length == 0
                && $('#<%=this.txtProgramMGMT.ClientID %>').val().length == 0
                && $('#<%=this.txtDeployment.ClientID %>').val().length == 0
                && $('#<%=this.txtProduction.ClientID %>').val().length == 0) validation.push('Please provide at least one CR Report Narrative.');
            if ($('#<%=this.ddlContract.ClientID %>').val() == 0) validation.push('Contract cannot be empty.');

            return validation.join('<br>');
        }

        function showHideSection_click(imgId, show, sectionName) {
            if (show) {
                $('#div' + sectionName).show();
                $('#table' + sectionName).show();
                $('#' + imgId).hide();
                $('#' + imgId.replace('Show', 'Hide')).show();
                $('tr[section="' + sectionName + '"]').show();
            }
            else {
                $('#div' + sectionName).hide();
                $('#table' + sectionName).hide();
                $('#' + imgId).hide();
                $('#' + imgId.replace('Hide', 'Show')).show();
                $('tr[section="' + sectionName + '"]').hide();
            }

            resizePage();
        }

        function resizePage() {
            var heightModifier = 0;

            if ($(popupManager.GetPopupByName('CRReportNarrative')).length > 0) {
                $(popupManager.GetPopupByName('CRReportNarrative'))[0].Frame.style.height = $('#divPage').height() + 'px';
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
            if ($(popupManager.GetPopupByName('CRReportNarrative')).length > 0) {
                $(popupManager.GetPopupByName('CRReportNarrative'))[0].Frame.parentElement.style.overflow = "auto";
            }
        }

        function initEvents() {
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#btnSave').click(function () { btnSave_click(); return false; });

            $('.hideSection').click(function (event) {
                showHideSection_click($(this).attr('id'), false, $(this).attr('sectionName'));
            });
            $('.showSection').click(function (event) {
                showHideSection_click($(this).attr('id'), true, $(this).attr('sectionName'));
            });
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();

            resizePage();
        });
	</script>
</asp:Content>