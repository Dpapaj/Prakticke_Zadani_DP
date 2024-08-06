using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Xml.Linq;

namespace App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadXmlButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                LoadXmlData(filePath);
            }
        }

        private void LoadXmlData(string filePath)
        {
            XDocument xDocument = XDocument.Load(filePath);
            var cars = xDocument.Descendants("Car").Select(c => new
            {
                Model = c.Element("Model").Value,
                DateOfSale = DateTime.ParseExact(c.Element("DateOfSale").Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Price = double.Parse(c.Element("Price").Value),
                DPH = double.Parse(c.Element("DPH").Value)
            }).ToList();

            CarsDataGrid.ItemsSource = cars;

            var results = cars.GroupBy(c => c.Model).Select(g => new
            {
                Model = g.Key,
                PriceWithoutVAT = g.Where(c => c.DateOfSale.DayOfWeek == DayOfWeek.Saturday || c.DateOfSale.DayOfWeek == DayOfWeek.Sunday).Sum(c => c.Price),
                PriceWithVAT = g.Where(c => c.DateOfSale.DayOfWeek == DayOfWeek.Saturday || c.DateOfSale.DayOfWeek == DayOfWeek.Sunday).Sum(c => c.Price + c.Price * c.DPH / 100)
            }).ToList();

            ResultsDataGrid.ItemsSource = results;
        }
    }
}
