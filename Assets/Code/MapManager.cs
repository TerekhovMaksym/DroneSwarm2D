using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class MapManager : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private SpriteRenderer mapDrawer;
    [SerializeField] private List<Drone> _drones;
    [SerializeField] private Drone _dronePrefab;
    [SerializeField] private Transform _droneSpawnPoint;
    [SerializeField] private Button _updatePathButton;

    private int droneCount;

    private void Awake()
    {
        DroneCountController.OnDroneCountChanged += OnDroneCountChanged;
        _updatePathButton.onClick.AddListener(RebuildPaths);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RebuildPaths();
        }
    }


    private void OnDroneCountChanged(int count)
    {
        while (_drones.Count > count)
        {
            Destroy(_drones.First().gameObject);
            _drones.RemoveAt(0);
        }

        while (_drones.Count < count)
        {
            var newDrone = Instantiate(_dronePrefab, _droneSpawnPoint.position, _droneSpawnPoint.rotation);
            newDrone.Init(Random.ColorHSV(0, 1, saturationMin: 1, saturationMax: 1));
            _drones.Add(newDrone);
        }

        droneCount = count;
    }

    private void RebuildPaths()
    {
        var cellSize = 25;

        var mask = MapCalculator.GetMask(cellSize, 0.2f, _sprite.texture);
        var graph = new FullMergedGraph(mask);

        var MST = MSTCalculator.GetMST(graph);

        var klasters = KlastersCreator.GetKlasters(MST, droneCount);

        var gridSizeX = 1f / _sprite.texture.width * cellSize * mapDrawer.size.x;
        var gridsSizeY = 1f / _sprite.texture.height * cellSize * mapDrawer.size.y;

        var mapShiftX = -mapDrawer.size.x / 2;
        var mapShiftY = -mapDrawer.size.y / 2;

        var mapSize = new Vector2(gridSizeX, gridsSizeY);
        var mapShift = new Vector2(mapShiftX, mapShiftY);

        var paths = klasters.Select(PathFinder.GetPath)
            .Select(x => PreparePath(x, mapSize, mapShift));

        var counter = 0;
        foreach (var path in paths)
        {
            _drones[counter].InitPath(path);
            counter++;
        }
    }

    private static List<Vector2> PreparePath(List<Joint> path, Vector2 mapSize, Vector2 mapShift)
    {
        return path.Select(x => x.Point2)
            .Select(x => new Vector2(((1f * x.X) * mapSize.x) + mapShift.x, ((1f * x.Y) * mapSize.y) + mapShift.y))
            .ToList();
    }
}