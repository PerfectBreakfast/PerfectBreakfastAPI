using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Models.FoodModels.Request
{
    public record CreateFoodRequestModels
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }

        //relationship
        public Guid? CategoryId { get; set; }
    }
}
