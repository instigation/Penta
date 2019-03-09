using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage { MENU, PUZZLE, SETTING, LEADERBOARD };

public class StageChanger : MonoBehaviour
{
    public GameObject[] allGameObjects;
    private Stage currentStage;
    private string[] correspondingTags = { "Menu", "PuzzleStage", "Setting", "Leaderboard" };
    public AdManager adManager;

    void Awake()
    {
        currentStage = Stage.MENU;
        adManager.requestBanner();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void toStage(Stage stage) { toStage((int)stage); }
    public void toStage(int stage)
    {
        setGameObjectsWithStage(currentStage, false);
        setGameObjectsWithStage((Stage)stage, true);
        setAds((Stage)stage);
        currentStage = (Stage)stage;
    }
    private void setGameObjectsWithStage(Stage stage, bool isActive)
    {
        foreach(GameObject obj in allGameObjects)
        {
            if (obj.tag == correspondingTags[(int)stage])
               obj.SetActive(isActive);
        }
    }
    private void setAds(Stage stage)
    {
        if (stage == Stage.PUZZLE)
            adManager.hideBanner();
        else
            adManager.showBanner();
    }
}
