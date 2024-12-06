using System;
using System.Collections.Generic;
using System.Linq;

public sealed partial class MapManager
{
    public static class KlastersCreator
    {
        public static List<Graph> GetKlasters(Graph MST, int droneCount)
        {
            MST = MSTCalculator.FullifyMST(MST, true);
            var totalLength = MST.Points.Count;
            var lenForOne = Math.Floor(1f * totalLength / droneCount);
        
            var uncheckedPoints = new HashSet<Point>(MST.Points);
            var klasters = new List<Graph>(droneCount);
            for (var i = 0; i < droneCount; i++)
            {
                var sumLen = 0f;
                var klaster = new Graph();
                klasters.Add(klaster);
        
                while (sumLen < lenForOne)
                {
                    if (uncheckedPoints.Count == 0)
                    {
                        break;
                    }
        
                    var point = uncheckedPoints.First();
                    klaster.AddPoint(point);
                    uncheckedPoints.Remove(point);
                    var jointsToCheck = MST.GetJoints(point).ToList();
        
                    for (var j = 0; j < jointsToCheck.Count; j++)
                    {
                        if (sumLen > lenForOne)
                        {
                            continue;
                        }
        
                        var joint = jointsToCheck[j];
        
                        if (klaster.AddJoint(joint) == false)
                        {
                            continue;
                        }
        
                        if (klaster.AddPoint(joint.Point1) && uncheckedPoints.Contains(joint.Point1))
                        {
                            sumLen += 1;
                        }
        
                        if (klaster.AddPoint(joint.Point2) && uncheckedPoints.Contains(joint.Point2))
                        {
                            sumLen += 1;
                        }
        
                        jointsToCheck.AddRange(MST.GetJoints(joint.Point2));
        
                        uncheckedPoints.Remove(joint.Point1);
                        uncheckedPoints.Remove(joint.Point2);
                    }
        
                    jointsToCheck.Clear();
                }
            }
        
            return klasters;
        }
    }
}