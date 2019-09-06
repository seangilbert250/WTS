<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="Workload_CrosswalkGrid_Items.aspx.cs" Inherits="Workload_CrosswalkGrid_Items" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
	<title>Workload Items</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
	<form id="form1" runat="server">

		<img id="imgSort" alt="Sort Items" title="Sort Items" src="images/icons/page_sort_a_z.png" width="15" height="15" style="cursor:pointer; margin-left:4px;" />

		<div id="divPageContainer">
			<iti_Tools_Sharp:Grid ID="grdWorkload" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
				CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
			</iti_Tools_Sharp:Grid>

			<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

		</div>
	
		<script id="jsVariables" type="text/javascript">
			var _pageUrls;
			var _selectedId = 0;
			var _canViewWorkItem = false, _canEditWorkItem = false;
			var _statuses = '';
			var _assigned = '';
			var _reReadDatabase = false;

			var useColumnOrdering = false;
		</script>

		<script id="jsAJAX" type="text/javascript">

			function save() {
				try {
					ShowDimmer(true, "Updating...", 1);

					var changedRows = [];
					var id = 0;
					var original_value = '', name = '', description = '', sortOrder = '', archive = '';

					var changed = false;
					$('.gridBody, .selectedRow', $('#<%=this.grdWorkload.ClientID%>_Grid')).each(function (i, row) {
						var changedRow = [];
						changed = false;

						if ($(this).attr('changed') && $(this).attr('changed') == '1') {
							changed = true;
						}
						if (changed) {
							for (var i = 0; i <= _dcc[0].length - 1; i++) {
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + (_dcc[0][i].ColumnName == 'TITLE' || _dcc[0][i].ColumnName == 'DESCRIPTION' ? encodeURIComponent(GetColumnValue(row, i)) : GetColumnValue(row, i)) + '"');
							}
							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					});

					if (changedRows.length == 0) {
					    _reReadDatabase = false;
					    ShowDimmer(false);
						MessageBox('You have not made any changes');
					}
					else {
					    _reReadDatabase = true;
					    ShowDimmer(true, "Updating...", 1);
						var json = '[' + changedRows.join(",") + ']';
						PageMethods.SaveChanges(json, save_done, on_error);
					}
				} catch (e) {
				    _reReadDatabase = false;
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
						msg = 'Successfully saved ' + saved + ' Workload Tasks.';
					}
					if (failed > 0) {
						msg += '\n' + 'Failed to save ' + failed + ' Workload Tasks.';
					}
					MessageBox(msg);

					refreshPage();

				} catch (e) { }
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

			function refreshPage(statuses, Assigned, /*workTypes,*/ showClosed) {
				var qs = document.location.href;
				//qs = editQueryStringValue(qs, 'RefData', 1); 
				qs = editQueryStringValue(qs, 'RefData', _reReadDatabase); 

				if (!statuses || statuses == null || statuses == undefined) {
					statuses = _statuses;
				}

				if (!Assigned || Assigned == null || Assigned == undefined) {
				    Assigned = _assigned;
				}

				if (showClosed == undefined) showClosed = '<%=this.ShowClosed %>';

				qs = editQueryStringValue(qs, 'Statuses', statuses);
				qs = editQueryStringValue(qs, 'ShowClosed', showClosed);
				qs = editQueryStringValue(qs, 'Assigned', Assigned);

				document.location.href = 'Loading.aspx?Page=' + qs;
			}

			function resizeFrame() {
				/*
				Hack job
				When grid AllowResize="false", pager doesn't display.
				When grid AllowResize="true", grid doesn't resize correctly, hence the need to manually resize.
				*/

			    var myFrame = getMyFrameFromParent();
				var fPageHeight = $('#grdWorkload_Grid').height();
				var pagerHeight = $('.gridPager').height() + 2;

				myFrame.style.height = fPageHeight + 8 + pagerHeight + 'px';
				$('#grdWorkload_BodyContainer').height(fPageHeight - pagerHeight + 8);

				if (parent.resizeFrames) parent.resizeFrames();  
			}

			function row_click(row) {
				if ($(row).attr('itemID')) {
				    _selectedId = $(row).attr('itemID');
				    localStorage.setItem("_selectedId", _selectedId);
				}
			}

			function lbEditWorkItem_click(recordId) {

			    _reReadDatabase = false;

				if (parent.parent.ShowFrameForChild) {
					parent.parent.ShowFrameForChild('WorkItem', recordId, recordId);
				}
				else {
					var title = '', url = '';
					var h = 700, w = 1400;

					title = 'Work Item - [' + recordId + ']';
					url = _pageUrls.Maintenance.WorkItemEditParent
						+ '?WorkItemID=' + recordId;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
			}

			function imgShowHideChildTasks_click(sender, direction, id) {
				try {

				    _reReadDatabase = false;

				    var blnTaskColumn = '<%=this.TaskColumn %>'.toUpperCase() == 'TRUE';

					if (id == "0" && blnTaskColumn) {
						var itemId = '0';

						$('[Name="img' + direction + '"]').each(function () {
							itemId = $(this).attr('workItemId');
							if (itemId && +itemId > 0) {
								imgShowHideChildTasks_click(this, direction, itemId);
							}
						});
					}
					else {

						if (direction.toUpperCase() == "SHOW") {
							//show row/div with child grid frame
							//get frame and pass url(if necessary)
							var td;

							$(sender).closest('tr').each(function () {
								var currentRow = $(this);
								var row;

								if (!blnTaskColumn) {
									row = $(currentRow).next('tr[Name="gridGroup_' + id + '"]');
								}
								else {
									row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
								}

								$(row).show();

								td = $('td:eq(<%=this.DCC == null ? 0 : this.ItemIDIndex %>)', row)[0];
								loadChildGrid_Tasks(td, id, row);
							});
						}
						else {
							if (!blnTaskColumn) {
								$('tr[Name="gridGroup_' + id + '"]').hide();
							}
							else {
								$('tr[Name="gridChild_' + id + '"]').hide();
							}
						}
					}
					$(sender).hide();
					$(sender).siblings().show();

				} catch (e) { }
			}

		    function loadChildGrid_Tasks(td, id, row) {

		        try {

	            _reReadDatabase = false;

				var url = '';

				if ('<%=this.TaskColumn %>'.toUpperCase() == 'FALSE') {
					var qs = document.location.href;
					
					if (row.attr('filters')) {
						var filters = row.attr('filters');
						
						qs = editQueryStringValue(qs, 'rowFilters', filters);
						qs = editQueryStringValue(qs, 'childList', true);
					}

					url = 'Loading.aspx?Page=' + qs;
				}
				else {
					url = 'Loading.aspx?Page='
							//+ _pageUrls.Maintenance.WorkItemGrid_Tasks;
							+ 'WorkItem_Tasks.aspx'
							+ '?workItemID=' + id
							+ '&parent=grid'
							+ '&SelectedStatuses=' + '<%=Request.QueryString["statuses"] == null ? "" : Request.QueryString["statuses"].ToString()%>'
                            + '&SelectedAssigned=' + '<%=Request.QueryString["Assigned"] == null ? "" : Request.QueryString["Assigned"].ToString()%>'
							+ '&ShowArchived=' + '<%=_showArchived %>'
							+ '&rowFilters=' + '<%=this.Filters %>'
                            + '&ShowBacklog=' + '<%=this.ShowBacklog %>';
				}

				$('iFrame', $(td)).each(function () {
					var src = $(this).attr('src');
					if (src == "javascript:''") {
						$(this).attr('src', url);
					}

					$(this).show();

				});
		        } catch (e) {
		            //alert("Error in child grid load routine. " + e.message);
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
					frame.style.height = fPageHeight + 8 + 'px';
				});

				resizePageElement($('#<%=this.grdWorkload.ClientID %>').attr('id'), 0);
				//<%=this.grdWorkload.ClientID %>.RedrawGrid();
				resizeFrame();
			}

			function chk_change(sender) {
				var value = '', originalValue = '';
				value = $(sender).prop('checked') ? 1 : 0;

				if ($(sender).attr("original_value")) {
					originalValue = $(sender).attr("original_value");
				}

				if (value != originalValue) {
					$(sender).closest('tr').attr('changed', '1');
				}
			}

			function txt_change(sender) {
				var original_value = '', new_value = '';
				if ($(sender).attr('original_value')) {
					original_value = $(sender).attr('original_value');
				}

				new_value = $(sender).val();

				if (new_value != original_value) {
					$(sender).closest('tr').attr('changed', '1');
				}
			}

			function ddl_change(ddl) {

				var value = '', originalValue = '';
				value = $('option:selected', $(ddl)).val();
				if ($(ddl).attr("original_value")) {
					originalValue = $(ddl).attr("original_value");
				}

				if (value != originalValue) {
					$(ddl).closest('tr').attr('changed', '1');
				}
			}


			function buttonSave_click() {
				save();
			}

			function SetTaskQty(workItemID, taskCount) {
				$('.taskCount_' + workItemID).text('(' + taskCount + ')');
			}

		</script>

		<script id="jsDropdowns" type="text/javascript">

			/*
				Start Grid Dropdown List loading
			*/
			function showDropdown(ddl) {
				var event;

				try {
					event = new MouseEvent('click', {
						view: window,
						bubbles: false,
						cancelable: true
					});

					/* can be added for i.e. compatiblity. */
					ddl.focus();
					var WshShell = new ActiveXObject("WScript.Shell");
					WshShell.SendKeys("%{DOWN}");
					/**/

					ddl.dispatchEvent(event);

					return;
				} catch (e) {

				}

				try {
					event = document.createEvent('MouseEvents');
					event.initMouseEvent('mousedown', true, true, window,
						0, 0, 0, 80, 20,
						false, false, false, false,
						0, null);
					ddl.dispatchEvent(event);

					/* can be added for i.e. compatiblity. */
					ddl.focus();
					var WshShell = new ActiveXObject("WScript.Shell");
					WshShell.SendKeys("%{DOWN}");
					/**/
				} catch (e) {

				}

				event.stopPropagation();
				return false;
			}


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
						case "ASSIGNED":					//WTS_RESOURCEID
							idField = "WTS_RESOURCEID";
							textField = "USERNAME";
							data = _userList;
							break;
						case "PRIMARY_DEVELOPER":			//WTS_RESOURCEID
							idField = "WTS_RESOURCEID";
							textField = "USERNAME";
							data = _userList;
							break;
						case "PRIMARYBUSINESSRESOURCE":		//WTS_RESOURCEID
							idField = "WTS_RESOURCEID";
							textField = "USERNAME";
							data = _userList;
							break;
					    case "SECONDARYBUSINESSRESOURCE":		//WTS_RESOURCEID
					        idField = "WTS_RESOURCEID";
					        textField = "USERNAME";
					        data = _userList;
					        break;
					    case "RESOURCEPRIORITYRANK":		//PRIORITYID
							idField = "PRIORITYID";
							textField = "PRIORITY";
							data = _priorityList;
							break;
						case "PRIMARYBUSINESSRANK":		//PRIORITYID
							idField = "PRIORITYID";
							textField = "PRIORITY";
							data = _priorityList;
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

			        var sURL = 'SortOptions.aspx?GridName=Workload_CrosswalkGrid_Items.ASPX&sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
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
		            var pURL = document.location.href;
		            pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
		            pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

		            document.location.href = 'Loading.aspx?Page=' + pURL;
		        }
		        catch (e) {
		        }
		    }


		</script>
	</form>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();
				_statuses = '<%=Request.QueryString["statuses"] == null ? "" : Request.QueryString["statuses"].ToString() %>';
			    _assigned = '<%=Request.QueryString["Assigned"] == null ? "" : Request.QueryString["Assigned"].ToString() %>';

			    useColumnOrdering = localStorage.getItem("UseColumnOrdering");


			} catch (e) { }
		}

		$(document).ready(function () {
			initVariables();

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			if ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE') {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { chk_change(this); });
				$('select', $('#<%=this.grdWorkload.ClientID %>_Grid')).on('change keyup mouseup', function () { ddl_change(this); });

				$('select', $('#<%=this.grdWorkload.ClientID %>_Grid')).on('click focus', function () { LoadList(this); });
			}

		    $('#imgSort').click(function (event) { imgSort_click(); return false; });

		    resizeFrame();

		});

	</script>
</body>
</html>
