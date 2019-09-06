/*
	Item represents WorkItem database object
	**This needs to match the properties specified in WorkloadItem.cs
*/
function WorkloadItem() {
	this.WorkItemID = 0;
	this.BugTracker_ID = 0;
	this.WorkRequestID = 0;
	this.WorkItemTypeID = 0;
	this.WTS_SystemID = 0;
	this.ProductVersionID = 0;
	this.Production = false;
	this.SR_Number = 0;
	this.Reproduced_Biz = false;
	this.Reproduced_Dev = false;
	this.PriorityID = 0;
	this.AllocationID = 0;
	this.MenuTypeID = 0;
	this.MenuNameID = 0;
	this.SubmittedByID = 0;
	this.AssignedResourceID = 0;
	this.PrimaryResourceID = 0;
	this.ResourcePriorityRank = 0;
	this.SecondaryResourceID = 0;
	this.SecondaryResourceRank = 0;
	this.PrimaryBusinessResourceID = 0;
	this.SecondaryBusinessResourceID = 0;
	this.PrimaryBusinessRank = 0;
	this.SecondaryBusinessRank = 0;
	this.WorkTypeID = 0;
	this.StatusID = 0;
	this.IVTRequired = false;
	this.NeedDate = '';
	this.EstimatedHours = 0;
	this.EstimatedCompletionDate = '';
	this.CompletionPercent = 0;
	this.WorkAreaID = 0;
	this.WorkloadGroupID = 0;
	this.Title = '';
	this.Description = '';
	this.Archive = false;
	this.Deployed_Comm = false;
	this.Deployed_Test = false;
	this.Deployed_Prod = false;
	this.DeployedBy_CommID = 0;
	this.DeployedBy_TestID = 0;
	this.DeployedBy_ProdID = 0;
	this.DeployedDate_Comm = '';
	this.DeployedDate_Test = '';
	this.DeployedDate_Prod = '';
	this.PlannedDesignStart = '';
	this.PlannedDevStart = '';
	this.ActualDesignStart = '';
	this.ActualDevStart = '';
	this.CVTStep = '';
	this.CVTStatus = '';
	this.TesterID = 0;
}


/*
	Item represents WorkRequest database object
	**This needs to match the properties specified in WorkloadItem.cs for WorkRequest
*/
function WorkRequest() {
	this.WORKREQUESTID = 0;
	this.REQUESTTYPEID = 0;
	this.REQUESTGROUPID = 0;
	this.CONTRACTID = 0;
	this.ORGANIZATIONID = 0;
	this.WTS_SCOPEID = 0;
	this.EFFORTID = 0;
	this.PROGRESSID = 0;  // Added to local 10-21-2016
	this.SMEID = 0;
	this.LEAD_IA_TWID = 0;
	this.LEAD_RESOURCEID = 0;
	this.OP_PRIORITYID = 0;
	this.Title = '';
	this.Description = '';
	this.Justification = '';
	this.Archive = false;
	this.SubmittedByID = 0;
}