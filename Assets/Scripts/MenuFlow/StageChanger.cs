using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Penta;

public enum Stage { MENU, PUZZLE, SETTING, LEADERBOARD };

public class StageChanger : MonoBehaviour
{
    // It uses gameobject name as identifier.
    public GameObject[] allGameObjects; // includes adRemoveButton
    public GameObject adRemoveButton;
    private Stage currentStage;
    private string[] correspondingTags = { "Menu", "PuzzleStage", "Setting", "Leaderboard" };
    public AdManager adManager;

    void Awake()
    {
        // MENU items are already activated in the fisrt place
        currentStage = Stage.MENU;
        if (adManager.isAdRemoved())
            setHide(adRemoveButton, true);
        setGameObjectsWithStage(Stage.PUZZLE, false);
        setGameObjectsWithStage(Stage.SETTING, false);
        setGameObjectsWithStage(Stage.LEADERBOARD, false);
        setGameObjectsWithStage(Stage.MENU, true);
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
            if ((obj.name == adRemoveButton.name) && adManager.isAdRemoved())
            {
                continue;
            }
            if (obj.tag == correspondingTags[(int)stage])
            {
                setHide(obj, !isActive);
            }
        }
    }
    private void setHide(GameObject obj, bool hide)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = (!hide) ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        }
        else
        {
            obj.SetActive(!hide);
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
