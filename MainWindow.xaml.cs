using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;

namespace WlcApCleaner
{
    public partial class MainWindow : Window
    {
        private List<AccessPoint> _showRunAps = new();
        private List<string> _apSummaryMacs = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadShowRunConfig_Click(object sender, RoutedEventArgs e)
        {
            var filePath = ShowFileDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            _showRunAps.Clear();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (line.Contains("username") && line.Contains("mac description"))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 4)
                    {
                        var mac = parts[1].ToLower();
                        var name = string.Join(" ", parts.Skip(3));
                        _showRunAps.Add(new AccessPoint
                        {
                            MacAddress = mac,
                            Name = name,
                            IsChecked = false
                        });
                    }
                }
            }

            ShowRunFileLabel.Text = $"Show Run Config: {Path.GetFileName(filePath)}";
            UpdateCountLabel();
        }

        private void LoadApSummary_Click(object sender, RoutedEventArgs e)
        {
            var filePath = ShowFileDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            _apSummaryMacs.Clear();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"\b([0-9a-f]{4}\.[0-9a-f]{4}\.[0-9a-f]{4})\b", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var formattedMac = match.Groups[1].Value.Replace(".", "").ToLower();
                    _apSummaryMacs.Add(formattedMac);
                }
            }

            ApSummaryFileLabel.Text = $"AP Summary: {Path.GetFileName(filePath)}";
            UpdateCountLabel();
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showRunAps.Count == 0 || _apSummaryMacs.Count == 0)
            {
                MessageBox.Show("Please load both the Show Run Config and AP Summary files.", "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var inactiveAps = _showRunAps
                .Where(ap => !_apSummaryMacs.Contains(ap.MacAddress.ToLower()))
                .ToList();

            InactiveApDataGrid.ItemsSource = inactiveAps;
            GenerateDeleteCliButton.IsEnabled = inactiveAps.Any();
            UpdateCountLabel(inactiveAps.Count);
        }

        private void GenerateCli_Click(object sender, RoutedEventArgs e)
        {
            var selected = InactiveApDataGrid.ItemsSource as List<AccessPoint>;
            if (selected == null) return;

            var toDelete = selected.Where(ap => ap.IsChecked).ToList();
            if (!toDelete.Any())
            {
                MessageBox.Show("Please select at least one AP to delete.");
                return;
            }

            var cli = toDelete.Select(ap =>
                $"no username {ap.MacAddress}\nno ap {FormatMacDotNotation(ap.MacAddress)}");

            CliOutput.Text = string.Join("\n", cli);
        }

        private string FormatMacDotNotation(string mac)
        {
            mac = mac.Replace(":", "").Replace("-", "").ToLower();
            return $"{mac.Substring(0, 4)}.{mac.Substring(4, 4)}.{mac.Substring(8, 4)}";
        }

        private string ShowFileDialog()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (InactiveApDataGrid.ItemsSource is List<AccessPoint> list)
            {
                foreach (var ap in list) ap.IsChecked = true;
                InactiveApDataGrid.Items.Refresh();
            }
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            if (InactiveApDataGrid.ItemsSource is List<AccessPoint> list)
            {
                foreach (var ap in list) ap.IsChecked = false;
                InactiveApDataGrid.Items.Refresh();
            }
        }

        private void UpdateCountLabel(int inactiveCount = -1)
        {
            var total = _showRunAps.Count;
            var inactive = inactiveCount >= 0 ? inactiveCount : (InactiveApDataGrid.ItemsSource as List<AccessPoint>)?.Count ?? 0;
            CountSummaryLabel.Text = $"Total Configured: {total} | Inactive: {inactive}";
        }

        // Help > About Click Handler
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string aboutText = "WLC AP Cleaner\n" +
                               "Version 1.0.0\n" +
                               "Created by: Chris Staniforth\n\n" +
                               "License: MIT License\n\n" +
                               "Permission is hereby granted, free of charge, to any person obtaining a copy " +
                               "of this software and associated documentation files (the \"Software\"), to deal " +
                               "in the Software without restriction, including without limitation the rights " +
                               "to use, copy, modify, merge, publish, distribute, sublicense, and/or sell " +
                               "copies of the Software.";

            MessageBox.Show(aboutText, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class AccessPoint
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }


    }
