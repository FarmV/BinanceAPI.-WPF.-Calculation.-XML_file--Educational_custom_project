using System.IO;
using System.Windows;

namespace AppPars
{
    public class SaveOneXmlFile : IDisposable
    {
        private static readonly ReaderWriterLockSlim _dataOperationLockSlim = new ReaderWriterLockSlim();
        private string _fileName;
        private bool isDisposed = false;
        private DataObject? _saveData;
        public SaveOneXmlFile(string fileName)
        {
            _fileName = fileName;
        }
        public void Dispose()
        {
            try { _dataOperationLockSlim?.Dispose(); }
            catch { }
            isDisposed = true;
            GC.SuppressFinalize(this);
        }
        public DataObject? SaveData
        {
            get => _saveData;
            set
            {
                if (_dataOperationLockSlim.IsWriteLockHeld is true)
                {
                    while (SpinWait.SpinUntil(() => _dataOperationLockSlim.IsWriteLockHeld is true, TimeSpan.FromSeconds(3)) is false) { }
                    if (_dataOperationLockSlim.IsWriteLockHeld is true) throw new InvalidOperationException();
                }
                _saveData = value;
            }
        }
        ~SaveOneXmlFile()
        {
            if (isDisposed is true) return;
            Dispose();
            isDisposed = true;
        }
        public DataObject? DataRead()
        {
            throw new NotImplementedException();
            if (isDisposed is true) throw new ObjectDisposedException($@"{nameof(ReaderWriterLockSlim)} is disposed");
            if (SaveData is null) return null;
            _dataOperationLockSlim.EnterReadLock();
            try
            {
                //File.Open()
            }
            finally
            {
                _dataOperationLockSlim.ExitReadLock();
            }
        }
        public void DataWrite()
        {
            if (isDisposed is true) throw new ObjectDisposedException($@"{nameof(ReaderWriterLockSlim)} is disposed");
            if (SaveData is null) throw new InvalidOperationException("Attempting to write empty data. SaveData is null");
            _dataOperationLockSlim.EnterWriteLock();
            try
            {

            }
            finally
            {
                _dataOperationLockSlim.ExitWriteLock();
            }

        }

        public bool СheckFile() => System.IO.File.Exists($"{Program.RelativePathSaveData}{_fileName}");
        public void CreateCreateEmptyFile()
        {
            System.IO.File.Create($"{Program.RelativePathSaveData}{_fileName}").Dispose(); ;
        }
        public void DeleteFile()
        {
            if (СheckFile() is false) throw new InvalidOperationException("Нельзя удалить несуществующий файл");
            File.Delete($"{Program.RelativePathSaveData}{_fileName}");
        }


    }



}