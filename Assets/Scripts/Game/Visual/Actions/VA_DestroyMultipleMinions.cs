using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VA_DestroyMultipleMinions : VisualAction
{
    public VisualEntity Source;
    public List<VisualEntity> Targets;
    public Vector3 SourcePosition;
    public List<Vector3> TargetPositions;

    public List<GameObject> Projectiles;
    public float ProjectileSize = 0.3f;

    public VA_DestroyMultipleMinions(VisualEntity source, List<VisualEntity> targets, Color color)
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
    }

    public override void Update()
    {
        base.Update();
        for (int i = 0; i < Projectiles.Count; i++)
        {
            Projectiles[i].transform.position = Vector3.Lerp(SourcePosition, TargetPositions[i], CurrentFrame / Frames);
        }
    }

    public override void OnDone()
    {
        foreach (VisualEntity target in Targets) GameObject.Destroy(target.gameObject);
        foreach (GameObject projectile in Projectiles) GameObject.Destroy(projectile);
    }
}
