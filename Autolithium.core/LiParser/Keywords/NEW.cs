using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression ParseKeyword_NEW(string Keyword)
        {
            ConsumeWS();
            string Name = Getstr(Reg_Any);
            
            List<Expression> Arguments = ParseArgExpList();
            var Params = Arguments.Select(x => x.Type).ToList();

            var Dest = Included.SelectMany(x => x.ExportedTypes).Concat(IncludedType).FirstOrDefault(x => x.FullName.ToUpper() == Name.ToUpper());
            if (Dest == default(Type)) throw new AutoitException(AutoitExceptionType.CLASSDOESNOTEXIXTS, LineNumber, Cursor, Name);
            var Candidates = Dest.GetTypeInfo().DeclaredConstructors.Where(x => x.GetParameters().Length == Params.Count);
            if (Candidates.Count() == 0) throw new AutoitException(AutoitExceptionType.CONSTRUCTORMISMATCH, LineNumber, Cursor);
            var Selected = Candidates.FirstOrDefault(x => x.GetParameters().Select(y => y.ParameterType).SequenceEqual(Params));
            if (Selected == default(ConstructorInfo)) Selected = Candidates.First();
            return Expression.New(Selected, 
                Arguments.Zip(Selected.GetParameters().Select(z => z.ParameterType) , (x, y) => x.ConvertTo(y)));
        }
    }
}
