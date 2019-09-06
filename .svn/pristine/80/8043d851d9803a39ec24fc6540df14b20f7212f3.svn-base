<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="Workload_Change_History.aspx.cs" Inherits="Workload_Change_History" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Workload Change History</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">History (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server"></asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <input type="button" id="btnDelete" value="Delete" style="vertical-align: middle; display: none; margin-right: 5px;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdChangeHistory" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _selectedHistoryID = 0;
    </script>

    <script type="text/javascript">
        function refreshPage() {
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'RefData', 1);

            document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function imgExport_click() {

        }

        function btnDelete_click() {
            QuestionBox('Confirm Product Version History Delete', 'Warning - Are you sure you want to delete this Product Version history? This cannot be undone and may adversely affect tracking from AOR.', 'Yes,No', 'confirmDelete', 300, 300, this);
        }

        function confirmDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.Delete('<%=this._type %>', _selectedHistoryID, delete_done, on_error);
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
                MessageBox('History has been deleted.');
                refreshPage();
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function row_click(obj) {
            var $obj = $(obj);
            var fieldChanged = $('td:eq(<%=this.FieldChangedIndex %>)', $obj).text();
            
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE' && $obj.attr('workitemhistory_id') && fieldChanged.toUpperCase() == 'PRODUCT VERSION') {
                _selectedHistoryID = $obj.attr('workitemhistory_id');

                $('#btnDelete').show();
            }
            else if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE' && $obj.attr('workitemtaskhistory_id') && fieldChanged.toUpperCase() == 'PRODUCT VERSION') {
                _selectedHistoryID = $obj.attr('workitemtaskhistory_id');

                $('#btnDelete').show();
            }
            else {
                _selectedHistoryID = 0;

                $('#btnDelete').hide();
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
        }

        function initEvents() {
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
        }

        $(document).ready(function () {
            initDisplay();
            initEvents();
        });
	</script>
</asp:Content>