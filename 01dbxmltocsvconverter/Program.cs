using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;

namespace CmgConverter
{

    class convert
    {
        static float[] ConvertByteArrayToFloat(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (bytes.Length % 4 != 0)
                throw new ArgumentException
                      ("bytes does not represent a sequence of floats");

            return Enumerable.Range(0, bytes.Length / 4)
                             .Select(i => BitConverter.ToSingle(bytes, i * 4))
                             .ToArray();
        }
        public static float[] ConvertByteToFloat(byte[] array)
        {
            float[] floatArr = new float[array.Length / 4];
            for (int i = 0; i < floatArr.Length; i++)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(array, i * 4, 4);
                }
                floatArr[i] = BitConverter.ToSingle(array, i * 4);
            }
            return floatArr;
        }
        static void Main(string[] args)

        {


            XDocument xml = XDocument.Load("..//..//exampledata.xml");

            //var a = X.Descendants("ID").First().Value;
            var xElement = xml.Element("Cmg");
            if (xElement != null)
            {
                foreach (var child in xElement.Elements())
                {
                    if (child.Name == "ID")
                        //Console.WriteLine(child.Name);
                        foreach (var item in child.Attributes())
                        {

                            //  Console.WriteLine(item.Name + ": " + item.Value);
                        } //hier beginnt data
                    foreach (var childElement in child.Elements())
                    {


                        if (childElement.Name == "Data")
                        {
                            Console.WriteLine("--->" + childElement.Name);

                            foreach (var ds in childElement.Attributes())
                            {
                                // Console.WriteLine(ds.Name + ": " + ds.Value + "; " + childElement.Value);
                                var data = childElement.Value;
                                byte[] bytes = Convert.FromBase64String(data);
                                var data2 = ConvertByteArrayToFloat(bytes);
                                for (int i = 0; i < data2.Length; i++)
                                {
                                    data2[i] = BitConverter.ToSingle(bytes, i * 4);
                                    Console.WriteLine("line: " + i + " horst " + data2[i]);
                                }








                                //   Console.WriteLine(data2);

                            }
                        }
                    }

                }
            }

            Console.ReadLine();
        }


    }

}