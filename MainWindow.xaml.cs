using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CalcSistemasLineares
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private double[,] matriz;
        private TextBox[,] textBoxes;
        Label[,] labels;
        Label[] resultadoDisplay;
        private int tamanho = 3;
        public MainWindow()
        {
            InitializeComponent();
            CriaGrid();
            CriarDisplayMatriz(uniformGrid1,"dism");
            CriarDisplayMatriz(uniformGrid2, "escal");
            CriarDisplayResultado();

        }
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!double.TryParse(((TextBox)sender).Text + e.Text, out double result)
                && !(((TextBox)sender).Text == "" && e.Text == "-") // permitir o sinal de menos no início da caixa de texto
                && !(((TextBox)sender).Text.Length == 1 && ((TextBox)sender).Text[0] == '-' && e.Text != ".") // permitir apenas o ponto decimal após o sinal de menos
                && !(((TextBox)sender).Text.Length > 1 && ((TextBox)sender).Text[0] == '-' && ((TextBox)sender).Text.LastIndexOf('.') < ((TextBox)sender).Text.IndexOf('-')) // permitir apenas um ponto decimal após o sinal de menos
                )
            {
                e.Handled = true;
                MessageBox.Show("Por favor, insira apenas números", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;

            }
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private Label FindLabelInUniformGrid(UniformGrid uniformGrid, string labelName)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(uniformGrid); i++)
            {
                var child = VisualTreeHelper.GetChild(uniformGrid, i);

                if (child is Label label && label.Name == labelName)
                {
                    return label;
                }
            }

            return null;
        }
        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var cellName = textBox.Name.Substring(2); // remove o "tb" do nome para obter o nome da célula
            //var cell = (Label)FindName("dism" + cellName); // encontra a célula correspondente na UniformGrid
            var cell = FindLabelInUniformGrid(uniformGrid1, "dism" + cellName);
            cell.Content = textBox.Text; // atualiza o conteúdo da célula com o texto da TextBox
        }
        private void CriaGrid()
        {
            // Definir as linhas e colunas do Grid dinamicamente
            txtGrid.RowDefinitions.Clear();
            txtGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < tamanho; i++)
            {
                txtGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < tamanho + 1; i++) { txtGrid.ColumnDefinitions.Add(new ColumnDefinition()); }
            // Adicionar os TextBoxes ao Grid
            txtGrid.Children.Clear();
            textBoxes = new TextBox[tamanho, tamanho + 1];
            for (int row = 0; row < tamanho; row++)
            {
                for (int col = 0; col < tamanho + 1; col++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Name = $"tb{row}{col}";
                    textBox.HorizontalAlignment = HorizontalAlignment.Center;
                    textBox.VerticalAlignment = VerticalAlignment.Center;
                    textBox.MinWidth = 172;
                    textBox.MinHeight = 30;
                    textBox.Height = 30;
                    textBox.Width = 172;
                    textBox.TextAlignment = TextAlignment.Center;
                    textBox.Margin = new Thickness(10);
                    textBox.TextChanged += tb_TextChanged;
                    textBox.PreviewTextInput += tb_PreviewTextInput;
                    
                    Grid.SetRow(textBox, row);
                    Grid.SetColumn(textBox, col);
                    
                    txtGrid.Children.Add(textBox);
                    textBoxes[row, col] = textBox;

                }
            }
        }

        private void CriarDisplayMatriz(UniformGrid uniformGrid, string NomeLabel)
        {
            uniformGrid.Children.Clear();
            labels = new Label[tamanho, tamanho+1];

            for (int i = 0; i < tamanho; i++) 
            {
                for (int j = 0; j < tamanho+1; j++) // percorre as colunas da matriz
                {
                    // cria uma nova instância de Label
                    Label label = new Label();
                    label.Name = NomeLabel + i.ToString() + j.ToString(); // define o nome da Label com base na posição da matriz
                    label.Foreground = Brushes.White;
                    label.FontWeight = FontWeights.Light;
                    label.FontSize = 14;

                    // adiciona a Label à matriz e ao UniformGrid
                    labels[i, j] = label;
                    uniformGrid.Children.Add(label);

                }
            }
        }
        private void CriarDisplayResultado() { 
            uniformGridResultado.Children.Clear();
            resultadoDisplay = new Label[tamanho];

            for(int i  = 0; i < tamanho;i++)
            {
                // cria uma nova instância de Label
                Label label = new Label();
                label.Name = "tbResultado" + i.ToString(); // define o nome da Label com base na posição da matriz
                label.Foreground = Brushes.LightGoldenrodYellow;
                label.FontWeight = FontWeights.SemiBold;
                label.FontSize = 15;
                label.HorizontalAlignment = HorizontalAlignment.Center;

                // adiciona a Label à matriz e ao UniformGrid
                resultadoDisplay[i] = label;
                uniformGridResultado.Children.Add(label);
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Obter o tamanho selecionado a partir do header do MenuItem
            MenuItem menuItem = (MenuItem)sender;
            tamanho = Convert.ToInt32(menuItem.Header);
            
            CriaGrid();
            CriarDisplayMatriz(uniformGrid1, "dism");
            CriarDisplayMatriz(uniformGrid2, "escal");
            CriarDisplayResultado();
        }
        
        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {

            // inicializa a matriz com os valores informados nos campos de texto
            matriz = new double[tamanho, tamanho + 1];
            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho + 1; j++)
                {
                    try
                    {
                        matriz[i, j] = double.Parse(textBoxes[i, j].Text);

                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Erro ao analisar '{textBoxes[i, j].Text}': {ex.Message}");
                    }
                }
            }
            for (int i = 0; i < tamanho + 1; i++)
            {
                var cell = FindLabelInUniformGrid(uniformGrid2, "escal" + 0 + i); // encontra a célula correspondente na UniformGrid
                cell.Content = matriz[0, i];
            }
            // realiza a eliminação de Gauss
            for (int i = 0; i < tamanho - 1; i++)
            {
                if (matriz[i, i] == 0)
                {
                    // Procura uma linha abaixo da atual que tenha um elemento diferente de zero na mesma coluna
                    bool found = false;
                    for (int j = i + 1; j < tamanho; j++)
                    {
                        if (matriz[j, i] != 0)
                        {
                            found = true;
                            // Troca as linhas i e j
                            for (int k = i; k < tamanho + 1; k++)
                            {
                                double temp = matriz[i, k];
                                matriz[i, k] = matriz[j, k];
                                matriz[j, k] = temp;
                            }
                            break;
                        }
                    }
                    // Se não houver uma linha disponível com um elemento diferente de zero, a matriz não pode ser resolvida usando esse método
                    if (!found)
                    {
                        
                        MessageBox.Show("Matriz não escalonável", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        CriarDisplayMatriz(uniformGrid2, "escal");
                        return;
                    }
                }
                for (int j = i + 1; j < tamanho; j++)
                {
                    double m = matriz[j, i] / matriz[i, i];
                    for (int k = i; k < tamanho + 1; k++)
                    {
                        matriz[j, k] = matriz[j, k] - m * matriz[i, k];

                        var cell = FindLabelInUniformGrid(uniformGrid2, "escal" + j + k); // encontra a célula correspondente na UniformGrid
                        cell.Content = Math.Round(matriz[j, k], 8); // atualiza o conteúdo da célula com o texto da TextBox

                    }
                }
            }

            // resolve o sistema linear
            double[] x = new double[tamanho];
            const double precision = 1e-16;
            for (int i = tamanho - 1; i >= 0; i--)
            {
                x[i] = matriz[i, tamanho];
                for (int j = i + 1; j < tamanho; j++)
                {
                    x[i] = x[i] - matriz[i, j] * x[j];
                }


                x[i] = x[i] / matriz[i, i];

                if (Math.Abs(x[i]) <= precision)
                {
                    x[i] = 0; // igualar a zero caso seja muito próximo de zero

                }
                var cell = FindLabelInUniformGrid(uniformGridResultado, "tbResultado" + i); 
                cell.Content = "x" + (i+1) + " = "+ x[i]; // atualiza o conteúdo da célula com o texto da TextBox
            }


        }


    }

}