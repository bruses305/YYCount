using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Services;

namespace YYCount.ViewModels
{
    public class EquipmentEditorViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        public ObservableCollection<EquipmentItem> Items { get; }

        private EquipmentItem _selectedItem;
        public EquipmentItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Set(ref _selectedItem, value))
                {
                    if (value != null)
                    {
                        NewName = value.Name;
                        NewUnitValue = value.UnitValue;
                    }
                    else
                    {
                        NewName = "";
                        NewUnitValue = 0;
                    }
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _newName;
        public string NewName
        {
            get => _newName;
            set => Set(ref _newName, value);
        }

        private double _newUnitValue;
        public double NewUnitValue
        {
            get => _newUnitValue;
            set => Set(ref _newUnitValue, value);
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public EquipmentEditorViewModel(ObservableCollection<EquipmentItem> currentItems, IDataService dataService)
        {
            _dataService = dataService;
            // Создаём отсортированную копию
            var sorted = currentItems.OrderBy(i => i.Name).ToList();
            Items = new ObservableCollection<EquipmentItem>(sorted);

            AddCommand = new RelayCommand(AddItem, CanAdd);
            EditCommand = new RelayCommand(EditItem, CanEdit);
            DeleteCommand = new RelayCommand(DeleteItem, CanDelete);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(NewName) && NewUnitValue > 0;
        private bool CanEdit() => SelectedItem != null && !string.IsNullOrWhiteSpace(NewName) && NewUnitValue > 0;
        private bool CanDelete() => SelectedItem != null;

        private void AddItem()
        {
            var newItem = new EquipmentItem
            {
                Id = Items.Any() ? Items.Max(i => i.Id) + 1 : 1,
                Name = NewName.Trim(),
                UnitValue = NewUnitValue
            };
            // Вставка по алфавиту
            int index = 0;
            while (index < Items.Count && 
                   string.Compare(Items[index].Name, newItem.Name, System.StringComparison.CurrentCultureIgnoreCase) < 0)
                index++;
            Items.Insert(index, newItem);
            NewName = "";
            NewUnitValue = 0;
            SelectedItem = null;
        }

        private void EditItem()
        {
            if (SelectedItem == null || string.IsNullOrWhiteSpace(NewName)) return;

            var itemToEdit = SelectedItem;
            var oldIndex = Items.IndexOf(itemToEdit);
            if (oldIndex < 0) return; // элемент не найден – защита

            // Обновляем данные
            itemToEdit.Name = NewName.Trim();
            itemToEdit.UnitValue = NewUnitValue;

            // Удаляем со старого места
            Items.RemoveAt(oldIndex);

            // Вставляем на новое место по алфавиту
            int newIndex = 0;
            while (newIndex < Items.Count && 
                   string.Compare(Items[newIndex].Name, itemToEdit.Name, System.StringComparison.CurrentCultureIgnoreCase) < 0)
                newIndex++;
            Items.Insert(newIndex, itemToEdit);

            NewName = "";
            NewUnitValue = 0;
            SelectedItem = null;
        }

        private void DeleteItem()
        {
            if (SelectedItem == null) return;
            if (!Items.Contains(SelectedItem)) return; // защита
            Items.Remove(SelectedItem);
            SelectedItem = null;
        }

        public void Save()
        {
            _dataService.SaveEquipment(Items.ToList());
        }

        public void Cancel()
        {
            // Закрыть без сохранения
        }
    }
}