using System.IO;
using System.Text;
using NUnit.Framework;

namespace NGit.Api
{
    public class ApplyCommandLineEndingsTests : RepositoryTestCase
    {
        // This test covers the case where Patch files where being parsed twice for windows line endings meaning a\r\r\nb would be converted to [a,b]
        // But the original file would be parsed as [a\r,b], so when being compared during apply it would detect the change and error
        // We had 3 possible fixes
        //  1 - Ignore trailing \r in ApplyCommand.cs (This just plasters over the issue of parsing being different)
        //  2 - Don't remove the \r during read and instead understand line windows line endings in the patch parsing (See https://github.com/red-gate/ngit/pull/17 for the change)
        //  3 - Convert all Windows Line Endings to Unix whenever we parse input. This is a much larger change, but would mean we can always assume Unix line endings, making parsing of the input easier.


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