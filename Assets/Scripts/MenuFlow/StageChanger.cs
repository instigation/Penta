using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage { MENU, PUZZLE, SETTING, LEADERBOARD };

public class StageChanger : MonoBehaviour
{
    private Stage currentStage;
    private string[] correspondingTags = { "Menu", "PuzzleStage", "Setting", "Leaderboard" };
    public GameObject[][] gameObjects;
    private AdManager adMaker;

    void Awake()
    {
        currentStage = Stage.MENU;
        adMaker = new AdManager();
        adMaker.requestBanner();
        initGameObjects();
        for (int i=1; i<correspondingTags.Length; i++)
        {
            setGameObjectsWithStage((Stage)i, false);
        }
    }
    private void initGameObjects()
    {
        gameObjects = new GameObject[correspondingTags.Length][];
        for (int i = 0; i < correspondingTags.Length; i++)
        {
            gameObjects[i] = GameObject.FindGameObjectsWithTag(correspondingTags[i]);
        }
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
        GameObject[] objs = gameObjects[(int)stage];
        foreach (GameObject obj in objs)
        {
            obj.SetActive(isActive);
        }
    }
    private void setAds(Stage stage)
    {
        if (stage == Stage.PUZZLE)
            adMaker.hideBanner();
        else
            adMaker.showBanner();
    }
}
