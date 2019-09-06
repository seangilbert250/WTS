<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="ConfigureSR.aspx.cs" Inherits="MDGrid_ItemType" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Work Item Type Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
                Available SR Resources (<span id="spanRowCount" runat="server">0</span>)
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonSave" value="Save" />
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

		//var _pageURLs = new PageURLs();
		//var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
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
						        var value = GetColumnValue(row, i);
								changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + encodeURIComponent(GetColumnValue(row, i)) + '"');
							}
							var obj = '{' + changedRow.join(',') + '}';
							changedRows.push(obj);
						}
					}
				});

			    	ShowDimmer(true, "Updating...", 1);
			    	var json = '[' + changedRows.join(",") + ']';
			    	PageMethods.SaveChanges(json, save_done, on_error);

				//if (changedRows.length == 0) {
				//	MessageBox('You have not made any changes');
				//}
				//else {
				//	ShowDimmer(true, "Updating...", 1);
				//	var json = '[' + changedRows.join(",") + ']';
				//	PageMethods.SaveChanges(json, save_done, on_error);
				//}
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
					//refreshPage();
				}
				else {
					MessageBox('Failed to save items. \n' + errorMsg);
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

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		
		function buttonSave_click() {
			save();
		}

		function row_click(row) {
		    if ($(row).attr('WTS_RESOURCEID')) {
			    _selectedId = $(row).attr('WTS_RESOURCEID');
			    activateSaveButton();
			}
		}

		function activateSaveButton() {
			$('#buttonSave').attr('disabled', false);
			$('#buttonSave').prop('disabled', false);
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

    	<script id="jsInit" type="text/javascript">

<%--		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxID = parseInt('<%=this.DCC["WTS_RESOURCEID"].Ordinal %>');
					_idxName = parseInt('<%=this.DCC["Name"].Ordinal %>');
					_idxReceiveEMail = parseInt('<%=this.DCC["ReceiveSREMail"].Ordinal %>');
					_idxIncludeInCounts = parseInt('<%=this.DCC["IncludeInSRCounts"].Ordinal %>');
				}
			} catch (e) {
			    alert("Error: " + e.message);
			}
		}--%>
		
		$(document).ready(function () {
		    //initVariables();

		    $('#buttonSave').click(function (event) { buttonSave_click(this); });

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

		});
	</script>
</asp:Content>
