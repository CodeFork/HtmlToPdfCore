using System;

namespace wkhtmltopdfCore.Options
{
    internal class OptionFlag : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public OptionFlag(string name)
        {
            this.Name = name;
        }
    }
}