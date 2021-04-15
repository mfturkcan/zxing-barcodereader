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

namespace zxingdecoder
{
    public class QRReader
    {
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
                    barcodeReader.Options.ReturnCodabarStartEnd = true;
                    SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imagePath);
                    image.Mutate(img => img.Resize(image.Width * 3, image.Height * 2)
                         .GaussianSharpen(sigma: 4.0f)
                         .Grayscale());
                    
                    qrCodeBitMap = ToBitmap(image);
                    qrcoderesult = barcodeReader.Decode(qrCodeBitMap);

                    Console.WriteLine($"\tText : {qrcoderesult.Text ?? "Null"}");
                   

                    MultiFormatReader multiFormatReader = new MultiFormatReader();
                    

                    ZXing.BinaryBitmap multiBitmap;

                    return qrcoderesult!=null?
                    qrcoderesult.Text:
                    
                    multiBitmap = new BinaryBitmap(new HybridBinarizer(new BitmapLuminanceSource(qrCodeBitMap)));
                    qrcoderesult = multiFormatReader.decode(multiBitmap);
                    Console.WriteLine($"\t Multiple part of Text : {qrcoderesult.Text ?? "Null"}");
                    return qrcoderesult;
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

