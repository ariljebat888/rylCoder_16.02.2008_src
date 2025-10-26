
from mcf_base import CMcfBase
import struct


class CMcfDecompiler:
    def __init__(self):
        self.functions = []
        self.ryl_file_version = 0
        self.ryl_file_type = 0

    def decompile(self, data):
        # This is a simplified version of the original decompiler.
        # It assumes a very specific structure and may not work for all .mcf files.
        # It is provided as a starting point for further development.
        num_functions = struct.unpack_from('<I', data, 0)[0]
        offset = 4
        for _ in range(num_functions):
            func = CMcfBase.SFunction()
            func.id = struct.unpack_from('<I', data, offset)[0]
            offset += 4
            name_len = struct.unpack_from('<I', data, offset)[0]
            offset += 4
            func.name = data[offset:offset + name_len].decode('ascii')
            offset += name_len
            func.is_external = struct.unpack_from('<?', data, offset)[0]
            offset += 1
            func.return_type = CMcfBase.DataType(struct.unpack_from('<B', data, offset)[0])
            offset += 1
            num_params = struct.unpack_from('<I', data, offset)[0]
            offset += 4
            for _ in range(num_params):
                param_type = CMcfBase.DataType(struct.unpack_from('<B', data, offset)[0])
                offset += 1
                func.parameter_types.append(param_type)
            num_script_lines = struct.unpack_from('<I', data, offset)[0]
            offset += 4
            for _ in range(num_script_lines):
                line = CMcfBase.SScriptLine()
                line.call_to = struct.unpack_from('<I', data, offset)[0]
                offset += 4
                num_line_params = struct.unpack_from('<I', data, offset)[0]
                offset += 4
                for _ in range(num_line_params):
                    param = CMcfBase.SParamElem()
                    param.type = CMcfBase.DataType(struct.unpack_from('<B', data, offset)[0])
                    offset += 1
                    if param.type == CMcfBase.DataType.EString:
                        val_len = struct.unpack_from('<I', data, offset)[0]
                        offset += 4
                        param.value = data[offset:offset + val_len].decode('ascii')
                        offset += val_len
                    elif param.type == CMcfBase.DataType.EInteger:
                        param.value = struct.unpack_from('<i', data, offset)[0]
                        offset += 4
                    elif param.type == CMcfBase.DataType.EFloat:
                        param.value = struct.unpack_from('<f', data, offset)[0]
                        offset += 4
                    elif param.type == CMcfBase.DataType.EBool:
                        param.value = struct.unpack_from('<?', data, offset)[0]
                        offset += 1
                    line.parameters.append(param)
                func.data.append(line)
            self.functions.append(func)
