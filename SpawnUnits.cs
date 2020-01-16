using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnUnits : MonoBehaviour
{
    public Map map;
    public CameraController cam;
    public OpenPanelTutorial tutorial;

    private void Start()
    {
        tutorial = GameObject.FindObjectOfType<OpenPanelTutorial>();
    }

    public void SpawnArcher()
    {
        if (map == null)
        {
            map = GameObject.FindObjectOfType<Map>();
        }

        if(cam == null)
        {
            cam = GameObject.FindObjectOfType<CameraController>();
        }
        if (map.AllPlayersCreated())
        {
            if (cam.selectat)
            {
                tutorial.OpenPanel("Archer Spawn Selected, choose one of your free hexes to spawn your archer.");
                cam.ChangeCameraModeToSpawnArcher();
            }
        }
        else
        {
            tutorial.OpenPanel("Your first unit will be an archer, select the a hex to be your capital.");
            map.UnitToSpawn = 0;
        }
    }

    public void SpawnDwarf()
    {

        if (map == null)
        {
            map = GameObject.FindObjectOfType<Map>();
        }

        if (cam == null)
        {
            cam = GameObject.FindObjectOfType<CameraController>();
        }
        if (map.AllPlayersCreated())
        {
            if (cam.selectat)
            {
                tutorial.OpenPanel("Soldier Spawn Selected, choose one of your free hexes to spawn your soldier");
                cam.ChangeCameraModeToSpawnDwarf();
            }
        }
        else
        {
            tutorial.OpenPanel("Your first unit will be a soldier, select the a hex to be your capital.");
            map.UnitToSpawn = 1;
        }
    }

    public void SpawnHorse()
    {
        if (map == null)
        {
            map = GameObject.FindObjectOfType<Map>();
        }

        if (cam == null)
        {
            cam = GameObject.FindObjectOfType<CameraController>();
        }
        if (map.AllPlayersCreated())
        {
            if (cam.selectat)
            {
                tutorial.OpenPanel("Horse Spawn Selected, choose one of your free hexes to spawn your horse");
                cam.ChangeCameraModeToSpawnHorse();
            }
        }
        else
        {
            tutorial.OpenPanel("Your first unit will be a horse, select the a hex to be your capital.");
            map.UnitToSpawn = 2;
        }
    }
}
