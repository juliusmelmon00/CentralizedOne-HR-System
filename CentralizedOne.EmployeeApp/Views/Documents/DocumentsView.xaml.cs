using CentralizedOne.EmployeeApp.Models;
using CentralizedOne.EmployeeApp.Services;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using CentralizedOne.EmployeeApp.Helpers;
using CentralizedOne.EmployeeApp.Views.Documents;

namespace CentralizedOne.EmployeeApp.Views.Documents
{
    /// <summary>
    /// Interaction logic for DocumentsView.xaml
    /// This view handles displaying employee documents and triggering the upload process.
    /// </summary>
    public partial class DocumentsView : UserControl
    {
        public DocumentsView()
        {
            InitializeComponent();

            // ✅ Attach ViewModel to enable MVVM binding
            DataContext = new CentralizedOne.EmployeeApp.ViewModels.DocumentsViewModel();

            // ✅ Load initial document list when view is initialized
            LoadDocuments();
        }

        /// <summary>
        /// Triggered when user clicks the Upload button (BtnUpload).
        /// Opens File Dialog → Prompts Expiry Date → Uploads via ApiClient → Shows Toast Feedback.
        /// </summary>
        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            // ✅ Allow user to select a file from disk
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF Files|*.pdf|Image Files|*.png;*.jpg;*.jpeg|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName; // ✅ Get selected file path

                // ✅ Open expiry date picker popup before upload
                var expiryDialog = new ExpiryDateDialog();
                if (expiryDialog.ShowDialog() == true)
                {
                    DateTime expiryDate = expiryDialog.SelectedExpiryDate;

                    // ✅ Call API uploader (returns URL if successful)
                    var resultUrl = await ApiClient.UploadDocumentAsync(filePath, expiryDate);

                    if (!string.IsNullOrEmpty(resultUrl))
                    {
                        // 🎉 Success — show global toast notification (no blocking MessageBox)
                        ToastService.ShowSuccess("📤 Document uploaded and pending review.");

                        // ✅ Refresh document list to reflect new upload
                        LoadDocuments();
                    }
                    else
                    {
                        // ❌ Failed — show toast error feedback
                        ToastService.ShowError("⚠️ Upload failed. Please try again.");
                    }
                }
            }
        }

        /// <summary>
        /// Fetches employee documents from API and displays them in the ListView.
        /// Also updates the status label for UX feedback.
        /// </summary>
        private async void LoadDocuments()
        {
            LblStatus.Visibility = Visibility.Visible;
            LblStatus.Text = "Loading documents..."; // ✅ Show loading text

            List<DocumentDto>? docs = await ApiClient.GetMyDocumentsAsync();

            if (docs == null || docs.Count == 0)
            {
                // ⚠ Show if no data was returned
                LblStatus.Text = "No documents found.";
                return;
            }

            // ✅ Hide status label and bind data to list
            LblStatus.Visibility = Visibility.Collapsed;
            ListDocuments.ItemsSource = docs;
        }
    }
}
