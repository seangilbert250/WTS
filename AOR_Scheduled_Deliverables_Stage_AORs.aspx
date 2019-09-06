﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_Stage_AORs.aspx.cs" Inherits="AOR_Scheduled_Deliverables_Stage_AORs" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Deployment / AOR Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Deployment / AOR (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonNew" value="Add/Move" disabled="disabled" />
                <input type="button" id="buttonEdit" value="View/Edit" disabled="disabled" />
                <input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonDelete" value="Disassociate" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPageContents" style="width: 100%;">
		<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
			CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
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
        var _idxDelete = 0, _idxID = 0, _selectedId = 0, _selectedAOR = 0, _selectedAORRelease = 0;
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

        function validate() {
            var validation = [];
            var sum = 0;

            $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
                var weight = $(row).find('td:eq(' + <%=DCC.IndexOf("Weight") %> + ') input:first').val().trim().replace('', 0);

                if (i > 0) sum += parseInt(weight);
            });

            if (sum > 100) validation.push('Total Weight exceeds 100%.');

            return validation.join('<br>');
        }

        function save() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    ShowDimmer(true, "Updating...", 1);

                    var changedRows = [];

                    $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
                        var changedRow = [];
                        var changed = false;

                        //temp fix for errors/slow checking
                        var primaryIndex = <%=DCC.IndexOf("AORReleaseDeliverable_ID") %>;
                        var weightIndex = <%=DCC.IndexOf("Weight") %>;
                        if (_dcc[0].length > 0) {
                            for (var i = 0; i <= _dcc[0].length - 1; i++) {
                                if (i == weightIndex) {
                                    var newval = GetColumnValue(row, i);
                                    var oldval = GetColumnValue(row, i, true);
                                    if (newval != oldval) {
                                        changed = true;
                                        break;
                                    }
                                }
                            }
                            if (changed) {
                                for (var i = 0; i <= _dcc[0].length - 1; i++) {
                                    if (i == primaryIndex || i == weightIndex) {
                                        changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + GetColumnValue(row, i) + '"');
                                    }
                                }
                                var obj = '{' + changedRow.join(',') + '}';
                                changedRows.push(obj);
                            }
                        }
                    });

                    if (changedRows.length == 0) {
                        ShowDimmer(false);
                        MessageBox('You have not made any changes');
                    }
                    else {
                        var json = '[' + changedRows.join(",") + ']';
                        PageMethods.SaveChanges(json, save_done, on_error);
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
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

                if (saved > 0) {
                    msg = 'Successfully saved ' + saved + ' items.';
                    if (opener && opener.refreshPage) {
                        opener.refreshPage();
                    }
                }
                if (failed > 0) {
                    msg += '\n' + 'Failed to save ' + failed + ' items';
                }
                MessageBox(msg);

            } catch (e) { }
        }

        function confirmDeliverableAORDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Disassociating...', 1);

                    PageMethods.DeleteDeliverableAOR(_selectedId, delete_done, on_error);
                    if (parent.opener) parent.opener.refreshPage(true);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox('AOR/Release has been disassociated.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to disassociate. <br>' + errorMsg);
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
	</script>

	<script id="jsEvents" type="text/javascript">
		
		function refreshPage() {
			var qs = document.location.href;
            qs = editQueryStringValue(qs, 'RefData', 1);

            document.location.href = 'Loading.aspx?Page=' + qs;
            resizeFrame();
        }

		function imgExport_click() {

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
		}

		function buttonNew_click() {
            var nWindow = 'AddAOR';
            var nTitle = 'Add/Move AOR';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORPopup + '?&ReleaseID=' + <%=this.ReleaseID%> + '&DeliverableID=' + <%=this.ReleaseScheduleDeliverableID%> + '&Type=Add/Move Deployment AOR';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function buttonEdit_click() {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + _selectedAOR + '&AORReleaseID=' + _selectedAORRelease;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function buttonSave_click() {
            save();
        }

        function buttonDelete_click() {
            QuestionBox('Confirm AOR/Release Disassociation', 'Are you sure you want to disassociate this AOR/Release from this Deployment?', 'Yes,No', 'confirmDeliverableAORDelete', 300, 300, this);
        }

        function activateSaveButton() {
            if (_canEdit) $('#buttonSave').prop('disabled', false);
        }

        function txt_change(sender) {
            var original_value = '', new_value = '';
            if ($(sender).attr('original_value')) {
                original_value = $(sender).attr('original_value');
            }

            if ($(sender).attr('id') && $(sender).attr('id').indexOf('Weight') != -1) {
                var nVal = $(sender).val();

                nVal = nVal.replace(/[^\d,]/g, '');

                if (nVal > 100) nVal = 100;

                $(sender).val(nVal);
            }

            new_value = $(sender).val();

            if (new_value != original_value) {
                activateSaveButton();
            }
        }

        function row_click(row) {
            if (_canViewAOR) {
                if ($(row).attr('itemID')) {
                    _selectedId = $(row).attr('itemID');
                    _selectedAOR = $(row).attr('aorid');
                    _selectedAORRelease = $(row).attr('aorreleaseid');
                    $('#buttonEdit').prop('disabled', false);
                }
            }

            if (_canEdit) {
                if ($(row).attr('itemID')) {
                    _selectedId = $(row).attr('itemID');
                    _selectedAOR = $(row).attr('aorid');
                    _selectedAORRelease = $(row).attr('aorreleaseid');
                    $('#buttonDelete').attr('disabled', false);
                }
            }
		}

        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

        function openAOR(AORID, AORReleaseID) {
            var nWindow = 'AOR';
            var nTitle = 'AOR';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        var sum = 0;
        function calc_total_weight(obj) {
            sum = 0;
            var high = 0;
            var moderate = 0;
            var low = 0;
            var total_risk = '';

            $('#<%=this.grdMD.ClientID %>_BodyContainer').find('.gridBody, .selectedRow').each(function (i) {
                if (i > 0) {
                    if ($(this).find('td:eq(' + <%=this.DCC["Risk"].Ordinal %> + ')').text() == 'High (Emergency)') {
                        if ($(this).find('td:eq(' + 5 + ') input:first').val() != '') {
                            high += parseInt($(this).find('td:eq(' + <%=this.DCC["Weight"].Ordinal %> + ') input:first').val());
                        }
                    }

                    if ($(this).find('td:eq(' + <%=this.DCC["Risk"].Ordinal %> + ')').text() == 'Moderate (Acceptable)') {
                        if ($(this).find('td:eq(' + 5 + ') input:first').val() != '') {
                            moderate += parseInt($(this).find('td:eq(' + <%=this.DCC["Weight"].Ordinal %> + ') input:first').val());
                        }
                    }

                    if ($(this).find('td:eq(' + <%=this.DCC["Risk"].Ordinal %> + ')').text() == 'Low (Routine)') {
                        if ($(this).find('td:eq(' + 5 + ') input:first').val() != '') {
                            low += parseInt($(this).find('td:eq(' + <%=this.DCC["Weight"].Ordinal %> + ') input:first').val());
                        }
                    }
                }
            });

            sum = high + moderate + low;

            if (high > 40) {
                total_risk = 'High (Emergency)';
            }
            else if (high + moderate > 70) {
                total_risk = 'Moderate (Acceptable)';
            } else {
                total_risk = 'Low (Routine)'
            }

            $('#<%=this.grdMD.ClientID %>_Grid_Clone').find("tr[rowID='total'] :nth-child(3)").text(sum);
            $('#<%=this.grdMD.ClientID %>_Grid_Clone').find("tr[rowID='total'] :nth-child(2)").text(total_risk);
            $('#<%=this.grdMD.ClientID %>_BodyContainer').find("tr[rowID='total'] :nth-child(2)").text(total_risk);

            resizeGrid();

            parent.calcDeploymentRisk(<%=this.ReleaseScheduleDeliverableID%>, total_risk);
        }
	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				_canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
                _canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
				_canViewAOR = ('<%=this.CanViewAOR.ToString().ToUpper()%>' == 'TRUE');
				_isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');

				if (_dcc[0] && _dcc[0].length > 0) {
					_idxID = +'<%=this.DCC == null ? 0 : this.DCC["AORReleaseDeliverable_ID"].Ordinal %>';
				}
			} catch (e) {

			}
		}
		
		$(document).ready(function () {

			initVariables();
			
			$('.pageContentHeader').hide();
			
			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');
			
			$('#imgReport').hide();
			$('#imgExport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
            if (_canEdit) {
                $('input:text, textarea, input').on('change keyup mouseup', function () { txt_change(this); });
                $('input').on('blur', function () { calc_total_weight(this); });

				$('#buttonNew').attr('disabled', false);
				$('#buttonNew').click(function (event) { buttonNew_click(); return false; });
                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });
			}

            $('#buttonEdit').click(function (event) { buttonEdit_click(); return false; });
			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			resizeFrame();
		});
	</script>

</asp:Content>