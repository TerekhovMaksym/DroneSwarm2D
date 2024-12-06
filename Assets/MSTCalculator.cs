using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public static class MSTCalculator
{
    public static Graph FullifyMST(Graph graph, bool addDiagonals = false)
    {
        var points = graph.Points;

        foreach (var point in points)
        {
            TryAddJointAt(graph, point, point.X + 1, point.Y,1f);
            TryAddJointAt(graph, point, point.X - 1, point.Y,1f);
            TryAddJointAt(graph, point, point.X, point.Y + 1,1f);
            TryAddJointAt(graph, point, point.X, point.Y - 1,1f);

            if (addDiagonals == false)
                return graph;
            
            TryAddJointAt(graph, point, point.X + 1, point.Y+1, 1.4f);
            TryAddJointAt(graph, point, point.X + 1, point.Y-1, 1.4f);
            TryAddJointAt(graph, point, point.X - 1, point.Y+1, 1.4f);
            TryAddJointAt(graph, point, point.X - 1, point.Y-1, 1.4f);
        }

        return graph;
    }

    private static void TryAddJointAt(Graph graph, Point point, int newPointX, int newPointY, float length)
    {
        if (graph.TryGetPoint(newPointX, newPointY, out var point2))
        {
            if (graph.DoHaveJointWith(point, point2) == false)
            {
                var joint = new Joint()
                {
                    Point1 = point,
                    Point2 = point2,
                    Length = length,
                };

                graph.AddJoint(joint);
            }
        }
    }

    public static Graph GetMST(FullMergedGraph graph)
    {
        var processedPoints = new HashSet<int>() { 0 };
        var mstGraph = new Graph();
        mstGraph.AddPoint(graph.Points.First());

        while (processedPoints.Count < graph.Points.Count)
        {
            var treeJoints = graph.GetAdjacentPoints(processedPoints);

            var minJoint = (pointX: Int32.MaxValue, pointY: Int32.MaxValue, length: Int32.MaxValue);
            foreach (var joint in treeJoints)
            {
                if (processedPoints.Contains(joint.pointX) && processedPoints.Contains(joint.pointY))
                {
                    continue;
                }

                if (joint.length < minJoint.length)
                {
                    minJoint = joint;
                }
            }

            var pointToAdd = processedPoints.Contains(minJoint.pointX) ? minJoint.pointY : minJoint.pointX;
            processedPoints.Add(pointToAdd);
            mstGraph.AddPoint(graph.Points[pointToAdd]);

            var jointToAdd = new Joint()
            {
                Point1 = graph.Points[minJoint.pointX],
                Point2 = graph.Points[minJoint.pointY],
                Length = minJoint.length,
            };

            mstGraph.AddJoint(jointToAdd);
        }

        return mstGraph;
    }
}