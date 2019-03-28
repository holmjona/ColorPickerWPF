using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPickerWPF {
    /// <summary>
    /// Interaction logic for ValueSlider.xaml
    /// </summary>
    public partial class ValueSlider : UserControl {
        private Byte _Value;
        private Color _BaseColor;
        private String _Text;
        public HTMLColorValue LinkedValue;
        private MainWindow _Parent;
        public static Boolean IgnoreChangeEvent = false;
        public ValueSlider() {
            InitializeComponent();
            sldValue.ValueChanged += sldValue_ValueChanged;
            txtDec.GotFocus += txt_GotFocus;
            txtHex.GotFocus += txt_GotFocus;
            txtHex.KeyUp += txtHex_KeyUp;
            txtDec.KeyUp += txtDec_KeyUp;
        }

        public byte Value {
            get {
                return _Value;
            }
            set {
                _Value = value;
                SetText(_Value);
                SetSlider(_Value);
                if (_Parent != null) Parent.updateDisplayColor();
            }
        }
        public Color BaseColor {
            get {
                return _BaseColor;
            }
            set {
                _BaseColor = value;
                if (value == gsBaseColorNot.Color) {
                    gsBaseColorNot.Color = Colors.White;
                }
                Color newColor = Color.FromArgb(value.A,
                    (byte)(value.R * 0.8),
                    (byte)(value.G * 0.8),
                    (byte)(value.B * 0.8));
                tbTitle.Foreground = new SolidColorBrush(newColor);
                brdContainer.BorderBrush = new SolidColorBrush(newColor);
                brdContainer.BorderThickness = new Thickness(3.0);
                brdContainer.Background = new SolidColorBrush(
                    Color.FromArgb(50, value.R, value.G, value.B));
                gsBaseColor.Color = value;
            }
        }
        public string Text {
            get {
                SetSlider(Value);
                return _Text;
            }
            set {
                _Text = value;
                tbTitle.Text = value;
            }
        }
        public MainWindow Parent {
            get {
                return _Parent;
            }
            set {
                _Parent = value;
            }
        }
        private void sldValue_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<Double> e) {// Handles sldValue.ValueChanged
            byte val = (byte)sldValue.Value;
            tbPercent.Text = String.Format("{1:##%}\r\n{0}\r\n{2:X2}",val, (val / 255.0),val);
            if (LinkedValue != null) LinkedValue.Value = val;
            if (txtHex.Text != HTMLColor.Hex(val) || txtDec.Text != sldValue.Value.ToString()) {
                SetText(val);
                //SetBackColor(val)
            }
            if (!IgnoreChangeEvent && Parent != null) Parent.updateDisplayColor();
        }
        private void SetBackColor(byte val) {
            byte offByte = (byte)(255 - val);
            Color clr = Color.FromArgb(50, offByte, offByte, offByte);
            if (Text == "Red") {
                clr.R = val;
            } else if (Text == "Green") {
                clr.G = val;
            } else if (Text == "Blue") {
                clr.B = val;
            } else { //'Alpha"
                clr.A = (byte)(val / 255.0 * 50);
            }
            brdContainer.Background = new SolidColorBrush(clr);
        }
        private void SetText(byte value) {
            string newText = HTMLColor.MakeSureIsTwo(HTMLColor.Hex(value));
            //If newText.Length < 2 Then newText = "0" & newText
            txtHex.Text = newText;
            txtDec.Text = value.ToString();
        }
        private void SetSlider(byte val) {
            sldValue.Value = val;
        }

        private void txt_GotFocus(object sender, System.Windows.RoutedEventArgs e) {// Handles txtDec.GotFocus, txtHex.GotFocus
            TextBox txt = (TextBox)sender;
            txt.SelectionStart = 0;
            txt.SelectionLength = txt.Text.Length;
            //If tabWasPressed Then
            //    Parent.tabOff(sender, Me)
            //    tabWasPressed = False
            //}
        }

        private void txtDec_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {// Handles txtDec.KeyUp
            byte newByte;

            if (byte.TryParse(txtDec.Text, out newByte)) {
                Value = newByte;
                VisualStateManager.GoToState(txtDec, "Valid", true);
            } else {
                VisualStateManager.GoToState(txtDec, "InvalidUnfocused", true);

            }
            //If e.Key = Key.Tab Then
            //    tabWasPressed = True
            //    Parent.tabOff(sender, Me)
            //}
        }
        private List<Key> _GoodHexKeys = new List<Key>()  {
                        Key.D1, Key.D2, Key.D3, Key.D4, Key.D5,
                        Key.D6, Key.D7, Key.D8, Key.D9, Key.D0,
                        Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5,
                        Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9, Key.NumPad0,
                        Key.A, Key.B, Key.C, Key.D, Key.E, Key.F,
                        Key.Back, Key.Tab};
        private void txtHex_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) { // Handles txtHex.KeyUp
            if (!_GoodHexKeys.Contains(e.Key)) {
                //delete last char
                txtHex.Text = txtHex.Text.Substring(0, txtHex.Text.Length - 1);
            }

            string textToUse = txtHex.Text;
            if (textToUse.Length > 0 && textToUse[0] == '0') textToUse = textToUse.Substring(1);
            if (textToUse.Length == 0) textToUse = "0";
            if (textToUse.Length < 3) {
                byte newByte = HTMLColor.FromHex(textToUse);
                Value = newByte;
                VisualStateManager.GoToState(txtHex, "Valid", true);
                txtHex.SelectionStart = txtHex.Text.Length;
            } else {
                VisualStateManager.GoToState(txtHex, "InvalidUnfocused", true);
            }
            //If e.Key = Key.Tab Then
            //    tabWasPressed = True
            //    Parent.tabOff(sender, Me)
            //}
        }
        //private tabWasPressed As Boolean = False


    }
}
