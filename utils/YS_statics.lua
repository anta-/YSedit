--[[
	TODO:
		ObjAtt*のNULL判定が不十分なようなので、突き止める
		メッセージが出たらB連打してメッセージを飛ばす
]]

local ffi = require("ffi")
ffi.cdef[[
int SendMessageA(
  unsigned hWnd,
  unsigned Msg,
  unsigned wParam,
  unsigned lParam
);
unsigned FindWindowA(
  const char *lpClassName,
  const char *lpWindowName
);
unsigned GetLastError();

unsigned MoveFileA(
  const char *lpExistingFileName,
  const char *lpNewFileName
);
unsigned DeleteFileA(
  const char *lpFileName
);

]]
local C = ffi.C
local takescreenshot
do
local screen_shot_dir = [[C:\test\data1\source\LuaExtension\winproject\mupen64\bin\ScreenShots]]
local default_file_path = screen_shot_dir .. "\\" .. "YOSHI STORY TEST-0.png"
local w = C.FindWindowA(nil, "Mupen 64  0.5 - YOSHI STORY TEST    ")
takescreenshot = function(path)
	print("take SS " .. path)
	C.SendMessageA(w, 0x0100, 0x7B, 0)
	emu.frameadvance()
	C.DeleteFileA(path)
	C.MoveFileA(default_file_path, path)
end
end



local ks = {}
for i = 0, 0x63f do
	local kind = memory.LHU(0x800a5920 + i * 0xc)
	if kind ~= 0 and kind < 0x8000 then
		table.insert(ks, kind)
	end
end
table.sort(ks)

local g_last_func_addr = 0
local g_addrsa = {}
do
	local addrsf = io.open([[C:\test\game\64\data\ys_object_funcs.txt]], "r")
	local exp = string.rep("[0-9a-f]", 8)
	local exp2 = "^" .. string.rep("[0-9a-f]", 4)
	for line in addrsf:lines() do
		local kind = tonumber(string.match(line, exp2), 16)
		for a_s in string.gmatch(line, exp) do
			local a = tonumber(a_s, 16)
			if not g_addrsa[a] then
				print(string.format("%08x: %04x", a, kind))
				g_addrsa[a] = kind
			end
		end
	end
	addrsf:close()
	for a, _ in pairs(g_addrsa) do
		memory.readbreak(a, function()
			g_last_func_addr = a
		end)
	end
end
local null_func_addr = nil
memory.pcbreak(0x80000180, function(pc)
	if memory.reg("epc") == 0 then
		print(string.format("@%08x ra=%08x, epc=%08x; last=%08x (%04x)",
			pc, memory.reg("ra"), memory.reg("epc"),
			g_last_func_addr, g_addrsa[g_last_func_addr]))
		null_func_addr = g_last_func_addr
	end
end)

-- --[[
local g_current_obj = 0
local g_current_deps = {}
local g_current_deps_a = {}

memory.pcbreak(0x8006bed4, function()
	print("deps")
	local p = memory.reg("s0") + 0x60
	for i, k in ipairs(g_current_deps) do
		memory.SH(p, k)
		p = p + 2
	end
	memory.SH(p, 0xffff)
end)
memory.pcbreak(0x8006bf30, function()
	memory.SH(memory.reg("s4") + 0, 0x0005)
end)
memory.pcbreak(0x8006bf54, function()
	local p = memory.LWU(memory.reg("sp") + 0x44)
	p = p + 0xc * 4
	memory.SH(p + 0, g_current_obj)
	memory.SH(p + 2, 0)
--	memory.SWC1(p + 4, 0xd8)
--	memory.SWC1(p + 8, 0x168)
end)


local g_a0, g_objatt_null = 0, false
memory.pcbreak(0x8004fea4, function()
	g_a0 = memory.reg("a0")
end)
memory.pcbreak(0x8004feac, function(pc)
	local v0 = memory.reg("v0")
	if v0 == 0 then
		print(string.format("NULL: %04x", g_a0))
		g_objatt_null = true
	end
end)

function find_object(k)
	local objsa = memory.LWU(0x800fca24+0x10)
	local objsn = memory.LHU(0x800fca24+6)
	local safety_counter = 0
	while safety_counter < 0x100 and objsn ~= 0 do
		local q = memory.LWU(objsa)
		if q ~= 0 then
			if q ~= 1 then
				local kind = memory.LHU(q + 0xc)
				if kind == k then
					return q
				end
			end
			objsn = objsn - 1
		end
		objsa = objsa + 4
		safety_counter = safety_counter + 1
	end
	if safety_counter == 0x100 then
		print("safety_counter == 0x100!")
	end
	return nil
end

local g_input_counter = 0
do
emu.atinput(function()
	joypad.set({B = find_object(0x4088) and g_input_counter % 2 ~= 0})
	g_input_counter = g_input_counter + 1
end)
end

-- do return end

function main()

local initsavename = "YSstatics_objattlist_init"
local ssdir = [[C:\test\game\64\data\ys_object_ss]]

-- savestate.savefile(initsavename)
emu.frameadvance()
emu.frameadvance()
emu.frameadvance()


print("start!")

local startkind = 0x45b7+1
local f = io.open([[C:\test\game\64\data\ys_object_deps.txt]], startkind and "a" or "w")
local childsf = io.open([[C:\test\game\64\data\ys_object_childs.txt]], startkind and "a" or "w")
local a = {}
for i, kind in ipairs(ks) do if not startkind or kind >= startkind then
	g_current_obj = kind
	g_current_deps = {0x4001,0x4002,0x4003,0x4004,0x4005,0x4006,0x4007,0x4008,0x4009,0x400a,0x400b,0x400c,0x400d,0x400e,0x400f,0x4010,0x4011,0x4012,0x4013,0x4014,0x4015,0x4016,0x4017,0x4018,0x4019,0x401a,0x401b,0x401c,0x401d,0x401e,0x401f,0x4020,0x4021,0x4022,0x4023,0x4024,0x4025,0x4026,0x4027,0x402c,0x4030,0x4031,0x4032,0x4033,0x4034,0x4035,0x4088,0x40f2,0x460c,0x460d, 0x4739}
	for _, o in ipairs(g_current_deps) do
		g_current_deps_a[o] = true
	end
	g_current_depfuncs = {}
	g_current_deps_a = {}
	local childs = {}
	
	local default_deps_len = #g_current_deps
	print(string.format("%d: %04x...", i, kind))
	
	local ok = false
	while not ok do
		savestate.loadfile(initsavename)
		g_objatt_null = false
		null_func_addr = nil
		g_input_counter = 0

		for i = 1, 480 do
			if i == 30 or i == 60 or i == 120 or i == 240 or i == 480 then
				takescreenshot(string.format([[%s\%04x_%d.png]], ssdir, kind, i))
			else
				if not ((kind == 0x406b or kind == 0x45b7) and i > 120) then
					emu.frameadvance()
				end
			end
			local q = find_object(kind)
			if q then
				local cn = memory.LHU(q + 0x184)
				local ca = q + 0x188
				while cn ~= 0 and ca ~= q + 0x188 + 0x14 * 4 do
					local t = memory.LWU(ca)
					if t ~= 0 then
						if t ~= 1 then
							local ck = memory.LHU(t + 0xc)
							childs[ck] = true
						end
						cn = cn - 1
					end
					ca = ca + 4
				end
			end
			
			if g_objatt_null then
				table.insert(g_current_deps, g_a0)
				g_current_deps_a[g_a0] = true
				break
			end
			if null_func_addr then
				local k = g_addrsa[null_func_addr]
				if g_current_deps_a[k] then
					print(string.format("g_current_deps_a[g_addra[%08x]]", null_func_addr))
					ok = true
					break
				end
				table.insert(g_current_deps, k)
				g_current_deps_a[k] = true
				g_current_depfuncs[k] = null_func_addr
				break
			end
		end
		if not (g_objatt_null or null_func_addr) then
			ok = true
		end
	end
	local s = string.format("%04x: ", kind)
	for j, k in ipairs(g_current_deps) do
		if j > default_deps_len then
			if g_current_depfuncs[k] then
				s = s .. string.format("!%08x", g_current_depfuncs[k])
			else
				s = s .. string.format("%04x", k)
			end
			if j ~= #g_current_deps then
				s = s .. ", "
			end
		end
	end
	f:write(s .. "\n")
	f:flush()
	local schilds = string.format("%04x: ", kind)
	for k, _ in pairs(childs) do
		print(string.format("child: %04x", k))
		if #schilds ~= 6 then
			schilds = schilds .. ", "
		end
		schilds = schilds .. string.format("%04x", k)
	end
	childsf:write(schilds .. "\n")
	childsf:flush()
end end
f:close()
childsf:close()
print("end")


end

main_wrap = coroutine.wrap(main)
function emu.frameadvance()
        coroutine.yield("frameadvance")
end
emu.atinput(function()
        if main_wrap and not main_wrap() then
                main_wrap = nil
        end
end)

-- ]]
