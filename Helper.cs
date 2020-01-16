using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{
    private Map map;
    public Hex[,] matrix;

    public Helper(Map m)
    {
        this.map = m;
        this.matrix = map.getGrid();
    }

    public Map getMap()
    {
        return map;
    }

    public Hex getHexAt(int x, int y)
    {
        if (matrix == null || y < 0 || y >= map.getNrLinii())
        {
            Debug.LogError("Hexagon invalid");
        }

        return matrix[x % map.getNrCol(), y];
    }

    public static void AllHexesInRange(Hex hex, int range)
    {
        //TO DO: implement
    }

}
