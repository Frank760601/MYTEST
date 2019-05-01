using System;

namespace DataRetrieve.Model
{
    public class ScheduleMail
    {
        public string ProgCode { get; set; }
        public string EmpNo { get; set; }
        public string Body { get; set; } = "";
        public int BodyType { get; set; } = 0;
        public string FilePath { get; set; } = "";
        public string Charset { get; set; } = "utf-8";
    }
}
