namespace Cookbook_1.Contracts
{
    public record JwtTokenVm (int UserId, string Token, DateTime ExpiresAt);
}
