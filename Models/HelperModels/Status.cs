using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models.HelperModels
{
    public class Status
    {
        private Status(string value) { Value = value; }

        public string Value { get; private set; }

        /// <summary>
        /// returns All valid status list
        /// </summary>
        public static Status[] StatusList => new Status[]{ Submitted, Approved, Rejected, Public, Hidden, Private };

        public static Status Submitted => new Status("Submitted");
        public static Status Approved => new Status("Approved");
        public static Status Rejected => new Status("Rejected");
        public static Status Public => new Status("Public");
        public static Status Hidden => new Status("Hidden");
        public static Status Private => new Status("Private");
        public static Status Unknown => new Status("Unknown");

        public override string ToString() { return Value; }

        /// <summary>
        /// Conver string to Status, returns Unknown if not found in status list
        /// </summary>
        public static Status From(string str)
        {
            var index = Array.FindIndex(StatusList, s => s.Value == str);
            if (index != -1)
            {
                return StatusList[index];
            }
            return Unknown;
        }

        public static implicit operator string(Status status) { return status.Value; }
    }

}