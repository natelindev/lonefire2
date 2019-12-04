using System;
namespace lonefire.Data
{
    public static class Constants
    {
        public const string GetOperationName = "Get";
        public const string PostOperationName = "Post";
        public const string PatchOperationName = "Patch";
        public const string PutOperationName = "Put";
        public const string DeleteOperationName = "Delete";
        public const string CensorOperationName = "Censor";

        public const string base64UrlRegex = @"^[A-Za-z0-9-_]{21}[AEIMQUYcgkosw048]$";

        public const string AdminName = "Admin";
        public const string EmptyUserName = "EmptyUser";

        public const string AdministratorsRole = "Administrator";
    }
}
