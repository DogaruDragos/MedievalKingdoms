using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    int id;
    int numberOfResources = 3;
    HashSet<Unit> units;
    HashSet<Hex> hexes;
    public int[] resources;
    public int[] money;
    public int[] resourcesConsumedPerTurn;

    public int roundsWithouthHome;
    public Hex homeHex;

    const int startingMoney = 50;
    const int pricePerUnit = 50;
    const int startingResourcesPerTurn = 0;
    const int homeMoney = 10;

    public int hexCount;
    public bool isInGame;

    public Player(int id, Hex home)
    {
        this.id = id;
        this.hexCount = 0;
        this.isInGame = true;
        this.roundsWithouthHome = 0;
        this.homeHex = home;
        units = new HashSet<Unit>();
        hexes = new HashSet<Hex>();
        resources = new int[numberOfResources];
        money = new int[numberOfResources];
        resourcesConsumedPerTurn = new int[numberOfResources];

        for (int i=0; i< numberOfResources; i++)
        {
            money[i] = startingMoney;
            resources[i] = startingResourcesPerTurn;
            resourcesConsumedPerTurn[i] = 0;
        }
    }
    public HashSet<Hex> GetHexes()
    {
        return hexes;
    }

    public int[] GetHowManyResourcesAreConsumedPerTurn()
    {
        int[] aux = new int[3];
        aux[0] = 0;
        aux[1] = 0;
        aux[2] = 0;

        foreach(Unit u in units)
        {
            aux[0] += (int)Mathf.Pow(3, u.getLevel(0));
            aux[1] += (int)Mathf.Pow(3, u.getLevel(1));
            aux[2] += (int)Mathf.Pow(3, u.getLevel(2));
        }
        return aux;
    }

    public void ObtainUnit(Unit unit)
    {
        unit.setPlayer(this);
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
        }
        else{
            Debug.Log("Unit is not owned by this player");
        }
    }

    public void ObtainHex(Hex hex)
    {
        hexCount++;
        hexes.Add(hex);
        incrementResources(hex.terrain);
    }
    
    public void RemoveHex(Hex hex)
    {
        hexes.Remove(hex);
        this.hexCount--;
        decrementResources(hex.terrain);
    }

    public int getId()
    {
        return id;
    }
    public int[] getResources()
    {
        return resources;
    }
    public int[] getMoney()
    {
        return money;
    }

    public int[] getActualResourcesPerTurn()
    {
        int[] aux = new int[numberOfResources];

        int[] aux2 = GetHowManyResourcesAreConsumedPerTurn();

        aux[0] = resources[0] - aux2[0];
        aux[1] = resources[1] - aux2[1];
        aux[2] = resources[2] - aux2[2];

        if (roundsWithouthHome == 0)
        {
            aux[0] += 10;
            aux[1] += 10;
            aux[2] += 10;
        }

        return aux;

    }
    public void incrementResources(int i)
    {
        //Debug.Log(System.String.Concat(i.ToString(), "resursa", resources[i].ToString()));
        resources[i]+=2;
    }

    public void decrementResources(int i)
    {
        resources[i]-=2;
    }

    public void DoTurn()
    {
        //Debug.Log("player turnnnn");
        for(int i=0;i < numberOfResources; i++)
        {
            if (roundsWithouthHome == 0)
                money[i] += homeMoney;
            

            money[i] += resources[i];
        }
    }

    public bool CanGetMoneyForArcher()
    {
        if (money[0] < pricePerUnit)
            return false;
        money[0] -= pricePerUnit;
        return true;
    }

    public bool CanGetMoneyForDwarf()
    {
        if (money[1] < pricePerUnit)
            return false;
        money[1] -= pricePerUnit;
        return true;
    }

    public bool CanGetMoneyForHorse()
    {
        if (money[2] < pricePerUnit)
            return false;
        money[2] -= pricePerUnit;
        return true;
    }

    public void FeedTroups()
    {
        List<Unit> toBeRemoved = new List<Unit>();
        foreach (Unit unit in units)
        {
            if (!CanFeedUnit(unit))
            {
                toBeRemoved.Add(unit);
            }
        }
        foreach (Unit u in toBeRemoved)
        {
            u.getHex().map.DeleteUnit(u);
        }
    }

    public bool CanFeedUnit(Unit u)
    {
        int wood = (int)Mathf.Pow(3, u.getLevel(0));
        int iron = (int)Mathf.Pow(3, u.getLevel(1));
        int food = (int)Mathf.Pow(3, u.getLevel(2));
        

        if (money[0] < wood)
            return false;
        if (money[1] < iron)
            return false;
        if (money[2] < food)
            return false;

        money[0] -= wood;
        money[1] -= iron;
        money[2] -= food;

        return true;
    }

    public bool SkipPlayerTurn()
    {
        if(units.Count == 0 && money[0] < pricePerUnit && money[1] < pricePerUnit && money[2] < pricePerUnit)
            return true;
        return false;
    }


}
