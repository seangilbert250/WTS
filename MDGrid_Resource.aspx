<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_Resource.aspx.cs" Inherits="MDGrid_Resource" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Resource Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
                 Resource (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;" >
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                     height="15" style="cursor: pointer; margin-right: 10px; float: right;" />
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
				<input type="button" id="buttonRemove" value="Delete" disabled="disabled"/>
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
    <div id="divAddResourcesPopup" runat="server" style="position:absolute; display:none; background-color:#ffffff;">
        <div class="pageContentInfo" style="text-align: right;">
            <input type="button" id="btnSaveResourceSelections" value="Save Resource" style="margin-left:3px;margin-top:3px;" />
            &nbsp;<input type="button" id="btnCloseResourceSelections" value="Close" style="margin-top:3px;" />
        </div>
        <div style="padding: 3px;">
            <span>Resource: </span>
            <asp:DropDownList ID="ddlResource" runat="server" Width="80%" style="padding: 3px; float: right; background-color: #f5f6ce;"></asp:DropDownList>
        </div>
        <div style="padding: 20px 3px 3px 3px;">
            <span>Intake Team: </span>
            <asp:CheckBox ID="chkActionTeam" runat="server" style="margin-right: 76%; float: right;"></asp:CheckBox>
        </div>
        <div style="padding: 20px 3px;">
            <span>Systems: </span>
            <div style="width: 80%; float: right;">
                <wts:MultiSelect runat="server" ID="msResourceSystems"
                ItemLabelColumnName="WTS_SYSTEM"
                ItemValueColumnName="WTS_SYSTEMID"
                IsOpen="true"
                KeepOpen="true"
                Width="100%"
                MaxHeight="330"
                HideDDLButton="true"
                SelectAll="true"
                />
            </div>
         </div>
    </div>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

		var _pageURLs = new PageURLs();
		var _idxDelete = 0, _idxID = 0, _currentLevel = 0, _selectedID = 0;
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
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
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
					PageMethods.SaveChanges(json, '<%=this._qfSystemSuiteID%>', '<%=this._qfProductVersionID%>', save_done, on_error);
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
                    popupManager.RemovePopupWindow('AddResourceToSuite');
				}
				else {
					MessageBox('Failed to save items. \n' + errorMsg);
				}
			} catch (e) { }
		}

        function removeItem(itemId) {
            try {
                ShowDimmer(true, "Removing Resource...", 1);

                PageMethods.RemoveItem(parseInt(itemId), <%=this._qfSystemSuiteID%>, <%=this._qfProductVersionID%>,removeItem_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to remove the resource.\n' + e.message);
            }
        }
        function removeItem_done(result) {
            ShowDimmer(false);

            var removed = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
                        removed = true;
                    }
                    if (obj.id) {
                        id = obj.id;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (removed) {
                    MessageBox('The resource has been removed from this item.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to remove resource from this item. \n' + errorMsg);
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

			document.location.href = 'Loading.aspx?Page=' + qs;
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

				var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=this.SortOrder%>';
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

		function deleteNewRow(img) {
            $(img).closest('tr').remove();
            resizeFrame();
		}

		function buttonNew_click() {
            var nWindow = 'AddResourceToSuite';
            var nTitle = 'Add Resource';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divAddResourcesPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function btnSaveResourceSelections_click() {
            var arrSystems = msResourceSystems_getSelections(true);
            var resource = top.$('#<%=this.ddlResource.ClientID%>').val();
            var actionTeam = top.$('#<%=this.chkActionTeam.ClientID%>')[0].checked;

            if (resource == 0) {
                dangerMessage('Please select a Resource', null, true, $(popupManager.GetPopupByName('AddResourceToSuite').Window).find('[id*=divAddResourcesPopup]')[0]);
            } else if (arrSystems.length == 0) {
                dangerMessage('Please select a least one System', null, true, $(popupManager.GetPopupByName('AddResourceToSuite').Window).find('[id*=divAddResourcesPopup]')[0]);
            }
            else {
                ShowDimmer(true, "Saving...", 1);
                var json = arrSystems.join(",");
                PageMethods.SaveResource(json, resource, actionTeam, '<%=this._qfSystemSuiteID%>', '<%=this._qfProductVersionID%>', save_done, on_error);
            }
        }

        function closeCloseResourceSelections() {
            popupManager.RemovePopupWindow('AddResourceToSuite');
        }

        function buttonRemove_click() {
            if (!_selectedId || _selectedId == '' || _selectedId == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will remove the resource from the suite.' + '\n' + 'Do you wish to continue?')) {
                removeItem(_selectedId);
            }
        }

		function buttonSave_click() {
			save();
		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
                _selectedId = $(row).attr('itemID');
                $('#buttonRemove').attr('disabled', false);
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
                activateSaveButton(sender);
                $(sender).closest('tr').attr('rowChanged', '1');
            }
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
							|| !this.DCC.Contains("USERNAME")) ? 0 : this.DCC["USERNAME"].Ordinal %>)', row)[0];
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

			url += 'MDGrid_Resource_System.aspx';// _pageUrls.MasterData.Grid.Effort;
            url += '?WTS_SYSTEM_SUITE_RESOURCEID=' + id;
			url += '&ChildView=0';
			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

                $(this).show();
            });
		}


        function resizeGrid() {
    setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
}
	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
                _pageUrls = new PageURLs();
                _currentLevel = '<%=this.CurrentLevel %>';

				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
				_isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxID = +'<%=this.DCC == null ? 0 : this.DCC["WTS_RESOURCEID"].Ordinal %>';
				}
			} catch (e) {

			}
		}
		
		$(document).ready(function () {

            initVariables();
			
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			
			$('#imgReport').hide();
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
			if (_canEdit) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
                $('input').on('change keyup mouseup', function () { txt_change(this); });
                $('select').on('change keyup mouseup', function () { ddl_change(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });

                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
                $('#buttonRemove').click(function (event) { buttonRemove_click(); return false; });

                $('#btnSaveResourceSelections').click(function (event) { btnSaveResourceSelections_click(); return false; });
                $('#btnCloseResourceSelections').on('click', function () { closeCloseResourceSelections(); });

			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });
			
            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
            msResourceSystems_init();
            $('.ms-drop, .bottom').css('width', '100%');
            resizeFrame();
		});
	</script>

</asp:Content>

