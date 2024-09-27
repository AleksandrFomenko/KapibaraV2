using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using FsmModules.FacadeModule.View;
using FsmModules.FacadeModule.ViewModel;
using FsmModules.MineModule.Model.MineModel;
using FsmModules.MineModule.ViewModel;
using FsmModules.Model;
using FsmModules.WallDecoration.View;
using FsmModules.WallDecoration.ViewModel;
using FsmModules.WaterSupply.View;
using FsmModules.WaterSupply.ViewModel;
using Microsoft.EntityFrameworkCore;


namespace FsmModules.ViewModels
{
    public partial class FsmModulesViewModel : ObservableObject
    {
        private Document _doc = Context.Document;

        [ObservableProperty]
        private object _selectedTab;
        
        public MineModuleViewModel MineModuleVM { get; }
        public FacadeModuleViewModel FacadeModuleVm { get; }
        public WallDecorationViewModel WallDecorationVm { get; }
        public WaterSupplyViewModel WaterSupplyVM { get; }

        
        public FsmModulesViewModel()
        {
            MineModuleVM = new MineModuleViewModel(_doc);
            FacadeModuleVm = new FacadeModuleViewModel();
            WallDecorationVm = new WallDecorationViewModel(_doc);
            WaterSupplyVM = new WaterSupplyViewModel();
        }
        
        [RelayCommand]
        private void Execute(Window window)
        {
            window?.Close();
            IWorker worker = null;

            if (_selectedTab is TabItem tabItem)
            {
                if (tabItem.Content is UserControl userControl)
                {
                    var viewModel = userControl.DataContext;

                    if (viewModel is MineModuleViewModel)
                    {
                        worker = MineModuleVM;
                    }
                    else if (viewModel is FacadeModuleViewModel)
                    {
                        worker = FacadeModuleVm;
                    }
                    else if (viewModel is WallDecorationViewModel)
                    {
                        worker = WallDecorationVm;
                    }
                    else if (viewModel is WaterSupplyViewModel)
                    {
                        worker = WaterSupplyVM;
                    }
                }
            }
            worker?.Start();
        }
    }
}



