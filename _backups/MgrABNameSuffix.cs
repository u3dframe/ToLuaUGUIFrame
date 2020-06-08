using System;
using System.Collections.Generic;
namespace Core
{
	/// <summary>
	/// 类名 : ab设定相关信息
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2017-03-07 09:29
	/// 功能 : 
	/// </summary>
	[System.Serializable]
	public class ABNameSuffix
	{
		public string strContains = null; // 资源路径 包含该字段
		public string abNameEnd = null; // AssetBundle 名字的结尾
		public string strStart = null; // 资源路径 以什么开头
		public string strEnd = null; // 资源路径 以什么结尾
		public string abSuffix = null; // AssetBundle 的后缀名
		
		public ABNameSuffix(){}
		public ABNameSuffix(string strContains,string abNameEnd){
			this.strContains = strContains;
			this.abNameEnd = abNameEnd;
		}
		
		public bool IsHas(string fp){
			if(string.IsNullOrEmpty(fp)){
				return false;
			}
			bool _ret = false;
			bool _isC = !string.IsNullOrEmpty(strContains);
			bool _isS = !string.IsNullOrEmpty(strStart);
			bool _isE = !string.IsNullOrEmpty(strEnd);
			
			if(_isC){
				_ret = fp.Contains(strContains);
			}
			
			if(_isS){
				bool _bl = fp.StartsWith(strStart);
				_ret = _isC ? (_ret && _bl) : _bl;
			}
			
			if(_isE){
				bool _bl = fp.EndsWith(strEnd);
				_ret = (_isC || _isS ) ? (_ret && _bl) : _bl;
			}
			return _ret;
		}
		
		static public ABNameSuffix ParseByRow(string row){
			if(string.IsNullOrEmpty(row)){
				return null;
			}
			string[] _arrs = GameFile.SplitComma(row);
			int lens = _arrs.Length;
			if(lens < 2)
				return null;
			
			ABNameSuffix ret = new ABNameSuffix(_arrs[0],_arrs[1]);
			if(lens > 2){
				ret.strStart = _arrs[2];
			}
			if(lens > 3){
				ret.strEnd = _arrs[3];
			}
			if(lens > 4){
				ret.abSuffix = _arrs[4];
			}
			return ret;
		}
	}
	
	/// <summary>
	/// 类名 : 管理 资源结束名和ab资源后缀名
	/// 作者 : Canyon / 龚阳辉
	/// 日期 : 2017-03-07 09:29
	/// 功能 : 
	/// </summary>
	public class MgrABNameSuffix{
		static public List<ABNameSuffix> list = new List<ABNameSuffix>();
		static bool _isInited = false;
		static public void Init(string content){
			if(_isInited || string.IsNullOrEmpty(content))
				return;
			
			string[] _rows = GameFile.SplitRow(content);
			if(GameFile.IsNullOrEmpty(_rows)){
				return;
			}
			
			int lens = _rows.Length;
			ABNameSuffix _it = null;
			for(int i = 0;i < lens;i++){
				_it = ABNameSuffix.ParseByRow(_rows[i]);
				if(_it == null)
					continue;
				list.Add(_it);
			}
			_isInited = true;
		}
		
		static public void LoadFn(string fn){
			Init(GameFile.GetText(fn));
		}
		
		static public ABNameSuffix GetObj(string fp){
			if(!_isInited)
				return null;
			
			int lens = list.Count;
			ABNameSuffix _it = null,_cur = null;
			for(int i = 0;i < lens;i++){
				_it = list[i];
				if(_it.IsHas(fp)){
					_cur = _it;
					break;
				}
			}
			
			return _cur;
		}
		
		static public string GetNameEndInfo(string fp,ref string abSuffix){
			abSuffix = null;
			if(!_isInited)
				return null;
			
			ABNameSuffix _cur = GetObj(fp);
			if(_cur == null)
				return null;
			
			abSuffix = _cur.abSuffix;
			return _cur.abNameEnd;
		}
	}
}
