using System.Windows.Media;
using ChatGPT.ViewModels;

using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace ChatGPT.Views;

public sealed partial class ChatGPTView
{
    public ChatGPTView(ChatGptViewModel viewModel)
    {
        InitializeMaterialDesign();
        DataContext = viewModel;
        InitializeComponent();
    }
    private void InitializeMaterialDesign()
    {
        var card = new Card();
        var hue = new Hue("Dummy", Colors.Black, Colors.White);
    }
}