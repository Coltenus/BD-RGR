from psycopg2 import connect

class Model:
    def __init__(self) -> None:
        self.conn = connect(
            host='localhost',
            database='lab1',
            user='postgres',
            password='1111')
        if self.conn.closed == 1:
            raise Exception('Conncetion closed')
        