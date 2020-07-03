--[[
	-- lua_fab 的类
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local tb = table
local super = LuaAsset
local M = class( "lua_fab",super )

function M:ctor()
	super.ctor( self )
end

function M:onAssetConfig( assetCfg )
	assetCfg = super.onAssetConfig( self, assetCfg )
	return assetCfg
end

return M