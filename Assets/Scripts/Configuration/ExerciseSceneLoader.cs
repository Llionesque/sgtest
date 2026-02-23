using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Util;

namespace Configuration
{
    public static class ExerciseSceneLoader
    {
        public static async Task<AbstractExerciseController> LoadExercise(ExerciseConfig config) 
        {
            var sceneName = config.GetSceneName();
        
            await AsyncSceneLoader.LoadSceneAsync(sceneName, true);
        
            return FindExerciseInScene(sceneName);
        }

        private static AbstractExerciseController FindExerciseInScene(string sceneName)
        {
            foreach (var root in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
            {
                var sceneReference = root.GetComponentInChildren<ExerciseSceneReference>();
                if (sceneReference) return sceneReference.Exercise;
            }

            throw new Exception($"Couldn't find any '{typeof(ExerciseSceneReference)}' in the scene to start the exercise");
        }
    }
}
