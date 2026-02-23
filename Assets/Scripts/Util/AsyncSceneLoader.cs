using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public static class AsyncSceneLoader
    {
        public static async Task LoadSceneAsync(string sceneName, bool additive = false, Action onComplete = null)
        {
            if (!additive && SceneManager.GetSceneByName(sceneName).isLoaded)
                throw new Exception($"Scene: '{sceneName}' is already loaded");
            
            var operation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            
            if (operation == null)
                throw new Exception($"Could not load scene: '{sceneName}'");
            
            operation.allowSceneActivation = true;

            await operation;
            
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                throw new Exception($"Scene '{sceneName}' failed to load");
            
            onComplete?.Invoke();
        }

        public static async Task UnloadSceneAsync(string sceneName, Action onComplete = null)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded) return;
            await SceneManager.UnloadSceneAsync(sceneName);
            onComplete?.Invoke();
        }
    }
}
