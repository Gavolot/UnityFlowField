using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldControllersPull : MonoBehaviour
{
    private List<GridControllerFlowField> controllersPull;
    void Start()
    {
        controllersPull = new List<GridControllerFlowField>(20);


        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        foreach (Transform child in transform)
        {
            var componentController = child.gameObject.GetComponent<GridControllerFlowField>();
            if (componentController)
            {
                controllersPull.Add(componentController);
                componentController.CustomStart();
                child.gameObject.SetActive(false);
            }
        }
    }

    public GridControllerFlowField GetFree()
    {
        foreach (var controller in controllersPull)
        {
            if (!controller.gameObject.activeSelf)
            {
                return controller;
            }
        }

        return null;
    }

    void Update()
    {
        
        foreach(var controller in controllersPull)
        {
            if(controller.hostUnits.Count != 0)
            {
                controller.CallInUpdate();
            }
        }
        
    }
}
