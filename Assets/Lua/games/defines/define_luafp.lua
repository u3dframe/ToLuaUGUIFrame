-- lua脚本的父节点
_LuaPacakge = {
	[1] = "games/basics/",
	[2] = "games/ugui/",
	[3] = "games/defines/",
	[4] = "games/net/",
	[5] = "games/logics/",
	[6] = "games/logics/login/",
}

-- 不需要全局变量的lua
_LuaFpNoKey = {
	"luaex/toolex",
	"class",
}

-- 基础
_LuaFpBasic = {
	{"Event","events"}, -- 引入通用的消息对象	
	{"LuaObject","lua_object",1}, -- 基础类
	{"LuUGobj","u_gobj",1}, -- gobj
	{"LuUTrsf","u_transform",1}, -- transform
	{"LuUComonet","u_component",1}, -- component
	{"LuCFabBasic","uc_fabbasic",1}, -- PrefabBasic
	{"LuCFabElement","uc_fabelement",1}, -- PrefabElement
	{"LuBase","ugui_base",2}, -- UGUI 组件 - 基础类
	{"LuText","ugui_text",2}, -- UGUI 组件 - 文本
	{"","define_csharp",3}, -- 常量 CSharp 相关
	{"","define_events",3}, -- 常量 事件 相关
	{"","define_global",3}, -- 常量 全局变量
}

-- 中间
_LuaFpMidle = {
	{"","protocal",4}, -- 常量 网络层协议
	{"Network","network",4}, -- 网络层
	{"","games/game_tools"}, -- 游戏需要的公共函数的封装
	{"MgrRes","mgr_res",5}, -- 控制 资源加载了
	{"LuaAsset","lua_asset",5}, -- 资源
	{"LuaFab","lua_fab",5}, -- 为场景对象和ui_base对象的父类
}

-- 最后
_LuaFpEnd = {
	{"MgrLogin","mgr_login",6}, -- 登录管理
}