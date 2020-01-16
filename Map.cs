using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Map : MonoBehaviour
{
    public Helper helper;
    public int UnitToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        UnitToSpawn = -1;
        if (StaticInformation.isHugeMap)
        {
            nrCol = 60;
            nrLinii = 30;
        }

        if (StaticInformation.isMediumMap)
        {
            nrCol = 40;
            nrLinii = 20;
        }
        if (StaticInformation.isSmallMap)
        {
            nrCol = 20;
            nrLinii = 10;
        }
        maxplayers = StaticInformation.NumberOfPlayers;
        GenerateMap();
        players = new List<Player>();
        
    }

    public void createPlayer(Hex home)
    {
        int i = players.Count;
        if(i < maxplayers)
            players.Add(new Player(i,home));
        currentPlayer++;
        if (currentPlayer >= maxplayers)
            currentPlayer = 0;
    }
    public bool AllPlayersCreated()
    {
        return players.Count == maxplayers;
    }

    public TextMeshProUGUI PlayerAtTurn;
    public TextMeshProUGUI CurrentPlayerWood;
    public TextMeshProUGUI CurrentPlayerIron;
    public TextMeshProUGUI CurrentPlayerFood;

    public TextMeshProUGUI WoodPerTurn;
    public TextMeshProUGUI IronPerTurn;
    public TextMeshProUGUI FoodPerTurn;


    void Update()
    {
        if (AllPlayersCreated())
        {
            if (players != null)
            {
                int[] money = players[currentPlayer].getMoney();
                int playerDeAfisat = currentPlayer + 1;

                int[] whatUGet = players[currentPlayer].getActualResourcesPerTurn();

                PlayerAtTurn.text = playerDeAfisat.ToString();

                if (money.Length == 3)
                {
                    CurrentPlayerWood.text = money[0].ToString();
                    CurrentPlayerIron.text = money[1].ToString();
                    CurrentPlayerFood.text = money[2].ToString();

                    WoodPerTurn.text = whatUGet[0].ToString();
                    IronPerTurn.text = whatUGet[1].ToString();
                    FoodPerTurn.text = whatUGet[2].ToString();

                }
            }

 //           foreach (Hex hex in matrix)
 //           {
 //               GameObject x = GetGameObjectFromHex(hex);
 //               if (!hex.isHexNeutral())
 //               {
 //                   x.GetComponentInChildren<TextMesh>().text = hex.UnitToMoveBarrier().ToString();
 //               }
 //           }
        }
    }
    private int currentPlayer = 0;
    public int maxplayers;
    List<Player> players;


    public GameObject DwarfPrefab;
    public GameObject ArcherPrefab;
    public GameObject HorsePrefab;
    public GameObject TowerPrefab;

    public GameObject HorseArcherPrefab;
    public GameObject HorseSoldierPrefab;
    public GameObject CatapultPrefab;

    public GameObject[] ForrestPrefabs;
    public GameObject[] MountainPrefabs;
    public GameObject WheatFieldPrefabHigh;
    public GameObject WheatFieldPrefabMedium;
    public GameObject WheatFieldPrefabLow;

    private int nrLinii;
    private int nrCol;

    public int getNrLinii() {
        return nrLinii;
    }

    public int getNrCol()
    {
        return nrCol;
    }

    public GameObject HexPrefab;

    public Material[] HexMaterial;
    public Material[] PlayerMaterial;

    //   public Material MatWater;
    //   public Material MatWheatField;
    //   public Material MatStone;
    //   public Material Forrest;

    private Hex[,] matrix;
    private Dictionary<Hex, GameObject> dictionary;
    private Dictionary<GameObject, Hex> dictionaryGOtoHEX;

    private HashSet<Unit> UnitsMatrix;
    private Dictionary<Unit, GameObject> UnitsDictionary;

    public Hex[,] getGrid()
    {
        return matrix;
    }

    virtual public void GenerateMap()
    {
        this.helper = new Helper(this);
        matrix = new Hex[getNrCol(), getNrLinii()];
        dictionary = new Dictionary<Hex, GameObject>();
        dictionaryGOtoHEX = new Dictionary<GameObject, Hex>();

        for (int col = 0;  col < getNrCol(); col++)
        {
            for (int row = 0; row < getNrLinii(); row++)
            {
                // Create one piece

                int random = Random.Range(0, HexMaterial.Length);
                Hex hexagon = new Hex(this, col, row, 0, null, random);
                matrix[col, row] = hexagon;

                Vector3 position = hexagon.PositionFromCamera(Camera.main.transform.position, getNrLinii(), getNrCol());

                GameObject hGO = (GameObject)Instantiate(HexPrefab,
                    position,
                    Quaternion.identity,
                    this.transform
                    );

                hGO.name = string.Format("HEX: {0},{1}", col, row);

                dictionary.Add(hexagon,hGO);
                dictionaryGOtoHEX.Add(hGO, hexagon);

                hGO.GetComponent<HexComponent>().hex = hexagon;
                hGO.GetComponent<HexComponent>().map = this;

                if (random == 0)
                {
                    int a = 0;
                    int b = 8;

                    if (StaticInformation.isMediumSettings)
                    {
                        a = 1;
                        b = 7;
                    }
                    if (StaticInformation.isLowSettings)
                    {
                        a = 0;
                        b = 6;
                    }
                    if (StaticInformation.isHighSettings)
                    {
                        a = 1;
                        b = 8;
                    }

                    int random2 = Random.Range(a, b);
                    if (StaticInformation.isHighSettings)
                    {
                        if (random2 == 3)
                            random2 = 1;
                        if (random2 == 5)
                            random2 = 7;
                    }

                    int random3 = Random.Range(0, 4);
                    //Random.bo
                   GameObject.Instantiate(ForrestPrefabs[random2], hGO.transform.position, Quaternion.Euler(0, random3 * 90f, 0), hGO.transform);
                }

                if (random == 1)
                {
                    int a = 0;
                    int b = 4;
                    int random4 = Random.Range(0, 4);
                    
                    if (StaticInformation.isMediumSettings)
                    {
                        a = 0;
                        b = 2;
                        random4 = Random.Range(0, 4);
                    }
                    if (StaticInformation.isLowSettings)
                    {
                        random4 = Random.Range(0, 8);
                        a = 0;
                        b = 2;
                    }
                    if (StaticInformation.isHighSettings)
                    {
                        a = 0;
                        b = 4;
                        random4 = Random.Range(0, 4);
                    }

                    int random2 = Random.Range(a, b);
                    int random3 = Random.Range(0, 4);
                    
                    if (random4 == 0)
                    {
                        GameObject.Instantiate(MountainPrefabs[random2], hGO.transform.position, Quaternion.Euler(0, random3 * 90f, 0), hGO.transform);
                    }

                }

                if(random == 2 && StaticInformation.isMediumSettings)
                {
                    GameObject.Instantiate(WheatFieldPrefabMedium, hGO.transform.position, Quaternion.identity, hGO.transform);
                }

                if (random == 2 && StaticInformation.isHighSettings)
                {
                    GameObject.Instantiate(WheatFieldPrefabHigh, hGO.transform.position, Quaternion.identity, hGO.transform);
                }

                if (random == 2 && StaticInformation.isLowSettings)
                {
                    GameObject.Instantiate(WheatFieldPrefabLow, hGO.transform.position, Quaternion.identity, hGO.transform);
                }

                //hGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", col, row);

                MeshRenderer meshRenderer = hGO.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material = HexMaterial[random];
            }
        }

    }

    public Hex getHexAt(int x, int y)
    {

        if (matrix == null || y < 0 || y >= getNrLinii())
        {
            return null;
        }

        if(x < 0)
        {
            while(x < 0)
            {
                x += getNrCol();
            }
        }
        return matrix[x % getNrCol(), y];
    }

    public GameObject lightEffect;

    public void SpawnUnitOnHex(Unit unit,GameObject prefab,Hex hex,Player player)
    {
        if (UnitsMatrix == null)
        {
            UnitsMatrix = new HashSet<Unit>();
            UnitsDictionary = new Dictionary<Unit, GameObject>();
        }

        if (hex.CanUnitSpawnHere())
        {

            player.ObtainUnit(unit);
            GameObject myHex = dictionary[hex];

            unit.SetHex(hex);
            GameObject unitGO = Instantiate(prefab, myHex.transform.position, Quaternion.identity, myHex.transform);
            Instantiate(lightEffect, myHex.transform.position, Quaternion.Euler(-90, 0, 0), myHex.transform);

            unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;
            UnitsMatrix.Add(unit);
            UnitsDictionary[unit] = unitGO;
        }
    }

    public bool SpawnFirstUnitAt(Unit unit, GameObject prefab, Hex hex, Player player)
    {
        if (UnitsMatrix == null)
        {
            UnitsMatrix = new HashSet<Unit>();
            UnitsDictionary = new Dictionary<Unit, GameObject>();
        }

        if (hex.CanFirstUnitSpawnHere(player.getId()))
        {
            player.ObtainUnit(unit);

            GameObject myHex = dictionary[hex];

            unit.SetHex(hex);
            GameObject unitGO = Instantiate(prefab, myHex.transform.position, Quaternion.identity, myHex.transform);
            Instantiate(lightEffect, myHex.transform.position, Quaternion.Euler(-90, 0, 0), myHex.transform);
            Instantiate(TowerPrefab, myHex.transform.position, Quaternion.identity, myHex.transform);


            unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;


            UnitsMatrix.Add(unit);
            UnitsDictionary[unit] = unitGO;
            return true;
        }
        else
            return false;
    }

    public void SpawnUnitAt(Unit unit,GameObject prefab,int r,int c , Player player)
    {
        Hex hex = getHexAt(r, c);
        if (UnitsMatrix == null)
        {
            UnitsMatrix = new HashSet<Unit>();
            UnitsDictionary = new Dictionary<Unit, GameObject>();
        }

        if (hex.CanUnitSpawnHere())
        {
            player.ObtainUnit(unit);

            GameObject myHex = dictionary[hex];

            unit.SetHex(hex);
            GameObject unitGO = Instantiate(prefab, myHex.transform.position, Quaternion.identity, myHex.transform);

            unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;


            UnitsMatrix.Add(unit);
            UnitsDictionary[unit] = unitGO;
        }
    }

    public Hex GetHexFromGameObject(GameObject hexGO)
    {
        if (dictionaryGOtoHEX.ContainsKey(hexGO))
            return dictionaryGOtoHEX[hexGO];
        else
            return null;
    }
    public GameObject GetGameObjectFromUnit(Unit unit)
    {
        if (UnitsDictionary.ContainsKey(unit))
            return UnitsDictionary[unit];
        else
            return null;
    }

    public GameObject GetGameObjectFromHex(Hex hex)
    {
        if (dictionary.ContainsKey(hex))
            return dictionary[hex];
        else
            return null;
    }

    public Vector3 GetHexPosition(Hex hex)
    {
        return hex.PositionFromCamera(Camera.main.transform.position, getNrLinii(), getNrCol());
    }

    public GameObject GameOver;

    public void nextPlayer()
    {
        Player CurrentPlayer = players[currentPlayer];
        int IdulHomeHexului = CurrentPlayer.homeHex.getPlayerId();

        if (IdulHomeHexului == currentPlayer)//nu am fost cotropit
        {
            CurrentPlayer.roundsWithouthHome = 0;
        }
        else
        {
            CurrentPlayer.roundsWithouthHome++;
            if (CurrentPlayer.roundsWithouthHome > 4)
            {
                Debug.Log("Player Over");
                CurrentPlayer.isInGame = false;

                currentPlayer++;
                if (currentPlayer >= maxplayers)
                {
                    currentPlayer = 0;
                }
                while (!players[currentPlayer].isInGame)
                {
                    currentPlayer++;
                    if (currentPlayer >= maxplayers)
                    {
                        currentPlayer = 0;
                    }
                }
                return;

            }


        }
        CurrentPlayer.DoTurn();
        CurrentPlayer.FeedTroups();


        currentPlayer++;
        if (currentPlayer >= maxplayers)
        {
            currentPlayer = 0;
        }
        while (!players[currentPlayer].isInGame)
        {
            currentPlayer++;
            if (currentPlayer >= maxplayers)
            {
                currentPlayer = 0;
            }
        }

        int nr = 0;
        foreach (Player playerIT in players)
        {
            if (playerIT.hexCount == 0)
                playerIT.isInGame = false;

            if (playerIT.isInGame == true)
                nr++;
        }
        if (nr == 1)
        {
            ///////
            GameOver.SetActive(true);
            GameOver.transform.Find("GameOverText").GetComponent<TextMeshProUGUI>().text = "Player " + (CurrentPlayer.getId() + 1).ToString() + " Won";
            Invoke("Application.Quit", 10f);
        }
    }

    public int getCurrentPlayer()
    {
        return this.currentPlayer;
    }
    public Player getCurrentPlayerObject()
    {
        return players[getCurrentPlayer()];
    }
    public Player GetPlayerI(int i)
    {
        return players[i];
    }

    public List<Hex> GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();
        if (range == -1)
            return results;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                if (dx + dy >= -range && dx + dy <= range)
                {
                    Hex hexAux = getHexAt(centerHex.Q + dx, centerHex.R + dy);
                    if (hexAux != null)
                        results.Add(hexAux);
                }
            }
        }

        return results;
    }
    public List<Hex> GetHexesAtRange(Hex centerHex, int range)
    {
        Hex hexAux;
        List<Hex> results = new List<Hex>();
        if (range == -1)
            return results;

        if (range == 0)
        {
            results.Add(centerHex);
            return results;
        }
        int i;

        for(i = 0; i < range; i++)
        {
            hexAux = getHexAt(centerHex.Q - range, centerHex.R + i);
            if (hexAux != null)
                results.Add(hexAux);
            hexAux = getHexAt(centerHex.Q + i, centerHex.R - range);
            if (hexAux != null)
                results.Add(hexAux);
        }

        for (i = -range + 1; i <= 0; i++)
        {
            hexAux = getHexAt(centerHex.Q + range, centerHex.R + i);
            if (hexAux != null)
                results.Add(hexAux);
            hexAux = getHexAt(centerHex.Q + i, centerHex.R + range);
            if (hexAux != null)
                results.Add(hexAux);
        }

        for (i = -range + 1; i < 0 ; i++)
        {
            int x = -range - i;
            hexAux = getHexAt(centerHex.Q + i, centerHex.R + x);
            if (hexAux != null)
                results.Add(hexAux);
        }

        hexAux = getHexAt(centerHex.Q - range, centerHex.R + range);
        if (hexAux != null)
            results.Add(hexAux);

        hexAux = getHexAt(centerHex.Q + range, centerHex.R - range);
        if (hexAux != null)
            results.Add(hexAux);


        for (i = 1; i < range; i++)
        {
            int x = range - i;
            hexAux = getHexAt(centerHex.Q + i, centerHex.R + x);
            if (hexAux != null)
                results.Add(hexAux);
        }

        return results;
    }

    public void DeleteUnit(Unit unit)
    {
        if (unit.getLevel(1) > 0)
            unit.removeDwarfField();

        GameObject unitGO = GetGameObjectFromUnit(unit);
        UnitsDictionary.Remove(unit);
        unit.getHex().RemoveUnit();
        unit.getPlayer().RemoveUnit(unit);//////
        UnitsMatrix.Remove(unit);
        Destroy(unitGO);
    }


    public HashSet<Unit> GetUnits()
    {
        if (UnitsMatrix == null)
            return null;
        return UnitsMatrix;
    }
}
