using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBlock : Block {
    private int blinkHash = Animator.StringToHash("blink");

    public void blink() {
        gameObject.GetComponent<Animator>().SetTrigger("blink");
    }
    new public void destroy()
    {
        Destroy(gameObject);
    }

}
