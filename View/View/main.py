import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import griddata
import csv


path_type = 'RoCp'
# Функция для чтения данных из файла
def read_data(file_path):
    x_values = []
    y_values = []
    with open(file_path, 'r') as file:
        for line in file:
            x, y = map(float, line.strip().split())
            x_values.append(x)
            y_values.append(y)
    return x_values, y_values


# Функция для автоматического определения количества строк и столбцов
def determine_grid_size(x_values):
    # Предполагаем, что количество точек в строке одинаково
    total_points = len(x_values)
    cols = x_values.count(x_values[0])  # Количество точек в первой строке
    rows = total_points // cols  # Количество строк
    return rows, cols


# Функция для построения графика
def plot_grid(grid_x, grid_y):
    plt.figure(figsize=(7, 6))

    # Соединяем точки по горизонтали (черные линии)
    for i in range(grid_x.shape[0]):
        plt.plot(grid_x[i, :], grid_y[i, :], '-', color='black')

    # Соединяем точки по вертикали (черные линии)
    for j in range(grid_x.shape[1]):
        plt.plot(grid_x[:, j], grid_y[:, j], '-', color='black')

    plt.title('Grid')
    plt.xlabel('R Values')
    plt.ylabel('Z Values')
    plt.grid(True)
    plt.show()


# Основной код
if __name__ == "__main__":
    file_path = 'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/New/PyGrid.txt'  # Укажите путь к вашему файлу
    x_values, y_values = read_data(file_path)

    # Определяем количество строк и столбцов автоматически
    cols, rows = determine_grid_size(x_values)

    # Создаем матрицу сетки
    grid_x = np.array(x_values).reshape(rows, cols)
    grid_y = np.array(y_values).reshape(rows, cols)

    # Строим график
    plot_grid(grid_x, grid_y)


#'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/New/PyGrid.txt'
def parse_float(value, decimal_separator):
    """Парсить строку с учетом разделителя между целой и дробной частью."""
    if decimal_separator == ',':
        value = value.replace(',', '.')
    return float(value)


def read_data_k(file_path, decimal_separator='.'):
    """Чтение данных из файла и преобразование их в массивы x, y, T."""
    x_vals = []
    y_vals = []
    t_vals = []

    with open(file_path, 'r') as file:
        reader = csv.reader(file, delimiter=' ')
        for row in reader:
            if len(row) != 3:
                continue  # Пропустить некорректные строки
            x = parse_float(row[0], decimal_separator)
            y = parse_float(row[1], decimal_separator)
            T = parse_float(row[2], decimal_separator)
            x_vals.append(x)
            y_vals.append(y)
            t_vals.append(T)

    return np.array(x_vals), np.array(y_vals), np.array(t_vals)


def plot_temperature_field(x, y, T):
    """Построение поля температуры на плоскости с интерполяцией."""

    # Создаем регулярную сетку для интерполяции
    grid_x, grid_y = np.mgrid[min(x):max(x):100j, min(y):max(y):100j]

    # Интерполяция данных температуры на регулярную сетку
    # method = linear, cubic
    grid_T = griddata((x, y), T, (grid_x, grid_y), method='cubic')

    # Ограничение интерполированных значений температуры в диапазоне от 0 до 500 градусов
    grid_T = np.clip(grid_T, 0, 2)

    # Построение графика
    plt.figure(figsize=(8, 6))
    plt.contourf(grid_x, grid_y, grid_T, levels=100, cmap='coolwarm')
    plt.colorbar(label='Temperature')
    # plt.scatter(x, y, c=T, cmap='coolwarm', edgecolors='black', s=50)  # точки измерений
    plt.xlabel('X Coordinate')
    plt.ylabel('Y Coordinate')
    plt.title('Temperature Field')
    plt.show()

def read_velocity_data(file_path, decimal_separator='.'):
    """Чтение данных из файла и преобразование их в массивы x, y, u_x, u_y."""
    x_vals = []
    y_vals = []
    u_x_vals = []
    u_y_vals = []

    with open(file_path, 'r') as file:
        reader = csv.reader(file, delimiter=' ')
        for row in reader:
            if len(row) != 4:
                continue  # Пропустить некорректные строки
            x = parse_float(row[0], decimal_separator)
            y = parse_float(row[1], decimal_separator)
            u_x = parse_float(row[2], decimal_separator)
            u_y = parse_float(row[3], decimal_separator)
            x_vals.append(x)
            y_vals.append(y)
            u_x_vals.append(u_x)
            u_y_vals.append(u_y)

    return np.array(x_vals), np.array(y_vals), np.array(u_x_vals), np.array(u_y_vals)


def plot_velocity_field(x, y, u_x, u_y):
    """Построение векторного поля скорости с управлением длиной стрелок."""
    plt.figure(figsize=(8, 6))

    scale_factor = 20  # Настройка масштаба длины стрелок
    u_x = u_x * scale_factor
    u_y = u_y * scale_factor

    # Построение векторов скорости с фиксированной шириной и наконечником
    plt.quiver(
        x, y, u_x, u_y,
        angles='xy',
        scale=1,  # Управление масштабом длины стрелок
        width=0.001,  # Толщина тела стрелки
        headwidth=7,  # Ширина наконечника стрелки
        headlength=7,  # Длина наконечника стрелки
        color='blue',
        alpha=0.8,
        pivot='tail'  # Стрелка начинается с заданной точки
    )

    # Настройка графика
    plt.xlabel('R Coordinate')
    plt.ylabel('Z Coordinate')
    plt.title('Velocity Field')
    plt.grid(True)
    plt.axis('equal')  # Одинаковый масштаб по осям
    plt.show()


def find_second_largest(arr):
    if len(arr) < 2:
        return "Массив слишком мал"

    # Инициализируем два значения: первый и второй по величине
    first = second = float('-inf')

    for num in arr:
        if num > first:
            # Если текущий элемент больше первого, обновляем оба значения
            second = first
            first = num
        elif num > second and num != first:
            # Если текущий элемент больше второго, но не равен первому
            second = num

    # Проверяем, найден ли второй по величине элемент
    if second == float('-inf'):
        return "Второй по величине элемент не найден"

    return second

def find_second_smallest(arr):
    if len(arr) < 2:
        return "Массив слишком мал"

    # Инициализируем два значения: первый и второй по величине
    first = second = float('+inf')

    for num in arr:
        if num < first:
            # Если текущий элемент больше первого, обновляем оба значения
            second = first
            first = num
        elif num < second and num != first:
            # Если текущий элемент больше второго, но не равен первому
            second = num

    # Проверяем, найден ли второй по величине элемент
    if second == float('+inf'):
        return "Второй по величине элемент не найден"

    return second
def plot_contour_lines(x, y, T, levels=10, cmap='coolwarm'):
    """
    Отрисовка изолиний для температурного поля.

    Параметры:
    - x, y: массивы координат точек.
    - T: массив значений температуры.
    - levels: количество уровней изолиний (или массив конкретных уровней).
    - cmap: цветовая карта.
    """
    # Создаем регулярную сетку для интерполяции
    grid_x, grid_y = np.mgrid[min(x):max(x):100j, min(y):max(y):100j]

    # Интерполяция данных температуры на регулярную сетку
    grid_T = griddata((x, y), T, (grid_x, grid_y), method='cubic')

    # Ограничение интерполированных значений температуры
    grid_T = np.clip(grid_T, 0, max(T))
    # grid_T = np.clip(grid_T, 0, max(T))  find_second_largest(T) find_second_smallest
    # Построение графика
    plt.figure(figsize=(8, 6))

    # Отрисовка изолиний
    contour = plt.contour(grid_x, grid_y, grid_T, levels=levels, colors='black', linewidths=0.2)

    # Подписи изолиний
    plt.clabel(contour, inline=False, fontsize=7, fmt='%1.1f')

    # Цветовая заливка для контекста
    plt.contourf(grid_x, grid_y, grid_T, levels=200, cmap=cmap, alpha=0.75)

    # Цветовая шкала
    plt.colorbar(label='Temperature')

    # Настройка графика
    plt.xlabel('R')
    plt.ylabel('Z')
    plt.title('Сomputational Field')
    plt.show()

def plot_heat_and_velocity_field(x, y, T, v_x, v_y, v_value_x, v_value_y):
    grid_x, grid_y = np.mgrid[min(x):max(x):100j, min(y):max(y):100j]

    # Интерполяция температуры
    grid_T = griddata((x, y), T, (grid_x, grid_y), method='linear')
    grid_T = np.clip(grid_T, 0, max(T))
    #grid_T = np.clip(grid_T, 0, max(T))  find_second_largest(T) find_second_smallest


    # Построение графика
    plt.figure(figsize=(8, 6))

    # Построение температурного поля
    contour = plt.contourf(grid_x, grid_y, grid_T, levels=50, cmap='coolwarm')

    # Построение векторов скорости с фиксированным цветом
   # plt.quiver(
   #     v_x, v_y, v_value_x, v_value_y,
   #     angles='xy',
   #     scale=0.7,  # Управление масштабом длины стрелок
   #     width=0.002,  # Толщина тела стрелки
   #     headwidth=6,  # Ширина наконечника стрелки
   #     headlength=6,  # Длина наконечника стрелки
   #     color='black',  # Фиксированный цвет стрелок
   #     pivot='tail',  # Стрелка начинается с заданной точки
   # )

    # Добавление шкалы только для температурного поля
    plt.colorbar(contour, label='T')  # Привязываем шкалу только к температуре
    plt.xlabel('R')
    plt.ylabel('Z')
    plt.title('Сomputational Field')
    plt.show()

decimal_separator = ','

file_path = f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime5.txt'
x, y, T = read_data_k(file_path, decimal_separator)

# Отрисовка изолиний
plot_contour_lines(x, y, T, levels=10, cmap='coolwarm')

file_path = f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime25.txt'
x, y, T = read_data_k(file_path, decimal_separator)

# Отрисовка изолиний
plot_contour_lines(x, y, T, levels=10, cmap='coolwarm')

file_path = f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime50.txt'
x, y, T = read_data_k(file_path, decimal_separator)

# Отрисовка изолиний
plot_contour_lines(x, y, T, levels=10, cmap='coolwarm')

file_path = f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/New/PyOutq.txt'
x, y, T = read_data_k(file_path, decimal_separator)

# Отрисовка изолиний
plot_contour_lines(x, y, T, levels=10, cmap='coolwarm')

# plot_temperature_field(x, y, T)

# PyOutq.txt
# PyOutQ1.txt
# PyOutTtime0.txt
# PyOutTtime1.txt
# PyOutTtime2.txt
# PyOutTtime3.txt
# PyOutTtime4.txt

velocity_file_path = 'velocity.txt'
v_point_x, v_point_y, u_x, u_y = read_velocity_data(velocity_file_path, decimal_separator)

# Построение поля скорости
# plot_velocity_field(x, y, u_x, u_y)
#plot_heat_and_velocity_field(x, y, T, v_point_x, v_point_y, u_x, u_y)






# Чтение данных из файла
x = []
y = []

with open(f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutPoint1.txt', 'r') as file:
    for index, line in enumerate(file):
        # Разделяем строку на x и y
        parts = line.strip().split()
        if len(parts) == 2:
            x.append(float(parts[0])*10)  # Используем индекс строки как x
            y.append(float(parts[1])*10)  # Второе значение как y

# Построение графика
plt.plot(x, y, label='Зависимость T от времени', color='blue', marker='o')

# Добавление подписей осей
plt.xlabel('время')
plt.ylabel('T')

# Добавление заголовка
plt.title('График зависимости T от времени')

# Добавление легенды
plt.legend()

# Отображение сетки
plt.grid(True)

# Показ графика
plt.show()

# Чтение данных из файла
x = []
y = []

with open(f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime5Z-2.3205000000000005.txt', 'r') as file:
    for index, line in enumerate(file):
        # Разделяем строку на x и y
        parts = line.strip().split()
        if len(parts) == 2:
            x.append(float(parts[0]))  # Используем индекс строки как x
            y.append(float(parts[1]))  # Второе значение как y

# Построение графика
plt.plot(x, y, label='Зависимость T от R', color='blue', marker='.')

# Добавление подписей осей
plt.xlabel('R')
plt.ylabel('T')

# Добавление заголовка
plt.title('График зависимости T от R')

# Добавление легенды
plt.legend()

# Отображение сетки
plt.grid(True)

# Показ графика
#plt.show()


# Чтение данных из файла
x = []
y = []

with open(f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime5Z-25.5795452242073.txt', 'r') as file:
    for index, line in enumerate(file):
        # Разделяем строку на x и y
        parts = line.strip().split()
        if len(parts) == 2:
            x.append(float(parts[0]))  # Используем индекс строки как x
            y.append(float(parts[1]))  # Второе значение как y

# Построение графика
plt.plot(x, y, label='Зависимость T от R', color='blue', marker='.')

# Добавление подписей осей
plt.xlabel('R')
plt.ylabel('T')

# Добавление заголовка
plt.title('График зависимости T от R')

# Добавление легенды
plt.legend()

# Отображение сетки
plt.grid(True)

# Показ графика
#plt.show()


# Чтение данных из файла
x = []
y = []

with open(f'C:/Users/dimap/source/repos/practice/FEMHeatElectromagnetic/Generated/{path_type}/PyOutTtime5Z-54.59088268863613.txt', 'r') as file:
    for index, line in enumerate(file):
        # Разделяем строку на x и y
        parts = line.strip().split()
        if len(parts) == 2:
            x.append(float(parts[0]))  # Используем индекс строки как x
            y.append(float(parts[1]))  # Второе значение как y

# Построение графика
plt.plot(x, y, label='Зависимость T от R', color='blue', marker='.')

# Добавление подписей осей
plt.xlabel('R')
plt.ylabel('T')

# Добавление заголовка
plt.title('График зависимости T от R')

# Добавление легенды
plt.legend()

# Отображение сетки
plt.grid(True)

# Показ графика
#plt.show()