﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BExIS.Security.Services.Authorization
{
    public interface IPermissionManager : IFeaturePermissionManager, IDataPermissionManager
    {
    }
}
