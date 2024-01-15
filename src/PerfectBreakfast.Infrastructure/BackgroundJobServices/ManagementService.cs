using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Infrastructure.BackgroundJobServices
{
    public class ManagementService : IManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly ICurrentTime _currentTime;

        public ManagementService(IUnitOfWork unitOfWork, IMailService mailService, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _currentTime = currentTime;
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
                    dailyOrder.Company = company;
                    dailyOrder.BookingDate = bookingDate.AddDays(1);
                    dailyOrder.Status = DailyOrderStatus.Pending;
                    dailyOrder.OrderQuantity = 0;
                    dailyOrder.TotalPrice = 0;
                    await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                }
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
            }
        }

        public async Task AutoUpdateDailyOrderAfter4PM()
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByCreationDate(_currentTime.GetCurrentTime());
                if (dailyOrders.Count > 0)
                {
                    foreach (var dailyOrderEntity in dailyOrders)
                    {
                        var orders = dailyOrderEntity.Orders;
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
                    throw new Exception("khong co dailyOrders");
                }

                var magementUnits = await _unitOfWork.ManagementUnitRepository.FindAll(m => m.Companies).ToListAsync();

                foreach (var magementUnit in magementUnits)
                {
                    var dailyOrderList = new List<DailyOrder>();
                    foreach (var company in magementUnit.Companies)
                    {
                        var updatedDailyOrder = await _unitOfWork.DailyOrderRepository.FindByCompanyId(company.Id);
                        dailyOrderList.Add(updatedDailyOrder);
                    }
                    
                    
                     // Gửi email với file đính kèm
                    var mailData = new MailDataViewModel(
                        to: new List<string> { "viethungdev23@gmail.com" }, // Thay bằng địa chỉ email người nhận
                        subject: "Daily Order Report",
                        body: "Attached is the daily order report. Please find the attached Excel file.",
                        excelAttachmentStream: CreateExcelAndReturnStream(dailyOrderList),
                        excelAttachmentFileName: $"DailyOrder_{magementUnit.Name}_{_currentTime.GetCurrentTime().ToString("yyyy-MM-dd")}.xlsx"
                    );
                    await _mailService.SendAsync(mailData, CancellationToken.None); 
                }

            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }

        }
        
        public byte[] CreateExcelAndReturnStream(List<DailyOrder> dailyOrders)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.Add("DailyOrders");

                // Thêm tiêu đề cột
                worksheet.Cells["A1"].Value = "Company";
                worksheet.Cells["B1"].Value = "TotalPrice";
                worksheet.Cells["C1"].Value = "OrderQuantity";
                worksheet.Cells["D1"].Value = "BookingDate";

                // Thêm dữ liệu từ danh sách DailyOrder
                int row = 2;
                foreach (var data in dailyOrders)
                {
                    worksheet.Cells[row, 1].Value = data.Company.Name;
                    worksheet.Cells[row, 2].Value = data.TotalPrice;
                    worksheet.Cells[row, 3].Value = data.OrderQuantity;
                    worksheet.Cells[row, 4].Value = data.BookingDate.ToString("yyyy-MM-dd");
                    row++;
                }

                byte[] byteArray = package.GetAsByteArray();
                return byteArray;
            }
        }
    }
}
