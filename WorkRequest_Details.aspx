<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="WorkRequest_Details.aspx.cs" Inherits="WorkRequest_Details" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Work Request</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<script type="text/javascript" src="Scripts/workload.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
	<img src="images/icons/pencil.png" alt="Review/Edit Work Request" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Work Request [<span id="spanWorkRequestNumber" runat="server"><span style="font-style:italic;">New</span></span>]</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server">&nbsp;</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="buttonCancel" value="Cancel" style="padding: 1px 2px 1px 2px; width: 47px;" />
	<input type="button" id="buttonSave" value="Save" disabled="disabled" style="padding: 1px 2px 1px 2px; width: 42px;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<div id="divPageContainer" class="pageContainer" style="overflow-y: scroll; overflow-x: hidden;">
		<div id="divDetailsContainer" class="attributesRow" style="vertical-align: top;">
			<div id="divDetailsHeader" class="pageContentHeader" style="padding: 5px;">
				<div class="attributesRequired" style="width: 10px; display: inline;">
					<img id="imgHideDetails" class="hideSection" sectionname="Details" alt="Hide Work Item Details" title="Hide Work Item Details" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
					<img id="imgShowDetails" class="showSection" sectionname="Details" alt="Show Work Item Details" title="Show Work Item Details" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
				</div>
				<div class="attributesLabel" style="padding-left: 5px; display: inline;">Work Request Details:</div>
			</div>
			<div id="divDetails" class="attributesValue" style="padding: 10px 0px 10px 20px;">
				<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding-top: 3px;">
					<tr>
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trRequestGroup" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Request Group:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlRequestGroup" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trWorkRequest" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Title:</td>
									<td class="attributesValue">
										<asp:TextBox ID="txtTitle" runat="server" Width="98%" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr id="trAuditing">
						<td id="tdLeftColumn1" style="width: 45%; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trCreated" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">&nbsp;</td>
									<td class="attributesValue">Created:&nbsp;
										<asp:Label ID="labelCreated" runat="server" />
									</td>
								</tr>
							</table>
						</td>
						<td id="tdRightColumn1" style="vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
								<tr id="trUpdated" class="attributesRow">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">&nbsp;</td>
									<td class="attributesValue" style="text-align: right; padding-right: 3px;">Updated:&nbsp;
										<asp:Label ID="labelUpdated" runat="server" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="2" style="text-align: left; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trContract" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Contract:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlContract" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trOrganization" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Organization:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlOrganization" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trDescription" class="attributesRow">
									<td class="attributesRequired" style="vertical-align: top;">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel" style="vertical-align:top;">Description:</td>
									<td class="attributesValue" style="vertical-align: top;">
										<textarea id="textAreaDescription" runat="server" rows="5" style="width: 98%;" maxlength="500"></textarea>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td id="tdLeftCol" style="width: 45%; vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left;">
								<tr id="trRequestType" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Request Type:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlRequestType" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trScope" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Scope:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlScope" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trEffort" class="attributesRow" style="display:none;">
									<td class="attributesRequired">&nbsp;</td>
									<td name="LeftColumn" class="attributesLabel">Effort:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlEffort" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trPriority" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td name="LeftColumn" class="attributesLabel">Operations Priority:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlPriority" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
						<td id="tdRightCol" style="vertical-align: top;">
							<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top;">
								<tr id="trSubmittedBy" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">Submitted By:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlSubmittedBy" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trSME" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">SME:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlSME" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trLead_IA_TW" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">IA/LTW:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlLead_IA_TW" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trLeadResource" class="attributesRow">
									<td class="attributesRequired">*</td>
									<td class="attributesLabel">Lead Resource:</td>
									<td class="attributesValue">
										<asp:DropDownList ID="ddlLeadResource" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
											<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
										</asp:DropDownList>
									</td>
								</tr>
								<tr id="trSpacer" class="attributesRow" style="display: none;">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">&nbsp;</td>
									<td class="attributesValue">&nbsp;</td>
								</tr>
								<tr id="trSpacer1" class="attributesRow" style="display: none;">
									<td class="attributesRequired">&nbsp;</td>
									<td class="attributesLabel">&nbsp;</td>
									<td class="attributesValue">&nbsp;</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>
			<div id="divJustificationContainer" class="attributesRow" style="vertical-align: top;">
				<div id="divJustificationHeader" class="pageContentHeader" style="padding: 5px;">
					<div class="attributesRequired" style="width: 10px; display: inline;">
						<img id="imgHideJustification" class="hideSection" sectionname="Justification" alt="Hide Work Item Justification" title="Hide Work Item Justification" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
						<img id="imgShowJustification" class="showSection" sectionname="Justification" alt="Show Work Item Justification" title="Show Work Item Justification" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
					</div>
					<div class="attributesLabel" style="padding-left: 5px; display: inline;">Justification:</div>
				</div>
				<div id="divJustification" class="attributesValue" style="padding: 10px 0px 10px 20px;">
					<textarea id="textAreaJustification" runat="server" rows="8" style="width: 98%;" maxlength="1000"></textarea>
				</div>
			</div>
			<div id="divSRContainer" class="attributesRow" style="vertical-align: top;">
				<div id="divSRsHeader" class="pageContentHeader" style="padding: 5px;">
					<div class="attributesRequired" style="width: 10px; display: inline;">
						<img id="imgHideSRs" class="hideSection" sectionname="SRs" alt="Hide Work Item SRs" title="Hide Work Item SRs" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
						<img id="imgShowSRs" class="showSection" sectionname="SRs" alt="Show Work Item SRs" title="Show Work Item SRs" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
					</div>
					<div class="attributesLabel" style="padding-left: 5px; display: inline;">SRs:</div>
				</div>
				<div id="divSRs" class="attributesValue" style="padding: 10px 0px 10px 20px;">
					<iframe id="frameSRs" src="javascript:'';" frameborder="0" scrolling="no" style="display: block; width: 100%;"></iframe>
				</div>
			</div>
		</div>
	</div>

	<script id="jsVariables" type="text/javascript">
		var _canEdit = false;
		var _pageUrls;
		var _id = 0;
	</script>

	<script id="jsAJAX" type="text/javascript">

		function buildWorkRequestObject() {
			var wr = new WorkRequest();

			wr.WORKREQUESTID = _id;
			wr.REQUESTGROUPID = parseInt($('#<%=this.ddlRequestGroup.ClientID %> option:selected').val());
			wr.Title = $('#<%=this.txtTitle.ClientID %>').val();
			wr.Description = $('#<%=this.textAreaDescription.ClientID %>').val();
			wr.Justification = $('#<%=this.textAreaJustification.ClientID %>').val();
			wr.REQUESTTYPEID = parseInt($('#<%=this.ddlRequestType.ClientID %> option:selected').val());
			
			wr.CONTRACTID = parseInt($('#<%=this.ddlRequestType.ClientID %> option:selected').val());
			wr.ORGANIZATIONID = parseInt($('#<%=this.ddlOrganization.ClientID %> option:selected').val());
			wr.WTS_SCOPEID = parseInt($('#<%=this.ddlScope.ClientID %> option:selected').val());
			wr.EFFORTID = parseInt($('#<%=this.ddlEffort.ClientID %> option:selected').val());
			wr.SubmittedByID = parseInt($('#<%=this.ddlSubmittedBy.ClientID %> option:selected').val());
			wr.SMEID = parseInt($('#<%=this.ddlSME.ClientID %> option:selected').val());
			wr.LEAD_IA_TWID = parseInt($('#<%=this.ddlLead_IA_TW.ClientID %> option:selected').val());
			wr.LEAD_RESOURCEID = parseInt($('#<%=this.ddlLeadResource.ClientID %> option:selected').val());
			wr.OP_PRIORITYID = parseInt($('#<%=this.ddlRequestType.ClientID %> option:selected').val());
			wr.Archive = false;

			return wr;
		}

		function buildWorkRequest2() {
			var wr = {
				WORKREQUESTID: _id
				, Title: $('#<%=this.txtTitle.ClientID %>').val()
				, Description: $('#<%=this.textAreaDescription.ClientID %>').val()
				, Justification: $('#<%=this.textAreaJustification.ClientID %>').val()
				, REQUESTTYPEID: parseInt($('#<%=this.ddlRequestType.ClientID %> option:selected').val())
				, REQUESTGROUPID: parseInt($('#<%=this.ddlRequestGroup.ClientID %> option:selected').val())
				, CONTRACTID: parseInt($('#<%=this.ddlContract.ClientID %> option:selected').val())
				, ORGANIZATIONID: parseInt($('#<%=this.ddlOrganization.ClientID %> option:selected').val())
				, WTS_SCOPEID: parseInt($('#<%=this.ddlScope.ClientID %> option:selected').val())
				, EFFORTID: parseInt($('#<%=this.ddlEffort.ClientID %> option:selected').val())
				, SubmittedByID: parseInt($('#<%=this.ddlSubmittedBy.ClientID %> option:selected').val())
				, SMEID: parseInt($('#<%=this.ddlSME.ClientID %> option:selected').val())
				, LEAD_IA_TWID: parseInt($('#<%=this.ddlLead_IA_TW.ClientID %> option:selected').val())
				, LEAD_RESOURCEID: parseInt($('#<%=this.ddlLeadResource.ClientID %> option:selected').val())
				, OP_PRIORITYID: parseInt($('#<%=this.ddlPriority.ClientID %> option:selected').val())
				, Archive: false
			};

			return wr;
		}

		function save() {
			//var workRequest = buildWorkRequestObject();
			var workRequest = buildWorkRequest2();

			try {
				PageMethods.SaveWorkRequest(
					workRequest, save_done, on_error);
			} catch (e) {
				MessageBox('An error occurred gathering data to save.' + '\n' + e.message);
			}
		}
		function save_done(result) {
			var saved = false;
			var id = 0;
			var errorMsg = '';

			try {
				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
						saved = true;
					}
					if (obj.id) {
						id = obj.id;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				if (saved) {
					MessageBox('Item updates have been saved.');
					if (parent) {
						if (parent.opener && parent.opener.refreshPage) {
							parent.opener.refreshPage();
						}
						else if (parent.parent && parent.parent.refreshPage) {
							parent.parent.refreshPage();
						}
						else {
							refreshPage(id);
						}
					}
				}
				else {
					MessageBox('Failed to save Work Request. \n' + errorMsg);
				}
			}
			catch (e) {

			}
		}

		function on_error(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

	</script>

	<script id="jsEvents" type="text/javascript">

		function refreshPage(newID) {
			var url = document.location.href;
			if (newID > 0) {
				url = editQueryStringValue(url, 'workRequestID', newID);
			}

			document.location.href = 'Loading.aspx?Page=' + url;
		}

		function resizePage() {
			resizePageElement('divPageContainer');

			resizeFrames();
		}

		function resizeFrames() {
			var frame;
			var fPageHeight = 0;

			$('iframe').each(function () {
				frame = $(this)[0];
				fPageHeight = 0;

				if (frame.contentWindow
					&& frame.contentWindow.document
					&& frame.contentWindow.document.body
					&& frame.contentWindow.document.body.offsetHeight) {
					fPageHeight = frame.contentWindow.document.body.offsetHeight;
				}
				frame.style.height = fPageHeight + 'px';
			});
		}

		function showHideSection_click(imgId, show, sectionName) {
			if (show) {
				$('#div' + sectionName).show();
				$('#' + imgId).hide();
				$('#' + imgId.replace('Show', 'Hide')).show();
				$('tr[section="' + sectionName + '"]').show();

				switch (sectionName) {
					case "WorkItems":
						if ($('#frameWorkItems').attr('src') == "javascript:'';") {
							loadWorkItems();
						}
						break;
				}
			}
			else {
				$('#div' + sectionName).hide();
				$('#' + imgId).hide();
				$('#' + imgId.replace('Hide', 'Show')).show();
				$('tr[section="' + sectionName + '"]').hide();
			}

			resizeFrames();
		}

		function activateSaveButton() {
			if (_canEdit) {
				$('#buttonSave').removeAttr('disabled');
			}
		}

		function ddl_change(sender) {
			activateSaveButton();

		}

		function txt_change(sender) {
			var original_value = '', new_value = '';
			if ($(sender).attr('original_value')) {
				original_value = $(sender).attr('original_value');
			}

			new_value = $(sender).val();

			if (new_value != original_value) {
				activateSaveButton();
			}
		}

		function buttonSave_click() {
			var msg = validateFields();

			if (msg.length > 0) {
				MessageBox('Please provide values for the following fields: \n' + msg);
			}
			else {
				save();
			}
		}

		function validateFields() {
			var emptyFields = [];
			var msg = '';

			if ($('#<%=this.txtTitle.ClientID %>').val() == '0') {
				emptyFields.push('Contract');
			}
			if ($('#<%=this.textAreaDescription.ClientID %>').val() == '0') {
				emptyFields.push('Contract');
			}

			if ($('#<%=this.ddlContract.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Contract');
			}
			if ($('#<%=this.ddlOrganization.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Organization');
			}
			if ($('#<%=this.ddlRequestType.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Request Type');
			}
			if ($('#<%=this.ddlRequestGroup.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Request Group');
			}
			if ($('#<%=this.ddlScope.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Scope');
			}
			if ($('#<%=this.ddlPriority.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Operations Priority');
			}
			if ($('#<%=this.ddlSME.ClientID %> option:selected').val() == '0') {
				emptyFields.push('SME');
			}
			if ($('#<%=this.ddlLead_IA_TW.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Lead IA / Lead TW');
			}
			if ($('#<%=this.ddlLeadResource.ClientID %> option:selected').val() == '0') {
				emptyFields.push('Lead Resource');
			}

			msg = emptyFields.join('\n');

			return msg;
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initializeSections() {
			try {
				if (_canEdit) {
					
				}
				//loadWorkItems();
			} catch (e) {

			}
		}
		
		function initializeEvents() {

			$(window).resize(resizePage);
			$('select').change(function () { ddl_change(this); });
			$('input:text').bind('change', function () { txt_change(this); });

			$('#imgRefresh').click(function () { refreshPage(); });
			$('#buttonSave').click(function () { buttonSave_click(); return false; });
			$('#buttonCancel').click(function () { refreshPage(); return false; });

			$('.hideSection').click(function (event) {
				showHideSection_click($(this).attr('id'), false, $(this).attr('sectionName'));
			});
			$('.showSection').click(function (event) {
				showHideSection_click($(this).attr('id'), true, $(this).attr('sectionName'));
			});
		}

		$(document).ready(function () {
			_pageUrls = new PageURLs();
			_id = +'<%=this.WorkRequestID%>';
			if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
				_canEdit = true;
			}

			activateSaveButton();

			initializeEvents();
			initializeSections();

			$('.pageContainer').css('background-color', '#FAFAFA');
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			$('.attributesLabel').css('width', 85);
			$('.attributesLabel[name="LeftColumn"]').css('width', 105);

			resizePage();
		});

	</script>
</asp:Content>