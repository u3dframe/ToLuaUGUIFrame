using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TNet;

public delegate void DF_OnUpdate(float dt);

/// <summary>
/// 类名 : Update 管理
/// 作者 : Canyon
/// 日期 : 2020-06-27 20:37
/// 功能 : 所有需要Update函数，统一从这里调用
/// </summary>
public class GameMgr : MonoBehaviour {

	static GameObject _mgrGobj;
	static public GameObject mgrGobj{
		get{
			if (!!_mgrGobj) {
				string NM_Gobj = "GameManager";
				_mgrGobj = GameObject.Find(NM_Gobj);
				if (!_mgrGobj)
				{
					_mgrGobj = new GameObject(NM_Gobj);
				}
				GameObject.DontDestroyOnLoad (_mgrGobj);
			}

			return _mgrGobj;
		}
	}

	static GameMgr _instance;
	static public GameMgr instance{
		get{
			if (_instance == null) {
				_instance = mgrGobj.GetComponent<GameMgr>();
				if (_instance == null)
				{
					_instance = mgrGobj.AddComponent<GameMgr> ();
				}
			}
			return _instance;
		}
	}
	
	static public DF_OnUpdate onUpdate = null;
	static List<IUpdate> mListUps = new List<IUpdate>(); // 无用质疑，直接调用函数，比代理事件快

	int lens = 0;
	IUpdate _item = null;
	float _dt = 0;
	
	/// <summary>
	/// 初始化
	/// </summary>
	public void Init()
	{
		Localization.language = PlayerPrefs.GetString("Language", "English");
	}

	/// <summary>
	///  更新 - 接受到数据
	/// </summary>
	void Update() {
		_dt = Time.deltaTime;
		lock(mListUps){
			lens = mListUps.Count;
			for (int i = 0; i < lens; i++)
			{
				_item = mListUps[i];
				if(_item != null){
					_item.OnUpdate(_dt);
				}
			}
		}

		if(onUpdate != null)
		{
			onUpdate(_dt);
		}
	}

	/// <summary>
	/// 销毁
	/// </summary>
	void OnDestroy() {
		mListUps.Clear();
	}
	
	static public void RegisterUpdate(IUpdate up) {
		lock (mListUps) {
			if(mListUps.Contains(up))
				return;
			mListUps.Add(up);
		}
	}

	static public void DiscardUpdate(IUpdate up) {
		lock (mListUps) {
			mListUps.Remove(up);
		}
	}
}