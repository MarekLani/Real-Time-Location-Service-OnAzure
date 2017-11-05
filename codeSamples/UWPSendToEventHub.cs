private static string sasToken = null;
private static string serviceNamespace = "sbNamespace";
private static string hubName = "hubname";
private static string keyName = "SendKey";
private static string hubKey = "WnRH8AO2RHldDmqGon4EXIy8LKUWh5deVS2HLoLwr38=";
private static string publisherName = "companyId";
private static int tokenValidityDuration = 60 * 60; //hour
private static int tokenLastGenerated;

public static async Task CallEventHubHttpAsync(string payload)
{
	var baseAddress = new Uri(string.Format("https://{0}.servicebus.windows.net/", serviceNamespace));
	var url = baseAddress + string.Format("{0}/publishers/{1}/messages", hubName, publisherName);

	// Create client
	var httpClient = new HttpClient();

	if (sasToken == null || !isTokenValid())
	{
		sasToken = getToken(url);
		TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
		tokenLastGenerated = (int)sinceEpoch.TotalSeconds;
	}


	httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", sasToken);

	var content = new StringContent(payload, Encoding.UTF8, "application/json");
	content.Headers.Add("ContentType", "application/json");

	var response = await httpClient.PostAsync(url, content);
}

private static bool isTokenValid()
{
	var sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
	return ((tokenLastGenerated + tokenValidityDuration) < (int)sinceEpoch.TotalSeconds);
}