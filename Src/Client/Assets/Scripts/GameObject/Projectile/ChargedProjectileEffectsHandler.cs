using Utilities;
using UnityEngine;

public class ChargedProjectileEffectsHandler : MonoBehaviour
{

    public GameObject ChargingObject;
    public MinMaxVector3 Scale;
    public MinMaxColor Color;

    MeshRenderer[] affectedRenderers;
    ProjectileBase projectileBase;

    void OnEnable()
    {
        projectileBase = GetComponent<ProjectileBase>();
        projectileBase.onShoot += OnShoot;
        affectedRenderers = ChargingObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var ren in affectedRenderers)
        {
            ren.sharedMaterial = Instantiate(ren.sharedMaterial);
        }
    }

    void OnShoot()
    {
        ChargingObject.transform.localScale = Scale.GetValueFromRatio(projectileBase.InitialCharge);

        foreach (var ren in affectedRenderers)
        {
            ren.sharedMaterial.SetColor("_Color", Color.GetValueFromRatio(projectileBase.InitialCharge));
        }
    }
}

