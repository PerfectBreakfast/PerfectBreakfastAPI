﻿using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.SupplyAssigmentModels.Request;

public class UpdateSupplyAssigment
{
    public Guid? SupplierId { get; set; }
    public Guid? ManagementUnitId { get; set; }
}