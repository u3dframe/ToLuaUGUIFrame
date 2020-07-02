--[[
	-- 资源加载逻辑吧
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local super = LuCFabElement
local M = class( "lua_asset",super )

function M:ctor()
end

function M:SetFabElement( obj )
	super.ctor(self,obj)
end


return M