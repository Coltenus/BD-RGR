from controller import Controller

def main():
    try:
        con = Controller()
        con.run()
    except Exception as err:
        if len(err.args) > 0:
            for arg in err.args:
                print(arg)
        else:
            print('There\'s an error')

if __name__ == "__main__":
    main()