using Capital.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataRetrieve.Controllers
{
    public abstract class IController: Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private IEmployee _emp;
        public IController(IEmployee employee, IHostingEnvironment hostingEnvironment)
        {
            _emp = employee;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<FileStreamResult> NPOITemplate(IWorkbook workbook, string sFileName)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var memory = new MemoryStream();
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            file.Delete();
            memory.Position = 0;
            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }

        public virtual IWorkbook ExcelFormat(dynamic data) {
            return new XSSFWorkbook();
        }

        public void ExcelHeadData(dynamic data, ISheet excelSheet, IRow row, int RowNum, XSSFCellStyle cs, XSSFCellStyle cs1)
        {
            row = excelSheet.CreateRow(RowNum);
            var BothAdrDetailCnt = ((List<string>)data.HEADER).Count();
            for (int i = 0; i < BothAdrDetailCnt; i++)
            {
                row.CreateCell(i).SetCellValue(data.HEADER[i]);
                row.GetCell(i).CellStyle = cs;
            }
        }

        public void ExcelRowData(dynamic data, ISheet excelSheet, IRow row, int RowNum, XSSFCellStyle cs, XSSFCellStyle cs1)
        {
            foreach (var Item in data.GRID)
            {
                row = excelSheet.CreateRow(RowNum);
                var datarow = (Item as IDictionary<string, object>).ToList();

                for (int i = 0; i < datarow.Count(); i++)
                {
                    var value = datarow[i].Value;
                    if (value != null)
                    {
                        if (new Type[] { typeof(int), typeof(float), typeof(double), typeof(decimal) }.Contains(value.GetType()))
                        {
                            row.CreateCell(i).SetCellValue((Decimal.ToInt64(Decimal.Parse(value.ToString())) == 0 ? 0 : Double.Parse(value.ToString())));
                            row.GetCell(i).CellStyle = cs1;
                        }
                        else
                        {
                            row.CreateCell(i).SetCellValue(value.ToString());
                            row.GetCell(i).CellStyle = cs;
                        }
                    }
                    else
                    {
                        row.CreateCell(i).SetCellValue(string.Empty);
                        row.GetCell(i).CellStyle = cs;
                    }
                }
                RowNum++;
            }
        }

        public void SHEETDATA(string SheetName,dynamic data, IWorkbook workbook, XSSFCellStyle cs, XSSFCellStyle cs1)
        {
            int RowNum = 1;
            ISheet excelSheet = workbook.CreateSheet(SheetName);
            IRow row;
            if (data.GRID.TotalItemCount > 0)
            {
                row = excelSheet.CreateRow(0);
                var HEADERCnt = ((List<string>)data.HEADER).Count();
                for (int i = 0; i < HEADERCnt; i++)
                {
                    row.CreateCell(i).SetCellValue(data.HEADER[i]);
                    row.GetCell(i).CellStyle = cs;
                }

                foreach (var Item in data.GRID)
                {
                    row = excelSheet.CreateRow(RowNum);
                    var datarow = (Item as IDictionary<string, object>).ToList();

                    for (int i = 0; i < datarow.Count(); i++)
                    {
                        var value = datarow[i].Value;
                        if (value != null)
                        {
                            if (new Type[] { typeof(int), typeof(float), typeof(double), typeof(decimal) }.Contains(value.GetType()))
                            {
                                row.CreateCell(i).SetCellValue((Decimal.ToInt64(Decimal.Parse(value.ToString())) == 0 ? 0 : Double.Parse(value.ToString())));
                                row.GetCell(i).CellStyle = cs1;
                            }
                            else
                            {
                                row.CreateCell(i).SetCellValue(value.ToString());
                                row.GetCell(i).CellStyle = cs;
                            }
                        }
                        else
                        {
                            row.CreateCell(i).SetCellValue(string.Empty);
                            row.GetCell(i).CellStyle = cs;
                        }
                    }
                    RowNum++;
                }
            }
            else
            {
                row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("查無資料");
            }
        }
    }
}
