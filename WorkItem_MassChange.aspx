﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.master" AutoEventWireup="true" CodeFile="WorkItem_MassChange.aspx.cs" Inherits="WorkItem_MassChange" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Mass Change Work Items</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPage" class="pageContainer">
		<div id="divHeader" class="pageContentHeader" style="font-size: 13px;">
			<table cellpadding="0" cellspacing="0" style="width: 100%;">
				<tr>
					<td style="text-align: left; padding-left: 4px; font-size: 14px; height: 30px;">Mass Change Primary Tasks</td>
					<td>&nbsp;</td>
					<td style="text-align: right; padding-right: 0px; width: 95px;">
						<button id="buttonBackToGrid" runat="server" value="Back to Grid" style="padding: 1px 2px 1px 2px; width: 88px; float: left; display: none;">Back to Grid</button>
					</td>
				</tr>
			</table>
		</div>
		<div id="divMassChangeHeaderButtons" class="pageContentInfo" style="height: auto; padding: 2px; border-bottom: 1px solid gainsboro;">
			<table cellpadding="0" cellspacing="0" style="width: 100%; text-align: left;">
				<tr>
					<td>
						<table cellpadding="0" cellspacing="0" style="white-space: nowrap;">
							<tr>
								<td style="text-align: left; padding-top: 2px;">
									<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="images/icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
		<div id="divOptionsContainer" class="mainPageContainer" style="padding-top:5px;">
			<table id="tableMassChangeOptions" class="attributes">
				<tr id="trAttribute" class="attributesRow">
					<td class="attributesLabel">Field to Change:</td>
					<td class="attributesValue">
						<asp:DropDownList ID="ddlAttribute" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false">
							<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trFromValue" class="attributesRow">
					<td class="attributesLabel">Current Value:</td>
					<td class="attributesValue">
						<asp:DropDownList ID="ddlFromValue" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
							<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr id="trToValue" class="attributesRow">
					<td class="attributesLabel">New Value:</td>
					<td class="attributesValue">
						<asp:DropDownList ID="ddlToValue" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="true">
							<asp:ListItem Text="-Select-" Value="0"></asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
			</table>
		</div>
	</div>

	<div id="divPageFooter" class="PopupFooter">
		<table cellpadding="0" cellspacing="0" style="width: 100%; text-align:right;">
			<tr>
				<td>&nbsp;</td>
				<td>
					<input type="button" id="buttonSave" value="Apply" />
				</td>
				<td>
					<input type="button" id="buttonClose" value="Close" />
				</td>
			</tr>
		</table>
	</div>

	<div id="divPageDimmer" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: grey; filter: alpha(opacity=60); opacity: .60; display: none;"></div>
	<div id="divSaving" style="position: absolute; left: 35%; top: 15%; padding: 10px; background: white; border: 1px solid grey; font-size: 18px; text-align: center; display: none;">
		<table>
			<tr>
				<td>WTS is Saving Data... Please wait...</td>
			</tr>
			<tr>
				<td>
					<img alt='' src="Images/loaders/progress_bar_blue.gif" /></td>
			</tr>
		</table>
	</div>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">
		var _canEdit = false;
		var _includeArchive = false;
		var _myData = true;
		var _selectedField = '';
	</script>

	<script id="jsAJAX" type="text/javascript">

		function LoadValueOptions(idField, columnName, textField) {
			try {
				if (idField.length == 0 || textField.length == 0) {
					MessageBox('Unable to gather Current Value list. Attribute selection is invalid.');
					return;
				}

				ShowDimmer(true, "Loading Available Values for " + textField + "...", 1);

				PageMethods.GetFieldValues(idField, columnName, textField, _includeArchive, _myData, LoadValueOptions_done, on_error);
			} catch (e) {
				ShowDimmer(false);
				MessageBox('There was an error gathering Current Value list.\n' + e.message);
			}
		}
		function LoadValueOptions_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);

				var loaded = false;
				var currentCount = 0, newCount = 0;
				var errorMsg = '';
				var dtCurrentOptions = {}, dtNewOptions = {};

				if (obj) {
					if (obj.loaded && obj.loaded.toUpperCase() == 'TRUE') {
						loaded = true;
					}
					if (obj.CurrentCount) {
						currentCount = parseInt(obj.CurrentCount);
					}
					if (obj.NewCount) {
						newCount = parseInt(obj.NewCount);
					}
					if (obj.error) {
						errorMsg = obj.error;
					}

					if (currentCount > 0 && obj.CurrentOptions) {
						dtCurrentOptions = jQuery.parseJSON(obj.CurrentOptions);
					}
					if (newCount > 0 && obj.NewOptions) {
						dtNewOptions = jQuery.parseJSON(obj.NewOptions);
					}

					populateOptions(dtCurrentOptions, dtNewOptions);
				}

			} catch (e) {
				ShowDimmer(false);
			}
		}

		function save() {
			var fieldName = '', fromValue = '', toValue = '';

			try {
				var opt = $('#<%=this.ddlAttribute.ClientID %> option:selected');
				var columnName = $(opt).val();
				var textField = $(opt).text();
				var idField = $(opt).attr('id_field');

				ShowDimmer(true, "Saving Values for " + textField + "...", 1);

				fromValue = $('#<%=this.ddlFromValue.ClientID %>').val();
				if (fromValue == undefined) {
					fromValue = '';
				}
				toValue = $('#<%=this.ddlToValue.ClientID %>').val();
				if (toValue == undefined) {
					toValue = '';
				}

				PageMethods.SaveChanges(columnName, fromValue, toValue, _includeArchive, _myData, save_done, on_error);
			} catch (e) {
				MessageBox('There was an error gathering data to save.');
				ShowDimmer(false);
			}
		}
		function save_done(result) {
			try {
				ShowDimmer(false);

				var obj = jQuery.parseJSON(result);

				var saved = false;
				var count = 0;
				var errorMsg = '';

				if (obj) {
					if (obj.saved) {
						saved = (obj.saved.toUpperCase() == 'TRUE');
					}
					if (obj.Count) {
						count = parseInt(obj.Count);
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				var msg = '';
				if (saved) {
					msg = 'Updated ' + count + ' Primary Tasks.';
				}
				else {
					msg = 'Failed to update Primary Tasks.';
				}

				if (errorMsg.length > 0) {
					if (msg.length > 0) {
						msg += '\n';
					}
					msg += errorMsg;
				}

				MessageBox(msg);

				if (opener && opener.refreshPage) {
					opener.refreshPage(true);
				}
				refreshPage();
			} catch (e) {
				ShowDimmer(false);
			}
		}

		function on_error(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText, 'Server Error');
		}

	</script>

	<script id="jsData" type="text/javascript">

		function setSelectedAttribute(selectedField) {
			
			$('#<%=this.ddlAttribute.ClientID %> option').each(function () {
				if ($(this).text().toUpperCase() == selectedField.toUpperCase()) {
					$('#<%=this.ddlAttribute.ClientID %>').val($(this).val());
					$('#<%=this.ddlAttribute.ClientID %>').trigger('change');
					return false;
				}
			});
		}

		function populateOptions(dtCurrentOptions, dtNewOptions) {
			$('#<%=this.ddlFromValue.ClientID %>').empty().append('<option value="0">-Select-</option>');
			$('#<%=this.ddlToValue.ClientID %>').empty().append('<option value="0">-Select-</option>');

			var value = '', text = '';
			$.each(dtCurrentOptions, function (index, val) {
				value = '';
				text = '';

				if (val.valueField) {
					value = val.valueField;
				}
				if (val.textField) {
					text = val.textField;
				}

				$('#<%=this.ddlFromValue.ClientID %>').append('<option value="' + value + '">' + text + '</option>');
			});

			$.each(dtNewOptions, function (index, val) {
				value = '';
				text = '';

				if (val.valueField) {
					value = val.valueField;
				}
				if (val.textField) {
					text = val.textField;
				}

				$('#<%=this.ddlToValue.ClientID %>').append('<option value="' + value + '">' + text + '</option>');
			});
		}

	</script>

	<script id="jsEvents" type="text/javascript">

		function resizePage() {
			try {
				var heightModifier = 0;
				heightModifier += 10; //$('#mainPageFooter').height();

				resizePageElement('divPage', heightModifier + 2);
				
			}
			catch (e) {
				var m = e.message;
			}
		}

		function refreshPage() {
			var url = document.location.href;

			document.location.href = 'Loading.aspx?Page=' + url;
		}

		function continueClose() {
			return true;
		}
		function buttonClose_click() {
			var contin = continueClose();
			if (!contin) {
				return;
			}

			if (parent.ShowFrame) {
				parent.ShowFrame('Grid');
			}
			else if (closeWindow) {
				closeWindow();
			}
		}

		function buttonSave_click() {
			try {
				if (top && top.setFilterSession) {
					top.setFilterSession(false, false);
				}

				if (_canEdit) {
					if ($('#<%=this.ddlAttribute.ClientID %>').val() == ''
						|| $('#<%=this.ddlAttribute.ClientID %>').val() == '0') {
						MessageBox('You must specifiy a field to change.');
						return;
					}
					else if ($('#<%=this.ddlToValue.ClientID %>').val() == '0') {
						MessageBox('You must specifiy a New Value.');
						return;
					}
					else {
						save();
					}
				}
				else {
					MessageBox('You do not have permission to update these Primary Tasks.');
					return;
				}
			} catch (e) {

			}
		}

		function ddlAttribute_change() {
			var opt = $('#<%=this.ddlAttribute.ClientID %> option:selected');
			var columnName = $(opt).val();
			var textField = $(opt).text();
			var idField = $(opt).attr('id_field');

			$('#<%=this.ddlFromValue.ClientID %>').empty().append('<option value="0">-Select-</option>');
			$('#<%=this.ddlToValue.ClientID %>').empty().append('<option value="0">-Select-</option>');

			LoadValueOptions(idField, columnName, textField);
		}

	</script>

	<script id="jsInit" type="text/javascript">

		$(document).ready(function () {
			try {
				_canEdit = ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE');
				_includeArchive = ('<%=this.IncludeArchive.ToString().ToUpper() %>' == 'TRUE');
				_myData = ('<%=this.MyData.ToString().ToUpper() %>' == 'TRUE');
				_selectedField = '<%=this.SelectedField %>';

				$(window).resize(resizePage);

				$('#imgRefresh').click(function () { refreshPage(); return false; });
				$('#<%=this.buttonBackToGrid.ClientID %>').click(function () { buttonClose_click(); return false; });
				$('#buttonClose').click(function () { buttonClose_click(); return false; });
				$('#buttonSave').click(function () { buttonSave_click(); return false; });
				
				$('#<%=this.ddlAttribute.ClientID %>').change(function () { ddlAttribute_change(); return false; });

				if (_selectedField.length > 0) {
					setSelectedAttribute(_selectedField);
				}

				resizePage();
			} catch (e) {
				var m = e.message;
			}
		});

	</script>
</asp:Content>