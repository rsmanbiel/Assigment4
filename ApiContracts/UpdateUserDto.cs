﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiContracts;

public class UpdateUserDto
{
    public required int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
