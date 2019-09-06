﻿<%@ Title="" Language="C#" EnableViewState="false" EnableEventValidation="false" AutoEventWireup="true" CodeFile="MDGrid_Image.aspx.cs" Inherits="MDGrid_Image" Theme="Default" MasterPageFile="~/Grids.master" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Image Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
             Image (<span id="spanRowCount" runat="server">0</span>)
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
				<input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" EmptyDataText="No data..." >
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
    <iframe id="frmDownload" style="display: none;"></iframe>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

	    var _pageURLs = new PageURLs();
	    var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
	    var _selectedId = 0;
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
				                changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + encodeURIComponent(GetColumnValue(row, i)) + '"');
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
                    refreshPage();
                }
            } catch (e) { }
        }

        function deleteItem(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);

                PageMethods.DeleteItem(parseInt(itemId), deleteItem_done, on_error);

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
			document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function imgExport_click() {
            window.location.href += '&Export=1';
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

        function deleteNewRow(img) {
            $(img).closest('tr').remove();
            resizeFrame();	
        }

        function buttonNew_click() {
            var url = 'Attachment_Edit.aspx'
                + window.location.search
                + '&Module=MDImage'
                + '&random=' + new Date().getTime();

            var filterPopup = popupManager.AddPopupWindow('AddNewImage', 'Add New Image'
                , 'Loading.aspx?Page=' + url, 300, 600, 'PopupWindow', window.self);
            if (filterPopup) {
                filterPopup.Open();
            }
        }

        function complete(result) {
            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                MessageBox('Image has been added.');
                refreshPage();
            }
            else {
                MessageBox('Failed to add. <br>' + errorMsg);
            }
        }

	    function buttonDelete_click(){
	        if (!_selectedId || _selectedId == '' || _selectedId == 0) {
	            MessageBox('You must specify an item to delete.');
	            return;
	        }
            
	        if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
	            deleteItem(_selectedId);
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

        function downloadImage(ImageID) {
            $('#frmDownload').attr('src', 'MDGrid_Image.aspx?ImageID=' + ImageID);
        }

		function row_click(row) {
		    if ($(row).attr('itemID')) {
		        _selectedId = $(row).attr('itemID');
                if (_canEdit) $('#buttonDelete').attr('disabled', false);
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
							|| !this.DCC.Contains("ImageName")) ? 0 : this.DCC["ImageName"].Ordinal %>)', row)[0];
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
        //TODO Handle code below with child
        function loadChildGrid(td, id) {
            var url = 'Loading.aspx?Page=';

            url += 'MDGrid_Image_CONTRACT.aspx';
            url += '?ImageID=' + id;

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
                $('input:text, textarea, input[type="number"]').on('change keyup mouseup', function () { txt_change(this); });
                $('input:checkbox').on('change', function () { txt_change(this); });

                $('#spnFiles').show();

                $('#buttonNew').attr('disabled', false);
                $('#buttonNew').click(function (event) { buttonNew_click(); return false; });

                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });

                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });
            }

            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });
			
            $(imgHelp).click(function () { MessageBox('Allowed file types:<br />.jpg<br />.jpeg<br />.png<br />.gif<br />'); });
            resizeFrame();
		});
	</script>
    </asp:Content>