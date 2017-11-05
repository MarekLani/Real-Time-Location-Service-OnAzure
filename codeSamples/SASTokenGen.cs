private static string createToken(string resourceUri)
{
	TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
	int duration = {some int};
	var expiry = Convert.ToString((int)sinceEpoch.TotalSeconds + duration);
	string stringToSign = Uri.EscapeDataString(resourceUri) + "\n" + expiry;
	
	HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
	
	//Create hash and convert to string
	var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
	
	//create SAS token using computed hash
	var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", Uri.EscapeDataString(resourceUri), Uri.EscapeDataString(signature), expiry, keyName);
	
	return sasToken;
}