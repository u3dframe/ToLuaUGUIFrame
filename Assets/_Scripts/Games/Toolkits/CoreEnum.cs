using System;
namespace Core
{
	/// <summary>
	/// 类名 : 枚举 - AB状态
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-06-26 08:29
	/// 功能 : 
	/// </summary>
	public enum ET_AssetBundle{
		None = 0,
		WaitLoadDeps = 1,
		PreLoad = 2,
		Loading = 3,
		CompleteLoad = 4,
		
		PreDestroy = 10,
		Destroying = 11,
		Destroyed = 12,
		
		Error = 100,
		Err_Null_Abcr = 101,
		Err_Null_AssetBundle = 102,
	}
	
	/// <summary>
	/// 类名 : 枚举 - Asset状态
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2020-06-26 09:09
	/// 功能 : 
	/// </summary>
	public enum ET_Asset{
		None = 0,
		PreLoad = 1,
		Loading = 2,
		CompleteLoad = 3,
		
		Error = 100,
		Err_Null_AbInfo = 101,
		Err_Null_AssetBundle = 102,
		Err_Null_Abr = 103,
	}
}
