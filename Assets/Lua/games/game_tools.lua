--[[
	-- 游戏的公共函数 func
	-- Author : canyon / 龚阳辉
	-- Date : 2020-06-27 13:25
	-- Desc : 
]]
local str_upper = string.upper
local str_format = string.format
local tb_insert = table.insert
local tb_concat = table.concat
local d_traceback = debug.traceback

local DE_BUG = CDebug.useLog;

function logMust(fmt,...)
	local str = str_format(tostring(fmt), ...)
	CHelper.Log(str)
end

function printLog(tag, fmt, ...)
	local _isErr,str = tag == "ERR";
	if (not _isErr) and (not DE_BUG) then
		return
	end

    local t = {
        "[",
        str_upper(tostring(tag)),
        "] ",
        str_format(tostring(fmt), ...)
	}
    if _isErr then
        tb_insert(t, d_traceback("", 3))  -- 打印要少前3行数据
	end
	str = tb_concat(t)
	if _isErr then
		CHelper.LogError(str)
	else
		CDebug.Log(str)
	end
end

function printError(fmt, ...)
    printLog("ERR", fmt, ...)
end

function printInfo(fmt, ...)
	printLog("INFO", fmt, ...)
end
  
function printWarn(fmt, ...)
	printLog("WARN", fmt, ...)
end