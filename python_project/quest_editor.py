
from PyQt5.QtWidgets import QWidget, QHBoxLayout, QTreeView, QGroupBox, QVBoxLayout, QLineEdit, QLabel, QFormLayout, QCheckBox, QComboBox, QTextEdit
from quest_parser import QLine


class QuestEditor(QWidget):
    def __init__(self):
        super().__init__()
        self.init_ui()

    def init_ui(self):
        main_layout = QHBoxLayout()

        # Left side: Tree view of quests
        self.quest_tree = QTreeView()
        main_layout.addWidget(self.quest_tree, 1)

        # Right side: Quest details
        self.quest_details = QGroupBox("Quest Details")
        details_layout = QVBoxLayout()

        # General section
        general_group = QGroupBox("General")
        general_layout = QFormLayout()
        self.quest_id = QLineEdit()
        self.quest_id.setReadOnly(True)
        self.quest_min_lvl = QLineEdit()
        self.quest_max_lvl = QLineEdit()
        self.quest_class = QComboBox()
        self.quest_nation = QComboBox()
        self.quest_type = QComboBox()
        self.quest_party = QCheckBox("Party Quest")
        self.quest_add_to_completed = QCheckBox("Add to completed list")
        self.quest_allow_delete = QCheckBox("Allow Delete")
        general_layout.addRow(QLabel("ID:"), self.quest_id)
        general_layout.addRow(QLabel("Min Level:"), self.quest_min_lvl)
        general_layout.addRow(QLabel("Max Level:"), self.quest_max_lvl)
        general_layout.addRow(QLabel("Class:"), self.quest_class)
        general_layout.addRow(QLabel("Nation:"), self.quest_nation)
        general_layout.addRow(QLabel("Type:"), self.quest_type)
        general_layout.addRow(self.quest_party)
        general_layout.addRow(self.quest_add_to_completed)
        general_layout.addRow(self.quest_allow_delete)
        general_group.setLayout(general_layout)
        details_layout.addWidget(general_group)

        # Title section
        title_group = QGroupBox("Title")
        title_layout = QFormLayout()
        self.quest_title_name = QLineEdit()
        self.quest_title_level = QLineEdit()
        self.quest_title_short_desc = QTextEdit()
        self.quest_title_award = QTextEdit()
        self.quest_title_desc = QTextEdit()
        title_layout.addRow(QLabel("Name:"), self.quest_title_name)
        title_layout.addRow(QLabel("Level:"), self.quest_title_level)
        title_layout.addRow(QLabel("Short Description:"), self.quest_title_short_desc)
        title_layout.addRow(QLabel("Award:"), self.quest_title_award)
        title_layout.addRow(QLabel("Description:"), self.quest_title_desc)
        title_group.setLayout(title_layout)
        details_layout.addWidget(title_group)

        # Phases section
        phases_group = QGroupBox("Phases")
        phases_layout = QFormLayout()
        self.quest_phases = QComboBox()
        self.quest_phase_name = QLineEdit()
        self.quest_phase_zone = QLineEdit()
        phases_layout.addRow(QLabel("Phase:"), self.quest_phases)
        phases_layout.addRow(QLabel("Name:"), self.quest_phase_name)
        phases_layout.addRow(QLabel("Zone:"), self.quest_phase_zone)
        phases_group.setLayout(phases_layout)
        details_layout.addWidget(phases_group)

        self.quest_details.setLayout(details_layout)
        main_layout.addWidget(self.quest_details, 2)

        self.setLayout(main_layout)

    def populate_quest_details(self, quest):
        self.quest_id.setText(str(quest.id))
        quest_start_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestStart]
        if quest_start_lines:
            quest_start_line = quest_start_lines[0]
            self.quest_min_lvl.setText(str(quest_start_line.params[1].value))
            self.quest_max_lvl.setText(str(quest_start_line.params[2].value))

        quest_title_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestTitle]
        if quest_title_lines:
            self.quest_title_name.setText(quest_title_lines[0].params[0].value)

        quest_level_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestLevel]
        if quest_level_lines:
            self.quest_title_level.setText(quest_level_lines[0].params[0].value)

        quest_short_desc_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestShortDesc]
        if quest_short_desc_lines:
            self.quest_title_short_desc.setText(quest_short_desc_lines[0].params[0].value)

        quest_award_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestAward]
        if quest_award_lines:
            self.quest_title_award.setText(quest_award_lines[0].params[0].value)

        quest_desc_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EQuestDesc]
        if quest_desc_lines:
            self.quest_title_desc.setText(quest_desc_lines[0].params[0].value)

        self.quest_phases.clear()
        add_phase_lines = [line for line in quest.i_lines if line.type == QLine.KnownType.EAddPhase]
        for line in add_phase_lines:
            self.quest_phases.addItem(line.params[2].value)
