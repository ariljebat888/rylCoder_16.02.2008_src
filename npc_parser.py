
from mcf_base import CMcfBase


class NpcParser:
    def __init__(self):
        self.npcs = []
        self.not_found_npc_lines = []
        self.functions = []
        self.loaded_npc = None
        self.ryl_version = 0

    def parse(self, funcs):
        self.npcs = []
        self.functions = funcs
        npc_lines = []
        i = 0
        for f in funcs:
            for v in f.data:
                try:
                    line_type = NPC_LINE_TYPES.index(funcs[v.call_to].name)
                    if line_type >= 0:
                        n_line = NpcLineEdit(
                            type=line_type,
                            params=v.parameters,
                            pos=i,
                            owner_func=f.id
                        )
                        npc_lines.append(n_line)

                        if n_line.type == NpcLineType.ESetNpc:
                            if self.ryl_version < 1:
                                self.ryl_version = 2 if len(n_line.params) > 5 else 1
                            npc = NpcScript(n_line.params[1].value, self.ryl_version)
                            self.npcs.append(npc)
                            n_line.npc_id = n_line.params[1].value
                            npc.add_line(n_line)
                        else:
                            npc_i = self.find_npc_index(n_line.params[0].value)
                            if npc_i >= 0:
                                n_line.npc_id = n_line.params[0].value
                                self.npcs[npc_i].add_line(n_line)
                            else:
                                vv = LineWithPos(line=v, pos=i, owner_function=f.id)
                                self.not_found_npc_lines.append(vv)
                except ValueError:
                    vv = LineWithPos(line=v, pos=i, owner_function=f.id)
                    self.not_found_npc_lines.append(vv)
                i += 1

    def get_functions(self):
        lines = []
        for npc in self.npcs:
            for line in npc.lines():
                ln = ScriptLine(parameters=line.params)
                for f in self.functions:
                    if f.name == NPC_LINE_TYPES[line.type]:
                        ln.call_to = f.id
                        break
                lp = LineWithPos(
                    owner_function=line.owner_func,
                    line=ln,
                    pos=line.pos
                )
                lines.append(lp)
        for line in self.not_found_npc_lines:
            lines.append(line)
        lines.sort(key=lambda l: l.pos)

        for i in range(len(self.functions)):
            self.functions[i].data = []

        for line in lines:
            f = self.functions[line.owner_function]
            f.data.append(line.line)
            self.functions[line.owner_function] = f

        return self.functions

    def find_npc_index(self, npc_id):
        for i, npc in enumerate(self.npcs):
            if npc.id == npc_id:
                return i
        return -1


class NpcScript:
    def __init__(self, npc_id, version):
        self.id = npc_id
        self.i_lines = []
        self.ryl_version = version

    def add_line(self, com):
        if self.i_lines:
            com.owner_func = self.i_lines[0].owner_func
        self.i_lines.append(com)

    def lines(self, line_type=None):
        if line_type is None:
            return self.i_lines
        return [line for line in self.i_lines if line.type == line_type]

    @property
    def name(self):
        for line in self.i_lines:
            if line.type == NpcLineType.ESetNpc:
                if self.ryl_version == 1:
                    return line.params[4].value
                else:
                    return line.params[5].value.split('\\\\')[0]
        return ""


class NpcLineEdit:
    def __init__(self, npc_id=0, params=None, type=None, pos=0, owner_func=0):
        self.npc_id = npc_id
        self.params = params if params is not None else []
        self.type = type
        self.pos = pos
        self.owner_func = owner_func


class NpcLineType:
    EAddItem = 1
    ESetPosition = 2
    EAddPopup = 3
    EAddWords = 4
    EAddDialog = 5
    ESetNpc = 6
    EAddQuestWords = 7
    EAddZoneMove = 8
    EAddQuest = 9
    ESetDropBase = 10
    ESetDropGrade = 11
    ESetNpcAttribute = 12


NPC_LINE_TYPES = [
    "", "AddItem", "SetPosition", "AddPopup", "AddWords", "AddDialog", "SetNPC",
    "AddQuestWords", "AddZoneMove", "AddQuest", "SetDropBase", "SetDropGrade", "SetNPCAttribute"
]


class LineWithPos:
    def __init__(self, line, pos, owner_function):
        self.line = line
        self.pos = pos
        self.owner_function = owner_function


class ScriptLine:
    def __init__(self, parameters=None, call_to=0):
        self.parameters = parameters if parameters is not None else []
        self.call_to = call_to
