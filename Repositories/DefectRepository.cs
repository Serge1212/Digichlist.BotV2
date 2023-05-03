namespace Digichlist.Bot.Repositories
{
    public class DefectRepository : IDefectRepository
    {
        readonly DigichlistContext _context;

        public DefectRepository(DigichlistContext context)
        {
            _context = context;
        }

        public IEnumerable<Defect> GetDefectsByChatId(long chatId)
        {
            return _context.Defects
                .Include(d => d.AssignedWorker)
                .Where(d => d.AssignedWorker.ChatId == chatId && d.ClosedAt == null);
        }

        public async Task SaveAsync(Defect defect)
        {
            await _context.Defects.AddAsync(defect);
            await _context.SaveChangesAsync();
        }
    }
}
