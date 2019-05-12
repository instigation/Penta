using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    public void scaleToZero()
    {
        gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
    }
}
