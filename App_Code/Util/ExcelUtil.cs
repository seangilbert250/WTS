﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

using Aspose.Cells;

namespace WTS.Util
{
    public class ExcelUtil
    {
        private Dictionary<int, List<ExcelDataTableFormat>> sheetFormats = new Dictionary<int, List<ExcelDataTableFormat>>();
        private Dictionary<string, string> columnRenames = new Dictionary<string, string>();
        public bool GroupRows = false;
        public Dictionary<int, string> sheetNames = new Dictionary<int, string>();
        public MultiLevelCrosswalkRowComparator RowComparator { get; set; }
        public bool HideIDColumns = true;

        public ExcelWorkbook CreateExcelWorkbook()
        {
            return new ExcelWorkbook();
        }

        public Dictionary<int, List<ExcelDataTableFormat>> SheetFormats
        {
            get
            {
                return sheetFormats;
            }

            set
            {
                sheetFormats = value;
            }
        }

        public Dictionary<string, string> ColumnRenames
        {
            get
            {
                return columnRenames;
            }

            set
            {
                columnRenames = value;
            }
        }

        public Dictionary<int, string> SheetNames
        {
            get
            {
                return sheetNames;
            }

            set
            {
                sheetNames = value;
            }
        }

        public void SetSheetName(int idx, string name)
        {
            sheetNames[idx] = name;
        }

        public void SetSheetFormat(int idx, ExcelDataTableFormat format)
        {
            sheetFormats[idx] = new List<ExcelDataTableFormat>() { format };
        }

        public void SetSheetFormats(int idx, List<ExcelDataTableFormat> formats)
        {
            sheetFormats[idx] = formats;
        }

        public void WriteDataTableToResponse(DataTable dt, HttpResponse response, bool endResponse = true)
        {
            WTS.Util.ExcelWorkbook ew = new WTS.Util.ExcelWorkbook();

            Worksheet ws = ew.Workbook.Worksheets[0];
            ws.Cells.ImportDataTable(dt, true, 0, 0, false, false);
            ws.AutoFitColumns();
                        
            ew.WriteToResponse(response, true);
        }

        public void WriteMultiLevelCrosswalkToResponse(
            List<DataTable> dts,
            string sheetName,
            HttpResponse response,
            bool endResponse = true)
        {
            SetSheetName(0, sheetName);

            List<List<DataTable>> dtSheets = new List<List<DataTable>>();
            dtSheets.Add(dts);

            WriteMultiLevelCrosswalkToResponse(dtSheets, response, endResponse);
        }

        /// <summary>
        /// This version allows you to specify multiple worksheets worth of data sets.
        /// </summary>
        /// <param name="dtSheets"></param>
        /// <param name="response"></param>
        /// <param name="endResponse"></param>
        public void WriteMultiLevelCrosswalkToResponse(
            List<List<DataTable>> dtSheets, 
            HttpResponse response, 
            bool endResponse = true)
        {
            WTS.Util.ExcelWorkbook ew = new WTS.Util.ExcelWorkbook();

            if (dtSheets == null)
            {
                throw new InvalidDataException("Data sheets cannot be null.");
            }
            
            ew.Workbook.Worksheets.Clear();

            for (int i = 0; i < dtSheets.Count; i++)
            {
                ew.Workbook.Worksheets.Add();
            }

            for (int i = 0; i < dtSheets.Count; i++)
            {
                if (sheetNames != null && sheetNames.ContainsKey(i))
                {
                    ew.Workbook.Worksheets[i].Name = sheetNames[i];
                }
            }

            for (int i = 0; i < dtSheets.Count; i++)
            {
                Worksheet ws = ew.Workbook.Worksheets[i];

                List<DataTable> dts = dtSheets[i];

                if (dts != null && dts.Count > 0)
                {
                    List<ExcelDataTableFormat> formats = null;
                    if (sheetFormats.ContainsKey(i))
                    {
                        formats = sheetFormats[i];
                    }

                    ExportDataTablesToWorksheet(ew, ws, dts, formats);
                    ws.AutoFitColumns();

                    // now that we've auto-fit columns, we go back and re-set the widths for columns that have explicit preferences
                    for (int f = 0; formats != null && f < formats.Count; f++)
                    {
                        ExcelDataTableFormat format = formats[f];

                        foreach (string key in format.ColumnWidths.Keys)
                        {
                            if (key.StartsWith("COL_"))
                            {
                                var width = format.ColumnWidths[key];
                                int col = Int32.Parse(key.Substring(4));

                                ws.Cells.SetColumnWidth(col, width);
                            }
                        }
                    }

                    // now that we're done, extend all styles out 100 columns
                    for (int r = 0; r < ws.Cells.Rows.Count; r++)
                    {
                        Style lastRowStyle = null;
                        Style finalRowStyle = null; // this allows us to share the style for the whole row

                        for (int c = 0; c < 100; c++)
                        {
                            Cell cell = ws.Cells[r, c];

                            if (cell != null)
                            {
                                if (cell.IsStyleSet)
                                {
                                    lastRowStyle = cell.GetStyle();
                                }
                                else
                                {
                                    // we've reached the "end" of the rows set in the data tables
                                    if (finalRowStyle == null)
                                    {
                                        finalRowStyle = new Style();
                                        finalRowStyle.Copy(lastRowStyle);
                                        finalRowStyle.IsTextWrapped = false;
                                    }

                                    cell.SetStyle(finalRowStyle);
                                }
                            }
                        }
                    }
                }
            }

            ew.WriteToResponse(response, true);
        }

        /// <summary>
        /// This function does the work of combining multiple levels of data tables into one worksheet.
        /// </summary>
        /// <param name="dts"></param>
        /// <param name="formats"></param>
        private void ExportDataTablesToWorksheet(ExcelWorkbook ew, Worksheet ws, List<DataTable> dts, List<ExcelDataTableFormat> formats)
        {
            DataTable dt = dts[0];

            Dictionary<string, Style> styleCollection = new Dictionary<string, Style>(); // we use this to cache and share styles because Excel can only handle a certain number of unique styles before it errors

            int level = 0;
            int row = 0;

            ExcelDataTableFormat format = formats != null && level < formats.Count ? formats[level] : ExcelDataTableFormat.GetDefaultFormat(level, dts.Count);
            if (format.ShowHeader)
            {
                AddHeaderToRow(ew, ws, level, dts.Count, row++, dt, formats, styleCollection);
            }

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    int rowStart = row;

                    AddDataToRow(ew, ws, level, dts.Count, ref row, dts, dr, formats, styleCollection);

                    int rowFinish = row;

                    if (GroupRows && level < dts.Count)
                    {
                        if (rowFinish - rowStart > 1)
                        {
                            ws.Cells.GroupRows(rowStart + 1, rowFinish - 1);
                        }
                    }
                }
            }
        }

        private void AddHeaderToRow(ExcelWorkbook ew, Worksheet ws, int level, int totalLevels, int row, DataTable dt, List<ExcelDataTableFormat> formats, Dictionary<string, Style> styleCollection)
        {
            int currentColumn = 0;

            ExcelDataTableFormat format = formats != null && level < formats.Count ? formats[level] : ExcelDataTableFormat.GetDefaultFormat(level, totalLevels);

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                string columnName = dt.Columns[c].ColumnName;

                if (!IsColumnHidden(columnName))
                {
                    string name = columnName;
                    if (columnRenames != null && columnRenames.ContainsKey(columnName))
                    {
                        name = columnRenames[columnName];
                    }

                    WriteCell(ew, ws, level, totalLevels, row, currentColumn, name, format, true, styleCollection);

                    if (format.ColumnWidths.ContainsKey(columnName))
                    {
                        format.ColumnWidths["COL_" + currentColumn.ToString()] = format.ColumnWidths[columnName];
                    }

                    currentColumn++;
                }
            }
        }

        private void AddDataToRow(ExcelWorkbook ew, Worksheet ws, int level, int totalLevels, ref int row, List<DataTable> dts, DataRow dr, List<ExcelDataTableFormat> formats, Dictionary<string, Style> styleCollection)
        {
            int currentColumn = 0;

            DataTable dt = dts[level];
            ExcelDataTableFormat format = formats != null && level < formats.Count ? formats[level] : ExcelDataTableFormat.GetDefaultFormat(level, totalLevels);

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                string columnName = dt.Columns[c].ColumnName;

                if (!IsColumnHidden(columnName))
                {
                    object data = dr[dt.Columns[c]];

                    WriteCell(ew, ws, level, totalLevels, row, currentColumn, data, format, false, styleCollection);

                    if (format.ColumnWidths.ContainsKey(columnName))
                    {
                        format.ColumnWidths["COL_" + currentColumn.ToString()] = format.ColumnWidths[columnName]; // store the width off for later
                    }

                    currentColumn++;
                }
            }

            row++;

            if ((level + 1) < dts.Count)
            {
                DataTable childDt = dts[level + 1];
                ExcelDataTableFormat childFormat = formats != null && (level + 1) < formats.Count ? formats[level + 1] : ExcelDataTableFormat.GetDefaultFormat(level, totalLevels);

                if (childDt != null && childDt.Rows.Count > 0)
                {
                    List<DataRow> childRows = GetChildRowsForCurrentRow(dt, dr, childDt);

                    if (childRows != null && childRows.Count > 0)
                    {
                        int rowStart = row;

                        if (childFormat.ShowHeader)
                        {
                            AddHeaderToRow(ew, ws, level + 1, totalLevels, row++, childDt, formats, styleCollection);
                        }

                        for (int c = 0; c < childRows.Count; c++)
                        {
                            DataRow childRow = childRows[c];

                            AddDataToRow(ew, ws, level + 1, totalLevels, ref row, dts, childRow, formats, styleCollection);
                            childRow.Delete(); // by removing the child rows that have matches as we progress we save time for future comparisons (each child row can have only one match)
                        }

                        int rowFinish = row;

                        if (GroupRows)
                        {
                            if (rowFinish - rowStart > 0)
                            {
                                ws.Cells.GroupRows(rowStart, rowFinish - 1);
                            }
                        }

                        childDt.AcceptChanges();
                    }
                }
            }            
        }

        private List<DataRow> GetChildRowsForCurrentRow(DataTable dt, DataRow row, DataTable childDt)
        {
            List<DataRow> childRows = new List<DataRow>();

            List<string> idColumns = new List<string>();
            Dictionary<string, string> idValues = new Dictionary<string, string>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string columnName = dt.Columns[i].ColumnName;

                if (columnName.EndsWith("_ID") || columnName.EndsWith("_ID_COUNTCOLUMN"))
                {
                    idColumns.Add(columnName);

                    object val = row[columnName];

                    if (val != DBNull.Value)
                    {
                        idValues.Add(columnName, val.ToString());
                    }
                    else
                    {
                        idValues.Add(columnName, "");
                    }
                }
            }

            for (int i = 0; i < childDt.Rows.Count; i++)
            {
                DataRow childRow = childDt.Rows[i];

                bool match = true;

                for (int x = 0; match && x < idColumns.Count; x++)
                {
                    string columnName = idColumns[x];
                    
                    string parentVal = idValues[columnName];
                    string childVal = null;

                    // the column name could be SYSTEM_ID, but in the child table it could be SYSTEM_ID_COUNTCOLUMN
                    object childValObj = null;

                    if (childDt.Columns.Contains(columnName))
                    {
                        childValObj = childRow[columnName];
                    }
                    else if (childDt.Columns.Contains(columnName + "_COUNTCOLUMN"))
                    {
                        childValObj = childRow[columnName + "_COUNTCOLUMN"];
                    }
                    else
                    {
                        // the column isn't shared between the two sets (some columns from parent are exluded from being passed down)
                        // in this case, we just ignore it
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(parentVal))
                    {
                        parentVal = null;
                    }

                    if (childValObj != null && childValObj != DBNull.Value)
                    {
                        childVal = childValObj.ToString();

                        if (string.IsNullOrWhiteSpace(childVal))
                        {
                            childVal = null;
                        }
                    }

                    if (RowComparator != null) // row comparator is needed when special logic needs to be done other than just comparing column to column
                    {
                        match &= RowComparator.DoesChildRowColumnMatchParentRowColumn(row, parentVal, childRow, childVal, columnName);
                    }
                    else if (parentVal != childVal)
                    {
                        match = false;
                    }
                }

                if (match)
                {
                    childRows.Add(childRow);                    
                }
            }

            return childRows;
        }

        private void WriteCell(ExcelWorkbook ew, Worksheet ws, int level, int totalLevels, int row, int column, object val, ExcelDataTableFormat format, bool useHeaderFormats, Dictionary<string, Style> styleCollection)
        {
            Cell cell = ws.Cells[row, column];
            cell.Value = val;

            bool cellHasLineBreaks = false;

            if (val != null && val is string)
            {
                if (string.IsNullOrWhiteSpace((string)val))
                {
                    cell.Value = null;
                }
                else
                {
                    string s = (string)val;

                    s = StringUtil.StripHTML(s);
                    s = s.Replace("&nbsp;", " ");
                    s = s.Replace("&NBSP;", " ");
                    s = s.Replace("<br />", "\n");
                    s = s.Replace("<br>", "\n");
                    s = s.Replace("", "-");
                    cell.Value = s;
                    //if (s.IndexOf("<") != -1 && s.IndexOf(">") != -1)
                    //{
                    //cell.HtmlString = s; // the aspose throws errors on html it doesn't recognize; commenting out for now                        
                    //}

                    if (s.IndexOf("\n") != -1)
                    {
                        cellHasLineBreaks = true;
                    }
                }
            }

            Style style = null;

            if (useHeaderFormats)
            {
                if (styleCollection.Keys.Contains("L" + level + "_HEADER" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")))
                {
                    style = styleCollection["L" + level + "_HEADER" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")];
                }
                else
                {
                    style = new Style();                    
                    style.Pattern = BackgroundType.Solid;

                    style.ForegroundColor = format.HeaderBackgroundColor; // the aspost style.foreground color actually sets the background
                    
                    style.Font.Name = format.HeaderFontName;
                    style.Font.Size = format.HeaderFontSize;
                    style.Font.IsBold = format.HeaderFontBold;
                    style.Font.IsItalic = format.HeaderFontItatlic;
                    style.Font.Color = format.HeaderFontColor;
                    style.Font.Underline = format.HeaderFontUnderline ? FontUnderlineType.Single : FontUnderlineType.None;

                    CellBorderType cbt = CellBorderType.None;

                    if (format.HeaderBorderWidth > 0 && format.HeaderBorderWidth < 1.0f)
                    {
                        cbt = format.HeaderBorderDotted ? CellBorderType.Dotted : format.HeaderBorderDashed ? CellBorderType.Dashed : CellBorderType.Hair;
                    }
                    else if (format.HeaderBorderWidth >= 1.0f && format.HeaderBorderWidth < 2.0f)
                    {
                        cbt = format.HeaderBorderDotted ? CellBorderType.Dotted : format.HeaderBorderDashed ? CellBorderType.Dashed : CellBorderType.Thin;
                    }
                    else if (format.HeaderBorderWidth >= 2.0f)
                    {
                        cbt = format.HeaderBorderDotted ? CellBorderType.Dotted : format.HeaderBorderDashed ? CellBorderType.Dashed : CellBorderType.Medium;
                    }

                    if (cbt != CellBorderType.None)
                    {
                        style.SetBorder(BorderType.TopBorder, cbt, format.HeaderBorderColor);
                        style.SetBorder(BorderType.RightBorder, cbt, format.HeaderBorderColor);
                        style.SetBorder(BorderType.BottomBorder, cbt, format.HeaderBorderColor);
                        style.SetBorder(BorderType.LeftBorder, cbt, format.HeaderBorderColor);
                    }

                    styleCollection["L" + level + "_HEADER" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")] = style;
                }
            }
            else
            {
                if (styleCollection.Keys.Contains("L" + level + "_DATA" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")))
                {
                    style = styleCollection["L" + level + "_DATA" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")];
                }
                else
                {
                    style = new Style();                    
                    style.Pattern = BackgroundType.Solid;

                    style.ForegroundColor = format.BackgroundColor;  // the aspost style.foreground color actually sets the background

                    style.Font.Name = format.FontName;
                    style.Font.Size = format.FontSize;
                    style.Font.IsBold = format.FontBold;
                    style.Font.IsItalic = format.FontItatlic;
                    style.Font.Color = format.FontColor;
                    style.Font.Underline = format.FontUnderline ? FontUnderlineType.Single : FontUnderlineType.None;

                    if (format.AllowWrap)
                    {
                        style.IsTextWrapped = cellHasLineBreaks;
                    }

                    CellBorderType cbt = CellBorderType.None;

                    if (format.BorderWidth > 0 && format.HeaderBorderWidth < 1.0f)
                    {
                        cbt = format.BorderDotted ? CellBorderType.Dotted : format.BorderDashed ? CellBorderType.Dashed : CellBorderType.Hair;
                    }
                    else if (format.BorderWidth >= 1.0f && format.HeaderBorderWidth < 2.0f)
                    {
                        cbt = format.BorderDotted ? CellBorderType.Dotted : format.BorderDashed ? CellBorderType.Dashed : CellBorderType.Thin;
                    }
                    else if (format.BorderWidth >= 2.0f)
                    {
                        cbt = format.BorderDotted ? CellBorderType.Dotted : format.BorderDashed ? CellBorderType.Dashed : CellBorderType.Medium;
                    }

                    if (cbt != CellBorderType.None)
                    {
                        style.SetBorder(BorderType.TopBorder, cbt, format.BorderColor);
                        style.SetBorder(BorderType.RightBorder, cbt, format.BorderColor);
                        style.SetBorder(BorderType.BottomBorder, cbt, format.BorderColor);
                        style.SetBorder(BorderType.LeftBorder, cbt, format.BorderColor);
                    }

                    styleCollection["L" + level + "_DATA" + (cellHasLineBreaks ? "_WITHLINEBREAK" : "")] = style;
                }
            }

            cell.SetStyle(style);
        }


        private bool IsColumnHidden(string columnName)
        {
            if (columnName == null)
            {
                return true;
            }

            columnName = columnName.ToUpper();
            
            return (HideIDColumns && (columnName.EndsWith("_ID") || columnName.EndsWith("_ID_COMBINED")))
                || columnName.EndsWith("_COUNTCOLUMN") || columnName.EndsWith("_HDN") || columnName == "X" || columnName == "Y" || columnName == "Z";
        }
    }













    public class ExcelDataTableFormat
    {
        public bool ShowHeader = true;

        public string HeaderFontName = "Calibri";
        public Color HeaderFontColor = Color.Black;
        public Color HeaderBackgroundColor = Color.White;
        public int HeaderFontSize = 11;
        public bool HeaderFontBold = true;
        public bool HeaderFontItatlic = false;
        public bool HeaderFontUnderline = false;
        public Color HeaderBorderColor = Color.Black;
        public float HeaderBorderWidth = 1.0f;
        public bool HeaderBorderDotted = false;
        public bool HeaderBorderDashed = false;

        public string FontName = "Calibri";
        public Color FontColor = Color.Black;
        public Color BackgroundColor = Color.White;
        public int FontSize = 11;
        public bool FontBold = false;
        public bool FontItatlic = false;
        public bool FontUnderline = false;
        public Color BorderColor = Color.Black;
        public float BorderWidth = 1.0f;
        public bool BorderDotted = false;
        public bool BorderDashed = false;
        public bool AllowWrap = true;
        public bool IsHTML = false;

        public Dictionary<string, float> ColumnWidths = new Dictionary<string, float>();
        
        public static ExcelDataTableFormat GetDefaultFormat(int level, int totalLevels)
        {
            ExcelDataTableFormat format = null;

            format = new ExcelDataTableFormat();

            if (level == 0)
            {
                format.HeaderBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#f4b084");
                format.HeaderFontBold = true;

                if (totalLevels > 1)
                {
                    format.FontBold = true;
                    format.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#f7d5be");
                }
            }
            else if (level == 1)
            {
                format.HeaderBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#0070c0");
                format.HeaderFontColor = Color.White;

                if (totalLevels > 2)
                {
                    format.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#c5d8e5");
                }
            }
            else if (level == 2)
            {
                format.HeaderBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#ffff9b");
                format.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#ffffc8");
            }
            else if (level == 3)
            {
                format.HeaderBackgroundColor = System.Drawing.ColorTranslator.FromHtml("#dddddd");
                format.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#eeeeee");
            }
            
            return format;
        }
    }

    public interface MultiLevelCrosswalkRowComparator
    {
        bool DoesChildRowColumnMatchParentRowColumn(DataRow parentRow, string parentVal, DataRow childRow, string childVal, string columnName);
    }






    public class ExcelWorkbook
    {
        private Aspose.Cells.Workbook workbook;

        public ExcelWorkbook()
        {
            workbook = new Workbook(FileFormatType.Xlsx);
        }

        public Aspose.Cells.Workbook Workbook
        {
            get
            {
                return workbook;
            }
        }

        public Aspose.Cells.Worksheet GetWorksheet(int idx, bool createIfMissing = false)
        {
            if (idx < workbook.Worksheets.Count)
            {
                return workbook.Worksheets[idx];
            }
            else
            {
                if (createIfMissing)
                {
                    workbook.Worksheets.Add();

                    return workbook.Worksheets[workbook.Worksheets.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public Aspose.Cells.Worksheet CreateWorksheet(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                workbook.Worksheets.Add();
            }
            else
            {
                workbook.Worksheets.Add(name);
            }

            return workbook.Worksheets[workbook.Worksheets.Count - 1];
        }

        public void WriteToResponse(HttpResponse response, bool endResponse = true)
        {
            MemoryStream ms = new MemoryStream();
            workbook.Save(ms, SaveFormat.Xlsx);

            response.ContentType = "application/xlsx";
            response.AddHeader("content-disposition", "attachment; filename=RQMTGrid_Export.xlsx");
            response.BinaryWrite(ms.ToArray());

            if (endResponse)
            {
                response.End();
            }
        }
    }
}