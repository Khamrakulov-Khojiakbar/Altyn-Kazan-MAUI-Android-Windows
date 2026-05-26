using AltynKazanMAUI.Models;
using System.Text.Json;

namespace AltynKazanMAUI
{
    public partial class MainPage : ContentPage
    {
        private List<Dish> _allDishes = new List<Dish>();

        public MainPage()
        {
            InitializeComponent();
            LoadMenu();
        }


        private async void LoadMenu()
        {
            try
            {
                // 1. Читаем JSON
                using var stream = await FileSystem.OpenAppPackageFileAsync("menu.json");
                using var reader = new StreamReader(stream);
                var jsonContents = await reader.ReadToEndAsync();

                // 2. Десериализуем полный список блюд
                _allDishes = JsonSerializer.Deserialize<List<Dish>>(jsonContents) ?? new List<Dish>();

                // 3. Вытаскиваем только уникальные категории для левого меню
                // Ключевое слово Distinct() убирает дубликаты (чтобы не было три раза "Супы")
                var categories = _allDishes.Select(dish => dish.Category).Distinct().ToList();

                // Добавим пункт "Всё меню", если гость захочет посмотреть всё сразу
                categories.Insert(0, "Всё меню");

                // 4. Отдаем категории в левую панель
                CategoriesCollectionView.ItemsSource = categories;

                // По умолчанию показываем вообще все блюда
                MenuCollectionView.ItemsSource = _allDishes;

                // Автоматически выбираем первый пункт "Всё меню" в списке
                CategoriesCollectionView.SelectedItem = categories.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
            }
        }

        private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
        {
            // Узнаем, какую категорию нажал пользователь
            var selectedCategory = e.CurrentSelection.FirstOrDefault() as string;

            if (string.IsNullOrEmpty(selectedCategory) || selectedCategory == "Всё меню")
            {
                // Если выбрали "Всё меню", показываем весь список
                MenuCollectionView.ItemsSource = _allDishes;
            }
            else
            {
                // Используем LINQ фильтр: выбираем только те блюда, у которых категория совпадает с нажатой
                var filteredDishes = _allDishes.Where(dish => dish.Category == selectedCategory).ToList();

                // Обновляем правую панель с блюдами
                MenuCollectionView.ItemsSource = filteredDishes;
            }
        }
    }
}
