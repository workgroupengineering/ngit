using System;
using System.IO;

namespace Sharpen
{
    class FileHelper
    {
        public static FileHelper Instance {
            get; set;
        }
        
        static FileHelper ()
        {
            Instance = new FileHelper();
        }

        public virtual bool CanExecute (FilePath path)
        {
            return false;
        }

        public virtual bool CanWrite (FilePath path)
        {
            return ((File.GetAttributes (path) & FileAttributes.ReadOnly) == 0);
        }
        
        public virtual bool Delete (FilePath path)
        {
            if (Directory.Exists (path)) {
                if (Directory.GetFileSystemEntries (path).Length != 0)
                    return false;
                MakeDirWritable (path);
                Directory.Delete (path, true);
                return true;
            }
            else if (File.Exists(path)) {
                MakeFileWritable (path);
                File.Delete (path);
                return true;
            }
            return false;
        }
        
        public virtual bool Exists (FilePath path)
        {
            return (File.Exists (path) || Directory.Exists (path));
        }
        
        public virtual bool IsDirectory (FilePath path)
        {
            return Directory.Exists (path);
        }

        public virtual bool IsFile (FilePath path)
        {
            return File.Exists (path);
        }

        public virtual long LastModified (FilePath path)
        {
            if (IsFile(path)) {
                var info2 = new FileInfo(path);
                return info2.Exists ? info2.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
            } else if (IsDirectory (path)) {
                var info = new DirectoryInfo(path);
                return info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
            }
            return 0;
        }

        public virtual long Length (FilePath path)
        {
            // If you call .Length on a file that doesn't exist, an exception is thrown
            var info2 = new FileInfo (path);
            return info2.Exists ? info2.Length : 0;
        }

        public virtual void MakeDirWritable (FilePath path)
        {
            foreach (string file in Directory.GetFiles (path)) {
                MakeFileWritable (file);
            }
            foreach (string subdir in Directory.GetDirectories (path)) {
                MakeDirWritable (subdir);
            }
        }

        public virtual void MakeFileWritable (FilePath file)
        {
            FileAttributes fileAttributes = File.GetAttributes (file);
            if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
                fileAttributes &= ~FileAttributes.ReadOnly;
                File.SetAttributes (file, fileAttributes);
            }
        }

        public virtual bool RenameTo (FilePath path, string name)
        {
            try {
                File.Move (path, name);
                return true;
            } catch {
                return false;
            }
        }

        public virtual bool SetExecutable (FilePath path, bool exec)
        {
            return false;
        }

        public virtual bool SetReadOnly (FilePath path)
        {
            try {
                var fileAttributes = File.GetAttributes (path) | FileAttributes.ReadOnly;
                File.SetAttributes (path, fileAttributes);
                return true;
            } catch {
                return false;
            }
        }

        public virtual bool SetLastModified(FilePath path, long milis)
        {
            try {
                DateTime utcDateTime = Extensions.MillisToDateTimeOffset(milis, 0L).UtcDateTime;
                if (IsFile(path)) {
                    File.SetLastWriteTimeUtc(path, utcDateTime);
                    return true;
                } else if (IsDirectory(path)) {
                    Directory.SetLastWriteTimeUtc(path, utcDateTime);
                    return true;
                }
            } catch  {

            }
            return false;
        }
    }
}

