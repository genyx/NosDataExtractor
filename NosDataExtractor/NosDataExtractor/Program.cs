using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NosDataExtractor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Nostale Data Extractor";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid .NOS-Files are:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  NScliData");
            Console.WriteLine("  NSetcData");
            Console.WriteLine("  NSgtdData - Main stuff like item.dat");
            Console.WriteLine("  NSlangData - Your Language stuff");
            Console.WriteLine("And maybe some more.");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Thread.Sleep(2000);

            while (true)
            {
                Console.WriteLine("Source?");

                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "NOS-File|*.NOS",
                    CheckFileExists = true
                };
                DialogResult dialogResult = ofd.ShowDialog();

                if (dialogResult != DialogResult.OK || string.IsNullOrEmpty(ofd.FileName))
                    break;


                Console.WriteLine("Target?");

                FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    SelectedPath = new FileInfo(ofd.FileName).DirectoryName
                };
                dialogResult = fbd.ShowDialog();

                if (dialogResult != DialogResult.OK || string.IsNullOrEmpty(fbd.SelectedPath))
                    break;


                Console.WriteLine("Starting...");

                try
                {
                    NosExtractor ne = new NosExtractor(Console.WriteLine);
                    ne.Extract(ofd.FileName, new DirectoryInfo(Path.Combine(fbd.SelectedPath, new FileInfo(ofd.FileName).Name.Replace(".NOS", ""))));

                    Console.WriteLine("Finished.");
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("NOS File invalid.");
                    Console.ResetColor();
                }

                Console.WriteLine("Restart...");
                Thread.Sleep(1000);
                Console.WriteLine("\n\n");
            }

            Console.WriteLine("end of program.");
            Thread.Sleep(2000);
        }
    }
}
