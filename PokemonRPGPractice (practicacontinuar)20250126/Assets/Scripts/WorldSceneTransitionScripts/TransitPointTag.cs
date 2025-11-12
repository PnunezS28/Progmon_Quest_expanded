using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitPointTag : MonoBehaviour
{
    [SerializeField] string TransitTag;
    [SerializeField] FacingDirectionEnum facingDirection;

    public string getTransitTag()
    {
        return TransitTag;
    }
    public FacingDirectionEnum FacingDirection => facingDirection;
}
