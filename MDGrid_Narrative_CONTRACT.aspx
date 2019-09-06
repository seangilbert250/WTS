﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_Narrative_CONTRACT.aspx.cs" Inherits="MDGrid_Narrative_CONTRACT" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Narrative / Contracts Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Narrative / Contracts (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonNew" value="Add" disabled="disabled" />
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

        function deleteContract(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);
                PageMethods.DeleteChild(itemId, delete_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data.\n' + e.message);
            }
        }

        function delete_done(result) {
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
            resizeFrame();
            $('#buttonNew').prop('disabled', true);

		}

        function buttonDelete_click() {
            if (!_selectedId || _selectedId == '' || _selectedId == 0) {
                MessageBox('You must specify an item to delete.');
                return;
            }

            if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
                deleteContract(_selectedId);
            }
        }

		function row_click(row) {
		    if ($(row).attr('itemID')) {
		        _selectedId = $(row).attr('itemID');
		        $('#buttonDelete').attr('disabled', false);
		    }
		}

		function activateSaveButton(sender) {
            if (parent.parent.parent.$('#buttonSave')) {
                parent.parent.parent.$('#buttonSave').attr('disabled', false);
                parent.parent.parent.$('#buttonSave').prop('disabled', false);
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
		        //$(ddl).attr("original_value", value);
		        activateSaveButton(ddl);
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
                _idxDelete = parseInt('<%=this.DCC["Z"].Ordinal %>');
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
                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; })
            }

		    $('.gridBody').click(function (event) { row_click(this); });
		    $('.selectedRow').click(function (event) { row_click(this); });

            resizeFrame();
		});
	</script>
</asp:Content>