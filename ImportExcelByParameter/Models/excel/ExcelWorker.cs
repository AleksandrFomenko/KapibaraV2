using System;
using System.Diagnostics;
using Autodesk.Revit.UI;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ImportExcelByParameter.Models.excel;

internal class ExcelWorker
{
    private int _columnParameterIndex = 0;
    private XLWorkbook _workbook;
    private IXLWorksheet _worksheet;
    
    internal string SheetName;
    internal string ParameterName;
    internal int RowNumber;
    
    internal static Action CloseExcel { get; set; }
    
    internal void OpenExcel(string path)
    {
        try
        {
            _workbook = new XLWorkbook(path);
            _worksheet = _workbook.Worksheet(SheetName);
            CloseExcel = () => _workbook?.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
    internal List<string> GetWorksheetNames(string path)
    {
        try
        {
            _workbook = new XLWorkbook(path);
            return _workbook?.Worksheets.Select(ws => ws.Name).ToList();
        }
        catch (Exception ex) when (ex.HResult == -2147024864)
        {
            TaskDialog.Show("Err", "ексель закрой");
            return new List<string>();
        }
        finally
        {
            _workbook?.Dispose();
        }
    }
    private List<int> GetParameterColumn()
    {
        int columnIndex = 1;
        int emptyCellCount = 0;
        const int maxEmptyCells = 3;
        int lastColumn = _worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        List<int> otherColumns = new List<int>();

        while (columnIndex <= lastColumn)
        {
            var cell = _worksheet.Cell(RowNumber, columnIndex);
            string cellValue = cell.GetString().Trim();

            if (string.Equals(cellValue, ParameterName, StringComparison.OrdinalIgnoreCase))
            {
                _columnParameterIndex = columnIndex;
                columnIndex++;
                continue; 
            }

            if (string.IsNullOrEmpty(cellValue))
            {
                emptyCellCount++;
                if (emptyCellCount >= maxEmptyCells)
                {
                    break;
                }
            }
            else
            {
                emptyCellCount = 0;
                otherColumns.Add(columnIndex);
            }

            columnIndex++;
        }

        return (otherColumns);
    }
    
    private int FindRow(string searchValue)
    {
        int rowIndex = RowNumber + 1; 
        int emptyCount = 0;
        const int stop = 10;
        int maxRows = 1048576;

        while (rowIndex <= maxRows)
        {
            var cell =_worksheet.Cell(rowIndex, _columnParameterIndex);

            if (cell.IsEmpty())
            {
                emptyCount++;
                if (emptyCount >= stop)
                {
                    break;
                }
            }
            else
            {
                emptyCount = 0; 
                var cellValue = cell.GetString().Trim();
                
                if (string.Equals(cellValue, searchValue, StringComparison.OrdinalIgnoreCase))
                {
                    return rowIndex;
                }
            }
            rowIndex++;
        }
        return 0;
    }

    internal Dictionary<string, string> Execute(string searchValue)
    {
        var otherColumns = GetParameterColumn();
        if (_columnParameterIndex == 0) 
            TaskDialog.Show("Error", $"Параметр {{ParameterName}} не найден в строчке {RowNumber}");
        var row = FindRow(searchValue);

        if(row > 0)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            var paramColumnName = _worksheet.Cell(RowNumber, _columnParameterIndex).GetString();
            var paramValue = _worksheet.Cell(row, _columnParameterIndex).GetString();
            result[paramColumnName] = paramValue;
            
            foreach(var col in otherColumns)
            {
                var colName = _worksheet.Cell(RowNumber, col).GetString();
                var cellValue = _worksheet.Cell(row, col).GetString();
                cellValue = cellValue.Replace("\n", Environment.NewLine);
                result[colName] = cellValue;
            }
            return result;
        }
        return new Dictionary<string, string>();
    }
}
