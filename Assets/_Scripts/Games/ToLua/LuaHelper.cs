﻿using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using LuaInterface;

public static class LuaHelper {
	/// <summary>
	/// getType
	/// </summary>
	/// <param name="classname"></param>
	/// <returns></returns>
	public static System.Type GetType(string classname) {
		Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
		System.Type t = null;
		t = assb.GetType(classname);
		return t;
	}
	
	public static long GetTime() {
		TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)ts.TotalMilliseconds;
	}

	/// <summary>
	/// 搜索子物体组件-GameObject版
	/// </summary>
	public static T Get<T>(GameObject go, string subnode) where T : Component {
		if (go != null) {
			Transform sub = go.transform.Find(subnode);
			if (sub != null) return sub.GetComponent<T>();
		}
		return null;
	}

	/// <summary>
	/// 搜索子物体组件-Transform版
	/// </summary>
	public static T Get<T>(Transform go, string subnode) where T : Component {
		if (go != null) {
			Transform sub = go.Find(subnode);
			if (sub != null) return sub.GetComponent<T>();
		}
		return null;
	}

	/// <summary>
	/// 搜索子物体组件-Component版
	/// </summary>
	public static T Get<T>(Component go, string subnode) where T : Component {
		return go.transform.Find(subnode).GetComponent<T>();
	}

	/// <summary>
	/// 添加组件
	/// </summary>
	public static T Add<T>(GameObject go) where T : Component {
		if (go != null) {
			T[] ts = go.GetComponents<T>();
			for (int i = 0; i < ts.Length; i++) {
				if (ts[i] != null) GameObject.Destroy(ts[i]);
			}
			return go.gameObject.AddComponent<T>();
		}
		return null;
	}

	/// <summary>
	/// 添加组件
	/// </summary>
	public static T Add<T>(Transform go) where T : Component {
		return Add<T>(go.gameObject);
	}

	/// <summary>
	/// 查找子对象
	/// </summary>
	public static GameObject Child(GameObject go, string subnode) {
		return Child(go.transform, subnode);
	}

	/// <summary>
	/// 查找子对象
	/// </summary>
	public static GameObject Child(Transform go, string subnode) {
		Transform tran = go.Find(subnode);
		if (tran == null) return null;
		return tran.gameObject;
	}

	/// <summary>
	/// 取平级对象
	/// </summary>
	public static GameObject Peer(GameObject go, string subnode) {
		return Peer(go.transform, subnode);
	}

	/// <summary>
	/// 取平级对象
	/// </summary>
	public static GameObject Peer(Transform go, string subnode) {
		Transform tran = go.parent.Find(subnode);
		if (tran == null) return null;
		return tran.gameObject;
	}

	/// <summary>
	/// 清理内存
	/// </summary>
	public static void ClearMemory() {
		GC.Collect(); Resources.UnloadUnusedAssets();
		GameLuaClient mgr = (GameLuaClient)GameLuaClient.Instance;
		if (mgr != null) mgr.LuaGC();
	}

	/// <summary>
	/// 网络可用
	/// </summary>
	public static bool NetAvailable {
		get {
			return Application.internetReachability != NetworkReachability.NotReachable;
		}
	}

	/// <summary>
	/// 是否是无线
	/// </summary>
	public static bool IsWifi {
		get {
			return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
		}
	}
	
	public static void Log(string str) {
		Debug.Log(str);
	}

	public static void LogWarning(string str) {
		Debug.LogWarning(str);
	}

	public static void LogError(string str) {
		Debug.LogError(str);
	}
}