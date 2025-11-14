using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCaptcha.Models
{
    internal class Tag
    {
        private int _clickCount = 0;

        private object _id = 0;

        public Tag(object id, int clickCount)
        {
            this._id = id;
            this._clickCount = clickCount;
        }

        public object Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public int ClickCount
        {
            get { return _clickCount; }
            set { _clickCount = value; }
        }
    }
}
