using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


using MultiCriteriaSort.Helpers;
using MultiCriteriaSort.Models;
using System.Collections.ObjectModel;

namespace MultiCriteriaSort
{
    public sealed partial class MainWindow : Window
    {
        private List<Employee> _originalList;
        private List<SortCriteria> _currentSortCriteria = new List<SortCriteria>();
        public ObservableCollection<Employee> DisplayList { get; set; } = new ObservableCollection<Employee>();

        public MainWindow()
        {
            this.InitializeComponent();

            _originalList = new List<Employee>
            {
                new Employee { Name = "Alice", Age = 30, Salary = 2000 },
                new Employee { Name = "Bob", Age = 25, Salary = 3000 },
                new Employee { Name = "Charlie", Age = 25, Salary = 1500 },
                new Employee { Name = "David", Age = 35, Salary = 2000 },
                new Employee { Name = "Eve", Age = 30, Salary = 2500 },
                new Employee { Name = "Frank", Age = 25, Salary = 3000 }
            };

            RefreshDisplay(_originalList);
            EmployeeListView.ItemsSource = DisplayList;
        }

        private void RefreshDisplay(IEnumerable<Employee> data)
        {
            DisplayList.Clear();
            foreach (var item in data)
            {
                DisplayList.Add(item);
            }
        }

        private void ApplySorting()
        {
            if (_currentSortCriteria.Count == 0)
            {
                RefreshDisplay(_originalList);
                SortStatusBlock.Text = "No active sort.";
                return;
            }

            var sortedResult = _originalList.MultipleSort(_currentSortCriteria).ToList();
            RefreshDisplay(sortedResult);

            var statusStrings = _currentSortCriteria.Select(c => $"{c.PropertyName} ({c.Direction})");
            SortStatusBlock.Text = "Sorted by: " + string.Join(" then by ", statusStrings);
        }

        private void ToggleSort(string propertyName)
        {
            var existingSort = _currentSortCriteria.FirstOrDefault(c => c.PropertyName == propertyName);

            if (existingSort != null)
            {
                if (existingSort.Direction == SortDirection.Ascending)
                {
                    existingSort.Direction = SortDirection.Descending;
                }
                else
                {
                    _currentSortCriteria.Remove(existingSort);
                }
            }
            else
            {
                _currentSortCriteria.Add(new SortCriteria(propertyName, SortDirection.Ascending));
            }

            ApplySorting();
        }

        private void Sort_Name_Click(object sender, RoutedEventArgs e) => ToggleSort(nameof(Employee.Name));
        private void Sort_Age_Click(object sender, RoutedEventArgs e) => ToggleSort(nameof(Employee.Age));
        private void Sort_Salary_Click(object sender, RoutedEventArgs e) => ToggleSort(nameof(Employee.Salary));
        private void Clear_Sort_Click(object sender, RoutedEventArgs e)
        {
            _currentSortCriteria.Clear();
            ApplySorting();
        }
    }
}
