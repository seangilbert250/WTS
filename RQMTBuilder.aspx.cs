﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WTS;

public partial class RQMTBuilder : WTSContentPage
{
    #region Variables
    protected bool CanEditRQMT = false;
    protected bool CanViewRQMT = false;
    protected string Title = string.Empty;
    protected string Type = string.Empty;
    protected string DescriptionTypeOptions = string.Empty;
    private DataColumnCollection DCC;
    protected string DefaultRQMTSystemIDs = string.Empty;
    protected int DefaultRQMTSet_RQMTSystemID = 0;
    protected int DefaultSystemSuiteID = 0;
    protected int DefaultSystemID = 0;
    protected int DefaultWorkAreaID = 0;
    protected int DefaultRQMTTypeID = 0;
    protected int DefaultRQMTSetID = 0;
    protected string DefaultRQMTSetName = "";
    protected int DefaultRQMTID = 0;

    protected string CustomLists = string.Empty;

    protected string ComplexityOptions = string.Empty;
    protected string SuiteOptions = string.Empty;
    protected string SystemOptions = string.Empty;
    protected string WorkAreaOptions = string.Empty;
    protected string RQMTTypeOptions = string.Empty;
    protected string FunctionalitySelectOptions = string.Empty;
    protected string FunctionalityCheckBoxOptions = string.Empty;
    protected string dtSystemsJSON = string.Empty;
    protected string dtWorkAreaSystemsJSON = string.Empty;

    protected Dictionary<string, string> AttributeOptions;
    protected string CopiedRQMTs = string.Empty;
    protected string CopiedRQMTSystems = string.Empty;
    protected bool QuickAddWarningEnabled = true;

    #endregion

    #region Page
    private void Page_Load(object sender, EventArgs e)
    {
        ReadQueryString();
        InitializeEvents();

        this.CanEditRQMT = UserManagement.UserCanEdit(WTSModuleOption.RQMT);
        this.CanViewRQMT = this.CanEditRQMT || UserManagement.UserCanView(WTSModuleOption.RQMT);

        switch (this.Type)
        {
            case "Add":
            case "Edit":
                this.Title = "Add/Edit a Requirement Set";
                break;
        }

        LoadControls();

        CustomLists = FillCustomLists();
    }

    private void ReadQueryString()
    {
        if (Request.QueryString["Type"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Type"]))
        {
            this.Type = Request.QueryString["Type"];
        }

        Int32.TryParse(Request.QueryString["SystemSuiteID"], out DefaultSystemSuiteID);
        Int32.TryParse(Request.QueryString["WTS_SYSTEMID"], out DefaultSystemID);
        Int32.TryParse(Request.QueryString["WorkAreaID"], out DefaultWorkAreaID);
        Int32.TryParse(Request.QueryString["RQMTTypeID"], out DefaultRQMTTypeID);
        Int32.TryParse(Request.QueryString["RQMTSetRQMTSystemID"], out DefaultRQMTSet_RQMTSystemID);
        Int32.TryParse(Request.QueryString["RQMTID"], out DefaultRQMTID);
        Int32.TryParse(Request.QueryString["RQMTSetID"], out DefaultRQMTSetID);

        if (DefaultRQMTSet_RQMTSystemID != 0)
        {
            // note, this data set contains the following tables:
            // [0] = RQMT
            // [1] = DESC
            // [2] = FUNC
            DataSet ds = RQMT.RQMTBuilder_Data_Get(0, null, 0, 0, 0, DefaultRQMTSet_RQMTSystemID);

            if (ds.Tables.Count > 0 && ds.Tables["RQMT"].Rows.Count > 0)
            {
                DataRow row = ds.Tables["RQMT"].Rows[0];

                DefaultSystemID = (int)row["WTS_SYSTEMID"];
                DefaultWorkAreaID = (int)row["WorkAreaID"];
                DefaultSystemSuiteID = (int)row["WTS_SYSTEM_SUITEID"];
                DefaultRQMTTypeID = (int)row["RQMTTypeID"];
                DefaultRQMTSetID = (int)row["RQMTSetID"];
                DefaultRQMTSetName = (string)row["RQMTSetName"];
            }
        }
        else if (DefaultRQMTSetID != 0)
        {
            DataSet ds = RQMT.RQMTBuilder_Data_Get(DefaultRQMTSetID, null, 0, 0, 0, 0);

            if (ds.Tables.Count > 0 && ds.Tables["RQMT"].Rows.Count > 0)
            {
                DataRow row = ds.Tables["RQMT"].Rows[0];

                DefaultSystemID = (int)row["WTS_SYSTEMID"];
                DefaultWorkAreaID = (int)row["WorkAreaID"];
                DefaultSystemSuiteID = (int)row["WTS_SYSTEM_SUITEID"];
                DefaultRQMTTypeID = (int)row["RQMTTypeID"];
                DefaultRQMTSetName = (string)row["RQMTSetName"];
            }
        }
        else if (DefaultSystemSuiteID == 0 && DefaultSystemID != 0)
        {
            DataTable dt = MasterData.WorkArea_SystemList_Get(DefaultWorkAreaID, DefaultSystemID);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int wasysid = (int)row["WorkArea_SystemID"];

                    if (wasysid != 0)
                    {
                        DefaultSystemSuiteID = (int)row["WTS_SYSTEM_SUITEID"];

                        break;
                    }
                }
            }
        }
    }

    private void InitializeEvents()
    {

    }
    #endregion

    #region Data
    private void LoadControls()
    {
        // suites ddl
        DataTable dtSuites = MasterData.SystemSuiteList_Get(0);
        PopulateDDLFromDataTable(ddlSuite, dtSuites, "WTS_SYSTEM_SUITEID", "WTS_SYSTEM_SUITE", null, null, false);
        SuiteOptions = CreateOptionStringFromDataTable(dtSuites, "WTS_SYSTEM_SUITEID", "WTS_SYSTEM_SUITE", "0", "", true);

        // systems
        DataTable dtSystems = MasterData.SystemList_Get(includeArchive: true, WTS_SYSTEM_SUITEID: 0);        
        SystemOptions = CreateOptionStringFromDataTable(dtSystems, "WTS_SYSTEMID", "WTS_SYSTEM", "0", "", true);

        List<string> columnsToRemove = new List<string>();
        List<string> columnsToKeep = new List<string>() { "WTS_SystemSuiteID", "WTS_SystemSuite", "WTS_SystemID", "WTS_SYSTEM" };
        foreach (DataColumn col in dtSystems.Columns)
        {
            if (!columnsToKeep.Contains(col.ColumnName))
            {
                columnsToRemove.Add(col.ColumnName);
            }
        }
        foreach (string colName in columnsToRemove)
        {
            dtSystems.Columns.Remove(colName);
        }
        dtSystems.AcceptChanges();
        dtSystems = dtSystems.DefaultView.ToTable(true);
        dtSystemsJSON = JsonConvert.SerializeObject(dtSystems);

        // workarea ddl
        DataTable dtWorkAreas = MasterData.WorkAreaList_Get();
        WorkAreaOptions = CreateOptionStringFromDataTable(dtWorkAreas, "WorkAreaID", "WorkArea", "0", "", true);

        DataTable dtWorkAreaSystems = MasterData.WorkArea_SystemList_Get();
        columnsToRemove = new List<string>();
        columnsToKeep = new List<string>() { "WorkArea_SystemID", "WorkArea", "WorkAreaID", "WTS_SYSTEM", "WTS_SYSTEMID" };
        foreach (DataColumn col in dtWorkAreaSystems.Columns)
        {
            if (!columnsToKeep.Contains(col.ColumnName))
            {
                columnsToRemove.Add(col.ColumnName);
            }
        }
        foreach (string colName in columnsToRemove)
        {
            dtWorkAreaSystems.Columns.Remove(colName);
        }
        dtWorkAreaSystems.AcceptChanges();
        dtWorkAreaSystems = dtWorkAreaSystems.DefaultView.ToTable(true);
        dtWorkAreaSystemsJSON = JsonConvert.SerializeObject(dtWorkAreaSystems);

        // RQMTType ddl (purpose)
        DataTable dtTypes = RQMT.RQMTTypeList_Get();
        PopulateDDLFromDataTable(ddlRQMTType, dtTypes, "RQMTTypeID", "RQMTType", null, null, false, "InternalType");        
        RQMTTypeOptions = CreateOptionStringFromDataTable(dtTypes, "RQMTTypeID", "RQMTType", "0", "", true, "InternalType");

        // Description Types option list
        DataTable dtDescTypes = RQMT.RQMTDescriptionTypeList_Get();
        DescriptionTypeOptions = CreateOptionStringFromDataTable(dtDescTypes, "RQMTDescriptionTypeID", "RQMTDescriptionType", null, null, true);

        // RQMTComplexity
        DataTable dtComplexity = RQMT.RQMTComplexityList_Get();
        ComplexityOptions = CreateOptionStringFromDataTable(dtComplexity, "RQMTComplexityID", "RQMTComplexity", null, null, true);

        // Functionality
        DataTable dtFunctionality = MasterData.WorkloadGroupList_Get();
        dtFunctionality.DefaultView.Sort = "WorkloadGroup";
        dtFunctionality = dtFunctionality.DefaultView.ToTable();
        FunctionalitySelectOptions = CreateOptionStringFromDataTable(dtFunctionality, "WorkloadGroupID", "WorkloadGroup", "0", "", true, null);
        FunctionalityCheckBoxOptions = CreateCheckBoxStringFromDataTable(dtFunctionality, "WorkloadGroupID", "WorkloadGroup", true, 1, null);

        // Attributes
        AttributeOptions = new Dictionary<string, string>();

        DataTable dtStage = RQMT.RQMTAttribute_Get((int)WTS.Enums.RQMTAttributeTypeEnum.Stage);
        AttributeOptions["stage"] = CreateOptionStringFromDataTable(dtStage, "RQMTAttributeID", "RQMTAttribute", "0", "", true);

        DataTable dtCrit = RQMT.RQMTAttribute_Get((int)WTS.Enums.RQMTAttributeTypeEnum.Criticality);
        AttributeOptions["criticality"] = CreateOptionStringFromDataTable(dtCrit, "RQMTAttributeID", "RQMTAttribute", "0", "", true);

        DataTable dtStatus = RQMT.RQMTAttribute_Get((int)WTS.Enums.RQMTAttributeTypeEnum.Status);
        AttributeOptions["status"] = CreateOptionStringFromDataTable(dtStatus, "RQMTAttributeID", "RQMTAttribute", "0", "", true);

        if (!string.IsNullOrWhiteSpace((string)Session["copied.rqmts"]))
        {
            string[] arr = ((string)Session["copied.rqmts"]).Split(',');
            string[] rqmts = arr[0].Split('|');
            
            CopiedRQMTs = "";
            CopiedRQMTSystems = arr[1];

            for (int i = 0; i < rqmts.Length; i++)
            {
                string rqmt = rqmts[i];

                if (CopiedRQMTs.Length > 0) CopiedRQMTs += ", ";

                if (CopiedRQMTs.Contains(rqmt))
                {
                    CopiedRQMTs += rqmt + "*";
                }
                else
                {
                    CopiedRQMTs += rqmt;
                }
            }
        }


        //ShowMultipleSetQuickAddWarning
        DataTable userSettingDt = LoggedInUser.UserSettingList_Get(LoggedInUserID, 2);
        if (userSettingDt != null && userSettingDt.Rows.Count > 0 && userSettingDt.Rows[0]["SettingValue"].ToString() == "0")
        {
            QuickAddWarningEnabled = false;
        }
    }

    private DataTable LoadData()
    {
        DataTable dt = new DataTable();

        return dt;
    }
    #endregion

    #region AJAX

    [WebMethod()]
    public static string GetSystemsForSuite(string suiteID)
    {
        DataTable dt = new DataTable();

        int sid = 0;
        if (Int32.TryParse(suiteID, out sid))
        {
            dt = MasterData.SystemList_Get(includeArchive: true, WTS_SYSTEM_SUITEID: sid);
        }

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetWorkAreasForSystems(string systemIDs)
    {
        DataTable dt = new DataTable();

        dt = MasterData.WorkAreaList_Get(includeArchive: false, cv: "0", systemIDs: systemIDs);

        return JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None);
    }

    [WebMethod()]
    public static string GetRequirementTypes()
    {
        DataTable dtTypes = RQMT.RQMTTypeList_Get();

        return WTSPage.SerializeResult(dtTypes);
    }

    [WebMethod()]
    public static string FillCustomLists()
    {
        Dictionary<string, object> results = new Dictionary<string, object>();

        // functional rqmt priority
        DataTable dt = MasterData.PriorityList_Get(false, false, (int)WTS.Enums.PriorityTypeEnum.RQMT);
        dt.Columns.Add("Default");
        foreach (DataRow row in dt.Rows)
        {
            row["Default"] = row["Priority"].ToString() == "Major" ? "true" : "false";
        }
        dt.AcceptChanges();
        results["RQMTPriority"] = dt;

        return WTSPage.SerializeResult(results);
    }

    [WebMethod()]
    public static string FilterRQMTSets(int RQMTSetID, string RQMTSetName, int WTS_SYSTEMID, int WorkAreaID, int RQMTTypeID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMTSetID < 0)
        {
            RQMTSetID = 0;
        }

        // when results from back from the server, they come back as a data set containing the following tables:
        // [0] = RQMT
        // [1] = DESC
        // [2] = FUNC
        // [3] = DEFECT
        // [4] = TASK
        DataSet ds = RQMT.RQMTBuilder_Data_Get(RQMTSetID, !string.IsNullOrWhiteSpace(RQMTSetName) ? RQMTSetName : null, WTS_SYSTEMID, WorkAreaID, RQMTTypeID, 0);

        return WTSPage.SerializeResult(ds);
    }

    [WebMethod()]
    public static string AddRQMTSet(string RQMTSetName, int WTS_SYSTEMID, int WorkAreaID, int RQMTTypeID)
    {
        var result = WTSPage.CreateDefaultResult();

        var RQMTSetID = RQMT.RQMTSet_Save(0, RQMTSetName, WTS_SYSTEMID, WorkAreaID, RQMTTypeID, 0, null);

        if (RQMTSetID > 0)
        {
            result["success"] = "true";
        }

        result["RQMTSetID"] = RQMTSetID.ToString();

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTSet(int RQMTSetID, string RQMTSetName, int WTS_SYSTEMID, int WorkAreaID, int RQMTTypeID, int RQMTComplexityID, string justification)
    {
        var result = WTSPage.CreateDefaultResult();

        var postSaveRQMTSetID = RQMT.RQMTSet_Save(RQMTSetID, string.IsNullOrWhiteSpace(RQMTSetName) ? "" : RQMTSetName, WTS_SYSTEMID, WorkAreaID, RQMTTypeID, RQMTComplexityID, justification);

        if (postSaveRQMTSetID == RQMTSetID)
        {
            result["success"] = "true";
        }
        else
        {
            result["exists"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string ExecuteComponentSearch(string compType, string txt)
    {
        DataTable dt = new DataTable();

        bool returnAll = txt == "[ALL]";

        if (txt != null && txt.Length > 2000)
        {
            txt = txt.Substring(0, 2000);
        }

        switch (compType)
        {
            case "rqmt":
                if (string.IsNullOrWhiteSpace(txt))
                {
                    txt = "[NONE]"; // we don't allow blank searches for rqmts
                }

                txt = txt.Trim();
                txt = txt.Replace("\r", "");
                txt = txt.Replace("\n", "[CR]");

                // do another check for empty string
                if (string.IsNullOrWhiteSpace(txt))
                {
                    txt = "[NONE]";
                }

                dt = RQMT.RQMTList_Get(0, txt, true);

                break;
            case "type":
                dt = RQMT.RQMTTypeList_Get(returnAll || string.IsNullOrWhiteSpace(txt) ? null : txt); // the type list is short so we default to showing all
                break;
            case "workarea":
                dt = MasterData.WorkAreaList_Get();
                if (!string.IsNullOrWhiteSpace(txt))
                {
                    txt = txt.ToUpper();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["WorkArea"].ToString().ToUpper().IndexOf(txt) == -1)
                        {
                            row.Delete();
                        }
                    }
                }
                break;
            case "sys":
                dt = MasterData.SystemList_Get();
                DataColumn col = dt.Columns["WTS_SystemID"];
                col.ColumnName = "WTS_SYSTEMID";

                if (!string.IsNullOrWhiteSpace(txt))
                {
                    txt = txt.ToUpper();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["WTS_SYSTEM"].ToString().ToUpper().IndexOf(txt) == -1)
                        {
                            row.Delete();
                        }
                    }
                }
                break;
            case "desc":
                if (string.IsNullOrWhiteSpace(txt))
                {
                    txt = "__NONE__"; // we don't allow blank searches for descriptions
                }
                dt = RQMT.RQMTBuilderDescriptionList_Get(0, txt, false);
                break;
        }

        // some of the source queries are returning invalid xml/json column names, so we fix the common errors
        foreach (DataColumn x in dt.Columns)
        {
            x.ColumnName = x.ColumnName.Replace(" ", "_").Replace("#", "NUM");
        }
        dt.AcceptChanges();

        string str = WTSPage.SerializeResult(dt);

        return WTSPage.SerializeResult(dt);
    }

    [WebMethod()]
    public static string AddRQMTToRQMTSet(int RQMTSetID, int RQMTID, string RQMTText, bool addAsChild)
    {
        var result = WTSPage.CreateDefaultResult();

        try
        {
            RQMTID = RQMT.RQMTSet_AddRQMT(RQMTSetID, RQMTID, RQMTText, addAsChild, 0, null);

            result["success"] = "true";
            result["RQMTID"] = RQMTID.ToString(); // only needed if we create a brand new RQMTID
        }
        catch (Exception e)
        {
            result["error"] = e.Message;
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string CreateNewComponent(string compType, string txt)
    {
        var result = WTSPage.CreateDefaultResult();

        try
        {
            if (compType == "rqmt")
            {
                txt = txt.Replace("|", "!");
                txt = txt.Replace("\r", "");
                txt = txt.Replace("\n", "|"); // we will be adding one rqmt for each |
                string[] txtarr = txt.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                txt = string.Join("|", txtarr);

                // rqmtsNeedingParents stores the indexes of the rqmts that had a > in front of them
                // after the saving is done, we make ajax calls to add the new rqmts to the sets displayed on the page, and will preserve parent-child relationships based on > characters
                string rqmtsNeedingParents = "";
                for (int i = 0; i < txtarr.Length; i++)
                {
                    if (i > 0) rqmtsNeedingParents += ",";

                    rqmtsNeedingParents += txtarr[i].Trim().StartsWith(">") ? "1" : "0";
                } 

                var saveResult = RQMT.RQMT_Save(true, 0, txt);

                result["success"] = saveResult["saved"].ToLower();
                result["rqmtid"] = saveResult["newID"];
                result["rqmtids"] = saveResult["newIDs"];
                result["exists"] = saveResult["exists"];
                result["rqmtsneedingparents"] = rqmtsNeedingParents;
            }
        }
        catch (Exception e)
        {
            result["error"] = e.Message;
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTBase(int RQMTID, string txt)
    {
        var result = WTSPage.CreateDefaultResult();

        var saveResult = RQMT.RQMT_Save(false, RQMTID, txt);

        result["success"] = saveResult["saved"].ToLower();
        result["exists"] = saveResult["exists"].ToLower();

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteRQMTBase(int RQMTID)
    {
        var result = WTSPage.CreateDefaultResult();

        var deleteResult = RQMT.RQMT_Delete(RQMTID, false);

        result["success"] = deleteResult["deleted"];
        result["hasdependencies"] = deleteResult["hasdependencies"];

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteRQMTFromSet(int RQMTSetID, string checkedRQMTIDs, bool globalDelete)
    {
        var result = WTSPage.CreateDefaultResult();

        string[] ids = checkedRQMTIDs.Split('|');

        for (int i = ids.Length - 1; i >= 0; i--) // we go backwards so we can re-order as we go and not delete parents before children
        {
            int id = Int32.Parse(ids[i]);

            if (globalDelete)
            {
                DataTable dt = RQMT.RQMT_AssociatedSets_Get(id);

                foreach (DataRow row in dt.Rows)
                {
                    RQMT.RQMTSet_DeleteRQMT((int)row["RQMTSetID"], id, 0, true);
                }
            }
            else
            {
                RQMT.RQMTSet_DeleteRQMT(RQMTSetID, id, 0, true);
            }
        }

        result["success"] = "true";

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteRQMTSet(int RQMTSetID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSet_Delete(RQMTSetID))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string GetRQMTSetNames()
    {
        return WTSPage.SerializeResult(RQMT.RQMTSetName_Get());
    }

    [WebMethod()]
    public static string RenameRQMTSetGroup(int RQMTSetNameID, string name)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSetName_Save(RQMTSetNameID, name))
        {
            result["success"] = "true";
        }
        else
        {
            result["error"] = "Name already exists and could not be changed.";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTSetOrdering(string order)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSet_ReorderRQMTs(0, order))
        {
            result["success"] = "true";
        }
        
        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string LoadAllRQMTSets()
    {
        return WTSPage.SerializeResult(RQMT.RQMTSetList_Get());
    }

    [WebMethod()]
    public static string LoadAllRQMTSetsForRQMT(int RQMTID)
    {
        return WTSPage.SerializeResult(RQMT.RQMT_AssociatedSets_Get(RQMTID));
    }

    [WebMethod()]
    public static string SaveRQMTChanges(string changes)
    {
        var result = WTSPage.CreateDefaultResult();

        try
        {
            JObject jobj = (JObject)JsonConvert.DeserializeObject(changes);

            int RQMTID = 0;
            string addToSets = "";
            string deleteFromSets = "";
            string RQMTText = "";

            foreach (KeyValuePair<string, JToken> token in (JObject)jobj)
            {
                string value = token.Value.ToString();

                if (token.Key == "RQMTID")
                {
                    RQMTID = Int32.Parse(value);
                }
                else if (token.Key == "adds")
                {
                    addToSets = value;
                }
                else if (token.Key == "deletes")
                {
                    deleteFromSets = value;
                }
                else if (token.Key == "RQMT")
                {
                    RQMTText = value;
                }
            }

            int returnID = RQMT.RQMTBuilder_RQMTUpdate(RQMTID, RQMTText, addToSets, deleteFromSets);

            if (returnID != RQMTID)
            {
                result["error"] = "Change cannot be saved. Another RQMT already exists with the same text (RQMT #" + returnID + ").";                
            }
            else
            {
                result["success"] = "true";
            }
        }
        catch (Exception e)
        {
            result["error"] = e.Message;
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTDescription(int RQMTSet_RQMTSystemID, int RQMTSystemRQMTDescriptionID, string RQMTDescription, int RQMTDescriptionTypeID, bool editMode)
    {
        var result = WTSPage.CreateDefaultResult();

        try
        {
            int RQMTDescriptionID = RQMT.RQMTSystem_SaveDescription(0, RQMTSet_RQMTSystemID, RQMTSystemRQMTDescriptionID, RQMTDescription, RQMTDescriptionTypeID, editMode, "all");

            result["success"] = "true";
            result["rqmtdescriptionid"] = RQMTDescriptionID.ToString();
        }
        catch (Exception e)
        {
            if (e.Message.IndexOf("UNIQUE KEY") != -1)
            {
                result["error"] = "This description/type combination already exists in this RQMT.";
            }
            else
            {
                result["error"] = e.Message;
            }
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteRQMTDescription(int RQMTSystemRQMTDescriptionID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSystem_DeleteDescription(RQMTSystemRQMTDescriptionID))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTAttributes(int RQMTSet_RQMTSystemID, int RQMTStageID, int CriticalityID, int RQMTStatusID, bool RQMTAccepted)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSystemAttributes_Save(RQMTSet_RQMTSystemID, RQMTStageID, CriticalityID, RQMTStatusID, RQMTAccepted))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string SaveRQMTFunctionality(int RQMTSetID, int RQMTSet_RQMTSystemID, string RQMTFunctionalities, int RQMTSetFunctionalityID, int FunctionalityID, int ComplexityID, string Justification)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTFunctionality_Save(RQMTSetID, RQMTSet_RQMTSystemID, RQMTFunctionalities, RQMTSetFunctionalityID, FunctionalityID, ComplexityID, Justification))
        {
            result["success"] = "true";
            result["RQMTSetID"] = RQMTSetID.ToString();
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string DeleteRQMTFunctionality(int RQMTSetID, int RQMTSet_RQMTSystemID, int RQMTSetFunctionalityID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTFunctionality_Delete(RQMTSetID, RQMTSet_RQMTSystemID, RQMTSetFunctionalityID))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string UpdateRQMTSetRQMTSystemUsage(int RQMTSet_RQMTSystemID, int month, bool selected)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSet_RQMTSystem_Usage_Update(RQMTSet_RQMTSystemID, month, selected))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string CopyRQMTs(string RQMTIDs, string SystemIDs, string RQMTSet_RQMTSystemIDs)
    {
        var result = WTSPage.CreateDefaultResult();

        if (string.IsNullOrWhiteSpace(RQMTIDs) || string.IsNullOrWhiteSpace(RQMTSet_RQMTSystemIDs))
        {
            HttpContext.Current.Session["copied.rqmts"] = "";

            result["rqmtids"] = "";
            result["rqmtidsstr"] = "";
            result["systemids"] = "";
            result["rqmtsetrqmtsystmeids"] = "";
        }
        else
        {
            string previousCopiedRQMTs = (string)HttpContext.Current.Session["copied.rqmts"];

            if (string.IsNullOrWhiteSpace(previousCopiedRQMTs))
            {
                HttpContext.Current.Session["copied.rqmts"] = RQMTIDs + "," + SystemIDs + "," + RQMTSet_RQMTSystemIDs;
            }
            else
            {
                string[] arr = previousCopiedRQMTs.Split(',');
                string prevRQMTIDs = arr[0];
                string prevSystemIDs = arr[1];
                string prevRQMTSet_RQMTSystemIDs = arr[2];

                string[] newRQMTIdArr = RQMTIDs.Split('|');
                string[] newSystemIdArr = SystemIDs.Split('|');
                string[] newRQMTSet_RQMTSystemArr = RQMTSet_RQMTSystemIDs.Split('|');

                string newRQMTIDs = prevRQMTIDs;
                string newSystemIDs = prevSystemIDs;
                string newRQMTSet_RQMTSystemIDs = prevRQMTSet_RQMTSystemIDs;

                for (int i = 0; i < newRQMTSet_RQMTSystemArr.Length; i++)
                {
                    if (!("|" + prevRQMTIDs + "|").Contains("|" + newRQMTIdArr[i] + "|"))
                    {                        
                        newRQMTSet_RQMTSystemIDs += "|" + newRQMTSet_RQMTSystemArr[i];
                        newSystemIDs += "|" + newSystemIdArr[i];

                        if (("|" + prevRQMTIDs.Replace("*", "") + "|").Contains("|" + newRQMTIdArr[i] + "|"))
                        {
                            newRQMTIDs += "|" + newRQMTIdArr[i] + "*";
                        }
                        else
                        {
                            newRQMTIDs += "|" + newRQMTIdArr[i];
                        }
                    }
                }

                RQMTIDs = newRQMTIDs;
                SystemIDs = newSystemIDs;
                RQMTSet_RQMTSystemIDs = newRQMTSet_RQMTSystemIDs;

                HttpContext.Current.Session["copied.rqmts"] = newRQMTIDs + "," + newSystemIDs + "," + newRQMTSet_RQMTSystemIDs;
            }

            result["rqmtids"] = RQMTIDs;
            result["rqmtidsstr"] = RQMTIDs.Replace("|", ", ");
            result["systemids"] = SystemIDs;
            result["rqmtsetrqmtsystmeids"] = RQMTSet_RQMTSystemIDs;
        }

        result["success"] = "true";

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string PasteRQMTs(int RQMTSetID, string options)
    {
        var result = WTSPage.CreateDefaultResult();

        string previousCopiedRQMTs = (string)HttpContext.Current.Session["copied.rqmts"]; // RQMTS, SYSIDS, RSRSIDS

        if (options == null) options = "";
        bool pasteAttributes = options.IndexOf("attr") != -1;
        bool pasteDefects = options.IndexOf("def") != -1;
        bool pasteDescriptions = options.IndexOf("desc") != -1;

        string pasteOptions = (pasteAttributes ? "attr," : "") + (pasteDefects ? "def," : "") + (pasteDescriptions ? "desc," : "");
        if (pasteOptions.Length > 0)
        {
            pasteOptions = pasteOptions.Substring(0, pasteOptions.Length - 1);
        }

        if (!string.IsNullOrWhiteSpace(previousCopiedRQMTs))
        {
            string[] arr = previousCopiedRQMTs.Split(',');
            string[] prevRQMTIDs = arr[0].Split('|');
            string[] prevRQMTSet_RQMTSystemIDs = arr[2].Split('|');

            for (int i = 0; i < prevRQMTIDs.Length; i++)
            {
                int RQMTID = Int32.Parse(prevRQMTIDs[i]);
                int RQMTSet_RQMTSystemID = Int32.Parse(prevRQMTSet_RQMTSystemIDs[i]);

                RQMT.RQMTSet_AddRQMT(RQMTSetID, RQMTID, null, false, RQMTSet_RQMTSystemID, pasteOptions);
            }

            RQMT.RQMTSet_ReorderRQMTs(RQMTSetID, null); // this forces us to reclaim space from previously deleted rqmts

            result["success"] = "true";
        }
        else // we shouldn't get here; the UI should have disabled the paste button if there is nothing on the clipboard
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string HideQuickAddWarning()
    {
        var result = WTSPage.CreateDefaultResult();

        WTS_User u = WTSPage.GetLoggedInUser();

        bool dup = false;
        string err = "";

        u.UserSetting_Update(0, 2, 0, "0", out dup, out err);

        result["success"] = "true";

        return WTSPage.SerializeResult(result);
    }

    [WebMethod()]
    public static string TaskAddedToRQMTSet(int RQMTSetID, int WORKITEM_TASKID)
    {
        var result = WTSPage.CreateDefaultResult();

        if (RQMT.RQMTSet_Task_Add(RQMTSetID, WORKITEM_TASKID))
        {
            result["success"] = "true";
        }

        return WTSPage.SerializeResult(result);        
    }

    [WebMethod()]
    public static string DeleteTaskFromRQMTSet(int RQMTSetID, int RQMTSetTaskID)
    {
        var result = WTS.WTSPage.CreateDefaultResult();


        if (RQMT.RQMTSet_Task_Delete(RQMTSetID, RQMTSetTaskID))
        {
            result["success"] = "true";
        }

        return WTS.WTSPage.SerializeResult(result);
    }

    #endregion
}