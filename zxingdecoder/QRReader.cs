using System;
using ZXing;
using System.IO;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using ZXing.Multi.QrCode;
using ZXing.Common;
using System.Collections;




namespace zxingdecoder
{
    

    public class QRReader
    {
        public Test[] test_variables = {
            new Test(width:2,sigma:2.0f),
            new Test(width:2,sigma:2.5f),
            new Test(width:2,sigma:3.0f),
            new Test(width:3,sigma:2.0f),
            new Test(width:3,sigma:2.5f),
            new Test(width:3,sigma:3.0f),
            new Test(width:4,sigma:2.0f),
            new Test(width:4,sigma:2.5f),
            new Test(width:4,sigma:3.0f),
        };

        public ArrayList list = new ArrayList(){};

        private string imagePath;

        public string ImagePath{
            get { return imagePath; }
            set { imagePath = value; }
        }

        private static QRReader qrReader = null;


        public static QRReader getInstance(string image)
        {

            if (qrReader == null)
            {
                qrReader = new QRReader(image);
            }

            return qrReader;

        }

        public dynamic ReadBarcode()
        {
            try
            {
                var qrCodeBitMap = (Bitmap)System.Drawing.Image.FromFile(imagePath);

                
                var barcodeReader = new BarcodeReader();
                barcodeReader.Options.TryHarder = true;
                barcodeReader.AutoRotate = true;
                barcodeReader.TryInverted = true;
                

                var qrcoderesult = barcodeReader.Decode(qrCodeBitMap);
                
                
                if (qrcoderesult != null)
                {
                    Console.WriteLine($"\tText : {qrcoderesult.Text ?? "Null"}");
                    return qrcoderesult.Text;
                }

                else
                {
                    foreach(Test test in test_variables)
                    {
                        qrcoderesult = tryExperiment(qrcoderesult: qrcoderesult,
                            qrCodeBitMap: qrCodeBitMap,
                            barcodeReader: barcodeReader,
                            width_height: test.widthFactor, sigma: test.sigmaFactor);

                        if (qrcoderesult != null)
                        {
                            list.Add(test);
                            return qrcoderesult.Text;
                        }
                    }

                }
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("File not found!");
                
            }
            catch(NullReferenceException nullBarCode)
            {
                Console.WriteLine("Barcode is not readable or null!");
                
            }
            return null;
        }


        private QRReader(string image)
        {
            imagePath = image;
        }


        public static System.Drawing.Bitmap ToBitmap(SixLabors.ImageSharp.Image image) 
        {
            using (var memoryStream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                image.Save(memoryStream, imageEncoder);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return new System.Drawing.Bitmap(memoryStream);
            }
            

        }

        public dynamic tryExperiment(dynamic qrcoderesult, dynamic qrCodeBitMap, dynamic barcodeReader, int width_height, float sigma)
        {
            try
            {
                barcodeReader.Options.ReturnCodabarStartEnd = true;
                SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imagePath);
                image.Mutate(img => img.Resize(image.Width * width_height, image.Height * width_height)
                             .GaussianSharpen(sigma: sigma)
                             .Grayscale());

                qrCodeBitMap = ToBitmap(image);
                qrcoderesult = barcodeReader.Decode(qrCodeBitMap);

                if (qrcoderesult != null)
                {
                    Console.WriteLine($"\tText : {qrcoderesult.Text ?? "Null"}");
                    return qrcoderesult;
                }
   
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
            }
}

}

/*
                    ZXing.Result[] results = { };

                    if(qrcoderesult == null)
                    {
                        results = barcodeReader.DecodeMultiple(qrCodeBitMap);

                        if (results != null)
                        {
                            foreach (var result in results)
                            {
                                Console.WriteLine($"\tText : {result.Text ?? "Null"}");
                            }
                            return results;
                        }
                        return null;

                    }


 */

