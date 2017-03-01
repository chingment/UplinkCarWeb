﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Sys.Role
{
    public class DetailsViewModel:BaseViewModel
    {
        private SysRole _sysRole = new SysRole();

        public SysRole SysRole
        {
            get
            {
                return _sysRole;
            }
            set
            {
                _sysRole = value;
            }
        }

        public DetailsViewModel()
        {

        }
        public DetailsViewModel(int id)
        {
            var sysRole = CurrentDb.Roles.Where(m => m.Id == id).FirstOrDefault();
            if (sysRole != null)
            {
                _sysRole = sysRole;
            }


        }
    }
}