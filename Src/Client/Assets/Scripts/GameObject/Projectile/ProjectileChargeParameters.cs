using Utilities;
using UnityEngine;

public class ProjectileChargeParameters : MonoBehaviour
{
    public MinMaxFloat Damage;
    public MinMaxFloat Radius;
    public MinMaxFloat Speed;
    public MinMaxFloat GravityDownAcceleration;
    public MinMaxFloat AreaOfEffectDistance;

    ProjectileBase projectileBase;

    void OnEnable()
    {
        projectileBase = GetComponent<ProjectileBase>();
        projectileBase.onShoot += OnShoot;
    }

    void OnShoot()
    {
        // Apply the parameters based on projectile charge
        Unity.FPS.Gameplay.ProjectileStandard proj = GetComponent<Unity.FPS.Gameplay.ProjectileStandard>();
        if (proj)
        {
            proj.Damage = Damage.GetValueFromRatio(projectileBase.InitialCharge);
            proj.Radius = Radius.GetValueFromRatio(projectileBase.InitialCharge);
            proj.Speed = Speed.GetValueFromRatio(projectileBase.InitialCharge);
            proj.GravityDownAcceleration =
                GravityDownAcceleration.GetValueFromRatio(projectileBase.InitialCharge);
        }
    }
}

