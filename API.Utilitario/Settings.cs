using Microsoft.AspNetCore.Authorization;

namespace AppPromotora.Api.Utilitario
{
    public static class Settings
    {

        public static string Secret = "fedaf7d8863b48e197b9287d492b708e";

		public const string FMX = "FMX";
		public const string Client = "Cliente";
		public const string Refresh = "Refresh";
		public static AuthorizationPolicy AdminPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(FMX).Build();
		}
		public static AuthorizationPolicy UserPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Client).Build();
		}
		public static AuthorizationPolicy RefreshPolicy()
		{
			return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Refresh).Build();
		}

		
	}
}
