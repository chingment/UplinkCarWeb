﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public abstract class BaseFactory
    {
        public static AppSettingsProvider AppSettings
        {
            get
            {
                return new AppSettingsProvider();
            }
        }

        public static SmsProvider Sms
        {
            get
            {
                return new SmsProvider();
            }
        }
    }
}
