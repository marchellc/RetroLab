namespace RetroLab.API.Authentification
{
    public enum AuthTokenValidationResult
    {
        Ok,

        MismatchedID,
        MismatchedIP,
        MismatchedUsage,

        Expired,

        Invalid
    }
}