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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static partial class ExpressionTypeBeam
    {
        static GetVarDelegate _Get;
        static SetVarDelegate _Set;
        static CreateVarDelegate _Create;
        static bool GlobalCache;
        public static void InitializeParameterEngine(GetVarDelegate G, SetVarDelegate S, CreateVarDelegate CR, bool SetupGlobalCache = false)
        {
            _Get = G;
            _Set = S;
            _Create = CR;
            GlobalCache = SetupGlobalCache;
        }

        static Stack<Dictionary<VarAutExpression, List<Type>>> OnDateType = new Stack<Dictionary<VarAutExpression, List<Type>>>();
        static Stack<List<ParameterExpression>> Scope = new Stack<List<ParameterExpression>>();
        public static void PushScope() 
        {
            OnDateType.Push(new Dictionary<VarAutExpression, List<Type>>());
            Scope.Push(new List<ParameterExpression>());
        }
        public static List<ParameterExpression> PopScope() 
        {
            OnDateType.Pop();
            return Scope.Pop();
        }

        static KeyValuePair<VarAutExpression, List<Type>>[] SnapshotData;
        public static void TakeSnapshot()
        {
            SnapshotData = OnDateType.Peek().ToArray();
        }
        public static Expression RestoreSnapshot()
        {
            if (SnapshotData.SequenceEqual(OnDateType.Peek())) return null;
            var Ret = Expression.Block(SnapshotData.Except(OnDateType.Peek()).SelectMany(x => x.Value.Select(y => x.Key.Getter(y))));
            SnapshotData = null;
            return Ret;
        }
        

        public static Expression Getter(this VarAutExpression E, Type T)
        {
            Expression Exp = null, Global = null;
            if (E.Index != null) E.Index = E.Index.Select(x => x.ConvertTo(typeof(int))).ToList();
            if (E.IsGlobal ?? true)
            {
                Exp = _Get(E.Name, E.Index != null ? null : T);
                if (Exp != null && E.Index != null) return E.UnboxTo != null ?
                    (Expression)Expression.Convert(Expression.ArrayAccess(Exp, E.Index), E.UnboxTo).ConvertTo(T) :
                    (Expression)Expression.ArrayAccess(Exp, E.Index).ConvertTo(T);
                else if (E.Index != null) try
                    {
                        return E.UnboxTo != null ?
                            (Expression)Expression.Convert(Expression.ArrayAccess(_Get(E.Name, null), E.Index), E.UnboxTo).ConvertTo(T) :
                            (Expression)Expression.ArrayAccess(_Get(E.Name, null), E.Index).ConvertTo(T);
                    }
                    catch { }
                else if (Exp != null) return Exp;
                else if (!GlobalCache) try {return _Get(E.Name, null).ConvertTo(T);} catch{}
            }
            if (E.Index != null)
            {
                var e = Expression.ArrayAccess(Scope.Peek().FirstOrDefault(x => x.Name == E.Name), E.Index);
                if (E.UnboxTo != null) return Expression.Convert(e, E.UnboxTo).ConvertTo(T ?? E.UnboxTo);
                else return e.ConvertTo(T ?? typeof(object));
            }
            else if (T == null) return Scope.Peek().FirstOrDefault(x => x.Name == E.Name && OnDateType.Peek()[E].Contains(x.Type));
            else if (OnDateType.Peek()[E].Contains(T)) return Scope.Peek().FirstOrDefault(x => x.Name == E.Name && x.Type == T);
            
            if (Exp != null) Global = Exp;
            Exp = Scope.Peek().FirstOrDefault(x => x.Name == E.Name && x.Type == T);
            if (Exp == null) Scope.Peek().Add((ParameterExpression)(Exp = Expression.Parameter(T, E.Name)));
            else if (Exp.Type == T) return Exp;
            var Ret = Expression.Block(T,
                Expression.Assign(Exp, (Scope.Peek().LastOrDefault(x => x.Name == E.Name && OnDateType.Peek()[E].Contains(x.Type)) ?? Global).ConvertTo(Exp.Type)),
                Exp);
            OnDateType.Peek()[E].Add(T);
            return Ret;
        }

        public static Expression Setter(this VarAutExpression E, Expression T)
        {
            Expression Exp = null;


            if (E.Index != null) E.Index = E.Index.Select(x => x.ConvertTo(typeof(int))).ToList();

            if (OnDateType.Peek().Any(x => x.Key.Name == E.Name))
                OnDateType.Peek().First(x => x.Key.Name == E.Name).Value.Clear();
            else if (E.Index == null) OnDateType.Peek().Add(E, new List<Type>());

            if (E.IsGlobal ?? true)
            {
                Exp = _Get(E.Name, E.Index != null ? null : T.Type);
                if (Exp != null && E.Index != null) return E.UnboxTo != null ? 
                    Expression.Assign(Expression.ArrayAccess(Exp, E.Index), T.ConvertTo(E.UnboxTo).ConvertTo(Exp.Type.GetElementType())) :
                    Expression.Assign(Expression.ArrayAccess(Exp, E.Index), T.ConvertTo(Exp.Type.GetElementType()));
                else if (E.Index != null && _Get(E.Name, null) != null) return E.UnboxTo != null ?
                    Expression.Assign(Expression.ArrayAccess(_Get(E.Name, null), E.Index), T.ConvertTo(E.UnboxTo).ConvertTo(_Get(E.Name, null).Type.GetElementType())) :
                    Expression.Assign(Expression.ArrayAccess(_Get(E.Name, null), E.Index), T.ConvertTo(_Get(E.Name, null).Type.GetElementType()));
                else if (Exp != null) return _Set(E.Name, T);
                else if (!GlobalCache) try { return _Set(E.Name, T.ConvertTo(_Get(E.Name, null).Type)); }
                    catch { }
            }
            //else
            {
                if (E.Index == null) OnDateType.Peek()[E].Add(T.Type);
                Exp = Scope.Peek().FirstOrDefault(x => x.Name == E.Name && (x.Type == T.Type || E.Index != null));
                if (Exp != null) return Expression.Assign(E.Index != null ? Expression.ArrayAccess(Exp, E.Index) : Exp, 
                    E.Index != null ? (E.UnboxTo != null ? T.ConvertTo(E.UnboxTo).ConvertTo(Exp.Type.GetElementType()) : 
                    T.ConvertTo(Exp.Type.GetElementType())) : T);
            }
            Scope.Peek().Add((ParameterExpression)(Exp = Expression.Parameter(T.Type, E.Name)));
            return Expression.Assign(Exp, T);
        }

        public static Expression Generator(this VarAutExpression E)
        {
            ParameterExpression Exp;
            Expression Out = null;

            if (E.Index != null) E.Index = E.Index.Select(x => x.ConvertTo(typeof(int))).ToList();

            if (E.Index == null) Exp = Expression.Parameter(typeof(object), E.Name);
            else if (E.UnboxTo != null)
            {
                Exp = Expression.Parameter(E.UnboxTo.MakeArrayType(), E.Name);
                Out = Expression.Assign(Exp, Expression.NewArrayBounds(E.UnboxTo, E.Index));
            }
            else
            {
                Exp = Expression.Parameter(typeof(object[]), E.Name);
                Out = Expression.Assign(Exp, Expression.NewArrayBounds(typeof(object), E.Index));
            }

            if (E.IsGlobal ?? false) _Create(E.Name, Exp.Type);
            else Scope.Peek().Add(Exp);
            return Out;
        }
    }
}
