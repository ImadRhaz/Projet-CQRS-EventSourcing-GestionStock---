namespace GestionFM1.Read.Queries
{
    public class GetCommandeByIdQuery
    {
        public int Id { get; }  // L'ID est un int

        public GetCommandeByIdQuery(int id)
        {
            Id = id;
        }
    }
}