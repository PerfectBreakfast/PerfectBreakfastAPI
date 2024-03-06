﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Models.CategoryModels.Response
{
    public record CategoryResponse
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
