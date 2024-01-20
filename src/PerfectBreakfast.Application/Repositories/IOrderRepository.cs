﻿using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Order> GetOrderByOrderCode(int orderCode);
        public Task<List<Order>> GetOrderByDailyOrderId(Guid dailyOrderId);
    }
}
