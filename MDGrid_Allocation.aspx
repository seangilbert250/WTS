﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" EnableViewState="false" EnableEventValidation="false" AutoEventWireup="true" CodeFile="MDGrid_Allocation.aspx.cs" Inherits="MDGrid_Allocation" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Allocation Assignment Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
             Allocation Assignment (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;" >
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                     height="15" style="cursor: pointer; margin-right: 10px; float: right;" />
            </td>   
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
			<td style="padding-left: 5px;  display:none">Allocation Category:
				<asp:DropDownList ID="ddlQF" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 200px;">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
			<td style="padding-left: 5px;">Allocation Group:
				<asp:DropDownList ID="ddlQFGroup" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 200px;">
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
				<input type="button" id="buttonNew" value="Add" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

		var _pageURLs = new PageURLs();
		var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
		var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';
		var imgHelp = document.getElementById("imgHelp");
	</script>

	<script id="jsAJAX" type="text/javascript">

		function save() {
			try {
				ShowDimmer(true, "Updating...", 1);
					
				var changedRows = [];
				var id = 0;
				var original_value = '', name = '', description = '', sortOrder = '', archive = '';

				var changed = false;
				$('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
					var changedRow = [];
					changed = false;

					if ($(this).attr('changed') && $(this).attr('changed') == '1') {
						changed = true;
					}

					if (changed) {
						for (var i = 0; i <= _dcc[0].length - 1; i++) {
							changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
						}
						var obj = '{' + changedRow.join(',') + '}';
						changedRows.push(obj);
					}
				});

				if (changedRows.length == 0) {
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
                    // 11-28-2016 - Moved to after refresh
					//MessageBox('Items have been saved.');
					refreshPage();
					MessageBox('Items have been saved.');
                }
				else {
					MessageBox('Failed to save items. \n' + errorMsg);
				}
			} catch (e) { }
		}

		function deleteItem(itemId, item) {
			try {
				ShowDimmer(true, "Deleting...", 1);

				PageMethods.DeleteItem(parseInt(itemId), item, deleteItem_done, on_error);

			} catch (e) {
				ShowDimmer(false);
				MessageBox('There was an error gathering data to save.\n' + e.message);
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

	<script id="jsEvents" type="text/javascript">
		
		function refreshPage() {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			qs = editQueryStringValue(qs, 'AllocationID', $('#<%=this.ddlQF.ClientID %> option:selected').val());
			qs = editQueryStringValue(qs, 'AllocationGroupID', $('#<%=this.ddlQFGroup.ClientID %> option:selected').val());

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {
			var url = window.location.href;
			url = editQueryStringValue(url, 'Export', true);
            url = editQueryStringValue(url, 'AllocationID', $('#<%=this.ddlQF.ClientID %> option:selected').val());
		    url = editQueryStringValue(url, 'AllocationGroupID', $('#<%=this.ddlQFGroup.ClientID %> option:selected').val());
		    window.open('Loading.aspx?Page=' + url);
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

		function ddlQF_change() {
			refreshPage();
		}

		function ddlQFGroup_change() {
		    refreshPage();
		}

		function deleteNewRow(img) {
			$(img).closest('tr').remove();
		}

		function buttonNew_click() {
			var grdMD = <%=this.grdMD.ClientID%>;
			
			var nRow = grdMD.Body.Rows[0].cloneNode(true);
			$(nRow.cells[_idxID]).text('0');//.innerText = '0';
			$(nRow.cells).each(function(i, td){
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
					$(td).find('select').on('change keyup mouseup', function () { ddl_change(this); });
					$(td).find('select').on('click focus', function () { LoadList(this); });
				}
				else{
					$(td).html('&nbsp;');
				}
			});
			
			grdMD.Body.Rows[0].parentNode.insertBefore(nRow,grdMD.Body.Rows[0]);
			//add delete button
			$(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
			$(nRow).show();
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
		
		function chk_change(sender) {
			var value = '', originalValue = '';
			value = $(sender).prop('checked') ? 1 : 0;

			if ($(sender).attr("original_value")) {
				originalValue = $(sender).attr("original_value");
			}

			if (value != originalValue) {
				$(sender).closest('tr').attr('changed','1');
				activateSaveButton();
			}
		}
		
		function txt_change(sender) {
			var original_value = '', new_value = '';
			if ($(sender).attr('original_value')) {
				original_value = $(sender).attr('original_value');
			}

			new_value = $(sender).val();

			if (new_value != original_value) {
				$(sender).closest('tr').attr('changed','1');
				activateSaveButton();
			}
		}
		
		function ddl_change(ddl) {
			
			var value = '', originalValue = '';
			value = $('option:selected', $(ddl)).val();
			if ($(ddl).attr("original_value")) {
				originalValue = $(ddl).attr("original_value");
			}

			if (value != originalValue) {
				$(ddl).closest('tr').attr('changed','1');
				activateSaveButton();
			}
		}
		

		/*
			Start Grid Dropdown List loading
		*/
        // 11-28-2016 - Not used, remarked out.
		//function showDropdown(ddl) {
		//	var event;

		//	try {
		//		event = new MouseEvent('click', {
		//			view: window,
		//			bubbles: false,
		//			cancelable: true
		//		});

		//		/* can be added for i.e. compatiblity. */
		//		ddl.focus();
		//		var WshShell = new ActiveXObject("WScript.Shell");
		//		WshShell.SendKeys("%{DOWN}");
		//		/**/

		//		ddl.dispatchEvent(event);

		//		return;
		//	} catch (e) {
				
		//	}
			
		//	try {
		//		event = document.createEvent('MouseEvents');
		//		event.initMouseEvent('mousedown', true, true, window,
		//			0, 0, 0, 80, 20,
		//			false, false, false, false,
		//			0, null);
		//		ddl.dispatchEvent(event);

		//		/* can be added for i.e. compatiblity. */
		//		ddl.focus();
		//		var WshShell = new ActiveXObject("WScript.Shell");
		//		WshShell.SendKeys("%{DOWN}");
		//		/**/
		//	} catch (e) {

		//	}

		//	event.stopPropagation();
		//	return false;
		//}

		
		/***********************
		Drop Down List Loading
		**********************/
		function LoadList(ctl) {
			try {
				var ddl;
				if (ctl.length == 0) {
					ddl = $(ctl.context);
				}
				else {
					ddl = ctl;
				}

				var selectedVal = '', selectedText = '';
				var idField = '', textField = '';
				var data = {};
				selectedVal = $(ddl).val();
				selectedText = $(ddl).find('option:selected').text();
				
				$('#lblMsg').show();
				textField = $(ddl).attr('field');
				if (textField == undefined || textField == 'undefined') {
					return;
				}
				switch (textField.toUpperCase()) {
					case "ALLOCATIONCATEGORY":			//AllocationCategoryID
						idField = "AllocationCategoryID";
						textField = "AllocationCategory";
						data = _AllocationCategoryList;
						break;
                        // 12092 - 3:
				    case "ALLOCATIONGROUP":			//AllocationGroupID
				        idField = "AllocationGroupID";
				        textField = "AllocationGroup";
				        data = _AllocationGroupList;
				        break;
				    case "DEFAULTASSIGNEDTO":			//WTS_RESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "UserName";
						data = _userList;
						break;
					case "DEFAULTSME":					//WTS_RESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "UserName";
						data = _userList;
						break;
					case "DEFAULTBUSINESSRESOURCE":		//WTS_RESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "UserName";
						data = _userList;
						break;
					case "DEFAULTTECHNICALRESOURCE":	//WTS_RESOURCEID
						idField = "WTS_RESOURCEID";
						textField = "UserName";
						data = _userList;
						break;
				}
				
				if (typeof data === 'undefined') {
					return;
				}

				$(ddl).empty();

				$.each(data[0], function (rowindex, row) {
					var mg1 = textField;
					var $option = $("<option />");
					// Add value and text to option
					$option.attr("value", row[idField]).text(row[textField]);
					$option.attr('title', row[textField]);
					$option.css('font-size', '12px');
					$option.css('font-family', 'Arial');
					// Add option to drop down list
					$(ddl).append($option);
				});

				if (!$('option[value="' + selectedVal + '"]', $(ddl))
					|| $('option[value="' + selectedVal + '"]', $(ddl)).length == 0) {
					var $o = $('<option />');
					$o.text('-Select-');
					$o.val('0');
					$o.css('font-size', '12px');
					$o.css('font-family', 'Arial');
					$(ddl).prepend($o);
					$(ddl).val('0');
				}
				else {
					$(ddl).val(selectedVal);
				}

				$(ddl).unbind('mousedown').unbind('focus').unbind('Onclick');
				$(ddl).prop('onclick', null);

				$('#lblMsg').hide();

				//showDropdown($(ddl)[0]);
			} catch (e) {
				MessageBox(e.message);
				$('#lblMsg').hide();
			}
		}
		/*
			End Grid Dropdown List loading
		*/

		
		function imgShowHideChildren_click(sender, direction, id) {
			try {
				if (id == "0" || id == "ALL") {
					var itemId = '0';

					$('[Name="img' + direction + '"]').each(function () {
						itemId = $(this).attr('itemId');
						if (requestId && +requestId > 0) {
							imgShowHideChildren_click(this, direction, itemId);
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

						td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("AllocationCategory")) ? 0 : this.DCC["AllocationCategory"].Ordinal %>)', row)[0];
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

			url += 'MDGrid_Allocation_System.aspx';// _pageUrls.MasterData.Grid.Effort;
			url += '?AllocationID=' + id;

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}
		
		function resizeFrame() {
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
				frame.style.height = fPageHeight + 8 + 'px';
			});

		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
				_isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxDelete = +'<%=this.DCC == null ? 0 : this.DCC["X"].Ordinal %>';
					_idxID = +'<%=this.DCC == null ? 0 : this.DCC["ALLOCATIONID"].Ordinal %>';
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
			$('#imgSort').click(function () { imgSort_click(); });
			if (_canEdit) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });
				$('select', $('#<%=this.grdMD.ClientID %>_Grid')).on('change keyup mouseup', function () { ddl_change(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });

				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });
			
			$('#<%=this.ddlQF.ClientID %>').change(function () { ddlQF_change(); return false; });
  			$('#<%=this.ddlQFGroup.ClientID %>').change(function () { ddlQFGroup_change(); return false; });

		    $('select', $('#<%=this.grdMD.ClientID %>_Grid')).on('click focus', function () { LoadList(this); });

		    resizePageElement($('#<%=this.grdMD.ClientID %>_BodyContainer').attr('id'), 23);
		    $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
		});
	</script>

</asp:Content>

