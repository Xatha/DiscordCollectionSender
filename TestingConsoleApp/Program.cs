using BenchmarkDotNet.Running;
using CompressionLibrary;
using Discord;
using DiscordCollectionSenderBot.MessageService.Message;
using log4net;
using log4net.Config;
using nQuant;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Text;
using TestingConsoleApp;
using static System.Net.Mime.MediaTypeNames;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
class Program
{
    private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

    static void Main()
    {

        var container = new FactoryContainer();
        container.Register<TestClass, ITestClass>();
        container.RegisterTypeS<int, IArgumentProvider>();
        var myClass = container.GetImplementation<ITestClass>();
        myClass.HelloWorld();

        //var sameClassAgain = container.GetImplemnationCached<ITestClass>();
        //sameClassAgain.HelloWorld();
        //await Benchy.InitLogin();

        //logger.Info("xD");
        //Benchy.InitContainer();

        //const int iterations = 1_000_000;
        //var stopwatch = Stopwatch.StartNew();
        //for (int i = 0; i < iterations; i++)
        //{
        //    var myClass = new TestClass();
        //    myClass.HelloWorld();
        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[Newing] Time Taken {stopwatch.ElapsedTicks}ms");

        //stopwatch.Restart();
        //for (int i = 0; i < iterations; i++)
        //{
        //    var myClass = container.GetImplementation<ITestClass>();
        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[GetImplementation] Time Taken {stopwatch.ElapsedMilliseconds}ms");

        //stopwatch.Restart();
        //for (int i = 0; i < iterations; i++)
        //{
        //    var myClass = container.GetImplemnationCached<ITestClass>();
        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[GetImplemnationCached] Time Taken {stopwatch.ElapsedMilliseconds}ms");

        //stopwatch.Restart();
        //for (int i = 0; i < iterations; i++)
        //{
        //    //var myClass = container.GetImplementationLambda<ITestClass>();

        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[GetImplementationLambda] Time Taken {stopwatch.ElapsedMilliseconds}ms");

        //stopwatch.Restart();
        //for (int i = 0; i < iterations; i++)
        //{
        //    //var myClass = container.GetImplemnationLambaFast<ITestClass>();
        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[GetImplemnationLambaFast] Time Taken {stopwatch.ElapsedMilliseconds}ms");

        //for (int i = 0; i < iterations; i++)
        //{
        //    var myClass = container.GetImplemnationInvoke<ITestClass>();
        //}
        //stopwatch.Stop();
        //Console.WriteLine($"[GetImplemnationInvoke] Time Taken {stopwatch.ElapsedTicks}ms");

        BenchmarkRunner.Run<Benchy>();




    }

    internal static bool AreImagePathsValidAsync(List<string> pathsOfImages)
    {
        bool result;
        result = pathsOfImages?.Any() ?? default;


        return result;
    }

    private static void ArgumentNullExceptionTest(string spookyVariable)
    {
        Console.WriteLine("Start of func!");

        _ = spookyVariable ?? throw new ArgumentNullException(nameof(spookyVariable));

        Console.WriteLine("End of func!");
    }

    void CompressPhoto()
    {
        //User uses slash command and attaches .zip file. Or download link. 

        //DownloadZip

        //Extract ZIP to testZipDest, and delete the .zip file

        //Get all fileNames in directory

        //If file is >8mb, compress it depending on size. Preferably create algorithm that compresses it so =8mb.

        //Queue up files that can be uploaded. Delete the files that have been uploaded.

        //Send each message as in the channel the slash command was used in.

        //Do additional cleanup if needed.

        try
        {
            //ZipFile.ExtractToDirectory(@"C:\Users\Luca\Desktop\testFolder\testZIP.zip", @"C:\Users\Luca\Desktop\testFolder\testZIPDest");
        }
        catch (Exception)
        {
            Console.WriteLine("Already Extracted");
        }

        List<string> fileNames = new List<string>(Directory.GetFiles(@"C:\Users\Luca\Desktop\testFolder\testZIPDest"));

        foreach (var fileName in fileNames)
        {
            Console.WriteLine(fileName);
        }

        List<FileInfo> fileInfo = new List<FileInfo>();

        foreach (var file in fileNames)
        {
            fileInfo.Add(new FileInfo(file));
        }


        LoggerLibrary.Logger logger = new LoggerLibrary.Logger();

        ResponseCallback response = new ResponseCallback();
        response.FilePath.ProgressChanged += FilePath_ProgressChanged;

        //Task.Run(() => CompressionMaster.InitAsync(fileInfo, response)).Wait();


        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);
        logger.Log(Task.Run(() => RandomStringAsync()).Result);

        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());
        logger.Log(GenerateFileDirectory());

        FileInfo fi = new FileInfo(@"C:\Users\Luca\Desktop\testFolder\orig1.jpg");
        long fiSize = fi.Length;
        Console.WriteLine(fiSize);

        var img = System.Drawing.Image.FromFile(@"C:\Users\Luca\Desktop\testFolder\original.png");
        img = ScaleImage(img, 2000);

        var img1 = System.Drawing.Image.FromFile(@"C:\Users\Luca\Desktop\testFolder\orig1.jpg");
        var img2 = System.Drawing.Image.FromFile(@"C:\Users\Luca\Desktop\testFolder\orig2.jpg");
        var img3 = System.Drawing.Image.FromFile(@"C:\Users\Luca\Desktop\testFolder\orig3.jpg");

        var imgBPP = new Bitmap(img);
        var img1BPP = ConvertTo16bpp(img1);
        var img2BPP = ConvertTo16bpp(img2);
        var img3BPP = ConvertTo16bpp(img3);

        //imgBPP.Save(@"C:\Users\Luca\Desktop\testFolder\original16BppBMP.png", ImageFormat.Png);
        //img1BPP.Save(@"C:\Users\Luca\Desktop\testFolder\orig116BppBMP.png", ImageFormat.Png);
        //img2BPP.Save(@"C:\Users\Luca\Desktop\testFolder\orig216BppBMP.png", ImageFormat.Png);
        //img3BPP.Save(@"C:\Users\Luca\Desktop\testFolder\orig316BppBMP.png", ImageFormat.Png);

        if (img == null)
        {
            Console.WriteLine("Value of img is: null");
            return;
        }

        //var bmp = new Bitmap(img, img.Width, img.Height);
        ////img.Save(@"C:\Users\Luca\Desktop\testFolder\compressedIMG.png");
        //bmp.Save(@"C:\Users\Luca\Desktop\testFolder\compressedBMP.png", ImageFormat.Png);

        //var qt = new WuQuantizer();

        //using(var quantized = qt.QuantizeImage(bmp))
        //{

        //    quantized.Save(@"C:\Users\Luca\Desktop\testFolder\compressed.png", ImageFormat.Png);
        //}

        System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\Luca\Desktop\testFolder\testZIPDest");
        foreach (FileInfo file in di.EnumerateFiles())
        {
            //file.Delete();

        }
    }

    static string GenerateFileDirectory()
    {
        var directoryName = @"C:\Users\Luca\Desktop\testFolder\";
        var fileName = Task.Run(() => @RandomStringAsync()).Result;
        var fileExtension = @".zip";

        var fullPath = new @StringBuilder().Append(directoryName).Append(fileName).Append(fileExtension).ToString();
        return fullPath;
    }

    static async Task<String> RandomStringAsync()
    {
        var random = await Task.Run(() => new Random());

        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];


        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new String(stringChars);
    }


    void FilePath_ProgressChanged(object? sender, string e)
    {
        //Console.WriteLine($"XD");
    }

    static Bitmap ConvertTo16bpp(System.Drawing.Image img)
    {
        var bmp = new Bitmap(img.Width, img.Height,
                      System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
        using (var gr = Graphics.FromImage(bmp))
            gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
        return bmp;
    }

    static System.Drawing.Image ScaleImage(System.Drawing.Image image, int height)
    {
        double ratio = (double)height / image.Height;
        int newWidth = (int)(image.Width * ratio);
        int newHeight = (int)(image.Height * ratio);
        Bitmap newImage = new Bitmap(newWidth, newHeight);
        using (Graphics g = Graphics.FromImage(newImage))
        {
            g.DrawImage(image, 0, 0, newWidth, newHeight);
        }
        image.Dispose();
        return newImage;
    }
}



//try
//{
//    ZipFile.ExtractToDirectory(x, @"C:\Users\Luca\Desktop\testFolder\Testdirectory");
//}
//catch (Exception e)
//{
//    Console.WriteLine(e);
//}

//CompressPhoto();
