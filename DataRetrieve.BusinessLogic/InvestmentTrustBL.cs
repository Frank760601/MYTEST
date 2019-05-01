using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using DataRetrieve.DataAccess;
using PagedList.Core;

namespace DataRetrieve.BusinessLogic
{
    public class InvestmentTrustBL
    {
        private static InvestmentTrustBL uniqueInstance;
        public static InvestmentTrustBL getInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new InvestmentTrustBL();
            return uniqueInstance;
        }
        public dynamic Query(string NO, string NAME, string IDNO, int index = 1, int PageSize = 0)
        {
            int count = 0;
            dynamic model = new ExpandoObject();

            var tempmodel = InvestmentTrustDA.getInstance().GetInvestmentTrust(NO, NAME, IDNO, (index - 1) * PageSize, PageSize);
            count = tempmodel.Count() > 0 ? tempmodel.FirstOrDefault().TOTALCNT : 0;

            model.TRUST = new StaticPagedList<dynamic>(tempmodel, index, PageSize, count);
            model.TRUSTHEAD = tempmodel.Count() > 0 ? (tempmodel.FirstOrDefault() as IDictionary<string, object>).Keys.ToList() : new List<string>();
            return model;
        }
        public (bool MsgCode, string MsgName) UpdateInvestmentTrust(string NO, string NAME, string IDNO)
        {
            var result = InvestmentTrustDA.getInstance().UpdateInvestmentTrust(NO, NAME, IDNO);

            return result ? (result, "成功") : (result, "失敗");
        }
        public (bool MsgCode, string MsgName) InsertInvestmentTrust(string NO, string NAME, string IDNO)
        {
            var result = InvestmentTrustDA.getInstance().InsertInvestmentTrust(NO, NAME, IDNO);

            return result;
        }
        public (bool MsgCode, string MsgName) DeleteInvestmentTrust(string NO)
        {
            var result = InvestmentTrustDA.getInstance().DeleteInvestmentTrust(NO);

            return result ? (result, "成功") : (result, "失敗");
        }
    }
}
