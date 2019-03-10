using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private Animator animator;
    private bool isDisappearAnimationFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
}
