using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DailyOrder.StatisticResponse;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.StatisticResponse;

namespace PerfectBreakfast.Infrastructure.ExportService;

public class ExportExcelService : IExportExcelService
{
    public ExportExcelService()
    {
        
    }
    
    public byte[] DownloadSupplierFoodAssignmentForSupplier(List<FoodAssignmentDownloadForSupplier> supplierFoodList)
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
                
            worksheet.Cells["A2"].Value = "Mã đơn phân chia";
            worksheet.Cells["B2"].Value = "Tên Đối Tác";
            worksheet.Cells["C2"].Value = "Thời Gian bàn giao";
            worksheet.Cells["D2"].Value = "Tên Món Ăn/đồ uống";
            worksheet.Cells["E2"].Value = "Số Lượng thực nhận";
            worksheet.Cells["F2"].Value = "Trạng Thái";
            worksheet.Cells["G2"].Value = "Hoàn thành";
                
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            var dataStartRow = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu
            
            
            // Ghi giá trị Date và các dữ liệu cụ thể
            foreach (var supplierFood in supplierFoodList)
            {
                worksheet.Cells[dataStartRow, 1].Value = supplierFood.Id;
                worksheet.Cells[dataStartRow, 2].Value = supplierFood.PartnerName;
                foreach (var foodAssignmentGroup in supplierFood.FoodAssignmentResponses)
                {
                    worksheet.Cells[dataStartRow, 3].Value = foodAssignmentGroup.DeliveryDeadline.ToString();
                    worksheet.Cells[dataStartRow, 4].Value = foodAssignmentGroup.FoodName;
                    worksheet.Cells[dataStartRow, 5].Value = foodAssignmentGroup.AmountCooked;
                    worksheet.Cells[dataStartRow, 6].Value = foodAssignmentGroup.Status;
                    dataStartRow++; 
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

    public byte[] DownloadSupplierFoodAssignmentForPartner(List<FoodAssignmentDownloadForSupplier> supplierFoodList)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("SupplierFoodAssignments");

            // Tạo tiêu đề động
            worksheet.Cells["A1:E1"].Merge = true;
            worksheet.Cells["A1"].Value = $"Perfect Breakfast - {supplierFoodList.First().PartnerName}";
            worksheet.Cells["A1"].Style.Font.Size = 20;
            worksheet.Cells["A1:E1"].Style.Font.Bold = true;
            worksheet.Cells["A1:E1"].Style.Font.Size = 14;
            worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:E1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 20;
                
            worksheet.Cells["A2"].Value = "Mã đơn phân chia";
            worksheet.Cells["b2"].Value = "Tên doanh nghiệp";
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
            foreach (var supplierFood in supplierFoodList)
            {
                worksheet.Cells[dataStartRow, 1].Value = supplierFood.Id;
                worksheet.Cells[dataStartRow, 2].Value = supplierFood.CompanyName;
                var isFirstAssignment = true;
                foreach (var foodAssignmentGroup in supplierFood.FoodAssignmentResponses)
                {
                    if (isFirstAssignment)
                    {
                        worksheet.Cells[dataStartRow, 3].Value = foodAssignmentGroup.DeliveryDeadline.ToString();
                        isFirstAssignment = false;
                    }
                    worksheet.Cells[dataStartRow, 4].Value = foodAssignmentGroup.FoodName;
                    worksheet.Cells[dataStartRow, 5].Value = foodAssignmentGroup.AmountCooked;
                    worksheet.Cells[dataStartRow, 6].Value = foodAssignmentGroup.Status;
                    dataStartRow++; 
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
            worksheet.Cells["A1"].Value = "Perfect Breakfast - Thống kê đơn hàng";
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
            worksheet.Cells["G2"].Value = "Thời gian bàn giao";
            worksheet.Cells["H2"].Value = "Tổng số lượng đơn";
            worksheet.Cells["I2"].Value = "Số lượng đơn thành công";
            worksheet.Cells["J2"].Value = "Số lượng đơn thất bại";
            worksheet.Cells["K2"].Value = "Tổng tiền";
            worksheet.Cells["L2"].Value = "Combo/món";
            worksheet.Cells["M2"].Value = "Món ăn";
            worksheet.Cells["N2"].Value = "Số lượng";
                
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
                        worksheet.Cells[dataStartRow, 7].Value = mealForDailyOrder.Time;
                        worksheet.Cells[dataStartRow, 8].Value = mealForDailyOrder.TotalOrderQuantity;
                        worksheet.Cells[dataStartRow, 9].Value = mealForDailyOrder.CompleteQuantity;
                        worksheet.Cells[dataStartRow, 10].Value = mealForDailyOrder.CancelQuantity;
                        worksheet.Cells[dataStartRow, 11].Value = mealForDailyOrder.TotalPrice;
                        
                        worksheet.Cells[dataStartRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[dataStartRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[dataStartRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        
                        foreach (var foodAssignmentResponse in mealForDailyOrder.FoodForDailyOrders)
                        {
                            worksheet.Cells[dataStartRow, 12].Value = foodAssignmentResponse.Name;
                            worksheet.Cells[dataStartRow, 13].Value = foodAssignmentResponse.Food;
                            worksheet.Cells[dataStartRow, 14].Value = foodAssignmentResponse.Quantity;
                            worksheet.Cells[dataStartRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            dataStartRow++;
                        }
                    }
                }
            }
            
            // Auto-fit columns
            for (var i = 1; i <= 30; i++)
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

    public byte[] DownloadDailyOrderStatisticForSuperAdmin(List<DailyOrderStatisticResponse> responses, List<SupplierFoodAssignmentForSuperAdmin> supplierFoods)
    {
        try
        {
            using var excelPackage = new ExcelPackage();
            var worksheet = excelPackage.Workbook.Worksheets.Add("Thống kê đơn hàng");

            // Tạo tiêu đề động
            worksheet.Cells["A1:N1"].Merge = true;
            worksheet.Cells["A1"].Value = "Perfect Breakfast - Thống kê đơn hàng";
            worksheet.Cells["A1"].Style.Font.Size = 30;
            worksheet.Cells["A1:N1"].Style.Font.Bold = true;
            worksheet.Cells["A1:N1"].Style.Font.Size = 14;
            worksheet.Cells["A1:N1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["A1:N1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:N1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 30;
                
            worksheet.Cells["A2"].Value = "Ngày đặt";
            worksheet.Cells["B2"].Value = "Ngày giao";
            worksheet.Cells["C2"].Value = "Khách hàng";
            worksheet.Cells["D2"].Value = "Tên đối tác";
            worksheet.Cells["E2"].Value = "Đơn vị vận chuyển";
            worksheet.Cells["F2"].Value = "Bữa Ăn";
            worksheet.Cells["G2"].Value = "Thời gian bàn giao";
            worksheet.Cells["H2"].Value = "Tổng số lượng đơn";
            worksheet.Cells["I2"].Value = "Số lượng đơn thành công";
            worksheet.Cells["J2"].Value = "Số lượng đơn thất bại";
            worksheet.Cells["K2"].Value = "Tổng tiền";
            worksheet.Cells["L2"].Value = "Combo/món";
            worksheet.Cells["M2"].Value = "Món ăn";
            worksheet.Cells["N2"].Value = "Số lượng";
                
            for (var i = 1; i <= 30; i++)
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
                        worksheet.Cells[dataStartRow, 7].Value = mealForDailyOrder.Time;
                        worksheet.Cells[dataStartRow, 8].Value = mealForDailyOrder.TotalOrderQuantity;
                        worksheet.Cells[dataStartRow, 9].Value = mealForDailyOrder.CompleteQuantity;
                        worksheet.Cells[dataStartRow, 10].Value = mealForDailyOrder.CancelQuantity;
                        worksheet.Cells[dataStartRow, 11].Value = mealForDailyOrder.TotalPrice;
                        
                        worksheet.Cells[dataStartRow, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[dataStartRow, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[dataStartRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        
                        foreach (var foodAssignmentResponse in mealForDailyOrder.FoodForDailyOrders)
                        {
                            worksheet.Cells[dataStartRow, 12].Value = foodAssignmentResponse.Name;
                            worksheet.Cells[dataStartRow, 13].Value = foodAssignmentResponse.Food;
                            worksheet.Cells[dataStartRow, 14].Value = foodAssignmentResponse.Quantity;
                            worksheet.Cells[dataStartRow, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            dataStartRow++;
                        }
                    }
                }
            }
            
            // Tạo tiêu đề động
            worksheet.Cells["R1:Z1"].Merge = true;
            worksheet.Cells["R1"].Value = "Perfect Breakfast - Thống kê món ăn";
            worksheet.Cells["R1"].Style.Font.Size = 30;
            worksheet.Cells["R1:Z1"].Style.Font.Bold = true;
            worksheet.Cells["R1:Z1"].Style.Font.Size = 14;
            worksheet.Cells["R1:Z1"].Style.Font.Color.SetColor(Color.Green);
            worksheet.Cells["R1:Z1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["R1:Z1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(1).Height = 30;
                
            worksheet.Cells["R2"].Value = "Ngày đặt";
            worksheet.Cells["S2"].Value = "Ngày giao";
            worksheet.Cells["T2"].Value = "Tên nhà cung cấp";
            worksheet.Cells["U2"].Value = "Thời gian giao";
            worksheet.Cells["V2"].Value = "Tên món ăn/đồ uống";
            worksheet.Cells["W2"].Value = "Số Lượng thực hiện";
            worksheet.Cells["X2"].Value = "Tỷ lệ hoa hồng";
            worksheet.Cells["Y2"].Value = "Thành tiền";
            worksheet.Cells["Z2"].Value = "Trạng thái";
                
            for (var i = 1; i <= 20; i++)
            {
                worksheet.Cells[2, i].Style.Font.Color.SetColor(Color.FromArgb(5, 75, 252)); // Dark Cornflower Blue
                worksheet.Cells[2, i].Style.Font.Bold = true;
            }

            var dataStartRow2 = 3; // Bắt đầu từ dòng thứ 3 để dành cho dữ liệu

            foreach (var supplierFood in supplierFoods)
            {
                // Ghi giá trị Date và các dữ liệu cụ thể
                worksheet.Cells[dataStartRow2, 18].Value = supplierFood.CreationDate;
                worksheet.Cells[dataStartRow2, 19].Value = supplierFood.BookingDate;

                foreach (var foodAssignmentGroup in supplierFood.FoodAssignmentGroupBySuppliers)
                {
                    worksheet.Cells[dataStartRow2, 20].Value = foodAssignmentGroup.SupplierName;
                    var isFirstAssignment = true;
                    foreach (var foodAssignmentResponse in foodAssignmentGroup.FoodAssignmentResponses)
                    {
                        if (isFirstAssignment)
                        {
                            worksheet.Cells[dataStartRow2, 21].Value = foodAssignmentResponse.DeliveryDeadline.ToString();
                            isFirstAssignment = false;
                        }
                        worksheet.Cells[dataStartRow2, 22].Value = foodAssignmentResponse.FoodName;
                        worksheet.Cells[dataStartRow2, 23].Value = foodAssignmentResponse.AmountCooked;
                        worksheet.Cells[dataStartRow2, 24].Value = foodAssignmentResponse.CommissionRate;
                        worksheet.Cells[dataStartRow2, 25].Value = foodAssignmentResponse.ReceivedAmount;
                        worksheet.Cells[dataStartRow2, 26].Value = foodAssignmentResponse.Status;
                        worksheet.Cells[dataStartRow2, 22].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        dataStartRow2++;
                    }
                    
                }
            }
            
            // Auto-fit columns
            for (var i = 1; i <= 30; i++)
            {
                worksheet.Column(i).AutoFit();
            }
            
            // Tính toán phạm vi cuối cùng
            var maxDataRowAN = dataStartRow - 1; // Đối với khối A-N
            var maxDataRowRZ = dataStartRow2 - 1; // Đối với khối R-Z


            worksheet.Cells[$"A2:N2"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"A{maxDataRowAN}:N{maxDataRowAN}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"A2:A{maxDataRowAN}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"N2:N{maxDataRowAN}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            
            worksheet.Cells[$"R2:Z2"].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"R{maxDataRowRZ}:Z{maxDataRowRZ}"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"R2:R{maxDataRowRZ}"].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            worksheet.Cells[$"Z2:Z{maxDataRowRZ}"].Style.Border.Right.Style = ExcelBorderStyle.Medium;


            return excelPackage.GetAsByteArray();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
}