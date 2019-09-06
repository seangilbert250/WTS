<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="Hotlist_Grid.aspx.cs" Inherits="Hotlist_Grid" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Hotlist</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Request Groups (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr id="trQuickFilters">
			<td style="padding-left: 10px; display:none;">View:
				<asp:DropDownList ID="ddlView" runat="server" TabIndex="1" AppendDataBoundItems="true">
					<asp:ListItem Text="-Select-" Value="0" />
				</asp:DropDownList>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				Show Archived<input type="checkbox" id="chkShowArchived" style="vertical-align:middle;" />
				&nbsp;
				<input type="button" id="buttonNewGroup" value="Add Request Group" disabled="disabled" />
				<input type="button" id="buttonNewRequest" value="Add Work Request" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save Group(s)" disabled="disabled" />
			</td>
			<td style="padding: 0px; margin: 0px;">
				<iti_Tools_Sharp:Menu ID="menuReports" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related Items <img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="padding: 0px; margin: 0px; display: none; float: left;"></iti_Tools_Sharp:Menu>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdHotlist" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">
		var _pageUrls;
		var _idxId = 0, _selectedId = 0, _myView = 0;
		var _canViewGroup = true, _canEditGroup = false;
		var _canViewRequest = true, _canEditRequest = false;
		
	</script>

	<script id="jsAJAX" type="text/javascript">

		function GetColumnValue(row, ordinal, blnoriginal_value) {
			try {
				var tdval = $(row).find('td:eq(' + ordinal + ')');
				var val = '';
				if ($(tdval).length == 0) { return ''; }

				if ($(tdval).children.length > 0) {
					if ($(tdval).find("select").length > 0) {
						if (blnoriginal_value) {
							val = $(tdval).find("select").attr('original_value');
						}
						else {
							val = $(tdval).find("select option:selected").val();
						}
					}
					else if ($(tdval).find('input[type=checkbox]').length > 0) {
						if (blnoriginal_value) {
							val = $(tdval).find('input[type=checkbox]').parent().attr("original_value");
						}
						else {
							if ($(tdval).find('input[type=checkbox]').prop('checked')) { val = '1'; }
							else { val = '0'; }
						}
					}
					else if ($(tdval).find('input[type=text]').length > 0) {
						if (blnoriginal_value) {
							val = $(tdval).find('input[type=text]').attr('original_value');

						}
						else {
							val = $(tdval).find('input[type=text]').val();
						}
					}
					else if ($(tdval).find('input[type=number]').length > 0) {
						if (blnoriginal_value) {
							val = $(tdval).find('input[type=number]').attr('original_value');

						}
						else {
							val = $(tdval).find('input[type=number]').val();
						}
					}
					else if ($(tdval).find('input').length > 0) {
						if (blnoriginal_value) {
							val = $(tdval).find('input').attr('original_value');

						}
						else {
							val = $(tdval).find('input').val();
						}
					}
					else {
						val = $(tdval).text();
					}

				}
				else {
					val = $(tdval).text();
				}
				return val;
			} catch (e) { return ''; }
		}

		function save() {
			try {
				ShowDimmer(true, "Updating...", 1);

				var changedRows = [];
				var id = 0;
				var original_value = '', name = '', description = '', sortOrder = '', archive = '';

				$('.gridBody, .selectedRow', $('#<%=this.grdHotlist.ClientID%>_Grid')).each(function (i, row) {
					var changedRow = [];
					var changed = false;

					if (!$(row).attr('itemID')
						|| $(row).attr('itemID') == ''
						|| $(row).attr('itemID') == 0) {
						return true;
					}

					if (_dcc[0].length > 0) {
						for (var i = 0; i <= _dcc[0].length - 1; i++) {
							var newval = GetColumnValue(row, i);
							var oldval = GetColumnValue(row, i, true);
							if (newval != oldval) {
								changed = true;
								break;
							}
						}
						if (changed) {
							for (var i = 0; i <= _dcc[0].length - 1; i++) {
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + GetColumnValue(row, i) + '"');
							}
							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					}
				});

				if (changedRows.length == 0) {
					ShowDimmer(false);
					MessageBox('You have not made any changes');
				}
				else {
					var json = '[' + changedRows.join(",") + ']';
					PageMethods.SaveChanges(json, save_done, on_error);
				}
			} catch (e) {
				ShowDimmer(false);
				MessageBox('There was an error gathering data to save.\n' + e.message);
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
					refreshPage();
				}
				else {
					MessageBox('Failed to save items. \n' + errorMsg);
				}
			} catch (e) { }
		}

		function on_error(result) {
			ShowDimmer(false);

			var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

	</script>

	<script id="jsEvents" type="text/javascript">

		function refreshPage() {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			qs = editQueryStringValue(qs, 'View', $('#<%=this.ddlView.ClientID %> option:selected').val());
			qs = editQueryStringValue(qs, 'ShowArchived', ($('#chkShowArchived').is(':checked') ? 1 : 0));

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {

		}

		function imgSort_click() {
			try {
				var sortableColumns = '<%=this.SortableColumns%>';
				while (sortableColumns.indexOf('<BR />') > -1) {
					sortableColumns = sortableColumns.replace("<BR />", ' ');
				}
				while (sortableColumns.indexOf('<BR/>') > -1) {
					sortableColumns = sortableColumns.replace("<BR/>", ' ');
				}
				while (sortableColumns.indexOf('<br />') > -1) {
					sortableColumns = sortableColumns.replace("<br />", ' ');
				}
				while (sortableColumns.indexOf('<br/>') > -1) {
					sortableColumns = sortableColumns.replace("<br/>", ' ');
				}

				while (sortableColumns.indexOf('...') > -1) {
					sortableColumns = sortableColumns.replace('...', '');
				}

				while (sortableColumns.indexOf('<BR>') > -1) {
					sortableColumns = sortableColumns.replace('<BR>', ' ');
				}
				while (sortableColumns.indexOf('<br>') > -1) {
					sortableColumns = sortableColumns.replace('<br>', ' ');
				}

				var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				var nPopup = popupManager.AddPopupWindow("Sorter", "Sort Grid", sURL, 200, 400, "PopupWindow", this.self);
				if (nPopup) {
					nPopup.Open();
				}
			}
			catch (e) {
			}
		}

		function applySort(sortValue) {
			try {
				var pURL = window.location.href;
				pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
				pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

				window.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
			}
		}

		function ddlView_change() {
			refreshPage();
		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
			}
		}

		function reports_click(action, type) {
			var name = "", title = "";

			if (action.toUpperCase() == 'AOR') {
				name = "AOR";
				var url = "";
				switch (type.toUpperCase()) {
					case "HOTLIST":
						name += type;
						title += " " + type;
						url = "Report_Hotlist.aspx";
						break;
				}

				var url = 'Loading.aspx?Page=' + url;

				var h = 800, w = 1250;

				//open in a popup
				//var openPopup = popupManager.AddPopupWindow(name, title, url, h, w, 'PopupWindow', this);
				//if (openPopup) {
				//	openPopup.Open();
				//}

				var w = window.open(url);
			}
		}

		function openMenuItem(url) {
			if (url.indexOf("relatedItems") > 0) {
				var str = url.split("'");
				relatedItems_click(str[1], str[3]);
			}
		}

		function resizeFrames() {
			var frame;
			var fPageHeight = 0;

			$('iFrame').each(function () {
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

			resizePageElement($('#<%=this.grdHotlist.ClientID %>').attr('id'), 0);
			//<%=this.grdHotlist.ClientID %>.RedrawGrid();
		}

		function buttonNewRequest_click() {
			var title = '', url = '';
			var h = 700, w = 1000;

			title = 'New Work Request';
			url = _pageUrls.Maintenance.WorkRequestEditParent;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('NewWorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function lbEditRequest_click(requestID) {
			if (parent.ShowFrameForWorkRequest) {
				parent.ShowFrameForWorkRequest(false, requestID, requestID, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'Work Request - [' + requestID + ']';
				url = _pageUrls.Maintenance.WorkRequestEditParent
					+ '?WorkRequestID=' + requestID;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}


		function imgShowHideChildren_click(sender, direction, id) {
			try {
				if (id == "0" || id == "ALL") {
					var parentId = '0';

					$('[Name="img' + direction + '"]').each(function () {
						parentId = $(this).attr('parentId');
						if (parentId && +parentId > 0) {
							imgShowHideChildren_click(this, direction, parentId);
						}
					});
				}

				if (direction.toUpperCase() == "SHOW") {
					//show row/div with child grid frame
					//get frame and pass url(if necessary)
					var td;

					$(sender).closest('tr').each(function () {
						var currentRow = $(this);
						var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
						$(row).show();

						td = $('td:eq(<%=this.DCC == null ? 0 : this.DCC["SORT_ORDER"].Ordinal%>)', row)[0];
						loadChildGrid(td, id);
					});
				}
				else {
					$('tr[Name="gridChild_' + id + '"]').hide();
				}

				$(sender).hide();
				$(sender).siblings().show();
			} catch (e) {
				var msg = e.message;
			}
		}

		function loadChildGrid(td, id) {
			var url = 'Loading.aspx?Page=';

			//url += _pageUrls.Maintenance.RequestGroupGrid_Requests;
			url += 'RequestGroupGrid_Requests.aspx';
			url += '?requestGroupId=' + id;
			url += '&ShowArchived=' + ($('#chkShowArchived').is(':checked') ? 1 : 0);

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}



		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
			}
		}

		function buttonSave_click() {
			if (_canEditGroup) {
				save();
			}
			else {
				return false;
			}
		}

		function activateSaveButton() {
			if (_canEditGroup) {
				$('#buttonSave').attr('disabled', false);
				$('#buttonSave').prop('disabled', false);
				$('#buttonSave').css('cursor', 'pointer');
			}
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

	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();
				_myView = parseInt('<%=this.MyView %>');

				_idx = +'<%=this.DCC != null ? this.DCC["RequestGroupID"].Ordinal : 0%>';

				_canEditGroup = ('<%=this.CanEditGroup.ToString().ToUpper()%>' == 'TRUE');
				_canEditRequest = ('<%=this.CanEditRequest.ToString().ToUpper()%>' == 'TRUE');
			} catch (e) { }

			try {
				_idxDescription = '<%=this.DCC == null ? 0 : this.DCC["DESCRIPTION"].Ordinal%>';
			} catch (e) { }
		}

		$(document).ready(function () {

			initVariables();

			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
			if (_canEditRequest) {
				$('#buttonNewRequest').attr('disabled', false);
				$('#buttonNewRequest').click(function (event) { buttonNewRequest_click(); return false; });
			}
			if (_canEditGroup) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });
				$('select').on('change', function () { txt_change(this); });

				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
			}

			$('#chkShowArchived').click(function () { $('#imgRefresh').trigger('click'); });

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			$('#<%=this.ddlView.ClientID %>').change(function () { ddlView_change(); return false; });
			$('#chkShowArchived').prop('checked', '<%=this._showArchived %>' == '1');
		});
	</script>

</asp:Content>