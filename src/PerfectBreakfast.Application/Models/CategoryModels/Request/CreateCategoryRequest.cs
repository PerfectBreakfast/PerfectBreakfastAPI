﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Models.CategoryModels.Request
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
