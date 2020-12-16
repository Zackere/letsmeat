import sys
import os

if len(sys.argv) != 2:
  raise Exception('Wrong number of arguments')

controller_name = f'{sys.argv[1]}Controller'

file_contents = f"""using LetsMeatAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsMeatAPI.Controllers {{
  [Route("[controller]")]
  [ApiController]
  public class {controller_name} : ControllerBase {{
    public {controller_name}(
      UserManager userManager,
      LMDbContext context,
      ILogger<{controller_name}> logger
    ) {{
      _userManager = userManager;
      _context = context;
      _logger = logger;
    }}

    private readonly UserManager _userManager;
    private readonly LMDbContext _context;
    private readonly ILogger<{controller_name}> _logger;
  }}
}}
"""
filepath = f'{os.path.dirname(os.path.realpath(__file__))}/../LetsMeatAPI/Controllers/{controller_name}.cs'
with open(filepath, 'w') as f:
  f.write(file_contents)
print(f'Output written to: {filepath}')