using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class MainScreen : MonoBehaviour
{
    [Header("Test Exercises")]
    [SerializeField]
    private ExerciseController[] exercises = null;
    
    [Header("UI Elements")]
    [SerializeField]
    private GameObject buttonPrefab = null;
    
    [SerializeField]
    private Transform buttonsContainer = null;
    
    [SerializeField]
    private TextMeshProUGUI exerciseLabel = null;

    [SerializeField]
    private Button quitButton = null;
    
    [Header("Backgrounds")]
    [SerializeField]
    private CanvasGroupFader exerciseLabelFader = null;

    [SerializeField]
    private Image backgroundImage = null;
    
    [SerializeField]
    private Image globalBackground = null;
    
    [SerializeField]
    private CanvasGroupFader backgroundFader = null;
    
    private ExerciseController currentExercise;

    private void Awake()
    {
        foreach (var exercise in exercises)
        {
            exercise.gameObject.SetActive(false);
            exercise.OnEnded += () =>
            {
                if (backgroundImage.gameObject.activeSelf) backgroundFader.FadeOut();
                ReturnToMainScreen();
            };
            
            CreateButtonForExercise(exercise);
        }
        
        quitButton.onClick.AddListener(() => currentExercise.End());
    }

    private void Start()
    {
        ReturnToMainScreen();
    }

    private void CreateButtonForExercise(ExerciseController exercise)
    {
        var newButton = Instantiate(buttonPrefab, buttonsContainer).GetComponent<Button>();
        newButton.onClick.AddListener(() => GotoExercise(exercise));
        
        var label = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label) label.text = exercise.Title;

        var image = newButton.transform.Find("Globe/Mask/Icon").GetComponent<Image>();
        if (image) image.sprite = exercise.Icon;
        
        var background = newButton.transform.Find("Background").GetComponent<Image>();
        if (background)
        {
            background.gameObject.SetActive(true);
            background.color = exercise.Color;
        }
        
        newButton.gameObject.name = exercise.Title;
        newButton.gameObject.SetActive(true);
    }

    private void GotoExercise(ExerciseController exercise)
    {
        gameObject.SetActive(false);

        exerciseLabel.text = exercise.Title;
        exerciseLabelFader.FadeIn();
        
        backgroundImage.overrideSprite = exercise.Background;
        if (backgroundImage.overrideSprite)
        {
            backgroundImage.gameObject.SetActive(true);
            backgroundFader.FadeIn();
        }
        else
        {
            backgroundImage.gameObject.SetActive(false);
            globalBackground.gameObject.SetActive(false);
        }
        
        currentExercise = exercise;
        quitButton.gameObject.SetActive(true);
        exercise.Begin();
    }

    private void ReturnToMainScreen()
    {
        gameObject.SetActive(true);
        exerciseLabelFader.FadeOut();
        currentExercise = null;
        quitButton.gameObject.SetActive(false);
        globalBackground.gameObject.SetActive(true);
    }
}
