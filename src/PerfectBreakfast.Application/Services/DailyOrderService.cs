using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Request;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services
{
    public class DailyOrderService : IDailyOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public DailyOrderService(IUnitOfWork unitOfWork, IMapper mapper, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
        }

        public async Task<OperationResult<DailyOrderResponse>> AutoCreate(DateTime dateTime)
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
                var bookingDate = DateOnly.FromDateTime(dateTime);
                foreach (var company in companies)
                {
                    var dailyOrder = new DailyOrder();
                    dailyOrder.Company = company;
                    dailyOrder.BookingDate = bookingDate.AddDays(1);
                    dailyOrder.Status = DailyOrderStatus.Pending;
                    dailyOrder.OrderQuantity = 0;
                    dailyOrder.TotalPrice = 0;
                    _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                }
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<DailyOrderResponse>> AutoUpdate(DateTime dateTime)
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var dailyOrders = await _unitOfWork.DailyOrderRepository.FindByCreationDate(dateTime);
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
                    result.AddUnknownError("Today doesn't have daily order");
                    return result;
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
                        to: new List<string> { "phil41005@gmail.com" }, // Thay bằng địa chỉ email người nhận
                        subject: "Daily Order Report",
                        body: "Attached is the daily order report. Please find the attached Excel file.",
                        excelAttachmentStream: CreateExcelAndReturnStream(dailyOrderList),
                        excelAttachmentFileName: $"DailyOrder_{magementUnit.Name}_{dateTime.ToString("yyyy-MM-dd")}.xlsx"
                    );
                    _mailService.SendAsync(mailData, CancellationToken.None);
                }

            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<DailyOrderResponse>> CreateDailyOrder(DailyOrderRequest dailyOrderRequest)
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var dailyOrder = _mapper.Map<DailyOrder>(dailyOrderRequest);
                dailyOrder.Status = DailyOrderStatus.Pending;
                dailyOrder.OrderQuantity = 0;
                dailyOrder.TotalPrice = 0;
                await _unitOfWork.DailyOrderRepository.AddAsync(dailyOrder);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<DailyOrderResponse>(dailyOrder);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<DailyOrderResponse>>> GetDailyOrderPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<DailyOrderResponse>>();
            try
            {
                var pagination = await _unitOfWork.DailyOrderRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<DailyOrderResponse>>(pagination);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<DailyOrderResponse>>> GetDailyOrders()
        {
            var result = new OperationResult<List<DailyOrderResponse>>();
            try
            {
                var dailyOrders = await _unitOfWork.DailyOrderRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<DailyOrderResponse>>(dailyOrders);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<DailyOrderResponse>> UpdateDailyOrder(Guid id, UpdateDailyOrderRequest updateDailyOrderRequest)
        {
            var result = new OperationResult<DailyOrderResponse>();
            try
            {
                var dailyOrderEntity = await _unitOfWork.DailyOrderRepository.GetByIdAsync(id);
                _mapper.Map(updateDailyOrderRequest, dailyOrderEntity);
                _unitOfWork.DailyOrderRepository.Update(dailyOrderEntity);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        static Stream CreateExcelAndReturnStream(List<DailyOrder> dailyOrders)
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

                MemoryStream stream = new MemoryStream();
                package.SaveAs(stream);

                // Đặt con trỏ stream về đầu để chuẩn bị cho việc đọc
                stream.Position = 0;

                return stream;
            }
        }

    }
}
