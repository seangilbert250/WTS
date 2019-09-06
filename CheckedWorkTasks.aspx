<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CheckedWorkTasks.aspx.cs" Inherits="CheckedWorkTasks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AOR</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="button" id="buttonNew" value="Add" />
        <input type="button" id="buttonRem" value="Remove" />
        <div>
             <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="3" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	        </iti_Tools_Sharp:Grid>
        </div>
    </form>
     <script id="jsInit" type="text/javascript">

         function buttonRem_click() {
             var a = 3;
             $(grdData.Body.Grid).find('tr').last().remove();
             
         }



         function buttonNew_click(startDate, endDate, duration, first, last) {
			//var grdData = <%=this.grdData.ClientID%>;
			
            
			/*$(nRow.cells[_idxID]).text('0');//.innerText = '0';
			$(nRow.cells).each(function(i, td){
				if($(td).find('input:text').length > 0) {
					$(td).find('input:text').attr('original_value', '');
					$(td).find('input:text').text('');
                    $(td).find('input:text').val('');
                }
                else if ($(td).find('textarea').length > 0) {
                    $(td).find('textarea').attr('original_value', '');
                    $(td).find('textarea').val('');
                }
				else if($(td).find('input:checkbox').length > 0) {
					$(td).find('input:checkbox').attr('original_value', '');
					$(td).find('input:checkbox').attr('checked', false);
					$(td).find('input:checkbox').prop('checked', false);
				}
				else if($(td).children('input').length > 0) {
					$(td).find('input').attr('original_value', '');
					$(td).find('input').text('');
					$(td).find('input').val('');
				}
				else if($(td).children('select').length > 0) {
					$(td).find('select').attr('original_value', '');
                }
				else{
					$(td).html('&nbsp;');
                }
            });

            $(nRow).find('input[id*="txtStartDate"]').attr('id', $(nRow).find('input[id*="txtStartDate"]').attr('id') + datepickerids);
            $(nRow).find('input[id*="txtEndDate"]').attr('id', $(nRow).find('input[id*="txtEndDate"]').attr('id') + datepickerids);
            datepickerids++;

            $(nRow).find('input[id*="txtStartDate"]').datepicker({
                showAnim: ""
                , changeMonth: true
                , showOtherMonths: true
                , selectOtherMonths: true
                , changeYear: true
                , onSelect: function () {
                    resizeFrame();
                    activateSaveButton($(this));
                    updateDuration($(this));
                }
                , onClose: function () { resizeFrame(); }
            }).click(function () { resizeFrame(); }).focus(function () { resizeFrame(); });

            $(nRow).find('input[id*="txtEndDate"]').datepicker({
                showAnim: ""
                , changeMonth: true
                , showOtherMonths: true
                , selectOtherMonths: true
                , changeYear: true
                , onSelect: function () {
                        resizeFrame();
                        activateSaveButton($(this));
                        updateDuration($(this));
                    }
                    , onClose: function () { resizeFrame(); }
                }).click(function () { resizeFrame(); }).focus(function () { resizeFrame(); });

            $(nRow).find('input[id*="txtDuration"]').on('change', function () { updateEndDate($(this)); });

            if (startDate && endDate && duration) {
                $(nRow).find('input[id*="txtReleaseSession"]').val('Session ' + startDate + ' - ' + endDate);
                $(nRow).find('input[id*="txtStartDate"]').val($.datepicker.formatDate('mm/dd/yy', new Date(startDate)));
                if (first || last) updateDuration($(nRow).find('input[id*="txtStartDate"]'));
                $(nRow).find('input[id*="txtEndDate"]').val($.datepicker.formatDate('mm/dd/yy', new Date(endDate)));
                if (first || last) updateDuration($(nRow).find('input[id*="txtEndDate"]'));
                $(nRow).find('input[id*="txtDuration"]').val(duration);
            }

            $(nRow).attr('fieldChanged', true);*/
            //debugger;
            var nRow = grdData.Body.Rows[0].cloneNode(true);
            $(nRow).insertAfter(grdData.Body.Rows[0]);
			//grdMD.Body.Rows[0].parentNode.insertBefore(nRow,grdMD.Body.Rows[0]);
			//add delete button
			//$(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
            $(nRow).show();
            //resizeFrame();
        }

        function initVariables() {

        }

        function initDisplay() {
            <%=this.grdData.ClientID %>.ResizeGrid();
            
         }

         $(document).ready(function () {
            initVariables();
             initDisplay();
             $('#buttonNew').click(function (event) { buttonNew_click(); return false; });
             $('#buttonRem').click(function (event) { buttonRem_click(); return false; });
        });
    </script>
</body>
</html>
