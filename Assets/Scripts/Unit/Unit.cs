using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int team = -1;
    public bool isSelected = false;

    [SerializeField]
    private SpriteRenderer selector;

    [SerializeField]
    private GridControllerFlowField targetGridController;

    //TO DO redo in save/load
    private static int newUnitID = 0;
    public int unitID { get; private set; }


    [SerializeField]
    private Rigidbody2D rb;


    private bool isMove = false;


    [SerializeField]
    private float moveSpeed = 100f;


    public Vector3 targetPos { get; private set; }

    void Start()
    {
        //TO DO redo in save/load
        selector.enabled = isSelected;
        unitID = newUnitID;
        newUnitID++;
    }


    private void FixedUpdate()
    {
        if (isMove)
        {
            if (targetGridController)
            {
                CellFlowField cellBelow = targetGridController.curFlowField.GetCellByWorldPosition(transform.position);
                if (cellBelow != null)
                {
                    Vector2 moveDirection = new Vector2(cellBelow.bestDirection.Vector.x, cellBelow.bestDirection.Vector.y);
                    rb.velocity = moveDirection * (moveSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void CommandMoveToPoint(Vector3 pos, GridControllerFlowField targetGridController)
    {
        targetPos = pos;
        if (this.targetGridController != null)
        {
            this.targetGridController.DeleteUnitFromHostUnits(unitID);
        }
        this.targetGridController = targetGridController;
        this.targetGridController.AddHostUnit(this);
        this.isMove = true;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        selector.enabled = isSelected;
    }
}
