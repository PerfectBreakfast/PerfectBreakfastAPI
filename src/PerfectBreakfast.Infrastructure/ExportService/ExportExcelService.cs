using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Infrastructure.ExportService;

public class ExportExcelService : IExportExcelService
{
    public ExportExcelService()
    {
        
    }
    
    public byte[] DownloadSupplierFoodAssignmentForSupplier(SupplierFoodAssignmentForSupplier supplierFood)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("SupplierFoodAssignments");

            // Tạo tiêu đề động
            worksheet.Cells["A1:E1"].Merge = true;
            worksheet.Cells["A1"].Value = "Perfect Breakfast";
            worksheet.Cells["A1"].Style.Font.Size = 20;
            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Font.Size = 14;
            worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 20;
                
            worksheet.Cells["A2"].Value = "Ngày Giao";
            worksheet.Cells["B2"].Value = "Tên Đối Tác";
            worksheet.Cells["C2"].Value = "Thời Gian bàn giao";
            worksheet.Cells["D2"].Value = "Tên Món Ăn/đồ uống";
            worksheet.Cells["E2"].Value = "Số Lượng thực nhận";
            worksheet.Cells["F2"].Value = "Trạng Thái";
                
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            var dataStartRow = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu

            // Ghi giá trị Date và các dữ liệu cụ thể
            worksheet.Cells[dataStartRow, 1].Value = supplierFood.Date;

            foreach (var foodAssignmentGroup in supplierFood.FoodAssignmentGroupByPartners)
            {
                worksheet.Cells[dataStartRow, 2].Value = foodAssignmentGroup.PartnerName;

                foreach (var deliveryTimeResponse in foodAssignmentGroup.SupplierDeliveryTimes)
                {
                    worksheet.Cells[dataStartRow, 3].Value = deliveryTimeResponse.DeliveryTime;

                    foreach (var foodAssignmentResponse in deliveryTimeResponse.FoodAssignmentResponses)
                    {
                        worksheet.Cells[dataStartRow, 4].Value = foodAssignmentResponse.FoodName;
                        worksheet.Cells[dataStartRow, 5].Value = foodAssignmentResponse.AmountCooked;
                        worksheet.Cells[dataStartRow, 6].Value = foodAssignmentResponse.Status;
                        dataStartRow++;
                    }
                }
            }
                
            var dataRange = worksheet.Cells[2, 1, dataStartRow - 1, 5];
            var border = dataRange.Style.Border;
            border.Top.Style = border.Left.Style = border.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

            // Auto-fit columns
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Column(i).AutoFit();
            }

            return excelPackage.GetAsByteArray();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public byte[] DownloadSupplierFoodAssignmentForSuperAdmin(List<SupplierFoodAssignmentForSuperAdmin> supplierFoods)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Món ăn phân chia");

            // Tạo tiêu đề động
            worksheet.Cells["A1:E1"].Merge = true;
            worksheet.Cells["A1"].Value = "Perfect Breakfast";
            worksheet.Cells["A1"].Style.Font.Size = 20;
            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Font.Size = 14;
            worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 20;
                
            worksheet.Cells["A2"].Value = "Ngày đặt";
            worksheet.Cells["B2"].Value = "Ngày giao";
            worksheet.Cells["C2"].Value = "Tên đối tác";
            worksheet.Cells["D2"].Value = "Thời gian bàn giao";
            worksheet.Cells["E2"].Value = "Tên món ăn/đồ uống";
            worksheet.Cells["F2"].Value = "Số Lượng thực hiện";
            worksheet.Cells["G2"].Value = "Tỷ lệ hoa hồng";
            worksheet.Cells["H2"].Value = "Thành tiền";
            worksheet.Cells["I2"].Value = "Trạng thái";
                
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            var dataStartRow = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu

            foreach (var supplierFood in supplierFoods)
            {
                // Ghi giá trị Date và các dữ liệu cụ thể
                worksheet.Cells[dataStartRow, 1].Value = supplierFood.CreationDate;
                worksheet.Cells[dataStartRow, 2].Value = supplierFood.BookingDate;

                foreach (var foodAssignmentGroup in supplierFood.FoodAssignmentGroupByPartners)
                {
                    worksheet.Cells[dataStartRow, 3].Value = foodAssignmentGroup.PartnerName;

                    foreach (var deliveryTimeResponse in foodAssignmentGroup.SupplierDeliveryTimes)
                    {
                        worksheet.Cells[dataStartRow, 4].Value = deliveryTimeResponse.DeliveryTime;

                        foreach (var foodAssignmentResponse in deliveryTimeResponse.FoodAssignmentResponses)
                        {
                            worksheet.Cells[dataStartRow, 5].Value = foodAssignmentResponse.FoodName;
                            worksheet.Cells[dataStartRow, 6].Value = foodAssignmentResponse.AmountCooked;
                            worksheet.Cells[dataStartRow, 7].Value = foodAssignmentResponse.CommissionRate + "%";
                            worksheet.Cells[dataStartRow, 8].Value = foodAssignmentResponse.ReceivedAmount;
                            worksheet.Cells[dataStartRow, 9].Value = foodAssignmentResponse.Status;
                            dataStartRow++;
                        }
                    }
                }
            }
            

            // Auto-fit columns
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Column(i).AutoFit();
            }

            return excelPackage.GetAsByteArray();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public byte[] DownloadDailyOrderStatistic(List<DailyOrderStatisticResponse> responses)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Thống kê đơn hàng");

            // Tạo tiêu đề động
            worksheet.Cells["A1:E1"].Merge = true;
            worksheet.Cells["A1"].Value = "Perfect Breakfast";
            worksheet.Cells["A1"].Style.Font.Size = 20;
            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Font.Size = 14;
            worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 20;
                
            worksheet.Cells["A2"].Value = "Ngày đặt";
            worksheet.Cells["B2"].Value = "Ngày giao";
            worksheet.Cells["C2"].Value = "Khách hàng";
            worksheet.Cells["D2"].Value = "Tên đối tác";
            worksheet.Cells["E2"].Value = "Đơn vị vận chuyển";
            worksheet.Cells["F2"].Value = "Bữa Ăn";
            worksheet.Cells["H2"].Value = "Thời gian bàn giao";
            worksheet.Cells["H2"].Value = "Combo/món";
            worksheet.Cells["I2"].Value = "Món ăn";
            worksheet.Cells["J2"].Value = "Số lượng";
                
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            var dataStartRow = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu

            foreach (var response in responses)
            {
                // Ghi giá trị Date và các dữ liệu cụ thể
                worksheet.Cells[dataStartRow, 1].Value = response.CreationDate;
                worksheet.Cells[dataStartRow, 2].Value = response.BookingDate;

                foreach (var companyForDailyOrder in response.CompanyForDailyOrders)
                {
                    worksheet.Cells[dataStartRow, 3].Value = companyForDailyOrder.Name;
                    worksheet.Cells[dataStartRow, 4].Value = companyForDailyOrder.Partner;
                    worksheet.Cells[dataStartRow, 5].Value = companyForDailyOrder.Delivery;

                    foreach (var mealForDailyOrder in companyForDailyOrder.MealForDailyOrders)
                    {
                        worksheet.Cells[dataStartRow, 6].Value = mealForDailyOrder.Meal;
                        worksheet.Cells[dataStartRow, 7].Value = mealForDailyOrder.TimeHandover;

                        foreach (var foodAssignmentResponse in mealForDailyOrder.FoodForDailyOrders)
                        {
                            worksheet.Cells[dataStartRow, 8].Value = foodAssignmentResponse.Name;
                            worksheet.Cells[dataStartRow, 9].Value = foodAssignmentResponse.Food;
                            worksheet.Cells[dataStartRow, 10].Value = foodAssignmentResponse.Quantity;
                            dataStartRow++;
                        }
                    }
                }
            }
            

            // Auto-fit columns
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Column(i).AutoFit();
            }

            return excelPackage.GetAsByteArray();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public byte[] DownloadOrderStatisticExcel(OrderStatisticResponse response)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add($"Thống kê đơn hàng từ ngày {response.FromDate.ToString("dd/MM/yyyy")} đến ngày {response.ToDate.ToString("dd/MM/yyyy")}");
            
            // Tạo tiêu đề động
            worksheet.Cells["A1:E1"].Merge = true;
            worksheet.Cells["A1"].Value = "Perfect Breakfast";
            worksheet.Cells["A1"].Style.Font.Size = 20;
            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Font.Size = 14;
            worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 20;
            
            worksheet.Cells["A2"].Value = "Từ ngày";
            worksheet.Cells["B2"].Value = "Tới ngày";
            worksheet.Cells["C2"].Value = "Tổng số lượng đơn";
            worksheet.Cells["D2"].Value = "Số lượng đơn hoàn thành";
            worksheet.Cells["E2"].Value = "Combo phổ biến nhất";
            
            for (var i = 1; i <= 5; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }
            
            var dataStartRow = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu
            
            
            
            
            return excelPackage.GetAsByteArray();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}