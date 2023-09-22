using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServerHost.Quickstart.UI;

public class TestUsers
{
	public static List<TestUser> Users =>
	new List<TestUser>
	{
			new TestUser
			{
				SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
				Username = "John",
				Password = "JohnPassword",
				Claims = new List<Claim>
				{
					new Claim("given_name", "John"),
					new Claim("family_name", "Doe"),
					new Claim("address", "John Doe's Boulevard 323"),
					new Claim("role", "Administrator"),
					new Claim("country", "USA")
				}
			},
			new TestUser
			{
				SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
				Username = "Jane",
				Password = "JanePassword",
				Claims = new List<Claim>
				{
					new Claim("given_name", "Jane"),
					new Claim("family_name", "Doe"),
					new Claim("address", "Jane Doe's Avenue 214"),
					new Claim("role", "Visitor"),
					new Claim("country", "USA")
				}
			}
	 };
}
