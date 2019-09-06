using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

using Microsoft.Win32;
using Microsoft.VisualBasic;


/// <summary>
/// Summary description for GridCols
/// </summary>
public class GridCols
{
    private GridColsCollection _Columns = new GridColsCollection();

    private GridColsGroupsCollection _Groups = new GridColsGroupsCollection();
    public GridColsCollection Columns
    {
        get { return _Columns; }
        set { _Columns = value; }
    }

    public GridColsGroupsCollection Groups
    {
        get { return _Groups; }
        set { _Groups = value; }
    }

    private char _SortDelimeter = ';';
    public char SortDelimeter
    {
        get { return _SortDelimeter; }
        set { _SortDelimeter = value; }
    }

    private char _ColumnDelimeter1 = '~';
    public char ColumnDelimeter1
    {
        get { return _ColumnDelimeter1; }
        set { _ColumnDelimeter1 = value; }
    }

    private char _ColumnDelimeter2 = '|';
    public char ColumnDelimeter2
    {
        get { return _ColumnDelimeter2; }
        set { _ColumnDelimeter2 = value; }
    }

    private string[] _ForcedVisibleColumns;

    private string[] _AvailableForcedColumns;

    public void Initialize(ref DataTable dt, string SortDelimeter, string ColumnDelimeter1, string ColumnDelimeter2, string AvailableForcedVisibleColumns = "", string ForcedVisibleColumns = "", string ForcedVisibleColumnsDelimeter = "")
    {
        _SortDelimeter = Convert.ToChar(SortDelimeter);
        _ColumnDelimeter1 = Convert.ToChar(ColumnDelimeter1);
        _ColumnDelimeter2 = Convert.ToChar(ColumnDelimeter2);
        if (!string.IsNullOrEmpty(ForcedVisibleColumns))
        {
            string[] splitForcedColumns = ForcedVisibleColumns.Split(Convert.ToChar(ForcedVisibleColumnsDelimeter));
            string[] splitAvailableForcedVisibleColumns = AvailableForcedVisibleColumns.Split(Convert.ToChar(ForcedVisibleColumnsDelimeter));
            _ForcedVisibleColumns = splitForcedColumns;
            _AvailableForcedColumns = splitAvailableForcedVisibleColumns;
            UpdateForcedColumns();

        }
        ReorderDataTable(ref dt, DefaultColumnOrderToString());
    }

    public void UpdateForcedColumns()
    {
		for(int i = 0; i < _ForcedVisibleColumns.Length; i++)
        {
            try
            {
                _Columns.ItemByColumnName(_ForcedVisibleColumns[i]).Viewable = true;
                _Columns.ItemByColumnName(_ForcedVisibleColumns[i]).Visible = true;

            }
            catch (Exception ex)
            {
            }
        }
		for (int i = 0; i < _Columns.Count; i++ )
		{
			bool canChange = false;
			for(int y = 0; y < _AvailableForcedColumns.Length; y++)
			{
				if (_Columns.Item(i).ColumnName == _AvailableForcedColumns[y])
				{
					canChange = true;
					break; // TODO: might not be correct. Was : Exit Do
				}
			}
			if (canChange == true)
			{
				bool columnExists = false;
				for(int y = 0; y < _ForcedVisibleColumns.Length; y++)
				{
					if (_Columns.Item(i).ColumnName == _ForcedVisibleColumns[y])
					{
						columnExists = true;
						break; // TODO: might not be correct. Was : Exit Do
					}
				}

				if (columnExists == false)
				{
					_Columns.Item(i).Visible = false;
				}
			}
		}
    }

    public GridColsCollection SortableColumns()
    {
        GridColsCollection sColumns = new GridColsCollection();
        foreach (GridColsColumn sColumn in _Columns)
        {
            if (sColumn.CanSort == true)
            {
                sColumns.Add(sColumn.ColumnName, sColumn.DisplayName, sColumn.Visible, sColumn.CanSort);
                sColumns.Item(sColumns.Count - 1).ColumnIndex = sColumn.ColumnIndex;
                sColumns.Item(sColumns.Count - 1).Viewable = sColumn.Viewable;
            }
        }
        return Columns;
    }

    public GridColsCollection CurrentOrderedColumns()
    {
        GridColsCollection oColumns = new GridColsCollection();
		for(int i = 0; i < _Columns.Count; i++)
        {
            GridColsColumn testColumn = _Columns.ItemByColumnOrder(i);
            if ((testColumn == null) == false)
            {
                if (testColumn.CanReorder == true & testColumn.Viewable == true)
                {
                    oColumns.Add(testColumn.ColumnName, testColumn.DisplayName, testColumn.Visible, testColumn.CanSort);
                    oColumns.Item(oColumns.Count - 1).ColumnIndex = testColumn.ColumnIndex;
                    oColumns.Item(oColumns.Count - 1).Viewable = testColumn.Viewable;
                }
            }
        }
        return oColumns;
    }

    public GridColsCollection VisibleColumns()
    {
        GridColsCollection columns = new GridColsCollection();
        GridColsColumn gcc = null;

        for (int i = 0; i <= _Columns.Count; i++)
        {
            gcc = _Columns.ItemByColumnOrder(i);
            if ((gcc == null) == false && gcc.Visible == true)
            {
                columns.Add(gcc.ColumnName, gcc.DisplayName, gcc.Visible, gcc.CanSort);
                columns.Item(columns.Count - 1).ColumnIndex = gcc.ColumnIndex;
                columns.Item(columns.Count - 1).Viewable = gcc.Viewable;
            }
        }

        return columns;
    }

    public List<GridColsColumn> VisibleColumnsList()
    {
        List<GridColsColumn> columns = new List<GridColsColumn>();
        GridColsColumn gcc = null;

        for (int i = 0; i <= _Columns.Count; i++)
        {
            gcc = _Columns.ItemByColumnOrder(i);
            if ((gcc == null) == false && gcc.Visible == true)
            {
                columns.Add(gcc);
            }
        }

        return columns;
    }

    public string[] VisibleColumnNamesArray()
    {
        GridColsCollection columns = VisibleColumns();
        if ((columns == null) || columns.Count == 0)
        {
            return new string[] { "" };
        }
        string[] colArray = new string[columns.Count + 1];
        GridColsColumn gcc = null;

        for (int i = 0; i <= columns.Count; i++)
        {
            gcc = columns.ItemByColumnOrder(i);
            if ((gcc != null) && gcc.Visible)
            {
                colArray[i] = gcc.ColumnName;
            }
        }

        return colArray;
    }

    public string SortableColumnsToString()
    {
        string strSortable = "";
        try
        {
            GridColsCollection columns = SortableColumns();
            foreach (GridColsColumn sColumn in columns)
            {
                if (sColumn.CanSort == true)
                {
                    if (string.IsNullOrEmpty(strSortable))
                    {
                        strSortable = sColumn.SortName;
                    }
                    else
                    {
                        strSortable += _SortDelimeter + sColumn.SortName;
                    }
                }
            }

        }
        catch (Exception ex)
        {
        }

        return strSortable;
    }

    public string VisibleColumnsToString()
    {
        string strVisible = "";
        try
        {
            GridColsCollection columns = VisibleColumns();
            foreach (GridColsColumn sColumn in columns)
            {
                if (sColumn.Visible == true)
                {
                    if (string.IsNullOrEmpty(strVisible))
                    {
                        strVisible = sColumn.ColumnName;
                    }
                    else
                    {
                        strVisible += _SortDelimeter + sColumn.ColumnName;
                    }
                }
            }

        }
        catch (Exception ex)
        {
        }

        return strVisible;
    }

    public string DefaultColumnOrderToString()
    {
        string strOrder = "";
        try
        {
            foreach (GridColsColumn sColumn in _Columns)
            {
                if (string.IsNullOrEmpty(strOrder))
                {
                    strOrder = sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + sColumn.Visible.ToString().ToLower() + _ColumnDelimeter2 + sColumn.CanReorder.ToString().ToLower() + _ColumnDelimeter2 + sColumn.GroupName + _ColumnDelimeter2 + sColumn.Viewable;
                }
                else
                {
                    strOrder += _ColumnDelimeter1 + sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + sColumn.Visible.ToString().ToLower() + _ColumnDelimeter2 + sColumn.CanReorder.ToString().ToLower() + _ColumnDelimeter2 + sColumn.GroupName + _ColumnDelimeter2 + sColumn.Viewable;
                }
            }

        }
        catch (Exception ex)
        {
        }
        return strOrder;
    }

    public string DefaultColumnOrderToStringNumerical()
    {
        string strOrder = "";
        try
        {
            foreach (GridColsColumn sColumn in _Columns)
            {
                if (string.IsNullOrEmpty(strOrder))
                {
                    strOrder = sColumn.ColumnName + _ColumnDelimeter2 + (sColumn.DisplayName) + _ColumnDelimeter2 + (sColumn.Visible == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.CanReorder == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.IsCurrency == true ? 1 : 0) + _ColumnDelimeter2 + sColumn.GroupName;
                }
                else
                {
                    strOrder += _ColumnDelimeter1 + sColumn.ColumnName + _ColumnDelimeter2 + (sColumn.DisplayName) + _ColumnDelimeter2 + (sColumn.Visible == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.CanReorder == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.IsCurrency == true ? 1 : 0) + _ColumnDelimeter2 + sColumn.GroupName;
                }
            }

        }
        catch (Exception ex)
        {
        }
        return strOrder;
    }

    public string CurrentColumnOrderToStringNumerical()
    {
        string strOrder = "";
        try
        {
            for(int i = 0; i < _Columns.Count; i++)
            {
                GridColsColumn sColumn = _Columns.ItemByColumnOrder(i);
                if ((sColumn == null) == false)
                {
                    if (string.IsNullOrEmpty(strOrder))
                    {
                        strOrder = sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + (sColumn.Visible == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.CanReorder == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.IsCurrency == true ? 1 : 0) + _ColumnDelimeter2 + sColumn.GroupName;
                    }
                    else
                    {
                        strOrder += _ColumnDelimeter1 + sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + (sColumn.Visible == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.CanReorder == true ? 1 : 0) + _ColumnDelimeter2 + (sColumn.IsCurrency == true ? 1 : 0) + _ColumnDelimeter2 + sColumn.GroupName;
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }

        return strOrder;
    }

    public string CurrentColumnOrderToString()
    {
        string strOrder = "";
        try
        {
            for(int i = 0; i < _Columns.Count; i++)
            {
                GridColsColumn sColumn = _Columns.ItemByColumnOrder(i);
                if ((sColumn == null) == false)
                {
                    if (string.IsNullOrEmpty(strOrder))
                    {
                        strOrder = sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + sColumn.Visible.ToString().ToLower() + _ColumnDelimeter2 + sColumn.CanReorder.ToString().ToLower() + _ColumnDelimeter2 + sColumn.GroupName + _ColumnDelimeter2 + sColumn.Viewable;
                    }
                    else
                    {
                        strOrder += _ColumnDelimeter1 + sColumn.ColumnName + _ColumnDelimeter2 + ReplaceHTMLTags(sColumn.DisplayName, " ") + _ColumnDelimeter2 + sColumn.Visible.ToString().ToLower() + _ColumnDelimeter2 + sColumn.CanReorder.ToString().ToLower() + _ColumnDelimeter2 + sColumn.GroupName + _ColumnDelimeter2 + sColumn.Viewable;
                    }
                }
            }

        }
        catch (Exception ex)
        {
        }
        return strOrder;
    }

    public void SortDataTable(ref DataTable dt, string sortColumns)
    {
        string sortString = "";
        string appendedColumnsString = "";
        string[] appendedColumns = null;
        if (!string.IsNullOrEmpty(sortColumns))
            try
            {


        {
            string[] splitSort = sortColumns.Split(_ColumnDelimeter1);
            for (int i =0; i < splitSort.Length - 1; i++)  // 12848 - 16 While testing changes found bug here where for loop went to far.  Added " - 1" to endpoint.
            {
                string sortField = splitSort[i].Split(_ColumnDelimeter2)[0];
                if (!string.IsNullOrEmpty(sortField) & sortField != "false")
                {
                    string sortDirection = (Convert.ToInt32(splitSort[i].Split(_ColumnDelimeter2)[1]) == 1 ? "ASC" : "DESC");
					if (_Columns.ItemByDisplayName(sortField) == null)
					{
						continue;
					}
                    string columnName = _Columns.ItemBySortName(sortField).ColumnName;

                    if ((dt.Columns[columnName] == null) == false)
                    {
                        if (dt.Columns[columnName].DataType == System.Type.GetType("System.Decimal") | dt.Columns[columnName].DataType == System.Type.GetType("System.Double") | dt.Columns[columnName].DataType == System.Type.GetType("System.Int16") | dt.Columns[columnName].DataType == System.Type.GetType("System.Int32") | dt.Columns[columnName].DataType == System.Type.GetType("System.Int64"))
                        {
                            string nSortName = columnName;
                            appendedColumnsString = appendedColumnsString + "~" + columnName + "_Sorter";
                            nSortName = nSortName + "_Sorter";

                            if ((dt.Columns[nSortName] == null))
                            {
                                dt.Columns.Add(nSortName);
                            }
                            foreach (DataRow eRow in dt.Rows)
                            {
                                eRow[nSortName] = ((Convert.ToBoolean(Information.IsDBNull(eRow[columnName])) == true ? 0 : Convert.ToInt32(eRow[columnName])) == 0 ? 1 : 0);
                            }

                            dt.AcceptChanges();

                            if (string.IsNullOrEmpty(sortString))
                            {
                                sortString = nSortName + " ASC";
                            }
                            else
                            {
                                sortString = sortString + ", " + nSortName + " ASC";
                            }
                        }

                        if (string.IsNullOrEmpty(sortString))
                        {
                            sortString = columnName + " " + sortDirection;
                        }
                        else
                        {
                            sortString = sortString + ", " + columnName + " " + sortDirection;
                        }
                    }
                }
            }
            dt.DefaultView.Sort = sortString;
            dt = dt.DefaultView.ToTable();
            appendedColumns = appendedColumnsString.Split('~');
            for(int y = 0; y < appendedColumns.Length; y++)
            {
                if (!string.IsNullOrEmpty(appendedColumns[y]))
                {
                    dt.Columns.Remove(appendedColumns[y]);
                }
            }
            dt.AcceptChanges();
        }

                // Catch
            }
            catch (Exception ex)
            {
                string err = ex.Message;  // Error 10-18-16 -> Rare:AffiliatedSort_Sorter does not belong to table.
                //throw;
            }
    }

    public void ReorderDataTable(ref DataTable dt, string orderColumns)
    {
        if (!string.IsNullOrEmpty(orderColumns))
        {
            try
            {
                string[] splitColumns = orderColumns.Split(_ColumnDelimeter1);
                dynamic columnLength = splitColumns.Length;
				for(int i = 0; i<columnLength; i++)
                {
                    string columnName = splitColumns[i].Split(_ColumnDelimeter2)[0];
                    string visible = splitColumns[i].Split(_ColumnDelimeter2)[2];
                    GridColsColumn sColumn = null;
					if (IsColumnInOrderList(splitColumns, ReplaceHTMLTags(_Columns.Item(i).ColumnName, " ")) == false)
                    {
                        sColumn = _Columns.Item(i);
                        columnLength += 1;
                    }
                    else
                    {
                        sColumn = _Columns.ItemByColumnName(columnName);
                    }

                    if ((sColumn == null) == false)
                    {
                        sColumn.ColumnIndex = i;
                        sColumn.Visible = (visible.ToUpper() == "TRUE" ? true : false);
                        dt.Columns[sColumn.ColumnName].SetOrdinal(sColumn.ColumnIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Empty;
            }

            dt.AcceptChanges();
            UpdateColumnGroups();
        }
    }

    protected bool IsColumnInOrderList(string[] orderColumns, string strColumn)
    {
        try
        {
            bool test = false;
			for (int i = 0; i < orderColumns.Length; i++)
			{
				if (orderColumns[i].Split(_ColumnDelimeter2)[0].ToUpper() == strColumn.ToUpper())
				{
					test = true;
					break; // TODO: might not be correct. Was : Exit Do
				}
			}
            return test;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    protected bool GroupExists(string strGroupList, string strGroup)
    {
        try
        {
            string[] splitGroup = strGroupList.Split('~');
            for(int i = 0; i < splitGroup.Length; i++)
            {
                if (splitGroup[i] == strGroup)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected void UpdateColumnGroups()
    {
        try
        {
            string strGroupNames = "";
            string[] groupNames = null;
			for(int i = 0; i < _Columns.Count; i++)
            {
                if (!string.IsNullOrEmpty(_Columns.Item(i).GroupName) & _Columns.Item(i).Visible == true)
                {
                    if (GroupExists(strGroupNames, _Columns.Item(i).GroupName) == false)
                    {
                        if (string.IsNullOrEmpty(strGroupNames))
                        {
                            strGroupNames = _Columns.Item(i).GroupName;
                        }
                        else
                        {
                            strGroupNames = strGroupNames + "~" + _Columns.Item(i).GroupName;
                        }
                    }
                }
            }

            groupNames = strGroupNames.Split('~');
			_Groups.Clear();

			for(int y = 0; y < groupNames.Length; y++)
            {
                int intColspan = 0;
                int intColcount = 0;
                if (!string.IsNullOrEmpty(groupNames[y]))
                {
                    _Groups.Add(groupNames[y]);
					for(int z = 0; z < _Columns.Count; z++)
                    {
                        if (_Columns.ItemByColumnOrder(z).GroupName == groupNames[y])
                        {
                            if (_Columns.ItemByColumnOrder(z).Visible == true & _Columns.ItemByColumnOrder(z).Viewable == true)
                            {
                                if (intColspan == 0)
                                {
                                    _Groups.Item(y).FirstColumnName = _Columns.ItemByColumnOrder(z).ColumnName;

                                }
                                _Groups.Item(y).LastColumnName = _Columns.ItemByColumnOrder(z).ColumnName;
                                intColspan += 1;
                            }
                            intColcount += 1;
                        }
                    }
                    _Groups.Item(y).ColumnSpan = intColspan;
                    _Groups.Item(y).ColumnCount = intColcount;
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void SetupGridHeader(GridViewRow row)
    {
        try
        {
            RemoveGridColumns(row);

			for (int i = 0; i < row.Cells.Count; i++)
			{
				if (_Columns.ItemByColumnOrder(i) != null)
				{
					row.Cells[i].Text = _Columns.ItemByColumnOrder(i).DisplayName;
				}
			}

            if (_Groups != null && _Groups.Count > 0)
            {
				GridColsColumn tColumn = null;
				for (int y = 0; y < row.Cells.Count; y++)
				{
					tColumn = _Columns.ItemByColumnOrder(y);
                    if (tColumn == null || string.IsNullOrEmpty(tColumn.GroupName))
                    {
                        if (row.RowIndex == 0)
                        {
                            row.Cells[y].RowSpan = 2;
                        }
                        else
                        {
                            row.Cells[y].Style["display"] = "none";
                        }
                    }
				}

                if (row.RowIndex == 0)
                {
					int startColumn = 0, lastColumn = 0;
					for (int z = 0; z < _Groups.Count; z++)
					{
						startColumn = _Columns.Item(_Groups.Item(z).FirstColumnName).ColumnIndex;
						lastColumn = _Columns.Item(_Groups.Item(z).LastColumnName).ColumnIndex + 1;
						row.Cells[startColumn].ColumnSpan = _Groups.Item(z).ColumnSpan;
						row.Cells[startColumn].Text = _Groups.Item(z).GroupName;

						startColumn += 1;

						for(int i = startColumn; i < lastColumn; i++)
						{
							row.Cells[i].Style["display"] = "none";
						}
					}
                }
            }
        }
        catch (Exception ex)
        {
            string msg = string.Empty;
        }
    }

    public void SetupGridBody(GridViewRow row)
    {
        RemoveGridColumns(row);
    }

    protected void RemoveGridColumns(GridViewRow row)
    {
		try
        {
			for (int i = 0; i < row.Cells.Count; i++)
			{
				if (_Columns.ItemByColumnOrder(i) == null || _Columns.ItemByColumnOrder(i).Visible == false)
                {
                    row.Cells[i].Style["display"] = "none";
                }
			}
        }
        catch (Exception ex)
        {
        }
    }

    private string ReplaceHTMLTags(string value, string replacement)
    {
        return Regex.Replace(value, "<.*>", replacement);
    }

}

public class GridColsCollection : System.Collections.CollectionBase
{
    public void Add(GridColsColumn Column)
    {
        Column.ColumnIndex = List.Count;
        List.Add(Column);
    }

    public GridColsColumn Add()
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnIndex = List.Count;
        List.Add(Column);
        return Column;
    }

    public GridColsColumn Add(string ColumnName, string DisplayName, string SortName, bool Visible, bool Sortable)
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnName = ColumnName;
        Column.DisplayName = DisplayName;
        Column.SortName = SortName;
        Column.Visible = Visible;
        Column.ColumnIndex = List.Count;
        Column.CanSort = Sortable;

        List.Add(Column);
        return Column;
    }

    public GridColsColumn Add(string ColumnName, string DisplayName, bool Visible, bool Sortable)
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnName = ColumnName;
        Column.DisplayName = DisplayName;
        Column.SortName = DisplayName;
        Column.Visible = Visible;
        Column.ColumnIndex = List.Count;
        Column.CanSort = Sortable;

        List.Add(Column);
        return Column;
    }

    public GridColsColumn Add(string ColumnName, string DisplayName, bool Visible)
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnName = ColumnName;
        Column.DisplayName = DisplayName;
        Column.SortName = DisplayName;
        Column.Visible = Visible;
        Column.ColumnIndex = List.Count;

        List.Add(Column);
        return Column;
    }

    public GridColsColumn Add(string ColumnName)
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnName = ColumnName;
        Column.DisplayName = ColumnName;
        Column.SortName = ColumnName;
        Column.ColumnIndex = List.Count;

        List.Add(Column);
        return Column;
    }

    public GridColsColumn Add(string ColumnName, string DisplayName)
    {
        GridColsColumn Column = new GridColsColumn();
        Column.ColumnName = ColumnName;
        Column.DisplayName = DisplayName;
        Column.ColumnIndex = List.Count;

        List.Add(Column);
        return Column;
    }

    public void Remove(int index)
    {
        if (index > Count - 1 | index < 0)
        {
            //'do nothing
        }
        else
        {
            List.RemoveAt(index);
        }
    }

    public GridColsColumn Item(object index)
    {
        return (GridColsColumn)List[IndexOf(index)];
    }

    public GridColsColumn ItemByColumnOrder(int index)
    {
		for (int i = 0; i < List.Count; i++)
		{
			if (((GridColsColumn)List[i]).ColumnIndex == index)
			{
				return (GridColsColumn)List[i];
			}
		}

        return null;
    }

    public GridColsColumn ItemByDisplayName(string DisplayName)
    {
		for (int i = 0; i < List.Count; i++)
		{
			if (RelaceHTMLTags(((GridColsColumn)List[i]).DisplayName.ToUpper(), " ") == DisplayName.ToUpper())
			{
				return (GridColsColumn)List[i];
			}
		}

        return null;
    }

    public GridColsColumn ItemByColumnName(string ColumnName)
    {
		for (int i = 0; i < List.Count; i++)
		{
			if (((GridColsColumn)List[i]).ColumnName.ToUpper() == ColumnName.ToUpper())
            {
                return (GridColsColumn)List[i];
            }
		}

        return null;
    }

    public GridColsColumn ItemBySortName(string SortName)
    {
		for (int i = 0; i < List.Count; i++)
		{
			if (RelaceHTMLTags(((GridColsColumn)List[i]).SortName.ToUpper(), " ") == SortName.ToUpper())
			{
				return (GridColsColumn)List[i];
			}
		}

        return null;
    }

    public int IndexOf(object obj)
    {
        if (obj.GetType().Name.ToString().Contains("Int"))
        {
            return (int)obj;
        }

        if (obj.GetType().Name.ToString() == "String")
        {
			for (int i = 0; i < List.Count; i++)
			{
				if (((GridColsColumn)List[i]).ColumnName.ToUpper() == ((string)obj).ToUpper())
				{
					return i;
				}
			}
        }
        else
        {
            throw new ArgumentException("Only a string or an integer is permitted for the indexer.");
        }

        return -1;
    }

    private string RelaceHTMLTags(string value, string replacement)
    {
        return Regex.Replace(value, "<.*>", replacement);
    }

}

public class GridColsGroupsCollection : System.Collections.CollectionBase
{
    public void Add(GridColsGroup Grid)
    {
        List.Add(Grid);
    }

    public void Add(string GroupName)
    {
        GridColsGroup nGroup = new GridColsGroup();
        nGroup.GroupName = GroupName;
        List.Add(nGroup);
    }

    public void Add()
    {
        GridColsGroup nGroup = new GridColsGroup();
        List.Add(nGroup);
    }


    public void Remove(int index)
    {
        if (index > Count - 1 | index < 0)
        {
            //'do nothing
        }
        else
        {
            List.RemoveAt(index);
        }
    }

    public GridColsGroup Item(object index)
    {
        return (GridColsGroup)List[IndexOf(index)];
    }

    public int IndexOf(object obj)
    {
        if (obj.GetType().Name.ToString().Contains("Int"))
        {
            return (int)obj;
        }
        if (obj.GetType().Name.ToString() == "String")
        {
			for (int i = 0; i < List.Count; i++)
			{
				if (((GridColsGroup)List[i]).GroupName.ToUpper() == ((string)obj).ToUpper())
				{
					return i;
				}
			}
        }
        else
        {
            throw new ArgumentException("Only a string or an integer is permitted for the indexer.");
        }

        return -1;
    }

    private string RelaceHTMLTags(string value, string replacement)
    {
        return Regex.Replace(value, "<.*>", replacement);
    }

}

public class GridColsColumn
{
    private string _ColumnName;
    public string ColumnName
    {
        get { return _ColumnName; }
        set { _ColumnName = value; }
    }

    private string _DisplayName;
    public string DisplayName
    {
        get { return _DisplayName; }
        set { _DisplayName = value; }
    }

    private int _ColumnIndex;
    public int ColumnIndex
    {
        get { return _ColumnIndex; }
        set { _ColumnIndex = value; }
    }

    private bool _Visible = true;
    public bool Visible
    {
        get { return _Visible; }
        set { _Visible = value; }
    }

    private string _MultiHeaderTitle;
    public string MultiHeaderTitle
    {
        get { return _MultiHeaderTitle; }
        set { _MultiHeaderTitle = value; }
    }

    private int _ColumnSpan = 0;
    public int ColumnSpan
    {
        get { return _ColumnSpan; }
        set { _ColumnSpan = value; }
    }

    private int _RowSpan = 0;
    public int RowSpan
    {
        get { return _RowSpan; }
        set { _RowSpan = value; }
    }

    private bool _CanReorder = true;
    public bool CanReorder
    {
        get { return _CanReorder; }
        set { _CanReorder = value; }
    }

    private bool _CanSort = true;
    public bool CanSort
    {
        get { return _CanSort; }
        set { _CanSort = value; }
    }

    private string _GroupName = "";
    public string GroupName
    {
        get { return _GroupName; }
        set { _GroupName = value; }
    }

    private bool _Viewable = true;
    public bool Viewable
    {
        get { return _Viewable; }
        set { _Viewable = value; }
    }

    private string _SortName = "";
    public string SortName
    {
        get { return _SortName; }
        set { _SortName = value; }
    }

    private bool _IsCurrency = false;
    public bool IsCurrency
    {
        get { return _IsCurrency; }
        set { _IsCurrency = value; }
    }

}

public class GridColsGroup
{
    private string _GroupName = "";
    public string GroupName
    {
        get { return _GroupName; }
        set { _GroupName = value; }
    }

    private int _ColumnSpan = 0;
    public int ColumnSpan
    {
        get { return _ColumnSpan; }
        set { _ColumnSpan = value; }
    }

    private int _ColumnCount = 0;
    public int ColumnCount
    {
        get { return _ColumnCount; }
        set { _ColumnCount = value; }
    }

    private string _FirstColumnName = "";
    public string FirstColumnName
    {
        get { return _FirstColumnName; }
        set { _FirstColumnName = value; }
    }

    private string _LastColumnName = "";
    public string LastColumnName
    {
        get { return _LastColumnName; }
        set { _LastColumnName = value; }
    }

}