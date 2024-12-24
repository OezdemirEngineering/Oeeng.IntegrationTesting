using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using UiAutomationRecorder.Models;

namespace UiAutomationRecorder.Utils;

internal static class RecordFileManagerUtil
{
    /// <summary>
    /// Loads recorded actions from a file.
    /// </summary>
    /// <returns>A list of loaded actions.</returns>
    public static List<SerializableAutomationElementModel> LoadActionsFromFile()
    {
        List<SerializableAutomationElementModel> loadedActions = null;
        try
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Select a file with actions"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var json = System.IO.File.ReadAllText(filePath);
                loadedActions = JsonSerializer.Deserialize<List<SerializableAutomationElementModel>>(json);

                if (loadedActions != null)
                {
                    MessageBox.Show("Actions loaded successfully.");
                }
                else
                {
                    MessageBox.Show("Error loading actions.");
                }
            }
            else
            {
                MessageBox.Show("No file selected.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading actions: {ex.Message}");
        }

        return loadedActions;
    }

    /// <summary>
    /// Saves recorded actions to a file.
    /// </summary>
    /// <param name="models">The list of recorded actions to save.</param>
    public static void SaveActionsToFileDialog(List<SerializableAutomationElementModel> models)
    {
        try
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Save recorded actions",
                FileName = "recorded_actions.json" // Default file name
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                var json = JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, json);

                MessageBox.Show("Actions saved successfully.");
            }
            else
            {
                MessageBox.Show("Save canceled.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving actions: {ex.Message}");
        }
    }
}

