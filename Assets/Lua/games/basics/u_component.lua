--[[
	-- 组件 component
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local super = LuUTrsf
local M = class( "lua_component",super )

function M:ctor( obj, component )
	super.ctor(self,obj)
	local com = self:GetComponent( component )
	if not com then
		printError( "can't find compnent by %s", component )
	end
	self.ucom = com
end

return M