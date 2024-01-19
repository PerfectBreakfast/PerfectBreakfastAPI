using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.RoleModels.Response
{
    public record RoleResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public UnitCode UnitCode { get; set; }
    }
}
