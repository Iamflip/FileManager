using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace XFileManager
{
    class Program
    {
        static List<string> filesAndDirectories = new List<string>();
        static List<string> commands = new List<string>();
        static int currentCommand = 0;

        static void Main(string[] args)
        {
            //Проверка наличия файла конфигурации
            if (File.Exists("prop.json"))
            {
                Properties properties = Initialize();

                Console.SetWindowSize(properties.Width, properties.Height);
                Console.SetBufferSize(properties.Width, properties.Height);

                if (properties.LastPath != null)
                {
                    if (Directory.Exists(properties.LastPath))
                    {
                        MakeTree(properties.LastPath);
                        Menu(properties.LastPath);
                    }
                    else
                    {
                        Menu(properties.LastPath);
                    }
                }
                else
                {
                    Menu(properties.LastPath);
                }
            }
            else
            {
                Properties properties = new Properties();
                File.WriteAllText("prop.json", JsonSerializer.Serialize(properties));

                Console.SetWindowSize(properties.Width, properties.Height);
                Console.SetBufferSize(properties.Width, properties.Height);

                Menu(properties.LastPath);
            }
        }
        /// <summary>
        /// Manage Methods
        /// </summary>

        public static void Menu(string path)
        {
            Console.Clear();
            Draw();

            int numOfPage = 1;
            bool isVisible = false; //Переменная "помошник" для корректной работы отображения ранее написанных команд
            Properties properties = Initialize();

            bool isNull = Directory.GetCurrentDirectory() == null ? true : false; //Проверка на null текущей дериктории


            int amountOfPage = 0;

            if (!isNull)//Если директория null то пропускает метод подсчёта страниц и отрисовку  
            {
                amountOfPage = CalculateAmountOfPage(properties);
                DrawTree(numOfPage, amountOfPage, path);
                InfoDirectory(properties, path);
            }

            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
            Console.Write("Нажмите Enter для ввода команд");

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if (!isNull)//Проверка на null для запрета смены страниц если никакая директория не открыта
                        {
                            if (numOfPage < amountOfPage)
                            {
                                numOfPage++;
                                DrawTree(numOfPage, amountOfPage, path);
                            }
                            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                            Console.Write("Нажмите Enter для ввода команд");
                        }
                    }
                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if (!isNull)//Проверка на null для запрета смены страниц если никакая директория не открыта
                        {
                            if (numOfPage > 1)
                            {
                                numOfPage--;
                                DrawTree(numOfPage, amountOfPage, path);
                            }
                            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                            Console.Write("Нажмите Enter для ввода команд");
                        }
                    }
                    if (key.Key == ConsoleKey.Enter)
                    {
                        ClearMessageBox();
                        Console.CursorVisible = true;
                        Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                        string command = Console.ReadLine();
                        ReadCommand(command);
                    }
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        PreviosCommand(ref isVisible);
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        NextCommand();
                    }
                }
            }
        }//Основной метод управляющий программой
        public static void ReadCommand(string command)
        {
            commands.Add(command);
            currentCommand = commands.Count;

            Properties properties = Initialize();

            string[] splitter = new string[] { " C:", " D:", " lvl" };
            string[] newCommand = command.Split(splitter, StringSplitOptions.None);//Разделение команды для определения запроса

            if (command == "exit")
            {
                Environment.Exit(0);
            }

            string CorD;//Из за сплита с разделением через C: и D: необходимо восстановить название диска

            if (command.Length < 3)//На случай слишком короткого запроса прекращает выполнение метода
            {
                ClearMessageBox();
                Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                Console.Write("Неверно введены данные, нажмите Enter для повторного ввода");
                return;
            }

            CorD = command[3] + ":";
            newCommand[1] = CorD + newCommand[1];

            switch (newCommand[0])
            {
                case "ls"://Команда которая открывает дерево каталогов по заданному пути
                    if (newCommand.Length == 3)//В случае указывания глубины поиска проверяет валидность входящих данных
                    {
                        try
                        {
                            int maxLevel = Convert.ToInt32(newCommand[2]);
                            if (maxLevel > -1 && maxLevel < 6)
                            {
                                CheckDirectoryAndFiles(newCommand[1], maxLevel);
                            }
                        }
                        catch
                        {
                            ClearMessageBox();
                            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                            Console.Write("Неверно введены данные, нажмите Enter для повторного ввода");
                        }
                    }
                    else
                    {
                        CheckDirectoryAndFiles(newCommand[1]);
                    }
                    break;
                case "cp"://Команда копирования файлов и директорий
                    if (newCommand.Length == 3)
                    {

                        CorD = command[command.Length - (newCommand[2].Length + 2)] + ":";

                        newCommand[2] = CorD + newCommand[2];

                        if (Directory.Exists(newCommand[1]))
                        {
                            CopyDirectory(newCommand[1], newCommand[2]);
                        }
                        else if (File.Exists(newCommand[1]))
                        {
                            CopyFiles(newCommand[1], newCommand[2]);
                        }
                        else
                        {
                            ClearMessageBox();
                            Console.Write("Такого файла не существует, нажмите Enter для повторного ввода команд");
                        }
                    }
                    else
                    {
                        ClearMessageBox();
                        Console.Write("Введены неверные данные, нажмите Enter для повторного ввода команд");
                    }
                    break;
                case "rm"://Команда удаления файлов и директорий
                    if (Directory.Exists(newCommand[1]))
                    {
                        RemoveDirectory(newCommand[1]);
                    }
                    else if (File.Exists(newCommand[1]))
                    {
                        RemoveFiles(newCommand[1]);
                    }
                    else
                    {
                        ClearMessageBox();
                        Console.Write("Введены неверные данные, нажмите Enter для повторного ввода команд");
                    }
                    break;
                case "file"://Команда для демонстрации веса и атрибута файлов и директорий

                    CorD = command[5] + ":";
                    newCommand[1] = CorD + newCommand[1].Remove(0, 2);

                    if (Directory.Exists(newCommand[1]))
                    {
                        InfoDirectory(properties, newCommand[1]);
                    }
                    else if (File.Exists(newCommand[1]))
                    {
                        InfoFiles(properties, newCommand[1]);
                    }
                    else
                    {
                        ClearMessageBox();
                        Console.Write("Введены неверные данные, нажмите Enter для повторного ввода команд");
                    }
                    break;
                default:
                    ClearMessageBox();
                    Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
                    Console.Write("Введенны неверные данные, нажмите Enter для повторного ввода команд");
                    break;
            }
        }

        /// <summary>
        /// Drawing Methods
        /// </summary>

        public static void MakeTree(string path, int maxLevel = 2, int level = 0, string defis = "")
        {
            if (level > maxLevel)
            {
                return;
            }
            string[] dir = Directory.GetDirectories(path);
            string[] fil = Directory.GetFiles(path);

            for (int i = 0; i < dir.Length; i++)
            {

                filesAndDirectories.Add(defis + dir[i].Remove(0, path.Length + 1));
                MakeTree(dir[i], maxLevel, level + 1, defis + " - ");
            }
            for (int j = 0; j < fil.Length; j++)
            {
                filesAndDirectories.Add(defis + fil[j].Remove(0, path.Length + 1));
            }
        }//Метод сбора файлов и директорий в лист
        public static void Draw()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            HorizontalLine TopLine = new HorizontalLine(0, 198, 0, '*');
            HorizontalLine PathLine = new HorizontalLine(0, 198, 2, '*');
            HorizontalLine InfoLine1 = new HorizontalLine(0, 198, 45, '*');
            HorizontalLine InfoLine2 = new HorizontalLine(0, 198, 57, '*');
            HorizontalLine BotLine = new HorizontalLine(0, 198, 59, '*');
            VerticalLine LeftLine = new VerticalLine(0, 59, 0, '*');
            VerticalLine RightLine = new VerticalLine(0, 60, 198, '*');
            Console.ForegroundColor = ConsoleColor.Cyan;
        }//Отрисовка Линий
        private static void DrawTree(int numOfPage, int amountOfPage, string path)
        {
            Properties properties = Initialize();
            

            Console.Clear();
            Draw();

            Console.SetCursorPosition(1, 1);
            Console.Write(path);
            Console.SetCursorPosition(90, 45);
            Console.Write($"Страница {numOfPage} из {amountOfPage}");

            int check; //Переменная для подсчёта элементов для правильного вывода постранично
            int numberOfString = 1;

            if (numOfPage == 1)
            {
                check = 1;
            }
            else
            {
                check = properties.AmountInOnePage * (numOfPage - 1);
            }

            for (int i = 0; i < properties.AmountInOnePage && check <= filesAndDirectories.Count; i++)
            {
                Console.SetCursorPosition(1, numberOfString + 2);
                Console.Write(filesAndDirectories[check - 1]);
                numberOfString++;
                check++;
            }
            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
            Console.CursorVisible = false;
            Console.Write("Нажмите Enter для ввода команд");
        }//Метод по выводу на консоль древа каталогов
        public static void CheckDirectoryAndFiles(string path, int maxLevel = 2)//Метод по команде ls
        {
            if (Directory.Exists(path))
            {
                RefreshFilesAndDirectories();//Обнуление элементов листа файлов и директорий

                SerializeLastPath(path);//Сериализация последнего пути

                MakeTree(path, maxLevel);//Добавление в лист новое дерево каталогов
                Menu(path);//Вызов меню
            }
            else
            {
                ClearMessageBox();
                Console.Write("Такой директории не существует, нажмите Enter для повторного ввода команд");
            }
        }

        /// <summary>
        /// Directory Methods
        /// </summary>
        
        public static void CopyDirectory(string source, string dest)
        {
            if (Directory.Exists(dest))
            {
                CopyDir(source, dest);
                ClearMessageBox();
                Console.Write("Копирование успешно завершено, нажмите Enter для повторного ввода команд");
            }
            else
            {
                ClearMessageBox();
                Console.Write("Указан неверный путь копирования, нажмите Enter для повторного ввода команд");
            }
        }//Метод по копированию директорий
        private static void CopyDir(string source, string dest)
        {
            if (String.IsNullOrEmpty(source) || String.IsNullOrEmpty(dest))
            {
                return;
            }
            Directory.CreateDirectory(dest);
            foreach (string fn in Directory.GetFiles(source))
            {
                File.Copy(fn, Path.Combine(dest, Path.GetFileName(fn)), true);
            }
            foreach (string dir_fn in Directory.GetDirectories(source))
            {
                CopyDir(dir_fn, Path.Combine(dest, Path.GetFileName(dir_fn)));
            }
        }//Приватный метод копирования директорий куда помещена логика
        public static void RemoveDirectory(string path)
        {
            Directory.Delete(path, true);
            string parent = Directory.GetParent(path).ToString();

            RefreshFilesAndDirectories();

            MakeTree(parent);
            Menu(parent);
        }//Удаление директории с последущим выходом в родительскую папку, выполнено рекурсивно
        public static void InfoDirectory(Properties properties, string path)
        {
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition);
            Console.Write(path);
            long size = CalculateWeightDirectory(path);
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition + 1);
            Console.Write(size + " BYTE");
            SystemAtributeDirectory(path);
            ClearMessageBox();
            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
            Console.Write("Нажмите Enter для ввода команд");
        }//Метод вывода доп информации по директории
        public static long CalculateWeightDirectory(string path)
        {
            long size = 0;

            DirectoryInfo dir = new DirectoryInfo(path);

            FileInfo[] fil = dir.GetFiles();
            foreach (FileInfo f in fil)
            {
                size += f.Length;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (var d in dirs)
            {
                size += CalculateWeightDirectory(d.ToString());
            }

            return size;
        }//Подсчёт веса директории
        public static void SystemAtributeDirectory(string path)
        {
            Properties properties = Initialize();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            var a = directoryInfo.Attributes;
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition + 2);
            Console.Write(a.ToString());
        }//Получение атрибутов директории

        /// <summary>
        /// Files Methods
        /// </summary>
        
        public static void CopyFiles(string source, string dest)
        {
            string[] splitHelper = dest.Split('\\');//Разделяет название файла и путь к ней для проверки на наличие папки
            if (Directory.Exists(dest.Remove(dest.Length - splitHelper[splitHelper.Length - 1].Length, splitHelper[splitHelper.Length - 1].Length)))
            {
                File.Copy(source, dest, true);
                ClearMessageBox();
                Console.Write("Копирование успешно завершено, нажмите Enter для повторного ввода команд");
            }
            else
            {
                ClearMessageBox();
                Console.Write("Указан неверный путь копирования, нажмите Enter для повторного ввода команд");
            }
        }//Метод по копированию файлов
        public static void RemoveFiles(string path)
        {
            File.Delete(path);
            ClearMessageBox();

            RefreshFilesAndDirectories();

            string newPath = path.Remove(path.Length - Path.GetFileName(path).Length - 1, Path.GetFileName(path).Length + 1);

            SerializeLastPath(newPath);
            MakeTree(newPath);
            Menu(newPath);
        }//Удаление файла с возвращением в папку откуда был удалён файл
        public static void InfoFiles(Properties properties, string path)
        {
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition);
            Console.Write(path);
            CalculateWeightFile(path);
            SystemAtributeFile(path);
            ClearMessageBox();
            Console.SetCursorPosition(properties.xPlace, properties.WritePlace);
            Console.Write("Нажмите Enter для ввода команд");
        }//Метод вывода доп информации по файлу
        public static void CalculateWeightFile(string path)
        {
            Properties properties = Initialize();

            FileInfo file = new FileInfo(path);
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition + 1);
            Console.Write(file.Length + " BYTES");
        }//Подсчёт веса файла
        public static void SystemAtributeFile(string path)
        {
            Properties properties = Initialize();

            FileAttributes fileAttributes = File.GetAttributes(path);
            Console.SetCursorPosition(properties.xPlace, properties.InfoPlaceStartPosition + 2);
            Console.Write(fileAttributes.ToString());
        }//Получение атрибутов файла

        /// <summary>
        /// Secondary Methods
        /// </summary>
        
        public static Properties Initialize()
        {
            string prop = File.ReadAllText("prop.json");
            Properties properties = JsonSerializer.Deserialize<Properties>(prop);
            return properties;
        }//Десереализация
        public static void SerializeLastPath(string lastPath)
        {
            Properties properties = new Properties(lastPath);
            File.WriteAllText("prop.json", JsonSerializer.Serialize(properties));
        }//Сериализация последнего пути
        public static void RefreshFilesAndDirectories()
        {
            if (filesAndDirectories.Count != 0)
            {
                filesAndDirectories.RemoveRange(0, filesAndDirectories.Count);
            }
        }//Обнуление списка файлов и директорий
        public static void ClearMessageBox()
        {
            Properties properties = Initialize();

            for (int i = 1; i < 200; i++)
            {
                Console.SetCursorPosition(i, properties.WritePlace);
                Console.Write(" ");
            }
            Console.SetCursorPosition(1, 58);
        }//Очистка поля для команд
        public static void PreviosCommand(ref bool isVisible)
        {
            if (commands.Count > 1 && currentCommand > 1)
            {
                if (!isVisible)//Добавлена переменная IsVisible для корректного отображения при первом нажатии
                {
                    ClearMessageBox();
                    Console.Write(commands[currentCommand - 1]);
                    isVisible = true;
                }
                else
                {
                    currentCommand--;

                    ClearMessageBox();
                    Console.Write(commands[currentCommand - 1]);
                }
            }
        }//Метод по выводу на консоль предыдущей команды
        public static void NextCommand()
        {
            if (commands.Count > 1 && currentCommand < commands.Count)
            {
                currentCommand++;

                ClearMessageBox();
                Console.Write(commands[currentCommand - 1]);
            }
        }//Метод по выводу на консоль следующей команды
        public static int CalculateAmountOfPage(Properties properties)
        {
            int amountOfPage;

            if (filesAndDirectories.Count < properties.AmountInOnePage || filesAndDirectories.Count == properties.AmountInOnePage)
            {
                amountOfPage = 1;
            }
            else if (filesAndDirectories.Count % properties.AmountInOnePage == 0)
            {
                amountOfPage = filesAndDirectories.Count / properties.AmountInOnePage;
            }
            else
            {
                amountOfPage = filesAndDirectories.Count / properties.AmountInOnePage + 1;
            }
            return amountOfPage;
        }//Подсчёт количества страниц
    }
}
