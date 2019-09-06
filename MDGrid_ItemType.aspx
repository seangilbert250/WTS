﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_ItemType.aspx.cs" Inherits="MDGrid_ItemType" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Work Activity Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
                Work Activity (<span id="spanRowCount" runat="server">0</span>)
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
			<td style="padding-left: 5px;">Work Activity:
				<asp:DropDownList ID="ddlQF" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 85px;">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
            <td style="padding-left: 5px;">Child View:
                <asp:DropDownList ID="ddlChildView" runat="server" TabIndex="1" Style="width: 145px;">               
                    <asp:ListItem Text="Status" Value="0" />
					<asp:ListItem Text="Resource Type" Value="1" />
					<asp:ListItem Text="Resource" Value="2" />
				</asp:DropDownList>
            </td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonSyncRes" value="Sync Resources" style="display: none;"/>
				<input type="button" id="buttonSyncResType" value="Sync Resource Types" style="display: none;"/>
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
    <div id="divSyncResourcesPopup" runat="server" style="position:absolute; display:none; background-color:#ffffff;">
        <div class="pageContentInfo" style="text-align: right;">
            <input type="button" id="btnSyncResourceType" value="Sync Resource Types" style="margin-left:3px;margin-top:3px;" />
            &nbsp;<input type="button" id="btnCloseSyncResourceTypes" value="Close" style="margin-top:3px;" />
        </div>
        <div style="padding: 3px;">
            <span>Work Activity: </span>
            <div style="padding-right: 100px; float: right;">
                <wts:MultiSelect runat="server" ID="msWorkActivity"
                ItemLabelColumnName="Item Type"
                ItemValueColumnName="WorkItemTypeID"
                IsOpen="false"
                KeepOpen="true"
                Width="100%"
                MaxHeight="330"
                />
            </div>
        </div>
    </div>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

		var _pageURLs = new PageURLs();
		var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
		var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';
		var imgHelp = document.getElementById("imgHelp");
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
			try {
				var changedRows = [];
				var id = 0;
				var original_value = '', name = '', description = '', sortOrder = '', archive = '';

				$('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
					var changedRow = [];
					var changed = false;

					if (_dcc[0].length > 0 && $(this)[0].hasAttribute('fieldChanged')) {
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
						        var value = GetColumnValue(row, i);
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + encodeURIComponent(GetColumnValue(row, i)) + '"');
							}
							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					}
				});

				if (changedRows.length == 0) {
					MessageBox('You have not made any changes');
				}
				else {
					ShowDimmer(true, "Updating...", 1);
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
			var resultText = 'An error occurred when communicating with the server';
			MessageBox('save error:  \n' + resultText);
		}

	</script>

    	<script id="jsEvents" type="text/javascript">
		
		function refreshPage() {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			qs = editQueryStringValue(qs, 'TypeID', $('#<%=this.ddlQF.ClientID %> option:selected').val());
            qs = editQueryStringValue(qs, 'ChildView', $('#<%=this.ddlChildView.ClientID %> option:selected').val());

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {
		    window.location.href += '&Export=1';
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

        function buttonSyncResType_click() {
            var nWindow = 'SyncResourceTypes';
            var nTitle = 'Sync Resource Types';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divSyncResourcesPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function buttonSyncRes_click() {
            var nWindow = 'SyncResources';
            var nTitle = 'Sync Resources';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divSyncResourcesPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function btnSyncResourceType_click() {
            var arrWorkActivity = msWorkActivity_getSelections(true);

            if (arrWorkActivity.length == 0) {
                dangerMessage('Please select a Work Activity', null, true, $(popupManager.GetPopupByName('SyncResourceTypes').Window).find('[id*=divSyncResourcesPopup]')[0]);
            } else {
                popupManager.RemovePopupWindow('SyncResourceTypes');

                ShowDimmer(true, "Saving...", 1);
                var workActivities = arrWorkActivity.join(",");
                PageMethods.SaveResourceType(workActivities, save_done, on_error);
            }
        }

        function btnSyncResource_click() {
            var arrWorkActivity = msWorkActivity_getSelections(true);

            if (arrWorkActivity.length == 0) {
                dangerMessage('Please select a Work Activity', null, true, $(popupManager.GetPopupByName('SyncResources').Window).find('[id*=divSyncResourcesPopup]')[0]);
            } else {
                popupManager.RemovePopupWindow('SyncResources');

                ShowDimmer(true, "Saving...", 1);
                var workActivities = arrWorkActivity.join(",");
                PageMethods.SaveResource(workActivities, save_done, on_error);
            }
        }

        function btnCloseSyncResourceTypes_click() {
            popupManager.RemovePopupWindow('SyncResourceTypes');
        }

		function deleteNewRow(img) {
            $(img).closest('tr').remove();
            resizeFrame();
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
                else if ($(td).children('select').length > 0) {
                    $(td).find('select').attr('original_value', '');
                    $(td).find('select').on('change keyup mouseup', function () { ddl_change(this); });
                    $(td).find('select').val('');
                }
				else{
					$(td).html('&nbsp;');
				}
			});

            $(nRow).attr('fieldChanged', true);
			grdMD.Body.Rows[0].parentNode.insertBefore(nRow,grdMD.Body.Rows[0]);
			//add delete button
			$(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
            $(nRow).show();
            resizeFrame();
		}

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
                        $(row).attr('nohighlight', 'true');

						td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("Item Type")) ? 0 : this.DCC["Item Type"].Ordinal %>)', row)[0];
						loadChildGrid(td, id);
					});
				}
				else {
					$('tr[Name="gridChild_' + id + '"]').hide();
				}

				$(sender).hide();
                $(sender).siblings().show();
                resizeFrame();
			} catch (e) {
				var msg = e.message;
			}
		}

		function loadChildGrid(td, id) {
			var url = 'Loading.aspx?Page=';
            switch ($('#<%=this.ddlChildView.ClientID %> option:selected').val()) {
                case "0": // Status
                    url += 'MDGrid_ItemType_Status.aspx';
                    url += '?WORKITEMTYPEID=' + id;
                    break;
                case "1": // Resource Type
                    url += 'MDGrid_ItemType_ResourceType.aspx';
                    url += '?WORKITEMTYPEID=' + id;
                    break;
                case "2": // Resource
                    url += 'MDGrid_ItemType_Resource.aspx';
                    url += '?WORKITEMTYPEID=' + id;
                    break;
                default:
                    break;
            }
			
			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}

		function lbEdit_click(itemID) {
			if (parent.ShowFrameForEdit) {
				parent.ShowFrameForEdit(_pageURLs.MasterData.MDType.Scope, false, itemID, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'Work Activity - [' + itemID + ']';
				url = _pageURLs.MasterData.Edit.Scope
					+ '?itemID=' + itemID;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('Work Activity', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
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

		function buttonSave_click() {
			save();
		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
			}
		}

		function activateSaveButton(sender) {
			if (_canEdit) {
				$('#buttonSave').attr('disabled', false);
                $('#buttonSave').prop('disabled', false);
                $(sender).closest('tr').attr('fieldChanged', true);
			}
		}

		function txt_change(sender) {
			var original_value = '', new_value = '';
			if ($(sender).attr('original_value')) {
				original_value = $(sender).attr('original_value');
			}

			new_value = $(sender).val();

			if (new_value != original_value) {
				activateSaveButton(sender);
			}
        }

        function ddl_change(sender) {
            var original_value = '', new_value = '';
            if ($(sender).attr('original_value')) {
                original_value = $(sender).attr('original_value');
            }

            new_value = $('option:selected', $(sender)).val();

            if (new_value != original_value) {
                activateSaveButton();
                $(sender).closest('tr').attr('fieldChanged', '1');
            }
        }

        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

	</script>

	<script id="jsAJAX" type="text/javascript">
	</script>

    	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
				_isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxDelete = parseInt('<%=this.DCC["X"].Ordinal %>');
					_idxID = parseInt('<%=this.DCC["WORKITEMTYPEID"].Ordinal %>');
					_idxName = parseInt('<%=this.DCC["Item Type"].Ordinal %>');
					_idxDescription = parseInt('<%=this.DCC["Description"].Ordinal %>');
					_idxSortOrder = parseInt('<%=this.DCC["Sort Order"].Ordinal %>');
					_idxArchive = parseInt('<%=this.DCC["Archive"].Ordinal %>');
				}
			} catch (e) {

			}
		}
		
		$(document).ready(function () {
            initVariables();
            if ($('#<%=this.ddlChildView.ClientID %> option:selected').val() === "1") {
                $('#buttonSyncResType').show();
                $('#btnSyncResourceType').click(function (event) { btnSyncResourceType_click(); return false; });
            } else if ($('#<%=this.ddlChildView.ClientID %> option:selected').val() === "2") {
                $('#buttonSyncRes').show();
                $('#btnSyncResourceType').click(function (event) { btnSyncResource_click(); return false; });
                $('#btnSyncResourceType').val('Sync Resources');
            }

			$('#imgReport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
			if (_canEdit) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });
                $('select').on('change keyup mouseup', function () { ddl_change(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });
                $('#buttonSyncResType').click(function (event) { buttonSyncResType_click(); return false; });
                $('#buttonSyncRes').click(function (event) { buttonSyncRes_click(); return false; });

                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });

                $('#btnCloseSyncResourceTypes').on('click', function () { btnCloseSyncResourceTypes_click(); });
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });
			
            $('#<%=this.ddlQF.ClientID %>').change(function () { ddlQF_change(); return false; });
			$('#<%=this.ddlChildView.ClientID %>').change(function () { ddlQF_change(); return false; });
            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
            msWorkActivity_init();
            $('.ms-drop, .bottom').css('width', '100%');
            resizeFrame();
		});
	</script>
</asp:Content>
