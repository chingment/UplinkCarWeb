﻿using Lumos.BLL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBack.Models.Biz.Merchant
{
    public class PrimaryAuditViewModel : BaseViewModel
    {
        private Lumos.Entity.Merchant _merchant = new Lumos.Entity.Merchant();

        private SysSalesmanUser _salesman;

        private List<Lumos.Entity.MerchantPosMachine> _merchantPosMachine = new List<Lumos.Entity.MerchantPosMachine>();

        private List<Lumos.Entity.BankCard> _bankCard = new List<Lumos.Entity.BankCard>();

        private Lumos.Entity.BizProcessesAudit _bizProcessesAudit = new Lumos.Entity.BizProcessesAudit();

        private List<Lumos.Entity.CarInsuranceCompany> _carInsuranceCompany = new List<Lumos.Entity.CarInsuranceCompany>();

        public Lumos.Entity.Merchant Merchant
        {
            get
            {
                return _merchant;
            }
            set
            {
                _merchant = value;
            }
        }

        public List<Lumos.Entity.BankCard> BankCard
        {
            get
            {
                return _bankCard;
            }
            set
            {
                _bankCard = value;
            }
        }

        public List<Lumos.Entity.MerchantPosMachine> MerchantPosMachine
        {
            get
            {
                return _merchantPosMachine;
            }
            set
            {
                _merchantPosMachine = value;
            }
        }

        public SysSalesmanUser Salesman
        {
            get
            {
                return _salesman;
            }
            set
            {
                _salesman = value;
            }
        }

        public Lumos.Entity.BizProcessesAudit BizProcessesAudit
        {
            get
            {
                return _bizProcessesAudit;
            }
            set
            {
                _bizProcessesAudit = value;
            }
        }

        private List<Lumos.Entity.MerchantEstimateCompany> _merchantEstimateCompany = new List<Lumos.Entity.MerchantEstimateCompany>();

        public int[] EstimateInsuranceCompanyIds
        {
            get; set;
        }

        public List<Lumos.Entity.MerchantEstimateCompany> MerchantEstimateCompany
        {
            get
            {
                return _merchantEstimateCompany;
            }
            set
            {
                _merchantEstimateCompany = value;
            }
        }

        public List<Lumos.Entity.CarInsuranceCompany> CarInsuranceCompany
        {
            get
            {
                return _carInsuranceCompany;
            }
            set
            {
                _carInsuranceCompany = value;
            }
        }

        public PrimaryAuditViewModel()
        {

        }

        public PrimaryAuditViewModel(int id)
        {

            var bizProcessesAudit = BizFactory.BizProcessesAudit.ChangeMerchantAuditStatus(this.Operater, id, Enumeration.MerchantAuditStatus.InPrimaryAudit);

            if (bizProcessesAudit != null)
            {
                _bizProcessesAudit = bizProcessesAudit;


                var merchant = CurrentDb.Merchant.Where(m => m.Id == bizProcessesAudit.AduitReferenceId).FirstOrDefault();
                if (merchant != null)
                {
                    _merchant = merchant;


                    if (_merchant.SalesmanId != null)
                    {
                        var salesman = CurrentDb.SysSalesmanUser.Where(m => m.Id == _merchant.SalesmanId).FirstOrDefault();
                        if (salesman != null)
                        {
                            _salesman = salesman;
                        }
                    }

                    var merchantPosMachine = CurrentDb.MerchantPosMachine.Where(m => m.MerchantId == merchant.Id).ToList();
                    if (merchantPosMachine != null)
                    {
                        _merchantPosMachine = merchantPosMachine;

                        foreach (var m in _merchantPosMachine)
                        {
                            var posMachine = CurrentDb.PosMachine.Where(q => q.Id == m.PosMachineId).FirstOrDefault();

                            m.PosMachine = posMachine;
                        }
                    }

                    var bankCard = CurrentDb.BankCard.Where(m => m.MerchantId == merchant.Id).ToList();
                    if (bankCard != null)
                    {
                        _bankCard = bankCard;
                    }

                    var carInsuranceCompany = CurrentDb.CarInsuranceCompany.ToList();
                    if (carInsuranceCompany != null)
                    {
                        _carInsuranceCompany = carInsuranceCompany;
                    }

                    var merchantEstimateCompany = CurrentDb.MerchantEstimateCompany.Where(m => m.MerchantId == merchant.Id).ToList();
                    if (merchantEstimateCompany != null)
                    {
                        _merchantEstimateCompany = merchantEstimateCompany;
                    }
                }
            }

        }

    }
}