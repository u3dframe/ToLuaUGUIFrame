using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TNet;

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
	
	static List<IUpdate> mListUps = new List<IUpdate>();
	int lens = 0;
	IUpdate _item = null;
	float _dt = 0;
	
	/// <summary>
	/// 初始化
	/// </summary>
	public void Init()
	{
	}

	/// <summary>
	///  更新 - 接受到数据
	/// </summary>
	void Update() {
		lock(mListUps){
			_dt = Time.deltaTime;
			lens = mListUps.Count;
			for (int i = 0; i < lens; i++)
			{
				_item = mListUps[i];
				if(_item != null){
					_item.OnUpdate(_dt);
				}
			}
		}
	}

	/// <summary>
	/// 销毁
	/// </summary>
	void OnDestroy() {
		mListUps.Clear();
	}
	
	public static void RegisterUpdate(IUpdate up) {
		lock (mListUps) {
			if(mListUps.Contains(up))
				return;
			mListUps.Add(up);
		}
	}

	public static void DiscardUpdate(IUpdate up) {
		lock (mListUps) {
			mListUps.Remove(up);
		}
	}
}