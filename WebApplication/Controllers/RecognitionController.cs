using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RecognitionOfPassports;

namespace WebApplication.Controllers
{
    public class RecognitionController : ApiController
    {
        private const string TempPath = @"C:\Users\Artem\Desktop\RecognitionOfPassports1\RecognitionOfPassports\data\temp.jpg";
        private const string ResultImgPath = @"C:\Users\Artem\Desktop\passp\website\public\static_content\result.jpg";
        private const string RecognitionResult = @"C:\Users\Artem\Desktop\RecognitionOfPassports1\RecognitionOfPassports\data\result.txt";

        [Route("api/recognizeImage"), HttpPost]
        public async Task<IEnumerable<string>> Recognize()
        {
            var content = await Request.Content.ReadAsMultipartAsync();

            var fileContent = content.Contents.First();

            var stream = await fileContent.ReadAsStreamAsync();

            using (var fileStream = File.Create(TempPath))
            {
                using (var inputStream = stream)
                {
                    inputStream.CopyTo(fileStream);

                    fileStream.Close();
                    inputStream.Close();
                }
            }

            var form = new Form1();

            var img = form.открытьToolStripMenuItem_Click(TempPath, RecognitionResult);
            img.Save(ResultImgPath);

            return File.ReadAllLines(RecognitionResult);
        }
    }
}
