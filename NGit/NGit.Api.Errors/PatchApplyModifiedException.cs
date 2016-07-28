using System;

namespace NGit.Api.Errors
{
    public sealed class PatchApplyModifiedException : Exception
    {
        public string FilePath { get; }
        public string Hunk { get; }
        public string HunkLine { get; }
        public string LineToReplace { get; }

        public PatchApplyModifiedException(string filePath, string hunk, string hunkLine, string lineToReplace)
        {
            FilePath = filePath;
            Hunk = hunk;
            HunkLine = hunkLine;
            LineToReplace = lineToReplace;
        }
    }
}