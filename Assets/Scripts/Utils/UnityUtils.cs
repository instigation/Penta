using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils{
    public static Vector3 toVector3(Coordinate coor) {
        return new Vector3(coor.x, coor.y);
    }


    public static void moveUIElementToAnchoredPosition(GameObject target, Vector3 position) {
        target.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public static float getWidthOfUIElement(GameObject target) {
        float scaleFactor = target.GetComponent<RectTransform>().localScale.x;
        return target.GetComponent<RectTransform>().rect.width * scaleFactor;
    }
    public static Vector3 getAnchoredPositionOfUIElement(GameObject target) {
        return target.GetComponent<RectTransform>().anchoredPosition;
    }
    public static Vector3 getLocalPositionOfUIElement(GameObject target)
    {
        return target.GetComponent<RectTransform>().localPosition;
    }

    public static Vector2 rotateClockwiseAQuater(Vector2 vector) {
        return new Vector2(vector.y, -vector.x);
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

    public class Rectangle {
        private Vector3 center;
        private float xLength, yLength;
        public Rectangle(Vector3 center, float xLength, float yLength) {
            this.center = center;
            this.xLength = xLength;
            this.yLength = yLength;
        }
        public bool includes(Vector3 point) {
            return
                (Mathf.Abs(center.x - point.x) < xLength / 2)
                &&
                (Mathf.Abs(center.y - point.y) < yLength / 2);
        }
    }
}