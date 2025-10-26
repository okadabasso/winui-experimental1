using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
namespace App1.ViewModels
{
    public partial class FormSamplePageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> SelectionItems { get; set; } = new ObservableCollection<string>();

        [ObservableProperty]
        public partial string SelectedItem {  get; set; }

        [ObservableProperty]
        public partial string Message { get; set; } = "Message";

        public FormSamplePageViewModel()
        {
            SelectionItems.Add("Item 1");
            SelectionItems.Add("Item 2");
            SelectionItems.Add("Item 3");

            SelectedItem = "";
        }
        public void SubmitForm()
        {
            // フォーム送信のロジックをここに追加
            // 例: 入力内容の検証、サーバーへの送信など

        }
    }
}
