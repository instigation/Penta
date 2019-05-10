using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelfFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void destroySelf()
    {
        Destroy(gameObject);
    }

    public void destroyParent()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void destroySelfAfter(float seconds)
    {
        StartCoroutine(destroySelfAfterEnumerator(seconds));
    }
    private IEnumerator destroySelfAfterEnumerator(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        destroySelf();
        yield return null;
    }
}
