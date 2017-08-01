using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils{
    public static Vector3 toVector3(Coordinate coor) {
        return new Vector3(coor.x, coor.y);
    }

    public static void moveGameObjectForDistance(GameObject target, Vector2 distance) {
        target.GetComponent<RectTransform>().anchoredPosition += distance;
    }

    public static void moveGameObjectToPosition(GameObject target, Vector3 position) {
        target.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public static float getWidthOfUIElement(GameObject target) {
        return target.GetComponent<RectTransform>().rect.width;
    }

    public static Vector3 getPositionOfUIElement(GameObject target) {
        return target.GetComponent<RectTransform>().anchoredPosition;
    }
}