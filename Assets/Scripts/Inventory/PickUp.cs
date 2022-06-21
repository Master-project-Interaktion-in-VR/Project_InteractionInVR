using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class PickUp : BaseInputHandler, IMixedRealityPointerHandler
{
    public MixedRealityInputAction PickUpAction;
    public SolverHandler SolverHandler;
    public InventoryManager InventoryManager;

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (eventData.MixedRealityInputAction == PickUpAction && InventoryManager.HandIsFree())
        {
            SolverHandler.UpdateSolvers = true;
            InventoryManager.PutItemInHand(gameObject);
        }
    }

    protected override void RegisterHandlers()
    {
        
    }

    protected override void UnregisterHandlers()
    {
        
    }
}
