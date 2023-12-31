﻿using Microsoft.EntityFrameworkCore.Storage;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Infrastructure.Repositories;

namespace PerfectBreakfast.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;
    

    public UnitOfWork(AppDbContext dbContext,ICurrentTime currentTime, IClaimsService claimsService)
    {
        _dbContext = dbContext;
        _currentTime = currentTime;
        _claimsService = claimsService;
    }
    
    public IUserRepository UserRepository => new UserRepository(_dbContext,_currentTime,_claimsService);

    

    public async Task<int> SaveChangeAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _dbContext.Database.BeginTransactionAsync();
    }
}
