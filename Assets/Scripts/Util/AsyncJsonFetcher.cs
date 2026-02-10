using System;
using System.Collections;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Util
{
	public class AsyncJsonFetcher : MonoBehaviour
	{
		private string currentTaskUrl;
		
		public string FullContent { get; private set; }

		public void Fetch<T>(string url, Action<T> onSuccess, Action<Exception> onError,
			Action<float> onProgress = null)
			where T : class
		{
			if (!string.IsNullOrEmpty(currentTaskUrl)) throw new Exception($"Already fetching from {currentTaskUrl}");
			
			StartCoroutine(FetchCoroutine(url, onSuccess, onError, onProgress));
		}

		private IEnumerator FetchCoroutine<T>(string url, Action<T> onSuccess, Action<Exception> onError,
			Action<float> onProgress = null)
		{
			using var request = UnityWebRequest.Get(url);

			FullContent = null;

			currentTaskUrl = url;
			{
				var requestOperation = request.SendWebRequest();

				while (!requestOperation.isDone)
				{
					onProgress?.Invoke(request.downloadProgress);
					
					yield return null;
				}
			}
			currentTaskUrl = null;

			if (request.result != UnityWebRequest.Result.Success)
			{
				onError?.Invoke(new WebException(request.error));
				yield break;
			}

			try
			{
				var json = request.downloadHandler.text;
				FullContent = json;
				onSuccess?.Invoke(JsonConvert.DeserializeObject<T>(json));
			}
			catch (Exception e)
			{
				onError?.Invoke(e);
			}
		}
	}
}
