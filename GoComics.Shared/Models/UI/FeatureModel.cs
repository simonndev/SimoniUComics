using Prism.Mvvm;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace GoComics.Shared.Models.UI
{
    [DataContract]
    public class FeatureModel : BindableBase
    {
        private int _id;
        private string _title;
        private string _author;
        private BitmapImage _icon;
        private bool _isReady = false;
        private bool? _isPoliticalSlant;

        [DataMember]
        public string IconUrl { get; set; }

        [DataMember]
        public string IconFilePath { get; set; }

        [DataMember]
        public int Id
        {
            get { return this._id; }
            set { base.SetProperty(ref this._id, value); }
        }

        [DataMember]
        public string Title
        {
            get { return this._title; }
            set { base.SetProperty(ref this._title, value); }
        }

        [DataMember]
        public string Author
        {
            get { return this._author; }
            set { base.SetProperty(ref this._author, value); }
        }

        [DataMember]
        public bool? IsPoliticalSlant
        {
            get { return this._isPoliticalSlant; }
            set { base.SetProperty(ref this._isPoliticalSlant, value); }
        }

        [IgnoreDataMember]
        public BitmapImage Icon
        {
            get { return this._icon; }
            set { base.SetProperty(ref this._icon, value); }
        }

        [IgnoreDataMember]
        public bool IsReady
        {
            get { return this._isReady; }
            set { base.SetProperty(ref this._isReady, value); }
        }

        public override string ToString()
        {
            return string.Format("Feature {0} - {1}", this.Id, this.Title);
        }
    }
}