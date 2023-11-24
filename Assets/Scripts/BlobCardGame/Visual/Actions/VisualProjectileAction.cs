using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualProjectileAction : VisualAction
{
    public VisualEntity Source;
    public VisualEntity Target;
    public Vector3 SourcePosition;
    public Vector3 TargetPosition;

    public int Amount;
    public Color Color;

    public GameObject Projectile;
    public float ProjectTileBaseSize;
    public float ProjectileSize;

    public float BobDurationRel;
    public float BobExtraScale;
    public float BobDuration;
    public Vector3 BobSourceScale;
    public Vector3 BobTargetScale;

    public VisualProjectileAction(VisualEntity source, VisualEntity target, int amount, Color color)
    {
        Source = source;
        Target = target;
        Amount = amount;
        Color = color;
    }

    protected void Init()
    {
        SourcePosition = Source.transform.position;
        TargetPosition = Target.transform.position;
        ProjectileSize = Amount * ProjectTileBaseSize;

        Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Projectile.GetComponent<Renderer>().material.color = Color;
        Projectile.transform.position = SourcePosition;
        Projectile.transform.localScale = new Vector3(ProjectileSize, ProjectileSize, ProjectileSize);

        BobDuration = Frames * BobDurationRel;
        BobSourceScale = Source.transform.localScale;
        BobTargetScale = new Vector3(BobSourceScale.x * BobExtraScale, BobSourceScale.y * BobExtraScale, BobSourceScale.z * BobExtraScale);
    }

    public override void Update()
    {
        base.Update();
        if(CurrentFrame <= BobDuration / 2)
        {
            float scale = BobSourceScale.x + ((BobTargetScale.x - BobSourceScale.x) * (CurrentFrame / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (CurrentFrame <= BobDuration)
        {
            float scale = BobTargetScale.x - ((BobTargetScale.x - BobSourceScale.x) * ((CurrentFrame - BobDuration/2) / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(scale, scale, scale);

            Projectile.transform.position = Vector3.Lerp(SourcePosition, TargetPosition, (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
        }
        else
        {
            Projectile.transform.position = Vector3.Lerp(SourcePosition, TargetPosition, (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
        }
    }

    public override void OnDone()
    {
        Source.transform.localScale = BobSourceScale;
        GameObject.Destroy(Projectile);
    }
}
