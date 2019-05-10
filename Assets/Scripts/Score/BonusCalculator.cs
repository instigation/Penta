using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusCalculator : MonoBehaviour
{
    private bool isLastInsertionCorrect;
    private bool isStreakOccured;
    private bool isCorrectInsertionOccured;
    public int __streak;
    public ProgressBar __progressBar;
    public GameObject __bonusText;
    public GameObject __normalParticle;
    public GameObject __specialParticle;
    public GameObject __canvas;
    private const int correctSoundIndex = 0, pentaCorrectSoundIndex = 1;

    void Start()
    {
        isLastInsertionCorrect = false;
        isStreakOccured = false;
        isCorrectInsertionOccured = false;
        __streak = 0;
        //combo = 0;
    }
    public void calculateOnInsertion(InsertionResult insertionResult)
    {
        switch (insertionResult)
        {
            case InsertionResult.CORRECT:
                isCorrectInsertionOccured = true;
                if (isLastInsertionCorrect)
                {
                    __streak++;
                    isStreakOccured = true;
                }
                else
                    isStreakOccured = false;
                isLastInsertionCorrect = true;
                break;
            case InsertionResult.WRONG:
                isCorrectInsertionOccured = false;
                __streak = 0;
                isStreakOccured = false;
                isLastInsertionCorrect = false;
                break;
            case InsertionResult.MISS:
                isCorrectInsertionOccured = false;
                isStreakOccured = false;
                break;
        }
    }
    public float getBonusTime()
    {
        return isStreakOccured ? 1 : 0;
    }
    public int getBonusScore()
    {
        return isCorrectInsertionOccured ? __progressBar.getStage() * (__streak + 1) : 0;
    }
    public void playBonusText(Vector2 position)
    {
        if (isCorrectInsertionOccured)
        {
            GameObject ret = Instantiate(__bonusText);
            ret.transform.SetParent(__canvas.transform, false);
            UnityUtils.moveUIElementToPosition(ret, position);
            ret.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = getScoreText();
            ret.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = getComboText();
            if ((__streak % 5 == 0) && (__streak > 0))
                ret.GetComponents<AudioSource>()[pentaCorrectSoundIndex].Play(0);
            else
                ret.GetComponents<AudioSource>()[correctSoundIndex].Play(0);
        }
    }
    private string getComboText()
    {
        string ret; //= "\n+" + getBonusScore().ToString();
        if (__streak != 0)
            ret = __streak.ToString() + " Combo!";// + ret;
        else ret = "";
        //if (combo != 0)
        //    ret = "Fast! x" + combo.ToString() + ret;
        return ret;
    }

    private string getScoreText()
    {
        string ret = "+" + getBonusScore().ToString();
        return ret;
    }
    public void playBonusParticle(Vector2 position)
    {
        if (isCorrectInsertionOccured)
        {
            if ((__streak % 5 == 0) && (__streak > 0))
                playParticle(__specialParticle, position);
            else
                playParticle(__normalParticle, position);
        }
    }
    private void playParticle(GameObject particle, Vector2 position)
    {
        UnityUtils.moveUIElementToPosition(particle, position);
        particle.GetComponent<ParticleSystem>().Play();
    }
}

