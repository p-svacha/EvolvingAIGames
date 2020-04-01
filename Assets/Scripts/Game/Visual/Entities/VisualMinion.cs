using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualMinion : VisualEntity
{

    public void SetHaloEnabled(bool enabled)
    {
        Component halo = gameObject.GetComponent("Halo");
        halo.GetType().GetProperty("enabled").SetValue(halo, enabled, null);
    }
}
