﻿/*
PageURLs class
- contains URLs for specified pages
- sub-classes point to URLs grouped by module
*/
function PageURLs() {
    var _this = this;

    //popup window sizes
    this.PriTaskHeight = 700;
    this.PriTaskWidth = 1400;
    this.SubTaskHeight = 700;
    this.SubTaskWidth = 850;

    this.Home = 'Home.aspx';
    this.Dashboard = 'Dashboard.aspx';
    this.AORHomeTabs = 'AOR_Home_Tabs.aspx';
    this.AORHome = 'AOR_Home.aspx';
    this.AORSummary = 'AOR_Summary.aspx';
    this.AORSummaryPopup = 'AOR_Summary_Popup.aspx';
    this.News = 'News.aspx';
    this.Metrics = 'Metrics.aspx';
    this.CVTMetrics = 'CVTMetrics.aspx';
    this.RequestedReports = 'Reports.aspx';

    this.Maintenance = new function () {
        this.AORContainer = 'AOR_Maintenance_Container.aspx';
        this.AORGrid = 'AOR_Grid.aspx';
        this.AORWizard = 'AOR_Wizard.aspx';
        this.AORReleaseBuilder = 'AOR_Release_Builder.aspx';
        this.AORTabs = 'AOR_Tabs.aspx';
        this.AOREdit = 'AOR_Edit.aspx';
        this.AORPopup = 'AOR_Popup.aspx';
        this.AORAttachments = 'AOR_Attachments.aspx';
        this.AORCRs = 'AOR_CRs.aspx';
        this.AORTasks = 'AOR_Tasks.aspx';
        this.AORResourceTeam = 'AOR_Resource_Team.aspx';
        this.AORMeetingGrid = 'AOR_Meeting_Grid.aspx';
        this.AORMeetingTabs = 'AOR_Meeting_Tabs.aspx';
        this.AORMeetingEdit = 'AOR_Meeting_Edit.aspx';
        this.AORMeetingInstanceContainer = 'AOR_Meeting_Instance_Container.aspx';
        this.AORMeetingInstanceGrid = 'AOR_Meeting_Instance_Grid.aspx';
        this.AORMeetingInstanceEdit = 'AOR_Meeting_Instance_Edit.aspx';
        this.AORMeetingInstancePopup = 'AOR_Meeting_Instance_Popup.aspx';
        this.AORCRGrid = 'AOR_CR_Grid.aspx';
        this.AORCRTabs = 'AOR_CR_Tabs.aspx';
        this.AORCREdit = 'AOR_CR_Edit.aspx';
        this.AORCRNarratives = 'AOR_CR_Narratives.aspx';
        this.AORCRSRsTasks = 'AOR_SR_Grid.aspx';
        this.AORCRAORs = 'AOR_CR_AORs.aspx';
        this.AORScheduledDeliverables = 'AOR_Scheduled_Deliverables_Grid.aspx';
        this.AORScheduledDeliverablesEdit = 'AOR_Scheduled_Deliverables_Edit.aspx';
        this.AORScheduledDeliverablesAORs = 'AOR_Scheduled_Deliverables_AORs.aspx';
        this.AORScheduledDeliverablesContracts = 'AOR_Scheduled_Deliverables_Contracts.aspx';
        this.AORScheduledDeliverablesTabs = 'AOR_Scheduled_Deliverables_Tabs.aspx';
        this.AORReleaseAssessment = 'AOR_Release_Assessment_Grid.aspx';
        this.AORReleaseAssessmentEdit = 'AOR_Release_Assessment_Edit.aspx';
        this.AORReleaseAssessmentDeployment = 'AOR_Release_Assessment_Deployment.aspx';

        this.CRReportBuilder = 'CRReportBuilder.aspx';

        this.NarrativeAdd = 'MDGrid_Narrative_Add.aspx';

        this.RQMTParameterPage = 'RQMT_ParameterPage.aspx';
        this.RQMTContainer = 'RQMT_Maintenance_Container.aspx';
        this.RQMTGrid = 'RQMT_Grid.aspx';
        this.RQMTTabs = 'RQMT_Tabs.aspx';
        this.RQMTEdit = 'RQMT_Edit.aspx';
        this.RQMTBuilder = 'RQMTBuilder.aspx';
        this.RQMTDescriptionGrid = 'RQMTDescription_Grid.aspx';
        this.RQMTDescriptionPopup = 'RQMTDescription_Popup.aspx';

        this.SRContainer = 'SR_Maintenance_Container.aspx';
        this.SRGrid = 'SR_Grid.aspx';
        this.SRTabs = 'SR_Tabs.aspx';
        this.SREdit = 'SR_Edit.aspx';
        this.SRAttachments = 'SR_Attachments.aspx';

        this.MassChangeWizard = 'MassChange_Wizard.aspx';

        this.WorkRequestContainer = 'WorkRequestMaintenanceContainer.aspx';
    	this.WorkRequestGrid = 'WorkRequestGrid.aspx';
    	this.WorkRequestEditParent = 'WorkRequestEditParent.aspx';

    	this.HotlistGrid = 'Hotlist_Grid.aspx';
    	this.RequestGroupGrid_Requests = 'RequestGroupGrid_Requests.aspx';

    	this.WorkItemContainer = 'WorkItemMaintenanceContainer.aspx';
    	this.WorkItemGrid = 'WorkItemGrid.aspx';
    	this.WorkloadGrid_SRs = 'WorkloadGrid_SRs.aspx';
    	this.WorkloadGrid_WorkItems = 'WorkloadGrid_WorkItems.aspx';
    	this.WorkItemGrid_Tasks = 'WorkItemGrid_Tasks.aspx';
    	this.WorkItemGrid_QM = 'WorkItemGrid_QM.aspx';
    	this.WorkItemEditParent = 'WorkItemEditParent.aspx';
    	this.WorkItemDetails = 'WorkItem_Details.aspx';
    	this.WorkItemAttachments = 'WorkItem_Attachments.aspx';
    	this.WorkItemTasks = 'WorkItem_Tasks.aspx';
    	this.WorkItemHistory = 'Workload_Change_History.aspx';
    	this.WorkloadConcerns = 'Workload_Concerns.aspx';
    	this.WorkItemDetailsHelp = 'WorkItem_Details_Help.aspx';
    	this.TaskEditHelp = 'Task_Edit_Help.aspx';
    	this.CrosswalkContainer = 'CrosswalkMaintenanceContainer.aspx';

    	this.TaskAdd = 'Task_Add.aspx';
        this.TaskEdit = 'Task_Edit.aspx';

        this.SprintBuilder = 'SprintBuilder.aspx';
    };

    this.Reports = new function () {
    	this.Parameters = 'Report_Parameters.aspx';
    	this.Wizard = 'Report_Wizard.aspx';
    	this.AOR = 'Report_AOR.aspx';
        this.WorkLoad = "Report_WorkLoad.aspx"
        this.CR = 'Report_ParameterSelection.aspx?reporttype=2';
        this.Task = 'Report_ParameterSelection.aspx?reporttype=3';
        this.Release_DSE = 'Report_ParameterSelection.aspx?reporttype=4';
    };

    this.MasterData = new function () {
    	this.MaintenanceContainer = 'MasterDataContainer.aspx';

    	this.MDType = new function () {
    	    this.AllocationCategory = 'AllocationCategory';
    	    this.AllocationGroup = 'AllocationGroup';
            this.Allocation = 'Allocation';
            this.AOREstimation = 'AOREstimation';
            this.AOR_Type = 'AOR_Type';
    		this.Contract = 'Contract';
    		this.Effort = 'Effort';
    		this.EffortArea = 'EffortArea';
            this.EffortSize = 'EffortSize';
            this.Image = 'Image';
            this.Narrative = 'Narrative';
    		this.Priority = 'Priority';
    		this.PDD_TDR_Phase = 'PDD_TDR_Phase';
    		this.PDD_TDR_Status = 'PDD_TDR_Status';
    		this.PDD_TDR_Progress = 'PDD_TDR_Progress';
    		this.PhaseWorkType = 'PhaseWorkType';
    		this.ProductVersion = 'ProductVersion';
    		this.Scope = 'Scope';
    		this.SystemSuite = 'SystemSuite';
    		this.System = 'System';
            this.RequestGroup = 'RequestGroup';
            this.RQMT_Type = 'RQMT_Type';
            this.RQMT_Description_Type = 'RQMT_Description_Type';
    		this.WorkArea = 'WorkArea';
    		this.WorkloadAllocation = 'WorkloadAllocation';
            this.WorkloadGroup = 'WorkloadGroup';
    		this.WorkType = 'WorkType';
    		this.WorkTypePhase = 'WorkTypePhase';
    		this.WorkTypeStatus = 'WorkTypeStatus';
    		this.ItemType = 'ItemType';
            this.WorkActivityGroup = 'WorkActivityGroup';
    	}
    	this.Grid = new function () {
    	    this.AllocationCategory = 'MDGrid_AllocationCategory.aspx';
    	    this.AllocationGroup = 'MDGrid_AllocationGroup.aspx';
            this.Allocation = 'MDGrid_Allocation.aspx';
            this.AOREstimation = 'MDGrid_AOREstimation.aspx';
            this.AOR_Type = 'MDGrid_AOR_Type.aspx';
    		this.Contract = 'MDGrid_Contract.aspx';
    		this.Effort = 'MDGrid_Effort.aspx';
    		this.EffortArea = 'MDGrid_EffortArea.aspx';
            this.EffortSize = 'MDGrid_EffortSize.aspx';
            this.Image = 'MDGrid_Image.aspx';
            this.Narrative = 'MDGrid_Narrative.aspx';
    		this.Priority = 'MDGrid_Priority.aspx';
    		this.PDD_TDR_Phase = 'MDGrid_PDD_TDR_Phase.aspx';
    		this.PDD_TDR_Status = 'MDGrid_PDD_TDR_Status.aspx';
    		this.PDD_TDR_Progress = 'MDGrid_PDD_TDR_Progress.aspx';
    		this.PhaseWorkType = 'MDGrid_Phase_WorkType.aspx';
    		this.ProductVersion = 'MDGrid_ProductVersion.aspx';
    		this.Scope = 'MDGrid_Scope.aspx';
    		this.SystemSuite = 'MDGrid_SystemSuite.aspx';
    		this.System = 'MDGrid_System.aspx';
            this.RequestGroup = 'MDGrid_RequestGroup.aspx';
            this.RQMT_Type = 'MDGrid_RQMT_Type.aspx';
            this.RQMT_Description_Type = 'MDGrid_RQMT_Description_Type.aspx'
    		this.WorkArea = 'MDGrid_WorkArea.aspx';
    		this.WorkloadAllocation = 'MDGrid_WorkloadAllocation.aspx';
            this.WorkloadGroup = 'MDGrid_WorkloadGroup.aspx';
    		this.WorkType = 'MDGrid_WorkType.aspx';
    		this.WorkTypeStatus = 'MDGrid_WorkType_Phase.aspx';
    		this.WorkTypeStatus = 'MDGrid_WorkType_Status.aspx';
    		this.ItemType = 'MDGrid_ItemType.aspx';
            this.WorkActivityGroup = 'MDGrid_WorkActivityGroup.aspx';
    	}
    	this.Edit = new function () {
    		this.Allocation = 'MDAddEdit_AllocationCategory.aspx';
    		this.Allocation = 'MDAddEdit_Allocation.aspx';
            this.AllocationGroup = 'MDGrid_AllocationGroup.aspx';
            this.AOR_Type = 'MDAddEdit_AOR_Type.aspx';
    		this.AORResource = 'MDAddEdit_AORResource.aspx';
            this.Contract = 'MDAddEdit_Contract.aspx';
    		this.Effort = 'MDAddEdit_Effort.aspx';
    		this.EffortArea = 'MDAddEdit_EffortArea.aspx';
            this.EffortSize = 'MDAddEdit_EffortSize.aspx';
            this.Image = 'MDAddEdit_Image.aspx';
            this.Narrative = 'MDAddEdit_Narrative.aspx';
    		this.Priority = 'MDAddEdit_Priority.aspx';
    		this.PDD_TDR_Phase = 'MDAddEdit_PDD_TDR_Phase.aspx';
    		this.PDD_TDR_Status = 'MDAddEdit_PDD_TDR_Status.aspx';
    		this.PDD_TDR_Progress = 'MDAddEdit_PDD_TDR_Progress.aspx';
    		this.PhaseWorkType = 'MDAddEdit_Phase_WorkType.aspx';
    		this.ProductVersion = 'MDAddEdit_ProductVersion.aspx';
    		this.Scope = 'MDAddEdit_Scope.aspx';
    		this.System = 'MDAddEdit_System.aspx';
    		this.SystemResource = 'MDAddEdit_SystemResource.aspx';
            this.RequestGroup = 'MDAddEdit_RequestGroup.aspx';
            this.RQMT_Type = 'MDAddEdit_RQMT_Type.aspx';
    		this.WorkArea = 'MDAddEdit_WorkArea.aspx';
            this.WorkloadGroup = 'MDAddEdit_WorkloadGroup.aspx';
    		this.WorkType = 'MDAddEdit_WorkType.aspx';
    		this.WorkTypePhase = 'MDAddEdit_WorkType_Phase.aspx';
    		this.WorkTypeStatus = 'MDAddEdit_WorkType_Status.aspx';
    		this.ItemType = 'MDGrid_ItemType.aspx';
    		this.SystemSuite = 'MDGrid_SystemSuite.aspx';
    	}
    };

    this.Administration = new function () {
    	this.ResourceMGMTContainer = 'Admin/ResourceMGMTContainer.aspx';
    	this.UserMaintenanceContainer = 'Admin/UserMaintenanceContainer.aspx';
    	this.UserGrid = 'Admin/User_Grid.aspx';
    	this.UserEditParent = 'Admin/UserProfileEditParent.aspx';
    	this.UserDetails = 'Admin/UserProfile_AddEdit.aspx';
    	this.UserOptions = 'Admin/UserProfile_Options.aspx';
    	this.UserRoles = 'Admin/UserProfile_Roles.aspx';
    	this.UserCertifications = 'Admin/UserProfile_Certifications.aspx';
    	this.UserHardware = 'Admin/UserProfile_Hardware.aspx';

    	this.ResourceMGMTGrid = 'Admin/User_Grid.aspx';

    	this.OrganizationContainer = 'OrganizationContainer.aspx';
    	this.OrganizationGrid = 'Organization_Grid.aspx';
    	this.OrganizationDetails = 'Organization_AddEdit.aspx';
    };
}

function ShowDimmer(blnshow, strmsg, blnloader) {
    try {

        if (strmsg == undefined) {
            strmsg = "WTS is Applying your Filters...  Please wait...";
        }

        var strimgsrc = 'Images/Loaders/progress_bar_blue.gif';
        if (blnloader == 1) {
            strimgsrc = 'Images/Loaders/loader_2.gif';
        }

        if (blnshow) {

            var dimmer = div_dimmer();
            dimmer.addClass("WTS_loadingbox").appendTo($(document.body));

            var loader = $("<div>");
            $(loader).css("position", "absolute")
            .css("top", "30%")
            .css("left", "38%")
            .css("background-color", "white")
            .css("border", "solid 1px grey")
            .css("font-size", "18px")
            .css("text-align", "center")
            .css("padding", "10px")
            .css("z-index", "120")
            .addClass("WTS_loadingbox");

            var msg = $("<table>");
            $(msg).append("<tr><td style='text-align: center;'>" + strmsg + "</td></tr>")
            $(msg).append("<tr><td><img alt='' src='" + strimgsrc + "' /></td></tr>")

            $(msg).appendTo($(loader));
            $(loader).appendTo($(document.body));

            //fix position Left
            try {
                var bodywidth = $(document.body)[0].clientWidth;
                var loaderWidth = $(loader)[0].clientWidth;
                var bodyHeight = $(document.body)[0].clientHeight;
                var loaderHeight = $(loader)[0].clientHeight;

                if (bodywidth == undefined) { bodywidth = 0; }
                if (loaderWidth == undefined) { loaderWidth = 0; }
                var leftpos = parseInt(parseInt(bodywidth) / 2) - parseInt(parseInt(loaderWidth) / 2);
                if (leftpos < 0) { leftpos = 0; }
                leftpos = parseInt((leftpos / bodywidth) * 100);
                $(loader).css("left", leftpos + "%");

            } catch (e) {
            }

            //fix position Top
            try {
                var bodyHeight = $(document.body)[0].clientHeight;
                var loaderHeight = $(loader)[0].clientHeight;

                if (bodyHeight == undefined) { bodyHeight = 0; }
                if (loaderHeight == undefined) { loaderHeight = 0; }
                var toppos = parseInt(parseInt(bodyHeight) / 2) - parseInt(parseInt(loaderHeight) / 2);
                if (toppos < 0) { toppos = 0; }
                toppos = parseInt((toppos / bodyHeight) * 100);
                $(loader).css("top", toppos + "%");

            } catch (e) {
            }
        }
        else {
            $(".WTS_loadingbox").remove();
        }
    } catch (e) { }

}
function div_dimmer() {
	try {

		var dimmer = $("<div>");
		$(dimmer).css("position", "absolute")
        .css("top", "0px")
        .css("left", "0px")
        .css("height", "100%")
        .css("width", "100%")
        .css("background", "grey")
        .css("opacity", "0.6")
        .css("filter", "alpha(opacity = 60)")
        .css("z-index", "100")
        .addClass('Dimmer')
        .on("keydown", function (e) { e = e || event; if (e.keyCode == 27) { $(this).remove(); $(".PMMA_loadingbox").remove(); } });//ESC;

		return dimmer;

	} catch (e) { return null; }
}

//End PageURLs class



//Edit Query String
function editQueryStringValue(queryString, key, value) {
	var cSource = queryString.split('?');
	var existed = false;

	if (cSource.length == 0) {
		cSource.push('');
	}
	if (cSource.length == 1) {
		cSource.push('&' + key + '=' + value);
	}

	var cSearch = cSource[1].split('&');

	for (var i = 0; i <= cSearch.length - 1; i++) {
		var cField = cSearch[i].split('=')[0];
		var cParam = cSearch[i].split('=')[1];

		if (cField.toUpperCase() == key.toUpperCase()) {
			cParam = value;
			cSearch[i] = cField + '=' + cParam;
			existed = true;
			break;
		}
	}

	if (!existed) {//if the key is new add it
		cSearch.push(key + '=' + value);
	}

	cSource[1] = cSearch.join('&');

	var nLoc = cSource.join('?');

	return nLoc;
}

function getQueryStringParameter(name, defval) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? (defval != null && defval.length > 0 ? defval : '') : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

function getQueryStringValue(url, name, defval) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(url);
    return results === null ? (defval != null && defval.length > 0 ? defval : '') : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

//End Edit Query String

/*
Searches for specified class(es) and shows or hides object based on 'archived' attribute value 
and show = true/false parameter
*/
function showHideArchived(cssClassList, show) {
	var classes = new Array();
	classes = cssClassList.split(',');

	var c = '';
	for (var i = 0; i < classes.length; i++) {
		c = classes[i];
		if (c === null || c.length <= 0) {
			continue;
		}

		$('.' + c).each(function () {
			if ($(this).attr('archived') != null
                    && $(this).attr('archived') != undefined
                    && $(this).attr('archived').toUpperCase() == 'TRUE') {
				if (show) {
					$(this).show();
				}
				else {
					$(this).hide();
				}
			}
		});
	}

	return false;
}


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


function resizeFrame() {
    //Dont resize a container page. Only resize pages inside the container
    if (location.href.indexOf('MDGrid_') != -1 || location.href.indexOf('MasterDataContainer') != -1 || location.href.indexOf('AOR_Scheduled_Deliverables') != -1 || location.href.indexOf('GridType=Deployments') != -1) {
        //var pagerHeight = $('#<%=this.grdMD.ClientID %>_PagerContainer').is(':visible') ? $('#<%=this.grdMD.ClientID %>_PagerContainer').height() : 0;
		//Dont execute this on top level page inside container
        if (parent.location.href.indexOf('MasterDataContainer') < 0 && parent.location.href.indexOf('AOR_Maintenance_Container') < 0) {
            $('.pageContentHeader').hide();
            var $grid = $('#ctl00_ContentPlaceHolderBody_grdMD_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#ctl00_ContentPlaceHolderBody_grdMD_BodyContainer table').height();
            var pagerTableHeight = $('#ctl00_ContentPlaceHolderBody_grdMD_PagerContainer table').height();
            if ($('.ms-drop').is(':visible')) {
                var qfHeight = $('.ms-drop').height() + 75;

                if (bodyTableHeight < qfHeight) {
                    bodyTableHeight = qfHeight;
                }
            }

            var nHeight = headerTop + bodyTableHeight + pagerTableHeight + 1;

            if ($('#ui-datepicker-div').is(':visible')) {
                var datepickerHeight = $('#ui-datepicker-div').height();
                var datepickerTop = $('#ui-datepicker-div').offset().top;

                if (nHeight < datepickerHeight + datepickerTop) nHeight = datepickerHeight + datepickerTop + 5;
            }

            //Use quick filter container height if more than page height
            var $divQuickFilters = $('#divQuickFilters');
            if ($divQuickFilters.is(':visible')) {
                var quickFilterHeight = $divQuickFilters.height();
                if (quickFilterHeight > nHeight) {
                    nHeight = headerTop + quickFilterHeight + 10;
                }
            }

            var nFrame = getMyFrameFromParent();
            hFoo = "<%=HiddenFooClientID %>";
            $(nFrame).height(nHeight);
            resizeGrid();
            parent.resizeFrame();
        }
        else {
            var $grid = $('#ctl00_ContentPlaceHolderBody_grdMD_Grid');
            var headerTop = $grid[0].offsetTop;//itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#ctl00_ContentPlaceHolderBody_grdMD_BodyContainer table').height();
            var pagerTableHeight = $('#ctl00_ContentPlaceHolderBody_grdMD_PagerContainer table').height();
            resizeGrid();

            //iti_Tools ResizeGrid() doesn't work sometimes in certain environments
            var bodyHeight = $('#ctl00_ContentPlaceHolderBody_grdMD_BodyContainer').height();
            if (bodyTableHeight < bodyHeight) {
                bodyHeight = bodyTableHeight - 3;
            }
            var pagerTop = headerTop + bodyHeight + pagerTableHeight - 5;
            $('#ctl00_ContentPlaceHolderBody_grdMD_PagerContainer').css('top', pagerTop + 'px');
        }
    }
}
