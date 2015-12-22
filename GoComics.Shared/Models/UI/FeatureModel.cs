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
        private string _iconUrl;
        private BitmapImage _icon;
        private bool _isReady = false;
        private bool? _isPoliticalSlant;

        public string IconUrl
        {
            get { return this._iconUrl; }
            set { base.SetProperty(ref this._iconUrl, value); }
        }

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

        [IgnoreDataMember]
        public BitmapImage Icon
        {
            get { return this._icon; }
            set { base.SetProperty(ref this._icon, value); }
        }

        [DataMember]
        public string IconFilePath { get; set; }

        [DataMember]
        public bool IsReady
        {
            get { return this._isReady; }
            set { base.SetProperty(ref this._isReady, value); }
        }

        [DataMember]
        public bool? IsPoliticalSlant
        {
            get { return this._isPoliticalSlant; }
            set { base.SetProperty(ref this._isPoliticalSlant, value); }
        }
    }
}