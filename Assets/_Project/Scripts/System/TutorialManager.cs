using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tutorial
{
    public string id;
    public GameObject canvas;
    public bool closeOtherCanvases = true;
    public float delayBeforeStart = 0f;
    private bool started = false;
    public bool HasAlreadyStarted() => started;
    public void SetAsStarted() => started = true;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private List<Tutorial> tutorials;

    private Dictionary<string, Tutorial> tutorialsByName = new();
    private Tutorial currentTutorial;
    private bool startTutorial = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        foreach (var tutorial in tutorials)
        {
            tutorialsByName[tutorial.id] = tutorial;
            if (tutorial.canvas != null)
            {
                GameObject canvas = Instantiate(tutorial.canvas, Vector3.one, Quaternion.identity);
                tutorial.canvas = canvas;
                tutorial.canvas.SetActive(false);
            }
        }
    }

    public void LaunchTutorial(string id)
    {
        if (!startTutorial || !tutorialsByName.ContainsKey(id)) return;
        Tutorial tutorial = tutorialsByName[id];
        if (tutorial.HasAlreadyStarted()) return;
        StartCoroutine(StartTutorialWithDelay(tutorial));
    }

    private IEnumerator StartTutorialWithDelay(Tutorial tutorial)
    {
        yield return new WaitForSeconds(tutorial.delayBeforeStart);

        if (tutorial.closeOtherCanvases) GameManager.Instance.CloseCurrentCanvas();
        GameManager.Instance.PositionCanvas(tutorial.canvas, 1.2f);
        tutorial.canvas.SetActive(true);
        tutorial.SetAsStarted();
        currentTutorial = tutorial;
    }

    public bool HasCurrentTutorialEnded()
    {
        if (currentTutorial == null) return true;
        else return HasTutorialEnded(currentTutorial.id);
    }

    public bool HasTutorialEnded(string id)
    {
        if (!startTutorial || !tutorialsByName.ContainsKey(id)) return true;
        TutorialCanvas tutorialCanvas = tutorialsByName[id].canvas.GetComponent<TutorialCanvas>();
        if (tutorialCanvas == null) return true;
        return tutorialCanvas.HasEnded();
    }

    public bool IsCurrentTutorialActive()
    {
        if (currentTutorial == null) return false;
        else return IsTutorialActive(currentTutorial.id);
    }

    public bool IsTutorialActive(string id)
    {
        if (!startTutorial || !tutorialsByName.ContainsKey(id)) return false;
        return tutorialsByName[id].canvas.activeInHierarchy;
    }

    public void CloseCurrentTutorialCanvas()
    {
        if (currentTutorial != null)
        {
            currentTutorial.canvas.SetActive(false);
            currentTutorial = null;
        }
    }

    public void SetStartTutorial(bool start) => startTutorial = start;
    public bool GetStartTutorial() => startTutorial;
}