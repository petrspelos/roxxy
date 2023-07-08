using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.IO;
using ReactiveUI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace Roxxy.AvaloniaApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<string, Unit> SaveImageCommand { get; }

    public ReactiveCommand<string, Unit> CopyImageCommand { get; }

    private Bitmap _image;
    public Bitmap Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public string Title { get; set; } = "Roxxy - Loading...";

    public string Resolution { get; set; } = string.Empty;

    public string FramesCount { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    private Image<Rgba32>? _sourceWebp;

    private readonly IFileSaving _fileSaving;

    public MainWindowViewModel(IFileSaving fileSaving)
    {
        _fileSaving = fileSaving;
        var args = Environment.GetCommandLineArgs();
        SaveImageCommand = ReactiveCommand.CreateFromTask<string>(ExecuteSaveFileCommand);
        CopyImageCommand = ReactiveCommand.CreateFromTask<string>(ExecuteCopyFileCommand);
        if(args.Length > 1)
        {
            LoadImage(args[1]);
        }
    }

    private async Task ExecuteSaveFileCommand(string extension)
    {
        var tempFile = Path.GetTempFileName();
        File.Delete(tempFile);
        var tempImg = Path.ChangeExtension(tempFile, extension);

        switch (extension)
        {
            case ".jpg":
                _sourceWebp.SaveAsJpeg(tempImg);
                await _fileSaving.SaveFile(tempImg);
                break;
            case ".png":
                _sourceWebp.SaveAsPng(tempImg);
                await _fileSaving.SaveFile(tempImg, "PNG image", ".png");
                break;
            case ".bmp":
                _sourceWebp.SaveAsBmp(tempImg);
                await _fileSaving.SaveFile(tempImg, "Bitmap Image", ".bmp");
                break;
            case ".tga":
                _sourceWebp.SaveAsTga(tempImg);
                await _fileSaving.SaveFile(tempImg, "TGA Image", ".tga");
                break;
            default:
                throw new InvalidOperationException($"unknown extension {extension}");
        }
    }

    private async Task ExecuteCopyFileCommand(string extension)
    {
        var tempFile = Path.GetTempFileName();
        File.Delete(tempFile);
        var tempImg = Path.ChangeExtension(tempFile, extension);

        switch (extension)
        {
            case ".jpg":
                _sourceWebp.SaveAsJpeg(tempImg);
                break;
            case ".png":
                _sourceWebp.SaveAsPng(tempImg);
                break;
            case ".bmp":
                _sourceWebp.SaveAsBmp(tempImg);
                break;
            case ".tga":
                _sourceWebp.SaveAsTga(tempImg);
                break;
            default:
                throw new InvalidOperationException($"unknown extension {extension}");
        }

        CopyImageToClipboard(tempImg);
    }

    private void LoadImage(string path)
    {
        FilePath = path;
        _sourceWebp = SixLabors.ImageSharp.Image.Load<Rgba32>(path);
        var memoryStream = new MemoryStream();

        Resolution = $"{_sourceWebp.Width}x{_sourceWebp.Height}";
        FramesCount = _sourceWebp.Frames.Count.ToString();

        _sourceWebp.SaveAsBmp(memoryStream);
        memoryStream.Position = 0;

        Image = new Bitmap(memoryStream);
        Title = $"Roxxy - {Path.GetFileName(path)}";
    }

    private void CopyImageToClipboard(string imagePath)
    {
        Process proc = new Process();
        proc.StartInfo.FileName = "imgtoclip.exe";
        proc.StartInfo.Arguments = imagePath;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.EnableRaisingEvents = true;
        proc.Start();
    }
}
