using RuoYi.Framework.Authorization;

namespace RuoYi.Admin.Authorization
{
    public class JwtHandler : AppAuthorizeHandler
    {
        public override async Task HandleAsync(AuthorizationHandlerContext context)
        {
            await Task.FromResult(true);
        }
    }
}