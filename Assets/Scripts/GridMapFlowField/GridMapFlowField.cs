using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CodeMonkey.Utils;

public class GridMapFlowField
{
    public static int newFlowFieldID = 0;
    public int ID = 0;

    public CellFlowField[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }

    public float cellRadius { get; private set; }
    public  float cellDiameter { get; private set; }

    private TextMesh[,] debugTextArray;
    private Transform owner;
    private TextMesh __zeroElement;

    private CellFlowField destination;

    public GridMapFlowField(float cellRadius, Vector2Int gridSize, Transform owner)
    {
        this.gridSize = gridSize;
        this.cellRadius = cellRadius;
        this.cellDiameter = cellRadius * 2f;
        this.owner = owner;
        this.ID = GridMapFlowField.newFlowFieldID;
        GridMapFlowField.newFlowFieldID++;
    }

    public void DisableDebugTextArray()
    {
        if (debugTextArray == null) return;

        if (__zeroElement.gameObject.activeSelf)
        {
            foreach (var debugText in debugTextArray)
            {
                debugText.gameObject.SetActive(false);
            }
        }
    }

    public void EnableDebugTextArray()
    {
        if (debugTextArray == null) return;

        if (!__zeroElement.gameObject.activeSelf)
        {
            foreach (var debugText in debugTextArray)
            {
                debugText.gameObject.SetActive(true);
            }
        }
    }

    public void CreateGrid()
    {
        this.grid = new CellFlowField[this.gridSize.x, this.gridSize.y];
        for (int x = 0; x < this.grid.GetLength(0); x++)
        {
            for (int y = 0; y < this.grid.GetLength(1); y++)
            {
                var worldPos = new Vector3((cellDiameter * x + cellRadius) + owner.position.x, (cellDiameter * y + cellRadius) + owner.position.y, -1f);
                this.grid[x, y] = new CellFlowField(worldPos, new Vector2Int(x, y));
            }
        }
    }

    public void CreateDebugTextObjects(string nameTextMeshElementGameObject = "debugTextGridMap")
    {
        if (debugTextArray != null) return;

        debugTextArray = new TextMesh[gridSize.x, gridSize.y];
        for (int x = 0; x < this.grid.GetLength(0); x++)
        {
            for (int y = 0; y < this.grid.GetLength(1); y++)
            {
                var cell = this.grid[x, y];
                var textMeshElement = UtilsClass.CreateWorldTextId(nameTextMeshElementGameObject, owner, cell.cost.ToString(), cell.worldPos - owner.position, 20, Color.black,
                TextAnchor.MiddleCenter, TextAlignment.Left, 5000);

                if (!__zeroElement)
                {
                    __zeroElement = textMeshElement;
                }
                textMeshElement.gameObject.SetActive(false);
                cell.debugTextMesh = textMeshElement;
                debugTextArray[x, y] = textMeshElement;
            }
        }
    }

    //То самое место где мы проверяем стены или "различную землю" на сцене и прибавляем ячейкам стоимость
    public void CreateCostField()
    {
        Vector3 cellHalfExtens = Vector3.one * cellRadius;
        int terrainMask = LayerMask.GetMask("Impassable", "RoughTerrain");

        foreach (CellFlowField curCell in this.grid)
        {
            Vector2 pos = Vector2.zero;
            pos.x = curCell.worldPos.x;
            pos.y = curCell.worldPos.y;

            Collider2D[] obstacles = Physics2D.OverlapBoxAll(pos, cellHalfExtens, 0f, terrainMask);
            bool hasIncreasedCost = false;

            foreach(var col in obstacles)
            {
                if(col.gameObject.layer == 6)
                {
                    curCell.IncreaseCost(byte.MaxValue);
                    continue;
                }
                else if(!hasIncreasedCost && col.gameObject.layer == 7)
                {
                    curCell.IncreaseCost(3);
                    hasIncreasedCost = true;
                }
            }
        }
    }

    //Поле интеграции
    public void CreateIntegrationField(CellFlowField destination)
    {
        this.destination = destination;
        this.destination.cost = 0;
        this.destination.SetBestCost(0);

        Queue<CellFlowField> cellsToCheck = new Queue<CellFlowField>();
        cellsToCheck.Enqueue(this.destination);

        while(cellsToCheck.Count > 0)
        {
            CellFlowField curCell = cellsToCheck.Dequeue();
            List<CellFlowField> curNeighbours = GetNeighborCells(curCell.gridIndex, GridMapFlowFieldDirection.CardinalDirections);

            foreach(var curNeighbor in curNeighbours)
            {
                if(curNeighbor.cost == byte.MaxValue)
                {
                    continue;
                }

                if(curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
                {
                    curNeighbor.SetBestCost((ushort)(curNeighbor.cost + curCell.bestCost));
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    /* TO DO: Размазать операцию по кадру */
    //Постройка самого поля потоков
    public void CreateFlowField()
    {
        foreach(CellFlowField curCell in grid)
        {
            List<CellFlowField> curNeighbors = GetNeighborCells(curCell.gridIndex, GridMapFlowFieldDirection.AllDirections);
            int bestCost = curCell.bestCost;

            foreach(CellFlowField curNeigbor in curNeighbors)
            {
                if(curNeigbor.bestCost < bestCost)
                {
                    bestCost = curNeigbor.bestCost;
                    curCell.bestDirection = GridMapFlowFieldDirection.GetDirectionFromV2I(curNeigbor.gridIndex - curCell.gridIndex);

                }
            }

            curCell.angle = (int)Vector2.SignedAngle(Vector2.right, curCell.bestDirection.Vector);
        }
    }

    public CellFlowField GetCellNearestAllowedDestination(Vector3 worldPos, int attempts = 10)
    {
        CellFlowField resultCell = null;
        var curCell = GetCellByWorldPosition(worldPos);
        if (curCell.cost < byte.MaxValue)
        {
            return curCell;
        }
        else
        {
            List<CellFlowField> curNeighbours = GetNeighborCells(curCell.gridIndex, GridMapFlowFieldDirection.GetRandomDirectionNoNone());
            do
            {
                foreach (var curNeighbor in curNeighbours)
                {
                    if (curNeighbor != null)
                    {
                        resultCell = curNeighbor;
                    }
                    if (curNeighbor.cost < byte.MaxValue)
                    {
                        return curNeighbor;
                    }
                }

                curNeighbours = GetNeighborCells(resultCell.gridIndex, GridMapFlowFieldDirection.GetRandomDirectionNoNone());
                attempts--;

            } while (resultCell != null && attempts >= 0);
        }

        return null;
    }

    public CellFlowField GetCellByWorldPosition(Vector3 worldPos)
    {
        var getX = Mathf.FloorToInt((worldPos.x - owner.position.x) / cellDiameter);
        var getY = Mathf.FloorToInt((worldPos.y - owner.position.y) / cellDiameter);
        if (getX >= 0 && getY >= 0 && getX < this.grid.GetLength(0) && getY < this.grid.GetLength(1))
        {
            return this.grid[getX, getY];
        }

        return null;
    }

    public void HardResetCells()
    {
        foreach (var cell in grid)
        {
            cell.HardResetCell();
        }
    }

    public void SoftResetCells()
    {
        foreach(var cell in grid)
        {
            cell.SoftResetCell();
        }
    }

    public CellFlowField GetCellAtRelativePos(Vector2Int orignPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = orignPos + relativePos;

        if(finalPos.x < 0 
            || finalPos.x >= gridSize.x 
            || finalPos.y < 0 
            || finalPos.y >= gridSize.y)
        {
            return null;
        }

        return grid[finalPos.x, finalPos.y];
    }

    private List<CellFlowField> GetNeighborCells(Vector2Int nodeIndex, List<GridMapFlowFieldDirection> directions)
    {
        List<CellFlowField> neighborCells = new List<CellFlowField>();

        foreach(Vector2Int curDirection in directions)
        {
            CellFlowField newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
            if (newNeighbor != null)
            {
                neighborCells.Add(newNeighbor);
            }
        }
        return neighborCells;
    }
}
