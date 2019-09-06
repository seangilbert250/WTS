<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="NewsOverview.aspx.cs" Inherits="NewsOverview" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS News Overview</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">WTS News Overview</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server"></asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
        <tr>
            <td>
                <input type="button" id="btnAdd" value="Add" style="display: none;" />
                <input type="button" id="btnEdit" value="Edit" disabled="disabled" style="display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="display: none;" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="gridNewsOverviewContainer">
        <iti_Tools_Sharp:Grid ID="gridNewsOverview" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
            CssClass="grid" BodyCssClass="gridBody_News" SelectedRowCssClass="selectedRow_News" HeaderCssClass="gridHeader_News" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
        </iti_Tools_Sharp:Grid>
    </div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script type="text/javascript">
        var _selectedNewsId = 0;

        function refreshGrid() {
            document.location.href = 'Loading.aspx?Page=' + document.location.href;
        }

        function resizePage() {
            try {
                var heightModifier = 0;
                heightModifer += $('#mainPageFooter').height();

                resizePageElement('divPage', heightModifier + 2);
            }
            catch (e) {
                var m = e.message;
            }
        }

        function showHideNewsDetails(row, newsId) {
            //get message row and show/hide it
            var table = $(row).parent();
            var childRow = $(table).children('[parentNewsOverviewId=' + newsId + ']')

            if ($(childRow).is(":visible")) {
                $(childRow).hide();
            }
            else {
                $(childRow).show();
                //TODO: check if row is already read
                $(row).removeClass('unreadNews');
                $(row).css('font-weight', 'normal');
            }
        }

        function readArticle(row, newsId) {
            var data = '{"newsOverviewId":"' + newsId + '"}';

            var url = '<%=this.RootUrl %>' + '/ReadArticle';

            try {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: data,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: readArticle_done,
                    error: on_error
                });
            } catch (e) { }
        }

        function readArticle_done(result) {
            try {
                var obj = jQuery.parseJSON(result.d);
                var saved = '', error = '', id = '';

                $.each(obj, function (index, val) {
                    //do something with data
                    switch (index.toUpperCase()) {
                        case 'SAVED':
                            saved = val;
                            break;
                        case 'ERROR':
                            error = val;
                            break;
                        case 'ID':
                            id = val;
                            break;
                    }
                });
            } catch (e) { }
        }

        function on_error(result) {
            var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

            MessageBox('save error:  \n' + resultText);
        }

        function row_click(row) {
            if ($(row).attr('newsId')) {
                _selectedNewsId = $(row).attr('newsId');
                showHideNewsDetails(row, _selectedNewsId);

                $('#btnEdit').prop('disabled', false);
                $('#btnDelete').prop('disabled', false);
            }
        }

        function btnEdit_onclick() {
            try {
                if (_selectedNewsId == 0) {
                    MessageBox('Please select a row to edit');
                    return;
                }

                var openPopup = popupManager.AddPopupWindow('addEditNews', 'Edit News', 'Loading.aspx?Page=News_AddEdit.aspx?newsId=' + _selectedNewsId + '&newsTypeID=2' + '&random=' + Math.random(), 575, 700, 'PopupWindow', window.self);

                if (openPopup) {
                    openPopup.Open();
                }
            }
            catch (e) {
                //DisplayErrorMessage('btnEdit_onclick', e.number, e.message);
            }
        }

        function btnAdd_onclick() {
            try {
                _selectedNewsId = -1;
                var openPopup = popupManager.AddPopupWindow('addEditNews', 'Edit News', 'Loading.aspx?Page=News_AddEdit.aspx?newsId=' + _selectedNewsId + '&newsTypeID=2' + '&random=' + Math.random(), 575, 700, 'PopupWindow', window.self);

                if (openPopup) {
                    openPopup.Open();
                }
            }
            catch (e) {
                //DisplayErrorMessage('btnEdit_onclick', e.number, e.message);
            }
        }

        function btnDelete_onclick() {
            try {
                if (_selectedNewsId == 0) {
                    MessageBox('Please select a row to edit');
                    return;
                }
                PageMethods.DeleteNews(
                    _selectedNewsId, DeleteNews_done, on_error);

            } catch (ex) {

            }
        }

        function DeleteNews_done(result) {
            try {
                var obj = jQuery.parseJSON(result);

                if (obj.toUpperCase() == 'TRUE') {
                    MessageBox('News Article has been deleted. ');
                    refreshGrid();
                } else {
                    MessageBox('Failed to delete News Article.', 'Delete Failed');
                }
            } catch (ex) {

            }
        }

        function on_error() {
            try {

            } catch (ex) {

            }
        }

        function buttonDownload_onclick(attachmentId) {
            try {
                window.open('Download_Attachment.aspx?attachmentID=' + attachmentId);
            }
            catch (e) { }
        }

        function resizeGrid() {
                <%=this.gridNewsOverview.ClientID %>.ResizeGrid();
                <%--<%=this.gridNewsOverview.ClientID %>.ResizeGrid();--%>

        }

        function init_controls() {
            resizeGrid();
            $(<%=this.gridNewsOverview.ClientID %>.Body.Rows[0]).hide();
        }

        $(document).ready(function () {
            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#imgReport').hide();

            if ('<%=this.CanEdit %>'.toUpperCase() == 'TRUE') {
                $('#btnAdd').show();
                //$('#btnEdit').show();
                $('#btnDelete').show();
            }

            $('#imgRefresh').click(function () { refreshGrid(); });
            //$('#btnEdit').click(function () { btnEdit_onclick(); return false; });
            $('#btnAdd').click(function () { btnAdd_onclick(); return false; });
            $('#btnDelete').click(function () { btnDelete_onclick(); return false; });

            $("a").attr("target", "_blank");
            $('.gridBody_News').click(function (event) { row_click(this); });
            init_controls();
        });
    </script>
</asp:Content>
