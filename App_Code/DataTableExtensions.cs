using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

public static class DataTableExtensions
{
	public static void SetColumnOrder(this DataTable dt, string data, string[] colNames, int currentLevel)
	{
		var columnIndex = 1;
		var delimiter = ',';
		var columnNames = new List<string>();
	    var allColNames = new List<string>();
        dynamic d = JsonConvert.DeserializeObject(data);
		var colIndexOrder = d.columnorder.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Replace(" ", "").Replace("\r\n", "").Trim().Split(delimiter);

	    if (currentLevel > 1)
	    {
            //Do this for the child rows
	        var subgridLevel = 2;
	        foreach (var obj in d)
	            if (obj.Name.IndexOf("subgrid") != -1)
	            {
	                if (subgridLevel == currentLevel)
	                {
	                    foreach (var items in obj)
	                        foreach (var item in items[0])
	                            if (item.Name.IndexOf("tblCols") != -1)
	                                foreach (var col in item)
	                                    foreach (var i in col)
	                                        allColNames.Add(i.name.ToString());

	                    foreach (var items in obj)
	                        foreach (var item in items[0])
	                            if (item.Name.IndexOf("columnorder") != -1)
	                                colIndexOrder = item.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Replace(" ", "").Replace("\r\n", "").Replace("columnorder:", "").Trim().Split(delimiter);
	                }
	                subgridLevel++;
	            }
	    }
	    else
	    {
            //Do this for the parent row
	        foreach (var col in d.tblCols)
	        {
		        if(dt.Columns.Count > 1 && dt.Columns[1].ToString().IndexOf("_ID") != -1) allColNames.Add(col.name.ToString());
				else allColNames.Add(col.alias.ToString() != "" ? col.alias.ToString() : col.name.ToString());
	        }
	    }

        //Get the column names in an ordinal position of the colIndexOrder int array
        for (int i = 0; i < colNames.Length; i++)
            columnNames.Add(allColNames[Convert.ToInt32(colIndexOrder[i]) - 1]);

        //Set the ordinal of the columns based on the position from the previous array
	    foreach (var name in columnNames)
	    {
	        try
	        {
	            dt.Columns[name].SetOrdinal(columnIndex);
	            columnIndex++;
	        }
	        catch (Exception){}
	    }
	}

	public static void SetSortOrder(this DataTable dt, string data, int currentLevel)
	{
		dynamic d = JsonConvert.DeserializeObject(data);
        var sortOrder = new StringBuilder();
	    var sortCount = 0;

        if (currentLevel == 1)
		{
            foreach (var item in d.tblCols)
                if (item.show == true && item.sortorder != "none")
                    sortCount++;

            if (sortCount > 0)
            {
                for (int i = 1; i < sortCount + 1; i++)
                    foreach (var item in d.tblCols)
                        if (item.show == true && item.sortorder.ToString() != "none" && item.sortpriority == i)
                        {
                            if (i == 1)
                                sortOrder.Append("[" + item.name.ToString() + "] " + item.sortorder.ToString());
                            else
                                sortOrder.Append(", [" + item.name.ToString() + "] " + item.sortorder.ToString());
                        }

                dt.DefaultView.Sort = sortOrder.ToString();
            }
        }
        else
		{
			var subgridLevel = 2;
            foreach (var obj in d)
                if (obj.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == currentLevel)
                    {
                        foreach (var items in obj)
                            foreach (var item in items[0])
                                if (item.Name.IndexOf("tblCols") != -1)
                                {
                                    foreach (var col in item)
                                        foreach (var i in col)
                                            if (i.show == true && i.sortorder != "none")
                                                sortCount++;

                                    if (sortCount > 0)
                                    {
                                        for (int i = 1; i < sortCount + 1; i++)
                                            foreach (var col in item)
                                                foreach (var j in col)
                                                    if (j.show == true && j.sortorder.ToString() != "none" && j.sortpriority == i)
                                                    {
                                                        
                                                            sortOrder.Append(", [" + j.name.ToString() + "] " + j.sortorder.ToString());
                                                    }

                                        //dt.DefaultView.Sort = sortOrder.ToString().Remove(0,1);
                                    }
                                }
                    }
                    subgridLevel++;
                }
		}
	}

	public static void SetColumnFilters(this DataTable dt, string data, int currentLevel)
	{

		dynamic d = JsonConvert.DeserializeObject(data);
		DataColumnCollection columns = dt.Columns;

		if (currentLevel == 1)
		{
			foreach (var item in d.tblCols)
				if (columns.Contains(item.name.ToString()))
					if (item.show == false)
						dt.Columns.Remove("" + item.name + "");
		}
		else
		{
			var subgridLevel = 2;
			foreach (var obj in d)
				if (obj.Name.IndexOf("subgrid") != -1)
				{
					if (subgridLevel == currentLevel)
						foreach (var item in obj)
							foreach (var col in item[0].tblCols)
								if (columns.Contains(col.name.ToString()))
									dt.Columns.Remove("" + col.name + "");

					subgridLevel++;
				}
		}
	}

	public static void SetConcatCols(this DataTable dt, string data, int currentLevel)
	{
		dynamic d = JsonConvert.DeserializeObject(data);
		DataColumnCollection columns = dt.Columns;

        if (currentLevel == 1)
		{
            foreach (var item in d.tblCols)
                if (item.concat == true)
                    foreach (var i in item.catcols)
                        foreach (DataColumn col in columns)
                            if (i.ToString() == col.ColumnName)
                                col.ColumnName = col.ColumnName.Replace(" ", "_");

            foreach (var item in d.tblCols)
				if (item.concat == true)
				{
                    var newCol = new DataColumn("" + item.name + "", typeof(string));
                    var expression = "";

                    foreach (var i in item.catcols)
                    {
                        if (string.IsNullOrEmpty(expression.Trim())) expression = "[" + i.ToString().Replace(" ", "_") + "]";
                        else expression += " + ' - ' + " + "[" + i.ToString().Replace(" ", "_") + "]";
                    }

                    newCol.Expression = expression;
                    columns.Add(newCol);
                }

		    foreach (var item in d.tblCols)
		        if (item.concat == true)
		            foreach (var i in item.catcols)
		            foreach (DataColumn col in columns)
		                if (i.ToString().Replace(" ", "_") == col.ColumnName)
		                    col.ColumnName = col.ColumnName.Replace("_", " ");
        }
        else
		{
			var subgridLevel = 2;
			foreach (var obj in d)
				if (obj.Name.IndexOf("subgrid") != -1)
				{
					if (subgridLevel == currentLevel)
                    {
                        foreach (var item in obj)
                            foreach (var col in item[0].tblCols)
                                if (col.concat == true)
                                    foreach (var i in col.catcols)
                                        foreach (DataColumn dc in columns)
                                            if (i.ToString() == dc.ColumnName)
                                                dc.ColumnName = dc.ColumnName.Replace(" ", "_");

                        foreach (var item in obj)
                            foreach (var col in item[0].tblCols)
                            {
                                if (col.concat == true)
                                {
                                    var newCol = new DataColumn("" + col.name + "", typeof(string));
                                    var expression = "";

                                    foreach (var i in col.catcols)
                                    {
                                        if (string.IsNullOrEmpty(expression.Trim())) expression = "[" + i.ToString().Replace(" ", "_") + "]";
                                        else expression += " + ' - ' + " + "[" + i.ToString().Replace(" ", "_") + "]";
                                    }

                                    newCol.Expression = expression;
                                    columns.Add(newCol);
                                }
                            }

                        foreach (var item in obj)
                            foreach (var col in item[0].tblCols)
                                if (col.concat == true)
                                    foreach (var i in col.catcols)
                                        foreach (DataColumn dc in columns)
                                            if (i.ToString().Replace(" ", "_") == dc.ColumnName)
                                                dc.ColumnName = dc.ColumnName.Replace("_", " ");
                    }
                    subgridLevel++;
				}
		}
    }

    //Set column names to the ones found in the json
    public static void SetColNames(this DataTable dt, string data, int currentLevel)
    {
        dynamic d = JsonConvert.DeserializeObject(data);
        DataColumnCollection columns = dt.Columns;

        if (currentLevel == 1)
        {
            foreach (var item in d.tblCols)
                if(!string.IsNullOrEmpty(item.alias.ToString()))
                    foreach (DataColumn dc in columns)
                        if (dc.ColumnName == item.name.ToString())
                            dc.ColumnName = item.alias.ToString();
        }
        else
        {
            var subgridLevel = 2;
            foreach (var obj in d)
                if (obj.Name.IndexOf("subgrid") != -1)
                {
                    if (subgridLevel == currentLevel)
                        foreach (var item in obj)
                            foreach (var col in item[0].tblCols)
                                if (!string.IsNullOrEmpty(col.alias.ToString()))
                                    foreach (DataColumn dc in columns)
                                        if (dc.ColumnName == col.name.ToString())
                                            dc.ColumnName = col.alias.ToString();
                    subgridLevel++;
                }
        }
    }

    public static void FilterColNames(this DataTable dt, string colName, string filter)
    {
        dt.DefaultView.RowFilter = "[" + colName + "] LIKE '%" + filter + "%'";
    }

    /// <summary>
    /// This function combines dt rows by merging column values.
    /// 
    /// For example, if your DT contained:
    /// ROW 1   A   5
    /// ROW 2   B   10
    /// ROW 2   B   11
    /// 
    /// The resulting DT would contain:
    /// ROW 1   A   5
    /// ROW 2   B   10 11
    /// </summary>
    public static DataTable CombineRowsOnColumn(this DataTable dt, string colName, string separator, bool renameCombinedColumn)
    {
        if (string.IsNullOrWhiteSpace(colName))
        {
            throw new ArgumentException("Column name cannot be empty.");
        }        

        return CombineRowsOnColumns(dt, new string[] { colName }, string.IsNullOrWhiteSpace(separator) ? null : new string[] { separator }, renameCombinedColumn);
    }

    /// <summary>
    /// This function combines dt rows by merging one or more column values. Combined columns are converted to strings.
    /// 
    /// For example, if your DT contained:
    /// ROW 1   A   5
    /// ROW 2   B   10
    /// ROW 2   B   11
    /// 
    /// The resulting DT would contain:
    /// ROW 1   A   5
    /// ROW 2   B   10 11
    /// </summary>
    public static DataTable CombineRowsOnColumns(this DataTable dt, string[] colNames, string[] separators, bool renameCombinedColumns)
    {
        if (colNames == null || colNames.Length == 0)
        {
            throw new ArgumentException("Column names cannot be empty.");
        }

        if (separators != null && separators.Length != colNames.Length)
        {
            throw new ArgumentException("Separator array must be NULL or contain the same number of separators as the specified columns.");
        }

        List<string> colNamesList = new List<string>(colNames);

        // Sample Data:
        // NAME     LETTER  NUMBER
        // ROW 1    A       5
        // ROW 2    B       10
        // ROW 3    B       11

        // Step 1: Create a distinct list
        // Step 2: Create placeholder columns to combine the data
        // Step 3: Insert the data from the columns into the distinct list

        // Step 1:
        // NAME     LETTER
        // ROW 1    A
        // ROW 2    B

        // Step 2:
        // NAME     LETTER  NUMBER
        // ROW 1    A       [NULL]
        // ROW 2    B       [NULL]

        // Step 3:
        // NAME     LETTER  NUMBER
        // ROW 1    A       5
        // ROW 2    B       10 11

        // STEP 1:        

        // store off the original column locations so we can put them back in the correct places (we need to store them in column index order so we can insert them correctly)
        SortedDictionary<int, string> originalColumnLocations = new SortedDictionary<int, string>();
        for (int i = 0; i < colNames.Length; i++)
        {
            if (dt.Columns.Contains(colNames[i]))
            {
                originalColumnLocations[dt.Columns.IndexOf(colNames[i])] = colNames[i];
            }
            else
            {
                throw new ArgumentException("Column name " + colNames[i] + " does not belong to the source data table.");
            }            
        }
        
        // remove the columns
        DataTable dtWithoutColumns = dt.Copy();
        for (int i = 0; i < colNames.Length; i++)
        {            
            dtWithoutColumns.Columns.Remove(colNames[i]);
        }

        // merge distinct on the new column list
        dtWithoutColumns.AcceptChanges();
        dtWithoutColumns = dtWithoutColumns.DefaultView.ToTable(true);

        // STEP 2:

        // insert the new columns in the correct index locations
        foreach (var kvp in originalColumnLocations)
        {
            int idx = kvp.Key;
            string colName = kvp.Value;

            DataColumn colSource = dt.Columns[colName];

            DataColumn dc = new DataColumn(colName, typeof(string));
            dtWithoutColumns.Columns.Add(dc);
            dc.SetOrdinal(idx);
        }

        // make sure Z stays at the end
        {
            dtWithoutColumns.Columns.Remove("Z");
            dtWithoutColumns.Columns.Add("Z", typeof(string));
        }

        // STEP 3:

        // find the matching children (search all columns but the ones being combined - if the other columns match, the rows can be combined)
        for (int i = 0; i < dtWithoutColumns.Rows.Count; i++)
        {
            DataRow distinctRow = dtWithoutColumns.Rows[i];

            List<DataRow> matchingRows = new List<DataRow>();

            for (int x = 0; x < dt.Rows.Count; x++)
            {
                DataRow sourceRow = dt.Rows[x];

                bool rowsMatch = true;

                for (int y = 0; y < dtWithoutColumns.Columns.Count; y++)
                {
                    string colName = dtWithoutColumns.Columns[y].ColumnName;

                    // DON'T COMPARE THE COMBINE COLUMNS!
                    if (!colNamesList.Contains(colName) && colName != "X" && colName != "Y" && colName != "Z")
                    {
                        object distinctObj = distinctRow[colName];
                        object sourceObj = sourceRow[colName];

                        if (distinctObj == null && sourceObj == null)
                        {
                            rowsMatch = true;
                        }
                        else if (distinctObj == DBNull.Value && sourceObj == DBNull.Value)
                        {
                            rowsMatch = true;
                        }
                        else if (distinctObj.ToString() != sourceObj.ToString())
                        {
                            rowsMatch = false;
                            break;
                        }
                    }
                }

                if (rowsMatch)
                {
                    matchingRows.Add(sourceRow);                    
                }
            }

            if (matchingRows.Count > 0)
            {
                Dictionary<string, StringBuilder> combinedColumns = new Dictionary<string, StringBuilder>();

                for (int x = 0; x < matchingRows.Count; x++)
                {
                    DataRow matchingRow = matchingRows[x];

                    for (int c = 0; c < colNames.Length; c++)
                    {
                        string colName = colNames[c];
                        object sourceObj = matchingRow[colName];
                        string sep = separators != null ? separators[c] : " ";

                        if (sourceObj != null && sourceObj != DBNull.Value)
                        {
                            string sourceStr = sourceObj.ToString();

                            if (!string.IsNullOrWhiteSpace(sourceStr))
                            {
                                StringBuilder combinedStr = null;

                                if (combinedColumns.ContainsKey(colName))
                                {
                                    combinedStr = combinedColumns[colName];
                                    combinedStr.Append(sep);
                                }
                                else
                                {
                                    combinedStr = new StringBuilder();
                                    combinedColumns[colName] = combinedStr;
                                }

                                combinedStr.Append(sourceStr.Trim());
                            }
                        }
                    }
                }

                for (int c = 0; c < colNames.Length; c++)
                {
                    string colName = colNames[c];

                    if (combinedColumns.ContainsKey(colName))
                    {
                        distinctRow[colName] = combinedColumns[colName].ToString();
                    }
                    else
                    {
                        distinctRow[colName] = (object)DBNull.Value;
                    }
                }

                // we remove matching rows from the source data set so we don't re-compare them again later (we already found where they matched)
                // we do this at the end, because once a row is marked as removed, the DataRow values all contain NULL
                for (int x = 0; x < matchingRows.Count; x++)
                {
                    dt.Rows.Remove(matchingRows[x]);
                }
                dt.AcceptChanges();
            }
        }

        // now that we are done, rename the combined columns if specified by the user
        if (renameCombinedColumns)
        {
            for (int c = 0; c < colNames.Length; c++)
            {
                string colName = colNames[c];

                DataColumn col = dtWithoutColumns.Columns[colName];
                col.ColumnName = col.ColumnName + "_COMBINED";
            }

            dtWithoutColumns.AcceptChanges();
        }

        return dtWithoutColumns;
    }
}