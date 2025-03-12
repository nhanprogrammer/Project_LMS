using OfficeOpenXml;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;

namespace Project_LMS.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<string> ExportSchoolAndBranchesToExcelAsync(SchoolResponse school, int schoolId)
        {
            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SchoolAndBranches");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "School ID";
                worksheet.Cells[1, 2].Value = "School Name";
                worksheet.Cells[1, 3].Value = "School Address";
                worksheet.Cells[1, 4].Value = "Branch ID";
                worksheet.Cells[1, 5].Value = "Branch Name";
                worksheet.Cells[1, 6].Value = "Branch Address";

                // Định dạng tiêu đề
                worksheet.Row(1).Style.Font.Bold = true;

                // Điền dữ liệu
                int row = 2;
                if (school.Branches == null || !school.Branches.Any())
                {
                    // Trường hợp không có chi nhánh nào trong danh sách
                    worksheet.Cells[row, 1].Value = school.Id;
                    worksheet.Cells[row, 2].Value = school.Name;
                    worksheet.Cells[row, 3].Value = $"{school.Ward}, {school.District}, {school.Province}";
                    worksheet.Cells[row, 4].Value = "N/A";
                    worksheet.Cells[row, 5].Value = "Không có chi nhánh";
                    worksheet.Cells[row, 6].Value = "N/A";
                    row++;
                }
                else
                {
                    // Trường hợp có nhiều chi nhánh
                    foreach (var branch in school.Branches)
                    {
                        worksheet.Cells[row, 1].Value = school.Id;
                        worksheet.Cells[row, 2].Value = school.Name;
                        worksheet.Cells[row, 3].Value = $"{school.Ward}, {school.District}, {school.Province}";
                        worksheet.Cells[row, 4].Value = branch.Id;
                        worksheet.Cells[row, 5].Value = branch.BranchName;
                        worksheet.Cells[row, 6].Value = branch.Address;
                        row++;
                    }
                }

                worksheet.Cells.AutoFitColumns();


                var fileBytes = package.GetAsByteArray();
                var base64String = Convert.ToBase64String(fileBytes);
                return await Task.FromResult(base64String);
            }
        }
    }
}