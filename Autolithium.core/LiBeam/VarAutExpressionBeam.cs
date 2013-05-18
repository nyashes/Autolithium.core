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
            if (E.IsGlobal ?? true)
            {
                Exp = _Get(E.Name, E.Index != null ? null : T);
                if (Exp != null && E.Index != null) return E.UnboxTo != null ?
                    (Expression)Expression.Convert(Expression.ArrayAccess(Exp, E.Index), E.UnboxTo).ConvertTo(T) :
                    (Expression)Expression.ArrayAccess(Exp, E.Index).ConvertTo(T);
                else if (E.Index != null) return E.UnboxTo != null ?
                    (Expression)Expression.Convert(Expression.ArrayAccess(_Get(E.Name, null), E.Index), E.UnboxTo).ConvertTo(T) :
                    (Expression)Expression.ArrayAccess(_Get(E.Name, null), E.Index).ConvertTo(T);
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
            
            var Ret = Expression.Block(T,
                Expression.Assign(Exp, (Scope.Peek().LastOrDefault(x => x.Name == E.Name && OnDateType.Peek()[E].Contains(x.Type)) ?? Global).ConvertTo(Exp.Type)),
                Exp);
            OnDateType.Peek()[E].Add(T);
            return Ret;
        }

        public static Expression Setter(this VarAutExpression E, Expression T)
        {
            Expression Exp = null;



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
