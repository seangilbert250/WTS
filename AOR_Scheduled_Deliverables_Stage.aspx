﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_Stage.aspx.cs" Inherits="AOR_Scheduled_Deliverables_Stage" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - System / Resources Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">System / Resources (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonNew" value="Add" disabled="disabled" />
                <input type="button" id="buttonEdit" value="View/Edit" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPageContents" style="width: 100%;">
		<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
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

		var _pageURLs = new PageURLs();
		var _idxDelete = 0, _idxID = 0;
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
                    else if ($(tdval).find('textarea').length > 0) {
                        if (blnoriginal_value) {
                            val = $(tdval).find('textarea').attr('original_value');

                        }
                        else {
                            val = $(tdval).find('textarea').val();
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

				$('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
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
					ShowDimmer(false);
					MessageBox('You have not made any changes');
				}
				else {
				    var json = '[' + changedRows.join(",") + ']';
					PageMethods.SaveChanges('<%=ReleaseID %>', json, save_done, on_error);
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
				    msg = 'Successfully saved ' + saved + ' items.';
					if (opener && opener.refreshPage) {
						opener.refreshPage();
					}
				}
				if (failed > 0) {
				    msg += '\n' + 'Failed to save ' + failed + ' items';
				}
				MessageBox(msg);

			} catch (e) { }
		}

		function deleteItem(itemId) {
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
					if (opener && opener.refreshPage) {
						opener.refreshPage();
					}
                    parent.refreshChildren();
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

            document.location.href = 'Loading.aspx?Page=' + qs;
            resizeFrame();
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

		function ddlQF_change() {
			refreshPage();
		}

		function deleteNewRow(img) {
			$(img).closest('tr').remove();
		}

		function buttonNew_click() {
            var obj = parent;

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('AOR', false, 1, true, 1);
            }
            else {
                var nWindow = 'Deployment';
                var nTitle = 'Deployment';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=true';
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function buttonEdit_click() {
            var obj = parent;

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('AOR', false, 1, true, 1);
            }
            else {
                var nWindow = 'Deployment';
                var nTitle = 'Deployment';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=false&DeliverableID=' + _selectedId;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function buttonDelete_click() {
            if (!_selectedId || _selectedId == '' || _selectedId == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
                deleteItem(_selectedId);
            }
        }

		function buttonSave_click() {
			save();
        }

        function openContracts(id) {
            var nWindow = 'AOR';
            var nTitle = 'Deployment';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=false&Tab=Contracts&DeliverableID=' + id;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

		function row_click(row) {
            if (_canView) {
                if ($(row).attr('itemID')) {
                    _selectedId = $(row).attr('itemID');
                    $('#buttonEdit').prop('disabled', false);
                }
            } 

            if (_canEdit) {
                if ($(row).attr('itemID')) {
                    _selectedId = $(row).attr('itemID');
                    $('#buttonDelete').attr('disabled', false);
                }
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
                        var currentRowStage = $(this);
                        var row = $(currentRowStage).next('tr[Name="gridChildStage_' + id + '"]');
                        $(row).show();
                        td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("ReleaseScheduleDeliverable")) ? 0 : this.DCC["ReleaseScheduleDeliverable"].Ordinal %>)', row)[0];
                        loadChildGrid(td, id, "Stage");

                        var currentRowSession = $(this);
                        var plannedEndDate = $(currentRowSession).find('td').eq('<%= this.DCC["PlannedEnd"].Ordinal %>').find('input').val();
                        var contractIDs = $(currentRowSession).find('td').eq('<%= this.DCC["ContractIDs"].Ordinal %>').text();
                        var AORReleaseIDs = $(currentRowSession).find('td').eq('<%= this.DCC["AORReleaseIDs"].Ordinal %>').text();
                        var row = $(currentRowSession).next().next('tr[Name="gridChildSession_' + id + '"]');
                        $(row).show();
                        td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("ReleaseScheduleDeliverable")) ? 0 : this.DCC["ReleaseScheduleDeliverable"].Ordinal %>)', row)[0];
                        loadChildGrid(td, id, "Session", plannedEndDate, contractIDs, AORReleaseIDs);
                    });
                }
                else {
                    $('tr[Name="gridChildStage_' + id + '"]').hide();
                    $('tr[Name="gridChildSession_' + id + '"]').hide();
                }

                $(sender).hide();
                $(sender).siblings().show();
                resizeFrame();
            } catch (e) {
                var msg = e.message;
            }
        }

        function loadChildGrid(td, id, gridNm, plannedEndDate, contractIDs, AORReleaseIDs) {
            if (gridNm == 'Stage') {
                var url = 'Loading.aspx?Page=';
                url += 'AOR_Scheduled_Deliverables_Stage_AORs.aspx?&ReleaseID=' + <%=ReleaseID%>+ '&ReleaseScheduleDeliverableID=' + id;


                $('iFrame', $(td)).each(function () {
                    var src = $(this).attr('src');
                    if (src == "javascript:''") {
                        $(this).attr('src', url);
                    }

                    $(this).show();
                });
            }
            if (gridNm == 'Session') {
                var url = 'Loading.aspx?Page=';
                url += 'MDGrid_ProductVersion_Session.aspx?&ProductVersionID=' + <%=ReleaseID%> + '&PlannedEndDate=' + plannedEndDate + '&SelectedContracts=' + contractIDs + '&SelectedAORs=' + AORReleaseIDs;


                $('iFrame', $(td)).each(function () {
                    var src = $(this).attr('src');
                    if (src == "javascript:''") {
                        $(this).attr('src', url);
                    }

                    $(this).show();
                });
            }
        }
		
        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

		function openSystemResource(releaseID, resourceID, resource) {
		    var nWindow = 'ResourceAllocation';
		    var nTitle = 'Resource Allocation';
		    var nHeight = 300, nWidth = 500;
		    var nURL = _pageUrls.MasterData.Edit.SystemResource;

		    nURL += '?ReleaseID=' + releaseID + '&ResourceID=' + resourceID + '&Resource=' + resource;

		    var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

		    if (openPopup) openPopup.Open();
        }

        function calcDeploymentRisk(deploymentID, risk) {
            $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
                if ($(this).find('td:eq(' + <%=DCC.IndexOf("ReleaseScheduleID") %> + ')').text() == deploymentID) {
                    $(this).find('td:eq(' + <%=DCC.IndexOf("Risk") %> + ')').text(risk);

                    if (risk == 'High (Emergency)') { $(this).find('td:eq(' + <%=DCC.IndexOf("Risk") %> + ')').css('background-color', 'red'); }
                    if (risk == 'Moderate (Acceptable)') { $(this).find('td:eq(' + <%=DCC.IndexOf("Risk") %> + ')').css('background-color', 'yellow'); }
                    if (risk == 'Low (Routine)') { $(this).find('td:eq(' + <%=DCC.IndexOf("Risk") %> + ')').css('background-color', 'limegreen'); }
                    resizeGrid();
                }
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
					_idxID = +'<%=this.DCC == null ? 0 : this.DCC["ReleaseScheduleID"].Ordinal %>';
				}
			} catch (e) {

			}
		}
		
		$(document).ready(function () {

			initVariables();
			
			$('.pageContentHeader').hide();
			
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			
			$('#imgReport').hide();
			$('#imgExport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
			if (_canEdit) {
				$('input:text, textarea').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox, select').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });
                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });
			}

            $('#buttonEdit').click(function (event) { buttonEdit_click(); return false; });
			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });
			
			
			resizeFrame();
		});
	</script>

</asp:Content>