namespace Entities.Entities;

public class FileRecord
{
    public FileInfo FileInfo { get; init; }
    public string Text { get; private set; }

    public FileRecord(FileInfo fileInfo)
    {
        FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
    }

    public FileRecord(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException($"'{nameof(filePath)}' cannot be null or whitespace.", nameof(filePath));
        FileInfo = new FileInfo(filePath);
    }

    public async Task ReadAsync()
    {
        this.Refresh();
        using StreamReader streamReader = FileInfo.OpenText();
        Text = await streamReader.ReadToEndAsync();
    }

    public void Delete()
    {
        this.Refresh();
        FileInfo.Delete();
    }

    public void Refresh() => FileInfo.Refresh();
}
