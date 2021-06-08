using System;
using System.Drawing;
using System.Threading;
using Parcs;

namespace FirstModule
{
    public class IntegralModule: IModule
    {


        private static Bitmap Blur(Bitmap image, Int32 blurSize, Int32 partOfImage, Int32 total)
        {
            int widthOfPart = image.Width / total;
            int x = partOfImage*widthOfPart;
            //Console.WriteLine(x);
            if (x > blurSize)
            {
                x -= blurSize;
                widthOfPart += blurSize;
            }
            return Blur(image, new Rectangle(x, 0, widthOfPart, image.Height), blurSize);
        }

        private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            Console.WriteLine(rectangle.X + " " + rectangle.Y);
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    for (int x = xx; x < xx + blurSize && x < image.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }

        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            Bitmap image = info.Parent.ReadObject<Bitmap>();
            int blurSize = info.Parent.ReadInt();
            int partOfImage = info.Parent.ReadInt();
            int totalParts = info.Parent.ReadInt();

            Bitmap res = Blur(image, blurSize, partOfImage, totalParts);
            Console.WriteLine("Finished");
            info.Parent.WriteObject(res);
        }
    }
}
