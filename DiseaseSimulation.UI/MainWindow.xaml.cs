using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using DiseaseSimulation.Core;
using DiseaseSimulation.Core.Models;

namespace DiseaseSimulation.UI;

public partial class MainWindow : Window
{
    private SimulationEngine? _engine;
    private int _cellSize = 25;
    private DispatcherTimer _timer;
    private int _currentStep = 0;

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer();
        _timer.Tick += Timer_Tick;
        SetSpeed(50);
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_engine == null)
        {
            return;
        }

        if (_currentStep >= _engine.Config.Steps)
        {
            _timer.Stop();
            return;
        }

        _engine.NextStep();
        _currentStep++;
        RefreshMap();
        UpdateStats();  
    }

    private void SetSpeed(double sliderValue)
    {
        if (_timer == null)
        {
            return;
        }
        
        int ms = (int)(1000 - sliderValue * 9.5);
        _timer.Interval = TimeSpan.FromMilliseconds(Math.Max(50, ms));
    }


    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (_engine == null)
        {
            MessageBox.Show("Load a map first via ⚙ Settings.", "No map loaded");
            return;
        }
        UpdateStats();
        _timer.Start();
    }
    
    private void StepButton_Click(object sender, RoutedEventArgs e)
    {
        if (_engine == null)
        {
            MessageBox.Show("Load a map first via ⚙ Settings.", "No map loaded");
            return;
        }
        _engine.NextStep();
        _currentStep++;
        RefreshMap();
        UpdateStats();  
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _currentStep = 0;
        DayText.Text = "Day: 1";
        
        StatsList.Items.Clear();

        if (_engine != null)
        {
            _engine = new SimulationEngine(_engine.Config);
            LoadAndDisplayMap();
        }
        
        int s = 0, e2 = 0, inf = 0, r = 0;
        for (int x = 0; x < _engine.World.Width; x++)
        for (int y = 0; y < _engine.World.Height; y++)
        {
            var entity = _engine.World.Cells[x, y].Entity;
            if (entity == null) continue;
            switch (entity.HealthState)
            {
                case HealthState.Susceptible: s++;   break;
                case HealthState.Exposed:     e2++;  break;
                case HealthState.Infected:    inf++; break;
                case HealthState.Recovered:   r++;   break;
            }
        }

        DayText.Text       = "Day: 1";
        TotalText.Text     = $"Total:  {s + e2 + inf + r}";
        SusceptText.Text   = $"🟢 Susceptible: {s}";
        ExposedText.Text   = $"🟡 Exposed:     {e2}";
        InfectedText.Text  = $"🔴 Infected:    {inf}";
        RecoveredText.Text = $"⚪ Recovered:   {r}";
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();

        var settings = new SettingsWindow { Owner = this };
        if (settings.ShowDialog() != true) return;

        var config = new SimulationConfig
        {
            MapFilepath = settings.SelectedFilePath!,
            Steps = settings.Steps,
            InfectionProbability = settings.InfectionProbability,
            InfectionDuration = settings.InfectionDuration,
            IncubationDuration = settings.IncubationDuration,
            MobilityRate = 0.2,
        };

        _engine = new SimulationEngine(config);
        _currentStep = 0;
        ModelTypeText.Text = config.Model.ToString() + " MODEL";

        StatsList.Items.Clear();
        
        int s = 0, e2 = 0, inf = 0, r = 0;
        for (int x = 0; x < _engine.World.Width; x++)
        for (int y = 0; y < _engine.World.Height; y++)
        {
            var entity = _engine.World.Cells[x, y].Entity;
            if (entity == null) continue;
            switch (entity.HealthState)
            {
                case HealthState.Susceptible: s++;   break;
                case HealthState.Exposed:     e2++;  break;
                case HealthState.Infected:    inf++; break;
                case HealthState.Recovered:   r++;   break;
            }
        }

        DayText.Text       = "Day: 1";
        TotalText.Text     = $"Total:  {s + e2 + inf + r}";
        SusceptText.Text   = $"🟢 Susceptible: {s}";
        ExposedText.Text   = $"🟡 Exposed:     {e2}";
        InfectedText.Text  = $"🔴 Infected:    {inf}";
        RecoveredText.Text = $"⚪ Recovered:   {r}";
        
        LoadAndDisplayMap();
    }

    private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        SetSpeed(e.NewValue);
    }

    private void LoadAndDisplayMap()
    {
        if (_engine?.World == null) return;

        int width = _engine.World.Width;
        int height = _engine.World.Height;

        MapGrid.Children.Clear();
        MapGrid.RowDefinitions.Clear();
        MapGrid.ColumnDefinitions.Clear();

        for (int x = 0; x < width; x++)
            MapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_cellSize) });

        for (int y = 0; y < height; y++)
            MapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_cellSize) });

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Entity? entity = _engine.World.Cells[x, y].Entity;
                HealthState state = entity?.HealthState ?? HealthState.Susceptible;

                var cell = new Grid();

                var rect = new Rectangle
                {
                    Fill = GetColor(state, entity == null),
                    Stroke = Brushes.DarkSlateGray,
                    StrokeThickness = 0.5
                };
                cell.Children.Add(rect);

                if (entity != null)
                {
                    var label = new TextBlock
                    {
                        Text = entity.Id.ToString(),
                        FontSize = 7,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold
                    };
                    cell.Children.Add(label);
                }

                Grid.SetRow(cell, y);
                Grid.SetColumn(cell, x);
                MapGrid.Children.Add(cell);
            }
        }
    }

    public void RefreshMap()
    {
        if (_engine?.World == null) return;

        int width = _engine.World.Width;
        int height = _engine.World.Height;
        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Entity? entity = _engine.World.Cells[x, y].Entity;
                HealthState state = entity?.HealthState ?? HealthState.Susceptible;

                if (index < MapGrid.Children.Count)
                {
                    var cellGrid = (Grid)MapGrid.Children[index];
                    var rect = (Rectangle)cellGrid.Children[0];
                    rect.Fill = GetColor(state, entity == null);

                    if (cellGrid.Children.Count > 1)
                        cellGrid.Children.RemoveAt(1);

                    if (entity != null)
                    {
                        var label = new TextBlock
                        {
                            Text = entity.Id.ToString(),
                            FontSize = 7,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = Brushes.Black,
                            FontWeight = FontWeights.Bold
                        };
                        cellGrid.Children.Add(label);
                    }
                }

                index++;
            }
        }
    }

    private Brush GetColor(HealthState state, bool isEmpty)
    {
        if (isEmpty) return Brushes.Transparent;
        return state switch
        {
            HealthState.Susceptible => Brushes.LightGreen,
            HealthState.Exposed     => Brushes.Gold,
            HealthState.Infected    => Brushes.Red,
            HealthState.Recovered   => Brushes.Blue,
            _                       => Brushes.White,
        };
    }
    public class StatsRow
    {
        public string Display { get; set; } = "";
    }
    
    private void UpdateStats()
    {
        if (_engine == null || _engine.Stats.Count == 0) return;

        var latest = _engine.Stats[^1];
        int day = _engine.Stats.Count;

        DayText.Text       = $"Day: {day}";
        TotalText.Text     = $"Total:  {latest.TotalPopulation}";
        SusceptText.Text   = $"🟢 Susceptible: {latest.Susceptible}";
        ExposedText.Text   = $"🟡 Exposed:     {latest.Exposed}";
        InfectedText.Text  = $"🔴 Infected:    {latest.Infected}";
        RecoveredText.Text = $"⚪ Recovered:   {latest.Recovered}";
        
        StatsList.Items.Add(new StatsRow
        {
            Display = $"D{day,3}: S={latest.Susceptible,3} E={latest.Exposed,3} I={latest.Infected,3} R={latest.Recovered,3}"
        });
        
        StatsScroller.ScrollToBottom();
    }
}