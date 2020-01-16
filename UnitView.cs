using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    Vector3 Position;
    Hex hex;

    void Start()
    {
        Position = this.transform.position;
    }
    public void OnUnitMoved(Hex oldhex, Hex newHex)
    {
        Position = newHex.PositionFromCamera();
        hex = newHex;
    }
    void Update()
    {
        if (hex == null)
            this.transform.position = Position;
        else
            this.transform.position = hex.Position2();
    }
}
