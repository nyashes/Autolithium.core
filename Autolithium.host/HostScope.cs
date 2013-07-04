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
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Autolithium.core;
using System.Reflection;

namespace Autolithium.host
{
    public class HostScope : Autolithium.core.VScope
    {
        private static List<HostScope> ObjectList = new List<HostScope>();
        public static readonly FieldInfo ObjectListField = typeof(HostScope).GetTypeInfo().DeclaredFields.First(x => x.Name == "ObjectList");
        private int ThisIndex;
        private readonly Expression ThisAccess;

        public Dictionary<string, Delegate> Functions = new Dictionary<string, Delegate>();
        public static readonly FieldInfo FunctionsField = typeof(HostScope).GetRuntimeField("Functions");


        public Dictionary<string, dynamic> Variables = new Dictionary<string, dynamic>();
        public static readonly FieldInfo VariablesField = typeof(HostScope).GetRuntimeField("Variables");


        public HostScope() : base(null)
        {
            ThisIndex = ObjectList.Count;
            ThisAccess = Expression.MakeIndex(
                            Expression.MakeMemberAccess(null, ObjectListField),
                            ObjectListField.FieldType.GetRuntimeProperty("Item"),
                            new Expression[] { Expression.Constant(ThisIndex, typeof(int)) });
            ObjectList.Add(this);
        }

        public override object DeclareFunc(string Name, Type Return, IEnumerable<ArgumentMeta> Parameters)
        {
            Functions.Add(Name, null);
            this.ScopeFunctions.Add(new FunctionMeta()
            {
                Name = Name,
                ReturnType = Return,
                Parameters = Parameters,
                //Information = Functions[Name]
            });
            return this.ScopeFunctions.Count - 1;
        }

        public override Expression CallFunc(FunctionMeta Name, IEnumerable<Expression> Parameters)
        {
            //Functions[Name]
            //var Info = (AssemblyFunction)Name.Information;
            return Expression.Invoke(
                Expression.MakeIndex(
                    Expression.MakeMemberAccess(
                        ThisAccess
                    , FunctionsField), 
                    FunctionsField.FieldType.GetRuntimeProperty("Item"), 
                    new Expression[] {Expression.Constant(Name.Name, typeof(string))}),
                Parameters);

        }

        public override void DefineFunc(object ID, System.Linq.Expressions.LambdaExpression Corpse)
        {
            Functions[ScopeFunctions[(int)ID].Name] = Corpse.Compile();
        }

        public override void DeclareVar(string Name, Type BaseT)
        {
            if (ScopeVariables.Any(x => __GetName(x) == Name)) return;
            ScopeVariables.Add(Expression.MakeIndex(
                    Expression.MakeMemberAccess(
                        ThisAccess
                    , VariablesField),
                    VariablesField.FieldType.GetRuntimeProperty("Item"),
                    new Expression[] { Expression.Constant(Name, typeof(string)) }));
        }

        private static string __GetName(Expression x) { return (string)((x as IndexExpression).Arguments.First() as ConstantExpression).Value; }
        public override System.Linq.Expressions.Expression GetVar(string Name, Type Desired)
        {
                
            //if (Desired == null)
                return this.ScopeVariables
                    .First(x => __GetName(x) == Name).ConvertTo(Desired ?? typeof(object));

            //return this.ScopeVariables
            //    .First(x => __GetName(x) == Name && x.Type == Desired);
        }

        public override System.Linq.Expressions.Expression SetVar(string Name, System.Linq.Expressions.Expression E)
        {
            return Expression.Assign(this.ScopeVariables
                    .First(x => __GetName(x) == Name), Expression.Convert(E, typeof(object)));
        }

        public override bool HasVar(string Name, Type T)
        {
            return false;// HasVar(Name);
        }

        public override bool HasVar(string Name)
        {
            return this.ScopeVariables
               .Any(x => __GetName(x) == Name);
        }
    }
}
