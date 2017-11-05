package sk.infotech.celeng.spinservice.webCommunication;

import android.util.Base64;
import android.util.Log;

import com.google.gson.JsonArray;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONTokener;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import sk.infotech.celeng.spinservice.items.WebServiceItem;




public class JSONsender {

	private static final String TAG = "JSONsender";


	public static List<WebServiceItem> sendJsonPost(String url, JsonArray data) {
		final JsonArray finalData = data;
		
		final ArrayList<WebServiceItem>[] resultArray = (ArrayList<WebServiceItem>[]) new ArrayList[1];
		HttpURLConnection httpcon;
		String result;
		try {
			//Connect to EventHub
			final int connectionTimeout = 20000; //20 sec
			final int readTimeout = 20000; //20 sec
			httpcon = (HttpURLConnection) ((new URL(url).openConnection()));
			httpcon.setDoOutput(true);
			httpcon.setConnectTimeout(connectionTimeout);
			httpcon.setReadTimeout(readTimeout);
			httpcon.setRequestProperty("Content-Type", "application/json");
			httpcon.setRequestProperty("Accept", "application/xml; charset=utf-8");
			httpcon.setRequestProperty("Authorization", "SharedAccessSignature sr=*"); //token goes here
			httpcon.setRequestMethod("POST");
			httpcon.connect();

			//Write message
			OutputStream os = httpcon.getOutputStream();
			BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
			writer.write(finalData.toString());
			writer.close();
			os.close();
			
			//Process response from event hub
			int code = httpcon.getResponseCode();
			if (code == 200){
				  resultArray[0].add(new WebServiceItem(null, 1, null));
			}else{
				  resultArray[0].add(new WebServiceItem(null, 0, null));
			}
			//Read
			BufferedReader br = new BufferedReader(new InputStreamReader(httpcon.getInputStream(), "UTF-8"));
			String line;
			StringBuilder sb = new StringBuilder();

			while ((line = br.readLine()) != null) {
				sb.append(line);
			}

			br.close();
			result = sb.toString();

		} catch (IOException e) {
			e.printStackTrace();
		} catch (JSONException e) {
			e.printStackTrace();
		}
			
		return resultArray[0];
	}
}
