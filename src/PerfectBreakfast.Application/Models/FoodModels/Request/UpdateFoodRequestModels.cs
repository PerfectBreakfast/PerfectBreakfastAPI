﻿using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Models.FoodModels.Request
{
    public record UpdateFoodRequestModels
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;

        //relationship
        //public Guid? CategoryId { get; set; }
    }
}