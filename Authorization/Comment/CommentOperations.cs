using lonefire.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace lonefire.Authorization
{
    public static class CommentOperations
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
    }
}