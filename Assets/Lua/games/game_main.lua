-- 游戏入口

local M = {}

function M.Init()
	-- 初始manager
	require("games/game_manager").Init()
end

return M