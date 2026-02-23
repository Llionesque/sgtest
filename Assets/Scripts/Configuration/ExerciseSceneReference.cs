using UnityEngine;

namespace Configuration
{
    public class ExerciseSceneReference : MonoBehaviour
    {
        [SerializeField]
        private AbstractExerciseController exerciseController = null;
        public AbstractExerciseController Exercise => exerciseController;
    }
}
