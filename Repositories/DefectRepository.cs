namespace Digichlist.Bot.Repositories
{
    public class DefectRepository : IDefectRepository
    {
        readonly DigichlistContext _context;

        public DefectRepository(DigichlistContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Defect defect)
        {
            await _context.Defects.AddAsync(defect);
            await _context.SaveChangesAsync();
        }
    }
}
