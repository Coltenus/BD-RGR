import psycopg2.errors
from psycopg2 import connect


class Model:
    def __init__(self) -> None:
        self.conn = connect(
            host='localhost',
            database='lab1',
            user='postgres',
            password='1111')
        if self.conn.closed == 1:
            raise Exception('Connection closed')
        self.cur = self.conn.cursor()

    def get_table(self, name: str, columns: list[str],
                  values: tuple[str, list[any, ...]] = ['None', []]) -> list[tuple[any, ...]]:
        try:
            col = self.make_str_list(columns)
            if col == '':
                col = '*'
            if values[0] == 'None' or len(values[1]) == 0:
                self.cur.execute(f"SELECT {col} FROM {name}")
            else:
                val = self.make_str_list(values[1])
                self.cur.execute(f"SELECT {col} FROM {name} WHERE {values[0]} in ({val})")
            return self.cur.fetchall()
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.UndefinedColumn:
            print(f"Деякі з цих стовпців не існують: {columns}")
        except psycopg2.errors.InFailedSqlTransaction as er:
            print(er.args[0])

    def get_element(self, name: str, columns: list[str],
                    value: tuple[str, any] = ['None', None]) -> tuple[any, ...]:
        try:
            col = self.make_str_list(columns)
            if col == '':
                col = '*'
            if value[0] == 'None' or value[1] is None:
                self.cur.execute(f"SELECT {col} FROM {name}")
            else:
                self.cur.execute(f"SELECT {col} FROM {name} WHERE {value[0]} in {value[1]}")
            return self.cur.fetchone()
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.UndefinedColumn:
            print(f"Деякі з цих стовпців не існують: {columns}")

    def create_table(self, name: str, columns: list[tuple[str, str, bool]]) -> None:
        try:
            if len(columns) > 0:
                col = ''
                for el in columns:
                    var_type = 'varchar(80)'
                    match el[1]:
                        case 'int':
                            var_type = 'integer'
                    if el[2]:
                        var_type = f"{var_type} NOT NULL"
                    col += f"{el[0]} {var_type},"
                col = col.removesuffix(',')
                self.cur.execute(f"CREATE TABLE {name} ({col})")
                self.conn.commit()
        except psycopg2.errors.DuplicateTable:
            print(f"Таблиця {name} вже існує")
        except psycopg2.errors.InFailedSqlTransaction as er:
            print(er.args[0])

    def delete_table(self, name: str) -> None:
        try:
            self.cur.execute(f"DROP TABLE {name}")
            self.conn.commit()
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.InFailedSqlTransaction as er:
            print(er.args[0])

    def add_element(self, name: str, attributes: list[str], values: list[any]) -> None:
        try:
            if len(attributes) > 0 and len(attributes) == len(values):
                attr = self.make_str_list(attributes)

                val = self.make_str_list(values, True)

                self.cur.execute(f"INSERT INTO {name} ({attr}) VALUES ({val})")
                self.conn.commit()
        except psycopg2.errors.NotNullViolation as er:
            print(er.args[0])
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.UndefinedColumn:
            print(f"Деякі з цих стовпців не існують: {attributes}")

    def del_elements(self, name: str, values: tuple[str, list[any, ...]]):
        try:
            if len(values[1]) > 0:
                val = self.make_str_list(values[1], True)

                self.cur.execute(f"DELETE FROM {name} WHERE {values[0]}=({val})")
                self.conn.commit()
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.UndefinedColumn:
            print(f"Цього стовпця не існує: {values[0]}")

    def edit_elements(self, name: str, sets: list[tuple[str, any]], values: list[str, list[any, ...]]):
        try:
            if len(values[1]) > 0 and len(sets) > 0:
                val = self.make_str_list(values[1], True)

                con = ''
                for el in sets:
                    el2 = el[1]
                    if type(el[1]) == str:
                        el2 = f"'{el2}'"
                    con += f"{el[0]}={el2},"
                con = con.removesuffix(',')

                self.cur.execute(f"UPDATE {name} SET {con} WHERE {values[0]}=({val})")
                self.conn.commit()
        except psycopg2.errors.UndefinedTable:
            print(f"Таблиці {name} не існує")
        except psycopg2.errors.UndefinedColumn as er:
            print(er.args[0])

    @staticmethod
    def make_str_list(values: list[any], need_quotes: bool = False) -> str:
        val = ''
        for el in values:
            if need_quotes and type(el) == str:
                el = f"'{el}'"
            val += f"{el},"
        val = val.removesuffix(',')
        return val
