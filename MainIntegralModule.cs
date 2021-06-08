using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Parcs;
using System.Linq;

namespace FirstModule
{
    class MainIntegralModule: IModule
    {
        public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }
        public static void Main(string[] args)
        {
            
            var job = new Job();
            if (!job.AddFile(Assembly.GetExecutingAssembly().Location/*"MyFirstModule.exe"*/))
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            new MainIntegralModule().Run(new ModuleInfo(job, null));
            Console.ReadKey();
        }

        public void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {
            Bitmap bitmap = new Bitmap("input.png");
            var pointsNum = 1;
            var points = new IPoint[pointsNum];
            var channels = new IChannel[pointsNum];
            var bmres = new Bitmap(bitmap);
            int widthOfPart = bitmap.Width / pointsNum;
            Console.WriteLine(widthOfPart);
            int h = bitmap.Height;
            for (int i = 0; i < pointsNum; ++i)
            {
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("FirstModule.IntegralModule");
            }

            
            for (int i = 0; i < pointsNum; ++i)
            {
                channels[i].WriteObject(new Bitmap(bitmap));
                channels[i].WriteData(5);
                channels[i].WriteData(i);
                channels[i].WriteData(pointsNum);
            }
            DateTime time = DateTime.Now;            
            Console.WriteLine("Waiting for result...");
            
            var bmps = channels.Select(c => new Lazy<Bitmap>(c.ReadObject<Bitmap>)).ToArray();
            
            Console.WriteLine("Blurred time = {0}", Math.Round((DateTime.Now - time).TotalSeconds,3));

            
            for (int i = 0; i < pointsNum; ++i)
            {
                int x = i * widthOfPart;
                var region = new Rectangle(x, 0, widthOfPart, h);
                Console.WriteLine("merge close to done");
                //bmps[i].Value.Save("output" + ".png");
                CopyRegionIntoImage(bmps[i].Value, region, ref bmres, region);
                //Console.WriteLine(bmps[i].Value.Height);
            }
            bmres.Save("output.png");
            Console.WriteLine("Merged time = {0}", Math.Round((DateTime.Now - time).TotalSeconds, 3));
        }
    }
}
