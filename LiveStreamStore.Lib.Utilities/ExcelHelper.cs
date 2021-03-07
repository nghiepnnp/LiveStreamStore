using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiveStreamStore.Lib.Utilities
{
    [AttributeUsage(AttributeTargets.All)]
    public class Column : System.Attribute
    {
        public int ColumnIndex { get; set; }


        public Column(int column)
        {
            ColumnIndex = column;
        }
    }
    public static class ExcelHelper
    {
        public static IEnumerable<T> ParseExcel<T>(this FileInfo fileInfo) where T : new()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
                {
                    ExcelPackage excel = new ExcelPackage(fileStream);
                    var workSheet = excel.Workbook.Worksheets.FirstOrDefault();
                    return workSheet.ConvertSheetToObjects<T>().ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static IEnumerable<T> ParseExcel<T>(this string FilePath) where T : new()
        {
            try
            {
                return ParseExcel<T>(new FileInfo(FilePath));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void SaveExcel<T>(this List<T> Objects, string[] Header, string FileName) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage())
            {

                excel.Workbook.Worksheets.Add("sheet1");

                var headerRow = new List<string[]>() { Header };

                // Determine the header range (e.g. A1:D1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";

                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets["sheet1"];

                // Popular header row data
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);

                worksheet.Cells[2, 1].LoadFromCollection(Objects);
                var excelFile = new FileInfo(FileName);
                excel.SaveAs(excelFile);
            }
        }

        public static IEnumerable<T> ConvertSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {

            Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(Column);

            var columns = typeof(T)
                    .GetProperties()
                    .Where(x => x.CustomAttributes.Any(columnOnly))
            .Select(p => new
            {
                Property = p,
                Column = p.GetCustomAttributes<Column>().First().ColumnIndex //safe because if where above
            }).ToList();


            var rows = worksheet.Cells
                .Select(cell => cell.Start.Row)
                .Distinct()
                .OrderBy(x => x);


            //Create the collection container
            var collection = rows.Skip(1)
                .Select(row =>
                {
                    dynamic colm = null;
                    try
                    {
                        var tnew = new T();
                        columns.ForEach(col =>
                        {
                            colm = col;
                            //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                            var val = worksheet.Cells[row, col.Column];
                            //If it is numeric it is a double since that is how excel stores all numbers
                            if (val.Value == null)
                            {
                                col.Property.SetValue(tnew, null);
                                return;
                            }
                            if (col.Property.PropertyType == typeof(Int32))
                            {
                                col.Property.SetValue(tnew, val.GetValue<int>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(double))
                            {
                                col.Property.SetValue(tnew, val.GetValue<double>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(DateTime))
                            {
                                col.Property.SetValue(tnew, val.GetValue<DateTime>());
                                return;
                            }
                            if (col.Property.PropertyType == typeof(decimal))
                            {
                                col.Property.SetValue(tnew, val.GetValue<decimal>());
                                return;
                            }
                            //Its a string
                            col.Property.SetValue(tnew, val.GetValue<string>());
                        });

                        return tnew;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Row:" + row + " column: " + colm?.Column + " Message: " + ex.Message);
                    }
                });


            //Send it back
            return collection;
        }

    }
}
