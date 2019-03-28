namespace ColorPickerWPF {
    public class HTMLColorValue {
        private byte _Value;
        
        internal HTMLColorValue(byte val) {
            _Value = val;
        }

        public byte Value {
            get { return _Value; }
            set { _Value = value; }
        }
    }
}