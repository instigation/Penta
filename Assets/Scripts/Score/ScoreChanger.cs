using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreChanger : MonoBehaviour{
    public GameObject __scoreObject;
    public GameObject __bestScoreObject;
    private int bestScore;
    private int scoreToChange = 0;
    private int trueScore; // When score rising animation is being played, presented score is different from the true score.
    private readonly int scoreChangeFrameCount = 30;
    private int increaseUnit;

    public void Start()
    {
        trueScore = GlobalInformation.getOrInitInt("score", 0);
        setScore(trueScore);
        bestScore = GlobalInformation.getOrInitInt("bestScore", 0);
        setBestScore(bestScore);
    }

    public void resetAndSave()
    {
        trueScore = 0;
        GlobalInformation.setInt("score", 0);
        setScore(0);
    }
    public void changeGradually(int amount)
    {
        trueScore += amount;
        if (scoreToChange > 0)
        {
            scoreToChange += amount;
            increaseUnit = scoreToChange / scoreChangeFrameCount;
            increaseUnit = increaseUnit > 1 ? increaseUnit : 1;
        }
        else
        {
            increaseUnit = amount / scoreChangeFrameCount;
            increaseUnit = increaseUnit > 1 ? increaseUnit : 1;
            StartCoroutine(changeGraduallyCoroutine(amount));
        }
    }
    public void saveScore()
    {
        GlobalInformation.setInt("score", trueScore);
    }
    public static void setSavedScoreToZero()
    {
        GlobalInformation.setInt("score", 0);
    }

    private IEnumerator changeGraduallyCoroutine(int amount) {
        scoreToChange = amount;
        while (scoreToChange > 0)
        {
            int score = getScore();
            int increaseAmount = increaseUnit <= scoreToChange ? increaseUnit : scoreToChange;
            score += increaseAmount;
            setScore(score);
            if (score > bestScore)
            {
                setBestScore(score);
            }
            scoreToChange -= increaseAmount;
            yield return null;
        }
        GlobalInformation.setInt("bestScore", bestScore);
        yield return null;
    }
    private int getScore() {
        // 형식이 안맞거나 32를 넘으면 위험할 수도
        string score = __scoreObject.GetComponent<Text>().text;
        return Convert.ToInt32(score);
    }
    private void setScore(int score) {
        __scoreObject.GetComponent<Text>().text = score.ToString();
    }
    private void setBestScore(int score)
    {
        bestScore = score;
        __bestScoreObject.GetComponent<Text>().text = "Best score: " + bestScore.ToString();
    }
}
