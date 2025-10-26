
import xml.etree.ElementTree as ET
import struct


def decrypt_gsf(data):
    # This is a placeholder for the actual decryption logic.
    # The decryption algorithm is not yet known.
    return data


class GsfCoder:
    def __init__(self, xml_file):
        self.tree = ET.parse(xml_file)
        self.root = self.tree.getroot()

    def parse_gsf(self, file_name, gsf_data):
        decrypted_data = decrypt_gsf(gsf_data)
        file_def = self.root.find(f".//file[@name='{file_name}']")
        if file_def is None:
            raise ValueError(f"File definition not found for '{file_name}'")

        structure = file_def.find("structure")
        if structure is None:
            return None  # No structure defined

        cells_def = structure.find("cells")
        size = int(cells_def.get("size"))
        has_columns = bool(int(cells_def.get("hascolumns")))

        data = []
        offset = 0
        while offset < len(decrypted_data):
            row = {}
            for cell in cells_def.findall("cell"):
                cell_type = cell.get("type")
                name = cell.get("name")
                if "char" in cell_type:
                    length = int(cell_type.split('[')[1].split(']')[0])
                    value = decrypted_data[offset:offset + length].decode('ascii').rstrip('\x00')
                    offset += length
                elif cell_type == 'byte':
                    value = struct.unpack_from('<B', decrypted_data, offset)[0]
                    offset += 1
                elif cell_type == 'i16':
                    value = struct.unpack_from('<h', decrypted_data, offset)[0]
                    offset += 2
                elif cell_type == 'ui16':
                    value = struct.unpack_from('<H', decrypted_data, offset)[0]
                    offset += 2
                elif cell_type == 'i32':
                    value = struct.unpack_from('<i', decrypted_data, offset)[0]
                    offset += 4
                elif cell_type == 'ui32':
                    value = struct.unpack_from('<I', decrypted_data, offset)[0]
                    offset += 4
                elif cell_type == 'float':
                    value = struct.unpack_from('<f', decrypted_data, offset)[0]
                    offset += 4

                replace_id = cell.get("replace")
                if replace_id:
                    replacement = file_def.find(f".//replacement[@id='{replace_id}']")
                    if replacement:
                        for elem in replacement.findall("elem"):
                            if int(elem.get("from")) == value:
                                value = elem.get("to")
                                break
                row[name] = value
            data.append(row)
        return data
