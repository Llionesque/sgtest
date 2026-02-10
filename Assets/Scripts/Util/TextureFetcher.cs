using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Util
{
    public class TextureFetcher : MonoBehaviour
    {
        private static readonly Dictionary<string, Texture2D> cache = new();

        [Header("UI Elements")]
        [SerializeField] 
        private GameObject placeholder = null;
        
        [SerializeField] 
        private RawImage textureContainer = null;
        
        [SerializeField] 
        private GameObject loadingIndicator = null;

        private string currentUrl = null;
        private Coroutine currentFetchOperation = null;

        public void FetchTextureAsync(string url, Action<Texture2D> onComplete = null, Action<Exception> onError = null)
        {
            if (cache.TryGetValue(url, out var cachedTexture))
            {
                HandleLoadingSuccess(url, cachedTexture, onComplete);
                return;
            }
            
            if (currentFetchOperation != null)
            {
                if (currentUrl == url)
                {
                    StartCoroutine(WaitForAlreadyFetchingTextureCoroutine(url, onComplete));
                    return;
                }
                else
                {
                    StopFetching();
                }
            }

            currentUrl = url;
            currentFetchOperation = StartCoroutine(FetchTextureCoroutine(url, onComplete, onError));
        }

        private IEnumerator FetchTextureCoroutine(string url, Action<Texture2D> onComplete, Action<Exception> onError)
        {
            HandleLoadingStarted();
            
            using var request = UnityWebRequestTexture.GetTexture(url);
            
            yield return request.SendWebRequest();
            
            StopFetching();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(new Exception(request.error));
                HandleLoadingFinished(false);
                
                yield break;
            }
            
            HandleLoadingSuccess(url, DownloadHandlerTexture.GetContent(request), onComplete);
        }

        private IEnumerator WaitForAlreadyFetchingTextureCoroutine(string url, Action<Texture2D> onComplete)
        {
            HandleLoadingStarted();
            
            yield return new WaitUntil(() => cache.ContainsKey(url));
            
            HandleLoadingSuccess(url, cache[url], onComplete);
        }

        private void HandleLoadingSuccess(string url, Texture2D texture, Action<Texture2D> onComplete)
        {
            if (textureContainer) textureContainer.texture = texture;
            
            cache.TryAdd(url, texture);
            HandleLoadingFinished(true);
            onComplete?.Invoke(texture);
        }
        
        private void HandleLoadingStarted()
        {
            if (placeholder) placeholder.SetActive(true);
            if (loadingIndicator) loadingIndicator.SetActive(true);
            if (textureContainer) textureContainer.gameObject.SetActive(false);
        }

        private void HandleLoadingFinished(bool success)
        {
            if (loadingIndicator) loadingIndicator.SetActive(false);
            if (placeholder) placeholder.SetActive(!success);
            if (textureContainer) textureContainer.gameObject.SetActive(success);
        }

        public void StopFetching()
        {
            if (currentFetchOperation != null)
            {
                StopCoroutine(currentFetchOperation);
                currentFetchOperation = null;
                currentUrl = null;
            }
        }
    }

}
