using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_DestroyMinion : VisualAction
{
    public VisualEntity Source;
    public VisualEntity Target;
    public Vector3 SourcePosition;
    public Vector3 TargetPosition;

    public GameObject Projectile;
    public float ProjectileSize = 0.3f;

    public VA_DestroyMinion(VisualEntity source, VisualEntity target, Color color)
    {
        Frames = 120;
        Source = source;
        SourcePosition = source.transform.position;
        Target = target;
        TargetPosition = target.transform.position;

        Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Projectile.GetComponent<Renderer>().material.color = color;
        Projectile.transform.position = SourcePosition;
        Projectile.transform.localScale = new Vector3(ProjectileSize, ProjectileSize, ProjectileSize);
    }

    public override void Update()
    {
        base.Update();
        Projectile.transform.position = Vector3.Lerp(SourcePosition, TargetPosition, CurrentFrame / Frames);
    }

    public override void OnDone()
    {
        GameObject.Destroy(Projectile);
        GameObject.Destroy(Target.gameObject);
    }
}
