using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Squora.Forms.Core.Helpers;
using Squora.Forms.Core.Models;

namespace Squora.Forms.Blazor.Components.Input
{
    /// <summary>
    /// Shared logic of the FileUpload components. The UI packages only render the picker and hand the chosen files to
    /// <see cref="AddFilesAsync(IEnumerable{IBrowserFile})"/>; reading, Base64 encoding, limits and writing the value happen here.
    /// The value is an <see cref="UploadedFile"/> when <see cref="Multiple"/> is false and a list of them when it is true.
    /// </summary>
    public class FileUploadComponentBase : InputFormComponent
    {
        public const long DefaultMaximumFileSize = 5 * 1024 * 1024;

        [Parameter]
        public string? Label { get; set; }

        /// <summary>Allow more than one file. The value then is a list of files.</summary>
        [Parameter]
        public bool Multiple { get; set; }

        /// <summary>Comma separated file types for the picker, like <c>.pdf,image/*</c>. Empty allows everything.</summary>
        [Parameter]
        public string? Accept { get; set; }

        /// <summary>Largest allowed size of one file, in bytes.</summary>
        [Parameter]
        public long MaximumFileSize { get; set; } = DefaultMaximumFileSize;

        /// <summary>Largest allowed number of files. Only used when <see cref="Multiple"/> is true; 0 means no limit.</summary>
        [Parameter]
        public int MaximumFiles { get; set; }

        private List<UploadedFile> _files = new();
        private string? _notice;

        public IReadOnlyList<UploadedFile> Files => _files;

        /// <summary>
        /// What went wrong with the last pick (a file too large, too many files). Files that were turned away never reach the
        /// data, so this is only a message; it does not make the field invalid and it clears on the next pick, removal or clear.
        /// </summary>
        public string? Notice => _notice;

        public override string ValidationString => InvalidMessage ?? ProblemWithFiles() ?? "Invalid file";

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            try
            {
                _files = UploadedFileConverter.ToFiles(GetValue());
            }
            catch (Exception)
            {
                _files = new List<UploadedFile>();
                IsValid = false;
            }
        }

        public Task AddFilesAsync(IEnumerable<IBrowserFile> files) =>
            AddFilesAsync(files.Select(f => new FileSource(f.Name, f.ContentType, f.Size, max => f.OpenReadStream(max))));

        /// <summary>For pickers that do not hand out an <see cref="IBrowserFile"/>.</summary>
        public async Task AddFilesAsync(IEnumerable<FileSource> sources)
        {
            _notice = null;
            var added = new List<UploadedFile>();

            foreach (var source in sources)
            {
                if (source.Size > MaximumFileSize)
                {
                    _notice = $"{source.Name} is larger than {FormatSize(MaximumFileSize)}.";
                    continue;
                }

                try
                {
                    using var stream = source.OpenRead(MaximumFileSize);
                    using var buffer = new MemoryStream();
                    await stream.CopyToAsync(buffer);
                    added.Add(UploadedFile.FromBytes(source.Name, source.ContentType, buffer.ToArray()));
                }
                catch (IOException)
                {
                    _notice = $"{source.Name} could not be read.";
                }
            }

            if (added.Count == 0)
            {
                return;
            }

            var result = Multiple ? _files.Concat(added).ToList() : new List<UploadedFile> { added[^1] };
            if (Multiple && MaximumFiles > 0 && result.Count > MaximumFiles)
            {
                _notice = $"At most {MaximumFiles} files are allowed.";
                result = result.Take(MaximumFiles).ToList();
            }

            SetFiles(result);
        }

        public void RemoveFile(int index)
        {
            if (index < 0 || index >= _files.Count) return;
            _notice = null;
            var result = _files.ToList();
            result.RemoveAt(index);
            SetFiles(result);
        }

        public void Clear()
        {
            _notice = null;
            SetFiles(new List<UploadedFile>());
        }

        private void SetFiles(List<UploadedFile> files)
        {
            _files = files;
            ComponentLogic.UpdateValue(this, (Multiple ? files : files.FirstOrDefault())!);
            Validate();
        }

        public override void Validate()
        {
            IsValid = ProblemWithFiles() == null;
            base.Validate();
        }

        // Judged on the files in the data, so a form loaded with too many or too large files is invalid until that is fixed.
        private string? ProblemWithFiles()
        {
            if (Required && _files.Count == 0) return "Select a file";
            if (Multiple && MaximumFiles > 0 && _files.Count > MaximumFiles) return $"At most {MaximumFiles} files are allowed.";
            var tooLarge = _files.FirstOrDefault(f => f.Size > MaximumFileSize);
            return tooLarge != null ? $"{tooLarge.Name} is larger than {FormatSize(MaximumFileSize)}." : null;
        }

        public static string FormatSize(long bytes) => bytes switch
        {
            >= 1024 * 1024 => $"{bytes / (1024.0 * 1024.0):0.#} MB",
            >= 1024 => $"{bytes / 1024.0:0.#} KB",
            _ => $"{bytes} B"
        };
    }

    public record FileSource(string Name, string ContentType, long Size, Func<long, Stream> OpenRead);
}
