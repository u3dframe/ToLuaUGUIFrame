--[[
	-- gameObject
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local super = LuaObject
local M = class( "lua_gobj",super )

function M:ctor( obj )
	assert( obj )
	super.ctor(self)
	self.obj = obj
	self.objType = type(obj)
	self.gobj = obj.gameObject or obj.gobj
end

function M:GetComponent( com )
	return self.gobj:getComponent( com )
end

function M:SetActive( isActive )
	isActive = isActive == true
	if self.isActive == nil or isActive ~= self.isActive then
		self.isActive = isActive
		self.gobj:SetActive( self.isActive )
	end
end

return M