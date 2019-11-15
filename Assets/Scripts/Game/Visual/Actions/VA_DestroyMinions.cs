using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VA_DestroyMinions : VisualAction
{
    public VisualEntity Source;
    public List<VisualEntity> Targets;
    public Vector3 SourcePosition;
    public List<Vector3> TargetPositions;

    public List<GameObject> Projectiles;
    public float ProjectileSize = 0.3f;

    public float BobDurationRel = 0.5f;
    public float BobExtraScale = 1.5f;
    public float BobDuration;
    public Vector3 BobSourceScale;
    public Vector3 BobTargetScale;

    public VA_DestroyMinions(VisualEntity source, List<VisualEntity> targets, Color color)
    {
        Projectiles = new List<GameObject>();
        Frames = 120;
        Source = source;
        SourcePosition = source.transform.position;
        Targets = targets;
        TargetPositions = targets.Select(x => x.transform.position).ToList();

        foreach (VisualEntity target in targets)
        {
            GameObject Projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Projectile.GetComponent<Renderer>().material.color = color;
            Projectile.transform.position = SourcePosition;
            Projectile.transform.localScale = new Vector3(ProjectileSize, ProjectileSize, ProjectileSize);
            Projectiles.Add(Projectile);
        }

        BobDuration = Frames * BobDurationRel;
        BobSourceScale = Source.transform.localScale;
        BobTargetScale = new Vector3(BobSourceScale.x * BobExtraScale, BobSourceScale.y * BobExtraScale, BobSourceScale.z * BobExtraScale);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentFrame <= BobDuration / 2)
        {
            float scale = BobSourceScale.x + ((BobTargetScale.x - BobSourceScale.x) * (CurrentFrame / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(scale, scale, scale);
        }
        else if (CurrentFrame <= BobDuration)
        {
            float scale = BobTargetScale.x - ((BobTargetScale.x - BobSourceScale.x) * ((CurrentFrame - BobDuration / 2) / (BobDuration / 2)));
            Source.transform.localScale = new Vector3(scale, scale, scale);

            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectiles[i].transform.position = Vector3.Lerp(SourcePosition, TargetPositions[i], (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
            }
        }
        else
        {
            for (int i = 0; i < Projectiles.Count; i++)
            {
                Projectiles[i].transform.position = Vector3.Lerp(SourcePosition, TargetPositions[i], (CurrentFrame - (BobDuration / 2)) / (Frames - (BobDuration / 2)));
            }
        }
    }

    public override void OnDone()
    {
        foreach (VisualEntity target in Targets) GameObject.Destroy(target.gameObject);
        foreach (GameObject projectile in Projectiles) GameObject.Destroy(projectile);
    }
}
