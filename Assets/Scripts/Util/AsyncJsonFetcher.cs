using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Util
{
	public class AsyncJsonFetch<T> where T : class
	{
		private string url;
		
		public string FullContent { get; private set; }

		public AsyncJsonFetch(string url)
		{
			this.url = url;
			FullContent = null;
		}

		public async Task<T> Fetch(Action<float> onProgress = null)
		{
			try
			{
				using var request = UnityWebRequest.Get(url);
				var operation = request.SendWebRequest();

				while (!operation.isDone)
				{
					onProgress?.Invoke(request.downloadProgress);
					await Task.Yield();
				}

				if (request.result != UnityWebRequest.Result.Success) 
					throw new WebException(request.error);

				return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
			}
			finally
			{
				url = null;
			}
		}
	}
}
