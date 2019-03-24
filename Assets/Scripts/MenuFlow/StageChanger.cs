using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        setGameObjectsWithStage(Stage.PUZZLE, false);
        setGameObjectsWithStage(Stage.SETTING, false);
        setGameObjectsWithStage(Stage.LEADERBOARD, false);
        setGameObjectsWithStage(Stage.MENU, true);
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
        // Unity official best practice says I should disable CanvasRenderer.
        // But unlike Renderer, CanvasRenderer has no enabled property.
        // Also, adjusting alpha don't work because hovering the cursor reveals the hidden object.
        // So just used localScale method instead.
        foreach(GameObject obj in allGameObjects)
        {
            if (obj.tag == correspondingTags[(int)stage])
            {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = isActive ? new Vector3(1,1,1) : new Vector3(0,0,0);
                }
                else
                {
                    obj.SetActive(isActive);
                }

            }
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
