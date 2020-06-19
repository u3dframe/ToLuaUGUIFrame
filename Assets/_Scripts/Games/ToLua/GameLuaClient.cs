using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class GameLuaClient : LuaClient
{
    protected override LuaFileUtils InitLoader()
    {
        return new LuaFileLoader();
    }

    protected override void OpenLibs()
    {
        base.OpenLibs();
		InitSelfLibs();
    }
	
	void InitSelfLibs()
	{
		/*
        luaState.BeginPreLoad();
        luaState.RegFunction("sproto.core", luaopen_sproto_core);
        luaState.EndPreLoad();
		*/
	}
	
	protected override void LoadLuaFiles()
	{
		InitLuaPath();
		base.LoadLuaFiles();
	}

	public void LuaGC(){
		luaState.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
	}
	
	/// <summary>
	/// 初始化Lua代码加载路径
	/// </summary>
	void InitLuaPath() {
		string rootPath = Application.dataPath;
#if UNITY_EDITOR
		luaState.AddSearchPath(rootPath + "/ToLua/Lua");
#else
		rootPath = Application.dataPath;
#endif
		luaState.AddSearchPath(rootPath + "/Lua");
	}
}
