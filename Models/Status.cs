namespace lonefire.Models
{
    public class Status
    {
        private Status(string value) { Value = value; }

        public string Value { get; private set; }

        public static Status Submitted => new Status("Submitted");
        public static Status Approved => new Status("Approved");
        public static Status Rejected => new Status("Rejected");
        public static Status Public => new Status("Public");
        public static Status Hidden => new Status("Hidden");
        public static Status Private => new Status("Private");

        public override string ToString() { return Value; }

        public static implicit operator string(Status status) { return status.Value; }
    }

}