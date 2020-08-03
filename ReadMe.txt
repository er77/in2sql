

Some ideas 
https://www.mrexcel.com/board/threads/import-csv-into-listobject.663865/

Public Function GetCSVData(ByVal strFile As String) As Object
    Dim lngSplit As Long, strTable As String, strPath As String
    Dim objRS As Object
    Dim strConnection As String, strSQL As String
    
    lngSplit = InStrRev(strFile, "\")
    
    strTable = Mid$(strFile, lngSplit + 1)
    strPath = Left$(strFile, lngSplit - 1)
    
    strConnection = "Provider=Microsoft.Jet.OLEDB.4.0;" & _
                    "Data Source=" & strPath & ";" & _
                    "Extended Properties=Text;"
                    
    strSQL = "SELECT * FROM " & strTable & ";"


    Set objRS = CreateObject("ADODB.Recordset")
    Call objRS.Open(strSQL, strConnection, 1, 3, 1)
End Function

    Dim varCsvData as Variant

    varCsvData = Application.Transpose(GetCSVData("c:/......../file.csv").GetRows)
    Me.lbx.List = varCsvData

    ----------
    https://docs.microsoft.com/ru-ru/visualstudio/vsto/listobject-control?view=vs-2019

    ListObject Events 

    BeforeAddDataBoundRow BeforeDoubleClick BeforeRightClick BindingContextChanged Change DataBindingFailure DataMemberChanged  DataSourceChanged Deselected ErrorAddDataBoundRow OriginalDataRestored
        Selected SelectedIndexChanged SelectionChange