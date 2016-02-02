﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using LiveCharts;
using LiveCharts.Annotations;
using LiveCharts.CoreComponents;
using LiveCharts.Optimizations;

namespace ChartsTest.Line_Examples.HighPerformance
{
    /// <summary>
    /// Interaction logic for HighPerformanceLine.xaml
    /// </summary>
    public partial class HighPerformanceLine : INotifyPropertyChanged
    {
        private DateTime _time;
        private ZoomingOptions _zoomingMode;

        public HighPerformanceLine()
        {
            InitializeComponent();

            //First you need to install LiveCharts.Optimizations
            //from Nuget:
            //Install-Package LiveCharts.Optimizations

            //low quality is actually really accurate
            //it could only have a +-3 pixels error
            //default is low quality.
            var highPerformanceMethod = new IndexedXAlgorithm<double>().WithQuality(DataQuality.Low);

            var config = new SeriesConfiguration<double>()
                .X((val, index) => index)
                .Y(val => val)
                .HasHighPerformanceMethod(highPerformanceMethod);

            Series = new SeriesCollection(config);

            var line = new LineSeries {Values = new ChartValues<double>()};

            var r = new Random();
            var trend = 0d;

            for (var i = 0; i < 1000000; i++)
            {
                if (i%1000 == 0) trend += r.Next(-500, 500);
                line.Values.Add(trend + r.Next(-10, 10));
            }

            Series.Add(line);

            var now = DateTime.Now.ToOADate();
            XFormat = val => DateTime.FromOADate(now + val/100).ToShortDateString();
            YFormat = val => Math.Round(val) + " ms";

            ZoomingMode = ZoomingOptions.XY;

            DataContext = this;
        }

        public SeriesCollection Series { get; set; }
        public Func<double, string> XFormat { get; set; }
        public Func<double, string> YFormat { get; set; }

        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                OnPropertyChanged();
            }
        }

        private void HighPerformanceLine_OnLoaded(object sender, RoutedEventArgs e)
        {
            //this is only to force animation everytime you change the current view.
            Chart.ClearAndPlot();
        }

        private void Chart_OnPlot(Chart obj)
        {
            //MessageBox.Show((DateTime.Now - _time).TotalMilliseconds.ToString("N0"));
        }

        private void XyOnClick(object sender, RoutedEventArgs e)
        {
            ZoomingMode = ZoomingOptions.XY;
        }

        private void XOnClick(object sender, RoutedEventArgs e)
        {
            ZoomingMode = ZoomingOptions.X;
        }

        private void YOnClick(object sender, RoutedEventArgs e)
        {
            ZoomingMode = ZoomingOptions.Y;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
