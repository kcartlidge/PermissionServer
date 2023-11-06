namespace PermissionServer
{
    public enum ConfirmationStatus
    {
        Unknown = 0,
        Okay,
        NoTokenFound,
        DoesNotMatchToken,
        HasExpired,
    }
}
