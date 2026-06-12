using AltynKazanMAUI.Models;
using System.Text.Json;

namespace AltynKazanMAUI;

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
            // Читаем локальный файл JSON из ресурсов сборки
            using var stream = await FileSystem.OpenAppPackageFileAsync("menu.json");
            using var reader = new StreamReader(stream);
            var jsonContents = await reader.ReadToEndAsync();

            // Переводим JSON в объекты C#
            _allDishes = JsonSerializer.Deserialize<List<Dish>>(jsonContents) ?? new List<Dish>();

            // Вытягиваем уникальные названия категорий для левого меню
            var categories = _allDishes.Select(dish => dish.Category).Distinct().ToList();
            categories.Insert(0, "Всё меню");

            // Наполняем левую панель
            CategoriesCollectionView.ItemsSource = categories;

            // Отображаем сразу сгруппированное "Всё меню"
            ShowGroupedMenu(null);

            // Задаем визуальный выбор на первый элемент списка
            CategoriesCollectionView.SelectedItem = categories.FirstOrDefault();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить данные меню: {ex.Message}", "OK");
        }
    }

    private void ShowGroupedMenu(string selectedCategory)
    {
        IEnumerable<Dish> dishesToGroup = _allDishes;

        // Если выбрана конкретная категория, сначала фильтруем список
        if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Всё меню")
        {
            dishesToGroup = _allDishes.Where(d => d.Category == selectedCategory);
        }

        // Группируем с помощью LINQ и превращаем в структуру DishGroup
        var groupedData = dishesToGroup
            .GroupBy(dish => dish.Category)
            .Select(group => new DishGroup(group.Key, group.ToList()))
            .ToList();

        // Отправляем сгруппированные данные в CollectionView
        MenuCollectionView.ItemsSource = groupedData;
    }

    private void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selectedCategory)
        {
            ShowGroupedMenu(selectedCategory);
        }
    }
}