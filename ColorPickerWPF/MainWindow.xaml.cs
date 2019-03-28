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
using System.Drawing;

namespace ColorPickerWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private List<HTMLColor> _WebSafeColors;
        private List<HTMLColor> _NamedColors;
        private Dictionary<String, String> _ColorsNamed;
        private HTMLColorValue _bytRed = new HTMLColorValue(255);
        private HTMLColorValue _bytGreen = new HTMLColorValue(255);
        private HTMLColorValue _bytBlue = new HTMLColorValue(255);
        private HTMLColorValue _bytAlpha = new HTMLColorValue(255);
        private HTMLColor _CustomColor;

        public MainWindow() {
            InitializeComponent();
            txtHexValue.GotFocus += txt_GotFocus;
            txtARGBValue.GotFocus += txt_GotFocus;
            txtRGBAValue.GotFocus += txt_GotFocus;
            cmbColorNames.SelectionChanged += cmbColorNames_SelectionChanged;
            vSldAlpha.Parent = vSldBlue.Parent = vSldGreen.Parent = vSldRed.Parent = this;
            FillColorLists();
            FillColors();
            FillSafeColorsGrid();
        }
        private void FillColorLists() {
            FillSafeColorsList();
            FillNamedColors();
            FillNamedColorsList();
        }
        private void FillColors() {
            _CustomColor = new HTMLColor(0, 0, 0);
            _CustomColor.ColorName = "Custom";
            _CustomColor.IsCustomColor = true;
            _CustomColor.A = 0;
            cmbColorNames.Items.Add(_CustomColor);
            cmbColorNames.SelectedItem = _CustomColor;
            foreach (HTMLColor hColor in _NamedColors)
                cmbColorNames.Items.Add(hColor);
            SetLinkage(vSldRed, _bytRed, this);
            SetLinkage(vSldGreen, _bytGreen, this);
            SetLinkage(vSldBlue, _bytBlue, this);
            SetLinkage(vSldAlpha, _bytAlpha, this);
            vSldAlpha.Value = 255;
        }
        private void UnselectAllColors() {
            foreach (HTMLColor hColor in _NamedColors) {
                hColor.IsSelected = false;
            }
            foreach (HTMLColor hColor in _WebSafeColors) {
                hColor.IsSelected = false;
            }
        }

        private void SelectColor(Color newColor) {
            uint UIntColor = HTMLColor.GetUint(newColor.R, newColor.G, newColor.B);
            foreach (HTMLColor hColor in _NamedColors) {
                if (hColor.AsUInt == UIntColor && hColor.A != 0) {
                    hColor.IsSelected = true;
                    break;
                }
            }
            foreach (HTMLColor hColor in _WebSafeColors) {
                if (hColor.AsUInt == UIntColor && hColor.A != 0) {
                    hColor.IsSelected = true;
                    break;
                }
            }
        }

        private void SetLinkage(ValueSlider sld, HTMLColorValue refByte, MainWindow par) {
            sld.Parent = par;
            sld.LinkedValue = refByte;
            sld.Value = 0;
        }
        private void txt_GotFocus(object sender, System.Windows.RoutedEventArgs e) {//    Handles txtHexValue.GotFocus, txtARGBValue.GotFocus, txtRGBAValue.GotFocus 
            TextBox txt = (TextBox)sender;
            txt.SelectionStart = 0;
            txt.SelectionLength = txt.Text.Length;
        }

        private void cmbColorNames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { //Handles cmbColorNames.SelectionChanged
            HTMLColor selectedItem = (HTMLColor)cmbColorNames.SelectedItem;
            if (!selectedItem.IsCustomColor) {
                Color selectedColor = ((SolidColorBrush)selectedItem.Foreground).Color;

                ValueSlider.IgnoreChangeEvent = true;
                vSldAlpha.Value = selectedColor.A;
                vSldRed.Value = selectedColor.R;
                vSldGreen.Value = selectedColor.G;
                vSldBlue.Value = selectedColor.B;
                ValueSlider.IgnoreChangeEvent = false;
            }
            updateDisplayColor();
        }

        public void updateDisplayColor() {
            Color newColor = Color.FromArgb(_bytAlpha.Value, _bytRed.Value, _bytGreen.Value, _bytBlue.Value);
            rctColor.Fill = new SolidColorBrush(newColor);
            UnselectAllColors();
            SelectColor(newColor);
            HTMLColor selectedColor;
            selectedColor = PickColorFromList();
            if (selectedColor.IsCustomColor) {
                selectedColor = SelectWebSafeColor();
            } else {
                SelectWebSafeColor();
            }
            if (selectedColor.IsCustomColor) {
                selectedColor = new HTMLColor(newColor.R, newColor.G, newColor.B);
                selectedColor.ColorName = _CustomColor.ColorName;
                selectedColor.A = newColor.A;
            }

            tbColorNameBlack.Text = selectedColor.ColorName;
            tbColorNameWhite.Text = selectedColor.ColorName;



            HTMLColor colorForFields = new HTMLColor(selectedColor.R, selectedColor.G, selectedColor.B);
            colorForFields.A = newColor.A;
            FillTextFields(colorForFields);



        }
        private void FillTextFields(HTMLColor col) {
            txtHexValue.Text = "#" + col.AsHex();
            txtHexWithAlphaValue.Text = "#" + HTMLColor.AsHex(col.A) + col.AsHex();
            txtARGBValue.Text = String.Format("argb({0},{1},{2},{3})", col.A, col.R, col.G, col.B);
            txtRGBAValue.Text = String.Format("rgba({1},{2},{3},{0})", (col.A / 255).ToString("#.##"), col.R, col.G, col.B);

            string aShort = HTMLColor.AsHex(col.A);
            string rShort = HTMLColor.AsHex(col.R);
            string gShort = HTMLColor.AsHex(col.G);
            string bShort = HTMLColor.AsHex(col.B);
            bool canBeShort = true;
            canBeShort = canBeShort && rShort[0] == rShort[1];
            canBeShort = canBeShort && gShort[0] == gShort[1];
            canBeShort = canBeShort && bShort[0] == bShort[1];
            if (canBeShort)
                txtHexShortValue.Text = String.Format("#{0}{1}{2}", rShort[0], gShort[0], bShort[0]);
            else
                txtHexShortValue.Text = txtHexValue.Text;


            canBeShort = canBeShort && aShort[0] == aShort[1];
            if (canBeShort)
                txtHexWithAlphaShortValue.Text = String.Format("#{0}{1}{2}{3}", aShort[0], rShort[0], gShort[0], bShort[0]);
            else
                txtHexWithAlphaShortValue.Text = txtHexWithAlphaValue.Text;

        }

        private HTMLColor PickColorFromList() {
            HTMLColor colorFound = null;
            foreach (HTMLColor hColor in cmbColorNames.Items) {
                if (hColor.IsSelected) {
                    colorFound = hColor;
                    break;
                }
            }
            if (colorFound == null) {
                colorFound = _CustomColor;
            }
            cmbColorNames.SelectedItem = colorFound;
            return colorFound;
        } // Function
        private void btnWebSafeColor_Click(object sender, EventArgs e) {
            Button btn = (Button)sender;
            HTMLColor hColor = (HTMLColor)btn.Tag;
            ValueSlider.IgnoreChangeEvent = true;
            vSldAlpha.Value = hColor.A;
            vSldRed.Value = hColor.R;
            vSldGreen.Value = hColor.G;
            vSldBlue.Value = hColor.B;
            ValueSlider.IgnoreChangeEvent = false;
            updateDisplayColor();
        }
        private void FillSafeColorsGrid() {
            int colCount = -1;
            int rowCount = 0;
            foreach (HTMLColor hColor in _WebSafeColors) {
                Button btn = new Button();
                btn.Content = hColor;
                btn.Tag = hColor;
                ToolTipService.SetToolTip(btn, "#" + hColor.AsHex());
                btn.Style = (Style)App.Current.Resources["htmlColorButton"];
                btn.Click += btnWebSafeColor_Click;

                if (colCount > 0 && colCount % 11 == 0) {
                    colCount = -1;
                    rowCount += 1;
                }
                colCount += 1;
                Grid.SetColumn(btn, colCount);
                Grid.SetRow(btn, rowCount);
                grdWebSafeColors.Children.Add(btn);
            }
        }
        private HTMLColor SelectWebSafeColor() {
            HTMLColor foundColor = _CustomColor;
            foreach (object obj in grdWebSafeColors.Children) {
                if (obj.GetType() == typeof(Button)) {
                    Button btn = (Button)obj;
                    HTMLColor hColor = (HTMLColor)btn.Tag;
                    if (hColor.IsSelected) {
                        foundColor = hColor;
                        VisualStateManager.GoToState(btn, "Selected", false);
                    } else {
                        VisualStateManager.GoToState(btn, "Unselected", false);
                    }
                }
            }
            return foundColor;
        } // Function
        public void tabOff(object txtB, ValueSlider sld) {
            TextBox txt = (TextBox)txtB;
            bool isDecBox = txt.Name.Contains("Dec");
            //vSldGreen.txtHex.IsTabStop = isDecBox
            //vSldBlue.txtHex.IsTabStop = isDecBox
            //vSldAlpha.txtHex.IsTabStop = isDecBox
            //vSldRed.txtHex.IsTabStop = isDecBox

            //vSldRed.txtDec.IsTabStop = isDecBox
            //vSldGreen.txtDec.IsTabStop = isDecBox
            //vSldBlue.txtDec.IsTabStop = isDecBox
            //vSldAlpha.txtDec.IsTabStop = isDecBox


            //If txt.Name.Contains("Hex")) {
            //    Select Case sld.Text
            //        Case "Red"
            //            vSldGreen.txtHex.Focus()
            //            Exit Select
            //        Case "Green"
            //            vSldBlue.txtHex.Focus()
            //            Exit Select
            //        Case "Blue"
            //            vSldAlpha.txtHex.Focus()
            //            Exit Select
            //        Case Else // Alpha
            //            vSldRed.txtDec.Focus()
            //    } // Select
            //ElseIf txt.Name.Contains("Dec")) {
            //    Select Case sld.Text
            //        Case "Red"
            //            vSldGreen.txtDec.Focus()
            //            Exit Select
            //        Case "Green"
            //            vSldBlue.txtDec.Focus()
            //            Exit Select
            //        Case "Blue"
            //            vSldAlpha.txtDec.Focus()
            //            Exit Select
            //        Case Else // Alpha
            //            vSldRed.txtHex.Focus()
            //    } // Select

            //} 
        }
        //public void unselectAllWebSafeButtons()
        //    foreach (obj As Object In grdWebSafeColors.Children
        //        If obj.GetType Is GetType(Button)) {
        //            btn As Button = CType(obj, Button)
        //            //VisualStateManager.GoToState(btn, "", False)
        //            VisualStateManager.GoToState(btn, "Notselected", False)
        //        } 
        //    }
        //} 
        private void FillNamedColorsList() {
            _NamedColors = new List<HTMLColor>();
            foreach (KeyValuePair<string, string> nColor in _ColorsNamed) {
                HTMLColor hColor = new HTMLColor(nColor.Value);
                hColor.ColorName = nColor.Key;
                _NamedColors.Add(hColor);
            }
        }
        private void FillSafeColorsList() {
            _WebSafeColors = new List<HTMLColor>();
            _WebSafeColors.Add(new HTMLColor("000000"));
            _WebSafeColors.Add(new HTMLColor("000033"));
            _WebSafeColors.Add(new HTMLColor("000066"));
            _WebSafeColors.Add(new HTMLColor("000099"));
            _WebSafeColors.Add(new HTMLColor("0000CC"));
            _WebSafeColors.Add(new HTMLColor("0000FF"));
            _WebSafeColors.Add(new HTMLColor("003300"));
            _WebSafeColors.Add(new HTMLColor("003333"));
            _WebSafeColors.Add(new HTMLColor("003366"));
            _WebSafeColors.Add(new HTMLColor("003399"));
            _WebSafeColors.Add(new HTMLColor("0033CC"));
            _WebSafeColors.Add(new HTMLColor("0033FF"));
            _WebSafeColors.Add(new HTMLColor("006600"));
            _WebSafeColors.Add(new HTMLColor("006633"));
            _WebSafeColors.Add(new HTMLColor("006666"));
            _WebSafeColors.Add(new HTMLColor("006699"));
            _WebSafeColors.Add(new HTMLColor("0066CC"));
            _WebSafeColors.Add(new HTMLColor("0066FF"));
            _WebSafeColors.Add(new HTMLColor("009900"));
            _WebSafeColors.Add(new HTMLColor("009933"));
            _WebSafeColors.Add(new HTMLColor("009966"));
            _WebSafeColors.Add(new HTMLColor("009999"));
            _WebSafeColors.Add(new HTMLColor("0099CC"));
            _WebSafeColors.Add(new HTMLColor("0099FF"));
            _WebSafeColors.Add(new HTMLColor("00CC00"));
            _WebSafeColors.Add(new HTMLColor("00CC33"));
            _WebSafeColors.Add(new HTMLColor("00CC66"));
            _WebSafeColors.Add(new HTMLColor("00CC99"));
            _WebSafeColors.Add(new HTMLColor("00CCCC"));
            _WebSafeColors.Add(new HTMLColor("00CCFF"));
            _WebSafeColors.Add(new HTMLColor("00FF00"));
            _WebSafeColors.Add(new HTMLColor("00FF33"));
            _WebSafeColors.Add(new HTMLColor("00FF66"));
            _WebSafeColors.Add(new HTMLColor("00FF99"));
            _WebSafeColors.Add(new HTMLColor("00FFCC"));
            _WebSafeColors.Add(new HTMLColor("00FFFF"));
            _WebSafeColors.Add(new HTMLColor("330000"));
            _WebSafeColors.Add(new HTMLColor("330033"));
            _WebSafeColors.Add(new HTMLColor("330066"));
            _WebSafeColors.Add(new HTMLColor("330099"));
            _WebSafeColors.Add(new HTMLColor("3300CC"));
            _WebSafeColors.Add(new HTMLColor("3300FF"));
            _WebSafeColors.Add(new HTMLColor("333300"));
            _WebSafeColors.Add(new HTMLColor("333333"));
            _WebSafeColors.Add(new HTMLColor("333366"));
            _WebSafeColors.Add(new HTMLColor("333399"));
            _WebSafeColors.Add(new HTMLColor("3333CC"));
            _WebSafeColors.Add(new HTMLColor("3333FF"));
            _WebSafeColors.Add(new HTMLColor("336600"));
            _WebSafeColors.Add(new HTMLColor("336633"));
            _WebSafeColors.Add(new HTMLColor("336666"));
            _WebSafeColors.Add(new HTMLColor("336699"));
            _WebSafeColors.Add(new HTMLColor("3366CC"));
            _WebSafeColors.Add(new HTMLColor("3366FF"));
            _WebSafeColors.Add(new HTMLColor("339900"));
            _WebSafeColors.Add(new HTMLColor("339933"));
            _WebSafeColors.Add(new HTMLColor("339966"));
            _WebSafeColors.Add(new HTMLColor("339999"));
            _WebSafeColors.Add(new HTMLColor("3399CC"));
            _WebSafeColors.Add(new HTMLColor("3399FF"));
            _WebSafeColors.Add(new HTMLColor("33CC00"));
            _WebSafeColors.Add(new HTMLColor("33CC33"));
            _WebSafeColors.Add(new HTMLColor("33CC66"));
            _WebSafeColors.Add(new HTMLColor("33CC99"));
            _WebSafeColors.Add(new HTMLColor("33CCCC"));
            _WebSafeColors.Add(new HTMLColor("33CCFF"));
            _WebSafeColors.Add(new HTMLColor("33FF00"));
            _WebSafeColors.Add(new HTMLColor("33FF33"));
            _WebSafeColors.Add(new HTMLColor("33FF66"));
            _WebSafeColors.Add(new HTMLColor("33FF99"));
            _WebSafeColors.Add(new HTMLColor("33FFCC"));
            _WebSafeColors.Add(new HTMLColor("33FFFF"));
            _WebSafeColors.Add(new HTMLColor("660000"));
            _WebSafeColors.Add(new HTMLColor("660033"));
            _WebSafeColors.Add(new HTMLColor("660066"));
            _WebSafeColors.Add(new HTMLColor("660099"));
            _WebSafeColors.Add(new HTMLColor("6600CC"));
            _WebSafeColors.Add(new HTMLColor("6600FF"));
            _WebSafeColors.Add(new HTMLColor("663300"));
            _WebSafeColors.Add(new HTMLColor("663333"));
            _WebSafeColors.Add(new HTMLColor("663366"));
            _WebSafeColors.Add(new HTMLColor("663399"));
            _WebSafeColors.Add(new HTMLColor("6633CC"));
            _WebSafeColors.Add(new HTMLColor("6633FF"));
            _WebSafeColors.Add(new HTMLColor("666600"));
            _WebSafeColors.Add(new HTMLColor("666633"));
            _WebSafeColors.Add(new HTMLColor("666666"));
            _WebSafeColors.Add(new HTMLColor("666699"));
            _WebSafeColors.Add(new HTMLColor("6666CC"));
            _WebSafeColors.Add(new HTMLColor("6666FF"));
            _WebSafeColors.Add(new HTMLColor("669900"));
            _WebSafeColors.Add(new HTMLColor("669933"));
            _WebSafeColors.Add(new HTMLColor("669966"));
            _WebSafeColors.Add(new HTMLColor("669999"));
            _WebSafeColors.Add(new HTMLColor("6699CC"));
            _WebSafeColors.Add(new HTMLColor("6699FF"));
            _WebSafeColors.Add(new HTMLColor("66CC00"));
            _WebSafeColors.Add(new HTMLColor("66CC33"));
            _WebSafeColors.Add(new HTMLColor("66CC66"));
            _WebSafeColors.Add(new HTMLColor("66CC99"));
            _WebSafeColors.Add(new HTMLColor("66CCCC"));
            _WebSafeColors.Add(new HTMLColor("66CCFF"));
            _WebSafeColors.Add(new HTMLColor("66FF00"));
            _WebSafeColors.Add(new HTMLColor("66FF33"));
            _WebSafeColors.Add(new HTMLColor("66FF66"));
            _WebSafeColors.Add(new HTMLColor("66FF99"));
            _WebSafeColors.Add(new HTMLColor("66FFCC"));
            _WebSafeColors.Add(new HTMLColor("66FFFF"));
            _WebSafeColors.Add(new HTMLColor("990000"));
            _WebSafeColors.Add(new HTMLColor("990033"));
            _WebSafeColors.Add(new HTMLColor("990066"));
            _WebSafeColors.Add(new HTMLColor("990099"));
            _WebSafeColors.Add(new HTMLColor("9900CC"));
            _WebSafeColors.Add(new HTMLColor("9900FF"));
            _WebSafeColors.Add(new HTMLColor("993300"));
            _WebSafeColors.Add(new HTMLColor("993333"));
            _WebSafeColors.Add(new HTMLColor("993366"));
            _WebSafeColors.Add(new HTMLColor("993399"));
            _WebSafeColors.Add(new HTMLColor("9933CC"));
            _WebSafeColors.Add(new HTMLColor("9933FF"));
            _WebSafeColors.Add(new HTMLColor("996600"));
            _WebSafeColors.Add(new HTMLColor("996633"));
            _WebSafeColors.Add(new HTMLColor("996666"));
            _WebSafeColors.Add(new HTMLColor("996699"));
            _WebSafeColors.Add(new HTMLColor("9966CC"));
            _WebSafeColors.Add(new HTMLColor("9966FF"));
            _WebSafeColors.Add(new HTMLColor("999900"));
            _WebSafeColors.Add(new HTMLColor("999933"));
            _WebSafeColors.Add(new HTMLColor("999966"));
            _WebSafeColors.Add(new HTMLColor("999999"));
            _WebSafeColors.Add(new HTMLColor("9999CC"));
            _WebSafeColors.Add(new HTMLColor("9999FF"));
            _WebSafeColors.Add(new HTMLColor("99CC00"));
            _WebSafeColors.Add(new HTMLColor("99CC33"));
            _WebSafeColors.Add(new HTMLColor("99CC66"));
            _WebSafeColors.Add(new HTMLColor("99CC99"));
            _WebSafeColors.Add(new HTMLColor("99CCCC"));
            _WebSafeColors.Add(new HTMLColor("99CCFF"));
            _WebSafeColors.Add(new HTMLColor("99FF00"));
            _WebSafeColors.Add(new HTMLColor("99FF33"));
            _WebSafeColors.Add(new HTMLColor("99FF66"));
            _WebSafeColors.Add(new HTMLColor("99FF99"));
            _WebSafeColors.Add(new HTMLColor("99FFCC"));
            _WebSafeColors.Add(new HTMLColor("99FFFF"));
            _WebSafeColors.Add(new HTMLColor("CC0000"));
            _WebSafeColors.Add(new HTMLColor("CC0033"));
            _WebSafeColors.Add(new HTMLColor("CC0066"));
            _WebSafeColors.Add(new HTMLColor("CC0099"));
            _WebSafeColors.Add(new HTMLColor("CC00CC"));
            _WebSafeColors.Add(new HTMLColor("CC00FF"));
            _WebSafeColors.Add(new HTMLColor("CC3300"));
            _WebSafeColors.Add(new HTMLColor("CC3333"));
            _WebSafeColors.Add(new HTMLColor("CC3366"));
            _WebSafeColors.Add(new HTMLColor("CC3399"));
            _WebSafeColors.Add(new HTMLColor("CC33CC"));
            _WebSafeColors.Add(new HTMLColor("CC33FF"));
            _WebSafeColors.Add(new HTMLColor("CC6600"));
            _WebSafeColors.Add(new HTMLColor("CC6633"));
            _WebSafeColors.Add(new HTMLColor("CC6666"));
            _WebSafeColors.Add(new HTMLColor("CC6699"));
            _WebSafeColors.Add(new HTMLColor("CC66CC"));
            _WebSafeColors.Add(new HTMLColor("CC66FF"));
            _WebSafeColors.Add(new HTMLColor("CC9900"));
            _WebSafeColors.Add(new HTMLColor("CC9933"));
            _WebSafeColors.Add(new HTMLColor("CC9966"));
            _WebSafeColors.Add(new HTMLColor("CC9999"));
            _WebSafeColors.Add(new HTMLColor("CC99CC"));
            _WebSafeColors.Add(new HTMLColor("CC99FF"));
            _WebSafeColors.Add(new HTMLColor("CCCC00"));
            _WebSafeColors.Add(new HTMLColor("CCCC33"));
            _WebSafeColors.Add(new HTMLColor("CCCC66"));
            _WebSafeColors.Add(new HTMLColor("CCCC99"));
            _WebSafeColors.Add(new HTMLColor("CCCCCC"));
            _WebSafeColors.Add(new HTMLColor("CCCCFF"));
            _WebSafeColors.Add(new HTMLColor("CCFF00"));
            _WebSafeColors.Add(new HTMLColor("CCFF33"));
            _WebSafeColors.Add(new HTMLColor("CCFF66"));
            _WebSafeColors.Add(new HTMLColor("CCFF99"));
            _WebSafeColors.Add(new HTMLColor("CCFFCC"));
            _WebSafeColors.Add(new HTMLColor("CCFFFF"));
            _WebSafeColors.Add(new HTMLColor("FF0000"));
            _WebSafeColors.Add(new HTMLColor("FF0033"));
            _WebSafeColors.Add(new HTMLColor("FF0066"));
            _WebSafeColors.Add(new HTMLColor("FF0099"));
            _WebSafeColors.Add(new HTMLColor("FF00CC"));
            _WebSafeColors.Add(new HTMLColor("FF00FF"));
            _WebSafeColors.Add(new HTMLColor("FF3300"));
            _WebSafeColors.Add(new HTMLColor("FF3333"));
            _WebSafeColors.Add(new HTMLColor("FF3366"));
            _WebSafeColors.Add(new HTMLColor("FF3399"));
            _WebSafeColors.Add(new HTMLColor("FF33CC"));
            _WebSafeColors.Add(new HTMLColor("FF33FF"));
            _WebSafeColors.Add(new HTMLColor("FF6600"));
            _WebSafeColors.Add(new HTMLColor("FF6633"));
            _WebSafeColors.Add(new HTMLColor("FF6666"));
            _WebSafeColors.Add(new HTMLColor("FF6699"));
            _WebSafeColors.Add(new HTMLColor("FF66CC"));
            _WebSafeColors.Add(new HTMLColor("FF66FF"));
            _WebSafeColors.Add(new HTMLColor("FF9900"));
            _WebSafeColors.Add(new HTMLColor("FF9933"));
            _WebSafeColors.Add(new HTMLColor("FF9966"));
            _WebSafeColors.Add(new HTMLColor("FF9999"));
            _WebSafeColors.Add(new HTMLColor("FF99CC"));
            _WebSafeColors.Add(new HTMLColor("FF99FF"));
            _WebSafeColors.Add(new HTMLColor("FFCC00"));
            _WebSafeColors.Add(new HTMLColor("FFCC33"));
            _WebSafeColors.Add(new HTMLColor("FFCC66"));
            _WebSafeColors.Add(new HTMLColor("FFCC99"));
            _WebSafeColors.Add(new HTMLColor("FFCCCC"));
            _WebSafeColors.Add(new HTMLColor("FFCCFF"));
            _WebSafeColors.Add(new HTMLColor("FFFF00"));
            _WebSafeColors.Add(new HTMLColor("FFFF33"));
            _WebSafeColors.Add(new HTMLColor("FFFF66"));
            _WebSafeColors.Add(new HTMLColor("FFFF99"));
            _WebSafeColors.Add(new HTMLColor("FFFFCC"));
            _WebSafeColors.Add(new HTMLColor("FFFFFF"));

            foreach (HTMLColor thClr in _WebSafeColors) {
                thClr.ColorName = "WebSafe(#" + thClr.AsHex() + ")";
            }
        }
        private void FillNamedColors() {
            _ColorsNamed = new Dictionary<string, string>();
            _ColorsNamed.Add("AliceBlue", "F0F8FF");
            _ColorsNamed.Add("AntiqueWhite", "FAEBD7");
            _ColorsNamed.Add("Aqua", "00FFFF");
            _ColorsNamed.Add("Aquamarine", "7FFFD4");
            _ColorsNamed.Add("Azure", "F0FFFF");
            _ColorsNamed.Add("Beige", "F5F5DC");
            _ColorsNamed.Add("Bisque", "FFE4C4");
            _ColorsNamed.Add("Black", "000000");
            _ColorsNamed.Add("BlanchedAlmond", "FFEBCD");
            _ColorsNamed.Add("Blue", "0000FF");
            _ColorsNamed.Add("BlueViolet", "8A2BE2");
            _ColorsNamed.Add("Brown", "A52A2A");
            _ColorsNamed.Add("BurlyWood", "DEB887");
            _ColorsNamed.Add("CadetBlue", "5F9EA0");
            _ColorsNamed.Add("Chartreuse", "7FFF00");
            _ColorsNamed.Add("Chocolate", "D2691E");
            _ColorsNamed.Add("Coral", "FF7F50");
            _ColorsNamed.Add("CornflowerBlue", "6495ED");
            _ColorsNamed.Add("Cornsilk", "FFF8DC");
            _ColorsNamed.Add("Crimson", "DC143C");
            _ColorsNamed.Add("Cyan", "00FFFF");
            _ColorsNamed.Add("DarkBlue", "00008B");
            _ColorsNamed.Add("DarkCyan", "008B8B");
            _ColorsNamed.Add("DarkGoldenrod", "B8860B");
            _ColorsNamed.Add("DarkGray", "A9A9A9");
            _ColorsNamed.Add("DarkGreen", "006400");
            _ColorsNamed.Add("DarkKhaki", "BDB76B");
            _ColorsNamed.Add("DarkMagenta", "8B008B");
            _ColorsNamed.Add("DarkOliveGreen", "556B2F");
            _ColorsNamed.Add("DarkOrange", "FF8C00");
            _ColorsNamed.Add("DarkOrchid", "9932CC");
            _ColorsNamed.Add("DarkRed", "8B0000");
            _ColorsNamed.Add("DarkSalmon", "E9967A");
            _ColorsNamed.Add("DarkSeaGreen", "8FBC8F");
            _ColorsNamed.Add("DarkSlateBlue", "483D8B");
            _ColorsNamed.Add("DarkSlateGray", "2F4F4F");
            _ColorsNamed.Add("DarkTurquoise", "00CED1");
            _ColorsNamed.Add("DarkViolet", "9400D3");
            _ColorsNamed.Add("DeepPink", "FF1493");
            _ColorsNamed.Add("DeepSkyBlue", "00BFFF");
            _ColorsNamed.Add("DimGray", "696969");
            _ColorsNamed.Add("DodgerBlue", "1E90FF");
            _ColorsNamed.Add("Firebrick", "B22222");
            _ColorsNamed.Add("FloralWhite", "FFFAF0");
            _ColorsNamed.Add("ForestGreen", "228B22");
            _ColorsNamed.Add("Fuchsia", "FF00FF");
            _ColorsNamed.Add("Gainsboro", "DCDCDC");
            _ColorsNamed.Add("GhostWhite", "F8F8FF");
            _ColorsNamed.Add("Gold", "FFD700");
            _ColorsNamed.Add("Goldenrod", "DAA520");
            _ColorsNamed.Add("Gray", "808080");
            _ColorsNamed.Add("Green", "008000");
            _ColorsNamed.Add("GreenYellow", "ADFF2F");
            _ColorsNamed.Add("Honeydew", "F0FFF0");
            _ColorsNamed.Add("HotPink", "FF69B4");
            _ColorsNamed.Add("IndianRed", "CD5C5C");
            _ColorsNamed.Add("Indigo", "4B0082");
            _ColorsNamed.Add("Ivory", "FFFFF0");
            _ColorsNamed.Add("Khaki", "F0E68C");
            _ColorsNamed.Add("Lavender", "E6E6FA");
            _ColorsNamed.Add("LavenderBlush", "FFF0F5");
            _ColorsNamed.Add("LawnGreen", "7CFC00");
            _ColorsNamed.Add("LemonChiffon", "FFFACD");
            _ColorsNamed.Add("LightBlue", "ADD8E6");
            _ColorsNamed.Add("LightCoral", "F08080");
            _ColorsNamed.Add("LightCyan", "E0FFFF");
            _ColorsNamed.Add("LightGoldenrodYellow", "FAFAD2");
            _ColorsNamed.Add("LightGray", "D3D3D3");
            _ColorsNamed.Add("LightGreen", "90EE90");
            _ColorsNamed.Add("LightPink", "FFB6C1");
            _ColorsNamed.Add("LightSalmon", "FFA07A");
            _ColorsNamed.Add("LightSeaGreen", "20B2AA");
            _ColorsNamed.Add("LightSkyBlue", "87CEFA");
            _ColorsNamed.Add("LightSlateGray", "778899");
            _ColorsNamed.Add("LightSteelBlue", "B0C4DE");
            _ColorsNamed.Add("LightYellow", "FFFFE0");
            _ColorsNamed.Add("Lime", "00FF00");
            _ColorsNamed.Add("LimeGreen", "32CD32");
            _ColorsNamed.Add("Linen", "FAF0E6");
            _ColorsNamed.Add("Magenta", "FF00FF");
            _ColorsNamed.Add("Maroon", "800000");
            _ColorsNamed.Add("MediumAquamarine", "66CDAA");
            _ColorsNamed.Add("MediumBlue", "0000CD");
            _ColorsNamed.Add("MediumOrchid", "BA55D3");
            _ColorsNamed.Add("MediumPurple", "9370DB");
            _ColorsNamed.Add("MediumSeaGreen", "3CB371");
            _ColorsNamed.Add("MediumSlateBlue", "7B68EE");
            _ColorsNamed.Add("MediumSpringGreen", "00FA9A");
            _ColorsNamed.Add("MediumTurquoise", "48D1CC");
            _ColorsNamed.Add("MediumVioletRed", "C71585");
            _ColorsNamed.Add("MidnightBlue", "191970");
            _ColorsNamed.Add("MintCream", "F5FFFA");
            _ColorsNamed.Add("MistyRose", "FFE4E1");
            _ColorsNamed.Add("Moccasin", "FFE4B5");
            _ColorsNamed.Add("NavajoWhite", "FFDEAD");
            _ColorsNamed.Add("Navy", "000080");
            _ColorsNamed.Add("OldLace", "FDF5E6");
            _ColorsNamed.Add("Olive", "808000");
            _ColorsNamed.Add("OliveDrab", "6B8E23");
            _ColorsNamed.Add("Orange", "FFA500");
            _ColorsNamed.Add("OrangeRed", "FF4500");
            _ColorsNamed.Add("Orchid", "DA70D6");
            _ColorsNamed.Add("PaleGoldenrod", "EEE8AA");
            _ColorsNamed.Add("PaleGreen", "98FB98");
            _ColorsNamed.Add("PaleTurquoise", "AFEEEE");
            _ColorsNamed.Add("PaleVioletRed", "DB7093");
            _ColorsNamed.Add("PapayaWhip", "FFEFD5");
            _ColorsNamed.Add("PeachPuff", "FFDAB9");
            _ColorsNamed.Add("Peru", "CD853F");
            _ColorsNamed.Add("Pink", "FFC0CB");
            _ColorsNamed.Add("Plum", "DDA0DD");
            _ColorsNamed.Add("PowderBlue", "B0E0E6");
            _ColorsNamed.Add("Purple", "800080");
            _ColorsNamed.Add("Red", "FF0000");
            _ColorsNamed.Add("RosyBrown", "BC8F8F");
            _ColorsNamed.Add("RoyalBlue", "4169E1");
            _ColorsNamed.Add("SaddleBrown", "8B4513");
            _ColorsNamed.Add("Salmon", "FA8072");
            _ColorsNamed.Add("SandyBrown", "F4A460");
            _ColorsNamed.Add("SeaGreen", "2E8B57");
            _ColorsNamed.Add("SeaShell", "FFF5EE");
            _ColorsNamed.Add("Sienna", "A0522D");
            _ColorsNamed.Add("Silver", "C0C0C0");
            _ColorsNamed.Add("SkyBlue", "87CEEB");
            _ColorsNamed.Add("SlateBlue", "6A5ACD");
            _ColorsNamed.Add("SlateGray", "708090");
            _ColorsNamed.Add("Snow", "FFFAFA");
            _ColorsNamed.Add("SpringGreen", "00FF7F");
            _ColorsNamed.Add("SteelBlue", "4682B4");
            _ColorsNamed.Add("Tan", "D2B48C");
            _ColorsNamed.Add("Teal", "008080");
            _ColorsNamed.Add("Thistle", "D8BFD8");
            _ColorsNamed.Add("Tomato", "FF6347");
            _ColorsNamed.Add("Transparent", "00FFFFFF");
            _ColorsNamed.Add("Turquoise", "40E0D0");
            _ColorsNamed.Add("Violet", "EE82EE");
            _ColorsNamed.Add("Wheat", "F5DEB3");
            _ColorsNamed.Add("White", "FFFFFF");
            _ColorsNamed.Add("WhiteSmoke", "F5F5F5");
            _ColorsNamed.Add("Yellow", "FFFF00");
            _ColorsNamed.Add("YellowGreen", "9ACD32");

        }
    }
}
