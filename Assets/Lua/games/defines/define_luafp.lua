-- lua脚本的父节点
_LuaPacakge = {
	[1] = "games/defines/",
	[2] = "games/net/",
	[3] = "ugui/",
}

-- 不需要全局变量的
_LuaFpNoKey = {
	"luaex/toolex",
	"class",
	"ugui/ugui_base",
}

-- 需要全局变量的
_LuaFpKv = {
	{"Event","events"}, -- 引入通用的消息对象
	{"","define_csharp",1}, -- 常量 CSharp 相关
	{"","define_events",1}, -- 常量 事件 相关
	{"","define_global",1}, -- 常量 全局变量 
	{"","protocal",2}, -- 常量 网络层协议
	{"Network","network",2}, -- 网络层
}