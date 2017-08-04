using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils{
    public static Vector3 toVector3(Coordinate coor) {
        return new Vector3(coor.x, coor.y);
    }

    public static void moveUIElementForDistance(GameObject target, Vector2 distance) {
        target.GetComponent<RectTransform>().anchoredPosition += distance;
    }

    public static void moveUIElementToPosition(GameObject target, Vector3 position) {
        target.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public static float getWidthOfUIElement(GameObject target) {
        return target.GetComponent<RectTransform>().rect.width;
    }

    public static List<Vector3> getPositionsOfUIElements(List<GameObject> targets) {
        List<Vector3> ret = new List<Vector3>();
        foreach (GameObject target in targets)
            ret.Add(getPositionOfUIElement(target));
        return ret;
    }
    public static Vector3 getPositionOfUIElement(GameObject target) {
        return target.GetComponent<RectTransform>().anchoredPosition;
    }

    public static Vector3 rotateClockwiseAQuater(Vector3 vector) {
        return new Vector3(vector.y, -vector.x);
    }

    public class Square {
        private Vector3 center;
        private float sideLength;
        public Square(Vector3 center, float sideLength) {
            this.center = center;
            this.sideLength = sideLength;
        }
        public bool includes(Vector3 point) {
            return
                (Mathf.Abs(center.x - point.x) < sideLength/2)
                &&
                (Mathf.Abs(center.y - point.y) < sideLength/2);
        }
    }
}