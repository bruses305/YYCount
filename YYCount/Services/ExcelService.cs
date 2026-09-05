using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
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
            var settings = AppSettings.GetTemplateSettings();
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(w => w.Name.Contains("КАРТОЧКА")) 
                            ?? package.Workbook.Worksheets[0];

            // 1. Замена одиночных тегов
            foreach (var cell in worksheet.Cells)
            {
                if (cell.Value is string text)
                {
                    if (text.Contains(settings.NameTag))
                    {
                        cell.Value = text.Replace(text, calculation.Name);
                    }
                    if (text.Contains(settings.SumTag))
                    {
                        cell.Value = text.Replace(text, calculation.TotalSum.ToString("F2"));
                    }
                }
            }

            // 2. Поиск строки-шаблона таблицы
            int tableStartRow = -1;
            Dictionary<int, string> columnTags = new Dictionary<int, string>();

            for (int row = 1; row <= worksheet.Dimension.Rows; row++)
            {
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    if (cell.Value is string text && text.Contains(settings.TableStartTag))
                    {
                        tableStartRow = row;
                        // Найдём все теги в этой строке
                        for (int c = 1; c <= worksheet.Dimension.Columns; c++)
                        {
                            var tagCell = worksheet.Cells[row, c];
                            if (tagCell.Value is string tagText)
                            {
                                if (tagText.Contains(settings.IdTag)) columnTags[c] = settings.IdTag;
                                else if (tagText.Contains(settings.EquipmentNameTag)) columnTags[c] = settings.EquipmentNameTag;
                                else if (tagText.Contains(settings.QuantityTag)) columnTags[c] = settings.QuantityTag;
                                else if (tagText.Contains(settings.UnitValueTag)) columnTags[c] = settings.UnitValueTag;
                                else if (tagText.Contains(settings.TotalTag)) columnTags[c] = settings.TotalTag;
                            }
                        }
                        break;
                    }
                }
                if (tableStartRow != -1) break;
            }

            if (tableStartRow == -1) return; // не найдено

            // 3. Вставляем строки для каждой позиции
            int currentRow = tableStartRow;
            foreach (var pos in calculation.Positions)
            {
                // Вставляем новую строку после текущей (копируем стили из строки-шаблона)
                worksheet.InsertRow(currentRow + 1, 1, currentRow);
                currentRow++; // переходим на новую строку

                // Заполняем ячейки в новой строке
                foreach (var col in columnTags.Keys)
                {
                    var cell = worksheet.Cells[currentRow, col];
                    string tag = columnTags[col];
                    string value = "";
                    if (tag == settings.IdTag) value = pos.Id.ToString();
                    else if (tag == settings.EquipmentNameTag) value = pos.EquipmentName;
                    else if (tag == settings.QuantityTag) value = pos.Quantity.ToString();
                    else if (tag == settings.UnitValueTag) value = pos.UnitValue.ToString();
                    else if (tag == settings.TotalTag) value = pos.Total.ToString("F2");
                    cell.Value = value;
                }
            }

            // 4. Удаляем строку-шаблон
            worksheet.DeleteRow(tableStartRow);
        }

        private void CreateSimpleReport(ExcelPackage package, Calculation calculation)
        {
            var worksheet = package.Workbook.Worksheets.Add("Расчёт");

            // ... (код создания простого отчёта, как ранее)
        }
    }
}