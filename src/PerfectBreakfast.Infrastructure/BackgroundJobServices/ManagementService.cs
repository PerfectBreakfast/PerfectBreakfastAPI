using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Commons;
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
        public async Task AutoCreateDailyOrderEachDay1AM()
        {
            try
            {
                var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
                var bookingDate = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
                foreach (var company in companies)
                {
                    var dailyOrder = new DailyOrder();
                    dailyOrder.CompanyId = company.Id;
                    dailyOrder.BookingDate = bookingDate.AddDays(1);
                    dailyOrder.Status = DailyOrderStatus.Pending;
                    dailyOrder.OrderQuantity = 0;
                    dailyOrder.TotalPrice = 0;
                    await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task AutoUpdateDailyOrderAfter4PM()
        {
            try
            {
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByCreationDate(_currentTime.GetCurrentTime());
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
                        dailyOrderEntity.Status = DailyOrderStatus.Fulfilled;
                        _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                    }
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    throw new Exception("khong co dailyOrders được tạo hôm nay");
                }

                // Xử lý dữ liệu để đẩy cho các đối tác theo cty 
                /*var magementUnits = await _unitOfWork.ManagementUnitRepository.FindAll(m => m.Companies).ToListAsync();

                foreach (var magementUnit in magementUnits)
                {
                    var dailyOrderList = new List<DailyOrder>();
                    var dailyOrderExcelList = new List<DailyOrderResponseExcel>();
                    foreach (var company in magementUnit.Companies)
                    {
                        var updatedDailyOrder = await _unitOfWork.DailyOrderRepository.FindAllDataByCompanyId(company.Id);
                        dailyOrderList.Add(updatedDailyOrder);
                        foreach (var dailyOrder in dailyOrders)
                        {
                            var orders = dailyOrder.Orders;
                            foreach (var order in orders)
                            {
                                var combos = order.OrderDetails.Select(c => c.Combo).ToList();
                                foreach (var combo in combos)
                                {
                                    var foods = combo.ComboFoods.Select(cf => cf.Food).ToList();
                                    var dailyOrderExcel = _mapper.Map<DailyOrderResponseExcel>(dailyOrder);
                                    dailyOrderExcel.FoodResponses = foods;
                                    dailyOrderExcelList.Add(dailyOrderExcel);
                                }
                            }
                        }
                    }
                    
                    

                    // Gửi email với file đính kèm
                    var mailData = new MailDataViewModel(
                        to: new List<string> { "phil41005@gmail.com" }, // Thay bằng địa chỉ email người nhận
                        subject: "Daily Order Report",
                        body: "Attached is the daily order report. Please find the attached Excel file.",
                        excelAttachmentStream: CreateExcelAndReturnStream(dailyOrderExcelList),
                        excelAttachmentFileName: $"DailyOrder_{magementUnit.Name}_{_currentTime.GetCurrentTime().ToString("yyyy-MM-dd")}.xlsx"
                    );
                    await _mailService.SendAsync(mailData, CancellationToken.None);
                }*/
                
                //// Xử lý dữ liệu để đẩy cho các đối tác theo cty
                var now = _currentTime.GetCurrentTime();
                var managementUnits = await _unitOfWork.ManagementUnitRepository.GetManagementUnits(now);
                foreach (var managementUnit in managementUnits)
                {
                    // Lấy danh sách các công ty thuộc MU
                    var companies = managementUnit.Companies;
                    
                    // xử lý mỗi cty
                    foreach (var company in companies)
                    {
                        var foodCounts = new Dictionary<string, int>();
                        
                        // Lấy danh sách các daily order
                        var dailyorders = company.DailyOrders.Where(x => x.CreationDate.Date == now.Date);
                        
                        // Duuyệt qua từng daily order
                        foreach (var dailyorder in dailyorders)  
                        {
                            // Lấy chi tiết các order detail
                            var orderDetails = dailyorder.Orders.SelectMany(o => o.OrderDetails);
     
                            // Đếm số lượng từng loại food
                            foreach (var orderDetail in orderDetails)
                            {
                                if (orderDetail.Combo != null) 
                                {
                                    // Nếu là combo thì lấy chi tiết các food trong combo
                                    var comboFoods = orderDetail.Combo.ComboFoods;
    
                                    // Với mỗi food trong combo, cộng dồn số lượng
                                    foreach (var comboFood in comboFoods)
                                    {
                                        var foodName = comboFood.Food.Name;
                                        //... cộng dồn số lượng cho từng food
                                        if (foodCounts.ContainsKey(foodName))
                                        {
                                            foodCounts[foodName] += orderDetail.Quantity;  
                                        }
                                        else
                                        {
                                            foodCounts[foodName] = orderDetail.Quantity;
                                        }
                                    }

                                }
                                else if (orderDetail.Food != null)
                                {
                                    // Xử lý order detail là food đơn lẻ
                                    var foodName = orderDetail.Food.Name;
                                    // cộng dồn số lượng cho từng food
                                    if (foodCounts.ContainsKey(foodName))
                                    {
                                        foodCounts[foodName] += orderDetail.Quantity;  
                                    }
                                    else
                                    {
                                        foodCounts[foodName] = orderDetail.Quantity;
                                    }
                                }
                            }
                        }
                        // console ra xem tính toán đúng chưa 
                        Console.OutputEncoding = Encoding.UTF8;
                        Console.WriteLine($"Company: {company.Name}");
                        foreach (var foodCount in foodCounts)
                        {
                            if(foodCounts.Count <= 0) Console.WriteLine("- không có đặt món nào!");
                            Console.WriteLine($"- {foodCount.Key}: {foodCount.Value}"); 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("looix gi do ");
                throw new Exception($"{e.Message}");
            }
        }

        public byte[] CreateExcelAndReturnStream(List<DailyOrderResponseExcel> dailyOrders)
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

                    // Thêm thông tin thực phẩm vào từng cột
                    if (data.FoodResponses != null && data.FoodResponses.Any())
                    {
                        // Lấy tên thực phẩm và đếm số lượng
                        var foodNameCount = data.FoodResponses
                            .GroupBy(food => food.Name)
                            .Select(group => new { FoodName = group.Key, Count = group.Count() })
                            .ToList();

                        // Thêm thông tin thực phẩm vào từng cột
                        foreach (var foodCount in foodNameCount)
                        {
                            worksheet.Cells[row, 5].Value = foodCount.FoodName;
                            worksheet.Cells[row, 6].Value = foodCount.Count;

                            row++;
                        }
                    }
                    else
                    {
                        // Nếu không có thông tin thực phẩm, di chuyển đến dòng tiếp theo
                        row++;
                    }
                }

                // Tự động điều chỉnh chiều rộng cột
                worksheet.Cells.AutoFitColumns();

                byte[] byteArray = package.GetAsByteArray();
                return byteArray;
            }
        }

    }
}
