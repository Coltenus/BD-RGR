from model import Model
from view import View


class Controller:
    def __init__(self) -> None:
        self.model = Model()

    def run(self) -> None:
        name = "patients"
        columns = ['patient_id', 'name']
        table = self.model.get_table(name, columns)
        View.show_table(name, columns, table, '\t\t')

        name = "test_table"
        columns = [['id', 'int', True], ['name', 'str', False], ['type', 'str', True]]
        self.model.create_table(name, columns)
        columns = ['id', 'name', 'type']
        self.model.add_element(name, columns, [1, 'Test', 'test_type'])
        table = self.model.get_table(name, columns)
        View.show_table(name, columns, table, '\t\t')
        self.model.del_elements(name, ['name', ['Test1']])
        table = self.model.get_table(name, columns)
        View.show_table(name, columns, table, '\t\t')
        self.model.edit_elements(name, [['type', 'test_edit']], ['id', [1]])
        table = self.model.get_table(name, columns)
        View.show_table(name, columns, table, '\t\t')
        self.model.delete_table(name)
