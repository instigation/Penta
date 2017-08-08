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

    public static void centerToOrigin(List<List<Coordinate>> coors) {
        Coordinate center = centerOfEnvelope(coors);
        for(int i = 0; i < coors.Count; i++) {
            for(int j = 0; j < coors[i].Count; j++) {
                coors[i][j] -= center;
            }
        }
    }
    private static Coordinate centerOfEnvelope(List<List<Coordinate>> coors) {
        List<Coordinate> mergedCoors = new List<Coordinate>();
        foreach(List<Coordinate> sub in coors) {
            mergedCoors.AddRange(sub);
        }
        return centerOfEnvelope(mergedCoors);
    }
    public static void centerToOrigin(List<Coordinate> coors) {
        Coordinate center = centerOfEnvelope(coors);
        for (int i = 0; i < coors.Count; i++)
            coors[i] -= center;
    }
    private static Coordinate centerOfEnvelope(List<Coordinate> coors) {
        int minX, minY, maxX, maxY;
        minX = minY = MAX_INT;
        maxX = maxY = MIN_INT;
        foreach(Coordinate coor in coors) {
            if (coor.x < minX)
                minX = coor.x;
            if (coor.x > maxX)
                maxX = coor.x;
            if (coor.y < minY)
                minY = coor.y;
            if (coor.y > maxY)
                maxY = coor.y;
        }
        return new Coordinate((minX + maxX)/2, (minY + maxY)/2);
    }

    public static void leftMostToOrigin(List<Coordinate> coors) {
        Coordinate leftMostPoint = leftMostOfEnvelope(coors);
        for(int i = 0; i < coors.Count; i++)
            coors[i] -= leftMostPoint;
    }
    private static Coordinate leftMostOfEnvelope(List<Coordinate> set) {
        int minX, minY, maxY;
        minX = minY = MAX_INT;
        maxY = MIN_INT;
        foreach (Coordinate coor in set) {
            if (coor.x < minX)
                minX = coor.x;
            if (coor.y < minY)
                minY = coor.y;
            if (coor.y > maxY)
                maxY = coor.y;
        }
        return new Coordinate(minX, (minY + maxY) / 2);
    }

    public static void rotateRandomlySavingWidth(List<Coordinate> set) {
        rotateRandomly(set);
        int height = getHeight(set);
        int width = getWidth(set);
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

    public static int getHeight(List<Coordinate> set) {
        int count = 0;
        List<int> occuredYCoordinates = new List<int>();
        foreach (Coordinate point in set) {
            if (!occuredYCoordinates.Contains(point.y)) {
                occuredYCoordinates.Add(point.y);
                count++;
            }
        }
        return count;
    }
    public static int getWidth(List<Coordinate> set) {
        int count = 0;
        List<int> occuredXCoordinates = new List<int>();
        foreach(Coordinate point in set) {
            if (!occuredXCoordinates.Contains(point.x)) {
                occuredXCoordinates.Add(point.x);
                count++;
            }
        }
        return count;
    }
}
