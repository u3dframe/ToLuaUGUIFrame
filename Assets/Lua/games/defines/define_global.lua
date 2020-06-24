-- atlas_uimain = "texture/ui/atlas/uimain.atlas"; -- 主要的
-- atlas_com = "texture/ui/atlas/uicommon.atlas"; -- 常用的
-- game_version = "12"; -- 游戏版本号

TB_EMPTY = {}; -- 全局空的对象(用于返回)
TB_NEW = {__call=function() return {}; end}; -- 用法: TB_NEW();