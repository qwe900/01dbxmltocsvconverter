using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.IO;

namespace CmgConverter
{

    class Program
    {



        static string MakeFileName(string s)
        {
            return Path.GetInvalidFileNameChars().Aggregate(s, (current, c) => current.Replace(c.ToString(), "_"));
        }



        static void Main(string[] args)
        {

            string inputPath = args.Length > 0 ? Environment.ExpandEnvironmentVariables(args[0]) : @"..\..\..\exampledata.xml";
            string outputPath = args.Length > 1 ? Environment.ExpandEnvironmentVariables(args[1]) : Environment.CurrentDirectory;

            XDocument xml = XDocument.Load(inputPath);
            var xElement = xml.Element("Cmg");

            if (xElement != null)
            {
                foreach (var child in xElement.Elements("ID"))
                {

                    string id = child.Attribute("val").Value;

                    string typeAndFamilyText = child.Element("TypeAndFamilyText").Value;
                    string datafilename = Path.GetFileNameWithoutExtension(child.Element("DataFileName").Value);
                    string place = child.Element("Place").Value;
                    string csvFileName = $"{id}_{place}_{datafilename}.csv";
                    string csvOutputPath = Path.Combine(outputPath, MakeFileName(csvFileName));
                    
                    string[] list = { "LAeq", "LAFeq" }; // List of necessary data
                    if (list.Contains(datafilename))
                  
                  
                    {
                        DateTime dtBegin = DateTime.Parse(child.Element("DateBegin").Value);
                        DateTime dtEnd = DateTime.Parse(child.Element("DateEnd").Value);
                        int duration = Convert.ToInt32(child.Element("Duration").Value);
                        int period = Convert.ToInt32(child.Element("Period").Value);
                        int rows = (duration / period);


                        Console.WriteLine("----------------- ID: " + id + " -------------------");
                        Console.WriteLine("FamilyText: " + typeAndFamilyText + " Filename: " + datafilename);
                        Console.WriteLine("DateBegin: " + dtBegin + " DateEnd: " + dtEnd);
                        Console.WriteLine(" Period: " + period + " Rows: " + rows);


                        var bytes = Convert.FromBase64String(child.Element("Data").Value);

                        using (var fStream = File.CreateText(csvOutputPath))
                        {
                            fStream.WriteLine("DATE\tVALUE");


                            for (int i = 0, h = 0; i < bytes.Length; i += 4, ++h)
                            {
                                var floatValue = BitConverter.ToSingle(bytes, i);
                                var floatTime = dtBegin.AddSeconds(h);

                                fStream.WriteLine($"{floatTime}\t{floatValue}");


                            }


                            fStream.Close();

                        }
                    }


                    if (typeAndFamilyText == "MP3")
                    {

                       
                        string mp3datafilename = Path.GetFileName(child.Element("DataFileName").Value);


                        var mp3base64string = Convert.FromBase64String(child.Element("Data").Value);
                        using (FileStream file = File.Create(mp3datafilename))
                        {
                            using (BinaryWriter writer = new BinaryWriter(file))
                            {
                                for (int i = 0; i < mp3base64string.Length; i += 4)
                                {

                                    writer.Write((byte)(967.644334f * BitConverter.ToSingle(mp3base64string, i)));
                                }
                            }     
                        }
                        //Console.ReadLine(); //for debug you can hold window open
                    }
                }
            }

        }
    }
}

