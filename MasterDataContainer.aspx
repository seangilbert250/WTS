<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasterDataContainer.aspx.cs" Inherits="MasterDataContainer" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Master Data</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js" ></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divFrameContainer">
			<iframe id="frameGrid" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
			<iframe id="frameEdit" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
		</div>
	</form>

	<script type="text/javascript">
		var _newItemCreated = false;

		var _pageUrls = new PageURLs();
		var _mdType = 'Allocation';

		function ShowFrame(type, mdType, newItem, itemId, refresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			if (refresh == undefined || refresh == null || refresh == ''
				|| (mdType.toUpperCase() != _mdType.toUpperCase())) {
				refresh = true;
			}

			switch (type.toUpperCase()) {
				case 'GRID':
					ShowFrameForGrid(mdType, refresh);						
					break;
				case 'ADD':
				case 'EDIT':
				case 'COPY':
					ShowFrameEdit(mdType, newItem, itemId, refresh);
					break;
			}
		}

		function ShowFrameForGrid(mdType, refresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			if (refresh == undefined || refresh == null || refresh == ''
				|| $('#frameGrid').attr('src') == ''
				|| $('#frameGrid').attr('src') == "javascript:'';") {
				refresh = true;
			}

			if (_newItemCreated == true || _newItemCreated == 1) {
				_newItemCreated = false;
				MessageBox('Please click Get Data.');
			}

			if (refresh) {
				$('#frameGrid').attr('src', "javascript:'';");

				var pageUrl = getPageURL('GRID', mdType);
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + pageUrl
					+ search
					+ '&random=' + new Date().getTime();

				$('#frameGrid').attr('src', url);
			}

			$('#frameGrid').show();

			resizePage();
		}

		function ShowFrameForEdit(mdType, newItem, itemId, refresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			$("#frameEdit").attr("src", "javascript:'';");

			if (refresh == undefined || refresh == null || refresh == ''
				|| $('#frameEdit').attr('src') == ''
				|| $('#frameEdit').attr('src') == "javascript:'';") {
				refresh = true;
			}

			if (refresh) {
				var pageUrl = getPageURL('EDIT', mdType);

				$('#frameEdit').attr('src', "javascript:'';");
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + pageUrl
					+ search
					+ '&newItem=' + newItem
					+ '&itemID=' + itemId
				;

				$('#frameEdit').attr('src', url);
			}

			$('#frameEdit').show();

			resizePage();
		}

		function getPageURL(pageType, mdType) {
			var url = '';

			switch (mdType.toUpperCase()) {
				case _pageUrls.MasterData.MDType.AllocationCategory.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.AllocationCategory : _pageUrls.MasterData.Edit.AllocationCategory;
					break;
			    case _pageUrls.MasterData.MDType.AllocationGroup.toUpperCase():
			        url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.AllocationGroup : _pageUrls.MasterData.Edit.AllocationGroup;
			        break;
				case _pageUrls.MasterData.MDType.Allocation.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Allocation : _pageUrls.MasterData.Edit.Allocation;
                    break;
                case _pageUrls.MasterData.MDType.AOREstimation.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.AOREstimation : ''; //_pageUrls.MasterData.Edit.AOR_Type;
                    break;
                case _pageUrls.MasterData.MDType.AOR_Type.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.AOR_Type : _pageUrls.MasterData.Edit.AOR_Type;
                    break;
				case _pageUrls.MasterData.MDType.Contract.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Contract : _pageUrls.MasterData.Edit.Contract;
					break;
				case _pageUrls.MasterData.MDType.Effort.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Effort : _pageUrls.MasterData.Edit.Effort;
					break;
				case _pageUrls.MasterData.MDType.EffortArea.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.EffortArea : _pageUrls.MasterData.Edit.EffortArea;
					break;
				case _pageUrls.MasterData.MDType.EffortSize.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.EffortSize : _pageUrls.MasterData.Edit.EffortSize;
                    break;
                case _pageUrls.MasterData.MDType.Image.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Image : _pageUrls.MasterData.Edit.Image;
                    break;
                case _pageUrls.MasterData.MDType.Narrative.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Narrative : _pageUrls.MasterData.Edit.Narrative;
                    break;
				case _pageUrls.MasterData.MDType.Priority.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Priority : _pageUrls.MasterData.Edit.Priority;
					break;
				case _pageUrls.MasterData.MDType.PDD_TDR_Phase.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.PDD_TDR_Phase : _pageUrls.MasterData.Edit.PDD_TDR_Phase;
					break;
				case _pageUrls.MasterData.MDType.PDD_TDR_Status.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.PDD_TDR_Status : _pageUrls.MasterData.Edit.PDD_TDR_Status;
					break;
				case _pageUrls.MasterData.MDType.PDD_TDR_Progress.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.PDD_TDR_Progress : _pageUrls.MasterData.Edit.PDD_TDR_Progress;
					break;
				case _pageUrls.MasterData.MDType.PhaseWorkType.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.PhaseWorkType : _pageUrls.MasterData.Edit.PhaseWorkType;
					break;
				case _pageUrls.MasterData.MDType.ProductVersion.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.ProductVersion : _pageUrls.MasterData.Edit.ProductVersion;
					break;
				case _pageUrls.MasterData.MDType.Scope.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.Scope : _pageUrls.MasterData.Edit.Scope;
					break;
			    case _pageUrls.MasterData.MDType.SystemSuite.toUpperCase():
			        url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.SystemSuite : _pageUrls.MasterData.Edit.SystemSuite;
			        break;
				case _pageUrls.MasterData.MDType.System.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.System : _pageUrls.MasterData.Edit.System;
					break;
				case _pageUrls.MasterData.MDType.RequestGroup.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.RequestGroup : _pageUrls.MasterData.Edit.RequestGroup;
                    break;
                case _pageUrls.MasterData.MDType.RQMT_Type.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.RQMT_Type : _pageUrls.MasterData.Edit.RQMT_Type;
                    break;
                case _pageUrls.MasterData.MDType.RQMT_Description_Type.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.RQMT_Description_Type : _pageUrls.MasterData.Edit.RQMT_Description_Type;
                    break;
				case _pageUrls.MasterData.MDType.WorkArea.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkArea : _pageUrls.MasterData.Edit.WorkArea;
					break;
				case _pageUrls.MasterData.MDType.WorkloadGroup.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkloadGroup : _pageUrls.MasterData.Edit.WorkloadGroup;
                    break;
                case _pageUrls.MasterData.MDType.WorkloadAllocation.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkloadAllocation : _pageUrls.MasterData.Edit.WorkloadAllocation;
                    break;
				case _pageUrls.MasterData.MDType.WorkType.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkType : _pageUrls.MasterData.Edit.WorkType;
					break;
				case _pageUrls.MasterData.MDType.WorkTypePhase.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkTypePhase : _pageUrls.MasterData.Edit.WorkTypePhase;
					break;
				case _pageUrls.MasterData.MDType.WorkTypeStatus.toUpperCase():
					url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkTypeStatus : _pageUrls.MasterData.Edit.WorkTypeStatus;
					break;
				case 'CVT':
					url = pageType.toUpperCase() == 'GRID' ? 'MDGrid_CVT.aspx' : 'MDAddEdit_CVT.aspx';
					break;
			    case _pageUrls.MasterData.MDType.ItemType.toUpperCase():
			        url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.ItemType : _pageUrls.MasterData.Edit.ItemType;
                    break;
                case _pageUrls.MasterData.MDType.WorkActivityGroup.toUpperCase():
                    url = pageType.toUpperCase() == 'GRID' ? _pageUrls.MasterData.Grid.WorkActivityGroup : _pageUrls.MasterData.Edit.WorkActivityGroup;
                    break;

			}

			return url;
		}

		function resizePage() {
			$('#divFrameContainer iframe').each(function () {
				resizePageElement($(this).attr('id'), 0);
			});
		}

		$(document).ready(function () {
			$(window).resize(resizePage);

			_pageUrls = new PageURLs();
			_mdType = '<%=this.MDType %>';

			ShowFrame('GRID', _mdType, false, 0);

			resizePage();
		});
	</script>
</body>
</html>
