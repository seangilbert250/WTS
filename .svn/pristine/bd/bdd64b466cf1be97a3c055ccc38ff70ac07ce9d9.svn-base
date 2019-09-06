<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RequestGroupGrid_Requests.aspx.cs" Inherits="RequestGroupGrid_Requests" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request Group - Work Requests</title>
	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divPageContainer">
			<asp:GridView runat="server" ID="gridRequest" Style="width: 100%; border: none;" GridLines="None" AllowPaging="false" AllowSorting="false" PageSize="10" CellPadding="0" CellSpacing="0"
				CssClass="grid" RowStyle-CssClass="gridBody" HeaderStyle-CssClass="gridHeader" SelectedRowStyle-CssClass="gridBody" FooterStyle-CssClass="gridPager" PagerStyle-CssClass="gridPager">
			</asp:GridView>
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

		<div id="divStatusHelp" style="position: absolute; left: 0px; top: 0px; height: 100%; background: #FFFFFF; padding:0px; display: none;">
			<table id="tableStatusHelp" style="text-align: left; width: 325px; padding:2px;" cellpadding="0" cellspacing="0">
				
			</table>
		</div>

		<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

		<script type="text/javascript" src="Scripts/jquery-ui.js"></script>

		<script type="text/javascript">
			var _pageUrls;
			var _parentId = 0, _originalHeight = 0;
			var _canEdit = false;
			var _myFrame;

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function resizePage(visibleDatepicker) {
				resizePageElement('divPageContainer');

				resizeFrame(visibleDatepicker);
			}

			function resizeFrame(visibleDatepicker) {
				if (visibleDatepicker == null || visibleDatepicker == undefined) {
					visibleDatepicker = false;
				}
				if (typeof visibleDatepicker != "boolean" && typeof visibleDatepicker != "string") {
					return;
				}

				var dpHeight = 207;
				var fPageHeight = 0;

				if (_myFrame
					&& _myFrame.contentWindow
					&& _myFrame.contentWindow.document
					&& _myFrame.contentWindow.document.body
					&& _myFrame.contentWindow.document.body.offsetHeight) {
					fPageHeight = _myFrame.contentWindow.document.body.offsetHeight;
					if (_originalHeight == 0) {
						_originalHeight = fPageHeight;
					}
				}
				if (visibleDatepicker) {
					if (fPageHeight < (_originalHeight + dpHeight)) {
						fPageHeight = (_originalHeight + dpHeight);
					}
				}
				else {
					fPageHeight = _originalHeight;
				}

				_myFrame.style.height = fPageHeight + 'px';
			}
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

					$('.gridBody, .selectedRow', $('#<%=this.gridRequest.ClientID%>')).each(function (i, row) {
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

			function showStatusHelp(img, field) {
				var dt = {};
				var statusType = '', helpText = '';

				switch (field.toUpperCase()) {
					case "TD_STATUS":
						if (_dtTDStatus && _dtTDStatus[0] && _dtTDStatus[0].length > 0) {
							dt = _dtTDStatus[0];
						}
						break;
					case "CD_STATUS":
						if (_dtCDStatus && _dtCDStatus[0] && _dtCDStatus[0].length > 0) {
							dt = _dtCDStatus[0];
						}
						break;
					case "C_STATUS":
						if (_dtCStatus && _dtCStatus[0] && _dtCStatus[0].length > 0) {
							dt = _dtCStatus[0];
						}
						break;
					case "IT_STATUS":
						if (_dtITStatus && _dtITStatus[0] && _dtITStatus[0].length > 0) {
							dt = _dtITStatus[0];
						}
						break;
					case "CVT_STATUS":
						if (_dtCVTStatus && _dtCVTStatus[0] && _dtCVTStatus[0].length > 0) {
							dt = _dtCVTStatus[0];
						}
						break;
					case "A_STATUS":
						if (_dtAStatus && _dtAStatus[0] && _dtAStatus[0].length > 0) {
							dt = _dtAStatus[0];
						}
						break;
					case "CR_STATUS":
						if (_dtCRStatus && _dtCRStatus[0] && _dtCRStatus[0].length > 0) {
							dt = _dtCRStatus[0];
						}
						break;
				}

				if (dt == null || dt.length == 0) {
					return;
				}

				$('#tableStatusHelp').empty();
				var headerRow = document.createElement('tr');
				$('#tableStatusHelp').append(headerRow);
				var headerCell = document.createElement('td');
				$(headerRow).append(headerCell);
				var header = document.createElement('span');
				$(headerCell).append(header);

				statusType = dt[0].StatusType_Description;
				$(header).css('font-weight', 'bold');
				$(header).text(statusType);

				for (var i = 0; i < dt.length; i++) {
					var row = document.createElement('tr');
					var cell = document.createElement('td');

					$('#tableStatusHelp').append(row);
					$(row).append(cell);

					$(cell).text(dt[i].STATUS + ' - ' + dt[i].Status_Description);
				}

				resizePageElement('divStatusHelp');

				var imgLeft = $(defaultParentPage.sidebar).width() + getAbsoluteLeft(_myFrame) + getAbsoluteLeft(img);
				var left = (imgLeft - $('#tableStatusHelp').width());
				var top = $(defaultParentPage.mainPageHeader).height() + getAbsoluteTop(_myFrame) + getAbsoluteTop(img) + $(img).height() + 10;

				MessageBox($('#divStatusHelp').html(), field + " Description", this);

				popupManager.ActivePopup.Window.style.top = top + "px";
				popupManager.ActivePopup.Window.style.left = left + "px";

			}

			function imgSave_click() {
				if (_canEdit) {
					save();
				}
				else {
					return false;
				}
			}

			function lbEditRequest_click(recordId) {
				
				if (parent.parent.ShowFrameForWorkRequest) {
					parent.parent.ShowFrameForWorkRequest(false, recordId, 0, true);
				}
				else {
					var title = '', url = '';
					var h = 700, w = 1000;

					title = 'Work Request - [' + recordId + ']';
					url = _pageUrls.Maintenance.WorkRequestEditParent
						+ '&WorkRequestID=' + recordId;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
			}

			function ddl_change(ddl) {
				var textField = $(ddl).attr('field');

				var value = '', originalValue = '';
				value = $('option:selected', $(ddl)).val();
				if ($(ddl).attr("original_value")) {
					originalValue = $(ddl).attr("original_value");
				}

				if (value != originalValue) {
					activateSaveButton(true);
				}
			}

			function row_click(row) {
				if ($(row).attr('itemID')) {
					_selectedId = $(row).attr('itemID');
				}
			}

			function activateSaveButton() {
				if (_canEdit) {
					$('[id*="imgSave"]').attr('disabled', false);
					$('[id*="imgSave"]').prop('disabled', false);
					$('[id*="imgSave"]').css('cursor', 'pointer');
					$('[id*="imgSave"]').unbind('click').click(function (event) { imgSave_click(); return false; });
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

    </form>

	<script id="jsInit" type="text/javascript">

		$(document).ready(function () {
			_pageUrls = new PageURLs();
			_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
			_myFrame = getMyFrameFromParent();

			if (_canEdit) {
				$('input:text').on('change keyup mouseup', function () { txt_change(this); });
				$('input:checkbox').on('change', function () { txt_change(this); });
				$('input').on('change keyup mouseup', function () { txt_change(this); });
				$('select').on('change', function () { txt_change(this); });

				$('.date').each(function () {
					$(this).datepicker({
						changeMonth: true
						, showOtherMonths: true
						, selectOtherMonths: true
						, changeYear: true
						, onSelect: function () { resizePage(true); }
						, onClose: function () { resizePage(false); }
					});
				}).click(function () { resizePage(true); }).focus(function () { resizePage(true); });
			}

			$(window).resize(resizeFrame);

			resizeFrame(false);
		});
	</script>
</body>
</html>
