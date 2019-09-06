<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Grid_Order.aspx.cs" Inherits="Grid_Order" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" href="Styles/jquery-ui.css" />

	<script type="text/javascript" src="scripts/shell.js"></script>
	<script type="text/javascript" src="scripts/popupWindow.js"></script>
	<script type="text/javascript" src="scripts/tablednd.js"></script>
	<script src="scripts/jquery-1.11.2.min.js" type="text/javascript"></script>
	<script src="Scripts/jquery-ui.js" type="text/javascript"></script>
	<style type="text/css">
		.draggableRow {
			border-top:solid 1px gray;
			border-bottom:solid 1px gray;
			padding:2px;
		}
		.draggableRowLock {
			border-top:solid 1px gray;
			border-bottom:solid 1px gray;
			padding:2px;
			width:16px;
		}
		.draggableRowChild {
			border-top:solid 1px gainsboro;
			border-bottom:solid 1px gainsboro;
			border-left:solid 1px gainsboro;
			padding:2px;
		}
		.draggableRowChildLock {
			border-top:solid 1px gainsboro;
			border-bottom:solid 1px gainsboro;
			border-right:solid 1px gainsboro;
			padding:2px;
			width:16px;
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">    
		<div id="divTabsContainer" class="mainPageContainer">
			<ul id="Tabs" runat="server">
				<li><a href="#columnOrderer" onclick="Tab_click('main');">Main Grid</a></li>
				<li><a href="#divSub">Sub Grid</a></li>
			</ul>
			<div id="pageHeaderInfo" class="pageContentInfo" style="padding: 6px; height: 30px;">
				Select Columns to be displayed, click and drag to order Columns.
				<br />Click Save to apply changes.
			</div>
			<div id="pageFooter" class="pageContentHeader">
				<table cellpadding="0" cellspacing="0" style="width: 100%;">
					<tr nodrag="True">
						<td style="text-align: left; width: 150px; padding:2px;">
							<button id="btnResetDefault" onclick="resetDefault(); return false;" style="width: 115px;">Reset Default</button>
						</td>
						<td id="tdButtons" style="text-align: right; padding-right: 10px;">
							<button id="btnClose" onclick="closeWindow(); return false;">Cancel</button>
							<button id="btnSave" onclick="saveOrder(); return false;">Save</button>
						</td>
					</tr>
				</table>
			</div>
			<div id="columnOrderer" style="height:300px; overflow-y:auto; overflow-x:hidden; margin-top:-1px;">
				<table id="tblColumnOrder" runat="server" cellspacing="0" cellpadding="0" style="width: 100%; border-collapse: collapse;"></table>
			</div>
			<div id="columnOrdererSub" style="height:300px; overflow-y:auto; overflow-x:hidden; margin-top:-1px;">
				<table id="tblColumnOrderSub" runat="server" cellspacing="0" cellpadding="0" style="width: 100%; border-collapse: collapse;"></table>
			</div>
		</div>
		

		<script type="text/javascript">
			var columnDataObjects = [];

			var columnDataObject = function (columnDBName, columnName, visible, canOrder, groupName, viewable) {
				this.columnDBName = columnDBName;
				this.columnName = columnName;
				this.visible = visible;
				this.canOrder = canOrder;
				this.groupName = groupName;
				this.viewable = viewable;
				this.isAdded = false;
			}

			function resizePage() {
				var offsetHeight = 25;
				resizePageElement('columnOrderer', offsetHeight);
				resizePageElement('columnOrdererSub', offsetHeight);
			}

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			var tableDnD = [];

			function resetDragDrop() {
				var tables = document.getElementsByTagName('table');
				for (var i = 0; i <= tableDnD.length - 1; i++) {
					tableDnD[i] = null;
				}
				tableDnD = [];
				for (var i = 0; i <= tables.length - 1; i++) {
					tableDnD[tableDnD.length] = new TableDnD();
					tableDnD[tableDnD.length - 1].init(tables[i]);
					//tableDnD[tableDnD.length - 1].onDrop = updateOrder();
				}
			}
			
			function saveOrder(type) {
				try {
					var newColumnOrder = buildColumnOrder('Main');
					var newColumnOrderSub = buildColumnOrder('Sub');

					if (opener && opener.updateColumnOrder) {
						opener.updateColumnOrder(newColumnOrder,newColumnOrderSub);
					}
					else if (parent.updateColumnOrder) {
						parent.updateColumnOrder(newColumnOrder, newColumnOrderSub);
					}

					closeWindow();
				}
				catch (e) {
				}
			}

			function popuplateColumnDataObjects(columnData) {
				columnDataObjects = [];
				if (columnData){
					for (var i = 0; i <= columnData.length - 1; i++) {
						//ss: added this to handle the DB_NAME passed
						var columnDBName = '';
						if (columnData[i].split("|").length > 5) {
							 columnDBName = columnData[i].split("|")[0]
							var columnName = columnData[i].split("|")[1];
							var visible = columnData[i].split("|")[2];
							var canOrder = columnData[i].split("|")[3];
							var groupName = columnData[i].split("|")[4];
							var viewable = columnData[i].split("|")[5];
						}
						else {
							var columnName = columnData[i].split("|")[0];
							var visible = columnData[i].split("|")[1];
							var canOrder = columnData[i].split("|")[2];
							var groupName = columnData[i].split("|")[3];
							var viewable = columnData[i].split("|")[4];
						}

						if (visible.toUpperCase() == 'TRUE'){
							visible = true;
						}
						else{
							visible = false;
						}

						if (canOrder.toUpperCase() == 'TRUE') {
							canOrder = true;
						}
						else {
							canOrder = false;
						}

						if (viewable.toUpperCase() == 'TRUE') {
							viewable = true;
						}
						else {
							viewable = false;
						}

						columnDataObjects[columnDataObjects.length] = new columnDataObject(columnDBName,columnName, visible, canOrder, groupName, viewable);
					}
				}
			}
			
			function populateTable(type,blnDefault) {
				try{
					var selectedColumnOrder;
					var pTable = '';

					if (type == 'Main') {
						if (opener && opener.getSelectedColumnOrder) {
							selectedColumnOrder = opener.getSelectedColumnOrder(blnDefault);
						}
						else if (parent.getSelectedColumnOrder) {
							selectedColumnOrder = parent.getSelectedColumnOrder(blnDefault);
						}
						pTable = $('#<%=this.tblColumnOrder.ClientID %>')[0];
					}
					else {
						if (opener && opener.getSelectedColumnOrderSub) {
							selectedColumnOrder = opener.getSelectedColumnOrderSub(blnDefault);
						}
						else if (parent.getSelectedColumnOrderSub) {
							selectedColumnOrder = parent.getSelectedColumnOrderSub(blnDefault);
						}
						pTable = $('#<%=this.tblColumnOrderSub.ClientID %>')[0];
					}
					

					var columnData = selectedColumnOrder.split("~");
					popuplateColumnDataObjects(columnData);

					/*while (pTable.rows.length > 0) {
						pTable.deleteRow(tblColumnOrder.rows.length - 1);
					}*/
					$(pTable).empty();

					for (var i = 0; i <= columnDataObjects.length - 1; i++) {
						if (!columnDataObjects[i].isAdded){
							var nRow = pTable.insertRow(pTable.rows.length);
							var nCell = nRow.insertCell(0);
							var nCellLock = nRow.insertCell(1);
							var nDiv = document.createElement('div');
							var checkBox = document.createElement('input');
							var checkBoxLabel = document.createElement('label');
							var isGroup = false;

							nCell.className = 'draggableRow';
							nCellLock.className = 'draggableRow';
							nCellLock.innerHTML = "&nbsp;";

							checkBox.id = 'column_' + i;
							checkBox.name = 'column_' + i;
							checkBox.setAttribute("type", "checkbox");
							checkBox.style.cursor = "default";
							checkBoxLabel.style.cursor = "default";
							checkBox.setAttribute('DBNAME', columnDataObjects[i].columnDBName);
							checkBox.onclick = function () { updateSubFromMain(this); };

							nCell.appendChild(nDiv);
							nDiv.appendChild(checkBox);
							nDiv.appendChild(checkBoxLabel);

							if (columnDataObjects[i].groupName) {
								checkBoxLabel.innerHTML = columnDataObjects[i].groupName;
								checkBox.setAttribute('exclude', true);
								buildGroupedRow(nRow, columnDataObjects[i].groupName);
								isGroup = true;

								nDiv.onmouseover = function () {
									var noDrag = this.parentNode.parentNode.getAttribute('NoDrag');
									if (noDrag) {
										this.parentNode.parentNode.setAttribute('NoDrag', undefined); resetDragDrop();
									}
								};
							}
							else {
								checkBoxLabel.innerHTML = columnDataObjects[i].columnName;
							}

							if (!columnDataObjects[i].canOrder) {
								nRow.setAttribute("NoDrag", true);
								nRow.setAttribute("NoDrop", true);

								if (!isGroup) {
									var imgLock = document.createElement('img');
									imgLock.src = "images/icons/lock.png";
									imgLock.alt = 'Locked';
									imgLock.style.height = '14px';
									imgLock.style.width = '14px';
									nCellLock.appendChild(imgLock);
								}
							}
							else {
								var p = '';
								try { p = parent.location.href } catch (e) { }
								nRow.onmouseup = function () { if (p.indexOf('CrosswalkParamContainer.aspx') != -1) saveOrder(); };
							}
							if (!columnDataObjects[i].viewable) {
								nRow.style.display = "none";
							}

							checkBoxLabel.setAttribute('htmlFor', 'column_' + i);
							checkBox.checked = columnDataObjects[i].visible;
							columnDataObjects[i].isAdded = true;
						}
					}
				}
				catch (e) {
				}
			}

			function buildGroupedRow(row, groupName) {
				try {
					var nDiv = document.createElement('div');
					var nTable = document.createElement('table');
					nTable.style.width = '100%';
					nTable.style.borderCollapse = 'collapse';
					nTable.cellPadding = 0;
					nTable.cellSpacing = 0;

					nDiv.style.marginLeft = '20px';
					nDiv.style.marginTop = '2px';
					nDiv.style.marginBottom = '2px';

					row.cells[0].appendChild(nDiv);
					nDiv.appendChild(nTable);

					for (var i = 0; i <= columnDataObjects.length - 1; i++) {
						if (columnDataObjects[i].groupName == groupName) {
							if (!columnDataObjects[i].isAdded) {
								var nRow = nTable.insertRow(nTable.rows.length);
								var nCell = nRow.insertCell(0);
								var nCellLock = nRow.insertCell(1);
								var checkBox = document.createElement('input');
								var checkBoxLabel = document.createElement('label');

								nCell.className = 'draggableRowChild';
								nCellLock.className = 'draggableRowChildLock';
								nCellLock.style.textAlign = 'right';
								nCellLock.innerHTML = "&nbsp;";

								checkBox.id = 'column_' + groupName + '_' + i;
								checkBox.name = 'column_' + groupName + '_' + i;
								checkBox.setAttribute("type", "checkbox");
								checkBox.onclick = function () { updateSubFromMain(this, groupName); };

								checkBoxLabel.innerHTML = columnDataObjects[i].columnName;
								checkBoxLabel.setAttribute('htmlFor', 'column_' + groupName + '_' + i);

								checkBox.style.cursor = "default";
								checkBoxLabel.style.cursor = "default";

								if (!columnDataObjects[i].canOrder) {
									nRow.setAttribute("NoDrag", true);
									nRow.setAttribute("NoDrop", true);
									var imgLock = document.createElement('img');
									imgLock.src = "images/icons/lock.png";
									imgLock.alt = 'Locked';
									imgLock.style.height = '14px';
									imgLock.style.width = '14px';
									nCellLock.appendChild(imgLock);
								}
								if (!columnDataObjects[i].viewable) {
									nRow.style.display = "none";
								}

								nCell.appendChild(checkBox);
								nCell.appendChild(checkBoxLabel);
								checkBox.checked = columnDataObjects[i].visible;
								columnDataObjects[i].isAdded = true;
								checkBox.setAttribute('DBNAME', columnDataObjects[i].columnDBName);
							}
						}
					}
					nTable.onmouseover = function () { this.parentNode.parentNode.parentNode.setAttribute('NoDrag', true); resetDragDrop(); };
				}
				catch (e) {
				}
			}

			function buildColumnOrder(type) {
				var strColumns = "";
				var checkBoxes;
				if (type == 'Main') {
					checkBoxes = tblColumnOrder.getElementsByTagName('input');
				}
				else {
					checkBoxes = tblColumnOrderSub.getElementsByTagName('input');
				}
				for (var i = 0; i <= checkBoxes.length - 1; i++) {
					if (!checkBoxes[i].getAttribute('exclude')) {
						var columnName = getCheckBoxText(checkBoxes[i], true);
						var checked = checkBoxes[i].checked;
						var DBNAME = checkBoxes[i].getAttribute('dbname');
						if (DBNAME.length > 0) {

							if (strColumns == "") {
								strColumns = DBNAME + '|' + columnName + '|' + checked;
							}
							else {
								strColumns += '~' + DBNAME + '|' + columnName + '|' + checked;
							}
						}
						else {
							if (strColumns == "") {
								strColumns = columnName + '|' + checked;
							}
							else {
								strColumns += '~' + columnName + '|' + checked;
							}
						}
					}
				}
				return strColumns;
			}

			function resetDefault() {
				populateTable('Main', true);
				populateTable('Sub', true);
			}

			function updateSubFromMain(mainCheckBox,groupName) {               
				return;
				if (mainCheckBox) {
					var mainColName = mainCheckBox.nextSibling.innerText;
					//uncheck all the checkbox under that group
					for (var i = 0; i <= tblColumnOrder.rows.length - 1; i++) {
						var subCheckBox;
						if (tblColumnOrder.rows[i]) {
							var groupRowName = tblColumnOrder.rows[i].cells[0].children[0].innerText;
							if (groupRowName == mainColName) {
								var gRows = tblColumnOrder.rows[i].getElementsByTagName('tr');
								for (var y = 0; y <= gRows.length - 1; y++) {
									var gRowName = gRows[y].innerText;
										subCheckBox = gRows[y].getElementsByTagName('input')[0];
										subCheckBox.checked = mainCheckBox.checked;
								}
							}
						}

					}
					for (var i = 0; i <= tblColumnOrderSub.rows.length - 1; i++) {
						var subCheckBox;

						if (groupName) {
							if (tblColumnOrderSub.rows[i]) {
								var groupRowName = tblColumnOrderSub.rows[i].cells[0].children[0].innerText;
								if (groupRowName == groupName) {
									var gRows = tblColumnOrderSub.rows[i].getElementsByTagName('tr');
									for (var y = 0; y <= gRows.length - 1; y++) {
										var gRowName = gRows[y].innerText;
										if ($.trim(gRowName) == $.trim(mainColName)) {
											subCheckBox = gRows[y].getElementsByTagName('input')[0];
										}
									}
								}
							}
						}
						else {
							var subCheckBox = tblColumnOrderSub.rows[i].getElementsByTagName('input')[0];
						}
						if (subCheckBox) {
							var subColName = subCheckBox.nextSibling.innerText;
							if (subColName == mainColName) {
								if (!mainCheckBox.checked) {
									subCheckBox.disabled = true;
								}
								else {
									subCheckBox.disabled = false;
								}
								subCheckBox.checked = mainCheckBox.checked;
								break;
							}
						}
					}
				}
			}

			function Tab_click(tabName) {
				$('#columnOrderer').hide();
				$('#columnOrdererSub').hide();

				switch (tabName.toUpperCase()) {
					case 'MAIN':
						$('#columnOrderer').show();

						break;
					case 'SUB':
						$('#columnOrdererSub').show();

						break;
				}

				resizePage();
			}

			function updateOrder() {
				var newColumnOrder = buildColumnOrder('Main');
				var newColumnOrderSub = buildColumnOrder('Sub');

				if (parent.updateColumnOrder) {
					parent.updateColumnOrder(newColumnOrder, newColumnOrderSub);
				}
			}
			
		</script>
	</form>
	
	<script id="jsInit" type="text/javascript">
		
		$(document).ready(function () {
			try {
				populateTable('Main');
				populateTable('Sub');

				resetDragDrop();

				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: 0
				});

				$('#imgRefresh').click(function () { refreshPage(); });
				$('input:checkbox').change(function () { updateOrder(); });

				if ( (!opener || !opener.getSelectedColumnOrderSub)
					&& (!parent || !parent.getSelectedColumnOrderSub)
					) {
					$("#divTabsContainer").tabs("option", "disabled", [1]);
				}

				if (!opener) {
					$('#btnClose').hide();
				}

				Tab_click('Main');

				resizePage();

				$.each(defaultParentPage.filterBox.filters.find({ groups: { ParentModule: "Work", Module: "Custom" } }), function () {
					if ($(this).attr('name').toUpperCase() == 'ALLOCATION ASSIGNMENT') {
						var chkAllocationAssignment = $("#<%=this.tblColumnOrder.ClientID %> tr:contains('Allocation Assignment')").find("input[type='checkbox']");
						var chkWorkArea = $("#<%=this.tblColumnOrder.ClientID %> tr:contains('Work Area')").find("input[type='checkbox']");

						if ($(this).attr('parameters').length == 1) {
							if ($(chkAllocationAssignment).is(':checked')) $(chkAllocationAssignment).trigger('click');
							if (!$(chkWorkArea).is(':checked')) $(chkWorkArea).trigger('click');
						}
						else {
							if (!$(chkAllocationAssignment).is(':checked')) $(chkAllocationAssignment).trigger('click');
							if ($(chkWorkArea).is(':checked')) $(chkWorkArea).trigger('click');
						}
					}
				});
			} catch (e) {
				var m = e.message;
			}
		});

	</script>
</body>
</html>

