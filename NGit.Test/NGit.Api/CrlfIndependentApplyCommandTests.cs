using System.Runtime.InteropServices;
using NGit.Diff;
using NUnit.Framework;
using Sharpen;

namespace NGit.Api
{
    public class CrlfIndependentApplyCommandTests : RepositoryTestCase
    {
        private PatchApplicationTester m_PatchApplicationTester;
        public RawText a => m_PatchApplicationTester.a;
        public RawText b => m_PatchApplicationTester.b;

        private ApplyResult Init()
        {
            m_PatchApplicationTester = new PatchApplicationTester(db);
            return m_PatchApplicationTester.Init("FileCasing");
        }

        [Test]
        public void PatchesContainingModifiedFilesWithDifferentFileNameCasingCanBeApplied()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Ignore("This doesn't make sense on a case-sensitive file system.");
            }
            ApplyResult result = Init();
            NUnit.Framework.Assert.AreEqual(2, result.GetUpdatedFiles().Count);
            CheckFile(new FilePath(db.WorkTree, "FILECASING"), b.GetString(0, b.Size(), false));
        }
    }
}