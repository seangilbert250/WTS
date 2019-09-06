﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_System.aspx.cs" Inherits="MDGrid_System" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - System Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
                System(Task) (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;" >
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                     height="15" style="cursor: pointer; margin-right: 10px; float: right; display: none;" />
            </td>   
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
            <td style="padding-left: 5px;">System Suite:
				<asp:DropDownList ID="ddlQF_Suite" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 85px;">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
			<td style="padding-left: 5px;">System(Task):
				<asp:DropDownList ID="ddlQF_System" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 85px;">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
			<td style="padding-left: 5px; display: none;">Child View:
                <asp:DropDownList ID="ddlChildView" runat="server" TabIndex="1" Style="width: 145px;">               
                    <asp:ListItem Text="Work Area" Value="0" />
                    <%--<asp:ListItem Text="Allocation Assignment" Value="1" />--%>
					<asp:ListItem Text="Resource" Value="2" />
                    <%--<asp:ListItem Text="Contract" Value="3" />--%>
				</asp:DropDownList>
            </td>
            <td id="tdRelease" style="padding-left: 5px; display: none;">Release:
                <asp:DropDownList ID="ddlRelease" runat="server" TabIndex="1" Style="width: 100px;"></asp:DropDownList>
            </td>
            <td id="tdPageSize" style="padding-left: 5px;">Page Size:
                <asp:DropDownList ID="ddlPageSize" runat="server" TabIndex="1" Style="width: 100px;">
                    <asp:ListItem Text="12" Value="1"></asp:ListItem>
                    <asp:ListItem Text="25" Value="2"></asp:ListItem>
                    <asp:ListItem Text="50" Selected="True" Value="3"></asp:ListItem>
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

				ShowDimmer(true, "Updating...", 1);

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
				                    changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + (_dcc[0][i].ColumnName == 'Description' ? encodeURIComponent(GetColumnValue(row, i)) : GetColumnValue(row, i)) + '"');
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
					msg = 'Successfully saved ' + saved + ' record(s).';
					if (opener && opener.refreshPage) {
						opener.refreshPage(true);
					}
				}
				if (failed > 0) {
					msg += '\n' + 'Failed to save ' + failed + ' record(s).';
				}
				MessageBox(msg);

				if (saved > 0) {
					refreshPage();
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
			qs = editQueryStringValue(qs, 'SystemID', $('#<%=this.ddlQF_System.ClientID %> option:selected').val());
            qs = editQueryStringValue(qs, 'SuiteID', $('#<%=this.ddlQF_Suite.ClientID %> option:selected').val());
		    qs = editQueryStringValue(qs, 'ChildView', $('#<%=this.ddlChildView.ClientID %> option:selected').val());

		    if ($('#<%=ddlChildView.ClientID %> option:selected').text() == 'Resource') {
                qs = editQueryStringValue(qs, 'ReleaseID', $('#<%=this.ddlRelease.ClientID %> option:selected').val());
            }

            qs = editQueryStringValue(qs, 'PageSize', $('#<%=this.ddlPageSize.ClientID %> option:selected').val());
            
			document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function refreshChildren() {
            // Collection of iframes
            var childFrames = $('iframe');

            for (var i = 0; i < childFrames.length; i++) {
                // Only reloads iframes that are relevant
                if (childFrames[i].id) {
                    childFrames[i].contentWindow.location.reload();
                }
            }
        }

		function imgExport_click() {
			var url = window.location.href;
			url = editQueryStringValue(url, 'Export', true);

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

		function ddlChildView_change() {
		    refreshPage();
		}

		function ddlRelease_change() {
		    refreshPage();
        }

        function ddlPageSize_change() {
            refreshPage();
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
				else if($(td).children('select').length > 0) {
				    $(td).find('select').attr('original_value', '');
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

		function lbEdit_click(itemID) {
			if (parent.ShowFrameForEdit) {
				parent.ShowFrameForEdit(_pageURLs.MasterData.MDType.System, false, itemID, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'System - [' + itemID + ']';
				url = _pageURLs.MasterData.Edit.System
					+ '?itemID=' + itemID;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('System', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function lbEditWorkAreas_click(itemID) {
			var title = '', url = '';
			var h = 500, w = 800;

			title = 'Work Area - Systems';
			url = 'MDGrid_WorkArea_System.aspx?SystemID=' + itemID;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('WorkArea_Systems', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
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
		        activateSaveButton(sender);
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

						td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("WTS_SYSTEM")) ? 0 : this.DCC["WTS_SYSTEM"].Ordinal %>)', row)[0];
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
            var cvVal = $('#<%=ddlChildView.ClientID %>').val();
	        var url = 'Loading.aspx?Page=';

	        switch ($('#<%=ddlChildView.ClientID %> option:selected').text()) {
	            case 'Resource':
	                url += 'MDGrid_System_Resource.aspx?SystemID=' + id + '&ReleaseID=' + $('#<%=ddlRelease.ClientID %> ').val();
	                break;
                case 'Contract':
	                url += 'MDGrid_System_Contract.aspx?SystemID=' + id;
	                break;
	            default:
	                url += 'MDGrid_WorkArea_System.aspx?SystemID=' + id + '&ChildView=' + cvVal;// _pageUrls.MasterData.Grid.Effort;
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

	    function getParameterByName(name, url) {
	        if (!url) url = window.location.href;
	        url = url.toLowerCase(); // This is just to avoid case sensitiveness  
	        name = name.replace(/[\[\]]/g, "\\$&").toLowerCase();// This is just to avoid case sensitiveness for query parameter name
	        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
	        if (!results) return null;
	        if (!results[2]) return '';
	        return decodeURIComponent(results[2].replace(/\+/g, " "));
        }

        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

        function openSystemContracts(systemID) {
            var title = '', url = '';
            var h = 250, w = 400;

            title = 'System - Contracts';
            url = 'MDGrid_System_Contract.aspx?SystemID=' + systemID;

            var openPopup = popupManager.AddPopupWindow('System_Contracts', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
            if (openPopup) {
                openPopup.Open();
            }
        }

        function reviewWorkAreas(systemID) {
            try {
                ShowDimmer(true, "Saving...", 1);
                if (confirm('This will mark this item as reviewed.' + '\n' + 'Do you wish to continue?')) {
                    PageMethods.ReviewWorkAreas(systemID, review_done, on_error);
                } else {
                    ShowDimmer(false);
                }
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }

        function review_done(result) {
            ShowDimmer(false);

            var saved = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.savedIds) {
                        id = obj.savedIds;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    $('#<%=this.grdMD.ClientID%>_Grid').find('tr[itemid="' + id + '"]').children()['<%=this.DCC["WorkItem_Count"].Ordinal - 1 %>'].innerHTML = '<img alt="Work Areas Reviewed" title="Work Areas Reviewed" src="images/icons/check.png" width="15" height = "15" style="cursor: pointer;" />';
                }
                else {
                    MessageBox('Failed to save item. \n' + errorMsg);
                }
            } catch (e) { }
        }
	</script>

    <script id="jsColOrder" type="text/javascript">

        function imgReorder_click() {
            openGridOrderer();
        }

        function openGridOrderer() {

            var sURL = 'Grid_Order.aspx';
            var nPopup = popupManager.AddPopupWindow("Orderer", "Order Grid Columns", sURL, 445, 500, "PopupWindow", this.self);
            if (nPopup) {
                nPopup.Open();
            }
        }

        function getSelectedColumnOrder(blnDefault) {
            try {
                var selectedColumnOrder = '<%=this.SelectedColumnOrder%>';
                var defaultColumnOrder = '<%=this.DefaultColumnOrder%>';

                if (blnDefault) {
                    return defaultColumnOrder;
                }
                else {
                    return selectedColumnOrder;
                }
            }
            catch (e) {
                return "";
            }
        }

        function updateColumnOrder(columnOrder) {
            try {

                var pURL = document.location.href;
                pURL = editQueryStringValue(pURL, 'columnOrder', escape(columnOrder));
                pURL = editQueryStringValue(pURL, 'columnOrderChanged', 'true');
                pURL = editQueryStringValue(pURL, 'RefData', '0');

                document.location.href = 'Loading.aspx?Page=' + pURL;
            }
            catch (e) {
                MessageBox('updateColumnOrder:\n' + e.number + '\n' + e.message);
            }
        }

        function SetTaskQty(workItemID, taskCount) {
            $('.taskCount_' + workItemID).text('(' + taskCount + ')');
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
					_idxDelete = parseInt('<%=this.DCC["X"].Ordinal %>');
					_idxID = parseInt('<%=this.DCC["WTS_SystemID"].Ordinal %>');
					_idxName = parseInt('<%=this.DCC["WTS_System"].Ordinal %>');
					_idxDescription = parseInt('<%=this.DCC["DESCRIPTION"].Ordinal %>');
					_idxSortOrder = parseInt('<%=this.DCC["SORT_ORDER"].Ordinal %>');
					_idxArchive = parseInt('<%=this.DCC["ARCHIVE"].Ordinal %>');
				}
			} catch (e) {

			}
		}
		
		$(document).ready(function () {
			initVariables();
			
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			
            $('#imgReorder').show();
			$('#imgReport').hide();

			switch ($('#<%=ddlChildView.ClientID %> option:selected').text()) {
			    case 'Resource':
			        $('#imgExport').hide();
			        $('#tdRelease').show();
			        break;
			    case 'Contract':
			        $('#imgExport').hide();
			        break;
			}

            $('#imgReorder').click(function () { imgReorder_click(); });
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

				$('#buttonSave').click(function (event) { buttonSave_click(); return false; });
			}

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });
			
			$('#<%=this.ddlQF_System.ClientID %>').change(function () { ddlQF_change(); return false; });
            $('#<%=this.ddlQF_Suite.ClientID %>').change(function () { ddlQF_change(); return false; });
		    $('#<%=this.ddlChildView.ClientID %>').change(function(){ ddlChildView_change(); return false;});
            $('#<%=this.ddlRelease.ClientID %>').change(function () { ddlRelease_change(); return false; });
		    $('#<%=this.ddlPageSize.ClientID %>').change(function(){ ddlPageSize_change(); return false;});
            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
            resizeFrame();
		});
	</script>

</asp:Content>

