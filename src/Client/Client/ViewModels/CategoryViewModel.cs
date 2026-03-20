using Client.Models;
using Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Client.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public ObservableCollection<Category> Categories { get; set; } = new();
        CategoryService _categoryService = new CategoryService();
        
        public CategoryViewModel() 
        {
            Categories = new ObservableCollection<Category>(
               _categoryService.GetAllCategories()
           );

            Categories.Insert(0, new Category
            {
                ID = 0,
                Name = "Tất cả"
              
            });

          
        }


    }
}
