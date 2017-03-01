using Lumos.Entity;
using Lumos.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class PosMachineProvider : BaseProvider
    {
        public CustomJsonResult Add(int operater, PosMachine posMachine)
        {
            CustomJsonResult result = new CustomJsonResult();

            var l_posMachine = CurrentDb.PosMachine.Where(m => m.DeviceId == posMachine.DeviceId).FirstOrDefault();
            if (l_posMachine != null)
                return new CustomJsonResult(ResultType.Failure, "该POS机设备ID已经登记");


            posMachine.CreateTime = this.DateTime;
            posMachine.Creator = operater;
            posMachine.IsUse = false;
            CurrentDb.PosMachine.Add(posMachine);
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, "登记成功");
        }

        public CustomJsonResult Edit(int operater, PosMachine posMachine)
        {
            CustomJsonResult result = new CustomJsonResult();

            var l_posMachine = CurrentDb.PosMachine.Where(m => m.Id == posMachine.Id).FirstOrDefault();
            if (l_posMachine == null)
                return new CustomJsonResult(ResultType.Failure, "不存在");

            l_posMachine.Deposit = posMachine.Deposit;
            l_posMachine.Rent = posMachine.Rent;
            l_posMachine.FuselageNumber = posMachine.FuselageNumber;
            l_posMachine.TerminalNumber = posMachine.TerminalNumber;
            l_posMachine.Version = posMachine.Version;
            l_posMachine.LastUpdateTime = this.DateTime;
            l_posMachine.Mender = operater;
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, "保存成功");
        }


    }

}
