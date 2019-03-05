using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    // stage: 1~, subStage: 0~3
    // end: subStage 3
    // start: subStage 0
    // end to start: subStage 3->0, stage++. 
    // progressByOne: start->1->2->end->start->...
    private int stage;
    private int subStage;
    private Transform slider;
    private Transform text;
    public Texture[] __progressImages;
    // Start is called before the first frame update
    void Start()
    {
        if (GlobalInformation.contains("stage"))
        {
            stage = (int)GlobalInformation.getValue("stage");
            subStage = (int)GlobalInformation.getValue("subStage");
        }
        else
        {
            stage = 1;
            GlobalInformation.storeKeyValue("stage", stage);
            subStage = 0;
            GlobalInformation.storeKeyValue("subStage", subStage);
        }
        slider = gameObject.transform.GetChild(1);
        text = gameObject.transform.GetChild(2);
        updateTextAndBar();
    }
    public bool isEnded()
    {
        return subStage == 3;
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
    public void resetStage()
    {
        subStage = 0;
        updateTextAndBar();
    }
    public int getStage()
    {
        return stage;
    }
    private void advanceStage()
    {
        stage++;
        subStage = 0;
        updateTextAndBar();
    }
    private void advanceSubStage()
    {
        subStage += 1;
        updateTextAndBar();
    }
    private void updateTextAndBar()
    {
        text.GetComponent<Text>().text = stage.ToString();
        slider.GetComponent<RawImage>().texture = __progressImages[subStage];
    }
}
