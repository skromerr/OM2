import numpy as np
import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
from decimal import Decimal as dcm

def rosen():
    xList = np.arange(-10, 10, 0.05)
    yList = np.arange(-10, 10, 0.05)
    X, Y = np.meshgrid(xList, yList)

    Z = 100 * (Y - X**2)**2 + (1 - X)**2

    return X, Y, Z

def quadratic():
    xList = np.arange(-10, 10, 0.05)
    yList = np.arange(-10, 10, 0.05)
    X, Y = np.meshgrid(xList, yList)

    Z = 100 * (Y - X)**2 + (1 - X)**2

    return X, Y, Z

def variant():
    xList = np.arange(-10, 10, 0.05)
    yList = np.arange(-10, 10, 0.05)
    X, Y = np.meshgrid(xList, yList)

    x1 = 1.0 + ((X - 3.0) / 2.0)**2 + ((Y - 2.0))**2
    x2 = 1.0 + ((X - 3.0))**2 + ((Y - 1.0) / 3.0)**2
    Z  = ((3.0/x1) + (1.0/x2))

    return X, Y, Z


def testing(num):
    switch = {
        "1": rosen(),
        "2": quadratic(),
        "3": variant(),
    }
    return switch.get(num, "Invalid input")

def main():
    x = []
    y = []

    with open("coords.txt") as file:
        for line in file:
            xC, yC = line.split()
            x.append(dcm(xC))
            y.append(dcm(yC))


    figure, axes = plt.subplots(1, 1)

    num = input("Enter the test: \n1) Rosenbrock \n2) Quadratic \n3) Function for 12 variant\n")

    if (num == "3") :  _levels = np.arange(0.5, 5, 0.5)
    else : _levels = np.arange(0, 100, 1)
    
    X, Y, Z = testing(num)

    plt.xlim(-10, 10)
    plt.ylim(-10, 10)

    axes.xaxis.set_major_locator(ticker.MultipleLocator(1))
    axes.xaxis.set_minor_locator(ticker.MultipleLocator(1))
    axes.yaxis.set_major_locator(ticker.MultipleLocator(1))
    axes.yaxis.set_minor_locator(ticker.MultipleLocator(1))

    plt.xlabel("x1")
    plt.ylabel("x2")

    plt.plot(x, y, '-o', markersize=6, color='w')
    plt.plot(x[-1], y[-1], 'o', markersize=9, color='r')

    _contourf = axes.contourf(X, Y, Z, levels=_levels, extend='both')
    cs = _contourf
    cs.cmap.set_over('black', alpha = 0.8)
    cs.cmap.set_under('black', alpha = 0.8)
    cs.changed()

    figure.colorbar(_contourf, shrink=1)

    plt.grid()
    axes.set_aspect(1)
    # plt.savefig("graphics/test_s_1.png")
    plt.show()

if __name__ == "__main__":
    main()
