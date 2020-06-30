using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 类名 : 缓存需要操作的对象
/// 作者 : Canyon / 龚阳辉
/// 日期 : 2017-08-04 00:10
/// 功能 : 脚本控制用
/// </summary>
public class PrefabElement : GobjLifeListener {
	static public new PrefabElement Get(GameObject gobj,bool isAdd){
		PrefabElement _r = gobj.GetComponent<PrefabElement> ();
		if (isAdd && _r == null) {
			_r = gobj.AddComponent<PrefabElement> ();
		}
		return _r;
	}

	static public new PrefabElement Get(GameObject gobj){
		return Get(gobj,true);
	}

	// 自身对象
	Transform _m_trsf;
	
	/// <summary>
	/// 自身对象
	/// </summary>
	public Transform m_trsf
	{
		get{
			if(_m_trsf == null){
				_m_trsf = transform;
			}
			return _m_trsf;
		}
	}
	
	GameObject _m_gobj;
	
	/// <summary>
	/// 自身对象
	/// </summary>
	public GameObject m_gobj
	{
		get{
			if(_m_gobj == null){
				_m_gobj = gameObject;
			}
			return _m_gobj;
		}
	}
	
	/// <summary>
	/// 操作的对象
	/// </summary>
	[SerializeField]
	private GameObject[] m_gobjs;
	
	/// <summary>
	/// key = name or Relative name (相对应自身对象);
	/// val = gobj;
	/// </summary>
	Dictionary<string,GameObject> m_dicName2Gobj = new Dictionary<string,GameObject>();
	
	bool isInit = false;

	/// <summary>
	/// 隐藏回调
	/// </summary>
	public System.Action m_callHide;

	/// <summary>
	/// 显示回调
	/// </summary>
	public System.Action m_callShow;

	/// <summary>
	/// 销毁回调
	/// </summary>
	// public System.Action m_callDestroy;
	
	void Awake()
	{
		Init();
	}

	void  OnDisable()
	{
		if (m_callHide != null)
			m_callHide ();
	}	

	void  OnEnable()
	{
		Init ();

		if (m_callShow != null)
			m_callShow ();
	}

	// void  OnDestroy()
	// {
	// 	if (m_callDestroy != null)
	// 		m_callDestroy ();
	// }
	
	void Init()
	{
		if(m_gobjs == null)
			return;
		
		if(isInit)
			return;
		isInit = true;
		
		GameObject tmp = null;
		string _tmpName = "";
		for(int i = 0; i < m_gobjs.Length;i++)
		{
			tmp = m_gobjs[i];
			if(tmp){
				_tmpName = tmp.name;
				if(!m_dicName2Gobj.ContainsKey(_tmpName)){
					m_dicName2Gobj.Add(_tmpName,tmp);
				}else{
					Debug.LogError(string.Format("the same name = [{0}] in gameObject.name = [{1}]",_tmpName,tmp.name));
				}
				
				_tmpName = GetRelativeName(tmp);
				if(!m_dicName2Gobj.ContainsKey(_tmpName)){
					m_dicName2Gobj.Add(_tmpName,tmp);
				}else{
					Debug.LogError(string.Format("the same name = [{0}] in gameObject.name = [{1}]",_tmpName,tmp.name));
				}
			}
		}
	}
	
	/// <summary>
	/// 取得自身对象下面的对象的相对路径name
	/// </summary>
	string GetRelativeName(Transform trsf,ref string refName)
	{
		if(!trsf){
			return refName;
		}
		
		if(trsf == m_trsf){
			if(string.IsNullOrEmpty(refName))
			{
				refName = "/";
			}
			return refName;
		}
		
		if(string.IsNullOrEmpty(refName))
		{
			refName = trsf.name;
		} else {
			refName = trsf.name + "/" + refName;
		}
		
		return GetRelativeName(trsf.parent,ref refName);
	}
	
	/// <summary>
	/// 取得自身对象下面的对象的相对路径name
	/// </summary>
	string GetRelativeName(GameObject gobj)
	{
		string ret = "";
		GetRelativeName(gobj.transform,ref ret);
		return ret;
	}
	
	/// <summary>
	/// 取得自身对象下面的对象的相对路径name
	/// </summary>
	string GetRelativeName(Transform trsf)
	{
		string ret = "";
		GetRelativeName(trsf,ref ret);
		return ret;
	}
	
	/// <summary>
	/// 取得可操作的对象
	/// </summary>
	public GameObject GetGobjElement(string elName){
		if(string.IsNullOrEmpty(elName))
			return null;
		
		if("/".Equals(elName))
		{
			return m_gobj;
		}
		
		Init();

		if(m_dicName2Gobj.ContainsKey(elName)){
			return m_dicName2Gobj[elName];
		}
		return null;
	}
	
	/// <summary>
	/// 取得子对象的组件
	/// </summary>
	[LuaInterface.NoToLua]
	public T GetComponent4Element<T>(string elName) where T : Component
	{
		GameObject gobj = GetGobjElement(elName);
		
		if(gobj == null){
			return null;
		}
		return gobj.GetComponent<T>();
	}
	
	/// <summary>
	/// 取得子对象的组件
	/// </summary>
	public Component GetComponent4Element(string elName,string comType)
	{
		GameObject gobj = GetGobjElement(elName);
		
		if(gobj == null){
			return null;
		}
		return gobj.GetComponent(comType);
	}
	
	/// <summary>
	/// 取得子对象的组件
	/// </summary>
	public Component GetComponent4Element(string elName,Type comType)
	{
		GameObject gobj = GetGobjElement(elName);
		
		if(gobj == null){
			return null;
		}
		return gobj.GetComponent(comType);
	}
	
	/// <summary>
	/// 设置元素显隐
	/// </summary>
	public void SetActive(string elName,bool isActive)
	{
		GameObject gobj = GetGobjElement(elName);
		if(gobj){
			gobj.SetActive(isActive);
		}
	}
	
	/// <summary>
	/// 是否包含元素
	/// </summary>
	public bool IsHasGobj(string elName)
	{
		GameObject gobj = GetGobjElement(elName);
		return !!gobj;
	}
	
	[ContextMenu("Re-Pars")]
	void ReSizeList(){
		List<GameObject> list = new List<GameObject> ();
		GameObject gobj = null;
		for (int i=0;i<m_gobjs.Length;++i)
		{
			gobj = m_gobjs [i];
			if (gobj != null)
			{
				if (!list.Contains (gobj)) {
					list.Add (gobj);
				}
			}
		}
		m_gobjs = list.ToArray ();
		list.Clear ();
	}

	[ContextMenu("Re-Pars this and childs")]
	void ReSizeListAll(){
		PrefabElement[] arrs = this.gameObject.GetComponentsInChildren<PrefabElement> (true);
		foreach (var item in arrs) {
			item.ReSizeList ();
		}
	}
	
	[ContextMenu("Re-Bind Transform's First All Childs")]
	void ReBindAllFirstChilds(){
		int lens = this.m_trsf.childCount;
		m_gobjs = new GameObject[lens];
		for(int i = 0;i < lens;i++) {
			m_gobjs[i] = this.m_trsf.GetChild(i).gameObject;
		}
	}
	
	[ContextMenu("PrintDicKeys")]
	void PrintDicKeys(){
		if(!isInit)
			return;
		
		foreach (var key in m_dicName2Gobj.Keys) {
			Debug.Log(key);
		}
	}
}