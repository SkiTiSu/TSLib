using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace TSLib.Photo
{
    /* 本类原名EXIFextractor，原作者为Asim Goheer，没有声明共享方式，项目地址如下
     * http://www.codeproject.com/Articles/11305/EXIFextractor-library-to-extract-EXIF-information
     * 该项目创建于2005年，后没有任何更改，作者官网已无法访问。
     * 由于该项目年久失修，存在各种bug且部分地方不符合规范，现由天书进行大整改并发布其衍生作品TSLib.Photo.EXIF系列。
     * EXIF参照CIPA在2012年的最新版规范，文档如下：
     * http://www.cipa.jp/std/documents/e/DC-008-2012_E.pdf
     * 还参照了以下文档
     * http://www.exiv2.org/tags.html
     * 最后再次感谢原作者Asim Goheer。
     *
     * 修改的部分请搜索“修改”关键词即可找到哦~
     */

    //TODO:继续修改，还有本地化哦。。。T T
    //TODO:将转换器和翻译器分开文件，EXIF namespace
    public class EXIF
    {
        /// <summary>
		/// EXIFextractor Class
		/// 
		/// </summary>
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
            private translation myHash;
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
                myHash = new translation();
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
                myHash = new translation();
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

                myHash = new translation();
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

    /// <summary>
    /// Summary description for translation.
    /// </summary>
    public class translation : Hashtable
    {
        public translation()
        {
            Add(0x8769, "Exif IFD");
            Add(0x8825, "Gps IFD");
            Add(0xFE, "New Subfile Type");
            Add(0xFF, "Subfile Type");
            //-----上头是啥。。。-----
            //-----TIFF Tags-----
            Add(0x100, "ImageWidth");
            Add(0x101, "ImageHeight");
            Add(0x102, "BitsPerSample");
            Add(0x103, "Compression");
            Add(0x106, "PhotometricInterpretation");
            //this.Add(0x107, "Thresh Holding");
            //this.Add(0x108, "Cell Width");
            //Add(0x109, "Cell Height");
            //Add(0x10A, "Fill Order");
            //Add(0x10D, "Document Name");
            Add(0x10E, "ImageDescription");
            Add(0x10F, "Make");
            Add(0x110, "Model");
            Add(0x111, "StripOffsets");
            Add(0x112, "Orientation");
            Add(0x115, "SamplesPerPixel");
            Add(0x116, "RowsPerStrip");
            Add(0x117, "StripBytesCount");
            //Add(0x118, "Min Sample Value");
            //Add(0x119, "Max Sample Value");
            Add(0x11A, "XResolution");
            Add(0x11B, "Y Resolution");
            Add(0x11C, "Planar Config");
            Add(0x11D, "Page Name");
            Add(0x11E, "X Position");
            Add(0x11F, "Y Position");
            Add(0x120, "Free Offset");
            Add(0x121, "Free Byte Counts");
            Add(0x122, "Gray Response Unit");
            Add(0x123, "Gray Response Curve");
            Add(0x124, "T4 Option");
            Add(0x125, "T6 Option");
            Add(0x128, "Resolution Unit");
            Add(0x129, "Page Number");
            Add(0x12D, "Transfer Funcition");
            Add(0x131, "Software Used");
            Add(0x132, "Date Time");
            Add(0x13B, "Artist");
            Add(0x13C, "Host Computer");
            Add(0x13D, "Predictor");
            Add(0x13E, "White Point");
            Add(0x13F, "Primary Chromaticities");
            Add(0x140, "ColorMap");
            Add(0x141, "Halftone Hints");
            Add(0x142, "Tile Width");
            Add(0x143, "Tile Length");
            Add(0x144, "Tile Offset");
            Add(0x145, "Tile ByteCounts");
            Add(0x14C, "InkSet");
            Add(0x14D, "Ink Names");
            Add(0x14E, "Number Of Inks");
            Add(0x150, "Dot Range");
            Add(0x151, "Target Printer");
            Add(0x152, "Extra Samples");
            Add(0x153, "Sample Format");
            Add(0x154, "S Min Sample Value");
            Add(0x155, "S Max Sample Value");
            Add(0x156, "Transfer Range");
            Add(0x200, "JPEG Proc");
            Add(0x201, "JPEG InterFormat");
            Add(0x202, "JPEG InterLength");
            Add(0x203, "JPEG RestartInterval");
            Add(0x205, "JPEG LosslessPredictors");
            Add(0x206, "JPEG PointTransforms");
            Add(0x207, "JPEG QTables");
            Add(0x208, "JPEG DCTables");
            Add(0x209, "JPEG ACTables");
            Add(0x211, "YCbCr Coefficients");
            Add(0x212, "YCbCr Subsampling");
            Add(0x213, "YCbCr Positioning");
            Add(0x214, "REF Black White");
            Add(0x8773, "ICC Profile");
            Add(0x301, "Gamma");
            Add(0x302, "ICC Profile Descriptor");
            Add(0x303, "SRGB RenderingIntent");
            Add(0x320, "Image Title");
            Add(0x8298, "Copyright");
            Add(0x5001, "Resolution X Unit");
            Add(0x5002, "Resolution Y Unit");
            Add(0x5003, "Resolution X LengthUnit");
            Add(0x5004, "Resolution Y LengthUnit");
            Add(0x5005, "Print Flags");
            Add(0x5006, "Print Flags Version");
            Add(0x5007, "Print Flags Crop");
            Add(0x5008, "Print Flags Bleed Width");
            Add(0x5009, "Print Flags Bleed Width Scale");
            Add(0x500A, "Halftone LPI");
            Add(0x500B, "Halftone LPIUnit");
            Add(0x500C, "Halftone Degree");
            Add(0x500D, "Halftone Shape");
            Add(0x500E, "Halftone Misc");
            Add(0x500F, "Halftone Screen");
            Add(0x5010, "JPEG Quality");
            Add(0x5011, "Grid Size");
            Add(0x5012, "Thumbnail Format");
            Add(0x5013, "Thumbnail Width");
            Add(0x5014, "Thumbnail Height");
            Add(0x5015, "Thumbnail ColorDepth");
            Add(0x5016, "Thumbnail Planes");
            Add(0x5017, "Thumbnail RawBytes");
            Add(0x5018, "Thumbnail Size");
            Add(0x5019, "Thumbnail CompressedSize");
            Add(0x501A, "Color Transfer Function");
            Add(0x501B, "Thumbnail Data");
            Add(0x5020, "Thumbnail ImageWidth");
            Add(0x502, "Thumbnail ImageHeight");
            Add(0x5022, "Thumbnail BitsPerSample");
            Add(0x5023, "Thumbnail Compression");
            Add(0x5024, "Thumbnail PhotometricInterp");
            Add(0x5025, "Thumbnail ImageDescription");
            Add(0x5026, "Thumbnail EquipMake");
            Add(0x5027, "Thumbnail EquipModel");
            Add(0x5028, "Thumbnail StripOffsets");
            Add(0x5029, "Thumbnail Orientation");
            Add(0x502A, "Thumbnail SamplesPerPixel");
            Add(0x502B, "Thumbnail RowsPerStrip");
            Add(0x502C, "Thumbnail StripBytesCount");
            Add(0x502D, "Thumbnail ResolutionX");
            Add(0x502E, "Thumbnail ResolutionY");
            Add(0x502F, "Thumbnail PlanarConfig");
            Add(0x5030, "Thumbnail ResolutionUnit");
            Add(0x5031, "Thumbnail TransferFunction");
            Add(0x5032, "Thumbnail SoftwareUsed");
            Add(0x5033, "Thumbnail DateTime");
            Add(0x5034, "Thumbnail Artist");
            Add(0x5035, "Thumbnail WhitePoint");
            Add(0x5036, "Thumbnail PrimaryChromaticities");
            Add(0x5037, "Thumbnail YCbCrCoefficients");
            Add(0x5038, "Thumbnail YCbCrSubsampling");
            Add(0x5039, "Thumbnail YCbCrPositioning");
            Add(0x503A, "Thumbnail RefBlackWhite");
            Add(0x503B, "Thumbnail CopyRight");
            Add(0x5090, "Luminance Table");
            Add(0x5091, "Chrominance Table");
            Add(0x5100, "Frame Delay");
            Add(0x5101, "Loop Count");
            Add(0x5110, "Pixel Unit");
            Add(0x5111, "Pixel PerUnit X");
            Add(0x5112, "Pixel PerUnit Y");
            Add(0x5113, "Palette Histogram");
            Add(0x829A, "Exposure Time");
            Add(0x829D, "F-Number");
            Add(0x8822, "Exposure Prog");
            Add(0x8824, "Spectral Sense");
            Add(0x8827, "ISO Speed");
            Add(0x8828, "OECF");
            Add(0x9000, "Ver");
            Add(0x9003, "DTOrig");
            Add(0x9004, "DTDigitized");
            Add(0x9101, "CompConfig");
            Add(0x9102, "CompBPP");
            Add(0x9201, "Shutter Speed");
            Add(0x9202, "Aperture");
            Add(0x9203, "Brightness");
            Add(0x9204, "Exposure Bias");
            Add(0x9205, "MaxAperture");
            Add(0x9206, "SubjectDist");
            Add(0x9207, "Metering Mode");
            Add(0x9208, "LightSource");
            Add(0x9209, "Flash");
            Add(0x920A, "FocalLength");
            Add(0x927C, "Maker Note");
            Add(0x9286, "User Comment");
            Add(0x9290, "DTSubsec");
            Add(0x9291, "DTOrigSS");
            Add(0x9292, "DTDigSS");
            Add(0xA000, "FPXVer");
            Add(0xA001, "ColorSpace");
            Add(0xA002, "PixXDim");
            Add(0xA003, "PixYDim");
            Add(0xA004, "RelatedWav");
            Add(0xA005, "Interop");
            Add(0xA20B, "FlashEnergy");
            Add(0xA20C, "SpatialFR");
            Add(0xA20E, "FocalXRes");
            Add(0xA20F, "FocalYRes");
            Add(0xA210, "FocalResUnit");
            Add(0xA214, "Subject Loc");
            Add(0xA215, "Exposure Index");
            Add(0xA217, "Sensing Method");
            Add(0xA300, "FileSource");
            Add(0xA301, "SceneType");
            Add(0xA302, "CfaPattern");
            Add(0x0, "Gps Ver");
            Add(0x1, "Gps LatitudeRef");
            Add(0x2, "Gps Latitude");
            Add(0x3, "Gps LongitudeRef");
            Add(0x4, "Gps Longitude");
            Add(0x5, "Gps AltitudeRef");
            Add(0x6, "Gps Altitude");
            Add(0x7, "Gps GpsTime");
            Add(0x8, "Gps GpsSatellites");
            Add(0x9, "Gps GpsStatus");
            Add(0xA, "Gps GpsMeasureMode");
            Add(0xB, "Gps GpsDop");
            Add(0xC, "Gps SpeedRef");
            Add(0xD, "Gps Speed");
            Add(0xE, "Gps TrackRef");
            Add(0xF, "Gps Track");
            Add(0x10, "Gps ImgDirRef");
            Add(0x11, "Gps ImgDir");
            Add(0x12, "Gps MapDatum");
            Add(0x13, "Gps DestLatRef");
            Add(0x14, "Gps DestLat");
            Add(0x15, "Gps DestLongRef");
            Add(0x16, "Gps DestLong");
            Add(0x17, "Gps DestBearRef");
            Add(0x18, "Gps DestBear");
            Add(0x19, "Gps DestDistRef");
            Add(0x1A, "Gps DestDist");
        }
    }
    /// <summary>
    /// private class
    /// </summary>
    internal class Rational
    {
        private int n;
        private int d;
        public Rational(int n, int d)
        {
            this.n = n;
            this.d = d;
            simplify(ref this.n, ref this.d);
        }
        public Rational(uint n, uint d)
        {
            this.n = Convert.ToInt32(n);
            this.d = Convert.ToInt32(d);

            simplify(ref this.n, ref this.d);
        }
        public Rational()
        {
            n = d = 0;
        }
        public string ToString(string sp)
        {
            if (sp == null) sp = "/";
            return n.ToString() + sp + d.ToString();
        }
        public double ToDouble()
        {
            if (d == 0)
                return 0.0;

            return Math.Round(Convert.ToDouble(n) / Convert.ToDouble(d), 5); //修改/曝光时间为0的bug，以前的相机性能是不是很差，竟然用2...
        }
        private void simplify(ref int a, ref int b)
        {
            if (a == 0 || b == 0)
                return;

            int gcd = euclid(a, b);
            a /= gcd;
            b /= gcd;
        }
        private int euclid(int a, int b)
        {
            if (b == 0)
                return a;
            else
                return euclid(b, a % b);
        }
    }
}
