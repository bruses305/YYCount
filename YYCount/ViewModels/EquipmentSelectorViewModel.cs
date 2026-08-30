using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Services;

namespace YYCount.ViewModels
{
    public class EquipmentSelectorViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private ObservableCollection<EquipmentItem> _allItems;

        // Свойства с сеттерами для всех команд
        public ICommand SelectCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SetSortCommand { get; set; }

        public ListCollectionView FilteredItems { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (Set(ref _searchText, value))
                    FilteredItems?.Refresh();
            }
        }

        private SortOption _sortBy;
        public SortOption SortBy
        {
            get => _sortBy;
            set
            {
                if (Set(ref _sortBy, value))
                    ApplySort();
            }
        }

        private EquipmentItem _selectedItem;
        public EquipmentItem SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public event EventHandler<EquipmentItem> ItemSelected;

        public EquipmentSelectorViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _allItems = new ObservableCollection<EquipmentItem>(_dataService.LoadEquipment());

            FilteredItems = new ListCollectionView(_allItems);
            FilteredItems.Filter = FilterPredicate;

            // Инициализация команд (будут переопределены в диалоге, но оставляем дефолтные)
            SelectCommand = new RelayCommand<EquipmentItem>(Select, CanSelect);
            CancelCommand = new RelayCommand(Cancel);
            SetSortCommand = new RelayCommand<string>(SetSort);

            SortBy = SortOption.NameAsc; // сортировка по умолчанию
        }

        private bool FilterPredicate(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            var item = obj as EquipmentItem;
            return item != null && item.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void ApplySort()
        {
            if (FilteredItems == null) return;
            FilteredItems.SortDescriptions.Clear();
            switch (SortBy)
            {
                case SortOption.NameAsc:
                    FilteredItems.SortDescriptions.Add(new SortDescription(nameof(EquipmentItem.Name), ListSortDirection.Ascending));
                    break;
                case SortOption.NameDesc:
                    FilteredItems.SortDescriptions.Add(new SortDescription(nameof(EquipmentItem.Name), ListSortDirection.Descending));
                    break;
                case SortOption.UnitAsc:
                    FilteredItems.SortDescriptions.Add(new SortDescription(nameof(EquipmentItem.UnitValue), ListSortDirection.Ascending));
                    break;
                case SortOption.UnitDesc:
                    FilteredItems.SortDescriptions.Add(new SortDescription(nameof(EquipmentItem.UnitValue), ListSortDirection.Descending));
                    break;
            }
        }

        private bool CanSelect(EquipmentItem item) => item != null;

        private void Select(EquipmentItem item)
        {
            if (item == null) return;
            ItemSelected?.Invoke(this, item);
        }

        private void Cancel()
        {
            // Закрытие без выбора
        }

        private void SetSort(string param)
        {
            if (Enum.TryParse<SortOption>(param, out var sort))
                SortBy = sort;
        }
    }

    public enum SortOption
    {
        NameAsc,
        NameDesc,
        UnitAsc,
        UnitDesc
    }
}