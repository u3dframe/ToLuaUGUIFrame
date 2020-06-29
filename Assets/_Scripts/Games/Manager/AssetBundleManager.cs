using UnityEngine;
using System;
using System.Collections.Generic;
namespace Core
{	
	using UObject = UnityEngine.Object;
	using UResources = UnityEngine.Resources;
	
	public delegate void DF_LoadedAsset(AssetBase asset);
	
	/// <summary>
	/// 类名 : ab 基础类
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-03-26 10:29
	/// 功能 : 用于继承
	/// </summary>
	[Serializable]
	public class AssetBase : IUpdate
	{
		public bool m_isOnUpdate = false;
		public bool IsOnUpdate(){ return this.m_isOnUpdate;} 

		public virtual void OnUpdate(float dt) {}
	}
	
	/// <summary>
	/// 类名 : ab的Asset资源
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-03-26 10:29
	/// 功能 : 暂时不考虑同名，不同类型的资源
	/// </summary>
	[Serializable]
	public class AssetInfo : AssetBase
	{
		public string m_abName;
		public string m_assetName;
		public Type m_assetType;

		private AssetBundleRequest m_abr; // 异步加载 Asset 资源
		public UObject m_obj;
		public ET_Asset m_upState = ET_Asset.None;
		public bool m_isDoned = false;
		
		public DF_LoadedAsset m_onLoadedAsset = null;
		
		private AssetInfo(string abName,string assetName,Type assetType)
		{
			this.m_abName = abName;
			this.m_assetName = assetName;
			this.m_assetType = assetType;
		}
		
		/// <summary>
		///  更新
		/// </summary>
		public override void OnUpdate(float dt) {
			UpLoadAsset();
		}
		
		void UpLoadAsset() {
			if(m_isDoned){
				_ExcuteLoadedCall();
				return;
			}
			
			switch(m_upState)
			{
				case ET_Asset.PreLoad:
				{
					var ab = AssetBundleManager.instance.GetABInfo(this.m_abName);
					if(ab == null)
					{
						this.m_upState = ET_Asset.Err_Null_AbInfo; // abinfo对象为空了
						this.m_isDoned = true;
						return;
					}
					
					if(!ab.m_isDoned){
						return;
					}
					
					if(ab.m_ab == null){
						this.m_upState = ET_Asset.Err_Null_AssetBundle; // abinfo的资源AssetBundel为空了
						this.m_isDoned = true;
						return;
					}
					
					if(m_assetType != null){
						this.m_abr = ab.m_ab.LoadAssetAsync(this.m_assetName,this.m_assetType);
					}else{
						this.m_abr = ab.m_ab.LoadAssetAsync(this.m_assetName);
					}
					this.m_upState = ET_Asset.Loading; 
				}
				break;
				case ET_Asset.Loading:
				{
					if(this.m_abr == null)
					{
						this.m_upState = ET_Asset.Err_Null_Abr; // asset的加载对象为空了
						this.m_isDoned = true;
						return;
					}
					
					if(!this.m_abr.isDone){
						return;
					}
					this.m_obj = this.m_abr.asset;
					this.m_upState = ET_Asset.CompleteLoad; 
					this.m_isDoned = true;
					this.m_abr = null;
				}
				break;
			}
		}
		
		void _ExcuteLoadedCall()
		{
			RegUpdate(false);

			var _func = this.m_onLoadedAsset;
			this.m_onLoadedAsset = null;
			if(_func != null)
			{
				_func(this);
			}
		}

		public void RegUpdate(bool isUp)
		{
			this.m_isOnUpdate = isUp;
			GameMgr.DiscardUpdate(this);
			if(isUp){
				GameMgr.RegisterUpdate(this);
			}
		}

		public void StartUpdate()
		{
			if(!this.m_isOnUpdate)
				RegUpdate(true);
		}

		public void OnUnloadAsset()
		{
			this.m_upState = ET_Asset.None;
			RegUpdate(false);

			this.m_onLoadedAsset = null;
			this.m_abr = null;
			var _obj = this.m_obj;
			this.m_obj = null;
			this.m_assetType = null;

			if(_obj != null){
				if(_obj.GetType() == GameFile.tpGobj){
					GameObject.DestroyImmediate(_obj,true);
				}else{
					UResources.UnloadAsset(_obj);
				}
			}
		}

		static public AssetInfo Builder(string abName,string assetName,Type assetType){
			return new AssetInfo(abName,assetName,assetType);
		}
	}
	
	/// <summary>
	/// 类名 : ab 的资源
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-03-26 10:29
	/// 功能 : 
	/// </summary>
	[Serializable]
	public class ABInfo : AssetBase
	{
		static float _defOutSec = 60 * 3; // 3 分钟
		
		public string m_abName; // assetbundle名
		public ET_AssetBundle m_abState = ET_AssetBundle.None; // 状态机制
		public ET_AssetBundle m_preAbState = ET_AssetBundle.None; // 状态机制
		public bool m_isDoned = false; // 是否已经执行了 LoadAB
		private int m_depNeedCount = 0;// 需要的依赖关系的数量;
		public int m_depNeedLoaded = 0;// 依赖关系加载了的数量;

		private AssetBundleCreateRequest m_abcr; // 异步加载 AssetBundle
		public AssetBundle m_ab; // 资源包
		public DF_LoadedAsset m_onLoadedAB = null;
		private List<ABInfo> m_depNeeds = new List<ABInfo>(); // 需要的依赖关系对象
		private ListDict<AssetInfo> m_assets = new ListDict<AssetInfo>(true); // 内部资源对象

		private int m_useCount = 0;// 引用计数,用于自动卸载;
		public bool m_isImmUnload = false; // 当引用计数为0时候，是否立即释放
		private float m_timeout = 0; // 当引用计数为0时候，不立即释放时控制

		public bool m_isStayForever = false; // 是否常驻，无需释放(shader资源)

		public bool isNeedUnload{ get { return this.m_abState == ET_AssetBundle.PreDestroy; }	}
		public bool isUnloaded{ get { return this.m_abState == ET_AssetBundle.Destroyed && this.m_preAbState == ET_AssetBundle.None; }	}
		public bool isLoaded{ get { return this.m_abState == ET_AssetBundle.CompleteLoad; }	}
		
		public ABInfo(string abName){
			this.m_abName = abName;
			SetUpState(ET_AssetBundle.WaitLoadDeps);
		}
		
		/// <summary>
		///  更新
		/// </summary>
		public override void OnUpdate(float dt) {
			UpLoadAB();
		}
		
		void UpLoadAB()
		{
			if(m_isDoned){
				_ExcuteLoadedCall();
				return;
			}

			switch(m_abState)
			{
				case ET_AssetBundle.WaitLoadDeps:
				{
					if(this.m_depNeedLoaded >= this.m_depNeedCount)
						SetUpState(ET_AssetBundle.PreLoad);
				}
				break;
				case ET_AssetBundle.PreLoad:
				{
					string fp = GameFile.GetPath(this.m_abName);
					this.m_abcr = AssetBundle.LoadFromFileAsync(fp);
					SetUpState(ET_AssetBundle.Loading);
				}
				break;
				case ET_AssetBundle.Loading:
				{
					if(this.m_abcr == null){
						this.m_isDoned = true;
						SetUpState(ET_AssetBundle.Err_Null_Abcr); // m_abcr 为空了
						return;
					}
					
					if(!this.m_abcr.isDone){
						return;
					}
					
					if(this.m_abcr.assetBundle == null){
						this.m_isDoned = true;
						this.m_abcr = null;
						SetUpState(ET_AssetBundle.Err_Null_AssetBundle); // 资源 assetBundle 为空了
						return;
					}
					
					this.m_ab = this.m_abcr.assetBundle;
					this.m_isDoned = true;
					this.m_abcr = null;
					SetUpState(ET_AssetBundle.CompleteLoad);
				}
				break;
			}
		}

		public void SetUpState(ET_AssetBundle state){
			this.m_preAbState = this.m_abState;
			this.m_abState = state;
		}

		public void RePreState(){
			SetUpState(this.m_preAbState);
		}

		void _ExcuteLoadedCall()
		{
			var _func = this.m_onLoadedAB;
			this.m_onLoadedAB = null;
			if(_func != null)
			{
				_func(this);
			}
		}

		public AssetInfo GetAsset(string assetName,Type assetType){
			string _key = assetName;
			if(assetType != null)
				_key = string.Format("{0}_{1}",assetName,assetType);

			return this.m_assets.Get(_key);
		}

		private AssetInfo GetOrNewAsset(string assetName,Type assetType){
			string _key = assetName;
			if(assetType != null)
				_key = string.Format("{0}_{1}",assetName,assetType);

			AssetInfo info = this.m_assets.Get(_key);
			if(info == null){
				info = AssetInfo.Builder(this.m_abName,assetName,assetType);
				this.m_assets.Add(_key,info);
			}
			return info;
		}

		public AssetInfo GetAssetAndCount(string assetName,Type assetType){
			RefCount();
			return GetOrNewAsset(assetName,assetType);
		}

		public void AddNeedDeps(ABInfo info){
			if(this.m_depNeeds.Contains(info))
				return;
			this.m_depNeeds.Add(info);
			this.m_depNeedCount++;
		}

		public void RefCount()
		{
			this.m_useCount++;
			this.m_timeout = 0;
		}

		public void Unload()
		{
			this.m_useCount--;
			if(this.m_useCount <= 0){
				
				if(m_isStayForever){
					this.m_useCount = 0;
					return;
				}

				if(this.m_isImmUnload){
					OnUnload();
				}else{
					this.m_timeout = _defOutSec;
					SetUpState(ET_AssetBundle.PreDestroy);
				}
			}
		}

		public void UpDestroy(float dt)
		{
			switch(m_abState)
			{
				case ET_AssetBundle.PreDestroy:
				{
					if(this.m_timeout > 0){
						this.m_timeout -= dt;
						if(this.m_timeout <= 0){
							SetUpState(ET_AssetBundle.Destroying);
						}
					}
				}
				break;
				case ET_AssetBundle.Destroying:
				{
					OnUnload();
				}
				break;
			}
		}

		public void OnUnload()
		{
			this.m_abState = ET_AssetBundle.Destroyed;
			this.m_preAbState = ET_AssetBundle.None;

			this.m_onLoadedAB = null;
			this.m_abcr = null;
			for(int i = 0; i < this.m_depNeedCount;i++){
				this.m_depNeeds[i].Unload();
			}
			this.m_depNeeds.Clear();
			this.m_depNeedCount = 0;

			int lens  = this.m_assets.m_list.Count;
			for(int i = 0; i < lens;i++){
				this.m_assets.m_list[i].OnUnloadAsset();
			}
			this.m_assets.Clear();

			var _ab = this.m_ab;
			this.m_ab = null;
			if(_ab != null){
				_ab.Unload(true);
			}
		}

		public override string ToString(){
			string fmt = "abName = [{0}] , isDone = [{1}] , state = [{2}] , pre = [{3}] , useCount = [{4}] , assetCount = [{5}] , ndDepCount = [{6}],depLoadCount = [{7}] , immUnlod = [{8}] , timeout = [{9}]";
			return string.Format(fmt,this.m_abName,this.m_isDoned,this.m_abState,this.m_preAbState,this.m_useCount,this.m_assets.m_list.Count,this.m_depNeedCount,this.m_depNeedLoaded,this.m_isImmUnload,this.m_timeout);
		}
	}
	
	/// <summary>
	/// 类名 : ab 的资源 管理器
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-03-26 10:29
	/// 功能 : 
	/// </summary>
	public class AssetBundleManager : MonoBehaviour,IUpdate
	{
		static AssetBundleManager _instance;
		static public AssetBundleManager instance{
			get{
				if (_instance == null) {
					GameObject _gobj = GameMgr.mgrGobj;
					_instance = _gobj.GetComponent<AssetBundleManager>();
					if (_instance == null)
					{
						_instance = _gobj.AddComponent<AssetBundleManager> ();
					}
				}
				return _instance;
			}
		}
		
		public int m_nMaxLoad = 5; // 限定加载
		private Dictionary<string, string[]> _dependsList = new Dictionary<string, string[]>(); // 依赖关系
		private ListDict<ABInfo> _ndLoad = new ListDict<ABInfo>(true); // 需要加载的 AB
		private ListDict<ABInfo> _loading = new ListDict<ABInfo>(true); // 正在加载的 AB
		private ListDict<ABInfo> _loaded = new ListDict<ABInfo>(true); // 已经加载了的 AB
		private ListDict<ABInfo> _unLoad = new ListDict<ABInfo>(true); // 需要销毁的 AB

		private int nUpLens = 0;
		private ABInfo upInfo = null;
		private List<ABInfo> upTemp = new List<ABInfo>();
		private List<ABInfo> upList = new List<ABInfo>();
		
		/// <summary>
		///  初始化
		/// </summary>
		void Awake(){
			LoadMainfest();
			m_isOnUpdate = true;
			GameMgr.RegisterUpdate(this);
		}

		/// <summary>
		/// 销毁
		/// </summary>
		void OnDestroy() {
			m_isOnUpdate = false;
			GameMgr.DiscardUpdate(this);
		}

#if UNITY_EDITOR
		public bool isDebug = true; //是否打印
#else
		public bool isDebug = false; //是否打印
#endif
		protected void LogErr(object msg){
			if(!isDebug || msg == null)
				return;
			Debug.LogErrorFormat("==== ABMgr = [{0}]",msg);
			// Debug.LogErrorFormat("== [{0}] == [{1}] == [{2}]",this.GetType(),this.GetInstanceID(),msg);
		}

		public bool m_isOnUpdate = true;
		public bool IsOnUpdate(){ return this.m_isOnUpdate;} 
		
		public void LoadMainfest()
		{
			string path = GameFile.m_fpABManifest;
			_dependsList.Clear();
			AssetBundle ab = AssetBundle.LoadFromFile(path); // LoadFromMemory

			if(ab == null)
			{
				string errormsg = string.Format("=== LoadMainfest ab is NULL , fp = [{0}]!",path);
				return;
			}

			AssetBundleManifest mainfest =  ab.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");;
			if (mainfest == null)
			{
				string errormsg = string.Format("=== LoadMainfest ab.mainfest is NULL , fp = [{0}]!",path);
				return;
			}

			foreach(string assetName in mainfest.GetAllAssetBundles())
			{
				// GetAllDependencies = 所有依赖的AssetBundle名字
				// GetDirectDependencies = 直接依赖的AssetBundle名字
				string[] dps = mainfest.GetDirectDependencies(assetName); 
				_dependsList.Add(assetName, dps);
			}
			ab.Unload(true);
			ab = null;
		}
		
		/// <summary>
		///  更新
		/// </summary>
		public void OnUpdate(float dt) {
			UpdateLoad(dt);
			UpdateReady(dt);
			UpdateUnLoad(dt);
		}

		void UpdateLoad(float dt){
			upTemp.AddRange(this._loading.m_list);
			nUpLens = upTemp.Count;
			for(int i = 0; i < nUpLens;i++){
				upInfo = upTemp[i];
				upInfo.OnUpdate(dt);
				if(upInfo.m_isDoned){
					upList.Add(upInfo);
				}
			}
			upTemp.Clear();

			nUpLens = upList.Count;
			for(int i = 0; i < nUpLens;i++){
				upInfo = upList[i];
				upInfo.OnUpdate(dt);
				this._loading.Remove(upInfo.m_abName);

				switch(upInfo.m_abState)
				{
					case ET_AssetBundle.CompleteLoad:
						this._loaded.Add(upInfo.m_abName,upInfo);
					break;
					case ET_AssetBundle.Err_Null_AssetBundle:
						LogErr("ab is null");
					break;
					case ET_AssetBundle.Err_Null_Abcr:
						LogErr("ab CreateRequest is null");
					break;
				}
			}
			upList.Clear();
		}

		void UpdateReady(float dt){
			upTemp.AddRange(this._ndLoad.m_list);
			int _l1 = upTemp.Count;
			if(_l1 <= 0){
				return;
			}
			
			int _l2 = this._loading.m_list.Count;
			nUpLens = this.m_nMaxLoad - _l2;
			nUpLens = (nUpLens > _l1) ? _l1 : nUpLens;
			for(int i = 0; i < nUpLens;i++){
				upInfo = upTemp[i];
				upList.Add(upInfo);
				this._loading.Add(upInfo.m_abName,upInfo);
			}
			upTemp.Clear();

			nUpLens = upList.Count;
			for(int i = 0; i < nUpLens;i++){
				upInfo = upList[i];
				this._ndLoad.Remove(upInfo.m_abName);
			}
			upList.Clear();
		}

		void UpdateUnLoad(float dt){
			upTemp.AddRange(this._unLoad.m_list);
			nUpLens = upTemp.Count;
			if(nUpLens <= 0)
				return;
			
			for(int i = 0; i < nUpLens;i++){
				upInfo = upTemp[i];
				upInfo.UpDestroy(dt);

				if(upInfo.isUnloaded){
					upList.Add(upInfo);
				}
			}
			upTemp.Clear();

			nUpLens = upList.Count;
			for(int i = 0; i < nUpLens;i++){
				upInfo = upList[i];
				this._unLoad.Remove(upInfo.m_abName);
			}
			upList.Clear();
		}

		
		public ABInfo GetABInfo(string abName)
		{
			ABInfo _abInfo = _loaded.Get(abName);
			if(_abInfo == null) {
				_abInfo = _ndLoad.Get(abName);
			}
			if(_abInfo == null) {
				_abInfo = _loading.Get(abName);
			}
			return _abInfo;
		}

		public ABInfo LoadAB(string abName,DF_LoadedAsset cfunc)
		{
			ABInfo _abInfo = _unLoad.Remove4Get(abName);
			if(_abInfo != null)
			{
				_abInfo.RePreState();
				if(_abInfo.m_isDoned){
					if(_abInfo.isLoaded){
						_loaded.Add(abName,_abInfo);
					}else{
						LogErr(_abInfo);
						_abInfo = null; // 出错的
					}
				}else{
					if(_abInfo.isUnloaded){
						LogErr(_abInfo);
						_abInfo = null; // 丢弃了
					}else{
						_ndLoad.Add(abName,_abInfo);
					}
				}
			} else {
				_abInfo = GetABInfo(abName);
			}

			if(_abInfo == null){
				_abInfo = new ABInfo(abName);
				if(cfunc != null){
					_abInfo.m_onLoadedAB += cfunc;
				}
				_ndLoad.Add(abName,_abInfo);

				// 依赖关系
				string[] _des = null;
				int lens = 0;
				if (_dependsList.ContainsKey(abName))
				{
					_des = _dependsList[abName];
					lens = _des.Length;
				}
				for(int i = 0; i < lens;i++){
					ABInfo abDep = LoadAB(_des[i],(obj) => {
						_abInfo.m_depNeedLoaded++;
					});
					abDep.RefCount();
					_abInfo.AddNeedDeps(abDep);
				}
			}
			return _abInfo;
		}

		public ABInfo LoadABRefUseCount(string abName,DF_LoadedAsset cfunc)
		{
			ABInfo _abInfo = LoadAB(abName,cfunc);
			_abInfo.RefCount();
			return _abInfo;
		}
		
		public AssetInfo LoadAsset(string abName,string assetName,Type assetType,DF_LoadedAsset cfunc)
		{
			if(string.IsNullOrEmpty(assetName)){
				if(cfunc != null){
					cfunc(null);
				}
				return null;
			}

			ABInfo _abInfo = LoadAB(abName,null);
			AssetInfo _info = _abInfo.GetAssetAndCount(assetName,assetType);
			if(_info != null){
				if(cfunc != null){
					_info.m_onLoadedAsset += cfunc;
				}
				_info.StartUpdate();
			}
			return _info;
		}

		public void UnLoadAB(string abName){
			var _ab = GetABInfo(abName);

			this._ndLoad.Remove(abName);
			this._loading.Remove(abName);
			this._loaded.Remove(abName);

			if(_ab != null){
				this._unLoad.Add(abName,_ab);
			}
		}

		public void UnLoadAB(ABInfo abInfo){
			if(abInfo != null){
				abInfo.Unload();
				if(abInfo.isNeedUnload || abInfo.isUnloaded){
					UnLoadAB(abInfo.m_abName);
				}
			}
		}

		public void UnLoadAsset(string abName)
		{
			UnLoadAB(GetABInfo(abName));
		}

		public void UnLoadAsset(AssetInfo info)
		{
			UnLoadAB(GetABInfo(info.m_abName));
		}		
	}
}
