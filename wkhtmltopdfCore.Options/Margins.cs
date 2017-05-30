using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace wkhtmltopdfCore.Options
{
    public class Margins
    {
        [OptionFlag("-B")]
        public int? Bottom;

        [OptionFlag("-L")]
        public int? Left;

        [OptionFlag("-R")]
        public int? Right;

        [OptionFlag("-T")]
        public int? Top;

        public Margins()
        {
        }

        public Margins(int top, int right, int bottom, int left)
        {
            this.Top = new int?(top);
            this.Right = new int?(right);
            this.Bottom = new int?(bottom);
            this.Left = new int?(left);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            FieldInfo[] fields = base.GetType().GetFields();
            FieldInfo[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo fieldInfo = array[i];
                OptionFlag optionFlag = fieldInfo.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault<object>() as OptionFlag;
                if (optionFlag != null)
                {
                    object value = fieldInfo.GetValue(this);
                    if (value != null)
                    {
                        stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", new object[]
                        {
                            optionFlag.Name,
                            value
                        });
                    }
                }
            }
            return stringBuilder.ToString().Trim();
        }
    }
}