using MapsterMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;
using System.Drawing;
using System.Text;

namespace PerfectBreakfast.Infrastructure.BackgroundJobServices
{
    public class ManagementService : IManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly ICurrentTime _currentTime;
        private readonly IMapper _mapper;

        public ManagementService(IUnitOfWork unitOfWork, IMailService mailService, ICurrentTime currentTime, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _currentTime = currentTime;
            _mapper = mapper;
        }
        public async Task AutoCreateDailyOrderEachDay4PM()
        {
            try
            {
                // var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
                // var bookingDate = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
                // var now = _currentTime.GetCurrentTime();
                //
                // // Kiểm tra xem đã có đơn hàng nào được tạo cho ngày hiện tại chưa
                // bool isDailyOrderCreated = await _unitOfWork.DailyOrderRepository.IsDailyOrderCreated(now);
                //
                // // Nếu đã có đơn hàng cho ngày hiện tại, thoát khỏi hàm
                // if (isDailyOrderCreated)
                // {
                //     return;
                // }
                // foreach (var company in companies)
                // {
                //     var dailyOrder = new DailyOrder();
                //     dailyOrder.CompanyId = company.Id;
                //     dailyOrder.BookingDate = bookingDate.AddDays(2);
                //     dailyOrder.Status = DailyOrderStatus.Initial;
                //     dailyOrder.OrderQuantity = 0;
                //     dailyOrder.TotalPrice = 0;
                //     await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                //     await _unitOfWork.SaveChangeAsync();
                // }
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task AutoUpdateAndCreateDailyOrderAfter4PM()
        {
            try
            {
                //Update daily order each day after 4PM
                var now = _currentTime.GetCurrentTime();
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByCreationDate(now);
                if (dailyOrders.Count > 0)
                {
                    foreach (var dailyOrderEntity in dailyOrders)
                    {
                        var orders = dailyOrderEntity.Orders;
                        orders = orders.Where(o => o.OrderStatus == OrderStatus.Paid).ToList();
                        var totalOrderPrice = orders.Sum(o => o.TotalPrice);
                        var totalOrder = orders.Count();
                        dailyOrderEntity.TotalPrice = totalOrderPrice;
                        dailyOrderEntity.OrderQuantity = totalOrder;
                        dailyOrderEntity.Status = DailyOrderStatus.Processing;
                        _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                    }
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    Console.WriteLine("Không có dailyOrders được tạo hôm nay.");
                    return;
                }

                //// Xử lý dữ liệu để đẩy cho các đối tác theo cty
                // var managementUnits = await _unitOfWork.PartnerRepository.GetPartnersByToday(now);
                // foreach (var managementUnit in managementUnits)
                // {
                //     // Lấy danh sách các công ty thuộc MU
                //     var companies = managementUnit.Companies;
                //     var dailyOrderExcelList = new List<DailyOrderResponseExcel>();
                //     //var users = await _unitOfWork.UserRepository.GetUserByManagementUnitId(managementUnit.Id);
                //     //var email = users.SelectMany(u => u.Email).ToList();
                //
                //     // xử lý mỗi cty
                //     foreach (var company in companies)
                //     {
                //         var foodCounts = new Dictionary<string, int>();
                //
                //         // Lấy daily order
                //         var dailyOrder = company.DailyOrders.SingleOrDefault(x => x.CreationDate.Date.AddDays(1) == now.Date && x.Status == DailyOrderStatus.Processing);
                //
                //         // Lấy chi tiết các order detail
                //         var orders = await _unitOfWork.OrderRepository.GetOrderByDailyOrderId(dailyOrder.Id);
                //         var orderDetails = orders.SelectMany(order => order.OrderDetails).ToList();
                //
                //         // Đếm số lượng từng loại food
                //         foreach (var orderDetail in orderDetails)
                //         {
                //             if (orderDetail.Combo != null)
                //             {
                //                 // Nếu là combo thì lấy chi tiết các food trong combo
                //                 var combo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(orderDetail.Combo.Id);
                //                 var comboFoods = combo.ComboFoods;
                //
                //                 // Với mỗi food trong combo, cộng dồn số lượng
                //                 foreach (var comboFood in comboFoods)
                //                 {
                //                     var foodName = comboFood.Food.Name;
                //                     //... cộng dồn số lượng cho từng food
                //                     if (foodCounts.ContainsKey(foodName))
                //                     {
                //                         foodCounts[foodName] += orderDetail.Quantity;
                //                     }
                //                     else
                //                     {
                //                         foodCounts[foodName] = orderDetail.Quantity;
                //                     }
                //                 }
                //
                //             }
                //             else if (orderDetail.Food != null)
                //             {
                //                 // Xử lý order detail là food đơn lẻ
                //                 var foodName = orderDetail.Food.Name;
                //                 // cộng dồn số lượng cho từng food
                //                 if (foodCounts.ContainsKey(foodName))
                //                 {
                //                     foodCounts[foodName] += orderDetail.Quantity;
                //                 }
                //                 else
                //                 {
                //                     foodCounts[foodName] = orderDetail.Quantity;
                //                 }
                //             }
                //         }
                //
                //         // console ra xem tính toán đúng chưa 
                //         Console.OutputEncoding = Encoding.UTF8;
                //         Console.WriteLine($"Company: {company.Name}");
                //         foreach (var foodCount in foodCounts)
                //         {
                //             if (foodCounts.Count <= 0) Console.WriteLine("- không có đặt món nào!");
                //             Console.WriteLine($"- {foodCount.Key}: {foodCount.Value}");
                //         }
                //         DailyOrderResponseExcel dailyOrderExcel = new DailyOrderResponseExcel()
                //         {
                //             Company = company,
                //             FoodCount = foodCounts,
                //             OrderQuantity = dailyOrder.OrderQuantity,
                //             TotalPrice = dailyOrder.TotalPrice,
                //             BookingDate = dailyOrder.BookingDate
                //         };
                //         dailyOrderExcelList.Add(dailyOrderExcel);
                //     }
                //
                //     // Gửi email với file đính kèm
                //     var mailData = new MailDataViewModel(
                //         to: new List<string> { "phil41005@gmail.com" }, // Thay bằng địa chỉ email người nhận (email)
                //         subject: "Daily Order Report",
                //         body: "Attached is the daily order report. Please find the attached Excel file.",
                //         excelAttachmentStream: CreateExcelForManagementUnit(dailyOrderExcelList),
                //         excelAttachmentFileName: $"DailyOrder_{managementUnit.Name}_{_currentTime.GetCurrentTime().ToString("yyyy-MM-dd")}.xlsx"
                //     );
                //     await _mailService.SendAsync(mailData, CancellationToken.None);
                //}
                
                //Create daily order after update
                var companiesv2 = await _unitOfWork.CompanyRepository.GetAllAsync();
                var bookingDate = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
                var today = _currentTime.GetCurrentTime();
                
                // Kiểm tra xem đã có đơn hàng nào được tạo cho ngày hiện tại chưa
                bool isDailyOrderCreated = await _unitOfWork.DailyOrderRepository.IsDailyOrderCreated(today);
        
                // Nếu đã có đơn hàng cho ngày hiện tại, thoát khỏi hàm
                if (isDailyOrderCreated)
                {
                    return;
                }
                foreach (var company in companiesv2)
                {
                    var dailyOrder = new DailyOrder();
                    dailyOrder.CompanyId = company.Id;
                    dailyOrder.BookingDate = bookingDate.AddDays(2);
                    dailyOrder.Status = DailyOrderStatus.Initial;
                    dailyOrder.OrderQuantity = 0;
                    dailyOrder.TotalPrice = 0;
                    await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("looix gi do ");
                throw new Exception($"{e.Message}");
            }
        }

        public byte[] CreateExcelForManagementUnit(List<DailyOrderResponseExcel> dailyOrders)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("DailyOrders");

                // Đặt kích thước của dòng 1
                worksheet.Row(1).Height = 30;

                // Thêm tiêu đề cột và định dạng
                var headerCells = worksheet.Cells["A1:D1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1"].Value = "Perfect Breakfast";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.DarkGreen);
                worksheet.Cells["E1:H1"].Merge = true;
                worksheet.Cells["E1"].Value = "Food";
                worksheet.Cells["E1"].Style.Font.Size = 16;
                worksheet.Cells["E1"].Style.Font.Color.SetColor(Color.DarkGreen);

                worksheet.Cells["A2"].Value = "Company";
                worksheet.Cells["B2"].Value = "Total Price";
                worksheet.Cells["C2"].Value = "Order Quantity";
                worksheet.Cells["D2"].Value = "Booking Date";



                // Thêm dữ liệu từ danh sách DailyOrder
                int row = 3; // Bắt đầu từ dòng 3
                foreach (var data in dailyOrders)
                {
                    worksheet.Cells[row, 1].Value = data.Company?.Name;
                    worksheet.Cells[row, 2].Value = data.TotalPrice;
                    worksheet.Cells[row, 3].Value = data.OrderQuantity;
                    worksheet.Cells[row, 4].Value = data.BookingDate.ToString("dd-MM-yyyy");

                    // Add data for each food item (if any)
                    if (data.FoodCount != null && data.FoodCount.Count > 0)
                    {
                        int foodColumn = 5; // Starting column for food items (E)
                        foreach (var foodItem in data.FoodCount)
                        {
                            // Create a formatted string with keys and values
                            string formattedString = $"{foodItem.Key}: {foodItem.Value}";

                            // Add the formatted string in the current row and column
                            worksheet.Cells[row, foodColumn].Value = formattedString;

                            foodColumn++; // Move to the next column
                        }
                    }
                    row++;
                }

                // Tự động điều chỉnh chiều rộng cột
                worksheet.Cells.AutoFitColumns();

                byte[] byteArray = package.GetAsByteArray();
                return byteArray;
            }
        }

        public byte[] CreateExcelAndForSupplier(List<Supplier> suppliers)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("DailyOrders");

                // Đặt kích thước của dòng 1
                worksheet.Row(1).Height = 30;

                // Thêm tiêu đề cột và định dạng
                var headerCells = worksheet.Cells["A1:D1"];
                headerCells.Style.Font.Bold = true;
                headerCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);

                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1"].Value = "Perfect Breakfast";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.DarkGreen);

                worksheet.Cells["A2"].Value = "Name";

                // Thêm dữ liệu từ danh sách DailyOrder
                int row = 3; // Bắt đầu từ dòng 3
                foreach (var data in suppliers)
                {
                    worksheet.Cells[row, 1].Value = data.Name;


                    row++;
                }

                // Tự động điều chỉnh chiều rộng cột
                worksheet.Cells.AutoFitColumns();

                byte[] byteArray = package.GetAsByteArray();
                return byteArray;
            }
        }

    }
}
