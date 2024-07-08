using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridMapFlowFieldDirection
{
    public readonly Vector2Int Vector;

    private GridMapFlowFieldDirection(int x, int y)
    {
        Vector = new Vector2Int(x, y);
    }

    public static implicit operator Vector2Int(GridMapFlowFieldDirection direction)
    {
        return direction.Vector;
    }

    public static GridMapFlowFieldDirection GetDirectionFromV2I(Vector2Int vector)
    {
        foreach (var direction in CardinalAndIntercardinalDirections)
        {
            if (direction.Vector == vector)
            {
                return direction;
            }
        }

        return None;
    }



    public static readonly GridMapFlowFieldDirection None = new GridMapFlowFieldDirection(0, 0);

    public static readonly GridMapFlowFieldDirection North = new GridMapFlowFieldDirection(0, 1);
    public static readonly GridMapFlowFieldDirection South = new GridMapFlowFieldDirection(0, -1);

    public static readonly GridMapFlowFieldDirection West = new GridMapFlowFieldDirection(-1, 0);
    public static readonly GridMapFlowFieldDirection East = new GridMapFlowFieldDirection(1, 0);

    public static readonly GridMapFlowFieldDirection NorthWest = new GridMapFlowFieldDirection(-1, 1);
    public static readonly GridMapFlowFieldDirection NorthEast = new GridMapFlowFieldDirection(1, 1);

    public static readonly GridMapFlowFieldDirection SouthWest = new GridMapFlowFieldDirection(-1, -1);
    public static readonly GridMapFlowFieldDirection SouthEast = new GridMapFlowFieldDirection(1, -1);

    public static readonly GridMapFlowFieldDirection Up = new GridMapFlowFieldDirection(0, 1);
    public static readonly GridMapFlowFieldDirection Down = new GridMapFlowFieldDirection(0, -1);
    public static readonly GridMapFlowFieldDirection Left = new GridMapFlowFieldDirection(-1, 0);
    public static readonly GridMapFlowFieldDirection Right = new GridMapFlowFieldDirection(1, 0);
    public static readonly GridMapFlowFieldDirection UpLeft = new GridMapFlowFieldDirection(-1, 1);
    public static readonly GridMapFlowFieldDirection UpRight = new GridMapFlowFieldDirection(1, 1);
    public static readonly GridMapFlowFieldDirection DownLeft = new GridMapFlowFieldDirection(-1, -1);
    public static readonly GridMapFlowFieldDirection DownRight = new GridMapFlowFieldDirection(1, -1);


    private static GridMapFlowFieldDirection[] RandomDirections = new[] { 
        new GridMapFlowFieldDirection(0, 1), 
        new GridMapFlowFieldDirection(0, -1), 
        new GridMapFlowFieldDirection(-1, 0),
        new GridMapFlowFieldDirection(1, 0),
        new GridMapFlowFieldDirection(-1, 1),
        new GridMapFlowFieldDirection(1, 1),
        new GridMapFlowFieldDirection(-1, -1),
        new GridMapFlowFieldDirection(1, -1),
    };

    public static List<GridMapFlowFieldDirection> GetRandomDirectionNoNone()
    {
        List<GridMapFlowFieldDirection> directions = new List<GridMapFlowFieldDirection>();

        foreach(var dir in CardinalAndIntercardinalDirections)
        {
            directions.Add(dir);
        }
        UtilsClass.ShuffleList(directions, 1);

        return directions;
    }

    public static readonly List<GridMapFlowFieldDirection> CardinalDirections = new List<GridMapFlowFieldDirection>
    {
        North, //Север
        East,  //Восток
        South, //Юг
        West   //Запад
    };

    public static readonly List<GridMapFlowFieldDirection> CardinalAndIntercardinalDirections = new List<GridMapFlowFieldDirection>
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };

    public static readonly List<GridMapFlowFieldDirection> AllDirections = new List<GridMapFlowFieldDirection>
    {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };
}
