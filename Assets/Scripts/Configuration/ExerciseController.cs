using System;
using System.Threading.Tasks;

namespace Configuration
{
	public abstract class ExerciseController<T> : AbstractExerciseController
		where T : ExerciseConfig
	{
		protected T config { get; private set; }
		public override ExerciseConfig Config => config;

		public sealed override async Task InitialiseAsync(ExerciseConfig config)
		{
			await base.InitialiseAsync(config);
			
			if (config is not T typedConfig)
				throw new ArgumentNullException($"Cannot initialise exercise {GetType()} with config of type {config.GetType()}");
			
			this.config = typedConfig;
			
			await InitialiseAsyncInternal(typedConfig);
		}

		protected virtual async Task InitialiseAsyncInternal(T typedConfig){}
	}
}