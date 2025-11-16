
from PyQt5.QtWidgets import QWidget, QHBoxLayout, QTreeView, QGroupBox, QVBoxLayout, QLineEdit, QLabel, QFormLayout, QComboBox, QTabWidget, QTextEdit
from npc_parser import NpcLineType
from gsf_coder import GsfCoder


class NpcEditor(QWidget):
    def __init__(self):
        super().__init__()
        self.gsf_coder = GsfCoder()
        self.init_ui()

    def open_gsf_file(self, file_path):
        with open(file_path, 'rb') as f:
            data = f.read()

        decrypted_data = self.gsf_coder.decrypt(data)

        file_name = os.path.basename(file_path)
        file_type = os.path.splitext(file_name)[0]

        # version should be detected or passed in
        version = "1600" # default for now

        structured_data = self.gsf_coder.data_to_struct(decrypted_data, file_type, version)

        # we need to populate the editor with the structured_data

    def init_ui(self):
        main_layout = QHBoxLayout()

        # Left side: Tree view of NPCs
        self.npc_tree = QTreeView()
        main_layout.addWidget(self.npc_tree, 1)

        # Right side: NPC details
        self.npc_details = QGroupBox("NPC Details")
        details_layout = QVBoxLayout()

        # General section
        general_group = QGroupBox("General")
        general_layout = QFormLayout()
        self.npc_id = QLineEdit()
        self.npc_id.setReadOnly(True)
        self.npc_name = QLineEdit()
        self.npc_desc = QLineEdit()
        self.npc_texture = QLineEdit()
        general_layout.addRow(QLabel("ID:"), self.npc_id)
        general_layout.addRow(QLabel("Name:"), self.npc_name)
        general_layout.addRow(QLabel("Description:"), self.npc_desc)
        general_layout.addRow(QLabel("Texture:"), self.npc_texture)
        general_group.setLayout(general_layout)
        details_layout.addWidget(general_group)

        # Location section
        location_group = QGroupBox("Location")
        location_layout = QFormLayout()
        self.npc_loc_zone = QLineEdit()
        self.npc_loc_x = QLineEdit()
        self.npc_loc_y = QLineEdit()
        self.npc_loc_z = QLineEdit()
        self.npc_loc_dir = QLineEdit()
        location_layout.addRow(QLabel("Zone:"), self.npc_loc_zone)
        location_layout.addRow(QLabel("X:"), self.npc_loc_x)
        location_layout.addRow(QLabel("Y:"), self.npc_loc_y)
        location_layout.addRow(QLabel("Z:"), self.npc_loc_z)
        location_layout.addRow(QLabel("Direction:"), self.npc_loc_dir)
        location_group.setLayout(location_layout)
        details_layout.addWidget(location_group)

        # Teleport section
        teleport_group = QGroupBox("Teleport")
        teleport_layout = QFormLayout()
        self.npc_tele_zone = QLineEdit()
        self.npc_tele_x = QLineEdit()
        self.npc_tele_y = QLineEdit()
        self.npc_tele_z = QLineEdit()
        teleport_layout.addRow(QLabel("Zone:"), self.npc_tele_zone)
        teleport_layout.addRow(QLabel("X:"), self.npc_tele_x)
        teleport_layout.addRow(QLabel("Y:"), self.npc_tele_y)
        teleport_layout.addRow(QLabel("Z:"), self.npc_tele_z)
        teleport_group.setLayout(teleport_layout)
        details_layout.addWidget(teleport_group)

        # Shop section
        shop_group = QGroupBox("Shop")
        shop_layout = QVBoxLayout()
        self.shop_index = QComboBox()
        self.shop_tabs = QTabWidget()
        shop_layout.addWidget(self.shop_index)
        shop_layout.addWidget(self.shop_tabs)
        shop_group.setLayout(shop_layout)
        details_layout.addWidget(shop_group)

        # Texts section
        texts_group = QGroupBox("Texts")
        texts_layout = QVBoxLayout()
        self.npc_texts = QTextEdit()
        texts_layout.addWidget(self.npc_texts)
        texts_group.setLayout(texts_layout)
        details_layout.addWidget(texts_group)

        self.npc_details.setLayout(details_layout)
        main_layout.addWidget(self.npc_details, 2)

        self.setLayout(main_layout)

    def populate_npc_details(self, npc):
        self.npc_id.setText(str(npc.id))
        self.npc_name.setText(npc.name)

        set_npc_lines = npc.lines(NpcLineType.ESetNpc)
        if set_npc_lines:
            set_npc_line = set_npc_lines[0]
            if npc.ryl_version == 2:
                self.npc_desc.setText(set_npc_line.params[5].value.split('\\\\')[1] if '\\\\' in set_npc_line.params[5].value else '')
                self.npc_texture.setText(set_npc_line.params[4].value)
            else:
                self.npc_desc.setEnabled(False)
                self.npc_texture.setText(set_npc_line.params[3].value)
            self.npc_loc_zone.setText(str(set_npc_line.params[0].value))

        set_pos_lines = npc.lines(NpcLineType.ESetPosition)
        if set_pos_lines:
            set_pos_line = set_pos_lines[0]
            self.npc_loc_x.setText(str(set_pos_line.params[2].value))
            self.npc_loc_y.setText(str(set_pos_line.params[3].value))
            self.npc_loc_z.setText(str(set_pos_line.params[4].value))
            self.npc_loc_dir.setText(str(set_pos_line.params[1].value))

        add_zone_move_lines = npc.lines(NpcLineType.EAddZoneMove)
        if add_zone_move_lines:
            add_zone_move_line = add_zone_move_lines[0]
            self.npc_tele_zone.setText(str(add_zone_move_line.params[1].value))
            self.npc_tele_x.setText(str(add_zone_move_line.params[2].value))
            self.npc_tele_y.setText(str(add_zone_move_line.params[3].value))
            self.npc_tele_z.setText(str(add_zone_move_line.params[4].value))

        self.npc_texts.clear()
        for line in npc.lines():
            if line.type in [NpcLineType.EAddWords, NpcLineType.EAddQuestWords, NpcLineType.EAddPopup, NpcLineType.EAddDialog]:
                self.npc_texts.append(line.params[1].value)
