using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataRetrieve.Helper
{
    public class Configurations
    {
        public static Configs _configurations;

        public static string ErrorMsg = "Unable to get {0} from Webconfig File. ErrorMsg：";
        public Configurations(IOptions<Configs> configurations)
        {
            _configurations = configurations.Value;
        }

        public string DSSConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.DSS;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "DSSConnectionString"), ex));
                }
            }
        }
        public string REGConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.REG;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "RegConnectionString"), ex));
                }
            }
        }

        public string AMDConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.AMD;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "AMDConnectionString"), ex));
                }
            }
        }

        public string EMRConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.EMR;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "EMRConnectionString"), ex));
                }
            }
        }

        public string MT5ConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.MT5;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "MT5ConnectionString"), ex));
                }
            }
        }

        public string FCEConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.FCE;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "FCEConnectionString"), ex));
                }
            }
        }

        public string SYSDATAConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.SYSDATA;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "SYSDATAConnectionString"), ex));
                }
            }
        }

        public string RISK_MT5ConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.RISK_MT5;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "RISK_MT5ConnectionString"), ex));
                }
            }
        }

        public string OAConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.OA;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "OAConnectionString"), ex));
                }
            }
        }
        public string CICConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.CIC;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "CICConnectionString"), ex));
                }
            }
        }

        public string CRMNETConnectionString
        {
            get
            {
                try
                {
                    return _configurations.connectionStrings.CRMNET;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "CRMNETConnectionString"), ex));
                }
            }
        }
    }

    public class Configs
    {
        public connectionStrings connectionStrings { get; set; }
        public Urls urls { get; set; }

        public string FTP { get; set; }
    }
    public class connectionStrings
    {
        public string DSS { get; set; }
        public string REG { get; set; }
        public string AMD { get; set; }
        public string EMR { get; set; }
        public string MT5 { get; set; }
        public string FCE { get; set; }
        public string SYSDATA { get; set; }
        public string RISK_MT5 { get; set; }
        public string OA { get; set; }
        public string CIC { get; set; }
        public string CRMNET { get; set; }
    }

    public class Urls
    {
        public string DEVSRVCRMWEB { get; set; }
        public string EGOPLUS { get; set; }
    }
}
