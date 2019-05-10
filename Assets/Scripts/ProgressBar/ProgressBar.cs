using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    // stage: 0~, subStage: 0~3
    // initial: 0-3(menu) => 1-0(puzzlestage)
    // end: subStage 3
    // start: subStage 0
    // end to start: subStage 3->0, stage++. 
    // progressByOne: start->1->2->end->start->...
    public int __stage;
    public int __subStage;
    public Transform __slider;
    public Transform __text;
    public Texture[] __progressImages;
    // Start is called before the first frame update
    void Start()
    {
        __stage = GlobalInformation.getOrInitInt("stage", __stage);
        __subStage = 3;
        updateTextAndBar();
    }
    public bool isEnded()
    {
        return __subStage == 3;
    }
    public void progressByOne()
    {
        if (isEnded())
        {
            advanceStage();
        }
        else
        {
            advanceSubStage();
        }
    }
    public void resetCurrentStage()
    {
        __subStage = 3;
        __stage--;
        updateTextAndBar();
    }
    public int getStage()
    {
        return __stage;
    }
    public int getSubStage()
    {
        return __subStage;
    }
    private void advanceStage()
    {
        __stage++;
        __subStage = 0;
        updateTextAndBar();
    }
    private void advanceSubStage()
    {
        __subStage += 1;
        updateTextAndBar();
        if (isEnded())
            GlobalInformation.setInt("stage", __stage);
    }
    private void updateTextAndBar()
    {
        __text.GetComponent<Text>().text = __stage.ToString();
        __slider.GetComponent<RawImage>().texture = __progressImages[__subStage];
    }
}
