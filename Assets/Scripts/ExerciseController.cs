using System;
using UnityEngine;

public abstract class ExerciseController : MonoBehaviour
{
	public abstract string Title { get; }
	
	[Header("Exercise metaData")]
	[SerializeField]
	private Sprite icon = null;
	public Sprite Icon => icon;

	[SerializeField]
	private Sprite background = null;
	public Sprite Background => background;
	
	[SerializeField]
	private Color color = Color.white;
	public Color Color => color;

	public event Action OnStarted;
	public event Action OnEnded;

	public virtual void Begin()
	{
		gameObject.SetActive(true);
		OnStarted?.Invoke();
	}

	public virtual void End()
	{
		gameObject.SetActive(false);
		OnEnded?.Invoke();
	}
}