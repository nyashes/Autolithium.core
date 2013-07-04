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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public static class Scope
    {
        public static int HasVar(this IScope S, string Name, Type Desired, bool Recursive = true)
        {
            int count = 0;
            foreach (var e in S)
            {
                if (e.HasVar(Name, Desired)) return count;
                ++count;
            }
            return -1;
        }

        public static int HasVar(this IScope S, string Name, bool Recursive = true)
        {
            int count = 0;
            foreach (var e in S)
            {
                if (e.HasVar(Name)) return count;
                ++count;
            }
            return -1;
        }

        public static Expression GetVarCached(this IScope S, string Name, Type Desired, bool Recursive = true)
        {
            bool MustUpdate = false;
            var C = S.HasVar(Name, Desired, Recursive);
            if (C == -1) { C = S.HasVar(Name, Recursive); MustUpdate = true; }
            if (C == -1) throw new AutoitException(AutoitExceptionType.MISSINGVAR, -1, -1, Name);

            var Sc = S.ElementAt(C);
            if (MustUpdate)
            {
                var ActualValue = Sc.GetVar(Name, null);
                if (Desired == null) return ActualValue;

                Sc.DeclareVar(Name, Desired);
                return Expression.Block(
                    Sc.SetVar(Name, ActualValue.ConvertTo(Desired)),
                    Sc.GetVar(Name, Desired));
            }
            else return Sc.GetVar(Name, Desired);
        }

        public static Expression SetVarCached(this IScope S, string Name, Expression E, bool Recursive = true)
        {
            var C = S.HasVar(Name, E.Type, Recursive);
            IScope Sc;
            if (C == -1)
                C = S.HasVar(Name, Recursive);
            if (C == -1)
            {
                Sc = S;
                Sc.DeclareVar(Name, E.Type);
            }
            else
            {
                Sc = S.ElementAt(C);
                if (!Sc.OnDateType.ContainsKey(Name)) Sc.OnDateType.Add(Name, new List<Type>());
                else Sc.OnDateType[Name].Clear();
                if (!Sc.HasVar(Name, E.Type)) Sc.DeclareVar(Name, E.Type);
                Sc.OnDateType[Name].Add(E.Type);
            }

            return Sc.SetVar(Name, E);
        }

        public static Expression CallFuncSmartly(this IScope S, string Name, IEnumerable<Expression> Arguments)
        {
            foreach (var scope in S)
            {
                IEnumerable<FunctionMeta> Try, Candidates = scope.ScopeFunctions.Where(x => x.Name.ToUpper() == Name.ToUpper());
                if (!Candidates.Any()) continue;

                Try = Candidates.Where(x => x.Parameters.Select(y => y.ArgType).SequenceEqual(Arguments.Select(y => y.Type)));
                if (!Try.Any()) Try = Candidates.Where(x => x.Parameters.Count() == Arguments.Count());
                if (!Try.Any()) Try = Candidates.Where(x => x.Parameters.Count() >= Arguments.Count() || (x.Parameters.Last().ArgType.IsArray));
                if (!Try.Any()) continue;
                var Selected = Try.First();

                Arguments = Arguments.Zip(Selected.Parameters, (x, y) => x.ConvertTo(y.ArgType));

                int CountDiff = Selected.Parameters.Count() - Arguments.Count();
                if (CountDiff > 0) Arguments = Arguments.Concat(Enumerable.Repeat(Expression.Constant(null, typeof(object)), CountDiff));
                else if (CountDiff < 0) Arguments = Arguments.Take(Selected.Parameters.Count() - 1).Concat(
                    new Expression[] { Expression.NewArrayBounds(Selected.Parameters.Last().ArgType.GetElementType(), Arguments.Skip(Selected.Parameters.Count() - 1)) });
                return scope.CallFunc(Selected, Arguments);
            }
            throw new AutoitException(AutoitExceptionType.NOFUNCMATCH, -1, -1, Name + "(" + string.Join(", ", Arguments.Select(x => x.Type.ToString()) + ")"));
        }
    }
}
