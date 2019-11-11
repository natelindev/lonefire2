using Microsoft.AspNetCore.Authorization.Infrastructure;
using lonefire.Data;

namespace lonefire.Authorization
{
    public static class ArticleOperations
    {
        public static OperationAuthorizationRequirement Get =
          new OperationAuthorizationRequirement { Name = Constants.GetOperationName };
        public static OperationAuthorizationRequirement Post =
          new OperationAuthorizationRequirement { Name = Constants.PostOperationName };
        public static OperationAuthorizationRequirement Patch =
          new OperationAuthorizationRequirement { Name = Constants.PatchOperationName };
        public static OperationAuthorizationRequirement Put =
          new OperationAuthorizationRequirement { Name = Constants.PutOperationName };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Censor =
          new OperationAuthorizationRequirement { Name = Constants.CensorOperationName };
    }
}