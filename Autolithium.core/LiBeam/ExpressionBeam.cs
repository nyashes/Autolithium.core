/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public static partial class ExpressionTypeBeam
    {
        //Required methods
        private static Expression InvariantCultureFieldAccess = Expression.MakeMemberAccess(null,
            typeof(CultureInfo).GetRuntimeProperty("InvariantCulture"));
        private static MethodInfo Convert_ToString(Type T)
        {
            try
            {
                return typeof(Convert)
                   .GetMethod("ToString", T, typeof(IFormatProvider));
            }
            catch
            {
                return typeof(Convert)
                    .GetMethod("ToString", typeof(object), typeof(IFormatProvider));
            }
        }
        private static MethodInfo Convert_ChangeType = typeof(Convert).GetMethod("ChangeType", typeof(object), typeof(Type));
        private static MethodInfo Numeric_Parse(Type Num)
        {
            return Num.GetMethod("Parse", typeof(string), typeof(IFormatProvider));
        }

        public static Expression ConvertTo(this Expression E, Type T)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(T);
            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(T);

            if (E.Type == T) return E;
            else if (T == typeof(string)) return E.ConvertToString();
            else if (NumericType.Contains(T)) return E.ConvertToNumber(T);
            else if (T == typeof(bool)) return E.ConvertToBool();
            else if (T == typeof(object)) return Expression.Convert(E, T);
            else return Expression.Convert(Expression.Call(Convert_ChangeType, Expression.Convert(E, typeof(object)), Expression.Constant(T, typeof(Type))), T);
        }

        public static Expression ConvertToNumber(this Expression E, Type Favorite = null)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(Favorite ?? typeof(double));
            if (NumericType.Contains(E.Type))
                if (E.Type == Favorite) return E;
                else if (Favorite != null) return Expression.Convert(E, Favorite);

            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(Favorite ?? typeof(double));

            if (NumericType.Contains(E.Type)) return (Favorite != null && E.Type != Favorite) ? Expression.Convert(E, Favorite) : E;
            else if (E.Type == typeof(string))
                return Expression.Call(Numeric_Parse(Favorite ?? typeof(double)), E, InvariantCultureFieldAccess);
            else if (E.Type == typeof(bool)) return Expression.IfThenElse(E, Expression.Constant(1, Favorite ?? typeof(int)), Expression.Constant(0, Favorite ?? typeof(int)));
            
            else return Expression.Convert(Expression.Call(Convert_ChangeType, Expression.Convert(E, typeof(object)), Expression.Constant(Favorite, typeof(Type))), Favorite);
        }

        public static Expression ConvertToString(this Expression E)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(typeof(string));
            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(typeof(string));

            if (E.Type == typeof(string)) return E;
            else return Expression.Call(Convert_ToString(E.Type), E, InvariantCultureFieldAccess);
        }

        public static Expression ConvertToBool(this Expression E)
        {
            if (E.Type == typeof(bool)) return E;
            else if (NumericType.Contains(E.Type)) return Expression.GreaterThan(E, Expression.Constant(0, typeof(int)));
            else if (E.Type == typeof(string)) return Expression.NotEqual(Expression.Constant("", typeof(string)), E);
            else return Expression.NotEqual(E, Expression.Constant(null));
        }
    }
}
