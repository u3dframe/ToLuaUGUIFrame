-- lua脚本的父节点
_LuaPacakge = {
	[1] = "games/defines/",
	[2] = "games/net/",
	[3] = "ugui/",
	[4] = "games/logics/login/",
}

-- 不需要全局变量的lua
_LuaFpNoKey = {
	"luaex/toolex",
	"class",
	"ugui/ugui_base",
}

-- 需要全局变量的lua
_LuaFpKv = {
	{"Event","events"}, -- 引入通用的消息对象
	{"","define_csharp",1}, -- 常量 CSharp 相关
	{"","define_events",1}, -- 常量 事件 相关
	{"","define_global",1}, -- 常量 全局变量
	{"","protocal",2}, -- 常量 网络层协议
	{"Network","network",2}, -- 网络层
	{"LuText","ugui_text",3}, -- UGUI 组件 - 文本
	{"","games/game_tools"}, -- 游戏需要的公共函数的封装
	{"MgrLogin","mgr_login",4}, -- 登录管理
}