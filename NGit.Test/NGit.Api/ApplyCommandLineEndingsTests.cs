using System.IO;
using NUnit.Framework;

namespace NGit.Api
{
    public class ApplyCommandLineEndingsTests : RepositoryTestCase
    {
        [Test]
        public void ApplyingPatchWithInconsistentMacLineEndingsIsSuccessfullyApplied()
        {
            const string pre = "a\r\r\nb";
            const string post = "a\r\nb";
            var git = new Git(db);
            var stream = new MemoryStream();
            var modifiedFile = Path.Combine(db.WorkTree, "file.txt");

            File.WriteAllText(modifiedFile, pre);
            git.Add().AddFilepattern(".").Call();
            git.Commit().SetMessage("Initial commit").Call();

            File.WriteAllText(modifiedFile, post);
            git.Diff().SetOutputStream(stream).Call();

            stream.Position = 0;
            File.WriteAllText(modifiedFile, pre);

            git.Apply().SetPatch(stream).Call();

            Assert.That(File.ReadAllText(modifiedFile), Is.EqualTo(post));
        }
    }
}