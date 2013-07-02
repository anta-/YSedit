local ks = {}
for i = 0, 0x63f do
	local kind = memory.LHU(0x800a5920 + i * 0xc)
	if kind ~= 0 and kind < 0x8000 then
		table.insert(ks, kind)
	end
end
table.sort(ks)

local g_current_obj = 0
local g_current_funcs = {}
local g_current_deps = {}
local g_end = false

-- --[[
memory.pcbreak(0x8006bed4, function()
	print(string.format("deps %04x", g_current_obj))
	local p = memory.reg("s0") + 0x60
	for i, k in ipairs(g_current_deps) do
		memory.SH(p, k)
		p = p + 2
	end
	memory.SH(p, g_current_obj)
	p = p + 2
	memory.SH(p, 0xffff)
end)
memory.pcbreak(0x8006bf30, function()
	memory.SH(memory.reg("s4") + 0, 1)
end)
memory.pcbreak(0x8006bf54, function()
	local p = memory.LWU(memory.reg("sp") + 0x44)
	memory.SH(p + 0, g_current_obj)
	memory.SH(p + 4, 0)
	memory.SWC1(p + 8, 0)
	memory.SWC1(p + 0xc, 0)
end)
memory.pcbreak(0x800503e0, function()
	print("@800503e0")
	g_end = true
end)
-- ]]

memory.pcbreak(0x80056718, function(pc)
	local a0, a1, a2, ra =
		memory.reg("a0"), memory.reg("a1"), memory.reg("a2"), memory.reg("ra")
	local addr = memory.LWU(a0 + 0x10) + a1 * 4
	local val = memory.LWU(addr)
	if val ~= 0 and val ~= 1 then return end
	if ra == 0x80042924 then return end
	print(string.format("0x%08x, -- %08x, %x, %08x, ra=%08x", addr, a0, a1, a2, ra))
	table.insert(g_current_funcs, addr)
end)

memory.pcbreak(0x80000180, function(pc)
	if memory.reg("epc") == 0 then
		print(string.format("@%08x ra=%08x, epc=%08x",
			pc, memory.reg("ra"), memory.reg("epc")))
	end
end)


function main()

local initsavename = "YSstatics_objfuncs_init"

-- savestate.savefile(initsavename)
emu.frameadvance()
emu.frameadvance()
emu.frameadvance()


print("start!")
savestate.loadfile(initsavename)

local f = io.open([[C:\test\game\64\data\ys_object_funcs.txt]], "w")
local a = {}
for i, kind in ipairs(ks) do
	g_current_obj = kind
	g_current_funcs = {}
	print(string.format("%d: %04x...", i, kind))
	
	if kind == 0x414d then
		g_current_funcs = {0x800fca3c}
	else
		savestate.loadfile(initsavename)
		for i = 1, 30 do
			emu.frameadvance()
			if g_end then
				g_end = false
				break
			end
		end
	end
	local s = string.format("%04x: ", kind)
	for j, f in ipairs(g_current_funcs) do
		s = s .. string.format("%08x", f)
		if j ~= #g_current_funcs then
			s = s .. ", "
		end
	end
	f:write(s .. "\n")
	f:flush()
end
f:close()
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
