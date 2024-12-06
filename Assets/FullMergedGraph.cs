using System.Collections.Generic;

public class FullMergedGraph
{
    public List<Point> Points { get; private set; }

    public FullMergedGraph(bool[,] map)
    {
        Points = new List<Point>();

        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == false)
                    continue;


                var point = new Point()
                {
                    X = x,
                    Y = y,
                };

                Points.Add(point);
            }
        }
    }

    public static int GetSqrLength(Point point1, Point point2)
    {
        return ((point1.X - point2.X) * (point1.X - point2.X)) + ((point1.Y - point2.Y) * (point1.Y - point2.Y));
    }

    public IEnumerable<(int pointX, int pointY, int length)> GetAdjacentPoints(ICollection<int> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = 0; j < Points.Count; j++)
            {
                if (j == i)
                {
                    continue;
                }

                yield return (i, j, GetSqrLength(Points[i], Points[j]));
            }
        }
    }
}