namespace IdentityServer.ViewModels
{
    public class ExternalSecretsModel
    {
        public const string Facebook = "ExternalSecrets:Facebook";

        public string Id { get; set; }

        public string Secret { get; set; }
    }
}
