using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkingspaceValidator : InputValidator {
    private GameObject workingArea;

    public WorkingspaceValidator(GameObject workingArea) {
        this.workingArea = workingArea;
    }

    public bool isValid(Vector3 mousePosition) {
        RectTransform rt = workingArea.GetComponent<RectTransform>();
        Vector2 lowerLeft = rt.offsetMin;
        Vector2 size = rt.rect.size;
        Rect rect = new Rect(lowerLeft.x, lowerLeft.y, size.x, size.y);
        return rect.Contains(mousePosition);
    }
}
