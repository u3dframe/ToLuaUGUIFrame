--[[
	-- lua_asset 的基础类
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local tb = table
local super = LuCFabElement
local M = class( "lua_asset",super )

function M:ctor(assetCfg)
	self.cfgAsset = {
		abName = nil,
		assetName = nil,
		assetLType = LE_AsType.Fab,
	}	
	self:onAssetConfig(assetCfg or self.cfgAsset)
	self._lfLoadAsset = handler(self,self._OnCFLoadAsset);
end

function M:onAssetConfig( assetCfg )
	return tb.merge(self.cfgAsset, assetCfg)
end

function M:LoadAsset()
	local _cfg = self.cfgAsset;
	if _cfg.abName and _cfg.assetName then
		MgrRes.LoadAsset(_cfg.abName,_cfg.assetName,_cfg.assetLType,self._lfLoadAsset);
	end
end

function M:_OnCFLoadAsset( obj )
	local _tp = self.cfgAsset.assetLType
	if LE_AsType.Fab == _tp then
		self:OnCF_Fab(obj);
	elseif LE_AsType.Sprite == _tp then
		self:OnCF_Sprite(obj);
	elseif LE_AsType.Texture == _tp then
		self:OnCF_Texture(obj);
	end
end

function M:OnCF_Fab( obj )
	super.ctor(self,obj)
end

function M:OnCF_Sprite( obj )
end

function M:OnCF_Texture( obj )
end
return M