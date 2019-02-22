using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Penta;

public class CsharpTest {
    private void addElem(List<Coordinate> original) {
        var item = new Coordinate(0, 0);
        original.Add(item);
    }
    private void modifyFirstElem(List<Coordinate> target) {
        target[0] -= new Coordinate(1, 1);
    }
    [Test]
    public void ListParameterBehaviourTest() {
        List<Coordinate> target = new List<Coordinate>();
        var item = new Coordinate(1, 1);
        target.Add(item);
        addElem(target);
        Assert.That(target.Count, Is.EqualTo(2));
        modifyFirstElem(target);
        Assert.That(target[0], Is.EqualTo(new Coordinate(0, 0)));
    }
}

public class PuzzleRendererTest {

    [Test]
    public void normalizeTest() {
        List<Coordinate> target = new List<Coordinate>();
        target.Add(new Coordinate(-1, 3));
        target.Add(new Coordinate(-2, 2));
        target.Add(new Coordinate(0, 2));
        target.Add(new Coordinate(0, 1));
        target.Add(new Coordinate(-1, 2));
        Utils.centerToOrigin(new List<List<Coordinate>> { target });
        foreach (var point in target) {
            Assert.IsFalse(point.y == 2);
        }
    }
}