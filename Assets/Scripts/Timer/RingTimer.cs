using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingTimer : MonoBehaviour
{
    public float __maxTime = 30.0F;
    public PuzzleStageController __puzzleStageController;
    private float leftoverTime;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponent<Image>();
        leftoverTime = __maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        leftoverTime -= Time.deltaTime;
        if (leftoverTime <= 0.0F)
            __puzzleStageController.resetStage();
        image.fillAmount = leftoverTime / __maxTime;
    }

    private void OnEnable()
    {
        leftoverTime = __maxTime;
    }
}
