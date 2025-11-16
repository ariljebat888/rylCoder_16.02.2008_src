import os
import struct
import xml.etree.ElementTree as ET
from collections import defaultdict

class GsfCoder:
    def __init__(self, gsf_struct_path='gsfStruct-release.xml'):
        self.gsf_struct_path = gsf_struct_path
        self.gsf_definitions = self._load_gsf_definitions()

    def _load_gsf_definitions(self):
        """Loads the GSF file structure definitions from the XML file."""
        tree = ET.parse(self.gsf_struct_path)
        root = tree.getroot()

        definitions = defaultdict(lambda: {'versions': {}, 'default': None})
        for file_elem in root.findall('file'):
            file_name = file_elem.get('name')
            version = file_elem.get('version')

            if version:
                definitions[file_name]['versions'][version] = file_elem
            else:
                definitions[file_name]['default'] = file_elem

        return definitions

    def _get_file_definition(self, file_type, version=None):
        """Retrieves the correct file definition based on file type and version."""
        file_defs = self.gsf_definitions.get(file_type)
        if not file_defs:
            return None

        if version and version in file_defs['versions']:
            return file_defs['versions'][version]

        return file_defs.get('default')

    def decrypt(self, data):
        """Placeholder for the GSF decryption logic."""
        return data

    def data_to_struct(self, data, file_type, version=None):
        """Converts GSF binary data to a structured format."""
        file_def = self._get_file_definition(file_type, str(version))
        if file_def is None:
            raise ValueError(f"No definition found for file type '{file_type}' and version '{version}'")

        structure = file_def.find('structure')
        if structure is None:
            return []

        cells_def = structure.find('cells')
        cell_size = int(cells_def.get('size'))

        table = []
        offset = 0
        while offset < len(data):
            row_data = data[offset:offset + cell_size]
            if len(row_data) < cell_size:
                break

            row = {}
            cell_offset = 0
            for cell in cells_def.findall('cell'):
                cell_type = cell.get('type')
                name = cell.get('name')

                if 'char' in cell_type:
                    length = int(cell_type.split('[')[1].split(']')[0])
                    value = row_data[cell_offset:cell_offset+length].decode('utf-8', errors='ignore').rstrip('\x00')
                    cell_offset += length
                else:
                    fmt, size = self._get_format(cell_type)
                    value = struct.unpack_from(f'<{fmt}', row_data, cell_offset)[0]
                    cell_offset += size

                row[name] = value
            table.append(row)
            offset += cell_size

        return table

    def struct_to_data(self, table, file_type, version=None):
        """Converts a structured format to GSF binary data."""
        file_def = self._get_file_definition(file_type, str(version))
        if file_def is None:
            raise ValueError(f"No definition found for file type '{file_type}' and version '{version}'")

        structure = file_def.find('structure')
        if structure is None:
            return b''

        cells_def = structure.find('cells')
        cell_size = int(cells_def.get('size'))

        binary_data = bytearray()
        for row in table:
            row_data = bytearray(cell_size)
            cell_offset = 0
            for cell in cells_def.findall('cell'):
                cell_type = cell.get('type')
                name = cell.get('name')
                value = row.get(name, 0)

                if 'char' in cell_type:
                    length = int(cell_type.split('[')[1].split(']')[0])
                    encoded_value = str(value).encode('utf-8', errors='ignore')
                    # Pad the byte string to the specified length
                    padded_value = encoded_value.ljust(length, b'\x00')
                    row_data[cell_offset:cell_offset+length] = padded_value
                    cell_offset += length
                else:
                    fmt, size = self._get_format(cell_type)
                    struct.pack_into(f'<{fmt}', row_data, cell_offset, value)
                    cell_offset += size

            binary_data.extend(row_data)

        return bytes(binary_data)

    def _get_format(self, cell_type):
        if cell_type == 'byte': return 'B', 1
        if cell_type == 'i16': return 'h', 2
        if cell_type == 'ui16': return 'H', 2
        if cell_type == 'i32': return 'i', 4
        if cell_type == 'ui32': return 'I', 4
        if cell_type == 'float': return 'f', 4
        raise ValueError(f"Unknown cell type: {cell_type}")

if __name__ == '__main__':
    coder = GsfCoder()
    # Example usage:
    # item_script_def = coder._get_file_definition('ItemScript', '1600')
    # if item_script_def:
    #     print("Successfully loaded ItemScript definition for version 1600.")

    # Example for testing data_to_struct and struct_to_data
    # This requires sample data and a known structure.
    # For now, this serves as a structural placeholder.
