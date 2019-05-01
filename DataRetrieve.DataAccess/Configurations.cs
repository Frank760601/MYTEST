using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataRetrieve.DataAccess
{
    public class Configurations
    {
        private static IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

        private static IConfigurationRoot Configuration = builder.Build();

        public static string ErrorMsg = "Unable to get {0} from Webconfig File. ErrorMsg：";
        public static string DSSConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:DSS"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "DSSConnectionString"),ex));
                }
            }
        }

        public static string REGConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:REG"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "RegConnectionString"), ex));
                }
            }
        }

        public static string AMDConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:AMD"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "AMDConnectionString"), ex));
                }
            }
        }

        public static string EMRConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:EMR"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "EMRConnectionString"), ex));
                }
            }
        }

        public static string MT5ConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:MT5"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "MT5ConnectionString"), ex));
                }
            }
        }

        public static string FCEConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:FCE"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "FCEConnectionString"), ex));
                }
            }
        }

        public static string SYSDATAConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:SYSDATA"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "SYSDATAConnectionString"), ex));
                }
            }
        }

        public static string RISK_MT5ConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:RISK_MT5"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "RISK_MT5ConnectionString"), ex));
                }
            }
        }

        public static string OAConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:OA"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "OAConnectionString"), ex));
                }
            }
        }
        public static string CICConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:CIC"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "CICConnectionString"), ex));
                }
            }
        }

        public static string CRMNETConnectionString
        {
            get
            {
                try
                {
                    return Configuration["connectionStrings:CRMNET"];
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Concat(string.Format(ErrorMsg, "CRMNETConnectionString"), ex));
                }
            }
        }
    }
}