<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="UserProfile_Hardware.aspx.cs" Inherits="Admin_UserProfile_Hardware" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">
</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
	<img src="../Images/Icons/pencil.png" alt="Review/Edit User Hardware" width="15" height="15" style="cursor: default;" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">User Hardware (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="buttonNew" value="Add" disabled="disabled" style="padding: 2px;" />
	<input type="button" id="buttonSave" value="Save" disabled="disabled" style="padding: 2px;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<asp:GridView runat="server" ID="grdHardware" AllowPaging="false" CssClass="grid" RowStyle-CssClass="gridBody" SelectedRowStyle-CssClass="selectedRow" HeaderStyle-CssClass="gridHeader" AlternatingRowStyle-BackColor="#dfdfdf" Style="width: 100%;"></asp:GridView>

	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>

	<script id="jsVariables" type="text/javascript">

		var _pageURLs = new PageURLs();
		var _userID = 0;
		var _idxDelete = 0, _idxID = 0, _idxSN = 0, _idxName = 0, _idxDescription = 0, _idxHasDevice = 0;
		var _htmlDeleteImage = '<img src="../Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';

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
				var changedRows = [];
				var id = 0;
				var original_value = '', name = '', description = '', sortOrder = '', archive = '';

				$('.gridBody, .selectedRow', $('#<%=this.grdHardware.ClientID%>')).each(function (i, row) {
					var changedRow = [];
					var changed = false;

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
					alert('You have not made any changes');
				}
				else {
					ShowDimmer(true, "Updating...", 1);
					var json = '[' + changedRows.join(",") + ']';
					PageMethods.SaveChanges(_userID, json, save_done, on_error);
				}
			} catch (e) {
				ShowDimmer(false);
				alert('There was an error gathering data to save.\n' + e.message);
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
					alert('Items have been saved.');
					refreshPage();
				}
				else {
					alert('Failed to save items. \n' + errorMsg);
				}
			} catch (e) { }
		}

		function deleteItem(itemId, item) {
			try {
				ShowDimmer(true, "Deleting...", 1);

				PageMethods.DeleteItem(_userID, parseInt(itemId), item, deleteItem_done, on_error);

			} catch (e) {
				ShowDimmer(false);
				alert('There was an error gathering data to save.\n' + e.message);
			}
		}
		function deleteItem_done(result) {
			ShowDimmer(false);

			var deleted = false;
			var id = '', errorMsg = '';

			try {
				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
						deleted = true;
					}
					if (obj.id) {
						id = obj.id;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				if (deleted) {
					alert('Item has been deleted.');
					refreshPage();
				}
				else {
					alert('Failed to delete item. \n' + errorMsg);
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

			alert('save error:  \n' + resultText);
		}

	</script>

	<script id="jsEvents" type="text/javascript">
		
		function refreshPage() {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);

			document.location.href = '../Loading.aspx?Page=' + qs;
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

				var sURL = '../SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
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

				window.location.href = '../Loading.aspx?Page=' + pURL;
			}
			catch (e) {
			}
		}

		function deleteNewRow(img) {
			$(img).closest('tr').remove();
		}
		
		function buttonNew_click() {
			var grdHardware = <%=this.grdHardware.ClientID%>;
			
			if (grdHardware.rows.length == 0) {
				return;
			}
			var nRow = grdHardware.rows[grdHardware.rows.length - 1].cloneNode(true);
			$(nRow.cells[_idxID]).text('0');//.innerText = '0';
			$(nRow.cells).each(function(i, td) {
				if($(td).find('input:text').length > 0) {
					$(td).find('input:text').attr('original_value', '');
					$(td).find('input:text').text('');
					$(td).find('input:text').val('');
				}
				else if($(td).find('input:checkbox').length > 0) {
					$(td).find('input:checkbox').attr('original_value', '');
					$(td).find('input:checkbox').attr('checked', false);
					$(td).find('input:checkbox').prop('checked', false);
				}
				else if($(td).children('input').length > 0) {
					$(td).find('input').attr('original_value', '');
					$(td).find('input').text('');
					$(td).find('input').val('');
				}
				else if($(td).children('select').length > 0) {
					$(td).find('select').attr('original_value', '');
				}
				else{
					$(td).html('&nbsp;');
				}
			});
			
			grdHardware.rows[0].parentNode.insertBefore(nRow,grdHardware.rows[1]);
			//add delete button
			$(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
			$(nRow).show();
		}

		function imgDelete_click(itemID, item) {
			if (!itemID || itemID == '' || itemID == 0) {
				alert('You must specify an item to delete.');
				return;
			}

			if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
				deleteItem(itemID, item);
			}
		}

		function buttonSave_click() {
			save();
		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
			}
		}

		function activateSaveButton() {
			if (_canEdit) {
				$('#buttonSave').attr('disabled', false);
				$('#buttonSave').prop('disabled', false);
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

				_userID = +'<%=this.UserId%>';

				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
				_isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxDelete = +'<%=this.DCC == null ? 0 : this.DCC["X"].Ordinal%>';
					_idxID = +'<%=this.DCC == null ? 0 : this.DCC["WTS_Resource_HardwareID"].Ordinal %>';
					_idxName = +'<%=this.DCC == null ? 0 : this.DCC["DeviceName"].Ordinal %>';
					_idxDescription = +'<%=this.DCC == null ? 0 : this.DCC["Description"].Ordinal %>';
					_idxHasDevice = +'<%=this.DCC == null ? 0 : this.DCC["HasDevice"].Ordinal %>';
				}
			} catch (e) {

			}
		}

		$(document).ready(function () {

			initVariables();

			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');

			$('#imgReport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgRefresh').attr('src', '../Images/Icons/arrow_refresh_blue.png');
			if (_canEdit) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });

				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

		});
	</script>

</asp:Content>