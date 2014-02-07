using UnityEngine;
using System.Collections;
using System;

public class SpawnPoint : Building {

    public float radius = 20.0f;
    public float time = 3;

    private DateTime currentTime = DateTime.Now;

    protected override void Tick()
    {
        base.Tick();
        if (currentTime.Second > (System.DateTime.Now.Second - time))
        {
            Debug.Log("Poo");
        }

    }

}
