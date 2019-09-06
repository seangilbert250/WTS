﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Attachments.aspx.cs" Inherits="AOR_Attachments" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Attachment</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Attachments</span>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
			<td style="padding-left: 5px;">Type:
				<asp:DropDownList ID="ddlQF_Type" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 100px;">
					<asp:ListItem Text="ALL" Value="0" />
				</asp:DropDownList>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnAddPrevious" value="Copy from Previous Release" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <iframe id="frmDownload" style="display: none;"></iframe>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAORReleaseAttachmentID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnAddPrevious_click() {
            var nWindow = 'AddPreviousAORAttachment';
            var nTitle = 'Copy from Previous Attachment';
            var nHeight = 475, nWidth = 1250;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Previous Attachment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAdd_click() {
            var nWindow = 'AddAORAttachment';
            var nTitle = 'Add Attachment';
            var nHeight = 175, nWidth = 650;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Attachment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDelete_click() {
            QuestionBox('Confirm Attachment Delete', 'Are you sure you want to delete this Attachment?', 'Yes,No', 'confirmAORAttachmentDelete', 300, 300, this);
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

        function confirmAORAttachmentDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteAORAttachment(_selectedAORReleaseAttachmentID, delete_done, on_error);
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
                MessageBox('Attachment has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('textarea[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                        var $obj = $(this);

                        arrChanges.push({'aorreleaseattachmentid': $obj.attr('aorreleaseattachment_id'), 'field': $obj.attr('field'), 'value': $obj.val()});
                    });

                    if (arrChanges.length > 0) {
                        ShowDimmer(true, 'Saving...', 1);

                        var nJSON = '{update:' + JSON.stringify(arrChanges) + '}';

                        PageMethods.SaveChanges(nJSON, save_done, on_error);
                    }
                    else {
                        MessageBox('You have not made any changes.');
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                var fieldCount = $('textarea[fieldChanged="1"], select[fieldChanged="1"]').length;
                var rowCount = $('tr[rowChanged="1"]').length;

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];
            var $rows = $('#<%=this.grdData.ClientID %>_BodyContainer table tr').not(':first');
            var blnExit = false;

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table textarea[field="AOR Attachment Name"]'), function() {
                var nText = $(this).val();
                
                if (nText.length == 0) {
                    if ($.inArray('Attachment Name cannot be empty.', validation) == -1) validation.push('Attachment Name cannot be empty.');
                }
            });

            $.each($rows, function () {
				if (blnExit) return false;
				
				var typeID = $(this).find('td:eq(' + <%=this.DCC["AORAttachmentType_ID"].Ordinal %> + ')').text();
                var attachmentName = $(this).find('textarea[field="AOR Attachment Name"]').val();

				$.each($rows.not($(this)), function () {
				    if ($(this).find('td:eq(' + <%=this.DCC["AORAttachmentType_ID"].Ordinal %> + ')').text() == typeID && $(this).find('textarea[field="AOR Attachment Name"]').val() == attachmentName) {
				        validation.push('Type/Attachment Name cannot have duplicates.');
						blnExit = true;
						return false;
					}
				});
			});

            return validation.join('<br>');
        }

        function downloadAORAttachment(AORReleaseAttachmentID) {
            $('#frmDownload').attr('src', 'AOR_Attachments.aspx?Type=Attachment&AORReleaseAttachmentID=' + AORReleaseAttachmentID);
        }

        function approveAORAttachment(AORReleaseAttachmentID, approve) {
            PageMethods.ApproveAORAttachment(AORReleaseAttachmentID, approve, function (result) { approve_done(result, AORReleaseAttachmentID); }, on_error_approve);
        }

        function approve_done(result, AORReleaseAttachmentID) {
            var blnSaved = false;
            var errorMsg = '';
            var blnApproved = false;
            var approvedBy = '';
            var approvedDate = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
                if (obj.approved && obj.approved.toUpperCase() == 'TRUE') blnApproved = true;
                if (obj.approvedBy) approvedBy = obj.approvedBy;
                if (obj.approvedDate) approvedDate = obj.approvedDate;
            }

            if (blnSaved) {
                var nHTML = '';

                if (blnApproved) {
                    nHTML = '<span style="font-size: 10px; white-space: nowrap;">' + approvedBy.toLowerCase() + ' ' + approvedDate + '<br></span><a onclick="approveAORAttachment(\'' + AORReleaseAttachmentID + '\', \'False\'); return false;" href="javascript:void()">Reject</a>';
                }
                else {
                    nHTML = '<span style="font-size: 10px; white-space: nowrap;">&nbsp;</span><a onclick="approveAORAttachment(\'' + AORReleaseAttachmentID + '\', \'True\'); return false;" href="javascript:void()">Approve</a>';
                }

                $('#tdApproved' + AORReleaseAttachmentID).html(nHTML);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
                refreshPage(true);
            }
        }

        function on_error_approve() {
            MessageBox('An error has occurred.');
            refreshPage(true);
        }

        function row_click(obj) {
            _selectedAORReleaseAttachmentID = $(obj).attr('aorreleaseattachment_id');

            $('#btnDelete').prop('disabled', false);
        }

        function input_change(obj) {
            var $obj = $(obj);

            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'GridPageIndex', blnRetainPageIndex ? '<%=this.grdData.PageIndex %>' : '0');
            nURL = editQueryStringValue(nURL, 'TypeID', $('#<%=this.ddlQF_Type.ClientID %> option:selected').val());

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function ddl_change() {
            refreshPage(false);
        }

        function resizeGrid() {
            setTimeout(function() { <%=this.grdData.ClientID %>.ResizeGrid(); }, 1);

            //iti_Tools ResizeGrid() doesn't work sometimes in certain environments
            var $grid = $('#<%=this.grdData.ClientID %>_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#<%=this.grdData.ClientID %>_BodyContainer table').height();
            var pagerHeight = $('#<%=this.grdData.ClientID %>_PagerContainer').is(':visible') ? $('#<%=this.grdData.ClientID %>_PagerContainer').height() : 0;
            var bodyHeight = $('#<%=this.grdData.ClientID %>_BodyContainer').height(); 
            if (bodyTableHeight < bodyHeight) bodyHeight = bodyTableHeight - pagerHeight + 3;
            var pagerTop = headerTop + bodyHeight + pagerHeight - 5;
            $('#<%=this.grdData.ClientID %>_PagerContainer').css('top', pagerTop + 'px');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            $('#imgExport').hide();

            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('#btnAdd').show();

                if ('<%=this.PreviousExists%>'.toUpperCase() == 'TRUE') $('#btnAddPrevious').show();
                if ('<%=this.RowCount %>' != '0') $('#btnDelete').show();
                if ($('.saveable').length > 0) $('#btnSave').show();
            }

            resizeGrid();

            if (parent.updateTab) parent.updateTab('Attachments', <%=this.RowCount %>);
        }

        function initEvents() {
            $('#imgSort').click(function () { imgSort_click(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnAddPrevious').click(function () { btnAddPrevious_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            $('#<%=this.ddlQF_Type.ClientID %>').change(function () { ddl_change(); return false; });
        }

        $(document).ready(function () {
            if (parseInt('<%=this.AORReleaseAttachmentID %>') > 0) return;

            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>