using System;
using ZXing;
using System.Drawing;
using System.IO;

namespace zxingdecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"/Users/mfturkcanoglu/Downloads/mini-foto";

            var files = System.IO.Directory.GetFiles(path);

            QRReader reader = QRReader.getInstance(files[0]);

            int count = 0;
            for (int i= 0;i<50;i++)
            {

                Console.WriteLine($"Path : {files[i]}");


                reader.ImagePath = $@"{files[i]}";
               

                Console.Write($"#: {i}");
                var result = reader.ReadBarcode();
                if (result != null) count += 1;
                
                Console.WriteLine("********");
            }

            foreach(Test test in reader.list)
            {
                Console.WriteLine($"width : {test.widthFactor} || sigma :{test.sigmaFactor}");
            }

            Console.WriteLine($"Truths Count : {count}");
        }
    }
}
