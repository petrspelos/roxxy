using System.Threading.Tasks;

namespace Roxxy.AvaloniaApp;

public interface IFileSaving
{
    Task SaveFile(string sourcePath, string fileTypeName = "JPEG image", string extension = ".jpg", string title = "Save Image");
}
