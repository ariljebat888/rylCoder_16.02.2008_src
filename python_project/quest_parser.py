
class QuestParser:
    def __init__(self):
        self.quests = []
        self.i_funcs = None
        self.open_quest = None
        self.open_phase = None
        self.ryl_version = 2

    def parse(self, funcs):
        self.i_funcs = funcs
        self.quests = []
        if self.ryl_version == 1:
            for f in funcs:
                if not f.is_external and f.name and f.data:
                    q = Quest(f.id, f.name)
                    for line in f.data:
                        try:
                            q_line = QLine(line, QLine.string_to_type(funcs[line.call_to].name))
                            q_line.owner_function = f.id
                            q.add_line(q_line)
                        except (ValueError, IndexError):
                            pass
                    self.quests.append(q)
        else:
            going_q = -1
            for f in funcs:
                for line in f.data:
                    try:
                        q_line = QLine(line, QLine.string_to_type(funcs[line.call_to].name))
                        q_line.owner_function = f.id
                        if q_line.type == QLine.KnownType.EQuestStart:
                            q_id = q_line.params[0].value
                            q = Quest(q_id)
                            going_q += 1
                            q.add_line(q_line)
                            self.quests.append(q)
                        elif going_q >= 0:
                            self.quests[going_q].add_line(q_line)
                    except (ValueError, IndexError):
                        pass

    def get_functions(self):
        for i in range(len(self.i_funcs)):
            self.i_funcs[i].data = []
        for i, quest in enumerate(self.quests):
            lines = quest.i_lines
            for line in lines:
                ln = ScriptLine(parameters=line.params)
                for f in self.i_funcs:
                    if f.name == QLine.type_to_string(line.type):
                        ln.call_to = f.id
                        break
                f2 = self.i_funcs[line.owner_function]
                f2.data.append(ln)
                f2.name = quest.id_string
                self.i_funcs[line.owner_function] = f2
        self.i_funcs = [f for f in self.i_funcs if f.is_external or f.data or not f.name]
        return self.i_funcs


class QLine:
    def __init__(self, line=None, a_type=None):
        self.params = line.parameters if line else []
        self.type = a_type
        self.owner_function = 0

    class KnownType:
        EQuestEnd = 0
        EQuestSkillPointBonus = 1
        EQuestStart = 2
        EQuestType = 3
        EQuestArea = 4
        EQuestTitle = 5
        EQuestDesc = 6
        EQuestShortDesc = 7
        EQuestIcon = 8
        EQuestCompleteSave = 9
        EQuestLevel = 10
        EQuestAward = 11
        EAddPhase = 12
        EPhase_Target = 13
        ETrigger_Start = 14
        ETrigger_Puton = 15
        ETrigger_Geton = 16
        ETrigger_Talk = 17
        ETrigger_Kill = 18
        ETrigger_Pick = 19
        ETrigger_Fame = 20
        ETrigger_LevelTalk = 21
        EElse = 22
        EEvent_Disappear = 23
        EEvent_Get = 24
        EEvent_Spawn = 25
        EEvent_MonsterDrop = 26
        EEvent_Award = 27
        EEvent_MsgBox = 28
        EEvent_Phase = 29
        EEvent_End = 30
        EEvent_AwardItem = 31
        EEvent_AddQuest = 32
        EEvent_Move = 33
        EEvent_TheaterMode = 34

    @staticmethod
    def string_to_type(txt):
        return getattr(QLine.KnownType, "E" + txt)

    @staticmethod
    def type_to_string(type_):
        for name, value in QLine.KnownType.__dict__.items():
            if value == type_:
                return name[1:]
        return ""


class Quest:
    def __init__(self, a_id=0, a_id_string=""):
        self.id = a_id
        self.id_string = a_id_string
        self.i_lines = []

    def add_line(self, com):
        if self.i_lines:
            com.owner_function = self.i_lines[0].owner_function
        self.i_lines.append(com)

    @property
    def name(self):
        for line in self.i_lines:
            if line.type == QLine.KnownType.EQuestTitle:
                return line.params[0].value
        return f"Quest {self.id}"


class ScriptLine:
    def __init__(self, parameters=None, call_to=0):
        self.parameters = parameters if parameters is not None else []
        self.call_to = call_to
