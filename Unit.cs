using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Unit
{
    public Player player;
    public int HorseUpgrade;
    public int SoldierUpgrade;
    public int ArcherUpgrade;

    public Hex hex { get; protected set; }

    public delegate void UnitMovedDelegate(Hex oldhex, Hex newHex);

    public event UnitMovedDelegate OnUnitMoved;

    public bool isArcherSoldier;

    public Unit(int h, int s, int a)
    {
        this.HorseUpgrade = h;
        this.SoldierUpgrade = s;
        this.ArcherUpgrade = a;
        this.isArcherSoldier = false;
    }
    public Hex getHex()
    {
        return hex;
    }

    public bool IsThisUnitArcherSoldier()
    {
        if (getLevel(0) > 0 && getLevel(1) > 0)
            return true;
        return false;
    }

    public bool IsThisUnitHorseSoldier()
    {
        if (getLevel(2) > 0 && getLevel(1) > 0)
            return true;
        return false;
    }

    public bool IsThisUnitHorseArcher()
    {
        if (getLevel(2) > 0 && getLevel(0) > 0)
            return true;
        return false;
    }

    public int getLevel(int i)
    {
        if (i == 2)
            if (HorseUpgrade > 0)
                return (int)Math.Truncate(Math.Log(HorseUpgrade, 2)) + 2;
            else return 0;
        if (i == 1)
            if (SoldierUpgrade > 0)
                return (int)Math.Truncate(Math.Log(SoldierUpgrade, 2)) + 2;
            else return 0;
        else
            if (ArcherUpgrade > 0)
            return (int)Math.Truncate(Math.Log(ArcherUpgrade, 2)) + 2;
        return 0;
    }

    public void SetHex(Hex newHex)
    {
        newHex.ChangeOwner(player);
        Hex oldHex = this.hex;
        if (this.hex != null)
        {
            this.hex.RemoveUnit();
        }
        this.hex = newHex;

        this.hex.AddUnit(this);

        if (OnUnitMoved != null)
        {
            OnUnitMoved(oldHex, newHex);
        }
    }

    public void setPlayer(Player player)
    {
        this.player = player;
    }

    public Player getPlayer()
    {
        return this.player;
    }

    public int getPlayerId()
    {
        return player.getId();
    }

    public void DoTurn()
    {
        Hex oldhex = hex;
        int q = oldhex.Q + 2;
        if (q > oldhex.map.getNrCol())
            q = 0;
        Hex newhex = oldhex.map.getHexAt(q, oldhex.R);
        SetHex(newhex);
    }

    public bool CheckIfNotDamagedArcherSoldier(Unit u)
    {
        if (u.getLevel(0) > 0 && u.getLevel(1) > 0 && u.isArcherSoldier == false)
            return false;
        return true;
    }
    //0=archer 1=soldier 2=horse
    public bool CheckIfIsSimpleArcher(Unit unit)
    {
        if (unit.getLevel(1) == 0 && unit.getLevel(2) == 0)
            return true;
        return false;
    }

    public bool CheckIfIsSimpleSoldier(Unit unit)
    {
        if (unit.getLevel(0) == 0 && unit.getLevel(2) == 0)
            return true;
        return false;
    }

    public bool CheckIfIsSimpleHorse(Unit unit)
    {
        if (unit.getLevel(0) == 0 && unit.getLevel(1) == 0)
            return true;
        return false;
    }


    public void AbsorbUnit(Unit unit)
    {
        bool newIsArcherSoldier = isArcherSoldier || unit.isArcherSoldier;

        if (CheckIfIsSimpleSoldier(this) && CheckIfIsSimpleArcher(unit))
            newIsArcherSoldier = true;
        if (CheckIfIsSimpleSoldier(unit) && CheckIfIsSimpleArcher(this))
            newIsArcherSoldier = true;

        HorseUpgrade += unit.HorseUpgrade;
        SoldierUpgrade += unit.SoldierUpgrade;
        ArcherUpgrade += unit.ArcherUpgrade;
        isArcherSoldier = newIsArcherSoldier;

        UpdateUi();
    }

    public void CreateNewUnitFromUnits(Unit unitMoved,Unit thisUnit)
    {
        bool newIsArcherSoldier = thisUnit.isArcherSoldier || unitMoved.isArcherSoldier;


        bool isAS = thisUnit.IsThisUnitArcherSoldier();
        bool isHA = thisUnit.IsThisUnitHorseArcher();
        bool isHS = thisUnit.IsThisUnitHorseSoldier();

        if (thisUnit.CheckIfIsSimpleSoldier(thisUnit) && thisUnit.CheckIfIsSimpleArcher(unitMoved))
        {
            newIsArcherSoldier = true;
        }
        if (thisUnit.CheckIfIsSimpleSoldier(unitMoved) && thisUnit.CheckIfIsSimpleArcher(thisUnit))
        {
            newIsArcherSoldier = true;
        }

        thisUnit.HorseUpgrade += unitMoved.HorseUpgrade;
        thisUnit.SoldierUpgrade += unitMoved.SoldierUpgrade;
        thisUnit.ArcherUpgrade += unitMoved.ArcherUpgrade;
        thisUnit.isArcherSoldier = newIsArcherSoldier;

        int h = thisUnit.HorseUpgrade;
        int s = thisUnit.SoldierUpgrade;
        int a = thisUnit.ArcherUpgrade;


        Hex hex = thisUnit.getHex();
        if (thisUnit.IsThisUnitArcherSoldier() != isAS)
        {
            Unit u = new Unit(h, s, a);
            thisUnit.createDwarfField();
            hex.map.DeleteUnit(thisUnit);
            hex.map.SpawnUnitOnHex(u, hex.map.CatapultPrefab, hex, hex.map.getCurrentPlayerObject());
            u.isArcherSoldier = newIsArcherSoldier;
            u.UpdateUi();
        }
        else if(thisUnit.IsThisUnitHorseArcher() != isHA)
        {
            Unit u = new Unit(h, s, a);
            thisUnit.createDwarfField();
            hex.map.DeleteUnit(thisUnit);
            hex.map.SpawnUnitOnHex(u, hex.map.HorseArcherPrefab, hex, hex.map.getCurrentPlayerObject());
            hex.getUnit().UpdateUi();
        }
        else if(thisUnit.IsThisUnitHorseSoldier() != isHS)
        {
            Unit u = new Unit(h, s, a);
            thisUnit.createDwarfField();
            hex.map.DeleteUnit(thisUnit);
            hex.map.SpawnUnitOnHex(u, hex.map.HorseSoldierPrefab, hex, hex.map.getCurrentPlayerObject());
            u.UpdateUi();
        }
        else {
            thisUnit.UpdateUi();
        }

        
    }

    public void UpdateUi()
    {
        Hex h = getHex();
        if (h == null)
            Debug.Log("UpdateUi err");
        else
        {
            h.map.GetGameObjectFromUnit(this).GetComponentInChildren<Text>().text = getLevel(0).ToString() + "--" + getLevel(1).ToString() + "--" + getLevel(2).ToString();
            h.map.GetGameObjectFromUnit(this).GetComponentInChildren<Canvas>().transform.eulerAngles = new Vector3(30, 0, 0);
        }
    }

    public bool CanAbsorbUnit(Unit unit)
    {
        if (HorseUpgrade + unit.HorseUpgrade > 0 && SoldierUpgrade + unit.SoldierUpgrade > 0 && ArcherUpgrade + unit.ArcherUpgrade > 0)
            return false;
        return true;
    }

    public List<Hex> getListOfHexesInDwarfField(){
        int dwarfRange = getLevel(1) - 1;
        if (hex != null)
        {
            return hex.map.GetHexesWithinRangeOf(hex, dwarfRange);
        }
        return new List<Hex>();
    }

    public List<Hex> getListOfHexesOwnedInRange(int range)
    {
        if (hex != null)
        {
            return hex.map.GetHexesAtRange(hex, range);
        }
        return new List<Hex>();
    }


    public void createDwarfField()
    {
        int range = getLevel(1);
        if (range > 0)
        {
            for(int i = 0; i < range; i++)
            {
                List<Hex> list = getListOfHexesOwnedInRange(i);
                foreach(Hex hex in list)
                {
                    if (hex.isHexOfPlayerId(getPlayerId()))
                        hex.addObstruction(range - i);
                }
            }
        }
    }

    public void removeDwarfField()
    {
        int range = getLevel(1);
        if (range > 0)
        {
            for (int i = 0; i < range; i++)
            {
                List<Hex> list = getListOfHexesOwnedInRange(i);
                foreach (Hex hex in list)
                {
                    if (hex.isHexOfPlayerId(getPlayerId()))
                        hex.removeObstruction(range - i);
                }
            }
        }
    }
    public void Move(Hex hex)
    {
        UnitRotate(hex);
        removeDwarfField();
        getHex().RemoveUnit();
        SetHex(hex);
        createDwarfField();

        //
        UpdateUi();
    }

    public void UnitRotate(Hex hex)
    {

        int colMovement = hex.Q - this.getHex().Q;
        int rowMovement = hex.R - this.getHex().R;

        if (colMovement == getHex().map.getNrCol() - 1)
            colMovement = -1;
        if (colMovement == -(getHex().map.getNrCol() - 1))
            colMovement = 1;

        GameObject unitGO = getHex().map.GetGameObjectFromUnit(this);
        float currentAngle = unitGO.transform.eulerAngles.y;

        if (colMovement == 0 && rowMovement == -1)
        {
            //SV ---rotation set to 30  
            unitGO.transform.eulerAngles = new Vector3(0, 30, 0);
        }
        else if(colMovement == -1 && rowMovement == 0)
        {
            //V -- 90
            unitGO.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (colMovement == -1 && rowMovement == 1)
        {
            //NV 150
            unitGO.transform.eulerAngles = new Vector3(0, 150, 0);
        }
        else if (colMovement == 0 && rowMovement == 1)
        {
            //NE 210
            unitGO.transform.eulerAngles = new Vector3(0, 210, 0);
        }
        else if (colMovement == 1 && rowMovement == 0)
        {
            //E 270
            unitGO.transform.eulerAngles = new Vector3(0, 270, 0);
        }
        else if (colMovement == 1 && rowMovement == -1)
        {
            //SE 330
            unitGO.transform.eulerAngles = new Vector3(0, 330, 0);
        }
        else
        {
            Debug.Log("Unit Rotate fail");
        }



    }
}
