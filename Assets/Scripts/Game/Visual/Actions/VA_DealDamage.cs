using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_DealDamage : VisualAction
{
    public VisualEntity Source;
    public VisualEntity Target;
    public Vector3 SourcePosition;
    public Vector3 TargetPosition;

    public GameObject Projectile;
    public float ProjectTileBaseSize = 0.05f;
    public float ProjectileSize;

    public VA_DealDamage(VisualEntity source, VisualEntity target, int amount, Color color)
    {
        Frames = 80;
        Source = source;
        SourcePosition = source.transform.position;
        Target = target;
        TargetPosition = target.transform.position;
        ProjectileSize = amount * ProjectTileBaseSize;

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
    }
}
