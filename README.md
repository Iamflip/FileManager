# FileManager
Консольный файловый менеджер  с небольшим набором функций и работы с файлами и директориями.
Поддерживает просмотр файловой структуры (В глубину до 5 уровней) сделанная пейджингом
(42 элемента на странице). 
Имеет конфигурационный файл с настройками выполенный через JSON.
При выходе конфигурационный файл сохраняет последнюю просмотренную директорию и при повторном 
входе запускает её вновь.
Имеет такие консольные команды как:
1.Копирование директории
2.Копирование файлов
3.Удаление директорий рекурсивно
4.Удаление файлов
5.Вывод информации о директориях
6.Вывод информации о файлах
7.Переход в новую папку (с указанием глубины входа)

Команды: 
1.ls <Полный путь к директории>
2.cp <Полный путь к директории/файлу> <Полный путь указания куда копировать>
3.rm <Полный путь к директории/файлу>
4.file <Полный путь к директории/файлу>

Примечания:
1.При команде копирования файла, после папки нужно прописывать имя файла, если не хотите менять
то пропишите такое же название файла, при желании можно сделать копирование с переименовыванием,
для этого после указания пути копирования допишите новое название и тип файла
Пример: cp C:\source.txt D:\target.txt
2.При использовании команды ls можно указать глубину входа в директории.
По умолчанию стоит глубина входа два, если хотите указать другую глубину входа
(Допустимые нормы от 0 до 5 включительно) то после указания директории 
через пробел пропишите lvl<число> (Между lvl и числом пробел не нужен).
3.Стрелочки влево и вправо позволяют переключаться между страницами.
4.Стрелочки вверх и вниз позволяют смотреть последние ипользованные команды.