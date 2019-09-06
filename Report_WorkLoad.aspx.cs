using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Aspose.Cells;
using System.IO;
using System.Web.Services;
using System.Web.Script.Services;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.Security;

public partial class Report_WorkLoad : System.Web.UI.Page {
    protected int reportID;
    protected int userID;
    protected String Excel = "";

    protected void Page_Load(object sender, EventArgs e) {
        this.reportID = WTS_Reports.getReportIDbyName("Workload Summary Report");
        DataTable dt = new DataTable();
        MembershipUser u = Membership.GetUser();
        WTS_User user = new WTS_User(u.ProviderUserKey.ToString());
        user.Load_GUID();
        this.userID = user.ID;
        string userName = HttpContext.Current.User.Identity.Name;
        WTS_Reports.get_Report_Parameter_List(ref dt, userID, this.reportID);
        if (dt != null && dt.Rows.Count > 0)
        {
            foreach(DataRow row in dt.Rows)
            {
                string name = row.Field<string>("Name");
                string value = row.Field<Int32>("ParamsID").ToString();
                ListItem li = new ListItem(name, value);
                if (row.Field<bool>("Process"))
                {
                    li.Attributes.Add("OptionGroup", "Process Views");
                }
                else
                {
                    li.Attributes.Add("OptionGroup", "Custom Views");
                }
                ddlParameters.Items.Insert(ddlParameters.Items.Count, li);
            }
        }
        ListItem def = new ListItem("Default", "Default");
        def.Attributes.Add("OptionGroup", "Process Views");
        ddlParameters.Items.Insert(0, def);

        // 13419 - 7:
        ListItem defBacklog = new ListItem("Default (Backlog)", "Default (Backlog)");
        def.Attributes.Add("OptionGroup", "Process Views");
        ddlParameters.Items.Insert(1, defBacklog);


        if (IsPostBack) {
            DataSet ds = (DataSet)Session["WorkloadSummaryData"];
            String selectedColumns = (String)Session["SelectedColumns"];
            String SummaryOverviewsSection1 = (String)Session["SummaryOverviewsSecion1"];
            String SummaryOverviewsSection2 = (String)Session["SummaryOverviewsSecion2"];


            String ddlValue = (String)Session["ddlValue"];

            Excel = (String)Session["Excel"];

            exportReport(ds, selectedColumns, SummaryOverviewsSection1, SummaryOverviewsSection2, ddlValue);
        }
    }

    private void exportReport(DataSet ds, String SelectedColumnsString, String SummaryOverviewsSection1, String SummaryOverviewsSection2, String ddlValue)
    {
        String[] selectedColumns = null;
        String[] SummaryOverview = null;
        String[] SummaryOverview1 = null;
        string[] SummaryOverview2 = null;
        string SummaryOverviewsCombined = "";
        String strName = "Summary Work Load Report";

        bool dev = false;
        bool bus = false;
        try
        {
            dev = ds.Tables.Contains("Developers");
        }
        catch (Exception)
        {
            dev = false;
        }
        //bool dev = ds.Tables.Contains("Developers");
        try
        {
            bus = ds.Tables.Contains("Business Team");
        }
        catch (Exception)
        {
            bus = false;
        }
        //bool bus = ds.Tables.Contains("Business Team");


        Workbook wb = new Workbook(FileFormatType.Pdf);
        MemoryStream ms = new MemoryStream();
        Worksheet ws = wb.Worksheets[0];

        if (!String.IsNullOrEmpty(SelectedColumnsString))
        {
            selectedColumns = SelectedColumnsString.Split(',').ToArray();
        }
        if (!String.IsNullOrEmpty(SummaryOverviewsSection1))
        {
            SummaryOverview1 = SummaryOverviewsSection1.Replace(" ASC", "").Replace(" DESC", "").Replace("]", "").Replace("[", "").Split(',').ToArray();
            SummaryOverviewsCombined += SummaryOverviewsSection1;
        }
        if (!String.IsNullOrEmpty(SummaryOverviewsSection2))
        {
            SummaryOverview2 = SummaryOverviewsSection2.Replace(" ASC", "").Replace(" DESC", "").Replace("]", "").Replace("[", "").Split(',').ToArray();
            if (!String.IsNullOrEmpty(SummaryOverviewsSection1))
            {
                SummaryOverviewsCombined += "," + SummaryOverviewsSection2;
            }
            else
            {
                SummaryOverviewsCombined += SummaryOverviewsSection2;
            }
        }
        if (!String.IsNullOrEmpty(SummaryOverviewsCombined))
        {
            SummaryOverview = SummaryOverviewsCombined.Replace(" ASC", "").Replace(" DESC", "").Replace("]", "").Replace("[", "").Split(',').ToArray(); //The summary overviews strings come in formatted to be a part of a sql query string. They need to be formatted so they can be used in a .net select statement. Removes ASC,DESC, and angle brackets and then cast string to an array
        }

        reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
        if (!object.ReferenceEquals(SummaryOverview, null)) //if summary overview columns are selected, export the summary overview table. 
        {
            exportSummaryOverviews(ds.Tables["Summary Overview Parent"], ds.Tables["Summary Overview Child"], ws, SummaryOverview1, SummaryOverview2); //format the worksheet
            wb.Worksheets.Add(SheetType.Worksheet); //add a new worksheet to the worbook
            ws = wb.Worksheets[wb.Worksheets.Count - 1]; //store the reference to the new worksheet in the local variable.
            reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
        }
        if (ds.Tables.Contains("Developers") == true)
        {
            exportResourceOverview(ds.Tables["Developers"], ws, "Developers Overview");
            wb.Worksheets.Add(SheetType.Worksheet);
            ws = wb.Worksheets[wb.Worksheets.Count - 1];
            reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
        }
        if (ds.Tables.Contains("Business Team") == true)
        {
            exportResourceOverview(ds.Tables["Business Team"], ws, "Business Team Overview");
            wb.Worksheets.Add(SheetType.Worksheet);
            ws = wb.Worksheets[wb.Worksheets.Count - 1];
            reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
        }

        // 13419 - 5:
        if (dev || bus)
        { 
            if (ds.Tables.Contains("Work Type counts") == true)
            {
                exportResourceOverview(ds.Tables["Work Type counts"], ws, "Work Type counts");
                wb.Worksheets.Add(SheetType.Worksheet);
                ws = wb.Worksheets[wb.Worksheets.Count - 1];
                reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
            }
        }



        if (!object.ReferenceEquals(selectedColumns, null))
        {
            exportSummaryDetail(ds.Tables["Summary Detail Task"], ds.Tables["Summary Detail Sub-Task"], ws, selectedColumns, SummaryOverview);
            wb.Worksheets.Add(SheetType.Worksheet);
            ws = wb.Worksheets[wb.Worksheets.Count - 1];
            reportPageSetup(ws.PageSetup, SummaryOverview1, SummaryOverview2, selectedColumns, dev, bus);
        }
        //if (Excel.ToLower() == "yes")
        //    wb.Save(ms, SaveFormat.Excel97To2003);
        //else
        //    wb.Save(ms, SaveFormat.Pdf);

        //wb.Save(ms, SaveFormat.Pdf);

        try
        {
            if (Excel.ToLower() == "yes")
            {
                wb.Save(ms, SaveFormat.Excel97To2003);
                Response.BufferOutput = true;
                Response.ContentType = "application/xls";
                Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".xls");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
            else
            {
                wb.Save(ms, SaveFormat.Pdf);
                Response.BufferOutput = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;  filename=" + strName + ".pdf");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
        catch (Exception e)
        {
            // Thread was being aborted here, OK?
        }
        return;
    }
    private static void exportSummaryOverviews(DataTable parentTable, DataTable childTable, Worksheet ws, String[] SummaryOverview1 = null, String[] SummaryOverview2 = null)
    {
        int rowCount = 5; //holds the integer value of the current row in the document to write too. 
        int childRowsPrinted = 0;
        string headerRow = "";
        string[] parentColumns = object.ReferenceEquals(SummaryOverview1, null) ? SummaryOverview2 : SummaryOverview1; //it is possible for there to be only one summary level. 
        bool level1 = !Object.ReferenceEquals(SummaryOverview1, null);
        bool level2 = !Object.ReferenceEquals(SummaryOverview2, null);
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        cellStyle.Font.Underline = FontUnderlineType.Single;
        cellStyle.Font.IsBold = true;
        cellStyle.IsTextWrapped = true;
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.Font.Size = cellStyle.Font.Size + 2;

        if (level1) //if there is a level 1 summary, add the column names to the header row
        {
            headerRow = String.Join(",", SummaryOverview1);
        }
        if (level1 && level2) //if there is both a level one and level two summary, add a \ to deliminate the too. 
        {
            headerRow += " \\ ";
        }
        if (level2) //if there is a level 2 summary, add the column names to the header row. 
        {
            headerRow += String.Join(",", SummaryOverview2);
        }

        if (headerRow.Length > 90) //if the header row is really long then you have to manually set the column widths so everthing fits. 
        {
            ws.Cells.SetColumnWidthInch(0, 1);
            ws.Cells.SetColumnWidthInch(1, 6);
            ws.Cells.SetColumnWidthInch(2, 1);
            ws.Cells.SetColumnWidthInch(3, 1);
        }


        ws.Cells.Merge(0, 0, 1, 4);
        ws.Cells["A1"].PutValue("Summary Overview");
        ws.Cells["A1"].SetStyle(cellStyle);
        ws.AutoFitRow(0, 0, 0);
        cellStyle.Font.Size = cellStyle.Font.Size - 2;
        ws.Cells.Merge(2, 0, 1, 4);
        ws.Cells["A3"].PutValue(headerRow);
        ws.Cells["A3"].SetStyle(cellStyle);
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        ws.Cells["A4"].PutValue("Rank");
        ws.Cells["A4"].SetStyle(cellStyle);
        ws.Cells["B4"].PutValue(headerRow); //print table header row
        ws.Cells["B4"].SetStyle(cellStyle);
        ws.Cells["C4"].PutValue("Tasks");
        ws.Cells["C4"].SetStyle(cellStyle);
        ws.Cells["D4"].PutValue("Sub Tasks");
        ws.Cells["D4"].SetStyle(cellStyle);

        foreach (DataRow parentRow in parentTable.Rows)
        {
            printParentRows(parentColumns, parentRow, rowCount, ws);
            rowCount++;
            if (!Object.ReferenceEquals(childTable, null)) //if there is a child table, print the child rows. 
            {
                int newRowsPrinted = 0;
                newRowsPrinted = printChildRows(parentColumns, SummaryOverview2, parentRow, childTable, rowCount, childRowsPrinted, ws);
                rowCount += newRowsPrinted;
                childRowsPrinted += newRowsPrinted;
            }
        }

        if (headerRow.Length > 90)
        {
            ws.AutoFitRows();
        }
        else
        {
            ws.AutoFitColumns();
        }

        printTotalRow(parentTable, ws, rowCount);
        ws.Name = "Summary Overviews";
        return;
    }
    private static void printParentRows(string[] parentColumns, DataRow parentRow, int rowCount, Worksheet ws)
    {
        //prints a row from the parent table.
        string parentSummaries = "";
        string parentRanking = "";
        int value;
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.Pattern = BackgroundType.Solid;
        cellStyle.ForegroundColor = Color.LightGray;
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.IsTextWrapped = true;

        foreach (string column in parentColumns)
        {
            string rankingColumn = String.Concat(column, " Rank");
            parentSummaries += parentRow.Field<string>(column) + ',';
            parentRanking += parentRow.Field<int>(rankingColumn).ToString() + ',';
        }
        parentSummaries = parentSummaries.TrimEnd(',');
        parentRanking = parentRanking.TrimEnd(',');
        ws.Cells['B' + rowCount.ToString()].PutValue(parentSummaries);
        ws.Cells['B' + rowCount.ToString()].SetStyle(cellStyle);
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        ws.Cells['A' + rowCount.ToString()].PutValue(parentRanking);
        ws.Cells['A' + rowCount.ToString()].SetStyle(cellStyle);
        value = parentRow.Field<Int32>("Tasks");
        ws.Cells['C' + rowCount.ToString()].PutValue(value);
        ws.Cells['C' + rowCount.ToString()].SetStyle(cellStyle);
        value = parentRow.Field<Int32>("Subtasks");
        ws.Cells['D' + rowCount.ToString()].PutValue(value);
        ws.Cells['D' + rowCount.ToString()].SetStyle(cellStyle);
        return;
    }
    private static int printChildRows(string[] parentColumns, string[] SummaryOverviews, DataRow parentRow, DataTable childTable, int rowCount, int childRowsPrinted, Worksheet ws)
    {
        //prints rows from the child table, beginning at childRowsPrinted, where the child table attributes are equal to the parent table attributes. Returns the number of rows printed. 
        //this method gets called for each parent row, and it picks up where it left off last time in the child table.
        int newRowsPrinted = 0;
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.IsTextWrapped = true;
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;

        for (int i = childRowsPrinted; i < childTable.Rows.Count; i++, newRowsPrinted++)
        {
            DataRow childRow = childTable.Rows[i];
            string childSummaries = "";
            string childRanking = "";
            int value;
            foreach (string column in parentColumns) //check if childrow attributes are equal to the current parent row attributes. If equal print the row, else return. 
            {
                if (parentRow.Field<string>(column) != childRow.Field<string>(column))
                {
                    return newRowsPrinted; //stop print child rows. Return to this spot in the table to print next time.
                }
            }
            foreach (string column in SummaryOverviews)
            {
                string rankingColumn = String.Concat(column, " Rank");
                childSummaries += childRow.Field<string>(column) + ',';
                childRanking += childRow.Field<int>(rankingColumn).ToString() + ',';
            }
            cellStyle.HorizontalAlignment = TextAlignmentType.Left;
            childSummaries = childSummaries.TrimEnd(',');
            childRanking = childRanking.TrimEnd(',');
            ws.Cells['B' + (rowCount + newRowsPrinted).ToString()].PutValue("   " + childSummaries);
            ws.Cells['B' + (rowCount + newRowsPrinted).ToString()].SetStyle(cellStyle);
            cellStyle.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells['A' + (rowCount + newRowsPrinted).ToString()].PutValue(childRanking);
            ws.Cells['A' + (rowCount + newRowsPrinted).ToString()].SetStyle(cellStyle);
            value = childRow.Field<Int32>("Tasks");
            ws.Cells['C' + (rowCount + newRowsPrinted).ToString()].PutValue(value);
            ws.Cells['C' + (rowCount + newRowsPrinted).ToString()].SetStyle(cellStyle);
            value = childRow.Field<Int32>("Subtasks");
            ws.Cells['D' + (rowCount + newRowsPrinted).ToString()].PutValue(value);
            ws.Cells['D' + (rowCount + newRowsPrinted).ToString()].SetStyle(cellStyle);
        }
        return newRowsPrinted;
    }
    static void printTotalRow(DataTable dt, Worksheet ws, int rowCount)
    {
        int totalTasks = 0;
        int totalSubtasks = 0;
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.Font.IsBold = true;
        cellStyle.IsTextWrapped = true;
        foreach (DataRow row in dt.Rows)
        {
            totalTasks += row.Field<int>("Tasks"); //computer total tasks/subtasks from parent table
            totalSubtasks += row.Field<int>("Subtasks");
        }
        ws.Cells['B' + rowCount.ToString()].PutValue("Total"); //print to worksheet
        ws.Cells['B' + rowCount.ToString()].SetStyle(cellStyle);
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        cellStyle.Font.IsBold = false;
        ws.Cells['A' + rowCount.ToString()].SetStyle(cellStyle);
        ws.Cells['C' + rowCount.ToString()].PutValue(totalTasks);
        ws.Cells['C' + rowCount.ToString()].SetStyle(cellStyle);
        ws.Cells['D' + rowCount.ToString()].PutValue(totalSubtasks);
        ws.Cells['D' + rowCount.ToString()].SetStyle(cellStyle);
    }
    private static void exportResourceOverview(DataTable dt, Worksheet ws, string worksheetName)
    {

        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        cellStyle.Font.IsBold = true;
        cellStyle.Font.Underline = FontUnderlineType.Single;
        ws.Cells.ImportDataTable(dt, true, "A3");
        cellStyle.IsTextWrapped = true;
        cellStyle.Font.Size = cellStyle.Font.Size + 2;
        ws.Cells.Merge(0, 0, 1, 3);
        ws.Cells["A1"].PutValue(worksheetName);
        ws.Cells["A1"].SetStyle(cellStyle);
        ws.AutoFitRow(0, 0, 0);
        cellStyle.Font.Size = cellStyle.Font.Size - 2;
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        cellStyle.Font.Underline = FontUnderlineType.None;

        for (int i = 0; i < dt.Columns.Count; i++) //header row
        {
            ws.Cells[2, i].SetStyle(cellStyle);
        }
        cellStyle.Font.IsBold = false;
        for (int i = 3; i < dt.Rows.Count+2; i++) //body
        {
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 0) //left column is left alligned. 
                {
                    cellStyle.HorizontalAlignment = TextAlignmentType.Left;
                    ws.Cells[i, j].SetStyle(cellStyle);
                    cellStyle.HorizontalAlignment = TextAlignmentType.Center;
                }
                else
                ws.Cells[i, j].SetStyle(cellStyle);
            }
        }
        cellStyle.Font.IsBold = true;
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thick, Color.Black);
        for (int i = 0; i < dt.Columns.Count; i++) //Total All row
        {
            if (i == 0)
            {
                cellStyle.HorizontalAlignment = TextAlignmentType.Left;
                ws.Cells[dt.Rows.Count + 2, i].SetStyle(cellStyle);
                cellStyle.HorizontalAlignment = TextAlignmentType.Center;
            }else
            ws.Cells[dt.Rows.Count + 2, i].SetStyle(cellStyle);
        }

        ws.AutoFitColumns();
        ws.Name = worksheetName;
        return;
    }
    private static void exportSummaryDetail(DataTable taskTable, DataTable subTaskTable, Worksheet ws, String[] SelectColumns, String[] SummaryOverviews)
    {

        string[] taskAttributes = null; //split up selected columns into task and sub task attributes. Task attributes exist in taskTable while subTask attributes exist in subTask table. Summary overview attributers exist in both tables. 
        string[] subTaskAttributes = null;
        DataRow currentRow = null;
        int taskRowsPrinted = 0; //need to alternate between printing tasks and sub tasks. Need to know where you stopped in the table, so you can start from that position next time you print. 
        int subTaskRowsPrinted = 0;
        int rowCount = 1; //keep track all rows printed. 
        int newRowsPrinted = 0; //keep track of new rows printed to be added to the above row counts. 
        taskAttributes = Array.FindAll<string>(SelectColumns, x => x.StartsWith("Task")); //task attributes all begin with "task". Sub task attributres all begin with "Sub-Task". 
        subTaskAttributes = Array.FindAll<string>(SelectColumns, x => x.StartsWith("Sub-Task"));
        int mergedCellCount = subTaskAttributes.Length > taskAttributes.Length ? subTaskAttributes.Length : taskAttributes.Length; //the number of columns that will be in the table, used for creating cell headers and the like. 
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        cellStyle.Font.IsBold = true;
        cellStyle.Font.Underline = FontUnderlineType.Single;
        cellStyle.IsTextWrapped = true;
        cellStyle.Font.Size = cellStyle.Font.Size + 2;
        cellStyle.IsTextWrapped = true;
        bool autosize = true; ; //if true let aspose size the columns, if false manually size columns so that all columns fit in pdf page width. 
        List<double> columSizeRatio = getColumnSizeRatio(taskTable, subTaskTable, taskAttributes, subTaskAttributes, mergedCellCount, ref autosize); //this list contains the ratio of column sizes, where the ratio is defined as the neccessary size of the column devided by the total size of all columns. This list is null if autosize is set to true.

        if (autosize ==  false)
        for (int i = 0; i < mergedCellCount; i++) //resize columns so they fit on one page. 
        {

            double size = (double)9.0 * columSizeRatio[i]; //in ladscape mode with 1 inch margins the available page space is about 9 inches. 
            ws.Cells.SetColumnWidthInch(i, size);
        }

        ws.Cells.Merge(0, 0, 1, mergedCellCount);
        ws.Cells["A1"].PutValue("Workload Overview");
        ws.Cells["A1"].SetStyle(cellStyle);
        ws.AutoFitRow(0, 0, 0);

        rowCount += 2;
        rowCount += newRowsPrinted;
        while (taskRowsPrinted < taskTable.Rows.Count)  //the task table is the driver of the loop, because it is not possible to have orphan sub tasks; that is there should never be an instance where a sub task has attributes distinct from all higher level tasks
        {
            DataRow oldRow = currentRow;
            currentRow = taskTable.Rows[taskRowsPrinted]; //get the next row to be printed
            newRowsPrinted = printOverviews(currentRow, oldRow, SummaryOverviews, ws, rowCount, mergedCellCount);
            rowCount += newRowsPrinted;  
            newRowsPrinted = printTasks(taskAttributes, taskTable, ws, rowCount, ref taskRowsPrinted, currentRow, SummaryOverviews, mergedCellCount, alternateColor: false);
            rowCount += newRowsPrinted;
            newRowsPrinted = printTasks(subTaskAttributes, subTaskTable, ws, rowCount, ref subTaskRowsPrinted, currentRow, SummaryOverviews, mergedCellCount, alternateColor: true);
            rowCount += newRowsPrinted;
        }
        if (autosize == true)
        {
            ws.AutoFitColumns();
        }
        else
        {
            ws.AutoFitRows();
        }
        ws.Name = "Summary Detail";
        return;
    }

    private static List<double> getColumnSizeRatio(DataTable taskTable, DataTable subTaskTable, string[] taskAttributes, string[] subTaskAttributes, int numColumns, ref bool autosize)
    {
        //this function finds the maximum length of each column to be displayed in the report, and then computes the ratio between the maximum length of each column and the maximum lenght of all the columns. This returned list object will be used to dynamically size the columns to "best fit" the displayed columns. 
        List<double> columnSizeRatio = new List<double>();
        int[] maxColumnLengths = new int[numColumns];
        int totalColumnLengths = 0;
        double sizeRatio = 1;

        for (int i = 0; i < numColumns; i++) //iterate through all the selected columns
        {
            int maxTaskLength = 0; //max lenght of the current task column
            int maxSubTaskLength = 0; //max length of the current sub task column
            foreach(DataRow row in taskTable.Rows) //iterate through the datatable
            {
                if (i < taskAttributes.Length && !String.IsNullOrEmpty(row.Field<string>(taskAttributes[i]))) //the number of task and sub task columns to be displayed is not always the same. Also null values throw excepitions when you compute their string length. 
                {
                    int currentCellLength = row.Field<string>(taskAttributes[i]).Length; //get the length of the cell
                    if (currentCellLength > maxTaskLength) //set max length equal to the greater of max length or the current cell. 
                    {
                        maxTaskLength = currentCellLength;
                    }
                }
            }
            foreach (DataRow row in subTaskTable.Rows)
            {
                if (i < subTaskAttributes.Length && !String.IsNullOrEmpty(row.Field<string>(subTaskAttributes[i])))
                {
                    int currentCellLength = row.Field<string>(subTaskAttributes[i]).Length;
                    if (currentCellLength > maxTaskLength)
                    {
                        maxTaskLength = currentCellLength;
                    }
                }
            }
            int maxColumnLength = maxTaskLength > maxSubTaskLength ? maxTaskLength : maxSubTaskLength; //the task and sub task column lengths will be different. Max column length set to th greate of the two. 
            maxColumnLengths[i] = maxColumnLength; //insert into the array
            totalColumnLengths += maxColumnLength;  //increment totalcolumnlength so that it is equal to the the combined length of all columns. 
        }

        if (totalColumnLengths < 140) //if the total length of the columns is less than 140, then aspose is capable of autosizing everything to fit without introducing page breaks. 
        {
            autosize = true;
            return columnSizeRatio;
        }
        else
        {
            autosize = false;
        }

        for (int i = 0; i < maxColumnLengths.Length; i++)
        {
            sizeRatio = (double)maxColumnLengths[i] / (double)totalColumnLengths;
            columnSizeRatio.Add(sizeRatio); //ratio of max column length to the combined max column length. 
        }

        return columnSizeRatio;
    }

    private static int printHeaderRow(Worksheet ws, String[] SelectedColumns, int rowCount, String Worklog, int mergedCellCount, bool alternateColor)
    {
        int uniA = (int)'A';
        int numberRowsPrinted = 0;
        String cellName = "A" + rowCount.ToString();
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.IsTextWrapped = true;
        cellStyle.Font.IsBold = true;
        cellStyle.HorizontalAlignment = TextAlignmentType.Center;
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.ForegroundColor = Color.Orange;
        cellStyle.Pattern = BackgroundType.Solid;
        ws.Cells.Merge(rowCount - 1, 0, 1, mergedCellCount);
        ws.Cells[cellName].PutValue(Worklog); //print current worklog, where work log is either production, release, or backlog
        ws.Cells[cellName].SetStyle(cellStyle);
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        numberRowsPrinted++;
        cellStyle.ForegroundColor = (alternateColor) ? Color.LightGreen : Color.LightBlue;
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.IsTextWrapped = true;
        for (int i = 0; i < SelectedColumns.Length; i++) //print the attribute names
        {

            string column = (SelectedColumns[i] == "Task #" || SelectedColumns[i] == "Sub-Task #") ? SelectedColumns[i] : SelectedColumns[i].Replace("Sub-Task", "").Replace("Task", "");
            cellName = (char)(uniA + i) + (rowCount + numberRowsPrinted).ToString();
            ws.Cells[cellName].PutValue(column);
            ws.Cells[cellName].SetStyle(cellStyle);
        }
        numberRowsPrinted++;
        return numberRowsPrinted;
    }
    private static int printTasks(string[] SelectColumns, DataTable dt, Worksheet ws, int rowCount, ref int startRow, DataRow currentOverviews, string[] overViewsColumnNames, int mergedCellCount, bool alternateColor)
    {
        if (startRow >= dt.Rows.Count) return 0; //don't print if all the rows in the table have already been printed. 
        int newRowsPrinted = 0; //number of rows printed to the document. This include table values and header cells. 
        int currentRow = 0; //number of rows traversed in the table. 
        bool overviewAttributesUnchanged = true;
        string CurrentWorkLog = "";
        if (checkOverviews(currentOverviews, dt.Rows[startRow], overViewsColumnNames))
        {
            CurrentWorkLog = dt.Rows[startRow].Field<string>("Work Log");
            newRowsPrinted += printHeaderRow(ws, SelectColumns, rowCount, CurrentWorkLog, mergedCellCount, alternateColor); //print worklog and task attributes
        }
        else
        {
            return 0;
        }

        while (overviewAttributesUnchanged && startRow + currentRow < dt.Rows.Count) //keep looping until overview values change or all the rows are printed
        {
            DataRow row = dt.Rows[startRow + currentRow];
            overviewAttributesUnchanged = checkOverviews(currentOverviews, row, overViewsColumnNames); //check if the overview values of the next row are different than the current overviews values
            if (overviewAttributesUnchanged == false) break;
            string thisRowsWorkLog = row.Field<string>("Work Log");
            if (CurrentWorkLog != thisRowsWorkLog) //if worklog value of the next row is different from previous, print the header again
            {
                newRowsPrinted += printHeaderRow(ws, SelectColumns, rowCount + newRowsPrinted, thisRowsWorkLog, mergedCellCount, alternateColor);
                CurrentWorkLog = thisRowsWorkLog;
            }
            newRowsPrinted += printSelectedColumns(SelectColumns, rowCount + newRowsPrinted, row, ws); //print the columns selected
            currentRow++;
        }
        startRow += currentRow;
        return newRowsPrinted;
    }
    private static int printSelectedColumns(string[] SelectColumns, int rowCount, DataRow row, Worksheet ws)
    {
        int uniA = (int)'A'; //unicode A value. Used to iterate up through 'A'-'Z'. 
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        cellStyle.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;
        cellStyle.IsTextWrapped = true;
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        for (int i = 0; i < SelectColumns.Length; i++)
        {
            string column = SelectColumns[i]; //get column name
            string value = row.Field<string>(column); //get the value of that column
            string cellName = (char)(uniA + i) + (rowCount).ToString(); //This casts the unicode value back to a character and appends a number 1 - N
            ws.Cells[cellName].PutValue(value); //print value
            ws.Cells[cellName].SetStyle(cellStyle);
        }
        return 1;
    }
    private static bool checkOverviews(DataRow currentOverviews, DataRow row, string[] columnNames) //current overviews role contains baseline values. Task and subtasks will continue to print until this baseline row attributes are no longer equal to the current row to be printed. 
    {
        if (object.ReferenceEquals(columnNames, null)) return true; //if  there are no summary overviews, then there is nothing to check

        foreach (string column in columnNames) //iterate through the summary overview columns, checking if  are equal to eachother. 
        {
            if (currentOverviews.Field<string>(column) != row.Field<string>(column))
            {
                return false;
            }
        }
        return true;
    }
    private static int printOverviews(DataRow currentOverivews, DataRow oldOverviews, string[] Overviews, Worksheet ws, int rowCount, int mergeCellsCount)
    {
        int newRowsPrinted = 0;
        int index = 0;
        //styling and formatting
        Aspose.Cells.Style cellStyle = new Aspose.Cells.Style();
        cellStyle.Pattern = BackgroundType.Solid;
        cellStyle.Font.IsBold = true;
        cellStyle.IsTextWrapped = true;
        cellStyle.HorizontalAlignment = TextAlignmentType.Left;
        cellStyle.Font.Name = "Calibri";
        cellStyle.Font.Size = 10;

        if (object.ReferenceEquals(Overviews, null)) return 0; //if there are no summary overviews, they should not be printed.  
        if (!object.ReferenceEquals(currentOverivews, null) && !object.ReferenceEquals(oldOverviews, null))
        {
            while (index < Overviews.Length) //check for any differences in the overviews. If there is a difference, print the current overviews
            {
                string columnName = Overviews[index];
                if (currentOverivews.Field<string>(columnName) != oldOverviews.Field<string>(columnName))
                {
                    break;
                }
                index++;
            }
        }
        string overviewRowValue = "";
        foreach(string columnName in Overviews) { 
            overviewRowValue += "    " + currentOverivews.Field<string>(columnName);
        }
        ws.Cells.Merge(rowCount + newRowsPrinted - 1, 0, 1, mergeCellsCount);
        string cellName = 'A' + (rowCount + newRowsPrinted).ToString();
        cellStyle.ForegroundColor = Color.Yellow;
        ws.Cells[cellName].PutValue(overviewRowValue);
        ws.Cells[cellName].SetStyle(cellStyle);
        newRowsPrinted++;
        return newRowsPrinted;
    }
    private static void reportPageSetup(PageSetup page, String[] Section1, String[] Section2, String[] SelectColumns, bool dev, bool bus) //this function formats the header and footer
    {
        string header1 = "";
        string header3 = "";
        List<FilterObject> filters = (List<FilterObject>)HttpContext.Current.Session["filters"];
        string footer1 = "";
        if (!Object.ReferenceEquals(SelectColumns, null))
        {
            string taskAttributes = String.Join(", ", Array.FindAll<string>(SelectColumns, x => x.StartsWith("Task"))).Replace("Task", ""); //task attributes all begin with "task". Sub task attributres all begin with "Sub-Task". 
            string subTaskAttributes = String.Join(", ", Array.FindAll<string>(SelectColumns, x => x.StartsWith("Sub-Task"))).Replace("Sub-Task", "");
            if (taskAttributes.Length > 0)
            {
                header3 += Regex.Replace("Task Criteria: " + taskAttributes, ".{80,120},", "$0\n,"); //this regular expression will add new lines after a comma when seperated by at least 30 characters. This is useful because text in a header does not word wrap. 
            }
            if (subTaskAttributes.Length > 0)
            {
                header3 += Regex.Replace("\nSubtask Criteria: " + subTaskAttributes, ".{80,120},", "$0\n,");
            }
        }
        if (!Object.ReferenceEquals(Section1, null))
        {
            header1 += Regex.Replace("Section 1: " + String.Join(", ", Section1), ".{80,120},", "$0\n,");
        }
        else if (!Object.ReferenceEquals(Section2, null))
        {
            header1 += Regex.Replace("Section 1: " + String.Join(", ", Section2), ".{80,120},", "$0\n,");
        }
        if (!Object.ReferenceEquals(Section1, null) && !Object.ReferenceEquals(Section2, null))
        {
            header1 += Regex.Replace("\nSection 2: " + String.Join(", ", Section2), ".{80,120},", "$0\n,");
        }
        if (dev)
        {
            header1 += "\nTeam(s): Dev";
        }
        else if (bus)
        {
            header1 += "\nTeam(s): Bus";
        }
        if (dev && bus)
        {
            header1 += ",Bus";
        }
        if (!object.ReferenceEquals(filters, null) && filters.Count > 0)
        {
            footer1 = "&\"Calibri\"&8 Applied Filters: ";
            foreach(FilterObject f in filters)
            {
                footer1 += " " + f.text + ": " + f.value;
            }
        }
        footer1 = Regex.Replace(footer1, ".{120,160},", "$0\n,");
        //unreadable syntax is unreadable http://www.aspose.com/docs/display/cellsnet/Setting+Headers+and+Footers
        page.SetHeader(0, "&\"Calibri\"&8 " + header1 + "\n"); //selected overviews and teams


        var ddlValue = "Default";
        try
        {
            ddlValue = HttpContext.Current.Session["ddlValue"].ToString();
        }
        catch (Exception)
        {
        }


        if (ddlValue.ToString().ToLower() == "default (backlog)")
            page.SetHeader(1, "&\"Calibri,Bold\"&14 BACKLOG SUMMARY REPORT");  //title
        else
            page.SetHeader(1, "&\"Calibri,Bold\"&14 WORKLOAD SUMMARY REPORT");  //title
        page.SetHeader(2, "&\"Calibri\"&8 " + header3); //selected columns
        page.SetFooter(0, footer1); //applied filters
        page.SetFooter(2, "&\"Calibri\"&8 Created By: " + HttpContext.Current.User.Identity.Name + "\nPrinted On: &D\nPage &P of &N \n"); //page number, user name, and date
        page.Orientation = PageOrientationType.Landscape;

    }
    [WebMethod (EnableSession=true)]
    public static void saveReportData(String SummaryOverviewsSection1, String SummaryOverviewsSection2, String Organization, String SelectedColumns, String filtersJSON, String Delimeter, String ddlValue, String Excel)
    {
        //this method calls the data base stored procedure in AJAX and binds the data to the session. The javascript will then trigger a post back, so the aspose stuff can execute and create the report.
        DataSet ds = new DataSet();
        List<FilterObject> filters = new JavaScriptSerializer().Deserialize<List<FilterObject>>(filtersJSON);

        // 13419 - 7
        var backLog = "";
        if (ddlValue.ToString().ToLower() == "default (backlog)")
            backLog = "true";
        else
            backLog = "false";

        ds = WTS_Reports.getWorkloadSummary(SummaryOverviewsSection1, SummaryOverviewsSection2, Organization, filters, Delimeter, backLog);
        HttpContext.Current.Session["WorkloadSummaryData"] = ds;
        HttpContext.Current.Session["SelectedColumns"] = SelectedColumns;
        HttpContext.Current.Session["SummaryOverviewsSecion1"] = SummaryOverviewsSection1;
        HttpContext.Current.Session["SummaryOverviewsSecion2"] = SummaryOverviewsSection2;
        HttpContext.Current.Session["filters"] = filters;

        // 13419 - 7:
        HttpContext.Current.Session["ddlValue"] = ddlValue;

        HttpContext.Current.Session["Excel"] = Excel;

        return;
    }
    [WebMethod(EnableSession = true)]
    public static void updateReportParameters(string JSON, int paramsID)
    {
        string errorMessage = WTS_Reports.update_Report_Parameters(JSON, paramsID, HttpContext.Current.User.Identity.Name);
        if (errorMessage != "Success")
        {
            throw new Exception(errorMessage);
        }
    }
    [WebMethod(EnableSession = true)]
    public static KeyValuePair<int, string> addReportParameters(string JSON, string name, int reportID, int userID, bool Process)
    {
        string userName = HttpContext.Current.User.Identity.Name;
        int paramsID = 0;
        string errorMessage = WTS_Reports.createReportParameters(JSON, reportID, name, userID, userName, Process, ref paramsID);
        if (errorMessage != "Success")
        {
            throw new Exception(errorMessage);
        }
        return new KeyValuePair<int, string>(paramsID, name);
    }

    [WebMethod(EnableSession = true)]
    public static void deleteReportParameters(int paramsID)
    {
        string errorMessage = WTS_Reports.deleteReportParameters(paramsID);
    }

    [WebMethod(EnableSession = true)]
    public static string getReportParameters(int paramsID)
    {
        string JSON = "";
        string errorMessage = WTS_Reports.get_Report_Parameters(ref JSON, paramsID);
        if (errorMessage != "Success")
        {
            throw new Exception(errorMessage);
        }
        return JSON;
    }
}