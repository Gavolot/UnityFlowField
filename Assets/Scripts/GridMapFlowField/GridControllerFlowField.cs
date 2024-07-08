using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridControllerFlowField : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public GridMapFlowField curFlowField { get; private set; }
    public bool isDrawCurDebugFlowField = false;
    public bool isTextDebugEnabled = false;

    public bool isDoNotMakeDebugTextObjectsInFlowField = false;

    private GridMapFlowField curFlowFieldDebug;

    public List<Unit> hostUnits { get; private set; }

    //--
    //--
    [SerializeField]
    private float ArrowWidth = 19;
    [SerializeField]
    private float ArrowHeight = 13;

    [SerializeField]
    private float ArrowDiagWidth = 17;
    [SerializeField]
    private float ArrowDiagHeight = 17;
    //--
    //--
    [SerializeField]
    private Texture2D arrowRightTexture;
    [SerializeField]
    private Texture2D arrowLeftTexture;
    [SerializeField]
    private Texture2D arrowUpTexture;
    [SerializeField]
    private Texture2D arrowDownTexture;
    [SerializeField]
    private Texture2D arrowRightDownTexture;
    [SerializeField]
    private Texture2D arrowLeftDownTexture;
    [SerializeField]
    private Texture2D arrowRightUpTexture;
    [SerializeField]
    private Texture2D arrowLeftUpTexture;

    public void DeleteUnitFromHostUnits(int unitID)
    {
        foreach(var unit in hostUnits)
        {
            if(unit.unitID == unitID)
            {
                hostUnits.Remove(unit);
                break;
            }
        }

        if(hostUnits.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartSearch(Vector3 targetPos)
    {
        var pos = targetPos;

        if (curFlowField != null)
        {
            var cell = this.curFlowField.GetCellNearestAllowedDestination(pos);

            if (cell != null)
            {
                this.curFlowField.SoftResetCells();
                this.curFlowField.CreateIntegrationField(cell);
                this.curFlowField.CreateFlowField();
            }
            else
            {
                ClearHostUnits();
            }
        }
    }

    public void ClearHostUnits()
    {
        hostUnits.Clear();
        this.gameObject.SetActive(false);
    }

    public void AddHostUnit(Unit unit)
    {
        if(hostUnits.Count == 0)
        {
            StartSearch(unit.targetPos);
        }
        hostUnits.Add(unit);
        this.gameObject.SetActive(true);
    }

    private void InitializeFlowField()
    {
        curFlowField = new GridMapFlowField(cellRadius, gridSize, transform);
        curFlowField.CreateGrid();

        curFlowFieldDebug = curFlowField;
        hostUnits = new List<Unit>(30);
    }


    public void CustomStart()
    {
        UtilsClass.InitRandomSeedByDateTimeTicks();

        InitializeFlowField();

        if (!isDoNotMakeDebugTextObjectsInFlowField)
        {
            this.curFlowField.CreateDebugTextObjects();
        }

        this.curFlowField.CreateCostField();
    }

    public void CallInUpdate()
    {
        if (hostUnits.Count > 0)
        {
            //TO DO: UpdateUnitDirection
        }
        if (!isTextDebugEnabled)
        {
            curFlowField.DisableDebugTextArray();
        }
        else
        {
            curFlowField.EnableDebugTextArray();
        }


        /*
        if (Input.GetMouseButtonDown(2))
        {
            if (curFlowField != null)
            {
                var pos = UtilsClass.GetMouseWorldPosition();
                var cell = this.curFlowField.GetCellByWorldPosition(pos);

                if (cell != null)
                {
                    Debug.Log(cell.angle);
                }
            }
        } 
        */
    }

    private void OnGUI()
    {
        if (isDrawCurDebugFlowField && curFlowFieldDebug != null)
        {
            float cellWidth = cellRadius;
            float cellHeight = cellRadius;

            Camera camera = Camera.main;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var cell = curFlowFieldDebug.grid[x, y];

                    if (cell.bestDirection == GridMapFlowFieldDirection.None || cell.cost == byte.MaxValue)
                    {
                        continue;
                    }

                    Vector3 screenPos = camera.WorldToScreenPoint(cell.worldPos);
                    float posX = screenPos.x - cellWidth / 2;
                    float posY = Screen.height - screenPos.y - cellHeight / 2;

                    float angle = cell.angle;

                    // Определение направления и рисование текстуры
                    if (angle == 0)
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowWidth, ArrowHeight), arrowRightTexture);
                    }
                    else
                    if (angle == 90)
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowHeight, ArrowWidth), arrowUpTexture);
                    }
                    else
                    if (angle == -90)
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowHeight, ArrowWidth), arrowDownTexture);
                    }
                    else
                    if (angle == 180)
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowWidth, ArrowHeight), arrowLeftTexture);
                    }
                    else
                    if (angle == 45) //вверх вправо
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowDiagWidth, ArrowDiagHeight), arrowRightUpTexture);
                    }
                    else
                    if (angle == -45) //вниз вправо
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowDiagWidth, ArrowDiagHeight), arrowRightDownTexture);
                    }
                    else
                    if (angle == -135) //вниз влево
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowDiagWidth, ArrowDiagHeight), arrowLeftDownTexture);
                    }
                    else
                    if (angle == 135) //вверх влево
                    {
                        GUI.DrawTexture(new Rect(posX, posY, ArrowDiagWidth, ArrowDiagHeight), arrowLeftUpTexture);
                    }
                }
            }
        }
    }
}
