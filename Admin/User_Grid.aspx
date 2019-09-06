<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="User_Grid.aspx.cs" Inherits="User_Grid" Theme="Default" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
</asp:Content>
<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">Users</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" Runat="Server">Users (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="server">
    <div style="padding-left:4px;">
        Organization: <asp:DropDownList ID="ddlOrganizationFilter" runat="server" style="font-size:11px;"></asp:DropDownList>
        Search Name: <asp:TextBox ID="txtUserNameSearch" runat="server" style="font-size:11px;" />
    </div>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server">
    <input type="checkbox" id="chkShowArchive" title="Show Archived data" value="Show Archive" style="vertical-align:middle; text-align:left;" /><label for="chkShowArchive" class="gridArchivedRow" style="vertical-align:middle;">Show Archived</label>
    <!-- Add, Edit and any other buttons -->
    <input type="button" id="buttonAdd" value="Add" style="vertical-align:middle; display:inline-block;" />
    <input type="button" id="buttonEdit" value="Edit" style="vertical-align:middle; display:none;" />
    <input type="button" id="buttonView" value="View" style="vertical-align:middle; display:none;" />
    <input type="button" id="buttonDelete" value="Delete" style="vertical-align:middle; display:inline-block;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
    <input type="hidden" id="txtSortField" runat="server" value="UserName" />
    <input type="hidden" id="txtSortDirection" runat="server" value="ASC" />
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<iti_Tools_Sharp:Grid ID="gridUsers" runat="server" AllowPaging="false" PageSize="30" AllowResize="true"  CssClass="grid" BodyCssClass="gridBody" AlternatingRowColor="#dfdfdf" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="pageFooter" SelectedRowCssClass="selectedRow" />

	<div id="popupContainer" class="popupPageContainer"></div>

    <script type="text/javascript">
    	var _selectedId = 0;
    	var popupContainer = document.getElementById('popupContainer');
    	var popupManager = new PopupWindowManager(popupContainer);

        function chkShowArchive_change() {
            var show = false;
            show = $('#chkShowArchive').is(':checked');

            var cssClassList = 'gridBody,gridSelectedRow';
            showHideArchived(cssClassList, show);

            <%=this.gridUsers.ClientID %>.RedrawGrid();
        }

        function buttonAdd_click() {
        	var url = 'UserProfileEditParent.aspx?New=true&popup=true&random=' + new Date().getTime();

            var nPopup = popupManager.AddPopupWindow('AddUser', 'Add User', url, 650, 1050, 'PopupWindow', this);
            if (nPopup) {
                nPopup.Open();
            }

            return false;
        }

        function buttonEdit_click(viewOnly) {
        	if (_selectedId == 0 || _selectedId == undefined) {
                alert('Please select a row to edit.');
                return false;
            }

        	var url = 'UserProfileEditParent.aspx?' +
                '&UserID=' + _selectedId + 
                '&New=false' +
                '&popup=true' + 
                '&random=' + new Date().getTime()
            ;
            if (viewOnly != null && viewOnly == true) {
                url += '&View=true';
            }
            //window.open(url);
            var nPopup = popupManager.AddPopupWindow('EditUser', 'Edit User', url, 850, 1050, 'PopupWindow', this);
            if (nPopup) {
                nPopup.Open();
            }

            return false;
        }
		
        function lbEdit_click(id) {
        	var url = 'UserProfileEditParent.aspx?' +
                '&UserID=' + id + 
                '&New=false' +
                '&popup=true' + 
                '&random=' + new Date().getTime()
        	;
        	//window.open(url);
        	var nPopup = popupManager.AddPopupWindow('EditUser', 'Edit User', url, 850, 1050, 'PopupWindow', this);
        	if (nPopup) {
        		nPopup.Open();
        	}

        	return false;
        }

        function buttonDelete_click() {
        	if (_selectedId == undefined || _selectedId == 0) {
                alert('Please select a row to delete.');
                return false;
            }
            
            if (!confirm('Are you sure you would like to permanently delete the selected user?\nThis cannot be undone.')) {
                return false;
            }

            ShowDimmer(true, "Deleting...", 1);

        	try {
        		PageMethods.DeleteUser(+_selectedId, deleteUser_done, on_error);
        	} catch (e) {
        		ShowDimmer(false);
        	}

            return false;
        }

        function deleteUser_done(result) {
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
                	MessageBox('Successfully deleted user profile');
                    refreshGrid(false);
                }
                else if (archived) {
                	alert('The user is assigned to an item. They have been archived.');
                    refreshGrid(false);
                }
                else {
                	MessageBox('Failed to delete user profile.  ' + errorMsg);
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

            MessageBox('save error:  \n' + resultText);
        }

        function refreshGrid(exportGrid) {
            if (exportGrid === undefined || !exportGrid) {
                exportGrid = false;
            }
            var url = window.location.href;
            var showArchive = $('#chkShowArchive').is(':checked');
            url = editQueryStringValue(url, 'ShowArchived', showArchive.toString());
            url = editQueryStringValue(url, 'OrganizationID', $('#<%=this.ddlOrganizationFilter.ClientID %> option:selected').val());
            url = editQueryStringValue(url, 'UserNameSearch', $('#<%=this.txtUserNameSearch.ClientID %>').val());
            url = editQueryStringValue(url, 'SortField', $('#<%=this.txtSortField.ClientID %>').val());
            url = editQueryStringValue(url, 'SortDirection', $('#<%=this.txtSortDirection.ClientID %>').val());
            url = editQueryStringValue(url, 'Export', exportGrid);

            window.location.href = url;
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

				var sURL = '../SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
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

        function row_click(row) {
            _selectedId = $(row).attr('userId');
            return false;
        } //end row_click


        function initializeEvents() {
        	try {
        		$('#imgExport').bind('click', function (event) { refreshGrid(true); });
        		$('#imgSort').bind('click', function (event) { imgSort_click(); });
        		$('#imgRefresh').bind('click', function () { refreshGrid(false); });

        		$('.gridBody').bind('click', function (event) { row_click(this); });
        		$('.gridSelectedRow').bind('click', function (event) { row_click(this); });

        		if ('<%=this.AllowEdit %>'.toUpperCase() == 'TRUE') {
        			$('#buttonAdd').click(function () { buttonAdd_click(); return false; });
        			//$('#buttonEdit').click(function () { buttonEdit_click(); return false; });
        			//$('#buttonEdit').show();
        			//$('#buttonView').hide();
        		}
        		else {
        			$('#buttonAdd').attr('disabled', true);
        			//$('#buttonEdit').attr('disabled', true);
        			//$('#buttonEdit').hide();
        			//$('#buttonView').bind('click', function () { buttonEdit_click(true); });
        			//$('#buttonView').show();
        		}

        		if ('<%=this.AllowDelete %>'.toUpperCase() == 'TRUE') {
					$('#buttonDelete').bind('click', function () { buttonDelete_click(); });
				}
				else {
					$('#buttonDelete').hide();
				}

				$('#chkShowArchive').bind('change', function () {
					chkShowArchive_change();
					return false;
				});

				$('#<%=this.ddlOrganizationFilter.ClientID %>').bind('change', function (event) {
        			refreshGrid(false);
        		});
        		$('#<%=this.txtUserNameSearch.ClientID %>').bind('keypress', function (event) {
					if (event.which == 13) {
						event.preventDefault();
						refreshGrid(false);
					}
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
        	initializeEvents();
        	temp_fixImages();
            
            var showArchived = '<%=this.ShowArchived %>';
            $('#chkShowArchive').attr('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);
            $('#chkShowArchive').prop('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);

        });
	</script>
</asp:Content>