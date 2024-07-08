using UnityEngine;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Draws a selection rectangle on the left mouse button down & dragging
///
/// You only need an InputReceiverManager that basically tracks
/// - if the left mouse button is currently down and saves it in "LeftMouseButtonDown"
/// - saves the initial click position when mouse button was clicked  and saves it in "InitialMousePositionOnLeftClick"
/// - updates the current mouse position and saves it in "CurrentMousePosition"
///
/// </summary>
public class SelectionTool : MonoBehaviour
{
    public Color SelectionRectangleFillerColor;
    public Color SelectionRectangleBorderColor;
    public int SelectionRectangleBorderThickness = 2;
    private Texture2D _selectionRectangleFiller;
    private Texture2D _selectionRectangleBorder;
    private bool _drawSelectionRectangle;
    private Vector2 initialMousePositionOnLeftClick;
    private Vector2 currentMousePosition;
    private bool isStartSelection = false;

    [SerializeField]
    private UnitController unitController;

    [SerializeField]
    private FlowFieldControllersPull flowFieldControllersPull;

    void Start()
    {
        _selectionRectangleFiller = new Texture2D(1, 1);
        _selectionRectangleFiller.SetPixel(0, 0, SelectionRectangleFillerColor);
        _selectionRectangleFiller.Apply();
        _selectionRectangleFiller.wrapMode = TextureWrapMode.Clamp;
        _selectionRectangleFiller.filterMode = FilterMode.Point;

        _selectionRectangleBorder = new Texture2D(1, 1);
        _selectionRectangleBorder.SetPixel(0, 0, SelectionRectangleBorderColor);
        _selectionRectangleBorder.Apply();
        _selectionRectangleBorder.wrapMode = TextureWrapMode.Clamp;
        _selectionRectangleBorder.filterMode = FilterMode.Point;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isStartSelection = true;
            initialMousePositionOnLeftClick = Input.mousePosition;
        }

        if (isStartSelection)
        {
            currentMousePosition = Input.mousePosition;
            _drawSelectionRectangle = Input.GetMouseButton(0);

            if (!_drawSelectionRectangle)
            {
                TrySelectUnits();
                isStartSelection = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            GridControllerFlowField pull = null;
            var selectedUnits = unitController.GetSelectedUnits();
            if(selectedUnits.Count > 0)
            {
                pull = flowFieldControllersPull.GetFree();

                if (pull)
                {
                    foreach (var unit in selectedUnits)
                    {
                        unit.CommandMoveToPoint(UtilsClass.GetMouseWorldPosition(), pull);
                    }
                }
            }
        }
    }
    private void OnGUI()
    {
        if (_drawSelectionRectangle)
        {
            DrawSelectionRectangle();
        }
    }
    private void DrawSelectionRectangle()
    {
        // Convert coordinates from screen space
        var screenHeight = Camera.main.pixelHeight;

        float x1 = initialMousePositionOnLeftClick.x;
        float x2 = currentMousePosition.x;
        float y1 = screenHeight - initialMousePositionOnLeftClick.y;
        float y2 = screenHeight - currentMousePosition.y;


        GUI.DrawTexture(new Rect(Mathf.Min(x1, x2), Mathf.Min(y1, y2), Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1)), _selectionRectangleFiller);


        GUI.DrawTexture(new Rect(Mathf.Min(x1, x2), Mathf.Min(y1, y2), Mathf.Abs(x2 - x1), SelectionRectangleBorderThickness), _selectionRectangleBorder); // Top border
        GUI.DrawTexture(new Rect(Mathf.Min(x1, x2), Mathf.Max(y1, y2) - SelectionRectangleBorderThickness, Mathf.Abs(x2 - x1), SelectionRectangleBorderThickness), _selectionRectangleBorder); // Bottom border
        GUI.DrawTexture(new Rect(Mathf.Min(x1, x2), Mathf.Min(y1, y2), SelectionRectangleBorderThickness, Mathf.Abs(y2 - y1)), _selectionRectangleBorder); // Left border
        GUI.DrawTexture(new Rect(Mathf.Max(x1, x2) - SelectionRectangleBorderThickness, Mathf.Min(y1, y2), SelectionRectangleBorderThickness, Mathf.Abs(y2 - y1)), _selectionRectangleBorder); // Right border
    }

    private void TrySelectUnits()
    {
        Vector2 worldStart = Camera.main.ScreenToWorldPoint(initialMousePositionOnLeftClick);
        Vector2 worldEnd = Camera.main.ScreenToWorldPoint(currentMousePosition);

        Vector2 bottomLeft = new Vector2(Mathf.Min(worldStart.x, worldEnd.x), Mathf.Min(worldStart.y, worldEnd.y));
        Vector2 topRight = new Vector2(Mathf.Max(worldStart.x, worldEnd.x), Mathf.Max(worldStart.y, worldEnd.y));

        Collider2D[] units = Physics2D.OverlapAreaAll(bottomLeft, topRight);

        List<Collider2D> candidatUnits = new List<Collider2D>();

        foreach (var unit in units)
        {
            if(unit.gameObject.layer == 8)
            {
                candidatUnits.Add(unit);
            }
        }

        unitController.TryDeselectUnits();

        if(candidatUnits.Count > 0)
        {
            unitController.TrySelectUnits(candidatUnits);
        }
    }
}
