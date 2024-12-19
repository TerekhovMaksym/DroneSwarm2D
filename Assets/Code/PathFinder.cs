using System.Collections.Generic;
using System.Linq;


public static class PathFinder
{
    public static List<Joint> GetPath(Graph klaster)
    {
        klaster = MSTCalculator.FullifyMST(klaster, addDiagonals: true);

        var uncheckedPoints = new HashSet<Point>(klaster.Points);
        var path = new List<Joint>();
        var p = Bfs(klaster, uncheckedPoints, path);
        p = FinishPath(p);
        return p;
    }

    private static List<Joint> FinishPath(List<Joint> path)
    {
        if (path.Count == 0)
        {
            return path;
        }

        var lastPoint = path.Last().Point2;
        var firstPoint = path.First().Point1;
        var lastPathJoint = new Joint()
        {
            Point1 = lastPoint,
            Point2 = firstPoint,
            Length = FullMergedGraph.GetSqrLength(lastPoint, firstPoint)
        };
        path.Add(lastPathJoint);

        for (int i = 1; i < path.Count; i++)
        {
            var prevPoint = path[i - 1].Point2;
            var currentPoint = path[i].Point1;

            if (prevPoint.Equals(currentPoint))
            {
                continue;
            }

            var newJoint = new Joint()
            {
                Point1 = prevPoint,
                Point2 = currentPoint,
                Length = FullMergedGraph.GetSqrLength(prevPoint, currentPoint)
            };

            path.Insert(i, newJoint);
        }

        return path;
    }

    private static List<Joint> Bfs(Graph klaster, HashSet<Point> uncheckedPoints, List<Joint> path)
    {
        if (uncheckedPoints.Any() == false)
        {
            return path;
        }

        Point point;
        if (path.Count == 0)
        {
            point = klaster.Points.First();
            uncheckedPoints.Remove(point);
        }
        else
        {
            point = path.Last().Point2;
        }

        var joints = klaster.GetJoints(point).OrderBy(x => x.Length);
        foreach (var joint in joints)
        {
            var anotherPoint = joint.Point1.Equals(point) ? joint.Point2 : joint.Point1;

            if (uncheckedPoints.Contains(anotherPoint))
            {
                path.Add(new Joint { Point1 = point, Point2 = anotherPoint, Length = joint.Length });
                uncheckedPoints.Remove(anotherPoint);
                path = Bfs(klaster, uncheckedPoints, path);
            }
        }

        return path;
    }
}