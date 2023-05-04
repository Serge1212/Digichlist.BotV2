namespace Digichlist.Bot.Repositories
{
    public class DefectRepository : IDefectRepository
    {
        readonly DigichlistContext _context;

        public DefectRepository(DigichlistContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<Defect> GetSingleAsync(int defectId) =>
            await _context.Defects
                .Include(d => d.AssignedWorker)
                .FirstOrDefaultAsync(d => d.Id == defectId);

        /// <inheritdoc />
        public IEnumerable<Defect> GetManyByChatId(long chatId) =>
            _context.Defects
                .Include(d => d.AssignedWorker)
                .Where(d => d.AssignedWorker.ChatId == chatId && d.ClosedAt == null);

        /// <inheritdoc />
        public async Task SaveAsync(Defect defect)
        {
            await _context.Defects.AddAsync(defect);
            await _context.SaveChangesAsync();
        }
    }
}
