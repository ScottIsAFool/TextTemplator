using System;
using System.ComponentModel;

namespace TextTemplator.Model
{
    public class Template : INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
