using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public int team = -1;
    private List<Unit> selectedUnits;
    void Start()
    {
        selectedUnits = new List<Unit>(30);
    }

    public void ClearUnits()
    {
        selectedUnits.Clear();
    }

    public List<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    public void TrySelectUnits(List<Collider2D> candidatUnits)
    {
        foreach(var unit in candidatUnits)
        {
            var unitComponent = unit.gameObject.GetComponent<Unit>();
            if (unitComponent)
            {
                if(unitComponent.team == team)
                {
                    selectedUnits.Add(unitComponent);
                    unitComponent.SetSelected(true);
                }
            }
        }
    }

    public void TryDeselectUnits()
    {
        foreach(var unitComponent in selectedUnits)
        {
            unitComponent.SetSelected(false);
        }
        ClearUnits();
    }
}
