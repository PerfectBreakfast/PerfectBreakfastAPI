using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.FoodModels.Request
{
    public record CreateFoodRequestModels
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        public int? FoodStatus { get; set; }
        //relationship
        public Guid? CategoryId { get; set; }
    }
}
