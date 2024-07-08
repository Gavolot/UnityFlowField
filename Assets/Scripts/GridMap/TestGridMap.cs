using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestGridMap : MonoBehaviour
{
    private GridMap gridMap;
    private void Start()
    {
        this.gridMap = new GridMap(34, 16, this.transform, 2);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var pos = UtilsClass.GetMouseWorldPosition();
            this.gridMap.SetValue(pos, this.gridMap.GetValue(pos) + 1);
        }
    }
}
