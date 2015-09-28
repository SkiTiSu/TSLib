using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSLib.Photo.EXIF
{
    public class EXIFtranslation : Hashtable
    {
        public EXIFtranslation()
        {
            //Add(0x0B, "ProcessingSoftware");
            Add(0xFE, "NewSubfileType");
            Add(0xFF, "SubfileType");
            //-----TIFF Tags-----
            Add(0x100, "ImageWidth");
            Add(0x101, "ImageHeight");
            Add(0x102, "BitsPerSample");
            Add(0x103, "Compression");
            Add(0x106, "PhotometricInterpretation");
            Add(0x107, "ThreshHolding");
            Add(0x108, "CellWidth");
            Add(0x109, "CellHeight");
            Add(0x10A, "FillOrder");
            Add(0x10D, "DocumentName");
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
            Add(0x11B, "YResolution");
            Add(0x11C, "PlanarConfiguration");
            //Add(0x11D, "Page Name");
            //Add(0x11E, "X Position");
            //Add(0x11F, "Y Position");
            //Add(0x120, "Free Offset");
            //Add(0x121, "Free Byte Counts");
            Add(0x122, "GrayResponseUnit");
            Add(0x123, "GrayResponseCurve");
            Add(0x124, "T4Option");
            Add(0x125, "T6Option");
            Add(0x128, "ResolutionUnit");
            Add(0x129, "PageNumber");
            Add(0x12D, "TransferFuncition");
            Add(0x131, "Software");
            Add(0x132, "DateTime");
            Add(0x13B, "Artist");
            Add(0x13C, "HostComputer");
            Add(0x13D, "Predictor");
            Add(0x13E, "WhitePoint");
            Add(0x13F, "PrimaryChromaticities");
            //
            Add(0x140, "ColorMap");
            Add(0x141, "HalftoneHints");
            Add(0x142, "TileWidth");
            Add(0x143, "TileLength");
            Add(0x144, "TileOffset");
            Add(0x145, "TileByteCounts");
            Add(0x14A, "SubIFDs");
            Add(0x14C, "InkSet");
            Add(0x14D, "InkNames");
            Add(0x14E, "NumberOfInks");
            Add(0x150, "DotRange");
            Add(0x151, "TargetPrinter");
            Add(0x152, "ExtraSamples");
            Add(0x153, "SampleFormat");
            Add(0x154, "SMinSampleValue");
            Add(0x155, "SMaxSampleValue");
            Add(0x156, "TransferRange");
            Add(0x157, "ClipPath");
            Add(0x158, "XClipPathUnits");
            Add(0x159, "YClipPathUnits");
            Add(0x15A, "Indexed");
            Add(0x15B, "JPEGTables");
            Add(0x15F, "OPIProxy");
            Add(0x200, "JPEGProc");
            Add(0x201, "JPEGInterchangeFormat");
            Add(0x202, "JPEGInterchangeFormatLength");
            Add(0x203, "JPEGRestartInterval");
            Add(0x205, "JPEGLosslessPredictors");
            Add(0x206, "JPEGPointTransforms");
            Add(0x207, "JPEGQTables");
            Add(0x208, "JPEGDCTables");
            Add(0x209, "JPEGACTables");
            //
            Add(0x211, "YCbCrCoefficients");
            Add(0x212, "YCbCrSubsampling");
            Add(0x213, "YCbCrPositioning");
            Add(0x214, "ReferenceBlackWhite");
            Add(0x8298, "Copyright");
            Add(0x8769, "Exif IFD Pointer");
            Add(0x8825, "GPS Info IFD Pointer");
            //
            //TODO: 修正这里的EXIFname们
            Add(0x8773, "ICC Profile");
            Add(0x301, "Gamma");
            Add(0x302, "ICC Profile Descriptor");
            Add(0x303, "SRGB RenderingIntent");
            Add(0x320, "Image Title");
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
            //Exif Private Tags
            Add(0x829A, "ExposureTime");
            Add(0x829D, "FNumber");
            Add(0x8822, "ExposureProgram");
            Add(0x8824, "SpectralSensitivity");
            Add(0x8827, "ISOSpeedRatings");
            Add(0x8828, "OECF");
            Add(0x9000, "ExifVersion");
            Add(0x9003, "DateTimeOriginal");
            Add(0x9004, "DateTimeDigitized");
            Add(0x9101, "ComponentsConfiguration");
            Add(0x9102, "CompressedBitsPerPixel");
            Add(0x9201, "ShutterSpeedValue");
            Add(0x9202, "ApertureValue");
            Add(0x9203, "BrightnessValue");
            Add(0x9204, "ExposureBiasValue");
            Add(0x9205, "MaxApertureValue");
            Add(0x9206, "SubjectDist");
            Add(0x9207, "MeteringMode");
            Add(0x9208, "LightSource");
            Add(0x9209, "Flash");
            Add(0x920A, "FocalLength");
            Add(0x927C, "MakerNote");
            Add(0x9286, "UserComment");
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
            Add(0xA214, "SubjectLocation");
            Add(0xA215, "ExposureIndex");
            Add(0xA217, "SensingMethod");
            Add(0xA300, "FileSource");
            Add(0xA301, "SceneType");
            Add(0xA302, "CfaPattern");
            //
            Add(0xA405, "FocalLengthIn35mmFilm");
            //
            //GPS Info Tags
            Add(0x0, "GPSVersionID");
            Add(0x1, "GPSLatitudeRef");
            Add(0x2, "GPSLatitude");
            Add(0x3, "GPSLongitudeRef");
            Add(0x4, "GPSLongitude");
            Add(0x5, "GPSAltitudeRef");
            Add(0x6, "GPSAltitude");
            Add(0x7, "GPSTimeStamp");
            Add(0x8, "GPSSatellites");
            Add(0x9, "GPSStatus");
            Add(0xA, "GPSMeasureMode");
            Add(0xB, "GPSDOP");
            Add(0xC, "GPSSpeedRef");
            Add(0xD, "GPSSpeed");
            Add(0xE, "GPSTrackRef");
            Add(0xF, "GPSTrack");
            Add(0x10, "GPSImgDirectionRef");
            Add(0x11, "GPSImgDirection");
            Add(0x12, "GPSMapDatum");
            Add(0x13, "GPSDestLatitudeRef");
            Add(0x14, "GPSDestLatitude");
            Add(0x15, "GPSDestLongitudeRef");
            Add(0x16, "GPSDestLongitude");
            Add(0x17, "GPSDestBearingRef");
            Add(0x18, "GPSDestBearing");
            Add(0x19, "GPSDestDistanceRef");
            Add(0x1A, "GPSDestDistance");
            //
            Add(0X1B, "GPSProcessingMethod");
            Add(0X1C, "GPSAreaInformation");
            Add(0X1D, "GPSDateStamp");
            Add(0X1E, "GPSDifferential");
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
