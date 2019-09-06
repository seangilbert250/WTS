<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="Organization_Grid.aspx.cs" Inherits="Admin_Organization_Grid" Theme="Default" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
</asp:Content>
<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">Organizations</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" Runat="Server">Organizations</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server">
    <input type="checkbox" id="chkShowArchive" title="Show Archived data" value="Show Archive" style="vertical-align:middle; text-align:left;" /><label for="chkShowArchive" class="gridArchivedRow" style="vertical-align:middle;">Show Archived</label>
    <!-- Add, Edit and any other buttons -->
    <input type="button" id="buttonAdd" value="Add" style="vertical-align:middle; display:inline-block;" />
    <input type="button" id="buttonEdit" value="Edit" style="vertical-align:middle; display:inline-block;" />
    <input type="button" id="buttonView" value="View" style="vertical-align:middle; display:inline-block;" />
    <input type="button" id="buttonDelete" value="Delete" style="vertical-align:middle; display:inline-block;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<input type="hidden" id="txtSortField" runat="server" value="Organization" />
	<input type="hidden" id="txtSortDirection" runat="server" value="ASC" />
    <iti_Tools_Sharp:Grid ID="gridOrganizations" runat="server" AllowPaging="true" PageSize="30" AllowResize="true" CssClass="grid" BodyCssClass="gridBody" AlternatingRowColor="#dfdfdf" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="pageFooter" SelectedRowCssClass="selectedRow" />

	<div id="popupContainer" class="popupPageContainer"></div>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script type="text/javascript">
    	var _selectedId = 0;
    	var popupContainer = document.getElementById('popupContainer');
    	var popupManager = new PopupWindowManager(popupContainer);

        function chkShowArchive_change() {
            var show = false;
            show = $('#chkShowArchive').is(':checked');

            var cssClassList = 'gridBody,gridSelectedRow';
            showHideArchived(cssClassList, show);

            <%=this.gridOrganizations.ClientID %>.RedrawGrid();
        }

        function buttonAdd_click() {
            var url = 'Organization_AddEdit.aspx?New=true&random=' + new Date().getTime();

            var nPopup = popupManager.AddPopupWindow('AddOrganization', 'Add Organization', url, 475, 750, 'PopupWindow', this);
            if (nPopup) {
                nPopup.Open();
            }

            return false;
        }

        function buttonEdit_click(viewOnly) {
            if (_selectedId == 0 || _selectedId == undefined) {
                alert('Please select a row to view.');
                return false;
            }

            var url = 'Organization_AddEdit.aspx?'
                        + 'OrganizationID=' + _selectedId
                        + '&New=false'
                        + '&random=' + new Date().getTime()
            ;
            if (viewOnly != null && viewOnly == true) {
                url += '&View=true';
            }

            var nPopup = popupManager.AddPopupWindow('EditOrganization', 'Edit Organization', url, 475, 750, 'PopupWindow', this);
            if (nPopup) {
                nPopup.Open();
            }

            return false;
        }

        function buttonDelete_click() {
            if (_selectedId == 0 || _selectedId == undefined) {
                alert('Please select a row to delete.');
                return false;
            }

            if (!confirm('Are you sure you would like to permanently delete the selected Organization?\nThis cannot be undone.')) {
                return false;
            }

            ShowDimmer(true, "Deleting...", 1);

            try {
            	PageMethods.DeleteOrganization(+_selectedId, deleteOrganization_done, on_error);
            } catch (e) {
            	ShowDimmer(false);
            }

            return false;
        }

        function deleteOrganization_done(result) {
        	ShowDimmer(false);

        	try {
                var obj = jQuery.parseJSON(result);
                var exists = false, hasDependencies = false, deleted = false, archived = false;
                var errorMsg = '';

                if (obj) {
                	if (obj.Exists && obj.Exists.toUpperCase() == 'TRUE') {
                		exists = true;
                	}
                	if (obj.HasDependencies && obj.HasDependencies.toUpperCase() == 'TRUE') {
                		hasDependencies = true;
                	}
                	if (obj.Deleted && obj.Deleted.toUpperCase() == 'TRUE') {
                		deleted = true;
                	}
                	if (obj.Archived && obj.Archived.toUpperCase() == 'TRUE') {
                		archived = true;
                	}
                	if (obj.Error) {
                		errorMsg = obj.Error;
                	}
                }

                if (deleted) {
                    MessageBox('Successfully deleted Organization');
                    refreshGrid(false);
                }
                else if (archived) {
                    alert('The Organization is currently used for existing User(s). It has been archived.');
                    refreshGrid(false);
                }
                else {
                	MessageBox('Failed to delete Organization.  ' + errorMsg);
                }
            } catch (e) {
                //TODO: since the error is in the "done" procedure, log message instead of displaying
                refreshGrid(false);
            }
        }

        function on_error(result) {
        	ShowDimmer(false);

        	var resultText = 'An error occurred when communicating with the server';/*\n' +
            'readyState = ' + result.readyState + '\n' +
            'responseText = ' + result.responseText + '\n' +
            'status = ' + result.status + '\n' +
            'statusText = ' + result.statusText;*/

            alert('save error:  \n' + resultText);
        }

        function refreshGrid(exportGrid) {
            if (exportGrid === undefined || !exportGrid) {
                exportGrid = false;
            }
            var url = window.location.href;
            var showArchive = $('#chkShowArchive').is(':checked');
            url = editQueryStringValue(url, 'ShowArchived', showArchive.toString());
            url = editQueryStringValue(url, 'Export', exportGrid);

            window.location.href = url;
        }

        function imgSort_click() {
        	getSortOption();

        	return false;
        }
        function getSortOption() {
        	var url = 'SortOption.aspx?SortFields=' + '<%=this.SortFields %>' +
                '&SelectedSortField=' + $('#<%=this.txtSortField.ClientID %>').val() +
                '&SelectedSortDirection=' + $('#<%=this.txtSortDirection.ClientID %>').val();

			var nPopup = popupManager.AddPopupWindow('Sort Options', 'Sort Options', url, 70, 315, 'PopupWindow', this);
			if (nPopup) {
				nPopup.Open();
			}

			return false;
		}
		function getSortOption_done(sortOptions) {
			if (sortOptions == null) { return; }

			$('#<%=this.txtSortField.ClientID %>').val(sortOptions.Field);
            $('#<%=this.txtSortDirection.ClientID %>').val(sortOptions.Direction);

        	refreshGrid(false);
        }

        function row_click(row) {
            _selectedId = $(row).attr('OrganizationId');
            return false;
        } //end row_click


        function initializeEvents() {
        	try {
        		$('#imgExport').click(function (event) { refreshGrid(true); });
        		$('#tdSort').hide();
        		$('#imgRefresh').click(function () { refreshGrid(false); });

        		$('.gridBody').click(function (event) { row_click(this); });
        		$('.gridSelectedRow').click(function (event) { row_click(this); });

        		if ('<%=this.AllowEdit %>'.toUpperCase() == 'TRUE') {
            	$('#buttonAdd').click(function () { buttonAdd_click(); return false; });
            	$('#buttonEdit').click(function () { buttonEdit_click(); return false; });
            	$('#buttonEdit').show();
            	$('#buttonView').hide();
            }
            else {
            	$('#buttonAdd').attr('disabled', true);
            	$('#buttonEdit').attr('disabled', true);
            	$('#buttonEdit').hide();
            	$('#buttonView').click(function () { buttonEdit_click(true); return false; });
            	$('#buttonView').show();
            }

            if ('<%=this.AllowDelete %>'.toUpperCase() == 'TRUE') {
        			$('#buttonDelete').click(function () { buttonDelete_click(); return false; });
        		}
        		else {
        			$('#buttonDelete').hide();
        		}

        		$('#chkShowArchive').bind('change', function () {
        			chkShowArchive_change();
        			return false;
        		});
        	} catch (e) {

        	}
		}

        function temp_fixImages() {
        	try {
        		$('#imgSort').attr('src', '../Images/Icons/page_sort_a_z.png');
        		$('#imgExport').attr('src', '../Images/Icons/page_white_excel.png');
        		$('#imgReport').attr('src', '../Images/Icons/report.png');
        		$('#imgRefresh').attr('src', '../Images/Icons/arrow_refresh_blue.png');
        	} catch (e) {

        	}
        }

        $(document).ready(function () {
        	temp_fixImages();
        	initializeEvents();

            var showArchived = '<%=this.ShowArchived %>';
            $('#chkShowArchive').attr('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);
            $('#chkShowArchive').prop('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);
        });
	</script>
</asp:Content>

