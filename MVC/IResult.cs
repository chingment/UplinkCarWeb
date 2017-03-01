﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Mvc
{
    public enum ResultType
    {
        Unknown = 0,
        Success = 1,
        Failure = 2,
        Exception = 3
    }

    public enum ResultCode
    {
        Unknown = 0,
        Success = 1000,
        WithdrawConfirm = 1012,
        Failure = 2000,
        FailureSign = 2010,
        FailureSignIn = 2020,
        FailureNoData = 2030,
        FailureValidCode = 2040,
        FailureUserNameNotExists = 2050,
        Exception = 3000
    }


    public interface IResult
    {
        ResultType Result { get; set; }
        ResultCode Code { get; set; }
        string Message { get; set; }
        object Data { get; set; }
    }
}
