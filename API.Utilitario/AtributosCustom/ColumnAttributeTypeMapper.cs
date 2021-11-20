using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AppPromotora.Api.Utilitario
{
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper()
            : base(new SqlMapper.ITypeMap[]
                {
                new CustomPropertyTypeMap(
                   typeof(T),
                   (type, columnName) =>
                       type.GetProperties().FirstOrDefault(prop =>
                           prop.GetCustomAttributes(false)
                               .OfType<ColumnAttribute>()
                               .Any(attr => attr.Name.ToUpper() == columnName.ToUpper())
                           )
                   ),
                new DefaultTypeMap(typeof(T))
                })
        {
        }
    }
}
