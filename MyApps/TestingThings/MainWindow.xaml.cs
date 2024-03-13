using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestingThings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var LocalStorage = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LocalStorage = System.IO.Path.Combine(LocalStorage, "ToDoApp");
            if (!Directory.Exists(LocalStorage))
            {
                Directory.CreateDirectory(LocalStorage);
                LocalStorage = System.IO.Path.Combine(LocalStorage, "storage.json");
                if (!File.Exists(LocalStorage))
                {
                    File.Create(LocalStorage);
                }
            }
            InitializeComponent();
        }

        public class TodoObject(string title, string text, string date)
        {
            public string Title { get; set; } = title;
            public string Text { get; set; } = text;
            public string Date { get; set; } = date;

        }

        public class ToDoNode
        {

            Border? Container { get; set; }
            Button? DeleteButton { get; set; }
            public int? Id { get; set; }
            public string? Title { get; set; }
            public string? Content { get; set; }
            public string? Date { get; set; }

            public ToDoNode CreateANode(WrapPanel mainPanel, int id, string title, string content, string date, ResourceDictionary resources) { 
                Id = id;
                Title = title;
                Content = content;
                Date = date;

                Container = new Border {
                    Style = (Style)resources["ThingToDoBorder"],
                    Name = "id" + Id
                };

                DeleteButton = new Button
                {
                    Style = (Style)resources["XButton"],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                DeleteButton.Click += Delete;

                TextBlock dateBlock = new()
                {
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Right,
                    Text = "do until: " + (date == "" ? "no date" : date)
                };
                Grid.SetColumn(dateBlock, 1);

                TextBlock titleBlock = new()
                {
                    FontSize = 20,
                    FontWeight = FontWeights.Heavy,
                    TextAlignment = TextAlignment.Center,
                    Text = title
                };
                Grid.SetRow(titleBlock, 1);

                TextBlock contentBlock = new()
                {
                    FontSize = 15,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center,
                    Text = content,
                };
                Grid.SetRow(contentBlock, 2);

                Container.Child = new Grid
                {
                    Style = (Style)resources["ThingToDoGrid"],
                    RowDefinitions =
                    {
                        new RowDefinition{ Height = new GridLength(3, GridUnitType.Star) },
                        new RowDefinition{ Height = new GridLength(4, GridUnitType.Star) },
                        new RowDefinition{ Height = new GridLength(13, GridUnitType.Star) },
                    },
                    Children =
                    {
                        new Grid {
                            Margin = new Thickness(5),
                            ColumnDefinitions = {
                                new ColumnDefinition{ Width = new GridLength (2, GridUnitType.Star) },
                                new ColumnDefinition{ Width = new GridLength(8, GridUnitType.Star) }
                            },
                            Children =
                            {
                                DeleteButton,
                                dateBlock
                            }
                        },
                        titleBlock,
                        contentBlock
                    }
                };

                mainPanel.Children.Add(Container);
                return this;
            }

            public ToDoNode? GetNodeFromElement(UIElement container) {
                Container = (Border)container;
                if (Container.Name == "addNode") return null;
                DeleteButton = (Button)((Grid)((Grid)Container.Child).Children[0]).Children[0];
                Id = int.Parse(Container.Name.Split("id")[1]);

                UIElementCollection gridNode = ((Grid)Container.Child).Children;

                Date = ((TextBlock)((Grid)gridNode[0]).Children[1]).Text;
                Title = ((TextBlock)gridNode[1]).Text;
                Content = ((TextBlock)gridNode[2]).Text;

                return this;
            }

            private void ChangeId()
            {
                if( Container == null ) return;
                Id--;
                Container.Name = "id" + Id;
            }

            private void Delete(object sender, RoutedEventArgs e)
            {
                if (Container == null) return;
                WrapPanel mainPanel = (WrapPanel)Container.Parent;
                Id = int.Parse(Container.Name.Split("id")[1]);
                mainPanel.Children.Remove(Container);

                string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                string fileText = File.ReadAllText(storageFile);
                List<TodoObject> toDo = [.. JsonSerializer.Deserialize<TodoObject[]>(fileText)];

                toDo.RemoveAt(int.Parse(Container.Name.Split("id")[1]));
                TodoObject[] savetoDo = [.. toDo];

                File.WriteAllText(storageFile, JsonSerializer.Serialize(savetoDo));

                foreach (UIElement Child in mainPanel.Children)
                {
                    ToDoNode ToDoTask = new ToDoNode().GetNodeFromElement(Child);
                    if (ToDoTask == null) continue;
                    if (ToDoTask.Id >= Id) ToDoTask.ChangeId();
                }
            }

        }

        public class AddToDoNode
        {
            Border Container { get; set; }
            ResourceDictionary Resources { get; set; }
            public AddToDoNode(ResourceDictionary resources, WrapPanel mainPanel ) {
                Resources = resources;
                Container = new()
                {
                    Style = (Style)resources["ThingToDoBorder"],
                    Name = "addNode"
                };

                mainPanel.Children.Add(Container);

                Grid AddTaskGrid = new()
                {
                    Style = (Style)resources["ThingToDoGrid"]
                };

                Container.Child = AddTaskGrid;

                Button addTask = new()
                {
                    Style = (Style)resources["PlusButton"]
                };
                addTask.Click += AddTask;

                AddTaskGrid.Children.Add(addTask);
            }

            private void AddTask(object sender, RoutedEventArgs e)
            {
                
                ((Grid)Container.Child).Children.Clear();

                ((Grid)Container.Child).RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(3, GridUnitType.Star)
                });

                ((Grid)Container.Child).RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(12, GridUnitType.Star)
                });

                ((Grid)Container.Child).RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(3, GridUnitType.Star)
                });

                ((Grid)Container.Child).RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(2, GridUnitType.Star)
                });

                Border title = new()
                {
                    Style = (Style)Resources["InputBorder"],
                    Child = new TextBox
                    {
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    }
                };
                Grid.SetRow(title, 0);

                Border text = new()
                {
                    Style = (Style)Resources["InputBorder"],
                    Child = new TextBox
                    {
                        FontSize = 20,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center
                    }
                };
                Grid.SetRow(text, 1);

                DatePicker datePicker = new();
                Grid.SetRow(datePicker, 2);

                Button cancelButton = new() {
                    Content = "Cancel",
                    Margin = new Thickness(4)
                };

                Grid.SetColumn(cancelButton, 0);
                cancelButton.Click += CanceleAdd;

                Button saveButton = new();
                Grid.SetColumn(saveButton, 1);
                saveButton.Content = "Save";
                saveButton.Margin = new Thickness(4);
                saveButton.Click += (object sender, RoutedEventArgs e) => {
                    string saveTitle = ((TextBox)title.Child).Text;
                    string saveText = ((TextBox)text.Child).Text;
                    string saveDate = datePicker.Text;

                    TodoObject saveObject = new(title: saveTitle, text: saveText, date: saveDate);

                    string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                    string fileText = File.ReadAllText(storageFile);
                    TodoObject[] toDo = JsonSerializer.Deserialize<TodoObject[]>(fileText);

                    int id = toDo.Length;

                    toDo = [.. toDo, saveObject];

                    File.WriteAllText(storageFile, JsonSerializer.Serialize(toDo));

                    new ToDoNode().CreateANode((WrapPanel)Container.Parent, id, saveTitle, saveText, saveDate, Resources);
                    _ = new AddToDoNode(Resources, (WrapPanel)Container.Parent);
                    ((WrapPanel)Container.Parent).Children.Remove(Container);
                };

                Grid buttons = new()
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(){ Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition(){ Width = new GridLength(1, GridUnitType.Star) }
                    },
                    Children =
                    {
                        cancelButton,
                        saveButton
                    }
                };
                Grid.SetRow(buttons, 3);

                ((Grid)Container.Child).Children.Add(title);
                ((Grid)Container.Child).Children.Add(text);
                ((Grid)Container.Child).Children.Add(datePicker);
                ((Grid)Container.Child).Children.Add(buttons);
            }

            public void CanceleAdd(object sender, EventArgs e) {
                    ((Grid) Container.Child).Children.Clear();
                    ((Grid) Container.Child).RowDefinitions.Clear();

                Button addTask = new()
                {
                    Style = (Style)Resources["PlusButton"]
                };
                addTask.Click += AddTask;

                    ((Grid) Container.Child).Children.Add(addTask);
            }
    }

        private void PickAnApp(object sender, RoutedEventArgs e)
        {

            TextBlock titleBlock = (TextBlock)FindName("AppTitle");
            Grid appContainer = (Grid)FindName("ThingsToDoPanel");
            appContainer.Children.Clear();


            switch (((Button)sender).Name.ToString())
            {
                case "ToDoList":

                    WrapPanel mainPanel = new() {
                        Name = "main"
                    };
                    appContainer.Children.Add(mainPanel);

                    titleBlock.Text = "To do list";

                    string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                    string fileText = File.ReadAllText(storageFile);

                    TodoObject[] toDo = JsonSerializer.Deserialize<TodoObject[]>(fileText);

                    for (int i = 0; i < toDo.Length; i++)
                    {
                        new ToDoNode().CreateANode(mainPanel, i, toDo[i].Title, toDo[i].Text, toDo[i].Date, Resources);
                    }

                    _ = new AddToDoNode(Resources, mainPanel);
                    break;
                default:
                    titleBlock.Text = "Jeszcze Nie dodaliśmy tej aplikacji";
                    break;
            }

        }
    }
}