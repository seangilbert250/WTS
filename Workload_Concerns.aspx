<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Workload_Concerns.aspx.cs" Inherits="Workload_Concerns" Theme="Default"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <!-- Copyright (c) 2015 Infinite Technologies, Inc. -->
	<title></title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divContents">
			<asp:GridView runat="server" ID="gridConcern" Style="width: 100%; border: none;" GridLines="None" AllowPaging="true" AllowSorting="false" PageSize="20" CellPadding="0" CellSpacing="0"
				CssClass="grid" RowStyle-CssClass="gridBody" HeaderStyle-CssClass="gridHeader" SelectedRowStyle-CssClass="selecteddrilldown" FooterStyle-CssClass="gridPager" PagerStyle-CssClass="gridPager">
			</asp:GridView>
	</div>

    <script type="text/javascript">


    </script>
    </form>
</body>
</html>
