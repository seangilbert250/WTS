﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.master" AutoEventWireup="true" CodeFile="ITI_Settings.aspx.cs" Inherits="ITI_Settings" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server"></asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/cupertino/jquery-ui.css" />
	<link href="Styles/ctxMenu.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	<div id="divTreeview" style="float: left; width: 205px; height: 782px; border-style: solid; padding: 5px 0px; margin-left: 3px; margin-top: 3px; border-width: thin; border-color: lightgray; overflow: scroll;">
		<asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
			<ContentTemplate>
				<asp:Button ID="cmdUpdateTreeView" runat="server" Text="Button" Style="display: none" />
				<asp:TreeView ID="paramTreeView" runat="server" ForeColor="black"></asp:TreeView>
			</ContentTemplate>
		</asp:UpdatePanel>
		<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
			<ProgressTemplate>
				Update in progress...
			</ProgressTemplate>
		</asp:UpdateProgress>
	</div>
	<div style="float: left; max-width: 700px;">
		<div id="divPage" class="pageContainer">
			<asp:HiddenField ID="ddlViewSettings_all" runat="server" />
			<div id="divTabsContainer" class="mainPageContainer">
				<ul id="tabsUl" class="sortable">
					<li id="tabBasic"><a href="#divBasic">Section Order</a></li>
					<li id="tabAdvanced"><a href="#divAdvanced" onclick="Tab_click(this);">Grid View</a></li>
				</ul>
				<div id="divCommon">
					<table id="tableCommon" style="width: 99%; vertical-align: top; text-align: left; padding: 5px; margin-bottom: 5px" cellpadding="0" cellspacing="0">
						<tr>
							<td style="padding: 5px;">
								<img src="Images/menuRight_Black.gif" style="margin-bottom: 2px; margin-right: 5px; cursor: pointer" onclick="toggleGridViewList()" /><span id="viewType">Grid View Name:</span></td>
							<td id="addsubgrid" style="text-align: right">
								<asp:Label ID="Label1" runat="server" Text="Label">Add / Remove SubGrids</asp:Label>
								<img id="imgAddGrid" src="Images/Icons/add_blue.png" style="margin-left: 5px; cursor: pointer" />
								<img id="imgDelGrid" src="Images/Icons/delete.png" style="margin-left: 5px; cursor: pointer" />
							</td>
						</tr>
						<tr>
							<td colspan="2" style="border: 1px solid black; padding: 10px;">
								<asp:DropDownList ID="ddlView" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false" />
								<img id="imgSaveView" src="Images/Icons/disk.png" title="Save View" alt="Save View" style="cursor: pointer;" />
								<img id="imgDeleteView" src="Images/Icons/delete.png" title="Delete View" alt="Delete View" style="cursor: pointer;" />
								<div id="newProperty" style="float: right; margin-right: 7px; cursor: pointer; font-weight: bold">
									<button type="button" style="cursor: pointer" onclick="addProperty()">New Property</button>
								</div>
							</td>
						</tr>
					</table>
					<div>
						<div id="propFilters" class="filters">
							<img id='imgHelp' title='Drag and drop to re-order properties.<br/>Right click properties for additional functions.' src='Images/Icons/help.png' style="padding: 5px; padding-top: 5px; float: left; margin-left: 5px" />
							<span style="padding: 5px; padding-top: 5px; float: left;"><< Filter by text or selections >><input id="filterText" type="text" style="width: 75px; height: 10px; float: left; font-size: xx-small; vertical-align: middle; margin-right: 5px" /></span>
						</div>
					</div>
				</div>
				<div id="divBasic">
					<table id="tableBasic" style="width: 99%; vertical-align: top; text-align: left; padding: 5px;" cellpadding="0" cellspacing="0">
						<tr>
							<td style="border: 1px solid black; padding: 5px;">
								<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px 5px 0px;">
									<tr class="attributesRow">
										<td class="attributesValue">
											<ul id="sortable" class="sortable"></ul>
										</td>
									</tr>
								</table>
							</td>
						</tr>
						<tr>
							<td colspan="2" style="text-align: right; padding-top: 10px;">
								<input id="btnClose" type="button" value="Get Data" />
							</td>
						</tr>
					</table>
				</div>
				<div id="divAdvanced" style="padding-bottom: 10px">
					<div id="divAdvancedScroll" style="overflow-y: scroll; height: 625px;">
						<table id="tableAdvanced" style="width: 99%; vertical-align: top; text-align: left; padding: 5px;" cellpadding="0" cellspacing="0">
							<tr>
								<td style="border: 1px solid black; padding: 5px;">
									<table id="tableAttributesAdv" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px 5px 0px;">
										<tr class="attributesRow">
											<td class="attributesValue">
												<ul id="sortableAdv" class="sortable"></ul>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</div>
					<div style="float: left; margin-left: 18px; margin-top: 10px">
						<input type="button" value="Clear All" onclick="selectAllProps(this)" /></div>
					<div id="divBtnClose" style="text-align: right; margin: 10px; margin-right: 18px">
						<input id="btnCloseAdv" type="button" value="Get Data" />
					</div>
				</div>
			</div>
		</div>
		<div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
		<div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
			<table style="width: 100%;">
				<tr>
					<td class="pageContentInfo">
						<span id="svFrmName">Grid View Name:</span>
					</td>
				</tr>
				<tr>
					<td>
						<select id="ddlSaveView" style="width: 255px;"></select>
					</td>
				</tr>
				<tr id="trViewName">
					<td>
						<asp:TextBox ID="txtViewName" runat="server" MaxLength="50" Width="250"></asp:TextBox>
					</td>
				</tr>
				<tr>
					<td>
						<input type="checkbox" id="chkProcessView" style="vertical-align: middle;" />
						<label for="chkProcessView" style="vertical-align: middle;">Process View</label>
					</td>
				</tr>
				<tr>
					<td>
						<input type="button" id="buttonSaveView" value="Save" /><input type="button" id="buttonCancelView" value="Cancel" style="margin-left: 5px" />
					</td>
				</tr>
			</table>
		</div>
		<div id="dialog-confirm" title="Delete Current Record?" style="display: none">
			<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>This record will be permanently deleted and cannot be recovered. Are you sure?</p>
		</div>
		<div id="mnuCntnr">
			<ul id="ctxMenu">
				<li class="ctxMnuGroups">
					<div>Add to Group</div>
					<ul id="colGroups">
						<li></li>
						<li>
							<div onclick="addColToGroup()">New Group</div>
						</li>
					</ul>
				</li>
				<li class="ctxMnuRemoveGroups" style="display: none">
					<div>Remove a Group</div>
					<ul id="remColGroups">
						<li>
							<div onclick="removeGroup('all')">Remove Group</div>
						</li>
					</ul>
				</li>
				<li class="ctxMnuRemoveGroups" id="ctxRemoveFromGroup" style="display: none">
					<div onclick="removeFromGroup()">Remove from Group</div>
				</li>
				<li class="ctxMnuRemoveGroups" id="ctxRenameSpacer"></li>
				<li class="ctxMnuRemoveGroups" id="ctxRenameGroup" style="display: none">
					<div onclick="renameGroup()">Edit Group Name</div>
				</li>
				<li class="ctxMnuCatCols" style="display: none">
					<div>Edit Concatenated Columns</div>
					<ul id="concatCols">
						<li>
							<div onclick="editConcatColumns()">Edit Columns</div>
						</li>
					</ul>
				</li>
				<li class="ctxMnuCatCols" style="display: none">
					<div onclick="deleteConcatColumn()">Delete Concatenated Column</div>
				</li>
				<li class="ctxAddColAlias"></li>
				<li class="ctxAddColAlias" onclick="editColumnName()">Edit Column Name</li>
				<li class="ctxAddColAlias">
					<div id="cntIndicator" onclick="toggleChildRowCount()">Hide / show child row count</div>
				</li>
				<li class="ctxGvHeader"></li>
				<li class="ctxGvHeader">
					<div onclick="toggleGridViewHeader()">Hide / show gridview header</div>
				</li>
			</ul>
		</div>
	</div>
	<script>
		var chkArray;
		var listArray;
		var colArray;
		var subGridCount;
		var viewChng = false;

		$("#sortable").sortable({
			cursor: "move",
			stop: function (event, ui) {
				var data = JSON.parse(getDefaultPageItisettings());
				data.sectionorder = $("#sortable").sortable("toArray");
				setDefaultPageItisettings(JSON.stringify(data));
				UpdateTreeView();
			}
		});

		$("#sortableAdv").sortable({
			cursor: "move",
			stop: function (event, ui) {
				var itisettings = JSON.parse(getDefaultPageItisettings());
				itisettings.columnorder = $("#sortableAdv").sortable("toArray");
				setDefaultPageItisettings(JSON.stringify(itisettings))
				updateTabText($("#divTabsContainer .ui-tabs-panel:visible").attr("id"));
				UpdateTreeView();
			}
		});

		function setDefaultPageItisettings(paramItiSettings) {
			//Chrome Bug Fix- when setting parentPage itisettings the DDL selectedItem is cleared
			var selectedDDLItem = $('#ctl00_ContentPlaceHolderBody_ddlView').val();
			$(defaultParentPage.itisettings).text(paramItiSettings);
			$('#ctl00_ContentPlaceHolderBody_ddlView').val(selectedDDLItem);


		}

		function getDefaultPageItisettings() {
			return $(defaultParentPage.itisettings).text();
		}

		function UpdateTreeView() {
			$('#<%= cmdUpdateTreeView.ClientID %>').trigger("click");
		}

		function Treeview_click(args) {
			alert("You clicked me!");
		}

		function imgSaveView_click(obj) {
			$('#divDimmer').show();
			var pos = $(obj).position();

			$('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true).change();
			$('#trViewName').show();
			$('#chkProcessView').prop('checked', false);

			$('#divViewName').css({
				position: "fixed",
				top: pos.top + "px",
				left: $(this).outerWidth() / 4 + "px"
			}).slideDown(function () { $('#<%=txtViewName.ClientID %>').focus(); });
		}

		function toggleGridViewList() {
			var ddlViewSettings_all_json = JSON.parse($("#<%= ddlViewSettings_all.ClientID %>").val());

			$('#<%=ddlView.ClientID %>').empty();

			if ($("#viewType").text() === "Grid View Name:") {
				$("#viewType").text("Tab View Name:");
				$("#svFrmName").text("Tab View Name:");
				var firstOption = $('<option>', { value: 0, html: "Select one...", style: "font-style: italic" });
				$(firstOption).attr('OptionGroup', "Process Views");
				$('#<%=ddlView.ClientID %>').append(firstOption);
			} else {
				$("#viewType").text("Grid View Name:");
				$("#svFrmName").text("Grid View Name:");
			}

			$.each(ddlViewSettings_all_json, function (val, text) {
				if ($("#viewType").text() === "Grid View Name:")
					if (text.ViewType === 1) {
						var newOption = $('<option>', { value: text.GridViewID, html: text.ViewName });
						$(newOption).attr('OptionGroup', (text.WTS_RESOURCEID != null ? "Custom Views" : "Process Views"));
						$(newOption).attr('ViewType', text.ViewType);
						$(newOption).attr('MyView', text.MyView);
						$('#<%=ddlView.ClientID %>').append(newOption);
					}

				if ($("#viewType").text() === "Tab View Name:")
					if (text.ViewType === 2) {
						var newOption = $('<option>', { value: text.GridViewID, html: text.ViewName });
						$(newOption).attr('OptionGroup', (text.WTS_RESOURCEID != null ? "Custom Views" : "Process Views"));
						$(newOption).attr('ViewType', text.ViewType);
						$(newOption).attr('MyView', text.MyView);
						$('#<%=ddlView.ClientID %>').append(newOption);
					}
			});

			$("#<%=this.ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
			$("#<%=this.ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
		}

		function buttonSaveView_click() {
			var $opt = $('#ddlSaveView option:selected');

			if ($opt.text() === '--Create New--') {
				var viewName = $('#<%=txtViewName.ClientID %>').val().trim().toUpperCase();
				if (viewName === 'DEFAULT') {
					MessageBox('You cannot save with grid view name Default.');
				}
				else if (viewName !== '') {
					var exists =
						$('#<%=ddlView.ClientID %> option').filter(function () {
							return $(this).text().trim().toUpperCase() === viewName;
						}).length > 0;

					if (!exists) {
						confirmViewName('YES');
					}
					else {
						var myView = $('#<%=ddlView.ClientID %> option').filter(function () {
							return $(this).text().trim().toUpperCase() === viewName;
						}).first().attr('MyView');

						if (myView === '1') {
							QuestionBox('Confirm View Name', 'View name already exists. Would you like to overwrite?', 'Yes,No', 'confirmViewName', 300, 300, window.self);
						}
						else {
							MessageBox('View name already exists. You cannot overwrite view name which you did not create.');
						}
					}
				}
				else {
					MessageBox('Please enter a view name.');
				}
			}
			else if ($opt.text().toUpperCase() == 'DEFAULT') {
				MessageBox('You cannot save with grid view name Default.');
			}
			else {
				var myView = $opt.attr('MyView');

				if (myView === '1') {
					//QuestionBox('Confirm View Name', 'View name already exists. Would you like to overwrite?', 'Yes,No', 'confirmViewName', 300, 300, window.self);
					confirmViewName('YES');
				}
				else {
					MessageBox('You cannot overwrite view name which you did not create.');
				}
			}
		}

		function btnClose_click() {
			if (closeWindow) {
				opener.refreshPage();
				closeWindow();
			}
			else {
				window.close();
			}
		}

		function selectAllProps(args) {
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = args.parentElement.parentElement.id;
			var itemList = $("#" + subgrid + " li:visible");

			$.each(itemList, function (s, e) {
				$(itemList[s]).find("input[type='checkbox']").prop('checked', false);
				var itemName = $(itemList[s]).find("input[type='checkbox']").attr('id').substring(3);

				if (subgrid === "divAdvanced") {
					$.each(data.tblCols, function (i, v) {
						if (itemName === v.name) {
							v.show = false;
							v.sortorder = "none";
							return;
						}
					});
				} else {
					$.each(data, function (i, v) {
						if (i === subgrid) {
							$.each(v[0].tblCols, function (item, val) {
								if (itemName === val.name) {
									val.show = false;
									val.sortorder = "none";
									return;
								}
							});
						}
					});
				}

				$(itemList[s]).find(".rdospan").fadeTo(0, 0.33);
				$(itemList[s]).find($("[id='rdo" + itemName + "N']")).prop('checked', true);
				$(itemList[s]).find($('input.spinner:text')).prop("disabled", true).val("");
			});

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function ddlView_change() {
			var ddlViewSettings = JSON.parse($("#<%= ddlViewSettings_all.ClientID %>").val());

			viewChng = true;
			$.each(ddlViewSettings, function (i, v) {
				if (v.ViewName === $("#<%=ddlView.ClientID %> option:selected").text())
					PageMethods.GetTier1Data(v.GridViewID, ddlView_Change_Done, on_error);
			});
		}

		function ddlView_Change_Done(results) {
			var data = JSON.parse(results);
			if ($("#viewType").text() === "Grid View Name:") {
				for (var i = 0; i < data.sectionorder.length; i++) $('#sortable').append(listArray[data.sectionorder[i] - 1]);
				for (var j = 0; j < chkArray.length; j++) chkArray[j].checked = data.sectionexpanded[chkArray[j].id];

				setDefaultPageItisettings(JSON.stringify(data));

				if (parseInt('<%=_activeTab %>') > 0) {
					$("#sortableAdv").empty();
					removeAllSubgrids();
					recreateSubGrids();
					buildSortable(1);
					colArray = $('#sortableAdv li');
					for (var l = 0; l < data.columnorder.length; l++) {
						$('#sortableAdv').append(colArray[data.columnorder[l] - 1]);
					}
					var tabName = $("#sortableAdv > li > label").first().html();
					$("#tabsUl #tabAdvanced > a").html("Lvl 1 - " + tabName);
					$("#divTabsContainer").tabs({ active: 1 });
				} else {
					buildSortable(0);
					for (var k = 0; k < chkArray.length; k++) {
						chkArray[k].checked = data.sectionexpanded[chkArray[k].id];
					}
				}
				resetCtxMenu();
				UpdateTreeView();
			} else {
				var d = JSON.parse(getDefaultPageItisettings());
				var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

				if (subgrid === "divAdvanced") {
					d.tblCols = data[0].tblCols;
					d.columnorder = data[0].columnorder;
					d.showcolumnheader = data[0].showcolumnheader;
				} else {
					$.each(d, function (i, v) {
						if (i === subgrid) {
							v[0].tblCols = data[0].tblCols;
							v[0].columnorder = data[0].columnorder;
							v[0].showcolumnheader = data[0].showcolumnheader;
							return;
						}
					});
				}

				setDefaultPageItisettings(JSON.stringify(d));

				if (subgrid === "divAdvanced") buildSortable(1);
				else popTplSubGrid(subgrid);

				if (subgrid === "divAdvanced") {
					var newColArray = $('#sortableAdv > li');
					$.each(data[0].tblCols, function (x, s) {
						$("#sortableAdv > li > input[id*= 'chk" + s.name + "']").prop('checked', s.show);
					});

					for (var j = 0; j < data[0].columnorder.length; j++)
						$('#sortableAdv').append(newColArray[data[0].columnorder[j] - 1]);
				} else {
					var newColArray2 = $('#' + subgrid + 'Ul > li');
					$.each(data[0].tblCols, function (x, s) {
						$("#" + subgrid + "Ul > li > input[id*= 'chk" + s.name + "']").prop('checked', s.show);
					});

					for (var r = 0; r < data[0].columnorder.length; r++)
						$('#' + subgrid + 'Ul').append(newColArray2[data[0].columnorder[r] - 1]);
				}

				updateTabText(subgrid);
				if (subgrid === "divAdvanced") {
					$.each(data[0].tblCols, function (i, v) {
						if (v.sortorder === "asc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "A']").prop('checked', true);
						if (v.sortorder === "desc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "D']").prop('checked', true);
						if (v.sortorder === "none") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "N']").prop('checked', true);
					});
				} else {
					$.each(data[0].tblCols, function (index, value) {
						if (value.sortorder === "asc") $("#" + subgrid + "Ul > li > span > input[id*= 'rdo" + value.name + "A']").prop('checked', true);
						if (value.sortorder === "desc") $("#" + subgrid + "Ul > li > span > input[id*= 'rdo" + value.name + "D']").prop('checked', true);
						if (value.sortorder === "none") $("#" + subgrid + "Ul > li > span > input[id*= 'rdo" + value.name + "N']").prop('checked', true);
					});
				}
			}
			resizePage(parseInt('<%=_activeTab %>'));
		}

		function ddlSaveView_change() {
			var $opt = $('#ddlSaveView option:selected');

			if ($opt.text() != '--Create New--') {
				$('#trViewName').hide();
				$('#chkProcessView').prop('checked', $opt.attr('OptionGroup') == 'Process Views');
			}
			else {
				$('#<%=txtViewName.ClientID %>').val('');
				$('#trViewName').show();
				$('#chkProcessView').prop('checked', false);
			}
		}

		function setColumnWidth(args) {
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");
			if (subgrid === null) return;
			var data = JSON.parse(getDefaultPageItisettings());

			if (args.value === "Hide") args.value = "Show";
			else args.value = "Hide";

			if (subgrid.indexOf("subgrid") === -1) {
				if (data.sectionexpanded[args.id] !== undefined && subgrid !== "divAdvanced") {
					data.sectionexpanded[args.id] = args.checked;
				} else {
					$.each(data.tblCols, function (i, v) {
						if (args.id.substring(3) === v.name) {
							if (args.value === "Show") v.columnwidth = 0;
							else v.columnwidth = 1;
							return;
						}
					});
				}
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (args.id.substring(3) === v.name) {
								if (args.value === "Show") v.columnwidth = 0;
								else v.columnwidth = 1;
								return;
							}
						});
					}
				});
			}

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function setSectionExpanded(args) {
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");
			if (subgrid === null) return;
			var data = JSON.parse(getDefaultPageItisettings());
			var origValue;

			if (subgrid.indexOf("subgrid") === -1) {
				if (data.sectionexpanded[args.id] !== undefined && subgrid !== "divAdvanced") {
					data.sectionexpanded[args.id] = args.checked;
				} else {
					$.each(data.tblCols, function (i, v) {
						if (args.id.substring(3) === v.name) {
							v.show = args.checked;
							if (args.checked === false) {
								v.sortorder = "none";
								v.sortpriority = "";
							}
							return;
						}
					});
				}
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (args.id.substring(3) === v.name) {
								v.show = args.checked;
								if (args.checked === false) {
									v.sortorder = "none";
									v.sortpriority = "";
								}
								return;
							}
						});
					}
				});
			}

			if (args.checked && subgrid !== "divBasic") {
				$('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find(".rdospan").fadeTo(50, 1);
				$('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().insertBefore($('#' + subgrid + " li > input[type='checkbox']").not(":checked").first().parent());

				if (subgrid !== "divAdvanced") {
					$.each(data, function (i, v) {
						if (i === subgrid) v[0].columnorder = $("#" + subgrid + "Ul").sortable("toArray");
					});
				} else data.columnorder = $('#sortableAdv').sortable("toArray");

				setDefaultPageItisettings(JSON.stringify(data));
			} else {
				if (subgrid !== "divAdvanced") {
					$('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find(".rdospan").fadeTo(75, 0.33);
					$('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($("[id='rdo" + args.id.substring(3) + "N']")).prop('checked', 'checked');
					origValue = $('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();
					$('#' + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", true).val("");
				} else {
					$("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find(".rdospan").fadeTo(75, 0.33);
					$("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($("[id='rdo" + args.id.substring(3) + "N']")).prop('checked', 'checked');
					origValue = $("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();
					$("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", true).val("");
				}
				setDefaultPageItisettings(JSON.stringify(data));
				correctPriorityOrder(subgrid, args.id, origValue);
			}

			updateTabText(subgrid);
			UpdateTreeView();
		}

		function setSortOrder(args) {
			var subgrid = args.getAttribute('parentGrid');
			var data = JSON.parse(getDefaultPageItisettings());
			var spinnerCount = getSpinnerCount(subgrid);
			var origValue = 0;

			if (subgrid.indexOf("subgrid") === -1) {
				$.each(data.tblCols, function (i, v) {
					if (args.id.indexOf(v.name) !== -1) {
						if (args.id.slice(-1) !== "N")
							if (v.sortorder === "none") v.sortpriority = parseInt(spinnerCount) + 1;
						if (args.id.slice(-1) === "A") v.sortorder = "asc";
						if (args.id.slice(-1) === "D") v.sortorder = "desc";
						if (args.id.slice(-1) === "N") {
							v.sortorder = "none";
							v.sortpriority = "";
							origValue = $("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();
							$("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", true).val("");
							setDefaultPageItisettings(JSON.stringify(data));
							correctPriorityOrder(subgrid, args.id, origValue);
							return;
						} else {
							if ($("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val() === "")
								$("#sortableAdv [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", false).val(parseInt(spinnerCount) + 1);

							setDefaultPageItisettings(JSON.stringify(data));
						}
						return;
					}
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (args.id.indexOf(v.name) !== -1) {
								if (args.id.slice(-1) !== "N")
									if (v.sortorder === "none") v.sortpriority = parseInt(spinnerCount) + 1;
								if (args.id.slice(-1) === "A") v.sortorder = "asc";
								if (args.id.slice(-1) === "D") v.sortorder = "desc";
								if (args.id.slice(-1) === "N") {
									v.sortorder = "none";
									v.sortpriority = "";
									origValue = $("#" + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();
									$("#" + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", true).val("");
									setDefaultPageItisettings(JSON.stringify(data));
									correctPriorityOrder(subgrid, args.id, origValue);
									return;
								} else {
									if ($("#" + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val() === "")
										$("#" + subgrid + " [id='" + args.id.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).prop("disabled", false).val(parseInt(spinnerCount) + 1);

									setDefaultPageItisettings(JSON.stringify(data));
								}
								return;
							}
						});
					}
				});
			}
		}

		function reOrderSort(args) {
			var origSpr = "";
			var origVal = "0";
			var subgrid = $(args).closest("ul").attr('id').toString();
			var spr = $(args).siblings("input").attr('id');
			var sprVal = $(args).siblings("input").val();
			var data = JSON.parse(getDefaultPageItisettings());

			if (subgrid.indexOf("subgrid") === -1)
				$.each(data.tblCols, function (i, v) { if (spr.substring(3).indexOf(v.name) !== -1) origVal = v.sortpriority; });
			else
				$.each(data, function (i, v) {
					if (i + 'Ul' === subgrid)
						$.each(v[0].tblCols, function (i, v) {
							if (spr.substring(3).indexOf(v.name) !== -1) origVal = v.sortpriority;
						});
				});

			$.each($('#' + subgrid + ' li span span input'), function (i, v) {
				if (parseInt(v.value) > 0)
					if (parseInt(sprVal) === parseInt(v.value) && spr.toString() !== v.id.toString()) {
						v.value = origVal;
						origSpr = v.id;
					}
			});

			if (subgrid.indexOf("subgrid") === -1) {
				$.each(data.tblCols, function (i, v) {
					if (spr.substring(3).indexOf(v.name) !== -1) v.sortpriority = sprVal;
					if (origSpr.substring(3).indexOf(v.name) !== -1) v.sortpriority = origVal;
				});
			} else {
				$.each(data, function (i, v) {
					if (i + 'Ul' === subgrid)
						$.each(v[0].tblCols, function (i, v) {
							if (spr.substring(3).indexOf(v.name) !== -1) v.sortpriority = sprVal;
							if (origSpr.substring(3).indexOf(v.name) !== -1) v.sortpriority = origVal;
						});
				});
			}

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function correctPriorityOrder(subgrid, element, origValue) {
			var spr = "spr" + element.substring(3);
			var sprVal = $("#" + subgrid + " [id='" + spr.replace("#", "\\#") + "']").siblings("input").val() ? 0 : origValue;
			var data = JSON.parse(getDefaultPageItisettings());

			$.each($('#' + subgrid + ' li span span input'), function (i, v) {
				if (parseInt(v.value) > parseInt(sprVal)) v.value = parseInt(v.value) - 1;
			});

			if (subgrid.indexOf("subgrid") === -1) {
				$.each(data.tblCols, function (i, v) {
					spr = "spr" + v.name;
					sprVal = $("#" + subgrid + " [id='" + spr.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();

					if (sprVal !== undefined && parseInt(sprVal) > 0) v.sortpriority = sprVal;
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							spr = "spr" + v.name;
							sprVal = $("#" + subgrid + " [id='" + spr.replace("#", "\\#") + "']").parent().find($('input.spinner:text')).val();

							if (sprVal !== undefined && parseInt(sprVal) > 0) v.sortpriority = sprVal;
						});
					}
				});
			}

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function getSpinnerCount(args) {
			var count = 0;

			if (args.indexOf("subgrid") === -1) {
				$.each($('#sortableAdv li span span input'), function (i, v) {
					if (parseInt(v.value) > parseInt(count)) count = v.value;
				});
			} else {
				$.each($('#' + args + ' li span span input'), function (i, v) {
					if (parseInt(v.value) > parseInt(count)) count = v.value;
				});
			}

			return (parseInt(count));
		}

		function recreateSubGrids() {
			var data = JSON.parse(getDefaultPageItisettings());

			$.each(data, function (i, v) {
				if (i.indexOf("subgrid") !== -1) {
					addSubGrid(i);
					var newColArray = $('#' + i + ' li');

					for (var j = 0; j < v[0].columnorder.length; j++)
						$('#' + i + 'Ul').append(newColArray[v[0].columnorder[j] - 1]);

					updateTabText(i);
					$.each(v[0].tblCols, function (x, s) {
						$("#" + i + "Ul > li > input[id*= 'chk" + s.name + "']").prop('checked', s.show);
					});
				}
			});
		}

		function addSubGrid(args) {
			var panelId = args || "subgrid" + subGridCount;
			var newPanel = $('<div>', { "id": panelId, html: $("#tmplSubGrid").html() });
			var newAnchor = $('<a>', { href: "#subgrid" + subGridCount, onclick: "Tab_click(this);", html: "Lvl " + ($('#tabsUl li > a').length) + " - " + JSON.parse(getDefaultPageItisettings()).tblCols[0].name });
			var newSpan = $('<span>', { "class": "ui-icon ui-icon-close", html: "Remove Tab" });
			var newListItem = $('<li>', { "id": "subGrid" + subGridCount, "class": "ui-tabs" });

			$(newListItem).append(newAnchor);
			$(newListItem).append(newSpan);
			$(newListItem).on("click", 'span.ui-icon-close', function () { removeTab($(this).parent().attr('id')); resizePage(parseInt('<%=_activeTab %>')); });

			$('#divTabsContainer').tabs().find(".ui-tabs-nav").append(newListItem);
			$('#divTabsContainer').append(newPanel);
			$('#divTabsContainer').tabs("refresh");
			$("#divTabsContainer").tabs({ active: $('#tabsUl li').length - 1 });

			if (args === undefined) {
				updateJson(panelId);
				popTplSubGrid(panelId);
			} else popTplSubGridR1(panelId);

			subGridCount++;
		}

		function deleteSubGrid() {
			removeTab($("#divTabsContainer .ui-tabs-panel:visible").attr("id"));
		}

		function removeTab(tabId) {
			var origData = JSON.parse(getDefaultPageItisettings());
			var data = JSON.parse(getDefaultPageItisettings());
			var hrefStr = "a[href='#" + tabId.toLowerCase() + "']";
			var beginCount = false;
			var sgList = [];

			$(hrefStr).closest("li").remove();
			$('#' + tabId.toLowerCase() + '').remove();
			$('#' + tabId.toLowerCase() + 'Ul_code').remove();
			$("#divTabsContainer").tabs("refresh");

			delete data[tabId.toLowerCase()];
			setDefaultPageItisettings(JSON.stringify(data));

			$.each(data, function (i, v) {
				if (i.indexOf("subgrid") !== -1) sgList.push(i);
			});

			if (sgList.length > 0)
				$.each(origData, function (i, v) {
					if (i === tabId.toLowerCase()) {
						beginCount = true;
						sgList = [];
					}
					if (beginCount && i.indexOf("subgrid") !== -1 && i !== tabId.toLowerCase()) sgList.push(i);
				});

			if (sgList.length > 0) {
				var sName = tabId.toLowerCase();
				$.each(sgList, function (i, v) {
					var strData = getDefaultPageItisettings().replace(v, sName);
					setDefaultPageItisettings(strData);
					sName = v;
				});

				removeAllSubgrids();
				recreateSubGrids();
			}

			subGridCount = $('#tabsUl li').length + 1;
			Tab_click(tabId);
			UpdateTreeView();
		}

		function removeAllSubgrids() {
			$('#divTabsContainer ul li[id^="subGrid"]').remove();
			$('#divTabsContainer div[id^="subgrid"]').remove();
			$('script[id^="subgrid"]').remove();
			$('#divTabsContainer').tabs("refresh");
			subGridCount = $('#tabsUl li').length + 1;
			$("#divTabsContainer").tabs({ active: $('#tabsUl li').length - 1 });
		}

		function updateJson(args) {
			var data = JSON.parse(getDefaultPageItisettings());
			var tempData = "{\"" + args + "\":[{\"tblCols\":" + JSON.stringify(data.tblCols) + ",\"columnorder\":" + JSON.stringify(data.columnorder) + "}]}";

			$.extend(data, JSON.parse(tempData));
			setDefaultPageItisettings(JSON.stringify(data));
			data = JSON.parse(getDefaultPageItisettings());

			$.each(data, function (i, v) {
				if (i === args) {
					var defaultColumnOrder = [];
					$.each(v[0].tblCols,
						function (i, v) {
							v.show = false;
							v.sortorder = "none";
							v.sortpriority = "";
							v.groupname = "";
							v.concat = false;
							v.alias = "";
						}
					);
					for (i = 0; i < colArray.length; i++) {
						defaultColumnOrder.push((i + 1).toString());
					}
					v[0].columnorder = defaultColumnOrder;
					v[0].showcolumnheader = true;
				}
			});

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function initEvents() {
			var data = JSON.parse(getDefaultPageItisettings());

			for (var i = 0; i < data.sectionorder.length; i++) {
				$('#sortable').append(listArray[data.sectionorder[i] - 1]);
			}
			for (var k = 0; k < chkArray.length; k++) {
				chkArray[k].checked = data.sectionexpanded[chkArray[k].id];
			}
			for (var j = 0; j < data.columnorder.length; j++) {
				$('#sortableAdv').append(colArray[data.columnorder[j] - 1]);
			}

			var tabName = $("#sortableAdv > li > label").first().html();
			$("#tabsUl #tabAdvanced > a").html("Lvl 1 - " + tabName);
			recreateSubGrids();

			$.each(data.tblCols, function (i, v) {
				if (v.sortorder === "asc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "A']").prop('checked', true);
				if (v.sortorder === "desc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "D']").prop('checked', true);
				if (v.sortorder === "none") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "N']").prop('checked', true);
			});

			if (parseInt('<%=_activeTab %>') !== 0) {
				if (data.columngroups != null) {
					$.each(data.columngroups,
						function (i, v) {
							var groupColor;

							if (v === 'AOR') groupColor = "Red";
							else if (v === 'CR') groupColor = "Blue";
							else if (v === 'SR') groupColor = "Green";
							else if (v === 'Task') groupColor = "Orange";
							else groupColor = "Black";

							$('#propFilters').append($('<label>', { for: "Checkbox" + (i + 1), html: v, style: "color: " + groupColor }));
							$('#propFilters').append($('<input>',
								{
									name: "propFilter",
									id: "Checkbox" + (i + 1),
									type: "checkbox",
									value: v,
									style: "cursor: pointer",
									checked: "checked"
								}));
						});
				} else alert("You have an older gridview file that should be replaced or updated.");
				$('#propFilters').append($('<div>', { style: "padding-top: 5px; margin-right: 30px;" }));
			} else {
				$('#propFilters').empty();
				$('#propFilters').append("<img id='imgHelp' title='Select sections to be expanded.<br/>Drag and drop to re-order.' src='Images/Icons/help.png' style='padding: 5px; padding - top: 5px; float: left'/>");
				$('#propFilters').append($('<span>', { html: "Select sections to be expanded.", style: "padding: 5px; padding-top: 5px; float: left" }));
			}

			$('#btnClose, #btnCloseAdv').click(function () { btnClose_click(); return false; });
			$('#imgSaveView').click(function () { imgSaveView_click(this); });
			$('#buttonSaveView').click(function () { buttonSaveView_click(); return false; });
			$('#buttonCancelView').click(function () { $('#divViewName').slideUp(function () { $('#divDimmer').hide(); }); return false; });
			$('#<%=ddlView.ClientID %>').on("change", function () { ddlView_change(); return false; });
			$('#imgDeleteView').click(function () { imgDeleteView_click(); });
			$('#imgAddGrid').click(function () { addSubGrid(); UpdateTreeView(); resizePage(parseInt('<%=_activeTab %>')); });
			$('#imgDelGrid').click(function () { deleteSubGrid(); resizePage(parseInt('<%=_activeTab %>')); });
			$('#<%=ddlView.ClientID %> option').filter(function () { return $.trim($(this).text()) === data.viewname; }).prop('selected', true);
			$('#ddlSaveView').on('change', function () { ddlSaveView_change(); });
		}

		function confirmViewName(answer) {
			if ($.trim(answer).toUpperCase() === 'YES') {
				var viewType = 1;
				var gridViewID = 0;
				var viewName = '';
				var $opt = $('#ddlSaveView option:selected');

				if ($opt.text() != '--Create New--') {
					gridViewID = $opt.val();
					viewName = $('#ddlSaveView option:selected').text();
				}
				else {
					viewName = $('#<%=txtViewName.ClientID %>').val().trim();

					//Search for existing viewName in ddl. If it exists update the current view
					var exists = $('#ddlSaveView option').filter(function () {
						return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
					}).length > 0;

					if (exists) {
						gridViewID = $('#<%=ddlView.ClientID %> option').filter(function () {
							return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
						}).val();
					}
				}

				try {
					var data = JSON.parse(getDefaultPageItisettings());
					data.viewname = viewName;//$('#<%=txtViewName.ClientID %>').val().trim();
					setDefaultPageItisettings(JSON.stringify(data));
					var settings = getDefaultPageItisettings();

					if ($("#viewType").text() === "Tab View Name:") {
						var data = JSON.parse(getDefaultPageItisettings());
						var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

						viewType = 2;
						if (subgrid === "divAdvanced") {
							settings = "[{\"tblCols\":" + JSON.stringify(data.tblCols) + ",\"columnorder\":" + JSON.stringify(data.columnorder) + ",\"showcolumnheader\":" + JSON.stringify(data.showcolumnheader) + "}]";
						} else {
							$.each(data, function (i, v) {
								if (i === subgrid) {
									settings = JSON.stringify(v);
									return;
								}
							});
						}
					}

					PageMethods.SaveView(gridViewID, viewName, ($('#chkProcessView').is(':checked') ? 1 : 0), settings, viewType, btnSaveView_Done, on_error);

				} catch (e) {
					alert('Error in Save View. ' + e.message);
				}
			}
		}

		function on_error() {
			if (result.indexOf('True') > 0) MessageBox('Saved');
			else MessageBox('Error saving:' + result);
		}

		function btnSaveView_Done(result) {
			if (JSON.parse(result).saved === true) {
				var data = JSON.parse(getDefaultPageItisettings());
				MessageBox('Saved');
				$('#divDimmer').hide();
				$('#divViewName').hide();

				$('#<%=ddlView.ClientID %>').empty();

				var newOption = $('<option>', { html: "Customized View" });
				$('#<%=ddlView.ClientID %>').append(newOption);

				$.each(JSON.parse(result).ddlItems, function (val, text) {
					if ($("#viewType").text() === "Grid View Name:")
						if (text.ViewType === 1) {
							var newOption = $('<option>', { value: text.GridViewID, html: text.ViewName });
							$(newOption).attr('OptionGroup', (text.WTS_RESOURCEID != null ? "Custom Views" : "Process Views"));
							$(newOption).attr('ViewType', text.ViewType);
							$(newOption).attr('MyView', text.MyView);
							$('#<%=ddlView.ClientID %>').append(newOption);
						}

					if ($("#viewType").text() === "Tab View Name:")
						if (text.ViewType === 2) {
							var newOption = $('<option>', { value: text.GridViewID, html: text.ViewName });
							$(newOption).attr('OptionGroup', (text.WTS_RESOURCEID != null ? "Custom Views" : "Process Views"));
							$(newOption).attr('ViewType', text.ViewType);
							$(newOption).attr('MyView', text.MyView);
							$('#<%=ddlView.ClientID %>').append(newOption);
						}
				});

				$('#<%=ddlView.ClientID %> option').filter(function () {
					return ($(this).text() === data.viewname);
				}).prop('selected', true);

				$('#<%=ddlViewSettings_all.ClientID %>').val(JSON.stringify(JSON.parse(result).ddlItems));
				$("#<%=this.ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
				$("#<%=this.ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");

				buildSaveView();
			}
			else MessageBox('Error saving:' + JSON.parse(result).error);
		}

		function imgDeleteView_click() {
			var gv = $.trim($('#<%=ddlView.ClientID %> option:selected').text()).toUpperCase();

			if (gv === "DEFAULT" || gv === "-- NEW GRIDVIEW --") {
				MessageBox('You cannot delete this grid view.');
			} else {
				$("#dialog-confirm").dialog({
					resizable: false,
					height: "auto",
					width: 400,
					modal: true,
					buttons: {
                        "Yes": function () {
							if (gv === "DEFAULT" || gv === "-- NEW GRIDVIEW --") {
								MessageBox('You cannot delete this grid view.');
							}
							else if ($('#<%=ddlView.ClientID %> option:selected').attr('MyView') !== '1') {
								MessageBox('You cannot delete a grid view which was not created by you.');
							}
							else {
                                PageMethods.DeleteView($('#<%=ddlView.ClientID %>').val(), imgDeleteView_Done, on_error);
							}
							$(this).dialog("close");
						},
						"No": function () {
							$(this).dialog("close");
						}
					}
				});
			}
		}

		function imgDeleteView_Done(result) {
			$('#<%=ddlView.ClientID %>').empty();
			$.each(JSON.parse(result).ddlItems, function (val, text) {
				var newOption = $('<option>', { value: text.GridViewID, html: text.ViewName });
				$(newOption).attr('OptionGroup', (text.WTS_RESOURCEID != null ? "Custom Views" : "Process Views"));
				$(newOption).attr('ViewType', text.ViewType);
				$(newOption).attr('MyView', text.MyView);
				$('#<%=ddlView.ClientID %>').append(newOption);
			});
			$.each(JSON.parse(result).ddlItems, function (i, v) {
				if (v.ViewName === 'Default') {
					setDefaultPageItisettings(v.Tier1Columns);
					return;
				}
			});

			$("#<%=this.ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
			$("#<%=this.ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
			buildSortable();
            buildSaveView();
            opener.refreshPage();
		}

		function buildSortable(args) {
			var count;
			if (args === 0 || args === 2) {
				count = 1;
				$("#sortable").empty();
				$.each(JSON.parse(getDefaultPageItisettings()).sectionexpanded, function (index, value) {
					var liChk = $('<input>', { id: index, type: "checkbox", checked: value });
					var liLabel = $('<label>', { for: index, html: index.substring(3) });
					var newListItem = $('<li>', { "id": count, "class": "ui-state-default" });

					$(liChk).attr('parentGrid', "divBasic");
					$(newListItem).append(liChk);
					$(newListItem).append(liLabel);
					$("#sortable").append(newListItem);
					count++;
				});
				$("#sortable input:checkbox").on("click", function () { setSectionExpanded(this); });
			}

			if (args === 1 || args === 2) {
				count = 1;
				$("#sortableAdv").empty();
				$.each(JSON.parse(getDefaultPageItisettings()).tblCols, function (index, value) {
					var liChk = $('<input>', { id: "chk" + value.name, type: "checkbox", checked: value.show });
					var liLabel = $('<label>', { for: "chk" + value.name, html: value.alias || value.name, "class": "sortItemLable" });
					var liSpin = $('<input>', { id: "spr" + value.name, "class": "spinner" });

					var liRdospan = $('<span>', { id: "rdospan", "class": "rdospan" });
					var liCmd = $('<input>', { id: "cmd" + value.name, type: "button", value: "Hide", "class": "sortableBtn" });

					var liRdoA = $('<input>', { id: "rdo" + value.name + "A", name: value.name, "class": "sortRdo margin-left-25", type: "radio" });
					var liLabelA = $('<label>', { for: "rdo" + value.name + "A", "class": "sortRdo", html: "ASC" });

					var liRdoD = $('<input>', { id: "rdo" + value.name + "D", name: value.name, "class": "sortRdo", type: "radio" });
					var liLabelD = $('<label>', { for: "rdo" + value.name + "D", "class": "sortRdo", html: "DESC" });

					var liRdoN = $('<input>', { id: "rdo" + value.name + "N", name: value.name, "class": "sortRdo", type: "radio" });
					var liLabelN = $('<label>', { for: "rdo" + value.name + "N", "class": "sortRdoN", html: "None" });
					var newListItem = $('<li>', { "id": count, "class": "ui-state-default", attr: { colgroup: value.colgroup } });

					$(liChk).attr('parentGrid', "divAdvanced");
					$(liRdoA).attr('parentGrid', "divAdvanced");
					$(liRdoD).attr('parentGrid', "divAdvanced");
					$(liRdoN).attr('parentGrid', "divAdvanced");

					if (value.sortorder === "asc") liRdoA[0].checked = true;
					if (value.sortorder === "desc") liRdoD[0].checked = true;
					if (value.sortorder === "none") {
						liRdoN[0].checked = true;
						$(liSpin).prop("disabled", true);
					} else $(liSpin).prop("disabled", false);

					if (value.sortpriority !== "") $(liSpin).val(value.sortpriority);
					if (value.groupname !== "") $(liLabel).addClass("groupLabel");
					if (value.name.toUpperCase().indexOf("PRIORITY") !== -1) $(liRdospan).append(liCmd);
					if (value.columnwidth === 0) $(liCmd).attr("value", "Show");

					$(liRdospan).append(liRdoA);
					$(liRdospan).append(liLabelA);
					$(liRdospan).append(liRdoD);
					$(liRdospan).append(liLabelD);
					$(liRdospan).append(liRdoN);
					$(liRdospan).append(liLabelN);
					$(liRdospan).append(liSpin);

					var groupColor;
					if (value.colgroup === 'AOR') groupColor = "Red";
					else if (value.colgroup === 'CR') groupColor = "Blue";
					else if (value.colgroup === 'SR') groupColor = "Green";
					else if (value.colgroup === 'Task') groupColor = "Orange";
					else groupColor = "Black";

					$(newListItem).append("<div style='float: left; color: " + groupColor + "; margin-left: -15px;'>&bull;</div>");
					$(newListItem).append(liChk);
					$(newListItem).append(liLabel);
					$(newListItem).append(liRdospan);

					if (!value.show) $(liRdospan).fadeTo(100, 0.33);
					$(newListItem).on("mouseenter", function () {
						$(liLabel).css("font-weight", "bold");
					});
					$(newListItem).on("mouseleave", function () {
						$(liLabel).css("font-weight", "normal");
					});

					$(liCmd).attr("title", "Allows you to sort the data by this property but not display the property in the gridview. When the button text is Hide the field shows in the grid. When the button text is Show the field is hidden in the grid");
					$(liCmd).tooltip({
						content: function () {
							var element = $(this);
							return element.attr("title");
						}
					});

					if (value.concat) {
						$(liLabel).addClass("concatItemLable");
						$(newListItem).attr("title", "Columns included:");
						$(newListItem).droppable({
							drop: function (event, ui) {
								var newTitle = $(this).attr("title");
								newTitle += '<br>* ' + ui.draggable[0].children[0].id.substring(3);
								$(this).attr("title", newTitle);
								$(ui.draggable[0]).remove();
								updateConCatNames(ui.draggable[0], this);
							}
						});
						$(newListItem).tooltip({
							content: function () {
								var element = $(this);
								return element.attr("title");
							}
						});
						$.each(value.catcols, function (index, value) {
							var newTitle = $(newListItem).attr("title");
							newTitle += '<br>* ' + value;
							$(newListItem).attr("title", newTitle);
						});
					}

					$("#sortableAdv").append(newListItem);
					if (value.groupname.toString() !== "") addCtxMenuItem(value.groupname.toString());
					count++;
				});

				//TODO: Update this code so that removes properties that are in a concatenated column.
				//$.each(JSON.parse(getDefaultPageItisettings()).tblCols, function (i, v) {
				//    if (v.concat)
				//        $.each(v.catcols, function (int, val) {
				//            //if()
				//            //$("#sortableAdv").remove();
				//        });
				//});

				resetCtxMenu("divAdvanced");
				$(".spinner").sortPriority();
				$("#sortableAdv input:radio").click(function () { setSortOrder(this); });
				$('#sortableAdv .ui-spinner-button').click(function () { reOrderSort(this); });
				$("#sortableAdv input:button").on("click", function () { setColumnWidth(this); });
				$("#sortableAdv input:checkbox").on("click", function () { setSectionExpanded(this); });
				if (JSON.parse(getDefaultPageItisettings()).showchildrc) $("#cntIndicator").addClass("cntIndicator");
				else $("#cntIndicator").removeClass("cntIndicator");
			}
		}

		function addColToGroup(args) {
			var groupName;
			var columnName = $("#ctxMenu").data('columnName');
			var data = JSON.parse(getDefaultPageItisettings());

			if (args === undefined) groupName = prompt("Please enter your group name", "Enter Name");
			else groupName = args;

			if (groupName !== null) {
				var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

				if (subgrid.indexOf("subgrid") === -1) {
					$.each(data.tblCols, function (i, v) {
						if (columnName === v.name.toString()) v.groupname = groupName;
					});
					data.showcolumnheader = true;
				} else {
					$.each(data, function (i, v) {
						if (i === subgrid) {
							$.each(v[0].tblCols, function (i, v) {
								if (columnName === v.name.toString()) v.groupname = groupName;
							});
						}
					});
				}

				setDefaultPageItisettings(JSON.stringify(data));

				if (args === undefined) addCtxMenuItem(groupName);
				if (subgrid.indexOf("subgrid") !== -1) subgrid += "Ul";

				$.each($("#" + subgrid + " li label[for*= 'chk" + columnName + "']"), function () {
					$(this).addClass("groupLabel");
				});
			}
		}

		function addCtxMenuItem(args) {
			var itemExists = false;

			$.each($("#colGroups li div"), function () {
				if (args === this.innerHTML) itemExists = true;
			});

			if (!itemExists) {
				$("#colGroups").prepend("<li><div style='font-weight:bold' onclick='addColToGroup(this.innerHTML);'>" + args + "</div></li>");
				//$("#remColGroups").prepend("<li><div style='font-weight:bold' onclick='removeGroup(this.innerHTML);'>" + args + "</div></li>");
			}
		}

		function resetCtxMenu(args) {
			var data = JSON.parse(getDefaultPageItisettings());

			$("#colGroups").empty();
			$("#remColGroups").empty();

			$("#colGroups").append('<li></li><li><div onclick="addColToGroup()">New Group</div></li>');
			$("#remColGroups").append('<li></li><li><div onclick="removeGroup(\'all\')">Remove Group</div></li>');

			if (args === "divAdvanced") {
				$(".ctxGvHeader").show();
				$.each(data.tblCols, function (i, v) {
					if (v.groupname !== "") addCtxMenuItem(v.groupname);
				});
			} else {
				$(".ctxGvHeader").hide();
				$.each(data, function (i, v) {
					if (i === args) {
						$.each(v[0].tblCols, function (i, v) {
							if (v.groupname !== "") addCtxMenuItem(v.groupname);
						});
					}
				});
			}
		}

		function removeGroup(args) {
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if (args === "all") {
				if (subgrid === "divAdvanced") {
					$.each(data.tblCols, function (i, v) {
						v.groupname = "";
					});
					$("#" + subgrid + " .groupLabel").removeClass("groupLabel");
				} else {
					$.each(data, function (i, v) {
						if (i === subgrid) {
							$.each(v[0].tblCols, function (index, value) {
								value.groupname = "";
							});
							$("#" + subgrid + "Ul .groupLabel").removeClass("groupLabel");
						}
					});
				}
			} else {
				if (subgrid === "divAdvanced") {
					$.each(data.tblCols, function (i, v) {
						if (v.groupname === args) v.groupname = "";
					});
					$("#" + subgrid + " .groupLabel").removeClass("groupLabel");
				} else {
					$.each(data, function (i, v) {
						if (i === args) {
							$.each(v[0].tblCols, function (i, v) {
								if (v.groupname === args) v.groupname = "";
							});
							$("#" + subgrid + "Ul .groupLabel").removeClass("groupLabel");
						}
					});
				}
			}

			setDefaultPageItisettings(JSON.stringify(data));
			resetCtxMenu(subgrid);
		}

		function removeFromGroup() {
			var columnName = $("#ctxMenu").data('columnName');
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if (subgrid === "divAdvanced") {
				$.each(data.tblCols, function (i, v) {
					if (v.name === columnName) v.groupname = "";
				});
				$.each($("#" + subgrid + " li label[for*= 'chk" + columnName + "']"), function () {
					$(this).removeClass("groupLabel");
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (v.name === columnName) v.groupname = "";
						});
						$.each($("#" + subgrid + "Ul li label[for*= 'chk" + columnName + "']"), function () {
							$(this).removeClass("groupLabel");
						});
					}
				});
			}

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function renameGroup() {
			var curGroupName = "";
			var columnName = $("#ctxMenu").data('columnName');
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if (subgrid === "divAdvanced") {
				$.each(data.tblCols, function (i, v) {
					if (v.name === columnName) curGroupName = v.groupname;
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (v.name === columnName) curGroupName = v.groupname;
						});
					}
				});
			}

			var newGroupName = prompt("Please enter your group name", curGroupName);
			if (newGroupName !== "") {
				if (subgrid === "divAdvanced") {
					$.each(data.tblCols, function (i, v) {
						if (v.groupname === curGroupName) v.groupname = newGroupName;
					});
				} else {
					$.each(data, function (i, v) {
						if (i === subgrid) {
							$.each(v[0].tblCols, function (i, v) {
								if (v.groupname === curGroupName) v.groupname = newGroupName;
							});
						}
					});
				}
				setDefaultPageItisettings(JSON.stringify(data));
			}
		}

		function toggleGridViewHeader() {
			var data = JSON.parse(getDefaultPageItisettings());

			$.each(data.tblCols, function (i, v) {
				v.groupname = "";
			});

			$("#divAdvanced .groupLabel").removeClass("groupLabel");

			if (data.showcolumnheader === true) data.showcolumnheader = false;
			else data.showcolumnheader = true;

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function toggleChildRowCount() {
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if (subgrid === "divAdvanced") {
				if (data.showchildrc) {
					data.showchildrc = false;
					$("#cntIndicator").removeClass("cntIndicator");
				} else {
					data.showchildrc = true;
					$("#cntIndicator").addClass("cntIndicator");
				}
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						if (v[0].showchildrc) {
							v[0].showchildrc = false;
							$("#cntIndicator").removeClass("cntIndicator");
						} else {
							v[0].showchildrc = true;
							$("#cntIndicator").addClass("cntIndicator");
						}
					}
				});
			}

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function editColumnName() {
			var curAlias = "";
			var columnName = $("#ctxMenu").data('columnName');
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if (subgrid === "divAdvanced") {
				$.each(data.tblCols, function (i, v) {
					if (v.name === columnName || v.alias === columnName) curAlias = v.alias;
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (v.name === columnName || v.alias === columnName) curAlias = v.alias;
						});
					}
				});
			}

			var newAlias = prompt("Please enter your new column name", curAlias);
			if (subgrid === "divAdvanced") {
				$.each(data.tblCols, function (i, v) {
					if (v.name === columnName || v.alias === columnName) {
						v.alias = newAlias;
						if (newAlias === "") newAlias = v.name;
						$("#sortableAdv li label[for*= 'chk" + v.name + "']").html(newAlias);
					}
				});
			} else {
				$.each(data, function (i, v) {
					if (i === subgrid) {
						$.each(v[0].tblCols, function (i, v) {
							if (v.name === columnName || v.alias === columnName) {
								v.alias = newAlias;
								if (newAlias === "") newAlias = v.name;
								$("#" + subgrid + "Ul li label[for*= 'chk" + v.name + "']").html(newAlias);
							}
						});
					}
				});
			}
			setDefaultPageItisettings(JSON.stringify(data));
		}

		function editConcatColumns() {
			alert("This function is in development.");
		}

		function deleteConcatColumn() {
			var columnName = $("#ctxMenu").data('columnName');
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			$("#dialog-confirm").dialog({
				resizable: false,
				height: "auto",
				width: 400,
				modal: true,
				buttons: {
					"Yes": function () {
						if (subgrid === "divAdvanced") {
							$.each(data.tblCols, function (i, v) {
								if (v.name === columnName || v.alias === columnName) {
									data.tblCols.splice(i, 1);
									$.each(data.columnorder, function (item, value) {
										if (value === (i + 1).toString()) data.columnorder.splice(item, 1);
									});
								}
							});
							$("#sortableAdv li label[for*= 'chk" + columnName + "']").parent().remove();
						} else {
							$.each(data, function (i, v) {
								if (i === subgrid) {
									$.each(v[0].tblCols, function (index, val) {
										if (val.name === columnName || val.alias === columnName) {
											v[0].tblCols.splice(index, 1);
											$.each(v[0].columnorder, function (item, value) {
												if (value === (index + 1).toString()) v[0].columnorder.splice(item, 1);
											});
										}
									});
									$("#" + subgrid + "Ul li label[for*= 'chk" + columnName + "']").parent().remove();
								}
							});
						}
						setDefaultPageItisettings(JSON.stringify(data));
						$(this).dialog("close");
					},
					"No": function () {
						$(this).dialog("close");
					}
				}
			});
		}

		function addProperty() {
			var colName = prompt("Please enter your name", "New Property Name");

			if (colName !== null) {
				var data = JSON.parse(getDefaultPageItisettings());
				var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");
				var sgUl;

				if (subgrid === "divAdvanced") {
					subgrid = "sortableAdv";
					sgUl = "sortableAdv";
				}
				else sgUl = subgrid + "Ul";

				var count = $('#' + sgUl + ' li').length + 1;

				var liChk = $('<input>', { id: "chk" + colName, type: "checkbox", checked: true });
				var liLabel = $('<label>', { for: "chk" + colName, html: colName, "class": "concatItemLable" });
				var liSpin = $('<input>', { id: "spr" + colName, "class": "spinner" });

				var liRdospan = $('<span>', { id: "rdospan", "class": "rdospan" });
				var liRdoA = $('<input>', { id: "rdo" + colName + "A", name: colName, "class": "sortRdo margin-left-25", type: "radio" });
				var liLabelA = $('<label>', { for: "rdo" + colName + "A", "class": "sortRdo", html: "ASC" });

				var liRdoD = $('<input>', { id: "rdo" + colName + "D", name: colName, "class": "sortRdo", type: "radio" });
				var liLabelD = $('<label>', { for: "rdo" + colName + "D", "class": "sortRdo", html: "DESC" });

				var liRdoN = $('<input>', { id: "rdo" + colName + "N", name: colName, "class": "sortRdo", type: "radio" });
				var liLabelN = $('<label>', { for: "rdo" + colName + "N", "class": "sortRdoN", html: "None" });
				var newListItem = $('<li>', { "id": count, "class": "ui-state-default", title: "Columns included:" });

				$(liChk).on("click", function () { setSectionExpanded(this); });

				$(liChk).attr('parentGrid', subgrid);
				$(liRdoA).attr('parentGrid', subgrid);
				$(liRdoD).attr('parentGrid', subgrid);
				$(liRdoN).attr('parentGrid', subgrid);
				$(newListItem).attr('parentGrid', subgrid);

				liRdoN[0].checked = true;
				$(liSpin).prop("disabled", true);

				$(liRdospan).append(liRdoA);
				$(liRdospan).append(liLabelA);
				$(liRdospan).append(liRdoD);
				$(liRdospan).append(liLabelD);
				$(liRdospan).append(liRdoN);
				$(liRdospan).append(liLabelN);
				$(liRdospan).append(liSpin);

				$(newListItem).append(liChk);
				$(newListItem).append(liLabel);
				$(newListItem).append(liRdospan);

				$(liRdospan).fadeTo(100, 1);

				$(newListItem).on("mouseenter", function () {
					$(liLabel).css("font-weight", "bold");
				});
				$(newListItem).on("mouseleave", function () {
					$(liLabel).css("font-weight", "normal");
				});
				$(newListItem).droppable({
					drop: function (event, ui) {
						var newTitle = $(this).attr("title");
						newTitle += '<br>* ' + ui.draggable[0].children[0].id.substring(3);
						$(this).attr("title", newTitle);
						$(ui.draggable[0]).remove();
						updateConCatNames(ui.draggable[0], this);
					}
				});
				$(newListItem).tooltip({
					content: function () {
						var element = $(this);
						return element.attr("title");
					}
				});

				$('#' + sgUl + '').prepend(newListItem);
				$(".spinner").sortPriority();

				//TODO: Refactor the following two methods into one.
				if (subgrid.indexOf("subgrid") === -1) {
					data.tblCols.push({
						name: colName,
						alias: "",
						show: true,
						sortorder: "none",
						sortpriority: "",
						groupname: "",
						concat: true,
						catcols: [],
						colgroup: ""
					}
					);
					data.columnorder.splice(0, 0, count.toString());
				} else {
					$.each(data, function (i, v) {
						if (i === subgrid) {
							v[0].tblCols.push({
								name: colName,
								alias: "",
								show: true,
								sortorder: "none",
								sortpriority: "",
								groupname: "",
								concat: true,
								catcols: [],
								colgroup: ""
							}
							);
							v[0].columnorder.splice(0, 0, count.toString());
						}
					});
				}

				if (subgrid === "sortableAdv") subgrid = "divAdvanced";

				updateTabText(subgrid);
				$('#' + sgUl + ' input:radio').off("click");
				$('.ui-spinner-button').off("click").on("click", function () { reOrderSort(this); });
				$('#' + sgUl + ' input:radio').click(function () { setSortOrder(this); });

				setDefaultPageItisettings(JSON.stringify(data));
			}
		}

		function popTplSubGridR1(args) {
			var count = 1;
			var firstCol = 0;
			var sgUl = args + "Ul";
			$("#gridItemBar").parent().attr("id", sgUl);
			$('#' + sgUl + '').empty();
			$('#' + sgUl + '').addClass("sortable");

			$.each(JSON.parse(getDefaultPageItisettings()), function (i, v) {
				if (i === args) firstCol = v[0].columnorder[0];
			});

			$.each(JSON.parse(getDefaultPageItisettings()), function (i, v) {
				if (i === args) {
					$.each(v[0].tblCols, function (index, value) {
						if (index === firstCol - 1) {
							var liChk = $('<input>', { id: "chk" + value.name, type: "checkbox", checked: false });
							var liLabel = $('<label>', { for: "chk" + value.name, html: value.alias || value.name, "class": "sortItemLable" });
							var liSpin = $('<input>', { id: "spr" + value.name, "class": "spinner" });

							var liRdospan = $('<span>', { id: "rdospan", "class": "rdospan" });
							var liRdoA = $('<input>', { id: "rdo" + value.name + "A", name: value.name, "class": "sortRdo margin-left-25", type: "radio" });
							var liLabelA = $('<label>', { for: "rdo" + value.name + "A", "class": "sortRdo", html: "ASC" });

							var liRdoD = $('<input>', { id: "rdo" + value.name + "D", name: value.name, "class": "sortRdo", type: "radio" });
							var liLabelD = $('<label>', { for: "rdo" + value.name + "D", "class": "sortRdo", html: "DESC" });

							var liRdoN = $('<input>', { id: "rdo" + value.name + "N", name: value.name, "class": "sortRdo", type: "radio" });
							var liLabelN = $('<label>', { for: "rdo" + value.name + "N", "class": "sortRdoN", html: "None" });
							var newListItem = $('<li>', { "id": count, "class": "ui-state-default", attr: { colgroup: value.colgroup } });

							$(liChk).attr('parentGrid', args);
							$(liRdoA).attr('parentGrid', args);
							$(liRdoD).attr('parentGrid', args);
							$(liRdoN).attr('parentGrid', args);
							$(newListItem).attr('parentGrid', args);

							if (value.sortorder === "asc") liRdoA[0].checked = true;
							if (value.sortorder === "desc") liRdoD[0].checked = true;
							if (value.sortorder === "none") {
								liRdoN[0].checked = true;
								$(liSpin).prop("disabled", true);
							} else {
								$(liSpin).prop("disabled", false);
							}

							if (value.sortpriority !== "") $(liSpin).val(value.sortpriority);
							if (value.groupname !== "") $(liLabel).addClass("groupLabel");

							$(liRdospan).append(liRdoA);
							$(liRdospan).append(liLabelA);
							$(liRdospan).append(liRdoD);
							$(liRdospan).append(liLabelD);
							$(liRdospan).append(liRdoN);
							$(liRdospan).append(liLabelN);
							$(liRdospan).append(liSpin);

							var groupColor;
							if (value.colgroup === 'AOR') groupColor = "Red";
							else if (value.colgroup === 'CR') groupColor = "Blue";
							else if (value.colgroup === 'SR') groupColor = "Green";
							else if (value.colgroup === 'Task') groupColor = "Orange";
							else groupColor = "Black";

							$(newListItem).append("<div style='float: left; color: " + groupColor + "; margin-left: -15px;'>&bull;</div>");
							$(newListItem).append(liChk);
							$(newListItem).append(liLabel);
							$(newListItem).append(liRdospan);

							$('#' + sgUl + '').append(newListItem);
						}
					});
				}
			});
		}

		function popTplSubGrid(args) {
			var count = 1;
			var sgUl = args + "Ul";
			$("#gridItemBar").parent().attr("id", sgUl);
			$('#' + sgUl + '').empty();
			$('#' + sgUl + '').addClass("sortable");
			$.each(JSON.parse(getDefaultPageItisettings()), function (i, v) {
				if (i === args) {
					$.each(v[0].tblCols, function (index, value) {
						var liChk = $('<input>', { id: "chk" + value.name, type: "checkbox", checked: false });
						var liLabel = $('<label>', { for: "chk" + value.name, html: value.alias || value.name, "class": "sortItemLable" });
						var liSpin = $('<input>', { id: "spr" + value.name, "class": "spinner" });

						var liRdospan = $('<span>', { id: "rdospan", "class": "rdospan" });
						var liCmd = $('<input>', { id: "cmd" + value.name, type: "button", value: "Hide", "class": "sortableBtn" });

						var liRdoA = $('<input>', { id: "rdo" + value.name + "A", name: value.name, "class": "sortRdo margin-left-25", type: "radio" });
						var liLabelA = $('<label>', { for: "rdo" + value.name + "A", "class": "sortRdo", html: "ASC" });

						var liRdoD = $('<input>', { id: "rdo" + value.name + "D", name: value.name, "class": "sortRdo", type: "radio" });
						var liLabelD = $('<label>', { for: "rdo" + value.name + "D", "class": "sortRdo", html: "DESC" });

						var liRdoN = $('<input>', { id: "rdo" + value.name + "N", name: value.name, "class": "sortRdo", type: "radio" });
						var liLabelN = $('<label>', { for: "rdo" + value.name + "N", "class": "sortRdoN", html: "None" });
						var newListItem = $('<li>', { "id": count, "class": "ui-state-default", attr: { colgroup: value.colgroup } });

						$(liChk).attr('parentGrid', args);
						$(liRdoA).attr('parentGrid', args);
						$(liRdoD).attr('parentGrid', args);
						$(liRdoN).attr('parentGrid', args);
						$(newListItem).attr('parentGrid', args);

						if (value.sortorder === "asc") liRdoA[0].checked = true;
						if (value.sortorder === "desc") liRdoD[0].checked = true;
						if (value.sortorder === "none") {
							liRdoN[0].checked = true;
							$(liSpin).prop("disabled", true);
						} else {
							$(liSpin).prop("disabled", false);
						}

						if (value.sortpriority !== "") $(liSpin).val(value.sortpriority);
						if (value.groupname !== "") $(liLabel).addClass("groupLabel");
						if (value.name.toUpperCase().indexOf("PRIORITY") !== -1) $(liRdospan).append(liCmd);
						if (value.columnwidth === 0) $(liCmd).attr("value", "Show");

						$(liRdospan).append(liRdoA);
						$(liRdospan).append(liLabelA);
						$(liRdospan).append(liRdoD);
						$(liRdospan).append(liLabelD);
						$(liRdospan).append(liRdoN);
						$(liRdospan).append(liLabelN);
						$(liRdospan).append(liSpin);

						var groupColor;
						if (value.colgroup === 'AOR') groupColor = "Red";
						else if (value.colgroup === 'CR') groupColor = "Blue";
						else if (value.colgroup === 'SR') groupColor = "Green";
						else if (value.colgroup === 'Task') groupColor = "Orange";
						else groupColor = "Black";

						$(newListItem).append("<div style='float: left; color: " + groupColor + "; margin-left: -15px;'>&bull;</div>");
						$(newListItem).append(liChk);
						$(newListItem).append(liLabel);
						$(newListItem).append(liRdospan);

						if (!value.show) $(liRdospan).fadeTo(100, 0.33);
						$(newListItem).on("mouseenter", function () {
							$(liLabel).css("font-weight", "bold");
						});
						$(newListItem).on("mouseleave", function () {
							$(liLabel).css("font-weight", "normal");
						});

						$(liCmd).attr("title", "Allows you to sort the data by this property but not display the property in the gridview. When the button text is Hide the field shows in the grid. When the button text is Show the field is hidden in the grid");
						$(liCmd).tooltip({
							content: function () {
								var element = $(this);
								return element.attr("title");
							}
						});

						if (value.concat) {
							$(liLabel).addClass("concatItemLable");
							$(newListItem).attr("title", "Columns included:");
							$(newListItem).droppable({
								drop: function (event, ui) {
									var newTitle = $(this).attr("title");
									newTitle += '<br>* ' + ui.draggable[0].children[0].id.substring(3);
									$(this).attr("title", newTitle);
									$(ui.draggable[0]).remove();
									updateConCatNames(ui.draggable[0], this);
								}
							});
							$(newListItem).tooltip({
								content: function () {
									var element = $(this);
									return element.attr("title");
								}
							});
							$.each(value.catcols, function (index, value) {
								var newTitle = $(newListItem).attr("title");
								newTitle += '<br>* ' + value;
								$(newListItem).attr("title", newTitle);
							});
						}

						$('#' + sgUl + '').append(newListItem);
						count++;
					}
					);
				}
			});

			$('#' + sgUl + ' input:radio').click(function () { setSortOrder(this); });
			$('#' + sgUl + ' input:button').on("click", function () { setColumnWidth(this); });
			$('#' + sgUl + ' input:checkbox').on("click", function () { setSectionExpanded(this); });

			var data = JSON.parse(getDefaultPageItisettings());
			if (colArray > 0) {
				for (var j = 0; j < data.columnorder.length; j++) {
					$('#' + sgUl + '').append(colArray[data.columnorder[j] - 1]);
				}
			}

			var s = document.createElement('script');
			s.id = sgUl + '_code';
			s.type = 'text/javascript';
			var code = $('#tmplSubGridCode').html().replace("#sortable", "#" + sgUl);
			try {
				s.appendChild(document.createTextNode(code));
				document.body.appendChild(s);
			} catch (e) {
				s.text = code;
				document.body.appendChild(s);
			}

			$(".spinner").sortPriority();
			$('#' + sgUl + ' .ui-spinner-button').off("click").on("click", function () { reOrderSort(this); });
			$("#" + args).bind("contextmenu", "li", function (e) {
				e.preventDefault();
				$("#mnuCntnr").css("left", e.pageX);
				$("#mnuCntnr").css("top", e.pageY);
				$("#mnuCntnr").fadeIn(500, startFocusOut());
				$("#ctxMenu").data('originalElement', this);
				setupCtxMnu();
			});
		}

		function updateConCatNames(source, target) {
			var conCatColName = $(target).find($("input[id*= 'chk']"));
			var data = JSON.parse(getDefaultPageItisettings());
			var subgrid = source.children[0].getAttribute("parentGrid");
			var name = source.children[0].id.substring(3);

			if (subgrid === "divAdvanced")
				$.each(data.tblCols, function (i, v) {
					if (v.name === name) v.show = true;
					if (v.name === conCatColName[0].id.substring(3)) v.catcols.push(name.toString());
				});
			else
				$.each(data, function (i, v) {
					if (i === subgrid)
						$.each(v[0].tblCols, function (int, value) {
							if (value.name === name) value.show = true;
							if (value.name === conCatColName[0].id.substring(3)) value.catcols.push(name.toString());
						});
				});

			setDefaultPageItisettings(JSON.stringify(data));
		}

		function renameTabs() {
			var count = 0;
			$('#tabsUl li > a').each(function () {
				if (count > 1) {
					var loc = $(this).text().indexOf("-");
					var tabText = $(this).text().substring(loc + 1);

					$(this).text("Lvl " + count + " - " + tabText);
				}
				count++;
			});
			$("#divTabsContainer").tabs("refresh");
		}

		function updateTabText(args) {
			var count = 0;
			var liCount = $("#" + args + "Ul > li").length;
			var tabName = $("#" + args + "Ul > li > :checkbox:checked:first");

			$('#tabsUl li > a').each(function () {
				if (this.href.indexOf(args) !== - 1) {
					if (args !== "divAdvanced") {
						if (tabName.length === 0) tabName = "No Selection";
						else tabName = tabName[0].id.substring(3);
						if (liCount === 1) tabName = $("#" + args + "Ul > li > label").first().html();

						$("#tabsUl #" + args.replace("g", "G") + " > a").html("Lvl " + count + " - " + tabName);
					} else {
						tabName = $("#sortableAdv > li > :checkbox:checked:first");
						if (tabName.length === 0) tabName = "No Selection";
						else tabName = tabName[0].id.substring(3);

						$("#tabsUl #tabAdvanced > a").html("Lvl 1 - " + tabName);
					}
				}
				count++;
			});
			$("#divTabsContainer").tabs("refresh");
		}

		function Tab_click(args) {
			var tabId;
			var data = JSON.parse(getDefaultPageItisettings());

			if (args.hash !== undefined) tabId = args.hash.substring(1);
			else tabId = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			if ($("#" + tabId + "Ul > li").length === 1) {
				popTplSubGrid(tabId);
				$.each(data, function (i, v) {
					if (i.indexOf(tabId) !== -1) {
						var newColArray = $('#' + i + ' li');
						$.each(v[0].tblCols, function (x, s) {
							$("#" + i + "Ul > li > input[id*= 'chk" + s.name + "']").prop('checked', s.show);
						});

						for (var j = 0; j < v[0].columnorder.length; j++)
							$('#' + i + 'Ul').append(newColArray[v[0].columnorder[j] - 1]);
					}
				});
			}

			if (tabId === "divAdvanced") {
				$.each(data.tblCols, function (i, v) {
					if (v.sortorder === "asc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "A']").prop('checked', true);
					if (v.sortorder === "desc") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "D']").prop('checked', true);
					if (v.sortorder === "none") $("#sortableAdv > li > span > input[id*= 'rdo" + v.name + "N']").prop('checked', true);
				});

				if (data.showchildrc) $("#cntIndicator").addClass("cntIndicator");
				else $("#cntIndicator").removeClass("cntIndicator");
			} else {
				$.each(data, function (i, v) {
					if (i === tabId) {
						$.each(v[0].tblCols, function (index, value) {
							if (value.sortorder === "asc") $("#" + tabId + "Ul > li > span > input[id*= 'rdo" + value.name + "A']").prop('checked', true);
							if (value.sortorder === "desc") $("#" + tabId + "Ul > li > span > input[id*= 'rdo" + value.name + "D']").prop('checked', true);
							if (value.sortorder === "none") $("#" + tabId + "Ul > li > span > input[id*= 'rdo" + value.name + "N']").prop('checked', true);
						});

						if (v[0].showchildrc) $("#cntIndicator").addClass("cntIndicator");
						else $("#cntIndicator").removeClass("cntIndicator");
					}
				});
			}
			resetCtxMenu(tabId);
			if (parseInt($("#tabsUl").height()) > 30) resizePage(parseInt('<%=_activeTab %>'));

			$("#propFilters input[type='checkbox']").off("click").click(function () {
				filterFields($("#divTabsContainer .ui-tabs-panel:visible").attr("id"));
			});
		}

		function filterFields(args) {
			$("#filterText").val("");
			$("#" + args + " li").show();
			$.each($("input[name='propFilter']:not(:checked)"), function () {
				$("#" + args + " li[colgroup=" + $(this).val() + "]").hide();
			});
		}

		$("#filterText").on('input', function (evt) {
			var subgrid = $("#divTabsContainer .ui-tabs-panel:visible").attr("id");

			$("input[name='propFilter']").prop('checked', true);
			$("#" + subgrid + " li").show();
			$("#" + subgrid + " li > label:not(:icontains('" + $("#filterText").val() + "'))").parent().hide();
		});

		$.widget("tj.sortPriority", $.ui.spinner, {
			options: { step: 1, min: 0 }
		});

		$(document).ready(function () {
			buildSortable(parseInt('<%=_activeTab %>'));
			chkArray = $('#sortable input[type="checkbox"]');
			subGridCount = $('#tabsUl li').length + 1;
			listArray = $('#sortable li');
			colArray = $('#sortableAdv li');
			initEvents();

			$('#divTabsContainer').tabs({ active: parseInt('<%=_activeTab %>') });

			if (parseInt('<%=_activeTab %>') === 0) {
				$('#tabsUl > :not(#tabBasic)').hide();
				$('#divTreeview').hide();
				$('#imgOpenFilters').hide();
				$('#newProperty').hide();
				$('#addsubgrid').hide();
				$('#tabBasic').show();
			} else {
				$('#tabBasic').hide();
				$('#divTreeview').show();
				$('#tabAdvanced').show();
				$('#addsubgrid').show();
				$('#spanSelect').text("Select columns to be displayed. Drag and drop to re-order:");
			}

			$("#<%=ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
			$("#<%=ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");

			$("#propFilters input[type='checkbox']").click(function () {
				filterFields($("#divTabsContainer .ui-tabs-panel:visible").attr("id"));
			});

			$('#imgOpenFilters').tooltip({
				content: function () {
					var element = $(this);
					return element.attr("title");
				}
			});

			$(".filters").show();
			$('#filterArrow').hide();

			$("#imgHelp").tooltip({
				content: function () {
					var element = $(this);
					return element.attr("title");
				},
				open: function (event, ui) {
					ui.tooltip.css("max-width", "500px");
				}
			});

			resizePage(parseInt('<%=_activeTab %>'));

			$(defaultParentPage.itisettings).bind("DOMSubtreeModified", function () {
				if (!viewChng) $('#<%=ddlView.ClientID %> option').filter(function () { return ($(this).text() === "Customized View"); }).prop('selected', true);
				else viewChng = false;
			});

			$.extend($.expr[":"], {
				"icontains": function (elem, i, match, array) {
					return (elem.textContent || elem.innerText || "").toLowerCase().indexOf((match[3] || "").toLowerCase()) >= 0;
				}
			});

			buildSaveView();
			$(document).on("click", "#<%=cmdUpdateTreeView.ClientID %>", function () {
				PageMethods.UpdateSessionData(getDefaultPageItisettings(), function () {
					__doPostBack('<%= cmdUpdateTreeView.ClientID  %>', getDefaultPageItisettings());
				});
			});
		});

		function buildSaveView() {
			$('#<%=txtViewName.ClientID %>').val('');
			$('#ddlSaveView').html($('#<%=ddlView.ClientID %>').html());
			$('#ddlSaveView option').filter(function () { return ($(this).text() === "Customized View"); }).remove();
			$('#ddlSaveView').prepend('<option>--Create New--</option>');
			$('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true).change();
		}

		function resizePage(args) {
			var tab = args;
			var nWindow = popupManager.GetPopupByName('ITI_Settings');
			var nHeight = 393, nWidth = 400;

			if (tab > 0) {
				nHeight = 823;
				nWidth = 925;
				$("#divTreeview").height("758px");
				if (parseInt($("#tabsUl").height()) > 30) {
					nHeight = 847;
					$("#divTreeview").height("782px");
				}
			}

			nWindow.SetHeight(nHeight);
			nWindow.SetWidth(nWidth);
		}

		function setupCtxMnu() {
			var data = JSON.parse(getDefaultPageItisettings());
			var subGrid = $("#ctxMenu").data('originalElement').id;
			var colName = $("#ctxMenu").data('columnName');
			var count = 0;

			$(".ctxMnuRemoveGroups").hide();
			if (subGrid === "sortableAdv") {
				$.each(data.tblCols, function (i, v) {
					if (v.groupname !== "") count++;
				});
			} else {
				$.each(data, function (i, val) {
					if (i === subGrid) {
						$.each(val[0].tblCols, function (j, v) {
							if (v.groupname !== "") count++;
						});
					}
				});
			}

			if (subGrid === "sortableAdv") {
				$.each(data.tblCols, function (i, v) {
					if (colName === v.name)
						if (v.concat) {
							$(".ctxMnuGroups").hide();
							$(".ctxMnuCatCols").show();
						} else {
							if (count > 0) $(".ctxMnuRemoveGroups").show();
							if (v.groupname === "") {
								$("#ctxRemoveFromGroup").hide();
								$("#ctxRenameSpacer").hide();
								$("#ctxRenameGroup").hide();
							}
							$(".ctxMnuGroups").show();
							$(".ctxMnuCatCols").hide();
						}
				});
			} else {
				$.each(data, function (i, val) {
					if (i === subGrid) {
						$.each(val[0].tblCols, function (j, v) {
							if (colName === v.name)
								if (v.concat) {
									$(".ctxMnuGroups").hide();
									$(".ctxMnuCatCols").show();
								} else {
									if (count > 0) $(".ctxMnuRemoveGroups").show();
									if (v.groupname === "") {
										$("#ctxRemoveFromGroup").hide();
										$("#ctxRenameSpacer").hide();
										$("#ctxRenameGroup").hide();
									}
									$(".ctxMnuGroups").show();
									$(".ctxMnuCatCols").hide();
								}
						});
					}
				});
			}
		}

		if (parseInt('<%=_activeTab %>') !== 0) {
			$(document).mousedown(function (e) {
				if (e.button === 2)
					if (e.target.childNodes.length > 0)
						if (e.target.childNodes[0].data !== undefined)
							$("#ctxMenu").data('columnName', e.target.childNodes[0].data);

			});

			$("#divAdvanced").find("ul").on("contextmenu", function (e) {
				e.preventDefault();
				$("#mnuCntnr").css("left", e.pageX);
				$("#mnuCntnr").css("top", e.pageY);
				$("#mnuCntnr").fadeIn(500, startFocusOut());
				$("#ctxMenu").data('originalElement', this);
				setupCtxMnu();
			});

			function startFocusOut() {
				$(document).on("click", function () {
					$("#mnuCntnr").hide(500);
					$(document).off("click");
				});
			}

			$("#ctxMenu").menu({ selector: '.context-menu-one' }).data('originalElement', this);
			$("#tabsUl").sortable({
				cursor: "move",
				stop: function (event, ui) {
					var sourceData = JSON.parse(getDefaultPageItisettings());
					var data = JSON.parse(getDefaultPageItisettings());

					$.each($('#tabsUl li a'), function (i, v) {
						if (i > 0) {
							if (v.innerHTML === "Section Order") {
								$("#tabsUl").sortable("cancel");
								return false;
							}

							if (v.innerHTML.indexOf(i.toString()) === -1) {
								var endPoint = v.innerHTML.indexOf("-");
								var sourceGridLevel = v.innerHTML.substring(4, endPoint).trim();
								var levelCount = 2;
								var sourceTblCols;
								var sourceColumnorder;
								var sourceShowcolumnheader;

								if (sourceGridLevel === "1") {
									sourceTblCols = sourceData.tblCols;
									sourceColumnorder = sourceData.columnorder;
									sourceShowcolumnheader = sourceData.showcolumnheader;
								} else {
									$.each(sourceData, function (j, val) {
										if (j.indexOf("subgrid") !== -1) {
											if (levelCount.toString() === sourceGridLevel) {
												sourceTblCols = val[0].tblCols;
												sourceColumnorder = val[0].columnorder;
												sourceShowcolumnheader = val[0].showcolumnheader;
											}
											levelCount++;
										}
									});
								}

								if (i === 1) {
									data.tblCols = sourceTblCols;
									data.columnorder = sourceColumnorder;
									data.showcolumnheader = sourceShowcolumnheader;
								} else {
									levelCount = 2;
									$.each(data, function (j, val) {
										if (j.indexOf("subgrid") !== -1) {
											if (levelCount === i) {
												val[0].tblCols = sourceTblCols;
												val[0].columnorder = sourceColumnorder;
												val[0].showcolumnheader = sourceShowcolumnheader;
											}
											levelCount++;
										}
									});
								}
							}
						}
					});

					setDefaultPageItisettings(JSON.stringify(data));
					$.each($('#tabsUl li a'), function (i, v) {
						if (i > 0)
							if (v.innerHTML.indexOf(i.toString()) === -1) {
								var startPoint = v.innerHTML.indexOf("-");
								var tabText = v.innerHTML.substring(startPoint);

								v.innerHTML = "Lvl " + i + " " + tabText;
							}
					});
				}
			});
		}
	</script>
	<script id="tmplSubGrid" type="text/template">
		<div style="overflow-y: scroll; height: 625px;">
			<table style="width: 99%; vertical-align: top; text-align: left; padding: 5px;" cellpadding="0" cellspacing="0">
				<tr>
					<td style="border: 1px solid black; padding: 5px;">
						<table cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px 5px 0px;">
							<tr class="attributesRow">
								<td class="attributesValue">
									<ul id="subGridFoo">
										<li id="gridItemBar"></li>
									</ul>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
		<div style="float: left; margin-left: 18px; margin-top: 10px">
			<input type="button" value="Clear All" onclick="selectAllProps(this)" /></div>
		<div style="text-align: right; margin: 10px; margin-right: 18px">
			<input type="button" value="Get Data" onclick="btnClose_click();" />
		</div>
	</script>
	<script id="tmplSubGridCode" type="text/template">
		$("#sortable").sortable({
			stop: function (event, ui) {
				var subgrid = ui.item[0].getAttribute('parentGrid');
				var data = JSON.parse(getDefaultPageItisettings());
				
				if (subgrid.indexOf("subgrid") !== -1)
					$.each(data, function(i, v) {
						if (i === subgrid) v[0].columnorder = $("#" + subgrid + "Ul").sortable("toArray");
					});

				setDefaultPageItisettings(JSON.stringify(data));
				updateTabText($("#divTabsContainer .ui-tabs-panel:visible").attr("id"));
				UpdateTreeView();
			}
		});
		
		$('input[type="checkbox"]').off("click");
		$('input[type="radio"]').off("click");
		
		$('input[type="checkbox"]').on('click', function () { setSectionExpanded(this); });
		$('.ui-spinner-button').off("click").on("click", function () { reOrderSort(this); });
		$('input[type="radio"]').on('click', function () { setSortOrder(this); });
	</script>
</asp:Content>