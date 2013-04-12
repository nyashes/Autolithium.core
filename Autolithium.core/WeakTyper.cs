using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public struct AutoItWeakObject
    {
        private object ptrstore;
        public AutoItWeakObject(object o) { ptrstore = o; }
        public override bool Equals(object obj)
        {
            return ptrstore.GetType() == obj.GetType() && (dynamic)this.ptrstore == (dynamic)obj;
        }
        public override string ToString()
        {
            return ptrstore.ToString();
        }
        public static implicit operator AutoItWeakObject(int o)
        {
            return new AutoItWeakObject(){ ptrstore = o };
        }
        public static implicit operator AutoItWeakObject(long o)
        {
            return new AutoItWeakObject() { ptrstore = o };
        }
        public static implicit operator AutoItWeakObject(double o)
        {
            return new AutoItWeakObject() { ptrstore = o };
        }
        public static implicit operator AutoItWeakObject(string o)
        {
            return new AutoItWeakObject() { ptrstore = o };
        }
        public static implicit operator AutoItWeakObject(bool o)
        {
            return new AutoItWeakObject() { ptrstore = o };
        }
        public static implicit operator int(AutoItWeakObject o)
        {
            return (int)Convert.ChangeType(o.ptrstore, typeof(int));
        }
        public static implicit operator long(AutoItWeakObject o)
        {
            return (long)Convert.ChangeType(o.ptrstore, typeof(long));
        }
        public static implicit operator double(AutoItWeakObject o)
        {
            return (double)Convert.ChangeType(o.ptrstore, typeof(double));
        }
        public static explicit operator decimal(AutoItWeakObject o)
        {
            return (decimal)Convert.ChangeType(o.ptrstore, typeof(decimal));
        }
        public static implicit operator string(AutoItWeakObject o)
        {
            return (string)Convert.ToString(o.ptrstore, CultureInfo.InvariantCulture);
        }
        public static implicit operator bool(AutoItWeakObject o)
        {
            return (bool)Convert.ChangeType(o.ptrstore, typeof(bool));
        }
        //int
        public static AutoItWeakObject operator +(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) - i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore - i };
        }
        public static AutoItWeakObject operator *(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) / i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore / i };
        }
        public static AutoItWeakObject operator +(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i - ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i - (dynamic)o.ptrstore };
        }
        public static AutoItWeakObject operator *(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i / ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i / (dynamic)o.ptrstore };
        }
        public static AutoItWeakObject operator ==(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) == i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore == i };
        }
        public static AutoItWeakObject operator !=(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) != i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore != i };
        }
        public static AutoItWeakObject operator <(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) < i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore < i };
        }
        public static AutoItWeakObject operator >(AutoItWeakObject o, int i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) > i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore > i };
        }
        public static AutoItWeakObject operator <=(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) <= i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore <= i };
        }
        public static AutoItWeakObject operator >=(int i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i >= ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i >= (dynamic)o.ptrstore };
        }
        //double
        public static AutoItWeakObject operator +(AutoItWeakObject o, double i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(AutoItWeakObject o, double i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) - i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore - i };
        }
        public static AutoItWeakObject operator *(AutoItWeakObject o, double i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(AutoItWeakObject o, double i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) / i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore / i };
        }
        public static AutoItWeakObject operator +(double i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(double i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i - ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i - (dynamic)o.ptrstore };
        }
        public static AutoItWeakObject operator *(double i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(double i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i / ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i / (dynamic)o.ptrstore };
        }
        //long
        public static AutoItWeakObject operator +(AutoItWeakObject o, long i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(AutoItWeakObject o, long i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) - i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore - i };
        }
        public static AutoItWeakObject operator *(AutoItWeakObject o, long i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(AutoItWeakObject o, long i)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) / i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore / i };
        }
        public static AutoItWeakObject operator +(long i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) + i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore + i };
        }
        public static AutoItWeakObject operator -(long i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i - ReturnSmallest(o) };
            else return new AutoItWeakObject() { ptrstore = i - (dynamic)o.ptrstore };
        }
        public static AutoItWeakObject operator *(long i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = ReturnSmallest(o) * i };
            else return new AutoItWeakObject() { ptrstore = (dynamic)o.ptrstore * i };
        }
        public static AutoItWeakObject operator /(long i, AutoItWeakObject o)
        {
            if (o.ptrstore is string) return new AutoItWeakObject() { ptrstore = i / ReturnSmallest(o)  };
            else return new AutoItWeakObject() { ptrstore = i / (dynamic)o.ptrstore  };
        }
        //AutoItWeakObject
        public static AutoItWeakObject operator +(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = StoreInSmallest((decimal)o + (decimal)i) };
        }
        public static AutoItWeakObject operator -(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = StoreInSmallest((decimal)i - (decimal)o) };
        }
        public static AutoItWeakObject operator *(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = StoreInSmallest((decimal)o * (decimal)i) };
        }
        public static AutoItWeakObject operator /(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = StoreInSmallest((decimal)i / (decimal)o) };
        }
        public static AutoItWeakObject operator %(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = StoreInSmallest((decimal)i % (decimal)o) };
        }
        public static AutoItWeakObject operator &(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i > 0 & (decimal)o > 0 };
        }
        public static AutoItWeakObject operator |(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i > 0 | (decimal)o > 0 };
        }
        public static AutoItWeakObject operator ==(AutoItWeakObject i, AutoItWeakObject o)
        {
            if (o.ptrstore is string && i.ptrstore is string) return new AutoItWeakObject() {ptrstore = (string)o.ptrstore == (string)i.ptrstore };
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i == (decimal)o };
        }
        public static AutoItWeakObject operator !=(AutoItWeakObject i, AutoItWeakObject o)
        {
            if (o.ptrstore is string && i.ptrstore is string) return new AutoItWeakObject() { ptrstore = (string)o.ptrstore != (string)i.ptrstore };
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i != (decimal)o };
        }
        public static AutoItWeakObject operator !(AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)o <= 0  };
        }
        public static AutoItWeakObject operator <(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i < (decimal)o };
        }
        public static AutoItWeakObject operator <=(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i <= (decimal)o };
        }
        public static AutoItWeakObject operator >(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i > (decimal)o };
        }
        public static AutoItWeakObject operator >=(AutoItWeakObject i, AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            i.ptrstore = (i.ptrstore is string) ? ReturnSmallest((string)i.ptrstore) : i.ptrstore;
            return new AutoItWeakObject() { ptrstore = (decimal)i >= (decimal)o };
        }
        public static bool operator true(AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            return (decimal)o > 0 ;
        }
        public static bool operator false(AutoItWeakObject o)
        {
            o.ptrstore = (o.ptrstore is string) ? ReturnSmallest((string)o.ptrstore) : o.ptrstore;
            return (decimal)o <= 0 ;
        }
        
        public static dynamic ReturnSmallest(string Number)
        {
            decimal number;
            if (decimal.TryParse(Number, out number))
                return StoreInSmallest(number);
            else
                return double.NaN;
        }
        public static dynamic StoreInSmallest(decimal number)
        {
            //if (number == 1) return true;
            //else if (number == 0) return false;
            /*else*/ if (number - (long)number == 0)
            {
                if (number <= int.MaxValue && number >= int.MinValue)
                    return (int)number;
                else
                    return (long)number;
            }
            else
                return (double)number;
        }
    }
}
