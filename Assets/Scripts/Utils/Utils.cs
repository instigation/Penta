using System.Collections.Generic;
using System;

public static class Utils {
    private static Random rng = new Random();

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
}
