﻿using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz.Task
{
    public interface ITask
    {
        CustomJsonResult Run();
    }
}
