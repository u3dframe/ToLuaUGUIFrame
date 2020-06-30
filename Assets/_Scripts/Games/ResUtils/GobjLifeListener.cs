using UnityEngine;
using System.Collections;

public delegate void OnNotifyDestry(GobjLifeListener obj);

/// <summary>
/// 类名 : GameObject对象 生命周期 监听
/// 作者 : Canyon
/// 日期 : 2017-03-21 10:37
/// 功能 : 只针对 OnDestroy的回调
/// </summary>
public class GobjLifeListener : MonoBehaviour,IUpdate {
	static public GobjLifeListener Get(GameObject gobj){
		GobjLifeListener _r = gobj.GetComponent<GobjLifeListener> ();
		if (!_r) {
			_r = gobj.AddComponent<GobjLifeListener> ();
		}
		return _r;
	}

	// 接口函数
	[System.NonSerialized]
	public bool m_isOnUpdate = true;
	public bool IsOnUpdate(){ return this.m_isOnUpdate;} 
	public virtual void OnUpdate(float dt) {}
	
	[System.NonSerialized]
	public string poolName = "";
	
	bool m_isCallDestroy = true;
	/// <summary>
	/// 继承对象实现的销毁回调 (比代理事件快)
	/// </summary>
	protected virtual void OnCall4Destroy(){}
	/// <summary>
	/// 销毁回调
	/// </summary>
	public OnNotifyDestry m_onDestroy;
	
	void _ExcDestoryCall(){
		var _call = this.m_onDestroy;
		this.m_onDestroy = null;
		if (_call != null)
			_call (this);
	}

	void OnDestroy(){
		// Debug.Log ("Destroy,poolName = " + poolName+",gobjname = " + gameObject.name);
		this.m_isOnUpdate = false;
		if(!this.m_isCallDestroy) return;
		OnCall4Destroy();
		_ExcDestoryCall();
	}

	public void DetroyComp(bool isCallDestroy){
		this.m_isCallDestroy = isCallDestroy;
		GameObject.Destroy(this);
	}

	public void DetroyComp(){
		DetroyComp(true);
	}
}
