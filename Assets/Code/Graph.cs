using System.Collections.Generic;
using System.Linq;

public class Graph
{
    public IReadOnlyList<Point> Points => _points;
    public IReadOnlyList<Joint> Joints => _joints;

    private List<Point> _points;
    private List<Joint> _joints;
    private Dictionary<Point, Dictionary<Point, Joint>> _jointsToPoint;
    private Dictionary<int, Dictionary<int, Point>> _pointToCoord;

    public Graph()
    {
        _points = new List<Point>();
        _joints = new List<Joint>();
        _jointsToPoint = new Dictionary<Point, Dictionary<Point, Joint>>();
        _pointToCoord = new Dictionary<int, Dictionary<int, Point>>();
    }

    public bool TryGetPoint(int x, int y, out Point point)
    {
        point = new Point();
        return _pointToCoord.ContainsKey(x) && _pointToCoord[x].TryGetValue(y, out point);
    }

    public bool DoHaveJointWith(Point x, Point y)
    {
        return _jointsToPoint.ContainsKey(x) && _jointsToPoint[x].ContainsKey(y);
    }
    
    public IEnumerable<Joint> GetJoints(Point point)
    {
        return _joints.Where(x => x.Point1.Equals(point) || x.Point2.Equals(point));
    }

    public bool AddPoint(Point point)
    {
        if (TryGetPoint(point.X, point.Y, out var _))
        {
            return false;
        }

        _points.Add(point);
        if (_pointToCoord.ContainsKey(point.X) == false)
        {
            _pointToCoord.Add(point.X, new Dictionary<int, Point>());
        }

        _pointToCoord[point.X][point.Y] = point;
        return true;
    }

    public bool AddJoint(Joint joint)
    {
        var isThereJointFrom = _jointsToPoint.TryGetValue(joint.Point1, out var jointsFrom);

        if (isThereJointFrom)
        {
            var isThereJointTo = jointsFrom.ContainsKey(joint.Point2);

            if (isThereJointTo)
            {
                return false;
            }
        }

        _joints.Add(joint);

        if (isThereJointFrom == false)
        {
            _jointsToPoint[joint.Point1] = new Dictionary<Point, Joint>();
        }

        _jointsToPoint[joint.Point1][joint.Point2] = joint;


        var isThereJointFrom2 = _jointsToPoint.ContainsKey(joint.Point2);
        if (isThereJointFrom2 == false)
        {
            _jointsToPoint[joint.Point2] = new Dictionary<Point, Joint>();
        }

        _jointsToPoint[joint.Point2][joint.Point1] = joint;
        return true;
    }
}