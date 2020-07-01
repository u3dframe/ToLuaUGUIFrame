using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using LuaInterface;

public sealed class LuaHelper  : UtilityHelper {
	/// <summary>
	/// 清理内存
	/// </summary>
	static public void ClearMemory() {
		GC.Collect(); Resources.UnloadUnusedAssets();
		GameLuaClient mgr = (GameLuaClient)GameLuaClient.Instance;
		if (mgr != null) mgr.LuaGC();
	}

	/// <summary>
	/// 最多9个参数
	/// </summary>
	static public bool CFuncLua(string funcName, params object[] args) {
		GameLuaClient mgr = (GameLuaClient)GameLuaClient.Instance;
		if (mgr != null) { return mgr.CFuncLua(funcName,args); } 
		return false;
	}
}