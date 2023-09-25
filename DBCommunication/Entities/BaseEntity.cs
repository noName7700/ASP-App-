using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCommunication.Entities
{
    internal class BaseEntity : IEntity
    {
        public Guid Id { get; set ; }
    }
}
