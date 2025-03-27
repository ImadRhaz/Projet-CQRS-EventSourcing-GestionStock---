using System;

namespace GestionFM1.Read.Queries
{
    public class GetFM1ByIdQuery
    {
        public Guid Id { get; }

        public GetFM1ByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}