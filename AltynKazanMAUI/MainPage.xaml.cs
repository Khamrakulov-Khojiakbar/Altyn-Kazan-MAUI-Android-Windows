using AltynKazanMAUI.Models;
using System.Text.Json;

namespace AltynKazanMAUI;

public partial class MainPage : ContentPage
{
    private List<Dish> _allDishes = new List<Dish>();
    private List<string> _categories = new List<string>();

    // Флаг для предотвращения зацикливания событий клика и скролла
    private bool _isScrollingProgrammatically = false;

    public MainPage()
    {
        InitializeComponent();
        LoadMenu();
    }

    private async void LoadMenu()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("menu.json");
            using var reader = new StreamReader(stream);
            var jsonContents = await reader.ReadToEndAsync();

            _allDishes = JsonSerializer.Deserialize<List<Dish>>(jsonContents) ?? new List<Dish>();

            // Получаем только чистые категории из вашей базы данных (без "Всё меню")
            _categories = _allDishes.Select(dish => dish.Category).Distinct().ToList();

            CategoriesCollectionView.ItemsSource = _categories;

            // Отображаем группы
            ShowAllGroupedMenu();

            // По умолчанию выделяем самую первую категорию из списка
            if (_categories.Any())
            {
                _isScrollingProgrammatically = true;
                CategoriesCollectionView.SelectedItem = _categories.FirstOrDefault();
                _isScrollingProgrammatically = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось загрузить данные меню: {ex.Message}", "OK");
        }
    }

    private void ShowAllGroupedMenu()
    {
        var groupedData = _allDishes
            .GroupBy(dish => dish.Category)
            .Select(group => new DishGroup(group.Key, group.ToList()))
            .ToList();

        BindableLayout.SetItemsSource(MenuGroupsContainer, groupedData);
    }

    // ЛОГИКА 1: Клик на категорию слева -> Плавный переход к группе блюд
    private async void OnCategoryChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isScrollingProgrammatically) return;

        if (e.CurrentSelection.FirstOrDefault() is string selectedCategory)
        {
            _isScrollingProgrammatically = true;

            var children = MenuGroupsContainer.Children;
            foreach (var child in children)
            {
                if (child is VisualElement element && element.BindingContext is DishGroup group && group.CategoryName == selectedCategory)
                {
                    await MenuScrollView.ScrollToAsync(element, ScrollToPosition.Start, true);
                    break;
                }
            }

            _isScrollingProgrammatically = false;
        }
    }

    // ЛОГИКА 2: Scroll-Spy (Подсветка категории при ручном пролистывании)
    private void OnMenuScrolled(object sender, ScrolledEventArgs e)
    {
        if (_isScrollingProgrammatically || !_categories.Any()) return;

        // По умолчанию активной считается самая первая категория списка
        string activeCategory = _categories.First();
        double currentScrollY = e.ScrollY;

        var children = MenuGroupsContainer.Children;
        foreach (var child in children)
        {
            if (child is VisualElement element && element.BindingContext is DishGroup group)
            {
                // Если проскроллили ниже верхней границы секции (с небольшим отступом в 40px для точности)
                if (currentScrollY >= element.Y - 40)
                {
                    activeCategory = group.CategoryName;
                }
            }
        }

        // Если при скролле мы перешли на новую категорию, обновляем левую панель
        if (CategoriesCollectionView.SelectedItem as string != activeCategory)
        {
            _isScrollingProgrammatically = true;
            CategoriesCollectionView.SelectedItem = activeCategory;
            _isScrollingProgrammatically = false;
        }
    }
}