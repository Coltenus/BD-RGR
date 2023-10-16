class View:
    @staticmethod
    def show_table(name: str, attr: list[str], table: list[tuple[any, ...]], end: str = '\t'):
        try:
            print(f"\n{name}:")
            for par in attr:
                print(par, end=end)
            print()
            for el in table:
                for i in range(len(attr)):
                    print(el[i], end=end)
                print()
            print()
        except TypeError:
            print(f"Один з типів не співпадає:\nname: {type(name)}\nattr: {type(attr)}\ntable: {type(table)}")
