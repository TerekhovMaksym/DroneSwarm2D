using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

[TestFixture]
public class KlastersCreatorTests
{
    private Graph testGraph;

    [SetUp]
    public void Setup()
    {
        testGraph = new Graph();
        var point1 = new Point { X = 0, Y = 0 };
        var point2 = new Point { X = 1, Y = 0 };
        var point3 = new Point { X = 0, Y = 1 };
        var point4 = new Point { X = 1, Y = 1 };

        testGraph.AddPoint(point1);
        testGraph.AddPoint(point2);
        testGraph.AddPoint(point3);
        testGraph.AddPoint(point4);

        testGraph.AddJoint(new Joint { Point1 = point1, Point2 = point2, Length = 1f });
        testGraph.AddJoint(new Joint { Point1 = point2, Point2 = point4, Length = 1f });
        testGraph.AddJoint(new Joint { Point1 = point1, Point2 = point3, Length = 1f });
    }

    [Test]
    public void GetKlasters_WithTwoDrones_ShouldCreateTwoEqualKlasters()
    {
        var klasters = MapManager.KlastersCreator.GetKlasters(testGraph, 2);

        Assert.That(klasters, Has.Count.EqualTo(2));
        Assert.That(klasters[0].Points, Has.Count.EqualTo(2));
        Assert.That(klasters[1].Points, Has.Count.EqualTo(2));
        Assert.That(klasters[0].Joints.Any());
        Assert.That(klasters[1].Joints.Any());
    }

    [Test]
    public void GetKlasters_WithOneDrone_ShouldReturnSingleKlaster()
    {
        var klasters = MapManager.KlastersCreator.GetKlasters(testGraph, 1);

        Assert.That(klasters, Has.Count.EqualTo(1));
        Assert.That(klasters[0].Points, Has.Count.EqualTo(4));
        Assert.That(klasters[0].Joints.Count(), Is.EqualTo(3));
    }

    [Test]
    public void GetKlasters_WithMoreDronesThanPoints_ShouldHandleCorrectly()
    {
        var smallGraph = new Graph();
        var point1 = new Point { X = 0, Y = 0 };
        var point2 = new Point { X = 1, Y = 0 };
        smallGraph.AddPoint(point1);
        smallGraph.AddPoint(point2);
        smallGraph.AddJoint(new Joint { Point1 = point1, Point2 = point2, Length = 1f });

        var klasters = MapManager.KlastersCreator.GetKlasters(smallGraph, 3);

        Assert.That(klasters, Has.Count.EqualTo(3));
        Assert.That(klasters.Sum(k => k.Points.Count), Is.EqualTo(2));
    }

    [Test]
    public void GetKlasters_WithDisconnectedGraph_ShouldHandleIslands()
    {
        var disconnectedGraph = new Graph();
        var point1 = new Point { X = 0, Y = 0 };
        var point2 = new Point { X = 1, Y = 0 };
        var point3 = new Point { X = 3, Y = 3 };
        var point4 = new Point { X = 4, Y = 3 };

        disconnectedGraph.AddPoint(point1);
        disconnectedGraph.AddPoint(point2);
        disconnectedGraph.AddPoint(point3);
        disconnectedGraph.AddPoint(point4);

        disconnectedGraph.AddJoint(new Joint { Point1 = point1, Point2 = point2, Length = 1f });
        disconnectedGraph.AddJoint(new Joint { Point1 = point3, Point2 = point4, Length = 1f });

        var klasters = MapManager.KlastersCreator.GetKlasters(disconnectedGraph, 2);

        Assert.That(klasters, Has.Count.EqualTo(2));
        Assert.That(klasters[0].Points, Has.Count.EqualTo(2));
        Assert.That(klasters[1].Points, Has.Count.EqualTo(2));
    }
}

[TestFixture]
public class MSTCalculatorTests
{
    [Test]
    public void FullifyMST_WithoutDiagonals_ShouldAddOrthogonalConnections()
    {
        var graph = new Graph();
        var point1 = new Point { X = 0, Y = 0 };
        var point2 = new Point { X = 1, Y = 0 };
        graph.AddPoint(point1);
        graph.AddPoint(point2);

        var result = MSTCalculator.FullifyMST(graph, false);

        Assert.That(result.GetJoints(point1).Count(), Is.EqualTo(1));
        var joint = result.GetJoints(point1).First();
        Assert.That(joint.Length, Is.EqualTo(1f));
    }

    [Test]
    public void GetMST_ShouldCreateMinimumSpanningTree()
    {
        var map = new bool[2, 2] { { true, true }, { true, true } };
        var fullGraph = new FullMergedGraph(map);
        
        var mst = MSTCalculator.GetMST(fullGraph);

        Assert.That(mst.Points.Count, Is.EqualTo(4));
        Assert.That(mst.Joints.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetMST_WithLargeGraph_ShouldOptimizeConnections()
    {
        var map = new bool[100, 100];
        for (int i = 0; i < 100; i++)
            for (int j = 0; j < 100; j++)
                map[i, j] = true;

        var fullGraph = new FullMergedGraph(map);
        var startTime = DateTime.Now;
        
        var mst = MSTCalculator.GetMST(fullGraph);
        var executionTime = (DateTime.Now - startTime).TotalSeconds;

        Assert.That(executionTime, Is.LessThan(1.0));
        Assert.That(mst.Points.Count, Is.EqualTo(10000));
        Assert.That(mst.Joints.Count, Is.EqualTo(9999));
    }

    [Test]
    public void FullifyMST_WithDiagonals_ShouldAddAllConnections()
    {
        var graph = new Graph();
        var points = new Point[3, 3];
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                points[i, j] = new Point { X = i, Y = j };
                graph.AddPoint(points[i, j]);
            }

        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 3; j++)
                graph.AddJoint(new Joint { 
                    Point1 = points[i, j], 
                    Point2 = points[i + 1, j], 
                    Length = 1f 
                });

        var result = MSTCalculator.FullifyMST(graph, true);

        foreach (var point in graph.Points)
        {
            var joints = result.GetJoints(point);
            Assert.That(joints.Count(j => j.Length == 1.4f), Is.GreaterThan(0));
        }
    }
}

[TestFixture]
public class MapCalculatorTests
{
    private Texture2D testTexture;

    [SetUp]
    public void Setup()
    {
        testTexture = new Texture2D(4, 4);
    }

    [Test]
    public void GetMask_WithFullyOpaqueTexture_ShouldReturnTrueCells()
    {
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++)
            pixels[i] = new Color(1, 1, 1, 1);
        testTexture.SetPixels(pixels);
        testTexture.Apply();

        var mask = MapCalculator.GetMask(2, 0.5f, testTexture);

        Assert.That(mask.GetLength(0), Is.EqualTo(2));
        Assert.That(mask.GetLength(1), Is.EqualTo(2));
        Assert.That(mask[0, 0], Is.True);
        Assert.That(mask[1, 1], Is.True);
    }

    [Test]
    public void GetMask_WithGradientTexture_ShouldHandlePartialTransparency()
    {
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++)
            pixels[i] = new Color(1, 1, 1, i / 15f);
        testTexture.SetPixels(pixels);
        testTexture.Apply();

        var mask = MapCalculator.GetMask(2, 0.5f, testTexture);

        var trueCount = 0;
        for (int i = 0; i < mask.GetLength(0); i++)
            for (int j = 0; j < mask.GetLength(1); j++)
                if (mask[i, j]) trueCount++;

        Assert.That(trueCount, Is.EqualTo(1));
    }

    [TearDown]
    public void Cleanup()
    {
        UnityEngine.Object.Destroy(testTexture);
    }
}

[TestFixture]
public class PathFinderTests
{
    [Test]
    public void GetPath_WithSimpleGraph_ShouldCreateCircularPath()
    {
        var graph = new Graph();
        var points = new Point[2, 2];
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
            {
                points[i, j] = new Point { X = i, Y = j };
                graph.AddPoint(points[i, j]);
            }

        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
            {
                if (i < 1) graph.AddJoint(new Joint { 
                    Point1 = points[i, j], 
                    Point2 = points[i + 1, j], 
                    Length = 1f 
                });
                if (j < 1) graph.AddJoint(new Joint { 
                    Point1 = points[i, j], 
                    Point2 = points[i, j + 1], 
                    Length = 1f 
                });
            }

        var path = PathFinder.GetPath(graph);

        Assert.That(path.First().Point1, Is.EqualTo(path.Last().Point2));
        Assert.That(path.Count, Is.EqualTo(5));
    }

    [Test]
    public void GetPath_WithDisconnectedPoints_ShouldHandleCorrectly()
    {
        var graph = new Graph();
        var point1 = new Point { X = 0, Y = 0 };
        var point2 = new Point { X = 1, Y = 0 };
        var point3 = new Point { X = 10, Y = 10 };

        graph.AddPoint(point1);
        graph.AddPoint(point2);
        graph.AddPoint(point3);
        graph.AddJoint(new Joint { Point1 = point1, Point2 = point2, Length = 1f });

        var path = PathFinder.GetPath(graph);

        Assert.That(path.Count, Is.EqualTo(2));
        Assert.That(path.Any(j => j.Point1.Equals(point3) || j.Point2.Equals(point3)), Is.False);
    }

    [Test]
    public void GetPath_WithSinglePoint_ShouldReturnValidPath()
    {
        var graph = new Graph();
        var point = new Point { X = 0, Y = 0 };
        graph.AddPoint(point);

        var path = PathFinder.GetPath(graph);

        Assert.That(path, Is.Not.Empty);
        Assert.That(path.First().Point1, Is.EqualTo(path.Last().Point2));
        Assert.That(path.All(j => j.Point1.Equals(point) && j.Point2.Equals(point)));
    }
}