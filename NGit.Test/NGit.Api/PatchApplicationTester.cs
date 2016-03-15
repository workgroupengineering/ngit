using NGit.Diff;
using NGit.Junit;
using NGit.Test.NGit.Util.IO;
using Sharpen;

namespace NGit.Api
{
    public class PatchApplicationTester
    {
        public RawText a { get; private set; }
        public RawText b { get; private set; }
        public Repository db { get; }
        private readonly bool useCrlfFiles;
        private readonly bool useCrlfPatches;

        public PatchApplicationTester(Repository db, bool useCrlfPatches = false, bool useCrlfFiles = false)
        {
            this.db = db;
            this.useCrlfFiles = useCrlfFiles;
            this.useCrlfPatches = useCrlfPatches;
        }

        /// <exception cref="System.Exception"></exception>
        public ApplyResult Init(string name, bool preExists = true, bool postExists = true)
        {
            Git git = new Git(db);
            if (preExists)
            {
                a = new RawText(ReadFile(name + "_PreImage", useCrlfFiles));
                JGitTestUtil.Write(new FilePath(db.Directory.GetParent(), name), a.GetString(0, a.Size(), false
                    ));
                git.Add().AddFilepattern(name).Call();
                git.Commit().SetMessage("PreImage").Call();
            }
            if (postExists)
            {
                bool postShouldHaveCrlf = preExists && a.GetLineDelimiter() != null ? a.GetLineDelimiter() != "\n" : useCrlfPatches;
                b = new RawText(ReadFile(name + "_PostImage", postShouldHaveCrlf));
            }
            return git.Apply().SetPatch(typeof(DiffFormatterReflowTest).GetResourceAsStream(name + ".patch", useCrlfPatches)).Call();
        }

        /// <exception cref="System.IO.IOException"></exception>
        private byte[] ReadFile(string patchFile, bool useCrlfFiles)
        {
            InputStream @in = typeof(DiffFormatterReflowTest).GetResourceAsStream(patchFile, useCrlfFiles);
            if (@in == null)
            {
                NUnit.Framework.Assert.Fail("No " + patchFile + " test vector");
                return null;
            }
            // Never happens
            try
            {
                byte[] buf = new byte[1024];
                ByteArrayOutputStream temp = new ByteArrayOutputStream();
                int n;
                while ((n = @in.Read(buf)) > 0)
                {
                    temp.Write(buf, 0, n);
                }
                return temp.ToByteArray();
            }
            finally
            {
                @in.Close();
            }
        }
    }
}