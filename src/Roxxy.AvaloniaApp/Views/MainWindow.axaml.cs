using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Roxxy.AvaloniaApp.ViewModels;

namespace Roxxy.AvaloniaApp.Views;

public partial class MainWindow : Window, IFileSaving
{
    public MainWindow()
    {
        InitializeComponent();
        this.KeyDown += HandleKeyPress;
    }

    private void HandleKeyPress(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            this.Close();
        }
    }

    public async Task SaveFile(string sourcePath, string fileTypeName = "JPEG image", string extension = ".jpg", string title = "Save Image")
    {
        var file = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            DefaultExtension = extension,
            FileTypeChoices = new [] {
                new FilePickerFileType(fileTypeName)
                {
                    Patterns = new [] { $"*{extension}" },
                }
            }
            /* FileTypeChoices here could be instead of separate buttons? */ 
        });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            
            using var sourceStream = File.OpenRead(sourcePath);
            
            await sourceStream.CopyToAsync(stream);
        }
    }
}
