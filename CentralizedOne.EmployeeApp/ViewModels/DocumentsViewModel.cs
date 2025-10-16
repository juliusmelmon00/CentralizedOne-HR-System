using CentralizedOne.EmployeeApp.Models;
using CentralizedOne.EmployeeApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;

namespace CentralizedOne.EmployeeApp.ViewModels
{
    public class DocumentsViewModel : INotifyPropertyChanged
    {
        // ✅ Collection bound to UI
        private ObservableCollection<DocumentDto> _documents = new();
        public ObservableCollection<DocumentDto> Documents
        {
            get => _documents;
            set { _documents = value; OnPropertyChanged(); }
        }

        // ✅ UI State: Is Uploading
        private bool _isUploading;
        public bool IsUploading
        {
            get => _isUploading;
            set { _isUploading = value; OnPropertyChanged(); }
        }

        // ✅ Commands
        public ICommand UploadDocumentCommand { get; }

        public DocumentsViewModel()
        {
            UploadDocumentCommand = new RelayCommandAsync(UploadDocumentAsync);
            _ = LoadDocumentsAsync(); // Auto-load when ViewModel is constructed
        }

        // ✅ Fetch list of documents from server
        public async Task LoadDocumentsAsync()
        {
            var docs = await ApiClient.GetMyDocumentsAsync();
            if (docs != null)
            {
                Documents = new ObservableCollection<DocumentDto>(docs);
            }
        }

        private async Task UploadDocumentAsync()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select document to upload",
                Filter = "All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                // ✅ Step 1: Pick expiry date
                var expiryDialog = new CentralizedOne.EmployeeApp.Views.Documents.ExpiryDateDialog();

                if (expiryDialog.ShowDialog() == true) // User confirmed
                {
                    IsUploading = true;

                    var resultUrl = await ApiClient.UploadDocumentAsync(dlg.FileName, expiryDialog.SelectedExpiryDate);

                    IsUploading = false;

                    if (!string.IsNullOrEmpty(resultUrl))
                    {
                        await LoadDocumentsAsync(); // ✅ Refresh list
                    }
                }
            }
        }

        // ✅ PropertyChanged for Data Binding
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
