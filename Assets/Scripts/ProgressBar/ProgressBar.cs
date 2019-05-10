﻿using System.Collections;
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
    public Transform __slider;
    public Transform __text;
    public Texture[] __progressImages;
    // Start is called before the first frame update
    void Start()
    {
        stage = GlobalInformation.getOrInitInt("stage", stage);
        subStage = 3;
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
    public void resetCurrentStage()
    {
        subStage = 3;
        stage--;
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
        if (isEnded())
            GlobalInformation.setInt("stage", stage);
    }
    private void updateTextAndBar()
    {
        __text.GetComponent<Text>().text = stage.ToString();
        __slider.GetComponent<RawImage>().texture = __progressImages[subStage];
    }
}
