using Cookbook_1.Contracts;

namespace Cookbook_1.Models
{
    public partial class JwtToken
    {
        public JwtTokenVm ToJwtTokenVm()
        {
            return new JwtTokenVm(UserId, Token, ExpiresAt);
        }
    }
}
