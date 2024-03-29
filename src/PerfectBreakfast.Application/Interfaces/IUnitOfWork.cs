﻿using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public Task<int> SaveChangeAsync();
    IDbTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync();
    public UserManager<User> UserManager { get; }
    public SignInManager<User> SignInManager { get; }
    public RoleManager<Role> RoleManager { get; }
    public IUserRepository UserRepository { get; }
    public ICompanyRepository CompanyRepository { get; }
    public ISupplierRepository SupplierRepository { get; }
    public IDeliveryRepository DeliveryRepository { get; }
    public IPartnerRepository PartnerRepository { get; }
    public IRoleRepository RoleRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IFoodRepository FoodRepository { get; }
    public IMenuRepository MenuRepository { get; }
    public IPaymentMethodRepository PaymentMethodRepository { get; }
    public IComboRepository ComboRepository { get; }
    public IDailyOrderRepository DailyOrderRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public ISupplierCommissionRateRepository SupplierCommissionRateRepository { get; }
    public ISupplyAssigmentRepository SupplyAssigmentRepository { get; }
    public ISupplierFoodAssignmentRepository SupplierFoodAssignmentRepository { get; }
    public IShippingOrderRepository ShippingOrderRepository { get; }
    public IMealRepository MealRepository { get; }
    public IMealSubscriptionRepository MealSubscriptionRepository { get; }
    public IMenuFoodRepository MenuFoodRepository{ get;}
    public IComboFoodRepository ComboFoodRepository{ get;}
    public ISettingRepository SettingRepository{ get;}
    public IActionHistoryRepository ActionHistoryRepository{ get;}
}
