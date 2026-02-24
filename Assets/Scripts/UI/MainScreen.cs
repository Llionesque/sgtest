using System;
using Configuration;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class MainScreen : MonoBehaviour
{
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
    
    [Header("Loading")]
    [SerializeField]
    private GameObject loadingPanel = null;
    
    private AbstractExerciseController currentExercise;

    private void Awake()
    {
        foreach (var config in ExerciseConfig.LoadAll())
        {
            CreateButtonForConfig(config);
        }
        
        quitButton.onClick.AddListener(() => currentExercise.End());
    }

    private void Start()
    {
        ReturnToMainScreen();
    }

    private void CreateButtonForConfig(ExerciseConfig config)
    {
        Instantiate(buttonPrefab, buttonsContainer)
            .GetComponent<ExerciseButton>()
            .Configure(config, LoadAndStartExercise,
                HandleUnitTestStarted, HandleUnitTestEnded);
    }

    private async void LoadAndStartExercise(ExerciseConfig config)
    {
        gameObject.SetActive(false);
        loadingPanel.SetActive(true);

        exerciseLabel.text = config.FullTitle;
        exerciseLabelFader.FadeIn();
        
        backgroundImage.overrideSprite = config.Background;
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

        try
        {
            currentExercise = await ExerciseSceneLoader.LoadExercise(config);

            await currentExercise.InitialiseAsync(config);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            loadingPanel.SetActive(false);
        }
        
        quitButton.gameObject.SetActive(true);
        
        currentExercise.Begin();
        currentExercise.OnEnded += HandleCurrentExerciseEnded;
    }

    private async void HandleCurrentExerciseEnded()
    {
        if (backgroundImage.gameObject.activeSelf) backgroundFader.FadeOut();
        
        loadingPanel.SetActive(true);

        try
        {
            await AsyncSceneLoader.UnloadSceneAsync(currentExercise.Config.GetSceneName());
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            ReturnToMainScreen();
        }
    }

    private void ReturnToMainScreen()
    {
        if (currentExercise)
        {
            currentExercise.OnEnded -= HandleCurrentExerciseEnded;
            currentExercise = null;
        }
        
        gameObject.SetActive(true);
        loadingPanel.SetActive(false);
        exerciseLabelFader.FadeOut();
        quitButton.gameObject.SetActive(false);
        globalBackground.gameObject.SetActive(true);
    }

#region Unit testing
    private void HandleUnitTestStarted() => loadingPanel.SetActive(true);
    private void HandleUnitTestEnded(bool pass) => loadingPanel.SetActive(false);
#endregion
}
