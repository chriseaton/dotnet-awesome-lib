/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Net;
using System.Web;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Awesome.Library.Utilities {

	public static class Website {

		/// <summary>
		/// Sends specified data to url using POST or GET; returns server response
		/// </summary>
		/// <param name="data">ex: "var1=data&amp;var2=more data"</param>
		/// <param name="URL">URL to the site</param>
		/// <param name="method">POST or GET</param>
		/// <param name="contentType">ex: application/x-www-form-urlencoded</param>
		/// <returns>Value from webstite (Full text including HTML)</returns>
		public static string SendData(string data, string URL, string method, string contentType) {
			//clear previous
			string returnStr="";
			//send data
			byte[] dataBytes=System.Text.Encoding.ASCII.GetBytes(data);//byte conversion of string
			try {
				HttpWebResponse res;
				HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(URL); 
				req.Method=method;
				req.ContentType = contentType; 
				req.ContentLength = dataBytes.Length;
				Stream reqOut = req.GetRequestStream();
				reqOut.Write(dataBytes,0,dataBytes.Length);
				reqOut.Close();
				res=(HttpWebResponse)req.GetResponse();
				StreamReader sr = new StreamReader(res.GetResponseStream());
				returnStr = sr.ReadToEnd();
			} catch {
				//new PDXError(t).ShowDialog();
			}
			return returnStr;
		}

		/// <summary>
		/// Returns page html in plain text from the specified url
		/// </summary>
		/// <param name="url">The website url to grab the html from</param>
		/// <returns>Specified webpage HTML</returns>
		public static string GetHTML(string url) {
			string retVal = "";
			try {
				WebClient client = new WebClient();
				byte[] buf = client.DownloadData(url);//download from webpage
				retVal =  System.Text.Encoding.ASCII.GetString(buf,0,buf.Length);
			} catch {}
			return retVal;
		}

		/// <summary>
		/// Returns data (binary) from the specified url
		/// </summary>
		/// <param name="url">The website url to grab the data from</param>
		/// <returns>Specified webpage HTML</returns>
		public static byte[] GetData(string url) {
			byte[] retVal = null;
			try {
				WebClient client = new WebClient();
				retVal = client.DownloadData(url);//download from webpage
			} catch {}
			return retVal;
		}

	}
}
