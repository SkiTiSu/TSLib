/* 本类原名EXIFextractor，原作者为Asim Goheer，没有声明共享方式，项目地址如下
 * http://www.codeproject.com/Articles/11305/EXIFextractor-library-to-extract-EXIF-information
 * 该项目创建于2005年，后没有任何更改，作者官网已无法访问。
 * 由于该项目年久失修，存在各种bug且部分地方不符合规范，现由天书进行大整改并发布其衍生作品TSLib.Photo.EXIF系列。
 * EXIF参照CIPA在2012年的最新版规范，文档如下：
 * http://www.cipa.jp/std/documents/e/DC-008-2012_E.pdf
 * 还参照了以下文档，增加了一些不在以上文档中的参数，意在更多的兼容
 * http://www.exiv2.org/tags.html
 * 最后再次感谢原作者Asim Goheer。
 *
 * 修改的部分请搜索“修改”关键词即可找到哦~
 *
 * 一点小感慨：2002年的EXIF2.2的文档竟然还是存在柯达的网站上，时光荏苒。
 *
 * 如果需要读取高级的东西（品牌自定义参数），可以使用metadata-extractor-dotnet，
 * https://github.com/drewnoakes/metadata-extractor-dotnet
 */

//TODO:继续修改，还有本地化哦。。。T T
//TODO:将转换器和翻译器分开文件，EXIF namespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace TSLib.Photo.EXIF
{
    public class EXIFextractor : IEnumerable
    {
        /// <summary>
        /// Get the individual property value by supplying property name
        /// </summary>
        public object this[string index]
        {
            get
            {
                return properties[index];
            }
        }
        //
        private System.Drawing.Bitmap bmp;
        //
        private string data;
        //
        private EXIFtranslation myHash;
        //
        private Hashtable properties;
        //
        internal int Count
        {
            get
            {
                return properties.Count;
            }
        }
        //
        string sp;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="len"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void setTag(int id, string data)
        {
            Encoding ascii = Encoding.ASCII;
            setTag(id, data.Length, 0x2, ascii.GetBytes(data));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="len"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void setTag(int id, int len, short type, byte[] data)
        {
            PropertyItem p = CreatePropertyItem(type, id, len, data);
            bmp.SetPropertyItem(p);
            buildDB(bmp.PropertyItems);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        /// <param name="len"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PropertyItem CreatePropertyItem(short type, int tag, int len, byte[] value)
        {
            PropertyItem item;

            // Loads a PropertyItem from a Jpeg image stored in the assembly as a resource.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream emptyBitmapStream = assembly.GetManifestResourceStream("EXIFextractor.decoy.jpg");
            System.Drawing.Image empty = System.Drawing.Image.FromStream(emptyBitmapStream);

            item = empty.PropertyItems[0];

            // Copies the data to the property item.
            item.Type = type;
            item.Len = len;
            item.Id = tag;
            item.Value = new byte[value.Length];
            value.CopyTo(item.Value, 0);

            return item;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="sp"></param>
        public EXIFextractor(ref System.Drawing.Bitmap bmp, string sp)
        {
            properties = new Hashtable();
            //
            this.bmp = bmp;
            this.sp = sp;
            //
            myHash = new EXIFtranslation();
            buildDB(this.bmp.PropertyItems);
        }
        string msp = "";
        public EXIFextractor(ref System.Drawing.Bitmap bmp, string sp, string msp)
        {
            properties = new Hashtable();
            this.sp = sp;
            this.msp = msp;
            this.bmp = bmp;
            //				
            myHash = new EXIFtranslation();
            buildDB(bmp.PropertyItems);

        }
        public static PropertyItem[] GetExifProperties(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream,
                             /* useEmbeddedColorManagement = */ true,
                             /* validateImageData = */ false);
            return image.PropertyItems;
        }
        public EXIFextractor(string file, string sp, string msp)
        {
            properties = new Hashtable();
            this.sp = sp;
            this.msp = msp;

            myHash = new EXIFtranslation();
            //				
            buildDB(GetExifProperties(file));

        }

        /// <summary>
        /// 
        /// </summary>
        private void buildDB(System.Drawing.Imaging.PropertyItem[] parr)
        {
            properties.Clear();
            //
            data = "";
            //
            Encoding ascii = Encoding.ASCII;
            //
            foreach (System.Drawing.Imaging.PropertyItem p in parr)
            {
                string v = "";
                string name = (string)myHash[p.Id];
                // tag not found. skip it
                if (name == null) continue;
                //
                data += name + ": ";
                //
                //1 = BYTE An 8-bit unsigned integer.,
                if (p.Type == 0x1)
                {
                    v = p.Value[0].ToString();
                }
                //2 = ASCII An 8-bit byte containing one 7-bit ASCII code. The final byte is terminated with NULL.,
                else if (p.Type == 0x2)
                {
                    // string					
                    v = ascii.GetString(p.Value);
                }
                //3 = SHORT A 16-bit (2 -byte) unsigned integer,
                else if (p.Type == 0x3)
                {
                    // orientation // lookup table					
                    switch (p.Id)
                    {
                        case 0x8827: // ISO
                            v = "ISO-" + convertToInt16U(p.Value).ToString();
                            break;
                        case 0xA217: // sensing method
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 1: v = "Not defined"; break;
                                    case 2: v = "One-chip color area sensor"; break;
                                    case 3: v = "Two-chip color area sensor"; break;
                                    case 4: v = "Three-chip color area sensor"; break;
                                    case 5: v = "Color sequential area sensor"; break;
                                    case 7: v = "Trilinear sensor"; break;
                                    case 8: v = "Color sequential linear sensor"; break;
                                    default: v = " reserved"; break;
                                }
                            }
                            break;
                        case 0x8822: // aperture 
                            switch (convertToInt16U(p.Value))
                            {
                                case 0: v = "Not defined"; break;
                                case 1: v = "Manual"; break;
                                case 2: v = "Normal program"; break;
                                case 3: v = "Aperture priority"; break;
                                case 4: v = "Shutter priority"; break;
                                case 5: v = "Creative program (biased toward depth of field)"; break;
                                case 6: v = "Action program (biased toward fast shutter speed)"; break;
                                case 7: v = "Portrait mode (for closeup photos with the background out of focus)"; break;
                                case 8: v = "Landscape mode (for landscape photos with the background in focus)"; break;
                                default: v = "reserved"; break;
                            }
                            break;
                        case 0x9207: // metering mode
                            switch (convertToInt16U(p.Value))
                            {
                                case 0: v = "unknown"; break;
                                case 1: v = "Average"; break;
                                case 2: v = "CenterWeightedAverage"; break;
                                case 3: v = "Spot"; break;
                                case 4: v = "MultiSpot"; break;
                                case 5: v = "Pattern"; break;
                                case 6: v = "Partial"; break;
                                case 255: v = "Other"; break;
                                default: v = "reserved"; break;
                            }
                            break;
                        case 0x9208: // light source
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0: v = "unknown"; break;
                                    case 1: v = "Daylight"; break;
                                    case 2: v = "Fluorescent"; break;
                                    case 3: v = "Tungsten"; break;
                                    case 17: v = "Standard light A"; break;
                                    case 18: v = "Standard light B"; break;
                                    case 19: v = "Standard light C"; break;
                                    case 20: v = "D55"; break;
                                    case 21: v = "D65"; break;
                                    case 22: v = "D75"; break;
                                    case 255: v = "other"; break;
                                    default: v = "reserved"; break;
                                }
                            }
                            break;
                        case 0x9209:
                            {
                                switch (convertToInt16U(p.Value))
                                {
                                    case 0: v = "Flash did not fire"; break;
                                    case 1: v = "Flash fired"; break;
                                    case 5: v = "Strobe return light not detected"; break;
                                    case 7: v = "Strobe return light detected"; break;
                                    default: v = "reserved"; break;
                                }
                            }
                            break;
                        default:
                            v = convertToInt16U(p.Value).ToString();
                            break;
                    }
                }
                //4 = LONG A 32-bit (4 -byte) unsigned integer,
                else if (p.Type == 0x4)
                {
                    // orientation // lookup table					
                    v = convertToInt32U(p.Value).ToString();
                }
                //5 = RATIONAL Two LONGs. The first LONG is the numerator and the second LONG expresses the//denominator.,
                else if (p.Type == 0x5)
                {
                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    uint a = convertToInt32U(n);
                    uint b = convertToInt32U(d);
                    Rational r = new Rational(a, b);
                    //
                    //convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9202: // aperture
                            v = "F/" + Math.Round(Math.Pow(Math.Sqrt(2), r.ToDouble()), 2).ToString();
                            break;
                        case 0x920A:
                            v = r.ToDouble().ToString();
                            break;
                        case 0x829A: //修改/曝光时间为0的bug，且分数与数字自动切换
                            double temp = r.ToDouble();
                            if (temp > 1)
                            {
                                v = temp.ToString();
                            }
                            else
                            {
                                v = r.ToString("/");
                            }
                            break;
                        case 0x829D: // F-number
                            v = "F/" + r.ToDouble().ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }

                }
                //7 = UNDEFINED An 8-bit byte that can take any value depending on the field definition,
                else if (p.Type == 0x7)
                {
                    switch (p.Id)
                    {
                        case 0xA300:
                            {
                                if (p.Value[0] == 3)
                                {
                                    v = "DSC";
                                }
                                else
                                {
                                    v = "reserved";
                                }
                                break;
                            }
                        case 0xA301:
                            if (p.Value[0] == 1)
                                v = "A directly photographed image";
                            else
                                v = "Not a directly photographed image";
                            break;
                        default:
                            v = "-";
                            break;
                    }
                }
                //9 = SLONG A 32-bit (4 -byte) signed integer (2's complement notation),
                else if (p.Type == 0x9)
                {
                    v = convertToInt32(p.Value).ToString();
                }
                //10 = SRATIONAL Two SLONGs. The first SLONG is the numerator and the second SLONG is the
                //denominator.
                else if (p.Type == 0xA)
                {

                    // rational
                    byte[] n = new byte[p.Len / 2];
                    byte[] d = new byte[p.Len / 2];
                    Array.Copy(p.Value, 0, n, 0, p.Len / 2);
                    Array.Copy(p.Value, p.Len / 2, d, 0, p.Len / 2);
                    int a = convertToInt32(n);
                    int b = convertToInt32(d);
                    Rational r = new Rational(a, b);
                    //
                    // convert here
                    //
                    switch (p.Id)
                    {
                        case 0x9201: // shutter speed
                            v = "1/" + Math.Round(Math.Pow(2, r.ToDouble()), 2).ToString();
                            break;
                        case 0x9203:
                            v = Math.Round(r.ToDouble(), 4).ToString();
                            break;
                        default:
                            v = r.ToString("/");
                            break;
                    }
                }
                // add it to the list
                if (properties[name] == null)
                    properties.Add(name, v);
                // cat it too
                data += v;
                data += sp;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        int convertToInt32(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        int convertToInt16(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return arr[1] << 8 | arr[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        uint convertToInt32U(byte[] arr)
        {
            if (arr.Length != 4)
                return 0;
            else
                return Convert.ToUInt32(arr[3] << 24 | arr[2] << 16 | arr[1] << 8 | arr[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        uint convertToInt16U(byte[] arr)
        {
            if (arr.Length != 2)
                return 0;
            else
                return Convert.ToUInt16(arr[1] << 8 | arr[0]);
        }
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            // TODO:  Add EXIFextractor.GetEnumerator implementation
            return (new EXIFextractorEnumerator(properties));
        }

        #endregion
    }

    //
    // dont touch this class. its for IEnumerator
    // 
    //
    class EXIFextractorEnumerator : IEnumerator
    {
        Hashtable exifTable;
        IDictionaryEnumerator index;

        internal EXIFextractorEnumerator(Hashtable exif)
        {
            exifTable = exif;
            Reset();
            index = exif.GetEnumerator();
        }

        #region IEnumerator Members

        public void Reset()
        {
            index = null;
        }

        public object Current
        {
            get
            {
                return (new KeyValuePair<string, string>(index.Key.ToString(), index.Value.ToString()));
            }
        }

        public bool MoveNext()
        {
            if (index != null && index.MoveNext())
                return true;
            else
                return false;
        }

        #endregion

    }
}
