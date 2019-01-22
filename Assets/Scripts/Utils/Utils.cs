using System.Collections.Generic;
using System;
using UnityEngine;

public static class Utils {
    /// <summary>
    /// set이란 connected blocks.
    /// </summary>
    private const int MIN_INT = -1000;
    private const int MAX_INT = 1000;
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Coordinate rotatePointToRotation(Coordinate original, Rotation rot) {
        ///<summary>
        /// NORTH가 default direction이라고 가정함
        /// center가 원점이라고 가정함
        ///</summary>
        switch (rot) {
            case Rotation.NORTH:
                return original;
            case Rotation.EAST:
                return new Coordinate(original.y, -original.x);
            case Rotation.SOUTH:
                return new Coordinate(-original.x, -original.y);
            case Rotation.WEST:
                return new Coordinate(-original.y, original.x);
            default:
                throw new ArgumentException("Rotation in argument is null");
        }
    }

    private class Envelope {
        Coordinate min, max;
        public Envelope(List<Coordinate> set) {
            if (set.Count == 0) {
                min = new Coordinate(0, 0);
                // 이래야 getWidth/Height 할때 0이 나옴
                max = new Coordinate(-1, -1);
            }
            else {
                min = new Coordinate(set[0]);
                max = new Coordinate(set[0]);
                foreach(Coordinate point in set) {
                    if (point.x < min.x)
                        min.x = point.x;
                    if (point.x > max.x)
                        max.x = point.x;
                    if (point.y < min.y)
                        min.y = point.y;
                    if (point.y > max.y)
                        max.y = point.y;
                }
            }
        }
        public Coordinate center() {
            return new Coordinate((min.x + max.x) / 2, (min.y + max.y) / 2);
        }
        public Coordinate leftMost() {
            return new Coordinate(min.x, (min.y + max.y) / 2);
        }
        public Coordinate leftTop()
        {
            return new Coordinate(min.x, max.y);
        }
        public int getWidth() {
            return max.x - min.x + 1;
        }
        public int getHeight() {
            return max.y - min.y + 1;
        }
    }
    public static void centerToOrigin(List<List<Coordinate>> set) {
        Coordinate center = centerOfEnvelope(set);
        for(int i = 0; i < set.Count; i++) {
            for(int j = 0; j < set[i].Count; j++) {
                set[i][j] -= center;
            }
        }
    }
    private static Coordinate centerOfEnvelope(List<List<Coordinate>> set) {
        List<Coordinate> mergedSet = new List<Coordinate>();
        foreach(List<Coordinate> sub in set) {
            mergedSet.AddRange(sub);
        }
        return (new Envelope(mergedSet)).center();
    }
    public static void centerToOrigin(List<Coordinate> set) {
        Envelope envelope = new Envelope(set);
        Coordinate center = envelope.center();
        for (int i = 0; i < set.Count; i++)
            set[i] -= center;
    }
    public static void leftTopToOrigin(List<Coordinate> set)
    {
        Envelope envelope = new Envelope(set);
        Coordinate center = envelope.leftTop();
        for (int i = 0; i < set.Count; i++)
            set[i] -= center;
    }

    public static void leftMostToOrigin(List<Coordinate> set) {
        Envelope envelope = new Envelope(set);
        Coordinate leftMostPoint = envelope.leftMost();
        for (int i = 0; i < set.Count; i++)
            set[i] -= leftMostPoint;
    }

    public static void rotateRandomlySavingWidth(List<Coordinate> set) {
        rotateRandomly(set);
        Envelope envelope = new Envelope(set);
        int height = envelope.getHeight();
        int width = envelope.getWidth();
        if (height < width)
            rotateSetForTimes(set, 1);
    }
    public static void rotateRandomly(List<Coordinate> set) {
        int randomRotation = rng.Next(Rotator.numRotation());
        rotateSetForTimes(set, randomRotation);
    }
    public static void rotateSetForTimes(List<Coordinate> set, int times) {
        Coordinate center = set[0];
        for (int i = 1; i < set.Count; i++) {
            Coordinate vectorFromCenter = set[i] - center;
            Coordinate result = rotatePointToRotation(vectorFromCenter, (Rotation) times);
            set[i] = result + center;
        }
    }
    public static bool isXConsistent(List<Coordinate> set)
    {

        if (set.Count == 0)
            return true;
        else
        {
            int x = set[0].x;
            foreach(Coordinate elem in set)
            {
                if (elem.x != x)
                    return false;
            }
        }
        return true;
    }
}