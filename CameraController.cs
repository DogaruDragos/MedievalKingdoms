using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Update_CurrentFunction = Update_DetectModeStart;
        map = GameObject.FindObjectOfType<Map>();
        selectat = true;
        miscari = 0;
        arrow = true;
        cancelTurn = false;
        tutorial = GameObject.FindObjectOfType<OpenPanelTutorial>();
    }

    public OpenPanelTutorial tutorial;
    public Map map;
    public TextMeshProUGUI MiscariAleUnitatiiCurente;

    Unit selectedUnit;
    Hex hexUnderMouse;
    Hex lastHexUnderMouse;

    int mouseDragThreshold = 4;

    public LayerMask LayerIdForHexTiles;

    Vector3 lastPosition;

    Vector3 lastGroundClickPosition;
    //bool isDraggingCamera = false;

    public float perspectiveZoomSpeed = .5f;
    public float orthoZoomSpeed = .5f;

    delegate void UpdateFunction();
    UpdateFunction Update_CurrentFunction;

    public void ChangeCameraModeToSpawnArcher()
    {
        Update_CurrentFunction = Update_UnitSpawnArcher;
    }

    public bool DoIClickButton()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            return false;
        return true;
    }

    void Update_CreatePlayers()
    {
        if (map.AllPlayersCreated() || map.UnitToSpawn==-1)
            Update_CurrentFunction = Update_DetectModeStart;

        else if (Input.GetMouseButtonUp(0) && !DoIClickButton())
        {
            Hex hex = MouseToHex();
            if (hex != null && hex.CanFirstUnitSpawnHere(map.getCurrentPlayer()))
            {
                int currentPlayer = map.getCurrentPlayer();
                map.createPlayer(hex);
                if (map.UnitToSpawn == 0)
                    map.SpawnFirstUnitAt(new Unit(0, 0, 1), map.ArcherPrefab, hex, map.GetPlayerI(currentPlayer));

                if (map.UnitToSpawn == 1)
                    map.SpawnFirstUnitAt(new Unit(0, 1, 0), map.DwarfPrefab, hex, map.GetPlayerI(currentPlayer));

                if (map.UnitToSpawn == 2)
                    map.SpawnFirstUnitAt(new Unit(1, 0, 0), map.HorsePrefab, hex, map.GetPlayerI(currentPlayer));
                map.UnitToSpawn = -1;
            }
            else if(hex != null)
            {
                tutorial.OpenPanel("You can't have two capitals this close too eachother, or a capital on water.You now have to reselect the unit to spawn.");
                map.UnitToSpawn = -1;
            }
        }
    }

    public void ChangeCameraModeToSpawnDwarf()
    {
        Update_CurrentFunction = Update_UnitSpawnDwarf;
    }

    public void ChangeCameraModeToSpawnHorse()
    {
        Update_CurrentFunction = Update_UnitSpawnHorse;
    }


    IEnumerator FlashHexes(List<Hex> hexes)
    {
        Dictionary<Hex, Color32> dict=new Dictionary<Hex, Color32>();
        foreach(Hex hex in hexes)
        {
            GameObject hGO = map.GetGameObjectFromHex(hex);
            dict.Add(hex, hGO.GetComponentInChildren<MeshRenderer>().material.color);
            hGO.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
        yield return new WaitForSeconds(.5f);
        foreach (Hex hex in hexes)
        {
            GameObject hGO = map.GetGameObjectFromHex(hex);
            if (hGO.GetComponentInChildren<MeshRenderer>().material.color == Color.yellow)
                hGO.GetComponentInChildren<MeshRenderer>().material.color = dict[hex];
            else
                hGO.GetComponentInChildren<MeshRenderer>().material.color = map.PlayerMaterial[hex.getPlayerId()].color;
        }
    }

    void Update_DetectModeStart()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
        else if (Input.GetMouseButtonUp(0) && !DoIClickButton())
        {
            if (!map.AllPlayersCreated() && map.UnitToSpawn != -1)
            {
                Update_CurrentFunction = Update_CreatePlayers;
                Update_CurrentFunction();
            }
            else
            {
                Hex hex = MouseToHex();
                if (hex != null)
                {
                    //Debug.Log("Mouse UP");
                    selectedUnit = hex.getUnit();

                    //flash archerrrrrrrrrrrrrrrrrrrrrrrrrrrr

                    if(selectedUnit != null)
                    {
                        tutorial.OpenPanel("Unit levels are :" + selectedUnit.getLevel(0).ToString() + "--" + selectedUnit.getLevel(1).ToString() + "--" + selectedUnit.getLevel(2).ToString());
                    }

                    if (selectedUnit != null && selectedUnit.getLevel(0) > 0)
                    {
                        StartCoroutine(FlashHexes(map.GetHexesWithinRangeOf(hex, selectedUnit.getLevel(0))));
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                }
                if (selectedUnit != null && selectedUnit.getPlayer().getId() == map.getCurrentPlayer())
                {
                    Update_CurrentFunction = Update_UnitMovement;
                }
                else
                {
                    tutorial.OpenPanel("Select one of your units to command or a new unit to spawn in one of your free hexes.");
                }
            }
            
        }
        else if (Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, lastPosition) > mouseDragThreshold)
        {

            Update_CurrentFunction = Update_CameraDrag;
            lastGroundClickPosition = TouchToGroundPlane(Input.mousePosition);
            Update_CurrentFunction();
        }
        if (map.AllPlayersCreated())
        {
            if (map.getCurrentPlayerObject().SkipPlayerTurn())
                map.nextPlayer();
        }
    }


    void Update()
    {
  //      hexUnderMouse = MouseToHex();
 //       if(Input.touchCount == 0)
 //       {
  //          CancelUpdateFuntion();
  //      }
        if(Update_CurrentFunction == null)
        {
            Update_CurrentFunction = Update_DetectModeStart;
        }
        Update_CurrentFunction();

        lastPosition = Input.mousePosition;

 //       lastHexUnderMouse = hexUnderMouse;
    }

    void CancelUpdateFuntion()
    {
        Update_CurrentFunction = Update_DetectModeStart;
        selectedUnit = null; 
        //Debug.Log("CancelUpdateFuntion()");
    }


    public Hex MouseToHex()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerIdForHexTiles.value;

        if(Physics.Raycast(mouseRay, out hitInfo,Mathf.Infinity, layerMask))
        {

            GameObject hexGO = hitInfo.rigidbody.gameObject;
            
            return map.GetHexFromGameObject(hexGO);

        }
        return null;
    }

    Vector3 TouchToGroundPlane(Vector3 mousePo)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePo);
        if (mouseRay.direction.y >= 0)
        {
            Debug.LogError("how?");
            return Vector3.zero;
        }
        float rayLength = mouseRay.origin.y / mouseRay.direction.y;
        Vector3 hitPosition = mouseRay.origin - (mouseRay.direction * rayLength);

        return hitPosition;
    }

    public bool selectat;
    public int miscari;
    public bool arrow;
    public bool cancelTurn;

    public GameObject bloodEffect;

    void Update_UnitMovement()
    {
        //Debug.Log("Unit moving");
        if (selectedUnit != null)
        {
            if (selectat)
            {
                miscari = selectedUnit.getLevel(2) + 1;
                selectat = false;
            }
            if (Input.GetMouseButtonUp(0) && !DoIClickButton())
            {
                Hex hex = MouseToHex();
                if (hex != null)
                {
    
                    if(arrow && selectedUnit.getLevel(0) > 0 && hex.getUnit() != null && hex.getUnit().getPlayer()!=selectedUnit.getPlayer() && map.GetHexesWithinRangeOf(selectedUnit.getHex(), selectedUnit.getLevel(0)).Contains(hex))
                    {
                        if (hex.getUnit().isArcherSoldier)
                        {
                            tutorial.OpenPanel("Catapult units have to be attacked twice by archers to be destroyed");
                            hex.getUnit().isArcherSoldier = false;
                        }
                        else
                        {
                            map.DeleteUnit(hex.getUnit());
                        }
                        arrow = false;
                        miscari = 0;

                        //Blood effect
                        GameObject kGO=map.GetGameObjectFromHex(hex);
                        Instantiate(bloodEffect, kGO.transform.position, Quaternion.Euler(-90, 0, 0), kGO.transform);
                        //hex = selectedUnit.getHex(); what was this line for?
                    }

                    if (hex.CanUnitMoveHere(selectedUnit) && arrow)//verifica daca nu e apa,daca e la 1 patratica si daca nu e acelasi hex
                    {
                        bool unIf = true;
                        if(unIf && !hex.isHexOfCurrentPlayer() && hex.UnitToMoveBarrier() > 0 && hex.UnitToMoveBarrier() > selectedUnit.getLevel(1))//dwarf compare barrier =  means dwarf can kill dwarf
                        {
                            //map.DeleteUnit(selectedUnit);
                            //miscari = 0;
                            tutorial.OpenPanel("That hex is protected by an enemy soldier, you can't move there unless you have a soldier level >= " + hex.UnitToMoveBarrier().ToString());
                            unIf = false;
                        }
                        if(unIf && hex.hasUnit() && !hex.isHexOfCurrentPlayer())
                        {
                            map.DeleteUnit(hex.getUnit());
                            selectedUnit.Move(hex);
                            miscari = 0;
                            unIf = false;


                            //Blood effect
                            GameObject kGO = map.GetGameObjectFromHex(hex);
                            Instantiate(bloodEffect, kGO.transform.position, Quaternion.Euler(-90, 0, 0), kGO.transform);
                        }
                        if (unIf && hex.hasUnit() && hex.isHexOfCurrentPlayer())
                        {
                            //AbsorbUnit
                            if (hex.getUnit().CanAbsorbUnit(selectedUnit))
                            {
                                hex.getUnit().removeDwarfField();
                                /////////////hex.getUnit().AbsorbUnit(selectedUnit);
                                selectedUnit.CreateNewUnitFromUnits(selectedUnit, hex.getUnit());
                                miscari = 0;
                                map.DeleteUnit(selectedUnit);
                                hex.getUnit().createDwarfField();
                            }
                            else
                            {
                                Debug.Log("Unitatea nu poate fii absorbita");
                                //TODO messaj nu se pot crea unitati cu toate 3 upgradeurile
                                tutorial.OpenPanel("You can't create units with all 3 upgrades");
                            }
                            unIf = false;
                        }
                        if(unIf && hex.isHexNeutral())
                        {
                            miscari = 0;
                            selectedUnit.Move(hex);
                            tutorial.OpenPanel("After Conquering a neutral hex your turn will end, even if you still have moves left.");
                            unIf = false;
                        }
                        if (unIf && !hex.isHexOfCurrentPlayer())
                        {
                            if (!selectedUnit.IsThisUnitHorseArcher()) {
                                //miscari = 0; //Cai se pot misca in terenul inamic
                                if (selectedUnit.IsThisUnitHorseSoldier())
                                {
                                    miscari--;
                                    hex.resetHexBarrier();
                                    selectedUnit.Move(hex);
                                    unIf = false;
                                }
                                else
                                {
                                    miscari = 0;
                                    selectedUnit.Move(hex);
                                    tutorial.OpenPanel("Only Horse Soldier units can conquer multiple enemy hexes.");
                                }
                            }
                            else
                            {
                                tutorial.OpenPanel("Horse Archer units can't conquer enemy players land");
                            }
                        }
                        if(unIf && hex.isHexOfCurrentPlayer())
                        {
                            miscari--;
                            selectedUnit.Move(hex);
                            unIf = false;
                        }

                        if (selectedUnit == null)
                        {
                            Debug.Log("Unitatea selectata este nula");
                        }
                    }
                    else
                    {
                        if (miscari == selectedUnit.getLevel(2) + 1 && hex.terrain == 3)
                        {
                            miscari = 0;//FISHY to verify
                            tutorial.OpenPanel("Clicking on water takes you back to unit selection and camera movement, but only if the selected unit hasn't moved.");
                            CancelUpdateFuntion();//TODO: message
                            cancelTurn = true;
                            selectat = true;
                            Debug.Log("A fost dat click pe apa");
                        }
                        else
                        {
                            tutorial.OpenPanel("A unit has to move one hex at a time. Units can't move in water hexes.");
                        }
                    }
                }
            }
            if(miscari == 0 && !cancelTurn)
            {
                selectat = true;
                arrow = true;
                map.nextPlayer();
                CancelUpdateFuntion();
            }
            else
            {
                if (cancelTurn)
                    cancelTurn = false;
            }
        }
        if(miscari == 0)
            MiscariAleUnitatiiCurente.text = "";
        else
            MiscariAleUnitatiiCurente.text = miscari.ToString();
    }
    void Update_UnitSpawnArcher()
    {
        if (Input.GetMouseButtonUp(0) && !DoIClickButton())
        {
            Hex hex = MouseToHex();
            if (hex != null)
            {
                if (hex.CanUnitSpawnHere() && map.getCurrentPlayerObject().CanGetMoneyForArcher())
                {
                    Unit archer = new Unit(0, 0, 1);
                    map.SpawnUnitOnHex(archer, map.ArcherPrefab, hex, map.getCurrentPlayerObject());
                    map.nextPlayer();
                    CancelUpdateFuntion();
                }
                else
                {
                    tutorial.OpenPanel("This hex is not owned by you or it has a unit in it. Archer Spawn was deselected. If you still wish to spawn an archer you have to reselect it.");
                    CancelUpdateFuntion();
                }
            }
        }
    }

    void Update_UnitSpawnDwarf()
    {
        if (Input.GetMouseButtonUp(0) && !DoIClickButton())
        {
            Hex hex = MouseToHex();
            if (hex != null)
            {
                if (hex.CanUnitSpawnHere() && map.getCurrentPlayerObject().CanGetMoneyForDwarf())
                {
                    Unit dwarf = new Unit(0, 1, 0);
                    map.SpawnUnitOnHex(dwarf, map.DwarfPrefab, hex, map.getCurrentPlayerObject());
                    dwarf.createDwarfField();
                    map.nextPlayer();
                    CancelUpdateFuntion();
                }
                else{
                    tutorial.OpenPanel("This hex is not owned by you or it has a unit in it. Soldier Spawn was deselected. If you still wish to spawn a soldier you have to reselect it.");
                    CancelUpdateFuntion();
                }
            }
        }
    }

    void Update_UnitSpawnHorse()
    {
        if (Input.GetMouseButtonUp(0) && !DoIClickButton())
        {
            Hex hex = MouseToHex();
            if (hex != null)
            {
                if (hex.CanUnitSpawnHere() && map.getCurrentPlayerObject().CanGetMoneyForHorse())
                {
                    Unit horse = new Unit(1, 0, 0);
                    map.SpawnUnitOnHex(horse, map.HorsePrefab, hex, map.getCurrentPlayerObject());
                    map.nextPlayer();
                    CancelUpdateFuntion();
                }
                else
                {
                    tutorial.OpenPanel("This hex is not owned by you or it has a unit in it. Horse Spawn was deselected. If you still wish to spawn a horse you have to reselect it.");
                    CancelUpdateFuntion();
                }
            }
        }
    }

    // Update is called once per frame
    void Update_CameraDrag()
    {
        //if(Input.touchCount == 0)
        //Debug.Log("Dragging");
        if (Input.GetMouseButtonUp(0))
        {
            CancelUpdateFuntion();
            return;
        }

        Vector3 hitPosition = TouchToGroundPlane(Input.mousePosition);


        //Camera.main.Scree
        Vector3 diff = lastGroundClickPosition - hitPosition;
        Camera.main.transform.Translate(diff, Space.World);
        lastGroundClickPosition = hitPosition;

        lastGroundClickPosition = hitPosition = TouchToGroundPlane(Input.mousePosition);

        //Zoom in----Zoom out
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudediff = prevTouchDeltaMag - touchDeltaMag;
            deltaMagnitudediff = 0f - (deltaMagnitudediff / 1000);

            Vector3 direction = hitPosition - Camera.main.transform.position;

            Camera.main.transform.Translate(direction * deltaMagnitudediff, Space.World);
        }

        Vector3 newPosition = Camera.main.transform.position;

        if (newPosition.y < 3)
        {
            newPosition.y = 3;
        }
        if (newPosition.z < -1)
        {
            newPosition.z = -1;
        }

        if (newPosition.y > map.getNrLinii())
        {
            newPosition.y = map.getNrLinii();
        }

        if (newPosition.z > (map.getNrLinii() - newPosition.y) * 1.5f)
        {
            newPosition.z = (map.getNrLinii() - newPosition.y) * 1.5f;
        }

        Camera.main.transform.position = newPosition;
    }
}
