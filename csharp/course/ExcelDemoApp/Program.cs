﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Net.WebSockets;

namespace ExcelDemoApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string path = System.IO.Directory.GetCurrentDirectory();
            Console.WriteLine(path +  "\\excelDemo.xlsx");
            var file = new FileInfo(path + "\\excelDemo.xlsx");

            var people = GetSetupData();
            await SaveExcelFile(file, people);

            List<PersonModel> peopleFromExcel = await LoadExcelFile(file);

            foreach (var p in peopleFromExcel)
            {
                Console.WriteLine($"{p.Id} {p.FirstName} {p.LastName}");
            }
        }

        private static async Task<List<PersonModel>> LoadExcelFile(FileInfo file)
        {
            List<PersonModel> output = new List<PersonModel>();

            using (var package = new ExcelPackage(file))
            {
                await package.LoadAsync(file);

                var ws = package.Workbook.Worksheets[0];

                int row = 3;
                int col = 1;

                while (string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()) == false)
                {
                    PersonModel p = new PersonModel();
                    p.Id = int.Parse(ws.Cells[row, col].Value.ToString());
                    p.FirstName = ws.Cells[row, col + 1].Value.ToString();
                    p.LastName  = ws.Cells[row, col + 2].Value.ToString();
                    output.Add(p);
                    row += 1;
                }
                return output;
            }

        }

        private static async Task SaveExcelFile(FileInfo file, List<PersonModel> people)
        {
            DeleteIfExists(file);

            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add("MainReport");
                var range = ws.Cells["A2"].LoadFromCollection(people, true);
                range.AutoFitColumns();

                //Formats the Headers
                ws.Cells["A1"].Value = "Our Cool report";
                ws.Cells["A1:C1"].Merge = true;
                ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Row(1).Style.Font.Size = 24;
                ws.Row(1).Style.Font.Color.SetColor(OfficeOpenXml.Drawing.eThemeSchemeColor.Accent4);

                ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Row(2).Style.Font.Bold = true;
                ws.Column(3).Width = 20;

                await package.SaveAsAsync(file);
            }

            


        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private static List<PersonModel> GetSetupData()
        {
            List<PersonModel> output = new List<PersonModel>()
        {
            new PersonModel() {Id = 1 , FirstName ="Tim", LastName = "Corey"},
            new PersonModel() {Id = 2 , FirstName ="Lukas", LastName = "Podolsky"},
            new PersonModel() {Id = 3 , FirstName ="Jan", LastName = "Shmidt"}
        };

            return output;
        }
    }

    
}