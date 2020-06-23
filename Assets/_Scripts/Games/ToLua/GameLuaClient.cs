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

	public bool CFuncLua(string funcName, params object[] args) {
		LuaFunction func = luaState.GetFunction(funcName);
		if (func != null) {
			int lens = 0;
			if(args != null){
				lens = args.Length;
			}
			switch(lens){
				case 1:
					func.Call(args[0]);
					break;
				case 2:
					func.Call(args[0],args[1]);
					break;
				case 3:
					func.Call(args[0],args[1],args[2]);
					break;
				case 4:
					func.Call(args[0],args[1],args[2],args[3]);
					break;
				case 5:
					func.Call(args[0],args[1],args[2],args[3],args[4]);
					break;
				case 6:
					func.Call(args[0],args[1],args[2],args[3],args[4],args[5]);
					break;
				case 7:
					func.Call(args[0],args[1],args[2],args[3],args[4],args[5],args[6]);
					break;
				case 8:
					func.Call(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7]);
					break;
				case 9:
					func.Call(args[0],args[1],args[2],args[3],args[4],args[5],args[6],args[7],args[8]);
					break;
				default:
					func.Call();
					break;
			}
			return true;
		}
		return false;
	}

	/**
	public object[] CallFunction(string funcName, params object[] args) {
		LuaFunction func = luaState.GetFunction(funcName);
		if (func != null) {
			return func.LazyCall(args);
		}
		return null;
	}

	public static object[] CallMethod(string module, string func, params object[] args) {
		return CallFunction(module + "." + func, args);
	}
	*/
}
