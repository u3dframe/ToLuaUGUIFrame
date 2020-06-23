fmt_s1_s2 = "%s%s"
local str_format = string.format

-- lua脚本的父节点
local _lbLuaPacakge = {
	[1] = "games/",
	[2] = "games/logic/",
}

-- 不需要全局变量的
local _lbNoKey = {
	"luaex/toolex",
	"class",
	"games/define_csharp",
	"games/define_global",
}

-- 需要全局变量的
local _lbInitKV = {
	{"Event","events"}, -- 引入通用的消息对象
}

local M = {}

--游戏初始化主要接口
function M.Init()
	local _MG,_entity,_fp = _G;
	for _,v in ipairs(_lbNoKey) do
		_entity = require(v)
	end

	for _,v in ipairs(_lbInitKV) do
		_fp = "";
		if v[3] then
			_fp = _lbLuaPacakge[v[3]] or ""
		end
		_fp = str_format(fmt_s1_s2,_fp,v[2])
		_entity = require(_fp)
		if type(_entity) == "table" and _entity.Init then
			_entity:Init();
		end

		if not (v[1] == "" or v[1] == "nil") then
			_MG[v[1]] = _entity;
		end
	end
	
	-- 按键控制 发布真机包时 会移除
	-- if GM_IsEditor then
	--	require("manager.keycodecallback").Init()
	-- end
end

return M;
