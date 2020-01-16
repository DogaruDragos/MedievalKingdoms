using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hex
{
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    Unit unit;

    private int BarriertoMoveIn;
    Player player;
    public int terrain;
    public readonly Map map;

    static readonly float grosimeHexC = Mathf.Sqrt(3) / 2;

    private List<int> listOfObstructions;

    public void setBarrier()
    {
        BarriertoMoveIn+= 1;
    }

    public void addObstruction(int i)
    {
        listOfObstructions.Add(i);
    }
    public void removeObstruction(int i)
    {
        listOfObstructions.Remove(i);
    }

    public void removeBarrier()
    {
        BarriertoMoveIn-= 1;
        if (BarriertoMoveIn < 0)
        {
            Debug.Log("bariera e mai mica ca 0: removeBarrier");
            BarriertoMoveIn = 0;
        }
    }
    public void resetHexBarrier()
    {
        listOfObstructions.Clear();
    }

    public Hex(Map map,int q, int r, int barrier, Player player, int terrain)
    {
        this.map = map;
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
        this.BarriertoMoveIn = barrier;
        this.player = player;
        this.terrain = terrain;
        listOfObstructions = new List<int>();
    }


    public float getHexHeight()
    {
        return 1f * 2;
    }

    public float getHexWidth()
    {
        return grosimeHexC * getHexHeight();
    }
    public float getZSpacing()
    {
        return getHexHeight() * 0.75f;
    }

    public float getXSpacing()
    {
        return getHexWidth();
    }

    public float getOffsetZ()
    {
        return getHexHeight() * 0.75f * this.R;
    }
    
    public float getOffsetX()
    {
        return (this.Q + this.R / 2f) * getHexWidth();
    }

    public Vector3 Position()
    {
        return new Vector3(getOffsetX(), 0 , getOffsetZ());
    }

    public Vector3 PositionFromCamera()
    {
        return map.GetHexPosition(this);
    }

    public Vector3 Position2()
    {
        return PositionFromCamera(Camera.main.transform.position,
                map.getNrLinii(),
                map.getNrCol());
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numOfRows, float numOfCol)
    {
        float mapHeight = numOfRows * getZSpacing();
        float mapWidth = numOfCol * getXSpacing();

        Vector3 position = Position();

        float xHexesFromCamera = (position.x - cameraPosition.x) / mapWidth;

        if(Mathf.Abs(xHexesFromCamera) <= 0.5f)
        {
            return position;
        }

        if(xHexesFromCamera > 0)
        {
            xHexesFromCamera += 0.5f;
        }
        else
        {
            xHexesFromCamera -= 0.5f;
        }
        int aux = (int)xHexesFromCamera;
        position.x -= aux * mapWidth;

        return position;
    }
    public void AddUnit(Unit unit)
    {
        if(this.unit == null)
            this.unit = unit;
        else
        {
            Debug.Log("Added 2 units to a hex:Hex--AddUnit");
        }
    }

    public void RemoveUnit()
    {
        this.unit = null;
    }
    public Unit getUnit()
    {
        if (this.unit != null)
            return unit;
        return null;
    }
    public void ChangeOwner(Player player)
    {
        if (this.player != null)
        {
            this.player.RemoveHex(this);
        }

        this.player = player;
        player.ObtainHex(this);

        GameObject hGO = map.GetGameObjectFromHex(this);
        MeshRenderer meshRenderer = hGO.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = map.PlayerMaterial[player.getId()];
    }

    public bool CanUnitMoveHere(Unit unit)
    {
        Hex prevHex = unit.getHex();
        if (this.Q == prevHex.Q && this.R == prevHex.R)
            return false;

        List<Hex> array = map.GetHexesWithinRangeOf(prevHex, 1);
        if (!array.Contains(this))
        {
            return false;
        }
        if (terrain == 3)
            return false;
        return true;
    }

    public bool CanUnitSpawnHere()
    {
        if(terrain == 3)
        {
            return false;
        }
        if(this.unit != null)
        {
            return false;
        }
        if (getPlayerId() != map.getCurrentPlayer())
            return false;

        return true;
    }

    public bool CanFirstUnitSpawnHere(int playerId)
    {
        if (terrain == 3)
        {
            return false;
        }
        if (playerId == 0)
            return true;

        List<Hex> lista = map.GetHexesWithinRangeOf(this, map.getNrCol() / (2 * map.maxplayers + 1));
        for (int i = 0; i < playerId; i++)
            if (lista.Contains(map.GetPlayerI(i).homeHex))
                return false;

        return true;
    }

    public int UnitToMoveBarrier()
    {
        //return BarriertoMoveIn;
        int max = 0;
        foreach (int i in listOfObstructions)
            if (i > max)
                max = i;
        return max;
    }

    public int getPlayerId()
    {
        if (player == null)
            return -1;

        return player.getId();
    }
    public Player GetPlayer()
    {
        return player;
    }

    public bool hasUnit()
    {
        if (unit == null)
            return false;
        return true;
    }

    public bool isHexOfCurrentPlayer()
    {
        if (getPlayerId() == map.getCurrentPlayer())
            return true;
        return false;
    }

    public bool isHexOfPlayerId(int id)
    {
        if (getPlayerId() == id)
            return true;
        return false;
    }

    public bool isHexOfThisUnit(int id)
    {
        if (getPlayerId() == id)
            return true;
        return false;
    }
    public bool isHexNeutral()
    {
        if (player == null)
            return true;
        return false;
    }

}
