using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPickerWPF {
    public class HTMLColor : Control {
        private string _HexValue;
        private byte _Red;
        private byte _Green;
        private byte _Blue;
        private byte _Alpha;
        private string _ColorName;
        private bool _IsCustomColor;
        private bool _IsSelected;
        public static DependencyProperty ColorNameProperty =
            DependencyProperty.Register("ColorName", typeof(string),
                typeof(HTMLColor), new PropertyMetadata(""));

        public HTMLColor(byte red, byte green, byte blue) {
            _Alpha = 255;
            _Red = red;
            _Green = green;
            _Blue = blue;
            _HexValue = AsHex(red, green, blue);
        }

        public HTMLColor(uint uInt) {
            A = 255;
            R = Red(uInt);
            G = Green(uInt);
            B = Blue(uInt);
            _HexValue = AsHex(uInt);
        }
        public HTMLColor(string hex) {
            A = 255;
            R = FromHex(Red(hex));
            G = FromHex(Green(hex));
            B = FromHex(Blue(hex));
            _HexValue = hex;
        }
        private void setForeGround() {
            Foreground = new SolidColorBrush(Color.FromArgb(A, R, G, B));
        }
        public string ColorName {
            get {
                return GetValue(ColorNameProperty).ToString();
            }
            set {
                SetValue(ColorNameProperty, value);
            }
        }
        public byte A {
            get {
                return _Alpha;
            }
            set {
                _Alpha = value;
                setForeGround();
            }
        }
        public byte R {
            get {
                return _Red;
            }
            set {
                _Red = value;
                setForeGround();
            }
        }
        public byte B {
            get {
                return _Blue;
            }
            set {
                _Blue = value;
                setForeGround();
            }
        }
        public byte G {
            get {
                return _Green;
            }
            set {
                _Green = value;
                setForeGround();
            }
        }
        public string AsHex() {
            return AsHex(R, G, B);
        }
        public uint AsUInt {
            get {
                return GetUint(R, G, B);
            }
            set {
                R = Red(value);
                G = Green(value);
                B = Blue(value);
            }
        }

        public bool IsSelected {
            get {
                return _IsSelected;
            }
            set {
                _IsSelected = value;    
            }
        }
        public bool IsCustomColor {
            get {
                return _IsCustomColor;
            }
            set {
                _IsCustomColor = value;
            }
        }

        /// <summary>
        /// Color should be in the format of RRRGGGBBB 
        /// </summary>
        public static byte Red(uint uInt) {
            return (byte)(uInt / 1000000);
        }
        /// <summary>
        /// Color should be in the format of RRRGGGBBB 
        /// </summary>
        public static byte Green(uint uInt) {
            return (byte)(uInt / 1000 % 1000);
        }
        /// <summary>
        /// Color should be in the format of RRRGGGBBB 
        /// </summary>
        public static byte Blue(uint uInt) {
            return (byte)(uInt % 1000);
        }
        /// <summary>
        /// Color should be in the format of RRGGBB
        /// </summary>
        public static string Red(string hex) {
            return hex.Substring(0, 2);
        }
        /// <summary>
        /// Color should be in the format of RRGGBB 
        /// </summary>
        public static string Green(string hex) {
            return hex.Substring(2, 2);
        }
        /// <summary>
        /// Color should be in the format of RRGGBB 
        /// </summary>
        public static string Blue(string hex) {
            return hex.Substring(4, 2);
        }
        /// <summary>
        /// Color should be in the format of FF
        /// </summary>
        public static byte FromHex(string hex) {
            return (byte)(Convert.ToInt32(hex, 16));
        }

        /// <summary>
        /// Get Color in the format of RRRGGGBBB 
        /// </summary>
        public static uint GetUint(byte re, byte gr, byte bl) {
            return (uint)(re * 1000000 + gr * 1000 + bl);
        }

        /// <summary>
        /// Get Color in the Hex Format of RRGGBB 
        /// </summary>
        public static string AsHex(uint uInt) {
            return AsHex(Red(uInt), Green(uInt), Blue(uInt));
        }

        /// <summary>
        /// Get Color in the Hex Format of RRGGBB 
        /// </summary>
        public static string AsHex(byte re, byte gr, byte bl) {
            return AsHex(re) + AsHex(gr) + AsHex(bl);
        }
        public static string MakeSureIsTwo(string xStr) {
            if (xStr.Length < 2) xStr = "0" + xStr;
            return xStr;
        }

        /// <summary>
        /// Color should be in the format of RRR 
        /// </summary>
        public static string AsHex(byte byt) {
            return MakeSureIsTwo(Hex(byt));
    }

        public static string Hex(byte byt) {
            // convert to 2 digit hex from byte
            return string.Format("{0:X}", byt);
        }

        // /// <summary>
        // /// Color should be in the format of GGG 
        // /// </summary>
        //public static string xGreen(int As Byte) As String
        //    return MakeSureIsTwo(Hex(int))
        //}
        // /// <summary>
        // /// Color should be in the format of BBB 
        // /// </summary>
        //public static string xBlue(int As Byte) As String
        //    return MakeSureIsTwo(Hex(int))
        //}

        /// <summary>
        /// Color should be in the format of RRRGGGBBB 
        /// </summary>
        public static string xRed(uint uInt) {
            return AsHex(Red(uInt));
    }
    /// <summary>
    /// Color should be in the format of RRRGGGBBB 
    /// </summary>
    public static string xGreen(uint uInt) {
            return AsHex(Green(uInt));
        }
    /// <summary>
    /// Color should be in the format of RRRGGGBBB 
    /// </summary>
    public static string xBlue(uint uInt) {
            return AsHex(Blue(uInt));
        }

        public override string ToString() {
            return ColorName;
        }
    }
}