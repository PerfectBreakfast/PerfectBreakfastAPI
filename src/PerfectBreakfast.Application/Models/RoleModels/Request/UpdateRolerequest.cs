﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Models.RoleModels.Request
{
    public record UpdateRolerequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
