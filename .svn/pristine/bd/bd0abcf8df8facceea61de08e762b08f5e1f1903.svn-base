
Imports System.Web.Services

Partial Class Grid_Order
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ''PopulateSortContainer()

    End Sub

    Protected Sub PopulateSortContainer()
        Try
            Dim strFields As String = Request.QueryString("columnOrder")
            Dim strSplitFields As String() = strFields.Split("~")

            Dim i As Integer = 0
            Do Until i = strSplitFields.Length
                Dim field As String = strSplitFields(i).Split("|")(0)
                Dim visible As String = strSplitFields(i).Split("|")(1)
                Dim viewable As String = strSplitFields(i).Split("|")(2)

                Dim row As New HtmlTableRow
                Dim cell As New HtmlTableCell
                Dim checkbox As New CheckBox
                checkbox.Text = field
                checkbox.Style("cursor") = "default"
                If visible.ToUpper = "TRUE" Then
                    checkbox.Checked = True
                End If

                cell.Controls.Add(checkbox)
                row.Cells.Add(cell)

                If viewable.ToUpper <> "TRUE" Then
                    row.Style("display") = "none"
                End If

                tblColumnOrder.Rows.Add(row)

                i += 1
            Loop

        Catch ex As Exception

        End Try
    End Sub

    <WebMethod()> Public Shared Function UpdateSubColumns(SubColumnsSelected As String) As String

        HttpContext.Current.Session("SubColumnsSelected") = SubColumnsSelected

        Return "Set"
    End Function

End Class
