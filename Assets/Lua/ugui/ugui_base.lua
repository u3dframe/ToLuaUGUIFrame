--[[
	-- ugui的基础类
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local M = class( "ugui_base" )

function M:ctor( gobj, component )
	assert( gobj )
	self.gobj = gobj.gameObject;

	local com = self.gobj:getComponent( component )
	if not com then
		printError( "can't find compnent by name %s", component )
	end
	self.ucom = com
end

function M:clean()
	self.gobj = nil
	self.ucom = nil
end

return M