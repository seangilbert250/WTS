<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Apply_Filters.aspx.cs" Inherits="Apply_Filters" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <script type="text/javascript">
        if (parent.applyFilters_Done) {
            var blnSkipLoad = false;
            var blnMetrics = false;
            if ('<%=Request.Form["blnSkipLoad"] %>' == 'true') blnSkipLoad = true;
            if ('<%=Request.Form["metrics"] %>' == 'true') blnMetrics = true;
            parent.applyFilters_Done(blnMetrics, blnSkipLoad);
        }
    </script>
    </form>
</body>
</html>
