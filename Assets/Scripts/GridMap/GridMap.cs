using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CodeMonkey.Utils;

public class GridMap
{
    private int width;
    private int height;
    private int[,] cells;
    private float cellSize;

    private TextMesh[,] debugTextArray;


    private Transform owner;

    public GridMap(int width, int height, Transform owner, float cellSize, bool isDebugWorldText = true)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        cells = new int[width, height];

        this.owner = owner;

        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for(int y = 0; y < cells.GetLength(1); y++)
            {
                if (isDebugWorldText)
                {
                    var textMeshElement = UtilsClass.CreateWorldTextId("debugTextGridMap", owner, cells[x, y].ToString(), GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.white, 
                    TextAnchor.MiddleCenter, TextAlignment.Left, 5000);

                    debugTextArray[x, y] = textMeshElement;
                }
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        var cellSizesX = (cellSize * (float)cells.GetLength(0));
        var cellSizesY = (cellSize * (float)cells.GetLength(1));

        var vect = new Vector3(x, y) * cellSize;

        return vect;
    }


    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(((worldPosition - owner.position).x) / cellSize);
        y = Mathf.FloorToInt(((worldPosition - owner.position).y) / cellSize);
    }


    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            this.cells[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        int result = 0;

        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            result = cells[x, y];
        }
        return result;
    }

    public int GetValue(int x, int y)
    {
        int result = 0;
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            result = cells[x, y];
        }
        return result;
    }

}
