namespace RegAPP.Models
{
    public class DataDictionary
    {

    }
    public class Office
    {
        public string Officename { get; set; }
        public string License { get; set; }
    }

    public class OfficeVersion
    {
        public int OfficeId { get; set; }
        public string Version { get; set; }
    }

    public class Enrolment
    {
        public string MachineCode { get; set; }
        public string AuthorizationCode { get; set; }
    }

    public class Users
    {
        public string Officename { get; set; }
        public string License { get; set; }
    }

    public class MachineInfo
    {
        public string Officename { get; set; }
        public string MachineCode { get; set; }
        public string Version { get; set; }
    }
    public class UpdataMachine
    {
        public string Officename { get; set; }
        public string License { get; set; }
        public string MachineCode { get; set; }
        public string Version { get; set; }
    }
}