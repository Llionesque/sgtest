using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Configuration
{
    public abstract class AbstractExerciseController : MonoBehaviour
    {
        public abstract ExerciseConfig Config { get; }
        
        public event Action OnStarted;
        public event Action OnEnded;
        
        public virtual async Task InitialiseAsync(ExerciseConfig config) { }

        public virtual void Begin()
        {
            gameObject.SetActive(true);
            OnStarted?.Invoke();
        }

        public virtual void End()
        {
            gameObject.SetActive(false);
            OnEnded?.Invoke();
            OnEnded = null;
        }
    }
}
