using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreChanger : MonoBehaviour{
    private GameObject scoreObject;

    public void Start()
    {
        scoreObject = gameObject;
        int score;
        if (GlobalInformation.contains("score"))
        {
            score = (int)GlobalInformation.getValue("score");
        }
        else
        {
            GlobalInformation.storeKeyValue("score", 0);
            score = 0;
        }
        setScore(score);
    }

    public IEnumerator changeGradually(int amount) {
        int originalScore = getScore();
        for(int score = originalScore; score <= originalScore + amount; score++) {
            setScore(score);
            yield return null;
        }
    }
    private int getScore() {
        // 형식이 안맞거나 32를 넘으면 위험할 수도
        string score = scoreObject.GetComponent<Text>().text;
        return Convert.ToInt32(score);
    }
    private void setScore(int score) {
        scoreObject.GetComponent<Text>().text = score.ToString();
    }
}
