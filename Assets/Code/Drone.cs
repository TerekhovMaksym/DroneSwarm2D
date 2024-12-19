using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Drone : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Color lineColor = Color.white;

    private List<Vector2> _path;
    private int _currentPathIndex;
    private bool _isPathSet;

    public void Init(Color lineColor)
    {
        this.lineColor = lineColor;
    }
    
    public void InitPath(List<Vector2> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogError("Path is null or empty!");
            return;
        }

        _path = path;
        _currentPathIndex = 0;
        _isPathSet = true;
        
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.loop = true;
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.Select(x => new Vector3(x.x, x.y, transform.position.z)).ToArray());
    }

    public void Update()
    {
        if (!_isPathSet || _path == null || _currentPathIndex >= _path.Count)
            return;

        Vector2 currentPoint = _path[_currentPathIndex];
        Vector2 currentPosition = transform.position;

        Vector2 newPosition = Vector2.MoveTowards(currentPosition, currentPoint, speed * Time.deltaTime);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);

        if (Vector2.Distance(newPosition, currentPoint) < 0.01f)
        {
            _currentPathIndex++;
        }

        if (_currentPathIndex >= _path.Count)
        {
            _currentPathIndex = 0;
        }
    }
}