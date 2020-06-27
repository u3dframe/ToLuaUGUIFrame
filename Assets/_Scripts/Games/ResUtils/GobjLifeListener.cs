using UnityEngine;
using System.Collections;

/// <summary>
/// 类名 : GameObject对象 生命周期 监听
/// 作者 : Canyon
/// 日期 : 2017-03-21 10:37
/// 功能 : 只针对 OnDestroy的回调
/// </summary>
public class GobjLifeListener : MonoBehaviour {
	static public GobjLifeListener Get(GameObject gobj){
		GobjLifeListener _r = gobj.GetComponent<GobjLifeListener> ();
		if (!_r) {
			_r = gobj.AddComponent<GobjLifeListener> ();
		}
		return _r;
	}
	
	[System.NonSerialized]
	public string poolName = "";
	
	/// <summary>
	/// 销毁回调
	/// </summary>
	public System.Action<GobjLifeListener> m_callDestroy;
	
	void OnDestroy(){
		// Debug.Log ("Destroy,poolName = " + poolName+",gobjname = " + gameObject.name);
		if (m_callDestroy != null)
			m_callDestroy (this);
	}
}
