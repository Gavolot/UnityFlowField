using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFlowField
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public byte cost;
    private byte initCost;
    public ushort bestCost { get; private set; }

    public TextMesh debugTextMesh;

    public CellFlowField destinationCell;

    public GridMapFlowFieldDirection bestDirection;
    public int angle;

    public CellFlowField(Vector3 worldPos, Vector2Int gridIndex)
    {
        this.worldPos = worldPos;
        this.gridIndex = gridIndex;

        cost = 1;
        initCost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridMapFlowFieldDirection.None;
        angle = -1000;
    }

    public void HardResetCell()
    {
        bestCost = ushort.MaxValue;
        cost = 1;
        initCost = 1;
        bestDirection = GridMapFlowFieldDirection.None;
        angle = -1000;

        if (debugTextMesh)
        {
            debugTextMesh.text = this.cost.ToString();
        }
    }

    public void SoftResetCell()
    {
        bestCost = ushort.MaxValue;
        cost = initCost;
        bestDirection = GridMapFlowFieldDirection.None;
        angle = -1000;

        if (debugTextMesh)
        {
            debugTextMesh.text = this.cost.ToString();
        }
    }


    public void SetBestCost(ushort bestCost)
    {
        this.bestCost = bestCost;
        if (debugTextMesh)
        {
            debugTextMesh.text = this.bestCost.ToString();
        }
    }

    public void IncreaseCost(byte amount, bool increaseInitCost = true)
    {
        if (this.cost == byte.MaxValue)
        {
        }
        else
        if (amount + this.cost >= byte.MaxValue)
        {
            this.cost = byte.MaxValue;
        }
        else
        {
            this.cost += amount;
        }

        if (increaseInitCost)
        {
            this.initCost = this.cost;
        }

        if (debugTextMesh)
        {
            debugTextMesh.text = this.cost.ToString();
        }
    }
}
