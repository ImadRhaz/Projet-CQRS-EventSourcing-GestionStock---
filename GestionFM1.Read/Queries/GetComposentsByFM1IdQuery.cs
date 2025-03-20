// GetComposentsByFM1IdQuery.cs
using System;

namespace GestionFM1.Read.Queries
{
    public class GetComposentsByFM1IdQuery
    {
        public Guid FM1Id { get; }

        public GetComposentsByFM1IdQuery(Guid fm1Id)
        {
            FM1Id = fm1Id;
        }
    }
}