﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_SystemSuite_Assignment.aspx.cs" Inherits="MDGrid_SystemSuite_Assignment" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - System Suite Assignment</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Systems (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
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
	<div id="divPageContents" style="width: 100%;">
		<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
			CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" EmptyDataText="No data">
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
	            var parentID = '<%=_qfSystemSuiteID%>';
	            $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
    	            var changedRow = [];
                    var changed = false;
                    var ddlId = $(this).find('[id*="eddl"]').attr('id');
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
                                if (_dcc[0][i].ColumnName === "WTS_SystemID" && escape(GetColumnValue(row, i)) === '%A0') {
                                    changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i + 1)) + '"');
                                } else if (_dcc[0][i].ColumnName === "WTS_SYSTEM") {
                                    changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape($('#' + ddlId + ' option:selected').text()) + '"');
                                } else {
                                    changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + escape(GetColumnValue(row, i)) + '"');
                                }
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
                    PageMethods.SaveChanges(parentID, json, save_done, on_error);
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
                else if (saved == 'True' || saved > 0) {
                    msg = 'Successfully saved';
                }
                else if (failed > 0) {
                    msg += '<br>' + 'Failed to save ' + failed + ' record(s).';
                }
                if (msg.length > 0) {
                    MessageBox(msg);
                }

                refreshPage();

            } catch (e) { }
        }

        function deleteSystem(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);
                PageMethods.RemoveSystemFromSuite(itemId, delete_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data.\n' + e.message);
            }
        }

        function delete_done(result) {
            ShowDimmer(false);

            var saved = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.id) {
                        id = obj.id;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('System has been removed from suite.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to remove system from suite. \n' + errorMsg);
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
            $('#buttonNew').prop('disabled', false);
            resizeFrame();
        }

        function buttonNew_click() {
            var grdMD = <%=this.grdMD.ClientID%>;

            var nRow = grdMD.Body.Rows[0].cloneNode(true);

		    $(nRow.cells[_idxID]).val('0');//.innerText = '0';
            $(nRow.cells).each(function (i, td) {
                if ($(td).children('select').length > 0) {
                    $(td).find('select').attr('original_value', '');
                    $(td).find('select').prop('disabled', false);
                    $(td).find('select').on('change keyup mouseup', function () { ddl_change(this); });
                    $(td).find('select').on('mousedown focus', function () { LoadList(this); });
                }
                else if ($(td).find('input:text').length > 0) {
		            $(td).find('input:text').attr('original_value', '');
		            $(td).find('input:text').text('');
                    $(td).find('input:text').val('');
		        }
		        else if($(td).find('input:checkbox').length > 0) {
		            $(td).find('input:checkbox').attr('original_value', '');
		            $(td).find('input:checkbox').attr('checked', false);
		            $(td).find('input:checkbox').prop('checked', false);
		        }
                else if ($(td).children('input').length > 0) {
                    $(td).find('input').attr('original_value', '');
                    $(td).find('input').text('');
                    $(td).find('input').val('');
                }

		        else{
		            $(td).html('&nbsp;');
		        }
            });

            $(nRow).attr('fieldChanged', true);
            grdMD.Body.Rows[0].parentNode.insertBefore(nRow, grdMD.Body.Rows[0]);
		    //add delete button
		    $(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
            $(nRow).show();
            $('#buttonNew').prop('disabled', true);
            resizeFrame();

		}

		function buttonSave_click() {
		    save();
		}

		function buttonDelete_click(){;
            deleteSystem(_selectedId);
		}

		function row_click(row) {
		    if ($(row).attr('itemID')) {
		        _selectedId = $(row).attr('itemID');
		        $('#buttonDelete').attr('disabled', false);
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
		
		function ddl_change(ddl) {
		    var value = '', originalValue = '';
            value = $('option:selected', $(ddl)).text();
            sysId = $('option:selected', $(ddl)).val();

		    if ($(ddl).attr("original_value")) {
		        originalValue = $(ddl).attr("original_value");
		    }

		    if (value != originalValue) {
		        $(ddl).closest('tr').attr('changed','1');
		        $(ddl).attr("original_value", value);
		        populateRowValues(sysId, ddl);
		        activateSaveButton(ddl);
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
	        var url = 'Loading.aspx?Page=MDGrid_WorkArea_System.aspx?SystemID=' + id;

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}

		function populateRowValues(systemID, ddl){
		    if (systemID <= 0) //this is the unselected value case. Just return. 
		        return;
		    var data = _system_unused[0]; //pushed down from server. Contains javascrip encoded datatable of all unassigned systems. 
            var row = undefined;
		    for (var i = 0; i < data.length; i++){ //linear search for the right row. Slow, but the data set is small. 
		        if (data[i]["WTS_SystemID"] == systemID){
                    row = data[i];
		            break;
		        } 
		    }

            if (row) { //populate the values into the row in the html. This is so the save routine works as if it is a regular grid row loaded by the server. 
                var tr = $(ddl).closest('tr');
                var selected = $(ddl).find('option:selected').text();
                var ddlText = ddl.id + '_Text';
                $('td:eq(<%=DCC.IndexOf("WTS_SYSTEM")%>)', tr).val(row['WTS_SYSTEM']);
                $('td:eq(<%=DCC.IndexOf("WTS_SystemID")%>)', tr).val(row['WTS_SystemID']);
                $('#' + ddlText).val(selected);
		        $('td:eq(<%=DCC.IndexOf("DESCRIPTION")%>) input[type=text]', tr).val(row['DESCRIPTION']);
		        $('td:eq(<%=DCC.IndexOf("SORT_ORDER")%>) input[type=text]', tr).val(row['SORT_ORDER']);
		        $('td:eq(<%=DCC.IndexOf("ARCHIVE")%>) input[type=checkbox]', tr).prop('checked', row['ARCHIVE'] == 1 ? true : false); //technically this won't ever happen
		        return;
		    }
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

		        var idField = "WTS_SystemID";
		        var textField = "WTS_SYSTEM";
		        var data = _system_unused;

		        selectedVal = $(ddl).val();
		        selectedText = $(ddl).find('option:selected').text();
				
		        $('#lblMsg').show();
				
		        if (typeof data === 'undefined') {
		            return;
		        }
                if (!$(ddl).find('option:selected').val()) {
                    $(ddl).empty();
                }

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

        function resizeGrid() {
    setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
}
		
	</script>

	<script id="jsInit" type="text/javascript">

	    function initVariables() {
	        try {
	            _pageUrls = new PageURLs();

	            _canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
			    _isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

            } catch (e) {

            }
        }
		
        $(document).ready(function () {

            initVariables();
			
			
            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');
			
            $('#imgReport').hide();
            $('#imgExport').hide();
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
                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; })
            }

		    $('.gridBody').click(function (event) { row_click(this); });
		    $('.selectedRow').click(function (event) { row_click(this); });

		    $('select', $('#<%=this.grdMD.ClientID %>_Grid')).on('mousedown focus', function () { LoadList(this); });
			
            resizeFrame();
		});
	</script>
</asp:Content>