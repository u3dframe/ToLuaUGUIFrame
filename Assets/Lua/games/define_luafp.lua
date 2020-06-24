-- lua脚本的父节点
_LuaPacakge = {
	[1] = "games/net/",
}

-- 不需要全局变量的
_LuaFpNoKey = {
	"luaex/toolex",
	"class",
	"games/define_csharp",
	"games/define_global",
	"games/define_events",
}

-- 需要全局变量的
_LuaFpKv = {
	{"Event","events"}, -- 引入通用的消息对象
	{"","protocal",1}, -- 网络层常量
	{"Network","network",1}, -- 网络层
}