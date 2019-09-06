<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Release_Assessment_Edit.aspx.cs" Inherits="AOR_Release_Assessment_Edit" MasterPageFile="~/AddEdit.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Release Assessment</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2018 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
		<div id="divReleaseAssessment" style="padding: 10px;">
			<table style="width: 100%;">
				<tr>
					<td style="width: 5px;"><span style="color: red;">*</span></td>
					<td style="width: 115px;">Release Version:</td>
					<td>
						<asp:DropDownList ID="ddlRelease" runat="server" Width="155" Enabled="false"></asp:DropDownList>
					</td>
					<td style="width: 5px;">&nbsp;</td>
				</tr>
				<tr>
					<td><span style="color: red;">*</span></td>
					<td>Contract:</td>
					<td>
						<asp:DropDownList ID="ddlContract" runat="server" Width="155" Enabled="false"></asp:DropDownList>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td style="vertical-align: top;">Review Narrative:</td>
					<td>
						<asp:TextBox ID="txtReviewNarrative" runat="server" TextMode="MultiLine" Rows="4" Width="100%" Enabled="false"></asp:TextBox>
					</td>
					<td>&nbsp;</td>
				</tr>
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Mitigation:
					</td>
					<td>
						<asp:CheckBox ID="chkMitigation" runat="server" Enabled="false" Checked="false" />&nbsp;&nbsp;
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr id="trMitigationNarr" style="display: none;">
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Mitigation Narrative:
					</td>
					<td>
						<asp:TextBox ID="txtMitigationNarrative" runat="server" TextMode="MultiLine" Rows="4" Width="100%" Enabled="false"></asp:TextBox>
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Reviewed:
					</td>
					<td>
						<asp:CheckBox ID="chkReviewed" runat="server" Enabled="false" Checked="false" />&nbsp;&nbsp;
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
            </table>
		</div>
        <div id="divDeploymentsContainer" style="display: none;">
			<div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
				<span id="DeploymentsTitle">Deployments (0)</span>
			</div>
			<div id="divDeployments">
				<table style="width: 100%;">
					<tr>
						<td>
							<div id="divReleaseAssessmentDeployments" runat="server">
                                <iframe id="frameDeployments" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
							</div>
						</td>
					</tr>
				</table>
			</div>
		</div>
    </div>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);
                    PageMethods.Save('<%=this.AssessmentID %>', $('#<%=this.ddlRelease.ClientID %>').val(), $('#<%=this.ddlContract.ClientID %>').val(),
                        $('#<%=this.txtReviewNarrative.ClientID %>').val(), $('#<%=this.chkMitigation.ClientID %>').is(':checked'),
                        $('#<%=this.txtMitigationNarrative.ClientID %>').val(), $('#<%=this.chkReviewed.ClientID %>').is(':checked'), save_done, on_error);
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.' + e);
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var newID = 0;
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
                if (obj.newID != '0') newID = obj.newID;
            }

            if (blnSaved) {
                MessageBox('Release Assessment saved.');
                opener.refreshPage();

                if (newID != "0") window.location.href = 'Loading.aspx?Page=' + window.location.href + '&ReleaseAssessmentID=' + newID;
                else refreshPage();
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            return validation.join('<br>');
        }

        function input_change(obj) {
            var $obj = $(obj);

            if ($obj[0].checked) $('#trMitigationNarr').show();
            else $('#trMitigationNarr').hide();
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function updateTab(tabName, newCount, subtasks) {
            switch (tabName.toUpperCase()) {
                case 'DEPLOYMENTS':
                    $('#DeploymentsTitle').text('Deployments (' + newCount + ')');
                    if (newCount > 0) $('[id$="ddlRelease"]').prop('disabled', true);
                    else $('[id$="ddlRelease"]').prop('disabled', false);
                    break;
            }
        }

        function refreshPage() {
            var nURL = window.location.href;

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeFrame(section) {
            if ($('#frame' + section).contents().find('table')[0]) $('#frame' + section)[0].height = $('#frame' + section).contents().find('table')[0].scrollHeight + $('#frame' + section).contents().find('[id$="Data_Grid"]').height() + $('#frame' + section).contents().find('[id$="PagerContainer"]').height();
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#pageContentHeader').hide();

            if (true) {
                $('#btnSave').prop('disabled', false);
            }

            if ($('[id$="chkMitigation"]')[0].checked) $('#trMitigationNarr').show();
            else $('#trMitigationNarr').hide();

            if ('<%=this.AssessmentID%>' != '0') {
                $('#divDeploymentsContainer').show();
                $('[id$="ddlRelease"]').prop('disabled', true);
            }

            if ($('#frameDeployments').attr('src') == "javascript:'';") $('#frameDeployments').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORReleaseAssessmentDeployment + window.location.search + '&ReleaseID=' + $('#<%=this.ddlRelease.ClientID %>').val());

            resizePage();
        }

        function initEvents() {
            $('[id$="imgRefresh"]').click(function () { imgRefresh_click(); });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('[id$="chkMitigation"]').on('change', function () { input_change(this); });

            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>

