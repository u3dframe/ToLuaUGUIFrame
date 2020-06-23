using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TNet;

public class NetworkManager : Core.Kernel.BasicManager<NetworkManager> {
	static readonly object m_lockObject = new object();
	static Queue<KeyValuePair<int, ByteBuffer>> mEvents = new Queue<KeyValuePair<int, ByteBuffer>>();
	
	private SocketClient socket = null;
	string lua_func = "Network.OnSocket";
	String m_host = null;
	int m_port = 0;
	
	/// <summary>
	///  更新 - 接受到数据
	/// </summary>
	void Update() {
		if (mEvents.Count > 0) {
			while (mEvents.Count > 0) {
				KeyValuePair<int, ByteBuffer> _event = mEvents.Dequeue();
				// 通知到lua那边
				OnCF2Lua(_event.Key,_event.Value);
				// 放入对象池				
				ByteBuffer.ReBack(_event.Value);
			}
		}
	}

	/// <summary>
	/// 销毁
	/// </summary>
	void OnDestroy() {
		socket.OnRemove();
	}

	/// <summary>
	/// 通知到lua那边
	/// </summary>
	void OnCF2Lua(int cmd,ByteBuffer data) {
		if(data == null)
			return;

		bool isState = LuaHelper.CFuncLua(lua_func,cmd,data);
		if(!isState)
			Debug.LogErrorFormat("=== OnCF2Lua Fails,lua func = [{0}], code = [{1}]",lua_func,cmd);
	}

	protected override void OnInitInstance(){
		socket = new SocketClient();
		socket.OnRegister();
	}

	public NetworkManager InitNet(string host,int port,string luaFunc){
		this.m_host = host;
		this.m_port = port;
		if(!string.IsNullOrEmpty(luaFunc))
			this.lua_func = luaFunc;
		return this;
	}
	
	///------------------------------------------------------------------------------------
	public static void AddEvent(int code, ByteBuffer data) {
		lock (m_lockObject) {
			mEvents.Enqueue(new KeyValuePair<int, ByteBuffer>(code, data));
		}
	}

	public void ReConnect(string host,int port) {
		if(this.socket == null)
			return;

		if(!string.IsNullOrEmpty(host)){
			this.m_host = host;
		}
		if(port > 0){
			this.m_port = port;
		}
		this.socket.Close();
		
		SendConnect();
	}

	/// <summary>
	/// 发送链接请求
	/// </summary>
	public void SendConnect() {
		if(this.m_host == null || this.m_port <= 0){
			return;
		}
		socket.SendConnect(this.m_host,this.m_port);
	}

	/// <summary>
	/// 发送SOCKET消息
	/// </summary>
	public void SendMessage(ByteBuffer buffer) {
		socket.SendMessage(buffer);
	}
}