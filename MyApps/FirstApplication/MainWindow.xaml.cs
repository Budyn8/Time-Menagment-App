using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FirstApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object titleBlock = FindName("AppTitle");
            object appContainer = FindName("ThingsToDoPanel");
            (appContainer as Grid).Children.Clear();


            switch ((sender as Button).Name.ToString()){
                case "ToDoList":

                    WrapPanel wrapPanel = new WrapPanel();
                    (appContainer as Grid).Children.Add(wrapPanel);

                    (titleBlock as TextBlock).Text = "To do list";

                    for (int i = 1; i < 5; i++)
                    {
                        string title = "Node nr." + i.ToString();
                        CreateToDoNode(wrapPanel, title, "Create this app");
                    }

                    break;
                default:
                    (titleBlock as TextBlock).Text = "Jeszcze Nie dodaliśmy tej aplikacji";
                    break;
            }

        }

        private void CreateToDoNode( object parent, string title, string desc )
        {
            Border newNode = new Border();
            newNode.Style = (Style)Resources["ThingToDoBorder"];

            (parent as WrapPanel).Children.Add(newNode);

            Grid newGrid = new Grid();
            newGrid.Style = (Style)Resources["ThingToDoGrid"];

            newNode.Child = newGrid;

            RowDefinition firstRowDefinition = new RowDefinition();
            firstRowDefinition.Height = new GridLength(1, GridUnitType.Star);

            RowDefinition secondRowDefinition = new RowDefinition();
            secondRowDefinition.Height = new GridLength(9, GridUnitType.Star);

            newGrid.RowDefinitions.Add(firstRowDefinition);
            newGrid.RowDefinitions.Add(secondRowDefinition);

            TextBlock titleBlock = new TextBlock();
            Grid.SetRow(titleBlock, 0);
            titleBlock.Text = title;
            titleBlock.FontSize = 20;
            titleBlock.FontWeight = FontWeights.Heavy;
            titleBlock.TextAlignment = TextAlignment.Center;

            newGrid.Children.Add(titleBlock);

            TextBlock descriptionBlock = new TextBlock();
            Grid.SetRow(descriptionBlock, 1);
            descriptionBlock.Text = desc;
            descriptionBlock.FontSize = 20;
            descriptionBlock.TextAlignment = TextAlignment.Center;

            newGrid.Children.Add(descriptionBlock);
        }
    }
}