using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.ViewModels
{
    public partial class HomePageViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial int IntValue { get; set; }
        [ObservableProperty]
        public partial string StringValue { get; set; }


        public HomePageViewModel(IOptions<SampleSettings> options)
        {
            IntValue = options.Value.IntValue;
            StringValue = options.Value.StringValue;
        }
    }
}
