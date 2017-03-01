using Lumos.DAL;
using Lumos.Mvc;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebBack.Models.Biz.Report;
using Lumos.Entity;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;

namespace WebBack.Controllers.Biz
{
    public class ReportTable
    {
        public ReportTable()
        {

        }

        public ReportTable(string html)
        {
            this.Html = html;
        }

        public string Html
        {
            get;
            set;
        }
    }

    public class ReportController : WebBackController
    {


        public ActionResult Withdraw(WithdrawViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>商户代码</th>");
            sbTable.Append("<th>商户名称</th>");
            sbTable.Append("<th>提现金额</th>");
            sbTable.Append("<th>申请时间</th>");
            sbTable.Append("<th>处理时间</th>");
            sbTable.Append("<th>结果</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"6\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select b.ClientCode,b.YYZZ_Name,a.Amount,a.SettlementStartTime,a.SettlementEndTime,a.Status from Withdraw a left join  Merchant b on a.MerchantId=b.Id  ");
                sql.Append(" where 1=1 ");


                if (model.Status != Enumeration.WithdrawStatus.Unknow)
                {
                    sql.Append(" and  a.Status='" + (int)model.Status + "'");
                }

                if (!string.IsNullOrEmpty(model.ClientCode))
                {
                    sql.Append(" and  b.ClientCode='" + model.ClientCode + "'");
                }
                if (model.StartTime != null)
                {
                    sql.Append(" and  a.SettlementStartTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.SettlementStartTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }

                sql.Append(" order by a.SettlementStartTime desc ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");
                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 5:
                                td_value = GetWithdrawStatusName(dtData.Rows[r][c].ToString().Trim());
                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商户提现报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult Merchant(MerchantViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>商户代码</th>");
            sbTable.Append("<th>商户名称</th>");
            sbTable.Append("<th>POS机ID</th>");
            sbTable.Append("<th>营业执照编号</th>");
            sbTable.Append("<th>商户地址</th>");
            sbTable.Append("<th>法人</th>");
            sbTable.Append("<th>法人身份证</th>");
            sbTable.Append("<th>联系人</th>");
            sbTable.Append("<th>联系方式</th>");
            sbTable.Append("<th>绑定银行卡账号</th>");
            sbTable.Append("<th>持卡人</th>");
            sbTable.Append("<th>开户行</th>");
            sbTable.Append("<th>激活时间</th>");
            sbTable.Append("<th>注销时间</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"14\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select b.ClientCode,b.YYZZ_Name,d.DeviceId,b.YYZZ_RegisterNo,b.YYZZ_Address,b.FR_Name,b.FR_IdCardNo,b.ContactName,b.ContactPhoneNumber,c.BankAccountNo,c.BankAccountName,c.BankName,a.DepositPayTime,a.ReturnTime from [dbo].[MerchantPosMachine] a ");
                sql.Append(" left join[dbo].[Merchant] b on a.MerchantId = b.Id");
                sql.Append(" left join[dbo].[BankCard] c on a.BankCardId = c.Id");
                sql.Append(" left join[dbo].[PosMachine] d on a.PosMachineId = d.Id  ");
                sql.Append(" where 1=1 ");

                if (!string.IsNullOrEmpty(model.ClientCode))
                {
                    sql.Append(" and  b.ClientCode='" + model.ClientCode + "'");
                }
                if (model.StartTime != null)
                {
                    sql.Append(" and  a.DepositPayTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.DepositPayTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }

                sql.Append(" order by a.DepositPayTime desc ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");
                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商户提现报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult NoActiveAccount(MerchantViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th style=\"width:50%\">商户代码</th>");
            sbTable.Append("<th style=\"width:50%\">POS机ID</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"2\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select b.ClientCode,d.DeviceId from [dbo].[MerchantPosMachine] a ");
                sql.Append(" left join[dbo].[Merchant] b on a.MerchantId = b.Id ");
                sql.Append(" ");
                sql.Append(" left join[dbo].[PosMachine] d on a.PosMachineId = d.Id  ");
                sql.Append(" where 1=1 and a.[Status]=2 ");

                if (!string.IsNullOrEmpty(model.ClientCode))
                {
                    sql.Append(" and  b.ClientCode='" + model.ClientCode + "'");
                }
                if (model.StartTime != null)
                {
                    sql.Append(" and  a.DepositPayTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.DepositPayTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }

                sql.Append(" order by a.DepositPayTime desc ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");
                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商户提现报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult CarInsure(CarInsureViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>商户代码</th>");
            sbTable.Append("<th>商户名称</th>");
            sbTable.Append("<th>保单号</th>");
            sbTable.Append("<th>投保人</th>");
            sbTable.Append("<th>商业险保费金额</th>");
            sbTable.Append("<th>交强险保费金额</th>");
            sbTable.Append("<th>车船税金额</th>");
            sbTable.Append("<th>总保费金额</th>");
            sbTable.Append("<th>商业险佣金金额</th>");
            sbTable.Append("<th>提交时间</th>");
            sbTable.Append("<th>支付时间</th>");
            sbTable.Append("<th>取消时间</th>");
            sbTable.Append("<th>状态</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"13\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder("select c.ClientCode,c.YYZZ_Name,b.InsuranceOrderId,b.CarOwner,b.CommercialPrice,b.CompulsoryPrice,b.TravelTaxPrice,a.Price,b.MerchantCommission,a.SubmitTime,a.PayTime,a.CancleTime,a.Status from  [dbo].[Order]  a ");
                sql.Append(" inner join [dbo].[OrderToCarInsure]  b on a.Id=b.Id ");
                sql.Append(" left join [dbo].[Merchant] c on a.MerchantId=c.Id ");

                sql.Append(" where 1=1 ");

                if (model.Status != Enumeration.OrderStatus.Unknow)
                {
                    sql.Append(" and  a.Status='" + (int)model.Status + "'");
                }

                if (!string.IsNullOrEmpty(model.ClientCode))
                {
                    sql.Append(" and  b.ClientCode='" + model.ClientCode + "'");
                }
                if (model.StartTime != null)
                {
                    sql.Append(" and  a.SubmitTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.SubmitTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }

                sql.Append(" order by a.SubmitTime desc ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");
                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 12:
                                td_value = GetOrderStatusName(dtData.Rows[r][c].ToString().Trim());
                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商户提现报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }

        public ActionResult DepositRent(MerchantViewModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>商户代码</th>");
            sbTable.Append("<th>商户名称</th>");
            sbTable.Append("<th>POS机ID</th>");
            sbTable.Append("<th>地址</th>");
            sbTable.Append("<th>联系人</th>");
            sbTable.Append("<th>联系电话</th>");
            sbTable.Append("<th>押金金额</th>");
            sbTable.Append("<th>租金金额</th>");
            sbTable.Append("<th>缴费时间</th>");
            sbTable.Append("<th>合计</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"10\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder("select c.ClientCode,c.YYZZ_Name,b.InsuranceOrderId,b.CarOwner,b.CommercialPrice,b.CompulsoryPrice,b.TravelTaxPrice,a.Price,b.MerchantCommission,a.SubmitTime,a.PayTime,a.CancleTime,a.Status from  [dbo].[Order]  a ");
                sql.Append(" inner join [dbo].[OrderToCarInsure]  b on a.Id=b.Id ");
                sql.Append(" left join [dbo].[Merchant] c on a.MerchantId=c.Id ");

                sql.Append(" where 1=1 ");



                if (!string.IsNullOrEmpty(model.ClientCode))
                {
                    sql.Append(" and  b.ClientCode='" + model.ClientCode + "'");
                }
                if (model.StartTime != null)
                {
                    sql.Append(" and  a.SubmitTime >='" + CommonUtils.ConverToShortDateStart(model.StartTime.Value) + "'"); ;
                }
                if (model.EndTime != null)
                {
                    sql.Append(" and  a.SubmitTime <='" + CommonUtils.ConverToShortDateEnd(model.EndTime.Value) + "'");
                }

                sql.Append(" order by a.SubmitTime desc ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");
                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            case 12:
                                td_value = GetOrderStatusName(dtData.Rows[r][c].ToString().Trim());
                                break;
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "商户提现报表");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }


        private string GetWithdrawStatusName(string status)
        {
            if (status == null)
                return "未知";

            string name = "未知";
            if (status == "1")
            {
                name = "发起请求";
            }
            else if (status == "2")
            {
                name = "处理中";
            }
            else if (status == "3")
            {
                name = "成功";
            }
            else if (status == "4")
            {
                name = "失败";
            }

            return name;
        }

        private string GetOrderStatusName(string status)
        {
            if (status == null)
                return "未知";

            string name = "未知";
            if (status == "1")
            {
                name = "已提交";
            }
            else if (status == "2")
            {
                name = "跟进中";
            }
            else if (status == "3")
            {
                name = "待支付";
            }
            else if (status == "4")
            {
                name = "已完成";
            }
            else if (status == "5")
            {
                name = "已取消";
            }
            return name;
        }
    }
}