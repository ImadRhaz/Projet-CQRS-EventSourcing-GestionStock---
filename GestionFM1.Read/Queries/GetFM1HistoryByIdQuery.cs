// GestionFM1.Read/Queries/GetFM1HistoryByIdQuery.cs
using System;

namespace GestionFM1.Read.Queries
{
    public class GetFM1HistoryByIdQuery
    {
        public Guid Id { get; }

        public GetFM1HistoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}