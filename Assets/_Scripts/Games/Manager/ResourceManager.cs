using UnityEngine;
using System.Collections.Generic;
namespace Core
{
	/// <summary>
	/// 类名 : 资源管理器
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-06-26 10:29
	/// 功能 : 
	/// </summary>
	public static class ResourceManager
	{
		static string fmtPoolName = "{0}@@{1}";
		static Dictionary<string,GameObjectPool> m_pools = new Dictionary<string, GameObjectPool>();
		static Transform _trsfRoot;
		static public Transform trsfRoot{
			get{
				if (UtilityHelper.IsNull(_trsfRoot)) {
					string NM_Gobj = "ObjectPools";
					GameObject gobj = GameObject.Find(NM_Gobj);
					if (UtilityHelper.IsNull(gobj))
					{
						gobj = new GameObject(NM_Gobj);
					}
					GameObject.DontDestroyOnLoad (gobj);
					_trsfRoot = gobj.transform;
				}

				return _trsfRoot;
			}
		}

		static public void LoadGameObject(string abName,string assetName,DF_LoadedFab callLoaded){
			string poolName = string.Format(fmtPoolName,abName,assetName);
			GameObjectPool gpool = null;
			if(!m_pools.TryGetValue(poolName,out gpool))
			{
				gpool = GameObjectPool.builder(poolName,1,trsfRoot);
				m_pools.Add(poolName,gpool);
			}

			if(gpool.isHasPrefab){
				if(callLoaded != null)
				{
					callLoaded(gpool.BorrowObject());
				}
			}else{
				gpool.m_cfLoadedFab += callLoaded;
			}
		}
	}
}
