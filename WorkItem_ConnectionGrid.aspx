﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="WorkItem_ConnectionGrid.aspx.cs" Inherits="WorkItem_ConnectionGrid" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Work Item Connections</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Primary Task Connections (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0" style="display: none;">
		<tr>
			<td style="padding-left: 5px;">Work Item:
				<asp:DropDownList ID="ddlQF_WorkItem" runat="server" TabIndex="1" AppendDataBoundItems="true">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
			<td style="padding-left: 5px;">Test Item:
				<asp:DropDownList ID="ddlQF_TestItem" runat="server" TabIndex="2" AppendDataBoundItems="true">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<span style="display:none;">Show Archived<input type="checkbox" id="chkShowArchived" style="vertical-align: middle;" />
				&nbsp;</span>
				<input type="button" id="buttonNew" value="Add" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
				<input type="button" id="buttonClose" value="Close" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPageContents" style="width: 100%;">
		<iti_Tools_Sharp:Grid ID="grdWorkItem" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
			CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
		</iti_Tools_Sharp:Grid>
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
		var _pageUrls;
		var _canEdit = false;
		var _sourceType = 'WorkItem';
		var _idxDelete = 0, _idxID = 0, _idxStatus = 0, _workTestID;
		var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';

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
							val = $(tdval).find("select").val();
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

		    var changedRows = [];
			var id = 0;
			var original_value = '', name = '', description = '', sortOrder = '', archive = '';

			try {
				ShowDimmer(true, "Updating...", 1);

				$('.gridBody, .selectedRow', $('#<%=this.grdWorkItem.ClientID%>_Grid')).each(function (i, row) {
					var changedRow = [];
					var changed = false;

					if (_dcc && _dcc[0] && _dcc[0].length > 0) {
						for (var i = 0; i <= _dcc[0].length - 1; i++) {
							if (i == +'<%=this.DCC == null ? 0 : this.DCC["WorkItem_TestItemID"].Ordinal %>') {
								continue;
							}
							var newval = GetColumnValue(row, i);
							var oldval = GetColumnValue(row, i, true);
							if (newval != oldval) {
								changed = true;
								break;
							}
						}
						if (changed) {
							for (var i = 0; i <= _dcc[0].length - 1; i++) {
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
							}
							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					}
				});

				if (changedRows.length == 0) {
					ShowDimmer(false);
					MessageBox('No changes were detected.');
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

				var saved = 0, failed = 0;
				var errorMsg = '', ids = '', failedIds = '';

				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.saved) {
						saved = parseInt(obj.saved);
					}
					if (obj.failed) {
						failed = parseInt(obj.failed);
					}
					if (obj.savedIds) {
						ids = obj.savedIds;
					}
					if (obj.failedIds) {
						failedIds = obj.failedIds;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				var msg = '';
				if (errorMsg.length > 0) {
					msg = 'An error occurred while saving: \n' + errorMsg;
				}

				if (saved > 0) {
					msg = 'Successfully saved ' + saved + ' Primary Tasks.';
					//if (opener && opener.refreshPage) {
					//	opener.refreshPage(true);
					//}
				}
				if (failed > 0) {
					msg += '\n' + 'Failed to save ' + failed + ' Primary Tasks.';
				}
				MessageBox(msg);

				refreshPage();
			} catch (e) { }
		}
		
		function deleteItem(itemId, item) {
			try {
				ShowDimmer(true, "Deleting...", 1);

				PageMethods.DeleteItem(parseInt(itemId), deleteItem_done, on_error);

			} catch (e) {
				ShowDimmer(false);
				MessageBox('There was an error gathering data to delete.\n' + e.message);
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
					MessageBox('Item has been deleted.');
					refreshPage();
				}
				else {
					MessageBox('Failed to delete item. \n' + errorMsg);
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

	<script id="jsSort" type="text/javascript">
		
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

	</script>

	<script id="jsEvents" type="text/javascript">

		function refreshPage(preservePageNum) {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			if (preservePageNum && (preservePageNum == true || preservePageNum == 1)) {
				var pageIndex = '<%=this.grdWorkItem.PageIndex %>';
				qs = editQueryStringValue(qs, 'PageIndex', pageIndex);
			}
			else {
				qs = editQueryStringValue(qs, 'PageIndex', 0);
			}
			qs = editQueryStringValue(qs, 'IncludeArchive', ($('#chkShowArchived').is(':checked') ? 1 : 0));

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {

		}
		
		function row_click(row) {
			if ($(row).attr('itemID')) {
			    _workTestID = $(row).attr('itemID');
			    activateDeleteButton();
            }                    
		}
		
		function lbEditWorkItem_click(recordId) {
			var title = '', url = '';
			var h = 700, w = 1400;

			title = 'Primary Task - [' + recordId + ']';
			url = _pageUrls.Maintenance.WorkItemEditParent
				+ '?WorkItemID=' + recordId;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function refreshWorkItemDetails(control, sourceType, itemId) {

		}

		function activateSaveButton() {
			if (_canEdit) {
				$('#buttonSave').attr('disabled', false);
				$('#buttonSave').prop('disabled', false);
			}
		}

		function activateDeleteButton() {
		    if (_canEdit) {
		        $('#buttonDelete').attr('disabled', false);
		        $('#buttonDelete').prop('disabled', false);
		    }
		}

		function gridControl_change(control) {
			var original_value = '', new_value = '';
			if ($(control).attr('original_value')) {
				original_value = $(control).attr('original_value');
			}

			new_value = $(control).val();

			if (new_value != original_value) {
			    activateSaveButton();
			    activateDeleteButton();
			}
		}

		function deleteNewRow(img) {
			$(img).closest('tr').remove();
		}

		function buttonNew_click() {
			try {
				var grdWorkItem = <%=this.grdWorkItem.ClientID%>;
			
				var nRow = grdWorkItem.Body.Rows[0].cloneNode(true);
				$(nRow.cells[_idxID]).text('0');//.innerText = '0';
				$(nRow.cells).each(function(i, td){
					if(!$(td).attr('sourceType') || $(td).attr('sourceType') != _sourceType){
						if($(td).find('input:text').length > 0) {
							$(td).find('input:text').attr('original_value', '');
							$(td).find('input:text').text('');
							$(td).find('input:text').val('');
						}
						else if($(td).find('input:checkbox').length > 0) {
							$(td).find('input:checkbox').attr('original_value', '');
							$(td).find('input:checkbox').attr('checked', false);
							$(td).find('input:checkbox').prop('checked', false);
							$(td).find('input:checkbox').prop('disabled', true);
						}
						else if($(td).children('input').length > 0) {
							$(td).find('input').attr('original_value', '');
							$(td).find('input').text('');
							$(td).find('input').val('');
						}
						else if($(td).children('select').length > 0) {
							$(td).find('select').attr('original_value', '');
							$(td).find('select').prop('disabled', true);
						}
						else{
						    $(td).html('&nbsp;');
						}
					}
				});
			
				grdWorkItem.Body.Rows[0].parentNode.insertBefore(nRow,grdWorkItem.Body.Rows[0]);
				//add delete button
				$(nRow.cells[_idxDelete]).html(_htmlDeleteImage);

				$(nRow).show();

			} catch (e) {
				//MessageBox('Error adding new row: ' + e.message);
			}
		}

		function buttonSave_click() {
			save();
		}
		
		function buttonDelete_click(itemID, item)  
		{
		    if (!_workTestID || _workTestID == '' || _workTestID == 0) {
		        MessageBox('You must specify an item to delete.');
		        return;
		    }
		    item = "";

		    if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
		        deleteItem(_workTestID, item);
		    }
		}

		function imgDelete_click(itemID, item) {
			if (!itemID || itemID == '' || itemID == 0) {
				MessageBox('You must specify an item to delete.');
				return;
			}

			if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
				deleteItem(itemID, item);
			}
		}

		function buttonClose_click()
		{
		    if (closeWindow) {
		        closeWindow();
		    }
		    else {
		        window.close();
		    }
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initSettings() {
			try {
				$(':input').css('font-family', 'Arial');
				$(':input').css('font-size', '12px');
				$('#imgReport').hide();
			
				_pageUrls = new PageURLs();
				_sourceType = '<%=this.SourceType.Trim() %>';
				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				
				_idxDelete = +'<%=this.DCC == null ? 0 : this.DCC["Y"].Ordinal %>';
			    _idxID = +'<%=this.DCC == null ? 0 : this.DCC["WorkItem_TestItemID"].Ordinal %>';

			} catch (e) { }
		}

		function initEvents() {

			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });

			if (_canEdit) {
				$('#buttonNew').prop('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });

				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
				$('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });
				$('#buttonClose').click(function (event) { buttonClose_click(); return false; });

				$('input').on('change keyup mouseup', function () { gridControl_change(this); return false; });
				$('select').on('change', function () { gridControl_change(this); return false; });
			}

			$('#chkShowArchived').click(function () { $('#imgRefresh').trigger('click'); });
			$('#chkShowArchived').prop('checked', '<%=this.ShowArchived %>' == 'True');

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

		}

		$(document).ready(function () {

			initSettings();
			initEvents();

		});
	</script>

</asp:Content>