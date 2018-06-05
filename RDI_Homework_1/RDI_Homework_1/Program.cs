using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDI_Homework_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Normal(args);
        }

        /// <summary>
        /// Normal homework exercise's solution.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Normal(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Not enough arguments!\n{0} source.hgt out.bmp #elevation", System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location));
                return;
            }

            Original.HeightMap k;
            try
            {
                k = Original.HeightMap.Parse(args[0]);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            int t;
            if (int.TryParse(args[2], out t))
                k.ElevationThreshold = t;
            else
            {
                Console.WriteLine("Invalid height. Use an integer value!");
                return;
            }
            k.SaveToBitmap(args[1]);
        }

        /// <summary>
        /// Own version of the exercise. Done out of curiosity.
        /// Generates an additional _log file where the log of heights are taken.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Own(string[] args)
        {
            var k = new OwnVersion.HeightMap(args[0]);
            k.ToBitmap().Save(args[1]);
            OwnVersion.HeightMap.ColorSetter setter = (low, high, current) =>
            {
                if (current == short.MinValue)
                    return System.Drawing.Color.Black;
                int logHeight = (int)(Math.Log(current - low == 0 ? 1 : current - low) / Math.Log(high - low == 0 ? 1 : high - low) * 255);
                return System.Drawing.Color.FromArgb(logHeight, logHeight, logHeight);
            };
            var temp = args[1].Split('.');
            k.ToBitmap(setter).Save(string.Format("{0}_log.{1}.bmp", temp[temp.Length - 2], temp[temp.Length - 1]));
        }
    }
}
namespace Original
{
    public class HeightMap
    {
        /// <summary>
        /// Internal representation of the HGT file.
        /// </summary>
        private short[] data;
        
        /// <summary>
        /// Determines the water level for the ToBitmap function.
        /// </summary>
        public int ElevationThreshold
        { get; set; }

        /// <summary>
        /// Private constructor. Use HeightMap.Parse instead.
        /// </summary>
        /// <param name="path">Path to file.</param>
        private HeightMap(short[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// HeightMap factory method. Use to create HeightMap instances.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns></returns>
        public static HeightMap Parse(string path)
        {
            byte[] buff;
            using (var sr = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                buff = new byte[sr.Length];
                sr.Read(buff, 0, buff.Length);
            }
            if (buff.Length != 1442401 * 2 && buff.Length != 12967201 * 2)
                throw new ArgumentException("Invalid source file.");
            short[] data = new short[buff.Length / 2];
            for (int i = 0; i < data.Length; i++)
                data[i] = (short)(buff[2 * i] * 256 + buff[2 * i + 1]);
            return new HeightMap(data.ToArray());
        }

        /// <summary>
        /// Gets the smallest and highest points in the heightmap.
        /// </summary>
        /// <returns>Int array with first element being the smallest, second the largest height in the HeightMap.</returns>
        private int[] GetExtremalElevations()
        {
            var k = new int[2];
            k[0] = data.Min((x) => { if (x == short.MinValue) return short.MaxValue; return x; }); //Remove unscanned areas.
            k[1] = data.Max();
            return k;
        }

        /// <summary>
        /// Saves the HeightMap into a Bitmap. Points under ElevationThreshold are blue, higher green.
        /// </summary>
        /// <param name="path">Path to save bitmap.</param>
        public void SaveToBitmap(string path)
        {
            int size = (int)Math.Sqrt(data.Length);
            var bmp = new System.Drawing.Bitmap(size, size);
            int[] extremals = GetExtremalElevations();
            //No precise coloring instructions were given and I refuse to try to spend a day trying to reverse engineer it from the given sample image. Close enough and should be error free and scalable to any HGT file.
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == short.MinValue)
                    bmp.SetPixel(i % size, i / size, System.Drawing.Color.Black);
                else
                {
                    var c = System.Drawing.Color.FromArgb(255 - (data[i] - extremals[0]) * 255 / (extremals[1] - extremals[0]), 255, 255 - (data[i] - extremals[0]) * 255 / (extremals[1] - extremals[0]));
                    bmp.SetPixel(i % size, i / size, data[i] < ElevationThreshold ? System.Drawing.Color.Blue : c);
                }
            }
            bmp.Save(path);
        }
    }
}
namespace OwnVersion
{
    /// <summary>
    /// HeightMap class for interesting drawing solutions.
    /// </summary>
    public class HeightMap
    {
        public delegate System.Drawing.Color ColorSetter(short lowest, short highest, short height);

        private short[] data;
        private short lowest = short.MinValue;
        private short highest = short.MinValue;

        /// <summary>
        /// Lowest valid value in the HGT file.
        /// </summary>
        public short Lowest
        {
            get
            {
                if (lowest == short.MinValue)
                {
                    //return lowest valid value in data
                    return lowest = data.Min((x) => { if (x == short.MinValue) return short.MaxValue; return x; });
                }
                return lowest;
            }
        }
        /// <summary>
        /// Highest valid value in the HGT file.
        /// </summary>
        public short Highest
        {
            get
            {
                if (highest == short.MinValue)
                    return highest = data.Max();
                return highest;
            }
        }

        /// <summary>
        /// HeightMap constructor.
        /// </summary>
        /// <param name="path">Path to the HGT file.</param>
        public HeightMap(string path)
        {
            byte[] buff;
            using (var sr = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                buff = new byte[sr.Length];
                sr.Read(buff, 0, buff.Length);
            }
            if (buff.Length != 1442401 * 2 && buff.Length != 12967201 * 2)
                throw new ArgumentException("Invalid source file.");
            short[] data = new short[buff.Length / 2];
            for (int i = 0; i < data.Length; i++)
                data[i] = (short)(buff[2 * i] * 256 + buff[2 * i + 1]);
            this.data = data;
        }

        /// <summary>
        /// Creates a default black and white bitmap from the HGT file, black being lowest or missing, white being highest.
        /// </summary>
        /// <returns>Black and white Bitmap.</returns>
        public System.Drawing.Bitmap ToBitmap()
        {
            ColorSetter setter = (lowest, highest, height) => 
            {
                if (height == short.MinValue)
                    return System.Drawing.Color.Black;
                return System.Drawing.Color.FromArgb((height - lowest) * 255 / (highest - lowest), (height - lowest) * 255 / (highest - lowest), (height - lowest) * 255 / (highest - lowest));
            };
            return ToBitmap(setter);
        }

        /// <summary>
        /// Creates a bitmap from the HGT file, where each height corresponds to a color set by the setter.
        /// </summary>
        /// <param name="setter">Maps the (lowest, highest) interval with short.MinValue for missing height value into RGB colorspace.</param>
        /// <returns>Bitmap created using the setter.</returns>
        public System.Drawing.Bitmap ToBitmap(ColorSetter setter)
        {
            int size = (int)Math.Sqrt(data.Length);
            var bmp = new System.Drawing.Bitmap(size, size);
            using (var k = new LockBitmap.LockBitmap(bmp))
            {
                for (int i = 0; i < data.Length; i++)
                    k.SetPixel(i % size, i / size, setter(Lowest, Highest, data[i]));
            }
            return bmp;
        }
    }
}