namespace Entities.Exceptions
{
    public sealed class RefreshTokenBadRequestException : BadRequestException
    {
        public RefreshTokenBadRequestException() : base("Invalid client request. The tokenDto has some invalid values.")
        {
        }
    }
}