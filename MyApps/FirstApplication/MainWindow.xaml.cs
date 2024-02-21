using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public class todoObject
        {
            public todoObject(string title, string text, string date)
            {
                this.title = title;
                this.text = text;
                this.date = date;
            }

            public string title { get; set; }
            public string text { get; set; }
            public string date { get; set; }

        }

        public MainWindow()
        {
            var LocalStorage = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LocalStorage = System.IO.Path.Combine( LocalStorage, "ToDoApp");
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

        private void PickAnApp(object sender, RoutedEventArgs e)
        {
            
            object titleBlock = FindName("AppTitle");
            object appContainer = FindName("ThingsToDoPanel");
            (appContainer as Grid).Children.Clear();


            switch ((sender as Button).Name.ToString()){
                case "ToDoList":

                    WrapPanel mainPanel = new WrapPanel();
                    (appContainer as Grid).Children.Add(mainPanel);

                    (titleBlock as TextBlock).Text = "To do list";

                    string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                    string fileText = File.ReadAllText(storageFile);
                    todoObject[] toDo = JsonSerializer.Deserialize<todoObject[]>(fileText);

                    for (int i = 0; i < toDo.Length; i++)
                    {
                        CreateToDoNode(mainPanel, toDo[i].title, toDo[i].text, toDo[i].date, i);
                    }

                    Border AddTaskNode = new Border();
                    AddTaskNode.Style = (Style)Resources["ThingToDoBorder"];

                    mainPanel.Children.Add(AddTaskNode);

                    Grid AddTaskGrid = new Grid();
                    AddTaskGrid.Style = (Style)Resources["ThingToDoGrid"];

                    AddTaskNode.Child = AddTaskGrid;

                    Button addTask = new Button();
                    addTask.Style = (Style)Resources["PlusButton"];
                    addTask.Click += AddTask;

                    AddTaskGrid.Children.Add(addTask);

                    break;
                default:
                    (titleBlock as TextBlock).Text = "Jeszcze Nie dodaliśmy tej aplikacji";
                    break;
            }

        }

        public void AddTask(object sender, EventArgs e) {

            Grid AddTaskGrid = ((sender as Button).Parent as Grid);
            AddTaskGrid.Children.Clear();

            AddTaskGrid.Style = (Style)Resources["ThingToDoGrid"];


            RowDefinition firstRowDefinition = new RowDefinition();
            firstRowDefinition.Height = new GridLength(3, GridUnitType.Star);

            RowDefinition secondRowDefinition = new RowDefinition();
            secondRowDefinition.Height = new GridLength(12, GridUnitType.Star);

            RowDefinition thirdRowDefinition = new RowDefinition();
            thirdRowDefinition.Height = new GridLength(3, GridUnitType.Star);

            RowDefinition fourthRowDefinition = new RowDefinition();
            fourthRowDefinition.Height = new GridLength(2, GridUnitType.Star);

            AddTaskGrid.RowDefinitions.Add(firstRowDefinition);
            AddTaskGrid.RowDefinitions.Add(secondRowDefinition);
            AddTaskGrid.RowDefinitions.Add(thirdRowDefinition);
            AddTaskGrid.RowDefinitions.Add(fourthRowDefinition);

            Border titleBorder = new Border();
            Grid.SetRow(titleBorder, 0);
            titleBorder.Style = (Style)Resources["InputBorder"];

            AddTaskGrid.Children.Add(titleBorder);

            TextBox titleInput = new TextBox();
            titleInput.FontSize = 20;
            titleInput.FontWeight = FontWeights.Heavy;
            titleInput.TextAlignment = TextAlignment.Center;

            titleBorder.Child = titleInput;

            Border textBorder = new Border();
            Grid.SetRow(textBorder, 1);
            textBorder.Style = (Style)Resources["InputBorder"];

            AddTaskGrid.Children.Add(textBorder);

            TextBox textInput = new TextBox();
            textInput.FontSize = 20;
            textInput.TextWrapping = TextWrapping.Wrap;
            textInput.TextAlignment = TextAlignment.Center;

            textBorder.Child = textInput;

            DatePicker datePicker = new DatePicker();
            Grid.SetRow(datePicker, 2);

            AddTaskGrid.Children.Add(datePicker);

            Grid buttonGrid = new Grid();
            Grid.SetRow(buttonGrid, 3);

            ColumnDefinition leftColumns = new ColumnDefinition();
            leftColumns.Width = new GridLength(1, GridUnitType.Star);

            ColumnDefinition rightColumns = new ColumnDefinition();
            rightColumns.Width = new GridLength(1, GridUnitType.Star);

            buttonGrid.ColumnDefinitions.Add(leftColumns);
            buttonGrid.ColumnDefinitions.Add(rightColumns);

            AddTaskGrid.Children.Add(buttonGrid);

            void Cancel(object sender, EventArgs e)
            {
                AddTaskGrid.Children.Clear();
                AddTaskGrid.RowDefinitions.Clear();

                Button addTask = new Button();
                addTask.Style = (Style)Resources["PlusButton"];
                addTask.Click += AddTask;

                AddTaskGrid.Children.Add(addTask);
            }

            Button cancelButton = new Button();
            Grid.SetColumn(cancelButton, 0);
            cancelButton.Content = "Cancel";
            cancelButton.Margin = new Thickness(4);
            cancelButton.Click += Cancel;

            void Save(object sender, EventArgs e)
            {
                string saveTitle = titleInput.Text;
                string saveText = textInput.Text;
                string saveDate = datePicker.Text;

                todoObject saveObject = new todoObject(title: saveTitle, text: saveText, date: saveDate);

                string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                string fileText = File.ReadAllText(storageFile);
                todoObject[] toDo = JsonSerializer.Deserialize<todoObject[]>(fileText);

                int id = toDo.Length;

                toDo = toDo.Append(saveObject).ToArray();

                File.WriteAllText(storageFile, JsonSerializer.Serialize(toDo));

                Border AddTaskNode = AddTaskGrid.Parent as Border;
                WrapPanel mainPanel = AddTaskNode.Parent as WrapPanel;
                mainPanel.Children.Remove(AddTaskNode);
                CreateToDoNode(mainPanel, saveTitle, saveText, saveDate, id);

                AddTaskNode = new Border();
                AddTaskNode.Style = (Style)Resources["ThingToDoBorder"];

                mainPanel.Children.Add(AddTaskNode);

                AddTaskGrid = new Grid();
                AddTaskGrid.Style = (Style)Resources["ThingToDoGrid"];

                AddTaskNode.Child = AddTaskGrid;

                Button addTask = new Button();
                addTask.Style = (Style)Resources["PlusButton"];
                addTask.Click += AddTask;

                AddTaskGrid.Children.Add(addTask);
            }

            Button saveButton = new Button();
            Grid.SetColumn(saveButton, 1);
            saveButton.Content = "Save";
            saveButton.Margin = new Thickness(4);
            saveButton.Click += Save;

            buttonGrid.Children.Add(cancelButton);
            buttonGrid.Children.Add(saveButton);

        }

        public void DeleteNode(object sender, EventArgs e)
        {

            int id = int.Parse((sender as Button).Name.Split("id")[1]);

            string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
            string fileText = File.ReadAllText(storageFile);
            List<todoObject> toDo = JsonSerializer.Deserialize<todoObject[]>(fileText).ToList();

            toDo.RemoveAt(id);
            todoObject[] savetoDo = [.. toDo];

            File.WriteAllText(storageFile, JsonSerializer.Serialize(savetoDo));

            Border? AddTaskNode = ((sender as Button).Parent as Grid).Parent as Border;
            WrapPanel? mainPanel = AddTaskNode?.Parent as WrapPanel;
            mainPanel?.Children.Remove(AddTaskNode);

            foreach (Border Node in mainPanel.Children)
            {
                if (Node == mainPanel.Children[mainPanel.Children.Count - 1]) break;

                Button deleteButton = (Node.Child as Grid).Children[1] as Button;
                int curr_id = int.Parse(deleteButton.Name.Split("id")[1]);

                if (curr_id > id)
                {
                    deleteButton.Name = "id" + (curr_id - 1).ToString();
                }
            }
        }

        public class ToDoNode
        {
            public ToDoNode( WrapPanel parent, string title, string text, string date, int id, ResourceDictionary resources) {

                this.title = title;
                this.text = text;
                this.date = date;
                this.id = id;

                Border newNode = new Border();
                newNode.Style = (Style)resources["ThingToDoBorder"];

                parent.Children.Add(newNode);

                Grid newGrid = new Grid();
                newGrid.Style = (Style)resources["ThingToDoGrid"];

                newNode.Child = newGrid;

                RowDefinition firstRowDefinition = new RowDefinition();
                firstRowDefinition.Height = new GridLength(3, GridUnitType.Star);

                RowDefinition secondRowDefinition = new RowDefinition();
                secondRowDefinition.Height = new GridLength(4, GridUnitType.Star);

                RowDefinition thirdRowDefinition = new RowDefinition();
                thirdRowDefinition.Height = new GridLength(13, GridUnitType.Star);

                newGrid.RowDefinitions.Add(firstRowDefinition);
                newGrid.RowDefinitions.Add(secondRowDefinition);
                newGrid.RowDefinitions.Add(thirdRowDefinition);

                TextBlock dateBlock = new TextBlock();
                Grid.SetRow(dateBlock, 0);
                dateBlock.Text = "Do unti: " + date;
                dateBlock.FontSize = 12;
                dateBlock.FontWeight = FontWeights.Bold;
                dateBlock.TextAlignment = TextAlignment.Right;

                newGrid.Children.Add(dateBlock);


                Button delete = new Button();
                Grid.SetRow(delete, 0);
                delete.Name = "id" + id.ToString();
                delete.HorizontalAlignment = HorizontalAlignment.Left;
                delete.Style = (Style)resources["XButton"];
                delete.Click += DeleteNode;

                this.delete = delete;

                newGrid.Children.Add(delete);

                TextBlock titleBlock = new TextBlock();
                Grid.SetRow(titleBlock, 1);
                titleBlock.Text = title;
                titleBlock.FontSize = 20;
                titleBlock.FontWeight = FontWeights.Heavy;
                titleBlock.TextAlignment = TextAlignment.Center;

                newGrid.Children.Add(titleBlock);

                TextBlock textBlock = new TextBlock();
                Grid.SetRow(textBlock, 2);
                textBlock.Text = text;
                textBlock.FontSize = 20;
                textBlock.TextAlignment = TextAlignment.Center;

                newGrid.Children.Add(textBlock);
            }

            private void DeleteNode(object sender, EventArgs e)
            {
                int id = int.Parse((sender as Button).Name.Split("id")[1]);

                string storageFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ToDoApp\\storage.json";
                string fileText = File.ReadAllText(storageFile);
                List<todoObject> toDo = JsonSerializer.Deserialize<todoObject[]>(fileText).ToList();

                toDo.RemoveAt(id);
                todoObject[] savetoDo = [.. toDo];

                File.WriteAllText(storageFile, JsonSerializer.Serialize(savetoDo));

                Border? AddTaskNode = ((sender as Button).Parent as Grid).Parent as Border;
                WrapPanel? mainPanel = AddTaskNode?.Parent as WrapPanel;
                mainPanel?.Children.Remove(AddTaskNode);

                foreach (Border Node in mainPanel.Children)
                {
                    if (Node == mainPanel.Children[mainPanel.Children.Count - 1]) break;

                    Button deleteButton = (Node.Child as Grid).Children[1] as Button;
                    int curr_id = int.Parse(deleteButton.Name.Split("id")[1]);

                    if (curr_id > id)
                    {
                        deleteButton.Name = "id" + (curr_id - 1).ToString();
                    }
                }
            }

            public void setButtonId() { delete.Name = "id" + (id - 1).ToString(); }

            public Button delete {  get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public string date { get; set; }

        }

        private void CreateToDoNode( WrapPanel parent, string title, string text, string date, int id )
        {
            Border newNode = new Border();
            newNode.Style = (Style)Resources["ThingToDoBorder"];

            parent.Children.Add(newNode);

            Grid newGrid = new Grid();
            newGrid.Style = (Style)Resources["ThingToDoGrid"];

            newNode.Child = newGrid; 

            RowDefinition firstRowDefinition = new RowDefinition();
            firstRowDefinition.Height = new GridLength(3, GridUnitType.Star);

            RowDefinition secondRowDefinition = new RowDefinition();
            secondRowDefinition.Height = new GridLength(4, GridUnitType.Star);

            RowDefinition thirdRowDefinition = new RowDefinition();
            thirdRowDefinition.Height = new GridLength(13, GridUnitType.Star);

            newGrid.RowDefinitions.Add(firstRowDefinition);
            newGrid.RowDefinitions.Add(secondRowDefinition);
            newGrid.RowDefinitions.Add(thirdRowDefinition);

            TextBlock dateBlock = new TextBlock();
            Grid.SetRow(dateBlock, 0);
            dateBlock.Text = "Do unti: " + date;
            dateBlock.FontSize = 12;
            dateBlock.FontWeight = FontWeights.Bold;
            dateBlock.TextAlignment = TextAlignment.Right;

            newGrid.Children.Add(dateBlock);


            Button delete = new Button();
            Grid.SetRow(delete, 0);
            delete.Name = "id" + id.ToString();
            delete.HorizontalAlignment = HorizontalAlignment.Left;
            delete.Style = (Style)Resources["XButton"];
            delete.Click += DeleteNode;

            newGrid.Children.Add(delete);

            TextBlock titleBlock = new TextBlock();
            Grid.SetRow(titleBlock, 1);
            titleBlock.Text = title;
            titleBlock.FontSize = 20;
            titleBlock.FontWeight = FontWeights.Heavy;
            titleBlock.TextAlignment = TextAlignment.Center;

            newGrid.Children.Add(titleBlock);

            TextBlock textBlock = new TextBlock();
            Grid.SetRow(textBlock, 2);
            textBlock.Text = text;
            textBlock.FontSize = 20;
            textBlock.TextAlignment = TextAlignment.Center;

            newGrid.Children.Add(textBlock);
        }
    }
}