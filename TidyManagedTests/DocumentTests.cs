using Microsoft.VisualStudio.TestTools.UnitTesting;
using Palaso.IO;
using System.IO;
using System.Text;
using TidyManaged;

namespace TidyManagedTests
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public void Save_UsingString_RoundTripsUtf8String()
        {
            // ŋ --> ?
            // β (03B2) --> &#65533;
            using (var tidy = TidyManaged.Document.FromString("<!DOCTYPE html><html><head> <meta charset='UTF-8'></head><body>ŋ β</body></html>"))
            {
                tidy.InputCharacterEncoding = EncodingType.Utf8;
                tidy.OutputCharacterEncoding = EncodingType.Utf8;

                tidy.CleanAndRepair();
                using (var temp = new TempFile())
                {
                    tidy.Save(temp.Path);
                    var newContents = File.ReadAllText(temp.Path, Encoding.UTF8);
                    Assert.IsTrue(newContents.Contains("ŋ"), newContents);
                }
            }
        }

        [TestMethod]
        public void Save_UsingFile_RoundTripsUtf8File()
        {
            // ŋ (velar nasal)--> &#331;
            // β (greek beta) (03B2) --> &#946;
            using (var input = TempFile.CreateAndGetPathButDontMakeTheFile())
            {
                var source = "<!DOCTYPE html><html><head> <meta charset='UTF-8'></head><body>ŋ β</body></html>";
                File.WriteAllText(input.Path, source, Encoding.UTF8);
                using (var tidy = TidyManaged.Document.FromFile(input.Path))
                {
                    //tidy.CharacterEncoding = EncodingType.Utf8;
                    tidy.InputCharacterEncoding = EncodingType.Utf8; ;
                    tidy.OutputCharacterEncoding = EncodingType.Utf8;
                    tidy.CleanAndRepair();
                    using (var output = new TempFile())
                    {
                        tidy.Save(output.Path);
                        var newContents = File.ReadAllText(output.Path, Encoding.UTF8);
                        Assert.IsTrue(newContents.Contains("ŋ"), newContents);
                    }
                }
            }
        }

        [TestMethod]
        public void Save_UsingStream_RoundTripsUtf8File()
        {
            // ŋ (velar nasal)--> &#331;
            // β (greek beta) (03B2) --> &#946;
            using (var input = TempFile.CreateAndGetPathButDontMakeTheFile())
            {
                var source = "<!DOCTYPE html><html><head> <meta charset='UTF-8'></head><body>ŋ β</body></html>";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(source)))
                using (var tidy = TidyManaged.Document.FromStream(stream))
                {
                    tidy.CharacterEncoding = EncodingType.Utf8;
                    //					tidy.InputCharacterEncoding = EncodingType.Utf8; ;
                    //					tidy.OutputCharacterEncoding = EncodingType.Utf8;

                    tidy.CleanAndRepair();
                    using (var output = new TempFile())
                    {
                        tidy.Save(output.Path);
                        var newContents = File.ReadAllText(output.Path);
                        Assert.IsTrue(newContents.Contains("ŋ"), newContents);
                    }
                }
            }
        }
    }
}