using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using YYCount.Models;

namespace YYCount.Services
{
    public class ExcelService
    {
        public void ExportCalculationToExcel(Calculation calculation, string filePath)
        {
            ExportCalculationToExcel(calculation, filePath, null);
        }

        public void ExportCalculationToExcel(Calculation calculation, string filePath, string templatePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Local");

            try
            {
                if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                {
                    using var package = new ExcelPackage(new FileInfo(templatePath));
                    FillCalculationData(package, calculation);
                    package.SaveAs(new FileInfo(filePath));
                }
                else
                {
                    using var package = new ExcelPackage();
                    CreateSimpleReport(package, calculation);
                    package.SaveAs(new FileInfo(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка экспорта в Excel: {ex.Message}", ex);
            }
        }

        private void FillCalculationData(ExcelPackage package, Calculation calculation)
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name.Contains("КАРТОЧКА")) 
                            ?? package.Workbook.Worksheets[0];

            // Получаем настройки из AppSettings
            int startRow = AppSettings.ExcelStartRow;
            int unitColumn = AppSettings.ExcelUnitColumn;

            // Очищаем старые данные (начиная с startRow и на 6 строк вниз)
            for (int i = startRow; i <= startRow + 5; i++)
            {
                worksheet.Cells[i, 1].Value = null; // A
                worksheet.Cells[i, 2].Value = null; // B
                worksheet.Cells[i, unitColumn].Value = null; // колонка условных установок
            }

            int row = startRow;
            int index = 1;
            foreach (var pos in calculation.Positions)
            {
                worksheet.Cells[row, 1].Value = index++;
                worksheet.Cells[row, 2].Value = pos.EquipmentName;
                double totalForPosition = pos.Quantity * pos.UnitValue;
                worksheet.Cells[row, unitColumn].Value = totalForPosition;
                row++;
            }

            // Общая сумма – ищем строку с "Всего:" или ставим через 6 строк после startRow
            // По умолчанию это строка startRow + 7 (было 40 при startRow=33)
            int totalRow = startRow + 7;
            worksheet.Cells[totalRow, unitColumn].Value = calculation.TotalSum;
        }

        private void CreateSimpleReport(ExcelPackage package, Calculation calculation)
        {
            var worksheet = package.Workbook.Worksheets.Add("Расчёт");

            // Заголовки
            worksheet.Cells[1, 1].Value = "Название расчёта:";
            worksheet.Cells[1, 2].Value = calculation.Name;
            worksheet.Cells[2, 1].Value = "Дата:";
            worksheet.Cells[2, 2].Value = calculation.Date.ToString("dd.MM.yyyy HH:mm");
            worksheet.Cells[3, 1].Value = "Итого:";
            worksheet.Cells[3, 2].Value = calculation.TotalSum;
            worksheet.Cells[3, 2].Style.Numberformat.Format = "#,##0.00";

            // Шапка таблицы
            int row = 5;
            worksheet.Cells[row, 1].Value = "№";
            worksheet.Cells[row, 2].Value = "Наименование";
            worksheet.Cells[row, 3].Value = "Количество";
            worksheet.Cells[row, 4].Value = "Усл. за ед.";
            worksheet.Cells[row, 5].Value = "Итого";
            using (var range = worksheet.Cells[row, 1, row, 5])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int idx = 1;
            foreach (var pos in calculation.Positions)
            {
                row++;
                worksheet.Cells[row, 1].Value = idx++;
                worksheet.Cells[row, 2].Value = pos.EquipmentName;
                worksheet.Cells[row, 3].Value = pos.Quantity;
                worksheet.Cells[row, 4].Value = pos.UnitValue;
                worksheet.Cells[row, 5].Value = pos.Total;
                worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
            }

            row++;
            worksheet.Cells[row, 4].Value = "Итого:";
            worksheet.Cells[row, 5].Value = calculation.TotalSum;
            worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[row, 4, row, 5].Style.Font.Bold = true;

            worksheet.Cells[1, 1, row, 5].AutoFitColumns();
        }
    }
}