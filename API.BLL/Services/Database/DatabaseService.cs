using API.BLL.DTO;
using API.BLL.Exceptions;
using API.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace API.BLL.Services.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ApplicationContext _context;
        private readonly string _directoryPath;

        public DatabaseService(ApplicationContext context)
        {
            _context = context;
            _directoryPath = $"C:\\{_context.Database.GetDbConnection().Database}_Backups";

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public async Task<BackupDTO> CreateBackupAsync()
        {
            string dbname = _context.Database.GetDbConnection().Database;

            var backupName = $"{DateTime.UtcNow:yyyy-MM-dd_hh-mm-ss}";

            var path = Path.Combine(_directoryPath, backupName + ".bak");

            if (File.Exists(path))
            {
                throw new SuchFileExistException("File exists", backupName);
            }

            string sqlCommand = $"BACKUP DATABASE {dbname} TO DISK = '{_directoryPath}\\{backupName}.bak'";
            await _context.Database.ExecuteSqlRawAsync(sqlCommand);

            return new BackupDTO { BackupName = backupName };
        }

        public void DeleteBackup(string backupName)
        {
            if(backupName == null)
            {
                throw new ArgumentNullException(nameof(backupName));
            }

            var path = Path.Combine(_directoryPath, backupName + ".bak");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("No backup found");
            }

            File.Delete(path);
        }

        public IEnumerable<BackupDTO> GetBackups()
        {
            DirectoryInfo directory = new DirectoryInfo(_directoryPath);

            FileInfo[] files = directory.GetFiles("*.bak");

            return files.Select(x => new BackupDTO { BackupName = x.Name[..(x.Name.Length - 4)] });
        }

        public async Task RestoreAsync(string backupName)
        {
            if (backupName == null)
            {
                throw new ArgumentNullException(nameof(backupName));
            }

            var path = Path.Combine(_directoryPath, backupName + ".bak");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("No backup found");
            }

            string dbname = _context.Database.GetDbConnection().Database;
            string sqlCommand = $"ALTER DATABASE {dbname} SET Single_User WITH Rollback Immediate; " +
                $"USE master RESTORE DATABASE {dbname} FROM DISK = '{path}';" +
                $"ALTER DATABASE {dbname} SET Multi_User";

            await _context.Database.ExecuteSqlRawAsync(sqlCommand);
        }
    }
}
