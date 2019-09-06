<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SelectResources.ascx.cs" Inherits="Controls_SelectResources" %>
<div id="divSelectResourcesContainer" runat="server" style="width:100%;height:100%;overflow:hidden;background-color:#ffffff;position:absolute;display:none;">
    <div id="divSelectResourcesContainerButtons" runat="server" style="width:100%;padding:3px;border-bottom:1px solid #aaaaaa;" visible="false"></div>
    <div id="divSelectResourcesContainerResources" runat="server" style="width:100%;height:100%;overflow-y:auto;background-color:#ffffff;"></div>
</div>
<script type="text/javascript">
    function Show<%=this.ID%>Popup(clearChecks, defaultChecks) {
        var container = $('#<%=this.ContainerID%>');
        var buttons = $(container).children('[id*=divSelectResourcesContainerButtons]');
        var resources = $(container).children('[id*=divSelectResourcesContainerResources]');

        if (clearChecks) {
            $(resources).find('[name=cbSelectResource]').prop('checked', false);
        }

        if (defaultChecks) {
            var chkArr = defaultChecks.split(',');

            for (var i = 0; i < chkArr.length; i++) {
                var chkID = chkArr[i];

                if (chkID != null && chkID.length > 0) {
                    $(resources).find('[name=cbSelectResource][value=' + chkID + ']').prop('checked', true);
                }
            }
        }

        if (<%=IncludeSaveButton ? "true" : "false"%>) {
            if (<%=AllowSaveWithNoSelections ? "false" : "true"%>) {
                var checked = $(resources).find('[name=cbSelectResource]:checked');

                $(buttons).find('#<%=this.ID%>_SaveButton').prop('disabled', checked == null || checked.length == 0);
            }
            else {
                $(buttons).find('#<%=this.ID%>_SaveButton').prop('disabled', false);
            }
        }

        <%=this.ID%>SortCheckboxes();

        var nWindow = '<%=this.ID%>_Popup';
        var nTitle = '<%=(Title != null ? Title.Replace("'", "") : "Select Resources")%>';
        var nHeight = 350, nWidth = 200;
        var nURL = null;

        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=this.ContainerID%>');
            if (openPopup) openPopup.Open();  
    }

    function Close<%=this.ID%>Popup(save) {
        if (save) {
            if (<%=(SaveFunctionName != null ? "true" : "false")%>) {
                var container = $('#<%=this.ContainerID%>');
                var resources = $(container).children('[id*=divSelectResourcesContainerResources]');
                var checked = $(resources).find('[name=cbSelectResource]:checked');
                var notChecked = $(resources).find(':not([name=cbSelectResource]:checked)');
                var cJoin = $(checked).map(function () { return this.value }).get().join(',');
                var ncJoin = $(notChecked).map(function () { return this.value }).get().join(',');
                var checkedNames = $(checked).map(function () { return $(this).attr('rscname') }).get().join(',');
                var checkedEmails = $(checked).map(function () { return $(this).attr('email') }).get().join(',');
                var checkedCustAttr = (<%=(CustomReturnAttribute != null ? "true" : "false")%>) ? $(checked).map(function () { return $(this).attr('<%=CustomReturnAttribute%>') }).get().join(',') : '';
                
                <%=(SaveFunctionName != null ? SaveFunctionName : "_")%>(cJoin, ncJoin, checkedNames, checkedEmails, checkedCustAttr);
            }
        }

        popupManager.RemovePopupWindow('<%=this.ID%>_Popup');
    }

    function <%=this.ID%>CheckboxChanged(cb) {
        if (<%=(OnChangeFunctionName != null ? "true" : "false")%>) {
            <%=(OnChangeFunctionName != null ? OnChangeFunctionName : "_")%>($(cb).val(), $(cb).is(':checked'));
        }

        var container = $('#<%=this.ContainerID%>');
        var buttons = $(container).children('[id*=divSelectResourcesContainerButtons]');
        var resources = $(container).children('[id*=divSelectResourcesContainerResources]');

        if (<%=IncludeSaveButton ? "true" : "false"%>) {
            if (<%=AllowSaveWithNoSelections ? "false" : "true"%>) {
                var checked = $(resources).find('[name=cbSelectResource]:checked');

                $(buttons).find('#<%=this.ID%>_SaveButton').prop('disabled', checked == null || checked.length == 0);
            }
            else {
                $(buttons).find('#<%=this.ID%>_SaveButton').prop('disabled', false);
            }
        }

        <%=this.ID%>SortCheckboxes();
    }

    function <%=this.ID%>SortCheckboxes() {
        if (<%=(KeepCheckedResourcesOnTop ? "true" : "false")%>) {
            var container = $('#<%=this.ContainerID%>');
            var resources = $(container).children('[id*=divSelectResourcesContainerResources]');
            var rows = $(resources).find('[id*=<%=this.ID%>_selectrsctable]').find('tbody').children();
            var startRow = <%=(IncludeHeader ? 1 : 0)%>;

            var sorted = true;
            do {
                sorted = true;
                
                for (var i = startRow; i < rows.length - 1; i++) { // don't eval header row
                    var r1 = rows[i];
                    var r2 = rows[i + 1];

                    var check1 = $(r1).find('input');
                    var check2 = $(r2).find('input');

                    var c1 = $(check1).is(':checked');
                    var c2 = $(check2).is(':checked');

                    var r1OrigSort = parseInt($(r1).attr('origsort'));
                    var r2OrigSort = parseInt($(r2).attr('origsort'));

                    var swap = false;

                    if ((!c1 && c2) ||
                        (c1 && c2 && r1OrigSort > r2OrigSort) ||
                        (!c1 && !c2 && r1OrigSort > r2OrigSort)) {
                        swap = true;
                        sorted = false;
                    }

                    if (swap) {
                        rows[i] = r2;
                        rows[i + 1] = r1;
                        $(r2).detach().insertBefore(r1);
                    }
                }

            } while (!sorted);
        }
    }
</script>