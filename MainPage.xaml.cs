using OOP_Lab2.Models;
using OOP_Lab2.Strategies;
using System.Xml.Xsl;

namespace OOP_Lab2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Кнопка "Пошук"
        private async void OnSearchClicked(object sender, EventArgs e)
        {
            // 1. Перевіряємо шлях до файлу
            string filePath = GetFilePath("Schedule.xml");

            if (!File.Exists(filePath))
            {
                // ЦЕ ПОВІДОМЛЕННЯ ПОКАЖЕ, ДЕ ПРОГРАМА ШУКАЄ ФАЙЛ
                await DisplayAlertAsync("Помилка файлу", $"Файл не знайдено за адресою:\n{filePath}\n\nПеревірте властивість 'Copy to Output Directory'!", "OK");
                return;
            }

            // 2. Перевіряємо стратегію
            if (StrategyPicker.SelectedIndex == -1)
            {
                await DisplayAlertAsync("Помилка", "Будь ласка, оберіть метод аналізу (Strategy)!", "OK");
                return;
            }
            // Створення об'єкту критеріїв (ОБОВ'ЯЗКОВО ОБРОБКА NULL)
            SearchResult criteria = new SearchResult
            {
                Faculty = FacultyEntry.Text ?? "",
                Department = DeptEntry.Text ?? "",
                TeacherName = TeacherEntry.Text ?? "",
                Subject = SubjectEntry.Text ?? "",

                // Додаємо нові критерії
                Groups = GroupEntry.Text ?? "",
                Room = RoomEntry.Text ?? ""
            };

            ISearchStrategy strategy = null;
            string strategyName = StrategyPicker.SelectedItem.ToString();

            if (strategyName == "LINQ to XML") strategy = new LinqToXmlStrategy();
            else if (strategyName == "DOM API") strategy = new DomStrategy();
            else if (strategyName == "SAX API") strategy = new SaxStrategy();

            // Очистка перед пошуком
            ResultsCollection.ItemsSource = null;

            try
            {
                var results = strategy.Search(criteria, filePath);

                if (results.Count > 0)
                {
                    ResultsCollection.ItemsSource = results;
                }
                else
                {
                    await DisplayAlertAsync("Результат", "Пошук виконано, але нічого не знайдено. Перевірте критерії або структуру XML.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Критична помилка", ex.Message, "OK");
            }
        }

        // Кнопка "Очистити"
        private void OnClearClicked(object sender, EventArgs e)
        {
            FacultyEntry.Text = string.Empty;
            DeptEntry.Text = string.Empty;
            TeacherEntry.Text = string.Empty;
            SubjectEntry.Text = string.Empty;
            GroupEntry.Text = string.Empty; // +
            RoomEntry.Text = string.Empty;  // +

            StrategyPicker.SelectedIndex = -1;
            ResultsCollection.ItemsSource = null;
        }

        // Кнопка "Вихід" (З підтвердженням, як у завданні)
        private async void OnExitClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlertAsync("Підтвердження", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
            if (answer)
            {
                Application.Current.Quit();
            }
        }

        // Кнопка "Конвертувати в HTML"
        private async void OnTransformClicked(object sender, EventArgs e)
        {
            string xmlPath = GetFilePath("Schedule.xml");
            string xslPath = GetFilePath("Schedule.xsl"); // Цей файл ми зараз створимо!

            // Шлях, куди збережемо HTML (у папку кешу програми)
            string htmlPath = Path.Combine(FileSystem.CacheDirectory, "Schedule.html");

            try
            {
                // Трансформація
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(xslPath);
                transform.Transform(xmlPath, htmlPath);

                await DisplayAlertAsync("Успіх", $"Файл збережено: {htmlPath}", "OK");

                // Спробуємо відкрити його у браузері
                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    Title = "Відкрити розклад",
                    File = new ReadOnlyFile(htmlPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Помилка трансформації", ex.Message, "OK");
            }
        }

        // Допоміжний метод для отримання шляху до файлу
        private string GetFilePath(string fileName)
        {
            // Для Windows/Desktop програм файл лежить в папці збірки
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }
    }
}