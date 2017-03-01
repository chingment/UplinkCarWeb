﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.DAL.AuthorizeRelay;

namespace System
{

    /// <summary>
    /// 权限代码
    /// </summary>
    public class PermissionCode
    {
        public const string 所有用户管理 = "Sys1000";
        public const string 后台用户管理 = "Sys2000";
        public const string 前台用户管理 = "Sys3000";
        public const string 角色管理 = "Sys4000";
        public const string 菜单管理 = "Sys5000";
    }

}
