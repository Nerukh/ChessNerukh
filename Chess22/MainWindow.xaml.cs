using System.IO;
using System.Reflection.Emit;
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

namespace Chess22
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

        private void UpdateLabels()
        {
            Registration_label.Content = Chess22.Language.GetTranslation("Registration");
            Name_label.Content = Chess22.Language.GetTranslation("Name");
            Password_label.Content = Chess22.Language.GetTranslation("Password");
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\nv192\source\repos\Chess\Chess22\Users.txt";
            List<string> namesList = new List<string>();
            List<string> passwordList = new List<string>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    string[] nameParts = parts[0].Split(' ');
                    string[] passwordParts = parts[1].Split(' ');
                    string names = nameParts[1].ToLower();
                    string password = passwordParts[2];
                    namesList.Add(names);
                    passwordList.Add(password);
                }
                if (Name_TextBox.Text == "Server" && Password_TextBox.Text == "123") 
                {
                    Server server = new Server();
                    server.Show();
                    this.Close();
                }
                else if(sing_in_RadioButton.IsChecked == true)
                {
                    bool userExists = namesList.Contains(Name_TextBox.Text.ToLower());

                    if (!userExists)
                    {
                    string dataToWrite = $" \nName: {Name_TextBox.Text}; Password: {Password_TextBox.Text}; Wins: 0; Lose: 0; Games: 0; Rank: Newcomer";

                        File.AppendAllText(filePath, dataToWrite);

                        User user = new User();
                    user.Show();
                    this.Close();
                    }
                    else
                    {
                        Name_TextBox.Foreground = new SolidColorBrush(Colors.Red);
                        Name_TextBox.Text = Chess22.Language.GetTranslation("Error_Registration");
                    }

                }
                else
                {
                    for (int i = 0; i < passwordList.Count; i++)
                    {
                        if (passwordList[i] == Password_TextBox.Text && namesList[i].ToLower() == Name_TextBox.Text.ToLower())
                        {
                            
                            User user = new User();
                            user.Show();
                            this.Close();
                        }
                    }
                    Password_TextBox.Foreground = new SolidColorBrush(Colors.Red);
                    Password_TextBox.Text = Chess22.Language.GetTranslation("Error_Password");

                }
            }
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Language_ComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedLang = selectedItem.Tag.ToString();
                Chess22.Language.SetLanguage(selectedLang);
                UpdateLabels();
            }
        }

    }
}