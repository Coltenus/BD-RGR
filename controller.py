from model import Model
from view import View

class Controller:
    def __init__(self) -> None:
        self.model = Model()
        self.view = View()

    def run(self) -> None:
        pass