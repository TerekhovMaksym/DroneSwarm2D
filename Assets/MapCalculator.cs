using UnityEngine;

public static class MapCalculator
{
    public static bool[,] GetMask(int cellSize, float strictness, Texture2D mapTexture)
    {
        var gridSizeX = (int)System.MathF.Floor(1f * mapTexture.width / cellSize);
        var gridsSizeY = (int)System.MathF.Floor(1f * mapTexture.height / cellSize);

        var map = new bool[gridSizeX, gridsSizeY];

        for (var x = 0; x < gridSizeX; x++)
        {
            for (var y = 0; y < gridsSizeY; y++)
            {
                var coloredCount = 0f;
                var totalCount = 0f;

                for (var xx = 0; xx < cellSize; xx++)
                {
                    for (var yy = 0; yy < cellSize; yy++)
                    {
                        var pixelCoordX = (x * cellSize) + xx;
                        var pixelCoordY = (y * cellSize) + yy;

                        if (pixelCoordX > mapTexture.width || pixelCoordY > mapTexture.height)
                        {
                            continue;
                        }

                        totalCount++;

                        var isColored = mapTexture.GetPixel(pixelCoordX, pixelCoordY).a > 0;

                        if (isColored)
                        {
                            coloredCount++;
                        }
                    }
                }

                map[x, y] = coloredCount / totalCount >= strictness;
            }
        }

        return map;
    }
}