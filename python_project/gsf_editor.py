
from PyQt5.QtWidgets import QWidget, QVBoxLayout, QTableView
from PyQt5.QtGui import QStandardItemModel, QStandardItem


class GsfEditor(QWidget):
    def __init__(self):
        super().__init__()
        self.init_ui()

    def init_ui(self):
        main_layout = QVBoxLayout()
        self.table_view = QTableView()
        main_layout.addWidget(self.table_view)
        self.setLayout(main_layout)

    def populate_data(self, data):
        if not data:
            return
        model = QStandardItemModel()
        model.setHorizontalHeaderLabels(data[0].keys())
        for row in data:
            items = [QStandardItem(str(value)) for value in row.values()]
            model.appendRow(items)
        self.table_view.setModel(model)
