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
        static float ConvertByteArrayToFloat(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length % 4 != 0)
                throw new ArgumentException
                      ("bytes does not represent a sequence of floats");

            return BitConverter.ToSingle(bytes, 0);
        }

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
                    string place = child.Element("Place").Value;
                    string csvFileName = $"{id}_{place}_{typeAndFamilyText}.csv";
                    string csvOutputPath = Path.Combine(outputPath, MakeFileName(csvFileName));
                    DateTime dtBegin = DateTime.Parse(child.Element("DateBegin").Value);
                    DateTime dtEnd = DateTime.Parse(child.Element("DateEnd").Value);
                    TimeSpan duration = TimeSpan.FromSeconds(Convert.ToInt32(child.Element("Duration").Value));
                    var dataElement = child.Element("Data");

                    var bytes = Convert.FromBase64String(dataElement.Value);

                    using (var fStream = File.CreateText(csvOutputPath))
                    {
                        fStream.WriteLine("DATE\tVALUE");

                        for (int i = 0; i < bytes.Length; i += 4)
                        {
                            var floatValue = BitConverter.ToSingle(bytes, i);
                            var floatTime = dtBegin.AddSeconds(i + 1);

                            fStream.WriteLine($"{floatValue}\t{floatTime}");
                        }

                        fStream.Close();
                    }

                }
            }
        }
    }

}