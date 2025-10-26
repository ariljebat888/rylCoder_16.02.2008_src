
class CMcfBase:
    class SFunction:
        def __init__(self):
            self.id = 0
            self.name = ""
            self.is_external = False
            self.return_type = None
            self.parameter_types = []
            self.data = []

    class SScriptLine:
        def __init__(self):
            self.call_to = 0
            self.parameters = []

    class SParamElem:
        def __init__(self):
            self.type = None
            self.value = None

    class DataType:
        EString = 0
        EInteger = 1
        EFloat = 2
        EBool = 3
