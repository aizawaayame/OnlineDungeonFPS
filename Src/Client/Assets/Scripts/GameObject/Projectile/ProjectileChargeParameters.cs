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
        ProjectileStandard proj = GetComponent<ProjectileStandard>();
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

