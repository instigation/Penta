using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // 8.5~: 파란색 7.5~8.5: 중간 ~7.5: 빨간색 
    private readonly float firstThreshold = 8.5f;
    private readonly float secondThreshold = 7.5f;
    private readonly float middleThreshold = 8.0F;
    // fillArea를 조절해서 fill의 width를 조절한다
    // fillArea는 anchorMin,Max의 y가 0.25~0.75일떄가 100%, 0~1일때가 200%다
    private Slider timeSlider;
    private Transform fill;
    private Transform fillArea;
    private AudioSource notMuchTimeSound;
    private readonly float maxTimeInSecond = 30;
    private readonly float timeChangeDurationInSecond = 0.2F;
    // value보다 정확한 버전이다. value만 자꾸 바꿔쓰면 오차 많아질까봐 (deltaTime쓰니)
    // 그래서 value는 leftoverTimeInSecond로 update만 하고 읽지 않는다 
    // leftoverTimeInSecond는 음수일 수도 있다.
    private float leftoverTimeInSecond;
    private float deltaTime;
    private bool isPaused = true;
    // 1초가 지나면 다시 AudioSource를 Play할 수 있게 한다.
    private readonly float audioSourceDelay = 1.0F;
    private float timeAfterPlayedAudioSourceInSecond = 0.0F;
    private bool inAudioSourceDelay = false;
    private float timeToChangeInSecond = 0.0f;

    //타이머 색깔
    private Color sky, scarlet;
    // Start is called before the first frame update
    void Start()
    {
        timeSlider = gameObject.GetComponent<Slider>();
        leftoverTimeInSecond = maxTimeInSecond*timeSlider.value;
        fillArea = timeSlider.transform.GetChild(1);
        fill = fillArea.GetChild(0);
        notMuchTimeSound = gameObject.GetComponent<AudioSource>();

        sky = new Color(0, 0.706f, 1);
        scarlet = new Color(1, 0.204f, 0.349f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            deltaTime = Time.deltaTime;
            changeTime(-deltaTime);
            playAudioSourceIfNotMuchTimeLeft();
        }
    }

    public void refillTime()
    {
        manuallyChangeTime(maxTimeInSecond - leftoverTimeInSecond);
    }

    public void manuallyChangeTime(float second)
    {
        const float epsilon = 0.02f;
        if (timeToChangeInSecond >= epsilon)
        {
            timeToChangeInSecond += second;
        }
        else
            StartCoroutine(manuallyChangeTimeCoroutine(second));
    }

    public IEnumerator manuallyChangeTimeCoroutine(float second)
    {
        float timeChangeSpeedFactor = second / timeChangeDurationInSecond;
        timeToChangeInSecond = second;
        while(timeToChangeInSecond > deltaTime*timeChangeSpeedFactor)
        {
            changeTime(deltaTime*timeChangeSpeedFactor);
            timeToChangeInSecond -= deltaTime*timeChangeSpeedFactor;
            yield return null;
        }
        if (timeToChangeInSecond >= 0)
        {
            changeTime(timeToChangeInSecond);
            timeToChangeInSecond = 0.0f;
        }
        else
            Debug.Log("timeLeftNegativeError");
        yield return null;
    }

    public float getLeftoverTimeInSecond()
    {
        return leftoverTimeInSecond;
    }

    public void pause() { isPaused = true; }

    public void run() { isPaused = false; }

    private void changeTime(float deltaTimeInSecond)
    {
        timeSlider.value += deltaTimeInSecond / maxTimeInSecond;
        leftoverTimeInSecond += deltaTimeInSecond;
        setColorAccordingToLeftoverTime();
        setWidthAccordingToLeftoverTime();
    }
    private void setColorAccordingToLeftoverTime()
    {
        if(leftoverTimeInSecond >= firstThreshold)
        {
            fill.GetComponent<Image>().color = sky;
        }
        else if (leftoverTimeInSecond >= secondThreshold)
        {
            float blueWeight = (leftoverTimeInSecond - secondThreshold) / (firstThreshold - secondThreshold);
            float redWeight = 1 - blueWeight;
            fill.GetComponent<Image>().color = (redWeight*scarlet+blueWeight*sky);
        }
        else
        {
            fill.GetComponent<Image>().color = scarlet;
        }
    }
    private void setWidthAccordingToLeftoverTime()
    {
        if((leftoverTimeInSecond <= firstThreshold)&&(leftoverTimeInSecond >= secondThreshold))
        {
            // temp=1: 100%, temp=0: 200%
            float temp = Mathf.Abs((firstThreshold+secondThreshold)/2-leftoverTimeInSecond) / ((firstThreshold-secondThreshold)/2);
            fillArea.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.25f*temp);
            fillArea.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1-0.25f*temp);
        }
        else
        {
            fillArea.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.25f);
            fillArea.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.75f);
        }
    }
    private void playAudioSourceIfNotMuchTimeLeft()
    {
        const float epsilon = 0.1F;
        if ((middleThreshold - epsilon <= leftoverTimeInSecond) && (leftoverTimeInSecond <= middleThreshold + epsilon) && !inAudioSourceDelay)
        {
            notMuchTimeSound.Play();
            inAudioSourceDelay = true;
        }
        if (inAudioSourceDelay)
        {
            timeAfterPlayedAudioSourceInSecond += deltaTime;
            if (timeAfterPlayedAudioSourceInSecond >= audioSourceDelay)
            {
                timeAfterPlayedAudioSourceInSecond = 0.0F;
                inAudioSourceDelay = false;
            }
        }
    }
}
