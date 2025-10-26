
import sys
import os
from PyQt5.QtWidgets import QApplication, QMainWindow, QAction, qApp, QTabWidget, QWidget, QFileDialog
from PyQt5.QtGui import QIcon, QStandardItemModel, QStandardItem
from npc_editor import NpcEditor
from quest_editor import QuestEditor
from gsf_editor import GsfEditor
from mcf_coder import decrypt_area
from mcf_decompiler import CMcfDecompiler
from npc_parser import NpcParser
from quest_parser import QuestParser
from gsf_coder import GsfCoder


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.init_ui()

    def init_ui(self):
        self.statusBar()

        menubar = self.menuBar()
        file_menu = menubar.addMenu('&File')

        open_action = QAction('&Open', self)
        open_action.setShortcut('Ctrl+O')
        open_action.setStatusTip('Open MCF file')
        open_action.triggered.connect(self.open_file)
        file_menu.addAction(open_action)

        exit_action = QAction(QIcon('exit.png'), '&Exit', self)
        exit_action.setShortcut('Ctrl+Q')
        exit_action.setStatusTip('Exit application')
        exit_action.triggered.connect(qApp.quit)
        file_menu.addAction(exit_action)

        self.tabs = QTabWidget()
        self.npc_editor_tab = NpcEditor()
        self.quest_editor_tab = QuestEditor()
        self.gsf_editor_tab = GsfEditor()
        self.script_editor_tab = QWidget()

        self.tabs.addTab(self.npc_editor_tab, "NPC Editor")
        self.tabs.addTab(self.quest_editor_tab, "Quest Editor")
        self.tabs.addTab(self.gsf_editor_tab, "GSF Editor")
        self.tabs.addTab(self.script_editor_tab, "Script Editor")

        self.setCentralWidget(self.tabs)

        self.setGeometry(300, 300, 800, 600)
        self.setWindowTitle('Ryl MC&F Editor')
        self.show()

    def open_file(self):
        options = QFileDialog.Options()
        file_name, _ = QFileDialog.getOpenFileName(self, "Open File", "", "All Files (*);;MCF Files (*.mcf);;GSF Files (*.gsf)", options=options)
        if file_name:
            with open(file_name, 'rb') as f:
                data = f.read()

            base_name = os.path.basename(file_name)
            file_title, file_ext = os.path.splitext(base_name)

            if file_ext.lower() == '.mcf':
                decrypted_data = decrypt_area(data)
                decompiler = CMcfDecompiler()
                decompiler.decompile(decrypted_data)
                npc_parser = NpcParser()
                npc_parser.parse(decompiler.functions)
                self.populate_npc_tree(npc_parser.npcs)
                quest_parser = QuestParser()
                quest_parser.parse(decompiler.functions)
                self.populate_quest_tree(quest_parser.quests)
            elif file_ext.lower() == '.gsf':
                gsf_coder = GsfCoder('gsfStruct-release.xml')
                parsed_data = gsf_coder.parse_gsf(file_title, data)
                self.gsf_editor_tab.populate_data(parsed_data)

    def populate_npc_tree(self, npcs):
        self.npcs = npcs
        model = QStandardItemModel()
        self.npc_editor_tab.npc_tree.setModel(model)
        for npc in npcs:
            item = QStandardItem(npc.name)
            model.appendRow(item)
        self.npc_editor_tab.npc_tree.selectionModel().selectionChanged.connect(self.npc_selection_changed)

    def npc_selection_changed(self, selected, deselected):
        index = selected.indexes()[0]
        npc = self.npcs[index.row()]
        self.npc_editor_tab.populate_npc_details(npc)

    def populate_quest_tree(self, quests):
        self.quests = quests
        model = QStandardItemModel()
        self.quest_editor_tab.quest_tree.setModel(model)
        for quest in quests:
            item = QStandardItem(quest.name)
            model.appendRow(item)
        self.quest_editor_tab.quest_tree.selectionModel().selectionChanged.connect(self.quest_selection_changed)

    def quest_selection_changed(self, selected, deselected):
        index = selected.indexes()[0]
        quest = self.quests[index.row()]
        self.quest_editor_tab.populate_quest_details(quest)


if __name__ == '__main__':
    app = QApplication(sys.argv)
    ex = MainWindow()
    sys.exit(app.exec_())
