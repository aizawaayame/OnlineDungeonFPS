using UnityEngine;

public class WeaponFuelCellHandler : MonoBehaviour
{

    #region Fields
    public bool simultaneousFuelCellsUsage = false;
    public GameObject[] fuelCells;
    public Vector3 fuelCellUsedPosition;
    public Vector3 fuelCellUnusedPosition = new Vector3(0f, -0.1f, 0f);
    
    WeaponController weapon;
    bool[] fuelCellsCooled;
    #endregion
    
    void Start()
    {
        weapon = GetComponent<WeaponController>();

        fuelCellsCooled = new bool[fuelCells.Length];
        for (int i = 0; i < fuelCellsCooled.Length; i++)
        {
            fuelCellsCooled[i] = true;
        }
    }

    void Update()
    {
        if (simultaneousFuelCellsUsage)
        {
            for (int i = 0; i < fuelCells.Length; i++)
            {
                fuelCells[i].transform.localPosition = Vector3.Lerp(fuelCellUsedPosition, fuelCellUnusedPosition,
                    weapon.CurrentAmmoRatio);
            }
        }
        else
        {
            // TODO: needs simplification
            for (int i = 0; i < fuelCells.Length; i++)
            {
                float length = fuelCells.Length;
                float lim1 = i / length;
                float lim2 = (i + 1) / length;

                float value = Mathf.InverseLerp(lim1, lim2, weapon.CurrentAmmoRatio);
                value = Mathf.Clamp01(value);

                fuelCells[i].transform.localPosition =
                    Vector3.Lerp(fuelCellUsedPosition, fuelCellUnusedPosition, value);
            }
        }
    }
}


