using System.Windows;
using Microsoft.Win32;

namespace DiseaseSimulation.UI;

public partial class SettingsWindow : Window
{
    public string? SelectedFilePath { get; private set; }
    public int Steps { get; private set; }
    public double InfectionProbability { get; private set; }
    public int InfectionDuration { get; private set; }
    public int IncubationDuration { get; private set; }

    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = "Choose map file",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        };
        if (dlg.ShowDialog() == true)
            FilePathBox.Text = dlg.FileName;
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate
        if (string.IsNullOrEmpty(FilePathBox.Text))
        {
            MessageBox.Show("Please select a map file.", "Missing file", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(StepsBox.Text, out int steps) || steps <= 0)
        {
            MessageBox.Show("Steps must be a positive number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!double.TryParse(InfectionProbBox.Text, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double prob) || prob < 0 || prob > 1)
        {
            MessageBox.Show("Infection probability must be between 0.0 and 1.0.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(InfectionDurBox.Text, out int infDur) || infDur <= 0)
        {
            MessageBox.Show("Infection duration must be a positive number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(IncubationDurBox.Text, out int incDur) || incDur <= 0)
        {
            MessageBox.Show("Incubation duration must be a positive number.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedFilePath = FilePathBox.Text;
        Steps = steps;
        InfectionProbability = prob;
        InfectionDuration = infDur;
        IncubationDuration = incDur;

        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}