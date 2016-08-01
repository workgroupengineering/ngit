using System.IO;
using System.Text;
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
            var modifiedFile = Path.Combine(db.WorkTree, "file.txt");

            File.WriteAllText(modifiedFile, pre);
            git.Add().AddFilepattern(".").Call();
            git.Commit().SetMessage("Initial commit").Call();

            var patch = GetPatch(modifiedFile, pre, post, git);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(patch)))
            {
                git.Apply().SetPatch(stream).Call();
            }

            Assert.That(File.ReadAllText(modifiedFile), Is.EqualTo(post));
        }

        private static string GetPatch(string modifiedFile, string originalText, string newText, Git git)
        {
            using (var stream = new MemoryStream())
            {
                File.WriteAllText(modifiedFile, newText);
                git.Diff().SetOutputStream(stream).Call();
                
                File.WriteAllText(modifiedFile, originalText);

                return Encoding.ASCII.GetString(stream.ToArray());
            }
        }
    }
}