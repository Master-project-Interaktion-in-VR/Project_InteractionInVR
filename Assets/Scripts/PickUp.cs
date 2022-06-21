using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class PickUp : BaseInputHandler, IMixedRealityHandJointHandler, IMixedRealityPointerHandler
{
    public MixedRealityInputAction PickUpAction;
    public bool CollidedWithHand;
    
    protected override void RegisterHandlers()
    {
        
    }

    protected override void UnregisterHandlers()
    {
        
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        CollidedWithHand = !CollidedWithHand;
    }

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
        if (eventData.MixedRealityInputAction == PickUpAction && CollidedWithHand)
        {
            gameObject.SetActive(false);
        }
    }
}
