using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameMgr.instance.Init();
        GameObject gobj = new GameObject("LuaLooper",typeof(GameLuaClient));
        GameObject.DontDestroyOnLoad(gobj);
    }

    // Update is called once per frame
    // void Update()
    // {    
    // }
}
