using System;

namespace GestionFM1.Read.Queries
{
    public class GetComposentByIdQuery
    {
        public Guid Id { get; }

        public GetComposentByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}