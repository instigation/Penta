using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    private Animator animator;
    private bool isDisappearAnimationFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();


        originalColor = getColor();
    }

    public void playDisappearAnimation()
    {
        animator.SetTrigger("StageClear");
    }
    public bool isDisappearFinished() { return isDisappearAnimationFinished; }

    private void onDisappearFinish()
    {
        isDisappearAnimationFinished = true;
    }



    private Color originalColor;

    public Vector2 getAnchoredPosition()
    {
        return gameObject.GetComponent<RectTransform>().anchoredPosition;
    }
    public void moveToAnchoredPosition(Vector2 targetPosition)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = targetPosition;
    }
    public void moveForAnchoredVector(Vector2 deltaPosition)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition += deltaPosition;
    }
    public float getWidth()
    {
        float scaleFactor = gameObject.GetComponent<RectTransform>().localScale.x;
        return gameObject.GetComponent<RectTransform>().rect.width * scaleFactor;
    }
    public void scale(float factor)
    {
        gameObject.GetComponent<RectTransform>().localScale *= factor;
    }
    public Color getColor()
    {
        return gameObject.GetComponent<Image>().color;
    }
    public void resetColor()
    {
        setColor(originalColor);
    }
    public void setColor(Color targetColor)
    {
        gameObject.GetComponent<Image>().color = targetColor;
    }
    public void destroy()
    {
        Destroy(gameObject);
    }
}
